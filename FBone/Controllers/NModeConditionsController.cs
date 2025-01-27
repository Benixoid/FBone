using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using FBone.Database;
using FBone.Models.NMode;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PISDK;


namespace FBone.Controllers
{
    [Authorize("User")]
    public class NModeConditionsController : Controller
    {
        private NMManager manager;
        private DataManager DataManager;
        public NModeConditionsController(DataManager dm)
        {
            DataManager = dm;
            manager = dm.NMManager;
        }

        // GET: NModeConditions
        public ActionResult Index()
        {
            if (!DataManager.IsUserInRoles(User, new Enums.Roles[] { Enums.Roles.IsNModeEditor, Enums.Roles.IsNModeUser }))
                return RedirectToAction("Denied", "Access");

            var conditions = manager.NModeConditions;

            return View(conditions);
        }

        // GET: NModeConditions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || manager.NModeConditions == null)
            {
                return NotFound();
            }

            var nModeCondition = await Task.Run(() => manager.NModeConditions.FirstOrDefault(m => m.Id == id));
            if (nModeCondition == null)
            {
                return NotFound();
            }

            return View(nModeCondition);
        }

        // GET: NModeConditions/Create
        public IActionResult Create(int Id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            NModeRecord rec = manager.NModeRecords.FirstOrDefault(r => r.Id == Id);
            var viewModel = new ConditionEditViewModel() { Condition = new NModeCondition() { NModeRecord = rec, NModeRecordId = Id }, Records = manager.NModeRecords };
            ViewData["PreviousPage"] = Request.Headers.Referer[0];

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, ConditionEditViewModel cevm, string Comment, string PrevPage)
        {
            NModeCondition nModeCondition = cevm.Condition;

            ModelState.Remove("Condition.NModeRecord");
            ModelState.Remove("Records");
            if (nModeCondition.NModeRecordId != 0 && nModeCondition.NModeRecord == null)
            {
                nModeCondition.NModeRecord = manager.NModeRecords.FirstOrDefault(r => r.Id == nModeCondition.NModeRecordId);
            }

            ValidateModel(nModeCondition);

            if (ModelState.IsValid)
            {
                try
                {
                    var rec = nModeCondition.NModeRecord;
                    string oldConfig = rec.ConditionToString();
                    rec.Conditions.Add(nModeCondition);
                    nModeCondition.NModeRecord.Changes.Add(new NModeChangeRecord
                    { Comment = Comment, User = User.Identity.Name, ChangeRecordConfig = rec.ConditionToString(), PreviousRecordConfig = oldConfig, Date = DateTime.Now });
                    //manager.SetState(rec, EntityState.Modified);
                    EntityState state = manager.GetState(rec);
                    await Task.Run(() => manager.SaveObject(nModeCondition));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NModeConditionExists(nModeCondition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (PrevPage != null)
                    return Redirect(PrevPage);
                else
                    return RedirectToAction(nameof(Edit), "NModeRecords", new { Id = nModeCondition.NModeRecordId });
            }
            cevm.Records = manager.NModeRecords;
            return View(cevm);
        }

        // GET: NModeConditions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.NModeConditions == null)
            {
                return NotFound();
            }

            var nModeCondition = await Task.Run(() => manager.NModeConditions.FirstOrDefault(c => c.Id == id));
            if (nModeCondition == null)
            {
                return NotFound();
            }
            var viewModel = new ConditionEditViewModel() { Condition = nModeCondition, Records = manager.NModeRecords.ToList() };
            ViewData["PreviousPage"] = Request.Headers.Referer[0];
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConditionEditViewModel cevm, string Comment, string PrevPage)
        {
            NModeCondition nModeCondition = cevm.Condition;
            if (id != nModeCondition.Id)
            {
                return NotFound();
            }
            if (nModeCondition.NModeRecordId != 0 && nModeCondition.NModeRecord == null)
            {
                nModeCondition.NModeRecord = manager.NModeRecords.FirstOrDefault(r => r.Id == nModeCondition.NModeRecordId);
            }
            ValidateModel(nModeCondition);

            if (ModelState.IsValid)
            {
                try
                {
                    NModeCondition oldCond = manager.NModeConditions.FirstOrDefault(c => c.Id == nModeCondition.Id);
                    if (oldCond != null) { manager.SetState(oldCond, EntityState.Detached); }
                    var rec = nModeCondition.NModeRecord;
                    string oldConfig = oldCond.NModeRecord.ConditionToString();
                    var indx = rec.Conditions.IndexOf(oldCond);
                    rec.Conditions[indx] = nModeCondition;
                    manager.SetState(nModeCondition, EntityState.Modified);
                    nModeCondition.NModeRecord.Changes.Add(new NModeChangeRecord
                    { Comment = Comment, User = User.Identity.Name, ChangeRecordConfig = oldConfig, PreviousRecordConfig = rec.ConditionToString(), Date = DateTime.Now });

                    await Task.Run(() => manager.SaveObject(nModeCondition));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NModeConditionExists(nModeCondition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (PrevPage != null)
                    return Redirect(PrevPage);
                else
                    return RedirectToAction(nameof(Edit), "NModeRecords", new { Id = nModeCondition.NModeRecordId });
            }
            cevm.Records = manager.NModeRecords.ToList();
            return View(cevm);// viewModel);
        }
        private void ValidateModel(NModeCondition condition)
        {
            var record = condition.NModeRecord;
            if (record.Settings == null) record.Settings = manager.Settings;

            TagHealthStatus status = condition.ValidateTag();
            switch (status)
            {
                case TagHealthStatus.BadPointsource:
                    ModelState.AddModelError("Condition.Tagname", $"Bad pointsource");
                    break;
                case TagHealthStatus.TagNotExists:
                    ModelState.AddModelError("Condition.Tagname", "Tag is not found");
                    break;
                case TagHealthStatus.ScanOff:
                    ModelState.AddModelError("Condition.Tagname", "Tag is Scan Off");
                    break;
            }
        }
        // GET: NModeConditions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.NModeConditions == null)
            {
                return NotFound();
            }

            var nModeCondition = await Task.Run(() => manager.NModeConditions.FirstOrDefault(m => m.Id == id));
            if (nModeCondition == null)
            {
                return NotFound();
            }

            ViewData["PreviousPage"] = Request.Headers.Referer[0];

            return View(nModeCondition);
        }

        // POST: NModeConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string Comment, string PrevPage)
        {
            if (manager.NModeConditions == null)
            {
                return Problem("Entity set 'AppDBContext.NModeConditions'  is null.");
            }
            var nModeCondition = await Task.Run(() => manager.NModeConditions.FirstOrDefault(m => m.Id == id));
            NModeRecord rec = null;
            if (nModeCondition != null)
            {
                rec = nModeCondition.NModeRecord;
                string oldConfig = rec.ConditionToString();
                rec.Conditions.Remove(nModeCondition);
                //EntityState state = manager.GetState(rec);

                rec.Changes.Add(new NModeChangeRecord
                { Comment = Comment, User = User.Identity.Name, ChangeRecordConfig = rec.ConditionToString(), PreviousRecordConfig = oldConfig, Date = DateTime.Now });

                await Task.Run(() => manager.DeleteObject(nModeCondition));
            }
            if (PrevPage != null)
                return Redirect(PrevPage);
            else
                return RedirectToAction(nameof(Edit), "NModeRecords", new { Id = nModeCondition.NModeRecordId });
        }

        private bool NModeConditionExists(int id)
        {
            return (manager.NModeConditions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
