using FBone.Database;
using FBone.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public AreaController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        
        [HttpGet("{id}")]
        public SelectList Get([FromRoute] int id)
        {
            return _dataManager.tArea.GetAreasByFacility(id,false);
        }
    }
}
