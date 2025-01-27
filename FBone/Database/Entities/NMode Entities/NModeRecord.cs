using FBone.Migrations;
using FBone.Service.WriteToPI;
using PISDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Claims;

namespace FBone.Models.NMode;

public class NModeRecord : IdObject
{
    public NModeRecord() { }
    public int Id { get; set; }

    //public string Lcn { get; set; } = null!;
    public int AreaId { get; set; }

    public int LcnId { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual Lcn Lcn { get; set; } = null!;
    //public string Area { get; set; }

    public string Tagname { get; set; }

    public string Descriptor { get; set; }

    private string _Nmode = string.Empty;
    public string Nmode
    {
        get
        {
            return _Nmode;
        }
        set
        {
            _Nmode = value;
            if (Settings != null)
                iNMODE = Settings.States.IndexOf(_Nmode);
        }
    }
    [DefaultValue("=")]
    public string Operator { get; set; }
    [DefaultValue(true)]
    [DisplayName("Enabled")]
    public bool CountIt { get; set; }

    [DefaultValue(true)]
    [DisplayName("Conditions are ORed")]
    public bool ConditionORed { get; set; } = true;

    [DefaultValue(true)]
    [DisplayName("NMode is ORed")]
    public bool NModeORed { get; set; } = true;

    public string Creator { get; set; }
    public string Editor { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ChangeDate { get; set; }
    public string TagnameNmode { get { return $"{Tagname}{Operator}{Nmode}"; } }
    public virtual NModeConditions Conditions { get; set; } = new NModeConditions();// new List<NModeCondition>(); //ICollection<NModeCondition>
    public override string ToString()
    {
        return $"Id:{Id},Area{Area},LCN:{Lcn},Tagname: {Tagname}, NMode: {Nmode}";
    }
    public string ConditionToString()
    {
        string Rcondition = $"{Tagname} {Operator} {Nmode}";// + (CountIt ? " (+)" : " (-)");
        if (Conditions == null || Conditions.Count == 0)
            return Rcondition + (CountIt ? " (+)" : " (-)");
        else
        {
            string conditions = string.Empty;
            if (NModeORed)
            {
                Rcondition += " OR " + (Conditions.Count > 1 ? "(" : "");
                for (int indx = 0; indx < Conditions.Count; indx++)
                {
                    conditions += (indx > 0 ? (ConditionORed ? " OR " : " AND ") : "") + Conditions[indx].ToString();
                }
            }
            else
            {
                Rcondition += " AND " + (Conditions.Count > 1 ? "(" : ""); ;
                for (int indx = 0; indx < Conditions.Count; indx++)
                {
                    conditions += (indx > 0 ? (ConditionORed ? " OR " : " AND ") : "") + Conditions[indx].ToString();
                }
            }
            return Rcondition + conditions + (Conditions.Count > 1 ? ")" : "") + (CountIt ? " (+)" : " (-)");
        }
    }

    public virtual List<NModeResult> Results { get; set; } = new List<NModeResult>();
    public virtual List<NModeChangeRecord> Changes { get; set; } = new List<NModeChangeRecord>();


    #region Not Mapped
    private object PropGetter(string PropName)
    {
        NModeResult res = null;
        if (SelectedDate == DateTime.MinValue)
            if (Results == null || Results.Count() == 0)
            {
                switch (PropName)
                {
                    case "TimeStamp":
                    case "RecordTime":
                        return DateTime.MinValue;
                    case "Evaluation":
                        return false;
                    case "User":
                        return string.Empty;
                    default:
                        return (double)0;
                }
            }
            else
            {
                res = Results.OrderByDescending(rr => rr.TimeStamp).Take(1).ToList()[0];
            }
        else
        {
            res = Results.Where(rr => rr.TimeStamp <= _settings.ReportTimeStamp(SelectedDate)).OrderByDescending(rr => rr.TimeStamp).Take(1).ToList()[0];
        }
        switch (PropName)
        {
            case "DayNormal":
                return res.DayNormal;
            case "NightNormal":
                return res.NightNormal;
            case "DayManual":
                return res.DayManual;
            case "NightManual":
                return res.NightManual;
            case "DayOther":
                return res.DayOther;
            case "NightOther":
                return res.NightOther;
            case "TimeStamp":
                if (Snapshoted) return SelectedDate;
                return res.TimeStamp;
            case "RecordTime":
                if (Snapshoted) return SelectedDate;
                return res.RecordTime;
            case "User":
                if (Snapshoted) return Writer;
                return res.User;
            case "Evaluation":
                return res.Evaluation;
            default:
                return 0;
        }
    }

    [NotMapped]
    public string Writer { get; set; }
    private void PropSetter(string PropName, double value)
    {
        string userName = Writer;
        bool valueChanged = false;
        if (string.IsNullOrEmpty(userName))
        {
#pragma warning disable CA1416
            userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
#pragma warning restore CA1416
        }

        if (Results == null)
        {
            Results = new List<NModeResult>();
            Results.Add(new NModeResult() { TimeStamp = SelectedDate, DayNormal = value, User = userName });
        }
        else
        {
            NModeResult res = Results.Where(rr => rr.TimeStamp == _settings.ReportTimeStamp(SelectedDate)).FirstOrDefault();

            if (res == null)
            {
                res = new NModeResult()
                {
                    TimeStamp = _settings.ReportTimeStamp(SelectedDate),
                    RecordTime = DateTime.Now,
                    User = userName
                };
                Results.Add(res);
            }
            switch (PropName)
            {
                case "DayNormal":
                    if (res.DayNormal != value)
                    {
                        res.DayNormal = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
                case "NightNormal":
                    if (res.NightNormal != value)
                    {
                        res.NightNormal = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
                case "DayManual":
                    if (res.DayManual != value)
                    {
                        res.DayManual = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
                case "NightManual":
                    if (res.NightManual != value)
                    {
                        res.NightManual = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
                case "DayOther":
                    if (res.DayOther != value)
                    {
                        res.DayOther = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
                case "NightOther":
                    if (res.NightOther != value)
                    {
                        res.NightOther = value;
                        res.RecordTime = DateTime.Now;
                        valueChanged = true;
                    }
                    break;
            }
            if (valueChanged) res.User = userName;
        }
    }
    [NotMapped]
    public int ParentId { get; set; }

    [NotMapped]
    [DisplayName("TimeStamp")]
    public DateTime TimeStamp
    {
        get
        {
            return (DateTime)PropGetter("TimeStamp");
        }
    }
    [NotMapped]
    [DisplayName("Recorded Time")]
    public DateTime RecordTime
    {
        get
        {
            return (DateTime)PropGetter("RecordTime");
        }
    }
    [NotMapped]
    public bool Evaluation
    {
        get
        {
            return (bool)PropGetter("Evaluation");
        }
    }
    private NModeResult MRResult = null;
    [NotMapped]
    public NModeResult MostRecentResult
    {
        get
        {
            if (MRResult == null)
            {
                if (SelectedDate == DateTime.MinValue)
                    if (Results == null || Results.Count() == 0)
                    {
                        MRResult = null;
                    }
                    else
                    {
                        MRResult = Results.OrderByDescending(rr => rr.TimeStamp).Take(1).ToList()[0];
                    }
                else
                {
                    MRResult = Results.Where(rr => rr.TimeStamp <= SelectedDate).OrderByDescending(rr => rr.TimeStamp).Take(1).ToList()[0];
                }
            }
            return MRResult;
        }
    }
    [NotMapped]
    [DisplayName("Writer")]
    public string User
    {
        get
        {
            return (string)PropGetter("User");
        }
    }
    [NotMapped]
    public double NormalTotal
    {
        get
        {
            return (DayNormal + NightNormal) / 2;
        }
    }
    [NotMapped]
    [DisplayName("Day Normal")]
    public double DayNormal
    {
        get
        {
            return (double)PropGetter("DayNormal");
        }
        set
        {
            PropSetter("DayNormal", value);
        }
    }
    [NotMapped]
    public int iNMODE { get; private set; }
    [NotMapped]
    [DisplayName("Day Manual")]
    public double DayManual
    {
        get
        {
            return (double)PropGetter("DayManual");
        }
        set
        {
            PropSetter("DayManual", value);
        }
    }
    [NotMapped]
    [DisplayName("Day Other")]
    public double DayOther
    {
        get
        {
            return (double)PropGetter("DayOther");
        }
        set
        {
            PropSetter("DayOther", value);
        }
    }
    [NotMapped]
    [DisplayName("Night Normal")]
    public double NightNormal
    {
        get
        {
            return (double)PropGetter("NightNormal");
        }
        set
        {
            PropSetter("NightNormal", value);
        }
    }
    [NotMapped]
    [DisplayName("Night Manual")]
    public double NightManual
    {
        get
        {
            return (double)PropGetter("NightManual");
        }
        set
        {
            PropSetter("NightManual", value);
        }
    }
    [NotMapped]
    [DisplayName("Night Other")]
    public double NightOther
    {
        get
        {
            return (double)PropGetter("NightOther");
        }
        set
        {
            PropSetter("NightOther", value);
        }
    }
    NModeSettings _settings;
    [NotMapped]
    internal virtual NModeSettings Settings
    {
        get
        {
            return _settings;
        }
        set
        {
            _settings = value;
            if (Settings != null && Settings.States.Contains(Nmode))
            {
                iNMODE = Settings.States.IndexOf(Nmode);
            }
        }
    }
    [NotMapped]
    internal bool Calculated { get; set; }
    [NotMapped]
    internal bool Snapshoted { get; set; }
    [NotMapped]
    internal DateTime SelectedDate { get; set; }
    [NotMapped]
    internal Dictionary<string, List<TagValue>> AllConditionsValues { get; set; }
    [NotMapped]
    internal List<string> NormalIntervals { get; set; }

    [NotMapped]
    internal TagHealthStatus TagHealthStatus { get; private set; } = TagHealthStatus.Unchecked;
    #endregion
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
    public void Calculate(DateTime? dt = null, string Writer = "")
    {
        this.Writer = Writer;
        Double _DayNormal = 0; Double _DayManual = 0; Double _DayOther = 0; Double _NightNormal = 0; Double _NightManual = 0; Double _NightOther = 0;

        SelectedDate = dt.Value;
        DateTime now = DateTime.Now;
        if (dt == null)
        {
            dt = DateTime.Now.AddDays(-1);
        }
        DateTime StartDTime = dt.Value;
        StartDTime = new DateTime(StartDTime.Year, StartDTime.Month, StartDTime.Day, Area.StartingHour, 0, 0);
        if (StartDTime > now) StartDTime = now;

        DateTime EndDTime = Area.SplitToShift ? StartDTime.AddHours(12) : StartDTime.AddHours(24);
        DateTime EndNTime;
        if (EndDTime > now)
        {
            EndDTime = now;
            EndNTime = now;
        }
        else
        {
            EndNTime = Area.SplitToShift ? EndDTime.AddHours(12) : EndDTime;
            if (EndNTime > now) EndNTime = now;
        }

        double factor = 100 / (double)Area.InterpolatedValuesCount;

        int iManMode = Settings.States.IndexOf(Settings.ManualState);
        string ManMode = Settings.ManualState;
        List<TagValue> values = null;

        if (Settings.NONEasNormal && Nmode == "NONE")
        {
            _DayNormal = 100;
            _NightNormal = 100;
            Calculated = true;
        }
        else
        {
            try
            {
                TagValue value = null;
                NormalIntervals = new List<string>();
                Dictionary<string, List<TagValue>> ConditionsValues = new Dictionary<string, List<TagValue>>();
                values = PIFunctions.GetInterpolatedValuesByCount(Settings.PIServer, Tagname.Trim(), StartDTime, EndDTime, Area.InterpolatedValuesCount);
                ConditionsValues.Add(Tagname, values);
                foreach (NModeCondition cond in Conditions)
                {
                    if (!ConditionsValues.ContainsKey(cond.Tagname))
                        ConditionsValues.Add(cond.Tagname, PIFunctions.GetInterpolatedValuesByCount(Settings.PIServer, cond.Tagname.Trim(), StartDTime, EndDTime, Area.InterpolatedValuesCount));
                }
                for (int i = 0; i < values.Count; i++)
                {
                    value = values[i];

                    int istate = (int)value.DValue;
                    if (IsNormal(ConditionsValues, i))
                    {
                        _DayNormal += 1;
                        NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Normal");
                    }
                    else
                    {
                        if (istate == iManMode)
                        {
                            _DayManual += 1;
                            NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Manual");
                        }
                        else
                        {
                            _DayOther += 1;
                            NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Other");
                        }
                    }
                }
                _DayNormal *= factor;
                _DayManual *= factor;
                _DayOther *= factor;

                AllConditionsValues = new Dictionary<string, List<TagValue>>();
                foreach (string key in ConditionsValues.Keys)
                {
                    AllConditionsValues[key] = ConditionsValues[key];
                }
                //Night shift
                if (Area.SplitToShift)
                {
                    ConditionsValues.Clear();
                    values = PIFunctions.GetInterpolatedValuesByCount(Settings.PIServer, Tagname.Trim(), EndDTime, EndNTime, Area.InterpolatedValuesCount);
                    ConditionsValues.Add(Tagname, values);
                    foreach (NModeCondition cond in Conditions)
                    {
                        if (!ConditionsValues.ContainsKey(cond.Tagname))
                            ConditionsValues.Add(cond.Tagname, PIFunctions.GetInterpolatedValuesByCount(Settings.PIServer, cond.Tagname.Trim(), EndDTime, EndNTime, Area.InterpolatedValuesCount));
                    }
                    for (int i = 0; i < values.Count; i++)
                    {
                        value = values[i];
                        int istate = (int)value.DValue;
                        if (IsNormal(ConditionsValues, i))
                        {
                            _NightNormal += 1;
                            NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Normal");
                        }
                        else
                        {
                            if (istate == iManMode)
                            {
                                _NightManual += 1;
                                NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Manual");
                            }
                            else
                            {
                                _NightOther += 1;
                                NormalIntervals.Add($"{i + 1}. {values[i].TimeStamp}: Other");
                            }
                        }
                    }
                    _NightNormal *= factor;
                    _NightManual *= factor;
                    _NightOther *= factor;

                    foreach (string key in ConditionsValues.Keys)
                    {
                        AllConditionsValues[key].AddRange(ConditionsValues[key]);
                    }
                }
                else
                {
                    _NightNormal = _DayNormal;
                    _NightManual = _DayManual;
                    _NightOther = _DayOther;
                }
                Calculated = true;
            }
            catch
            {
                _DayNormal = 0;
                _DayManual = 0;
                _DayOther = 100;
                _NightNormal = 0;
                _NightManual = 0;
                _NightOther = 100;
            }
        }

        DayNormal = _DayNormal;
        DayManual = _DayManual;
        DayOther = _DayOther;
        NightNormal = _NightNormal;
        NightManual = _NightManual;
        NightOther = _NightOther;

    }

    public void Snapshot(DateTime? dt = null, string Writer = "")
    {
        this.Writer = Writer;
        Double _DayNormal = 0; Double _DayManual = 0; Double _DayOther = 0;

        SelectedDate = dt.Value;
        DateTime now = DateTime.Now;
        if (dt == null)
        {
            dt = now;
        }

        double factor = 100;

        int iManMode = Settings.States.IndexOf(Settings.ManualState);
        string ManMode = Settings.ManualState;
        TagValue value = null;

        if (Settings.NONEasNormal && Nmode == "NONE")
        {
            _DayNormal = 100;
        }
        else
        {
            try
            {
                Dictionary<string, TagValue> ConditionsValues = new Dictionary<string, TagValue>();
                value = PIFunctions.GetRecordedValue(Settings.PIServer, Tagname.Trim(), dt, RetrievalTypeConstants.rtAuto);
                ConditionsValues.Add(Tagname, value);
                foreach (NModeCondition cond in Conditions)
                {
                    if (!ConditionsValues.ContainsKey(cond.Tagname))
                        ConditionsValues.Add(cond.Tagname, PIFunctions.GetRecordedValue(Settings.PIServer, cond.Tagname.Trim(), dt, RetrievalTypeConstants.rtAuto));
                }

                int istate = (int)value.DValue;
                if (IsNormal(ConditionsValues))
                {
                    _DayNormal = 1;
                }
                else
                {
                    if (istate == iManMode)
                    {
                        _DayManual = 1;
                    }
                    else
                    {
                        _DayOther = 1;
                    }
                }

                _DayNormal *= factor;
                _DayManual *= factor;
                _DayOther *= factor;

            }
            catch
            {
                _DayNormal = 0;
                _DayManual = 0;
                _DayOther = 100;
            }
        }

        DayNormal = _DayNormal;
        DayManual = _DayManual;
        DayOther = _DayOther;
        NightNormal = _DayNormal;
        NightManual = _DayManual;
        NightOther = _DayOther;

        Calculated = true;
        Snapshoted = true;
    }

    public bool IsNormal(Dictionary<string, List<TagValue>> ConditionsValues, int iteration)
    {
        int istate = (int)ConditionsValues[Tagname][iteration].DValue;
        bool bstate = IsTrue(istate);
        if (Conditions == null || Conditions.Count == 0) return bstate;
        try
        {
            if (NModeORed)
            {
                //NMODE OR (COND1 AND/OR Cond2)
                if (bstate) return true;
                bool cstate = false;
                foreach (NModeCondition cond in Conditions)
                {
                    TagValue val = ConditionsValues[cond.Tagname][iteration];
                    if (ConditionORed)
                    {
                        cstate = cstate | cond.IsTrue(val);
                        if (cstate) break;
                    }
                    else
                    {
                        cstate = cstate & cond.IsTrue(val);
                    }
                }
                return bstate | cstate;
            }
            else
            {
                //NMODE AND (COND1 AND/OR Cond2)
                if (!bstate) return false;
                bool cstate = false;
                foreach (NModeCondition cond in Conditions)
                {
                    TagValue val = ConditionsValues[cond.Tagname][iteration];
                    if (ConditionORed)
                    {
                        cstate = cstate | cond.IsTrue(val);
                        if (cstate) break;
                    }
                    else
                    {
                        cstate = cstate & cond.IsTrue(val);
                    }
                }
                return bstate & cstate;
            }
        }
        catch
        {
            return false;
        }
    }

    public bool IsNormal(Dictionary<string, TagValue> ConditionsValues)
    {
        int istate = (int)ConditionsValues[Tagname].DValue;
        bool bstate = IsTrue(istate);
        if (Conditions == null || Conditions.Count == 0) return bstate;
        try
        {
            if (NModeORed)
            {
                //NMODE OR (COND1 AND/OR Cond2)
                if (bstate) return true;
                bool cstate = false;
                foreach (NModeCondition cond in Conditions)
                {
                    TagValue val = ConditionsValues[cond.Tagname];
                    if (ConditionORed)
                    {
                        cstate = cstate | cond.IsTrue(val);
                        if (cstate) break;
                    }
                    else
                    {
                        cstate = cstate & cond.IsTrue(val);
                    }
                }
                return bstate | cstate;
            }
            else
            {
                //NMODE AND (COND1 AND/OR Cond2)
                if (!bstate) return false;
                bool cstate = false;
                foreach (NModeCondition cond in Conditions)
                {
                    TagValue val = ConditionsValues[cond.Tagname];
                    if (ConditionORed)
                    {
                        cstate = cstate | cond.IsTrue(val);
                        if (cstate) break;
                    }
                    else
                    {
                        cstate = cstate & cond.IsTrue(val);
                    }
                }
                return bstate & cstate;
            }
        }
        catch
        {
            return false;
        }
    }

    public bool IsTrue(int Value)
    {
        switch (Operator)
        {
            case "=":
                return iNMODE == Value;
            case ">":
                return Value > iNMODE;
            case "<":
                return Value < iNMODE;
            case "<>":
                return iNMODE != Value;
            default:
                return false;
        }
    }
    public string ResultToString()
    {
        if (Conditions == null || Conditions.Count == 0)
            return $"{_settings.ReportTimeStamp(SelectedDate)}, DNormal: {DayNormal:f2} %,DManual: {DayManual:f2} %,DOther: {DayOther:f2} %,NNormal: {NightNormal:f2} %,NManual: {NightManual:f2} %,NOther: {NightOther:f2} %,NTotal: {NormalTotal:f2} %";
        else
            return $"{_settings.ReportTimeStamp(SelectedDate)}, DNormal: {DayNormal:f2} %,DManual: {DayManual:f2} %,DOther: {DayOther:f2} %,NNormal: {NightNormal:f2} %,NManual: {NightManual:f2} %,NOther: {NightOther:f2} %,NTotal: {NormalTotal:f2} %";

    }


}

public class NModeRecords : List<NModeRecord>
{
    public NModeRecords() { }
    public NModeRecords(IEnumerable<NModeRecord> records) { base.AddRange(records); }
    public void Calculate(DateTime Day, string Writer = "")
    {
        this.ForEach(record => { record.Calculate(Day, Writer); });
    }

}

//public class RecordViewModel
//{
//    public RecordViewModel() { }
//    public RecordViewModel(NModeRecord record) { Record = record; }
//    public NModeRecord Record { get; set; }
//    //public List<Area> Areas { get; set; }
//    //public List<Lcn> Lcns { get; set; }
//}

public enum TagHealthStatus
{
    Unchecked,
    Good,
    TagNotExists,
    BadPointsource,
    ScanOff
}

