using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FetionLoginer.VerifyHelper;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace WeiBoGrab
{
    public partial class FLogin : Form
    {
        UUVerifyImp imp = null;
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        Dictionary<string, string> dict = new Dictionary<string, string>();

        Queue<Item> queue = new Queue<Item>();

        System.Threading.Timer timer;
       
        OneObjectParaDelegate handler;
        int _totalCount = 0;
        WaitCallback _doJobAsyncHandler;
        OneObjectParaDelegate _doJobHandler;

        int index = 1;
        int _awardNum = 0;

        string uuUserName, uuPwd;

        public FLogin()
        {
            InitializeComponent();
            handler = new OneObjectParaDelegate(TimeOutHandler);
            //this.Load+= // += new EventHandler(FLogin_Click);
            this.Load += new EventHandler(FLogin_Load);
            _doJobAsyncHandler = new WaitCallback(DoJobAsync);
            _doJobHandler = new OneObjectParaDelegate(DoJob);

            uuUserName = System.Configuration.ConfigurationManager.AppSettings["user"];
            uuPwd = System.Configuration.ConfigurationManager.AppSettings["pwd"];
        }

        void FLogin_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
            {
                this.Invoke(new InvokeDelegate(StartUp));

            }), null);
        }
        private void StartUp()
        {
            webBrowser1.Navigate(new Uri(ConfigCore.FetionUrl));
            ParserFile();
            Start();

        }
        void Start()
        {
            try
            {
                imp = new UUVerifyImp();
                if (imp.Login(uuUserName, uuPwd))
                {
                    log.Info(string.Format("uuUserName:{0},uuPwd:{1}", uuUserName, uuPwd));
                    log.Info("验证码工具初始化成功");
                }
                else
                {
                    MessageBox.Show("验证码工具登录失败", "错误");
                    log.Error("验证码工具初始化失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("验证码工具登录失败", "错误");
                log.Error(ex);
                return;
            }

            this.execLabel.Text = string.Format(" {0}/{1}  ", 0, dict.Count);
            ThreadPool.QueueUserWorkItem(_doJobAsyncHandler, null);

        }
        private void DoJobAsync(object state)
        {
            ExecContext context = null;
            if (state == null)
            {
                if (queue.Count > 0)
                {
                    Item item = queue.Dequeue();
                    context = new ExecContext
                  {
                      ExecThread = Thread.CurrentThread,
                      ExecItem = item
                  };
                }
            }
            else
            {
                index--;//回退指针
                context = state as ExecContext;
            }
            if (context != null)
            {
                timer = new System.Threading.Timer(new TimerCallback(TimeOutHandler), context, 2 * 60 * 1000, System.Threading.Timeout.Infinite);
                this.Invoke(_doJobHandler, context.ExecItem);
            }
        }

        private void DoJob(object state)
        {
            try
            {
                Item item = state as Item;
                if (item == null) return;

                this.execLabel.Text = string.Format(" {0}/{1}  ", index++, _totalCount);
                FLoginSubmit submit = new FLoginSubmit(item.Tel, item.Pwd);
                submit.Tel = item.Tel;
                _awardNum += submit.Do(webBrowser1, imp);
                this.awardNum.Text = " " + awardNum + " ";
                if (index % 500 == 0)
                {
                    this.uuScore.Text = "  " + imp.GetScore(uuUserName, uuPwd).ToString() + " 分  ";
                }
                if (index == 1)
                {
                    this.uuScore.Text = "  " + imp.GetScore(uuUserName, uuUserName).ToString() + " 分  ";
                    log.Info(this.uuScore.Text);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                timer.Dispose();
                ThreadPool.QueueUserWorkItem(_doJobAsyncHandler, null);
            }
        }
        private void TimeOutHandler(object obj)
        {
            if (this.InvokeRequired)
                this.Invoke(new OneObjectParaDelegate(ReLoadWebBrowser), obj);
        }
        private void ReLoadWebBrowser(object state)
        {
            log.Warn("执行超时，任务将重启。");
            ExecContext context = state as ExecContext;
            if (context == null) return;
            try
            {
                context.ExecThread.Abort();
            }
            catch (ThreadAbortException)
            {
            }
            this.Controls.Remove(this.webBrowser1);
            webBrowser1 = new WebBrowser() { Dock = DockStyle.Fill };
            this.Controls.Add(webBrowser1);
            webBrowser1.Navigate(ConfigCore.FetionUrl);
            ThreadPool.QueueUserWorkItem(_doJobAsyncHandler, context);
        }
        private void ParserFile()
        {
            dict.Clear();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/号码.txt");
            if (!File.Exists(filePath))
            {
                MessageBox.Show(filePath + "\r\n 不存在", "错误");
            }
            Encoding gbk = Encoding.GetEncoding("gbk");


            string line = string.Empty;
            using (StreamReader reader = new StreamReader(filePath, gbk))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#")) continue;
                    Match match = Regex.Match(line, @"(\d+)\s+(\S+)", RegexOptions.IgnoreCase);

                    if (!match.Success || match.Groups.Count < 3) continue;

                    string tel = match.Groups[1].Value;
                    string pwd = match.Groups[2].Value;

                    queue.Enqueue(new Item { Tel = tel, Pwd = pwd });
                    //dict.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }
            _totalCount = queue.Count;
        }



        private void statusStrip1_Click(object sender, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
            //{
            //    this.Invoke(new InvokeDelegate(StartUp));

            //}), null);
        }
    }


    public class Item
    {
        public string Tel { get; set; }
        public string Pwd { get; set; }
    }

    public class ExecContext
    {
        public Thread ExecThread { get; set; }
        public Item ExecItem { get; set; }
    }
}
