using FBone.Database.Entities;
using FBone.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class DeviceTable
    {
        private readonly AppDBContext _context;
        public DeviceTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<Device> GetDevices()
        {
            return _context.Device.OrderBy(i=>i.Name);
        }        

        public Device GetDeviceById(int id)
        {
            return _context.Device.FirstOrDefault(i => i.Id == id);
        }

        internal void SaveDevice(Device entity)
        {
            _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            _context.Device.Remove(new Device() { Id = id });
            _context.SaveChanges();
        }
              

        public SelectList GetDevicesSelectList(bool isEmptyFirst)
        {
            var deviceList = new List<tListValue>();

            var devices = _context.Device.OrderBy(i=>i.Name).ToList();
            foreach (var device in devices)
                {
                    var item = new tListValue() { Id = device.Id, Value = device.Name };
                    if (deviceList.Find(i => i.Id == item.Id) == null)
                        deviceList.Add(item);
                }            

            if (isEmptyFirst)
                deviceList.Insert(0, new tListValue { Id = 0, Value = "..." });

            return new SelectList(deviceList, "Id", "Value");
        }
    }
}
