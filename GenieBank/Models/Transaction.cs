using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GenieBank.Models
{
	[CustomValidation(typeof(Transaction), "ValidateAmount")]
	public class Transaction
	{
		public int Id { get; set; }

		[Display(Name = "Recipient")]
		public string Account { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Transaction Date")]
		[CustomValidation(typeof(Transaction), "ValidateDate")]
		public DateTime TransactionDate { get; set; }
		
		[DataType(DataType.Currency)]
		[CustomValidation(typeof(Transaction), "ValidateAmountNotZero")]
		public decimal Amount { get; set; }

		[Required (ErrorMessage = "You must enter a note explaining purpose of purchase or expense")]
		public string Note { get; set; }

		public int BankAccountId { get; set; }
		public virtual BankAccount BankAccount { get; set; }

		public static ValidationResult ValidateDate(DateTime date, ValidationContext context)
		{
			if (date.Year < DateTime.Now.Year)
			{
				return new ValidationResult("Transaction Date cannot be before current year");
			}
			else if (date > DateTime.Now)
			{
				return new ValidationResult("Transaction Date cannot be in the future");
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateAmount(Transaction transaction, ValidationContext context)
		{
			if (transaction == null)
			{
				return ValidationResult.Success;
			}
			ApplicationDbContext db = new 	ApplicationDbContext();
			Models.BankAccount bankAccount = db.BankAccounts.Find(transaction.BankAccountId);

			if (bankAccount?.CurrentBalanceWithInterest() + transaction.Amount < 0 )
			{
				return new ValidationResult("Withdraw cannot exceed current balance");
			}
			return ValidationResult.Success;
		}

		public static ValidationResult ValidateAmountNotZero(decimal amount, ValidationContext context)
		{
			if (amount == 0)
			{
				return new ValidationResult("Transaction Amount cannot be equal to 0");
			}
			return ValidationResult.Success;
		}
	}
}