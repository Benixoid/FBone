using FBone.Database;
using FBone.Database.Entities;
using FBone.Models.Reporting;
using FBone.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActItemController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public ActItemController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("{id}")]
        public tActItems Get([FromRoute] int id, string tagname)
        {
            return _dataManager.tActItems.GetActItemByTagname(tagname);
        }

        [HttpGet]
        public List<ActItemsReportingGridRecord> GetActItems()
        {
            var userHelper = new UserHelper(_dataManager);
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return new List<ActItemsReportingGridRecord>();
            }
            return _dataManager.tAct.GetGridActItems(user.lang.ToUpper(), user.Area.FacilityId);
        }
    }
}
