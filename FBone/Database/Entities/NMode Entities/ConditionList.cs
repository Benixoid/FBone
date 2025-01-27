using System.Collections.Generic;

namespace FBone.Models.NMode;

public class NModeConditions : List<NModeCondition>
{
    public NModeConditions()
    {
        
    }
    public NModeConditions(IEnumerable<NModeCondition> records) { base.AddRange(records); }

    public override string ToString()
    {
        if (this.Count == 0 || this[0].NModeRecord == null)
            return base.ToString();
        else
        {
            NModeRecord record = this[0].NModeRecord;
            string Ored = record.ConditionORed ? "OR" : "AND";
            string Cop = Ored == "OR" ? "AND" : "OR";
            string condstring = "";
            foreach (var cond in this)
            {
                if (condstring == "")
                    condstring = $"{cond.Tagname} {cond.Operator} {cond.Value}";
                else
                    condstring += $" {Cop} {cond.Tagname} {cond.Operator} {cond.Value}";
            }
            string result = $"{record.Tagname}={record.Nmode} {Ored} ({condstring})";
            return result;
        }
    }
}

