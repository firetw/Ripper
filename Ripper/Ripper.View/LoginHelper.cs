using log4net;
using Ripper.View.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WebLoginer.Core;

namespace Ripper.View
{
    class LoginHelper
    {
        string loginUrl = "https://xj.ac.10086.cn/login";
        string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
        string initPage = "https://www.xj.10086.cn/app?service=page/IcsLogin&listener=initPage";
        string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";
        string logFileName = string.Empty;
        SetResultDelegate _setResultHandler = null;

        public LoginHelper()
        {
            DateTime now = DateTime.Now;
            _setResultHandler = new SetResultDelegate(SetEntity);
            logFileName = string.Format("兑换结果_" + now.ToString("yyyy-MM-dd") + ".txt");
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
        /// 好像不用重建CookieContainer就可以登陆了,OY!
        /// </summary>
        public WebOperResult Login(string phone, string pwd, ILog log)
        {
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

            System.Diagnostics.Stopwatch sw = null;
            if (Config.LogLevel != 2)
                sw = Stopwatch.StartNew();
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


            if (Config.LogLevel == 0)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, readResult.Text));
            }
            else if (Config.LogLevel == 1)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, "第一步请求"));
            }



            if (Config.LogLevel != 2)
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
            if (Regex.IsMatch(postResult.Text, "window.alert(\"手机号码或服务密码错误\");"))
            {
                return null;
            }


            if (Config.LogLevel == 0)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, postResult.Text));
            }
            else if (Config.LogLevel == 1)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, "提交用户名和密码"));
            }


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

            if (Config.LogLevel != 2)
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

            if (Config.LogLevel == 0)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, lResult.Text));
            }
            else if (Config.LogLevel == 1)
            {
                sw.Stop();
                log.Debug(GetMsg(phone, sw.ElapsedMilliseconds, "跳转兑换页"));
            }

            //$.ajaxSubmit({page:'ownedbusi.SaleActivityVIPScoreHome',listener:'exchangeReward',param:'GIFT_CODE='+$('#GIFT_CODE').val(),partId:'beanRefreash',afterFn:exchangeAfter});
            // http://www.xj.10086.cn/app?service=ajaxDirect/1/ownedbusi.SaleActivityVIPScoreHome/ownedbusi.SaleActivityVIPScoreHome/javascript/beanRefreash&pagename=ownedbusi.SaleActivityVIPScoreHome&eventname=exchangeReward&GIFT_CODE=20120263&partids=beanRefreash&ajaxSubmitType=post&ajax_randomcode=0.907221282168889 HTTP/1.1

            lResult.CookieContainer = lContext.CookieContainer;
            lResult.Cookies = lResult.Response.Cookies;


            return lResult;
            //RequestContext lContext1 = RequestContext.DefaultContext();
            //lContext1.ContentType = "text/html";
            //lContext1.URL = referer;
            //lContext1.Cookies.Add(lContext.Cookies);
            //lContext1.Cookies.Add(lResult.Cookies);

            //WebOperResult lResult1 = WnHttpHelper.Get(lContext1);
        }

        public string DuiHuan(Entity entity, ILog log, string gitfCode)
        {
            System.Diagnostics.Stopwatch sw = null;
            if (Config.LogLevel != 2)
                sw = Stopwatch.StartNew();

            string result = string.Empty;
            Random random = new Random();
            string gift = "http://www.xj.10086.cn/app?service=ajaxDirect/1/ownedbusi.SaleActivityVIPScoreHome/ownedbusi.SaleActivityVIPScoreHome/javascript/beanRefreash&pagename=ownedbusi.SaleActivityVIPScoreHome&eventname=exchangeReward&GIFT_CODE=" + gitfCode + "&partids=beanRefreash&ajaxSubmitType=post&ajax_randomcode=" + random.NextDouble();
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
            giftContext.CookieContainer = entity.CookieContainer; //lContext.CookieContainer;
            giftContext.CookieContainer.Add(entity.Cookies);

            WebOperResult giftResult = HttpWebHelper.Post(giftContext);
            string context = giftResult.Text;
            //&lt;input value="false" id="GIFT_FLAGE" type="hidden"&gt;&lt;/input&gt;
            //&lt;input value="40" id="GIFT_SUM" type="hidden"&gt;&lt;/input&gt;
            //&lt;input value="&amp;#24456;&amp;#36951;&amp;#25022;&amp;#65292;&amp;#24744;&amp;#30340;&amp;#20048;&amp;#35910;&amp;#19981;&amp;#36275;&amp;#65292;&amp;#19981;&amp;#33021;&amp;#20817;&amp;#25442;&amp;#35813;&amp;#35805;&amp;#36153;&amp;#65292;&amp;#35831;&amp;#20817;&amp;#25442;&amp;#20854;&amp;#23427;&amp;#22870;&amp;#21697;&amp;#25110;&amp;#21442;&amp;#21152;&amp;#25277;&amp;#22870;&amp;#65281;&amp;#36186;&amp;#21462;&amp;#26356;&amp;#22810;&amp;#20048;&amp;#35910;&amp;#21644;&amp;#20048;&amp;#20540;&amp;#35831;&amp;#24744;&amp;#28857;&amp;#20987;&amp;#8220;&amp;#25105;&amp;#30340;&amp;#20219;&amp;#21153;&amp;#8221;&amp;#25353;&amp;#38062;&amp;#65292;&amp;#23436;&amp;#25104;&amp;#26356;&amp;#22810;&amp;#20219;&amp;#21153;&amp;#21487;&amp;#32047;&amp;#35745;&amp;#26356;&amp;#22810;&amp;#20048;&amp;#35910;&amp;#21644;&amp;#20048;&amp;#20540;&amp;#21734;&amp;#65281;" id="CIFT_MESSAGE" 

            XElement element = XElement.Parse(context);
            foreach (var item in element.Elements("part"))
            {
                if (item.Attribute("id") != null && item.Attribute("id").Value == "beanRefreash")
                {
                    string content = System.Web.HttpUtility.HtmlDecode(item.Value);
                    bool status = true;
                    if (Regex.IsMatch(context, "value=\"false\"\\s+id=\"GIFT_FLAGE\""))
                    {
                        status = false;
                    }
                    Match match = null;
                    if ((match = Regex.Match(content, "value=\"([^\"]*)\" id=\"CIFT_MESSAGE\"")).Success)
                    {
                        if (string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            result = "兑换失败!";
                        }
                        else
                        {
                            result = System.Web.HttpUtility.HtmlDecode(match.Groups[1].Value);
                            if (result.Contains("恭喜您"))
                            {
                                entity.Success = true;
                            }
                        }
                    }
                }
            }
            log.Debug(GetMsg(entity.Tel.ToString(), sw.ElapsedMilliseconds, context));
            if (Config.LogLevel == 0)
            {
                sw.Stop();
                log.Debug(GetMsg(entity.Tel.ToString(), sw.ElapsedMilliseconds, result + System.Environment.NewLine + giftResult.Text));
            }
            else if (Config.LogLevel == 1)
            {
                sw.Stop();
                log.Debug(GetMsg(entity.Tel.ToString(), sw.ElapsedMilliseconds, result));
            }
            entity.Dispatcher.BeginInvoke(_setResultHandler, new object[] { entity, result, "1" }); //_leDouHandler, new object[] { item, context, wr }); 

            return result;
        }

        public WebOperResult QueryItem(CookieContainer cc, CookieCollection cookies, string postData, string url)
        {
            RequestContext getContext = RequestContext.DefaultContext();
            getContext.ContentType = "text/html";
            getContext.URL = url;
            getContext.Method = "GET";
            getContext.CookieContainer = cc;
            if (cookies != null)
                getContext.CookieContainer.Add(cookies);

            WebOperResult getResult = HttpWebHelper.Get(getContext);

            string token = GetValueById(getResult.Text, "com.ailk.ech.framework.html.TOKEN");
            string data = string.Format(postData, token);


            string postUrl = "http://www.xj.10086.cn/app";

            RequestContext postContext = RequestContext.DefaultContext();
            postContext.URL = postUrl;
            //postContext.Allowautoredirect = false;
            postContext.Method = "POST";
            postContext.Accept = "application/xhtml+xml, */*";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = data;
            //postContext.Cookies = readContext.Cookies;
            postContext.CookieContainer = cc;
            if (cookies != null)
                postContext.CookieContainer.Add(cookies);

            WebOperResult postResult = HttpWebHelper.Post(postContext);

            return postResult;
        }
        private void SetEntity(Entity entity, string status, string cmd)
        {
            if (entity == null) return;
            entity.TaskStatus = status;

            if (cmd == "1")
            {
                using (StreamWriter writer = File.AppendText(logFileName))
                {
                    writer.WriteLine(string.Format("{0}	{1}	{2}", entity.Tel, entity.LeDou, entity.TaskStatus));
                }
            }
        }

        public void Query(Entity entity)
        {
            CookieContainer cc = entity.CookieContainer;
            CookieCollection cookies = entity.Cookies;

            string hfUrl = "http://www.xj.10086.cn/service/fee/feequery/BalanceQuery/";//话费
            string zdUrl = "http://www.xj.10086.cn/service/fee/feequery/BillQueryNew/";//账单
            string yhUrl = "http://www.xj.10086.cn/service/fee/svcquery/DictimeQuery/";//优惠


            string huafeiData = "service=direct%2F1%2Ffeequery.BalanceQuery%2F%24Form&sp=S0&Form0=%24FormConditional%2CBLANCE%2C%24FormConditional%240&%24FormConditional=T&%24FormConditional%240=F&BLANCE=%B2%E9%D1%AF&operHipInfo=&com.ailk.ech.framework.html.TOKEN={0}";
            string zhangdanData = "service=direct%2F1%2Ffeequery.BillQueryNew%2F%24Form&sp=S0&Form0=%24Submit%2C%24FormConditional&%24FormConditional=F&MONTH=" + DateTime.Now.ToString("yyyyMM") + "&%24Submit=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}&SMSOUTNEW=";
            string youhuiData = "service=direct%2F1%2Fsvcquery.DictimeQuery%2F%24Form&sp=S0&Form0=bquery&BCYC_ID=" + DateTime.Now.ToString("yyyyMM") + "&bquery=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}";

            string getUrl = "http://www.xj.10086.cn/service/fee/svcquery/AllFunctionOperation/";

            WebOperResult hfWr = QueryItem(cc, cookies, huafeiData, hfUrl);
            entity.Dispatcher.Invoke(_setResultHandler, new object[] { entity, "话费查询完毕", "" });

            WebOperResult zdWr = QueryItem(cc, cookies, zhangdanData, zdUrl);
            entity.Dispatcher.BeginInvoke(_setResultHandler, new object[] { entity, "账单查询完毕", "" });
            WebOperResult yhWr = QueryItem(cc, cookies, youhuiData, yhUrl);
            entity.Dispatcher.Invoke(_setResultHandler, new object[] { entity, "优惠查询完毕", "" });

            RequestContext reContext = RequestContext.DefaultContext();
            reContext.ContentType = "text/html";
            reContext.URL = getUrl;
            reContext.CookieContainer = cc;
            reContext.CookieContainer.Add(cookies);
            WebOperResult reResult = HttpWebHelper.Get(reContext);

            entity.Dispatcher.Invoke(_setResultHandler, new object[] { entity, "开通业务查询完毕。查询完毕", "" });
        }


    }
}
