using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CapData
{
    public partial class XuZhou_Estate : CapData.FormBrowser
    {
        public XuZhou_Estate()
        {
            InitializeComponent();
        }

        int page = 0;
        int pageMax = -1;
        private void XuZhou_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://www.xzhouse.com.cn/houseSearch.aspx?page="))
            {
                if (pageMax < 0)
                {
                    try
                    {
                        Regex reg = new Regex("共(\\d+)页");
                        Match m = reg.Match(Document.Body.InnerText);
                        pageMax = int.Parse(m.Groups[1].Value);
                    }
                    catch { pageMax = 100; }
                }
                foreach (HtmlElement link in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = link.GetAttribute("href");
                        if (strLink.StartsWith("http://www.xzhouse.com.cn/ItemSellInfo.aspx?ID=") && !Fn.IsExistEstUrl(strLink) && !UrlInLinkList(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText));
                    }
                    catch { }
                }

                this.NavToNextPage();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("徐州");
                me.SourceLink = linkCurrent.Key;
                me.EstNum = me.SourceLink.Split('=')[1].Split('&')[0];
                me.EstName = Document.GetElementById("lblItemName").InnerText;
                me.DistrictName = Document.GetElementById("lblDist").InnerText;
                me.EstAddr = Document.GetElementById("lblItemSite").InnerText;
                me.PlateName = Document.GetElementById("lblZone").InnerText;
                me.Developer = Document.GetElementById("lblCorpName").InnerText;
                me.EstUsage = Document.GetElementById("lblPlUse").InnerText;
                me.ProjectApprovals = Document.GetElementById("lblItAcPaper").InnerText;
                me.ProjectPermitNum = Document.GetElementById("lblItPlLicNum").InnerText;
                me.LandPermitNum = Document.GetElementById("lblItLaUseLic").InnerText;
                me.ConstructionPermitNum = Document.GetElementById("lblIStruLicNum").InnerText;
                me.GroundAreaAll = double.Parse(Document.GetElementById("lblLaAcre").InnerText);
                me.GroundPermitNum = Document.GetElementById("lblLaCertNum").InnerText;
                me.CompletionTime = Document.GetElementById("lblWoFinDate").InnerText;
                me.CountAll = int.Parse(Document.GetElementById("lblSumSuits").InnerText);
                me.BuildingAreaAll = double.Parse(Document.GetElementById("lblSumArea").InnerText);
                me.AreaSold = double.Parse(Document.GetElementById("lblSelledArea").InnerText);
                me.Save();
            }
            catch { }

            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            if (page > pageMax && pageMax > 0) { this.NavToNext(); return; }
            this.Navigate("http://www.xzhouse.com.cn/houseSearch.aspx?page=" + page);
        }
    }
}
