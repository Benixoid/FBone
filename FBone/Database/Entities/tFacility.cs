using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBone.Database.Entities
{
    public class tFacility
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public IEnumerable<tArea> Areas { get; set; }
        public IEnumerable<tArea> AreasShiftEng { get; set; }
        public IEnumerable<tUser> Users { get; set; }
        public long Force_maxId { get; set; }
        public long Alarm_maxId { get; set; }        
        public string TranslatorEmail { get; set; }
        public string TagBypassTotal { get; set; }
        public string TagForcesTotal { get; set; }        
    }
}
