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
    public class tImpactValuesController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public tImpactValuesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }        

        // GET: api/tLookupValues/5
        [HttpGet("{id}")]
        public tActImpact GettLookupValue([FromRoute] int id)
        {
            return _dataManager.tActImpact.GetImpactById(id);
        }
    }
}