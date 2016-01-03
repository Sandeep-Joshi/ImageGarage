using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using ImageSharingWithCloudStorage.DAL;

namespace ImageSharingWithCloudStorage.Models
{
    public class LogEntry : TableEntity
    {
        public LogEntry() { }
        public LogEntry(int imageId) { CreateKeys(imageId); }

        public DateTime EntryDate { get; set; }

        public string Userid { get; set; }

        public string userName()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser Usr = db.Users.Find(this.Userid);
            if (Usr != null)
                return Usr.UserName;
            else
                return Userid;
        }

        public string Caption { get; set; }

        public string Uri { get; set; }

        public int ImageId { get; set; }

        public void CreateKeys(int imageId)
        {
            EntryDate = DateTime.UtcNow;
            PartitionKey = EntryDate.ToString("MMddyyyy");
            this.ImageId = imageId;
            RowKey = string.Format("{0}-{1:10}_{2}", 
                ImageId, 
                DateTime.MaxValue.Ticks - EntryDate.Ticks, 
                Guid.NewGuid()); 
        }
    }
}