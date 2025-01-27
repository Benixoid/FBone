using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.DB_Helper
{
    public class RequestLogTable
    {
        private readonly AppDBContext _context;
        public RequestLogTable(AppDBContext context)
        {
            this._context = context;
        }
        public IEnumerable<RequestLog> GetRequestLogs()
        {
            return _context.RequestLogs;
        }        
        internal void SaveRequestLog(RequestLog entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
