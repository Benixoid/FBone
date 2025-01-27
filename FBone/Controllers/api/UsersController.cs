using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Models;
using FBone.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataManager _dataManager;

        public UsersController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        [HttpGet("GetUsersByPosition")]
        public string GetUsersByPosition([FromQuery] int positionId)
        {
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);
            return _dataManager.tUser.GetUsersList(positionId, userLang);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public List<tListValue> GetUsersForPosition([FromRoute] int id)
        {
            var lang = _dataManager.tUser.GetUserByCAI(User.Identity.Name).lang.ToUpper();
            List<tListValue> list = new List<tListValue>();
            var users = _dataManager.tUser.GetUsersByPosition(id, lang);
            
            foreach (var user in users)
            {
                var val = new tListValue() { Id = user.Id};
                if (lang == "KK")
                {
                    val.Value = user.Name_KK;
                }
                else if (lang == "RU")
                {
                    val.Value = user.Name_RU;
                }
                else if (lang == "EN")
                {
                    val.Value = user.Name_EN;
                }
                list.Add(val);
            }
            return list;
        }
    }
}