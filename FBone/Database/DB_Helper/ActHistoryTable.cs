using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class ActHistoryTable
    {
        private readonly AppDBContext _context;
        public ActHistoryTable(AppDBContext context)
        {
            this._context = context;
        }

        public List<ActHistory> GetHistoryByActId(int id)
        {
            return _context.ActHistory.Include(i=>i.User).Where(i=>i.ActId == id).ToList();
        }        

        public ActHistory GetHistoryById(int id)
        {
            return _context.ActHistory.FirstOrDefault(i => i.Id == id);
        }

        internal void SaveHistory(ActHistory entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void AddHistory(int actId, int userId, int actionCode, string comment)
        {
            var actHistory = new ActHistory()
            {
                ActId = actId,
                ActionCode = actionCode,
                date = DateTime.Now,
                UserId = userId,
                Comment = comment
            };
            SaveHistory(actHistory);
        }
    }
}
