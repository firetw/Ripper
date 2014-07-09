using Ripper.View.Model;
using Ripper.View.SamSung;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebLoginer.Core;

namespace SamSung
{
    public class RegisterTask : WebTask
    {

        private bool _autoVerify;
        private CookieContainer _container;
        private CookieCollection _cookies;
        private string _serviceId;
        private string _imgVerifyCode;
        private string _attachCode = string.Empty;

        //jXeEK0fmWkinputEmailID=&
        //jXeEK0fmWkinputPhoneID=8618790781747&
        //jXeEK0fmWkinputPassword=1qaz%21QAZ&
        //jXeEK0fmWkreInputPassword=1qaz%21QAZ&
        //jXeEK0fmWk
        private string _randomCode = string.Empty;

        //英文、答案、中文
        private Tuple<string, string, string> _securityItem;
        private int _smsVerifyCompleted = 0;

        static List<string> DayOfWeek = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        static List<string> MonthOfYear = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };


        public bool AutoVerify
        {
            get { return _autoVerify; }
            set
            {
                _autoVerify = value;
            }
        }

        //先验证短信，然后再图片

        /// <summary>
        /// 0:是短信验证码
        /// 1:图片验证码
        /// 
        /// string 新验证码图片的地址
        /// </summary>
        public event Action<VerifyFailedCode, string> OnVerifyCodeFailed;
        public event Action<string> OnImgVerifyCompleted;
        public event Action OnGetSmsCompleted;
        public event Action<byte[]> OnImgCompleted;

        Encoding encoding = null;

        public RegisterTask()
        {
            encoding = Encoding.UTF8;
        }

        public override void Do(RegisterContext context)
        {
            Register();
        }

