using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class tActCauseTable
    {
        private readonly AppDBContext _context;
        public tActCauseTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tActCause> GetActCauses()
        {
            return _context.tActCause;
        }

        public tActCause GetCauseById(int id)
        {
            return _context.tActCause.FirstOrDefault(i => i.Id == id);
        }
        
        internal void SaveCause(tActCause entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
