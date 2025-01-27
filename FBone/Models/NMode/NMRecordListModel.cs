using DocumentFormat.OpenXml.ExtendedProperties;
using FBone.Database;
using FBone.Database.Entities;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FBone.Models.NMode
{
    public class NMRecordListModel
    {
        //public NMRecordListModel()
        //{ }
        public NMRecordListModel(NMTotalRecordViewModel trvm, int AId, int LId)
        {
            NModeRecords = trvm.NModeRecords;
            Search = trvm.Search;
            SortViewModel = trvm.SortViewModel;
            Page = trvm.Page;
            ValidateTags = trvm.ValidateTags;
            ResultDetails = trvm.ResultDetails;
            HideDetails = trvm.HideDetails;
            HideInActiveRecords = trvm.HideInActiveRecords;
            Hide100PercentResults = trvm.Hide100PercentResults;
            Calculated = trvm.Calculated;
            Snapshot = trvm.Snapshot;
            AllowCalculate = trvm.AllowCalculate;
            SelectedDate = trvm.SelectedDate;
            AreaId = AId;
            LcnId = LId;
            ResultsAvailable = trvm.ResultsAvailable;
            ParentId = trvm.TotalRecord?.Id ?? 0;
        }
        public NMRecordListModel(NMManager mngr, int AreaId, int LcnId, DateTime dt, bool HideInActiveRecords = false, string SearchText = "", bool Hide100PercentResults = false)
        {
            if (dt == DateTime.MinValue) dt = DateTime.Now;
            if (!string.IsNullOrEmpty(SearchText))
            {
                NModeRecords = mngr.GetNModeRecords(dt).Where(rec => rec.Tagname.ToLower().Contains(SearchText.ToLower())).ToList();
            }
            else
            {
                NModeRecords = mngr.GetNModeRecords(dt);
            }
            this.HideInActiveRecords = HideInActiveRecords;
            NModeRecords = HideInActiveRecords ? NModeRecords.Where(r => r.CountIt).ToList() : NModeRecords;
            this.Hide100PercentResults = Hide100PercentResults;
            NModeRecords = Hide100PercentResults ? NModeRecords.Where(r => r.NormalTotal != 100).ToList() : NModeRecords;

            Search = SearchText;

            var area = mngr.Areas.FirstOrDefault(a => a.Id == AreaId);
            this.AreaId = area == null ? 0 : AreaId;
            if (AreaId > 0)
                NModeRecords = NModeRecords.Where(rec => rec.Area.Name == area?.Name).ToList();

            var areas = NModeRecords.Select(rec => rec.Area).Distinct().ToList();
            areas.Insert(0, new Area() { Id = 0, Name = "All" });
            Areas = new SelectList(areas, "Id", "Name", AreaId);

            List<Lcn> lcns = null;
            lcns = NModeRecords.Select(rec => rec.Lcn).Distinct().ToList();
            lcns.Sort();
            lcns.Insert(0, new Lcn() { Id = 0, Name = "All" });

            var lcn = lcns.FirstOrDefault(a => a.Id == LcnId);
            LCNs = new SelectList(lcns, "Id", "Name", LcnId);
            this.LcnId = lcn == null ? 0 : LcnId;
            if (LcnId > 0)
                if (NModeRecords.Count(rec => rec.Lcn.Name == lcn?.Name) > 0)
                    NModeRecords = NModeRecords.Where(rec => rec.Lcn.Name == lcn?.Name).ToList();
                else
                    NModeRecords = new List<NModeRecord>();
        }

        public SelectList Areas { get; set; }
        public int AreaId { get; set; }
        public SelectList LCNs { get; set; }
        public int LcnId { get; set; }
        public PageViewModel Page { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public List<NModeRecord> NModeRecords { get; set; }
        public DateTime SelectedDate { get; set; }
        public bool Calculated { get; set; }
        public bool AllowCalculate { get; set; }
        public string Search { get; set; }

        [Display(Name = "Validate tags")]
        public bool ValidateTags { get; set; }
        public bool ResultDetails { get; set; }
        public bool HideDetails { get; set; }
        public bool HideInActiveRecords { get; set; }
        public bool Hide100PercentResults { get; set; }
        public bool ResultsAvailable { get; set; }
        public int ParentId { get; set; }
        public bool Snapshot { get; set; }
        //public string ModelName
        //{
        //    get
        //    {
        //        var sName = "";
        //        if (NModeRecords != null && NModeRecords.Count > 0)
        //        {
        //            if (NModeRecords.DistinctBy(r => r.ParentId).Count() > 0)
        //            {
        //                sName=NModeRecords[0].P
        //            }

        //            if (NModeRecords.DistinctBy(r => r.AreaId).Count() > 0)
        //            {

        //            }
        //            if (NModeRecords.DistinctBy(r => r.LcnId).Count() > 0)
        //            {

        //            }

        //        }
        //        return sName;
        //    }
        //}
    }

    public class IdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class FilteredRecordsModel
    {
        public FilteredRecordsModel(List<NModeRecord> records, int Area)
        {
            Records = records;// new SelectList(records, "Id", "Tagname");
            SelectedArea = Area;
        }
        public List<NModeRecord> Records { get; }
        public int SelectedArea { get; }
    }

    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int PageSize { get; }
        public int RecordCount { get; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int StartRecord
        {
            get
            {
                return (PageNumber - 1) * PageSize + 1;
            }
        }
        public int EndRecord
        {
            get
            {
                int iend = PageNumber * PageSize;
                return HasNextPage ? iend : RecordCount;
            }
        }
        public NMRecordListModel NMRecordListModel { get; set; }
        public PageViewModel(int count, int pageNumber, int pageSize)//, NMRecordListModel model)
        {
            RecordCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            //NMRecordListModel = model;

            PageSizeList = new SelectList(new[] { new { Id = 0, Name = "All" }, new { Id = 5, Name = "5" }, new { Id = 15, Name = "15" }, new { Id = 25, Name = "25" }, new { Id = 50, Name = "50" }, new { Id = 100, Name = "100" } }, "Id", "Name", PageSize);
        }
        //public int PageSizeId { get; set; }
        public SelectList PageSizeList { get; set; }

    }

    public class SortViewModel
    {
        public SortState TagnameSort { get; }
        public SortState AreaSort { get; }
        public SortState LcnSort { get; }
        public SortState NormalTotalSort { get; }
        public SortState ConditionsSort { get; }
        public SortState EditorSort { get; }
        public SortState ChangeDateSort { get; }

        public SortState Current { get; }

        public SortViewModel(SortState? sortOrder)
        {
            if (sortOrder == null) sortOrder = SortState.TagnameAsc;
            TagnameSort = sortOrder == SortState.TagnameAsc ? SortState.TagnameDesc : SortState.TagnameAsc;
            AreaSort = sortOrder == SortState.AreaAsc ? SortState.AreaDesc : SortState.AreaAsc;
            LcnSort = sortOrder == SortState.LcnAsc ? SortState.LcnDesc : SortState.LcnAsc;
            NormalTotalSort = sortOrder == SortState.NormalTotalAsc ? SortState.NormalTotalDesc : SortState.NormalTotalAsc;
            ConditionsSort = sortOrder == SortState.ConditionsAsc ? SortState.ConditionsDesc : SortState.ConditionsAsc;
            EditorSort = sortOrder == SortState.EditorAsc ? SortState.EditorDesc : SortState.EditorAsc;
            ChangeDateSort = sortOrder == SortState.ChangeDateAsc ? SortState.ChangeDateDesc : SortState.ChangeDateAsc;
            Current = sortOrder.Value;
        }
    }

    public class ConditionEditViewModel
    {
        public NModeCondition Condition { get; set; }
        public List<NModeRecord> Records { get; set; }
        //public string Comment { get;set; }
    }

    public enum SortState
    {
        Unsorted,
        TagnameAsc,
        TagnameDesc,
        AreaAsc,
        AreaDesc,
        LcnAsc,
        LcnDesc,
        NormalTotalAsc,
        NormalTotalDesc,
        ConditionsAsc,
        ConditionsDesc,
        EditorAsc,
        EditorDesc,
        ChangeDateAsc,
        ChangeDateDesc
    }

    public enum NModeState
    {
        MAN, AUTO, CAS, BCAS, NONE
    }
}
