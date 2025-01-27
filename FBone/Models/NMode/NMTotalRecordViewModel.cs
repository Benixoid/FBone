using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FBone.Models.NMode
{
    public class NMTotalRecordViewModel
    {
        public NMTotalRecordViewModel() { }
        public NMTotalRecordViewModel(NMTotalRecord TRecord, bool HideInActiveRecords = false, DateTime? dt = null, string SearchText = "", bool Hide100PercentResults = false)
        {
            TotalRecord = TRecord;

            NModeRecords = HideInActiveRecords ? TRecord.NMRecords.Where(r => r.CountIt).ToList() : TRecord.NMRecords;
            NModeRecords = Hide100PercentResults ? NModeRecords.Where(r => r.NormalTotal != 100).ToList() : NModeRecords;
            this.HideInActiveRecords = HideInActiveRecords;
            this.Hide100PercentResults = Hide100PercentResults;

            if (!string.IsNullOrEmpty(SearchText))
            {
                NModeRecords = NModeRecords.Where(rec => rec.Tagname.ToLower().Contains(SearchText.ToLower())).ToList();
            }
            Search = SearchText;

        }

        public NMTotalRecord TotalRecord { get; set; }
        public PageViewModel Page { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public List<NModeRecord> NModeRecords { get; set; }
        public DateTime SelectedDate { get; set; }
        public bool Calculated { get; set; }
        public bool AllowCalculate { get; set; } = true;
        public bool ResultDetails { get; set; }
        public bool HideDetails { get; set; }
        public bool HideInActiveRecords { get; set; }
        public bool Hide100PercentResults { get; set; }
        public string Search { get; set; }
        [Display(Name = "Validate tags")]
        public bool ValidateTags { get; set; }
        public bool ResultsAvailable { get; set; }
        public string SavedResult { get; set; }
        public string CalculatedResult { get; set; }
        public string SnapshotResult { get; set; }
        public bool Snapshot { get; set; }
    }
}
