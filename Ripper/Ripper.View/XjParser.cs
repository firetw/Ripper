using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Ripper.View.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Ripper.View
{
    public class XjParser
    {
        Encoding gbk = Encoding.UTF8; //Encoding.GetEncoding("gbk");
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public double ParserYuE(string content)
        {
            if (string.IsNullOrEmpty(content)) return 0;

            double result = 0f;
            string line = string.Empty;
            bool breakLoop = false;
            string tmpStr = string.Empty;
            Match match = null;
            using (StreamReader reader = new StreamReader(new MemoryStream(gbk.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    match = Regex.Match(line, @"您当前的账户余额为", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        tmpStr += line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            tmpStr += line;
                            match = Regex.Match(line, @"</strong>", RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                breakLoop = true;
                                break;
                            }

                        }
                    }
                    if (breakLoop)
                        break;
                }
            }
            tmpStr = Regex.Replace(tmpStr, @"\r\n", "");
            tmpStr = Regex.Replace(tmpStr, "@\r", "");
            tmpStr = Regex.Replace(tmpStr, "@\n", "");

            if ((match = Regex.Match(tmpStr, "<b>\\s*(\\S+)\\s*</b>")).Success)
            {
                double.TryParse(match.Groups[1].Value, out result);
                return result;
            }
            return result;
        }

        public List<double> ZhangDan(string content)
        {
            List<double> list = new List<double>();
            list.Add(0);
            list.Add(0);
            if (string.IsNullOrEmpty(content)) return list;

            double result = 0f, result1 = 0f;
            string line = string.Empty;
            bool breakLoop = false;
            string tmpStr = string.Empty, tmpStr2 = string.Empty;
            Match match = null;
            using (StreamReader reader = new StreamReader(new MemoryStream(gbk.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    match = Regex.Match(line, @"6&nbsp;代收费", RegexOptions.IgnoreCase);
                    if (!match.Success) continue;
                    while ((line = reader.ReadLine()) != null)
                    {
                        match = Regex.Match(line, @"color:red;text-align:center;", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            tmpStr += line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                tmpStr += line;
                                match = Regex.Match(line, @"</b>", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    break;
                                }
                            }
                            while ((line = reader.ReadLine()) != null)
                            {
                                match = Regex.Match(line, @"费用合计<元>:", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    tmpStr2 += line;

                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        match = Regex.Match(line, @"</tr>:", RegexOptions.IgnoreCase);
                                        tmpStr2 += line;
                                        if (match.Success)
                                        {
                                            breakLoop = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (breakLoop)
                        break;
                }
            }
            tmpStr = RemoveNewRow(tmpStr);
            tmpStr2 = RemoveNewRow(tmpStr2);
            if ((match = Regex.Match(tmpStr, @"<b>\s*([-\d,\.]+)\s*</b>")).Success)
            {
                double.TryParse(match.Groups[1].Value, out result);
                list[0] = result;
            }
            if ((match = Regex.Match(tmpStr2, @"<b>\s*([-\d,\.]+)\s*</b>")).Success)
            {
                if (list.Count == 0) list.Add(0);
                double.TryParse(match.Groups[1].Value, out result1);
                list[1] = result1;
            }
            return list;
        }

        public string RemoveNewRow(string content)
        {
            if (string.IsNullOrEmpty(content)) return content;
            content = Regex.Replace(content, @"\r\n", "");
            content = Regex.Replace(content, "@\r", "");
            content = Regex.Replace(content, "@\n", "");
            return content;
        }

        public double ParserJiFen(string content)
        {
            if (string.IsNullOrEmpty(content)) return 0;
            Match match = null;
            double result = 0f;

            if ((match = Regex.Match(content, "<font color=\"red\">(\\S+)分</font>")).Success)
            {
                double.TryParse(match.Groups[1].Value, out result);
                return result;
            }
            return 0;
        }

        //  // <td style="text-align: center;">0.03</td>
        public double JiaoFei(string content)
        {
            double result = 0;
            if (string.IsNullOrEmpty(content)) return result;
            //<div id="payHiaPaginationPart">
            int index = content.IndexOf("id=\"payHiaPaginationPart\"");
            if (index == -1) return result;

            int tBeginIndex = content.IndexOf("id=\"table\"", index);
            if (tBeginIndex == -1) return result;
            int tEndIndex = content.IndexOf("</table>", tBeginIndex);
            if (tEndIndex == -1) return result;

            string tmp = content.Substring(tBeginIndex, tEndIndex - tBeginIndex);
            result = JiaoFeiValue(tmp);
            return result;
        }

        private double JiaoFeiValue(string content)
        {
            double result = 0;
            if (string.IsNullOrEmpty(content)) return result;

            content = RemoveNewRow(content);
            content = content.Replace("</td>", "</td>" + System.Environment.NewLine);
            string line = string.Empty;
            Match match = null;
            using (StreamReader reader = new StreamReader(new MemoryStream(gbk.GetBytes(content))))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if ((match = Regex.Match(line, @">\s*([-\d,\.]+)\s*</td>")).Success)
                    {
                        result += Convert.ToDouble(match.Groups[1].Value);
                    }
                }
            }
            return result;
        }

        public double JiaoFeiFanYe(string content)
        {
            double result = 0;

            if (string.IsNullOrEmpty(content)) return result;
            content = content.Trim();
            content = content.Replace("\0", "");
            XElement element = XElement.Parse(content);
            foreach (var item in element.Elements("part"))
            {
                if (item.Attribute("id") != null && item.Attribute("id").Value == "payHiaPaginationPart")
                {
                    string tmp = System.Web.HttpUtility.HtmlDecode(item.Value);
                    result = JiaoFeiValue(tmp);
                }
            }
            return result;
        }

    }
    public class NPOIHelper
    {
        static readonly NPOIHelper instance = new NPOIHelper();

        public static NPOIHelper Instance { get { return instance; } }
        private object lockObject = new object();

        private NPOIHelper()
        {
            InitializeWorkbook();
        }


        HSSFWorkbook hssfworkbook;
        ISheet _currentSheet = null;
        int _currentMask = 0;
        void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "业务查询清单";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "业务查询清单";
            hssfworkbook.SummaryInformation = si;

            _currentSheet = hssfworkbook.CreateSheet("业务查询清单");

            GenerateHeader();

        }


        public void ReLoad()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "业务查询清单";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "业务查询清单";
            hssfworkbook.SummaryInformation = si;

            _currentSheet = hssfworkbook.CreateSheet("业务查询清单");

            GenerateHeader();
        }

        public void GenerateHeader()
        {
            IRow row = _currentSheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("手机号");
            row.CreateCell(1).SetCellValue("查询密码");

            for (int i = 0; i < ExportItem.Columns.Count; i++)
            {
                int col = i + 2;
                row.CreateCell(col).SetCellValue(ExportItem.Columns[i]);
            }
            _currentMask = 1;
        }

        public void WriteRow(ExportItem item)
        {
            lock (lockObject)
            {
                if (item == null) return;
                IRow row = _currentSheet.CreateRow(_currentMask);

                row.CreateCell(0).SetCellValue(item.Tel);
                row.CreateCell(1).SetCellValue(item.Pwd);

                for (int i = 0; i < item.Data.Count; i++)
                {
                    int col = i + 2;
                    row.CreateCell(col).SetCellValue(item.Data[i]);
                }
                row.CreateCell(item.Data.Count + 2).SetCellValue(item.Status);

                System.Threading.Interlocked.Add(ref _currentMask, 1);
            }
        }

        public void WriteToFile()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("查询结果{0}.xls", DateTime.Now.ToString("MMdd")));
            if (File.Exists(fileName))
                File.Delete(fileName);
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                hssfworkbook.Write(file);
            }
        }

    }
}
