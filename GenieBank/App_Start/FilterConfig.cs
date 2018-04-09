using System.Web;
using System.Web.Mvc;
using System;

namespace GenieBank
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
            //This locks down all when we are ready
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
        }
	}
}
