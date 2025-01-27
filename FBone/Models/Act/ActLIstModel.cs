using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Models.Act
{
    public class ActListModel
    {
        public List<ActGridRecord> ActList { get; set; }
        public bool CanCreateAct { get; set; }
        public SelectList ItemPerPageList { get; set; }
        public int PageIndex { get; set; }
        public int ItemPerPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalEntities { get; set; }

        public SelectList Facilities { get; set; }
        public int SelectedFacilityId { get; set; }

        public SelectList Areas { get; set; }
        public int SelectedAreaId { get; set; }

        public int SelectedActType { get; set; }
        public int SelectedActStatus { get; set; }

        [DataType(DataType.Date)]        
        public DateTime DateFrom { get; set; }

        public string DateFromS { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        public string DateToS { get; set; }

        public bool FlagNotClosed { get; set; }

        public string SmartSearch { get; set; }
        
        //public string Date1 { get; set; }
        //public string Date2 { get; set; }

    }
}
