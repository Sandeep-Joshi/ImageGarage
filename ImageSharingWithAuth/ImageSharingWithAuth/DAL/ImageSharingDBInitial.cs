using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ImageSharingWithAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ImageSharingWithAuth.Controllers;

namespace ImageSharingWithAuth.DAL
{
    public class ImageSharingDBInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext db)
        {
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db);
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);

            RoleManager<IdentityRole> rm = new RoleManager<IdentityRole>(roleStore);
            UserManager<ApplicationUser> um = new UserManager<ApplicationUser>(userStore);

            IdentityResult ir;
            ApplicationUser Sandeep = createUser("sandeep@example.org");
            ApplicationUser Batman = createUser("Batman@gotham.org");
            ApplicationUser nixon = createUser("nixon@watergate.org");

            ir = um.Create(Sandeep, "SandeepJoshi");
            //Sandeep = um.FindByName(Sandeep.UserName);

            ir = um.Create(Batman, "BruceWayne");
            //Batman = um.FindByName(Batman.UserName);

            ir = um.Create(nixon, "RichardNixon");
            //nixon = um.FindByName(nixon.UserName);

            rm.Create(new IdentityRole("User"));
            if (!um.IsInRole(Batman.Id, "User"))
            {
                um.AddToRole(Batman.Id, "User");
            }
            if (!um.IsInRole(nixon.Id, "User"))
            {
                um.AddToRole(nixon.Id, "User");
            }
            if (!um.IsInRole(Sandeep.Id, "User"))
            {
                um.AddToRole(Sandeep.Id, "User");
            }

            rm.Create(new IdentityRole("Admin"));
            if (!um.IsInRole(Sandeep.Id, "Admin"))
            {
                um.AddToRole(Sandeep.Id, "Admin");
            }

            rm.Create(new IdentityRole("Approver"));
            if (!um.IsInRole(Batman.Id, "Approver"))
            {
                um.AddToRole(Batman.Id, "Approver");
            }

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
                Approved = true
            });

            db.Images.Add(new Image
            {
                Caption = "Spike Speagel",
                Description = "Cowboy Bebop",
                DateTaken = new DateTime(2015, 01, 01),
                Userid = Sandeep.Id,
                TagId = BaseController.getIdForTag("Anime"),   //Convert this to method to get Tag id from name
                Approved = false
            });

            db.SaveChanges();
            base.Seed(db);
        }

        private ApplicationUser createUser(String userName)
        {
            return new ApplicationUser { UserName = userName, Email = userName };
        }
    }
}