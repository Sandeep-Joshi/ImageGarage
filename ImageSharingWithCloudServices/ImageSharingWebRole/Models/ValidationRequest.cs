using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using ImageSharingWebRole.DAL;

namespace ImageSharingWebRole.Models
{
    [Serializable()]
    public class ValidationRequest
    {
        public int imageId { get; set; }
        public string UserId { get; set; }

        public ValidationRequest(int id, string usrid)
        {
            this.imageId = id;
            this.UserId = usrid;
        }

        //public ServerSerializable server { get; set; }

        //public ValidationRequest(int id, string usrId, HttpServerUtilityBase server)
        //{
        //    this.imageId = id;
        //    this.UserId = usrId;
        //    this.server = new ServerSerializable(server);
        //}

    }
}