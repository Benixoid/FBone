using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models
{
    public class tUserViewModel
    {
        public tUser User { get; set; }
        public SelectList Positions { get; set; }
        public SelectList Facilities { get; set; }
        public SelectList Areas { get; set; }
        public string URL { get; set; }
    }
}
