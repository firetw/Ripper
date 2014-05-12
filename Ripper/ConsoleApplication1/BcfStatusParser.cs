using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace ConsoleApplication1
{
    class BcfStatusParser
    {
        Encoding gbk = Encoding.GetEncoding("gbk");

        public string Parser(string content)
        {
            string line = null;
            string result = string.Empty;
            Dictionary<string, Ne> btsMap = null;
            Dictionary<string, List<Ne>> trxMap = null;
            MemoryStream ms = new MemoryStream(gbk.GetBytes(content));
            bool isBcfStatusJudge = false, isBtsStatusJudge = false;
            string bcfName = string.Empty;
            string btsName = string.Empty;
            Match match = null;
            using (StreamReader reader = new StreamReader(ms))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (!isBcfStatusJudge && IsMatch(match = Regex.Match(line, @"^BCF-(\d+)\s+ULTRASITE\s+(\S+)\s+(\S+)\s+\d+\s+\S+\s+(\S+)")))
                    {
                        if (match.Groups.Count < 5)
                        {
                            result = "解析失败";
                            break;
                        }
                        bcfName = "BCF-" + match.Groups[1].Value;
                        if (match.Groups[4].Value != "WO")
                        {
                            result = "O&M链路传输不通";
                        }
                        else if (match.Groups[2].Value == "L")
                        {
                            result = "BCF被人为闭锁";
                        }
                        else if (match.Groups[3].Value != "WO")
                        {
                            result = "BCF工作不正常";
                        }
                        if (!string.IsNullOrEmpty(result))
                        {
                            break;
                        }
                        btsMap = new Dictionary<string, Ne>();
                        isBcfStatusJudge = true;
                    }
                    // 14711 55791 BTS-0105  U WO                                                0   2
                    else if (IsMatch(match = Regex.Match(line, @"\d+\s+\d+\s+BTS-(\d+)\s+(\S+)\s+(\S+)")))//判断BTS状态
                    {
                        if (match.Groups.Count < 4) continue;
                        btsName = "BTS-" + match.Groups[1].Value;
                        if (match.Groups[2].Value == "L" || match.Groups[3].Value != "WO")
                        {
                            Ne bts = new Ne
                            {
                                BcfName = bcfName,
                                Name = "BTS-" + match.Groups[1].Value,
                                Status = match.Groups[2].Value == "L" ? NeStatus.Lock : NeStatus.UnWork
                            };
                            btsMap.Add(btsName, bts);
                        }
                    }
                    //EDGE TRX-001  U WO       94  0 1796 MBCCH         P  3
                    else if (IsMatch(match = Regex.Match(line, @"\S+\s+TRX-(\d+)\s+(\S+)\s+(\S+)\s+")))
                    {
                        if (match.Groups.Count < 4) continue;
                        if (match.Groups[2].Value == "L" || match.Groups[3].Value != "WO")
                        {
                            Ne trx = new Ne
                            {
                                BcfName = bcfName,
                                Name = "TRX-" + match.Groups[1].Value,
                                Status = match.Groups[2].Value == "L" ? NeStatus.Lock : NeStatus.UnWork
                            };
                            if (trxMap == null) trxMap = new Dictionary<string, List<Ne>>();
                            if (!trxMap.ContainsKey(btsName))
                            {
                                trxMap.Add(btsName, new List<Ne>());
                            }
                            trxMap[btsName].Add(trx);
                        }
                    }
                }
            }
            if (!isBcfStatusJudge)
            {
                return content;
            }
            return GenerateReport(result, btsMap, trxMap);
        }

        private string GenerateReport(string status, Dictionary<string, Ne> btsMap, Dictionary<string, List<Ne>> trxMap)
        {
            if (!string.IsNullOrEmpty(status))
                return status;
            if (btsMap != null && btsMap.Count > 0)
            {
                return NeReport(btsMap.Values);
            }
            if (trxMap != null && trxMap.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                List<string> lockList = new List<string>();
                List<string> unWorkList = new List<string>();
                string lockNes = string.Empty;
                string unWorkNes = string.Empty;
                foreach (var bts in trxMap)
                {
                    IEnumerable<Ne> nes = bts.Value;
                    var lockQuery = nes.Where(item => item.Status == NeStatus.Lock).Select(item => item.Name);
                    var unWorkQuery = nes.Where(item => item.Status == NeStatus.UnWork).Select(item => item.Name);
                    lockNes = JoinComma(lockQuery);
                    unWorkNes = JoinComma(unWorkQuery);

                    if (!string.IsNullOrEmpty(lockNes))
                    {
                        lockNes = bts.Key + "下载频" + lockNes;
                        lockList.Add(lockNes);
                    }
                    if (!string.IsNullOrEmpty(unWorkNes))
                    {
                        unWorkNes = bts.Key + "下载频" + unWorkNes;
                        unWorkList.Add(unWorkNes);
                    }
                }
                lockNes = JoinComma(lockList);
                unWorkNes = JoinComma(unWorkList);
                if (!string.IsNullOrEmpty(lockNes))
                    builder.AppendLine(lockNes + "被人为闭锁");
                if (!string.IsNullOrEmpty(unWorkNes))
                    builder.AppendLine(unWorkNes + "工作不正常");
                return builder.ToString();
            }
            return "BCF工作正常";
        }

        private string JoinComma(IEnumerable<string> strs)
        {
            if (strs == null) return null;
            string result = string.Empty;
            foreach (var item in strs)
            {
                result += item + ",";
            }
            if (result.Length > 0)
                result = result.TrimEnd(',');
            return result;
        }
        private string NeReport(IEnumerable<Ne> nes)
        {
            if (nes == null) return null;
            StringBuilder builder = new StringBuilder();

            var lockQuery = nes.Where(item => item.Status == NeStatus.Lock).Select(item => item.Name);
            var unWorkQuery = nes.Where(item => item.Status == NeStatus.UnWork).Select(item => item.Name);

            string lockNes = JoinComma(lockQuery);
            if (!string.IsNullOrEmpty(lockNes))
            {
                builder.AppendLine(lockNes + "被人为闭锁");
            }
            //工作不正常   
            string unWorkNes = JoinComma(unWorkQuery);
            if (!string.IsNullOrEmpty(unWorkNes))
            {
                builder.AppendLine(unWorkNes + "工作不正常");
            }
            return builder.ToString();

        }
        private bool IsMatch(Match match)
        {
            if (match == null) return false;
            return match.Success;
        }

        #region 示例报文
        /*
        execmd  ZEEI:BCF=105:;
ZEEI:BCF=105:;


LOADING PROGRAM VERSION 28.17-0




FlexiBSC  ZZBSC155                  2014-03-19  13:59:38
RADIO NETWORK CONFIGURATION IN BSC:
                                                         E P  B
                                      F                  T R  C D-CHANNEL  BUSY
                      AD OP           R  ET- BCCH/CBCH/  R E  S O&M LINK  HR  FR
 LAC   CI         HOP ST STATE  FREQ  T  PCM ERACH       X F  U NAME  ST
                                                                             /GP
===================== == ====== ==== == ==== =========== = = == ===== == === ===

BCF-0105  ULTRASITE    U WO                                   4 BF105 WO
 14711 55791 BTS-0105  U WO                                                0   2
 WANZUO1          BB/- 
                                                                              12
         EDGE TRX-001  U WO       94  0 1796 MBCCH         P  3
         EDGE TRX-002  U WO        6  0 1796                  1
         EDGE TRX-003  U WO       10  0 1796                  5
         EDGE TRX-004  U WO       20  0 1796                  5
 14711 55792 BTS-0106  U WO                                                0   3
 WANZUO2          -/-  
                                                                               4
         EDGE TRX-005  U WO       32  0 1796                  4
         EDGE TRX-006  U WO       87  0 1796 MBCCH            2
 14711 55793 BTS-0107  U WO                                                0   0
 WANZUO3          -/-  
                                                                               4
         EDGE TRX-009  U WO       67  0 1817                  3
         EDGE TRX-010  U WO       82  0 1817 MBCCH         P  1


COMMAND EXECUTED


BASE STATION CONTROLLER HANDLING COMMAND <EE_>
<  
NETELNETD_SUCCESS
NETELNETD>
        */
        #endregion
    }
    enum NeStatus
    {
        Lock,
        UnWork
    }

    class Ne
    {
        public string BcfName;

        public string Name;

        public NeStatus Status;
    }
}
