using System;

namespace FBone.Database.Entities
{
    public class RequestLog
    {
        public int Id { get; set; }
        public string CAI { get; set; }
        public DateTime RequestTime { get; set; }
        public string RequestURL { get; set; }
        public string Comment { get; set; }
    }
}
