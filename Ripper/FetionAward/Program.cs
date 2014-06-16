using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeiBoGrab.Verify.SamSung;

namespace WeiBoGrab
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FLogin());

            //Application.Run(new RegisterForm());

            Application.Run(new SamSungManual());
        }
    }
}
