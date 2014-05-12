using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ripper.TaskDispather;

namespace Ripper
{
    /// <summary>
    /// Interaction logic for FetionView.xaml
    /// </summary>
    public partial class FetionView : Window
    {
        public FetionView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(FetionView_Loaded);
        }

        void FetionView_Loaded(object sender, RoutedEventArgs e)
        {
            WebContext context = new WebContext();
            FetionWebTask fetion = new FetionWebTask(context);
            fetion.DoTask();












        }
    }
}
