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
    public class FetionWebTask : BaseTask
    {
        HttpWebRequest _request = null;
        Encoding _encode = null;
        ILogger logger;
        WebBrowser browser = null;

        public WebBrowser Browser
        {
            get { return browser; }
            set { browser = value; }
        }
        public FetionWebTask(WebContext context)
        {
            this.Context = context;
            logger = LoggerManager.GetLog();
            _encode = GetEncoding();
        }
        const string sUserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36";
        const string sContentType = "application/x-www-form-urlencoded";

        public const string FetionUrl = "http://gz.feixin.10086.cn/Bootlick/index";
        [STAThread]
        public override void DoTask()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(FetionUrl);
            request.CookieContainer = new CookieContainer();
            WebResponse response = request.GetResponse();
            response.GetResponseStream().Close();
            //request.CookieContainer   
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

}

