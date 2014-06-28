using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WebLoginer;
using WebLoginer.Core;

namespace WeiBoGrab
{
    //ListViewItemAddStringDelegate
    //InvokeDelegate
    public delegate void ListViewItemAddStringDelegate(ListViewItem state, string subItemText);

    public partial class LoginForm : Form
    {

        Encoding encode = null;
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ListViewItemAddStringDelegate lvItemHandle = null;
        public LoginForm()
        {
            InitializeComponent();
            lvItemHandle = new ListViewItemAddStringDelegate(SetListViewItemText);
            this.Load += LoginForm_Load;
        }

        void LoginForm_Load(object sender, EventArgs e)
        {
            encode = Encoding.GetEncoding(System.Configuration.ConfigurationManager.AppSettings["encoding"]);
        }

        private string GetWtFpc()
        {
            string value = "2";
            Random random = new Random();
            long time = (long)(DateTime.Now.Subtract(TimeSpan.FromHours(8)) - DateTime.Parse("1970-1-1 00:00:00")).TotalMilliseconds;
            for (int i = 2; i < 32 - time.ToString().Length; i++)
            {
                value += ((int)Math.Floor(random.NextDouble() * 16.0)).ToString("X");
            }
            value += time;

            string wtFpc = string.Format("id={0}:lv={1}:ss={2}", System.Web.HttpUtility.UrlEncode(value), time + 1800000, time);

            return wtFpc;
        }

        private string GetMsg(string phone, double time, string msg)
        {
            return string.Format("Tel:{0},耗时:{1} 毫秒\r\n{2}", phone, time, msg);
        }

        /// <summary>
        /// 好像用重建CookieContainer就可以登陆了,OY!
        /// </summary>
        private string NewXjLogin(string phone, string pwd)
        {
            string loginUrl = "https://xj.ac.10086.cn/login";
            string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            string initPage = "https://www.xj.10086.cn/app?service=page/IcsLogin&listener=initPage";
            string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";



            #region WebBrowser
            //webBrowser1.Navigate(loginUrl);
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}

            //HtmlElement SERIAL_NUMBER = webBrowser1.Document.GetElementById("SERIAL_NUMBER");
            //HtmlElement USER_PASSWD = webBrowser1.Document.GetElementById("USER_PASSWD");
            //SERIAL_NUMBER.SetAttribute("value", "13639939435");
            //USER_PASSWD.SetAttribute("value", "337339");

            ////<a href="#" onclick="submitForm(2)">网上营业厅</a>
            //HtmlElementCollection collection = webBrowser1.Document.GetElementsByTagName("A");

