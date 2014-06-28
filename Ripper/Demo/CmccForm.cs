using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WebLoginer;
using WebLoginer.Core;


namespace WeiBoGrab
{
    public partial class CmccForm : Form
    {
        HttpHelper http = new HttpHelper();
        public CmccForm()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //BaiDuLogin();
            //CsdnLogin();
            //CcLogin();
            //Simple();

            //XjLogin();

            //HttpTest();
            //XjLogin();


            //FetionWebLoginer loginer = new FetionWebLoginer();
            //string loginUrl = "https://xj.ac.10086.cn/login";
            //string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            //string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";
            //Encoding encode = Encoding.UTF8;

            //string tunnelUrl = "http://xj.ac.10086.cn:443";
            //HttpWebResponse thp = loginer.TunnelHttpResponse(tunnelUrl, null, null, HttpVersion.Version10);
            //WebOperResult twr = new WebOperResult(thp, encode, false);

            //HttpWebResponse hp = loginer.CreateGetHttpResponse(loginUrl, null, twr.Cookies);
            //WebOperResult wr = new WebOperResult(hp, encode);


            //string numID = string.Empty;
            //string postdata = string.Format("ERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
            //     "13565801462", "337339", System.Web.HttpUtility.UrlEncode(GetValueById(wr.Text, "numId")));
            //HttpWebResponse lhp = loginer.CreatePostHttpResponse(postUrl, postdata, null, encode, wr.Cookies, wr.ResponseHeaders);

            //WebOperResult lwr = new WebOperResult(lhp, encode);
            //Console.WriteLine(lwr.Text);




            ////parserContent();
            ////return;

            #region Delete
            //while (true)
            //{
            //    string loginUrl = "https://xj.ac.10086.cn/login";
            //    string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";

            //    //Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8

            //    HttpItem item = new HttpItem()
            //    {
            //        URL = "https://xj.ac.10086.cn/login",
            //        Method = "GET",
            //        Accept = "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
            //        UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36"
            //    };
            //    HttpResult result = http.GetHtml(item);
            //    string Cookies = result.Cookie;
            //    string retCode = result.Html;


            //    HttpItem login = new HttpItem()
            //    {
            //        URL = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal",
            //        Method = "POST",
            //        Accept = "*/*",
            //        Referer = "https://xj.ac.10086.cn/login",
            //        Postdata = string.Format("ERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
            //        "13565801462", "337339", System.Web.HttpUtility.UrlEncode(GetValueById(retCode, "numId"))),//"ERIAL_NUMBER=13565801462&USER_PASSWD=337339&passwordType=00&numId=111sq1nTf9JNTPycpTCJvqt4rDW2bJfjWrv1z1dnX8FJsLvv3HLpPGh%21-1280290979%211402994057161&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes", //"VerifyCode=" + txtVerCode.Text + "&__VerifyValue=" + VerifyValue + "&__name=" + txtUser.Text + "&password=" + txtPass.Text + "&cid=216&cname=凤凰V&banbeng=1&systemversion=4_4&",
            //        Cookie = Cookies
            //    };
            //    HttpResult result1 = http.GetHtml(login);
            //    Cookies += result1.Cookie; //result.Cookie.Replace("sysinfo=0; ", "");
            //    retCode = result1.Html;
            //}
            #endregion
        }

