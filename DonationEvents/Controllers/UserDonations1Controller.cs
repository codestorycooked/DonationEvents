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
    public class UserDonations1Controller : Controller
    {
        private EventsMasterEntities db = new EventsMasterEntities();

        // GET: UserDonations1
        public ActionResult Index()
        {
            var userDonations = db.UserDonations.Include(u => u.Event);
            return View(userDonations.ToList());
        }

        // GET: UserDonations1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDonation userDonation = db.UserDonations.Find(id);
            if (userDonation == null)
            {
                return HttpNotFound();
            }
            return View(userDonation);
        }

        // GET: UserDonations1/Create
        public ActionResult Create()
        {
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName");
            return View();
        }

        // POST: UserDonations1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserID,EventID,PayPalResponse,TransactionID,status")] UserDonation userDonation)
        {
            if (ModelState.IsValid)
            {
                db.UserDonations.Add(userDonation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", userDonation.EventID);
            return View(userDonation);
        }

        // GET: UserDonations1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDonation userDonation = db.UserDonations.Find(id);
            if (userDonation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", userDonation.EventID);
            return View(userDonation);
        }

        // POST: UserDonations1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserID,EventID,PayPalResponse,TransactionID,status")] UserDonation userDonation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userDonation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventID = new SelectList(db.Events, "Id", "EventName", userDonation.EventID);
            return View(userDonation);
        }

        // GET: UserDonations1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDonation userDonation = db.UserDonations.Find(id);
            if (userDonation == null)
            {
                return HttpNotFound();
            }
            return View(userDonation);
        }

        // POST: UserDonations1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserDonation userDonation = db.UserDonations.Find(id);
            db.UserDonations.Remove(userDonation);
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
