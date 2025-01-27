using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.ExtendedProperties;
using FBone.Database;
using FBone.Models.NMode;
using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TestController.Controllers
{
    [Authorize("User")]
    public class NModeAreasController : Controller
    {
        private DataManager DataManager;
        private readonly NMManager manager;
        public NModeAreasController(DataManager dm)
        {
            DataManager = dm;
            manager = DataManager.NMManager;
        }

        // GET: Areas
        public ActionResult Index()
        {
            return manager.Areas != null ?
                       View(manager.Areas.Where(a => a.Name != "All")) :
                       Problem("Entity set 'NmodesContext.Areas'  is null.");
        }

        // GET: Areas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || manager.Areas == null)
            {
                return NotFound();
            }

            var area = manager.Areas.FirstOrDefault(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Areas/Create
        public IActionResult Create()
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Id,Name,StartingHour,SplitToShift,InterpolatedValuesCount")] Area area)
        {
            if (ModelState.IsValid)
            {
                manager.SaveObject(area);
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: Areas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.Areas == null)
            {
                return NotFound();
            }

            var area = manager.Areas.FirstOrDefault(a => a.Id == id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind("Id,Name,StartingHour,SplitToShift,InterpolatedValuesCount")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    manager.SaveObject(area);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.Id))
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
            return View(area);
        }

        // GET: Areas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.Areas == null)
            {
                return NotFound();
            }

            var area = manager.Areas.FirstOrDefault(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (manager.Areas == null)
            {
                return Problem("Entity set 'NmodesContext.Areas'  is null.");
            }
            var area = manager.AreasWithDependencies().FirstOrDefault(a => a.Id == id);
            if (area != null)
            {
                manager.DeleteObject(area);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(int id)
        {
            return (manager.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
