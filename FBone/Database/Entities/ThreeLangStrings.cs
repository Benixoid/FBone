using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class ThreeLangStrings
    {
        public int Id { get; set; }

        [MinLength(2)]
        [Display(Name = "Name ENG")]
        [Required(ErrorMessage = "Name ENG is required")]
        public string Name_EN { get; set; }

        [MinLength(2)]
        [Display(Name = "Name RUS")]
        [Required(ErrorMessage = "Name RUS is required")]
        public string Name_RU { get; set; }

        [MinLength(2)]
        [Display(Name = "Name KAZ")]
        [Required(ErrorMessage = "Name KAZ is required")]
        public string Name_KK { get; set; }
    }
}
