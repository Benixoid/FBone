using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class NModeLcnsController : Controller
    {
        private readonly DataManager DataManager;
        private readonly NMManager manager;

        public NModeLcnsController(DataManager dm)
        {
            DataManager = dm;
            manager = DataManager.NMManager;
        }

        // GET: Lcns
        public IActionResult Index()
        {
            if (manager.Lcns != null)
            {
                List<Lcn> Lcns = manager.Lcns.Where(l => l.Name != "All").ToList();
                Lcns.Sort();
                return View(Lcns);
            }
            else
                return Problem("Entity set 'NmodesContext.Lcns'  is null.");
        }

        // GET: Lcns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || manager.Lcns == null)
            {
                return NotFound();
            }

            var lcn = await Task.Run(() => manager.Lcns.FirstOrDefault(m => m.Id == id));
            if (lcn == null)
            {
                return NotFound();
            }

            return View(lcn);
        }

        // GET: Lcns/Create
        public IActionResult Create()
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            return View();
        }

        // POST: Lcns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Lcn lcn)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => manager.SaveObject(lcn));
                return RedirectToAction(nameof(Index));
            }
            return View(lcn);
        }

        // GET: Lcns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.Lcns == null)
            {
                return NotFound();
            }

            var lcn = await Task.Run(() => manager.Lcns.FirstOrDefault(l => l.Id == id));
            if (lcn == null)
            {
                return NotFound();
            }
            return View(lcn);
        }

        // POST: Lcns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Lcn lcn)
        {
            if (id != lcn.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => manager.SaveObject(lcn));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LcnExists(lcn.Id))
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
            return View(lcn);
        }

        // GET: Lcns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!DataManager.IsUserInRole(User, Enums.Roles.IsNModeEditor))
                return RedirectToAction("Denied", "Access");

            if (id == null || manager.Lcns == null)
            {
                return NotFound();
            }

            var lcn = await Task.Run(() => manager.LcnsWithDependencies().FirstOrDefault(m => m.Id == id));
            if (lcn == null)
            {
                return NotFound();
            }

            return View(lcn);
        }

        // POST: Lcns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (manager.Lcns == null)
            {
                return Problem("Entity set 'NmodesContext.Lcns'  is null.");
            }
            var lcn = manager.LcnsWithDependencies().FirstOrDefault(l=>l.Id==id);
            if (lcn != null)
            {
                await Task.Run(()=>manager.DeleteObject(lcn));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LcnExists(int id)
        {
            return (manager.Lcns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
