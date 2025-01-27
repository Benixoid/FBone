using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBone.Database.Entities
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

    }
}
