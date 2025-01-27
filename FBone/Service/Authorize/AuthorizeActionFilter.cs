using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FBone.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Security.Claims;

namespace FBone.Service.Authorize
{
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string _permission;
        private readonly DataManager _dataManager;

        public AuthorizeActionFilter(string permission, DataManager dataManager)
        {
            _permission = permission;
            _dataManager = dataManager;
        }
        public AuthorizeActionFilter(List<string> permission)
        {
            //_permission = permission;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isAuthorized;
            if (context.HttpContext.Request.Path.Value.Equals("/Home/About") || context.HttpContext.Request.Path.Value.Equals("/Home/ChangeLanguage"))
            {
                isAuthorized = true;
            } else
            {
                isAuthorized = CheckUserPermission(context.HttpContext.User, _permission, context.HttpContext.Request.Path.Value);
            }            
            if (!isAuthorized)
            {                
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Access" },
                    { "action", "Denied" }
                });
            }
        }

        private bool CheckUserPermission(ClaimsPrincipal user, string permission, string path)
        {
            bool res = false;
            var userHelper = new UserHelper(_dataManager);
            if (permission.ToUpper().Equals("ADMIN"))
            {
                res = UserHelper.IsUserAdmin(user.Identity.Name, path);
            }
            else if (permission.ToUpper().Equals("USER"))
            {
                res = UserHelper.IsUserExist(user.Identity.Name, path);
            }
            else if (permission.ToUpper().Equals("USER_SHIFT"))
            {
                res = userHelper.IsHasRole(user.Identity.Name, Enums.Roles.isShiftEngineer);
            }            
            //if (user.IsInGroup("GroupName"))
            //{

            //}
            //var _user = (WindowsIdentity)user.Identity;
            //if (_user.Groups != null)
            //{
            //    foreach (var group in _user.Groups)
            //    {
            //        string gname = group.Translate(typeof(NTAccount)).ToString();
            //    }
            //}
            return res;
        }
    }
}
