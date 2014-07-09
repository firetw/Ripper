using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSung
{
    public class SamSungConfig
    {
        //https://chn.account.samsung.com/account/userVerifyCheckChinese.do?checkType=CN_IDCard&serviceID=xna99pg346&separatorName=JoinNow
        public static string StartUrl = "http://membership.samsung.com/cn";

        public static string InitilizeUrl = "https://chn.account.samsung.com/account/userVerifyCheckChinese.do?checkType=CN_IDCard&serviceID=xna99pg346&separatorName=JoinNow";

        public static bool CreateBrowserPerContext = false;

        public static int Timeout = 2 * 60 * 1000;

        public static bool AutoSplitName = true;

        public static bool Fiddler { get; private set; }

        static SamSungConfig()
        {
            Fiddler = System.Configuration.ConfigurationManager.AppSettings["Fiddler"] == "1";
        }
    }
}
