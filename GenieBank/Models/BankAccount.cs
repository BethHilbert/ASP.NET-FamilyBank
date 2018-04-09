using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Gtt.InterestCalculator;

namespace GenieBank.Models
{
	public class BankAccount
	{
		public int Id { get; set; }

		[Display(Name = "Parent's Email")]
		[EmailAddress]
		public string OwnerEmail { get; set; }

		[Display(Name = "Recipient's Email")]
		[EmailAddress]
		public string RecipientEmail { get; set; }

		[Required(ErrorMessage = "Recipient Name is Required")]
		[Display(Name = "Recipient's Name")]
		public string Name { get; set; }

		[Display(Name = "Open Date")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Column(TypeName = "DateTime2")]
		public DateTime OpenDate { get; set; }

		[Display(Name = "Interest Rate")]
		[CustomValidation(typeof(BankAccount), "ValidateBetweenZeroAndOne")]
		public decimal InterestRate { get; set; }

		public virtual List<Transaction> Transactions { get; set; }
		public virtual List<Wish> Wishes { get; set; }

		public BankAccount()
		{
			Transactions = new List<Transaction>();
			Wishes = new List<Wish>();
		}

		[Display(Name = "Interest Earned")]
		public decimal YearToDateInterestEarned()
		{
			decimal startingBalance = 0;
			decimal interestRate = InterestRate;
			var intTrans = new List<InterestTransaction>();
			foreach (var t in Transactions)
			{
				var intTx = new InterestTransaction(t.TransactionDate, t.Amount);
				intTrans.Add(intTx);
			}
			decimal interest = CompoundInterestCalculator.CalculateYearToDateInterest(startingBalance, interestRate, intTrans.ToArray());
			return Math.Round(interest, 2);
		}

		[Display(Name = "Principle")]
		public decimal Principle()
		{
			decimal currentPrinciple = Transactions.Sum(x => x.Amount);
			return currentPrinciple;
		}

		[Display(Name = "Current Balance With Interest")]
		public decimal CurrentBalanceWithInterest()
		{
			decimal currentBalanceWithInterest = YearToDateInterestEarned() + Principle();
			return Math.Round(currentBalanceWithInterest, 2);
		}

		public decimal PercentInterest()
		{
			if (CurrentBalanceWithInterest() == 0)
			{
				return 0;
			}
			decimal percentInterest = YearToDateInterestEarned() / CurrentBalanceWithInterest();
			return percentInterest;
		}

		public decimal PercentPrinciple()
		{
			if (CurrentBalanceWithInterest() == 0)
			{
				return 0;
			}
			decimal percentPrinciple = Principle() / CurrentBalanceWithInterest();
			return percentPrinciple;
		}

		public static ValidationResult ValidateBetweenZeroAndOne(decimal InterestRate, ValidationContext context)
		{
			if (InterestRate <= 0 || InterestRate >= 1)
			{
				return new ValidationResult("Interest rate must expressed as a decimal above 0 and below 1");
			}
			return ValidationResult.Success;

		}
	}
}