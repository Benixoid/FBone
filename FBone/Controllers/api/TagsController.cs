using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("User")]
    public class TagsController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public TagsController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpPost]
        public int PostTag( int deviceid, string tagnumber, string service, int area)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return 0;
            }
            return _dataManager.Tag.CreateOrUpdateTag(deviceid, tagnumber, service, area);
        }
    }
}