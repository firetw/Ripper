using QqPiao.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QqPiao.Model
{
    public class City : Context
    {
        public string flag { get; set; }

        public string name { get; set; }

        public string pinyin { get; set; }
        public string id { get; set; }

        public override string ToString()
        {
            return string.Format("name:[{0}],id:[{0}]", name ?? "", id ?? "");
        }
    }
}
