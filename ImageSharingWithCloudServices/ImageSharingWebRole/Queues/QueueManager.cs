using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageSharingWebRole.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ImageSharingWebRole.Queues
{   //for user specific queues
    public class QueueManager
    {
        private string UserMessagesQueueName; //Queuename is based on username or id

        public QueueManager(string queueName) 
        {
            this.UserMessagesQueueName = queueName;
            //Create queue if not there
            //CloudQueue queue = ConnectToQueue();
        }

        public void SendImageEntityToQueue(Image image)
        {
            // Retrieve storage account from connection string.
            CloudQueue queue = CreateQueue();

            // Create a message and add it to the queue.
            String msg = "Image: " + image.Caption + "is";
            if (image.Validated)
            {
                msg += " Validated but awaits approval.";
            }
            if (!image.Validated)
            {
                msg += " Image uploaded by awaits validation.";
            }
            if (image.Approved)
            {
                msg += " Approved.";
            }

            CloudQueueMessage message = new CloudQueueMessage(msg);
            queue.AddMessage(message);
        }

        public void addMessage(Image image, string message)
        {
            CloudQueue queue = CreateQueue();
            string tmp = "";
            if (image == null)
            {
                tmp = message;
            }
            else
            {
                tmp = "Image: " + image.Caption + " " + message;
            }
            CloudQueueMessage msg = new CloudQueueMessage(tmp);
            queue.AddMessage(msg);
        }

        public //async Task<
            CloudQueue ConnectToQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(UserMessagesQueueName);
            //await 
            return queue;

        }

        public CloudQueue CreateQueue()
        {
            CloudQueue queue = ConnectToQueue();
            try
            {
                queue.CreateIfNotExists();
            }
            catch (Exception ex)
            {

            }
            return queue;
        }

        public void clear()
        {
            CloudQueue queue = ConnectToQueue();
            queue.Clear();
        }

        public List<QueueMessage> ReadFromQueue()
        {
            CloudQueue queue = ConnectToQueue();

            IEnumerable<CloudQueueMessage> retrievedMessages = queue.PeekMessages(20); //might want to use peek here
            List<QueueMessage> messages = new List<QueueMessage>();
            foreach (CloudQueueMessage message in retrievedMessages)
            {
                QueueMessage m = new QueueMessage();
                m.Id = message.Id;
                m.InsertionTime = message.InsertionTime.ToString();
                m.Message = message.AsString;
                messages.Add(m);
                //queue.DeleteMessage(message);
            }
            return messages;
        }

        public void deleteMessage(string id)
        {
            CloudQueue queue = ConnectToQueue();

            IEnumerable<CloudQueueMessage> retrievedMessages = queue.GetMessages(20); //might want to use peek here
            foreach (CloudQueueMessage message in retrievedMessages)
            {
                if((message!= null)&&(id == message.Id))
                {
                    CloudQueueMessage qmsg = queue.GetMessage();
                    queue.DeleteMessage(message);
                    break;
                }            
            }
         }

        // Get all queues
        public static IEnumerable<CloudQueue> getQueues()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.ListQueues();
            
        }

        //Delete all queues
        public static void deleteQueues()
        {
            foreach( CloudQueue q in getQueues())
            {
                q.Clear();
                q.DeleteIfExists();
            }
        }
    }

}