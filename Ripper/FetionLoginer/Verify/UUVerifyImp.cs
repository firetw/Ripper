using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FetionLoginer.VerifyHelper.UU;
using UU = FetionLoginer.VerifyHelper.UU;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Controls;
using System.Diagnostics;

namespace FetionLoginer.VerifyHelper
{
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

            Image img = null;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder result = new StringBuilder();
            int codeId = Wrapper.uu_recognizeByCodeTypeAndPath(url, codeType, result);
            string resultCode = CheckResult(result.ToString(), NSOFTID, codeId, strCheckKey);
            sw.Stop();
            return resultCode;
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
            string resultCode = CheckResult(result.ToString(), NSOFTID, codeId, strCheckKey);
            sw.Stop();
            return resultCode;
        }

        public string RecognizeByCookie(string url, string cookie)
        {
            //下面是软件id对应的dll校验key。在开发者后台-我的软件里面可以查的到。
            string strCheckKey = DLL_KEY;

            int codeType = 1004;

            Image img = null;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            StringBuilder result = new StringBuilder();

            string cookieResult = string.Empty;
            int codeId = Wrapper.uu_recognizeByCodeTypeAndUrl(url, cookie, codeType, cookieResult, result);
            string resultCode = CheckResult(result.ToString(), NSOFTID, codeId, strCheckKey);
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
