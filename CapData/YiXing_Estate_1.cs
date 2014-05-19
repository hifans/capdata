using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class YiXing_Estate_1 : CapData.FormBrowser
    {
        int page = 0;
        string linkList = "http://www.yxhouse.net/searchNewbuilding.action?pageNumber=";
        public YiXing_Estate_1()
        {
            InitializeComponent();
        }

        private void YiXing_Estate_1_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith(linkList))
            {
                HtmlElement divList = GetElementByClassName("tjlp_left", "div");
                if (divList == null||divList.Children.Count==0)
                {
                    this.NavToNext();
                    return;
                }

                foreach (HtmlElement item in divList.Children)
                {
                    try
                    {
                        if (item.GetAttribute("classname") != "tjlp_left_center") continue;
                        HtmlElement link = item.Children[1].Children[0].Children[0];
                        string strLink = link.GetAttribute("href");
                        listLink.Add(new KeyValuePair<string, object>(strLink, strLink.Split('=')[1]));
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("宜兴");
                me.EstNum = linkCurrent.Value.ToString();
                me.EstName = GetElementByClassName("tjlp_listshow_left_center_bg1_1", "span").InnerText;
                me.SaleTele = GetElementByClassName("tjlp_listshow_left_center_bg1_4", "span").InnerText;

                HtmlElement tbInfo = GetElementByClassName("tjlp_listshow_left_center_bg2_right", "div",0).Children[0].Children[0];
                me.DistrictName = tbInfo.Children[0].Children[1].InnerText;
                me.PlateName = tbInfo.Children[0].Children[3].InnerText;
                me.OpeningTime = tbInfo.Children[1].Children[1].InnerText;
                me.HandoverTime = tbInfo.Children[1].Children[3].InnerText;
                me.EstAddr = tbInfo.Children[2].Children[1].InnerText;
                me.EstUsage = tbInfo.Children[3].Children[1].InnerText;
                me.Developer = tbInfo.Children[5].Children[1].InnerText;

                tbInfo = GetElementByClassName("tjlp_lpxx_left_center_bg2_right", "div", 0).Children[0].Children[0];
                try { me.CountAll = int.Parse(tbInfo.Children[0].Children[1].InnerText.Split(' ')[0].Trim()); }
                catch { }
                me.GreeningRate = tbInfo.Children[1].Children[1].InnerText;
                me.PlotRatio = tbInfo.Children[1].Children[3].InnerText;
                me.PropertyCompany = tbInfo.Children[2].Children[1].InnerText;
                me.PropertyFee = tbInfo.Children[2].Children[3].InnerText;
                me.SaleAgent = tbInfo.Children[3].Children[1].InnerText;
                me.SaleAddr = tbInfo.Children[4].Children[1].InnerText;
                me.Traffic = tbInfo.Children[5].Children[1].InnerText;

                tbInfo = GetElementByClassName("xfpd_tjlpshowncont_tab", "div");
                me.EstIntroduction = tbInfo.Children[0].GetElementsByTagName("textarea")[0].GetAttribute("value");
                me.DevIntroduction = tbInfo.Children[1].GetElementsByTagName("textarea")[0].GetAttribute("value");
                me.EstFacility = tbInfo.Children[2].GetElementsByTagName("textarea")[0].GetAttribute("value");
                me.SrdFacility = tbInfo.Children[3].GetElementsByTagName("textarea")[0].GetAttribute("value");

                me.Save();
            }
            catch { }

            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            this.Navigate(linkList + page);
        }
    }
}
