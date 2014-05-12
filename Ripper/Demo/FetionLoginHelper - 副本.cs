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
using System.Threading;

namespace WeiBoGrab
{
    //用户登陆类
    public class FLoginSubmit
    {
        private string username;
        private string password;

        public string Tel { get; set; }


        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Logger.ILogger awardRecoder = null;
        //初始化登陆对象
        public FLoginSubmit(string username, string password)
        {
            this.username = username;
            this.password = password;
            awardRecoder = Logger.LoggerManager.GetLog("award.log", Logger.LogType.Task);
        }
        private string GetMsg(string msg)
        {
            return string.Format("Tel:{0}  {1}", Tel, msg);
        }
        //点击登陆
        public bool LoginClick(WebBrowser browser, UUVerifyImp imp)
        {
            bool result = false;
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            while (browser.Document != null && browser.Document.GetElementById("pl_login_form") != null)//while (webBrowser1.Document.GetElementById("pl_login_form").InnerHtml == null)
            {
                Application.DoEvents();
            }
            //登陆页面的登陆模块
            HtmlElement trigger = browser.Document.GetElementById("patHorse1");
            trigger.InvokeMember("click");

            HtmlElement loginForm = browser.Document.GetElementById("loginfrm");
            if (loginForm == null) return result;

            HtmlElementCollection collection = loginForm.GetElementsByTagName("INPUT");
            HtmlElement userNameForm = collection[0];
            //登陆页面的登陆模块
            //HtmlElement userNameForm = browser.Document.GetElementById("loginInputText");

            userNameForm.InvokeMember("click");
            userNameForm.SetAttribute("value", username);


            HtmlElement pwdForm = collection[1];
            if (pwdForm == null) return result;
            pwdForm.InvokeMember("click");
            pwdForm.SetAttribute("value", password);

            HtmlElement verifyorm = collection[2];


            HtmlElement imgForm = loginForm.Document.GetElementById("verifyImgpic") as HtmlElement;
            if (pwdForm == null) return result;
            if (imgForm == null)
            {
                log.Error(GetMsg("获取验证码失败"));
                return false;
            }
            Bitmap bitmap = GetImage(imgForm.DomElement);
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Position = 0;
                ms.Flush();
                bytes = ms.ToArray();
            }
            string verify = string.Empty;
            try
            {
                log.Info(GetMsg("开始获取验证码"));
                verify = imp.RecognizeByBytes(bytes);
                log.Info(GetMsg(string.Format("获取验证码:[{0}]", verify)));
            }
            catch (Exception ex)
            {
                log.Error(GetMsg("验证码验证过程异常 " + ex.ToString()));
                return result;
            }
            if (string.IsNullOrEmpty(verify) || verify == "TIMEOUT" || Core.UnRecoginize == verify)
            {
                log.Error(GetMsg(string.Format("Got verify code Error verify:[{0}]", verify)));
                return result;
            }
            for (int i = 0; i < collection.Count; i++)
            {
                HtmlElement item = collection[i];
                if (item.GetAttribute("name") == "verify")
                {
                    item.InvokeMember("click");
                    item.SetAttribute("value", verify);
                    break;
                }
            }
            HtmlElementCollection inputCollection = browser.Document.GetElementsByTagName("INPUT");
            //<input type="button" class="d-button d-state-highlight" value="确定">
            for (int i = 0; i < inputCollection.Count; i++)
            {
                HtmlElement item = inputCollection[i];
                if (item.GetAttribute("type") == "button" && item.GetAttribute("value") == "确定")
                {
                    item.InvokeMember("click");
                    result = true;
                    break;
                }
            }
            if (!result)
            {
                log.Info(GetMsg("自动登录失败"));
            }
            return result;
        }
        private bool Parser(WebBrowser browser)
        {
            bool result = false;

            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            while (!Logined(browser))
            {
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            log.Info(GetMsg("自动登录成功"));
            //chanceNums
            HtmlElement loginForm = null;
            while (browser.Document.GetElementById("chanceNums") == null)
            {
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            if (loginForm.InnerText == "0")
            {
                log.Info(GetMsg("该手机号抽奖机会为:0"));
                return result;
            }

            HtmlElement trigger = browser.Document.GetElementById("patHorse1");
            trigger.InvokeMember("click");

            //<input type="button" class="d-button d-state-highlight" value="点击领取">
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            while (browser.DocumentText.Contains("点击领取" != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            HtmlElementCollection inputCollection = browser.Document.GetElementsByTagName("INPUT");
            //<input type="button" class="d-button d-state-highlight" value="确定">

            bool awardFlag = false;
            for (int i = 0; i < inputCollection.Count; i++)
            {
                HtmlElement item = inputCollection[i];
                if (item.GetAttribute("type") == "button" && item.GetAttribute("value") == "点击领取")
                {
                    item.InvokeMember("click");
                    awardFlag = true;
                    break;
                }
            }
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            if (!awardFlag)
            {
                log.Info(GetMsg("该号未中奖"));
            }
            else
            {
                HtmlElementCollection divCollection = browser.Document.GetElementsByTagName("DIV");
                for (int i = 0; i < divCollection.Count; i++)
                {
                    HtmlElement item = divCollection[i];
                    if (item.GetAttribute("class") == "award_list1 award_list")
                    {
                        HtmlElementCollection h5Collection = item.GetElementsByTagName("H5");
                        if (h5Collection != null)
                        {
                            for (int j = 0; j < h5Collection.Count; j++)
                            {
                                HtmlElement h5Element = divCollection[i];
                                awardRecoder.Log("WeiBoGrab.FLoginSubmit", "Parase", h5Element.OuterText, Logger.LogLevel.INFO);
                                result = true;
                            }
                        }
                    }
                }
                //<div class="award_list1 award_list"><h5>恭喜你！获得【和你找工作】<em>业务体验包</em> 拍马屁好礼</h5><dl><dt>奖品说明：</dt><dd>体验包是以手机报形式，重点整合校园双选会、校园招聘会、名企招聘岗位等求职信息，贴近学生生活，及时、准确地为大学生第一时间传递所需的就业、讲座、竞赛、考证等所有服务资讯内容。</dd><dd><label for="musicjd1" class="musicjd "><input type="checkbox" action-type="musicjd" id="musicjd1" checked="checked">关注和你找工作公众账号</label></dd></dl></div>
                LoginOut(divCollection);
            }
            return true;
        }

        private bool Logined(WebBrowser browser)
        {
            //HtmlElementCollection divCollection = browser.Document.GetElementsByTagName("DIV");
            //for (int i = 0; i < divCollection.Count; i++)
            //{
            //    HtmlElement item = divCollection[i];
            //    if (item.GetAttribute("class") == "bootlick_wrap")
            //    {
            //        HtmlElementCollection aCollection = item.GetElementsByTagName("A");
            //        return aCollection.Count > 2;
            //    }
            //}
            return browser.DocumentText.Contains("退出");
        }

        private void LoginOut(HtmlElementCollection divCollection)
        {
            //<div class="nav_right"><ul><li><a href="javascript:;"> 欢迎您！</a></li><li><a class="mail" href="http://i2.feixin.10086.cn/messages" target="_blank">查看站内信</a></li><li><a class="zone" href="http://i2.feixin.10086.cn/" target="_blank">进入我的空间</a></li><li><a href="http://gz.feixin.10086.cn/Bootlick/loginout">退出</a></li></ul></div>
            for (int i = 0; i < divCollection.Count; i++)
            {
                HtmlElement item = divCollection[i];
                if (item.GetAttribute("class") == "nav_right")
                {
                    HtmlElementCollection aCollection = item.GetElementsByTagName("A");
                    if (aCollection != null)
                    {
                        for (int j = 0; j < aCollection.Count; j++)
                        {
                            HtmlElement a = divCollection[j];
                            if (a.InnerText == "退出")
                            {
                                a.InvokeMember("click");
                                break;
                            }

                        }
                    }
                }

            }

        }

        [ComImport, InterfaceType((short)1), Guid("3050F669-98B5-11CF-BB82-00AA00BDCE0B")]
        private interface IHTMLElementRenderFixed
        {
            void DrawToDC(IntPtr hdc);
            void SetDocumentPrinter(string bstrPrinterName, IntPtr hdc);
        }

        public Bitmap GetImage(object obj)
        {
            IHTMLImgElement img = (IHTMLImgElement)obj;
            IHTMLElementRenderFixed render = (IHTMLElementRenderFixed)img;

            Bitmap bmp = new Bitmap(img.width, img.height);
            Graphics g = Graphics.FromImage(bmp);
            IntPtr hdc = g.GetHdc();
            render.DrawToDC(hdc);
            g.ReleaseHdc(hdc);
            return bmp;
        }



        public int Do(WebBrowser browser, UUVerifyImp imp)
        {
            bool isLogin = LoginClick(browser, imp);
            if (!isLogin) return 0;
            if (Parser(browser)) return 1;
            return 0;
        }



        #region Delete
        ///// <summary>
        ///// 返回指定WebBrowser中图片<IMG></IMG>中的图内容
        ///// </summary>
        ///// <param name="WebCtl">WebBrowser控件</param>
        ///// <param name="ImgeTag">IMG元素</param>
        ///// <returns>IMG对象</returns>
        //private Image GetWebImage(WebBrowser WebCtl, HtmlElement ImgeTag)
        //{
        //    HTMLDocument doc = (HTMLDocument)WebCtl.Document.DomDocument;
        //    HTMLBody body = (HTMLBody)doc.body;

        //    IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
        //    IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址

        //    Image oldImage = Clipboard.GetImage();
        //    rang.add(Img);
        //    rang.execCommand("Copy", false, null); //拷贝到内存
        //    Image numImage = Clipboard.GetImage();
        //    try
        //    {
        //        Clipboard.SetImage(oldImage);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    return numImage;
        //}
        #endregion
    }
}
