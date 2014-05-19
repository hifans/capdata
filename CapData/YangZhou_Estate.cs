using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class YangZhou_Estate : CapData.FormBrowser
    {
        public YangZhou_Estate()
        {
            InitializeComponent();
        }

        string url = "http://www.yzfdc.cn/BuildingDish_Project_Search.aspx?Type=1";
        private void YangZhou_Estate_Load(object sender, EventArgs e)
        {
            Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }


    }
}
