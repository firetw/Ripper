using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace WeiBoGrab
{
    public class FileParser
    {
        /// <summary>
        /// 上个字段存的东西
        /// </summary>
        private string _preLine;
        private string _timeLine;
        private string _line;
        private List<AwardItem> _items;


        public FileParser()
        {
            _items = new List<AwardItem>();
        }
        public List<AwardItem> AwardItems
        {
            get { return _items; }
        }

        private DateTime GetTime(string line)
        {

            return DateTime.Parse(GetTimeStr(line));
        }
        private string GetTimeStr(string line)
        {
            if (string.IsNullOrEmpty(line)) return DateTime.MinValue.ToString();
            Match timeMatch = Regex.Match(_line, @"[(\w+)] CLASS");
            if (timeMatch != null && timeMatch.Success)
            {
                return timeMatch.Groups[1].Value;

            }
            return DateTime.Now.ToString();
        }
        public void Parser(DateTime timeMask, string file)
        {
            bool canParser = false;
            using (StreamReader reader = new StreamReader(file))
            {
                while ((_line = reader.ReadLine()) != null)
                {
                    //【和你找工作】业务体验包 领取成功
                    //【飞信书友会】2元图书券 领取成功
                    if (!canParser && GetTime(_line) > timeMask)
                    {
                        canParser = true;
                    }
                    if (!canParser) continue;
                    if (Regex.IsMatch(_line, @"领取成功"))
                    {
                        string award = _line.Substring(0, _line.IndexOf("领取成功"));
                        string tel = string.Empty;
                        Match match = Regex.Match(_preLine, @"Tel:(\d+)\s+恭喜你");
                        if (match.Success && match.Groups.Count > 1)
                        {
                            tel = match.Groups[1].Value;
                        }
                        _items.Add(new AwardItem
                        {
                            Award = award,
                            Tel = tel,
                            RecordTime = GetTimeStr(_timeLine)
                        });
                    }
                    _preLine = _line;

                    _timeLine = _preLine;
                }
            }


        }

       
    }
    public class AwardItem
    {
        public string Award { get; set; }
        public string Tel { get; set; }
        public string RecordTime { get; set; }
    }
}
