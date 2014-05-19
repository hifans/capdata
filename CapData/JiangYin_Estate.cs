using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class JiangYin_Estate : CapData.FormBrowser
    {
        public JiangYin_Estate()
        {
            InitializeComponent();
        }

        int page = 0;

        private void JiangYin_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://www.jy510.com/sinfo-page-"))
            {
                HtmlElement list = null;
                try
                {
                    list = Document.GetElementById("searchList").Children[0].Children[0];
                }
                catch { this.NavToNext(); return; }

                foreach (HtmlElement item in list.Children)
                {
                    try
                    {
                        HtmlElement link = item.Children[1].Children[0].Children[0];
                        string strLink = link.GetAttribute("href");
                        if (!this.UrlInLinkList(strLink))
                        {
                            if (!Fn.IsExistEstUrl(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText));
                        }
                        else
                        {
                            this.NavToNext(); return;
                        }
                    }
                    catch { }
                }

                this.NavToNextPage();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("江阴");
                HtmlElement info1 = null;
                HtmlElement info2 = null;
                HtmlElement info3 = null;
                HtmlElement info4 = null;
                foreach (HtmlElement item in Document.GetElementsByTagName("div"))
                {
                    string cls = item.GetAttribute("classname");
                    if (cls == "house_information") info1 = item;
                    else if (cls == "generalize_con") info2 = item;
                    else if (cls == "nearby_table") info4 = item.Children[1].Children[1];
                    if (info1 != null && info2 != null && info4 != null) break;
                    
                }

                foreach (HtmlElement item in Document.GetElementsByTagName("dl"))
                {
                    string cls = item.GetAttribute("classname");
                    if (cls == "generalize_table") { info3 = item; break; }
                }

                me.SourceLink = linkCurrent.Key;
                me.EstNum = me.SourceLink.Replace("http://www.jy510.com/sinfo-view-", "").Split('.')[0];
                me.EstName = linkCurrent.Value.ToString();

                if (info1 != null)
                {
                    me.EstAddr = info1.Children[0].Children[0].Children[1].InnerText;
                    me.DistrictName = info1.Children[0].Children[1].Children[1].InnerText;
                    me.PlateName = info1.Children[1].Children[1].Children[1].InnerText;
                    me.SaleAddr = info1.Children[2].Children[0].Children[1].InnerText;
                    me.DecorationLevel = info1.Children[2].Children[1].Children[1].InnerText;
                    me.EstUsage = info1.Children[3].Children[0].Children[1].InnerText;
                    me.SaleAgent = info1.Children[3].Children[1].Children[1].InnerText;
                    me.SaleTele = info1.Children[4].Children[0].Children[0].Children[1].InnerText;
                    me.Developer = info1.Children[5].Children[0].Children[1].InnerText.Replace("(查看)","").Trim();
                }

                if (info2 != null)
                {
                    me.EstIntroduction = info2.InnerText.Trim();
                }

                if (info3 != null)
                {
                    me.CountAll = int.Parse(info3.Children[1].InnerText.Replace("套", ""));
                    me.BuildingAreaAll = double.Parse(info3.Children[5].InnerText.Replace("万平方米", ""))*10000;
                    me.GroundAreaAll = double.Parse(info3.Children[7].InnerText.Replace("亩", "")) * 666.6666667;
                    me.PlotRatio = info3.Children[9].InnerText;
                    me.GreeningRate = info3.Children[11].InnerText;
                    me.PropertyCompany = info3.Children[13].InnerText;
                    me.PropertyFee = info3.Children[17].InnerText;
                }

                if (info4 != null)
                {
                    me.Traffic = info4.Children[1].InnerText;
                    me.SrdFacility = info4.Children[2].Children[1].InnerText;
                }

                me.Save();
            }
            catch { }

            this.NavToNext();

        }

        private void NavToNextPage()
        {
            this.page++;
            //if (page > 1) { this.NavToNext(); return; }
            this.Navigate("http://www.jy510.com/sinfo-page-" + page + ".html");
        }
    }
}