        private void CsdnLogin()
        {
            string loginUrl = "http://www.dataguru.cn/member.php?mod=logging&action=login";

            string userName = "firetw@163.com";
            string password = "1qaz!QAZ";
            crifanLib lib = new crifanLib();


            HttpWebResponse getrep = HttpUtils.CreateGetHttpResponse(loginUrl, null, null, null);
            WebOperResult getwr = new WebOperResult(getrep, Encoding.GetEncoding("gbk"));

            CookieCollection getcc = getrep.Cookies;
            if (getrep.Cookies == null || getrep.Cookies.Count < 1)
            {
                getcc = lib.parseSetCookie(getrep.Headers["Set-Cookie"]);
            }

            //formhash=a1b39417&referer=http%3A%2F%2Fwww.dataguru.cn%2F&loginfield=username&username=firetw&password=1qaz%21QAZ&questionid=0
            //&answer=&loginsubmit=true

            IDictionary<string, string> parameters = new Dictionary<string, string>();


            parameters.Add("formhash", "a1b39417");
            parameters.Add("referer", "http%3A%2F%2Fwww.dataguru.cn%2F");
            parameters.Add("loginfield", "username");
            parameters.Add("username", "firetw");
            parameters.Add("password", "1qaz%21QAZ");
            parameters.Add("questionid", "0");
            parameters.Add("answer", "");
            parameters.Add("loginsubmit", "true");


            //parameters.Add("username_LXBlY", System.Web.HttpUtility.UrlEncode(userName));
            //parameters.Add("password3_LXBlY", System.Web.HttpUtility.UrlEncode(password));
            //parameters.Add("reback", "");


            string postUrl = "http://www.dataguru.cn/member.php?mod=logging&action=login&loginsubmit=yes&loginhash=L01P1&inajax=1";
            HttpWebResponse response = HttpUtils.CreatePostHttpResponse(postUrl, parameters, null, null, Encoding.GetEncoding("gbk"), getcc);
            WebOperResult wr51 = new WebOperResult(response, Encoding.GetEncoding("gbk"));




            HttpWebResponse getrep1 = HttpUtils.CreateGetHttpResponse(loginUrl, null, null, response.Cookies);
            WebOperResult getwr1 = new WebOperResult(getrep1, Encoding.GetEncoding("gbk"));
        }
        private void BaiDuLogin()
        {


            //List<string> list = GetGoList(File.ReadAllText("refresh.txt"));
            //return;
            BaiduLoginer loginer = new BaiduLoginer();
            string loginUrl = "https://passport.baidu.com/v2/getpublickey?token=789025683ceee61c9870862fcdefebea&tpl=mn&apiver=v3&tt=1403158872786&callback=bd__cbs__nuzy53 ";
            string postUrl = "https://passport.baidu.com/v2/api/?login";
            string referer = "http://www.baidu.com/";
            Encoding encode = Encoding.UTF8;

            //string tunnelUrl = "http://xj.ac.10086.cn:443";
            //HttpWebResponse thp = loginer.TunnelHttpResponse(tunnelUrl, null, null, HttpVersion.Version10);
            //WebOperResult twr = new WebOperResult(thp, encode, false);

            /*
             * HTTP/1.1 200 OK
Server: 
Date: Thu, 19 Jun 2014 06:21:12 GMT
Content-Type: application/javascript; charset=UTF-8
Connection: keep-alive
Last-Modified: Thu, 19 Jun 2014 06:21:12 6JunGMT
Pragma: public
Expires: 0
Cache-Control: public
ETag: w/"jU8mifTwE5oKjCSZM1S0vQhNkLUsdlQ2:1403158872"
P3P: CP=" OTI DSP COR IVA OUR IND COM "
Content-Length: 381

bd__cbs__nuzy53({"errno":'0',"msg":'',"pubkey":'-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDCkfkltdQWAFilw0B7EDi9n+h9\r\nnL5snW6NNS9lt9W7AmBJ1Q0Gy6tQoZxmpIPLAcJwayeenFgwQm5ahlQKy0faWO\/v\r\nWS\/6CrByNya8h4FRdAEZFmryCFKaa2YLHWYgTn2+C\/T74AXAwLfS2N9iBup9l5xd\r\nf54VsZgO7kM0f0ZOoQIDAQAB\r\n-----END PUBLIC KEY-----',"key":'TMlilDXCM7tErR2ETYxcekomDK4FetdU'})
             * */

            //HttpWebResponse hp = loginer.CreateGetHttpResponse(loginUrl, null, null);
            //WebOperResult wr = new WebOperResult(hp, encode);

            ////Cookie: BAIDUID=E5B186D2E9D5C84C029EF714114700DA:FG=1; HISTORY=49d76034e84632c69096ce22873d; SAVEUSERID=4873d54346f74bd12335; USERNAMETYPE=1; UBI=fi_PncwhpxZ%7ETaMM9VamhqhrbWQMOJXIZ5xK8c93V2SJXiMKNObVRBYvaTtCw0Xg9gBW8kPneipIJ694Q5IKTuF8-G1DBxz1J3jLS-wY09T7W1zZAc%7Eg3ZO9sTvoc%7EymrPkbDBggeL8rzp9bbMVdYvbasU_; H_PS_PSSID=6567_4145_5231_1438_5225_6996_6505_7056_6018_7157_6930_6860_6699_6836_7134_7050_6983; HOSUPPORT=1


            //string data = "staticpage=http%3A%2F%2Fwww.baidu.com%2Fcache%2Fuser%2Fhtml%2Fv3Jump.html&charset=UTF-8&token=789025683ceee61c9870862fcdefebea&tpl=mn&subpro=&apiver=v3&tt=1403158877107&codestring=&safeflg=0&u=http%3A%2F%2Fwww.baidu.com%2F&isPhone=false&quick_user=0&logintype=dialogLogin&logLoginType=pc_loginDialog&loginmerge=true&splogin=rate&username=firetw&password=wNVX64zefMC54bYc2pCMPv4yfQa1r3U1fHrhBkDK9eqGCNqaRx74V2T6%2FQRU3zrW3eLbmeutARaMVMV6IRi3uRtHTZ654GUYUFfqA3KWEu8clFNiL5rCuQ%2FU3h3NHWSdsKsMohRw14zKQ3IeNlEYWnoychGW1IcCy8qmjsWfxfM%3D&verifycode=&mem_pass=on&rsakey=TMlilDXCM7tErR2ETYxcekomDK4FetdU&crypttype=12&ppui_logintime=39548&callback=parent.bd__pcbs__qkz2ic";
            //HttpWebResponse lhp = loginer.CreatePostHttpResponse(postUrl, data, null, encode, wr.Cookies, wr.ResponseHeaders);
            //WebOperResult lwr = new WebOperResult(hp, Encoding.GetEncoding("ISO-8859-1"));


            //loginUrl = "http://home.51cto.com/index.php?s=/Index/doLogin";
            //string userName = "firetw@163.com";
            //string password = "1qaz!QAZ";

            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("email", userName);
            //parameters.Add("passwd", password);

            //HttpWebResponse response = loginer.CreatePostHttpResponse(loginUrl, parameters, null, Encoding.UTF8, null);
            //WebOperResult wr51 = new WebOperResult(response, Encoding.GetEncoding("ISO-8859-1"));


            //HttpWebResponse get = loginer.CreateGetHttpResponse("http://home.51cto.com/index.php?s=/Home/index", null, wr51.Cookies);
            //WebOperResult get51 = new WebOperResult(response, Encoding.UTF8);

            HttpHelper http = new HttpHelper();
            string Cookies = string.Empty, Cookies1 = string.Empty;

            //HttpItem item = new HttpItem()
            //{
            //    URL = "http://home.51cto.com/index.php?s=/Index/doLogin",
            //    Method = "POST",
            //    Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
            //    Referer = "http://home.51cto.com/index.php",
            //    Postdata = "email=firetw%40163.com&passwd=1qaz%21QAZ&reback=",
            //};
            //HttpResult result = http.GetHtml(item);
            //Cookies += result.Cookie;//.Replace("sysinfo=0; ", "");
            //string retCode = result.Html;
            //HttpItem item1 = new HttpItem()
            // {
            //     URL = "http://home.51cto.com/index.php?s=/Home/index",
            //     Method = "GET",
            //     Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
            //     Cookie = Cookies
            // };
            //HttpResult result1 = http.GetHtml(item1);
            //Cookies1 += result1.Cookie;
            //string retCode1 = result1.Html;

            loginUrl = "http://home.51cto.com/index.php?s=/Index/doLogin";
            string userName = "firetw@163.com";
            string password = "1qaz!QAZ";
            crifanLib lib = new crifanLib();


            HttpWebResponse getrep = HttpUtils.CreateGetHttpResponse(loginUrl, null, null, null);
            WebOperResult getwr = new WebOperResult(getrep, Encoding.UTF8);

            CookieCollection getcc = getrep.Cookies;
            if (getrep.Cookies == null || getrep.Cookies.Count < 1)
            {
                getcc = lib.parseSetCookie(getrep.Headers["Set-Cookie"]);
            }

            //email=firetw%40163.com&passwd=1qaz%21QAZ&reback=
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("email", "firetw%40163.com");
            parameters.Add("passwd", "1qaz%21QAZ");
            parameters.Add("reback", "");

            //CookieCollection collection = new CookieCollection();
            //collection.Add(new Cookie("Hm_lvt_f77ea1ecd95cb2a1bc65cbcb3aaba7d4", "1401935787"));
            //collection.Add(new Cookie("_ourplusFirstTime", "114-6-5-11-10-29"));
            //collection.Add(new Cookie("__gads", "ID=0ee46ada76730fdd:T=1402040817:S=ALNI_MZr82s795ClpHxF9NdBCKMOOVBUMw"));
            //collection.Add(new Cookie("PHPSESSID", "a5b896a78503912205286be48e38a5da"));
            //collection.Add(new Cookie("lastlogin", "on"));
            //collection.Add(new Cookie("pub_cookietime", "0"));
            //collection.Add(new Cookie("CNZZDATA80510366", "cnzz_eid%3D766588797-1403161439-http%253A%252F%252Fhome.51cto.com%252F%26ntime%3D1403169289"));
            //collection.Add(new Cookie("_ourplusReturnCount", "13"));
            //collection.Add(new Cookie("_ourplusReturnTime", "114-6-19-17-31-14"));

            //for (int i = 0; i < collection.Count; i++)
            //{
            //    Cookie c = collection[i];
            //    c.Domain = ".51cto.com";
            //}

            HttpWebResponse response = HttpUtils.CreatePostHttpResponse(loginUrl, parameters, null, null, Encoding.UTF8, getcc);
            WebOperResult wr51 = new WebOperResult(response, Encoding.UTF8);




            string cookie = "Hm_lvt_f77ea1ecd95cb2a1bc65cbcb3aaba7d4=1401935787; _ourplusFirstTime=114-6-5-11-10-29; __gads=ID=0ee46ada76730fdd:T=1402040817:S=ALNI_MZr82s795ClpHxF9NdBCKMOOVBUMw; PHPSESSID=a5b896a78503912205286be48e38a5da; lastlogin=on; pub_cookietime=0; CNZZDATA80510366=cnzz_eid%3D766588797-1403161439-http%253A%252F%252Fhome.51cto.com%252F%26ntime%3D1403169289; _ourplusReturnCount=11; _ourplusReturnTime=114-6-19-17-15-54";

            CookieCollection cc = lib.parseSetCookie(response.Headers["Set-Cookie"]);

            List<string> list = GetGoList(wr51.Text);

            CookieCollection tmpCC = response.Cookies; //wr51.Cookies;
            foreach (var item in list)
            {

                //HttpWebResponse tmphp = HttpUtils.CreateGetHttpResponse(item, null, null, tmpCC);
                //WebOperResult tmpwr = new WebOperResult(tmphp, Encoding.UTF8);
                //tmpCC = tmphp.Cookies;
            }

            string url = "http://home.51cto.com/index.php?s=/Home/index";
            HttpWebResponse wr = HttpUtils.CreateGetHttpResponse(url, null, null, response.Cookies);
            WebOperResult wr1 = new WebOperResult(wr, Encoding.UTF8);


            //Hm_lvt_f77ea1ecd95cb2a1bc65cbcb3aaba7d4=1401935787; _ourplusFirstTime=114-6-5-11-10-29; __gads=ID=0ee46ada76730fdd:T=1402040817:S=ALNI_MZr82s795ClpHxF9NdBCKMOOVBUMw; PHPSESSID=a5b896a78503912205286be48e38a5da;
            //lastlogin=on; pub_cookietime=0; CNZZDATA80510366=cnzz_eid%3D766588797-1403161439-http%253A%252F%252Fhome.51cto.com%252F%26ntime%3D1403169289; _ourplusReturnCount=13; _ourplusReturnTime=114-6-19-17-31-14
        }



