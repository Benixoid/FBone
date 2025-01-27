using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models;
using FBone.Models.Act;
using FBone.Service;
using Inventory.Models;
using System.Net.Mail;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class ActsController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly IWebHostEnvironment _env;

        public ActsController(DataManager dataManager, IWebHostEnvironment env)
        {
            _dataManager = dataManager;
            _env = env;
        }

        [HttpGet]
        public List<Tag> GetBulkTags(string unit, int areaId, string equipment)
        {
            var list = _dataManager.Tag.GetBulkTags(unit, areaId, equipment);
            var i = 0;
            foreach (var item in list)
            {
                list[i].Equipment = equipment;
                i++;
            }
            return list;
        }

        [HttpGet]
        public List<string> GetEquipment(string unit, int areaId)
        {
            return _dataManager.Tag.GetEquipmentForBulkInsert(unit, areaId);
        }
                
        public IActionResult Start(int ActId)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var act = _dataManager.tAct.GetActById(ActId);
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            if (act.StatusId != (int) Enums.ActStatusCode.Approved)
            {
                return RedirectToAction("Denied", "Access");
            }

            act.StatusId = (int) Enums.ActStatusCode.Active;
            act.OrderColumn = (int)Enums.ActSortCode.Active;
            act.StartedOn = DateTime.Now;
            _dataManager.tAct.Save(act);
            _dataManager.ActHistory.AddHistory(ActId, user.Id, (int)Enums.ActHistoryActionCodes.started, "");
            return RedirectToAction(nameof(ActDetails), new { Id = ActId });
        }

        public IActionResult Delete(int ActId)
        {            
            var act = _dataManager.tAct.GetActById(ActId);
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            if (act.StatusId != (int)Enums.ActStatusCode.Draft)
            {
                return RedirectToAction("Denied", "Access");
            }

            _dataManager.tAct.DeleteAct(ActId);
            return RedirectToAction("Index");
        }

        public IActionResult ReturnDraft(int ActId)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var act = _dataManager.tAct.GetActById(ActId);
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            if (!(act.StatusId == (int)Enums.ActStatusCode.InApproval || act.StatusId == (int)Enums.ActStatusCode.Approved))
            {
                return RedirectToAction("Denied", "Access");
            }

            act.StatusId = (int)Enums.ActStatusCode.Draft;
            act.OrderColumn = (int)Enums.ActSortCode.Draft;
            act.ApprovedBy1On = new DateTime(1, 1, 1);
            act.ApprovedBy2On = new DateTime(1, 1, 1);
            act.ApprovedBy3On = new DateTime(1, 1, 1);
            act.ApprovedBy4On = new DateTime(1, 1, 1);
            act.ApprovedBy5On = new DateTime(1, 1, 1);
            act.ApprovedBy6On = new DateTime(1, 1, 1);
            act.ApprovedBy7On = new DateTime(1, 1, 1);

            act.is1Approved = false;
            act.is2Approved = false;
            act.is3Approved = false;
            act.is4Approved = false;
            act.is5Approved = false;
            act.is6Approved = false;
            act.is7Approved = false;

            _dataManager.tAct.Save(act);
            _dataManager.ActHistory.AddHistory(ActId, user.Id, (int)Enums.ActHistoryActionCodes.returntodraft, "");
            return RedirectToAction(nameof(ActDetails), new { Id = ActId });
        }

        [HttpPost]
        public IActionResult RejectConfirmed(int ActId, string rejectcomment)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var act = _dataManager.tAct.GetActById(ActId);
            var approveFlag = CanApprove(user, act);
            if (approveFlag == 0)
            {
                return RedirectToAction("Denied", "Access");
            }
            act.StatusId = (int) Enums.ActStatusCode.Draft;
            act.OrderColumn = (int)Enums.ActSortCode.Draft;
            act.is1Approved = false;
            act.is2Approved = false;
            act.is3Approved = false;
            act.is4Approved = false;
            act.is5Approved = false;
            act.is6Approved = false;
            act.is7Approved = false;
            act.ApprovedBy1On = new DateTime(1,1,1);
            act.ApprovedBy2On = new DateTime(1, 1, 1);
            act.ApprovedBy3On = new DateTime(1, 1, 1);
            act.ApprovedBy4On = new DateTime(1, 1, 1);
            act.ApprovedBy5On = new DateTime(1, 1, 1);
            act.ApprovedBy6On = new DateTime(1, 1, 1);
            act.ApprovedBy7On = new DateTime(1, 1, 1);
            _dataManager.tAct.Save(act);
            _dataManager.ActHistory.AddHistory(ActId, user.Id, (int)Enums.ActHistoryActionCodes.rejected, rejectcomment);
            
            //notify act creator            
            var mailto = new List<MailAddress> { new MailAddress(act.CreateByUser.Email) };
            
            string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";
            var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("ActRejected");

            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body
                .Replace("@number@", act.Id.ToString())
                .Replace("@rejecteduser@", user.Name_EN)
                .Replace("@ActURL@",actURL);

            MailHelper.SendMail(mailto, subject, body);

            return RedirectToAction("Index");
            
        }
        
        [HttpPost]
        public IActionResult DelegateConfirmed(int ActId, int position_d, int selecteduser)
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            if (_dataManager.tUser.GetUserById(selecteduser) == null)
            {
                return NotFound();
            }

            var act = _dataManager.tAct.GetActById(ActId);

            if (act == null)
            {
                return NotFound();
            }
            switch (position_d)
            {
                case 1:
                    act.Approver1 = selecteduser;
                    break;
                case 2:
                    act.Approver2 = selecteduser;
                    break;
                case 3:
                    act.Approver3 = selecteduser;
                    break;
                case 4:
                    act.Approver4 = selecteduser;
                    break;
                case 5:
                    act.Approver5 = selecteduser;
                    break;
                case 6:
                    act.Approver6 = selecteduser;
                    break;
                case 7:
                    act.Approver7 = selecteduser;
                    break;
                case 8:
                    act.ApproverAdd = selecteduser;
                    break;
            }
            _dataManager.tAct.Save(act);
            var delegateUser = _dataManager.tUser.GetUserById(selecteduser);
            _dataManager.ActHistory.AddHistory(ActId, user.Id, (int)Enums.ActHistoryActionCodes.delegated, delegateUser.Name_EN);

            //notify delegated user
            var mailto = new List<MailAddress> { new MailAddress(_dataManager.tUser.GetUserById(selecteduser).Email) };
            string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";

            var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("ActDelegated");
            string subject = emailTemplate.Subject;            

            string body = emailTemplate.Body
                .Replace("@number@", act.Id.ToString())
                .Replace("@delegatoruser@", user.Name_EN)
                .Replace("@ActURL@", actURL);
            MailHelper.SendMail(mailto, subject, body);

            return RedirectToAction(nameof(ActDetails), new {Id = ActId});
        }

        public IActionResult CopyAct(int ActId)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            var act = _dataManager.tAct.GetActById(ActId);
            var new_act = new tAct()
            {
                AreaId = act.AreaId,
                OriginalLang = user.lang.ToUpper(),
                Crew = act.Crew,
                CreatedOn = DateTime.Now,
                StatusId = (int)Enums.ActStatusCode.Draft,
                OrderColumn = (int)Enums.ActSortCode.Draft,
                CreatedByUserId = user.Id,
                Type = act.Type,
                Approver1 = act.Approver1,
                Approver2 = act.Approver2,
                Approver3 = act.Approver3,
                Approver4 = act.Approver4,
                Approver5 = act.Approver5,
                Approver6 = act.Approver6,
                Approver7 = act.Approver7,
                ApproverPos1 = act.ApproverPos1,
                ApproverPos2 = act.ApproverPos2,
                ApproverPos3 = act.ApproverPos3,
                ApproverPos4 = act.ApproverPos4,
                ApproverPos5 = act.ApproverPos5,
                ApproverPos6 = act.ApproverPos6,
                ApproverPos7 = act.ApproverPos7,                
                IsCauseTranslated = false,                
                IsImpactTranslated = false,
                IsProtectTranslated = false,
                IsTranslated = act.IsTranslated,
                ActNotes = act.ActNotes
            };
            if (user.lang.ToUpper().Equals("EN"))
            {
                new_act.CauseEN = act.CauseEN;
                new_act.ImpactEN = act.ImpactEN;
                new_act.ProtectEN = act.ProtectEN;
            } else if (user.lang.ToUpper().Equals("RU"))
            {
                new_act.CauseRU = act.CauseRU;
                new_act.ImpactRU = act.ImpactRU;
                new_act.ProtectRU = act.ProtectRU;
            } else
            {
                new_act.CauseKK = act.CauseKK;
                new_act.ImpactKK = act.ImpactKK;
                new_act.ProtectKK = act.ProtectKK;
            }
            if (!act.IsCauseTranslated)
            {
                switch (new_act.OriginalLang)
                {
                    case "RU":
                        new_act.CauseEN = "";
                        new_act.CauseKK = "";
                        break;
                    case "EN":
                        new_act.CauseRU = "";
                        new_act.CauseKK = "";
                        break;
                    case "KK":
                        new_act.CauseEN = "";
                        new_act.CauseRU = "";
                        break;
                }                
            }

            if (!act.IsImpactTranslated)
            {
                switch (new_act.OriginalLang)
                {
                    case "RU":
                        new_act.ImpactEN = "";
                        new_act.ImpactKK = "";
                        break;
                    case "EN":
                        new_act.ImpactRU = "";
                        new_act.ImpactKK = "";
                        break;
                    case "KK":
                        new_act.ImpactEN = "";
                        new_act.ImpactRU = "";
                        break;
                }
            }

            if (!act.IsProtectTranslated)
            {
                switch (new_act.OriginalLang)
                {
                    case "RU":
                        new_act.ProtectEN = "";
                        new_act.ProtectKK = "";
                        break;
                    case "EN":
                        new_act.ProtectRU = "";
                        new_act.ProtectKK = "";
                        break;
                    case "KK":
                        new_act.ProtectEN = "";
                        new_act.ProtectRU = "";
                        break;
                }
            }

            var actid = _dataManager.tAct.Save(new_act);            
            foreach (var item in act.ActItems)
            {
                var newitem = new tActItems()
                {
                    ActId = actid,
                    TagName = item.TagName,
                    Unit = item.Unit,
                    Equipment = item.Equipment,
                    DeviceId = item.DeviceId,
                    Location = item.Location               
                };
                _dataManager.tActItems.SaveItem(newitem);                
            }            
            _dataManager.ActHistory.AddHistory(actid, user.Id, (int)Enums.ActHistoryActionCodes.copied, "#" + act.Id.ToString());
            return RedirectToAction(nameof(ActDetails), new { id = actid });            
        }

        public IActionResult Approve(int ActId)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var act = _dataManager.tAct.GetActById(ActId);
            var approveFlag = CanApprove(user, act);
            var statusFlag = false;
            int nextUserId = 0;
            switch (approveFlag)
            {
                case 0:
                    return RedirectToAction("Denied", "Access");
                case 1:
                {
                    act.is1Approved = true;
                    act.ApprovedBy1On = DateTime.Now;
                    if (act.Approver2 != 0)
                    {                        
                        nextUserId = act.Approver2;
                    }
                    else
                    {
                        statusFlag = true;
                    }                        
                    break;
                }
                case 2:
                {
                    act.is2Approved = true;
                    act.ApprovedBy2On = DateTime.Now;
                    if (act.Approver3 != 0)
                    {
                        nextUserId = act.Approver3;
                    }
                    else
                        statusFlag = true;
                    break;
                }
                case 3:
                {
                    act.is3Approved = true;
                    act.ApprovedBy3On = DateTime.Now;
                    if (act.Approver4 != 0)
                    {
                        nextUserId = act.Approver4;
                    }
                    else
                        statusFlag = true;
                    break;
                }
                case 4:
                {
                    act.is4Approved = true;
                    act.ApprovedBy4On = DateTime.Now;
                    if (act.Approver5 != 0)
                    {
                        nextUserId = act.Approver5;
                    }
                    else
                        statusFlag = true;
                    break;
                }
                case 5:
                {
                    act.is5Approved = true;
                    act.ApprovedBy5On = DateTime.Now;
                    if (act.Approver6 != 0)
                    {
                        nextUserId = act.Approver6;
                    }
                    else
                        statusFlag = true;
                    break;
                }
                case 6:
                {
                    act.is6Approved = true;
                    act.ApprovedBy6On = DateTime.Now;
                    if (act.Approver7 != 0)
                    {
                        nextUserId = act.Approver7;
                    }
                    else
                        statusFlag = true;
                    break;
                }
                case 7:
                {
                    act.is7Approved = true;
                    act.ApprovedBy7On = DateTime.Now;
                    statusFlag = true;
                    break;
                }
                case 8:
                {
                    act.isAddApproved = true;
                    act.ApprovedByAddOn = DateTime.Now;
                    break;
                }
            }

            if (statusFlag)
            {
                act.StatusId = (int)Enums.ActStatusCode.Approved;
                act.OrderColumn = (int)Enums.ActSortCode.Approved;
                _dataManager.tAct.Save(act);
                _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.approvedby, "");
                _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.approved, "");
                //Notify act creator
                var mailto = new List<MailAddress> { new MailAddress(act.CreateByUser.Email) };
                
                string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";

                var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("ActApproved");
                string subject = emailTemplate.Subject;

                var additionalNotify = "";
                if (act.Type == (int)Enums.ActTypeCode.force && !String.IsNullOrEmpty(act.Area.NotifyOnForceActApproved))
                    additionalNotify = act.Area.NotifyOnForceActApproved;
                else if (act.Type == (int)Enums.ActTypeCode.bypass && !String.IsNullOrEmpty(act.Area.NotifyOnBypassActApproved))
                    additionalNotify = act.Area.NotifyOnBypassActApproved;
                else if (act.Type == (int)Enums.ActTypeCode.s2of3 && !String.IsNullOrEmpty(act.Area.NotifyOn2oo3ActApproved))
                    additionalNotify = act.Area.NotifyOn2oo3ActApproved;
                else if ((act.Type == (int)Enums.ActTypeCode.inactive || act.Type == (int)Enums.ActTypeCode.inhibited) && !String.IsNullOrEmpty(act.Area.NotifyOnInactiveActApproved))
                    additionalNotify = act.Area.NotifyOnInactiveActApproved;

                if (!string.IsNullOrEmpty(additionalNotify))
                {                    
                    foreach (var mailS in additionalNotify.Split(";"))
                    {
                        mailto.Add(new MailAddress(mailS));
                    }                    
                }

                string body = emailTemplate.Body                
                    .Replace("@number@", act.Id.ToString())                    
                    .Replace("@ActURL@", actURL);
                subject = subject.Replace("@number@", act.Id.ToString());
                MailHelper.SendMail(mailto, subject, body);
            }
            else if (act.StatusId == (int) Enums.ActStatusCode.InApprovalAdd)
            {
                act.StatusId = (int)Enums.ActStatusCode.Active;
                act.OrderColumn = (int)Enums.ActSortCode.Active;
                _dataManager.tAct.Save(act);
                _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.approvedby, "");                
            }
            else
            {
                _dataManager.tAct.Save(act);
                _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.approvedby, "");
                //Notify next approval
                var mailto = new List<MailAddress> { new MailAddress(_dataManager.tUser.GetUserById(nextUserId).Email) };
                
                string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";

                var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("ActApproval");
                string subject = emailTemplate.Subject;

                string body = emailTemplate.Body
                        .Replace("@number@", act.Id.ToString())
                        .Replace("@ActURL@", actURL);
                MailHelper.SendMail(mailto, subject, body);
            }
            return RedirectToAction(nameof(Index));
        }

        public Dictionary<int, bool> CheckCanCloseAct(int actId)
        {
            Dictionary<int, bool> list = new Dictionary<int, bool>();

            var act = _dataManager.tAct.GetActById(actId);
            if (act == null || act.StatusId != (int)Enums.ActStatusCode.Active)
                return list;

            var flag = true;
            foreach (var actitem in act.ActItems)
            {
                var eventByActItemId = _dataManager.Event.GetEventByActItemId(actitem.Id);
                if (eventByActItemId != null && !eventByActItemId.AddedManually && eventByActItemId.EventDateTimeClear == null)
                {
                    list[actitem.Id] = false;
                    flag = false;
                }
                else
                    list[actitem.Id] = true;
            }
            return flag ? new Dictionary<int, bool>() : list;
        }

        public IActionResult Close(int ActId)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }

            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var check = CheckCanCloseAct(ActId);
            if (check.Count != 0)
            {
                var model = new ActListModel()
                {
                    FlagNotClosed = true
                };
                return RedirectToAction(nameof(Index), new { val = model });
            }
                

            var act = _dataManager.tAct.GetActById(ActId);
            foreach (var actActItem in act.ActItems)
            {
                var eventByActItemId = _dataManager.Event.GetEventByActItemId(actActItem.Id);
                if (eventByActItemId != null && eventByActItemId.AddedManually)
                {
                    eventByActItemId.EventDateTimeClear = DateTime.Now;
                    _dataManager.Event.SaveEvent(eventByActItemId);
                }
            }

            act.StatusId = (int)Enums.ActStatusCode.Closed;
            act.OrderColumn = (int)Enums.ActSortCode.Closed;
            act.ClosedOn = DateTime.Now;
            act.ClosedByUserId = user.Id;
            _dataManager.tAct.Save(act);
            _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.closed, "");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PrintAct(int actid)
        {
            var webReport = new WebReport();
            webReport.Report.Load(System.IO.Path.Combine(_env.WebRootPath + "/Reports/", "Act.frx"));
            webReport.Report.Dictionary.Connections[0].ConnectionString = Config.GetConnectionString();
            webReport.Report.SetParameterValue("actid", actid);
            if (webReport.Report.Prepare())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(webReport.Report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", System.IO.Path.GetFileNameWithoutExtension("ACT_") + actid + ".pdf");
                }
            }
            return View(webReport);
        }
        
        public IActionResult Index(ActListModel val)
        {
            //var userHelper = new UserHelper(_dataManager);
            
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);
            var actListGrid = new List<ActGridRecord>();
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var listPageSizeItems = GeneralHelper.GetDefaultPageItems();            
            var formatS = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            var model = new ActListModel
            {
                ActList = actListGrid,
                CanCreateAct = _dataManager.tUser.CanCreateAkt(User.Identity.Name),
                FlagNotClosed = val.FlagNotClosed,
                SmartSearch = val.SmartSearch,
                ItemPerPageList = new SelectList(listPageSizeItems, "Id", "Name"),
                ItemPerPage = val.ItemPerPage != 0 ? val.ItemPerPage : listPageSizeItems[0].Id,
                PageIndex = val.PageIndex != 0 ? val.PageIndex : 1,
                SelectedActType = val.SelectedActType,
                SelectedActStatus = val.SelectedActStatus,
                DateFrom = val.DateFrom == new DateTime(1, 1, 1) ? new DateTime(2019, 1, 1) : val.DateFrom,                
                DateTo = val.DateTo == new DateTime(1, 1, 1) ? DateTime.Today : val.DateTo
            };
            if (!string.IsNullOrEmpty(val.DateFromS))
                model.DateFrom = DateTime.ParseExact(val.DateFromS, formatS, provider);
            if (!string.IsNullOrEmpty(val.DateToS))
                model.DateTo = DateTime.ParseExact(val.DateToS, formatS, provider);

            model.DateFromS = model.DateFrom.ToString(formatS);
            model.DateToS = model.DateTo.ToString(formatS);            
            model.SelectedFacilityId = val.SelectedFacilityId == 0 ? user.FacilityId : val.SelectedFacilityId;
            model.Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name");

            if (user.isDefaultAreaUsed)
                model.SelectedAreaId = val.SelectedAreaId != 0 ? val.SelectedAreaId : model.CanCreateAct ? user.AreaId : -1;
            else
                model.SelectedAreaId = val.SelectedAreaId != 0 ? val.SelectedAreaId : -1;
            model.Areas = _dataManager.tArea.GetAreasByFacility(model.SelectedFacilityId, true);

            var actList = _dataManager.tAct.GetPaginatedList(model);
            if (actList != null)
            {
                foreach (var item in actList)
                {
                    var record = new ActGridRecord
                    {
                        Id = item.Id,
                        AreaName = userLang == "KK" ? item.Area.Name_KK: userLang == "RU" ? item.Area.Name_RU: item.Area.Name_EN,
                        FacilityName = item.Area.Facility.Name,
                        ActDate = item.CreatedOn,
                        StatusId = item.StatusId,
                        Type =  item.Type,
                        Equipment = item.ActItems.FirstOrDefault()?.Equipment,
                        Unit = item.ActItems.FirstOrDefault()?.Unit
                    };
                    if (item.StatusId == (int)Enums.ActStatusCode.Closed)
                        record.CloseDate = item.ClosedOn;
                    actListGrid.Add(record);
                }
            }
            if (actList != null)
            {
                model.HasNextPage = actList.HasNextPage;
                model.HasPreviousPage = actList.HasPreviousPage;
                model.TotalPages = actList.TotalPages;
                model.TotalEntities = actList.TotalEntities;                
            }
            model.ActList = actListGrid;
            
            return View(model);
        }

        public IActionResult ChangeAct(ActDetailsModel model1)
        {
            if (model1.Act == null)
                return RedirectToAction(nameof(Index));

            if (model1.Act != null && model1.Act.StatusId != (int)Enums.ActStatusCode.Draft)
            {
                return RedirectToAction("Denied", "Access");
            }
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);
            tAct newAct;
            if (model1.Act != null && model1.Act.Id != 0)
            {
                newAct = _dataManager.tAct.GetActById(model1.Act.Id);
                newAct.Type = model1.Act.Type;
                newAct.AreaId = model1.Act.AreaId;
                newAct.ApprovedBy1On = new DateTime(1,1,1);
                newAct.Approver1 = 0;
                newAct.ApprovedBy2On = new DateTime(1, 1, 1);
                newAct.Approver2 = 0;
                newAct.ApprovedBy3On = new DateTime(1, 1, 1);
                newAct.Approver3 = 0;
                newAct.ApprovedBy4On = new DateTime(1, 1, 1);
                newAct.Approver4 = 0;
                newAct.ApprovedBy5On = new DateTime(1, 1, 1);
                newAct.Approver5 = 0;
                newAct.ApprovedBy6On = new DateTime(1, 1, 1);
                newAct.Approver6 = 0;
                newAct.ApprovedBy7On = new DateTime(1, 1, 1);
                newAct.Approver7 = 0;
                newAct.ApprovedByAddOn = new DateTime(1, 1, 1);
                newAct.ApproverAdd = 0;
                newAct.ApprovedByAddOn = new DateTime(1, 1, 1);
                newAct = FillActApprovers(newAct);
            }
            else
            {
                newAct = new tAct()
                {
                    Type = model1.Act.Type,
                    AreaId = model1.Act.AreaId,
                    Crew = model1.Act.Crew,
                    CreatedOn = DateTime.Now,
                    StatusId = (int) Enums.ActStatusCode.Draft,
                    OrderColumn = (int)Enums.ActSortCode.Draft,
                    OriginalLang = userLang,
                    IsTranslated = true,
                    ActItems = new List<tActItems>()
                };
                newAct.ActItems = newAct.ActItems.Append(new tActItems());
            }
            var model = new ActDetailsModel
            {
                Act = newAct,
                Lang = userLang,
            };
            FillActModel(model);
            model.CanBeClosed = CheckCanCloseAct(0);
            return View("ActDetails", model);
        }

        public IActionResult RegisterAct(ActDetailsModel model1)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            tAct newAct;
            if (model1.Act != null)
            {
                newAct = model1.Act;
            }
            else
            {
                newAct = new tAct()
                {
                    Type = 1,
                    AreaId = user.AreaId,
                    ActItems = new List<tActItems>()
                };
                newAct.ActItems = newAct.ActItems.Append(new tActItems());
            }

            newAct.CreatedOn = DateTime.Now;
            newAct.StatusId = (int) Enums.ActStatusCode.Draft;
            newAct.OrderColumn = (int)Enums.ActSortCode.Draft;
            newAct.OriginalLang = userLang;
            newAct.IsTranslated = true;
            newAct.IsCauseTranslated = true;
            newAct.IsProtectTranslated = true;
            newAct.IsImpactTranslated = true;
            
            var model = new ActDetailsModel
            {
                Act = newAct,
                Lang = userLang,
            };
            FillActModel(model);
            model.CanBeClosed = CheckCanCloseAct(0);
            return View("ActDetails", model);
        }

        private void FillActModel(ActDetailsModel model)
        {
            var area = _dataManager.tArea.GetAreaById(model.Act.AreaId);
            model.Type6Disabled = area.Approvers5Disabled;
            if (model.Type6Disabled && model.Act.Type == (int)Enums.ActTypeCode.manual)
            {
                model.Act.Type = (int)Enums.ActTypeCode.force;
                model.Act.ApproverPos1 = area.Approver1_1;
                model.Act.ApproverPos2 = area.Approver1_2;
                model.Act.ApproverPos3 = area.Approver1_3;
                model.Act.ApproverPos4 = area.Approver1_4;
                model.Act.ApproverPos5 = area.Approver1_5;
                model.Act.ApproverPos6 = area.Approver1_6;
                model.Act.ApproverPos7 = area.Approver1_7;
            }                

            if (model.Act.Id == 0) 
                model.Act = FillActApprovers(model.Act);
            model.Positions = new SelectList(_dataManager.tPosition.GetActivePositions(), "Id", "Name");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            model.Areas = _dataManager.tArea.getAreaSelectList(user);
            
            model = FillModelApprovers(model);
            model = FillSelectLists(model);
            model.canEdit = true;
            model.URL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            if (model.Act.Id != 0)
                model.ActHistories = _dataManager.ActHistory.GetHistoryByActId(model.Act.Id);
            else
                model.ActHistories = new List<ActHistory>();
        }

        private ActDetailsModel FillSelectLists(ActDetailsModel model)
        {
            var userLang = model.Lang;
            switch (userLang)
            {
                case "EN":
                    model.CauseList = new SelectList(_dataManager.tActCause.GetActCauses(), "Id", "Name_EN");
                    model.ImpactList = new SelectList(_dataManager.tActImpact.GetActImpacts(), "Id", "Name_EN");
                    model.ProtectList = new SelectList(_dataManager.tActProtect.GetActProtects(), "Id", "Name_EN");
                    break;
                case "RU":
                    model.CauseList = new SelectList(_dataManager.tActCause.GetActCauses(), "Id", "Name_RU");
                    model.ImpactList = new SelectList(_dataManager.tActImpact.GetActImpacts(), "Id", "Name_RU");
                    model.ProtectList = new SelectList(_dataManager.tActProtect.GetActProtects(), "Id", "Name_RU");
                    break;
                case "KK":
                    model.CauseList = new SelectList(_dataManager.tActCause.GetActCauses(), "Id", "Name_KK");
                    model.ImpactList = new SelectList(_dataManager.tActImpact.GetActImpacts(), "Id", "Name_KK");
                    model.ProtectList = new SelectList(_dataManager.tActProtect.GetActProtects(), "Id", "Name_KK");
                    break;
            }
            
            model.Devices = _dataManager.Device.GetDevicesSelectList(true);
            model.BulkUnit = new SelectList(_dataManager.Tag.GetUnitsForBulkInsert(model.Act.AreaId));
            if (model.BulkUnit != null && model.BulkUnit.Any())
                model.BulkEquipment = new SelectList(_dataManager.Tag.GetEquipmentForBulkInsert(model.BulkUnit.First().Text, model.Act.AreaId));

            var listCrew = new List<ListModel>
            {
                new ListModel(1),
                new ListModel(2),
                new ListModel(3),
                new ListModel(4)
            };
            model.CrewList = new SelectList(listCrew, "Id", "Name");
            return model;
        }

        private ActDetailsModel FillModelApprovers(ActDetailsModel model)
        {
            tAct act = model.Act;
            var lang = model.Lang;
            var userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPosAdd, lang.ToUpper()).ToList();
            if (act.ApproverPosAdd != 0)
            {
                if (act.ApproverAdd != 0 && userList.FirstOrDefault(i => i.Id == act.ApproverAdd) == null)
                    userList.Add(_dataManager.tUser.GetUserById(act.ApproverAdd));
                model.ApproversAdd = new SelectList(userList, "Id", $"Name_{lang}");
            }
            userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos1, lang.ToUpper()).ToList();
            if (act.ApproverPos1 != 0)
            {
                if (act.Approver1 !=0 && userList.FirstOrDefault(i => i.Id == act.Approver1) == null)
                    userList.Add(_dataManager.tUser.GetUserById(act.Approver1));
                model.Approvers1 = new SelectList(userList, "Id", $"Name_{lang}");
                if (act.ApproverPos2 != 0)
                {
                    userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos2, lang.ToUpper()).ToList();
                    if (act.Approver2 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver2) == null)
                        userList.Add(_dataManager.tUser.GetUserById(act.Approver2));
                    model.Approvers2 = new SelectList(userList, "Id", $"Name_{lang}");
                    if (act.ApproverPos3 != 0)
                    {
                        userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos3, lang.ToUpper()).ToList();
                        if (act.Approver3 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver3) == null)
                            userList.Add(_dataManager.tUser.GetUserById(act.Approver3));
                        model.Approvers3 = new SelectList(userList, "Id", $"Name_{lang}");
                        if (act.ApproverPos4 != 0)
                        {
                            userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos4, lang.ToUpper()).ToList();
                            if (act.Approver4 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver4) == null)
                                userList.Add(_dataManager.tUser.GetUserById(act.Approver4));
                            model.Approvers4 = new SelectList(userList, "Id", $"Name_{lang}");
                            if (act.ApproverPos5 != 0)
                            {
                                userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos5, lang.ToUpper()).ToList();
                                if (act.Approver5 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver5) == null)
                                    userList.Add(_dataManager.tUser.GetUserById(act.Approver5));
                                model.Approvers5 = new SelectList(userList, "Id", $"Name_{lang}");
                                if (act.ApproverPos6 != 0)
                                {
                                    userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos6, lang.ToUpper()).ToList();
                                    if (act.Approver6 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver6) == null)
                                        userList.Add(_dataManager.tUser.GetUserById(act.Approver6));
                                    model.Approvers6 = new SelectList(userList, "Id", $"Name_{lang}");
                                    if (act.ApproverPos7 != 0)
                                    {
                                        userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos7, lang.ToUpper()).ToList();
                                        if (act.Approver7 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver7) == null)
                                            userList.Add(_dataManager.tUser.GetUserById(act.Approver7));
                                        model.Approvers7 = new SelectList(userList, "Id", $"Name_{lang}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return model;
        }

        private tAct FillActApprovers(tAct act)
        {
            tArea area = _dataManager.tArea.GetAreaById(act.AreaId);
            if (act.Type == (int)Enums.ActTypeCode.force)
            {
                act.ApproverPos1 = area.Approver1_1;
                act.ApproverPos2 = area.Approver1_2;
                act.ApproverPos3 = area.Approver1_3;
                act.ApproverPos4 = area.Approver1_4;
                act.ApproverPos5 = area.Approver1_5;
                act.ApproverPos6 = area.Approver1_6;
                act.ApproverPos7 = area.Approver1_7;
            } else if (act.Type == (int)Enums.ActTypeCode.bypass)
            {
                act.ApproverPos1 = area.Approver2_1;
                act.ApproverPos2 = area.Approver2_2;
                act.ApproverPos3 = area.Approver2_3;
                act.ApproverPos4 = area.Approver2_4;
                act.ApproverPos5 = area.Approver2_5;
                act.ApproverPos6 = area.Approver2_6;
                act.ApproverPos7 = area.Approver2_7;
            }
            else if (act.Type == (int)Enums.ActTypeCode.s2of3)
            {
                act.ApproverPos1 = area.Approver3_1;
                act.ApproverPos2 = area.Approver3_2;
                act.ApproverPos3 = area.Approver3_3;
                act.ApproverPos4 = area.Approver3_4;
                act.ApproverPos5 = area.Approver3_5;
                act.ApproverPos6 = area.Approver3_6;
                act.ApproverPos7 = area.Approver3_7;
            }
            else if (act.Type == (int)Enums.ActTypeCode.manual)
            {
                act.ApproverPos1 = area.Approver5_1;
                act.ApproverPos2 = area.Approver5_2;
                act.ApproverPos3 = area.Approver5_3;
                act.ApproverPos4 = area.Approver5_4;
                act.ApproverPos5 = area.Approver5_5;
                act.ApproverPos6 = area.Approver5_6;
                act.ApproverPos7 = area.Approver5_7;
            }
            else if (act.Type == (int)Enums.ActTypeCode.inactive || act.Type == (int)Enums.ActTypeCode.inhibited)
            {
                act.ApproverPos1 = area.Approver4_1;
                act.ApproverPos2 = area.Approver4_2;
                act.ApproverPos3 = area.Approver4_3;
                act.ApproverPos4 = area.Approver4_4;
                act.ApproverPos5 = area.Approver4_5;
                act.ApproverPos6 = area.Approver4_6;
                act.ApproverPos7 = area.Approver4_7;
            }            
            return act;
        }

        public IActionResult ActDetails(ActDetailsModel request)
        {
            if (request.Id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var act = _dataManager.tAct.GetActById(request.Id ?? default);
            
            //if act not exist
            if (act == null)
            {
                return RedirectToAction(nameof(Index));
            }


            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var userHelper = new UserHelper(_dataManager);
            
            var model = new ActDetailsModel
            {
                Act = act,
                Lang = UserHelper.GetUserLanguage(User.Identity.Name),
                SelectedActStatus = request.SelectedActStatus,
                SelectedFacilityId = request.SelectedFacilityId,
                SelectedAreaId = request.SelectedAreaId,
                SelectedActType = request.SelectedActType,
                PageIndex = request.PageIndex,
                ItemPerPage = request.ItemPerPage,
                SmartSearch = request.SmartSearch
            };
            //var formatS = "dd-MM-yyyy";
            //CultureInfo provider = CultureInfo.InvariantCulture;
            
            if (!string.IsNullOrEmpty(request.DateFrom))
            {
                //model.DateFrom_d = DateTime.ParseExact(request.DateFrom, formatS, provider);
                model.DateFrom = request.DateFrom;
            }                
            if (!string.IsNullOrEmpty(request.DateTo))
            {
                //model.DateTo_d = DateTime.ParseExact(request.DateTo, formatS, provider);
                model.DateTo = request.DateTo;
            }
            FillActModel(model);
            model.canEdit = userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor) && act.Area.FacilityId == user.Area.FacilityId;
            model.CanBeClosed = CheckCanCloseAct(act.Id);

            //if current user can approve
            if (model.Act.StatusId == (int) Enums.ActStatusCode.InApproval)
                model.CanApprove = CanApprove(user, act) != 0;

            //if current user can approve next shift
            if (model.Act.StatusId == (int) Enums.ActStatusCode.InApprovalAdd && model.Act.ApproverAdd != 0 && model.Act.ApproverPosAdd != 0)
            {
                var userAdd = _dataManager.tUser.GetUserById(model.Act.ApproverAdd);
                if (user.PositionId == userAdd.PositionId)
                {
                    model.Act.ApproverAdd = user.Id;
                    model.CanApprove = true;
                    act.ApproverAdd = user.Id;
                    _dataManager.tAct.Save(act);
                }
            }

            var list = new List<tListValue>();
            foreach (var item in _dataManager.tUser.GetActiveUsers())
            {
                var userDB = new tListValue() { Id = item.Id };
                if (user.lang.ToUpper() == "KK")
                    userDB.Value = item.Name_KK;
                else if (user.lang.ToUpper() == "RU")
                    userDB.Value = item.Name_RU;
                else if (user.lang.ToUpper() == "EN")
                    userDB.Value = item.Name_EN;
                list.Add(userDB);
            }
            list = list.OrderBy(i => i.Value).ToList();
            model.Users = list;
            return View("ActDetails", model);
        }

        private int CanApprove(tUser user, tAct act)
        {
            var res = 0;
            if (act.StatusId == (int) Enums.ActStatusCode.InApproval)
            {
                if (!act.is1Approved && act.Approver1 == user.Id)
                {
                    res = 1;
                }
                else if (!act.is2Approved && act.Approver2 == user.Id && act.is1Approved)
                {
                    res = 2;
                }
                else if (!act.is3Approved && act.Approver3 == user.Id && act.is2Approved)
                {
                    res = 3;
                }
                else if (!act.is4Approved && act.Approver4 == user.Id && act.is3Approved)
                {
                    res = 4;
                }
                else if (!act.is5Approved && act.Approver5 == user.Id && act.is4Approved)
                {
                    res = 5;
                }
                else if (!act.is6Approved && act.Approver6 == user.Id && act.is5Approved)
                {
                    res = 6;
                }
                else if (!act.is7Approved && act.Approver7 == user.Id && act.is6Approved)
                {
                    res = 7;
                }
            } else if (act.StatusId == (int) Enums.ActStatusCode.InApprovalAdd)
            {
                if (act.ApproverAdd == user.Id)
                {
                    res = 8;
                }
            }
            return res;
        }
        // POST: tActs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAct(ActDetailsModel ActModel)
        {
            var userHelper = new UserHelper(_dataManager);
            var user =_dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor))
            {
                return RedirectToAction("Denied", "Access");
            }
            
            if ((ActModel.Act.StatusId == (int)Enums.ActStatusCode.Draft && ModelState.IsValid) || ActModel.Act.StatusId != (int)Enums.ActStatusCode.Draft)
            {
                try
                {
                    var act = ActModel.Act;
                    var newFlag = act.Id == 0;
                    var timestamp = DateTime.Now;
                    act.OriginalLang = user.lang.ToUpper();

                    if (act.StatusId == (int) Enums.ActStatusCode.Draft && ActModel.startApporval)
                    {
                        act.StatusId = (int) Enums.ActStatusCode.InApproval;
                        act.OrderColumn = (int)Enums.ActSortCode.InApproval;                        
                    }


                    if (newFlag)
                    {
                        act.CreatedOn = timestamp;
                        act.CreatedByUserId = user.Id;
                    }

                    var actid = _dataManager.tAct.SaveActWithItems(act);
                    act = _dataManager.tAct.GetActById(actid);
                    _dataManager.ActHistory.AddHistory(act.Id, user.Id, newFlag ? (int)Enums.ActHistoryActionCodes.created : (int)Enums.ActHistoryActionCodes.saved, "");
                    

                    if (ActModel.startApporval)
                    {
                        //check if IPL tags on board )))
                        bool isIPL =  _dataManager.tAct.isIPL(actid);
                        if(isIPL)
                        {
                            act.isIPL = true;
                            _dataManager.tAct.Save(act);
                        }
                        _dataManager.ActHistory.AddHistory(act.Id, user.Id, (int)Enums.ActHistoryActionCodes.approvalstarted, "");
                        //Notify first approver                        
                        var mailto = new List<MailAddress> { new MailAddress(_dataManager.tUser.GetUserById(act.Approver1).Email) };
                        
                        string actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";

                        var emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("ActApproval");
                        string subject = emailTemplate.Subject;

                        string body = emailTemplate.Body
                                .Replace("@number@", act.Id.ToString())
                                .Replace("@ActURL@", actURL);
                        MailHelper.SendMail(mailto, subject, body);

                        //Notify other parties
                        if (!String.IsNullOrEmpty(act.Area.NotifyOnActCreationEmails))
                        {
                            mailto = new List<MailAddress>();
                            foreach (var mailS in act.Area.NotifyOnActCreationEmails.Split(";"))
                            {
                                mailto.Add(new MailAddress(mailS));
                            }
                            actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Acts/ActDetails/{act.Id}";
                            emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("NotifyOthers");
                            subject = emailTemplate.Subject;

                            body = emailTemplate.Body
                                .Replace("@number@", act.Id.ToString())
                                .Replace("@ActURL@", actURL);
                            MailHelper.SendMail(mailto, subject, body);
                        }

                        //Notify act translator
                        if (!act.IsTranslated)
                        {
                            mailto = new List<MailAddress>();
                            string translatorEmail = act.Area.Facility.TranslatorEmail;
                            if (!String.IsNullOrEmpty(translatorEmail))
                            {
                                foreach (var mailS in translatorEmail.Split(";"))
                                {
                                    mailto.Add(new MailAddress(mailS));
                                }
                                actURL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/Translation/Translate/{act.Id}";

                                emailTemplate = _dataManager.EmailTemplate.GetEmailTemplateByEmailId("TranslationRequired");
                                subject = emailTemplate.Subject;

                                body = emailTemplate.Body
                                    .Replace("@number@", act.Id.ToString())
                                    .Replace("@ActURL@", actURL);
                                MailHelper.SendMail(mailto, subject, body);
                            }
                        }                        
                        return RedirectToAction(nameof(ActDetails), new { id = actid });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            
            var model = new ActDetailsModel
            {
                Act = ActModel.Act,
                Lang = UserHelper.GetUserLanguage(User.Identity.Name)
            };
            FillActModel(model);
            
            return View("ActDetails", model);
        }
    }
}
