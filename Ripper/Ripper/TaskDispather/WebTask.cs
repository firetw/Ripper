using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text;
using System.Threading;

namespace Ripper.TaskDispather
{
    public class WebTask : BaseTask
    {
        HttpWebRequest _request = null;
        Encoding _gb2312 = null;

        public WebTask(WebContext context)
        {
            this.Context = context;
            _gb2312 = Encoding.GetEncoding("GB2312");
        }

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
            for (int index = 1; index <= 10; index++)
            {
                string url = orgUrl;
                if (index != 1)
                {
                    url = url.Replace("page=1", "page=" + index);
                }
                _request = WebRequest.Create(url) as HttpWebRequest;
                if (_request == null) return;
                _request.CookieContainer = context.CookieContainer;

                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();

                Stream dataStream = response.GetResponseStream();

                string html = string.Empty;
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    html = reader.ReadToEnd();
                    string filePath = Path.Combine(context.Dir, index + ".html");
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    File.WriteAllText(filePath, html);
                }
                Thread.Sleep(context.Span);
            }


            //HtmlDocument htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(html);
        }

    }
}
