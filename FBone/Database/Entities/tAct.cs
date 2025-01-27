using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class tAct
    {
        public int Id { get; set; }        
        public int StatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedByUserId { get; set; }
        public tUser CreateByUser { get; set; }

        public DateTime ClosedOn { get; set; }

        public DateTime StartedOn { get; set; }

        public int? ClosedByUserId { get; set; }
        public tUser ClosedByUser { get; set; }

        public int AreaId { get; set; }
        public tArea Area { get; set; }

        public byte Crew { get; set; }

        public byte Type { get; set; }
        public string OriginalLang { get; set; }

        public string CauseRU { get; set; }
        public string CauseKK { get; set; }
        public string CauseEN { get; set; }
        public bool IsCauseTranslated { get; set; }

        public string ImpactRU { get; set; }
        public string ImpactKK { get; set; }
        public string ImpactEN { get; set; }
        public bool IsImpactTranslated { get; set; }

        public string ProtectRU { get; set; }
        public string ProtectKK { get; set; }
        public string ProtectEN { get; set; }
        public bool IsProtectTranslated { get; set; }

        [DataType(DataType.MultilineText)]
        public string ActNotes { get; set; }
        public bool IsTranslated { get; set; }

        [Display(Name = "Approver 1")]
        public int ApproverPos1 { get; set; }
        [Display(Name = "Approver 2")]
        public int ApproverPos2 { get; set; }
        [Display(Name = "Approver 3")]
        public int ApproverPos3 { get; set; }
        [Display(Name = "Approver 4")]
        public int ApproverPos4 { get; set; }
        [Display(Name = "Approver 5")]
        public int ApproverPos5 { get; set; }
        [Display(Name = "Approver 6")]
        public int ApproverPos6 { get; set; }
        [Display(Name = "Approver 7")]
        public int ApproverPos7 { get; set; }        
        [Display(Name = "Additional Approver (prolong to next shift)")]
        public int ApproverPosAdd { get; set; }

        public int Approver1 { get; set; }        
        public int Approver2 { get; set; }
        public int Approver3 { get; set; }        
        public int Approver4 { get; set; }
        public int Approver5 { get; set; }
        public int Approver6 { get; set; }
        public int Approver7 { get; set; }        
        public int ApproverAdd { get; set; }
        public bool is1Approved { get; set; }
        public bool is2Approved { get; set; }
        public bool is3Approved { get; set; }
        public bool is4Approved { get; set; }
        public bool is5Approved { get; set; }
        public bool is6Approved { get; set; }
        public bool is7Approved { get; set; }        
        public bool isAddApproved { get; set; }

        public DateTime ApprovedBy1On { get; set; }
        public DateTime ApprovedBy2On { get; set; }
        public DateTime ApprovedBy3On { get; set; }
        public DateTime ApprovedBy4On { get; set; }
        public DateTime ApprovedBy5On { get; set; }
        public DateTime ApprovedBy6On { get; set; }
        public DateTime ApprovedBy7On { get; set; }        
        public DateTime ApprovedByAddOn { get; set; }

        public IEnumerable<tActItems> ActItems { get; set; }
        public IEnumerable<ActHistory> ActHistories { get; set; }
        public bool Flag72h { get; set; }
        public bool MOCflag { get; set; }
        public bool isIPL { get; set; }
        public int OrderColumn { get; set; }

    }
}
