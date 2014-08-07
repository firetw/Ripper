using QqPiao.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QqPiao
{
    public class DbHelper
    {
        static string connstr;
        static string dir = AppDomain.CurrentDomain.BaseDirectory + "/Db";
        static string file = dir + "/QqJd.db";
        static DbHelper()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = file;
            connstr = builder.ToString();

            CreateDb();
        }

        private static void CreateDb()
        {

            if (File.Exists(file)) return;
            SQLiteConnection.CreateFile(file);


            //      public string JianJie { get; set; }
            //public string XuZhi { get; set; }
            //public string YuDing { get; set; }
            //public string XinXi { get; set; }
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();

                string sql = "CREATE TABLE Xq(jian_jie text,xu_zhi text,yu_ding text,xin_xi text,id text,pid text)";
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                //       public int scecityid { get; set; }
                //public string scecityname { get; set; }
                //public string scegrade { get; set; }
                //public int sceid { get; set; }
                //public string sceinfo { get; set; }
                //public string scename { get; set; }
                //public int scepaymode { get; set; }
                //public string scepic { get; set; }
                //public int scepid { get; set; }
                //public double scepprice { get; set; }
                //public double sceprice { get; set; }
                //public int scetheme { get; set; }
                //public int scethemeid { get; set; }
                //public int scetypeid { get; set; }

                string jdSql = @"CREATE TABLE Jd(scecityid integer,scecityname nvarchar(255),scegrade nvarchar(255),sceid integer,sceinfo text,scename nvarchar(255),
                scepaymode integer,scepic nvarchar(255),scepid integer,scepprice float,sceprice float,scetheme integer,scethemeid integer,scetypeid integer)";
                cmd.CommandText = jdSql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();


                //public string flag { get; set; }
                //public string name { get; set; }
                //public string pinyin { get; set; }
                //public string id { get; set; }

                string citySql = @"CREATE TABLE City(flag nvarchar(255),name nvarchar(255),pinyin nvarchar(255),id nvarchar(255))";
                cmd.CommandText = citySql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddXiangQing(XiangQing xq)
        {
            if (xq == null)
                return;
            string insertSql = string.Format(@"INSERT INTO XQ(JIAN_JIE,XU_ZHI,YU_DING,XIN_XI,ID,PID) VALUES
({0},{1},{2},{3},{4},{5})", GetDbString(xq.JianJie), GetDbString(xq.XuZhi), GetDbString(xq.YuDing), GetDbString(xq.XinXi), GetDbString(xq.Id.ToString()), GetDbString(xq.Pid.ToString()));
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
                cmd.CommandText = insertSql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetDbString(string content)
        {
            if (content == null) return "NULL";
            if (content == string.Empty) return "''";

            return "'" + content.Replace("'", "''") + "'";
        }

        public static void AddCity(City city)
        {
            if (city == null)
                return;
            string insertSql = string.Format(@"INSERT INTO City(flag,name,pinyin,id) VALUES
({0},{1},{2},{3})", GetDbString(city.flag), GetDbString(city.name), GetDbString(city.pinyin), GetDbString(city.id));
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
                cmd.CommandText = insertSql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddJingDian(JingDian jingDian)
        {
            if (jingDian == null)
                return;

            //            string jdSql = @"CREATE TABLE Jd(scecityid integer,scecityname nvarchar(255),scegrade nvarchar(255),sceid integer,sceinfo text,scename nvarchar(255),
            //                scepaymode integer,scepic nvarchar(255),scepid integer,scepprice float,sceprice float,scetheme integer,scethemeid integer,scetypeid integer)";

            string insertSql = string.Format(@"INSERT INTO Jd(scecityid,scecityname,scegrade,sceid,sceinfo,scename,scepaymode,scepic,scepid,scepprice,sceprice,scetheme,scethemeid,scetypeid) VALUES
({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13})", jingDian.scecityid, GetDbString(jingDian.scecityname), GetDbString(jingDian.scegrade), jingDian.sceid,
                      GetDbString(jingDian.sceinfo),
                      GetDbString(jingDian.scename),
                      jingDian.scepaymode,
                      GetDbString(jingDian.scepic),
                      jingDian.scepid,
                      jingDian.scepprice,
                      jingDian.sceprice,
                      jingDian.scetheme,
                      jingDian.scethemeid,
                      jingDian.scetypeid);
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand();
                cmd.CommandText = insertSql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
