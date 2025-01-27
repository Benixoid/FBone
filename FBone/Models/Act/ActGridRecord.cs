using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Act
{
    public class ActGridRecord
    {
        public int Id { get; set; }        
        //public string StatusName { get; set; }
        //public bool IsClosed { get; set; }
        public string AreaName { get; set; }
        public string FacilityName { get; set; }        
        public DateTime ActDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public int StatusId { get; set; }
        public int Type { get; set; }
        public string Equipment { get; set; }        
        public string Unit { get; set; }
    }
}
