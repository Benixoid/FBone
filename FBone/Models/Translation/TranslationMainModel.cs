using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Models.Act;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Models.Translation
{
    public class TranslationMainModel
    {
        public SelectList Facilities { get; set; }
        public int SelectedFacilityId { get; set; }
        public List<ActGridRecord> ActList { get; set; }
        

        public SelectList ItemPerPageList { get; set; }
        public int PageIndex { get; set; }
        public int ItemPerPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalEntities { get; set; }
        public int ActNumber { get; set; }
    }
}
