using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoOrder.Models;

namespace AutoOrder.Controllers
{
    public class AutoparksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Autoparks
        public ActionResult Index()
        {
            var autoparks = db.Autoparks.Include(a => a.TrailerType);
            return View(autoparks.ToList());
        }

        // GET: Autoparks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autopark autopark = db.Autoparks.Find(id);
            if (autopark == null)
            {
                return HttpNotFound();
            }
            return View(autopark);
        }

        // GET: Autoparks/Create
        public ActionResult Create()
        {
            ViewBag.TrailerTypeId = new SelectList(db.TrailerTypes, "Id", "Name");
            return View();
        }

        // POST: Autoparks/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Trailer,TrailerCount,TrailerTypeId,TrailerLength,TrailerWidth,TrailerHeight")] Autopark autopark)
        {
            if (ModelState.IsValid)
            {
                db.Autoparks.Add(autopark);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TrailerTypeId = new SelectList(db.TrailerTypes, "Id", "Name", autopark.TrailerTypeId);
            return View(autopark);
        }

        // GET: Autoparks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autopark autopark = db.Autoparks.Find(id);
            if (autopark == null)
            {
                return HttpNotFound();
            }
            ViewBag.TrailerTypeId = new SelectList(db.TrailerTypes, "Id", "Name", autopark.TrailerTypeId);
            return View(autopark);
        }

        // POST: Autoparks/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Trailer,TrailerCount,TrailerTypeId,TrailerLength,TrailerWidth,TrailerHeight")] Autopark autopark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autopark).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TrailerTypeId = new SelectList(db.TrailerTypes, "Id", "Name", autopark.TrailerTypeId);
            return View(autopark);
        }

        // GET: Autoparks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autopark autopark = db.Autoparks.Find(id);
            if (autopark == null)
            {
                return HttpNotFound();
            }
            return View(autopark);
        }

        // POST: Autoparks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Autopark autopark = db.Autoparks.Find(id);
            db.Autoparks.Remove(autopark);
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
