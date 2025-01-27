using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using FBone.Database;
using FBone.Models.NMode;
using FBone.Service.WriteToPI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Components
{
    public class NMTotalAverage : ViewComponent
    {
        private readonly DataManager DataManager;
        private readonly NMManager manager;
        public NMTotalAverage(DataManager dm)
        {
            DataManager = dm;
            manager = dm.NMManager;
        }
        public IViewComponentResult Invoke(int RecordId, bool Snapshot = false, bool LoadSubTotals = true, DateTime? StartTime = null, DateTime? EndTime = null)
        {
            var now = DateTime.Now;
            if (EndTime == null) EndTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            if (StartTime == null) StartTime = EndTime.Value.AddDays(-7);
            NMTotalAverageRecord model = null;
            if (RecordId > -1)
            {
                var TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == RecordId);
                if (TRecord != null)
                {
                    model = new NMTotalAverageRecord(TRecord, StartTime.Value, EndTime.Value, LoadSubTotals);
                    if (Snapshot)
                        model.LoadSnapshot();
                    else
                        model.LoadAverage(manager.Settings.PIServer);
                }
                else
                    return null;
            }
            else
            {
                NMTotalRecords records = new NMTotalRecords(manager.TotalRecords.Where(tr => tr.Parent == null));
                records.Sort();
                model = new NMTotalAverageRecord(records, StartTime.Value, EndTime.Value);
                if (Snapshot)
                    model.LoadSnapshot();
                else
                    model.LoadAverage(manager.Settings.PIServer);
            }
            model.Snapshot = Snapshot;
            return View(model);
        }

        //public void LoadAverage(NMTotalAverageRecord model)
        //{
        //    var TotalVal = PIFunctions.GetAverageValue(manager.Settings.PIServer, model.TotalRecord.Tagname, model.StartTime, model.EndTime);
        //    model.NormalTotal = TotalVal.DValue;

        //}
    }

    public class NMTotalAverageRecord
    {
        public NMTotalAverageRecord(NMTotalRecord record, DateTime dtS, DateTime dtE, bool LoadSubTotals = true)
        {
            TotalRecord = record;
            StartTime = dtS;
            EndTime = dtE;
            this.LoadSubTotals = LoadSubTotals;
            if (LoadSubTotals && TotalRecord.SubTotals != null && TotalRecord.SubTotals.Count > 0)
            {
                //SubTotals = new List<NMTotalAverageRecord>();
                TotalRecord.SubTotals.ForEach(tr =>
                {
                    NMTotalAverageRecord rec = new NMTotalAverageRecord(tr, dtS, dtE);
                    SubTotals.Add(rec);
                });
            }
        }

        public NMTotalAverageRecord(List<NMTotalRecord> records, DateTime dtS, DateTime dtE)
        {
            StartTime = dtS;
            EndTime = dtE;
            //SubTotals = new List<NMTotalAverageRecord>();
            LoadSubTotals = true;
            records.ForEach(tr =>
            {
                NMTotalAverageRecord rec = new NMTotalAverageRecord(tr, dtS, dtE, false);
                SubTotals.Add(rec);
            });
        }

        public void LoadAverage(string ServerName)
        {
            if (TotalRecord != null)
            {
                var dtS = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, 2, 0, 0);
                var dtE = EndTime;
                if (dtE.Hour == 0) dtE = dtE.AddHours(2);
                var TotalVal = PIFunctions.GetAverageValue(ServerName, TotalRecord.Tagname, dtS, dtE);
                NormalTotal = TotalVal.DValue;
            }
            if (LoadSubTotals && SubTotals != null) SubTotals.ForEach(rec => rec.LoadAverage(ServerName));
        }

        public void LoadSnapshot()
        {
            if (TotalRecord != null)
            {
                TotalRecord.Snapshot(StartTime);
                NormalTotal = TotalRecord.NormalTotalPercent;
            }
            if (LoadSubTotals && SubTotals != null) SubTotals.ForEach(rec => rec.LoadSnapshot());
        }


        public NMTotalRecord TotalRecord { get; set; }
        public int RecordId
        {
            get
            {
                if (TotalRecord == null) return -1;
                return TotalRecord.Id;
            }
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Double NormalTotal { get; set; }
        public bool Snapshot { get; set; }
        public bool LoadSubTotals { get; set; }
        public List<NMTotalAverageRecord> SubTotals { get; set; } = new List<NMTotalAverageRecord>();
    }
}
