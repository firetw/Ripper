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

namespace Ripper.TaskDispather
{


    public class SpecialTask : BaseTask
    {
        HttpWebRequest _request = null;
        Encoding _gb2312 = null;
        ILogger logger;

        public SpecialTask(WebContext context)
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
                        url = url.Replace(context.Flag, "page=" + index);
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
                    foreach (Cookie cookie in response.Cookies)
                    {
                        context.CookieContainer.Add(cookie);
                    }

                    Stream dataStream = response.GetResponseStream();

                    string html = string.Empty;
                    using (StreamReader reader = new StreamReader(dataStream, _gb2312))
                    {
                        html = reader.ReadToEnd();
                        string filePath = string.Empty;
                        if (html.Contains("小泽"))
                        {
                            if (!Directory.Exists(Path.Combine(context.Dir, "小泽")))
                            {
                                Directory.CreateDirectory(Path.Combine(context.Dir, "小泽"));
                            }
                            filePath = Path.Combine(context.Dir, "小泽", index + ".html");
                            File.WriteAllText(filePath, html);
                        }
                        if (html.Contains("苍"))
                        {
                            if (!Directory.Exists(Path.Combine(context.Dir, "苍")))
                            {
                                Directory.CreateDirectory(Path.Combine(context.Dir, "苍"));
                            }
                            filePath = Path.Combine(context.Dir, "苍", index + ".html");
                            File.WriteAllText(filePath, html);
                        }
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
            //Cookie: CNZZDATA950900=cnzz_eid%3D1027444223-1390573990-%26ntime%3D1390573990%26cnzz_a%3D0%26ltime%3D1390573985381

            //HtmlDocument htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(html);


        }
    }

    public class StartJob
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
                    Url = "http://365url.tk/thread0806.php?fid=2&search=&page=" + (i * 44 + 1),
                    Dir = "I:/wb/other",
                    Flag = "page=" + (i * 44 + 1),
                    Start = (i * 44 + 1),
                    End = (i + 1) * 44 + 1,
                    Span = 1000
                };
                if (context.End == 441)
                    context.End = 440;

                SpecialTask st = new SpecialTask(context);
                //list.Add(st);

            }
            CookieContainer cc = new CookieContainer();
            //new DateTime(1970, 1, 1).AddSeconds(i);
            //Cookie: cna=SZsACxV/yQ4CAdrO6VIPeH+W
            //Cookie: CNZZDATA950900=cnzz_eid%3D1027444223-1390573990-%26ntime%3D1390573990%26cnzz_a%3D0%26ltime%3D1390573985381
            //cc.Add(new Cookie("CNZZDATA950900", "cnzz_eid%3D1027444223-1390573990-%26ntime%3D1390573990%26cnzz_a%3D0%26ltime%3D1390573985381", "/*", "365url.tk"));
            //Cookie: CNZZDATA950900=cnzz_eid%3D1027444223-1390573990-%26ntime%3D1390573990%26cnzz_a%3D1%26ltime%3D1390573985381
            cc.Add(new Cookie("CNZZDATA950900", "cnzz_eid%3D1027444223-1390573990-%26ntime%3D1390573990%26cnzz_a%3D1%26ltime%3D1390573985381", "/*", "365url.tk"));
            WebContext context2 = new WebContext
            {
                Url = "http://365url.tk/thread0806.php?fid=2&search=&page=" + 8,
                Dir = "I:/wb/other",
                Flag = "page=" + 8,
                Start = 8,
                End = 440,
                CookieContainer = cc,
                Span = 10000
            };


            list.Add(new SpecialTask(context2));
            TaskManager.Instance.AddTask(list);

        }
    }
}

