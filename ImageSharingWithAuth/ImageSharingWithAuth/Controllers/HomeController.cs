using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageSharingWithAuth.Models;
using System.Data.Entity;

namespace ImageSharingWithAuth.Controllers
{
    public class HomeController : BaseController
    {
        private Int32 Limit = 4 * 1024 * 1024;  //Image size restricted to 4mb

        // GET: Home
        public ActionResult Index(String id = "Stranger")
        {
            CheckAda();
            ViewBag.Title = "Welcome !";
            ApplicationUser user = GetLoggedInUser();
            if (user == null)
            {
                ViewBag.Id = id;
            }
            else
            {
                ViewBag.Id = user.UserName;
            }
            return View();
        }

        public ActionResult Error(String errid = "Unspecified")
        {
            if ("Details".Equals(errid))
            {
                ViewBag.Message = "Problem with details actions";
            }
            else
            {
                ViewBag.Message = "Unspecified Error!";
            }
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Image hosting website.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us.";
            return View();
        }
    }
}