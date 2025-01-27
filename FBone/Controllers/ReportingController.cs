using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models;
using FBone.Models.Reporting;
using FBone.Service;
using FBone.Service.Authorize;
using FBone.Service.WriteToPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using OSIsoft.AF.PI;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class ReportingController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly IWebHostEnvironment _env;

        public ReportingController(DataManager dataManager, IWebHostEnvironment env)
        {
            _dataManager = dataManager;
            _env = env;            
        }

        [HttpGet]
        public IActionResult GeneratePeriodic(string start, string end, int facilityId, bool flag1, bool flag2)//, bool flag3, bool flag4)
        {            
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var format = "yyyy-MM-dd";
            CultureInfo provider = new CultureInfo("ru-RU");
            var startDate = DateTime.ParseExact(start, format, provider);
            var endDate = DateTime.ParseExact(end, format, provider);
            int eventType = 0;
            if (flag1 && flag2)
                eventType = 3;
            else if (flag1)
                eventType = 1;
            else if (flag2)
                eventType = 2;            
            var list = _dataManager.Event.GetGridEvents(startDate, endDate, 3, eventType, user.lang.ToUpper(), facilityId).OrderBy(i => i.Tagnumber).ThenBy(i => i.SetTime).ToList();            
            //start export to excel
            
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                

                //Fill header
                worksheet.Cell(1, 1).Value = "Area";
                worksheet.Cell(1, 2).Value = "Tagnumber";
                worksheet.Cell(1, 3).Value = "Device";
                worksheet.Cell(1, 4).Value = "Event Type";
                worksheet.Cell(1, 5).Value = "Service";
                worksheet.Cell(1, 6).Value = "Event set time";
                worksheet.Cell(1, 7).Value = "Event clear time";
                worksheet.Cell(1, 8).Value = "Duration";
                worksheet.Cell(1, 9).Value = "Act#";
                worksheet.Cell(1, 10).Value = "Cause";
                worksheet.Cell(1, 11).Value = "ReportIt";


                //Fill data
                var row = 2;
                if (list.Count==0)
                {
                    worksheet.Cell(row, 1).Value = "No data for export";
                }                    
                else
                {
                    foreach (var item in list)
                    {
                        worksheet.Cell(row, 1).Value = item.Area;
                        worksheet.Cell(row, 2).Value = item.Tagnumber;
                        worksheet.Cell(row, 3).Value = item.Device;
                        if (item.EventType == 1)
                            worksheet.Cell(row, 4).Value = "Force";
                        else if (item.EventType == 2)
                            worksheet.Cell(row, 4).Value = "Bypass";
                        else if (item.EventType == 3)
                            worksheet.Cell(row, 4).Value = "Alarm disable";
                        else if (item.EventType == 4)
                            worksheet.Cell(row, 4).Value = "Alarm inhibit";
                        worksheet.Cell(row, 5).Value = item.Service;
                        worksheet.Cell(row, 6).Value = item.SetTime;
                        worksheet.Cell(row, 7).Value = item.ClearTime;
                        worksheet.Cell(row, 8).Value = item.ClearTime - item.SetTime;
                        if (item.ActNum !=0)
                            worksheet.Cell(row, 9).Value = item.ActNum;
                        worksheet.Cell(row, 10).Value = item.Cause;
                        worksheet.Cell(row, 11).Value = item.ReportIt;                        

                        row++;
                    }
                    //format excel                
                    worksheet.Columns().AdjustToContents();
                }              

                //send a file to user
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ReportForPeriod.xlsx");
                }
            }
        }

        public IActionResult ReportList()
        {
            var model = new ReportListModel();
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            model.ReportGroups = _dataManager.tArea.GetAllReportGroups();
            model.EventTo = DateTime.Now;
            model.EventFrom = new DateTime(model.EventTo.Year, model.EventTo.Month, 1);
            model.QAQCTo = DateTime.Now;
            model.QAQCFrom = DateTime.Now.AddDays(-1);
            model.Lang = user.lang;
            model.URL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            model.Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name");
            model.ReportGroup = new SelectList(model.ReportGroups);
            model.DateFormat = Config.ServerDateFormat;
            return View(model);
        }

        public IActionResult WriteToPi()
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return RedirectToAction("Denied", "Access");
            }
            var areaList = _dataManager.tArea.GetAreasByFacility(user.FacilityId);
            var bypassesTotal = 0;
            var forcesTotal = 0;
            foreach (var area in areaList)
            {
                var listEvent = _dataManager.Event.GetAllEvents();
                var cnt = 0;
                
                var tagName = "";
                if (!string.IsNullOrEmpty(area.TagBypasActive))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Bypass && i.EventDateTimeClear == null && i.Tag.AreaId == area.Id).Count();
                    bypassesTotal += cnt;
                    tagName = area.TagBypasActive;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagForcesActive))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Force && i.EventDateTimeClear == null && i.Tag.AreaId == area.Id).Count();
                    forcesTotal += cnt;
                    tagName = area.TagForcesActive;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagAlarmDisabled))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Inactive && i.EventDateTimeClear == null && i.Tag.AreaId == area.Id).Count();                    
                    tagName = area.TagAlarmDisabled;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagAlarmInhibited))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Inhibited && i.EventDateTimeClear == null && i.Tag.AreaId == area.Id).Count();
                    tagName = area.TagAlarmInhibited;
                    UpdatePI(tagName, cnt);
                }
                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0);
                DateTime yestd = today.AddDays(-1);
                DateTime yestd_1 = today.AddDays(-2);

                if (!string.IsNullOrEmpty(area.TagBypasDaily))
                {                                        
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Bypass && i.EventDateTimeSet >= yestd && i.Tag.AreaId == area.Id).Count();
                    tagName = area.TagBypasDaily;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagForcesDaily))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Force && i.EventDateTimeSet >= yestd && i.Tag.AreaId == area.Id).Count();
                    tagName = area.TagForcesDaily;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagAlarmDisabledYestd))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Inactive && i.EventDateTimeSet >= yestd_1 && i.EventDateTimeSet<=yestd && i.Tag.AreaId == area.Id).Count();
                    tagName = area.TagAlarmDisabledYestd;
                    UpdatePI(tagName, cnt);
                }
                if (!string.IsNullOrEmpty(area.TagAlarmInhibitedYestd))
                {
                    cnt = listEvent.Where(i => i.ReportIt == true && i.TypeId == (int)Enums.EventTypeCode.Inhibited && i.EventDateTimeSet >= yestd_1 && i.EventDateTimeSet <= yestd && i.Tag.AreaId == area.Id).Count();
                    tagName = area.TagAlarmInhibitedYestd;
                    UpdatePI(tagName, cnt);
                }
            }
            var facility = _dataManager.tFacility.GetFacilityById(user.FacilityId);
            if(!string.IsNullOrEmpty(facility.TagBypassTotal))
            {
                UpdatePI(facility.TagBypassTotal, bypassesTotal);
            }
            if (!string.IsNullOrEmpty(facility.TagForcesTotal))
            {
                UpdatePI(facility.TagForcesTotal, forcesTotal);
            }
            return RedirectToAction("Index");
        }

        private void UpdatePI(string ptname, object value)
        {
            PIData pd = new PIData();
            pd.Add(new PITagData(ptname, new List<PIEvent>()
            {
                new PIEvent(value)                
            }));
            try
            {
                pd.WriteToPI(Config.PICollectiveName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in write to PI: " + ex.Message);
            }

        }

        public IActionResult Index()
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer)) { 
                return RedirectToAction("Denied", "Access");
            }
            var model = new ReportingModel()
            {
                EventTo = DateTime.Now,
                EventFrom = DateTime.Now.AddDays(-1),
                ActItemsTo = DateTime.Now,
                ActItemsFrom = DateTime.Now.AddDays(-1),
                Lang = user.lang,
                DefaultDevice = -1,
                DefaultEventType = Config.ReportingDefaultEventType,
                DefaultReportIt = Config.ReportingDefaultEventType
            };            
            
            model.EventList = _dataManager.Event.GetGridEvents(model.EventFrom, model.EventTo, model.DefaultReportIt, model.DefaultEventType, user.lang.ToUpper(), user.Area.FacilityId).ToList();
            model.ActItemsList = _dataManager.tAct.GetGridActItems(user.lang.ToUpper(), user.Area.FacilityId).ToList();
            model.URL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            model.DateFormat = Config.ServerDateFormat;
            model.Areas = _dataManager.tArea.GetAreasByFacility(user.FacilityId, true);
            model.Devices = _dataManager.Device.GetDevicesSelectList(true);
            model.ReportGroups = _dataManager.tArea.GetReportGroupsForFacility(user.FacilityId);
            return View(model);
        }

        public IActionResult ActiveLocksExcel(ActiveLocksModel model)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var list = _dataManager.Event.GetGridEventsActive(model.TypeId, user.lang.ToUpper(), model.FacilityId);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ActiveLocks");


                //Fill header
                worksheet.Cell(1, 1).Value = "Facility";
                worksheet.Cell(1, 2).Value = "Area";
                worksheet.Cell(1, 3).Value = "Tagnumber";
                worksheet.Cell(1, 4).Value = "Device";
                worksheet.Cell(1, 5).Value = "Event Type";
                worksheet.Cell(1, 6).Value = "Service";
                worksheet.Cell(1, 7).Value = "Event set time";                
                worksheet.Cell(1, 8).Value = "Act#";
                worksheet.Cell(1, 9).Value = "Cause";
                worksheet.Cell(1, 10).Value = "Source";

                //Fill data
                var row = 2;
                if (list.Count == 0)
                {
                    worksheet.Cell(row, 1).Value = "No data for export";
                }
                else
                {
                    foreach (var item in list)
                    {
                        worksheet.Cell(row, 1).Value = item.Facility;
                        worksheet.Cell(row, 2).Value = item.Area;
                        worksheet.Cell(row, 3).Value = item.Tagnumber;
                        worksheet.Cell(row, 4).Value = item.Device;
                        if (item.EventType == 1)
                            worksheet.Cell(row, 5).Value = "Force";
                        else if (item.EventType == 2)
                            worksheet.Cell(row, 5).Value = "Bypass";
                        else if (item.EventType == 3)
                            worksheet.Cell(row, 5).Value = "Alarm disable";
                        else if (item.EventType == 4)
                            worksheet.Cell(row, 5).Value = "Alarm inhibit";
                        worksheet.Cell(row, 6).Value = item.Service;
                        worksheet.Cell(row, 7).Value = item.SetTime;                        
                        worksheet.Cell(row, 8).Value = item.ActNum;
                        worksheet.Cell(row, 9).Value = item.Cause;
                        worksheet.Cell(row, 10).Value = item.DataOrigin;

                        row++;
                    }
                    //format excel                
                    worksheet.Columns().AdjustToContents();
                }

                //send a file to user
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ActiveLocks.xlsx");
                }
            }
        }

        public IActionResult ActiveLocks(ActiveLocksModel model)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            model.ActiveEventList = _dataManager.Event.GetGridEventsActive(model.TypeId, user.lang.ToUpper(), model.FacilityId);
            var listFacility = _dataManager.tFacility.GetFacilities().ToList();
            var facility = new tFacility()
            {
                Id=0,
                Name="..."
            };
            listFacility.Insert(0, facility);
            model.Facilities = new SelectList(listFacility, "Id", "Name");

            var typeList = Enums.GetEventTypeList();
            tListValue fld = new tListValue
            {
                Id = 150,
                Value = "Forces/Bypasses"
            };
            typeList.Add(fld);
            fld = new tListValue
            {
                Id = 160,
                Value = "Disable/Inhibited"
            };
            typeList.Add(fld);
            
            fld = new tListValue
            {
                Id = 0,
                Value = "..."
            };
            typeList.Insert(0,fld);
            model.Types = new SelectList(typeList, "Id", "Value");
            model.Host = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            model.Lang = user.lang;
            return View(model);
        }

        public IActionResult PrintDailyReport(string reportgroup)
        {
            var webReport = new WebReport();
            webReport.Report.Load(Path.Combine(_env.WebRootPath + "/Reports/", "ForcesReport.frx"));            
            webReport.Report.Dictionary.Connections[0].ConnectionString = Config.GetConnectionString();
            webReport.Report.SetParameterValue("ReportGroup", reportgroup);
            string date = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
            if (webReport.Report.Prepare())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(webReport.Report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", date + " " + reportgroup + " Daily Force Report.pdf");
                }
            }
            return View(webReport);
        }
        public IActionResult PrintSummaryReportAlarm()
        {
            var webReport = new WebReport();
            webReport.Report.Load(Path.Combine(_env.WebRootPath + "/Reports/", "Summary.frx"));
            webReport.Report.Dictionary.Connections[0].ConnectionString = Config.GetConnectionString();            
            string date = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
            if (webReport.Report.Prepare())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(webReport.Report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", date + " Summary Report.pdf");
                }
            }
            return View(webReport);
        }

        public IActionResult PrintDailyReportAlarm(string reportgroup)
        {
            var webReport = new WebReport();
            webReport.Report.Load(Path.Combine(_env.WebRootPath + "/Reports/", "AlarmsReport.frx"));            
            webReport.Report.Dictionary.Connections[0].ConnectionString = Config.GetConnectionString();            
            webReport.Report.SetParameterValue("ReportGroup", reportgroup);
            string date = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
            if (webReport.Report.Prepare())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(webReport.Report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", date + " " + reportgroup + " Daily Alarm Report.pdf");
                }
            }
            return View(webReport);
        }
        public IActionResult GenerateQAQC(string reportGroup, string QAQCFrom, string QAQCTo)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name); 
            var format = "yyyy-MM-dd";
            CultureInfo provider = new CultureInfo("ru-RU");
            
            var startDate = DateTime.ParseExact(QAQCFrom, format, provider);
            var endDate = DateTime.ParseExact(QAQCTo, format, provider);
            var webReport = new WebReport();
            
            webReport.Report.Load(Path.Combine(_env.WebRootPath + "/Reports/", "QAQC.frx"));
            webReport.Report.Dictionary.Connections[0].ConnectionString = Config.GetConnectionString();
            webReport.Report.SetParameterValue("ReportGroup", reportGroup);
            webReport.Report.SetParameterValue("start1", startDate.ToString(Config.SQLServerDateFormat));
            webReport.Report.SetParameterValue("end1", endDate.ToString(Config.SQLServerDateFormat));

            //string date = DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2");
            if (webReport.Report.Prepare())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(webReport.Report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", "QA Report.pdf");
                }
            }
            return View(webReport);            
        }
        

        //[HttpPost]
        public IActionResult AddToEventAllActs()
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return RedirectToAction("Denied", "Access");
            }

            var list = _dataManager.tAct.GetGridActItems(user.lang.ToUpper(), user.Area.FacilityId).ToList();
            foreach (var item in list)
            {
                var tagid = _dataManager.Tag.CreateOrUpdateTag(item.DeviceId, item.TagNumber, "", item.AreaId);
                _dataManager.Event.CreateNewEvent(item.ActType, item.Id, tagid);
            }
            return Ok();
        }
    }
}