using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models.Reporting;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("User")]
    public class EventsController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public EventsController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        
        //GET LIST
        [HttpGet]
        public List<EventGridRecord> GetEvents(string start, string end, int reportit, int eventType)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return null;
            }
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var format = "yyyy-MM-dd HH:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;
            provider = new CultureInfo("ru-RU");
            
            
            start = start.Replace('T', ' ').Replace('Z', ' ').Substring(0,19);
            end = end.Replace('T', ' ').Replace('Z', ' ').Substring(0, 19);

            var startDate = DateTime.ParseExact(start, format, provider);
            var endDate = DateTime.ParseExact(end, format, provider);            
            var list = _dataManager.Event.GetGridEvents(startDate, endDate, reportit, eventType, user.lang.ToUpper(), user.Area.FacilityId).ToList();
            return list;
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent([FromRoute] int id)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return null;
            }

            if (_dataManager.Event.GetEventById(id) == null)
                return NotFound();

            _dataManager.Event.Delete(id);
            return Ok();
        }

        //change ReportIt
        [HttpGet("{id}")]
        public IActionResult ChangeReportIt([FromRoute] int id)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return null;
            }

            var eventEntity = _dataManager.Event.GetEventById(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            eventEntity.ReportIt = !eventEntity.ReportIt;
            _dataManager.Event.SaveEvent(eventEntity);

            return Ok(eventEntity);
        }

        //change event
        [HttpPut("{id}")]
        public IActionResult LinkUnlinkEvent(int id, int actitemid, bool linkFlag, string action)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return null;
            }

            var eventEntity = _dataManager.Event.GetEventById(id);
            if (eventEntity == null)
            {
                return NotFound();
            }

            if (linkFlag)
            {
                var actitem = _dataManager.tActItems.GetActItemById(actitemid);
                
                if (actitem == null)
                {
                    return NotFound();
                }
                eventEntity.ActItemId = actitemid;
                if (actitem.DeviceId!=0)
                {
                    var tag = _dataManager.Tag.GetTagById(eventEntity.TagId);
                    tag.DeviceId = actitem.DeviceId;
                    _dataManager.Tag.SaveTag(tag);
                }
            }
            else
            {
                eventEntity.ActItemId = null;
            }

            if (!string.IsNullOrEmpty(action?.Trim())) {
                eventEntity.Action = action;
            }
            _dataManager.Event.SaveEvent(eventEntity);
            return Ok();
        }

        
        //add new event
        [HttpPost]
        public IActionResult AddEvent(int acttype, int actitemid, int tagid)
        {
            var userHelper = new UserHelper(_dataManager);
            if (!userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer))
            {
                return null;
            }
            _dataManager.Event.CreateNewEvent(acttype, actitemid, tagid);
            return Ok();
        }
    }
}