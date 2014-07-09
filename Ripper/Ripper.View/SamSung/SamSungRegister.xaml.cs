using Microsoft.Win32;
using Ripper.View.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using Uuwise;
using WebLoginer.Core;

namespace SamSung
{
    /// <summary>
    /// SamSungRegister.xaml 的交互逻辑
    /// </summary>
    public partial class SamSungRegister : Window
    {
        public enum ExecMode
        {
            FromCurrent = 0,
            OnlyCurrent = 1
        }

        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RegisterTask _task = null;
        DispatcherTimer _smsTimer = new DispatcherTimer();
        UUVerifyImp _imp = null;
        ContextMenu ctlMenu = null;
        RegisterContext _currentConext = null;
        List<RegisterContext> Contexts = new List<RegisterContext>();
        ExecMode _currentMode = ExecMode.FromCurrent;
        Encoding _encode = null;
        bool _isSmsVerify = false;


        public SamSungRegister()
        {
            InitializeComponent();

            Initilize();

        }

        private void Initilize()
        {
            _encode = Encoding.GetEncoding(System.Configuration.ConfigurationManager.AppSettings["encoding"]);
            ctlMenu = new ContextMenu();
            Label execCurrent = new Label();
            execCurrent.MouseLeftButtonUp += Exec;
            execCurrent.Content = "从本行开始";
            execCurrent.Tag = ExecMode.FromCurrent;

            Label onlyCurrentRow = new Label();
            onlyCurrentRow.MouseLeftButtonUp += Exec;
            onlyCurrentRow.Content = "仅执行本行";
            onlyCurrentRow.Tag = ExecMode.OnlyCurrent;

            this.Loaded += SamSungRegister_Loaded;
            _smsTimer.Interval = TimeSpan.FromSeconds(1);
            _smsTimer.Tick += _smsTimer_Tick;

            ctlMenu.Items.Add(execCurrent);
            ctlMenu.Items.Add(onlyCurrentRow);
            this.maiListView.ContextMenu = ctlMenu;
            this.tbTime.Text = "";
            this.tbTelCode.TextChanged += tbTelCode_TextChanged;
            this.tbImgVerify.TextChanged += tbImgVerify_TextChanged;

            this.btTelCodeVerify.Visibility = Visibility.Collapsed;
            this.btSubmit.Visibility = Visibility.Collapsed;
        }

