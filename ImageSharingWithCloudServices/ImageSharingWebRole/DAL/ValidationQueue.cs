using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using ImageSharingWebRole.Models;

namespace ImageSharingWebRole.DAL
{
    public static class ValidationQueue
    {
        // The name of your queue
        public const string QueueName = "ValidationQueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        static QueueClient Client;
        //ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public static void Initialize()
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
            //return base.OnStart();
        }

        public static void Send(ValidationRequest request)
        {
            if (Client == null)
            {
                Initialize();
            }
            //Send message
            Client.Send(new BrokeredMessage(request));
        }

        public static void flush()
        { //Need better method here

            if (Client == null)
            {
                Initialize();
            }

            while (Client.Peek() != null)
            {
                var brokeredMessage = Client.Receive();
                brokeredMessage.Complete();
            }
        }

        public static void Finalize()
        {
            if (Client == null)
            {
                Initialize();
            }
            // Close the connection to Service Bus Queue
            Client.Close();
            //CompletedEvent.Set();
            //base.OnStop();
        }

    }
}