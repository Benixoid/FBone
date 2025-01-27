using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBone.Database;
using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class tProtectValuesController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public tProtectValuesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }        

        // GET: api/tLookupValues/5
        [HttpGet("{id}")]
        public tActProtect GettLookupValue([FromRoute] int id)
        {
            return _dataManager.tActProtect.GetProtectById(id);
        }
    }
}