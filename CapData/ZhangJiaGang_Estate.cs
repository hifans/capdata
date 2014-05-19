using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class ZhangJiaGang_Estate : CapData.FormBrowser
    {
        public ZhangJiaGang_Estate()
        {
            InitializeComponent();
        }

        string url = "http://www.zjgfc.gov.cn/web/lp/search_loupan.aspx";
        private void ZhangJiaGang_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
        }
    }
}
