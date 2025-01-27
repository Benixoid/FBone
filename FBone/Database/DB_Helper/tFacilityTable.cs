using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class tFacilityTable
    {
        private readonly AppDBContext _context;
        public tFacilityTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tFacility> GetFacilities()
        {
            return _context.tFacility;
        }        

        public tFacility GetFacilityById(int id)
        {
            return _context.tFacility.FirstOrDefault(i => i.Id == id);
        }

        internal void SaveFacility(tFacility entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
