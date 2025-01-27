using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBone.Models.Act
{
    public class DelegateModel
    {
        public int actid { get; set; }
        public int position { get; set; }
        public List<tListValue> Users { get; set; }
        public int selecteduser { get; set; }
    }
}
