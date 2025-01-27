using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FBone.Models.Reporting
{
    public class EventGridRecord
    {
        public int Id { get; set; }
        public string Facility { get; set; }
        public string Area { get; set; }
        public string Device { get; set; }
        //public string Unit { get; set; }
        //public string Type { get; set; }
        public string Tagnumber { get; set; }
        public string Service { get; set; }
        public DateTime SetTime { get; set; }
        public DateTime? ClearTime { get; set; }
        public string SetTimeS { get; set; }
        public string ClearTimeS { get; set; }
        public int ActNum { get; set; }
        public string Cause { get; set; }
        public string DataOrigin { get; set; }
        public bool AddedManually { get; set; }
        public bool ReportIt { get; set; }
        public int EventType { get; set; }
        public string Action { get; set; }
    }
}
