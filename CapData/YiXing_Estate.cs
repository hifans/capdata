using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class YiXing_Estate : CapData.FormBrowser
    {
        public YiXing_Estate()
        {
            InitializeComponent();
        }

        int page = 0;
        int cityId = Fn.GetCityIdByName("宜兴");

        string q = "$" + Fn.GetRandomStr() + "$";
        private void YiXing_Estate_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://beian.yxhouse.net/showConditionBuild.action"))
            {
                try
                {
                    if (Document.GetElementById("page").GetAttribute("value") != page.ToString()||page>2)
                    {
                        this.NavToNext(); return;
                    }
                }
                catch { this.NavToNext(); }

                HtmlElement div = null;
                foreach (HtmlElement el in Document.GetElementsByTagName("div"))
                {
                    if (el.GetAttribute("classname").ToLower() == "htba_center_left_1")
                    {
                        div = el; break;
                    }
                }
                if (div == null) { this.NavToNext(); return; }

                foreach (HtmlElement el in div.GetElementsByTagName("a"))
                {
                    try
                    {
                        string url = el.GetAttribute("href");
                        if (url.StartsWith("http://beian.yxhouse.net/buildDetail.action?blid="))
                        {
                            string prjid = url.Split('=')[1];
                            if (Fn.GetDT_PgSQL("select * from estate_info where cityid=" + cityId + " and estnum=" + q + prjid + q).Rows.Count > 0) continue;
                            listLink.Add(new KeyValuePair<string, object>(url, prjid));  
                        }
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
                me.SourceLink = linkCurrent.Key;
                foreach (HtmlElement div in Document.GetElementsByTagName("div"))
                {
                    try
                    {
                        if (div.GetAttribute("classname") != "sslp_lpxx_table_center") continue;
                        string name = div.Children[0].InnerText.Trim();
                        switch (name)
                        {
                            case "项目暂定名":
                                me.PublicName = div.Children[1].InnerText.Trim();
                                break;

                            case "项目现定名":
                                me.EstName = div.Children[1].InnerText.Trim();
                                break;

                            case "开发商":
                                me.Developer = div.Children[1].InnerText.Trim();
                                break;

                            case "坐落":
                                me.EstAddr = div.Children[1].InnerText.Trim();
                                break;

                            case "行政区":
                                me.DistrictName = div.Children[1].InnerText.Trim();
                                break;

                            case "区位":
                                me.PlateName = div.Children[1].InnerText.Trim();
                                break;

                            case "总建筑面积":
                                me.BuildingAreaAll = Convert.ToDouble(div.Children[1].InnerText.Trim().Replace("（单位：平方米）",""));
                                break;

                            case "立项批文":
                                me.ProjectApprovals = div.Children[1].InnerText.Trim();
                                break;

                            case "规划许可证号":
                                me.ProjectPermitNum = div.Children[1].InnerText.Trim();
                                break;

                            case "土地证号":
                                me.GroundPermitNum = div.Children[1].InnerText.Trim();
                                break;

                            case "施工许可证号":
                                me.ConstructionPermitNum = div.Children[1].InnerText.Trim();
                                break;

                            case "总用地":
                                me.GroundAreaAll = Convert.ToDouble(div.Children[1].InnerText.Trim().Replace("（单位：平方米）",""));
                                break;

                            case "当期用地":
                                me.GroundAreaCurrent = Convert.ToDouble(div.Children[1].InnerText.Trim().Replace("（单位：平方米）", ""));
                                break;

                            case "预计竣工日期":
                                me.CompletionTime = div.Children[1].InnerText.Trim();
                                break;

                            case "预售总面积":
                                me.BuildingAreaAll = Convert.ToDouble(div.Children[1].InnerText.Trim());
                                break;

                            case "总套数":
                                me.CountAll = Convert.ToInt32(div.Children[1].InnerText.Trim());
                                break;

                            case "代销公司":
                                me.SaleAgent = div.Children[1].InnerText.Trim();
                                break;

                            case "项目销售地点":
                                me.SaleAddr = div.Children[1].InnerText.Trim();
                                break;

                            case "电话":
                                me.SaleTele = div.Children[1].InnerText.Trim();
                                break;

                            case "物业公司":
                                me.PropertyCompany = div.Children[1].InnerText.Trim();
                                break;

                            case "物管费":
                                me.PropertyFee = div.Children[1].InnerText.Trim();
                                break;
                        }
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
            this.Navigate("http://beian.yxhouse.net/showConditionBuild.action?page="+page);
        }
    }
}
