using FBone.Database.Entities;
using FBone.Models.Act;
using FBone.Models.Audits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Database.DB_Helper
{
    public class AuditTable
    {
        private readonly AppDBContext _context;
        public AuditTable(AppDBContext context)
        {
            this._context = context;
        }

        public List<Audit> GetAudits()
        {
            return _context.Audits.ToList();
        }
        public Audit GetAuditById(int id)
        {
            return _context.Audits
                .Include(a => a.CreatedByUser)                
                .AsNoTracking()
                .FirstOrDefault(i => i.Id == id);
        }

        public AuditPaginatedList<Audit> GetPaginatedList(AuditListModel val)
        {
            //var format = "dd-MM-yyyy";
            //CultureInfo provider = CultureInfo.InvariantCulture;
            var prepList = _context.Audits
                .Include(i => i.Area)
                .Include(j => j.Area.Facility)
                //.Include(k=>k.CreatedByUser)
                //.Where(i => i.Area.FacilityId == val.SelectedFacilityId && i.CreatedOn >= DateTime.ParseExact(val.DateFromS,format,provider) && i.CreatedOn<= DateTime.ParseExact(val.DateToS, format,provider).AddDays(1))
                .Where(i => i.Area.FacilityId == val.SelectedFacilityId && i.CreatedAt >= val.DateFrom && i.CreatedAt <= val.DateTo.AddDays(1))
                .AsNoTracking();

            if (val.SelectedAreaId > 0)
                prepList = prepList.Where(i => i.AreaId == val.SelectedAreaId);

            switch (val.SelectedAuditStatus)
            {
                case 2:
                    prepList = prepList.Where(i => i.StatusCode != 4);
                    break;
                case 3:
                    prepList = prepList.Where(i => i.StatusCode == 1);
                    break;
                case 4:
                    prepList = prepList.Where(i => i.StatusCode == 2 || i.StatusCode == 6);
                    break;
                case 5:
                    prepList = prepList.Where(i => i.StatusCode == 3 || i.StatusCode == 6);
                    break;
            }

            if (!string.IsNullOrEmpty(val.SmartSearch))
            {
                //var actid = _context.tActItems.Where(i =>
                //     i.TagName.Contains(val.SmartSearch)
                ////|| i.Unit.Contains(val.SmartSearch) 
                ////|| i.Equipment.Contains(val.SmartSearch) 
                ////|| i.Location.Contains(val.SmartSearch)
                //).Select(ai => ai.ActId).ToList();

                prepList = prepList.Where(i =>
                    i.Id.ToString().Contains(val.SmartSearch)
                    //|| actid.Contains(i.Id)
                    || i.Name.Contains(val.SmartSearch)
                    //|| i.CauseEN.Contains(val.SmartSearch)
                    //|| i.CauseKK.Contains(val.SmartSearch)
                    //|| i.CauseRU.Contains(val.SmartSearch)
                );
            }

            //if (val.SelectedActType != 0)
            //    prepList = prepList.Where(i => i.Type == val.SelectedActType);

            prepList = prepList.OrderByDescending(i => i.CreatedAt);

            //prepList = prepList.OrderByDescending(i => i.CreatedOn);

            return AuditPaginatedList<Audit>.Create(prepList, val.PageIndex, val.ItemPerPage);
        }

        public void Save(Audit audit)
        {
            if (audit.Id == 0)
                _context.Audits.Add(audit);
            else
                _context.Audits.Update(audit);
            _context.SaveChanges();
        }
    }
}
