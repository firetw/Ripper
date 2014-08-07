using QqPiao.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QqPiao.Model
{
    public class JingDian : Context
    {
        //        index_begin":0,
        //"total_num":34,
        //data:[{"scecityid":10,
        //"scecityname":"北京",
        //"scegrade":"AAAAA",
        //"sceid":3150,
        //"sceinfo":"气宇轩宏明十三，帝王之气显示无遗\r\n",
        //"scename":"十三陵",
        //"scepaymode":0,
        //"scepic":"scenerypic_tencent/2013/03/06/2/2013030616401979386.jpg",
        //"scepid":101,
        //"scepprice":31,
        //"sceprice":35,
        //"scetheme":"0",
        //"scethemeid":0,
        //"scetypeid":0

        /*
        Project p = new Project() { Input = "stone", Output = "gold" };
JsonSerializer serializer = new JsonSerializer();
StringWriter sw = new StringWriter();
serializer.Serialize(new JsonTextWriter(sw), p);
Console.WriteLine(sw.GetStringBuilder().ToString());

StringReader sr = new StringReader(@"{""Input"":""stone"", ""Output"":""gold""}");
Project p1 = (Project)serializer.Deserialize(new JsonTextReader(sr), typeof(Project));
Console.WriteLine(p1.Input + "=>" + p1.Output);*/
        public int scecityid { get; set; }
        public string scecityname { get; set; }
        public string scegrade { get; set; }
        public int sceid { get; set; }
        public string sceinfo { get; set; }
        public string scename { get; set; }
        public int scepaymode { get; set; }
        public string scepic { get; set; }
        public int scepid { get; set; }
        public double scepprice { get; set; }
        public double sceprice { get; set; }
        public int scetheme { get; set; }
        public int scethemeid { get; set; }
        public int scetypeid { get; set; }
    }
}
