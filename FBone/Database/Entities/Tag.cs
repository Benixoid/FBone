using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Tagnumber { get; set; }
        [Required]
        public string TagnumberByp { get; set; }
        //public string LCN { get; set; }
        //public string Device { get; set; }
        public string Equipment { get; set; }
        public string Type { get; set; }
        public string Service { get; set; }
        public string Unit { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public int AreaId { get; set; }
        public tArea Area { get; set; }
        public int? DeviceId { get; set; }
        public Device Device { get; set; }
        public bool isFG { get; set; }
        public bool isIPL { get; set; }
        public bool isForBulkInsert { get; set; }

    }
}
