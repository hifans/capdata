using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class NanTong_Estate : CapData.FormBrowser
    {
        public NanTong_Estate()
        {
            InitializeComponent();
        }

        int pageMax = -1;
        int page = 0;
        private void NanTong_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != Document.Url.ToString()) return;
            if (e.Url.ToString().StartsWith("http://newhouse.ntfdc.net/house_search.aspx?pageindex="))
            {
                if (pageMax < 0)
                {
                    HtmlElement divPage = null;
                    foreach (HtmlElement item in Document.GetElementsByTagName("div"))
                    {
                        try
                        {
                            string cls = item.GetAttribute("classname");
                            if (cls == "multipage") { divPage = item; break; }
                        }
                        catch { }
                    }
                    if (divPage != null)
                    {
                        try { pageMax = int.Parse(divPage.Children[divPage.Children.Count - 2].InnerText.Trim()); }
                        catch { pageMax = 10; }
                    }
                    else pageMax = 10;
                }

                HtmlElement ulLP = null;
                foreach (HtmlElement item in Document.GetElementsByTagName("div"))
                {
                    try
                    {
                        string cls = item.GetAttribute("classname");
                        if (cls == "lplist") { ulLP = item.Children[0]; break; }
                    }
                    catch { }
                }

                if (ulLP == null) { this.NavToNext(); return; }
                foreach (HtmlElement li in ulLP.Children)
                {
                    try
                    {
                        HtmlElement link = li.Children[1].Children[0];
                        string strLink = link.GetAttribute("href");
                        if (!UrlInLinkList(strLink) && !Fn.IsExistEstUrl(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText));
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("南通");
                me.SourceLink = linkCurrent.Key;
                me.PublicName = linkCurrent.Value.ToString();
                me.EstNum = me.SourceLink.Split('=')[1];

                HtmlElement house_info2 = GetElementByClassName("house_info2", "div");
                
                HtmlElement info = house_info2.Children[0].Children[0];
                me.EstName = info.Children[0].InnerText.Split('：')[1];
                me.DecorationLevel = info.Children[1].InnerText.Split('：')[1];
                me.CountAll = int.Parse(info.Children[2].InnerText.Split('：')[1].Replace("套",""));
                me.Developer = info.Children[4].InnerText.Split('：')[1];
                me.DistrictName = info.Children[5].Children[1].InnerText;
                me.EstAddr = info.Children[6].InnerText.Split('：')[1];

                me.SaleTele = house_info2.Children[1].Children[0].Children[2].InnerText;

                info = house_info2.Children[3].Children[0];
                me.OpeningTime = info.Children[0].InnerText.Split('：')[1];
                me.HandoverTime = info.Children[1].InnerText.Split('：')[1];
                me.GroundAreaAll = double.Parse(info.Children[2].InnerText.Split('：')[1].Replace("平方米", ""));
                me.BuildingAreaAll = double.Parse(info.Children[3].InnerText.Split('：')[1].Replace("平方米", ""));
                me.PlotRatio = info.Children[4].InnerText.Split('：')[1];
                me.GreeningRate = info.Children[5].Children[1].InnerText;
                me.PropertyFee = info.Children[6].InnerText.Split('：')[1];
                me.PropertyCompany = info.Children[7].InnerText.Split('：')[1];
                me.EstUsage = info.Children[8].InnerText.Split('：')[1];
                me.SaleAddr = info.Children[9].InnerText.Split('：')[1];
                me.SaleAgent = info.Children[10].InnerText.Split('：')[1];
                me.Builder = info.Children[11].InnerText.Split('：')[1];

                info = GetElementByClassName("house_project", "div");
                if (info != null)
                {
                    try
                    {
                        info = info.Children[1];
                        me.EstIntroduction = info.Children[0].GetAttribute("data-value");
                        me.SrdFacility = info.Children[1].GetAttribute("data-value");
                        me.Traffic = info.Children[2].GetAttribute("data-value");
                        me.DevIntroduction = info.Children[4].GetAttribute("data-value");
                        me.Remark = info.Children[5].GetAttribute("data-value");
                    }
                    catch { }
                }
                me.Save();
            }
            catch { }

            this.NavToNext();
        }

        
        private void NavToNextPage()
        {
            page++;
            if (pageMax > 0 && pageMax < page) { this.NavToNext(); return; }
            this.Navigate("http://newhouse.ntfdc.net/house_search.aspx?pageindex=" + page);
        }
    }
}
