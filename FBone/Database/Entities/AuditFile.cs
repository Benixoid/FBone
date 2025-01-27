using System;

namespace FBone.Database.Entities
{
    public class AuditFile
    {
        public int Id { get; set; }
        public int? AuditId { get; set; }
        public Audit Audit { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
