using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebLoginer.Core;

namespace WebLoginer.Test
{

    [TestClass]
    public class WapTest
    {
        [TestMethod]
        public void WapGetTest()
        {

            string url = "http://wap.xj.10086.cn/";
            CookieContainer _container = new CookieContainer();
            CookieCollection cookies = new CookieCollection();
            WebOperResult tmpWr = Get(url, _container, cookies);
            string content = tmpWr.Text;
            Console.WriteLine(content);
        }

        private WebOperResult Get(string url, CookieContainer container, CookieCollection cookies)
        {
            RequestContext reContext = RequestContext.DefaultContext();
            reContext.Encoding = Encoding.UTF8;
            reContext.ContentType = "text/html";
            reContext.URL = url;
            reContext.CookieContainer = container;
            reContext.CookieContainer.Add(cookies);

            WebOperResult reResult = HttpWebHelper.Get(reContext);
            return reResult;
        }

        [TestMethod]
        public void GetTime()
        {
            long time = (long)(DateTime.Now.Subtract(TimeSpan.FromHours(8)) - DateTime.Parse("1970-1-1 00:00:00")).TotalMilliseconds;



        }
    }
}
