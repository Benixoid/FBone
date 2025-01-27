using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class tUser
    {
        public tUser()
        {
            Id = 0;
            CAI = "ct\\";
            IsAdmin = false;
            IsActive = true;
            Email = "@tengizchevroil.com";
            lang = "ru";
            isDefaultAreaUsed = true;
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "CAI is required")]
        [StringLength(50, MinimumLength = 7)]
        public string CAI { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "User name (ENG)")]
        [Required(ErrorMessage = "User name ENG is required")]
        public string Name_EN { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "User name (RUS)")]
        [Required(ErrorMessage = "User name RUS is required")]
        public string Name_RU { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "User name (KAZ)")]
        [Required(ErrorMessage = "User name KAZ is required")]
        public string Name_KK { get; set; }

        [Display(Name = "Email address")]
        [EmailAddress]
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; }

        [Display(Name = "Default language")]
        [Required(ErrorMessage = "Language is required")]
        public string lang { get; set; }

        [Display(Name = "Is Administrator?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Is User Active?")]
        public bool IsActive { get; set; }

        [Display(Name = "Position name")]
        public int PositionId { get; set; }

        [Display(Name = "Position")]
        public tPosition Position { get; set; }

        [Display(Name = "Default area")]
        public int AreaId { get; set; }

        [Display(Name = "Area")]
        public tArea Area { get; set; }

        public int FacilityId { get; set; }
        public tFacility Facility { get; set; }
        public IEnumerable<ActHistory> ActHistories { get; set; }
        public IEnumerable<tAct> ActCreators { get; set; }
        public IEnumerable<tAct> ActClosers { get; set; }
        [Display(Name = "Open default area?")]
        public bool isDefaultAreaUsed { get; set; }
    }
}
