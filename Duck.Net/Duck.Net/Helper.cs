using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Duck.Net
{
    class Helper
    {
        public static bool isString(String str)
        {
            return (!string.IsNullOrEmpty(str) && str.StartsWith("\"") && str.EndsWith("\""));
        }
        public static String extractStringContent(String str)
        {
            return str.Substring(1, str.Length - 2);
        }
        public static bool isNumber(String str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(str);
        }
    }
}
