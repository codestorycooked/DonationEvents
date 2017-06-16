using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DonationEvents;
using PayPal.Api;
using Microsoft.AspNet.Identity;
using DonationEvents.Models;

namespace DonationEvents.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private EventsMasterEntities db = new EventsMasterEntities();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Where(a => a.IsActive == true).OrderByDescending(a=>a.DateAdded).ToList();
            if (events.Count == 1)
            {
                return RedirectToAction("Details", "Events", new { id = events[0].Id });
            }
            return View(events);
        }

        public ActionResult Donate(int id)
        {

            int donationID = Convert.ToInt32(id);
            var userid1 = Guid.Parse(User.Identity.GetUserId());
            var donation1 = db.UserDonations.Where(m => m.EventID == donationID && m.UserID == userid1 && m.status.ToLower() == "payment successful").FirstOrDefault();
            if (donation1 != null)
            {
                return RedirectToAction("AlreadyDonated");
            }
            //Get Donation Ranges
            GetDonationRange(id);

            EventDonationViewModel object1 = new EventDonationViewModel();
            var events = db.Events.Find(id);
            var range = db.DonationRanges.Where(e => e.Id == events.Id).ToList();
            object1.events = events;
            object1.ranges = range;
            return View(object1);
        }

        private void GetDonationRange(int id)
        {
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
            donationValue = donationValue + "," + "Other Amount";
            ViewBag.donationValue = donationValue;
        }

        public ActionResult AlreadyDonated()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Donate(EventDonationViewModel model, string eventID)
        {
            var events = db.Events.Find(Convert.ToInt32(eventID));
            if (model.DonationValue == 0)
            {
                EventDonationViewModel object1 = new EventDonationViewModel();
                
                var range = db.DonationRanges.Where(e => e.Id == events.Id).ToList();
                object1.events = events;
                object1.ranges = range;
                ViewBag.Error = "Amount Cannot be 0";
                //Get Donation Ranges
                GetDonationRange(Convert.ToInt32(eventID));
                return View(object1);
            }
           

            var returnURL = Url.Action("Thankyou", "Events", new { donationid = events.Id, userid = User.Identity.GetUserId() }, "http");
            var cancelURL = Url.Action("Cancelled", "Events", new { donationid = events.Id, userid = User.Identity.GetUserId() }, "http");
            Payment payment = Pyament(returnURL, cancelURL, User.Identity.GetUserId(), model.DonationValue.ToString());
            UserDonation donation = new UserDonation { EventID = events.Id, UserID = Guid.Parse(User.Identity.GetUserId()), status = "Payment Started" };
            db.UserDonations.Add(donation);
            db.SaveChanges();
            return Redirect(payment.GetApprovalUrl());



        }
        public ActionResult ViewEvent(string eventID)
        {
            if (eventID != null)
            {
                //redirect
            }
            var events = db.Events.Find(Convert.ToInt32(eventID));

            return View(events);
        }

        private static Payment Pyament(string returnURL, string cancelURL, string userid, string amount)
        {

            // Authenticate with PayPal

            var accessToken = new OAuthTokenCredential("AYjSIaE86VBAd_Uya-iNoxIkp7r6iC16oii3bdxx4nDmvborenF22kETl2KzS1g_KI4XHLSXYW0pkUf5", "EIdm5xJMIeA5IfT_scj96q9LiVfm9e5x15VVBLeL6hqL-y4GKeEtXQz2hzUzXiBYxZ-xLPRdBgIhXhj7", null).GetAccessToken();

            var apiContext = new APIContext(accessToken);

            // Make an API call
            var payment = Payment.Create(apiContext, new Payment
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = new List<Transaction>
    {
        new Transaction
        {
            description = "Transaction description.",
            invoice_number = "001",
            amount = new Amount
            {
                currency = "USD",
                total = amount,
                details = new Details
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = amount
                }
            },
            item_list = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = "Donation Amount",
                        currency = "USD",
                        price = amount,
                        quantity = "1",
                        sku = "sku"
                    }
                }
            }
        }
    },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnURL,
                    cancel_url = cancelURL
                }
            });
            return payment;
        }

        public ActionResult Cancelled(string donationid, string userid, string token)
        {
            int donationID = Convert.ToInt32(donationid);
            var userid1 = Guid.Parse(userid);
            var donation = db.UserDonations.Where(m => m.EventID == donationID && m.UserID == userid1).FirstOrDefault();
            donation.PayPalResponse = token;
            donation.status = "Payment Cancelled";
            db.Entry(donation).State = EntityState.Modified;
            db.SaveChanges();

            return View();
        }


        public ActionResult Failure()
        {
            return View();
        }
        public ActionResult Thankyou(string donationid, string userid, string paymentId)

        {
            int donationID = Convert.ToInt32(donationid);
            var userid1 = Guid.Parse(userid);
            var donation = db.UserDonations.Where(m => m.EventID == donationID && m.UserID == userid1).FirstOrDefault();
            donation.TransactionID = paymentId;
            donation.status = "Payment successful";
            db.Entry(donation).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.donation = donationid;
            ViewBag.payment = paymentId;
            return View();
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userid1 = Guid.Parse(User.Identity.GetUserId());

            //var isTransacted = db.UserDonations.FirstOrDefault(a => a.EventID == id && a.UserID == userid1);
            //if (isTransacted == null)
            //{
            //    var isPresent = db.ViewedEvents.FirstOrDefault(a => a.userID == userid1 && a.EventID == id);
            //    if (isPresent != null)
            //    {
            //        return RedirectToAction("Donate", "Events", new { id = id });
            //    }
            //}

            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
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

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateViewCount(int? id)
        {
            var userid1 = Guid.Parse(User.Identity.GetUserId());
            var isPresent = db.ViewedEvents.FirstOrDefault(a => a.userID == userid1 && a.EventID == id);
            if (isPresent != null)
            {
                return RedirectToAction("Donate", "Events", new { id = id });
            }
            else
            {

                var viewed = new ViewedEvent { EventID = id, Viewed = true, userID = userid1 };
                db.ViewedEvents.Add(viewed);
                db.SaveChanges();
                return RedirectToAction("Donate", "Events", new { id = id });
            }


        }
        public ActionResult PreviewVideo(string id, string eventid)

        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Index");
            }
            ViewBag.ID = id;
            ViewBag.eventID = eventid;
            return View();
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
