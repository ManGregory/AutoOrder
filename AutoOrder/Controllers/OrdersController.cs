using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoOrder.Models;
using Microsoft.AspNet.Identity;

namespace AutoOrder.Controllers
{
    [Authorize(Roles = "admin, client")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private string CurrentUserId
        {
            get { return User.Identity.GetUserId(); }
        }

        private bool IsAdmin
        {
            get { return User.IsInRole("admin"); }
        }

        // GET: Orders
        public ActionResult Index(string sortOrder, string filter, string sortType)
        {
            SetSortOrderParams(sortOrder);
            IEnumerable<Order> orders = db.Orders
                .Include(o => o.TransportType)
                .Include(o => o.User)
                .Include(o => o.Autopark)
                .AsEnumerable();
            ViewBag.CurrentFilter = filter ?? "lastDecade";
            ViewBag.CurrentSortType = sortType ?? "asc";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                if (filter == "lastDecade") orders = orders.Where(o => o.IsInLastDecade);
            }
            else
            {
                orders = orders.Where(o => o.IsInLastDecade);
            }
            if (!IsAdmin)
            {
                orders = orders.Where(o => o.UserId == CurrentUserId);
            }
            switch (sortOrder)
            {
                case "dateProspective":
                    orders = sortType == "desc"
                        ? orders.OrderByDescending(o => o.ProspectiveInDate)
                        : orders.OrderBy(o => o.ProspectiveInDate);
                    break;
                case "dateFact":
                    orders = sortType == "desc"
                        ? orders.OrderByDescending(o => o.FactOutDate)
                        : orders.OrderBy(o => o.FactOutDate);
                    break;
                case "userName":
                    orders = sortType == "desc"
                        ? orders.OrderByDescending(o => o.User.UserName)
                        : orders.OrderBy(o => o.User.UserName);
                    break;
                default:
                    orders = orders.OrderByDescending(o => o.ProspectiveInDate);
                    break;
            }
            return View(orders.ToList());
        }

        private void SetSortOrderParams(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateProspectiveSort = string.IsNullOrEmpty(sortOrder) ? "dateProspective" : "";
            ViewBag.DateFactSort = sortOrder == "dateFact" ? "dateFactDesc" : "dateFact";
            ViewBag.UserNameSort = sortOrder == "userName" ? "userNameDesc" : "userName";
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        private void SetViewBag(int? autoParkId = null, Order order = null)
        {
            ViewBag.TransportTypeId = order == null
                ? new SelectList(db.TrailerTypes, "Id", "Name")
                : new SelectList(db.TrailerTypes, "Id", "Name", order.TransportTypeId);
            IEnumerable<Autopark> availableAutopark = GetAvailableAutopark(autoParkId, order);
            ViewBag.AutoparkId = new SelectList(availableAutopark, "Id", "Name", autoParkId);
        }

        private IEnumerable<Autopark> GetAvailableAutopark(int? currentAutoparkId = null, Order currentOrder = null)
        {
            List<Order> orders = db.Orders.AsEnumerable().ToList();
            List<Autopark> autoparks = db.Autoparks.AsEnumerable().ToList();
            IEnumerable<Autopark> result =
                autoparks.Where(
                    a =>
                        (a.Id == currentAutoparkId ||
                         !orders.Any(
                             o =>
                                 (o.AutoparkId == a.Id) && (o.FactOutDate == null) && !o.IsCancelled)));
            if (currentOrder != null)
            {
                result =
                    result.Where(
                        a => a.CanCarry(currentOrder.ObjectLength, currentOrder.ObjectWidth, currentOrder.ObjectHeight));
            }
            return result;
        }

        // POST: Orders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Id,UserId,Phone,TransportedObjectType,TransportTypeId,ObjectLength,ObjectWidth,ObjectHeight,ProspectiveInDate,ProspectiveOutDate,AddressFrom,AddressTo,FactInDate,FactOutDate,AutoparkId"
                )] Order order)
        {
            Check(order);
            if (ModelState.IsValid)
            {
                order.UserId = CurrentUserId;
                SetAutopark(order);
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetViewBag();
            return View(order);
        }

        private void Check(Order order)
        {
            CheckDates(ModelState, order);
            CheckLogic(ModelState, order);
        }

        private void CheckDates(ModelStateDictionary modelState, Order order)
        {
            if (order.FactInDate > order.FactOutDate)
            {
                modelState.AddModelError("", "Фактическая дата погрузки должна быть меньше фактической даты разгрузки");
            }
            if (order.ProspectiveInDate > order.ProspectiveOutDate)
            {
                modelState.AddModelError("", "Перспективная дата погрузки должна быть меньше фактической даты погрузки");
            }
            if (order.ProspectiveInDate < DateTime.Now || order.ProspectiveOutDate < DateTime.Now ||
                order.FactInDate < DateTime.Now || order.FactOutDate < DateTime.Now)
            {
                modelState.AddModelError("", "Все даты должны быть больше либо равны текущей");
            }
        }

        private void CheckLogic(ModelStateDictionary modelState, Order order)
        {
            if ((order.FactInDate != null || order.FactOutDate != null) && (order.AutoparkId == null))
            {
                modelState.AddModelError("", "Перед занесением фактических дат необходимо назначить автомобиль");
            }
        }

        private void SetAutopark(Order order)
        {
            IEnumerable<Autopark> availableAutoPark = GetAvailableAutopark();
            if (availableAutoPark != null)
            {
                Autopark firstOrDefault = availableAutoPark
                    .AsEnumerable()
                    .FirstOrDefault(
                        a =>
                            a.CanCarry(order.ObjectLength, order.ObjectWidth, order.ObjectHeight) &&
                            a.TrailerTypeId == order.TransportTypeId);
                if (firstOrDefault != null)
                {
                    order.AutoparkId = firstOrDefault.Id;
                }
                else
                {
                    order.Comment =
                        "Нет свободных машин, которые могли бы выполнить вашу заявку. Обратитесь к администратору: admin@autoorders.com";
                }
            }
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            SetViewBag(order.AutoparkId, order);
            return View(order);
        }

        // POST: Orders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                Include =
                    "Id,UserId,Phone,TransportedObjectType,TransportTypeId,ObjectLength,ObjectWidth,ObjectHeight,ProspectiveInDate,ProspectiveOutDate,AddressFrom,AddressTo,FactInDate,FactOutDate,AutoparkId,Comment"
                )] Order order)
        {
            Check(order);
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetViewBag(order.AutoparkId, order);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            order.IsCancelled = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}