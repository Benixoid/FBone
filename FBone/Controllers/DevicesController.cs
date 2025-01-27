using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Service.Authorize;

namespace FBone.Controllers
{
    [Authorize("Admin")]
    public class DevicesController : Controller
    {
        private readonly DataManager _dataManager;

        public DevicesController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // GET: Devices
        public IActionResult Index()
        {
            return View(_dataManager.Device.GetDevices());
        }
        
        // GET: Devices/Create
        public IActionResult Create()
        {
            return View("Details", new Device());
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _dataManager.Device.GetDeviceById(id ?? default(int));
            if (device == null)
            {
                return NotFound();
            }
            return View("Details", device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveDetails(int id, [Bind("Id,Name")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataManager.Device.SaveDevice(device);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_dataManager.Device.GetDeviceById(id) == null)
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
            return View("Details", device);
        }

        // GET: Devices/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _dataManager.Device.GetDeviceById(id ?? default(int));
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dataManager.Device.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
