using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageSharingWithAuth.DAL;
using ImageSharingWithAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ImageSharingWithAuth.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }
        
        protected BaseController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }
        // GET: Base
        /*public ActionResult Index()
        {
            return View();
        }*/

        protected void CheckAda()
        {
            HttpCookie cookie = Request.Cookies.Get("ImageSharing");
            if (cookie != null)
            {
                //ViewBag.Userid = cookie["Userid"];
                if ("true".Equals(cookie["ADA"]))
                    ViewBag.isADA = true;
                else
                    ViewBag.isADA = false;
            }
            else
            {
                ViewBag.isADA = false;
            }
        }

        protected void SaveCookie(bool ADA)
        {
            HttpCookie cookie = new HttpCookie("ImageSharing");
            cookie.Expires = DateTime.Now.AddMonths(3);
            cookie.HttpOnly = true;
            //cookie["Userid"] = userid;
            cookie["ADA"] = ADA ? "true" : "false";
            Response.Cookies.Add(cookie);
        }

        protected IEnumerable<ApplicationUser> ActiveUsers()
        {
            var db = new ApplicationDbContext();
            return db.Users.Where(u => u.Active);
        }

        protected IEnumerable<Image> ApprovedImages(IEnumerable<Image> images)
        {
            var db = new ApplicationDbContext();
            return images.Where(img => img.Approved);
        }

        public static int getIdForTag(string tagName)
        {
            var db = new ApplicationDbContext();
            var tg = db.Tags.Where(u => u.Name.Equals(tagName));
            foreach(var row in tg)
            {
                return row.Id;
            }
            return 1;
        }

        protected IEnumerable<Image> ApprovedImages()
        {
            var db = new ApplicationDbContext();
            return ApprovedImages(db.Images);
        }

        protected SelectList UserSelectList()
        {
            String defaultId = GetLoggedInUser().Id;
            return new SelectList(ActiveUsers(), "Id", "UserName", defaultId);
        }

        protected ApplicationUser GetLoggedInUser()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }

        //protected String GetLoggedInUser()
        //{
        //    HttpCookie cookie = Request.Cookies.Get("ImageSharing");
        //    if (cookie != null && cookie["Userid"] != null)
        //    {
        //        return cookie["Userid"];
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        //protected ActionResult ForceLogin()
        //{
        //    return RedirectToAction("Login", "Account");
        //}

        protected String getFilePath(int id)
        {
            return Server.MapPath("~/Content/Images/img-" + id + ".jpg");
        }
    }
}