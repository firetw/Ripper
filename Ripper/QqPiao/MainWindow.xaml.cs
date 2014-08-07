using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Net;
using System.Windows.Shapes;
using WebLoginer.Core;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using QqPiao.Model;
using Newtonsoft.Json;
using System.Threading;

namespace QqPiao
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        List<City> _cities = new List<City>();
        List<City> _currentTasks = new List<City>();
        private object lockObject = new object();
        string _errorFile = string.Empty;
        private int _currentIndex = 0;
        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            string dir = AppDomain.CurrentDomain.BaseDirectory + "/log-file";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            _errorFile = dir + "/错误.txt";
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void StartWorkThread()
        {
            Thread workThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    City item = null;
                    lock (lockObject)
                    {
                        if (_currentIndex >= _cities.Count) return;
                        item = _cities[_currentIndex];
                        Interlocked.Add(ref _currentIndex, 1);
                    }
                    QqJdTask task = null;
                    try
                    {
                        task = new QqJdTask();
                        task.OnTaskTimeout += task_OnTaskTimeout;
                        if (item != null)
                        {
                            task.Process(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                    finally
                    {
                        if (task != null)
                        {
                            if (item.Status != Task.TaskStatus.Success)
                            {
                                lock (lockObject)
                                {
                                    using (StreamWriter writer = File.AppendText(_errorFile))
                                    {
                                        writer.Write(item.id + ",");
                                    }
                                }
                            }
                        }
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.collectLb.Content = string.Format("已采集:{0}", _currentIndex + 1);
                        }));

                        if (task != null)
                            task.OnTaskTimeout -= task_OnTaskTimeout;
                    }
                }
            }));
            workThread.Start();
        }

        void task_OnTaskTimeout(City context)
        {
            StartWorkThread();//意味着有一个工作线程被杀死
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Data/area.json", Encoding.GetEncoding("gbk"));

            Dictionary<string, City> map = JsonConvert.DeserializeObject<Dictionary<string, City>>(content);

            var keys = map.Keys;
            foreach (var item in keys)
            {
                map[item].id = item;
            }

            string filters = this.tbError.Text;
            List<string> filterCities = new List<string>();
            if (!string.IsNullOrEmpty(filters))
            {
                string[] tmp = filters.Split(',');
                foreach (var item in tmp)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        filterCities.Add(item);
                    }
                }
            }
            foreach (var item in map)
            {
                if ((filterCities.Count == 0) || (filterCities.Count > 0 && filterCities.Count(c => c == item.Key) > 0))
                {
                    _cities.Add(item.Value);
                }
            }
            this.cityLb.Content = string.Format("共有地市:{0}", _cities.Count);
            int threadCount = 5 > _cities.Count ? 1 : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ThreadCount"]);
            for (int i = 0; i < threadCount; i++)
            {
                StartWorkThread();
            }
        }
    }
}
