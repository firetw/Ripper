using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using mshtml;
using FetionLoginer.VerifyHelper;
using System.Threading;
using WeiBoGrab.Verify;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrEmpty(verify) || verify == "TIMEOUT" || Core.UnRecoginize == verify || verify.Length < Core.VerifyLength)
            {
                log.Error(GetMsg(string.Format("获取到验证码异常:[{0}]", verify)));
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
                    object obj = item.InvokeMember("click");
                    if (obj != null)
                        log.Error(obj.ToString());
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

        private bool IsTimeOut(DateTime time)
        {
            return IsTimeOut(time, 10);
        }
        /// <summary>
        /// 验证操作是否超时
        /// </summary>
        /// <param name="time">开始时间</param>
        /// <param name="timeOut">超时间隔，单位秒</param>
        /// <returns></returns>
        private bool IsTimeOut(DateTime time, double timeOut)
        {
            return ((DateTime.Now - time).TotalSeconds >= timeOut);
        }

        /// <summary>
        /// 登录是否成功
        /// 判断标识是否 还有"<div class="d-title">登录</div>"
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private bool ValidateLogin(WebBrowser browser)
        {
            bool result = false;
            DateTime beginTime = DateTime.Now;
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            while (browser.DocumentText.Contains("<div class=\"d-title\">登录</div>"))
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime))
                {
                    log.Warn(GetMsg(OperationMsg.LoginTimeOut));
                    return result;
                }
                return result;
            }
            while (!browser.DocumentText.Contains("退出"))
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime))
                {
                    log.Warn(GetMsg(OperationMsg.LoginTimeOut));
                    return result;
                }
            }
            return true;
        }
        private bool Parser(WebBrowser browser)
        {
            bool result = false;
            bool lingQuClickSuccess = true;
            DateTime beginTime = DateTime.Now;
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime))
                {
                    log.Warn(GetMsg(OperationMsg.LoginTimeOut));
                    return false;
                }
            }
            log.Info(GetMsg(OperationMsg.LoginSuccess));

            //chanceNums
            HtmlElement loginForm = null;
            while ((loginForm = browser.Document.GetElementById("chanceNums")) == null)
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime))
                {
                    log.Warn(GetMsg(OperationMsg.GetChanceNumsTimeOut));
                    return false;
                }
            }
            if (loginForm.InnerText == "0")
            {
                log.Info(GetMsg("该手机号抽奖机会为:0"));
                return result;
            }
            HtmlElement trigger = browser.Document.GetElementById("patHorse1");
            trigger.Focus();
            trigger.InvokeMember("click");

            //<input type="button" class="d-button d-state-highlight" value="点击领取">
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime))
                {
                    log.Warn(GetMsg("拍完之后的状态 " + OperationMsg.GetOpertaionResult));
                    return false;
                }
            }
            while (!Regex.IsMatch(browser.Document.GetElementById("patHorse1").Parent.OuterHtml, "style=\"DISPLAY: none\"", RegexOptions.IgnoreCase))
            {
                trigger.InvokeMember("click");
                Application.DoEvents(); //点击失败,重新点击
                if (IsTimeOut(beginTime, 40))
                {
                    log.Warn(GetMsg("验证是否点击成功：  " + OperationMsg.GetOpertaionResult));
                    return false;
                }
            }
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
                if (IsTimeOut(beginTime, 60))
                {
                    log.Warn(GetMsg("结果之前的状态：  " + OperationMsg.GetOpertaionResult));
                }
            }
            log.Info(GetMsg("进入操作结果判断"));
            //点完之后转那半天......

            //这个状态是否有问题
            DateTime start = DateTime.Now;
            //while (!(Regex.IsMatch(browser.DocumentText, "<div class=\"d-title\">\\s*很遗憾") || browser.DocumentText.Contains("点击领取")))
            HtmlElementCollection inputCollection = null; //browser.Document.GetElementsByTagName("INPUT");
            HtmlElement awardButton = null;
            bool failed = false;
            while (!IsTimeOut(start, 30))
            {
                inputCollection = browser.Document.GetElementsByTagName("INPUT");
                for (int i = 0; i < inputCollection.Count; i++)
                {
                    //<input type="button" class="d-button d-state-highlight" value="确定">
                    HtmlElement item = inputCollection[i];
                    if (Regex.IsMatch(item.OuterHtml, "点击领取"))
                    {
                        awardButton = item;
                        awardRecoder.Log("WeiBoGrab.FLoginSubmit", "Parase", GetMsg(awardButton.Parent.Parent.Parent.Parent.InnerText), Logger.LogLevel.INFO);
                        break;
                    }
                    //item.Parent.Parent.Parent.Parent.Parent.OuterText
                    if (item.Parent != null && item.Parent.Parent != null && item.Parent.Parent.Parent != null && item.Parent.Parent.Parent.Parent != null && item.Parent.Parent.Parent.Parent.Parent != null)
                    {
                        string context = item.Parent.Parent.Parent.Parent.Parent.OuterText;
                        if (Regex.IsMatch(context, "很遗憾"))
                        {
                            failed = true;
                            break;
                        }
                    }
                }
                if (failed) break;
                if (awardButton != null) break;
                Application.DoEvents();

            }
            if (awardButton != null)
            {
                result = true;
                //awardButton.InvokeMember("click");//这个是先获取焦点
                awardButton.Parent.Parent.Parent.Parent.Focus();
                awardButton.Focus();
                awardButton.InvokeMember("click");//这个是真正的发送事件

                //awardButton.InvokeMember("click");
                //start = DateTime.Now;

                //awardButton = null;
                //while (!IsTimeOut(start, 30))
                //{
                //    inputCollection = browser.Document.GetElementsByTagName("INPUT");
                //    for (int i = 0; i < inputCollection.Count; i++)
                //    {
                //        HtmlElement item = inputCollection[i];
                //        if (Regex.IsMatch(item.OuterHtml, "点击领取"))
                //        {
                //            awardButton = item;
                //            awardButton.Focus();
                //            awardButton.InvokeMember("click");
                //            lingQuClickSuccess = false;
                //            break;
                //        }
                //    }
                //    if (awardButton == null)
                //        lingQuClickSuccess = true;
                //    awardButton = null;
                //    if (lingQuClickSuccess)
                //        break;
                //    Application.DoEvents(); //点击失败,重新点击
                //}
            }
            if (!lingQuClickSuccess)
            {
                //log.Warn("领取失败");
            }
            if (lingQuClickSuccess && result)
            {
                //还有一个上行的指令需要发送....
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                //还有一个上行的指令需要发送....
                start = DateTime.Now;
                HtmlElement confirm = null;
                while (!IsTimeOut(start, 30))
                {
                    inputCollection = browser.Document.GetElementsByTagName("INPUT");
                    for (int i = 0; i < inputCollection.Count; i++)
                    {
                        HtmlElement item = inputCollection[i];
                        if (item.GetAttribute("type") == "button" && item.GetAttribute("value") == "确定")
                        {
                            log.Info(GetMsg("找到确定按钮：  " + OperationMsg.GetOpertaionResult));
                            confirm = item;
                            break;
                        }
                    }
                    if (confirm != null) break;
                    Application.DoEvents();
                }
                if (confirm != null)
                {
                    awardRecoder.Log("WeiBoGrab.FLoginSubmit", "Parase", GetMsg(confirm.Parent.Parent.Parent.Parent.Parent.OuterText), Logger.LogLevel.INFO);
                    log.Info(GetMsg("确定按钮点击：  " + OperationMsg.GetOpertaionResult));
                    confirm.Parent.Parent.Parent.Parent.Parent.Focus();
                    confirm.Focus();
                    //confirm.InvokeMember("click");//这个是先获取焦点
                    confirm.InvokeMember("click");//这个是真正的发送事件



                    //start = DateTime.Now;
                    //bool confirmSusscess = true;
                    //confirm = null;
                    //while (!IsTimeOut(start, 30))
                    //{
                    //    inputCollection = browser.Document.GetElementsByTagName("INPUT");
                    //    for (int i = 0; i < inputCollection.Count; i++)
                    //    {
                    //        HtmlElement item = inputCollection[i];
                    //        if (item.GetAttribute("type") == "button" && item.GetAttribute("value") == "确定")
                    //        {
                    //            confirm = item;
                    //            confirm.Focus();
                    //            confirm.InvokeMember("click");
                    //            confirmSusscess = false;
                    //            break;
                    //        }
                    //    }
                    //    if (confirm == null)
                    //        confirmSusscess = true;
                    //    confirm = null;
                    //    if (confirmSusscess)
                    //        break;
                    //}
                    //if (confirmSusscess = true)
                    //{
                    //    //log.Info("领取成功");
                    //}
                }
            }
            if (!result)
            {
                //log.Info(GetMsg(browser.DocumentText));
                log.Info(GetMsg("该号未中奖"));
            }
            return result;
        }
        private bool ShowView()
        {

            return false;
        }

        private void LoginOut(WebBrowser browser)
        {
            //<div class="nav_right"><ul><li><a href="javascript:;"> 欢迎您！</a></li><li><a class="mail" href="http://i2.feixin.10086.cn/messages" target="_blank">查看站内信</a></li><li><a class="zone" href="http://i2.feixin.10086.cn/" target="_blank">进入我的空间</a></li><li><a href="http://gz.feixin.10086.cn/Bootlick/loginout">退出</a></li></ul></div>
            //HtmlElementCollection divCollection = browser.Document.GetElementsByTagName("DIV");
            //for (int i = 0; i < divCollection.Count; i++)
            //{
            //    HtmlElement item = divCollection[i];
            //    if (item.GetAttribute("class") == "bootlick_wrap")
            //    {
            //        HtmlElementCollection aCollection = item.GetElementsByTagName("A");
            //        if (aCollection != null)
            //        {
            //            for (int j = 0; j < aCollection.Count; j++)
            //            {
            //                HtmlElement a = divCollection[j];
            //                if (a.InnerText == "退出")
            //                {
            //                    a.InvokeMember("click");
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
            Application.DoEvents();
            browser.Navigate(new Uri("http://gz.feixin.10086.cn/Bootlick/loginout"));
            browser.Navigate(new Uri("http://gz.feixin.10086.cn/Bootlick/loginout"));

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
            bool isLogin = false;
            int result = 0;
            try
            {
                isLogin = LoginClick(browser, imp);
                if (!isLogin)
                {
                    browser.Navigate(Core.FetionUrl);//应对验证码超时
                }
                if (!ValidateLogin(browser))
                {
                    log.Error(GetMsg(OperationMsg.LoginFailed));
                    browser.Navigate(Core.FetionUrl);//应对验证超时
                }
                if (Parser(browser)) result = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                if (isLogin)
                {
                    LoginOut(browser);
                }
            }
            return result;
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
