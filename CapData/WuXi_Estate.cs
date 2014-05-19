using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CapData
{
    public partial class WuXi_Estate : FormBrowser
    {
        public WuXi_Estate()
        {
            InitializeComponent();
        }
        string url = "http://www.wxhouse.com/search/house.html?PageIndex=";
        Regex regLink = new Regex("http://www.wxhouse.com/newhouse/([\\d]+)\\.html");
        Regex regNumber = new Regex("\\d+");

        int page = 0;
        int pageMax = -1;

        private void WuXi_Estate_Load(object sender, EventArgs e)
        {
            //this.needProxy = true;
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();

            //this.listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/newhouse/521.html", new string[] { "521", "新区哥伦布广场" }));
            //this.NavToNext();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            Model_Estate me = new Model_Estate("无锡");

            if (e.Url.ToString().StartsWith(url))
            {
                HtmlElementCollection tableList = Document.Body.Children;
                
                if (pageMax == -1)
                {
                    HtmlElement divNumList = GetElementByClassName("numList", "div");
                    Regex regTmp = new Regex("^第 ([\\d]+) 页$");
                    foreach (HtmlElement item in divNumList.GetElementsByTagName("a"))
                    {
                        Match mTmp = regTmp.Match(item.GetAttribute("title"));
                        if (mTmp.Success)
                        {
                            int intTmp = int.Parse(mTmp.Groups[1].Value);
                            if (intTmp > pageMax) pageMax = intTmp;
                        }
                    }
                }                
                foreach (HtmlElement table in tableList)
                {
                    if (table.TagName.ToLower() != "table") continue;
                    try
                    {
                        HtmlElement el =table.GetElementsByTagName("a")[1];
                        string link = el.GetAttribute("href");
                        Match mLink = regLink.Match(link);
                        if (!mLink.Success) continue;
                        string EstNum = mLink.Groups[1].Value;
                        string[] tmp = {EstNum, el.InnerText.Trim() };
                        bool isExist = me.IsExist(EstNum);
                        if (!listLink.Contains(new KeyValuePair<string, object>(link, tmp))&&!isExist) listLink.Add(new KeyValuePair<string, object>(link, tmp));
                        if (isExist) pageMax = Math.Min(page + 5, pageMax);
                    }
                    catch { continue; }
                }
                
                this.NavToNextPage();
                return;
            }

            
            me.EstNum = (linkCurrent.Value as string[])[0];
            me.EstName = (linkCurrent.Value as string[])[1];
            try
            {
                HtmlElement panel = Document.GetElementById("divmenuinfo1").Children[0].Children[0].Children[0].Children[2];

                try { me.SaleTele = panel.Children[5].Children[0].Children[0].Children[1].Children[0].InnerText; } catch { }

                try
                {
                    string[] tmp=panel.Children[0].Children[0].Children[0].Children[1].InnerText.Replace(" ", "").Replace("　", "").Trim().Trim('）').Split('（');
                    me.DistrictName = tmp[0];
                    me.PlateName = tmp[1];
                }
                catch { }

                try
                {
                    HtmlElement tbody = panel.Children[1].Children[0];
                    me.EstAddr = tbody.Children[0].Children[1].InnerText;
                    me.OpeningTime = tbody.Children[1].Children[1].InnerText;
                    me.HandoverTime = tbody.Children[2].Children[1].InnerText;
                }
                catch { }

                try
                {
                    HtmlElement tbody = panel.Children[2].Children[0];
                    me.EstUsage = tbody.Children[0].Children[1].InnerText;
                }
                catch { }

                try { me.Developer = panel.Children[3].Children[0].Children[0].Children[1].InnerText; }
                catch { }

                try
                {
                    HtmlElement tbody = Document.GetElementById("hs_sellNum").Parent.Parent;
                    try {
                        MatchCollection mc = regNumber.Matches(tbody.Children[0].Children[1].InnerText);
                        foreach (Match m in mc)
                        {
                            me.CountAll += int.Parse(m.Value);
                        }
                        //me.CountAll = int.Parse(tbody.Children[0].Children[1].InnerText); 
                    }
                    catch { }

                    try
                    {
                        MatchCollection mc = regNumber.Matches(tbody.Children[0].Children[3].InnerText);
                        foreach (Match m in mc)
                        {
                            me.CountSold += int.Parse(m.Value);
                        }
                        me.CountSold = me.CountAll - me.CountSold;
                        if (me.CountSold < 0) me.CountSold = 0;
                    }
                    catch { }

                    me.GreeningRate = tbody.Children[1].Children[1].InnerText;
                    me.PlotRatio = tbody.Children[1].Children[3].InnerText;
                    me.PropertyCompany = tbody.Children[2].Children[1].InnerText;
                    me.PropertyFee = tbody.Children[2].Children[3].InnerText;
                    me.SaleAgent = tbody.Children[3].Children[1].InnerText;
                    me.GroundName = tbody.Children[3].Children[3].InnerText;

                    try
                    {
                        tbody = tbody.Parent.NextSibling.Children[0];
                        me.SaleAddr = tbody.Children[0].Children[1].InnerText;
                        me.Traffic = tbody.Children[1].Children[1].InnerText;
                    }
                    catch { }
                }
                catch { }
                

                me.EstIntroduction = Document.GetElementById("tabtabm21").InnerText;
                me.DevIntroduction = Document.GetElementById("tabtabm22").InnerText;
                me.EstFacility = Document.GetElementById("tabtabm23").InnerText;
                me.SrdFacility = Document.GetElementById("tabtabm24").InnerText;

                me.SourceLink = linkCurrent.Key;
                int estId= me.Save(true);

                //开始计算报价信息
                Model_Estate_Price mep = new Model_Estate_Price(estId);
                mep.SourceLink = "http://www.wxhouse.com";
                mep.SourceName = "无锡房地产市场网";
                panel = Document.GetElementById("divmenuinfo3").Children[0].Children[0].Children[0].Children[0].Children[0].Children[0];
                Regex regPrice = new Regex("(\\d+(元/㎡)?[-~])?(\\d+)元/㎡");
                foreach (HtmlElement tr in panel.Children)
                {
                    try { mep.PriceDate = DateTime.Parse(tr.Children[0].InnerText.Trim()); }
                    catch { continue; }
                    if (tr.Children[1].InnerText == null) continue;
                    string[] arrInfo = tr.Children[1].InnerText.Replace(";","；").Split('；');
                    foreach (string info in arrInfo)
                    {
                        try
                        {
                            if (info.IndexOf('：') > 0)
                            {
                                mep.EstUsage = info.Substring(0, info.IndexOf('：'));
                            }
                            else mep.EstUsage = "商品房";

                            MatchCollection mc = regPrice.Matches(info);
                            if (mc.Count == 0) continue;
                            mep.PriceMin = 0;
                            mep.PriceMax = 0;
                            foreach (Match m in mc)
                            {
                                int min = 0;
                                try
                                {
                                    min = int.Parse(m.Groups[1].Value.Replace("-", "").Replace("~", "").Replace("元/㎡", ""));
                                }
                                catch { }
                                int max = int.Parse(m.Groups[3].Value);

                                if ((mep.PriceMin > min || mep.PriceMin == 0) && min > 0) mep.PriceMin = min;
                                if (mep.PriceMin > max || mep.PriceMin == 0) mep.PriceMin = max;
                                if (mep.PriceMax < max) mep.PriceMax = max;

                            }
                            mep.Remark = info;
                            mep.Save(false);
                        }
                        catch { }
                    }
                }
            }
            catch(Exception ex) {
                Program.MainForm.AddMessage("错误！无锡楼盘数据抓取错误。链接："+me.SourceLink+"。楼盘Id：" + me.EstNum + "。错误消息："+ex.Message);
            }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            if (pageMax > 0 && page > pageMax)
            {
                //添加库中已存在的未开盘或可售量大于0的数据

                //DataTable dt = Fn.GetDT_PgSQL("select sourcelink, estnum, estname,countall, countsold from estate_info where cityid=" + Fn.GetCityIdByName("无锡").ToString() + " and (countall=0 or countall>countsold) and sourcelink like 'http://www.wxhouse.com/newhouse/%'");
                //foreach (DataRow dr in dt.Rows)
                //{
                //    string link = dr[0].ToString();
                //    string[] tmp = { dr[1].ToString(), dr[2].ToString() };
                //    if (!listLink.Contains(new KeyValuePair<string, object>(link, tmp))) listLink.Add(new KeyValuePair<string, object>(link, tmp));
                //}

                this.NavToNext();
                return;
            }

            this.Navigate(url + page);
        }
    }
}
