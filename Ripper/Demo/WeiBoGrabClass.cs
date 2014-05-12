using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HtmlAgilityPack;
using System.Drawing;
using System.Runtime.InteropServices;
using mshtml;
using FetionLoginer.VerifyHelper;

namespace WeiBoGrab
{
    class WeiBoGrabClass
    {
    }

    public class GetPage
    {
        //加载初始页面
        public string GetLoginPage(WebBrowser browser)
        {
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            while (browser.Document.GetElementById("pl_login_form").InnerHtml == null)
            {
                Application.DoEvents();
            }
            return "加载登陆页面完成。";
        }
        //加载用户主页
        public string GetMainPage(WebBrowser browser)
        {
            while (browser.DocumentTitle != "我的首页 新浪微博-随时随地分享身边的新鲜事儿")
            {
                Application.DoEvents();
            }
            //确保加载完所需内容
            while (browser.Document.GetElementById("pl_rightmod_myinfo") != null &&
                browser.Document.GetElementById("pl_rightmod_myinfo").Children.Count < 2)
            {
                Application.DoEvents();
            }

            return "加载个人主页完成。";
        }
        //加载用户关注对象的第一页
        public string GetFollowsPage(WebBrowser browser)
        {
            while (browser.DocumentTitle != "我关注的人 新浪微博-随时随地分享身边的新鲜事儿")
            {
                Application.DoEvents();
            }
            while (browser.Document.GetElementById("pl_relation_myfollow") == null)
            {
                Application.DoEvents();
            }
            while (browser.Document.GetElementById("pl_relation_myfollow").Children.Count < 3)
            {
                Application.DoEvents();
            }
            return "关注对象页面第一页加载完成。";
        }
        //加载用户关注对象的下一页
        public string GetFollowsNextPage(WebBrowser browser)
        {
            //将原页面的关注对象列表清空（关注对象列表为children[2].children[1]）
            //加载新页面3=browser.Document.GetElementById("pl_relation_myfollow").Children[2].Children.Count
            //不明白，孩子个数显示明明是3，但是述操作却正确。。。 
            //browser.Document.GetElementById("pl_relation_myfollow").Children[2].Children.Count < 4
            //<!--  -->此类标签有时会被当做标签计数或提取，需要实际分析

            while (browser.Document.GetElementById("pl_relation_myfollow").Children.Count < 3 ||
                   browser.Document.GetElementById("pl_relation_myfollow").Children[2].Children.Count < 4)
            {
                Application.DoEvents();
            }
            //当上述条件满足后，再加载，便是新生成的内容
            return "关注对象下一页加载完成。";
        }
        //加载关注对象的主页的第一页
        public string GetFollowMainPage(WebBrowser browser)
        {
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            //当微博是杂志、新闻类时
            if (browser.Document.GetElementById("epfeedlist") != null)
            {
                while (browser.Document.GetElementById("feed_list") == null)
                {
                    Application.DoEvents();
                }
                return "关注对象主页第一页加载完成。";
            }
            //当微博是个人、媒体类时
            if (browser.Document.GetElementById("pl_content_hisFeed") == null)
            {
                while (browser.Document.GetElementById("profileFeed").InnerHtml == null)
                {
                    Application.DoEvents();
                }
            }
            while (browser.Document.GetElementById("pl_content_hisFeed").InnerHtml == null)
            {
                Application.DoEvents();
            }
            //找到feed
            HtmlElementCollection ps = browser.Document.GetElementById("pl_content_hisFeed").Children;
            int feed_postion = 0;
            //有的微博页面需要此步骤
            while (browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].InnerText == "正在加载，请稍候..." ||
                browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].InnerText == "正在加载中，请稍候...")
            {
                Application.DoEvents();
            }
            //pl_content_hisFeed加载不全
            while (browser.Document.GetElementById("pl_content_hisFeed").Children.Count < 2)
            {
                Application.DoEvents();
            }
            foreach (HtmlElement p in ps)
            {
                if (p.GetAttribute("node-type") != null && p.GetAttribute("node-type") == "feed_list")
                {
                    break;
                }
                else
                    feed_postion++;
            }
            //非第一页加载时，有此等待
            while (browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[0].InnerText == "正在加载中，请稍候..."
                   || browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[0].InnerText == "正在加载，请稍候...")
            {
                Application.DoEvents();
            }
            //微博数量及等待加载模块所在位置表示
            int hisFeed_count = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children.Count - 1;
            //表示正在加载
            bool loading = true;
            //找出加载模块位置
            HtmlElement load = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[hisFeed_count];
            int i;
            for (i = 1; (i < 10) && (hisFeed_count - i >= 0); i++)
            {
                if (load.InnerText == "正在加载中，请稍候..." || load.InnerText == "正在加载，请稍候...")
                    break;
                load = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[hisFeed_count - i];
            }
            while (loading)
            {
                loading = false;
                load.ScrollIntoView(false);
                while (load.InnerText == "正在加载中，请稍候..." || load.InnerText == "正在加载，请稍候...")
                {
                    load.ScrollIntoView(false);
                    Application.DoEvents();
                    load = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[hisFeed_count - i];
                }
                //微博加载
                //限制次数，limit有待商榷，过小会使有的微博可能会加载失败
                int Limit = 100;
                int L = 0;
                while ((browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children.Count < hisFeed_count + 2) &&
                    (L < Limit))
                {
                    L++;//防止无限加载的等待
                    Application.DoEvents();
                }
                //更新加载模块位置
                hisFeed_count = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children.Count - 1;
                //更新加载模块
                load = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[hisFeed_count];
                for (int j = 1; (j < 10) && (hisFeed_count - j >= 0); j++)//假设无效的标签数不超过10个
                {
                    if (load.InnerText == "正在加载中，请稍候..." || load.InnerText == "正在加载，请稍候...")
                        break;
                    load = browser.Document.GetElementById("pl_content_hisFeed").Children[feed_postion].Children[hisFeed_count - j];
                }
                if (load != null && (load.InnerText == "正在加载中，请稍候..." || load.InnerText == "正在加载，请稍候..."))
                {
                    loading = true;
                    load.ScrollIntoView(false);
                }
            }
            return "加载关注对象主页第一页面完成。";

        }
        //加载关注对象的的主页的下一页
        public string GetFollowMainNextPage(WebBrowser browser)
        {
            Application.DoEvents();
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            GetFollowMainPage(browser);
            //针对杂志、新闻类微博
            if (browser.Document.GetElementById("epfeedlist") == null)
                Application.DoEvents();
            return "加载关注对象后续页面完成。";
        }
    }
    //用户登陆类
    public class LoginSubmit
    {
        private string username;
        private string password;
        //初始化登陆对象
        public LoginSubmit(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        //点击登陆
        public void LoginClick(WebBrowser browser)
        {
            //登陆页面的登陆模块
            HtmlElement pl_login_form = browser.Document.GetElementById("pl_login_form");
            if (pl_login_form == null) return;
            //登陆模块中的用户名_INPUT
            HtmlElement pl_login_form_username = pl_login_form.GetElementsByTagName("INPUT")[0];
            if (pl_login_form_username == null) return;
            //让用户名输入框获取焦点(目的清空输入框)
            pl_login_form_username.InvokeMember("click");
            pl_login_form_username.SetAttribute("value", username);

            //登陆模块的密码_INPUT
            HtmlElement pl_login_form_password = pl_login_form.GetElementsByTagName("INPUT")[1];
            if (pl_login_form_password == null) return;
            //让密码输入框获取焦点(目的清空输入框)
            pl_login_form_password.InvokeMember("click");
            pl_login_form_password.SetAttribute("value", password);

            //找到登陆按钮并点击
            HtmlElementCollection IsClick = pl_login_form.GetElementsByTagName("span");
            foreach (HtmlElement Click in IsClick)
            {
                if (Click.GetAttribute("node-type") != null && Click.GetAttribute("node-type") == "submitStates")
                {
                    Click.InvokeMember("click");
                    break;
                }
            }
        }
    }


  
    //将关注对象设为一类
    public class Follow
    {
        //获取关注对象（点击用户关注对象的超链接）
        public void GetFollows(WebBrowser browser)
        {
            //获取用户的信息模块
            HtmlElement pl_rightmod_myinfo = browser.Document.GetElementById("pl_rightmod_myinfo");
            //获取关注对象子模块
            HtmlElement my_info_follow = pl_rightmod_myinfo.GetElementsByTagName("strong")[0];
            if (my_info_follow.GetAttribute("node-type") == "follow")
            {
                //判断用户是否有关注对象
                if (my_info_follow.InnerText == "0")
                    return;
                my_info_follow.InvokeMember("click");
                GetPage getfollowpage = new GetPage();
                getfollowpage.GetFollowsPage(browser);
            }
        }
        //获取关注对象的url,并写到txt中
        public void GetFollowsUrl(WebBrowser browser, StreamWriter sw)
        {
            //是否还有下一页
            bool Next = true;
            int UrlCount = 0;
            while (Next)
            {
                //默认没有下一页
                Next = false;

                HtmlElement FollowLinks = browser.Document.GetElementById("pl_relation_myfollow");
                HtmlElementCollection Links = FollowLinks.GetElementsByTagName("div");

                foreach (HtmlElement Link in Links)
                {
                    if (Link.GetAttribute("action-type") == "ignore_list")
                    {
                        HtmlNode href = HtmlNode.CreateNode(Link.InnerHtml);

                        string url = href.Attributes["href"].Value;
                        string followname = href.FirstChild.Attributes["alt"].Value;

                        sw.WriteLine("No.{0}|{1}|{2}", ++UrlCount, followname, url);
                    }
                }
                HtmlElementCollection pages = FollowLinks.GetElementsByTagName("span");

                //判断是否有下一页
                foreach (HtmlElement page in pages)
                {
                    if (page.InnerText == "下一页")
                    {
                        Next = true;
                        page.InvokeMember("click");

                        //Console.WriteLine("这个标签是："+browser.Document.GetElementById("pl_relation_myfollow").Children[2].Children[2].OuterHtml);
                        browser.Document.GetElementById("pl_relation_myfollow").Children[2].Children[2].OuterHtml = null;

                        //载入关注对象页面的下一页
                        GetPage GetNext = new GetPage();
                        GetNext.GetFollowsNextPage(browser);
                        break;
                    }
                }
            }
            sw.Close();
        }
    }
    //将微博设为一类
    public class WeiBo
    {
        private string FollowName;
        private string FollowUrl;

        public WeiBo(string FollowName, string FollowUrl)
        {
            this.FollowName = FollowName;
            this.FollowUrl = FollowUrl;
        }
        public void GetWeiBo(WebBrowser browser)
        {
            StreamWriter sw = File.CreateText("D:\\weibo\\" + FollowName + ".txt");
            bool Next = true;
            int WeiBoCount = 0;
            browser.Navigate(new Uri(@FollowUrl));
            GetPage GetNext = new GetPage();
            GetNext.GetFollowMainPage(browser);
            //默认还没登记此类微博
            string Kind = "N";

            HtmlElement epfeedlist = browser.Document.GetElementById("epfeedlist");
            HtmlElement pl_content_hisFeed = browser.Document.GetElementById("pl_content_hisFeed");
            if (pl_content_hisFeed != null)
            {
                //媒体类微博的pl_content_hisFeed.Children[1].Children[0].TagName = "dl"

                //个人微博的pl_content_hisFeed.Children[1].OuterHtml =<!-- /高级搜索 -->
                if (pl_content_hisFeed.Children[1].Children.Count != 0)
                    //媒体（小）微博
                    Kind = "M";

                    //个人微博                
                else
                    Kind = "P";
            }
            if (epfeedlist != null)
                //杂志，新闻等微博
                Kind = "J";
            while (Next)
            {
                Next = false;
                switch (Kind)
                {
                    case "P":
                        {
                            //爬取各条微博
                            HtmlElementCollection divs = browser.Document.GetElementById("pl_content_hisFeed").GetElementsByTagName("div");
                            foreach (HtmlElement div in divs)
                            {
                                if (div.GetAttribute("node-type") == "feed_list_content")
                                    sw.WriteLine("第{0}条|" + div.InnerText, ++WeiBoCount);
                            }
                            //判断是否还有下一页
                            HtmlElementCollection spans = browser.Document.GetElementById("pl_content_hisFeed").GetElementsByTagName("span");
                            foreach (HtmlElement span in spans)
                            {
                                if (span.InnerText == "下一页")
                                {
                                    span.InvokeMember("click");
                                    Next = true;
                                    GetNext.GetFollowMainNextPage(browser);
                                    break;
                                }
                            }
                        } break;
                    case "J":
                        {
                            //爬取各条微博
                            int count_li = browser.Document.GetElementById("feed_list").Children.Count;
                            for (int i = 0; i < count_li; i++)
                            {
                                sw.WriteLine("第{0}条|" + browser.Document.GetElementById("feed_list").Children[i].GetElementsByTagName("p")[0].InnerText, ++WeiBoCount);
                            }
                            //判断是否还有下一页
                            HtmlElementCollection ems = browser.Document.GetElementById("feed_list").NextSibling.GetElementsByTagName("em");
                            int end = ems.Count;
                            if (ems[end - 1].InnerText == "下一页")
                            {
                                ems[end - 1].InvokeMember("click");
                                browser.Document.GetElementById("feed_list").OuterHtml = null;
                                GetNext.GetFollowMainNextPage(browser);
                                Next = true;
                            }
                        } break;
                    case "M":
                        {
                            HtmlElementCollection ps = browser.Document.GetElementById("pl_content_hisFeed").GetElementsByTagName("p");
                            foreach (HtmlElement p in ps)
                            {
                                if (p.GetAttribute("node-type") == "feed_list_content")
                                    sw.WriteLine("第{0}条|" + p.InnerText, ++WeiBoCount);
                            }
                            //判断是否还有下一页
                            HtmlElementCollection spans = browser.Document.GetElementById("pl_content_hisFeed").GetElementsByTagName("span");
                            foreach (HtmlElement span in spans)
                            {
                                if (span.InnerText == "下一页")
                                {
                                    span.InvokeMember("click");
                                    Next = true;
                                    GetNext.GetFollowMainNextPage(browser);
                                    break;
                                }
                            }

                        } break;
                    default: return;//还没记录的微博
                }
            }
            sw.Close();
        }
    }
}