using CommandLine;
using Ripper.View.Model;
using SamSung;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ripper.View
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            MainWindow mw = new MainWindow();

            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(e.Args, options))
            {

                mw.Initilize(options);

            }
            else
            {
                mw.Initilize();
            }
            mw.Show();




            //SamSung.SamSungRegister register = new SamSung.SamSungRegister();

            

            //SamSungRegister register = new SamSungRegister();
            //register.Show();



        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            _log.Error(sender.ToString() + "\r\n" + e.Dispatcher.ToString() + "\r\n" + e.Exception);
        }
    }


}

