using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class tActImpactTable
    {
        private readonly AppDBContext _context;
        public tActImpactTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tActImpact> GetActImpacts()
        {
            return _context.tActImpact;
        }

        public tActImpact GetImpactById(int id)
        {
            return _context.tActImpact.FirstOrDefault(i => i.Id == id);
        }

        internal void SaveImpact(tActImpact entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
