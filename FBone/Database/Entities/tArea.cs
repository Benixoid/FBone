using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class tArea : ThreeLangStrings
    {
        public IEnumerable<tAct> Acts { get; set; }
        public string ConnectionString { get; set; }        
        public string SQLquery { get; set; }
        public string SQLqueryAlarm { get; set; }
        public long MaxId { get; set; }
        public long MaxIdAlarm { get; set; }
        public DateTime LastImportDate { get; set; }
        public int LastRecordId { get; set; }
        [Display(Name = "Approver 1")]
        public int Approver1_1 { get; set; }
        [Display(Name = "Approver 2")]
        public int Approver1_2 { get; set; }
        [Display(Name = "Approver 3")]
        public int Approver1_3 { get; set; }
        [Display(Name = "Approver 4")]
        public int Approver1_4 { get; set; }
        [Display(Name = "Approver 5")]
        public int Approver1_5 { get; set; }
        [Display(Name = "Approver 6")]
        public int Approver1_6 { get; set; }
        [Display(Name = "Approver 7")]
        public int Approver1_7 { get; set; }
        [Display(Name = "Approver 1")]
        public int Approver2_1 { get; set; }
        [Display(Name = "Approver 2")]
        public int Approver2_2 { get; set; }
        [Display(Name = "Approver 3")]
        public int Approver2_3 { get; set; }
        [Display(Name = "Approver 4")]
        public int Approver2_4 { get; set; }
        [Display(Name = "Approver 5")]
        public int Approver2_5 { get; set; }
        [Display(Name = "Approver 6")]
        public int Approver2_6 { get; set; }
        [Display(Name = "Approver 7")]
        public int Approver2_7 { get; set; }
        [Display(Name = "Approver 1")]
        public int Approver3_1 { get; set; }
        [Display(Name = "Approver 2")]
        public int Approver3_2 { get; set; }
        [Display(Name = "Approver 3")]
        public int Approver3_3 { get; set; }
        [Display(Name = "Approver 4")]
        public int Approver3_4 { get; set; }
        [Display(Name = "Approver 5")]
        public int Approver3_5 { get; set; }
        [Display(Name = "Approver 6")]
        public int Approver3_6 { get; set; }
        [Display(Name = "Approver 7")]
        public int Approver3_7 { get; set; }
        [Display(Name = "Approver 1")]
        public int Approver4_1 { get; set; }
        [Display(Name = "Approver 2")]
        public int Approver4_2 { get; set; }
        [Display(Name = "Approver 3")]
        public int Approver4_3 { get; set; }
        [Display(Name = "Approver 4")]
        public int Approver4_4 { get; set; }
        [Display(Name = "Approver 5")]
        public int Approver4_5 { get; set; }
        [Display(Name = "Approver 6")]
        public int Approver4_6 { get; set; }
        [Display(Name = "Approver 7")]
        public int Approver4_7 { get; set; }
        [Display(Name = "Approver 1")]
        public int Approver5_1 { get; set; }
        [Display(Name = "Approver 2")]
        public int Approver5_2 { get; set; }
        [Display(Name = "Approver 3")]
        public int Approver5_3 { get; set; }
        [Display(Name = "Approver 4")]
        public int Approver5_4 { get; set; }
        [Display(Name = "Approver 5")]
        public int Approver5_5 { get; set; }
        [Display(Name = "Approver 6")]
        public int Approver5_6 { get; set; }
        [Display(Name = "Approver 7")]
        public int Approver5_7 { get; set; }
        [Display(Name = "Disabled")] 
        public bool Approvers5Disabled { get; set; }

        [Display(Name = "Additional approver")]
        public int ApproverAdd { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<tUser> Users { get; set; }
        public IEnumerable<NodeReferences> References { get; set; }
        public int FacilityId { get; set; }
        
        public tFacility Facility { get; set; }

        [Display(Name = "Resposible Shift Engineer")]
        public int ShiftEngFacilityId { get; set; }
        public tFacility ShiftEngFacility { get; set; }
        //public tPosition Notifier { get; set; }
        [Display(Name = "Area gets events from PSS")]
        public bool IsEventsFromPSS { get; set; }
        public string ReportGroup { get; set; }
        
        public string TagForcesActive { get; set; }
        public string TagForcesDaily { get; set; }
        public string TagBypasActive { get; set; }
        public string TagBypasDaily { get; set; }
        public string TagAlarmDisabled { get; set; }
        public string TagAlarmInhibited { get; set; }
        public string TagAlarmDisabledYestd { get; set; }
        public string TagAlarmInhibitedYestd { get; set; }

        [Display(Name = "Emails to notify when new act approval is started, separated by ;")]
        public string NotifyOnActCreationEmails { get; set; }

        [Display(Name = "Additional emails to notify when 'Force' act fully approved, separated by ;")]
        public string NotifyOnForceActApproved { get; set; }
        [Display(Name = "Additional emails to notify when 'Bypass' act fully approved, separated by ;")]
        public string NotifyOnBypassActApproved { get; set; }

        [Display(Name = "Additional emails to notify when '2 oo 3' act fully approved, separated by ;")]
        public string NotifyOn2oo3ActApproved { get; set; }

        [Display(Name = "Additional emails to notify when 'Inactive' act fully approved, separated by ;")]
        public string NotifyOnInactiveActApproved { get; set; }
        [Display(Name = "Emails to send a notification about the need to initiate the MOC, separated by ;")]
        public string NotifyForMocInitiate { get; set; }
        [Display(Name = "Additional emails to notify when 'Manual Startup override' act fully approved, separated by ;")]
        public string NotifyOnType5ActApproved { get; set; }
        [Display(Name = "Audit verificator position")]
        public int? VerificatorId { get; set; }
        public tPosition Verificator { get; set; }
        [Display(Name = "ASD tags(acts) approver")]
        public int? AsdApproverId { get; set; }
        public tPosition AsdApprover { get; set; }
    }
}
