using FBone.Database;
using FBone.Database.Entities;
using FBone.Models.Audits;
using FBone.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FBone.Controllers
{
    public class AuditingController : Controller
    {
        private readonly DataManager _dataManager;
        public IActionResult Delete(int auditId)
        {
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            if (audit == null)
                return BadRequest("Audit doesn't exist!");

            var userHelper = new UserHelper(_dataManager);
            if (audit.StatusCode != (int)Enums.AuditStatusCode.Draft || !userHelper.IsHasRole(User.Identity.Name, Enums.Roles.IsAuditCreatorOrEditor))
                return RedirectToAction("Denied", "Access");
            
            _dataManager.AuditTable.DeleteAudit(auditId);
            return RedirectToAction("Index");
        }
        private void StartProcess(Audit audit, tUser user)
        {
            audit.StatusCode = (int)Enums.AuditStatusCode.InProgress;
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.Submitted, "");
            NotifyActionOwner(audit);
        }        
        public IActionResult CompleteAction(int auditId, string message)
        {
            message = message.Replace("/-NN-/", Environment.NewLine);
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            if (audit == null)
                return BadRequest("Audit doesn't exist!");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            
            if (audit.StatusCode != (int)Enums.AuditStatusCode.InProgress || !IsUserOrB2B(audit.ActionOwnerPositionId, user.Id))
                return RedirectToAction("Denied", "Access");

            audit.ActionTakenNote = message;
            audit.CompletedByUserId = user.Id;
            audit.ActionCompletedOn = DateTime.Now;
            audit.StatusCode=(int)Enums.AuditStatusCode.OnVerification;
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.ActionCompleted, "");
                       
            NotifyVerificator(audit);

            return RedirectToAction("Index");
        }
        public IActionResult VerifyAudit(int auditId, string message)
        {
            message = message.Replace("/-NN-/", Environment.NewLine);
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            if (audit == null)
                return BadRequest("Audit doesn't exist!");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var verificatorId = _dataManager.tArea.GetAreaById(audit.AreaId).VerificatorId ?? default;
            if (verificatorId == 0)
            {
                throw new InvalidOperationException("Verificator is not set for areaId=" + audit.AreaId);
            }
            if (audit.StatusCode != (int)Enums.AuditStatusCode.OnVerification || !IsUserOrB2B(verificatorId, user.Id))
                return RedirectToAction("Denied", "Access");

            audit.VerificationNote = message;
            audit.VerifiedByUserId = user.Id;
            audit.VerifiedOn = DateTime.Now;
            audit.StatusCode = (int)Enums.AuditStatusCode.OnApproval1;
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.Verified, "");

            NotifyApprover1(audit);

            return RedirectToAction("Index");
        }
        public IActionResult ApproveAudit2(int auditId, string message)
        {
            message = message?.Replace("/-NN-/", Environment.NewLine);
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            if (audit == null)
                return BadRequest("Audit doesn't exist!");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var approver2 = Config.AuditApproverPosition2;

            if (audit.StatusCode != (int)Enums.AuditStatusCode.OnApproval2 || !IsUserOrB2B(approver2, user.Id))
                return RedirectToAction("Denied", "Access");

            audit.Approval2Note = message;
            audit.Approved2ByUserId = user.Id;
            audit.Approved2On = DateTime.Now;
            audit.CloseDate = DateTime.Now;
            audit.StatusCode = (int)Enums.AuditStatusCode.Closed;
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.Approved, "Approved by second approver");
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.Closed, "Closed by system");

            NotifyOnCompletion(audit);

            return RedirectToAction("Index");
        }
        public IActionResult ApproveAudit1(int auditId, string message)
        {
            message = message?.Replace("/-NN-/", Environment.NewLine);
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            if (audit == null)
                return BadRequest("Audit doesn't exist!");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var approver1 = Config.AuditApproverPosition1;
            
            if (audit.StatusCode != (int)Enums.AuditStatusCode.OnApproval1 || !IsUserOrB2B(approver1, user.Id))
                return RedirectToAction("Denied", "Access");

            audit.Approval1Note = message;
            audit.Approved1ByUserId = user.Id;
            audit.Approved1On = DateTime.Now;
            audit.StatusCode = (int)Enums.AuditStatusCode.OnApproval2;
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id, user.Id, (int)Enums.AuditHistoryCode.Approved, "Approved by first approver");

            NotifyApprover2(audit);

            return RedirectToAction("Index");
        }
        internal void NotifyActionOwner(Audit audit)
        {
            var actionUsers = _dataManager.tUser.GetUsersByPosition(audit.ActionOwnerPositionId, "EN");
            var mailto = new List<MailAddress>();
            foreach (var useritem in actionUsers)
            {
                mailto.Add(new MailAddress(useritem.Email));
            }
            var mailcc = new List<MailAddress>() { new MailAddress(audit.CreatedByUser.Email) };

            ProceedEmailPreparation(audit.Id, "AuditStarted", mailto, mailcc);            
        }
        internal void NotifyOnCompletion(Audit audit)
        {
            //owner
            var mailto = new List<MailAddress>()
            {
                new MailAddress(audit.CreatedByUser.Email)
            };
            //Responsible
            var actionUser = _dataManager.tUser.GetUserById(audit.CompletedByUserId ?? default);
            mailto.Add(new MailAddress(actionUser.Email));
            //Verificator
            var verificator = _dataManager.tUser.GetUserById(audit.VerifiedByUserId ?? default);
            mailto.Add(new MailAddress(verificator.Email));
            //Approver1
            var approver1 = _dataManager.tUser.GetUserById(audit.Approved1ByUserId ?? default);
            mailto.Add(new MailAddress(approver1.Email));
            //Approver1
            var approver2 = _dataManager.tUser.GetUserById(audit.Approved2ByUserId ?? default);
            mailto.Add(new MailAddress(approver2.Email));

            ProceedEmailPreparation(audit.Id, "AuditCompleted", mailto, null);            
        }
        internal void NotifyOwnerOnReject(Audit audit)
        {
            var mailto = new List<MailAddress>()
            {
                new MailAddress(audit.CreatedByUser.Email)
            };
            ProceedEmailPreparation(audit.Id, "AuditRejected", mailto, null);
        }
        internal void NotifyApprover2(Audit audit)
        {
            var approver2 = Config.AuditApproverPosition2;
            //var approverId = _dataManager.tArea.GetAreaById(audit.AreaId).VerificatorId ?? default;
            var approverUsers = _dataManager.tUser.GetUsersByPosition(approver2, "EN");
            var mailto = new List<MailAddress>();
            foreach (var useritem in approverUsers)
            {
                mailto.Add(new MailAddress(useritem.Email));
            }
            var mailcc = new List<MailAddress>() { new MailAddress(audit.CreatedByUser.Email) };
            ProceedEmailPreparation(audit.Id, "AuditApprovalStarted", mailto, mailcc);
        }        
        internal void NotifyApprover1(Audit audit)
        {            
            var approverUsers = _dataManager.tUser.GetUsersByPosition(Config.AuditApproverPosition1, "EN");
            var mailto = new List<MailAddress>();
            foreach (var useritem in approverUsers)
            {
                mailto.Add(new MailAddress(useritem.Email));
            }
            var mailcc = new List<MailAddress>() { new MailAddress(audit.CreatedByUser.Email) };            
            ProceedEmailPreparation(audit.Id, "AuditApprovalStarted", mailto, mailcc);            
        }
        internal void NotifyVerificator(Audit audit)
        {
            var verificatorId = _dataManager.tArea.GetAreaById(audit.AreaId).VerificatorId ?? default;
            var verificatorUsers = _dataManager.tUser.GetUsersByPosition(verificatorId, "EN");
            var mailto = new List<MailAddress>();
            foreach (var useritem in verificatorUsers)
            {
                mailto.Add(new MailAddress(useritem.Email));
            }
            var mailcc = new List<MailAddress>() { new MailAddress(audit.CreatedByUser.Email) };
            ProceedEmailPreparation(audit.Id, "AuditVerificationStarted", mailto, mailcc);            
        }
        internal void ProceedEmailPreparation(int auditId, string templateName, List<MailAddress> mailto, List<MailAddress> mailcc)
        {
            string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Auditing/Details/{auditId}";
            var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId(templateName);
            string subject = emailTemplate.Subject;

            string body = emailTemplate.Body
                .Replace("@number@", auditId.ToString())
                .Replace("@AuditURL@", actURL);

            MailHelper.SendMail(mailto, mailcc, subject, body);
        }
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
            if (model.Audit.StatusCode != (int)Enums.AuditStatusCode.Draft)
            {
                return RedirectToAction("Denied", "Access");
            }
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.IsAuditCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }
            var validation = ValidateAudit(model);
            if (!validation.IsNullOrEmpty())
                return BadRequest(validation);

            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var audit = model.Audit;
                
                var newFlag = model.Audit.Id == 0;
                _dataManager.AuditTable.Save(audit);
                _dataManager.AuditTable
                    .CreateAuditHistoryRecord(audit.Id, user.Id, newFlag ? (int)Enums.AuditHistoryCode.Created : (int)Enums.AuditHistoryCode.Updated, "");
                
                if (model.startApproval)
                    StartProcess(audit, user);
                
                if (model.FileId != 0)
                    _dataManager.AuditFileTable.LinkFileToAudit(audit.Id, model.FileId);
                return RedirectToAction(nameof(Index));
            }

            FillModel(model);
            return View("Details", model);
        }
        private string ValidateAudit(AuditDetailsModel model)
        {
            if (model.startApproval && model.FileId == 0)
            {
                ModelState.AddModelError("FileId", "Please upload Audit form");
            }
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
            ModelState.Clear();
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
        [HttpPost]
        public IActionResult Reject(int auditId, string rejectcomment)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var audit = _dataManager.AuditTable.GetAuditById(auditId);
            var verificatorId = _dataManager.tArea.GetAreaById(audit.AreaId).VerificatorId ?? default;            
            audit.CreatedByUser = null;
            var canReject = 
                (audit.StatusCode == (int)Enums.AuditStatusCode.InProgress && IsUserOrB2B(audit.ActionOwnerPositionId, user.Id)) ||
                (audit.StatusCode == (int)Enums.AuditStatusCode.OnVerification && IsUserOrB2B(verificatorId, user.Id)) ||
                (audit.StatusCode == (int)Enums.AuditStatusCode.OnApproval1 && IsUserOrB2B(Config.AuditApproverPosition1, user.Id)) ||
                (audit.StatusCode == (int)Enums.AuditStatusCode.OnApproval2 && IsUserOrB2B(Config.AuditApproverPosition1, user.Id));
            if (!canReject)
            {
                return RedirectToAction("Denied", "Access");
            }
            audit.StatusCode = (int)Enums.AuditStatusCode.Draft;
            ClearAudit(audit);
            _dataManager.AuditTable.Save(audit);
            _dataManager.AuditTable.CreateAuditHistoryRecord(audit.Id,user.Id,(int)Enums.AuditHistoryCode.Rejected,rejectcomment);
            NotifyOwnerOnReject(audit);
            return RedirectToAction("Index");
        }
        private void ClearAudit(Audit audit)
        {
            audit.ActionCompletedOn = null;
            audit.CompletedByUserId = null;
            audit.VerifiedOn = null;
            audit.VerifiedByUserId = null;
            audit.Approved1On = null;
            audit.Approved1ByUserId = null;
            audit.Approved2On = null;
            audit.Approved2ByUserId = null;
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
                    //model.Audit.Tags = "111/-/222/-/333/-/444";
                }
            }
            model.AuditHistory = new List<AuditHistory>();
            if (model.Audit.Id != 0)
            {
                var file = _dataManager.AuditFileTable.GetFileByAuditId(model.Audit.Id);
                if (file != null)
                {
                    model.FileId = file.Id;
                    model.FileName = file.Name;
                }
                model.CanEdit = model.Audit.StatusCode == (int)Enums.AuditStatusCode.Draft && IsAuditCreatorOrB2B(model.Audit, user);
                model.AuditHistory = _dataManager.AuditTable.GetAditHistory(model.Audit.Id);
                model.CanComplete = model.Audit.StatusCode == (int)Enums.AuditStatusCode.InProgress && IsUserOrB2B(model.Audit.ActionOwnerPositionId, user.Id);
                if (model.Audit.StatusCode == (int)Enums.AuditStatusCode.OnVerification)
                {
                    var verificatorId = _dataManager.tArea.GetAreaById(model.Audit.AreaId).VerificatorId ?? default;
                    model.CanVerify = IsUserOrB2B(verificatorId,user.Id);
                }
                    
                if (model.Audit.StatusCode == (int)Enums.AuditStatusCode.OnApproval1)
                {                    
                    var approver1 = Config.AuditApproverPosition1;                    
                    model.CanApprove1 = IsUserOrB2B(approver1, user.Id);
                }
                if (model.Audit.StatusCode == (int)Enums.AuditStatusCode.OnApproval2)
                {
                    var approver2 = Config.AuditApproverPosition2;
                    model.CanApprove2 = IsUserOrB2B(approver2, user.Id);
                }
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
        private bool IsUserOrB2B(int positionId, int userId)
        {
            var users = _dataManager.tUser.GetUsersByPosition(positionId, "EN");
            return users.Any(u => u.Id == userId);            
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
