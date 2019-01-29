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
            String value = "";
            int start = 0;
            if (string.IsNullOrEmpty(str) || !str.StartsWith("\""))
                return string.Empty;
            str = str.Trim();
            try
            {
                int i = 0;
                while (str[i] == '"')
                    i++;

                start = i;

                i = str.Length - 1;
                while (str[i] == '"')
                    i--;
                value = str.Substring(start, i-1);
                return value;
            }
            catch(Exception e)
            {
                return string.Empty;
            }
            
        }
        public static bool isNumber(String str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            int intrev;
            return Int32.TryParse(str, out intrev);
        }

        public static string extractDigitContent(string str)
        {
            String value = Int32.Parse(str).ToString();
            return value;
        }
        public static string trasformToString(string str)
        {
            str = string.IsNullOrEmpty(str) ? "" : str;
            return "\"" + str + "\"";
        }
    }
}
