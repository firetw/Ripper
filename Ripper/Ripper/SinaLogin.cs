using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ripper.TaskDispather;
using System.Net;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using Ripper.Model;

namespace Ripper
{
    public class SinaLogin
    {

        public static LoginResult Login(string uid, string pwd)
        {

            LoginResult result = new LoginResult(uid, pwd);
            CookieContainer cc = new CookieContainer();
            try
            {
                int retCode = Login(uid, pwd, cc);
                result.Code = retCode;
                if (retCode == 0)
                    result.CookieContainer = cc;

            }
            catch (Exception ex)
            {
                result.Code = -999;
                result.Exception = ex;
            }
            return result;
        }

        private static int Login(string uid, string psw, CookieContainer cc)
        {
            string uidbase64 = Base64Code(uid);
            string url = "http://login.sina.com.cn/sso/prelogin.php?entry=miniblog&callback=sinaSSOController.preloginCallBack&user="
               + uidbase64 + "&client=ssologin.js(v1.3.16)&rsakt=mod";
            HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webRequest1.CookieContainer = cc;
            HttpWebResponse response1 = (HttpWebResponse)webRequest1.GetResponse();
            StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
            string res = sr1.ReadToEnd();
            int start = res.IndexOf("servertime");
            if (start < 0 || start >= res.Count()) return -1;
            int end = res.IndexOf(',', start);
            if (end < 0 || end >= res.Count()) return -1;
            string servertime = res.Substring(start + 12, end - start - 12);
            start = res.IndexOf("nonce");
            if (start < 0 || start >= res.Count()) return -1;
            end = res.IndexOf(',', start);
            if (end < 0 || end >= res.Count()) return -1;
            string nonce = res.Substring(start + 8, end - start - 9);
            start = res.IndexOf("pubkey");
            if (start == -1) return -1;
            end = res.IndexOf(',', start);
            if (end == -1) return -1;
            string pubkey = res.Substring(start + 9, end - start - 10);
            string password = hex_sha1("" + hex_sha1(hex_sha1(psw)) + servertime + nonce);
            start = res.IndexOf("rsakv");
            if (start == -1) return -1;
            end = res.IndexOf(',', start);
            if (end == -1) return -1;
            string rsakv = res.Substring(start + 8, end - start - 9);
            byte[] bytes;
            string str;
            RSASetPublic(pubkey, "10001");
            password = RSAEncrypt(servertime + "\t" + nonce + "\n" + psw);
            str = "entry=weibo&gateway=1&from=&savestate=7&useticket=1&vsnf=1&su=" + uidbase64 +
               "&service=miniblog&servertime=" + servertime + "&nonce=" + nonce + "&pwencode=rsa2&rsakv=" + rsakv + "&sp=" + password
               + "&encoding=utf-8&prelt=34&url=" +
               HttpUtility.UrlEncode("http://weibo.com/ajaxlogin.php?framelogin=1&callback=parent.sinaSSOController.feedBackUrlCallBack") +
               "&returntype=META";
            ASCIIEncoding encoding = new ASCIIEncoding();
            bytes = encoding.GetBytes(str);
            HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.5)");
            webRequest2.Method = "POST";
            webRequest2.ContentType = "application/x-www-form-urlencoded";
            webRequest2.ContentLength = bytes.Length;
            webRequest2.CookieContainer = cc;
            Stream stream;
            stream = webRequest2.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            HttpWebResponse response2 = (HttpWebResponse)webRequest2.GetResponse();
            StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.Default);
            string res2 = sr2.ReadToEnd();
            int pos = res2.IndexOf("retcode");
            if (pos < 0 || pos > res2.Count()) return -1;
            int retcode = -1;
            for (pos += 8; pos < 100 + res2.Count(); pos++)
            {
                if (res2[pos] < '0' || res2[pos] > '9')
                {
                    retcode = 0;
                    break;
                }
                else if (res2[pos] > '0' && res2[pos] <= '9')
                    break;
            }
            if (retcode == -1) return -1;
            start = res2.IndexOf("location.replace");
            end = res2.IndexOf("\")", start);
            url = res2.Substring(start + 18, end - start - 18);
            HttpWebRequest webRequest3 = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webRequest3.CookieContainer = cc;
            HttpWebResponse response3 = (HttpWebResponse)webRequest3.GetResponse();
            StreamReader sr3 = new StreamReader(response3.GetResponseStream(), Encoding.UTF8);
            res = sr3.ReadToEnd();
            foreach (Cookie cookie in response3.Cookies)
            {
                cc.Add(cookie);
            }
            return 0;
        }
        private static Random rand = new Random();
        private static BigInteger n = null;
        private static BigInteger e = null;
        private static BigInteger pkcs1pad2(string a, int b)
        {
            if (b < a.Length + 11)
            {
                throw new Exception("Message too long for RSA");
            }
            byte[] c = new byte[b];
            int d = a.Length - 1;
            while (d >= 0 && b > 0)
            {
                int e = (int)a[d--];
                if (e < 128)
                {
                    c[--b] = Convert.ToByte(e);
                }
                else if ((e > 127) && (e < 2048))
                {
                    c[--b] = Convert.ToByte(((e & 63) | 128));
                    c[--b] = Convert.ToByte((e >> 6) | 192);
                }
                else
                {
                    c[--b] = Convert.ToByte((e & 63) | 128);
                    c[--b] = Convert.ToByte(((e >> 6) & 63) | 128);
                    c[--b] = Convert.ToByte(((e >> 12) | 224));
                }
            }
            c[--b] = Convert.ToByte(0);
            byte[] temp = new byte[1];
            while (b > 2)
            {
                temp[0] = Convert.ToByte(0);
                while (temp[0] == 0)
                    rand.NextBytes(temp);
                c[--b] = temp[0];
            }
            c[--b] = 2;
            c[--b] = 0;
            return new BigInteger(c);
        }
        public static void RSASetPublic(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                throw new Exception("Message too long for RSA");
            }
            n = new BigInteger(a, 16);
            e = new BigInteger(b, 16);
        }
        private static BigInteger RSADoPublic(BigInteger x)
        {
            return x.modPow(e, n);
        }
        public static string RSAEncrypt(string a)
        {
            BigInteger tmp = pkcs1pad2(a, (n.bitCount() + 7) >> 3);
            tmp = RSADoPublic(tmp);
            string result = tmp.ToHexString();
            return 0 == (result.Length & 1) ? result : "0" + result;
        }
        // sha-1加密
        public static string hex_sha1(string Source_String)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
        //base64加密
        public static string Base64Code(string Message)
        {
            char[] Base64Code = new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T', 
         'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n', 
         'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7', 
         '8','9','+','/','='};
            byte empty = (byte)0;
            System.Collections.ArrayList byteMessage = new System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes(Message));
            System.Text.StringBuilder outmessage;
            int messageLen = byteMessage.Count;
            int page = messageLen / 3;
            int use = 0;
            if ((use = messageLen % 3) > 0)
            {
                for (int i = 0; i < 3 - use; i++)
                    byteMessage.Add(empty);
                page++;
            }
            outmessage = new System.Text.StringBuilder(page * 4);
            for (int i = 0; i < page; i++)
            {
                byte[] instr = new byte[3];
                instr[0] = (byte)byteMessage[i * 3];
                instr[1] = (byte)byteMessage[i * 3 + 1];
                instr[2] = (byte)byteMessage[i * 3 + 2];
                int[] outstr = new int[4];
                outstr[0] = instr[0] >> 2;
                outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                if (!instr[1].Equals(empty))
                    outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                else
                    outstr[2] = 64;
                if (!instr[2].Equals(empty))
                    outstr[3] = (instr[2] & 0x3f);
                else
                    outstr[3] = 64;
                outmessage.Append(Base64Code[outstr[0]]);
                outmessage.Append(Base64Code[outstr[1]]);
                outmessage.Append(Base64Code[outstr[2]]);
                outmessage.Append(Base64Code[outstr[3]]);
            }
            return outmessage.ToString();
        }
    }
}
