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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form2_Load);

        }

        void Form2_Load(object sender, EventArgs e)
        {
            InitializeOpenFileDialog();
            InitializePictureBox();
        }
        private void InitializePictureBox()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox1.BorderStyle =
                System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Location = new System.Drawing.Point(72, 112);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 136);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;

        }

        private void InitializeOpenFileDialog()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1.FileOk += new CancelEventHandler(openFileDialog1_FileOk);
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";

        }

        private void fileButton_Click(System.Object sender, System.EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }


        // This method handles the FileOK event.  It opens each file 
        // selected and loads the image from a stream into pictureBox1.
        private void openFileDialog1_FileOk(object sender,
            System.ComponentModel.CancelEventArgs e)
        {

            this.Activate();
            string[] files = openFileDialog1.FileNames;

            // Open each file and display the image in pictureBox1.
            // Call Application.DoEvents to force a repaint after each
            // file is read.        
            foreach (string file in files)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                System.IO.FileStream fileStream = fileInfo.OpenRead();
                pictureBox1.Image = System.Drawing.Image.FromStream(fileStream);
                //Application.DoEvents();
                fileStream.Close();

                // Call Sleep so the picture is briefly displayed, 
                //which will create a slide-show effect.
                System.Threading.Thread.Sleep(2000);
            }
            pictureBox1.Image = null;
        }
    }
}
