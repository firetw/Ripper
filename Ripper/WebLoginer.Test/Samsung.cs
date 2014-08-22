using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using SamSung;
using System.IO;
using Tools;

namespace WebLoginer.Test
{
    /// <summary>
    /// Samsung 的摘要说明
    /// </summary>
    [TestClass]
    public class SamsungTest
    {
        public SamsungTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void IdTest()
        {
            string idCar = "410225198312086132";
            Match match = Regex.Match(idCar, @"\d{6}(\d{4})(\d{2})(\d{2})");
            if (match.Success)
            {
                string year = match.Groups[1].Value;
                string month = match.Groups[2].Value;
                string day = match.Groups[3].Value;

                string birthday = year + month + day;
            }
        }

        [TestMethod]
        public void NumTest()
        {
            int seq = 'a';
            StringBuilder builder = new StringBuilder();
            for (int i = seq; i < seq + 26; i++)
            {
                builder.Append("\'" + (char)i + "\',");
            }
            string lowchars = builder.ToString();
            Console.WriteLine(lowchars);

            seq = 'A';
            builder.Clear();
            for (int i = seq; i < seq + 26; i++)
            {
                builder.Append("\'" + (char)i + "\',");
            }
            string upperChars = builder.ToString();
            Console.WriteLine(upperChars);

            Console.WriteLine(PwdRep.Instance.GetPwd());
            Console.WriteLine(PwdRep.Instance.GetPwd());
            Console.WriteLine(PwdRep.Instance.GetPwd());
            Console.WriteLine(PwdRep.Instance.GetPwd());
            Console.WriteLine(PwdRep.Instance.GetPwd());
            Console.WriteLine(PwdRep.Instance.GetPwd());

            //            Fri Jul 04 2014 10:44:34 GMT+0800 (中国标准时间)


        }

        [TestMethod]
        public void TimeFormat()
        {
            string tm1 = DateTime.Now.ToLongDateString();
        }
        [TestMethod]
        public void TestAttatchCode()
        {

            //RegisterTask task = new RegisterTask();

            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/signup.txt");

            string context = File.ReadAllText(file, Encoding.UTF8);

            string code = ParserAttachCode(context);
            Console.WriteLine(code);
            Assert.IsTrue(!string.IsNullOrEmpty(code));
            Assert.IsTrue(code.Split('&').Length == 5);
        }

        [TestMethod]
        public void GetRandomCode()
        {
            Match match = null;

            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/report.txt");
            string imgVerifyReport = File.ReadAllText(file);
            string randomCode = string.Empty;
            if ((match = Regex.Match(imgVerifyReport, "<input\\s+name=\"(\\S+)inputEmailID\"\\s+type=\"text\"\\s+id=\"\\1inputEmailID\"")).Success)
            {
                randomCode = match.Groups[1].Value;
            }
        }

        [TestMethod]
        public void CheckResult()
        {

            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/check.txt");
            string resultReport = File.ReadAllText(file);
            string randomCode = string.Empty;
            Assert.IsTrue(!string.IsNullOrEmpty(resultReport) && Regex.IsMatch(resultReport, @"恭喜您!<br/>您的帐户已获得授权。请立即了解您可以通过"));


        }

        public string ParserAttachCode(string content)
        {
            string line = string.Empty;
            string code = string.Empty;
            Dictionary<string, string> questionList = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, "fnSetEleValue\\(document.signUpForm", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        break;
                    }
                }
            }
            string[] array = line.Split(';');
            foreach (var item in array)
            {
                Match match = Regex.Match(item, "fnSetEleValue\\(document.signUpForm,\\s*[\"\']([^\"]*)[\"\'],\\s*[\"\']([^\"]*)[\"\']\\)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (!string.IsNullOrEmpty(code))
                        code += "&";
                    code += match.Groups[1].Value + "=" + match.Groups[2].Value;
                }
            }
            return code;



            //fnSetEleValue(document.signUpForm, "acajvq", "dgtlsf");fnSetEleValue(document.signUpForm, "kppztx", "jbyrfx");fnSetEleValue(document.signUpForm, "zuexdv", "kkbhts");fnSetEleValue(document.signUpForm, "njuxpe", "lcmjzn");fnSetEleValue(document.signUpForm, 'cbvenx', "lnusgw");
            //fnSetEleValue(document.signUpForm, "acajvq", "dgtlsf");fnSetEleValue(document.signUpForm, "kppztx", "jbyrfx");fnSetEleValue(document.signUpForm, "zuexdv", "kkbhts");fnSetEleValue(document.signUpForm, "njuxpe", "lcmjzn");fnSetEleValue(document.signUpForm, 'cbvenx', "lnusgw");

        }
    }
}
