using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBone.Database.Entities
{
    public class Audit
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserID { get; set; }
        public tUser CreatedByUser { get; set; }
        public int StatusCode { get; set; }
        public int FacilityId { get; set; }
        public tFacility Facility { get; set; }
        public int AreaId { get; set; }
        public tArea Area { get; set; }
        [Display(Name = "Shift date")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime ShiftDate { get; set; }        
        public int ActId { get; set; }
        public tAct Act { get; set; }
        
        [Required]
        public string RequiredActionNote { get; set; }
        public string ActionTakenNote { get; set; }
        public int? CompletedByUserId { get; set; }
        public tUser CompletedByUser { get; set; }
        public int ActionOwnerPositionId { get; set; }
        public tPosition ActionOwnerPosition { get; set; }
        public DateTime? ActionCompletedOn { get; set; }

        public string VerificationNote { get; set; }        
        public int? VerifiedByUserId { get; set; }
        public tUser VerifiedByUser { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public string Approval1Note { get; set; }
        public string Approval2Note { get; set; }
        public int? Approved1ByUserId { get; set; }
        public tUser Approved1ByUser { get; set; }
        public int? Approved2ByUserId { get; set; }
        public tUser Approved2ByUser { get; set; }
        public DateTime? Approved1On { get; set; }
        public DateTime? Approved2On { get; set; }
        [Required]
        public string Tags { get; set; }
        public DateTime? CloseDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        public string LinkToVerificationDocs { get; set; }
    }
}
