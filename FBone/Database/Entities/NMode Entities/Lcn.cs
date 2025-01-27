using System;
using System.Collections.Generic;

namespace FBone.Models.NMode;

public partial class Lcn:IComparable<Lcn>,IdObject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<NModeRecord> NModeRecords { get; set; } = new List<NModeRecord>();

    public virtual ICollection<NMTotalRecord> NMTotalRecords { get; set; } = new List<NMTotalRecord>();
    public int CompareTo(Lcn other)
    {
        return string.Compare(this.Name, other.Name);
    }
    public override string ToString()
    {
        return Name;
    }
}
