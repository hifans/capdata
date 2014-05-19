using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CapWebPageToImage
{
    static class Program
    {
        public static int Result = 0;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static int Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormCap(args));
            return Result;
        }
    }
}
