using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor
{
    public static class Util
    {
        public static string GetDebugString(object value)
        {
            if (value == null) return null;
            else if (value is UInt64)
            {
                var v = (UInt64)value;
                return v.ToString("X16") + "(" + v + ")";
            }
            else if (value is UInt32)
            {
                var v = (UInt32)value;
                return v.ToString("X08") + "(" + v + ")";
            }
            else if (value is UInt16)
            {
                var v = (UInt16)value;
                return v.ToString("X04") + "(" + v + ")";
            }
            else if (value is byte)
            {
                var v = (byte)value;
                return v.ToString("X02") + "(" + v + ")";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
