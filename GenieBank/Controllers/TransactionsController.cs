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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: Transactions
        //public ActionResult Index()
        //{
        //    var transactions = db.Transactions.Include(t => t.BankAccount);
        //    return View(transactions.ToList());
        //}

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(transaction);
        }

        // GET: Transactions/Create
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
            var transaction = new Transaction {BankAccountId = BankAccountId, Account = recipient, TransactionDate = DateTime.Now};
			return View(transaction);
        }
		// POST: Transactions/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Account,Amount,Note,TransactionDate,BankAccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
                if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "BankAccounts", new { Id = transaction.BankAccountId });
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.BankAccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.BankAccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Account,Amount,Note,TransactionDate,BankAccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
			{
                BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
                if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                db.Entry(transaction).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Details", "BankAccounts", new { Id = transaction.BankAccountId });
			}
			ViewBag.BankAccountId = new SelectList(db.BankAccounts, "Id", "OwnerEmail", transaction.BankAccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);
            if (User.Identity.Name != bankAccount.OwnerEmail && User.Identity.Name != bankAccount.RecipientEmail)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db.Transactions.Remove(transaction);
            db.SaveChanges();
	        return RedirectToAction("Details", "BankAccounts", new { Id = transaction.BankAccountId });
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
