using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace FBone.Database.Entities
{
    public class ActHistory
    {
        public int Id { get; set; }
        public int ActId { get; set; }
        public DateTime date { get; set; }
        public int UserId { get; set; }
        public tAct Act { get; set; }
        public tUser User { get; set; }
        public string Comment { get; set; }
        public int ActionCode { get; set; }
    }
}
