using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GenieBank.Models;

namespace GenieBank.Controllers
{
    public class WishesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Wishes
		public ActionResult Index(string searchString, decimal? searchCost, int? bankAccountId)
		{
            var wish = db.Wish.Include(w => w.BankAccount).Where(x => x.BankAccountId == bankAccountId);
            if (!String.IsNullOrEmpty(searchString))
			{
				wish = wish.Where(w => w.Description.ToUpper().Contains(searchString.ToUpper()));
			}
			if (searchCost.HasValue)
			{
				wish = wish.Where(w => w.Cost == searchCost);
			}
            ViewBag.BankAccountId = bankAccountId;
            var results = wish.ToList();
			return View(results);
		}

		// GET: Wishes/Details/5
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wish wish = db.Wish.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(wish);
        }

		// GET: Wishes/Create
		public ActionResult Create(int BankAccountId, string recipient)
		{
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail");
			if (BankAccountId <= 0)
			{
				return View();
			}
			if (string.IsNullOrWhiteSpace(recipient))
			{
				return View();
			}
            BankAccount bankAccount = db.BankAccounts.Find(BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var wish = new Wish { BankAccountId = BankAccountId, Account = recipient, WishDate = DateTime.Now };
			return View(wish);
		}

        // POST: Wishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Account,Cost,Description,WebsiteLink,Purchased,WishDate,BankAccountId")] Wish wish)
        {
            if (ModelState.IsValid)
            {
                BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
                if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                db.Wish.Add(wish);
                db.SaveChanges();
                return RedirectToAction("Details", "BankAccounts", new { Id = wish.BankAccountId });
			}

            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wish.BankAccountId);
            return View(wish);
        }

        // GET: Wishes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wish wish = db.Wish.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wish.BankAccountId);
            return View(wish);
        }

        // POST: Wishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,Cost,Description,WebsiteLink,Purchased,WishDate,BankAccountId")] Wish wish)
        {
            if (ModelState.IsValid)
            {
                BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
                if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                db.Entry(wish).State = EntityState.Modified;
                db.SaveChanges();
	            return RedirectToAction("Details", "BankAccounts", new { Id = wish.BankAccountId });
			}
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", wish.BankAccountId);
            return View(wish);
        }

        // GET: Wishes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wish wish = db.Wish.Find(id);
            if (wish == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(wish);
        }

        // POST: Wishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wish wish = db.Wish.Find(id);
            BankAccount bankAccount = db.BankAccounts.Find(wish.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db.Wish.Remove(wish);
            db.SaveChanges();
	        return RedirectToAction("Details", "BankAccounts", new { Id = wish.BankAccountId });
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
