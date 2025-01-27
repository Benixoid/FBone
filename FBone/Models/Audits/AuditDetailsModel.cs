﻿using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Models.Audits
{
    public class AuditDetailsModel
    {
        //public int? Id { get; set; }
        public Audit Audit { get; set; }
        public SelectList Areas { get; set; }
        public SelectList Facilities { get; set; }
        public string URL { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public bool CanEdit { get; set; }
        public int PageIndex { get; set; }
        public int ItemPerPage { get; set; }
        public int SelectedFacilityId { get; set; }
        public int SelectedAreaId { get; set; }
        public int SelectedAuditStatus { get; set; }        
        public string DateFrom { get; set; }        
        public string DateTo { get; set; }        
        public string SmartSearch { get; set; }
        public bool startApproval { get; set; }
        public SelectList Acts { get; set; }
        public SelectList Tags { get; set; }
        public SelectList Positions { get; set; }
        public string UsersInPosition { get; set; }
    }
}
