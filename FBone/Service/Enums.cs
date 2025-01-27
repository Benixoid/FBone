using FBone.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FBone.Service
{
    public class Enums
    {
        public enum AuditStatusCode : int
        {
            //used for Audit.StatusCode
            Draft = 1,
            InProgress = 2,
            OnVerification = 3,
            OnApproval = 4,
            Closed = 5            
        };
        public enum Roles : int
        {
            //used for check permissions
            isAktCreatorOrEditor = 1,
            isTranslator = 2,
            isShiftEngineer = 3,
            IsNModeEditor=4,
            IsNModeUser = 5,
            IsNModeAdministrator = 6,
            IsAuditCreatorOrEditor = 7
        };

        public enum ShiftType : int
        {
            //Event.ShiftType
            Night = 1,
            Day = 2            
        };

        public enum ActStatusCode : int
        {
            //used for tAct.StatusId
            Draft = 1,
            InApproval = 2,
            Active = 3,
            Closed = 4,
            Approved = 5,
            InApprovalAdd = 6
        };
        public enum ActSortCode : int
        {
            //used for tAct.OrderColumn
            Draft = 5,
            InApproval = 4,
            Active = 1,
            Closed = 6,
            Approved = 2,
            InApprovalAdd = 3
        };

        public enum EventTypeCode : int
        {
            //used for Event.TypeId
            Force = 1,
            Bypass = 2,
            Inactive = 3,
            Inhibited = 4
        };

        public enum ActTypeCode : int
        {
            //used for tAct.Type
            force = 1,
            bypass = 2,
            s2of3 = 3,
            inactive = 4,
            inhibited = 5,
            manual = 6
        };

        public enum ActHistoryActionCodes : int
        {
            //used for tActHistory.ActionCode
            created = 1,
            saved = 2,
            approvalstarted = 3,
            approvedby = 4,
            rejected = 5,
            delegated = 6,
            approved = 7,
            closed = 8,
            translated = 9,
            copied = 10,
            started = 11,
            approvalstartedadd = 12,
            notifyMOC = 13,
            returntodraft = 14
        };

        public static string GetActStatusName(int id)
        {
            string name = "Status index is out off range!";
            if (id > 0 && id <= Enum.GetNames(typeof(ActStatusCode)).Count())
            {
                name = Enum.GetName(typeof(ActStatusCode), id);
            }
            return name;
        }

        public static List<tListValue> GetEventTypeList()
        {
            List<tListValue> list = new List<tListValue>();
            int i = 1;
            foreach (var item in Enum.GetNames(typeof(EventTypeCode)))
            {
                tListValue fld = new tListValue
                {
                    Id = i,
                    Value = item
                };
                list.Add(fld);
                i++;
            }
            return list;
        }
    }
}
