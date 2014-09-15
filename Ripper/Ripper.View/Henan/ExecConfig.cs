using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.View.Henan
{
    public class ExecConfig
    {
        public static int RetryCount { get; set; }


        public static string RsaUrl { get; set; }

        static ExecConfig()
        {
            RetryCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RetryCount"]);
            RsaUrl = System.Configuration.ConfigurationManager.AppSettings["rsaUrl"];
        }
    }
}
