using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ImageSharingWebRole.DAL
{   [DataContract]
    [KnownType(typeof(custHttp))]
    public class ServerSerializable
    {
        public ServerSerializable() { }

        public custHttp server { get; set; }
    }

    [DataContract]
    public class custHttp : HttpServerUtilityBase
    {
        public custHttp(HttpServerUtilityBase server)
        {
            this.MachineName = server.MachineName;
            this.ScriptTimeout = server.ScriptTimeout;
        }
        [DataMember]
        public virtual string MachineName { get; }
        [DataMember]
        public virtual int ScriptTimeout { get; set; }

    }
}