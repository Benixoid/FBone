using FBone.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Models.Audits   
{
    public class AuditPaginatedList<Audit> : List<Audit>
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalEntities { get; set; }

        public AuditPaginatedList(IQueryable<Audit> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalEntities = count;
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static AuditPaginatedList<Audit> Create(IQueryable<Audit> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);            
            return new AuditPaginatedList<Audit>(items, count, pageIndex, pageSize);
        }
    }
}