using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class NodeReferences
    {
        public int Id { get; set; }

        public int AreaId { get; set; }

        public tArea Area { get; set; }

        public int UCN { get; set; }

        public int Node { get; set; }

        public string Device { get; set; }

        public string SRCNode { get; set; }
    }
}
