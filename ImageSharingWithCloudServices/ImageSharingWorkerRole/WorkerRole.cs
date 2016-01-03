using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using ImageSharingWebRole.DAL;
using ImageSharingWebRole.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using System.Web;
using ImageSharingWebRole.Queues;

namespace ImageSharingWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = ValidationQueue.QueueName;

        bool IsStopped;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        ApplicationDbContext db = new ApplicationDbContext();

        public override void Run()
        {
            //while (!IsStopped)
            //{
            //    ImageValidationRequest req = ValidationRequestQueue.Re
            //}

            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage( OnMessageReceived);
            CompletedEvent.WaitOne();
        }

        public void OnMessageReceived(BrokeredMessage receivedMessage)
        {
            try
            {
                Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                ApplicationDbContext db = new ApplicationDbContext();
                // Process the message
                ValidationRequest request = receivedMessage.GetBody<ValidationRequest>();

                // Validate that it is a JPEG message
                Image image = db.Images.Find(request.imageId);
                image.Validated = ValidateImage(image);
                QueueManager qm = new QueueManager(image.User.Id);

                if (image.Validated)
                {
                    db.Entry(image).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    qm.addMessage(image, " Validated but still requires final approval.");
                }
                else
                {
                    //Remove the blobs
                   // HttpServerUtilityBase  server = new HttpServerUtilityWrapper(HttpContext.Current.Server);
                    //Remove the db entry
                    qm.addMessage(image, " failed validation. Please only upload JPEGs");
                    db.Images.Remove(image);
                    ImageStorage.DeleteFile(null, image.Id);
                }
             }
            catch (Exception ex)
            {
                // Handle any message processing specific exceptions here
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }

        private bool ValidateImage(Image image)
        {
            bool isValid = false; //image.Validated;
            if (!isValid)
            {
                try {
                    System.IO.Stream imageFile = ImageStorage.GetImageFromBlob(image.Id);
                    System.Drawing.Image img = System.Drawing.Image.FromStream(imageFile);
                    if (img.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
                    {
                        isValid = true;
                    }
                }catch(Exception ex)
                {
                    return false; //this happens when its not an image entirely
                }
            }

            return isValid;
        }

        public void WriteLog(String msg)
        {
            //if (!EventLog.SourceExists("Application"))
            //  EventLog.CreateEventSource("Application", "ImageSharing");
            EventLog.WriteEntry("Application", msg);
        }
    }
}
