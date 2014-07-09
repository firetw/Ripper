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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //MainWindow mw = new MainWindow();

            //var options = new Options();
            //if (CommandLine.Parser.Default.ParseArguments(e.Args, options))
            //{

            //    mw.Initilize(options);

            //}
            //else
            //{
            //    mw.Initilize();
            //}
            //mw.Show();

            //SamSung.SamSungRegister register = new SamSung.SamSungRegister();
            SamSungRegister register = new SamSungRegister();
            register.Show();



        }
    }


}

