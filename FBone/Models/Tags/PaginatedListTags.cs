using System.Linq;
using System;
using FBone.Database.Entities;
using System.Collections.Generic;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using Tag = FBone.Database.Entities.Tag;

namespace Inventory.Models.NetworkDeviceModels
{
    public class PaginatedListTags<NetworkDevice>: List<Tag>
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalEntities { get; set; }

        public PaginatedListTags(IQueryable<Tag> items, int count, int pageIndex, int pageSize)
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

        public static PaginatedListTags<Tag> CreateAsync(IQueryable<Tag> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedListTags<Tag>(items, count, pageIndex, pageSize);
        }
    }
}
