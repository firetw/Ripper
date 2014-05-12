#region 声明
/****************************************************************************
*                                                                            
* 作者: 王宁                                                                                                          
* 创建时间：
* 描述： 
* 修改时间:
* 修改记录:
*               
*             
* **************************************************************************/
#endregion

#region Namespace
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Ripper
{
    /// <summary>
    /// 请求网页
    /// </summary>
    public class RipperWebRequest
    {

        /// <summary>
        /// 保持提交到同一个Session
        /// </summary>
        private CookieContainer _CookieCtn = new CookieContainer();

        /// <summary>
        /// 
        /// </summary>
        public CookieContainer CookieCtn
        {
            get
            {
                return _CookieCtn;
            }
            set
            {
                _CookieCtn = value;
            }
        }

        /**/
        /// <summary>
        /// 保持连接
        /// </summary>
        private bool isKeepAlive = false;

        public bool IsKeepAlive
        {
            get { return isKeepAlive; }
            set { isKeepAlive = value; }
        }

        /**/
        /// <summary>
        /// 获取指定地址的html
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="PostData"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string GetHTML(string URL, string PostData, System.Text.Encoding encoding)
        {
            isKeepAlive = false;
            string _Html = "";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel," +
                             " application/msword, application/x-shockwave-flash, */*";

            request.CookieContainer = this.CookieCtn;

            //提交的数据
            if (PostData != null && PostData.Length > 0)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                byte[] b = encoding.GetBytes(PostData);
                request.ContentLength = b.Length;
                using (System.IO.Stream sw = request.GetRequestStream())
                {
                    try
                    {
                        sw.Write(b, 0, b.Length);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("Post Data Error!!", ex);
                        Console.WriteLine("Post数据时出错！", ex);
                        return null;
                    }
                    finally
                    {
                        if (sw != null) { sw.Close(); }
                    }
                }
            }
            else
            {
                request.Method = "GET";
            }

            HttpWebResponse response = null;
            System.IO.StreamReader sr = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                //_Cookies = response.Cookies;

                sr = new System.IO.StreamReader(response.GetResponseStream(), encoding);

                _Html = sr.ReadToEnd();

            }
            catch (WebException webex)
            {
                if (webex.Status == WebExceptionStatus.KeepAliveFailure)
                {
                    isKeepAlive = true;
                }
                else
                {
                    Console.WriteLine(string.Format("下载网页({0})出错！", URL), webex);
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                //throw new Exception("DownLoad Data Error", ex);
                Console.WriteLine(string.Format("下载网页({0})出错！", URL), ex);
                return null;

            }
            finally
            {
                if (sr != null) { sr.Close(); }
                if (response != null) { response.Close(); }
                response = null;
                request = null;
            }

            return _Html;

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ckContainer"></param>
        /// <param name="strFileUrl"></param>
        /// <returns></returns>
        public bool DownLoadFile(string strFileUrl, string savePath)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(strFileUrl);

            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel," +
                            " application/msword, application/x-shockwave-flash, */*";

            request.CookieContainer = this.CookieCtn;


            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                //_Cookies = response.Cookies;

                Stream st = response.GetResponseStream();

                long len = response.ContentLength;

                FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);

                //读取的字节总数
                int readCnt = 0;
                //每次读取的字节数
                int perCnt = 0;
                while (readCnt < len)
                {
                    byte[] bt = new byte[1024];
                    perCnt = st.Read(bt, 0, bt.Length);
                    readCnt += perCnt;
                    //根据读取的字节数来写入文件
                    if (perCnt == bt.Length)
                    {
                        fs.Write(bt, 0, bt.Length);
                    }
                    else
                    {
                        fs.Write(bt, 0, perCnt);
                    }
                }
                fs.Close();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("下载文件({0})时出错！", strFileUrl), ex);
                return false;
            }
        }

        /// <summary>
        /// 访问页面，Get
        /// </summary>
        /// <param name="url"></param>
        public string RequestPage(string url)
        {
            return RequestPage(url, null, null, null);
        }

        /// <summary>
        /// 访问页面，Get
        /// </summary>
        /// <param name="url"></param>
        public string RequestPage(string url, Encoding en)
        {
            return RequestPage(url, null, null, null, en);
        }

        /// <summary>
        /// 访问页面，Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string RequestPage(string url, string postData)
        {
            return RequestPage(url, postData, Encoding.GetEncoding("GB2312"));
        }

        /// <summary>
        /// 访问页面，Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string RequestPage(string url, string postData, Encoding en)
        {
            string strHTML;

            try
            {
                do
                {
                    strHTML = GetHTML(url, postData, en);
                }
                while (IsKeepAlive);
            }
            catch (Exception ex)
            {
                Console.WriteLine("访问链接(" + url + ")出现错误！", ex);
                return null;
            }
            return strHTML;
        }

        /// <summary>
        /// 访问页面，Post
        /// </summary>
        /// <returns></returns>
        public string RequestPage(string url, string __VIEWSTATE, Dictionary<string, string> dicParam, string __EVENTVALIDATION)
        {
            return RequestPage(url, __VIEWSTATE, dicParam, __EVENTVALIDATION, Encoding.GetEncoding("GB2312"));
        }

        /// <summary>
        /// 访问页面，Post
        /// </summary>
        /// <returns></returns>
        public string RequestPage(string url, string __VIEWSTATE, Dictionary<string, string> dicParam, string __EVENTVALIDATION, Encoding en)
        {

            /*
            string strPostData = "";

            //判断是否参数符合要求，不符合则为Get，否则为Post
            if (__VIEWSTATE != null && __VIEWSTATE != "" && dicParam != null && dicParam.Count > 0)
            {
                __VIEWSTATE = HttpUtility.UrlEncode(__VIEWSTATE);

                if (__EVENTVALIDATION == null)
                {
                    __EVENTVALIDATION = "";
                }
                else
                {
                    __EVENTVALIDATION = HttpUtility.UrlEncode(__EVENTVALIDATION);
                }

                string param = "";
                foreach (string key in dicParam.Keys)
                {
                    if (param == "")
                    {
                        param = string.Format("{0}={1}", key, dicParam[key]);
                    }
                    else
                    {
                        param = string.Format("{0}&{1}={2}", param, key, dicParam[key]);
                    }
                }

                if (__EVENTVALIDATION != "")
                {
                    strPostData = String.Format("__VIEWSTATE={0}&{1}&__EVENTVALIDATION={2}"
                                                , __VIEWSTATE, param, __EVENTVALIDATION);
                }
                else
                {
                    strPostData = String.Format("__VIEWSTATE={0}&{1}"
                                                , __VIEWSTATE, param);
                }
            }
            return RequestPage(url, strPostData, en);
             * */
            return string.Empty;
        }
    }
}
