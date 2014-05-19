using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class WuXi_Permit : CapData.FormBrowser
    {
        public WuXi_Permit()
        {
            InitializeComponent();
        }

        List<string> listEstNum = new List<string>();
        int indexEstNum = -1;

        private void WuXi_Permit_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Browser.Navigating += new WebBrowserNavigatingEventHandler(Browser_Navigating);

            Start();
            //this.timerWait.Start();
        }

        private void Start()
        {
            DataTable dt = Fn.GetDT_PgSQL("select estnum from estate_info where cityId='" + Fn.GetCityIdByName("无锡") + "'");
            foreach (DataRow dr in dt.Rows)
            {
                listEstNum.Add(dr[0].ToString().Trim());
            }

            this.NavToNextEstate();
        }

        void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() == "http://pub.wxlife.cn/default.html") e.Cancel = true;
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            Model_Permit mp = new Model_Permit("无锡");
            
            if (e.Url.ToString().StartsWith("http://www.wxhouse.com/newhouse/bainfo.html?houseid="))
            {
                foreach (HtmlElement elLink in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = elLink.GetAttribute("href");
                        string strPermitName = elLink.InnerText.Trim();
                        
                        if (strLink.StartsWith("http://pub.wxlife.cn/BuildInfo.pub?blid="))
                        {
                            string strPermitNum=strLink.Split('=')[1];
                            if (mp.IsExist(strPermitNum)) continue;
                            //strLink = "http://pub.wxlife.cn/ifrm_BuildStat.pub?blid=" + strLink.Split('=')[1];
                            string strDate = elLink.Parent.NextSibling.InnerText.Split('：')[1];
                            listLink.Add(new KeyValuePair<string, object>(strLink, new string[] { listEstNum[indexEstNum], strPermitName, strDate }));
                        }
                    }
                    catch { }
                }
                this.NavToNextEstate();
                return;
            }

            mp.SourceLink = Document.Url.ToString();
            try
            {
                mp.EstNum = (linkCurrent.Value as string[])[0];
                mp.PermitNum = linkCurrent.Key.Split('=')[1];
                mp.PermitName = (linkCurrent.Value as string[])[1];
                mp.IssueDate = DateTime.Parse((linkCurrent.Value as string[])[2]);

                HtmlElement tbody = Document.Window.Frames[0].Document.GetElementById("info").Children[0];
                try { mp.ConstructionDate = DateTime.Parse(tbody.Children[17].Children[1].InnerText); }
                catch { }
                try { mp.CompletionDate = DateTime.Parse(tbody.Children[18].Children[1].InnerText); }
                catch { }
                try { mp.SaleArea = double.Parse(tbody.Children[20].Children[1].InnerText.Split(' ')[0]); }
                catch { }
                try { mp.SaleCount = int.Parse(tbody.Children[22].Children[1].InnerText); }
                catch { }

                mp.Save();

                Model_PermitSale mps = new Model_PermitSale(mp);
                mps.SaleArea = mp.SaleArea;
                mps.SaleCount = mp.SaleCount;
                mps.Save(true);

                Model_PermitSold mpsd = new Model_PermitSold(mp);
                foreach (HtmlElement item in Document.GetElementsByTagName("td"))
                {
                    if (item.InnerText == "已　售：")
                    {
                        try
                        {
                            mpsd.SoldCount = int.Parse(item.NextSibling.InnerText.Split(' ')[0].Replace("套",""));
                        }
                        catch { }
                        break;
                    }
                }
                if (mpsd.SoldCount > 0) mpsd.Save(true);
            }
            catch(Exception ex) {
                Program.MainForm.AddMessage("错误！无锡许可证数据抓取错误。链接：" + mp.SourceLink + "。错误消息：" + ex.Message);
            }

            this.NavToNext();
            
        }

        private void NavToNextEstate()
        {
            indexEstNum++;
            if (indexEstNum >= listEstNum.Count)
            {
                DataTable dt = Fn.GetDT_MySQL("select SourceLink, estNum, PermitNum from permitinfo where cityid='" + Fn.GetCityIdByName("无锡").ToString() + "' and saleCount=-1");
                foreach (DataRow dr in dt.Rows)
                {
                    listLink.Add(new KeyValuePair<string, object>(dr[0].ToString(), new string[] { dr[1].ToString(), dr[2].ToString(), DateTime.Today.ToString("yyyy-MM-dd") }));
                }
                this.NavToNext();
                return;
            }

            this.Navigate("http://www.wxhouse.com/newhouse/bainfo.html?houseid="+listEstNum[indexEstNum]);
        }

        private void timerWait_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(WuXi_Estate).Name)) return;
            this.timerWait.Stop();

            //Start();
        }
    }
}
