using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Reporting
{
    public class ActItemsReportingGridRecord
    {
        public int Id { get; set; }
        public int ActId { get; set; }
        public string TagNumber { get; set; }
        //public string Unit { get; set; }
        public string Equipment { get; set; }
        public string StartedOn { get; set; }
        public string ClosedOn { get; set; }
        public string Cause { get; set; }
        public int ActType { get; set; }
        public string Area { get; set; }
        public string Facility { get; set; }
        public int AreaId { get; set; }
        public string Device { get; set; }
        public int DeviceId { get; set; }
        public string Action { get; set; }
    }
}
