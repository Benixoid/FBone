using FBone.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBone.Models.Reporting;
using FBone.Service;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FBone.Models;

namespace FBone.Database.DB_Helper
{
    public class EventTable
    {
        private readonly AppDBContext _context;
        public EventTable(AppDBContext context)
        {
            this._context = context;
        }

        public IQueryable<Event> GetAllEvents()
        {
            return _context.Event;
        }

        public List<EventGridRecord> GetGridEvents(DateTime fromDateTime, DateTime toDateTime, int reportIt, int eventType, string lang, int facilityId)
        {
            /*ReportIt
             * 1 - only true
             * 2 - only false
             * 3 - All
             */

            /*eventType
                1 - Force/Bypass
                2 - Disable/Inhibit
                3 - All
            */

            var list = new List<EventGridRecord>();
            List<Event> list_DB = new List<Event>();
            var areas = _context.tArea.Where(i => i.ShiftEngFacilityId == facilityId).Select(i=>i.Id).ToArray();
            var prep_list =_context.Event
                .Include(i => i.Tag)
                .Include(i => i.Tag.Area)
                .Include(i => i.Tag.Area.Facility)
                .Include(i => i.Tag.Device)
                .Include(i => i.ActItem)
                //.Include(i => i.ActItem.Action)
                .Include(i => i.ActItem.Act)
                .Where(i=> (
                    (i.EventDateTimeSet >= fromDateTime && i.EventDateTimeSet <= toDateTime) //если установлен в этом промежутке времени
                    || (i.EventDateTimeClear == null && i.EventDateTimeSet <= fromDateTime) // не закрытый и установлен до текущейго промежутка
                    || (i.EventDateTimeSet <= fromDateTime && i.EventDateTimeClear > toDateTime) // установлен раннее и закрыт позже текущего промежутка
                    || (i.EventDateTimeClear > fromDateTime && i.EventDateTimeClear < toDateTime) //закрыт в этом промежутке времени
                ) && areas.Contains(i.Tag.AreaId));

            if (eventType == 1)
            {
                prep_list = prep_list.Where(i => i.TypeId == (int) Enums.EventTypeCode.Force || i.TypeId == (int) Enums.EventTypeCode.Bypass);
            } else if (eventType == 2)
            {
                prep_list = prep_list.Where(i => i.TypeId == (int)Enums.EventTypeCode.Inactive || i.TypeId == (int)Enums.EventTypeCode.Inhibited);
            }

            if (reportIt == 1)
                list_DB = prep_list.Where(i=> i.ReportIt)
                    .OrderByDescending(i=>i.EventDateTimeSet).ToList();
            else if (reportIt == 2)
                list_DB = prep_list.Where(i => !i.ReportIt)
                    .OrderByDescending(i => i.EventDateTimeSet).ToList();
            else if (reportIt == 3)
                list_DB = prep_list.OrderByDescending(i => i.EventDateTimeSet).ToList();
            
            foreach (var item in list_DB)
            {
                var record = new EventGridRecord()
                {
                    Id = item.Id,
                    EventType = item.TypeId,
                    Device = item.Tag.Device == null ? "" : item.Tag.Device.Name,                    
                    Tagnumber = item.Tag.Tagnumber,                    
                    Service = item.Tag.Service,
                    SetTimeS = item.EventDateTimeSet.ToString("dd-MM-yyyy HH:mm"),
                    ClearTime = item.EventDateTimeClear,
                    SetTime = item.EventDateTimeSet,
                    ClearTimeS = item.EventDateTimeClear == null ? "" : item.EventDateTimeClear?.ToString("dd-MM-yyyy HH:mm"),
                    ActNum = item.ActItem?.ActId ?? 0,
                    Cause = item.ActItem != null ? lang == "RU" ? item.ActItem.Act.CauseRU : lang == "KK" ? item.ActItem.Act.CauseKK : item.ActItem.Act.CauseEN : "",
                    DataOrigin = item.DataOrigin,
                    AddedManually = item.AddedManually,
                    ReportIt = item.ReportIt,
                    Action = item.Action,
                    Facility = item.Tag.Area.Facility.Name
                };
                if (item.DataOrigin == "PSS")
                    record.DataOrigin = record.DataOrigin + "_" + item.PSSEventId;
                if (lang == "RU")
                    record.Area = item.Tag.Area.Name_RU;
                else if (lang == "KK")
                    record.Area = item.Tag.Area.Name_KK;
                else if (lang == "EN")
                    record.Area = item.Tag.Area.Name_EN;
                list.Add(record);                
            }            
            return list;
        }

