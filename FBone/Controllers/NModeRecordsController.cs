using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using FBone.Database;
using FBone.Models.NMode;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class NModeRecordsController : Controller
    {
        private DataManager DataManager;
        private readonly NMManager manager;
        public NModeRecordsController(DataManager dm)
        {
            DataManager = dm;
            manager = dm.NMManager;
        }

        public ActionResult NModeIndex()
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");
            return View();
        }

        public ActionResult Settings()
        {
            return View(manager.Settings);
        }

        public ActionResult Index(DateTime SelectedDate, bool Calculate = false, bool Snapshot = false, int AreaId = 0, int LcnId = 0, SortState sortOrder = SortState.Unsorted, int page = 1,
            string Search = "", int PageSize = 15, bool ValidateTags = false, bool HideDetails = false, bool ResultDetails = false, bool HideInActiveRecords = true, bool Hide100PercentResults = false)
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");

            if (SelectedDate == DateTime.MinValue) SelectedDate = DateTime.Now.AddDays(-1);

            NMRecordListModel model = FormModel(SelectedDate, AreaId, LcnId, sortOrder, page, Calculate, Snapshot, Search, PageSize, ValidateTags, HideInActiveRecords, Hide100PercentResults);

            model.HideDetails = HideDetails;
            model.ResultDetails = ResultDetails;

            ViewData["Controller"] = "NModeRecords";
            ViewData["Action"] = "Index";
            return View("Index", model);
        }

        private NMRecordListModel FormModel(DateTime SelectedDate, int AreaId = 0, int LcnId = 0, SortState sortOrder = SortState.Unsorted, int page = 1,
            bool Calculate = false, bool Snapshot = false, string Search = "", int PageSize = 15, bool ValidateTags = false, bool HideInActiveRecords = true, bool Hide100PercentResults = false)
        {
            NMRecordListModel model = new NMRecordListModel(manager, AreaId, LcnId, SelectedDate, HideInActiveRecords, Search, Hide100PercentResults);

            if (Calculate)
            {
                foreach (var record in model.NModeRecords)
                {
                    if (record.CountIt)
                    {
                        record.Calculate(SelectedDate, User.Identity.Name);
                        manager.SaveObject(record);
                    }
                }
                Snapshot = false;
            }

            //count of records with corresponding result on that day
            DateTime dtReport = manager.Settings.ReportTimeStamp(SelectedDate);
            var ResultCount = model.NModeRecords.Where(r => r.CountIt && r.Results.Where(rr => rr.TimeStamp == dtReport).Count() == 1).Count();
            if (model.NModeRecords.Count != 0 && ResultCount == model.NModeRecords.Where(r => r.CountIt).Count()) // If results available for all enabled records
                model.ResultsAvailable = true;

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

            model.SelectedDate = SelectedDate;
            model.ValidateTags = ValidateTags;
            if (ValidateTags)
            {
                model.NModeRecords.ForEach(r => r.ValidateTag());
            }
            if (Snapshot)
            {
                foreach (var record in model.NModeRecords)
                {
                    if (record.CountIt)
                    {
                        if (Snapshot)
                        {
                            record.Snapshot(SelectedDate, User.Identity.Name);
                        }
                    }
                }
                model.Snapshot = true;
            }

            model.Page = new PageViewModel(count, page, PageSize);

            model.AllowCalculate = true;

            return model;
        }

        // GET: NModeRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.NModeRecords == null)
            {
                return NotFound();
            }

            var nModeRecord = manager.NModeRecords.FirstOrDefault(m => m.Id == id);
            var ParentId = manager.TotalRecords.Where(tr => tr.AreaId == nModeRecord.AreaId && tr.LcnId == nModeRecord.LcnId).FirstOrDefault()?.Id;
            nModeRecord.ParentId = (int)ParentId;
            if (nModeRecord == null)
            {
                return NotFound();
            }

            return View(nModeRecord);
        }

        // GET: NModeRecords/Create
        public IActionResult Create()
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeAdministrator))
                return RedirectToAction("Denied", "Access");

            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;

            return View();
        }

        // POST: NModeRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NModeRecord nModeRecord, string Comment)//[Bind("NModeID,Lcn,Area,Tagname,Descriptor,Nmode,CountIt,ConditionORed")] NModeRecord nModeRecord)
        {
            ValidateModel(nModeRecord);
            ModelState.Remove("Area");
            ModelState.Remove("Lcn");

            if (ModelState.IsValid)
            {
                nModeRecord.Editor = User.Identity.Name;
                nModeRecord.Creator = nModeRecord.Editor;
                nModeRecord.ChangeDate = DateTime.Now;
                nModeRecord.CreationDate = nModeRecord.ChangeDate;
                nModeRecord.Changes.Add(new NModeChangeRecord
                { Comment = Comment, User = User.Identity.Name, ChangeRecordConfig = nModeRecord.ConditionToString(), PreviousRecordConfig = "", Date = nModeRecord.ChangeDate });
                manager.SaveObject(nModeRecord);
                return RedirectToAction(nameof(Index));
            }
            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;

            return View(nModeRecord);
        }

        private void ValidateModel(NModeRecord record, bool EditMode = false)
        {
            if (record.Settings == null) record.Settings = manager.Settings;
            TagHealthStatus status = record.ValidateTag();
            switch (status)
            {
                case TagHealthStatus.BadPointsource:
                    ModelState.AddModelError(nameof(record.Tagname), $"Bad pointsource");
                    break;
                case TagHealthStatus.TagNotExists:
                    ModelState.AddModelError(nameof(record.Tagname), "Tag is not found");
                    break;
                case TagHealthStatus.ScanOff:
                    ModelState.AddModelError(nameof(record.Tagname), "Tag is Scan Off");
                    break;
            }

            if (record.Area == null)
                ModelState.AddModelError(nameof(record.Area), "Area must be set");
            if (record.Lcn == null)
                ModelState.AddModelError(nameof(record.Lcn), "Lcn must be set");
            if (!EditMode && manager.NModeRecords.Any(r => r.Tagname.ToLower() == record.Tagname.ToLower()))
                ModelState.AddModelError(nameof(record.Tagname), "Tagname is already present in database");
        }


        // GET: NModeRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            NModeRecord nModeRecord = null;
            if (id == null || manager.NModeRecords == null)
            {
                nModeRecord = new NModeRecord();
            }
            else
            {
                nModeRecord = manager.NModeRecords.FirstOrDefault(r => r.Id == id);
                if (nModeRecord == null)
                {
                    nModeRecord = new NModeRecord();
                }
            }
            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;
            if (Request.Headers.Referer.Count > 0)
            {
                string PrevPage = Request.Headers.Referer[0];
                if (!PrevPage.Contains("Create") && !PrevPage.Contains("Delete"))
                    ViewData["PreviousPage"] = PrevPage;
            }
            return View(nModeRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,LcnId,Lcn,AreaId,Area,Tagname,Nmode,Operator,Descriptor,CountIt,NModeORed,ConditionORed")] NModeRecord nModeRecord, string Comment, string PrevPage)
        {
            if (id != nModeRecord.Id)
            {
                return NotFound();
            }
            ValidateModel(nModeRecord, true);
            ModelState.Remove(nameof(Area));
            ModelState.Remove(nameof(Lcn));
            NModeRecord oldR = null;
            if (ModelState["Tagname"].ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid && nModeRecord.CountIt == false)
                ModelState.Remove("Tagname");
            if (ModelState.IsValid)
            {
                try
                {
                    oldR = manager.NModeRecords.FirstOrDefault(r => r.Id == nModeRecord.Id);
                    if (oldR != null)
                    {
                        string oldConfig = $"Area: {oldR.Area.Name}, Lcn: {oldR.Lcn.Name}, Conditions: {oldR.ConditionToString()}";
                        var stateOld = manager.GetState(oldR);
                        CopyRecord(oldR, nModeRecord);
                        string newConfig = $"Area: {oldR.Area.Name}, Lcn: {oldR.Lcn.Name}, Conditions: {oldR.ConditionToString()}";
                        var state = manager.GetState(oldR);
                        if (state == EntityState.Modified)
                        {
                            var dt = DateTime.Now;
                            oldR.Changes.Insert(0, new NModeChangeRecord
                            { Comment = Comment, User = User.Identity.Name, ChangeRecordConfig = newConfig, PreviousRecordConfig = oldConfig, Date = dt });
                            oldR.Editor = User.Identity.Name;
                            oldR.ChangeDate = dt;
                            manager.SaveObject(oldR);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NModeRecordExists(nModeRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (!string.IsNullOrEmpty(PrevPage))
                    return Redirect(PrevPage);
                else
                {
                    ViewData["Areas"] = manager.Areas;
                    ViewData["Lcns"] = manager.Lcns;
                    //oldR.Changes = oldR.Changes.OrderByDescending(chng => chng.Date).ToList();
                    return View(oldR);
                }
            }
            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;

            return View(oldR);
        }
        public ActionResult Evaluate(int Id, DateTime SelectedDate, string nmode, string Operator, bool NModeOr, bool CondOr)
        {
            var rec = manager.NModeRecords.FirstOrDefault(r => r.Id == Id);// manager.GetNModeRecord(SelectedDate, Id);// manager.NModeRecords.FirstOrDefault(r => r.Id == Id);

            if (rec == null)
            {
                return NotFound();
            }

            if (SelectedDate == DateTime.MinValue) SelectedDate = DateTime.Now.AddDays(-1);
            if (nmode != null && Operator != null)
            {
                rec.Nmode = nmode; rec.Operator = Operator; rec.NModeORed = NModeOr; rec.ConditionORed = CondOr;
            }
            rec.Calculate(SelectedDate);
            //manager.SaveObject(rec);
            ViewData["Areas"] = manager.Areas;
            ViewData["Lcns"] = manager.Lcns;
            return PartialView("_CalculationResult", rec);
        }

        private void CopyRecord(NModeRecord oldR, NModeRecord newR)
        {
            if (oldR.AreaId != newR.AreaId)
            {
                oldR.AreaId = newR.AreaId;
                oldR.Area = manager.Areas.FirstOrDefault(area => area.Id == newR.AreaId);
            }
            if (oldR.LcnId != newR.LcnId)
            {
                oldR.LcnId = newR.LcnId;
                oldR.Lcn = manager.Lcns.FirstOrDefault(lcn => lcn.Id == newR.LcnId);
            }
            oldR.CountIt = newR.CountIt;
            oldR.NModeORed = newR.NModeORed;
            oldR.ConditionORed = newR.ConditionORed;
            oldR.Nmode = newR.Nmode;
            oldR.Operator = newR.Operator;
            oldR.Tagname = newR.Tagname;
            oldR.Descriptor = newR.Descriptor;
        }

        // GET: NModeRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeAdministrator))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.NModeRecords == null)
            {
                return NotFound();
            }

            var nModeRecord = await Task.Run(() => manager.NModeRecords.FirstOrDefault(m => m.Id == id));
            if (nModeRecord == null)
            {
                return NotFound();
            }

            return View(nModeRecord);
        }

        // POST: NModeRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (manager.NModeRecords == null)
            {
                return Problem("Entity set 'AppDBContext.NModeRecords'  is null.");
            }
            var nModeRecord = manager.NModeRecords.FirstOrDefault(r => r.Id == id);
            if (nModeRecord != null)
            {
                manager.DeleteObject(nModeRecord);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult GetRecordString(int Id, string nmode, string Operator, bool NModeOr, bool CondOr, bool Enabled)
        {
            var rec = manager.NModeRecords.FirstOrDefault(r => r.Id == Id);
            JsonResponseViewModel response = new JsonResponseViewModel();
            if (rec == null)
            {
                return NotFound();
            }
            else
            {
                if (nmode != null && Operator != null)
                {
                    rec.Nmode = nmode; rec.Operator = Operator; rec.NModeORed = NModeOr; rec.ConditionORed = CondOr;
                    rec.CountIt = Enabled;
                    response.ResponseCode = 0;
                    response.ResponseMessage = rec.ConditionToString();
                }
                else
                {
                    response.ResponseCode = 1;
                    response.ResponseMessage = rec.ConditionToString();
                }
            }
            return Json(response);
        }
        private bool NModeRecordExists(int id)
        {
            return (manager.NModeRecords?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    [Serializable]
    public class JsonResponseViewModel
    {
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; } = string.Empty;
    }
}