        protected System.Net.WebProxy GetWebProxy()
        {
            System.Net.WebProxy proxy = new System.Net.WebProxy("127.0.0.1", 8888);

            return proxy;
        }

        private void HttpTest()
        {
            string loginUrl = "https://xj.ac.10086.cn/login";
            string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";
            string lurl = "http://www.xj.10086.cn/my/";
            string service = "http://www.xj.10086.cn/service/";

            Encoding encode = Encoding.UTF8;

            string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
            HttpItem litem = new HttpItem
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = DefaultUserAgent,
                URL = loginUrl,
                WebProxy = GetWebProxy(),
                Method = "GET",
                Encoding = encode,
            };

            HttpResult result = http.GetHtml(litem);
            string Cookies = result.Cookie;
            string retCode = result.Html;


            string postdata = string.Format("ERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 "13609920469", "337339", GetValueById(retCode, "numId"));//System.Web.HttpUtility.UrlEncode()

            postdata = System.Web.HttpUtility.UrlEncode(postdata);

            HttpItem pitem = new HttpItem
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = DefaultUserAgent,
                URL = postUrl,
                WebProxy = GetWebProxy(),
                Method = "POST",
                Encoding = encode,
                Cookie = Cookies,
                Postdata = postdata,
                Referer = service
            };

