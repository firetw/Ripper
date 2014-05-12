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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FetionLoginer.VerifyHelper;
using System.IO;
using System.Net;

namespace FetionLoginer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UUVerifyImp imp = null;
        public MainWindow()
        {
            InitializeComponent();



            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            imp = new UUVerifyImp();
            imp.Login("wangnuu", "123qwe!@#");
            SaveImg("1", "E:/uu1.png");
            SaveImg("2", "E:/uu2.png");
        }

        public void SaveImg(string url1, string filePath)
        {
            string url = "http://gz.feixin.10086.cn/Account/Login/verify?0.4702669670805335";

            BitmapImage bi = new BitmapImage(new Uri(url));
            if (url1 == "1")
                this.img1.Source = bi;
            else
                this.img2.Source = bi;
            //byte[] buffer = BitmapImageToByteArray(bi);

            byte[] buffer;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                buffer = ms.ToArray();
            }
            string result = imp.RecognizeByBytes(buffer);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bi));
            using (MemoryStream ms = new MemoryStream())
            {
                png.Save(ms);
                buffer = ms.ToArray();
            }
            result = imp.RecognizeByBytes(buffer);

            /*   //不能请求，否则图片会不一样
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            byte[] buffer = new byte[response.ContentLength];
            //stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Flush();


            string result = imp.RecognizeByBytes(buffer);
              */


        }
        public byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] byteArray = null;

            try
            {
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。   
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return byteArray;
        }

    }
}
