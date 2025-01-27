using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class tActItemsTable
    {
        private readonly AppDBContext _context;
        public tActItemsTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tActItems> GetAllActItems()
        {            
            return _context.tActItems.Include(t => t.Act);
        }
        public IQueryable<tActItems> GetActItemsByActId(int actId)
        {
            return _context.tActItems.Where(i => i.ActId == actId);
        }

        public tActItems GetActItemById(int id)
        {
            return _context.tActItems.Include(i=>i.Act).FirstOrDefault(i => i.Id == id);
        }

        internal tActItems GetActItemByTagname(string tagname)
        {
            return _context.tActItems.FirstOrDefault(i => i.TagName.Equals(tagname));
        }

        internal void SaveItem(tActItems entity)
        {
            _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
