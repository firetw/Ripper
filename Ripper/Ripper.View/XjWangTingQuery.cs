using Ripper.View.Henan;
using Ripper.View.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WebLoginer.Core;

namespace Ripper.View
{
    public class XjWangTingQuery : XjWTTask
    {
        string loginUrl = "https://xj.ac.10086.cn/login";
        string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
        string initPage = "https://www.xj.10086.cn/app?service=page/IcsLogin&listener=initPage";
        string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";

        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Random _random = new Random();
        ExportItem _exportItem = null;


        CookieContainer _container = new CookieContainer();
        CookieCollection _cookies = new CookieCollection();

        XjParser _parser = new XjParser();


        Encoding encoding = null;

        public XjWangTingQuery()
        {
            encoding = Encoding.GetEncoding("gbk"); //Encoding.UTF8;
        }

        public override void Do(Entity context)
        {
            _exportItem = new ExportItem
            {
                Tel = Context.Tel,
                Pwd = Context.Pwd
            };
            for (int i = 0; i < ExecConfig.RetryCount; i++)
            {
                try
                {
                    Context.TaskStatus = "";
                    if (Login(Context.Tel, Context.Pwd))
                    {
                        Context.TaskStatus = "登陆成功";
                        Query();
                        _exportItem.Status = "正常";
                        Context.TaskStatus = "正常结束";
                        Context.IsLogin = true;
                    }
                    else
                    {
                        Context.TaskStatus = "请检查用户名和密码";
                        Context.IsLogin = false;
                    }
                }
                catch (Exception ex)
                {
                    _exportItem.Status = "统计异常";
                    Context.TaskStatus = "执行出现异常";
                    _log.Error(ex);
                }
                if (_exportItem.Status == "正常")
                {
                    break;
                }
                else
                {
                    _exportItem.Status = "统计异常";
                }
            }
            NPOIHelper.Instance.WriteRow(_exportItem);
        }

        protected override void HandlerTimeout()
        {
            _log.Error(Context.ExecInfo);
            NPOIHelper.Instance.WriteRow(_exportItem);
        }

