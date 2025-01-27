using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class TagTable
    {
        private readonly AppDBContext _context;
        public TagTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<Tag> GetTags(tUser user)
        {
            var areas = _context.tArea.Where(i => i.FacilityId == user.FacilityId).AsNoTracking().Select(i => i.Id).ToList();
            if (user.IsAdmin)
            {
                areas = _context.tArea.AsNoTracking().Select(i => i.Id).ToList();
            }
            return _context.Tag
                .Include(i=>i.Area)
                .ThenInclude(f=>f.Facility)
                .Include(i=>i.Device)
                .Where(i=> areas.Contains(i.AreaId)).AsNoTracking();
        }        

        public Tag GetTagById(int id)
        {
            return _context.Tag.FirstOrDefault(i => i.Id == id);
        }

        public int CreateOrUpdateTag(int deviceid, string tagnumber, string service, int area)
        {
            Tag tag = GetTagByName(tagnumber);
            if (tag == null)
            {
                tag = new Tag()
                {
                    Tagnumber = tagnumber,
                    TagnumberByp = tagnumber,
                    AreaId = area,
                    //DeviceId = deviceid,                    
                    Service = service                    
                };
            }
            else
            {
                tag.AreaId = area;
                //tag.DeviceId = deviceid;                
                tag.Service = service;                
            }
            if (deviceid != 0)
                tag.DeviceId = deviceid;
            return SaveTag(tag);
        }

        internal List<Tag> GetBulkTags(string unit, int areaId, string equipment)
        {
            return _context.Tag.Where(i => i.Unit.Equals(unit) && i.isForBulkInsert && i.AreaId == areaId && i.Equipment.Contains(equipment)).ToList();
        }

        internal List<string> GetEquipmentForBulkInsert(string unit, int areaId)
        {
            var list = _context.Tag.Where(i => i.Unit.Equals(unit) && i.isForBulkInsert && i.AreaId == areaId && i.Equipment != null && i.Equipment != "").Select(i => i.Equipment).Distinct().ToList();
            var res = new List<string>();
            foreach (var item in list)
            {
                var equipmentList = item.Split("/-/");
                foreach (var eq in equipmentList)
                {
                    if (res.FirstOrDefault(i=>i.Equals(eq)) == null)
                    {
                        res.Add(eq);
                    }
                }
            }
            res.Sort();
            return res;
        }

        internal int SaveTag(Tag entity)
        {
            if (string.IsNullOrEmpty(entity.TagnumberByp))
                entity.TagnumberByp = entity.Tagnumber;
            _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
            return entity.Id;
        }

        internal Tag GetTagByName(string tagnum)
        {
            return _context.Tag.AsNoTracking().FirstOrDefault(i => i.Tagnumber == tagnum);
        }

        public List<string> GetUnitsForBulkInsert(int areaId)
        {
            return _context.Tag.Where(i => i.isForBulkInsert && i.AreaId == areaId).Select(i=>i.Unit).Distinct().ToList();
        }
        //internal void DeleteTag(Tag tag)
        //{            
        //    _context.Tag.Remove(tag);
        //    _context.SaveChanges();
        //}
    }
}
