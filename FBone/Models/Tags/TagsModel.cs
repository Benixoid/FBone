using FBone.Database.Entities;
using System.Collections.Generic;

namespace FBone.Models.Tags
{
    public class TagsModel
    {
        public List<Tag> Tags { get; set; }
        public string SearchString { get; set; }
        public int Page { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalEntities { get; set; }
    }
}
