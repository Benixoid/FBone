using FBone.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Reporting
{
    public class ActiveLocksModel
    {
        public int TypeId { get; set; } //Enums.EventTypeCode
        public int FacilityId { get; set; }
        public List<EventGridRecord> ActiveEventList { get; set; }
        public SelectList Facilities { get; set; }
        public SelectList Types { get; set; }
        public string Host { get; set; }
        public string Lang { get; set; }

    }
}
