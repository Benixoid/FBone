using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Reporting
{
    public class ReportListModel
    {
        public List<string> ReportGroups { get; set; }
        [DataType(DataType.Date)]
        public DateTime EventFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime EventTo { get; set; }
        [DataType(DataType.Date)]
        public DateTime QAQCFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime QAQCTo { get; set; }
        public string Lang { get; set; }
        public string URL { get; set; }
        public SelectList Facilities { get; set; }
        public SelectList ReportGroup { get; set; }
        public string DateFormat { get; set; }
    }
}
