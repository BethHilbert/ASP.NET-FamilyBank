using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GenieBank.Models;
using Microsoft.AspNet.Identity;

namespace GenieBank.Controllers
{
    public class BankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index()
        {
            var accounts = db.BankAccounts.Where(bankAccount => User.Identity.Name == bankAccount.OwnerEmail || User.Identity.Name == bankAccount.RecipientEmail).ToList();
            int accountCount = accounts.Count;
            BankAccount onlyAccount = accounts.FirstOrDefault(); 
            if(accountCount == 1 && User.Identity.Name == onlyAccount.RecipientEmail)
            {
                return RedirectToAction("Details", new { Id = onlyAccount.Id });
            }
            return View(accounts);
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            return View();
        }


		// POST: BankAccounts/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerEmail,OpenDate,RecipientEmail,Name,InterestRate")] BankAccount bankAccount)
        {
            bankAccount.OpenDate = DateTime.Now;
	        if (ModelState.IsValid)
	        {
		        int childEmailInstances = db.BankAccounts.Count(e => e.OwnerEmail == bankAccount.RecipientEmail 
						|| bankAccount.OwnerEmail == bankAccount.RecipientEmail 
						|| e.RecipientEmail == bankAccount.RecipientEmail);
		        if (childEmailInstances > 0)
		        {
			        ModelState.AddModelError("RecipientEmail", "A recipient can only have one account and must have a different email address than their parent. This email address is already assigned.");
			        return View(bankAccount);
		        }
	        }

	        if (ModelState.IsValid)
	        {
		        int parentEmailInstances = db.BankAccounts.Count(e => e.RecipientEmail == bankAccount.OwnerEmail || bankAccount.OwnerEmail == bankAccount.RecipientEmail);
		        if (parentEmailInstances > 0)
		        {
			        ModelState.AddModelError("OwnerEmail", "A parent cannot also be a recipient. This email address is used already assigned to a recipient.");
			        return View(bankAccount);
		        }
	        }

			if (ModelState.IsValid)
            {
                bankAccount.OwnerEmail = User.Identity.Name;
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerEmail,OpenDate,RecipientEmail,Name,InterestRate")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
	        {
		        int childEmailInstances = db.BankAccounts.Count(e => e.OwnerEmail == bankAccount.RecipientEmail || bankAccount.OwnerEmail == bankAccount.RecipientEmail);
		        if (childEmailInstances > 0)
		        {
			        ModelState.AddModelError("RecipientEmail", "A recipient can only have one account and must have a different email address than their parent. This email address is already assigned.");
			        return View(bankAccount);
		        }
	        }

	        if (ModelState.IsValid)
	        {
		        int parentEmailInstances = db.BankAccounts.Count(e => e.RecipientEmail == bankAccount.OwnerEmail || bankAccount.OwnerEmail == bankAccount.RecipientEmail);
		        if (parentEmailInstances > 0)
		        {
			        ModelState.AddModelError("OwnerEmail", "A parent cannot also be a recipient. This email address is used already assigned to a recipient.");
			        return View(bankAccount);
		        }
	        }

			if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
	        if (bankAccount.CurrentBalanceWithInterest() > 0)
	        {
		        return Content("Account cannot be deleted with a positive balance.");
	        }
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // need to find if wishes or transactions exist, if they do then remove			



            db.BankAccounts.Remove(bankAccount);
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
