using FBone.Database;
using FBone.Database.Entities;
using FBone.Models;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Controllers
{
    [Authorize("Admin")]
    public class AdminController : Controller
    {
        private readonly DataManager _dataManager;

        public AdminController(DataManager dataManager)
        {
            _dataManager = dataManager;            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View(_dataManager.tUser.GetUsers());
        }

        public IActionResult Positions()
        {
            return View(_dataManager.tPosition.GetPositions());
        }

        public IActionResult CreatePosition()
        {
            var position = new tPosition();
            return View("PositionDetails", position);
        }
                
        public IActionResult Enums()
        {
            var model = new List<EnumsModel>();

            //
            var record = new EnumsModel();
            record.Name = "ActStatusCode - used for tAct.StatusId";
            var list = new List<tListValue>();            
            foreach (var item in Enum.GetNames(typeof(Enums.ActStatusCode)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.ActStatusCode), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;

            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "Roles - used for check permissions";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.Roles)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.Roles), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "ShiftType - used for Event.ShiftType";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.ShiftType)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.ShiftType), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "ActSortCode - used for tAct.OrderColumn";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.ActSortCode)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.ActSortCode), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "EventTypeCode - used for Event.TypeId";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.EventTypeCode)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.EventTypeCode), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "ActTypeCode - used for tAct.Type";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.ActTypeCode)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.ActTypeCode), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);
            //
            record = new EnumsModel();
            record.Name = "ActHistoryActionCodes - used for tActHistory.ActionCode";
            list = new List<tListValue>();
            foreach (var item in Enum.GetNames(typeof(Enums.ActHistoryActionCodes)))
            {
                var enm = new tListValue();
                enm.Id = (int)Enum.Parse(typeof(Enums.ActHistoryActionCodes), item);
                enm.Value = item;
                list.Add(enm);
            }
            record.EnumsList = list;
            model.Add(record);

            return View(model);
        }
                
        public IActionResult EditPosition(int id)
        {            
            var position = _dataManager.tPosition.GetPositionById(id);
            if (position == null)
            {
                return NotFound();
            }
            return View("PositionDetails", position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePosition(tPosition position)
        {
            if (!ModelState.IsValid)
            {                
                return View("PositionDetails", position);
            }
            tPosition entity;
            if (position.Id == 0)
            {
                entity = new tPosition();
            }
            else
            {
                entity = _dataManager.tPosition.GetPositionById(position.Id);
            }
            entity.Name = position.Name;            
            entity.IsActive = position.IsActive;
            entity.CanCreateAct = position.CanCreateAct;
            entity.CanTranslateAct = position.CanTranslateAct;
            entity.IsShiftEngineer = position.IsShiftEngineer;
            entity.IsAuditCreator = position.IsAuditCreator;
            entity.IsNModeAdministrator = position.IsNModeAdministrator;
            entity.IsNModeEditor = position.IsNModeEditor;
            entity.IsNModeUser  = position.IsNModeUser;
            _dataManager.tPosition.SavePosition(entity);
            return RedirectToAction(nameof(Positions));
        }

        public IActionResult CreateUser()
        {
            //var user = new tUser();
            var model = new tUserViewModel
            {
                User = new tUser(),
                Positions = GetPositions(false),
                Facilities = GetFaclities(false),
                URL = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase)
            };
            model.Areas = GetAreas(int.Parse(model.Facilities.First().Value));
            return View("UserDetails", model);
        }

        public SelectList GetPositions(bool isFirstEmpty)
        {
            IEnumerable<Database.Entities.tPosition> db_posList = null;
            List<Database.Entities.tPosition> res = new List<tPosition>();
            db_posList = _dataManager.tPosition.GetPositions();

            if (db_posList.Count() != 0)
            {
                res = db_posList.ToList();
                if (isFirstEmpty)
                    res.Insert(0, new tPosition { Id = 0, Name = "..." });
            }
            //вернем            
            return new SelectList(res, "Id", "Name");
        }

        public SelectList GetAreas(int facilityId)
        {
            IEnumerable<Database.Entities.tArea> db_posList = null;
            List<Database.Entities.tArea> res = new List<tArea>();
            db_posList = _dataManager.tArea.GetAreas();
            if (facilityId == 0)
                db_posList = new List<tArea>();
            else
                db_posList = db_posList.Where(i => i.FacilityId == facilityId);

            if (db_posList.Count() != 0)
            {
                res = db_posList.ToList();
            }
            //вернем            
            return new SelectList(res, "Id", "Name_EN");
        }
        public IActionResult EditUser(int id)
        {
            var model = new tUserViewModel
            {
                Positions = GetPositions(false),
                Facilities = GetFaclities(false)
            };
            var user = _dataManager.tUser.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            model.User = user;
            model.Areas = GetAreas(user.FacilityId);
            model.URL = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase);
            return View("UserDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUser(tUserViewModel model)
        {
            var user = model.User;
            if (!ModelState.IsValid)
            {
                model.Positions = GetPositions(false);
                model.Facilities = GetFaclities(false);
                model.Areas = GetAreas(user.FacilityId);
                return View("UserDetails", model);
            }
            tUser entity;
            if (user.Id == 0)
            {
                entity = new tUser();
            }
            else
            {
                entity = _dataManager.tUser.GetUserById(user.Id);
            }
            entity.CAI = user.CAI;
            entity.Name_EN = user.Name_EN;
            entity.Name_KK = user.Name_KK;
            entity.Name_RU = user.Name_RU;
            entity.FacilityId = user.FacilityId;
            entity.lang = user.lang;
            entity.Email = user.Email;
            entity.IsActive = user.IsActive;
            entity.IsAdmin = user.IsAdmin;
            entity.isDefaultAreaUsed = user.isDefaultAreaUsed;
            entity.PositionId = user.PositionId;
            entity.AreaId = user.AreaId;
            _dataManager.tUser.SaveUser(entity);
            return RedirectToAction(nameof(Users));
        }

        public IActionResult ActCauses()
        {
            return View(_dataManager.tActCause.GetActCauses());
        }

        public IActionResult ActImpacts()
        {
            return View(_dataManager.tActImpact.GetActImpacts());
        }

        public IActionResult ActProtects()
        {
            return View(_dataManager.tActProtect.GetActProtects());
        }
        public IActionResult EditCause(int id)
        {
            var entity = _dataManager.tActCause.GetCauseById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return View("ActCauseDetails", entity);
        }
        public IActionResult CreateCause()
        {
            var entity = new tActCause();
            return View("ActCauseDetails", entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCause(tActCause actCause)
        {
            if (!ModelState.IsValid)
            {
                return View("ActCauseDetails", actCause);
            }
            tActCause entity;
            if (actCause.Id == 0)
            {
                entity = new tActCause();
            }
            else
            {
                entity = _dataManager.tActCause.GetCauseById(actCause.Id);
            }            
            entity.Name_EN = actCause.Name_EN;
            entity.Name_KK = actCause.Name_KK;
            entity.Name_RU = actCause.Name_RU;            
            _dataManager.tActCause.SaveCause(entity);
            return RedirectToAction(nameof(ActCauses));
        }

        public IActionResult EditImpact(int id)
        {
            var entity = _dataManager.tActImpact.GetImpactById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return View("ActImpactDetails", entity);
        }
        public IActionResult CreateImpact()
        {
            var entity = new tActImpact();
            return View("ActImpactDetails", entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveImpact(tActImpact actImpact)
        {
            if (!ModelState.IsValid)
            {
                return View("ActImpactDetails", actImpact);
            }
            tActImpact entity;
            if (actImpact.Id == 0)
            {
                entity = new tActImpact();
            }
            else
            {
                entity = _dataManager.tActImpact.GetImpactById(actImpact.Id);
            }
            entity.Name_EN = actImpact.Name_EN;
            entity.Name_KK = actImpact.Name_KK;
            entity.Name_RU = actImpact.Name_RU;
            _dataManager.tActImpact.SaveImpact(entity);
            return RedirectToAction(nameof(ActImpacts));
        }

        public IActionResult EditProtect(int id)
        {
            var entity = _dataManager.tActProtect.GetProtectById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return View("ActProtectDetails", entity);
        }
        public IActionResult CreateProtect()
        {
            var entity = new tActProtect();
            return View("ActProtectDetails", entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProtect(tActProtect actProtect)
        {
            if (!ModelState.IsValid)
            {
                return View("ActProtectDetails", actProtect);
            }
            tActProtect entity;
            if (actProtect.Id == 0)
            {
                entity = new tActProtect();
            }
            else
            {
                entity = _dataManager.tActProtect.GetProtectById(actProtect.Id);
            }
            entity.Name_EN = actProtect.Name_EN;
            entity.Name_KK = actProtect.Name_KK;
            entity.Name_RU = actProtect.Name_RU;
            _dataManager.tActProtect.SaveProtect(entity);
            return RedirectToAction(nameof(ActProtects));
        }

        public IActionResult Areas()
        {
            return View(_dataManager.tArea.GetAreas());
        }

        public IActionResult EditArea(int id)
        {
            var entity = _dataManager.tArea.GetAreaById(id);
            ViewData["Positions"] = GetPositions(true);
            ViewData["Facilities"] = GetFaclities(false);
            if (entity == null)
            {
                return NotFound();
            }
            return View("AreaDetails", entity);
        }

        public IActionResult CreateArea()
        {
            var entity = new tArea();
            ViewData["Facilities"] = GetFaclities(false);
            ViewData["Positions"] = GetPositions(true);
            return View("AreaDetails", entity);
        }

        public SelectList GetFaclities(bool isFirstEmpty)
        {
            IEnumerable<Database.Entities.tFacility> db_posList = null;
            List<Database.Entities.tFacility> res = new List<tFacility>();
            db_posList = _dataManager.tFacility.GetFacilities();

            if (db_posList.Count() != 0)
            {
                res = db_posList.ToList();
                if (isFirstEmpty)
                    res.Insert(0, new tFacility() { Id = 0, Name = "..." });
            }
            //вернем            
            return new SelectList(res, "Id", "Name");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveArea(tArea area)
        {
            if (area.IsEventsFromPSS)
            {
                if (string.IsNullOrEmpty(area.ConnectionString))
                    ModelState.AddModelError("ConnectionString", $"ConnectionString is empty!");                
                if (string.IsNullOrEmpty(area.SQLquery))
                    ModelState.AddModelError("SQLquery", $"SQLquery is empty!");
                if (area.MaxId == 0)
                    ModelState.AddModelError("MaxId", $"MaxId is 0!");
                if (string.IsNullOrEmpty(area.SQLqueryAlarm))
                    ModelState.AddModelError("SQLqueryAlarm", $"SQLqueryAlarm is empty!");
                if (area.MaxIdAlarm == 0)
                    ModelState.AddModelError("MaxIdAlarm", $"MaxIdAlarm is 0!");
            }
            if (!ModelState.IsValid)
            {
                ViewData["Facilities"] = GetFaclities(false);
                ViewData["Positions"] = GetPositions(true);
                return View("AreaDetails", area);
            }
            tArea entity;
            if (area.Id == 0)
            {
                entity = new tArea();
            }
            else
            {
                entity = _dataManager.tArea.GetAreaById(area.Id);
            }
            entity.Name_EN = area.Name_EN;
            entity.Name_RU = area.Name_RU;
            entity.Name_KK = area.Name_KK;
            entity.ReportGroup = area.ReportGroup;
            entity.ConnectionString = area.ConnectionString;            
            entity.SQLquery = area.SQLquery;
            entity.SQLqueryAlarm = area.SQLqueryAlarm;
            entity.MaxId= area.MaxId;
            entity.MaxIdAlarm = area.MaxIdAlarm;
            entity.FacilityId = area.FacilityId;
            entity.ShiftEngFacilityId = area.ShiftEngFacilityId;


            entity.Approver1_1 = area.Approver1_1;
            entity.Approver1_2 = area.Approver1_2;
            entity.Approver1_3 = area.Approver1_3;
            entity.Approver1_4 = area.Approver1_4;
            entity.Approver1_5 = area.Approver1_5;
            entity.Approver1_6 = area.Approver1_6;
            entity.Approver1_7 = area.Approver1_7;
            
            entity.Approver2_1 = area.Approver2_1;
            entity.Approver2_2 = area.Approver2_2;
            entity.Approver2_3 = area.Approver2_3;
            entity.Approver2_4 = area.Approver2_4;
            entity.Approver2_5 = area.Approver2_5;
            entity.Approver2_6 = area.Approver2_6;
            entity.Approver2_7 = area.Approver2_7;

            entity.Approver3_1 = area.Approver3_1;
            entity.Approver3_2 = area.Approver3_2;
            entity.Approver3_3 = area.Approver3_3;
            entity.Approver3_4 = area.Approver3_4;
            entity.Approver3_5 = area.Approver3_5;
            entity.Approver3_6 = area.Approver3_6;
            entity.Approver3_7 = area.Approver3_7;

            entity.Approver4_1 = area.Approver4_1;
            entity.Approver4_2 = area.Approver4_2;
            entity.Approver4_3 = area.Approver4_3;
            entity.Approver4_4 = area.Approver4_4;
            entity.Approver4_5 = area.Approver4_5;
            entity.Approver4_6 = area.Approver4_6;
            entity.Approver4_7 = area.Approver4_7;

            entity.Approver5_1 = area.Approver5_1;
            entity.Approver5_2 = area.Approver5_2;
            entity.Approver5_3 = area.Approver5_3;
            entity.Approver5_4 = area.Approver5_4;
            entity.Approver5_5 = area.Approver5_5;
            entity.Approver5_6 = area.Approver5_6;
            entity.Approver5_7 = area.Approver5_7;
            entity.Approvers5Disabled = area.Approvers5Disabled;

            entity.ApproverAdd = area.ApproverAdd;
            entity.IsEventsFromPSS = area.IsEventsFromPSS;
            entity.NotifyOnActCreationEmails = area.NotifyOnActCreationEmails;

            entity.NotifyOnForceActApproved = area.NotifyOnForceActApproved;
            entity.NotifyOn2oo3ActApproved = area.NotifyOn2oo3ActApproved;
            entity.NotifyOnBypassActApproved = area.NotifyOnBypassActApproved;
            entity.NotifyOnInactiveActApproved = area.NotifyOnInactiveActApproved;
            entity.NotifyOnType5ActApproved = area.NotifyOnType5ActApproved;

            entity.NotifyForMocInitiate = area.NotifyForMocInitiate;
            
            entity.TagForcesActive = area.TagForcesActive;
            entity.TagForcesDaily = area.TagForcesDaily;
            entity.TagBypasActive = area.TagBypasActive;
            entity.TagBypasDaily = area.TagBypasDaily;
            entity.TagAlarmDisabled = area.TagAlarmDisabled;
            entity.TagAlarmInhibited = area.TagAlarmInhibited;
            entity.TagAlarmDisabledYestd = area.TagAlarmDisabledYestd;
            entity.TagAlarmInhibitedYestd = area.TagAlarmInhibitedYestd;


            _dataManager.tArea.SaveArea(entity);
            return RedirectToAction(nameof(Areas));
        }
    }
}