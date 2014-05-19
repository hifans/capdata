using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class SuZhou_Estate : CapData.FormBrowser
    {
        int page = 0;
        public SuZhou_Estate()
        {
            InitializeComponent();
        }

        private void SuZhou_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://newhouse.suzhou.soufun.com/house/"))
            {
                HtmlElement list = GetElementByClassName("searchList", "div");
                if (list == null) { this.NavToNext(); return; }
                var flag = false;
                foreach (HtmlElement el in list.Children)
                {
                    if (el.GetAttribute("classname") != "searchListNoraml") continue;
                    try
                    {
                        string strLink = el.Children[1].Children[0].GetAttribute("href");
                        string strUsage = el.Children[2].Children[3].Children[0].Children[2].InnerText.Replace("[", "").Replace("]", "").Trim();
                        if (!Fn.IsExistEstUrl(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, strUsage));
                        flag = true;
                    }
                    catch { }
                }

                if (flag) this.NavToNextPage(); else this.NavToNext();
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("苏州");
                me.SourceLink = linkCurrent.Key;
                me.EstNum = Fn.GetMD5(linkCurrent.Key);
                me.EstName = Document.GetElementById("newsuzhouxq_B01_49").InnerText;
                me.EstUsage = linkCurrent.Value.ToString();
                me.EstAddr = "";

                HtmlElement intro = GetElementByClassName("zxseversubnav", "div");
                if (intro!=null && intro.NextSibling!=null && intro.NextSibling.NextSibling!=null)
                {
                    intro = intro.NextSibling.NextSibling;
                    intro = intro.Children[0];
                    if (intro!=null) me.EstIntroduction = intro.InnerText.Replace("简  介", "").Trim();
                }
                HtmlElement basicInfo = GetElementByClassName("basicinform", "div");
                if (basicInfo != null)
                {
                    try
                    {
                        basicInfo = basicInfo.Children[1].Children[0];

                        me.PropertyFee = basicInfo.Children[2].Children[0].Children[0].InnerText.Replace("物 业 费", "").Trim();
                        me.OpeningTime = basicInfo.Children[3].Children[0].InnerText.Replace("开盘时间","").Trim();
                        me.HandoverTime = basicInfo.Children[4].Children[0].InnerText.Replace("入住时间", "").Trim();
                        me.Developer = basicInfo.Children[5].Children[0].Children[0].InnerText.Replace("开 发 商", "").Trim();
                        me.SaleAddr = basicInfo.Children[6].Children[0].Children[0].InnerText.Replace("售楼地址","").Trim();
                        
                    }
                    catch { }
                }

                HtmlElement detailInfo = Document.GetElementById("newsuzhouxq_B02_28");
                if (detailInfo != null)
                {
                    detailInfo = detailInfo.Children[0];
                    if (detailInfo != null)
                    {
                        me.GroundAreaAll = double.Parse(detailInfo.Children[1].Children[0].InnerText.Split(' ')[1].Replace("平方米", ""));
                        me.BuildingAreaAll = double.Parse(detailInfo.Children[1].Children[1].InnerText.Split(' ')[1].Replace("平方米", ""));
                        me.PlotRatio = detailInfo.Children[2].Children[0].InnerText.Replace("容 积 率","").Trim().Split(' ')[0];
                        me.GreeningRate = detailInfo.Children[2].Children[1].InnerText.Replace("绿 化 率", "").Trim().Split(' ')[0];
                        me.PropertyCompany = detailInfo.Children[4].Children[0].InnerText.Replace("物业管理公司", "").Trim();
                        me.Traffic = detailInfo.Children[5].InnerText.Replace("交通状况", "").Trim();
                    }
                }

                me.Save();
            }
            catch { }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            //if (page > 5) { this.NavToNext(); return; }
            this.Navigate("http://newhouse.suzhou.soufun.com/house/%CB%D5%D6%DD_________________" + page + "_.htm");
        }
    }
}
