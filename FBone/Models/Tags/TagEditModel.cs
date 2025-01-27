using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Models.Tags
{
    public class TagEditModel
    {
        public Tag Tag { get; set; }
        public SelectList Devices { get; set; }
        public SelectList Areas { get; set; }
        public string SearchString { get; set; }
    }
}
