using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSung
{
    public class Utils
    {
        public static string[] Split(string line)
        {
            string tmp = line.Replace("'", "");
            tmp = Regex.Replace(tmp, @"\s*,\s*", ",");
            tmp = Regex.Replace(tmp, @"\s+", ",");
            string[] array = Regex.Split(tmp, ",");
            return array;
        }
    }
}
