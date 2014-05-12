using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text;
using System.Threading;
using Logger;
using System.Text.RegularExpressions;

namespace Ripper.TaskDispather
{


    public class MjTask : BaseTask
    {
        HttpWebRequest _request = null;
        Encoding _gb2312 = null;
        ILogger logger;

        public MjTask(WebContext context)
        {
            this.Context = context;
            logger = LoggerManager.GetLog();
            _gb2312 = Encoding.GetEncoding("GB2312");
        }
        const string sUserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1700.76 Safari/537.36";
        const string sContentType = "application/x-www-form-urlencoded";
        public override void DoTask()
        {
            WebContext context = Context as WebContext;
            if (context == null) return;
            if (string.IsNullOrEmpty(context.Dir)) return;
            if (!Directory.Exists(context.Dir))
            {
                Directory.CreateDirectory(context.Dir);
            }


            string orgUrl = context.Url;
            for (int index = context.Start; index <= context.End; index++)
            {
                try
                {
                    string url = orgUrl;
                    if (index != context.Start)
                    {
                        url = url.Replace(context.Flag, "targetPage=" + index);
                    }
                    _request = WebRequest.Create(url) as HttpWebRequest;
                    if (_request == null) return;
                    _request.CookieContainer = context.CookieContainer;
                    _request.UserAgent = sUserAgent;
                    _request.ContentType = sContentType;
                    _request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

                    //http://zhoufoxcn.blog.51cto.com/792419/561934
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    context.CookieContainer = new CookieContainer();

                    StringBuilder builder = new System.Text.StringBuilder();
                    foreach (Cookie cookie in response.Cookies)
                    {
                        context.CookieContainer.Add(cookie);
                        builder.Append(DumpCookie(cookie));

                    }
                    File.WriteAllText(Path.Combine(context.Dir, "cookie.cookie"), builder.ToString());

                    Stream dataStream = response.GetResponseStream();

                    string html = string.Empty;
                    using (StreamReader reader = new StreamReader(dataStream, _gb2312))
                    {
                        html = reader.ReadToEnd();
                        string filePath = string.Empty;
                        filePath = Path.Combine(context.Dir, index + ".html");
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        File.WriteAllText(filePath, html);
                    }
                    Thread.Sleep(context.Span);
                }
                catch (Exception ex)
                {
                    logger.Log("SpecialTask", "DoTask", ex.ToString(), LogLevel.ERROR);

                }
            }
        }

        private string DumpCookie(Cookie cook)
        {
            StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine(String.Format("{0} = {1}", cook.Name, cook.Value));
            builder.AppendLine(String.Format("Domain: {0}", cook.Domain));
            builder.AppendLine(String.Format("Path: {0}", cook.Path));
            builder.AppendLine(String.Format("Port: {0}", cook.Port));
            builder.AppendLine(String.Format("Secure: {0}", cook.Secure));

            builder.AppendLine(String.Format("When issued: {0}", cook.TimeStamp));
            builder.AppendLine(String.Format("Expires: {0} ", cook.Expires));
            builder.AppendLine(String.Format("Expired:{0}", cook.Expired));
            builder.AppendLine(String.Format("Don't save: {0}", cook.Discard));
            builder.AppendLine(String.Format("Comment: {0}", cook.Comment));
            builder.AppendLine(String.Format("Uri for comments: {0}", cook.CommentUri));
            builder.AppendLine(String.Format("Version: RFC {0}", cook.Version == 1 ? "2109" : "2965"));

            // Show the string representation of the cookie.
            builder.AppendLine(String.Format("String: {0}", cook.ToString()));
            builder.AppendLine(String.Format("-----------"));
            return builder.ToString();
        }
    }

    public class StartMjJob
    {
        public static void Start()
        {
            //440
            int start = 1;
            Dictionary<int, WebContext> contextMap = new Dictionary<int, WebContext>();
            List<ITask> list = new List<ITask>();
            for (int i = 0; i < 10; i++)
            {
                WebContext context = new WebContext
                {
                    Url = "http://guang.taobao.com/detail/index.htm?spm=a310p.2219213.6861921.232.yrLSmu&uid=78301140&sid=6781951878#!/s6758211290/?targetPage=1",
                    Dir = "I:/wb/mj",
                    Flag = "targetPage=1",
                    Start = 1,
                    End = 68,
                    Span = 1000
                };


                MjTask st = new MjTask(context);
                //list.Add(st);

            }
            CookieContainer cc = new CookieContainer();


            //Cookie cookie = new Cookie { };
            //System.Web.HttpCookie c = new System.Web.HttpCookie();
            WebContext context2 = new WebContext
            {
                Url = "http://guang.taobao.com/detail/index.htm?spm=a310p.2219213.6861921.232.yrLSmu&uid=78301140&sid=6781951878#!/s6758211290/?targetPage=1",
                Dir = "I:/wb/mj",
                Flag = "targetPage=1",
                Start = 1,
                End = 1,
                Span = 1000,
                CookieContainer = cc
            };
            CookieContainer c1 = MakeContainer("I:/wb/mj");
            list.Add(new MjTask(context2));
            context2.CookieContainer = c1;
            TaskManager.Instance.AddTask(list);
        }


        //如果 name  或者 value  有  逗号 就会报错。
        //直接替代value 为  "%2c"
        //http://msdn.microsoft.com/zh-cn/library/kesbe27x(v=vs.110).aspx
        private static CookieContainer MakeContainer(string path)
        {
            CookieContainer container = new CookieContainer();
            Cookie cookie = null;
            string line = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(path, "cookie.cookie")))
            {
                while (reader.Peek() > 0)
                {
                    line = reader.ReadLine();
                    if (Regex.IsMatch(line, "^-*$"))
                    {
                        container.Add(cookie);
                        cookie = null;
                    }
                    else
                    {
                        Match match = Regex.Match(line, @"(.*)\s*[:=]\s(.*)");
                        if (match.Success)
                        {
                            string key = match.Groups[1].Value.Trim();
                            string value = match.Groups[2].Value.Trim();

                            if (!string.IsNullOrEmpty(value))
                            {
                                switch (key)
                                {
                                    case "Domain":
                                        cookie.Domain = value;
                                        break;
                                    case "Path":
                                        cookie.Path = value;
                                        break;
                                    case "Port":
                                        cookie.Port = value;
                                        break;
                                    case "Secure":
                                        cookie.Secure = Boolean.Parse(value);
                                        break;
                                    case "When issued":
                                        //cookie.TimeStamp = Boolean.Parse(value);
                                        break;
                                    case "Expires":
                                        cookie.Expires = DateTime.Parse(value);
                                        break;
                                    case "Expired":
                                        cookie.Expires = DateTime.Parse(value);
                                        break;
                                    case "Don't save":
                                        cookie.Discard = Boolean.Parse(value);
                                        break;
                                    case "Comment":
                                    case "Version":
                                    case "String":
                                        break;
                                    case "Uri for comments":
                                        if (!string.IsNullOrEmpty(value))
                                            cookie.CommentUri = new Uri(value);
                                        break;
                                    default:
                                        cookie = new Cookie(key, value);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return container;
        }

    }
}

