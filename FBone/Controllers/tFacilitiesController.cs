using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Service.Authorize;

namespace FBone.Controllers
{
    [Authorize("Admin")]
    public class tFacilitiesController : Controller
    {
        private readonly DataManager _dataManager;

        public tFacilitiesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index()
        {
            return View(_dataManager.tFacility.GetFacilities());
        }
        
        public IActionResult Create()
        {
            return View("Details", new tFacility());
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tFacility = _dataManager.tFacility.GetFacilityById(id ?? default(int));
            if (tFacility == null)
            {
                return NotFound();
            }
            return View("Details", tFacility);
        }

        // POST: tFacilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Save(int id, [Bind("Id,Name,Force_maxId,Alarm_maxId")] tFacility tFacility)
        public IActionResult Save(int id, [Bind("Id,Name,Force_maxId,Alarm_maxId,TranslatorEmail,TagBypassTotal,TagForcesTotal")] tFacility tFacility)
        {
            if (id != tFacility.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataManager.tFacility.SaveFacility(tFacility);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tFacilityExists(tFacility.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Details", tFacility);
        }

        private bool tFacilityExists(int id)
        {
            return _dataManager.tFacility.GetFacilityById(id) != null;
        }
    }
}
