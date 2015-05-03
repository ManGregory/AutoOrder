using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoOrder.Models;
using Microsoft.AspNet.Identity;

namespace AutoOrder.Controllers
{
    [Authorize(Roles = "admin, client")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private string CurrentUserId
        {
            get { return User.Identity.GetUserId(); }
        }

        private bool IsAdmin
        {
            get { return User.IsInRole("admin"); }
        }

        // GET: Orders
        public ActionResult Index()
        {            
            var orders = db.Orders
                .Include(o => o.TransportType)
                .Include(o => o.User)
                .Include(o => o.Autopark);
            if (!IsAdmin)
            {
                orders = orders.Where(o => o.UserId == CurrentUserId);
            }
            return View(orders.OrderBy(o => o.FactOutDate.HasValue).ThenByDescending(o => o.FactOutDate).ToList());
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

        private void SetViewBag(int? autoParkId = null)
        {
            ViewBag.TransportTypeId = new SelectList(db.TrailerTypes, "Id", "Name");
            var availableAutopark = GetAvailableAutopark(autoParkId);
            ViewBag.AutoparkId = new SelectList(availableAutopark, "Id", "Name", autoParkId);
        }

        private IEnumerable<Autopark> GetAvailableAutopark(int? currentAutoparkId = null)
        {
            return
                db.Autoparks.Where(
                    a =>
                        a.Id == currentAutoparkId ||
                        !db.Orders.Any(o => (o.AutoparkId == a.Id) && (o.FactOutDate == null) && !o.IsCancelled));
        }

        // POST: Orders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Phone,TransportedObjectType,TransportTypeId,ObjectLength,ObjectWidth,ObjectHeight,ProspectiveInDate,ProspectiveOutDate,AddressFrom,AddressTo,FactInDate,FactOutDate,AutoparkId")] Order order)
        {
            CheckDates(ModelState, order);
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
        }

        private void SetAutopark(Order order)
        {
            var availableAutoPark = GetAvailableAutopark();
            if (availableAutoPark != null)
            {
                var firstOrDefault = availableAutoPark
                    .FirstOrDefault(a => a.Capacity >= order.Capacity && a.TrailerTypeId == order.TransportTypeId);
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
            SetViewBag(order.AutoparkId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Phone,TransportedObjectType,TransportTypeId,ObjectLength,ObjectWidth,ObjectHeight,ProspectiveInDate,ProspectiveOutDate,AddressFrom,AddressTo,FactInDate,FactOutDate,AutoparkId")] Order order)
        {
            CheckDates(ModelState, order);
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetViewBag(order.AutoparkId);
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
