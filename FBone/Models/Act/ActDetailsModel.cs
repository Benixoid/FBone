using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Act
{
    public class ActDetailsModel
    {
        public int? Id { get; set; }
        public tAct Act { get; set; }
        public SelectList Areas { get; set; }
        public SelectList Positions { get; set; }
        public SelectList Approvers1 { get; set; }
        public SelectList Approvers2 { get; set; }
        public SelectList Approvers3 { get; set; }
        public SelectList Approvers4 { get; set; }
        public SelectList Approvers5 { get; set; }
        public SelectList Approvers6 { get; set; }
        public SelectList Approvers7 { get; set; }
        public SelectList ApproversAdd { get; set; }
        public string Lang { get; set; }
        public SelectList CauseList { get; set; }
        public SelectList ProtectList { get; set; }
        public SelectList ImpactList { get; set; }
        public string URL { get; set; }
        public SelectList Devices { get; set; }
        public bool canEdit { get; set; }
        public bool startApporval { get; set; }
        public List<ActHistory> ActHistories { get; set; }
        public bool CanApprove { get; set; }
        public Dictionary<int, bool> CanBeClosed { get; set; }
        public SelectList BulkUnit { get; set; }
        public SelectList BulkEquipment { get; set; }
        public SelectList CrewList { get; set; }
        public int PageIndex { get; set; }
        public int ItemPerPage { get; set; }
        public int SelectedFacilityId { get; set; }
        public int SelectedAreaId { get; set; }
        public int SelectedActStatus { get; set; }
        public int SelectedActType { get; set; }
        public string DateFrom { get; set; }
        //public DateTime DateFrom_d { get; set; }
        public string DateTo { get; set; }
        //public DateTime DateTo_d { get; set; }
        public string SmartSearch { get; set; }
        public List<tListValue> Users { get; set; }
        public int selecteduser { get; set; }
        public bool Type6Disabled { get; set; }
    }
}
