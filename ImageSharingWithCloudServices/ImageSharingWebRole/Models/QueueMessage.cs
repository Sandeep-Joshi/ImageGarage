using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSharingWebRole.Models
{
    public class QueueMessage
    {
        public QueueMessage() { }

        public string Id { get; set; }

        public string InsertionTime { get; set; }

        public string Message { get; set; }
        
    }
}