        public void Register()
        {
            string step1 = "http://membership.samsung.com/cn/utl/front/EncEventProcessEventDetailPage?eventSeq=1000000025&boardId=&page=1";
            CookieContainer container = new CookieContainer();
            CookieCollection cookies = new CookieCollection();

            WebOperResult step1Result = Get(step1, container, cookies);

            string step2 = "http://membership.samsung.com/cn/login/SAJoin";
            string pd2 = "contentTypeCd=&parameter=&linkUrl=&goUrl=";
            WebOperResult step2Result = Post(step2, container, step1Result.Response.Cookies, pd2);
            string serviceIdReport = step2Result.Text;
            string serviceID = string.Empty;

            Match match = null;
            if (!string.IsNullOrEmpty(serviceIdReport) && (match = Regex.Match(serviceIdReport, "name=\"serviceID\"\\s+value=\"([^\"]*)\"")).Success)
            {
                serviceID = match.Groups[1].Value;
                _serviceId = serviceID;
                Context.Status = TaskStatus.Success;
                Context.ExecInfo = "获取ServiceID成功";
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "获取ServiceID失败";
                return;
            }
            string step3 = "https://chn.account.samsung.com/account/check.do";
            string pd3 = "actionID=SignupAP&serviceID=" + serviceID + "&serviceName=%E4%B8%89%E6%98%9F%E4%BC%9A%E5%91%98%E4%BF%B1%E4%B9%90%E9%83%A8&ssoType=OPT_VAL&domain=membership.samsung.com&countryCode=Cn&languageCode=zh&registURL=http%3A%2F%2Fmembership.samsung.com%2Fcn%2Flogin%2FSAFunction&returnURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&goBackURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&GUID=&isReturn=";
            WebOperResult step3Result = Post(step3, container, step2Result.Response.Cookies, pd3);


            string step4 = "https://chn.account.samsung.com/account/signUp.do";
            string pd4 = string.Format("serviceID={0}", serviceID);
            WebOperResult step4Result = Post(step4, container, step3Result.Response.Cookies, pd4);
            string basicText = step4Result.Text;
            string basicCode = string.Empty;
            //<input type='hidden' name='basicInfoVO' value='osp.idm.account.vo.BasicInfoVO@48a250' />
            if (!string.IsNullOrEmpty(basicText) && (match = Regex.Match(basicText, "name=[\"\']basicInfoVO[\"\']\\s+value=[\"\']([^\"]*)[\"\']")).Success)
            {
                Context.Status = TaskStatus.Success;
                basicCode = match.Groups[1].Value;
                Context.ExecInfo = "获取basicInfoVO[" + match.Groups[1].Value + "]成功";
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "获取basicInfoVO失败";
                return;
            }


            string step5 = "https://chn.account.samsung.com/account/userVerifyChinese.do";
            string pd5 = "idmSignupCapchaYN=S&isOthersFieldView=true&membershipContextUri=https%3A%2F%2Faccount.samsung.com%2Fmembership&basicInfoVO=" + basicCode + "&isTermsSignUpPage=Y&separatorName=JoinNow&serviceID=" + serviceID;
            WebOperResult step5Result = Post(step5, container, step4Result.Response.Cookies, pd5);


            string step6 = string.Format("https://chn.account.samsung.com/account/userVerifyCheckChinese.do?checkType=CN_IDCard&serviceID={0}&separatorName=JoinNow", serviceID);
            WebOperResult step6Result = Get(step6, container, step5Result.Response.Cookies);

            string userInfoReport = step6Result.Text;//用户信息表单
            if (!string.IsNullOrEmpty(userInfoReport) && Regex.IsMatch(userInfoReport, "id=\"lastName\"\\s+"))
            {
                Context.ExecInfo = "获取用户表单成功";
                Context.Status = TaskStatus.Success;
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "获取用户表单失败";
                return;
            }
            //呈现用户名和密码注册页面
            string step7 = "https://chn.account.samsung.com/account/userVerifyResultChinese.do";
            string pd7 = string.Format("serviceID={0}&separatorName=JoinNow&checkType=CN_IDCard&lastName={1}&firstName={2}&inputVerifyNumber={3}", serviceID, Context.LastName, Context.FirstName, Context.Id);
            WebOperResult step7Result = Post(step7, container, step6Result.Response.Cookies, pd7);
            string registerContent = step7Result.Text;
            if (!string.IsNullOrEmpty(registerContent) && Regex.IsMatch(registerContent, "NICE_VERIFY_SUCCESS"))
            {
                Context.ExecInfo = "身份认证已成功完成";
                Context.Status = TaskStatus.Success;
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "身份认证已失败";
                return;
            }

            //
            string step7_1 = string.Format("https://chn.account.samsung.com/account/userChineseNameCheckDupList.do?serviceID={0}&separatorName=JoinNow", _serviceId);
            WebOperResult step7_1Result = Get(step7_1, container, step6Result.Response.Cookies);


            string step8 = "https://chn.account.samsung.com/account/signUp.do";
            string pd8 = string.Format("serviceID={0}&separatorName=JoinNow", _serviceId);
            WebOperResult step8Result = Post(step8, container, step7Result.Response.Cookies, pd8);
            //尝试解决显示验证码

            string imgVerifyReport = step8Result.Text;
            if (Regex.IsMatch(imgVerifyReport, "使用现有\\s+ID\\s+登录"))// browser.DocumentText.Contains(""))
            {
                Context.ExecInfo = "该用户已注册";
                this.IsCompleted = true;
                return;
            }
            if (!string.IsNullOrEmpty(imgVerifyReport) && (match = Regex.Match(imgVerifyReport, "<img id=\"inputCaptchaCodeImg\" src=\"([^\"]*)\"")).Success)
            {
                string imgUrl = match.Groups[1].Value;
                if ((match = Regex.Match(imgVerifyReport, "<input\\s+name=\"(\\S+)inputEmailID\"\\s+type=\"text\"\\s+id=\"\\1inputEmailID\"")).Success)
                {
                    _randomCode = match.Groups[1].Value;
                }
                _attachCode = ParserAttachCode(imgVerifyReport);
                if (!string.IsNullOrEmpty(_attachCode))
                {
                    Context.ExecInfo = "获取附加码成功";
                    Context.Status = TaskStatus.Success;
                }
                else
                {
                    Context.Status = TaskStatus.Failed;
                    Context.ExecInfo = "获取附加码失败";
                    this.IsCompleted = true;
                    return;
                }

                Dictionary<string, string> question = ParserSecurity(imgVerifyReport);
                Tuple<string, string, string> keyValuePair = null;
                foreach (var item in question)
                {
                    SecurityItem si = SecurityQuestionManager.Instance.GetAnswer(item.Key);
                    if (si != null)
                    {
                        if (keyValuePair == null)
                            keyValuePair = new Tuple<string, string, string>(si.LabelEn, si.Answer(), si.LabelCn);
                    }
                    else
                    {
                        SecurityQuestionManager.Instance.AddQuestion(item.Key, item.Value);
                    }
                }
                if (keyValuePair == null)
                {
                    KeyValuePair<string, string> qa = SecurityQuestionManager.Instance.Questions.First();
                    keyValuePair = new Tuple<string, string, string>(qa.Key, SecurityQuestionManager.Instance.DefaultAnswer, qa.Value);
                }
                SecurityQuestionManager.Instance.FlushQuestion();
                _securityItem = keyValuePair;

                GetImageSource(imgUrl, container, step8Result.Response.Cookies);

                Context.ExecInfo = "获取验证码成功";
                Context.Status = TaskStatus.Success;
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "获取验证码失败";
                this.IsCompleted = true;
                return;
            }
            _cookies = cookies;
            _container = container;
            SendSms(container, cookies);
        }
        public string ParserAttachCode(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            string line = string.Empty;
            string code = string.Empty;
            Dictionary<string, string> questionList = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, "fnSetEleValue\\(document.signUpForm", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        break;
                    }
                }
            }
            string[] array = line.Split(';');
            foreach (var item in array)
            {
                Match match = Regex.Match(item, "fnSetEleValue\\(document.signUpForm,\\s*[\"\']([^\"]*)[\"\'],\\s*[\"\']([^\"]*)[\"\']\\)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (!string.IsNullOrEmpty(code))
                        code += "&";
                    code += match.Groups[1].Value + "=" + match.Groups[2].Value;
                }
            }
            return code;



            //fnSetEleValue(document.signUpForm, "acajvq", "dgtlsf");fnSetEleValue(document.signUpForm, "kppztx", "jbyrfx");fnSetEleValue(document.signUpForm, "zuexdv", "kkbhts");fnSetEleValue(document.signUpForm, "njuxpe", "lcmjzn");fnSetEleValue(document.signUpForm, 'cbvenx', "lnusgw");
            //fnSetEleValue(document.signUpForm, "acajvq", "dgtlsf");fnSetEleValue(document.signUpForm, "kppztx", "jbyrfx");fnSetEleValue(document.signUpForm, "zuexdv", "kkbhts");fnSetEleValue(document.signUpForm, "njuxpe", "lcmjzn");fnSetEleValue(document.signUpForm, 'cbvenx', "lnusgw");

        }
        private void SendSms(CookieContainer container, CookieCollection cookies)
        {
            string step2 = string.Format("https://chn.account.samsung.com/account/verifyPhoneID.do?serviceID={0}", _serviceId);
            WebOperResult step2Result = Get(step2, container, _cookies);

            string step3 = "https://chn.account.samsung.com/account/verifyPhoneIDSendCode.do";
            string pd3 = string.Format("serviceID={0}&cccNationality=CHN&inputPhoneNumber={1}", _serviceId, Context.Tel);
            WebOperResult step3Result = Post(step3, container, step2Result.Response.Cookies, pd3);

            _container = container;
            _cookies = step3Result.Response.Cookies;

            string smsText = step3Result.Text;
            if (string.IsNullOrEmpty(smsText))
            {
                Context.ExecInfo = "获取手机验证码失败";
                return;
            }
            if (Regex.IsMatch(smsText, "短信已发送"))
            {
                Context.ExecInfo = "请输入手机验证码";
                if (OnGetSmsCompleted != null)
                    OnGetSmsCompleted();
            }
            else if (Regex.IsMatch(smsText, "此电话号码已在使用"))
            {
                Context.ExecInfo = "此电话号码已在使用";
                this.IsCompleted = true;
            }
            else
            {
                Match match = null;
                if ((match = Regex.Match(smsText, "sendResultMsg\":\"([^\"]*)\",")).Success)
                {
                    Context.ExecInfo = match.Groups[1].Value;
                }
                else
                {
                    Context.ExecInfo = "获取手机验证码失败";
                }
            }
        }

        public void OnReceiveTelCodeCompleted(string telCode)
        {
            if (_smsVerifyCompleted == 1) return;
            if (string.IsNullOrEmpty(telCode)) return;

            //密码
            string step1 = "https://chn.account.samsung.com/account/verifyPhoneIDCheckCode.do";
            string pd1 = string.Format("serviceID={0}&cccNationality=CHN&inputPhoneNumber={1}&inputAuthenticateNumber={2}", _serviceId, Context.Tel, telCode);
            WebOperResult step1Result = Post(step1, _container, _cookies, pd1);
            string verifyTelReport = step1Result.Text;
            _cookies = step1Result.Response.Cookies;
            if (string.IsNullOrEmpty(verifyTelReport) || !Regex.IsMatch(verifyTelReport, "\"checkResultYN\":\"Y\""))
            {
                if (OnVerifyCodeFailed != null)
                {
                    OnVerifyCodeFailed(VerifyFailedCode.TelVerifyFailed, string.Empty);
                }
            }
            else
            {
                Context.ExecInfo = "短信验证通过";
            }

            //else if (AutoVerify)
            //{
            //    lock (DayOfWeek)
            //    {
            //        System.Threading.Interlocked.Exchange(ref _smsVerifyCompleted, 1);
            //        if (!string.IsNullOrEmpty(_imgVerifyCode))
            //        {
            //            OnReceiveImgCodeCompleted(_imgVerifyCode);
            //        }
            //    }
            //}
        }

        public void OnReceiveImgCodeCompleted(string imgVerifyCode)
        {
            Tuple<string, string, string> qa = _securityItem;

            ////<img id="inputCaptchaCodeImg" src="/account/find/accountSCaptchaCodeView.do?serviceID=ts3rap101s&amp;captchaGbn=SIGNUP&amp;Fri Jul 04 2014 10:44:34 GMT+0800 (中国标准时间)" width="300" height="57" alt="Security Code">
            //{"jsonData":{"checkResultTelephoneNumber":"8618790793746","checkResultYN":"Y"}}
            //验证手机验证码是否正确
            //<img id="inputCaptchaCodeImg" src="/account/find/accountSCaptchaCodeView.do?serviceID=ts3rap101s&amp;captchaGbn=SIGNUP" width="300" height="57" alt="Security Code">
            //图片 key

            //图片验证码
            string step2 = "https://chn.account.samsung.com/account/checkInputBasicInfo.do";
            //0：手机号
            //1：密码
            //2：名字
            //3：姓
            string pwd = PwdRep.Instance.GetPwd();
            string question = qa.Item1;//"SS_WHAT_IS_YOUR_FAVOURITE_BOOK_Q_ABB";
            SecurityItem si = SecurityQuestionManager.Instance.GetAnswer(question);
            string pd2 = string.Format(@"serviceID=" + _serviceId + @"&separatorName=JoinNow&loginIDTypeCode=001&" + _randomCode + "inputEmailID=&" + _randomCode + "inputPhoneID=86{0}&" + _randomCode + "inputPassword={1}&" + _randomCode + "reInputPassword={1}&title=&genderTypeCode=" + Context.GenderTypeCode + "&firstName={2}&lastName={3}&nickName=&nationality=CHN&yyyy={4}&mm={5}&dd={6}&birthDate={7}&securityQuestionID={8}&securityAnswer={9}&phoneNumber=&inputCaptchaCode={10}&newsLetter=Y&smsReceiveYNFlag=&postalCode=&checkTerms=Y&checkPdu=Y&checkDca=Y&check3rdParty=Y&" + _attachCode,
                Context.Tel,
                pwd,
                System.Web.HttpUtility.UrlEncode(Context.FirstName),
                System.Web.HttpUtility.UrlEncode(Context.LastName),
                Context.Year,
                Context.Month,
                Context.Day,
                Context.Birthday,
                qa.Item1,
                System.Web.HttpUtility.UrlEncode(qa.Item2),
                imgVerifyCode
                );
            WebOperResult step2Result = Post(step2, _container, _cookies, pd2);
            _cookies = step2Result.Response.Cookies;
            string verifySubmitReport = step2Result.Text;
            if (string.IsNullOrEmpty(verifySubmitReport) || !Regex.IsMatch(verifySubmitReport, "{\"jsonData\":{}}"))
            {
                if (OnVerifyCodeFailed != null)
                {
                    OnVerifyCodeFailed(VerifyFailedCode.ImgVerifyFailed, string.Empty);
                }
                //if (AutoVerify)
                //{
                //    ReGetImage();
                //}
            }
            else
            {
                SubmitUserInfo(pwd, qa, imgVerifyCode);
            }
        }
        void SubmitUserInfo(string pwd, Tuple<string, string, string> qa, string imgVerifyCode)
        {
            string step3 = "https://chn.account.samsung.com/account/doSignUp.do";
            string pd3 = string.Format(@"serviceID={0}&separatorName=JoinNow&loginIDTypeCode=001&" + _randomCode + "inputEmailID=&" + _randomCode + "inputPhoneID=86{1}&" + _randomCode + "inputPassword={2}&" + _randomCode + "reInputPassword={3}&title=&genderTypeCode=" + Context.GenderTypeCode + "&firstName={4}&lastName={5}&nickName=&nationality=CHN&yyyy={6}&mm={7}&dd={8}&birthDate={9}&securityQuestionID={10}&securityAnswer={11}&phoneNumber=&inputCaptchaCode={12}&newsLetter=Y&smsReceiveYNFlag=&postalCode=&checkTerms=Y&checkPdu=Y&checkDca=Y&check3rdParty=Y&" + _attachCode,
                    _serviceId,
                     Context.Tel,
                    System.Web.HttpUtility.UrlEncode(pwd),
                    System.Web.HttpUtility.UrlEncode(pwd),
                    System.Web.HttpUtility.UrlEncode(Context.FirstName),
                    System.Web.HttpUtility.UrlEncode(Context.LastName),
                    Context.Year,
                    Context.Month,
                    Context.Day,
                    Context.Birthday,
                    qa.Item1,
                    System.Web.HttpUtility.UrlEncode(qa.Item2),
                    imgVerifyCode
                    );

            CookieCollection cookies = new CookieCollection();

            string domain = "chn.account.samsung.com";
            cookies.Add(new Cookie("_common_country", "CN") { Domain = domain });
            cookies.Add(new Cookie("_common_lang", "zh_cn") { Domain = domain });
            cookies.Add(new Cookie("_common_lang_path", "zh") { Domain = domain });
            cookies.Add(new Cookie("deviceType", "pc") { Domain = domain });
            cookies.Add(new Cookie("deviceId", "etc") { Domain = domain });
            cookies.Add(new Cookie("s_ncomui_ev14", "%5B%5B%27cnsportal%27%2C%271402449345182%27%5D%5D") { Domain = domain });
            cookies.Add(new Cookie("_geoip_country", "cn") { Domain = domain });
            cookies.Add(new Cookie("s_ev7", "logged%20out") { Domain = domain });
            cookies.Add(new Cookie("s_cc", "true") { Domain = domain });
            cookies.Add(new Cookie("s_ncomui_campaign", "") { Domain = domain });
            cookies.Add(new Cookie("s_sq", "samsungmsc-web-prod%3D%2526pid%253Dcn%25253Apc%25253Agnb%25253Aaccount%25253Amain%2526pidt%253D1%2526oid%253Dfunctionanonymous%252528e%25252F%25252A%25252A%25252F%252529%25257Bvars%25253Ds_c_il%25255B0%25255D%25252Cb%25253Ds.eh%252528this%25252C%252522onclick%252522%252529%25253Bs.lnk%25253Dthis%25253Bs.t%252528%252529%25253Bs.lnk%25253D0%25253Bif%252528b%252529returnt%2526oidt%253D2%2526ot%253DA") { Domain = domain });


            _cookies = _container.GetCookies(new Uri(step3));

            WebOperResult step3Result = SubmitPost(step3, _container, _cookies, pd3);


            string guidReport = step3Result.Text;
            Match match = null;
            string guid = string.Empty;
            //<input type="hidden" name="GUID"            value="sjjrthfxlm"/>
            if (!string.IsNullOrEmpty(guidReport) && (match = Regex.Match(guidReport, "name=\"GUID\"\\s+value=\"([^\"]*)\"")).Success)
            {
                guid = match.Groups[1].Value;
                Context.Status = TaskStatus.Success;
                Context.ExecInfo = "获取用户唯一标识成功";
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "获取用户唯一标识失败";
                return;
            }
            //https://chn.account.samsung.com/account/doSignUp.do  最终验证的数据
            //serviceID=xna99pg346&separatorName=JoinNow&loginIDTypeCode=001&buVSZvinputEmailID=&buVSZvinputPhoneID=8618790793746&buVSZvinputPassword=1Hblsqt%21&buVSZvreInputPassword=1Hblsqt%21&title=&genderTypeCode=M&firstName=%E4%BA%91%E5%B3%B0&lastName=%E7%BF%9F&nickName=&nationality=CHN&yyyy=1963&mm=08&dd=16&birthDate=19630816&securityQuestionID=SS_WHAT_IS_YOUR_FAVOURITE_BOOK_Q_ABB&securityAnswer=%E8%AE%BA%E8%AF%AD&phoneNumber=&inputCaptchaCode=r4c7gt&newsLetter=Y&smsReceiveYNFlag=&postalCode=&checkTerms=Y&checkPdu=Y&checkDca=Y&check3rdParty=Y&hvxgic=roihfa&zjuchv=epbqwe&pbaqkw=ludhbi&xoduev=chhiuu&=%E7%BB%A7%E7%BB%AD



            //POST https://chn.account.samsung.com/account/check.do  验证
            //actionID=JoinNowSuccess&serviceID=xna99pg346&serviceName=%E4%B8%89%E6%98%9F%E4%BC%9A%E5%91%98%E4%BF%B1%E4%B9%90%E9%83%A8&inputEmailID=&countryCode=CN&languageCode=zh&GUID=sjjrthfxlm&registURL=http%3A%2F%2Fmembership.samsung.com%2Fcn%2Flogin%2FSAFunction&returnURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&goBackURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&serviceType=DFLT&ssoType=OPT_VAL

            string step4 = "https://chn.account.samsung.com/account/check.do";
            string pd4 = string.Format(@"actionID=JoinNowSuccess&serviceID={0}&serviceName=%E4%B8%89%E6%98%9F%E4%BC%9A%E5%91%98%E4%BF%B1%E4%B9%90%E9%83%A8&inputEmailID=&countryCode=CN&languageCode=zh&GUID={1}&registURL=http%3A%2F%2Fmembership.samsung.com%2Fcn%2Flogin%2FSAFunction&returnURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&goBackURL=http%3A%2F%2Fmembership.samsung.com%2Fcn&serviceType=DFLT&ssoType=OPT_VAL",
_serviceId,
 guid);
            WebOperResult step4Result = Post(step4, _container, step3Result.Response.Cookies, pd4);
            string resultReport = step4Result.Text;
            if (!string.IsNullOrEmpty(resultReport) && Regex.IsMatch(resultReport, @"恭喜您!<br/>您的帐户已获得授权。请立即了解您可以通过"))
            {
                Context.Status = TaskStatus.Success;

                Context.ExecInfo = "注册成功";
                string path = AppDomain.CurrentDomain.BaseDirectory + "/UserInfo";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string file = Path.Combine(path, "用户信息.txt");

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file, true))
                {
                    //姓名、手机号、身体证、密码、密保、密保内容
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}", Context.LastName + Context.FirstName, Context.Tel, Context.Id, pwd, qa.Item3, qa.Item2));
                }
                this.IsCompleted = true;
            }
            else
            {
                Context.Status = TaskStatus.Failed;
                Context.ExecInfo = "注册失败";
                this.IsCompleted = true;
            }
        }

        private Dictionary<string, string> ParserSecurity(string content)
        {
            string line = string.Empty;
            Dictionary<string, string> questionList = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, "id=\"securityQuestionID[^\"]*\"\\s+name=\"securityQuestionID[^\"]*\"", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (Regex.IsMatch(line, "^\\s*$")) continue;
                            if ((match = Regex.Match(line, "<option value=\"([^\"]*)\"\\s*>([^<]*)</option>", RegexOptions.IgnoreCase)).Success)
                            {
                                string labelEn = match.Groups[1].Value;
                                if (Regex.IsMatch(labelEn, "^\\s*$")) continue;
                                string labelCn = match.Groups[2].Value;

                                questionList.Add(labelEn, labelCn);
                            }
                            else if (Regex.IsMatch(line, "</select>"))
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return questionList;
        }

        public void ReSendSms()
        {
            string step1 = "https://chn.account.samsung.com/account/verifyPhoneIDSendCode.do";
            string pd1 = string.Format("serviceID={0}&cccNationality=CHN&inputPhoneNumber={1}", _serviceId, Context.Tel);
            WebOperResult step1Result = Post(step1, _container, _cookies, pd1);
            _cookies = step1Result.Response.Cookies;

            string smsText = step1Result.Text;
            if (string.IsNullOrEmpty(smsText))
            {
                Context.ExecInfo = "获取手机验证码失败";
                return;
            }
            if (Regex.IsMatch(smsText, "短信已发送"))
            {
                Context.ExecInfo = "请输入手机验证码";
                if (OnGetSmsCompleted != null)
                    OnGetSmsCompleted();
            }
            else if (Regex.IsMatch(smsText, "此电话号码已在使用"))
            {
                Context.ExecInfo = "此电话号码已在使用";
                this.IsCompleted = true;
            }
            else
            {
                Match match = null;
                if ((match = Regex.Match(smsText, "sendResultMsg\":\"([^\"]*)\",")).Success)
                {
                    Context.ExecInfo = match.Groups[1].Value;
                }
                else
                {
                    Context.ExecInfo = "获取手机验证码失败";
                }
            }
        }

        public void ReGetImage()
        {
            DateTime now = DateTime.Now;

            int month = now.Month;
            string step1 = string.Format("/account/find/accountSCaptchaCodeView.do?serviceID={0}&captchaGbn=SIGNUP&{1}%20{2}%20{3}%20{4}%20{5}:{6}:{7}%20GMT+0800%20(中国标准时间)",
                _serviceId,
                DayOfWeek[(int)now.DayOfWeek],
                MonthOfYear[--month],
                now.Day < 10 ? "0" + now.Day : now.Day.ToString(),
                now.Year,
                now.Hour < 10 ? "0" + now.Hour : now.Hour.ToString(),
                now.Minute < 10 ? "0" + now.Minute : now.Minute.ToString(),
                now.Second < 10 ? "0" + now.Second : now.Second.ToString());
            GetImageSource(step1, _container, _cookies);

            //////<img id="inputCaptchaCodeImg" src="/account/find/accountSCaptchaCodeView.do?serviceID=ts3rap101s&amp;captchaGbn=SIGNUP&amp;Fri Jul 04 2014 10:44:34 GMT+0800 (中国标准时间)" width="300" height="57" alt="Security Code">

            //https://chn.account.samsung.com/account/find/accountSCaptchaCodeView.do?serviceID=xna99pg346&captchaGbn=SIGNUP&Wed%20Jul%2002%202014%2017:48:19%20GMT+0800%20(中国标准时间)

        }

        private CookieCollection GetInitilizeCookie()
        {
            CookieCollection cookies = new CookieCollection();
            //            Cookie: _geoip_country=cn; s_cc=true; s_sq=%5B%5BB%5D%5D; _common_country=CN; _common_lang=zh_cn;
            //_common_lang_path=zh; deviceType=pc; deviceId=etc; s_ncomui_ev14=%5B%5B%27cnsportal%27%2C%271404284135966%27%5D%5D; 
            //s_ev7=logged%20out; keyviewbefore=open

            cookies.Add(new Cookie("_geoip_country", "cn") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("s_cc", "true") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("s_sq", "%5B%5BB%5D%5D") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("_common_country", "CN") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("_common_lang", "zh_cn") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("_common_lang_path", "zh") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("deviceType", "pc") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("deviceId", "etc") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("s_ncomui_ev14", "%5B%5B%27cnsportal%27%2C%271404284135966%27%5D%5D") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("s_ev7", "logged%20out") { Domain = "content.samsung.cn" });
            cookies.Add(new Cookie("keyviewbefore", "open") { Domain = "content.samsung.cn" });

            return cookies;
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

        private void GetImageSource(string url, CookieContainer container, CookieCollection cookies)
        {
            RequestContext reContext = RequestContext.DefaultContext();
            reContext.Encoding = encoding;
            reContext.ContentType = "text/html";
            reContext.URL = "https://chn.account.samsung.com" + url;
            reContext.CookieContainer = container;
            reContext.CookieContainer.Add(cookies);

            byte[] imgBytes = HttpWebHelper.GetVerifyImage(reContext);

            if (AutoVerify)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(UuwiseVerify), imgBytes);
            }
            if (imgBytes != null && OnImgCompleted != null)
                OnImgCompleted(imgBytes);
            Context.ExecInfo = "请输入新图片验证码";
        }

        private void UuwiseVerify(object state)
        {
            byte[] bytes = state as byte[];

            string verifyCode = Context.Imp.RecognizeByBytes(bytes);
            if (OnImgVerifyCompleted != null)
                OnImgVerifyCompleted(verifyCode);
            //lock (DayOfWeek)
            //{
            //    _imgVerifyCode = verifyCode;
            //    if (_smsVerifyCompleted == 1)
            //    {
            //        //OnReceiveImgCodeCompleted(_imgVerifyCode);
            //    }
            //}
        }

        private WebOperResult Post(string url, CookieContainer container, CookieCollection cookies, string postData)
        {
            RequestContext postContext = RequestContext.DefaultContext();
            postContext.Encoding = encoding;
            postContext.URL = url;
            postContext.Allowautoredirect = true;
            postContext.Method = "POST";
            postContext.Accept = "application/xhtml+xml, */*";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = postData;
            postContext.CookieContainer = container;
            postContext.CookieContainer.Add(cookies);

            WebOperResult postResult = HttpWebHelper.Post(postContext);


            return postResult;
        }
        private WebOperResult SubmitPost(string url, CookieContainer container, CookieCollection cookies, string postData)
        {
            RequestContext postContext = RequestContext.DefaultContext();
            postContext.Encoding = encoding;
            postContext.URL = url;
            postContext.Expect100Continue = false;
            postContext.Allowautoredirect = true;
            postContext.Method = "POST";
            postContext.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            postContext.ContentType = "application/x-www-form-urlencoded";
            postContext.Postdata = postData;
            postContext.Referer = "https://chn.account.samsung.com/account/signUp.do";
            postContext.Header.Add("Origin", "https://chn.account.samsung.com");
            //Cache-Control: max-age=0
            postContext.Header.Add("Cache-Control", "max-age=0");
            postContext.KeepAlive = true;

            postContext.CookieContainer = container;

            postContext.CookieContainer.Add(cookies);

            WebOperResult postResult = HttpWebHelper.Post(postContext);


            return postResult;
        }
    }


    public enum VerifyFailedCode
    {
        TelVerifyFailed = 0,

        ImgVerifyFailed = 1
    }
    public enum Content
    {
        // 摘要: 
        //     表示星期日。
        Sunday = 0,
        //
        // 摘要: 
        //     表示星期一。
        Monday = 1,
        //
        // 摘要: 
        //     表示星期二。
        Tuesday = 2,
        //
        // 摘要: 
        //     表示星期三。
        Wednesday = 3,
        //
        // 摘要: 
        //     表示星期四。
        Thursday = 4,
        //
        // 摘要: 
        //     表示星期五。
        Friday = 5,
        //
        // 摘要: 
        //     表示星期六。
        Saturday = 6,
    }
}
