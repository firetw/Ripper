using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<String> charMap = new List<String>();
                for (int i = 65; i < 91; i++)
                {
                    //Console.WriteLine((char)i);
                    charMap.Add(((char)i).ToString());
                }
                Random random = new Random();
                List<List<string>> list = new List<List<string>>();
                for (int i = 0; i < 20; i++)
                {
                    List<string> rows = new List<string>();
                    for (int j = 0; j < random.Next(30, 100); j++)
                    {
                        rows.Add(charMap[random.Next(1, 26)]);
                    }
                    list.Add(rows);
                }


                LinqTester.Test();

                //ExcelHelper.Test(list);
                Console.WriteLine("执行完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            Console.Read();
            double tmpValue = double.NaN;
            if ((0.0 / 0.0) == double.NaN)
            {
                Console.WriteLine("这是正常输出？");
            }
            else
            {
                Console.WriteLine("这是神马情况？");
            }

            Console.Read();
        }


        #region Property
        //////Console.WriteLine(string.Format("{0}",null));
        ////  Console.WriteLine("Begin Call Web Service");

        ////  //BcfStatusParser parser = new BcfStatusParser();
        ////  //string result = parser.Parser("execmd  ZEEI:BCF=105:;");
        ////  //Console.WriteLine(result);

        ////  FetionWebService.fetion_mathine_server server = new FetionWebService.fetion_mathine_server();
        ////  string result = server.CollectNeConfigInfoParser("BSC", "HBHWBSC05", "105", "", "200", "fetion2013", "fetion2013");
        ////  Console.WriteLine(result);
        ////  Console.WriteLine("a");

        ////  //string file = @"H:\ali\Hd\课作\开发课件\第7周作业素材\第7周作业\作业\digital\digital\mobile\mobile3233";
        ////  //string result = @"H:\ali\Hd\课作\开发课件\第7周作业素材\第7周作业\作业\digital\digital\mobile\wn1";
        ////  //using (StreamReader reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read)))
        ////  //{
        ////  //    string content = reader.ReadToEnd();
        ////  //    using (StreamWriter writer = new StreamWriter(File.Create(result), Encoding.UTF8))
        ////  //    {
        ////  //        writer.Write(content);
        ////  //    }
        ////  //}
        ////  //Console.WriteLine("Completed");
        #endregion



        public class A
        {
            public void SayName(string name)
            {
                Console.WriteLine(name);
            }
            public virtual void SayAddr(string address)
            {
            }
        }
        public class B : A
        {

            public new void SayName(string name, string age)
            {
                Console.WriteLine(name + age);
            }
            public new void SayName(string name)
            {
                Console.WriteLine(name);
            }
            // public overload 
        }
    }
}