            HttpResult presult = http.GetHtml(pitem);
            string pCookies = presult.Cookie;
            string pretCode = presult.Html;


            HttpItem mitem = new HttpItem
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = DefaultUserAgent,
                URL = lurl,
                WebProxy = GetWebProxy(),
                Method = "GET",
                Encoding = encode,
                Cookie = Cookies
            };

            HttpResult mresult = http.GetHtml(mitem);
            string mCookies = mresult.Cookie;
            string mretCode = mresult.Html;




        }

        private void XjLogin()
        {
            string loginUrl = "https://xj.ac.10086.cn/login";
            string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";
            string lurl = "https://www.xj.10086.cn/service/";


            string rUrl = "https://www.xj.10086.cn/app?service=page/IcsLogin&listener=initPage";
            //$("#icsloginhtml").load("/app?service=page/IcsLogin&listener=loginError&SailingSSO_error_code="+SailingSSO_error_code+"&SERIAL_NUMBER="+num, function(){login_sel_tongxz_list();});
            //$("#icsloginhtml").load("/app?service=page/IcsLogin&listener=initPage", function(){login_sel_tongxz_list();});

            //HttpItem litem = new HttpItem
            //{
            //    Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
            //    URL = loginUrl,
            //    WebProxy = GetWebProxy(),
            //    Method = "GET",
            //    Encoding = Encoding.UTF8,
            //};

            //HttpResult result = http.GetHtml(litem);
            //string Cookies = result.Cookie;
            //string retCode = result.Html;










            #region Delete
            FetionWebLoginer loginer = new FetionWebLoginer();

            Encoding encode = Encoding.UTF8;

            HttpWebResponse wr = loginer.CreateGetHttpResponse(loginUrl, null, null, null);
            WebOperResult wr1 = new WebOperResult(wr, Encoding.UTF8);


            string postdata = string.Format("SERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 "13639948029", "337339", GetValueById(wr1.Text, "numId"));//System.Web.HttpUtility.UrlEncode()

            //postdata = System.Web.HttpUtility.UrlEncode(postdata);

            HttpWebResponse lhp = loginer.CreatePostHttpResponse(postUrl, postdata, null, encode, wr.Cookies, wr.Headers);
            WebOperResult lwr = new WebOperResult(lhp, encode);

            string myUrl = "https://www.xj.10086.cn/service/";
            HttpWebResponse mywr = loginer.CreateGetHttpResponse(myUrl, null, lhp.Cookies, null);
            WebOperResult vmywr = new WebOperResult(mywr, Encoding.UTF8);

            HttpWebResponse rwr = loginer.CreateGetHttpResponse(referer, null, vmywr.Cookies, null);
            WebOperResult rwr1 = new WebOperResult(rwr, Encoding.UTF8);


            HttpWebResponse vwr = loginer.CreateGetHttpResponse(rUrl, null, vmywr.Cookies, null);
            WebOperResult vwr1 = new WebOperResult(vwr, Encoding.UTF8);



            Console.WriteLine(vmywr.Text);
            #endregion

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
        /// 好像用重建CookieContainer就可以登陆了,OY!
        /// </summary>
        private void NewXjLogin()
        {
            string loginUrl = "https://xj.ac.10086.cn/login";
            string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";
            string rUrl = "https://www.xj.10086.cn/app?service=page/IcsLogin&listener=initPage";



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




            //13639975410	151515
            string postdata = string.Format("SERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 "13639975410", "151515", GetValueById(readResult.Text, "numId"));//System.Web.HttpUtility.UrlEncode()

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



            RequestContext reContext = RequestContext.DefaultContext();
            reContext.ContentType = "text/html";
            reContext.URL = rUrl;
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


            //RequestContext lContext1 = RequestContext.DefaultContext();
            //lContext1.ContentType = "text/html";
            //lContext1.URL = referer;
            //lContext1.Cookies.Add(lContext.Cookies);
            //lContext1.Cookies.Add(lResult.Cookies);

            //WebOperResult lResult1 = WnHttpHelper.Get(lContext1);
        }


        /// <summary>
        /// 使用Fiddler Web Debugger生成的Cookie可以正常登陆页面
        /// 在Cookie不超时的情况下
        /// </summary>
        private void UseCookie()
        {
            //ICS_JSESSIONID_33=SJqkTq1H2nXhSXjSVfnJdvn1Ccrp1kztJkm4yYX14bhVRg2qnFs2!1420090150!-2073890956; AT-ICS-18844=CBOJOOAKEJJM; WEBTRENDS_ID=110.156.52.161-1403682576.545128; CmWebtokenid=13565824286,xj; WT_FPC=id=2c36cc72218569246af1403682834265:lv=1403682834265:ss=1403682834265
            //ICS_JSESSIONID_33=SJqkTq1H2nXhSXjSVfnJdvn1Ccrp1kztJkm4yYX14bhVRg2qnFs2!1420090150!-2073890956; 
            //AT-ICS-18844=CBOJOOAKEJJM; 
            //WEBTRENDS_ID=110.156.52.161-1403682576.545128; 
            //CmWebtokenid=13565824286,xj; 
            //WT_FPC=id=2c36cc72218569246af1403682834265:lv=1403682834265:ss=1403682834265
            string url = "http://www.xj.10086.cn/app?service=page/ownedbusi.SaleActivityVIPScoreHome&listener=initPage";


            CookieCollection cc = new CookieCollection();
            //Cookie: 
            //ICS_JSESSIONID_33=s9ncTqTSncR11k9GYZGnZK4mpDXRVysvY7R5bHZ7dgkFKPWGBn5w!1420090150!-2073890956; 
            //AT-ICS-18844=CBOJOOAKEJJM; 
            //WEBTRENDS_ID=110.156.52.161-1403682576.545128; 
            //AT-10086-47873=IEOJOOAKFAAA; 
            //CmWebtokenid=13565824286,xj; 
            //WT_FPC=id=2c36cc72218569246af1403682834265:lv=1403686746028:ss=1403686644736; 
            //CmProvid=xj

            cc.Add(new Cookie("ICS_JSESSIONID_33", "s9ncTqTSncR11k9GYZGnZK4mpDXRVysvY7R5bHZ7dgkFKPWGBn5w!1420090150!-2073890956", "/", ".10086.cn"));
            cc.Add(new Cookie("AT-ICS-18844", "CBOJOOAKEJJM", "/", ".10086.cn"));
            cc.Add(new Cookie("WEBTRENDS_ID", "110.156.52.161-1403682576.545128", "/", ".10086.cn"));
            cc.Add(new Cookie("CmWebtokenid", System.Web.HttpUtility.UrlEncode("13565824286,xj"), "/", ".10086.cn"));
            cc.Add(new Cookie("WT_FPC", "2c36cc72218569246af1403682834265:lv=1403686746028:ss=1403686644736", "/", ".10086.cn"));


            RequestContext reContext = RequestContext.DefaultContext();
            reContext.ContentType = "text/html";
            reContext.URL = url;
            reContext.Cookies.Add(cc);
            WebOperResult rebResult = HttpWebHelper.Get(reContext);


        }

        private void CcLogin()
        {
            FetionWebLoginer loginer = new FetionWebLoginer();

            string loginUrl = "https://xj.ac.10086.cn/login";
            string postUrl = "https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal";
            //https://xj.ac.10086.cn/cas2/loginSamlPortal?type=portal
            string referer = "http://www.xj.10086.cn/service/fee/ownedbusi/SaleActivityVIPScoreHome/";




            Encoding encode = Encoding.UTF8;


            //string tunnelUrl = "http://xj.ac.10086.cn:443";
            //HttpWebResponse thp = loginer.TunnelHttpResponse(tunnelUrl, null, null, HttpVersion.Version10);
            //WebOperResult twr = new WebOperResult(thp, encode, false);

            HttpWebResponse hp = loginer.CreateGetHttpResponse(loginUrl, null, null);
            WebOperResult wr = new WebOperResult(hp, encode);

            hp = loginer.CreateGetHttpResponse(loginUrl, null, hp.Cookies);
            wr = new WebOperResult(hp, encode);

            string numID = string.Empty;
            string postdata = string.Format("ERIAL_NUMBER={0}&USER_PASSWD={1}&passwordType=00&numId={2}&service=https%3A%2F%2Fwww.xj.10086.cn%2Fservice%2F&systemCode=111&failedUrl=https%3A%2F%2Fxj.ac.10086.cn%2Flogin&userType=0&fromLogin=yes",
                 "13565801462", "337339", GetValueById(wr.Text, "numId"));//System.Web.HttpUtility.UrlEncode()

            //CmProvid=xj; WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403245668946:ss=1403245668946

            //hp.Cookies.Add(new Cookie("CmProvid", "xj") { Domain = "xj.ac.10086.cn" });
            //hp.Cookies.Add(new Cookie("WT_FPC", "27ac83cfcf55d5478e01403232699581") { Domain = "xj.ac.10086.cn" });
            //hp.Cookies.Add(new Cookie("lv", "1403245668946") { Domain = "xj.ac.10086.cn" });
            //hp.Cookies.Add(new Cookie("ss", "1403245668946") { Domain = "xj.ac.10086.cn" });

            HttpWebResponse lhp = loginer.CreatePostHttpResponse(postUrl, postdata, null, encode, hp.Cookies, hp.Headers);

            WebOperResult lwr = new WebOperResult(lhp, encode);
            Console.WriteLine(lwr.Text);


            //HttpWebResponse hp1 = loginer.CreateGetHttpResponse(referer, null, null);
            //WebOperResult wr1 = new WebOperResult(hp, encode);
        }

        private void Simple()
        {
            string url = "https://www.xj.10086.cn/service/";

            //          Cookie: CmProvid=xj; WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403257454026:ss=1403257450852; CmWebtokenid=13565912478,xj; cmtokenid=8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn; ICS_JSESSIONID_33=LJTZTkCK1Z6GjzcpGKK3J2FRcTs29TFTnsLkRpmLChTMnl5Ds6tQ!1420090150!-2073890956; AT-ICS-18844=CBOJOOAKEJJM; AT-10086-47873=IEOJOOAKFAAA

            CookieCollection cc = new CookieCollection();
            //Cookie: CmProvid=xj; WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403257454026:ss=1403257450852; CmWebtokenid=13565912478,xj; cmtokenid=8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn; ICS_JSESSIONID_33=LJTZTkCK1Z6GjzcpGKK3J2FRcTs29TFTnsLkRpmLChTMnl5Ds6tQ!1420090150!-2073890956; AT-ICS-18844=CBOJOOAKEJJM; AT-10086-47873=IEOJOOAKFAAA
            //cc.Add(new Cookie("CmProvid", "xj") { Domain = "www.xj.10086.cn" });

            cc.Add(new Cookie("AT-10086-47873", "IEOJOOAKFAAA") { Domain = "www.xj.10086.cn" });
            cc.Add(new Cookie("AT-ICS-18844", "CBOJOOAKEJJM") { Domain = "www.xj.10086.cn" });
            cc.Add(new Cookie("CmProvid", "xj") { Domain = "www.xj.10086.cn" });
            cc.Add(new Cookie("cmtokenid", System.Web.HttpUtility.UrlEncode("8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn")) { Domain = "www.xj.10086.cn" });
            cc.Add(new Cookie("CmWebtokenid", System.Web.HttpUtility.UrlEncode("13565912478,xj")) { Domain = "www.xj.10086.cn" });
            cc.Add(new Cookie("ICS_JSESSIONID_33", "LJTZTkCK1Z6GjzcpGKK3J2FRcTs29TFTnsLkRpmLChTMnl5Ds6tQ!1420090150!-2073890956") { Domain = "www.xj.10086.cn" });

            CookieCollection wtFpc = new CookieCollection();
            wtFpc.Add(new Cookie("id", "27ac83cfcf55d5478e01403232699581") { Domain = "www.xj.10086.cn" });
            wtFpc.Add(new Cookie("lv", "1403257454026") { Domain = "www.xj.10086.cn" });
            wtFpc.Add(new Cookie("ss", "1403257450852") { Domain = "www.xj.10086.cn" });


            crifanLib lib = new crifanLib();

            string cookie = "CmProvid=xj; WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403245668946:ss=1403245668946; CmWebtokenid=13565912478,xj; cmtokenid=8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn; ICS_JSESSIONID_33=DcJKTjhh96Bpt3Y2Jxvn3PTRqhtVrhLh121yWTBRrhMDvmQDS1V2!1420090150!-2073890956; AT-ICS-18844=CBOJOOAKEJJM; AT-10086-47873=IEOJOOAKFAAA";
            CookieCollection getcc = lib.parseSetCookie(cookie, "www.xj.10086.cn");

            cc.Add(wtFpc);

            FetionWebLoginer loginer = new FetionWebLoginer();
            HttpWebResponse hp = loginer.CreateGetHttpResponse(url, null, getcc);
            WebOperResult wr = new WebOperResult(hp, Encoding.UTF8);

            //id=27ac83cfcf55d5478e01403232699581:lv=1403257454026:ss=1403257450852

            //ICS_JSESSIONID_33=LJTZTkCK1Z6GjzcpGKK3J2FRcTs29TFTnsLkRpmLChTMnl5Ds6tQ!1420090150!-2073890956
            //CmWebtokenid=13565912478,xj
            //cmtokenid=8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn
            //AT-10086-47873=IEOJOOAKFAAA
            //AT-ICS-18844=CBOJOOAKEJJM


            //Cookie: CmProvid=xj; WT_FPC=id=27ac83cfcf55d5478e01403232699581:lv=1403257454026:ss=1403257450852; CmWebtokenid=13565912478,xj; cmtokenid=8a6e699a458c9fe90146b8412ff42b90@xj.ac.10086.cn; ICS_JSESSIONID_33=LJTZTkCK1Z6GjzcpGKK3J2FRcTs29TFTnsLkRpmLChTMnl5Ds6tQ!1420090150!-2073890956; AT-ICS-18844=CBOJOOAKEJJM; AT-10086-47873=IEOJOOAKFAAA
        }


        string parserContent()
        {
            string content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/data.txt");
            /*
             * <input type="hidden" id="passwordType" name="passwordType" value="00" /> <input
	type="hidden" name="numId" id="numId"
	value="111sCbSTfXG0QpQ03tlLkZgHVDyh1DTDQqv9QnpsT289dLg5JympzPW!-1280290979!1402984198331" /> <input
	type="hidden" id="service" name="service"
	value="http://www.xj.10086.cn/app?service=page/myMobile.Home&listener=initPage" />
<input type="hidden" name="systemCode" value="111" /> <input
	name="failedUrl" type="hidden" value="https://xj.ac.10086.cn/login" /> <input
	type="hidden" name="userType" value="0" /> <input type="hidden"
	name="fromLogin" value="yes" /></form>
*/



            return null;
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

        private void button1_Click(object sender, EventArgs e)
        {
            NewXjLogin();
            //UseCookie();
        }
    }
}
