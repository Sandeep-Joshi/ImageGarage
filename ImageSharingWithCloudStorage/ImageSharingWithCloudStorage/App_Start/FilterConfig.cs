using System.Web;
using System.Web.Mvc;

namespace ImageSharingWithCloudStorage
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //Adds authorization to every container as a default
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            filters.Add(new System.Web.Mvc.RequireHttpsAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
