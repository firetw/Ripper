using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Net;
using System.Windows.Shapes;
using WebLoginer.Core;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using QqPiao.Model;
using Newtonsoft.Json;
using QqPiao.Task;
namespace QqPiao
{
    public class QqJdTask : BaseTask<City>
    {
        string root = "G:/root";

        CookieCollection cookies = new CookieCollection();
        CookieContainer container = new CookieContainer();
        WebClient client = new WebClient();


        //图片地址:img.17u.com
        //upload.17u.com


        string url = "http://piao.qq.com/jingdian/pw.html?city_type=10";


        public override void Do(QqPiao.Model.City city)
        {
            if (city == null) return;
            _log.Info(string.Format("开始采集:[ID:{0},NAME:{1}]", city.id, city.name));
            //http://piao.qq.com/jingdian/data/type/data_83_0_0_0.json?1563515

            CityJingDian jd = GetCityJingDian();

            if (jd != null)
            {
                foreach (var item in jd.Data)
                {
                    _log.Info(string.Format("采集景点:[sceid:{0},scepid:{1}]", item.sceid, item.scepid));

                    SaveImage(new Uri("http://upload.17u.com/uploadfile/" + item.scepic));

                    ParserJingDian(item.sceid, item.scepid);
                }
            }
            _log.Info(string.Format("采集:[ID:{0},NAME:{1}]结束", city.id, city.name));

        }
        public CityJingDian GetCityJingDian()
        {
            string url = string.Format("http://piao.qq.com/jingdian/data/type/data_{0}_0_0_0.json", Context.id);
            _log.Info(url);
            _log.Info(Context.ToString() + "\r\n\t" + url);
            WebOperResult content = QqPiao.Utils.Get(url, container, cookies);
            string text = content.Text;
            text = text.Replace("JDData.set(\"data_" + Context.id + "_0_0_0\",", "");
            text = text.TrimEnd(')', ';');
            CityJingDian obj = JsonConvert.DeserializeObject<CityJingDian>(text);

            return obj;
        }


        public void ParserJingDian(int sceid, int scepid)
        {
            string chanStr = sceid.ToString();
            if (chanStr.Length < 2) return;
            chanStr = chanStr.Substring(chanStr.Length - 2, 2);
            int channel = Convert.ToInt32(chanStr);


            string url = string.Format("http://piao.qq.com/jingdian/detail/{0}/detail_{1}_{2}.html", channel, sceid, scepid);
            XiangQing xq = new XiangQing();
            xq.Id = sceid;
            xq.Pid = scepid;

            WebOperResult content = Utils.Get(url, container, cookies);
            string text = content.Text;

            Uri webUri = new Uri(url);
            string hostName = GetHost(webUri);

            HtmlDocument document = new HtmlDocument();
            document.OptionUseIdAttribute = true;

            document.LoadHtml(text);


            HtmlNode one = document.GetElementbyId("jd_detail_1");

            HtmlNode two = document.GetElementbyId("jd_detail_2");

            if (one != null)
                xq.XuZhi = one.InnerHtml;
            if (two != null)
                xq.XinXi = two.InnerHtml;
            //<img title="" alt="" src="http://img.17u.com/jqadminpic/uploadpic/old/2013/8/14/2013081416064959993.jpg">
            //piao_mod_wrap_em jdd_item
            //XiangQing
            //var kwBox = document.DocumentNode.SelectSingleNode("//input[@name='kw']");


            #region 去除注释
            //foreach(var script in doc.DocumentNode.Descendants("script").ToArray())
            //    script.Remove();
            //foreach(var style in doc.DocumentNode.Descendants("style").ToArray())
            //    style.Remove();

            //foreach (var comment in doc.DocumentNode.SelectNodes("//comment()").ToArray())
            //    comment.Remove();//新增的代码

            //string innerText = doc.DocumentNode.InnerText;
            #endregion

            HtmlNode jianJieNode = document.DocumentNode.SelectSingleNode("//div[@class='piao_mod_wrap_em jdd_item']");
            if (jianJieNode != null)
                xq.JianJie = jianJieNode.InnerHtml;

            //<div class="jdd_book" style="display:;">
            HtmlNode yuDingNode = document.DocumentNode.SelectSingleNode("//div[@class='jdd_book']");
            if (yuDingNode != null)
                xq.YuDing = yuDingNode.InnerHtml;

            MatchCollection imgs = Regex.Matches(two.InnerHtml, "img[^<]*src=\"([^\"]+)\"");
            foreach (Match img in imgs)
            {
                if (img.Success)
                {
                    string imgUrl = img.Groups[1].Value;
                    if (!imgUrl.StartsWith("http"))
                    {
                        imgUrl = hostName + imgUrl;
                    }
                    Uri imgUri = new Uri(imgUrl);
                    SaveImage(imgUri);
                }
            }

            try
            {
                DbHelper.AddXiangQing(xq);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        private string GetHost(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            string hostName = string.Empty;
            if (uri.IsAbsoluteUri)
            {
                hostName = uri.Scheme + "//" + uri.Host + "/";
            }
            return hostName;
        }

        public string GetAttribute(HtmlNode node, string attName)
        {
            if (node == null) return string.Empty;
            if (node.Attributes == null || node.Attributes.Count < 1) return string.Empty;

            HtmlAttribute att = node.Attributes[attName];

            return att.Value;
        }






        public void SaveImage(Uri imageUri)
        {
            if (imageUri == null) return;
            string localFileName = root + imageUri.LocalPath;//System.IO.Path.Combine(root, imageUri.LocalPath);
            string dir = localFileName.Substring(0, localFileName.LastIndexOf('/'));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            client.DownloadFile(imageUri, localFileName);
        }


        public void SaveImage(string urlImage, string hostName)
        {
            WebClient client = new WebClient();
            string orignPath = urlImage;
            if (!urlImage.StartsWith("http://"))
            {
                orignPath = hostName + urlImage;
            }
            Uri uri = new Uri(orignPath);

            string localFileName = root + uri.LocalPath;// System.IO.Path.Combine(root, uri.LocalPath);
            string dir = localFileName.Substring(0, localFileName.LastIndexOf('/'));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            client.DownloadFile(uri, localFileName);
        }

    }
}
