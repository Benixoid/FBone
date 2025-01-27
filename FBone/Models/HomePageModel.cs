using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models
{
    public class HomePageModel
    {
        public bool canCreateAct { get; set; }
        public bool canTranslateAct { get; set; }
        public bool isShiftEngineer { get; set; }
        public List<HomePageFacilityEvent> Facilities { get; set; }
        public List<HomePageLocks> Locks { get; set; }
    }
}
