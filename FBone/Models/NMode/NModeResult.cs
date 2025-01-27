using System;
using System.ComponentModel.DataAnnotations;

namespace FBone.Models.NMode
{
    public class NModeResult
    {
        public NModeResult () { }
        [Key]
        public Guid UID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int RecordId { get; set; }
        public NModeRecord Record { get; set; }
        public double NormalTotal { get; set; }
        public double DayNormal { get; set; }
        public double DayManual { get; set; }
        public double DayOther { get; set; }
        public double NightNormal { get; set; }
        public double NightManual { get; set; }
        public double NightOther { get; set; }
        public DateTime RecordTime { get; set; }
        public string User { get; set; }
        public bool Evaluation { get; set; } = true;
    }
}
