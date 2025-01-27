using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Models.Act    
{
    public class ActPaginatedList<tAct> : List<tAct>
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalEntities { get; set; }

        public ActPaginatedList(IQueryable<tAct> items, int count, int pageIndex, int pageSize)
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

        public static ActPaginatedList<tAct> Create(IQueryable<tAct> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);            
            return new ActPaginatedList<tAct>(items, count, pageIndex, pageSize);
        }
    }
}