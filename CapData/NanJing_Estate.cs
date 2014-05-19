using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class NanJing_Estate : CapData.FormBrowser
    {
        public NanJing_Estate()
        {
            InitializeComponent();
        }

        string strUrl1 = "";
        string strUrl2 = "";
        string strUrl3 = "";

        Model_Estate me = null;

        private void NanJing_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://newhouse.njhouse.com.cn/kpgg/");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString() == "http://newhouse.njhouse.com.cn/kpgg/")
            {
                foreach (HtmlElement link in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strUrl = link.GetAttribute("href");
                        if (strUrl.StartsWith("http://newhouse.njhouse.com.cn/detail.php?id=") && !this.UrlInLinkList(strUrl) && !Fn.IsExistEstUrl(strUrl))
                        {
                            listLink.Add(new KeyValuePair<string, object>(strUrl, link.InnerText));
                        }
                    }
                    catch { }
                }
                this.NavToNext();
                return;
            }
            
            
            if (e.Url.ToString().StartsWith("http://newhouse.njhouse.com.cn/detail.php?id="))
            {
                try
                {
                    me = new Model_Estate("南京");

                    HtmlElement info1 = null;
                    HtmlElement info2 = null;
                    HtmlElement info3 = null;

                    foreach (HtmlElement td in Document.GetElementsByTagName("td"))
                    {
                        try
                        {
                            string tmp = td.InnerText.Replace(" ", "").Trim();
                            if (tmp == "项目地址：") info1 = td.Parent.Parent;
                            else if (tmp == "物业管理公司：") info2 = td.Parent.Parent;
                            else if (tmp == "周边环境：") info3 = td.Parent.Parent;
                        }
                        catch { }
                    }

                    me.SourceLink = linkCurrent.Key;
                    me.EstName = linkCurrent.Value.ToString();
                    me.EstAddr = info1.Children[0].Children[1].InnerText;
                    me.EstUsage = info1.Children[1].Children[1].InnerText;
                    me.DistrictName = info1.Children[2].Children[1].InnerText;

                    me.PropertyCompany = info2.Children[0].Children[1].InnerText;
                    me.Designer = info2.Children[1].Children[1].InnerText;
                    me.DecorationLevel = info2.Children[4].Children[1].InnerText;

                    me.SrdFacility = info3.Children[0].Children[1].InnerText;
                    me.EstFacility = info3.Children[1].Children[1].InnerText;
                    me.EstIntroduction = info3.Children[2].Children[1].InnerText;

                    HtmlElementCollection ifam = Document.GetElementsByTagName("iframe");
                    strUrl1 = ifam[2].GetAttribute("src");
                    strUrl2 = ifam[3].GetAttribute("src");
                    me.EstNum = strUrl1.Split('=')[1].Split('&')[0];
                }
                catch { me.Save(); this.NavToNext(); return; }

                this.Navigate(strUrl1);
            }
            else if (e.Url.ToString() == strUrl1)
            {
                try
                {
                    HtmlElement info = Document.Body.Children[0].Children[0];
                    me.Developer = info.Children[0].Children[1].InnerText;
                    me.SaleAgent = info.Children[1].Children[1].InnerText;
                    me.OpeningTime = info.Children[2].Children[3].InnerText;
                    //me.GroundPermitNum = info.Children[3].Children[1].InnerText;
                    //me.LandPermitNum = info.Children[3].Children[3].InnerText;
                    //me.ProjectPermitNum = info.Children[4].Children[1].InnerText;
                    //me.ConstructionPermitNum = info.Children[4].Children[3].InnerText;
                    this.Navigate(strUrl2);
                }
                catch { me.Save(); this.NavToNext(); return; }

            }
            else if (e.Url.ToString() == strUrl2)
            {
                try
                {
                    this.strUrl3 = "http://www.njhouse.com.cn/" + Document.GetElementsByTagName("iframe")[0].GetAttribute("src").Replace("../../", "");
                    this.Navigate(strUrl3);
                }
                catch { me.Save(); this.NavToNext(); return; }
            }
            else if (e.Url.ToString() == strUrl3)
            {
                try
                {
                    HtmlElement info = Document.Body.Children[0].Children[0];
                    me.CountAll = int.Parse(info.Children[0].Children[1].InnerText.Replace("套", ""));
                    me.BuildingAreaAll = double.Parse(info.Children[1].Children[1].InnerText.Replace("m2", ""));
                }
                catch { }
                me.Save(); this.NavToNext(); return;
            }
            else { me.Save(); this.NavToNext(); return; }
            
        }
    }
}
