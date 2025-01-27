using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FBone.Models;
using FBone.Service;
using FBone.Service.Authorize;
using FBone.Database;
using FBone.Service.WriteToPI;
//using PIWriterStandard;

namespace FBone.Controllers
{
    [Authorize("User")]
    public class HomeController : Controller
    {
        private readonly DataManager _dataManager;
        public HomeController(DataManager dataManager)
        {
            _dataManager = dataManager;            
        }

        private void test()
        {
            PIData pd = new PIData();            
            pd.Add(new PITagData("ALMOILDISABLEDYESTD", new List<PIEvent>()
            {
                new PIEvent(5),
                new PIEvent(7)
            }));
            try
            {
                pd.WriteToPI(Config.PICollectiveName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in write to PI" + ex.Message);
            }
            
        }

        public IActionResult Index()
        {
            var model = new HomePageModel();
            var userHelper = new UserHelper(_dataManager);
            var userLang = UserHelper.GetUserLanguage(User.Identity.Name);

            model.canCreateAct = userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isAktCreatorOrEditor);
            model.canTranslateAct = userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isTranslator);
            model.isShiftEngineer = userHelper.IsHasRole(User.Identity.Name, Enums.Roles.isShiftEngineer);

            //GetPssEvents(model, userLang.ToUpper());
            GetCurrentLocks(model);
            return View(model);
        }

        private void GetCurrentLocks(HomePageModel model)
        {
            var locks = new List<HomePageLocks>();
            foreach (var facility in _dataManager.tFacility.GetFacilities())
            {                
                var f = _dataManager.Event.GetActiveLocksCount(facility.Id);
                f.FacilityName = facility.Name;
                f.FacilityId = facility.Id;
                locks.Add(f);
            }
            model.Locks = locks;
        }

        private void GetPssEvents(HomePageModel model, string lang)
        {
            var facilities = new List<HomePageFacilityEvent>();
            foreach (var facility in _dataManager.tFacility.GetFacilities())
            {
                var f = new HomePageFacilityEvent()
                {
                    Name = facility.Name,
                    Areas = new List<HomePageAreaEvent>()
                };
                var areas = _dataManager.tArea.GetPSSAreasForFacility(facility.Id);
                foreach (var area in areas)
                {
                    var a = new HomePageAreaEvent()
                    {
                        Style = "success"
                    };
                    if (lang == "RU")
                        a.Name = area.Name_RU;
                    else if (lang == "EN")
                        a.Name = area.Name_EN;
                    else if (lang == "KK")
                        a.Name = area.Name_KK;
                    a.LastEventDate = _dataManager.Event.GetLastEventForArea(area.Id);
                    if ((DateTime.Now - a.LastEventDate).TotalHours > 2)
                        a.Style = "danger";
                    f.Areas.Add(a);
                }
                if (f.Areas.Count>0)
                    facilities.Add(f);
            }

            model.Facilities = facilities;
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string lang, string returnURL)
        {
            
            UserHelper.ChangeLanguage(User.Identity.Name, lang);
            return LocalRedirect(returnURL);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
