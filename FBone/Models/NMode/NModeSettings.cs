using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FBone.Models.NMode
{
    public class NModeSettings
    {
        public NModeSettings() { }
        //public NModeSettings(NMManager mngr)
        //{
        //    _manager = mngr;
        //}
        //private NMManager _manager = null;
        public string PIServer { get; set; }
        public bool WriteToPI { get; set; } = true;
        public int InterpolatedValuesCount { get; set; } = 72;
        public int StartingHour { get; set; } = 6;
        public string ManualState { get; set; } = "MAN";
        public bool NONEasNormal { get; set; } = true;
        public bool CaseInsensitive { get; set; } = true;
        public double NMODEThreshold { get; set; } = 95;
        public int TotalHistoryDays { get; set; } = 10;
        public int ReportHour { get; set; } = 1;
        public virtual List<string> States { get; set; } //= new List<string>() { "MAN", "AUTO", "CAS", "BCAS", "NONE" };

        public static void SerializeToFile(string FileName, NModeSettings sett)
        {
            File.WriteAllText(FileName, sett.Serialize());
        }
        public static NModeSettings DeserializeFromFile(string FileName)
        {
            if (!File.Exists(FileName)) return new NModeSettings();
            return JsonSerializer.Deserialize<NModeSettings>(File.ReadAllText(FileName));
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize<NModeSettings>(this, new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });// JavaScriptEncoder.Create(UnicodeRanges.All) });
        }

        public DateTime ReportTimeStamp(DateTime SelectedDate)
        {
            //Assumed that SelectedDate is the date of the report

            //DateTime dtReport = DateTime.Now;
            //dtReport = new DateTime(dtReport.Year, dtReport.Month, dtReport.Day, this.ReportHour, 0, 0);

            SelectedDate = SelectedDate.AddDays(1);
            DateTime dtReport = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, this.ReportHour, 0, 0);
            if (dtReport > DateTime.Now)
            {
                var now = DateTime.Now;
                dtReport = new DateTime(now.Year, now.Month, now.Day, this.ReportHour, 0, 0);
                //   throw new Exception("Bad date selected");
            }
            //if (SelectedDate == DateTime.MinValue || SelectedDate > dtReport)
            //    SelectedDate = dtReport;
            //else
            //{
            //    dtReport = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, this.ReportHour, 0, 0);
            //}
            return dtReport;
        }

    }
}
