using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Models.Reporting
{
    public class ReportingModel
    {
        public DateTime EventFrom { get; set; }
        public DateTime EventTo { get; set; }
        public List<EventGridRecord> EventList { get; set; }

        public DateTime ActItemsFrom { get; set; }
        public DateTime ActItemsTo { get; set; }
        public List<ActItemsReportingGridRecord> ActItemsList { get; set; }

        public string Lang { get; set; }
        public string URL { get; set; }
        public string DateFormat { get; set; }
        public SelectList Areas { get; set; }
        public SelectList Devices { get; set; }
        public List<string> ReportGroups { get; set; }
        public int DefaultDevice { get; set; }
        public int DefaultEventType { get; set; }
        public int DefaultReportIt { get; set; }

    }
}