        private void Query()
        {

            string hfUrl = "http://www.xj.10086.cn/service/fee/feequery/BalanceQuery/";//话费
            string zdUrl = "http://www.xj.10086.cn/service/fee/feequery/BillQueryNew/";//账单
            string yhUrl = "http://www.xj.10086.cn/service/fee/svcquery/DictimeQuery/";//优惠

            string yueUrl = "https://www.xj.10086.cn/service/fee/feequery/BalanceQuery/";//余额

            string jifenUrl = "https://www.xj.10086.cn/service/points/score/ScoreSumQuery/";
            string jiaoFeiUrl = "https://www.xj.10086.cn/service/payfee/feequery/PayHisQuery/";

            string beginTime = Context.StartTime;
            string endTime = Context.EndTime;
            DateTime now = DateTime.Now;
            DateTime lastMonth = DateTime.Now.AddMonths(-1);

            string yueData = "service=direct%2F1%2Ffeequery.BalanceQuery%2F%24Form&sp=S0&Form0=%24FormConditional%2CBLANCE%2C%24FormConditional%240&%24FormConditional=T&%24FormConditional%240=F&BLANCE=%B2%E9%D1%AF&operHipInfo=&com.ailk.ech.framework.html.TOKEN={0}";
            string huafeiData = "service=direct%2F1%2Ffeequery.BalanceQuery%2F%24Form&sp=S0&Form0=%24FormConditional%2CBLANCE%2C%24FormConditional%240&%24FormConditional=T&%24FormConditional%240=F&BLANCE=%B2%E9%D1%AF&operHipInfo=&com.ailk.ech.framework.html.TOKEN={0}";
            string zhangdanData = "service=direct%2F1%2Ffeequery.BillQueryNew%2F%24Form&sp=S0&Form0=%24Submit%2C%24FormConditional&%24FormConditional=F&MONTH=" + now.ToString("yyyyMM") + "&%24Submit=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}&SMSOUTNEW=";
            string lastZhangdanData = "service=direct%2F1%2Ffeequery.BillQueryNew%2F%24Form&sp=S0&Form0=%24Submit%2C%24FormConditional&%24FormConditional=F&MONTH=" + lastMonth.ToString("yyyyMM") + "&%24Submit=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}&SMSOUTNEW=";
            string jiFenData = "service=direct%2F1%2Fscore.ScoreSumQuery%2F%24Form&sp=S0&Form0=%24FormConditional%2CSCORE%2C%24FormConditional%240%2C%24FormConditional%241&%24FormConditional=T&%24FormConditional%240=F&%24FormConditional%241=F&SCORE=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}";

            string jiaoFeiData = "service=direct%2F1%2Ffeequery.PayHisQuery%2F%24Form&sp=S0&Form0=%24FormConditional%2C%24FormConditional%240%2C%24FormConditional%241%2Cbsubmit%2C%24FormConditional%242&%24FormConditional=T&%24FormConditional%240=F&%24FormConditional%241=T&%24FormConditional%242=F&BEGIN_TIME=" + beginTime + "&RECV_TIME=" + endTime + "&bsubmit=%B2%E9%D1%AF&com.ailk.ech.framework.html.TOKEN={0}&SMSOUTPUT=&operHipInfo=";


            Context.TaskStatus = "余额查询";
            //余额
            WebOperResult yeWr = QueryItem(_container, _cookies, yueData, yueUrl);
            _exportItem.Data[0] = _parser.ParserYuE(yeWr.Text);

            Context.TaskStatus = "话费查询";
            ///话费
            //WebOperResult hfWr = QueryItem(_container, _cookies, huafeiData, hfUrl);
            //_exportItem.Data[1] = 0;


            Context.TaskStatus = "账单查询";
            //账单
            WebOperResult zdWr = QueryItem(_container, _cookies, zhangdanData, zdUrl);
            List<double> list = _parser.ZhangDan(zdWr.Text);
            _exportItem.Data[1] = list[0];
            _exportItem.Data[2] = list[1];



            Context.TaskStatus = "历史账单查询";
            WebOperResult zdWr1 = QueryItem(_container, _cookies, lastZhangdanData, zdUrl);
            list = _parser.ZhangDan(zdWr1.Text);
            _exportItem.Data[3] = list[0];
            _exportItem.Data[4] = list[1];

            Context.TaskStatus = "交费历史查询";
            //交费历史查询
            WebOperResult jiaoFeiWr = QueryItem(_container, _cookies, jiaoFeiData, jiaoFeiUrl);
            string jiaoFeiContent = jiaoFeiWr.Text;
            double tmpJf = _parser.JiaoFei(jiaoFeiContent);
            int yeShu = GetYeShu(jiaoFeiContent);


            CookieCollection cookies = jiaoFeiWr.Response.Cookies;
            for (int i = 2; i <= yeShu; i++)
            {
                string url = "https://www.xj.10086.cn/app?service=ajaxDirect/1/feequery.PayHisQuery/feequery.PayHisQuery/javascript/payHiaPaginationPart&pagename=feequery.PayHisQuery&eventname=queryBusi&pagination_iPage=" + i + "&partids=payHiaPaginationPart&ajaxSubmitType=get&ajax_randomcode=" + _random.NextDouble();

                WebOperResult tmpWr = Get(url, _container, jiaoFeiWr.Response.Cookies);
                tmpJf += _parser.JiaoFeiFanYe(tmpWr.Text);

                cookies = tmpWr.Response.Cookies;
            }
            _exportItem.Data[5] = tmpJf;

            Context.TaskStatus = "积分查询";
            //积分
            WebOperResult jfWr = QueryItem(_container, _cookies, jiFenData, jifenUrl);
            _exportItem.Data[6] = _parser.ParserJiFen(jfWr.Text);

            //            <div class="PageLeft">
            //                    第
            //1/ 2 页 每页 10 项 共 12 项 
            //                </div>
        }

