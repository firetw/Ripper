using Microsoft.Win32;
using Ripper.View.Henan;
using Ripper.View.Model;
using Ripper.View.RipperDuplex;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebLoginer.Core;

namespace Ripper.View
{
    public delegate void LeDouSetDelegate(Entity state, string leDou, WebOperResult wr);
    public delegate void SetResultDelegate(Entity state, string status, string cmd);
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, RipperDuplex.BeerInventoryServiceCallback
    {
        Encoding _encode = null;
        Options _options = null;
        RipperService _service = null;
        ObservableCollection<Entity> EntityItems = new ObservableCollection<Entity>();
        List<Entity> _AllRep = new List<Entity>();
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        LoginHelper _helper = new LoginHelper();
        ServiceHost _host = null;
        LeDouSetDelegate _leDouHandler = null;
        BeerInventoryServiceClient _beerInventoryService = null;
        Timer _loginTimer = null;
        Timer _duiHuanTimer = null;
        Timer _heartBreakTimer = null;
        string _clientId = string.Empty;
        int _execCount = 0;
        int _currentIndex = 0;
        private object lockObject = new object();

        public MainWindow()
        {
            InitializeComponent();
            Initilize();
        }



        public void Initilize()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            this.tbLogin.Text = "23:55:00";
            this.tbDuiHuan.Text = "23:59:54";

            DateTime now = DateTime.Now;
            this.tbBegin.Text = now.AddMonths(-1).ToString("yyyy-MM-01");
            this.tbEnd.Text = now.ToString("yyyy-MM-dd");

            _leDouHandler = new LeDouSetDelegate(SetLeDouValue);
            this.Closing += MainWindow_Closing;
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;
            _encode = Encoding.GetEncoding(System.Configuration.ConfigurationManager.AppSettings["encoding"]);
            this.maiListView.ItemsSource = EntityItems;

            cbTasks.Items.Clear();
            //cbTasks.Items.Add(new ComboBoxItem { Content = "请选择↓", Tag = 0, IsSelected = true });

            foreach (var item in Config.GiftCodeMap)
            {
                cbTasks.Items.Add(new ComboBoxItem { Content = string.Format("兑换{0}元", item.Key), Tag = item.Value });
            }
            (cbTasks.Items[0] as ComboBoxItem).IsSelected = true;
        }

