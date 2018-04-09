using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GenieBank.Models;

namespace GenieBank.Controllers
{
	public class HomeController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult FinancialResources()
		{
			return View();
		}

		public ActionResult ProjectStatus()
		{
			ViewBag.Message = "Project Status";

			return View();
		}
	}
}