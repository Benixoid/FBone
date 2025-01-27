using FBone.Database.Entities;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBone.Models.Audits
{
    public class AuditListModel
    {
        public List<Audit> AuditList { get; set; }
        public bool CanCreateAudit { get; set; }
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
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }

        public string DateFromS { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        public string DateToS { get; set; }
        public string SmartSearch { get; set; }
        public int SelectedAuditStatus { get; set; }
    }
}