        private void Clean()
        {
            //关闭所有客户端
            if (_service != null)
                _service.SendCmd("-1");
            if (_heartBreakTimer != null)
                _heartBreakTimer.Dispose();
            if (_host != null)
                _host.Close();

            if (_heartBreakTimer != null)
                _heartBreakTimer.Dispose();
            if (_beerInventoryService != null)
                _beerInventoryService.Close();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Clean();
                if (_loginTimer != null)
                    _loginTimer.Dispose();
                if (_duiHuanTimer != null)
                    _duiHuanTimer.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void Initilize(Options option)
        {
            _options = option;

            Initilize();
            string inputFile = option.InputFile;
            if (!string.IsNullOrEmpty(inputFile))
            {
                ReadFile(inputFile);

                if (this.EntityItems.Count > 0)
                {
                    this.btLogin.IsEnabled = true;
                    this.btExport.IsEnabled = true;
                }
            }

            if (option.Master != 1)
            {
                this.btNewClient.Visibility = Visibility.Collapsed;
                this.cbMultiClient.Visibility = Visibility.Collapsed;

                this.Title = "话费助手(辅助)";

                this.opContainer.RowDefinitions[6].Height = new GridLength(0);
                this.opContainer.RowDefinitions[7].Height = new GridLength(0);

                try
                {
                    this.btLogin.Visibility = Visibility.Collapsed;
                    this.btJiFen.Visibility = Visibility.Collapsed;
                    this.btDuiHuan.Visibility = Visibility.Collapsed;


                    _clientId = Guid.NewGuid().ToString();
                    _beerInventoryService = new RipperDuplex.BeerInventoryServiceClient(new InstanceContext(this), "TcpBinding");
                    _beerInventoryService.Open();
                    _beerInventoryService.Register(_clientId);

                    _heartBreakTimer = new Timer(new TimerCallback((obj) =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                _beerInventoryService.HeartBreak(_clientId);
                                this.tbInfo.Text = "主程序连接正常";
                            }
                            catch (Exception ex)
                            {
                                this.tbInfo.Text = "连接不上主程序";
                                _log.Error(ex);
                            }
                        }));
                    }), null, 5000, 300000);
                    _log.Info("Start" + _clientId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                this.Title = "话费助手(主)";
            }
        }

        private void ShowHeartBreak(object state)
        {

        }
        private void btExcelExport_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Empty;
            try
            {
                NPOIHelper.Instance.WriteToFile();
                msg = "导出完成";
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                msg = "导出现异常";
            }
            Dispatcher.BeginInvoke(new Action<string>((o) =>
            {
                this.tbInfo.Text = o;
            }), msg);
        }
        private void btQuery_Click(object sender, RoutedEventArgs e)
        {
            string startTime = this.tbBegin.Text;
            string endTime = this.tbEnd.Text;
            if (!Regex.IsMatch(startTime, @"\d{4}-\d{1,2}-\d{1,2}") || !Regex.IsMatch(endTime, @"\d{4}-\d{1,2}-\d{1,2}"))
            {
                this.tbInfo.Text = "数据格式必须为 2014-04-01";
                return;
            }
            //for (int i = 0; i < EntityItems.Count; i++)
            //{
            //    Entity item = EntityItems[i];
            //    item.TaskStatus = "业务详情查询";
            //    ThreadPool.QueueUserWorkItem(new WaitCallback(entity =>
            //    {
            //        try
            //        {
            //            XjWangTingQuery query = new XjWangTingQuery();
            //            query.OnTaskCompleted += query_OnTaskCompleted;
            //            item.StartTime = startTime;
            //            item.EndTime = endTime;

            //            query.Context = item;
            //            query.Process(item);
            //        }
            //        catch (Exception ex)
            //        {
            //            _log.Error(ex);
            //        }
            //    }), item);
            //}
            int threadCount = 5 > _AllRep.Count ? 1 : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ThreadCount"]);
            for (int i = 0; i < threadCount; i++)
            {
                Thread workThread = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        Entity item = null;
                        lock (lockObject)
                        {
                            if (_currentIndex >= _AllRep.Count) return;
                            item = _AllRep[_currentIndex];
                            Interlocked.Add(ref _currentIndex, 1);
                        }
                        try
                        {
                            if (item != null)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    item.TaskStatus = "业务详情查询";
                                }));
                                XjWangTingQuery query = new XjWangTingQuery();
                                query.OnTaskCompleted += query_OnTaskCompleted;
                                item.StartTime = startTime;
                                item.EndTime = endTime;

                                query.Context = item;
                                query.Process(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);
                        }
                        //finally
                        //{
                        //    lock (lockObject)
                        //    {

                        //    }
                        //}

                    }

                }));
                workThread.Start();
            }
        }

        void query_OnTaskCompleted(bool obj)
        {
            lock (lockObject)
            {
                Interlocked.Add(ref _execCount, 1);

                if (_execCount == _AllRep.Count)
                {
                    string msg = string.Empty;
                    try
                    {
                        NPOIHelper.Instance.WriteToFile();
                        msg = "导出完成";
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        msg = "导出现异常";
                    }
                    Dispatcher.BeginInvoke(new Action<string>((o) =>
                    {
                        this.tbInfo.Text = o;
                    }), msg);
                }
            }
        }

        private void btImpport_Click(object sender, RoutedEventArgs e)
        {
            string line = string.Empty;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            bool? result = openFileDialog1.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                //index = 1;
                EntityItems.Clear();
                _AllRep.Clear();
                NPOIHelper.Instance.ReLoad();
                _execCount = 0;

                string file = openFileDialog1.FileName;
                ReadFile(file);

                #region  Delete
                //if ((myStream = openFileDialog1.OpenFile()) != null)
                //{
                //    using (StreamReader reader = new StreamReader(myStream, _encode))
                //    {
                //        while ((line = reader.ReadLine()) != null)
                //        {
                //            try
                //            {
                //                if (string.IsNullOrWhiteSpace(line)) continue;
                //                if (string.IsNullOrEmpty(line)) continue;

                //                Match match = Regex.Match(line, @"(\d+)\s+(\d+)");
                //                if (match.Success)
                //                {
                //                    Entity entity = new Entity(match.Groups[1].Value, match.Groups[2].Value);
                //                    entity.Seq = index;
                //                    EntityItems.Add(entity);
                //                    index++;
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                Console.WriteLine(ex);
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            if (this.EntityItems.Count > 0)
            {
                this.btLogin.IsEnabled = true;
                this.btExport.IsEnabled = true;
                this.btQuery.IsEnabled = true;
                this.btExcelExport.IsEnabled = true;
            }
        }
        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbMinValue.Text))
            {
                MessageBox.Show("导出范围最小值不能为空", "导出范围检查", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(this.tbMaxValue.Text))
            {
                MessageBox.Show("导出范围最大值不能为空", "导出范围检查", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int minValue = 0, maxValue = 0;

            bool minFlag = Int32.TryParse(this.tbMinValue.Text, out minValue);
            bool maxFlag = Int32.TryParse(this.tbMaxValue.Text, out maxValue);

            if (!minFlag || !maxFlag || maxValue < minValue)
            {
                MessageBox.Show("导出范围最小值不超过最大值", "导出范围检查", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            bool? result = saveFileDialog1.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                string file = saveFileDialog1.FileName; //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "号码.txt");
                using (StreamWriter writer = new StreamWriter(File.OpenWrite(file)))
                {
                    foreach (var item in EntityItems)
                    {
                        if (item.LeDou >= minValue && item.LeDou <= maxValue)
                        {
                            writer.WriteLine(string.Format("{0}	{1}	{2}", item.Tel, item.Pwd, item.LeDou));
                        }
                    }
                }
            }
            MessageBox.Show("导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EntityItems.Count; i++)// 
            {
                Entity item = EntityItems[i];
                item.TaskStatus = "准备登陆";
                ThreadPool.QueueUserWorkItem(new WaitCallback(SetLeDou), item);
            }

            this.btDuiHuan.IsEnabled = true;
            this.btJiFen.IsEnabled = true;
            if (_options != null && _options.Master == 1 && _service != null)
                _service.SendCmd("1");

        }
        private void SetLeDou(object state)
        {
            Entity item = state as Entity;
            if (state == null) return;

            string tel = item.Tel.ToString();
            string pwd = item.Pwd.ToString();

            try
            {
                WebOperResult wr = _helper.Login(tel, pwd, _log);
                if (wr == null)
                {
                    this.maiListView.Dispatcher.BeginInvoke(_leDouHandler, new object[] { item, "检查用户名或密码", null }); //BeginInvoke(lvItemHandle, new object[] { item, context });
                    return;
                }

                Match match = Regex.Match(wr.Text, "<p>乐豆：<span id=\"score_number_info\">\\s*(\\d+)\\s*</span>");

                if (match.Success)
                {
                    string context = match.Groups[1].Value;
                    this.maiListView.Dispatcher.BeginInvoke(_leDouHandler, new object[] { item, context, wr }); //BeginInvoke(lvItemHandle, new object[] { item, context });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        private void SetLeDouValue(Entity entity, string leDou, WebOperResult wr)
        {
            if (entity == null) return;
            if (wr == null)
            {
                entity.TaskStatus = leDou;
                entity.IsLogin = false;
                return;
            }
            int tmpLeDou = 0;
            Int32.TryParse(leDou, out tmpLeDou);
            entity.LeDou = tmpLeDou;
            entity.CookieContainer = wr.CookieContainer;
            entity.Cookies = wr.Cookies;
            entity.IsLogin = true;
            entity.TaskStatus = "登陆成功";

            if (cbJiFen.IsChecked.HasValue && cbJiFen.IsChecked.Value)
            {
                entity.TaskStatus = "查询积分";
                ThreadPool.QueueUserWorkItem(new WaitCallback(item =>
                {
                    try
                    {
                        _helper.Query(entity);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }

                }), entity);
            }
        }

        private void btDuiHuan_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EntityItems.Count; i++)
            {
                Entity item = EntityItems[i];
                if (!item.IsLogin)
                {
                    item.TaskStatus = "请检查用户名或密码";
                    continue;
                }
                item.TaskStatus = "准备兑换";
                string giftCode = (this.cbTasks.SelectedItem as ComboBoxItem).Tag.ToString();
                int leDou = 0;
                if (Config.GiftLeDouMap.ContainsKey(giftCode))
                {
                    leDou = Config.GiftLeDouMap[giftCode];
                }
                if (item.LeDou > leDou)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(entity =>
                    {
                        try
                        {
                            string result = _helper.DuiHuan(entity as Entity, _log, giftCode);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);
                        }

                    }), item);
                }
                else
                {
                    item.TaskStatus = "乐豆不足";
                }
            }
            if (_options != null && _options.Master == 1 && _service != null)
                _service.SendCmd("4");
        }

        /// <summary>
        /// 兑换积分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btJiFen_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EntityItems.Count; i++)
            {
                Entity item = EntityItems[i];
                if (!item.IsLogin)
                {
                    item.TaskStatus = "请检查用户名或密码";
                    continue;
                }
                item.TaskStatus = "查询积分";
                ThreadPool.QueueUserWorkItem(new WaitCallback(entity =>
                {
                    try
                    {
                        _helper.Query(item);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }), item);
            }
            if (_options != null && _options.Master == 1 && _service != null)
                _service.SendCmd("2");
        }

        private void ReadFile(string file)
        {
            int index = 1;
            string line = string.Empty;
            using (StreamReader reader = new StreamReader(file, _encode))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (string.IsNullOrEmpty(line)) continue;

                        Match match = Regex.Match(line, @"(\d+)\s+(\d+)");
                        if (match.Success)
                        {
                            Entity entity = new Entity(match.Groups[1].Value, match.Groups[2].Value);
                            entity.Dispatcher = this.Dispatcher;
                            entity.Seq = index;
                            EntityItems.Add(entity);
                            _AllRep.Add(entity);
                            index++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        private void cbSuccess_Checked(object sender, RoutedEventArgs e)
        {
            if (cbSuccess.IsChecked.HasValue && cbSuccess.IsChecked.Value)
            {
                EntityItems.Clear();

                for (int i = 0; i < _AllRep.Count; i++)
                {
                    Entity item = _AllRep[i];
                    if (item.Success)
                    {
                        EntityItems.Add(item);
                    }
                }
            }
        }

        private void cbSuccess_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _AllRep.Count; i++)
            {
                Entity item = _AllRep[i];
                EntityItems.Add(item);
            }
        }

        private void cbMultiClient_Checked(object sender, RoutedEventArgs e)
        {
            if (cbMultiClient.IsChecked.HasValue && cbMultiClient.IsChecked.Value)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _service = new RipperService();
                        // The service configuration is loaded from app.config

                        _host = new ServiceHost(_service);
                        _host.Open();
                        this.btNewClient.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        MessageBox.Show("开启多客户端失败，请查看日志", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }));
            }
            else
            {
                try
                {
                    Clean();
                    // The service configuration is loaded from app.config
                    this.btNewClient.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    MessageBox.Show("开启多客户端失败，请查看日志", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        /// <summary>
        /// 新客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNewClient_Click(object sender, RoutedEventArgs e)
        {
            string strFullPath = Process.GetCurrentProcess().MainModule.FileName;
            strFullPath = strFullPath.Replace(".vshost.", ".");// F:\gitrep\Ripper\Ripper\Ripper.View\bin\Debug\话费助手.vshost.exe
            Process process = Process.Start(strFullPath, " -m 0");
        }

        public void ReceiveCmd(string cmd)
        {
            switch (cmd)
            {
                case "-1"://关闭客户端
                    try
                    {
                        this.Close();
                        Process.GetCurrentProcess().Kill();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;
                case "1"://登陆
                    Dispatcher.Invoke(new Action(() =>
                    {
                        btLogin_Click(btLogin, new RoutedEventArgs());
                    }));
                    break;
                case "2"://积分
                    Dispatcher.Invoke(new Action(() =>
                    {
                        btJiFen_Click(btJiFen, new RoutedEventArgs());
                    }));
                    break;
                case "3"://点击登陆后自动查询
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.cbJiFen.IsChecked = true;
                    }));
                    break;
                case "4"://点击兑换
                    Dispatcher.Invoke(new Action(() =>
                    {
                        btDuiHuan_Click(btDuiHuan, new RoutedEventArgs());
                    }));
                    break;
                case "5"://取消登陆后自动查询
                    Dispatcher.Invoke(new Action(() =>
                    {
                        this.cbJiFen.IsChecked = false;
                    }));
                    break;
                default:
                    break;
            }
        }

        private void cbJiFen_Checked(object sender, RoutedEventArgs e)
        {
            if (cbJiFen.IsChecked.HasValue && cbJiFen.IsChecked.Value)
            {
                if (_options != null && _options.Master == 1 && _service != null)
                    _service.SendCmd("3");
            }
        }

        private void cbAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            this.tbInfo.Text = "自动执行取消成功";
            this.tbDuiHuan.IsEnabled = false;
            this.tbLogin.IsEnabled = false;
            this.btTimer.IsEnabled = false;

            if (_loginTimer != null)
                _loginTimer.Dispose();

            if (_duiHuanTimer != null)
                _duiHuanTimer.Dispose();

            if (cbJiFen.IsChecked.HasValue && cbJiFen.IsChecked.Value)
            {
                if (_options != null && _options.Master == 1 && _service != null)
                    _service.SendCmd("3");
            }
        }
        private void cbAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAuto.IsChecked.HasValue && cbAuto.IsChecked.Value)
            {
                this.tbDuiHuan.IsEnabled = true;
                this.tbLogin.IsEnabled = true;
                this.btTimer.IsEnabled = true;
            }
        }
        private void btTimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_loginTimer != null)
                    _loginTimer.Dispose();
                if (_duiHuanTimer != null)
                    _duiHuanTimer.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }


            DateTime now = DateTime.Now;
            DateTime loginTime = DateTime.Parse(now.Year + "-" + now.Month + "-" + now.Day + " " + this.tbLogin.Text.Trim());
            DateTime duiHuanTime = DateTime.Parse(now.Year + "-" + now.Month + "-" + now.Day + " " + this.tbDuiHuan.Text.Trim());

            _loginTimer = new Timer(new TimerCallback((obj) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    _log.Info(DateTime.Now.ToLongTimeString() + "开始登陆");
                    btLogin_Click(btLogin, new RoutedEventArgs());
                }));
            }), null, (long)(loginTime - now).TotalMilliseconds, 24 * 3600 * 1000);
            _duiHuanTimer = new Timer(new TimerCallback((obj) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    _log.Info(DateTime.Now.ToLongTimeString() + "开始兑换");
                    btDuiHuan_Click(btDuiHuan, new RoutedEventArgs());
                }));
            }), null, (long)(duiHuanTime - now).TotalMilliseconds, 24 * 3600 * 1000);

            this.tbInfo.Text = "自动执行设置成功";
        }
    }
}
