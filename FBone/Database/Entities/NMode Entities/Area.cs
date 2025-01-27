using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBone.Models.NMode;

public partial class Area : IComparable<Area>, IdObject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [Range(typeof(int), "0", "23")]
    public int StartingHour { get; set; } = 0;
    public bool SplitToShift { get; set; } = true;

    [Range(typeof(int),"1","86400")]
    public int InterpolatedValuesCount { get;set; } = 24;
    public virtual ICollection<NModeRecord> NModeRecords { get; set; } = new List<NModeRecord>();

    public virtual ICollection<NMTotalRecord> NMTotalRecords { get; set; } = new List<NMTotalRecord>();

    public int CompareTo(Area other)
    {
        return string.Compare(this.Name, other.Name);
    }

    public override string ToString()
    {
        return Name;
    }

}
