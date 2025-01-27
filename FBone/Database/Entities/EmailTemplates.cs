using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
