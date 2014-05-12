using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WeiBoGrab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = "firetw@163.com";
            this.textBox2.Text = "1qaz!QAZ";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string username = textBox1.Text.ToString();
            string password = textBox2.Text.ToString();
            string url = "http://weibo.com/";
            GetPage getpage = new GetPage();
            StreamWriter sw = File.CreateText("FollowUrl.txt");
            WebBrowser browser = webBrowser1;

            browser.Navigate(new Uri(@url));
            //加载登陆页面
            textBox3.Text += getpage.GetLoginPage(browser);
            //登陆操作
            LoginSubmit loginsubmit = new LoginSubmit(username, password);
            loginsubmit.LoginClick(browser);
            //加载个人主页
            textBox3.Text += getpage.GetMainPage(browser);
            //获取关注对象
            Follow follow = new Follow();
            follow.GetFollows(browser);
            follow.GetFollowsUrl(browser, sw);

            FileStream fs = new FileStream("FollowUrl.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                string[] arry = s.Split('|');
                string name = arry[1];
                string user_url = arry[2];
                WeiBo feed = new WeiBo(name, user_url);
                feed.GetWeiBo(browser);
            }
            sr.Close();
        }
    }
}

