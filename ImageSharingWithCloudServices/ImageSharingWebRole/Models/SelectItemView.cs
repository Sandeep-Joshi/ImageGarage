using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSharingWebRole.Models
{
    public class SelectItemView
    {
        //
        // GET: /SelectItemView/
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
        public string message { get; set; }

        public SelectItemView(string id, string name, bool c)
        {
            this.Id = id;
            this.Name = name;
            this.Checked = c;
            this.message = "";
        }

        public SelectItemView(string id, string time, bool c, string message)
        {
            this.Id = id;
            this.Name = time;
            this.Checked = c;
            this.message = message;
        }

        public SelectItemView() { }
    }
}