            //foreach (HtmlElement item in collection)
            //{
            //    if (item.InnerText == "网上营业厅")
            //    {
            //        item.InvokeMember("click");
            //        break;
            //    }
            //}
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}
            //webBrowser1.Navigate(lurl);
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}
            //webBrowser1.Navigate(referer);
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}
            #endregion


            System.Diagnostics.Stopwatch sw = Stopwatch.StartNew();
            Encoding encode = Encoding.UTF8;
            RequestContext readContext = RequestContext.DefaultContext();
            //Cookie: pgv_pvi=7629626368; CmProvid=xj; WT_FPC=id=2565d7b62df796a84871402838018774:lv=1403601331204:ss=1403599596801
            readContext.Cookies.Add(new Cookie("CmProvid", "xj") { Domain = ".10086.cn" });
            readContext.Cookies.Add(new Cookie("pgv_pvi", "7629626368") { Domain = ".10086.cn" });

            #region js 生成WT_FPC的代码
            //js 生成WT_FPC的代码
            //if($t.length<10)
            //{
            //    var $x=$u.getTime().toString();
            //    for(var i=2;i<=(32-$x.length);i++)
            //        $t+=Math.floor(Math.random()*16.0).toString(16);
            //      $t+=$x;
            //};
            //$t=encodeURIComponent($t);
            //this.p+="&WT.co_f="+$t;
            //var result="WT_FPC=id="+$t+":lv="+$u.getTime().toString()+":ss="+$w.getTime().toString()+"; expires="+$v.toGMTString()+"; path=/; domain=.10086.cn";


            //"id=2565d7b62df796a84871402838018774:lv=1403601331204:ss=1403599596801"
            #endregion

            Cookie wtFpcCookie = new Cookie("WT_FPC", GetWtFpc()) { Domain = ".10086.cn" };
            readContext.Cookies.Add(wtFpcCookie);
            //readContext.Cookies.Add(new Cookie("AT-ICS-18844", "CBOJOOAKEJJM", "/", ".10086.cn"));
            //readContext.Cookies.Add(new Cookie("WEBTRENDS_ID", "110.156.52.161-1403682576.545128", "/", ".10086.cn"));

            //readContext.Header.Add("Cookie", "pgv_pvi=7629626368; CmProvid=xj; WT_FPC=id=2565d7b62df796a84871402838018774:lv=1403601331204:ss=1403599596801");
            readContext.URL = loginUrl;
            readContext.ContentType = "text/html";
            readContext.CookieContainer.Add(readContext.Cookies);
            WebOperResult readResult = HttpWebHelper.Get(readContext);

            sw.Stop();
            log.Info(GetMsg(phone, sw.ElapsedMilliseconds, readResult.Text));


            sw.Restart();
            //13639975410	151515
            string postdata = string.Format("SERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 phone, pwd, GetValueById(readResult.Text, "numId"));//System.Web.HttpUtility.UrlEncode()

            //postdata = System.Web.HttpUtility.UrlEncode(postdata);

            RequestContext postContext = RequestContext.DefaultContext();
            postContext.URL = postUrl;
            postContext.Allowautoredirect = false;
            postContext.Method = "POST";
            postContext.Accept = "application/xhtml+xml, */*";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = postdata;
            //postContext.Cookies = readContext.Cookies;
            postContext.CookieContainer = readContext.CookieContainer;
            postContext.CookieContainer.Add(readResult.Response.Cookies);


            //postContext.Cookies.Add(readContext.Cookies);
            //postContext.Cookies.Add(readResult.Cookies);
            WebOperResult postResult = HttpWebHelper.Post(postContext);
            sw.Stop();
            log.Info(GetMsg(phone, sw.ElapsedMilliseconds, postResult.Text));

            //CookieCollection cc = new CookieCollection();
            //string cookieStr = string.Empty;
            //string result = WnHttpHelper.Login(postUrl, postdata, "utf-8", ref cookieStr, lurl);

            //crifanLib lib = new crifanLib();
            //cc = lib.parseSetCookie(cookieStr);

            //RequestContext webContext = RequestContext.DefaultContext();
            //webContext.ContentType = "text/html";
            //webContext.URL = lurl;
            //webContext.CookieCollection.Add(postResult.Cookies);
            ////webContext.CookieCollection.Add(cc);
            //WebOperResult webResult = WnHttpHelper.Get(webContext);


            sw.Restart();
            RequestContext reContext = RequestContext.DefaultContext();
            reContext.ContentType = "text/html";
            reContext.URL = initPage;
            reContext.CookieContainer = postContext.CookieContainer;
            reContext.CookieContainer.Add(postResult.Response.Cookies);
            //reContext.Cookies.Add(postResult.Cookies);
            WebOperResult reResult = HttpWebHelper.Get(reContext);


            RequestContext lContext = RequestContext.DefaultContext();
            lContext.ContentType = "text/html";
            lContext.URL = referer;
            //lContext.Cookies.Add(reResult.Cookies);
            //lContext.Cookies.Add(new Cookie("CmWebtokenid", System.Web.HttpUtility.UrlEncode("13565824286,xj"), "/", ".10086.cn"));
            //lContext.Cookies.Add(wtFpcCookie);
            //lContext.Cookies.Add(readResult.Cookies);

            lContext.CookieContainer = reContext.CookieContainer;
            lContext.CookieContainer.Add(reResult.Response.Cookies);

            WebOperResult lResult = HttpWebHelper.Get(lContext);
            sw.Stop();
            log.Info(GetMsg(phone, sw.ElapsedMilliseconds, lResult.Text));

            //$.ajaxSubmit({page:'ownedbusi.SaleActivityVIPScoreHome',listener:'exchangeReward',param:'GIFT_CODE='+$('#GIFT_CODE').val(),partId:'beanRefreash',afterFn:exchangeAfter});
            // http://www.xj.10086.cn/app?service=ajaxDirect/1/ownedbusi.SaleActivityVIPScoreHome/ownedbusi.SaleActivityVIPScoreHome/javascript/beanRefreash&pagename=ownedbusi.SaleActivityVIPScoreHome&eventname=exchangeReward&GIFT_CODE=20120263&partids=beanRefreash&ajaxSubmitType=post&ajax_randomcode=0.907221282168889 HTTP/1.1

            Random random = new Random();
            string gift = "http://www.xj.10086.cn/app?service=ajaxDirect/1/ownedbusi.SaleActivityVIPScoreHome/ownedbusi.SaleActivityVIPScoreHome/javascript/beanRefreash&pagename=ownedbusi.SaleActivityVIPScoreHome&eventname=exchangeReward&GIFT_CODE=30&partids=beanRefreash&ajaxSubmitType=post&ajax_randomcode=" + random.NextDouble();
            string giftCode = "GIFT_CODE=30";

            sw.Restart();
            RequestContext giftContext = RequestContext.DefaultContext();
            giftContext.URL = gift;
            giftContext.Allowautoredirect = true;
            giftContext.Method = "POST";
            giftContext.Accept = "application/xhtml+xml, */*";
            giftContext.ContentType = "application/x-www-form-urlencoded";
            //giftContext.Postdata = giftCode;
            //postContext.Cookies = readContext.Cookies;
            giftContext.CookieContainer = lContext.CookieContainer;
            giftContext.CookieContainer.Add(lResult.Response.Cookies);

            WebOperResult giftResult = HttpWebHelper.Post(giftContext);
            sw.Stop();
            log.Info(GetMsg(phone, sw.ElapsedMilliseconds, giftResult.Text));

            return lResult.Text;
            //RequestContext lContext1 = RequestContext.DefaultContext();
            //lContext1.ContentType = "text/html";
            //lContext1.URL = referer;
            //lContext1.Cookies.Add(lContext.Cookies);
            //lContext1.Cookies.Add(lResult.Cookies);

            //WebOperResult lResult1 = WnHttpHelper.Get(lContext1);
        }

        private List<string> GetGoList(string content)
        {
            List<string> result = new List<string>();
            MatchCollection collection = Regex.Matches(content, "src\\s*=\\s*\"([^\"]+)\">", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            //if (match.Success)
            //{
            //    for (int i = 1; i < match.Groups.Count; i++)
            //    {
            //        Group group = match.Groups[i];
            //        string value = group.Value;
            //        result.Add(value);
            //    }
            //}

            if (collection != null && collection.Count > 0)
            {
                foreach (Match item in collection)
                {
                    if (item.Success && item.Groups.Count > 0)
                    {
                        string value = item.Groups[1].Value;
                        result.Add(value);
                    }
                }
            }
            return result;
        }

        private string GetValueById(string content, string id)
        {
            string result = null;
            int index = content.IndexOf(string.Format("id=\"{0}\"", id));
            if (index == -1) return result;
            int end = content.IndexOf("/>", index); if (end == -1) return result;

            string line = content.Substring(index, end - index);

            Match match = Regex.Match(line, "value\\s*=\\s*\\\"(\\S+)\\\"");

            if (match != null && match.Success)
            {
                result = match.Groups[1].Value;
            }
            result = result.Replace("!", "%21");
            return result;
        }
        private string GetValueByName(string content, string name)
        {
            string result = null;
            int index = content.IndexOf(string.Format("name=\"{0}\"", name));
            if (index == -1) return result;
            int end = content.IndexOf("/>", index); if (end == -1) return result;

            string line = content.Substring(index, end - index);

            Match match = Regex.Match(line, "value\\s*=\\s*\\\"(\\S+)");

            if (match != null && match.Success)
            {
                return match.Groups[1].Value;
            }
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < mainListView.Items.Count; i++)// 
            {
                ListViewItem item = mainListView.Items[i];

                ThreadPool.QueueUserWorkItem(new WaitCallback(SetListViewItemValue), item);
            }
        }

        private void SetListViewItemText(ListViewItem item, string subItemText)
        {
            item.SubItems.Add(subItemText);

        }

        private void SetListViewItemValue(object state)
        {
            ListViewItem item = state as ListViewItem;
            if (state == null) return;


            string tel = item.SubItems[1].Text;
            string pwd = item.SubItems[2].Text;
            string content = NewXjLogin(tel, pwd);

            Match match = Regex.Match(content, "<p>乐豆：<span id=\"score_number_info\">\\s*(\\d+)\\s*</span>");



            if (match.Success)
            {
                string context = match.Groups[1].Value;
                this.mainListView.BeginInvoke(lvItemHandle, new object[] { item, context });
            }
        }

        int index = 1;
        private void btExport_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            string line = string.Empty;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                index = 1;
                this.mainListView.Items.Clear();
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    using (StreamReader reader = new StreamReader(myStream, encode))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                if (string.IsNullOrWhiteSpace(line)) continue;
                                if (string.IsNullOrEmpty(line)) continue;

                                Match match = Regex.Match(line, @"(\d+)\s+(\d+)");
                                if (match.Success)
                                {
                                    ListViewItem item = new ListViewItem(index.ToString());
                                    item.SubItems.Add(match.Groups[1].Value);
                                    item.SubItems.Add(match.Groups[2].Value);

                                    this.mainListView.Items.Add(item);
                                    index++;
                                }
                            }
                            catch (Exception ex)
                            {
                                //log.Error(ex);
                                Console.WriteLine(ex);
                            }

                        }
                    }
                }
            }

        }

    }
}
