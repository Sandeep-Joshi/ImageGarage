using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageSharingWithAuth.Models;
using ImageSharingWithAuth.DAL;
using System.Data.Entity;

namespace ImageSharingWithAuth.Controllers
{
    public class ImagesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private Int32 Limit = 4 * 1024 * 1024;  //Image size restricted to 4mb

        // GET: Images
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [RequireHttps]
        public ActionResult Upload()
        {
            CheckAda();
            ApplicationUser userid = GetLoggedInUser();
            if (userid != null)
            {
                ViewBag.Userid = userid.Id;
                ViewBag.Message = "";
                ViewBag.Tags = new SelectList(db.Tags, "Id", "Name", 1);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHttps]
        public ActionResult Upload(ImagesView image,
                                   HttpPostedFileBase ImageFile)
        {
            CheckAda();

            String message = "";
            Boolean flag = false;
            System.Drawing.Image img;

            TryUpdateModel(image);

            if (ModelState.IsValid)
            {

                //HttpCookie cookie = Request.Cookies.Get("ImageSharing");
                ApplicationUser userid = GetLoggedInUser();
                ApplicationUser user = db.Users.SingleOrDefault(u => u.Id.Equals(userid.Id));

                //if (cookie != null)
                //{
                //    image.Userid = cookie["Userid"];
                //    User user = db.Users.SingleOrDefault(u => u.Userid.Equals(image.Userid));

                    if (user != null)
                    {
                        /*
                        * Save image information in the database
                        */

                        //JavaScriptSerializer serializer = new JavaScriptSerializer();
                        //String jsonData = serializer.Serialize(image);

                        Image imageEntity = new Image();
                        imageEntity.Caption = image.Caption;
                        imageEntity.Description = image.Description;
                        imageEntity.DateTaken = image.DateTaken;
                        imageEntity.Userid = user.Id;  //navigation property
                        imageEntity.TagId = image.TagId; //navigation property

                        if (ImageFile != null && ImageFile.ContentLength > 0)
                        {

                            if (ImageFile.ContentLength > Limit)
                            {
                                flag = true;
                                message = "Image size exceed permissible values";
                            }
                            else if (ImageFile.ContentType != "image/jpeg")
                            {
                                flag = true;
                                message = "File type " + ImageFile.ContentType +
                                    " is not valid.  Only supports 'jpeg'.";
                            }
                            else
                            {
                                try
                                {
                                    img = System.Drawing.Image.FromStream(ImageFile.InputStream);

                                }
                                catch (SystemException ex)
                                {
                                    return View("Error");
                                }

                                if (img.RawFormat.Guid != System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
                                {
                                    flag = true;
                                    message = "File is invalid!";
                                }
                                else
                                {
                                    db.Images.Add(imageEntity);
                                    db.SaveChanges();  //Save the changes to db so we get ID chosen by db
                                    //Save the image
                                    //String imgFileName = Server.MapPath("~/Content/Images/img-" + imageEntity.Id + ".jpg");
                                    //ImageFile.SaveAs(imgFileName);
                                    ImageFile.SaveAs(getFilePath(imageEntity.Id));
                                }
                            }
                        }
                        else
                        {
                            flag = true;
                            message = "No file given or 0 sized file.";
                        }

                        if (flag)
                        {
                            ViewBag.Message = message;
                            return View();
                        }
                        else
                        {
                            ViewBag.Title = "Image uploaded.";
                            return RedirectToAction("Details", new { Id = imageEntity.Id });
                        }

                    }
                    else
                    {
                        ViewBag.Message = "No such user found.";
                        return View();
                    }
                //}
                //else
                //{
                //    ViewBag.Message = "You need to register first before uploading.";
                //    return View();
                //}
            }
            else
            {
                ViewBag.Message = "Errors in the form fields";
                return View();
            }

        }

        [HttpGet]
        [RequireHttps]
        //[ValidateAntiForgeryToken]
        public ActionResult Query()
        {
            CheckAda();
            if (GetLoggedInUser() != null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.Message = "";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Details(int Id)
        {
            CheckAda();
            //if (GetLoggedInUser() == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            //else
            //{
                Image imageEntity = db.Images.Find(Id);
                if (imageEntity != null)
                {
                    ImagesView imageView = new ImagesView();
                    imageView.Id = imageEntity.Id;
                    imageView.Caption = imageEntity.Caption;
                    imageView.Description = imageEntity.Description;
                    imageView.DateTaken = imageEntity.DateTaken;
                    imageView.Tagname = imageEntity.Tag.Name;
                    imageView.Userid = imageEntity.Userid;
                    return View(imageView);
                }
                else
                {

                    return RedirectToAction("Error", "Home", new { errid = "Details" });
                }
            //}
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            CheckAda();
            Image imageEntity = db.Images.Find(Id);

            if (imageEntity != null)
            {
                //Validate the person is the owner of the image
                ApplicationUser userid = GetLoggedInUser();
                if (imageEntity.Userid.Equals(userid.Id))
                {
                //HttpCookie cookie = Request.Cookies.Get("ImageSharing");
                //if ((cookie != null) && cookie["Userid"] != null && imageEntity.User.Userid.Equals(cookie["Userid"]))
                //{
                    ViewBag.Message = "";
                    ViewBag.Tags = new SelectList(db.Tags, "Id", "Name", imageEntity.TagId);
                    ImagesView image = new ImagesView();
                    image.Id = imageEntity.Id;
                    image.TagId = imageEntity.TagId;
                    image.Caption = imageEntity.Caption;
                    image.Description = imageEntity.Description;
                    image.DateTaken = imageEntity.DateTaken;
                    return View("Edit", image);
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { errid = "EditNotAuth" });
                }

            }
            else
            {
                ViewBag.Message = "Image with identifier " + Id + " not found";
                ViewBag.Id = Id;
                return RedirectToAction("Error", "Home", new { errid = "EditNotFound" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ImagesView image)
        {
            CheckAda();
            ApplicationUser userid = GetLoggedInUser();

            if (ModelState.IsValid)
            {
                Image imageEntity = db.Images.Find(id);
                if (imageEntity != null)
                {
                    if (imageEntity.Userid.Equals(userid.Id))
                    // HttpCookie cookie = Request.Cookies.Get("ImageSharing");
                    // if ((cookie != null) && cookie["Userid"] != null && imageEntity.User.Userid.Equals(cookie["Userid"]))
                    {
                        imageEntity.TagId = image.TagId;
                        imageEntity.Caption = image.Caption;
                        imageEntity.Description = image.Description;
                        imageEntity.DateTaken = image.DateTaken;
                        db.Entry(imageEntity).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Details", new { Id = id });
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { errid = "EditNotAuth" });

                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { errid = "EditNotFound" });
                }
            }
            else
            {
                //redisplay edit form
                return View("Edit", image);
            }

        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            CheckAda();
            ApplicationUser userid = GetLoggedInUser();
        
            //if (GetLoggedInUser() == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            //else
            //{
            Image imageEntity = db.Images.Find(Id);
            if (imageEntity != null)
            {
                ImagesView imageView = new ImagesView();
                imageView.Id = imageEntity.Id;
                imageView.Caption = imageEntity.Caption;
                imageView.Description = imageEntity.Description;
                imageView.DateTaken = imageEntity.DateTaken;
                imageView.Tagname = imageEntity.Tag.Name;
                imageView.Userid = imageEntity.Userid;
                return View(imageView);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errid = "Delete" });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection vals, int Id)
        {
            CheckAda();
            ApplicationUser userid = GetLoggedInUser();
            Image imageEntity = db.Images.Find(Id);

            if (imageEntity != null)
            {
                //Validate the person is the owner of the image
                if (imageEntity.Userid.Equals(userid.Id))
                {
                    //db.Entry(imageEntity).State = EntityState.Deleted;
                    db.Images.Remove(imageEntity);
                    db.SaveChanges();
                    // Delete image from the file system
                    System.IO.File.Delete(getFilePath(imageEntity.Id));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { errid = "DeleteNotAuth" });
                }
            }
            else
            {
                return RedirectToAction("Error", "Home", new { errid = "DeleteNotFound" });
            }
        }

        [HttpGet]
        public ActionResult ListAll()
        {
            CheckAda();
            IEnumerable<Image> images = ApprovedImages().ToList();
            ApplicationUser userid = GetLoggedInUser();
            if (userid != null)
            {
                ViewBag.Userid = userid.Id;
                return View(images);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        [HttpGet]
        public ActionResult ListByUser()
        {
            CheckAda();
            SelectList users = new SelectList(db.Users.Where(u => u.Active).ToList(), "Id", "UserName", 1);
            return View(users);
        }

        [HttpGet]
        public ActionResult DoListByUser(string Id)
        {
            CheckAda();
            String userid = GetLoggedInUser().Id;
            ApplicationUser user = db.Users.Find(Id);
            if (user != null)
             {
                    ViewBag.Userid = userid;
                    return View("ListAll", ApprovedImages(user.Images));
            }
             else
             {
                return RedirectToAction("Error", "Home", new { errid = "ListByUser" });
             }
        }
  
        [HttpGet]
        public ActionResult ListByTag()
        {
            CheckAda();
            SelectList tags = new SelectList(db.Tags, "Id", "Name", 1);
            return View(tags);
        }

        [HttpGet]
        public ActionResult DoListByTag(int Id)
        {
            CheckAda();
            String userid = GetLoggedInUser().Id;
            if (userid != null)
            {
                Tag tag = db.Tags.Find(Id);
                if (tag != null)
                {
                    ViewBag.Userid = userid;
                    return View("ListAll", ApprovedImages(tag.Images).ToList());
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { errid = "ListByTag" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Approver")]
        public ActionResult Approve()
        {
            CheckAda();
            ViewBag.Message = "";
            var db = new ApplicationDbContext();
            List<SelectItemView> model = new List<SelectItemView>();
            foreach (var u in db.Images)
            {
                if (!u.Approved)
                {
                    model.Add(new SelectItemView(u.Id.ToString(), u.Caption, u.Approved));
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Approver")]
        public ActionResult Approve(List<SelectItemView> model)
        {
            CheckAda();
            var db = new ApplicationDbContext();
            foreach (var imod in model)
            {
                Image image = db.Images.Find(int.Parse(imod.Id));
                if (imod.Checked)
                {
                    image.Approved = true;
                }
                imod.Name = image.Caption;
            }
            db.SaveChanges();
            ViewBag.Message = "Images approved.";
            return View(model);
        }
    }

}