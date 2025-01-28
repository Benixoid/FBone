using System;

namespace FBone.Database.Entities
{
    public class AuditHistory
    {
        public int Id { get; set; }
        public int AuditId { get; set; }
        public int HistoryCode { get; set; }
        public Audit Audit { get; set; }
        public DateTime EventDate { get; set; }
        public int ActionUserId { get; set; }
        public tUser ActionUser { get; set; }
        public string Comment { get; set; }
    }
}
