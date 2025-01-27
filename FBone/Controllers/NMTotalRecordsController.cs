using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using FastReport.Data;
using FBone.Database;
using FBone.Models.NMode;
using FBone.Service;
using FBone.Service.Authorize;
using FBone.Service.WriteToPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using SortState = FBone.Models.NMode.SortState;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class NMTotalRecordsController : Controller
    {
        private readonly DataManager DataManager;
        private readonly NMManager manager;
        public NMTotalRecordsController(DataManager dm)
        {
            DataManager = dm;
            manager = dm.NMManager;
        }

        // GET: NMTotalRecordsController
        public ActionResult Index(DateTime SelectedDate, bool LoadCalculatedResults = false)
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");
            if (SelectedDate == DateTime.MinValue) SelectedDate = DateTime.Now.AddDays(-1);

            NMTotalRecords records = new NMTotalRecords(manager.TotalRecords.Where(tr => tr.Parent == null));
            records.Sort();
            records.ForEach(trec => trec.LoadData(SelectedDate, manager, LoadCalculatedResults, true, LoadCalculatedResults));
            if (LoadCalculatedResults)
            {
                records.SetResultAvailability(SelectedDate, manager);
                records.ForEach(trec =>
                {
                    //if (trec.ResultsAvailable(SelectedDate, manager)) 
                    trec.SummarizeResult();
                });
            }


            records.SelectedDate = SelectedDate;
            return View(records);// model);
        }

        public ActionResult SaveToPI(int Id, double Value, DateTime SelectedDate)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return Unauthorized();
            try
            {
                NMTotalRecord TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == Id);
                if (TRecord == null) return NotFound();
                DateTime dtReport = manager.Settings.ReportTimeStamp(SelectedDate);
                if (dtReport > DateTime.Now) return Ok();
                //TRecord.SelectedDate = SelectedDate;
                TRecord._NormalTotalPercent = Value;
                if (manager.Settings.WriteToPI)
                    TRecord.SaveToPI(dtReport);

                return Ok();
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ExportToExcel(int TotalRecordId, int AreaId, int LcnId, DateTime dt, bool HideInActiveRecords = false, string SearchText = "", bool Snapshot = false)
        {
            NMTotalRecord TRecord = null;
            List<NModeRecord> records = null;
            NMTotalRecords TRecords = null;
            if (TotalRecordId == 0)
            {
                NMRecordListModel rlm = new NMRecordListModel(manager, AreaId, LcnId, dt, HideInActiveRecords, SearchText);
                records = rlm.NModeRecords;
            }
            else if (TotalRecordId > 0)
            {
                TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == TotalRecordId);
                if (TRecord == null)
                {
                    return NotFound();
                }
                TRecords = new NMTotalRecords();
                TRecords.Add(TRecord);
            }
            else if (TotalRecordId == -1)
            {
                TRecords = new NMTotalRecords(manager.TotalRecords.Where(tr => tr.Parent == null));
                TRecords.Sort();
                TRecords.ForEach(trec => trec.LoadData(dt, manager, false, true, false));
            }

            using (var bk = new XLWorkbook())
            {
                if (TotalRecordId == 0)
                {
                    var sht = bk.Worksheets.Add("Records");
                    WriteRecords(sht, records, 1);
                    sht.Cell(1, 1).Value = $"Report for " + (Snapshot ? $"Snapshot at {dt.ToString()}" : $"{dt.ToShortDateString()}");
                }
                else
                {
                    foreach (var trec in TRecords)
                        ExportTotalToExcel(bk, trec, dt, HideInActiveRecords, SearchText, Snapshot);

                    if (TRecords.Count > 1)
                    {
                        var sht = bk.Worksheets.Add($"Summary", 0);
                        var row = 1;
                        for (int i = 0; i < TRecords.Count; i++)
                        {
                            NMTotalRecord trec = TRecords[i];
                            WriteSubTotals(trec, sht, ref row, false, true, i == 0, $"Summary for {dt.ToShortDateString()}:");
                        }

                        IXLTable table = sht.Range(2, 1, row - 1, 4).CreateTable();
                        table.Theme = XLTableTheme.TableStyleLight8;
                        sht.Columns().AdjustToContents();
                    }
                }

                //send a file to user
                using (var stream = new MemoryStream())
                {
                    bk.SaveAs(stream);
                    var content = stream.ToArray();
                    Area area = null; Lcn lcn = null;
                    var sArea = "";
                    if (AreaId > 0) area = manager.Areas.FirstOrDefault(a => a.Id == AreaId);
                    if (LcnId > 0) lcn = manager.Lcns.FirstOrDefault(l => l.Id == LcnId);
                    if (area != null) sArea = area.Name;
                    if (lcn != null) sArea += "_" + lcn.Name;
                    if (TRecord != null) sArea += "_" + TRecord.Tagname;
                    if (Snapshot) sArea += "_Snapshot";
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"NModeReport_{sArea}_{dt.ToString("dd/MM/yy")}.xlsx");
                }
            }
        }

        private bool ExportTotalToExcel(XLWorkbook bk, NMTotalRecord TRecord, DateTime dt, bool HideInActiveRecords = false, string SearchText = "", bool Snapshot = false, bool Hide100PercentResults = false)
        {
            if (TRecord == null) return false;
            try
            {
                var sht = bk.Worksheets.Add($"{TRecord.Area.Name}_{TRecord.Lcn.Name}");
                NMTotalRecordViewModel trlm = FormModel(TRecord, dt, SortState.NormalTotalAsc, 1, false, Snapshot, SearchText, 0, false, HideInActiveRecords);
                List<NModeRecord> records = trlm.NModeRecords;
                records.AddRange(TRecord.SubNMRecords);
                if (HideInActiveRecords) records = records.Where(r => r.CountIt).ToList();
                if (Hide100PercentResults) records = records.Where(r => r.NormalTotal != 100).ToList();
                records = records.OrderBy(r => r.NormalTotal).ToList();

                var row = 1;

                if (TRecord.SubTotals.Count > 0)
                {
                    row = 4;
                    WriteSubTotals(TRecord, sht, ref row, true, false);
                }
                else
                    row++;

                WriteRecords(sht, records, row);

                sht.Cell(1, 1).Value = $"Report for " + (Snapshot ? $"Snapshot at {dt.ToString()}" : $"{dt.ToShortDateString()}");
                if (TRecord != null) sht.Cell(2, 1).Value = $"Total: {Math.Round(TRecord.NormalTotalPercent, 2)}% - [Day Normal: {Math.Round(TRecord.DayNormalTotalPercent, 2)}%, Night Normal: {Math.Round(TRecord.NightNormalTotalPercent, 2)}%]";
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void WriteSubTotals(NMTotalRecord TRecord, IXLWorksheet sht, ref int row, bool CreateTable = true, bool WriteParentData = true, bool WriteHeaders = true, string Header = "Sub totals:")
        {
            if (WriteHeaders)
            {
                sht.Cell(row, 1).Value = Header;
                row++;

                sht.Cell(row, 1).Value = "Area";
                sht.Cell(row, 2).Value = "Lcn";
                sht.Cell(row, 3).Value = "Tagname";
                sht.Cell(row, 4).Value = "Total";
                row++;
            }
            if (WriteParentData)
            {
                sht.Cell(row, 1).Value = TRecord.Area.Name;
                sht.Cell(row, 2).Value = TRecord.Lcn.Name;
                sht.Cell(row, 3).Value = TRecord.Tagname;
                sht.Cell(row, 4).Value = Math.Round(TRecord.NormalTotalPercent, 2);
                row++;
            }
            foreach (var st in TRecord.SubTotals)
            {
                sht.Cell(row, 1).Value = st.Area.Name;
                sht.Cell(row, 2).Value = st.Lcn.Name;
                sht.Cell(row, 3).Value = st.Tagname;
                sht.Cell(row, 4).Value = Math.Round(st.NormalTotalPercent, 2); ;
                row++;
            }
            if (CreateTable)
            {
                IXLTable table = sht.Range(5, 1, row - 1, 4).CreateTable();
                table.Theme = XLTableTheme.TableStyleLight8;
            }
        }

        private void WriteRecords(IXLWorksheet sht, List<NModeRecord> records, int row)
        {
            row += 2;
            //Fill header
            var ir = row;
            sht.Cell(ir, 1).Value = "Area";
            sht.Cell(ir, 2).Value = "Lcn";
            sht.Cell(ir, 3).Value = "Tagname";
            sht.Cell(ir, 4).Value = "Total";
            sht.Cell(ir, 5).Value = "NMode";
            sht.Cell(ir, 6).Value = "Enabled";

            //Fill data
            row++;
            if (records == null || records.Count == 0)
            {
                sht.Cell(row, 1).Value = "No data for export";
            }
            else
            {
                foreach (var record in records)
                {
                    sht.Cell(row, 1).Value = record.Area.Name;
                    sht.Cell(row, 2).Value = record.Lcn.Name;
                    sht.Cell(row, 3).Value = record.Tagname;
                    sht.Cell(row, 4).Value = Math.Round(record.NormalTotal, 2);
                    sht.Cell(row, 5).Value = record.Nmode;
                    sht.Cell(row, 6).Value = record.CountIt;
                    row++;
                }
            }
            var table = sht.Range(ir, 1, row - 1, 6).CreateTable();
            table.Theme = XLTableTheme.TableStyleLight8;
            sht.Columns().AdjustToContents();
        }

        public ActionResult DetailsTR(int Id, DateTime SelectedDate, SortState sortOrder = SortState.NormalTotalAsc, int page = 1, bool Calculate = false, bool Snapshot = false,
            string Search = "", int PageSize = 15, bool ValidateTags = false, bool HideDetails = false, bool ResultDetails = false, bool HideInActiveRecords = true, bool Hide100PercentResults = false)
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");

            if (SelectedDate == DateTime.MinValue) SelectedDate = DateTime.Now.AddDays(-1);

            var TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == Id);// manager.GetNMTotalRecord(SelectedDate, Id);// manager.TotalRecords.FirstOrDefault(r => r.Id == Id);
            if (TRecord == null)
            {
                return NotFound();
            }

            NMTotalRecordViewModel model = FormModel(TRecord, SelectedDate, sortOrder, page, Calculate, Snapshot, Search, PageSize, ValidateTags, HideInActiveRecords, Hide100PercentResults);

            model.HideDetails = HideDetails;
            model.ResultDetails = ResultDetails;

            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;
            ViewData["Controller"] = "NMTotalRecords";
            ViewData["Action"] = "DetailsTR";

            //if (Calculated)
            //{
            //    return RedirectToAction(nameof(DetailsTR), new { Id = TRecord.Id, Calculated = false, SelectedDate = SelectedDate, sortOrder = sortOrder, page = page, Search = Search, PageSize = PageSize });
            //}

            return View("DetailsTR", model);
        }

        private NMTotalRecordViewModel FormModel(NMTotalRecord TRecord, DateTime SelectedDate, SortState sortOrder = SortState.NormalTotalAsc, int page = 1,
            bool Calculate = false, bool Snapshot = false, string Search = "", int PageSize = 15, bool ValidateTags = false, bool HideInactiveRecords = true, bool Hide100PercentResults = false)
        {
            NMTotalRecordViewModel model = new NMTotalRecordViewModel(TRecord, HideInactiveRecords, SelectedDate, Search, Hide100PercentResults);

            model.SelectedDate = SelectedDate;

            if (Calculate)
            {
                TRecord.LoadData(SelectedDate, manager, true, false);
                TRecord.Calculate(SelectedDate, User.Identity.Name);
                manager.SaveObject(TRecord);
                model.Calculated = false;
                Snapshot = false;
            }

            TRecord.LoadData(SelectedDate, manager, !Calculate, true, !Calculate); //Load results if not calculated

            //Have to filter this only after data loaded instead of aplly filter inside constructor of NMTotalRecordViewModel
            //cause currently data for records of Total record are not loaded in manager.TotalRecords. Alternative is to use manager.GetNMTotalRecord(SelectedDate, Id). Currently first one is choosen
            if (Hide100PercentResults)
            {
                model.NModeRecords = model.NModeRecords.Where(r => r.NormalTotal != 100).ToList();
            }

            DateTime dtReport = manager.Settings.ReportTimeStamp(SelectedDate);
            model.ResultsAvailable = TRecord.ResultsAvailable(dtReport);

            var sresult = TRecord.GetSavedData(dtReport);
            model.SavedResult = $"Saved Normal Total: {sresult}%";
            if (model.ResultsAvailable)
            {
                TRecord.SummarizeResult();
                model.CalculatedResult =
                $"Calculated Normal Total: {TRecord.NormalTotalPercent:0.##}%" +
                (TRecord.Area.SplitToShift ? $" - [Day Normal: {TRecord.DayNormalTotalPercent:0.##}%, Night Normal: {TRecord.NightNormalTotalPercent:0.##}%]" : "");
            }

            switch (sortOrder)
            {
                case SortState.TagnameAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Tagname).ToList();
                    break;
                case SortState.TagnameDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.Tagname).ToList();
                    break;
                case SortState.AreaAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Area.Name).ToList();
                    break;
                case SortState.AreaDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.Area.Name).ToList();
                    break;
                case SortState.LcnAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Lcn.Name).ToList();
                    break;
                case SortState.LcnDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.Lcn.Name).ToList();
                    break;
                case SortState.NormalTotalAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.NormalTotal).ToList();
                    break;
                case SortState.NormalTotalDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.NormalTotal).ToList();
                    break;
                case SortState.ConditionsAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Conditions.Count).ToList();
                    break;
                case SortState.ConditionsDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.Conditions.Count).ToList();
                    break;
                case SortState.EditorAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Editor).ToList();
                    break;
                case SortState.EditorDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.Editor).ToList();
                    break;
                case SortState.ChangeDateAsc:
                    model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.ChangeDate).ToList();
                    break;
                case SortState.ChangeDateDesc:
                    model.NModeRecords = model.NModeRecords.OrderByDescending(rec => rec.ChangeDate).ToList();
                    break;
                default:
                    if (!model.ResultsAvailable) // If no results at all records
                    {
                        model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.Tagname).ToList();
                        sortOrder = SortState.TagnameAsc;
                    }
                    else
                    {
                        model.NModeRecords = model.NModeRecords.OrderBy(rec => rec.NormalTotal).ToList();
                        sortOrder = SortState.NormalTotalAsc;
                    }
                    break;
            }
            model.SortViewModel = new SortViewModel(sortOrder);

            var count = model.NModeRecords.Count();
            var pagecount = (int)Math.Ceiling(count / (double)PageSize);
            if (PageSize > 0)
            {
                if (pagecount == 1)
                {
                    page = 1;
                }
                else
                {
                    if (page > pagecount)
                        page = 1;
                }
                if (PageSize > 0) model.NModeRecords = model.NModeRecords.Skip((page - 1) * PageSize).Take(PageSize).ToList();
            }

            model.ValidateTags = ValidateTags;
            if (ValidateTags)
            {
                model.NModeRecords.ForEach(r => r.ValidateTag());
            }

            if (Snapshot)
            {
                TRecord.Snapshot(SelectedDate, User.Identity.Name);
                //foreach (var record in model.NModeRecords)
                //{
                //    if (record.CountIt)
                //    {
                //        record.Snapshot(SelectedDate);
                //    }
                //}
                model.Snapshot = true;
                model.SnapshotResult =
             $"Snapshot Normal Total: {TRecord.NormalTotalPercent:0.##}% at {SelectedDate}";
            }

            model.Page = new PageViewModel(count, page, PageSize);//, model);//AreaId, LcnId, sortOrder);
            return model;
        }

        public ActionResult GetCalculatedResult(int Id, DateTime SelectedDate)
        {
            var TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == Id);
            TRecord.LoadData(SelectedDate, manager, true, false);
            if (TRecord == null)
            {
                return NotFound();
            }
            TRecord.Calculate(SelectedDate);
            //manager.SaveObject(TRecord);
            // SelectedDate = (new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, manager.Settings.ReportHour, 0, 0)).AddDays(1);

            return Json(new
            {
                Tagname = TRecord.Tagname,
                NormalSaved = TRecord.GetSavedData(manager.Settings.ReportTimeStamp(SelectedDate)),
                Normal = TRecord.NormalTotalPercent,
                DayNormal = TRecord.DayNormalTotalPercent,
                NightNormal = TRecord.NightNormalTotalPercent,
                AllowedToSave = DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor)
            });
        }
        public ActionResult GetSavedResult(int Id, DateTime SelectedDate)
        {
            var TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == Id);
            if (TRecord == null)
            {
                return NotFound();
            }
            //SelectedDate = (new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, manager.Settings.ReportHour, 0, 0)).AddDays(1);
            TRecord.LoadData(SelectedDate, manager, true);
            return Json(new
            {
                Tagname = TRecord.Tagname,
                Normal = TRecord.NormalTotalPercent,
            });
        }

        public IActionResult LoadAverage(int RecordId, bool Snapshot, DateTime StartTime, DateTime EndTime)
        {
            return ViewComponent("NMTotalAverage", new { RecordId = RecordId, Snapshot = Snapshot, StartTime = StartTime, EndTime = EndTime });
        }

        // GET: NMTotalRecordsController/Create
        public ActionResult Create()
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeAdministrator))
                return RedirectToAction("Denied", "Access");

            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;
            return View();
        }

        // POST: NMTotalRecordsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NMTotalRecord trecord)// IFormCollection collection)
        {
            ValidateModel(trecord);
            ModelState.Remove("Id");
            ModelState.Remove("Area");
            ModelState.Remove("Lcn");

            if (!ModelState.IsValid)
            {
                ViewData["Areas"] = manager.Areas;
                ViewData["Lcns"] = manager.Lcns;
                return View(trecord);
            }
            try
            {
                await Task.Run(() => manager.SaveObject(trecord));

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}:/r/n{ex.StackTrace}");
                ViewData["Areas"] = manager.Areas;
                ViewData["Lcns"] = manager.Lcns;
                return View(trecord);
            }
        }

        private void ValidateModel(NMTotalRecord trecord, bool CheckAreaLCN = true)
        {
            if (trecord.Settings == null) trecord.Settings = manager.Settings;
            TagHealthStatus status = trecord.ValidateTag();
            switch (status)
            {
                case TagHealthStatus.BadPointsource:
                    ModelState.AddModelError(nameof(trecord.Tagname), $"Bad pointsource");
                    break;
                case TagHealthStatus.TagNotExists:
                    ModelState.AddModelError(nameof(trecord.Tagname), "Tag is not found");
                    break;
                case TagHealthStatus.ScanOff:
                    ModelState.AddModelError(nameof(trecord.Tagname), "Tag is Scan Off");
                    break;
            }

            if (trecord.Area == null)
                ModelState.AddModelError(nameof(trecord.Area), "Area must be set");
            if (trecord.Lcn == null)
                ModelState.AddModelError(nameof(trecord.Lcn), "Lcn must be set");

            if (CheckAreaLCN && trecord.Area != null && trecord.Lcn != null && manager.TotalRecords.Any(tr => tr.Area.Id == trecord.Area.Id && tr.Lcn.Id == trecord.Lcn.Id))
                ModelState.AddModelError("", "Total record already exists for this LCN");
        }

        // GET: NMTotalRecordsController/Edit/5
        public ActionResult EditTR(int id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == 0 || manager.TotalRecords == null)
            {
                return NotFound();
            }

            var TRecord = manager.TotalRecords.FirstOrDefault(r => r.Id == id);
            if (TRecord == null)
            {
                return NotFound();
            }

            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;
            List<NMTotalRecord> parents = manager.TotalRecords;
            TRecord.ListOfHeirs().ForEach(r => parents.Remove(r));
            parents.Insert(0, null);
            ViewData["Parents"] = parents;
            ViewData["PreviousPage"] = Request.Headers.Referer[0];
            return View(TRecord);
        }

        // POST: NMTotalRecordsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTR([Bind("Id,LcnId,AreaId,ParentId,Tagname")] NMTotalRecord TRecord, string PrevPage)// IFormCollection collection)
        {
            try
            {
                var tr = manager.TotalRecords.FirstOrDefault(r => r.Id == TRecord.Id);
                if (tr == null)
                {
                    return NotFound();
                }

                TRecord.Area = manager.Areas.Find(r => r.Id == TRecord.AreaId);
                TRecord.Lcn = manager.Lcns.Find(r => r.Id == TRecord.LcnId);
                if (TRecord.ParentId != null && TRecord.ParentId > 0)
                    TRecord.Parent = manager.TotalRecords.Find(r => r.Id == TRecord.ParentId);

                ValidateModel(TRecord, TRecord.AreaId != tr.AreaId || TRecord.LcnId != tr.LcnId);
                if (tr.AreaId != TRecord.AreaId)
                {
                    tr.AreaId = TRecord.AreaId;
                    tr.Area = TRecord.Area;
                }
                if (tr.LcnId != TRecord.LcnId)
                {
                    tr.LcnId = TRecord.LcnId;
                    tr.Lcn = TRecord.Lcn;
                }
                if (tr.Tagname != TRecord.Tagname) tr.Tagname = TRecord.Tagname;

                if (tr.ParentId != TRecord.ParentId)
                {
                    if (TRecord.ParentId == 0)
                    {
                        tr.ParentId = null;
                        tr.Parent = null;
                    }
                    else
                    {
                        tr.ParentId = TRecord.ParentId;
                        tr.Parent = TRecord.Parent;
                    }
                }


                if (!ModelState.IsValid)
                {
                    ViewData["Areas"] = manager.Areas;
                    ViewData["Lcns"] = manager.Lcns;
                    List<NMTotalRecord> parents = manager.TotalRecords;
                    TRecord.ListOfHeirs().ForEach(r => parents.Remove(r));
                    parents.Insert(0, null);
                    ViewData["Parents"] = parents;
                    return View(TRecord);
                }
                manager.SaveObject(tr);
                if (PrevPage != null)
                    return Redirect(PrevPage);
                else
                    return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"{ex.Message}:/r/n{ex.StackTrace}");
                ViewData["Areas"] = manager.Areas;
                ViewData["Lcns"] = manager.Lcns;
                return View(TRecord);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeAdministrator))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.TotalRecords == null)
            {
                return NotFound();
            }

            var TRecord = await Task.Run(() => manager.TotalRecords.FirstOrDefault(r => r.Id == id));
            if (TRecord == null)
            {
                return NotFound();
            }

            return View(TRecord);
        }

        // POST: NModeConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var TRecord = manager.TotalRecords.FirstOrDefault(tr => tr.Id == id);
            if (TRecord != null)
            {
                await Task.Run(() => manager.DeleteObject(TRecord));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
