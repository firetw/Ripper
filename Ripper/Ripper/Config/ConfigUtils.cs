using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.Config
{
    public static class ConfigUtils
    {
        public static int MaxTaskCount { get; set; }

        static ConfigUtils()
        {
            MaxTaskCount = 20;
        }
    }
}
