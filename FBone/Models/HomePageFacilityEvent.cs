using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models
{
    public class HomePageFacilityEvent
    {
        public string Name { get; set; }
        public List<HomePageAreaEvent> Areas { get; set; }
    }
}
