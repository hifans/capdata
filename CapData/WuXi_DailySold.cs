using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class WuXi_DailySold : CapData.FormBrowser
    {
        public WuXi_DailySold()
        {
            InitializeComponent();
        }

        private void WuXi_DailySold_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://pub.wxlife.cn/Default.pub");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Browser.Url) return;

            try
            {
                HtmlElement tbody = Document.GetElementById("statinfo").Children[0];
                HtmlElement tr = tbody.Children[1];
                Model_DailySold mds = new Model_DailySold("无锡");
                //mds.SoldCount = int.Parse(tr.Children[1].InnerText.Replace("套", ""));
                //mds.SoldArea = double.Parse(tr.Children[2].InnerText.Replace("m2", ""));
                //mds.avePrice = double.Parse(tr.Children[3].InnerText.Replace("万元", ""));
                //if (mds.SoldArea > 0) mds.avePrice = mds.avePrice * 10000 / mds.SoldArea;
                //mds.Save();

                if (Fn.GetDT_MySQL("select * from city_dailysold where cityid=" + mds.CityId + " and soldDate='" + DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd") + "'").Rows.Count == 0)
                {
                    //抓取前一天数据
                    tr = tbody.Children[2];
                    mds.SoldDate = mds.SoldDate.AddDays(-1);
                    mds.SoldCount = int.Parse(tr.Children[1].InnerText.Replace("套", ""));
                    mds.SoldArea = double.Parse(tr.Children[2].InnerText.Replace("m2", ""));
                    try
                    {
                        mds.avePrice = double.Parse(tr.Children[3].InnerText.Replace("万元", ""));
                    }
                    catch { }
                    if (mds.SoldArea > 0) mds.avePrice = mds.avePrice * 10000 / mds.SoldArea;
                    mds.Save();
                }

            }
            catch(Exception ex) {
                Program.MainForm.AddMessage("无锡每日成交信息抓取失败！错误消息："+ex.Message);
            }

            this.Close();
        }
    }
}
