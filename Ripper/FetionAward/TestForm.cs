using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;

namespace WeiBoGrab
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            this.comboBox1.Items.Add("UrlDecode");
            this.comboBox1.Items.Add("HtmlDecode");
            this.comboBox1.Items.Add("UrlEncode");
            this.comboBox1.Items.Add("HtmlEncode");
            this.label1.Text = string.Empty;
            this.Load += TestForm_Load;
            this.KeyPress += TestForm_KeyPress;
        }

        void TestForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar ==(char)13)
            {
                button1_Click(button1, EventArgs.Empty);
            }
        }

        void TestForm_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        void TestForm_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox1.Text)) return;
            string decode = string.Empty;
            switch (this.comboBox1.SelectedItem.ToString())
            {
                case "UrlEncode":
                    decode = System.Web.HttpUtility.UrlEncode(this.textBox1.Text);
                    break;
                case "UrlDecode":
                    decode = System.Web.HttpUtility.UrlDecode(this.textBox1.Text);
                    break;
                case "HtmlEncode":
                    decode = System.Web.HttpUtility.HtmlEncode(this.textBox1.Text);
                    break;
                case "HtmlDecode":
                    decode = System.Web.HttpUtility.HtmlDecode(this.textBox1.Text);
                    break;
                default:
                    break;
            }
            this.label1.Text = decode;

            this.listBox1.Items.Insert(0, string.Format("{0} {1}", this.textBox1.Text, decode));

        }
    }
}
