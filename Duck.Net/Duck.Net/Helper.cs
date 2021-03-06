﻿using System;
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
            return (!string.IsNullOrEmpty(str) && str.StartsWith("\"") && str.EndsWith("\"") || !string.IsNullOrEmpty(str) && str.StartsWith("'") && str.EndsWith("'"));
        }
        public static String extractStringContent(String str)
        {
            if (str != null)
            {
                if (str.StartsWith("\""))
                {
                    str = str.TrimStart('"');
                    str = str.TrimEnd('"');
                }
                else if (str.StartsWith("\'")) {
                    str = str.TrimStart('\'');
                    str = str.TrimEnd('\'');
                }
        
                return str;
            }
            return str;
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
            if (!Helper.isString(str))
                return $"\"{str}\"";
            else
                return str;
        }
    }
}
