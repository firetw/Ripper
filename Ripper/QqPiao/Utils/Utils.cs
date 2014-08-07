using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebLoginer.Core;

namespace QqPiao
{
    public class Utils
    {

        static Encoding encoding = Encoding.UTF8;

        public static WebOperResult Get(string url, CookieContainer container, CookieCollection cookies)
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

        public static WebOperResult Post(string url, CookieContainer container, CookieCollection cookies, string postData, bool autoRedirect = true)
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
