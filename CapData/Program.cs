using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CapData
{
    static class Program
    {
        /// <summary>
        /// 程序启动时的时间戳
        /// </summary>
        public static DateTime DateTime_ProStart = DateTime.Now;

        /// <summary>
        ///  程序主窗口
        /// </summary>
        public static MainForm MainForm = null;

        /// <summary>
        /// 浏览器窗口数量
        /// </summary>
        public static int CountBrowserForm = 0;

        /// <summary>
        /// 是否正在使用代理
        /// </summary>
        public static bool IsUsingProxy = false;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!Fn.InitConn())
            {
                Application.Exit();
            }
            else
            {
                MainForm = new MainForm();
                Application.Run(MainForm);
                Fn.CloseConn();
            }
        }
    }
}
