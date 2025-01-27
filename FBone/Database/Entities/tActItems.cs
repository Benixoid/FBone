using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class tActItems
    {
        public int Id { get; set; }
        public int ActId { get; set; }
        public tAct Act { get; set; }
        [Required]
        public string TagName { get; set; }
        [Required]
        public string Unit { get; set; }
        [Required]
        public string Equipment { get; set; }
        //public DateTime SetTime { get; set; }
        //public DateTime ClearTime { get; set; }
        public string Action { get; set; }
        public int DeviceId { get; set; }
        public string Location { get; set; }
        public Event Event { get; set; }
    }
}
