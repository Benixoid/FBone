using System.Collections.Generic;
using System.Linq;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models.Act;
using FBone.Models.Translation;
using FBone.Service;
using FBone.Service.Authorize;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class TranslationController : Controller
    {
        private readonly DataManager _dataManager;

        public TranslationController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Translate(int? Id, int ItemPerPage, int SelectedFacilityId, int PageIndex)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isTranslator))
            {
                return RedirectToAction("Denied", "Access");
            }

            var act = _dataManager.tAct.GetActById(Id ?? default(int));
            //if act not exist
            if (act == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new ActDetailsModel
            {
                Act = act,
                Lang = UserHelper.GetUserLanguage(User.Identity.Name),
                ItemPerPage = ItemPerPage,
                SelectedFacilityId = SelectedFacilityId,
                PageIndex = PageIndex
            };

            model = FillActModel(model);
            return View(model);
        }

        public IActionResult TranslationConfirmed(ActDetailsModel val)
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isTranslator))
            {
                return RedirectToAction("Denied", "Access");
            }

            var act = _dataManager.tAct.GetActById(val.Act.Id);
            //if act not exist
            if (act == null)
            {
                return RedirectToAction(nameof(Index));
            }

            switch (act.OriginalLang.ToUpper())
            {
                case "RU":
                    if (!act.IsCauseTranslated)
                    {
                        if (val.Act.CauseKK != "")
                            act.CauseKK = val.Act.CauseKK;
                        else
                            ModelState.AddModelError("CauseKK", $"Cause KAZ is empty!");
                        if (val.Act.CauseEN != "")
                            act.CauseEN = val.Act.CauseEN;
                        else
                            ModelState.AddModelError("CauseEN", $"Cause ENG is empty!");
                    }
                    if (!act.IsImpactTranslated)
                    {
                        if (val.Act.ImpactKK != "")
                            act.ImpactKK = val.Act.ImpactKK;
                        else
                            ModelState.AddModelError("ImpactKK", $"Impact KAZ is empty!");
                        if (val.Act.ImpactEN != "")
                            act.ImpactEN = val.Act.ImpactEN;
                        else
                            ModelState.AddModelError("ImpactEN", $"Impact ENG is empty!");
                    }
                    if (!act.IsProtectTranslated)
                    {
                        if (val.Act.ProtectKK != "")
                            act.ProtectKK = val.Act.ProtectKK;
                        else
                            ModelState.AddModelError("ProtectKK", $"Protect KAZ is empty!");
                        if (val.Act.ProtectEN != "")
                            act.ProtectEN = val.Act.ProtectEN;
                        else
                            ModelState.AddModelError("ProtectEN", $"Protect ENG is empty!");
                    }
                    break;
                case "KK":
                    if (!act.IsCauseTranslated)
                    {
                        if (val.Act.CauseRU != "")
                            act.CauseRU = val.Act.CauseRU;
                        else
                            ModelState.AddModelError("CauseRU", $"Cause RUS is empty!");
                        if (val.Act.CauseEN != "")
                            act.CauseEN = val.Act.CauseEN;
                        else
                            ModelState.AddModelError("CauseEN", $"Cause ENG is empty!");
                    }
                    if (!act.IsImpactTranslated)
                    {
                        if (val.Act.ImpactRU != "")
                            act.ImpactRU = val.Act.ImpactRU;
                        else
                            ModelState.AddModelError("ImpactRU", $"Impact RUS is empty!");
                        if (val.Act.ImpactEN != "")
                            act.ImpactEN = val.Act.ImpactEN;
                        else
                            ModelState.AddModelError("ImpactEN", $"Impact ENG is empty!");
                    }
                    if (!act.IsProtectTranslated)
                    {
                        if (val.Act.ProtectRU != "")
                            act.ProtectRU = val.Act.ProtectRU;
                        else
                            ModelState.AddModelError("ProtectRU", $"Protect RUS is empty!");
                        if (val.Act.ProtectEN != "")
                            act.ProtectEN = val.Act.ProtectEN;
                        else
                            ModelState.AddModelError("ProtectEN", $"Protect ENG is empty!");
                    }
                    break;
                case "EN":
                    if (!act.IsCauseTranslated)
                    {
                        if (val.Act.CauseRU != "")
                            act.CauseRU = val.Act.CauseRU;
                        else
                            ModelState.AddModelError("CauseRU", $"Cause RUS is empty!");
                        if (val.Act.CauseKK != "")
                            act.CauseKK = val.Act.CauseKK;
                        else
                            ModelState.AddModelError("CauseKK", $"Cause KAZ is empty!");
                    }
                    if (!act.IsImpactTranslated)
                    {
                        if (val.Act.ImpactRU != "")
                            act.ImpactRU = val.Act.ImpactRU;
                        else
                            ModelState.AddModelError("ImpactRU", $"Impact RUS is empty!");
                        if (val.Act.ImpactKK != "")
                            act.ImpactKK = val.Act.ImpactKK;
                        else
                            ModelState.AddModelError("ImpactKK", $"Impact KAZ is empty!");
                    }
                    if (!act.IsProtectTranslated)
                    {
                        if (val.Act.ProtectRU != "")
                            act.ProtectRU = val.Act.ProtectRU;
                        else
                            ModelState.AddModelError("ProtectRU", $"Protect RUS is empty!");
                        if (val.Act.ProtectKK != "")
                            act.ProtectKK = val.Act.ProtectKK;
                        else
                            ModelState.AddModelError("ProtectKK", $"Protect KAZ is empty!");
                    }
                    break;
            }

            if (!ModelState.IsValid)
            {
                var model = new ActDetailsModel
                {
                    Act = act,
                    Lang = UserHelper.GetUserLanguage(User.Identity.Name)
                };

                model = FillActModel(model);
                return View("Translate", model);
            }
            act.IsCauseTranslated = true;
            act.IsProtectTranslated = true;
            act.IsImpactTranslated = true;
            act.IsTranslated = true;
            _dataManager.tAct.Save(act);
            _dataManager.ActHistory.AddHistory(act.Id,user.Id,(int)Enums.ActHistoryActionCodes.translated,"");
            return RedirectToAction(nameof(Index), new { SelectedFacilityId = act.Area.FacilityId, ItemPerPage = val.ItemPerPage, PageIndex = val.PageIndex});
        }

        public IActionResult Index(TranslationMainModel val)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isTranslator))
            {
                return RedirectToAction("Denied", "Access");
            }
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);

            var listCount = new List<ListModel>
            {
                new ListModel(20),
                new ListModel(50),
                new ListModel(100),
                new ListModel(300)
            };

            var model = new TranslationMainModel
            {
                SelectedFacilityId = val.SelectedFacilityId == 0 ? user.FacilityId : val.SelectedFacilityId,
                ActNumber = val.ActNumber,
                Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name"),
                ItemPerPageList = new SelectList(listCount, "Id", "Name"),
                ItemPerPage = val.ItemPerPage != 0 ? val.ItemPerPage : listCount[0].Id,
                PageIndex = val.PageIndex != 0 ? val.PageIndex : 1
            };
            
            var actListGrid = new List<ActGridRecord>();
            var actList = _dataManager.tAct.GetTranslatorPaginatedList(model);
            if (actList != null)
            {
                foreach (var item in actList)
                {
                    var record = new ActGridRecord
                    {
                        Id = item.Id,
                        AreaName = user.lang == "KK" ? item.Area.Name_KK : user.lang == "RU" ? item.Area.Name_RU : item.Area.Name_EN,
                        FacilityName = item.Area.Facility.Name,
                        ActDate = item.CreatedOn,
                        //IsClosed = item.StatusId == (int)Enums.ActStatusCode.Closed,
                        StatusId = item.StatusId,
                        Type = item.Type
                    };
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

        private ActDetailsModel FillActModel(ActDetailsModel model)
        {
            model.Act = FillActApprovers(model.Act);
            model.Positions = new SelectList(_dataManager.tPosition.GetPositions(), "Id", "Name");
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            model.Areas = _dataManager.tArea.getAreaSelectList(user);

            model = FillModelApprovers(model);
            model.URL = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return model;
        }
        
        private ActDetailsModel FillModelApprovers(ActDetailsModel model)
        {
            tAct act = model.Act;
            var lang = model.Lang;
            var userList = _dataManager.tUser.GetUsersByPosition(act.ApproverPos1, lang.ToUpper()).ToList();
            if (act.ApproverPos1 != 0)
            {
                if (act.Approver1 != 0 && userList.FirstOrDefault(i => i.Id == act.Approver1) == null)
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
            }
            else if (act.Type == (int)Enums.ActTypeCode.bypass)
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
    }
}