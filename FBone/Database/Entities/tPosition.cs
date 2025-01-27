using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class tPosition
    {
        public tPosition()
        {
            Id = 0;
            CanCreateAct = false;
            CanTranslateAct = false;
        }

        public int Id { get; set; }

        [MinLength(5)]        
        [Display(Name = "Position name")]
        [Required(ErrorMessage = "Position name is required")]
        public string Name { get; set; }

        [Display(Name = "Is act creator?")]
        public bool CanCreateAct { get; set; }

        [Display(Name = "Is translator?")]
        public bool CanTranslateAct { get; set; }

        [Display(Name = "Is shift engineer?")]
        public bool IsShiftEngineer { get; set; }

        [Display(Name = "Is NMode administrator?"), DefaultValue(false)]
        public bool IsNModeAdministrator { get; set; }

        [Display(Name = "Is NMode editor?"),DefaultValue(false)]
        public bool IsNModeEditor { get; set; }

        [Display(Name = "Is NMode user?"), DefaultValue(false)]
        public bool IsNModeUser { get; set; }
        public IEnumerable<tUser> Users { get; set; }
        //public IEnumerable<tArea> NotifyAreas { get; set; }
        [Display(Name = "Is Audit creator?"), DefaultValue(false)]
        public bool IsAuditCreator { get; set; }

        [Display(Name = "Is Active"), DefaultValue(false)]
        public bool IsActive { get; set; }


    }
}
