using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.View
{
    public class Config
    {
        public static Dictionary<string, string> GiftCodeMap //= new Dictionary<string, string>();
        {
            get;
            private set;
        }
        public static Dictionary<string, int> GiftLeDouMap //= new Dictionary<string, string>();
        {
            get;
            private set;
        }

        public static int LogLevel { get; private set; }

        static Config()
        {
            GiftCodeMap = new Dictionary<string, string>();
            GiftLeDouMap = new Dictionary<string, int>();

            string gitfCode = System.Configuration.ConfigurationManager.AppSettings["giftCode"];
            string gitfLeDou = System.Configuration.ConfigurationManager.AppSettings["giftLeDou"];
            LogLevel = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["logLevel"]);

            //30:30
            string[] codes = gitfCode.Split(',');

            foreach (var item in codes)
            {
                string[] tmp = item.Split(':');
                if (tmp.Length < 2) continue;

                GiftCodeMap.Add(tmp[0], tmp[1]);
            }

            string[] leDou = gitfLeDou.Split(',');

            foreach (var item in leDou)
            {
                string[] tmp = item.Split(':');
                if (tmp.Length < 2) continue;

                GiftLeDouMap.Add(tmp[0], Convert.ToInt32(tmp[1]));
            }

        }


    }
}
