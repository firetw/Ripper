using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.View.Model
{
    public class ExportItem
    {
        public static List<string> Columns { get; private set; }

        static ExportItem()
        {
            Columns = new List<string>();

            Columns.Add("余额");

            //Columns.Add("当月话费");

            Columns.Add("代收费");
            Columns.Add("费用合计");

            Columns.Add("上月代收费");
            Columns.Add("上月费用合计");

            Columns.Add("缴费历史");

            Columns.Add("总积分");

            Columns.Add("是否正常");
        }

        public ExportItem()
        {
            Data = new List<double>();
            for (int i = 0; i < Columns.Count - 1; i++)
            {
                Data.Add(0);
            }
        }

        public string Status { get; set; }
        public string Tel { get; set; }

        public string Pwd { get; set; }

        public List<double> Data { get; set; }
    }


}
