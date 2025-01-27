using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models
{
    public class HomePageLocks
    {
        public string FacilityName { get; set; }
        public int FacilityId { get; set; }

        public int Forces { get; set; }
        public int Bypasses { get; set; }
        public int Disabled { get; set; }
        public int Inhibited { get; set; }
    }
}
