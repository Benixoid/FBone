using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Database;
using FBone.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActComboController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public ActComboController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetActIdsByShiftDateAndArea")]        
        public List<int> GetActIdsByShiftDateAndArea([FromQuery] int areaid, [FromQuery] DateTime shiftDate)
        {
            return _dataManager.tAct.getActListByShiftDateAndArea(shiftDate, areaid).Select(i=>i.Id).ToList();
        }

        [HttpGet("GetActTags")]
        public List<string> GetActTags([FromQuery] int actId)
        {
            return _dataManager.tActItems.GetActItemsByActId(actId).Select(i=>i.TagName).ToList();
        }

        [HttpGet("{id}")]
        public List<string> GettLookupValue([FromRoute] int id, string text)
        {
            List<string> list = new List<string>();
            if (id == 1)
            {
                list = _dataManager.tActItems.GetAllActItems().Where(i => i.TagName.Contains(text)).OrderBy(i => i.TagName).Select(i => i.TagName).Distinct().ToList();
            } else if (id == 2)
            {
                list = _dataManager.tActItems.GetAllActItems().Where(i => i.Unit.Contains(text)).OrderBy(i => i.Unit).Select(i => i.Unit).Distinct().ToList();
            } else if (id == 3)
            {
                list = _dataManager.tActItems.GetAllActItems().Where(i => i.Equipment.Contains(text)).OrderBy(i => i.Equipment).Select(i => i.Equipment).Distinct().ToList();
            }
            if (list.Count < 20)
                return list;
            else 
                return new List<string>();
        }
    }
}