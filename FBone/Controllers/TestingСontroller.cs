using FBone.Database;
using System;
using FBone.Models.Audits;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;
using FBone.Service.Authorize;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class TestingСontroller : Controller
    {
        private readonly DataManager _dataManager;

        public TestingСontroller(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        public IActionResult Index(AuditListModel val)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var listCount = new List<ListModel>
            {
                new ListModel(50),
                new ListModel(200),
                new ListModel(500),
                new ListModel(1000)
            };
            var formatS = "dd-MM-yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            var model = new AuditListModel()
            {
                CanCreateAudit = _dataManager.tUser.CanCreateAudit(User.Identity.Name),
                ItemPerPageList = new SelectList(listCount, "Id", "Name"),
                ItemPerPage = val.ItemPerPage != 0 ? val.ItemPerPage : listCount[0].Id,
                PageIndex = val.PageIndex != 0 ? val.PageIndex : 1,
                SelectedFacilityId = val.SelectedFacilityId == 0 ? user.FacilityId : val.SelectedFacilityId,
                Facilities = new SelectList(_dataManager.tFacility.GetFacilities(), "Id", "Name"),
                SmartSearch = val.SmartSearch,
                SelectedAreaId = val.SelectedAreaId != 0 ? val.SelectedAreaId : -1,
                SelectedAuditStatus = val.SelectedAuditStatus,
                DateFrom = val.DateFrom == new DateTime(1, 1, 1) ? new DateTime(2019, 1, 1) : val.DateFrom,
                DateTo = val.DateTo == new DateTime(1, 1, 1) ? DateTime.Today : val.DateTo

            };
            model.Areas = _dataManager.tArea.GetAreasByFacility(model.SelectedFacilityId, true);
            if (!string.IsNullOrEmpty(val.DateFromS))
                model.DateFrom = DateTime.ParseExact(val.DateFromS, formatS, provider);
            if (!string.IsNullOrEmpty(val.DateToS))
                model.DateTo = DateTime.ParseExact(val.DateToS, formatS, provider);
            model.DateFromS = model.DateFrom.ToString(formatS);
            model.DateToS = model.DateTo.ToString(formatS);           

            model.AuditList = _dataManager.AuditTable.GetAudits();
            return View(model);
        }
    }
}