        private int GetYeShu(string content)
        {
            if (string.IsNullOrEmpty(content)) return 0;
            int index = content.IndexOf("<div class=\"PageLeft\">");
            if (index == -1) return 0;

            int endIndex = content.IndexOf("</div>", index);
            if (endIndex == -1) return 0;
            string tmp = content.Substring(index, endIndex - index);
            tmp = _parser.RemoveNewRow(tmp);
            Match match = null;
            if ((match = Regex.Match(tmp, @"\d+/\s*(\d+)\s*页")).Success)
            {
                return Convert.ToInt32(match.Groups[1].Value);
            }

            return 0;
        }

        public WebOperResult QueryItem(CookieContainer cc, CookieCollection cookies, string postData, string url)
        {
            RequestContext getContext = RequestContext.DefaultContext();
            getContext.ContentType = "text/html";
            getContext.URL = url;
            getContext.Method = "GET";
            getContext.CookieContainer = cc;
            getContext.Encoding = encoding;
            if (cookies != null)
                getContext.CookieContainer.Add(cookies);

            WebOperResult getResult = HttpWebHelper.Get(getContext);

            string token = GetValueById(getResult.Text, "com.ailk.ech.framework.html.TOKEN");
            string data = string.Format(postData, token);


            string postUrl = "http://www.xj.10086.cn/app";

            RequestContext postContext = RequestContext.DefaultContext();
            postContext.URL = postUrl;
            postContext.Encoding = encoding;
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
        private string GetMsg(string phone, double time, string msg)
        {
            return string.Format("Tel:{0},耗时:{1} 毫秒\r\n{2}", phone, time, msg);
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
        /// <summary>
        /// 好像不用重建CookieContainer就可以登陆了,OY!
        /// </summary>
        public bool Login(string phone, string pwd)
        {
            System.Diagnostics.Stopwatch sw = null;
            if (Config.LogLevel != 2)
                sw = Stopwatch.StartNew();
            Encoding encode = Encoding.UTF8;

            Cookie wtFpcCookie = new Cookie("WT_FPC", GetWtFpc()) { Domain = ".10086.cn" };
            _cookies.Add(new Cookie("CmProvid", "xj") { Domain = ".10086.cn" });
            _cookies.Add(new Cookie("pgv_pvi", "7629626368") { Domain = ".10086.cn" });
            _cookies.Add(wtFpcCookie);

            WebOperResult readResult = Get(loginUrl, _container, _cookies);



            string postdata = string.Format("SERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 phone, pwd, GetValueById(readResult.Text, "numId"));//System.Web.HttpUtility.UrlEncode()


            WebOperResult postResult = Post(postUrl, _container, readResult.Response.Cookies, postdata, false);

            if (Regex.IsMatch(postResult.Text, "window.alert(\"手机号码或服务密码错误\");"))
            {
                return false;
            }

            string serviceUrl = "https://www.xj.10086.cn/service/";
            WebOperResult serviceResult = Get(serviceUrl, _container, readResult.Response.Cookies);


            WebOperResult reResult = Get(initPage, _container, postResult.Response.Cookies);
            //<a id="login" class="carmine" href="https://xj.ac.10086.cn/logout">[退出]</a>
            string content = reResult.Text;


            return true;
        }

        private WebOperResult Get(string url, CookieContainer container, CookieCollection cookies)
        {
            RequestContext reContext = RequestContext.DefaultContext();
            reContext.Encoding = encoding;
            reContext.ContentType = "text/html";
            reContext.URL = url;
            reContext.CookieContainer = container;
            reContext.CookieContainer.Add(cookies);

            WebOperResult reResult = HttpWebHelper.Get(reContext);
            return reResult;
        }

        private WebOperResult Post(string url, CookieContainer container, CookieCollection cookies, string postData, bool autoRedirect = true)
        {
            RequestContext postContext = RequestContext.DefaultContext();
            postContext.Encoding = encoding;
            postContext.URL = url;
            postContext.Allowautoredirect = autoRedirect;
            postContext.Method = "POST";
            postContext.Accept = "application/xhtml+xml, */*";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = postData;
            postContext.CookieContainer = container;
            postContext.CookieContainer.Add(cookies);

            WebOperResult postResult = HttpWebHelper.Post(postContext);


            return postResult;
        }



    }

}
