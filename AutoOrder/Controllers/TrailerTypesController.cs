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
    public class TrailerTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TrailerTypes
        public ActionResult Index()
        {
            return View(db.TrailerTypes.ToList());
        }

        // GET: TrailerTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrailerType trailerType = db.TrailerTypes.Find(id);
            if (trailerType == null)
            {
                return HttpNotFound();
            }
            return View(trailerType);
        }

        // GET: TrailerTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrailerTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TrailerType trailerType)
        {
            if (ModelState.IsValid)
            {
                db.TrailerTypes.Add(trailerType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trailerType);
        }

        // GET: TrailerTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrailerType trailerType = db.TrailerTypes.Find(id);
            if (trailerType == null)
            {
                return HttpNotFound();
            }
            return View(trailerType);
        }

        // POST: TrailerTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TrailerType trailerType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trailerType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trailerType);
        }

        // GET: TrailerTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrailerType trailerType = db.TrailerTypes.Find(id);
            if (trailerType == null)
            {
                return HttpNotFound();
            }
            return View(trailerType);
        }

        // POST: TrailerTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrailerType trailerType = db.TrailerTypes.Find(id);
            db.TrailerTypes.Remove(trailerType);
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
