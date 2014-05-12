using System;
using System.Collections.Generic;
using System.Text;
using FetionLoginer.VerifyHelper.UU;
using UU = FetionLoginer.VerifyHelper.UU;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace FetionLoginer.VerifyHelper
{

    public static class Core
    {

        static Dictionary<string, string> _errorMap = new Dictionary<string, string>();

        public const int VerifyLength = 4;
        public const string FetionUrl = "http://gz.feixin.10086.cn/Bootlick/index";
        /// <summary>
        /// 未识别的标识
        /// </summary>
        public static string UnRecoginize
        {
            get { return "结果校验不正确"; }
        }

        public static string GetErrorReason(string code)
        {
            if (_errorMap.ContainsKey(code)) return _errorMap[code];
            return "";
        }
        static Core()
        {
            Encoding gbk = Encoding.GetEncoding("gbk");
            string errorCode = @"-1001	网络连接失败
-1002	网络传输超时
-1003	文件读取失败
-1004	图像内存流无效
-1005	服务器返回内容错误
-1006	服务器状态错误
-1007	内存分配失败
-1008	没有取到验证码结果，返回此值指示codeID已返回
-1009	此时不允许进行该操作（用户没有登录会出现这个错误）
-1010	图片过大，限制1MB
-1011	图片转换为JPG失败，源图片无效
-1012	获取服务器配置信息失败，是由于网络连接失败造成的
-1013	传入的字符串缓冲区不足
-1014	URL下载失败！
-1015	连续重复的图片次数达到设定值
-1016	找不到窗口标题
-1017	找不到窗口句柄
-1020	切换账户登录超过20次
-1021	1分钟之内查分超过10次
-1022	查分时未登录，只有登录之后才能查分
-1023	文件上传成功，服务器返回的验证码id为0
-1024	上传时codeType为0
-1101	无效的异步操作句柄（异步调用函数专有）
-1102	异步操作尚未完成（异步调用函数专有）
-16009	1分钟之内查分超过10次
-16002	查分时未登录，只有登录之后才能查分
-11006	账户登录过于频繁
-14009	密码过于简单
-17009	报错过于频繁
-19001	UserAgent内容错误
-19002	API版本号错误
-19003	TTL错误
-19004	API文件MD5校验失败
-19005	API文件版本不存在
-19006	API文件版本被篡改
-19007	API文件版本已过期
-19008	此API版本已被禁用
-19011	软件SID参数无效
-19012	软件ID不存在
-19013	软件未启用
-19014	软件被禁用
-19015	软件状态不正常
-19020	服务器无法使用TEAKEY解密";

            string line = string.Empty;
            using (StreamReader reader = new StreamReader(new MemoryStream(gbk.GetBytes(errorCode))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = Regex.Match(line, @"(\d+)\s+(.+)", RegexOptions.IgnoreCase);
                    if (!match.Success || match.Groups.Count < 3) continue;


                    _errorMap.Add(match.Groups[1].Value, match.Groups[2].Value);


                }
            }
        }


    }

    public class UUVerifyImp : IVerify
    {
        const string LP_SOFT_KEY = "7739ee02e1564c788a75fd51f0c4c461";
        const int NSOFTID = 97541;
        const string DLL_KEY = "AD39F4D7-0D97-4774-99D0-A975278EB77D";



        /// <summary>
        /// 未识别的标识
        /// </summary>
        public string UnRecoginize
        {
            get { return "结果校验不正确"; }
        }

        public UUVerifyImp()
        {
            //软件信息设置
            FetionLoginer.VerifyHelper.UU.Wrapper.uu_setSoftInfo(NSOFTID, LP_SOFT_KEY);
        }

        public string RecognizeByUrl(string url)
        {
            //下面是软件id对应的dll校验key。在开发者后台-我的软件里面可以查的到。
            string strCheckKey = DLL_KEY;

            int codeType = 1004;


            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder result = new StringBuilder();
            int codeId = Wrapper.uu_recognizeByCodeTypeAndPath(url, codeType, result);
            string context = result.ToString();
            if (!string.IsNullOrEmpty(Core.GetErrorReason(context)))
            {
                return Core.UnRecoginize;
            }
            string resultCode = CheckResult(context, NSOFTID, codeId, strCheckKey);
            sw.Stop();
            return resultCode;
        }

        public int GetScore(string userName, string pwd)
        {
            return Wrapper.uu_getScore(userName, pwd);
        }
        public string RecognizeByBytes(byte[] picContent)
        {
            if (picContent == null) throw new ArgumentNullException("picContent");
            if (picContent.Length < 1) throw new ArgumentOutOfRangeException("picContent", "图片字节必须大于0");
            string strCheckKey = DLL_KEY;
            int codeType = 1004;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder result = new StringBuilder();
            int codeId = Wrapper.uu_recognizeByCodeTypeAndBytes(picContent, picContent.Length, codeType, result);
            string context = result.ToString();
            if (!string.IsNullOrEmpty(Core.GetErrorReason(context)))
            {
                return Core.UnRecoginize;
            }
            string resultCode = CheckResult(context, NSOFTID, codeId, strCheckKey);
            sw.Stop();
            return resultCode;
        }

        public string RecognizeByCookie(string url, string cookie)
        {
            //下面是软件id对应的dll校验key。在开发者后台-我的软件里面可以查的到。
            string strCheckKey = DLL_KEY;

            int codeType = 1004;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder result = new StringBuilder();

            string cookieResult = string.Empty;
            int codeId = Wrapper.uu_recognizeByCodeTypeAndUrl(url, cookie, codeType, cookieResult, result);
            string context = result.ToString();
            if (!string.IsNullOrEmpty(Core.GetErrorReason(context)))
            {
                return Core.UnRecoginize;
            }
            string resultCode = CheckResult(context, NSOFTID, codeId, strCheckKey);
            sw.Stop();
            return resultCode;
        }

        public string CheckResult(string result, int softId, int codeId, string checkKey)
        {
            //对验证码结果进行校验，防止dll被替换
            if (string.IsNullOrEmpty(result))
                return result;
            else
            {
                string[] modelReult = result.Split('_');
                //解析出服务器返回的校验结果
                string strServerKey = modelReult[0];
                string strCodeResult = modelReult[1];
                //本地计算校验结果
                string localInfo = softId.ToString() + checkKey + codeId.ToString() + strCodeResult.ToUpper();
                string strLocalKey = MD5Encoding(localInfo).ToUpper();
                //相等则校验通过
                if (strServerKey.Equals(strLocalKey))
                    return strCodeResult;
                return UnRecoginize;
            }
        }

        public bool Verify()
        {
            int softId = NSOFTID;
            string softKey = LP_SOFT_KEY;
            Guid guid = Guid.NewGuid();
            string strGuid = guid.ToString().Replace("-", "").Substring(0, 32).ToUpper();
            string DLLPath = System.Environment.CurrentDirectory + "\\UUWiseHelper.dll";
            string strDllMd5 = GetFileMD5(DLLPath);
            CRC32 objCrc32 = new CRC32();
            string strDllCrc = String.Format("{0:X}", objCrc32.FileCRC(DLLPath));
            //CRC不足8位，则前面补0，补足8位
            int crcLen = strDllCrc.Length;
            if (crcLen < 8)
            {
                int miss = 8 - crcLen;
                for (int i = 0; i < miss; ++i)
                {
                    strDllCrc = "0" + strDllCrc;
                }
            }
            //下面是软件id对应的dll校验key。在开发者后台-我的软件里面可以查的到。
            string strCheckKey = "AD39F4D7-0D97-4774-99D0-A975278EB77D".ToUpper();
            string yuanshiInfo = NSOFTID + strCheckKey + strGuid + strDllMd5.ToUpper() + strDllCrc.ToUpper();

            string localInfo = MD5Encoding(yuanshiInfo);
            StringBuilder checkResult = new StringBuilder();
            Wrapper.uu_CheckApiSign(softId, softKey, strGuid, strDllMd5, strDllCrc, checkResult);
            string strCheckResult = checkResult.ToString();

            return localInfo.Equals(strCheckResult);
        }


        /// <summary>
        /// 登录UU平台
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        public bool Login(string userName, string userPwd)
        {
            /*	优优云DLL 文件MD5值校验
           *  用处：近期有不法份子采用替换优优云官方dll文件的方式，极大的破坏了开发者的利益
           *  用户使用替换过的DLL打码，导致开发者分成变成别人的，利益受损，
           *  所以建议所有开发者在软件里面增加校验官方MD5值的函数
           *  如何获取文件的MD5值，通过下面的GetFileMD5(文件)函数即返回文件MD5
           */

            string DLLPath = System.Environment.CurrentDirectory + "\\UUWiseHelper.dll";
            string Md5 = GetFileMD5(DLLPath);
            string AuthMD5 = "7c0415db33190179697196004e57d7c4";//作者在编写软件时内置的比对用DLLMD5值，不一致时将禁止登录,具体需要各位自己先获取使用的DLL的MD5验证字符串
            if (Md5 != AuthMD5)
            {
                return false;
            }
            int res = Wrapper.uu_login(userName, userPwd);
            return res > 0;
        }









        #region 文件MD5
        /// <summary>
        /// 获取文件MD5校验值
        /// </summary>
        /// <param name="filePath">校验文件路径</param>
        /// <returns>MD5校验字符串</returns>
        private string GetFileMD5(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5byte = md5.ComputeHash(fs);
            int i, j;
            StringBuilder sb = new StringBuilder(16);
            foreach (byte b in md5byte)
            {
                i = Convert.ToInt32(b);
                j = i >> 4;
                sb.Append(Convert.ToString(j, 16));
                j = ((i << 4) & 0x00ff) >> 4;
                sb.Append(Convert.ToString(j, 16));
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD5 加密字符串
        /// </summary>
        /// <param name="rawPass">源字符串</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encoding(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}
