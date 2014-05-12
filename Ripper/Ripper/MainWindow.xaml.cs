using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ripper.TaskDispather;
using System.Net;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using HtmlAgilityPack;
using Ripper.Model;
using System.Diagnostics;
using System.Windows.Forms.Integration;

namespace Ripper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //User-Agent: Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36
        const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const string sContentType = "application/x-www-form-urlencoded";
        const string sRequestEncoding = "uft-8";
        const string sResponseEncoding = "uft-8";

        static CookieContainer cc = new CookieContainer();
        public MainWindow()
        {
            InitializeComponent();
        }

        private WebTask GetTask(string url, string dir, CookieContainer cc)
        {
            WebContext context = new WebContext
            {
                Url = url,
                Dir = dir,
                CookieContainer = cc
            };
            WebTask wt = new WebTask(context);
            return wt;
        }

        private void btCollector_Click(object sender, RoutedEventArgs e)
        {

            //StartJob.Start();
            //StartMjJob.Start();
            WindowsFormsHost host = new WindowsFormsHost();

            System.Windows.Forms.WebBrowser wb = WbStartJob.StartJob();
            host.Child = wb;
            root.Children.Add(host);
            Grid.SetRow(host, 0);




            return;
            //<input name="username" tabIndex="1" class="W_input " type="text" maxLength="128" node-type="username" action-data="text=邮箱/会员帐号/手机号" action-type="text_copy" autocomplete="off" value=""/>
            //<input name="password" tabIndex="2" class="W_input " type="password" maxLength="24" node-type="password" action-type="text_copy" value=""/>
            LoginResult loginResult = SinaLogin.Login("firetw@163.com", "1qaz!QAZ");


            System.Diagnostics.Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            WebTask wt1 = GetTask("http://weibo.com/p/1005052814680487/weibo?is_search=0&visible=0&is_tag=0&profile_ftype=1&page=1#feedtop", "I:/wb/笑话", loginResult.CookieContainer);
            WebTask wt2 = GetTask("http://weibo.com/p/1002061722052204/weibo?is_search=0&visible=0&is_tag=0&profile_ftype=1&page=1#feedtop", "I:/wb/鬼脚七的2014", loginResult.CookieContainer);
            WebTask wt3 = GetTask("http://weibo.com/p/1035051182389073/weibo?is_search=0&visible=0&is_tag=0&profile_ftype=1&page=1#feedtop", "I:/wb/任志强", loginResult.CookieContainer);

            Console.WriteLine(sw.ElapsedMilliseconds);
            List<ITask> list = new List<ITask> { wt1, wt2, wt3 };


            //TaskManager.Instance.AddTask(list);

            string baiDu = "http://www.baidu.com";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baiDu);
            request.UserAgent = sUserAgent;
            request.ContentType = sContentType;

            string postData = "kw=wang";
            byte[] b = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = b.Length;
            request.Method = "POST";

            using (System.IO.Stream stream = request.GetRequestStream())
            {
                try
                {
                    stream.Write(b, 0, b.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Post数据时出错！", ex);
                }
                finally
                {
                    if (stream != null) { stream.Close(); }
                }
            }
            HttpWebResponse response = null;
            System.IO.StreamReader sr = null;

            string html = string.Empty;
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                //_Cookies = response.Cookies;

                sr = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                html = sr.ReadToEnd();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            sw.Stop();
        }




    }
}
