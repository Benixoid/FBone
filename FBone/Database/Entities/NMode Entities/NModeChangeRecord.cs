using System;
using System.ComponentModel;

namespace FBone.Models.NMode
{
    public class NModeChangeRecord : IdObject
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public NModeRecord Record { get; set; }
        public DateTime Date { get; set; }
        public string Comment {  get; set; }
        [DisplayName ("Previous Record Config")]
        public string PreviousRecordConfig { get; set; }
        [DisplayName("Change Record Config")]
        public string ChangeRecordConfig { get; set; }
        public string User { get; set; }
        public override string ToString()
        {
            return $"Date: {Date}, Change Config: {ChangeRecordConfig}, Previous Config: {PreviousRecordConfig}, User: {User}, Comment: {Comment}";
        }
    }
}
