using Microsoft.Win32;
using Ripper.View.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ripper.View.Henan
{
    /// <summary>
    /// QueryView.xaml 的交互逻辑
    /// </summary>
    public partial class QueryView : Window
    {
        ObservableCollection<Entity> EntityItems = new ObservableCollection<Entity>();
        List<Entity> _AllRep = new List<Entity>();
        int _execCount = 0;
        int _currentIndex = 0;
        Encoding _encode;
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        List<Thread> threads = new List<Thread>();

        List<System.Windows.Forms.WebBrowser> browsers = null;
        private object lockObject = new object();


        public QueryView()
        {
            _encode = Encoding.UTF8;
            InitializeComponent();
            this.maiListView.ItemsSource = EntityItems;
        }

        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Empty;
            try
            {
                _execCount = 0;
                _currentIndex = 0;
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
                        if (browsers != null && browsers.Count > 0)
                        {
                            for (int i = 0; i < browsers.Count; i++)
                            {
                                browsers[i].Dispose();
                            }
                        }
                        this.tbInfo.Text = o;
                    }), msg);
                }
            }
        }
        private void btImport_Click(object sender, RoutedEventArgs e)
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
                EntityItems.Clear();
                _AllRep.Clear();
                NPOIHelper.Instance.ReLoad();
                _execCount = 0;

                string file = openFileDialog1.FileName;
                ReadFile(file);
            }
            this.btQuery.IsEnabled = true;
            this.tbInfo.Text = "导入完成";
        }

        private List<string> GetRsaInfo(int index, string rsaQueryUrl)
        {

            string rsaPhone = string.Empty;
            string rsaPwd = string.Empty;

            List<string> list = new List<string>();
            System.Windows.Forms.WebBrowser browser = browsers[index];
            browser.Navigate(rsaQueryUrl);

            while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            while (!browser.Document.GetElementById("telRsa").OuterHtml.Contains("value="))
            {
                System.Windows.Forms.Application.DoEvents();
            }
            rsaPhone = browser.Document.GetElementById("telRsa").GetAttribute("value");
            rsaPwd = browser.Document.GetElementById("pwdRsa").GetAttribute("value");
            list.Add(rsaPhone);
            list.Add(rsaPwd);
            return list;
        }
        private void btQuery_Click(object sender, RoutedEventArgs e)
        {
            int threadCount = 5 > _AllRep.Count ? 1 : Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ThreadCount"]);
            browsers = new List<System.Windows.Forms.WebBrowser>();

            for (int i = 0; i < threadCount; i++)
            {
                browsers.Add(new System.Windows.Forms.WebBrowser());
                Thread workThread = new Thread(new ParameterizedThreadStart((index) =>
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
                                HnWTQuery query = new HnWTQuery();
                                query.OnEncrptyPwd += query_OnEncrptyPwd;
                                query.OnTaskCompleted += query_OnTaskCompleted;
                                item.StartTime = DateTime.Now.ToString();
                                item.EndTime = DateTime.Now.ToString();
                                item.BrowserIndex = Convert.ToInt32(index);

                                query.Context = item;
                                query.Process(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);
                        }
                    }
                }));
                threads.Add(workThread);
                workThread.Start(i);
            }
            this.btExport.IsEnabled = true;
            this.tbInfo.Text = "正在执行";
        }

        List<string> query_OnEncrptyPwd(int index, string url)
        {
            List<string> result = new List<string>();
            Object obj = Dispatcher.Invoke(new Func<int, string, List<string>>(GetRsaInfo), index, url);
            return obj as List<string>;
        }
    }
}
