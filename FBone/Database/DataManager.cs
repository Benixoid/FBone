using FBone.Database.DB_Helper;
using FBone.Database.Entities;
using FBone.Models.NMode;
using FBone.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FBone.Database
{
    public class DataManager
    {
        public tUserTable tUser { get; set; }
        public tPositionTable tPosition { get; set; }
        public tActCauseTable tActCause { get; set; }
        public tActImpactTable tActImpact { get; set; }
        public tActProtectTable tActProtect { get; set; }
        public tAreaTable tArea { get; set; }
        public tActTable tAct { get; set; }
        public tActItemsTable tActItems { get; set; }
        public TagTable Tag { get; set; }
        public EventTable Event { get; set; }
        public DeviceTable Device { get; set; }
        public tFacilityTable tFacility { get; set; }
        public ActHistoryTable ActHistory { get; set; }
        public EmailTemplateTable EmailTemplate { get; set; }
        public RequestLogTable RequestLog { get; set; }
        public NMManager NMManager { get; set; }
        public AuditTable AuditTable { get; set; }
        public AuditFileTable AuditFileTable { get; set; }
        public DataManager(tUserTable _tUsersTable, tPositionTable _tPositionTable, tActCauseTable _tActCauseTable, tActImpactTable _tActImpactTable, tActProtectTable _tActProtectTable,
            tAreaTable _tAreaTable, tActTable _tActTable, TagTable _tagTable, EventTable _eventTable, tActItemsTable _tActItemsTable, DeviceTable _deviceTable,
            tFacilityTable _tFacilityTable, ActHistoryTable _actHistory, EmailTemplateTable _emailTemplate, RequestLogTable _requestLog, NMManager _NMManager,
            AuditTable auditTable, AuditFileTable auditFileTable)
        {
            tUser = _tUsersTable;
            tPosition = _tPositionTable;
            tActCause = _tActCauseTable;
            tActImpact = _tActImpactTable;
            tActProtect = _tActProtectTable;
            tArea = _tAreaTable;
            tAct = _tActTable;
            Tag = _tagTable;
            Event = _eventTable;
            tActItems = _tActItemsTable;
            Device = _deviceTable;
            tFacility = _tFacilityTable;
            ActHistory = _actHistory;
            EmailTemplate = _emailTemplate;
            RequestLog = _requestLog;
            NMManager = _NMManager;
            AuditTable = auditTable;
            AuditFileTable = auditFileTable;
        }
        public bool IsUserInRole(ClaimsPrincipal User, Enums.Roles role)
        {
            var user = tUser.GetUserByCAI(User.Identity.Name);
            var userHelper = new UserHelper(this);
            if (!userHelper.IsHasRole(User.Identity.Name, role))
            {
                return false;
            }
            return true;
        }
        public bool IsUserInRoles(ClaimsPrincipal User, Enums.Roles[] roles)
        {
            var user = tUser.GetUserByCAI(User.Identity.Name);
            var userHelper = new UserHelper(this);
            foreach (Enums.Roles role in roles)
            {
                if (IsUserInRole(User, role))
                    return true;
            }
            return false;
        }
    }
}
