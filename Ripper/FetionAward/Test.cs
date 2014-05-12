using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WeiBoGrab
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
            this.Load += new EventHandler(Test_Load);
        }

        void Test_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(new Uri("http://www.baidu.com"));
            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            HtmlElement element = webBrowser1.Document.GetElementById("su1");
            element.Focus();
        }
    }
}
