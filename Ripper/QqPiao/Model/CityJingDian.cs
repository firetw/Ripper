using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QqPiao.Model
{
    public class CityJingDian
    {
        public int index_begin { get; set; }
        public int total_num { get; set; }

        public JingDian[] Data { get; set; }
    }
}
