using FBone.Database.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Models;

namespace FBone.Database.DB_Helper
{
    public class tAreaTable
    {
        private readonly AppDBContext _context;
        public tAreaTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<tArea> GetAreas()
        {
            return _context.tArea.Include(i => i.Facility).Include(i=> i.ShiftEngFacility);
        }
        public IQueryable<tArea> GetAreasByFacility(int facilityId)
        {
            return _context.tArea.Where(i=>i.FacilityId == facilityId);
        }

        public tArea GetAreaById(int id)
        {
            return _context.tArea.FirstOrDefault(i => i.Id == id);
        }

        public List<string> GetAllReportGroups()
        {
            return _context.tArea.Where(i=> i.ReportGroup != null && i.ReportGroup !="").Select(i => i.ReportGroup).Distinct().ToList();
        }

        public SelectList GetAreasByFacility(int facilityId, bool isEmptyFirst)
        {
            var groupList = new List<tListValue>();

            if (facilityId != 0)
            {
                var facilities = _context.tArea.Where(l => l.FacilityId == facilityId);
                foreach (var location in facilities)
                {
                    var item = new tListValue() { Id = @location.Id, Value = location.Name_EN };
                    if (groupList.Find(i => i.Id == item.Id) == null)
                        groupList.Add(item);
                }
            }
                
            if (isEmptyFirst)
                groupList.Insert(0, new tListValue { Id = -1, Value = "..." });

            return new SelectList(groupList, "Id", "Value");
        }

        internal List<tArea> GetPSSAreasForFacility(int facilityId)
        {
            return _context.tArea.Where(i => i.IsEventsFromPSS && i.FacilityId == facilityId).ToList();
        }

        internal List<string> GetReportGroupsForFacility(int facilityId)
        {
            var areas = _context.tArea.Where(i => i.ShiftEngFacilityId == facilityId).Select(i => i.Id).ToArray();
            return _context.tArea
                .Where(i => areas.Contains(i.Id) 
                        && i.ReportGroup != null 
                        && i.ReportGroup != "")
                .Select(i => i.ReportGroup)
                .Distinct().ToList();
        }

        internal void SaveArea(tArea entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public SelectList getAreaSelectList(tUser user)
        {
            var list = new List<tListValue>();
            if (user.IsAdmin)
            {
                var listDB = GetAreas().ToList();
                
                foreach (var item in listDB)
                {
                    var ent = new tListValue()
                    {
                        Id = item.Id,
                        Value = "(" + item.Facility.Name + ") "
                    };
                    if (user.lang.ToUpper() == "KK")
                        ent.Value = ent.Value + item.Name_KK;
                    else if (user.lang.ToUpper() == "RU")
                        ent.Value = ent.Value + item.Name_RU;
                    else
                        ent.Value = ent.Value + item.Name_EN;
                    list.Add(ent);
                }
                list.Sort(delegate (tListValue x, tListValue y)
                {
                    if (x.Value == null && y.Value == null) return 0;
                    else if (x.Value == null) return -1;
                    else if (y.Value == null) return 1;
                    else return x.Value.CompareTo(y.Value);
                });
                return new SelectList(list, "Id", $"Value");
            }
            else
            {
                var listDB = GetAreasByFacility(user.FacilityId).ToList();
                foreach (var area in listDB)
                {
                    var ent = new tListValue()
                    {
                        Id = area.Id
                    };
                    if (user.lang.ToUpper() == "KK")
                        ent.Value = ent.Value + area.Name_KK;
                    else if (user.lang.ToUpper() == "RU")
                        ent.Value = ent.Value + area.Name_RU;
                    else
                        ent.Value = ent.Value + area.Name_EN;
                    list.Add(ent);
                }
                list.Sort(delegate (tListValue x, tListValue y)
                {
                    if (x.Value == null && y.Value == null) return 0;
                    else if (x.Value == null) return -1;
                    else if (y.Value == null) return 1;
                    else return x.Value.CompareTo(y.Value);
                });
                return new SelectList(list, "Id", $"Value");
            }
        }
    }
}
