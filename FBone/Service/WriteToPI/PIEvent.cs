using PISDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBone.Service.WriteToPI
{
    public class PIEvent
    {
        public PIEvent() { }
        public PIEvent(object ts, object value)
        {
            TimeStamp = ts;
            Value = value;
        }
        public PIEvent(object value)
        {
            TimeStamp = DateTime.Now;
            Value = value;
        }
        public object TimeStamp { get; set; }
        public object Value { get; set; }
    }

    public class TagValue
    {
        private double _DValue;
        private string _SValue = string.Empty;
        public bool _IsString = false;

        public TagValue() { }
        public TagValue(PISDK.PIValue val)
        {
            //if (val.Value is DigitalState)
            //    Value = ((DigitalState)val.Value).Name;
            //else
            Value = val.Value;
            TimeStamp = val.TimeStamp.LocalDate;
        }
        public object Value
        {
            get
            {
                if (_IsString)
                    return SValue;
                else
                    return DValue;
            }
            set
            {
                ValueType = value.GetType().ToString();

                if (value is DigitalState)
                {
                    DigitalState ds = (DigitalState)value;
                    _SValue = ds.Name;
                    _DValue = ds.Code;
                }
                else
                {
                    if (!double.TryParse(value.ToString(), out _DValue))
                    {
                        _IsString = true;
                        _SValue = value.ToString();
                    }
                }
            }
        }


        public string ValueType { get; private set; }
        public bool IsString
        {
            get
            {
                return _IsString;
            }
            private set
            {
                _IsString = value;
            }
        }

        public double DValue
        {
            get
            {
                return _DValue;
            }
        }
        public string SValue
        {
            get
            {
                return _SValue;
            }
        }

        public DateTime TimeStamp { get; set; }

        //public static AttributeValue operator +(AttributeValue a) => a;
        //public static AttributeValue operator -(AttributeValue a) => -a;

        public static TagValue operator +(TagValue a, TagValue b)
        {
            TagValue c = new TagValue();
            if (a.IsString || b.IsString)
                c.Value = "Calc Failed";
            else
                c.Value = (double)a.Value + (double)b.Value;
            return c;
        }

        public static TagValue operator -(TagValue a, TagValue b)
        {
            TagValue c = new TagValue();
            if (a.IsString || b.IsString)
                c.Value = "Calc Failed";
            else
                c.Value = (double)a.Value - (double)b.Value;
            return c;
        }

        public static TagValue operator *(TagValue a, TagValue b)
        {
            TagValue c = new TagValue();
            if (a.IsString || b.IsString)
                c.Value = "Calc Failed";
            else
                c.Value = (double)a.Value * (double)b.Value;
            return c;
        }

        public static TagValue operator /(TagValue a, TagValue b)
        {
            TagValue c = new TagValue();
            if (a.IsString || b.IsString)
                c.Value = "Calc Failed";
            else
            {
                if ((double)b.Value == 0)
                    throw new DivideByZeroException();
                else
                    c.Value = (double)a.Value / (double)b.Value;
            }
            return c;
        }

        public override string ToString()
        {
            return $"{TimeStamp} : {Value}" + (string.IsNullOrEmpty(_SValue) ? "" : $"[{_SValue}]");
        }
    }

}