        public List<EventGridRecord> GetGridEventsActive(int eventType, string lang, int facilityId)
        {
            var list = new List<EventGridRecord>();
            var listDB = _context.Event.Include(i => i.Tag)
                .Include(i => i.Tag.Area)
                .Include(i => i.Tag.Area.Facility)
                .Include(i => i.Tag.Device)
                .Include(i => i.ActItem)
                .Include(i => i.ActItem.Act)
                .Where(i => i.ReportIt == true && i.EventDateTimeClear == null);
            
            if (facilityId !=0)            
                listDB = listDB.Where(i => i.Tag.Area.FacilityId == facilityId);
            
            if (eventType != 0)
            {
                if (eventType < 100)
                    listDB = listDB.Where(i => i.TypeId == eventType);
                else if (eventType == 150)
                    listDB = listDB.Where(i => (i.TypeId == (int)Enums.EventTypeCode.Bypass || i.TypeId == (int)Enums.EventTypeCode.Force));
                else if (eventType == 160)
                    listDB = listDB.Where(i => (i.TypeId == (int)Enums.EventTypeCode.Inactive || i.TypeId == (int)Enums.EventTypeCode.Inhibited));
            }

            foreach (var item in listDB)
            {
                var record = new EventGridRecord()
                {
                    Id = item.Id,
                    EventType = item.TypeId,
                    Facility = item.Tag.Area.Facility.Name,
                    Device = item.Tag.Device == null ? "" : item.Tag.Device.Name,
                    //Unit = item.Tag.Unit,
                    Tagnumber = item.Tag.Tagnumber,
                    //Type = item.Tag.Type,
                    Service = item.Tag.Service,
                    SetTime = item.EventDateTimeSet,
                    ClearTime = item.EventDateTimeClear,
                    //SetTime = item.EventDateTimeSet.ToString("dd.MM.yyyy HH:mm:ss"),
                    //ClearTime = item.EventDateTimeClear == null ? "" : item.EventDateTimeClear?.ToString("dd.MM.yyyy HH:mm:ss"),
                    ActNum = item.ActItem?.ActId ?? 0,
                    Cause = item.ActItem != null ? lang == "RU" ? item.ActItem.Act.CauseRU : lang == "KK" ? item.ActItem.Act.CauseKK : item.ActItem.Act.CauseEN : "",
                    DataOrigin = item.DataOrigin,
                    AddedManually = item.AddedManually,
                    ReportIt = item.ReportIt
                };
                if (lang == "RU")
                    record.Area = item.Tag.Area.Name_RU;
                else if (lang == "KK")
                    record.Area = item.Tag.Area.Name_KK;
                else if (lang == "EN")
                    record.Area = item.Tag.Area.Name_EN;
                list.Add(record);
            }
            return list;
        }

        internal HomePageLocks GetActiveLocksCount(int facilityId)
        {
            var res = new HomePageLocks
            {
                Forces = _context.Event.Include(i => i.Tag).ThenInclude(i =>
                    i.Area.Facility).Count(i => i.Tag.Area.FacilityId == facilityId && i.TypeId == (int)Enums.EventTypeCode.Force && i.EventDateTimeClear == null && i.ReportIt == true),
                Bypasses = _context.Event.Include(i => i.Tag).ThenInclude(i =>
                    i.Area.Facility).Count(i => i.Tag.Area.FacilityId == facilityId && i.TypeId == (int)Enums.EventTypeCode.Bypass && i.EventDateTimeClear == null && i.ReportIt == true),
                Disabled = _context.Event.Include(i => i.Tag).ThenInclude(i =>
                    i.Area.Facility).Count(i => i.Tag.Area.FacilityId == facilityId && i.TypeId == (int)Enums.EventTypeCode.Inactive && i.EventDateTimeClear == null && i.ReportIt == true),
                Inhibited = _context.Event.Include(i => i.Tag).ThenInclude(i =>
                    i.Area.Facility).Count(i => i.Tag.Area.FacilityId == facilityId && i.TypeId == (int)Enums.EventTypeCode.Inhibited && i.EventDateTimeClear == null && i.ReportIt == true)
            };
            return res;
        }

        internal DateTime GetLastEventForArea(int areaId)
        {
            var entity = _context.Event.Include(i => i.Tag).Where(i => i.Tag.AreaId == areaId && !i.AddedManually)
                .OrderByDescending(i => i.EventDateTimeSet).FirstOrDefault();
            return entity == null ? new DateTime(1, 1, 1) : entity.EventDateTimeSet;
        }

        public Event GetEventById(int id)
        {
            var entity = _context.Event.FirstOrDefault(i => i.Id == id);
            if (entity != null)
                _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        internal int SaveEvent(Event entity)
        {
            if (entity.Id == 0)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity.Id;
        }

        public void Delete(int id)
        {
            _context.Event.Remove(new Event() { Id = id });
            _context.SaveChanges();
        }

        internal Event GetEventByActItemId(int actItemId)
        {
            return _context.Event.FirstOrDefault(i => i.ActItemId == actItemId);
        }

        public int CreateNewEvent(int eventType, int actitemid, int tagid)
        {
            var ai = _context.tActItems.Include(i => i.Act).FirstOrDefault(i => i.Id == actitemid);
            if (ai != null)
            {
                var act = ai.Act;
                var eventEntity = new Event()
                {
                    ActItemId = actitemid,
                    AddedManually = true,
                    DataOrigin = "ACT",
                    ReportIt = true,
                    EventDateTimeSet = act.StartedOn,
                    TagId = tagid,
                    TypeId = eventType
                };

                if (act.StartedOn.Hour >= 19 && act.StartedOn.Hour <= 23)
                {
                    eventEntity.ShiftType = (int)Enums.ShiftType.Night;
                    eventEntity.ShiftDate = new DateTime(act.StartedOn.Year, act.StartedOn.Month, act.StartedOn.Day);
                }
                else if (act.StartedOn.Hour >= 0 && act.StartedOn.Hour < 6)
                {
                    eventEntity.ShiftType = (int)Enums.ShiftType.Night;
                    var date1 = act.StartedOn.AddDays(-1);
                    eventEntity.ShiftDate = new DateTime(date1.Year, date1.Month, date1.Day);
                }
                else
                {
                    eventEntity.ShiftType = (int)Enums.ShiftType.Day;
                    eventEntity.ShiftDate = new DateTime(act.StartedOn.Year, act.StartedOn.Month, act.StartedOn.Day);
                }
                if (act.StatusId == (int)Enums.ActStatusCode.Closed)
                    eventEntity.EventDateTimeClear = act.ClosedOn;

                return SaveEvent(eventEntity);
            }
            return new();
        }
    }
}
