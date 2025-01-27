using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FBone.Models.Reporting;
using FBone.Models.Act;
using FBone.Service;
using FBone.Models.Translation;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FBone.Database.DB_Helper
{
    public class tActTable
    {
        private readonly AppDBContext _context;
        public tActTable(AppDBContext context)
        {
            this._context = context;
        }

        public ActPaginatedList<tAct> GetPaginatedList(ActListModel val)
        {
            //var format = "dd-MM-yyyy";
            //CultureInfo provider = CultureInfo.InvariantCulture;
            var prepList = _context.tAct
                .Include(i=>i.Area)
                .Include(j=>j.Area.Facility)
                .Include(i => i.ActItems)
                //.Where(i => i.Area.FacilityId == val.SelectedFacilityId && i.CreatedOn >= DateTime.ParseExact(val.DateFromS,format,provider) && i.CreatedOn<= DateTime.ParseExact(val.DateToS, format,provider).AddDays(1))
                .Where(i => i.Area.FacilityId == val.SelectedFacilityId && i.CreatedOn >= val.DateFrom && i.CreatedOn <= val.DateTo.AddDays(1))
                .AsNoTracking();
            
            if (val.SelectedAreaId > 0)
                prepList = prepList.Where(i => i.AreaId == val.SelectedAreaId);

            switch (val.SelectedActStatus)
            {
                case 2:
                    prepList = prepList.Where(i => i.StatusId != 4);
                    break;
                case 3:
                    prepList = prepList.Where(i => i.StatusId == 1);
                    break;
                case 4:
                    prepList = prepList.Where(i => i.StatusId == 2 || i.StatusId == 6);
                    break;
                case 5:
                    prepList = prepList.Where(i => i.StatusId == 3 || i.StatusId == 6);
                    break;
            }

            if (!string.IsNullOrEmpty(val.SmartSearch))
            {
                var actid = _context.tActItems.Where(i => 
                     i.TagName.Contains(val.SmartSearch) 
                  //|| i.Unit.Contains(val.SmartSearch) 
                  //|| i.Equipment.Contains(val.SmartSearch) 
                  //|| i.Location.Contains(val.SmartSearch)
                ).Select(ai => ai.ActId).ToList();

                prepList = prepList.Where(i => 
                    i.Id.ToString().Contains(val.SmartSearch) 
                    || actid.Contains(i.Id) 
                    || i.CauseEN.Contains(val.SmartSearch)
                    || i.CauseKK.Contains(val.SmartSearch)
                    || i.CauseRU.Contains(val.SmartSearch)
                );
            }

            if (val.SelectedActType !=0)
                prepList = prepList.Where(i => i.Type == val.SelectedActType);
            
            prepList = prepList.OrderBy(i => i.OrderColumn).ThenByDescending(i => i.CreatedOn);

            //prepList = prepList.OrderByDescending(i => i.CreatedOn);

            return ActPaginatedList<tAct>.Create(prepList, val.PageIndex, val.ItemPerPage);
        }

        public List<ActItemsReportingGridRecord> GetGridActItems(string lang, int facilityId)
        {
            List<ActItemsReportingGridRecord> list = new List<ActItemsReportingGridRecord>();
            var areas = _context.tArea.Where(i => i.ShiftEngFacilityId == facilityId).Select(i => i.Id).ToArray();
            var listDb = _context.tActItems.FromSqlRaw("SELECT tActItems.* FROM Event right JOIN tActItems ON Event.ActItemId = tActItems.Id where Event.id is null")
                .Include(i => i.Act)
                .ThenInclude(i=>i.Area)
                .Include(i => i.Act.Area.Facility)
                .Where(i=>i.Act.StatusId == (int)Enums.ActStatusCode.Active || i.Act.StatusId == (int)Enums.ActStatusCode.Closed || i.Act.StatusId == (int)Enums.ActStatusCode.InApprovalAdd)
                .Where(i=> areas.Contains(i.Act.AreaId)); 
            foreach (var item in listDb)
            {
                var record = new ActItemsReportingGridRecord
                {
                    Id = item.Id,
                    ActId = item.ActId,
                    ActType = (int)Enums.EventTypeCode.Bypass
                };
                //record.Action = item.Action;
                if (item.Act.Type == (int) Enums.ActTypeCode.s2of3)
                    record.ActType = (int) Enums.EventTypeCode.Force;
                else if (item.Act.Type == (int)Enums.ActTypeCode.inactive)
                    record.ActType = (int)Enums.EventTypeCode.Inactive;
                else if (item.Act.Type == (int)Enums.ActTypeCode.inhibited)
                    record.ActType = (int)Enums.EventTypeCode.Inhibited;
                if (item.DeviceId != 0)
                {
                    var device = _context.Device.FirstOrDefault(i => i.Id == item.DeviceId);
                    if (device != null)
                    {
                        record.Device = device.Name;
                    }
                }                
                record.DeviceId = item.DeviceId;
                //record.Unit = item.Unit;
                record.Equipment = item.Equipment;
                record.TagNumber = item.TagName;
                record.AreaId = item.Act.AreaId;
                record.StartedOn = item.Act.StartedOn == new DateTime(1,1,1) ? "" : item.Act.StartedOn.ToString("dd-MM-yyyy HH:mm");
                record.ClosedOn = item.Act.ClosedOn == new DateTime(1, 1, 1) ? "" : item.Act.ClosedOn.ToString("dd-MM-yyyy HH:mm");
                record.Facility = item.Act.Area.Facility.Name;
                if (lang == "RU")
                {
                    record.Area = item.Act.Area.Name_RU;
                    record.Cause = item.Act.CauseRU;
                }
                else if (lang == "KK")
                {
                    record.Area = item.Act.Area.Name_KK;
                    record.Cause = item.Act.CauseKK;
                }
                else if (lang == "EN")
                {
                    record.Area = item.Act.Area.Name_EN;
                    record.Cause = item.Act.CauseEN;
                }
                list.Add(record);
            }
            return list;
        }

        internal void DeleteAct(int actId)
        {            
            _context.tActItems.RemoveRange(_context.tActItems.Where(i => i.ActId == actId));
            _context.ActHistory.RemoveRange(_context.ActHistory.Where(i => i.ActId == actId));
            _context.tAct.Remove(GetActById(actId));
            _context.SaveChanges();
        }

        public ActPaginatedList<tAct> GetTranslatorPaginatedList(TranslationMainModel model)
        {
            var prepList = _context.tAct.OrderByDescending(i => i.Id)
                .Include(i => i.Area)
                .Include(j => j.Area.Facility)
                .Where(i => i.Area.FacilityId == model.SelectedFacilityId && i.IsTranslated == false && i.StatusId != (int)Enums.ActStatusCode.Draft)
                .AsNoTracking();
            if (model.ActNumber !=0)
            {
                prepList = prepList.Where(i => i.Id==model.ActNumber);
            }
            return ActPaginatedList<tAct>.Create(prepList, model.PageIndex, model.ItemPerPage);
        }

        public IQueryable<tAct> GetAllActs()
        {            
            var list = _context.tAct.Include(t => t.Area);            
            return list;
        }

        public tAct GetActById(int id)
        {
            var entity = _context.tAct.Include(i => i.ActItems).Include(i=>i.Area).Include(i=>i.CreateByUser).Include(i=>i.Area.Facility).FirstOrDefault(i => i.Id == id);
            if (entity != null)
                _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
                
        public int Save(tAct entity)
        {
            _context.Entry(entity).State = entity.Id == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
            return entity.Id;
        }

        public int SaveActWithItems(tAct act)
        {
            var actId = 0;
            var updateItemsFlag = false;
            //Save Act
            if (act.Id == 0)
            {
                actId = Save(act);
                updateItemsFlag = true;
            }
            else
            {
                actId = act.Id;
                var entity = _context.tAct.FirstOrDefault(i => i.Id == actId);
                if (entity == null)
                    return 0;

                if (entity.StatusId == (int) Enums.ActStatusCode.Draft)
                {
                    _context.Entry(entity).State = EntityState.Detached;
                    actId = Save(act);
                    updateItemsFlag = true;
                }
                else if(entity.StatusId != (int)Enums.ActStatusCode.Closed)
                {
                    //save only comment changed
                    entity.ActNotes = act.ActNotes;
                    actId = Save(entity);
                }
            }

            if (act.StatusId == (int) Enums.ActStatusCode.Draft || updateItemsFlag)
            {
                //Save act items
                var listFromUI = act.ActItems.ToList();

                var listFromDB = _context.tActItems.Where(i => i.ActId == actId).ToList();

                foreach (var item in listFromDB)
                {
                    var temp = listFromUI.Find(e => e.Id == item.Id);
                    if (temp == null)
                    {
                        //delete removed
                        var entity = _context.tActItems.FirstOrDefault(i => i.Id == item.Id);
                        if (entity != null)
                        {
                            _context.tActItems.Remove(entity);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        //update existing
                        item.TagName = temp.TagName.Trim();
                        item.Unit = temp.Unit;
                        item.Equipment = temp.Equipment;
                        item.Location = temp.Location;
                        item.DeviceId = temp.DeviceId;
                        item.Action = temp.Action;
                        _context.Entry(item).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                //add new items
                foreach (var item in listFromUI)
                {
                    if (item.Id == 0)
                    {
                        var entity = new tActItems()
                        {
                            ActId = actId,
                            TagName = item.TagName.Trim(),
                            Unit = item.Unit,
                            Equipment = item.Equipment,
                            Location = item.Location,
                            DeviceId = item.DeviceId,
                            Action = item.Action
                        };
                        _context.Entry(entity).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                }
            }
            return actId;
        }

        public bool isIPL(int actid)
        {
            var tags = _context.tActItems.Where(i => i.ActId == actid).AsNoTracking().Select(i => i.TagName).ToList();
            if (tags.Any())
                return _context.Tag.Where(i => tags.Contains(i.Tagnumber) && i.isIPL).Any();
            return false;
        }

        internal List<tAct> getActByShiftDateAndArea(DateTime shiftDate, int areaId)
        {
            DateTime start = new DateTime(shiftDate.Year, shiftDate.Month, shiftDate.Day, 6, 0, 0);
            DateTime end = start.AddDays(1);
            return _context.tAct
                .Where(a => 
                    a.AreaId == areaId &&
                    a.StartedOn >= start && 
                    a.StartedOn < end &&
                    (a.StatusId == (int)Enums.ActStatusCode.Active ||
                    a.StatusId == (int)Enums.ActStatusCode.InApprovalAdd ||
                    a.StatusId == (int)Enums.ActStatusCode.Closed)
                ).ToList();
        }
    }
}
