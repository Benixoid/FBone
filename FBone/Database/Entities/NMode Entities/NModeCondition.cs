using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FBone.Service.WriteToPI;
using PISDK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBone.Models.NMode;
public class NModeCondition:IdObject
{
    public int Id { get; set; }

    public int NModeRecordId { get; set; }
    public string Tagname { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public string Value { get; set; } = string.Empty;
    public virtual NModeRecord NModeRecord { get; set; } = null;

    public bool IsTrue(TagValue tval)
    {
        try
        {
            double val = 0;// Value;

            if (!double.TryParse(Value, out val))
            {
                if (NModeRecord.Settings.States.Contains(Value))
                    val = NModeRecord.Settings.States.IndexOf(Value);
                else
                    switch (Operator)
                    {
                        case "=":
                            return tval.SValue == Value;
                        case ">":
                        case "<":
                        case "<>":
                            return tval.SValue != Value;
                        default:
                            return false;
                    }
            }
            switch (Operator)
            {
                case "=":
                    return Convert.ToDouble(tval.Value) == val;
                case ">":
                    return Convert.ToDouble(tval.Value) > val;
                case "<":
                    return Convert.ToDouble(tval.Value) < val;
                case "<>":
                    return Convert.ToDouble(tval.Value) != val;
                default:
                    return false;
            }
        }
        catch 
        {
            return false;
        }
    }

    public bool IsPointExists()
    {
        if (string.IsNullOrEmpty(Tagname)) return false;
        return PIFunctions.IsPointExist(NModeRecord?.Settings.PIServer, Tagname);
    }
    public Dictionary<string, object> GetAttributeValues(IEnumerable<string> AttributeNames)
    {
        Dictionary<string, object> AttributeValues = new Dictionary<string, object>();

        PIPoint pt = PIFunctions.GetPIPoint(NModeRecord?.Settings.PIServer, Tagname);
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

    public override string ToString()
    {
        return $"{Tagname}{Operator}{Value}";
    }
}

