using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ImageSharingWithCloudStorage.DAL;

namespace ImageSharingWithCloudStorage.Models
{
    public class ImagesView
    {
        //View Model
        [Required]
        [StringLength(40)]
        public String Caption { get; set; }
        [Required]
        public int TagId { get; set; }
        [Required]
        [StringLength(220)]
        public String Description { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }

        [ScaffoldColumn(false)]
        public int Id;
        [ScaffoldColumn(false)]
        public string Uri;
        [ScaffoldColumn(false)]
        public String Tagname { get; set; }
        [ScaffoldColumn(false)]
        public String Userid { get; set; }

        public String uploader()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser Usr = db.Users.Find(Userid);
            if (Usr != null)
                return Usr.UserName;
            else
                return "";
        }

        public ImagesView()
        {

        }

        public ImagesView(Image imageEntity)
        {
            Id = imageEntity.Id;
            Caption = imageEntity.Caption;
            Description = imageEntity.Description;
            DateTaken = imageEntity.DateTaken;
            Tagname = imageEntity.Tag.Name;
            Userid = imageEntity.Userid;
        }
    }
}