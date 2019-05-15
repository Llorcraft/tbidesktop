using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbiDesktop.Extensions
{
    public static class StringExt
    {
        public static string ToNumber(this string value)
        {
            return value.Replace(",", ".");
        }
    }
}
