using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GenieBank.Models
{
	public class Wish
	{
		public int Id { get; set; }

		[Display(Name = "Recipient")]
		public string Account { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date Wish Added")]
		public DateTime WishDate { get; set; }

		[Required(ErrorMessage = "Approximate cost of item is required")]
		[DataType(DataType.Currency)]
		public decimal Cost { get; set; }

		[Required(ErrorMessage = "Description of item is required")]
		public string Description { get; set; }

		[Url]
		[Display(Name = "Website Link")]
		public string WebsiteLink { get; set; }

		public bool Purchased { get; set; }

		public int BankAccountId { get; set; }
		public virtual BankAccount BankAccount { get; set; }

		public bool Affordable()
		{
			decimal currentBalanceWithInterest = BankAccount.Principle();
			if (Cost >= currentBalanceWithInterest)
			{
				return true;
			}
			return false;
		}

		public decimal BalanceAfterPurchase()
		{
			decimal balancePostPurchase = BankAccount.CurrentBalanceWithInterest() - Cost;
			return Math.Round(balancePostPurchase,2);
		}

		public decimal WishPercentBalance()
		{
			decimal percentBalance = BankAccount.CurrentBalanceWithInterest()/Cost;
			return percentBalance;
		}

		public decimal WishPercentAmountNeeded()
		{
			decimal percentAmountNeeded = Math.Abs(BalanceAfterPurchase() / Cost);
			return percentAmountNeeded;
		}

	}
}