using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DonationEvents;

namespace DonationEvents.Controllers
{
    public class DonationRangesController : Controller
    {
        private EventsMasterEntities db = new EventsMasterEntities();

        // GET: DonationRanges
        public ActionResult Index()
        {
            var donationRanges = db.DonationRanges.Include(d => d.Event);
            return View(donationRanges.ToList());
        }

        // GET: DonationRanges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonationRange donationRange = db.DonationRanges.Find(id);
            if (donationRange == null)
            {
                return HttpNotFound();
            }
            return View(donationRange);
        }

        // GET: DonationRanges/Create
        public ActionResult Create()
        {
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName");
            return View();
        }

        // POST: DonationRanges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventID,DonationAmount")] DonationRange donationRange)
        {
            if (ModelState.IsValid)
            {
                db.DonationRanges.Add(donationRange);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", donationRange.EventID);
            return View(donationRange);
        }

        // GET: DonationRanges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonationRange donationRange = db.DonationRanges.Find(id);
            if (donationRange == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", donationRange.EventID);
            return View(donationRange);
        }

        // POST: DonationRanges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventID,DonationAmount")] DonationRange donationRange)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donationRange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", donationRange.EventID);
            return View(donationRange);
        }

        // GET: DonationRanges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonationRange donationRange = db.DonationRanges.Find(id);
            if (donationRange == null)
            {
                return HttpNotFound();
            }
            return View(donationRange);
        }

        // POST: DonationRanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonationRange donationRange = db.DonationRanges.Find(id);
            db.DonationRanges.Remove(donationRange);
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
