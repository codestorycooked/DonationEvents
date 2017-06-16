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
    public class EventManagerController : Controller
    {
        private EventsMasterEntities db = new EventsMasterEntities();

        // GET: EventManager
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: EventManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: EventManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventDescription,DateAdded,PixelLotID,Duration,IsActive")] Event @event, string donationValue)
        {

            @event.DateAdded = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);

                if (!string.IsNullOrEmpty(donationValue))
                {
                    string[] donationValues = donationValue.Split(',');
                    foreach (var item in donationValues)
                    {
                        var donationRange = new DonationRange { DonationAmount = Convert.ToDecimal(item), EventID = @event.Id };
                        db.DonationRanges.Add(donationRange);

                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            return View(@event);
        }

        // GET: EventManager/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);

            var donationValues = db.DonationRanges.Where(a => a.EventID == id).ToList();
            string donationValue = string.Empty;
            int counter = 0;
            
            foreach (var item in donationValues)
            {
                if (counter == 0)
                {
                    donationValue = ((decimal)(item.DonationAmount)).ToString("#.##");

                }
                else
                    donationValue = donationValue + "," + ((decimal)(item.DonationAmount)).ToString("#.##");
                counter++;
            }
            ViewBag.donationValue = donationValue;
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: EventManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,EventDescription,DateAdded,PixelLotID,Duration,IsActive")] Event @event, string donationValue)
        {
            if (ModelState.IsValid)
            {
                @event.DateAdded = DateTime.Now;
                db.Entry(@event).State = EntityState.Modified;
                //Delete old values

                if (!string.IsNullOrEmpty(donationValue))
                {
                    var oldRanges = db.DonationRanges.Where(a => a.EventID == @event.Id);
                    db.DonationRanges.RemoveRange(oldRanges);
                    db.SaveChanges();
                    string[] donationValues = donationValue.Split(',');
                    foreach (var item in donationValues)
                    {
                        var donationRange = new DonationRange { DonationAmount = Convert.ToDecimal(item), EventID = @event.Id };
                        db.DonationRanges.Add(donationRange);

                    }
                }
                else
                {
                    var oldRanges = db.DonationRanges.Where(a => a.EventID == @event.Id);
                    db.DonationRanges.RemoveRange(oldRanges);
                    db.SaveChanges();
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: EventManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: EventManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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
