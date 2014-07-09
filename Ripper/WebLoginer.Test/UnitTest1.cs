using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WebLoginer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string context = "1110N6XTjZFHlDRy7L3p5ClHXcHGTS1MJG2Qn8zsgqGGFGskzLlpQ2b!-1280290979!1403230469524";
            context = context.Replace("!", "%21");

            Assert.IsTrue(context.IndexOf("!") == -1);
        }


        [TestMethod]
        public void RegexTest()
        {
            string content = "value=\"123\" id=\"CIFT_MESSAGE\""; //"value=\"&amp;#24456;&amp;#36951;&amp;#25022;&amp;#65292;&amp;#24744;&amp;#26469;&amp;#26202;&amp;#20102;&amp;#65292;&amp;#20170;&amp;#22825;&amp;#30340;&amp;#35805;&amp;#36153;&amp;#24050;&amp;#32463;&amp;#34987;&amp;#25250;&amp;#23436;&amp;#20102;&amp;#65281;&amp;#35831;&amp;#24744;&amp;#20817;&amp;#25442;&amp;#20854;&amp;#23427;&amp;#22870;&amp;#21697;&amp;#25110;&amp;#21442;&amp;#21152;&amp;#25277;&amp;#22870;&amp;#21734;\" id=\"CIFT_MESSAGE\"";
            Match match = null;
            if ((match = Regex.Match(content, "value=\"([^\"]*)\" id=\"CIFT_MESSAGE\"")).Success)
            {
                Console.WriteLine(match.Groups[1].Value);
            }
            string str = System.Web.HttpUtility.HtmlEncode("\"");
            Console.WriteLine(str);
        }

        [TestMethod]
        public void BuilderCookie()
        {
            string cookie = @"CmProvid=xj; 
            WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403580212335:ss=1403579675263; 
            JSESSIONID=VvRdTykTDLgfFbf6xsyS7bBy5K1SmtX5BGN2ypvtTfPG6PPgSCJn!-1934896026";

            //首要问题是如何判断成功

            string[] cookies = cookie.Split(';');
            //CmProvid=xj;path=/;domain=10086.cn;expires=Wed, 24 Jun 2015 03:43:55 GMT

            //exp.setTime(exp.getTime() + Days*24*60*60*1000);
            System.Net.Cookie c = new System.Net.Cookie("CmProvid", "xj", "/", "10086.cn");
            c.Expires = DateTime.Now.AddMilliseconds((double)(365 * 24 * 60 * 60 * 1000.0));
        }

        [TestMethod]
        public void TestTime()
        {
            DateTime time1 = DateTime.Parse("1970-1-1 00:00:00").AddMilliseconds(1403661047401);

            DateTime endTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(1403661047401));

            string value = "2";
            Random random = new Random();
            long time = (long)(DateTime.Now.Subtract(TimeSpan.FromHours(8)) - DateTime.Parse("1970-1-1 00:00:00")).TotalMilliseconds;
            for (int i = 2; i < 32 - time.ToString().Length; i++)
            {
                value += ((int)Math.Floor(random.NextDouble() * 16.0)).ToString("X");
            }
            value += time;

            string wtFpc = string.Format("WT_FPC=id={0}:lv={1}:ss={2}", System.Web.HttpUtility.UrlEncode(value), time + 1800000, time);


        }


        [TestMethod]
        public void ReadData()
        {
            //string file = @"D:\xml1.xml";
            string file = @"E:\hf.xml";

            XElement element = XElement.Load(file);


            string content = element.Element("part").Value; //"&amp;#24685;&amp;#21916;&amp;#24744;&amp;#24050;&amp;#25104;&amp;#21151;&amp;#20817;&amp;#25442;20&amp;#26465;&amp;#30701;&amp;#20449;&amp;#30005;&amp;#23376;&amp;#28192;&amp;#36947;&amp;#20813;&amp;#36153;&amp;#20307;&amp;#39564;&amp;#21253;&amp;#65288;&amp;#24403;&amp;#26376;&amp;#29983;&amp;#25928;&amp;#65289;&amp;#19994;&amp;#21153;&amp;#65292;&amp;#28040;&amp;#32791;20&amp;#20048;&amp;#35910;&amp;#65292;&amp;#21097;&amp;#20313;95&amp;#20048;&amp;#35910;&amp;#12290;";
            string data = System.Web.HttpUtility.HtmlDecode(content);
            data = System.Web.HttpUtility.HtmlDecode(data);
            string result = ReadContext(data);

        }
        [TestMethod]
        public void ShowValue()
        {
            string context1 = "&amp;#24456;&amp;#36951;&amp;#25022;&amp;#65292;&amp;#20817;&amp;#25442;&amp;#22833;&amp;#36133;&amp;#65292;&amp;#20170;&amp;#26085;&amp;#35813;&amp;#22870;&amp;#21697;&amp;#24050;&amp;#34987;&amp;#20817;&amp;#25442;&amp;#23436;&amp;#27605;&amp;#21862;&amp;#65281;";
            //int avaiThread = ThreadPool.GetAvailableThreads();
            int t1 = 0, t2 = 0;
            ThreadPool.GetMaxThreads(out t1, out t2);

            Console.WriteLine();
            string data = System.Web.HttpUtility.HtmlDecode(context1);
            data = System.Web.HttpUtility.HtmlDecode(data);

            string context2 = "&amp;#24456;&amp;#36951;&amp;#25022;&amp;#65292;&amp;#24744;&amp;#26469;&amp;#26202;&amp;#20102;&amp;#65292;&amp;#20170;&amp;#22825;&amp;#30340;&amp;#35805;&amp;#36153;&amp;#24050;&amp;#32463;&amp;#34987;&amp;#25250;&amp;#23436;&amp;#20102;&amp;#65281;&amp;#35831;&amp;#24744;&amp;#20817;&amp;#25442;&amp;#20854;&amp;#23427;&amp;#22870;&amp;#21697;&amp;#25110;&amp;#21442;&amp;#21152;&amp;#25277;&amp;#22870;&amp;#21734;";
            string data1 = System.Web.HttpUtility.HtmlDecode(context2);
            data1 = System.Web.HttpUtility.HtmlDecode(data1);

        }
        [TestMethod]
        public void GetQuestion()
        {
            Match match = null;
            string e = string.Empty, q = string.Empty;
            string line = "<option value=\"SS_WHAT_IS_YOUR_ELDEST_COUSINS_FIRST_AND_LAST_NAME_Q\" >您最年长的表亲姓名是什么?</option>";
            if ((match = Regex.Match(line, "<option value=\"([^\"]*)\"\\s*>([^<]*)</option>", RegexOptions.IgnoreCase)).Success)
            {
                e = match.Groups[1].Value;
                q = match.Groups[2].Value;
            }
            Assert.IsTrue(!string.IsNullOrEmpty(e));
            Assert.IsTrue(!string.IsNullOrEmpty(q));
        }
        static List<string> DayOfWeek = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        static List<string> MonthOfYear = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Aug", "Aug", "Sep", "Oct", "Nov", "Dec" };

        [TestMethod]
        public void GenerateUrl()
        {
            DateTime now = DateTime.Now;

            int month = now.Month;
            string step1 = string.Format("{0} {1} {2} {3} {4}:{5}:{6} GMT+0800 (中国标准时间)",
                DayOfWeek[(int)now.DayOfWeek],
                MonthOfYear[--month],
                now.Day < 10 ? "0" + now.Day : now.Day.ToString(),
                now.Year,
                now.Hour < 10 ? "0" + now.Hour : now.Hour.ToString(),
                now.Minute < 10 ? "0" + now.Minute : now.Minute.ToString(),
                now.Second < 10 ? "0" + now.Second : now.Second.ToString());
            step1 = System.Web.HttpUtility.UrlEncode(step1);
            step1 = "https://account.samsung.cn/account/find/accountSCaptchaCodeView.do?serviceID=ts3rap101s&captchaGbn=SIGNUP&" + step1;
            Console.WriteLine(step1);

            Assert.IsNotNull(step1);

        }
        [TestMethod]
        public void JudgeGender()
        {
            string id1 = "41010319630816131X";
            Assert.IsTrue(JudgeGender(id1) == "M");

            string id2 = "412924197206153927";
            Assert.IsTrue(JudgeGender(id2) == "F");


            string id3 = "410703199003232013";
            Assert.IsTrue(JudgeGender(id3) == "M");
        }

        private string JudgeGender(string id)
        {
            int num = Convert.ToInt32(id.Substring(id.Length - 2, 1));

            if (num % 2 == 0)
                return "F";
            else
                return "M";
        }

        private string ReadContext(string data)
        {

            StringBuilder text = new StringBuilder();
            using (Stream reader = new GZipInputStream(new MemoryStream(Encoding.UTF8.GetBytes(System.Web.HttpUtility.HtmlDecode(data)))))
            {
                MemoryStream ms = new MemoryStream();
                int nSize = 4096;
                byte[] writeData = new byte[nSize];
                while (true)
                {
                    try
                    {
                        nSize = reader.Read(writeData, 0,
nSize);
                        if (nSize > 0)
                            ms.Write(writeData, 0,
nSize);
                        else
                            break;
                    }
                    catch (GZipException ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                reader.Close();
                text.Append(Encoding.UTF8.GetString(ms.GetBuffer()));
            }
            return text.ToString();
        }

    }
}
