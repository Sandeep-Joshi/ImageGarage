using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ImageSharingWebRole.DAL;

namespace ImageSharingWebRole.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [MaxLength(40)]
        public virtual string Caption { get; set; }
        [MaxLength(200)]
        public virtual string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public virtual DateTime DateTaken { get; set; }
        public virtual bool Approved { get; set; }
        public virtual bool Validated { get; set; }

        //[ForeignKey("User")]
        public virtual string Userid { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Tag")]
        public virtual int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public String uploader()
        {
            ImagesView i = new ImagesView(this);
            return i.uploader();
        }

        public Image()
        {
            Approved = false;
            Validated = false;
        }
    }
}