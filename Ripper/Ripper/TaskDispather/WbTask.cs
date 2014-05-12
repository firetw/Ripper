using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Logger;
using System.Windows.Forms;
using System.Windows;
using HtmlAgilityPack;
using System.Threading;

namespace Ripper.TaskDispather
{

    public class WbTask : BaseTask
    {
        HttpWebRequest _request = null;
        Encoding _gb2312 = null;
        ILogger logger;
        WebBrowser browser = null;

        public WebBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }
        public WbTask(WebContext context)
        {
            this.Context = context;
            logger = LoggerManager.GetLog();
            _gb2312 = Encoding.GetEncoding("GB2312");
        }
        const string sUserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36";
        const string sContentType = "application/x-www-form-urlencoded";
        [STAThread]
        public override void DoTask()
        {
            WebContext context = Context as WebContext;
            if (context == null) return;
            if (string.IsNullOrEmpty(context.Dir)) return;
            if (!Directory.Exists(context.Dir))
            {
                Directory.CreateDirectory(context.Dir);
            }

            
            browser = new WebBrowser();
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Thread.Sleep(100);
            }


            string url = "http://guang.taobao.com/detail/index.htm?spm=a310p.2219213.6861921.232.yrLSmu&uid=78301140&sid=6781951878#!/s6758211290/?targetPage=1";
            browser.Navigate(new Uri(@url));
           
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = sender as WebBrowser;
            if (wb == null) return;
            HtmlElement element = wb.Document.GetElementById("album-pagination");
            //foreach (HtmlElement item in element.FirstChild.FirstChild.Children)
            //{
            //    Console.WriteLine("1");
            //}
        }

        void browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //album-pagination





            Console.WriteLine(1);
        }

        void browser_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(2);
        }


    }


    public class WbStartJob
    {
        public static WebBrowser StartJob()
        {

            List<ITask> list = new List<ITask>();
            WebContext context2 = new WebContext
            {
                Url = "http://guang.taobao.com/detail/index.htm?spm=a310p.2219213.6861921.232.yrLSmu&uid=78301140&sid=6781951878#!/s6758211290/?targetPage=1",
                Dir = "I:/wb/mj",
                Flag = "targetPage=1",
                Start = 1,
                End = 1,
                Span = 1000,
                //CookieContainer = cc
            };

            //CookieContainer c1 = MakeContainer("I:/wb/mj");
            //list.Add(new MjTask(context2));
            //context2.CookieContainer = c1;
            //list.Add(new WbTask(context2));
            //TaskManager.Instance.AddTask(list);

            WbTask wt = new WbTask(context2);
            wt.DoTask();
            return wt.Browser;
        }
    }
}