        void tbImgVerify_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSmsVerify) return;
            if (tbImgVerify.Text.Trim().Length != 6) return;
            if (this.cbAutoSubmit.IsChecked.HasValue && this.cbAutoSubmit.IsChecked.Value)
            {
                this._task.OnReceiveImgCodeCompleted(this.tbImgVerify.Text.Trim());
            }
        }

        void tbTelCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTelCode.Text.Trim().Length != 4) return;


            if (this.cbAutoSubmit.IsChecked.HasValue && this.cbAutoSubmit.IsChecked.Value)
            {
                VerifyTelCode();
            }

        }

        void Exec(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;
            if (element.Tag == null) return;
            if (this.maiListView.SelectedItem == null) return;
            RegisterContext context = this.maiListView.SelectedItem as RegisterContext;
            if (context == null) return;

            ExecMode mode = (ExecMode)element.Tag;
            switch (mode)
            {
                case ExecMode.FromCurrent:
                    _currentMode = ExecMode.FromCurrent;
                    break;
                case ExecMode.OnlyCurrent:
                    _currentMode = ExecMode.OnlyCurrent;
                    break;
                default:
                    break;
            }
            _currentConext = context;
            DoTask(_currentConext);
        }
        int index = 1;
        void _smsTimer_Tick(object sender, EventArgs e)
        {
            index--;
            this.tbTime.Text = index.ToString();
            if (index == 0)
            {
                index = 60;
                _smsTimer.Stop();
            }
        }

        void SamSungRegister_Loaded(object sender, RoutedEventArgs e)
        {
            _imp = GeUu();
        }

        void _task_OnVerifyCodeFailed(VerifyFailedCode code, string message)
        {
            string msg = string.Empty;
            switch (code)
            {
                case VerifyFailedCode.TelVerifyFailed:
                    msg = "手机码验证失败，请重新获取";
                    break;
                case VerifyFailedCode.ImgVerifyFailed:
                    msg = "图片码验证失败，请重新获取";
                    break;
                default:
                    break;
            }
            Dispatcher.BeginInvoke(new Action(
              () =>
              {
                  this.msg.Text = msg;
              }
               ));
        }
        void _task_OnImgVerifyCompleted(string verifyCode)
        {
            Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    this.tbImgVerify.Text = verifyCode;
                }
                ));

        }

        void _task_OnImgCompleted(byte[] source)
        {
            Dispatcher.BeginInvoke(new Action(
               () =>
               {
                   BitmapImage bmp = null;
                   try
                   {
                       this.imgVerify.Source = null;
                       bmp = new BitmapImage();
                       bmp.BeginInit();
                       bmp.StreamSource = new MemoryStream(source);
                       bmp.EndInit();
                       this.imgVerify.BeginInit();
                       this.imgVerify.Source = bmp;
                       this.imgVerify.EndInit();
                   }
                   catch
                   {
                       bmp = null;
                   }
               }
               ));
        }
        private void btTelCodeVerify_Click(object sender, RoutedEventArgs e)
        {
            VerifyTelCode();
        }

        private void VerifyTelCode()
        {
            index = 60;
            _smsTimer.Stop();
            this._task.OnReceiveTelCodeCompleted(this.tbTelCode.Text.Trim());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._task.ReSendSms();
            index = 60;
            _smsTimer.Start();


        }

        private void btNewVerifyImg_Click(object sender, RoutedEventArgs e)
        {
            this._task.ReGetImage();
        }

        private void btSubmit_Click(object sender, RoutedEventArgs e)
        {
            this._task.OnReceiveImgCodeCompleted(this.tbImgVerify.Text.Trim());
        }

        private void cbAutoVerify_Click(object sender, RoutedEventArgs e)
        {
            this._task.AutoVerify = this.cbAutoVerify.IsChecked.HasValue && this.cbAutoVerify.IsChecked.Value;
        }
        private void cbAutoSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbAutoSubmit.IsChecked.HasValue && this.cbAutoSubmit.IsChecked.Value)
            {
                this.btSubmit.Visibility = Visibility.Collapsed;
                this.btTelCodeVerify.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.btSubmit.Visibility = Visibility.Visible;
                this.btTelCodeVerify.Visibility = Visibility.Visible;
            }
        }
        private void DoTask(RegisterContext context)
        {
            _task = new RegisterTask();
            _currentConext = context;
            if (_currentConext == null) return;
            _isSmsVerify = false;

            Dispatcher.Invoke(new Action(() =>
            {
                this.msg.Text = string.Empty;
                this._smsTimer.Stop();
                this.index = 60;
                _task.AutoVerify = this.cbAutoVerify.IsChecked.HasValue && this.cbAutoVerify.IsChecked.Value;
                this.tbInfo.Text = string.Format("SEQ:{0} TEL:{1} NAME:{2} ID:{3}", context.Seq, context.Tel, context.FullName, context.Id);

            }), null);


            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
            {
                context.Imp = _imp;
                context.OnExecInfoChanged += context_OnExecInfoChanged;

                _task.OnGetSmsCompleted += _task_OnGetSmsCompleted;
                _task.OnTaskCompleted += OnTaskCompleted;
                _task.OnImgCompleted += _task_OnImgCompleted;
                _task.OnImgVerifyCompleted += _task_OnImgVerifyCompleted;
                _task.OnVerifyCodeFailed += _task_OnVerifyCodeFailed;
                _task.Process(context);

            }), null);

        }

        void _task_OnGetSmsCompleted()
        {
            Dispatcher.BeginInvoke(new Action(
               () =>
               {
                   index = 60;
                   _smsTimer.Start();
               }
               ));
        }
        private void OnTaskCompleted(bool isCompleted)
        {
            _task.OnTaskCompleted -= OnTaskCompleted;
            _task.OnImgCompleted -= _task_OnImgCompleted;
            _task.OnImgVerifyCompleted -= _task_OnImgVerifyCompleted;
            _task.OnVerifyCodeFailed -= _task_OnVerifyCodeFailed;
            _isSmsVerify = false;

            Dispatcher.Invoke(new Action(() =>
            {
                if (_currentConext != null)
                    _currentConext.RegisterInfo = _currentConext.ExecInfo;

            }), null);

            switch (_currentMode)
            {
                case ExecMode.FromCurrent:
                    int index = Contexts.IndexOf(_currentConext);
                    if (index == -1 || index >= this.Contexts.Count - 1)
                        break;
                    index++;
                    DoTask(Contexts[index]);
                    break;
                case ExecMode.OnlyCurrent:
                    break;
                default:
                    break;
            }

        }


        private void btDoTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int startIndex = this.maiListView.SelectedIndex;
                if (startIndex == -1)
                    startIndex = 0;
                RegisterContext context = this.maiListView.Items[startIndex] as RegisterContext;
                if (context == null) return;
                DoTask(context);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        void context_OnExecInfoChanged(string message)
        {
            Dispatcher.BeginInvoke(new Action(
               () =>
               {
                   this.msg.Text = message;
                   if (_log != null)
                   {
                       _log.Info(GetMsg(message));
                   }
                   if (message == "注册成功" || message == "注册失败")
                   {
                       _task.OnImgCompleted -= _task_OnImgCompleted;
                       _task.OnImgVerifyCompleted -= _task_OnImgVerifyCompleted;
                       _task.OnVerifyCodeFailed -= _task_OnVerifyCodeFailed;
                   }
                   if (message == "短信验证通过")
                   {
                       _isSmsVerify = true;
                   }
               }
               ));
        }
        private string GetMsg(string msg)
        {
            if (_currentConext == null) return msg;
            return string.Format("Tel:{0},Message:{1}", _currentConext.Tel, msg);
        }
        private UUVerifyImp GeUu()
        {
            _log.Info("初始化优优云认证工具...");
            string uuUserName = System.Configuration.ConfigurationManager.AppSettings["user"];
            string uuPwd = System.Configuration.ConfigurationManager.AppSettings["pwd"];

            UUVerifyImp imp = null;
            try
            {
                imp = new UUVerifyImp();
                if (imp.Login(uuUserName, uuPwd))
                {
                    _log.Info(string.Format("uuUserName:{0},uuPwd:{1}", uuUserName, uuPwd));
                    _log.Info("验证码工具初始化成功");
                }
                else
                {
                    MessageBox.Show("验证码工具登录失败", "错误");
                    _log.Error("验证码工具初始化失败");
                }
            }
            catch (Exception ex)
            {
                imp = null;
                MessageBox.Show("验证码工具登录失败", "错误");
                _log.Error(ex);
                return null;
            }
            return imp;
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
                Contexts.Clear();
                this.maiListView.ItemsSource = null;
                string file = openFileDialog1.FileName;
                ReadFile(file);

                this.maiListView.ItemsSource = Contexts;
            }
        }

        private void ReadFile(string fileName)
        {
            int index = 1;
            string line = string.Empty;
            using (StreamReader reader = new StreamReader(fileName, _encode))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (string.IsNullOrEmpty(line)) continue;

                        RegisterContext entity = new RegisterContext(line);
                        if (!entity.IsValidate()) continue;
                        entity.Seq = index;
                        Contexts.Add(entity);
                        index++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}
