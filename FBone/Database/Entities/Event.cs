using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class Event
    {
        public int Id { get; set; }

        public int TagId { get; set; }

        [Range(minimum: 1, maximum: 4)]
        public int TypeId { get; set; }

        public DateTime ShiftDate { get; set; }

        //1 - Day, 2 - Night
        public int ShiftType { get; set; }

        public DateTime EventDateTimeSet { get; set; }

        public DateTime? EventDateTimeClear { get; set; }

        public string Action { get; set; }

        public string DataOrigin { get; set; }
                
        public int? ActItemId { get; set; }
        public tActItems ActItem { get; set; }

        public bool AddedManually { get; set; }

        public bool ReportIt { get; set; }

        public long PSSEventId { get; set; }

        public Tag Tag { get; set; }
    }
}
