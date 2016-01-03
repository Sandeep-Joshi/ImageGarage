using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

using ImageSharingWithCloudStorage.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Configuration;

namespace ImageSharingWithCloudStorage.DAL
{
    public class LogContext
    {
        public const string LOG_TABLE_NAME = "imageviews";

        protected CloudTable context;

        public LogContext(CloudTable context)
        {
            this.context = context;
        }

        public void addLogEntry(ApplicationUser user, ImagesView image)
        {
            LogEntry entry = new LogEntry(image.Id);
            entry.Userid = user.Id;
            entry.Caption = image.Caption;
            entry.ImageId = image.Id;
            entry.Uri = image.Uri;

            TableOperation insert = TableOperation.Insert(entry);
            GetContext().Execute(insert);
            //GetContext
            //context.AddObject(LOG_TABLE_NAME, entry);
            //context.SaveChangesWithRetries();

        }

        public static void CreateTable()
        {
            //dont need it happening in GetContext
        }

        public IEnumerable<LogEntry> select()
        {
            TableQuery<LogEntry> query = new TableQuery<LogEntry>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DateTime.UtcNow.ToString("MMddyyy")));

            return context.ExecuteQuery(query);

            //var results = from entity in context.CreateQuery<LogEntry>(LOG_TABLE_NAME)
            //              where entity.PartitionKey == DateTime.UtcNow.ToString("MMddyyyy")
            //              select entity;
            //return results.ToList();
        }
        
        protected static CloudTable GetContext()
        {
            string conn = CloudConfigurationManager.GetSetting("StorageConnectionString");
            //string conn = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            CloudStorageAccount account =
                    CloudStorageAccount.Parse(conn);
                    //(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
                    //(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(LOG_TABLE_NAME);
            table.CreateIfNotExists();
            return table;

            // Departed to join heavenly choir
            //LogContext context = new LogContext(client.GetTableServiceContext());
            //return context;
        }

        public static void AddLogEntry(ApplicationUser user, ImagesView image )
        {
            LogContext log = new LogContext(GetContext());
            log.addLogEntry(user, image);
        }

        public static IEnumerable<LogEntry> Select()
        {
            LogContext log = new LogContext(GetContext());
            return log.select();
        }

       
    }
}