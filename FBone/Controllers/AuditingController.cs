using Azure.Core;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models.Act;
using FBone.Models.Audits;
using FBone.Service;
using Inventory.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FBone.Controllers
{
    public class AuditingController : Controller
    {
        private readonly DataManager _dataManager;
        public AuditingController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        public IActionResult AuditDetails(AuditDetailsModel model)
        {
            if (model.Audit.Id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var audit = _dataManager.AuditTable.GetAuditById(model.Audit.Id);

            //if act not exist
            if (audit == null)
            {
                return RedirectToAction(nameof(Index));
            }
            model.Audit = audit;
            
            FillModel(model);
            ModelState.Clear();
            return View("Details", model);
        }
        public IActionResult SaveAudit(AuditDetailsModel model)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.IsAuditCreatorOrEditor) && model.Audit.StatusCode != (int)Enums.AuditStatusCode.Draft )
            {
                return RedirectToAction("Denied", "Access");
            }
            //ModelState
            if (model.Audit.StatusCode == (int)Enums.AuditStatusCode.Draft && model.startApproval && model.FileId == 0)
            {
                ModelState.AddModelError("FileId", "Please upload Audit form");
            }
            var validation = ValidateAudit(model);
            if (!validation.IsNullOrEmpty())
                return BadRequest(validation);

            if (ModelState.IsValid)
            {
                _dataManager.AuditTable.Save(model.Audit);
                if (model.FileId != 0)
                    _dataManager.AuditFileTable.LinkFileToAudit(model.Audit.Id, model.FileId);
                return RedirectToAction(nameof(Index));
            }

            FillModel(model);
            return View("Details", model);
        }
        private string ValidateAudit(AuditDetailsModel model)
        {
            if (model.Audit.Id != 0)
            {
                var audit = _dataManager.AuditTable.GetAuditById(model.Audit.Id);
                if (audit == null)
                {
                    return "Audit doesn't exist";
                }
                model.Audit.CreatedAt = audit.CreatedAt;
            }
            else
            {
                model.Audit.CreatedAt = DateTime.Now;
            }
            return null;
        }
        public IActionResult RegisterAudit()
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.IsAuditCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            Audit newAudit = new()
            {
                FacilityId = user.FacilityId,
                AreaId = user.AreaId,
                StatusCode = (int)Enums.AuditStatusCode.Draft,
                CreatedAt = DateTime.Now,
                CreatedByUserID = user.Id,
                CreatedByUser = user,
                ShiftDate = DateTime.Now
                //AuditItems = [new AuditItem()]
                 //ActItems = new List<tActItems>()
            };            
            AuditDetailsModel model = new()
            {
                Audit = newAudit
            };
            FillModel(model);
            return View("Details", model);
        }
        public IActionResult Index(AuditListModel val)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var listPageSizeItems = GeneralHelper.GetDefaultPageItems();
            var formatS = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            var model = new AuditListModel()
            {
                CanCreateAudit = _dataManager.tUser.CanCreateAudit(User.Identity.Name),
                ItemPerPageList = new SelectList(listPageSizeItems, "Id", "Name"),
                ItemPerPage = val.ItemPerPage != 0 ? val.ItemPerPage : listPageSizeItems[0].Id,
                PageIndex = val.PageIndex != 0 ? val.PageIndex : 1,
                SelectedFacilityId = val.SelectedFacilityId == 0 ? user.FacilityId : val.SelectedFacilityId,
                Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name"),
                SmartSearch = val.SmartSearch,
                SelectedAreaId = val.SelectedAreaId != 0 ? val.SelectedAreaId : -1,
                SelectedAuditStatus = val.SelectedAuditStatus,
                DateFrom = val.DateFrom == new DateTime(1, 1, 1) ? new DateTime(2025, 1, 1) : val.DateFrom,
                DateTo = val.DateTo == new DateTime(1, 1, 1) ? DateTime.Today : val.DateTo

            };
            model.Areas = _dataManager.tArea.GetAreasByFacility(model.SelectedFacilityId, true);
            if (!string.IsNullOrEmpty(val.DateFromS))
                model.DateFrom = DateTime.ParseExact(val.DateFromS, formatS, provider);
            if (!string.IsNullOrEmpty(val.DateToS))
                model.DateTo = DateTime.ParseExact(val.DateToS, formatS, provider);
            model.DateFromS = model.DateFrom.ToString(formatS);
            model.DateToS = model.DateTo.ToString(formatS);
            var auditList = _dataManager.AuditTable.GetPaginatedList(model);
            if (auditList != null)
            {
                model.HasNextPage = auditList.HasNextPage;
                model.HasPreviousPage = auditList.HasPreviousPage;
                model.TotalPages = auditList.TotalPages;
                model.TotalEntities = auditList.TotalEntities;
            }
            model.AuditList = auditList;
            return View(model);
        }
        private void FillModel(AuditDetailsModel model)
        {
            model.URL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);            
            model.Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name");
            model.Areas = new SelectList(_dataManager.tArea.GetAreasByFacility(model.Audit.FacilityId),"Id", "Name_EN");
            var positions = _dataManager.tPosition.GetActivePositions();
            model.Positions = new SelectList(positions, "Id", "Name");
            var actList = _dataManager.tAct.getActByShiftDateAndArea(model.Audit.ShiftDate, model.Audit.AreaId).ToList();
            
            List<tActItems> actItems = new();
            if (actList.Count > 0) {
                model.Acts = new SelectList(actList, "Id", "Id");
                actItems = _dataManager.tActItems.GetActItemsByActId(actList[0].Id).ToList();
                model.Tags = new SelectList(actItems, "TagName", "TagName");
                if (model.Audit.Id == 0)
                {
                    model.Audit.ActId = actList[0].Id;
                    model.Audit.Tags = "111/-/222/-/333/-/444";
                }                    
            }            
            if (model.Audit.Id != 0)
            {
                var file = _dataManager.AuditFileTable.GetFileByAuditId(model.Audit.Id);
                if (file != null)
                {
                    model.FileId = file.Id;
                    model.FileName = file.Name;
                }
                model.CanEdit = model.Audit.StatusCode == (int)Enums.AuditStatusCode.Draft && IsAuditCreatorOrB2B(model.Audit, user);
            }
            else
            {   
                model.Audit.ActionOwnerPositionId = positions.First().Id;                
                model.CanEdit = true;                
            }           
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);
            model.UsersInPosition = _dataManager.tUser.GetUsersList(model.Audit.ActionOwnerPositionId, userLang);
        }
        
        public ActionResult GetAttachment(int auditId, int fileId)
        {
            var attachment = _dataManager.AuditFileTable.GetFileContent(auditId, fileId);
            var stream = new MemoryStream(attachment.Item2);

            if (stream.Length == 0)
                return NotFound();

            return File(stream, "application/octet-stream", attachment.Item1);
        }
        public ActionResult DeleteAttachment(int auditId)
        {
            _dataManager.AuditFileTable.DeleteFiles(auditId);
            return Json(string.Empty);
        }
        private bool IsAuditCreatorOrB2B(Audit audit, tUser currUser)
        {
            if (audit.CreatedByUserID == currUser.Id)
                return true;
            else
            {
                var createdByUser = _dataManager.tUser.GetUserById(audit.CreatedByUserID);
                if (currUser.PositionId == createdByUser.PositionId)
                    return true;
            }
            return false;
        }
        private string ExtractFileName(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }
        public async Task<ActionResult> UploadAttachment(List<IFormFile> files)
        {
            var newFileId = 0;
            foreach (var file in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                filename = ExtractFileName(filename);

                using (var output = new MemoryStream())
                {
                    await file.CopyToAsync(output);
                    var auditFile = new AuditFile();
                    auditFile.Name = filename;
                    auditFile.File = output.ToArray();
                    newFileId = _dataManager.AuditFileTable.UploadFile(auditFile);
                }
            }
            return Json(new { fileId = newFileId });
        }
    }
}
