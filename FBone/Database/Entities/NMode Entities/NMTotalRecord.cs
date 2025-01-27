using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.ExtendedProperties;
using FBone.Service.WriteToPI;
using Microsoft.AspNetCore.Mvc;
using PISDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace FBone.Models.NMode
{
    public class NMTotalRecord : IComparable<NMTotalRecord>, IdObject
    {
        public NMTotalRecord() { }
        public NMTotalRecord(IEnumerable<NModeRecord> records) { Records.AddRange(records); }
        public int Id { get; set; }
        public string Tagname { get; set; } //= null!;
        public int AreaId { get; set; }
        public int LcnId { get; set; }
        public virtual Area Area { get; set; } = null!;

        public virtual Lcn Lcn { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual List<NMTotalRecord> SubTotals { get; set; } = new List<NMTotalRecord>();
#nullable enable
        public virtual NMTotalRecord? Parent { get; set; }
#nullable disable

        [NotMapped]
        public string ShortName
        {
            get
            {
                return this.ToString2();
            }
        }

        [NotMapped]
        public double DayNormalTotal { get; set; }
        [NotMapped]
        public double DayNormalTotalPercent
        {
            get
            {
                return DayNormalTotal == 0 ? 0 : (double)DayNormalTotal * 100 / RecordsCount;
            }
        }
        [NotMapped]
        public double NightNormalTotal { get; set; }
        [NotMapped]
        public double NightNormalTotalPercent
        {
            get
            {
                return NightNormalTotal == 0 ? 0 : (double)NightNormalTotal * 100 / RecordsCount;
            }
        }
        internal double _NormalTotalPercent = double.MinValue;
        [NotMapped]
        [DisplayName("NormalTotal % Calc")]
        public double NormalTotalPercent
        {
            get
            {
                if (_NormalTotalPercent == double.MinValue)
                {
                    var T = DayNormalTotalPercent + NightNormalTotalPercent;
                    _NormalTotalPercent = T / 2;
                }
                return _NormalTotalPercent;
            }
        }
        [NotMapped]
        [DisplayName("NormalTotal %")]
        public double NormalTotalPercentSaved { get; set; }

        private int _RecordsCount = int.MinValue;
        [NotMapped]
        public int RecordsCount
        {
            get
            {
                if (_RecordsCount == int.MinValue)
                {
                    _RecordsCount = Records.Where(r => r.CountIt).Count();
                    SubTotals.ForEach(st => _RecordsCount += st.RecordsCount);
                }
                return _RecordsCount;
            }
        }
        public NModeRecords GetAllRecords()
        {
            NModeRecords allRecords = new NModeRecords();
            allRecords.AddRange(Records);
            SubTotals.ForEach(st => allRecords.AddRange(st.GetAllRecords()));
            return allRecords;
        }
        [NotMapped]
        public string RecordCountString
        {
            get
            {
                NModeRecords allRecords = GetAllRecords ();
                var iTotal = allRecords.Count;
                var iActive = allRecords.Count(r => r.CountIt);
                var iInActive = iTotal - iActive;
                return $"{iTotal}: +{iActive}; -{iInActive}";
            }
        }
        private NModeRecords Records = new NModeRecords();
        [NotMapped]
        public NModeRecords NMRecords
        {
            get { return Records; }
            internal set { Records = value; }
        }

        private NModeRecords SubRecords = null;
        [NotMapped]
        public NModeRecords SubNMRecords
        {
            get
            {
                if (SubRecords == null)
                {
                    SubRecords = new NModeRecords();
                    if (this.SubTotals.Count > 0)
                        this.SubTotals.ForEach(st =>
                        {
                            SubRecords.AddRange(st.NMRecords);
                            SubRecords.AddRange(st.SubNMRecords);
                        });
                }
                return SubRecords;
            }
        }

        private List<TagValue> _TagValues = null;// new List<TagValue>();
        [NotMapped]
        public List<TagValue> TagValues
        {
            get
            {
                return _TagValues;
            }
        }
        [NotMapped]
        internal DateTime SelectedDate { get; set; }
        [NotMapped]
        internal bool Calculated { get; set; }
        [NotMapped]
        internal bool Snapshoted { get; set; }
        public void LoadData(DateTime Day, NMManager manager, bool LoadRecordResults, bool LoadPIData = true, bool LoadSubtotalResults = true)
        {
            if (LoadPIData)
            {
                DateTime dtReport = manager.Settings.ReportTimeStamp(Day);
                var TotalVal = PIFunctions.GetRecordedValue(Settings.PIServer, Tagname, dtReport);
                NormalTotalPercentSaved = TotalVal.DValue;
                //_NormalTotalPercent = TotalVal.DValue;
            }
            SubTotals.ForEach(st => st.LoadData(Day, manager, LoadSubtotalResults, LoadPIData, LoadSubtotalResults));
            if (LoadRecordResults && manager != null)
            {
                Records.ForEach(r =>
                {
                    if (r.Results.Count(rr => rr.TimeStamp == Day) == 0)
                    {
                        NModeResult res = manager.GetRecordResult(r.Id, Day);
                        if (res != null && r.Results.Count(rr => rr.TimeStamp == res.TimeStamp) == 0)
                        {
                            r.Results.Add(res);
                        }
                    }
                });
            }
        }
        public double GetSavedData(DateTime Day)
        {
            return PIFunctions.GetRecordedValue(Settings.PIServer, Tagname, Day).DValue;
        }

        public bool IsPointExists()
        {
            if (string.IsNullOrEmpty(Tagname)) return false;
            return PIFunctions.IsPointExist(Settings.PIServer, Tagname);
        }
        public Dictionary<string, object> GetAttributeValues(IEnumerable<string> AttributeNames)
        {
            Dictionary<string, object> AttributeValues = new Dictionary<string, object>();

            PIPoint pt = PIFunctions.GetPIPoint(Settings.PIServer, Tagname, true);
            foreach (string AttributeName in AttributeNames)
            {
                var val = pt.PointAttributes[AttributeName].Value;
                AttributeValues[AttributeName] = val;
            }
            return AttributeValues;
        }
        [NotMapped]
        public TagHealthStatus TagHealthStatus { get; private set; }
        public TagHealthStatus ValidateTag()
        {
            if (!IsPointExists())
            {
                TagHealthStatus = TagHealthStatus.TagNotExists;
            }
            else
            {
                var avalues = GetAttributeValues(new[] { "scan", "pointsource" });
                if ((Int16)avalues["scan"] == 0)
                    TagHealthStatus = TagHealthStatus.ScanOff;
                else if (((string)avalues["pointsource"]).Contains("_BAD"))
                    TagHealthStatus = TagHealthStatus.BadPointsource;
                else
                    TagHealthStatus = TagHealthStatus.Good;
            }
            return TagHealthStatus;
        }

        public void LoadHistoryData(DateTime Day)
        {
            _TagValues = PIFunctions.GetRecordedValues(Settings.PIServer, Tagname, Day.AddDays(-Settings.TotalHistoryDays), Day);
        }
        public void Calculate(DateTime Day, string Writer = "")
        {
            int dn = 0, nn = 0;
            if (SubTotals.Count > 0)
            {
                foreach (NMTotalRecord st in SubTotals)
                {
                    st.Calculate(Day, Writer);
                    dn += st.Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count();
                    nn += st.Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count();
                }
            }
            NMRecords.ForEach(record => { if (record.CountIt) record.Calculate(Day, Writer); });
            DayNormalTotal = Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count() + dn;
            NightNormalTotal = Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count() + nn;
            _NormalTotalPercent = double.MinValue;
            SelectedDate = Day;
            Calculated = true;
        }

        public void Snapshot(DateTime Day, string Writer = "")
        {
            int dn = 0, nn = 0;
            if (SubTotals.Count > 0)
            {
                foreach (NMTotalRecord st in SubTotals)
                {
                    st.Snapshot(Day, Writer);
                    dn += st.Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count();
                    nn += st.Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count();
                }
            }
            NMRecords.ForEach(record => { if (record.CountIt) record.Snapshot(Day, Writer); });
            DayNormalTotal = Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count() + dn;
            NightNormalTotal = Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count() + nn;
            _NormalTotalPercent = double.MinValue;
            SelectedDate = Day;
            Calculated = true;
            Snapshoted = true;
        }

        public bool ResultsAvailable(DateTime dt, NMManager manager = null)
        {
            DateTime dtReport = dt;
            bool isAvailable = false;
            if (manager != null) dtReport = manager.Settings.ReportTimeStamp(dt);
            //count of records with corresponding result on that day
            var ResultCount = Records.Where(r => r.CountIt && r.Results.Where(rr => rr.TimeStamp == dtReport).Count() == 1).Count();
            if (Records.Count != 0 && ResultCount == Records.Where(r => r.CountIt).Count()) // If results available for all records
                isAvailable = true;

            SubTotals.ForEach(st => isAvailable |= st.ResultsAvailable(dtReport));

            return isAvailable;
        }
        public void SummarizeResult()
        {
            int dn = 0, nn = 0;
            if (SubTotals.Count > 0)
            {
                foreach (NMTotalRecord st in SubTotals)
                {
                    st.SummarizeResult();
                    dn += st.Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count();
                    nn += st.Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count();
                }
            }

            DayNormalTotal = Records.Where(record => record.CountIt && (record.DayNormal > Settings.NMODEThreshold)).Count() + dn;
            NightNormalTotal = Records.Where(record => record.CountIt && (record.NightNormal > Settings.NMODEThreshold)).Count() + nn;
            _NormalTotalPercent = double.MinValue;
        }

        public bool SaveToPI(DateTime dt)
        {
            if (dt == DateTime.MinValue) return false;
            bool result = false;
            SubTotals.ForEach(st =>
            {
                //if (st.SelectedDate != SelectedDate) st.SelectedDate = SelectedDate;
                result &= st.SaveToPI(dt);
            });

            TagValue val = new TagValue() { TimeStamp = dt, Value = Math.Round(NormalTotalPercent, 2) };

            result &= PIFunctions.SaveToPI(Settings.PIServer, Tagname, val);
            return result;
        }

        NModeSettings _settings = null;
        [NotMapped]
        public virtual NModeSettings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                Records.ForEach(r => r.Settings = _settings);
            }
        }

        public List<NMTotalRecord> ListOfHeirs(List<NMTotalRecord> list = null)
        {
            if (list == null) list = new List<NMTotalRecord>();
            if (SubTotals == null || SubTotals.Count == 0) return list;
            list.AddRange(SubTotals);
            foreach (NMTotalRecord subrecord in SubTotals)
            {
                subrecord.ListOfHeirs(list);
            }

            return list;
        }
        public override string ToString()
        {
            return $"Area: {Area},LCN:{Lcn},DNT:{DayNormalTotal},DNTP:{DayNormalTotalPercent:f2} %,NNT:{NightNormalTotal},NNTP:{NightNormalTotalPercent:f2} %,NTP:{NormalTotalPercent:f2} %";
        }

        public string ToString2()
        {
            return $"Area: {Area}, Lcn: {Lcn} - Tagname: {Tagname}";
        }
        public int CompareTo(NMTotalRecord other)
        {
            return string.Compare($"{Area?.Name}.{Lcn?.Name}", $"{other.Area?.Name}.{other.Lcn?.Name}");
        }
    }
    public class NMTotalRecords : List<NMTotalRecord>
    {
        public NMTotalRecords() { }
        public NMTotalRecords(IEnumerable<NMTotalRecord> records) { base.AddRange(records); }

        public void Calculate(DateTime Day)
        {
            ForEach(record => { record.Calculate(Day); });
            SelectedDate = Day; Calculated = true;
        }

        public DateTime SelectedDate { get; set; }
        public bool Calculated { get; set; }

        public bool ResultsAvailable { get; set; }

        public bool SetResultAvailability(DateTime dt, NMManager manager = null)
        {
            DateTime dtReport = dt;
            bool isAvailable = false;
            if (manager != null) dtReport = manager.Settings.ReportTimeStamp(dt);

            isAvailable = this.Any(r => r.ResultsAvailable(dtReport));
            //this.ForEach(r => isAvailable &= r.ResultsAvailable(dtReport));
            ResultsAvailable = isAvailable;

            return isAvailable;
        }

    }
    //public class Totals
    //{
    //    public List<NMTotalRecord> Records { get; set; }
    //    public void Calculate(DateTime Day)
    //    {
    //        Records.ForEach(record => { record.Calculate(Day); });
    //        SelectedDate = Day; Calculated = true;
    //    }
    //    public DateTime SelectedDate { get; set; }
    //    public bool Calculated { get; set; }

    //    public List<double> NormalTotals
    //    {
    //        get
    //        {
    //            return Records.Select(rec => rec.NormalTotalPercent).ToList();
    //        }
    //    }
    //}

    //public class NMTotalProxy
    //{
    //    public NMTotalProxy() { }
    //    public NMTotalProxy(NMTotalRecord Trec)
    //    {
    //        _record = Trec;
    //        Id = _record.Id;
    //        Area = Trec.Area.Name;
    //        Lcn = Trec.Lcn.Name;
    //        Tagname = Trec.Tagname;
    //        DayNormalTotalPercent = Trec.DayNormalTotalPercent;
    //        NightNormalTotalPercent = Trec.NightNormalTotalPercent;
    //        NormalTotalPercent = Trec.NormalTotalPercent;
    //    }
    //    public int Id
    //    {
    //        get;
    //        private set;
    //    }
    //    public string Area
    //    {
    //        get;
    //        private set;
    //    }
    //    public string Lcn { get; private set; }
    //    public string Tagname { get; private set; }
    //    public double DayNormalTotalPercent { get; private set; }
    //    public double NightNormalTotalPercent { get; private set; }
    //    public double NormalTotalPercent { get; private set; }
    //    private NMTotalRecord _record;
    //    public NMTotalRecord Record
    //    {
    //        get
    //        {
    //            return _record;
    //        }
    //    }
    //    //public int? Id
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.Id;
    //    //    }
    //    //}
    //    //public string Area
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.Area.Name;
    //    //    }
    //    //}
    //    //public string Lcn
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.Lcn.Name;
    //    //    }
    //    //}
    //    //public string Tagname
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.Tagname;
    //    //    }
    //    //}
    //    //public double? DayNormalTotalPercent
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.DayNormalTotalPercent;
    //    //    }
    //    //}
    //    //public double? NightNormalTotalPercent
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.NightNormalTotalPercent;
    //    //    }
    //    //}
    //    //public double? NormalTotalPercent
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.NormalTotalPercent;
    //    //    }
    //    //}
    //    //public DateTime? SelectedDate
    //    //{
    //    //    get
    //    //    {
    //    //        return _record?.SelectedDate;
    //    //    }
    //    //}


    //}
    //public class NMTotalProxyList : List<NMTotalProxy>
    //{
    //    public NMTotalProxyList() { }
    //    public NMTotalProxyList(NMTotalRecords TRecords)
    //    {
    //        TRecords.ForEach(rec =>
    //        {
    //            NMTotalProxy proxy = new NMTotalProxy(rec);
    //            this.Add(proxy);
    //        });
    //        SelectedDate = TRecords.SelectedDate;
    //        Calculated = TRecords.Calculated;
    //    }
    //    public void Calculate(DateTime Day)
    //    {
    //        ForEach(proxy => { proxy.Record.Calculate(Day); });
    //        SelectedDate = Day; Calculated = true;
    //    }

    //    public DateTime SelectedDate { get; set; }
    //    public bool Calculated { get; set; }

    //}

}
