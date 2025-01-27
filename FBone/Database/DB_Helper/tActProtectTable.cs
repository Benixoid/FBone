using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class tActProtectTable
    {
        private readonly AppDBContext _context;
        public tActProtectTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tActProtect> GetActProtects()
        {
            return _context.tActProtect;
        }

        public tActProtect GetProtectById(int id)
        {
            return _context.tActProtect.FirstOrDefault(i => i.Id == id);
        }

        internal void SaveProtect(tActProtect entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
