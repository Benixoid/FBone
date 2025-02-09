using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using FBone.Database.Entities;
using FBone.Models.Act;
using FBone.Models.Audits;
using FBone.Service;
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
        public List<AuditHistory> GetAditHistory(int auditId)
        {
            return _context.AuditHistories
                .Include(x => x.ActionUser)
                .Where(i => i.AuditId == auditId)
                .ToList();
        }
        public AuditHistory CreateAuditHistoryRecord(int audtiId, int userId, int historyCode, string comment)
        {            
            var rec = new AuditHistory()
            {
                AuditId = audtiId,
                EventDate = DateTime.Now,
                ActionUserId = userId,
                Comment = comment,
                HistoryCode = historyCode
            };
            _context.Entry(rec).State = EntityState.Added;            
            _context.SaveChanges();
            return rec;
        }
        public Audit GetAuditById(int id)
        {
            var entity = _context.Audits
                .Include(a => a.CreatedByUser)
                .AsNoTracking()
                .FirstOrDefault(i => i.Id == id);
            if (entity != null)
                _context.Entry(entity).State = EntityState.Detached;
            return entity;            
        }

        public AuditPaginatedList<Audit> GetPaginatedList(AuditListModel val)
        {
            var prepList = _context.Audits
                .Include(i => i.Area)
                .Include(j => j.Area.Facility)
                //.Where(i => i.Area.FacilityId == val.SelectedFacilityId && i.CreatedAt >= val.DateFrom && i.CreatedAt <= val.DateTo.AddDays(1))
                .Where(i => i.CreatedAt >= val.DateFrom && i.CreatedAt <= val.DateTo.AddDays(1))
                .AsNoTracking();

            if (val.SelectedFacilityId > 0)
                prepList = prepList.Where(i => i.FacilityId == val.SelectedFacilityId);

            if (val.SelectedAreaId > 0)
                prepList = prepList.Where(i => i.AreaId == val.SelectedAreaId);

            if (val.SelectedAuditStatus > 0)
            {
                if (val.SelectedAuditStatus == (int)Enums.AuditStatusCode.OnApproval1)
                {
                    prepList = prepList.Where(i => i.StatusCode == (int)Enums.AuditStatusCode.OnApproval1 || i.StatusCode == (int)Enums.AuditStatusCode.OnApproval2);
                }
                else
                {
                    prepList = prepList.Where(i => i.StatusCode == val.SelectedAuditStatus);
                }
            }

            if (!string.IsNullOrEmpty(val.SmartSearch))
            {
                prepList = prepList.Where(i =>
                    i.Id.ToString().Contains(val.SmartSearch)
                    || i.Tags.Contains(val.SmartSearch)                
                );
            }
            prepList = prepList.OrderByDescending(i => i.CreatedAt);
            return AuditPaginatedList<Audit>.Create(prepList, val.PageIndex, val.ItemPerPage);
        }

        public void Save(Audit audit)
        {
            //audit.CreatedByUser = null;
            if (audit.Id == 0)
                _context.Entry(audit).State = EntityState.Added;
            else
                _context.Entry(audit).State = EntityState.Modified;
            _context.SaveChanges();
        }
        internal void DeleteAudit(int auditId)
        {            
            _context.AuditHistories.RemoveRange(GetAditHistory(auditId));
            var auditFile = _context.AuditFiles.FirstOrDefault(i => i.AuditId == auditId);
            if (auditFile != null) 
                _context.AuditFiles.Remove(auditFile);
            var audit = GetAuditById(auditId);
            audit.CreatedByUser = null;
            _context.Audits.Remove(audit);
            _context.SaveChanges();
        }        
    }
}
