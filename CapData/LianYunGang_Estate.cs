using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class LianYunGang_Estate : CapData.FormBrowser
    {
        public LianYunGang_Estate()
        {
            InitializeComponent();
        }
        int page = 0;
        private void LianYunGang_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != Document.Url.ToString()) return;
            if (e.Url.ToString().StartsWith("http://www.lygfdc.com/WebSite/Search/Default.aspx"))
            {
                HtmlElementCollection coll = Document.GetElementById("ctl00_CPH_M_sm_sBox_data").Children[0].GetElementsByTagName("dl");
                if (coll.Count == 0) { this.NavToNext(); return; }
                foreach (HtmlElement dl in coll)
                {
                    HtmlElement link = dl.Children[0].Children[0];
                    string strLink = link.GetAttribute("href");
                    if (!Fn.IsExistEstUrl(strLink) && !UrlInLinkList(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText));
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("连云港");
                me.SourceLink = linkCurrent.Key;
                me.EstName = linkCurrent.Value.ToString();
                me.EstNum = me.SourceLink.Split('=')[1];

                HtmlElement info1 = Document.GetElementById("ctl00_CPH_M_sm_spfBox3").Children[0].Children[1].Children[0];
                me.DistrictName = info1.Children[0].Children[3].InnerText;
                me.EstAddr = info1.Children[1].Children[1].InnerText;
                me.CompletionTime = info1.Children[2].Children[3].InnerText;
                me.EstUsage = info1.Children[3].Children[1].InnerText;
                me.CountAll = int.Parse(info1.Children[5].Children[3].InnerText);
                me.Designer = info1.Children[6].Children[1].InnerText;
                me.Builder = info1.Children[6].Children[3].InnerText;
                me.PropertyCompany = info1.Children[8].Children[1].InnerText;
                me.PropertyFee = info1.Children[8].Children[3].InnerText;
                me.BuildingAreaAll = double.Parse(info1.Children[9].Children[1].InnerText);
                me.BuildingDensity = info1.Children[9].Children[3].InnerText;
                me.GroundAreaAll = double.Parse(info1.Children[11].Children[1].InnerText);
                me.GreeningRate = info1.Children[11].Children[3].InnerText;
                me.PlotRatio = info1.Children[12].Children[3].InnerText;

                info1 = Document.GetElementById("ctl00_CPH_M_sm_spfBox_dev").Children[0].Children[1].Children[0];
                me.Developer = info1.Children[0].Children[1].InnerText;

                info1 = Document.GetElementById("ctl00_CPH_M_sm_spfBox5").Children[0].Children[1];
                me.EstIntroduction = info1.InnerText;

                me.Save();
            }
            catch { }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            this.Navigate("http://www.lygfdc.com/WebSite/Search/Default.aspx?type=spf&key=%u8BF7%u8F93%u5165%u697C%u76D8%u540D%u79F0%u6216%u5F00%u53D1%u5546&page=" + page);
        }
    }
}
