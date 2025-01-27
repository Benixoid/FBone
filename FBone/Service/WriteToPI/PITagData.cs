using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBone.Service.WriteToPI
{
    public class PITagData
    {
        public PITagData() { }

        public PITagData(string tagname, List<PIEvent> events)
        {
            Tagname = tagname;
            Events = events;
        }

        public List<PIEvent> Events { get; set; }
        public string Tagname { get; set; }

    }
}
