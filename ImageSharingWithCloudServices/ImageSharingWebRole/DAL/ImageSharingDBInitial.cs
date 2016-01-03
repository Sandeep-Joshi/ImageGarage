using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ImageSharingWebRole.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ImageSharingWebRole.Controllers;
using System.Web.Security;
using WebMatrix.WebData;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using ImageSharingWebRole.Queues;

namespace ImageSharingWebRole.DAL
{
    public class ImageSharingDBInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    //public class ImageSharingDBInitializer : IDatabaseInitializer<ApplicationDbContext>
    {
        //public void InitializeDatabase(ApplicationDbContext db)
        //{
        //    if (db.Database.Exists())
        //    {
        //        //Single user mode not available in Azure SQL 
        //        db.Database.Delete();
        //    }
        //    db.Database.Create();
        //    WebSecurity.InitializeDatabaseConnection(
        //        "DefaultConnection",
        //        "Users",
        //        "Id",
        //        "userId",
        //        autoCreateTables: true);
        //    this.Seed(db);
        //    //throw new NotImplementedException();
        //}

        protected override void Seed(ApplicationDbContext db)
        {

            /*if (!WebSecurity.UserExists("sandeep@stevens.edu"))
            {
                WebSecurity.CreateUserAndAccount(
                    "sandeep@stevens.edu",
                    "SandeepJoshi",
                    new { ADA = false, Active = true });
            }

            if (!WebSecurity.UserExists("batman@gotham.org"))
            {
                WebSecurity.CreateUserAndAccount(
                    "batman@gotham.org",
                    "BruceWayne",
                    new { ADA = false, Active = true });
            }

            if (!WebSecurity.UserExists("nixon@watergate.org"))
            {
                WebSecurity.CreateUserAndAccount(
                    "nixon@watergate.org",
                    "RichardNixon",
                    new { ADA = true, Active = true });
            }

            db.Tags.Add(new Tag { Name = "Abstract" });
            db.Tags.Add(new Tag { Name = "Anime" });
            db.Tags.Add(new Tag { Name = "Music" });
            db.Tags.Add(new Tag { Name = "Nature" });
            db.Tags.Add(new Tag { Name = "Sports" });

            if (!Roles.RoleExists("User"))
                Roles.CreateRole("User");
            if (!Roles.RoleExists("Admin"))
                Roles.CreateRole("Admin");
            if (!Roles.RoleExists("Approver"))
                Roles.CreateRole("Approver");

            db.SaveChanges();

            if (!Roles.GetRolesForUser("batman@gotham.org").Contains("Approver"))
               Roles.AddUserToRole("batman@gotham.org", "Approver");

            if (!Roles.GetRolesForUser("sandeep@stevens.org").Contains("Admin"))
                Roles.AddUserToRole("sandeep@stevens.org", "Admin");
            
            if (!Roles.GetRolesForUser("nixon@watergate.org").Contains("User"))
                Roles.AddUserToRole("nixon@watergate.org", "User");
            */



            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db);
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);

            RoleManager<IdentityRole> rm = new RoleManager<IdentityRole>(roleStore);
            UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(userStore);

            IdentityResult ir;
            ApplicationUser Sandeep = createUser("sandeep@example.org");
            ApplicationUser Batman = createUser("Batman@gotham.org");
            ApplicationUser nixon = createUser("nixon@watergate.org");
            ApplicationUser super = createUser("superman@solitude.org");

            //Flush Validation queue
            ValidationQueue.flush();

            //Delete previous user queues
            QueueManager.deleteQueues();

            ir = um.Create(Sandeep, "SandeepJoshi");
            Sandeep.addQueue();
            ir = um.Create(Batman, "BruceWayne");
            Batman.addQueue();
            ir = um.Create(nixon, "RichardNixon");
            nixon.addQueue();
            ir = um.Create(super, "ClarkKent");
            super.addQueue();

            rm.Create(new IdentityRole("User"));
            if (!um.IsInRole(Batman.Id, "User"))
                um.AddToRole(Batman.Id, "User");

            if (!um.IsInRole(nixon.Id, "User"))
                um.AddToRole(nixon.Id, "User");

            if (!um.IsInRole(Sandeep.Id, "User"))
                um.AddToRole(Sandeep.Id, "User");

            rm.Create(new IdentityRole("Admin"));
            if (!um.IsInRole(Sandeep.Id, "Admin"))
                um.AddToRole(Sandeep.Id, "Admin");

            rm.Create(new IdentityRole("Approver"));
            if (!um.IsInRole(Batman.Id, "Approver"))
                um.AddToRole(Batman.Id, "Approver");

            rm.Create(new IdentityRole("Supervisor"));
            if (!um.IsInRole(super.Id, "Supervisor"))
                um.AddToRole(super.Id, "Supervisor");

            db.Tags.Add(new Tag { Name = "Abstract" });
            db.Tags.Add(new Tag { Name = "Anime" });
            db.Tags.Add(new Tag { Name = "Music" });
            db.Tags.Add(new Tag { Name = "Nature" });
            db.Tags.Add(new Tag { Name = "Sports" });

            db.SaveChanges();

            db.Images.Add(new Image
            {
                Caption = "Pink Floyd",
                Description = "Music gods",
                DateTaken = new DateTime(2015, 01, 01),
                Userid = Sandeep.Id,
                TagId = BaseController.getIdForTag("Music"),   //Convert this to method to get Tag id from name
                Approved = true,
                Validated = true
            });

            //upload image to blob
            //HttpPostedFileBase postedFile = new HttpPostedFileBase();
            //HttpServerUtilityBase server = null;
            //FileStream f = new FileStream((@"~/Images/PinkFloyd.jpg"), FileMode.Open, FileAccess.Read);
            //postedFile.InputStream.CopyTo(f);
            //f.Close();
            //ImageStorage.SaveFile(server, postedFile, 1);

            CloudStorageAccount account = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(ImageStorage.CONTAINER);
            CloudBlockBlob blob = container.GetBlockBlobReference(ImageStorage.FilePath(null, 1));
            try
            {
                using (var file = System.IO.File.OpenRead(@"C:\Users\joshi\Documents\Visual Studio 2015\Projects\ImageSharingWithCloudStorage\ImageSharingWithCloudStorage\Images\PinkFloyd.jpg"))
                {
                    blob.UploadFromStream(file);
                    //Add it in the user queue

                    //Add it in the validation queue
                }
            }
            catch (Exception ex)
            {

            }

            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(LogContext.LOG_TABLE_NAME);
            //Delete the table it if exists.
            try
            {
                table.DeleteIfExists();
            }
            catch (Exception ex)
            {
                ///do nothing.. we don't really need to delete this
            }



            //db.Images.Add(new Image
            //{
            //    Caption = "Spike Speagel",
            //    Description = "Cowboy Bebop",
            //    DateTaken = new DateTime(2015, 01, 01),
            //    Userid = Sandeep.Id,
            //    TagId = BaseController.getIdForTag("Anime"),   //Convert this to method to get Tag id from name
            //    Approved = false
            //});

            db.SaveChanges();
            base.Seed(db);

            //LogContext.CreateTable //Should happen on adding image by itself
        }

        private ApplicationUser createUser(String userName)
        {
            return new ApplicationUser { UserName = userName, Email = userName };
        }
    }
}