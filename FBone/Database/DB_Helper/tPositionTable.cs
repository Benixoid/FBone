using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class tPositionTable
    {
        private readonly AppDBContext _context;
        public tPositionTable(AppDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<tPosition> GetPositions()
        {
            return _context.tPosition.OrderBy(i => i.Name);
        }
        public IEnumerable<tPosition> GetActivePositions()
        {
            return _context.tPosition.Where(i=>i.IsActive).OrderBy(i => i.Name);
        }
        public tPosition GetPositionById(int id)
        {
            return _context.tPosition.FirstOrDefault(i => i.Id == id);
        }
        internal void SavePosition(tPosition entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
