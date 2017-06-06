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

namespace DonationEvents.Controllers
{
  public class EventsController : Controller
  {
    private EventsMasterEntities db = new EventsMasterEntities();

    // GET: Events
    public ActionResult Index()
    {
      return View(db.Events.ToList());
    }

    public ActionResult Donate(int id)
    {

      return View(db.Events.Where(m => m.Id == id).Include(e => e.DonationRanges));
    }

    [HttpPost]
    public ActionResult Donate()
    {
      // Authenticate with PayPal
      var config = ConfigManager.Instance.GetProperties();
      var accessToken = new OAuthTokenCredential(config).GetAccessToken();
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
                total = "1.00"
              
            },
            item_list = new ItemList
            {
                items = new List<Item>
                {
                    new Item
                    {
                        name = "Item Name",
                        currency = "USD",
                        price = "15",
                        quantity = "5",
                        sku = "sku"
                    }
                }
            }
        }
    },
        redirect_urls = new RedirectUrls
        {
          return_url = "http://mysite.com/return",
          cancel_url = "http://mysite.com/cancel"
        }
      });
      return View();
    }

    // GET: Events/Details/5
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
    public ActionResult Create([Bind(Include = "Id,EventName,EventDescription,DonatedValue,DateAdded")] Event @event)
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
    public ActionResult Edit([Bind(Include = "Id,EventName,EventDescription,DonatedValue,DateAdded")] Event @event)
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
