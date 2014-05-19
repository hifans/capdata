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
    public partial class NanTong_Ground : CapData.FormBrowser
    {
        public NanTong_Ground()
        {
            InitializeComponent();
        }

        int page = 0;

        int cityId = Fn.GetCityIdByName("南通");

        string q = "$" + Fn.GetRandomStr() + "$";

        private void NanTong_Ground_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Browser.Url) return;
            if (e.Url.ToString().StartsWith("http://www.ntgt.gov.cn/gtzygl/app/gpcrController/listGpcrBySx?"))
            {
                bool flag = false;
                foreach (HtmlElement el in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string url = el.GetAttribute("href").Trim();
                        if (!url.StartsWith("http://www.ntgt.gov.cn/gtzygl/app/gpcrController/getView0/")) continue;
                        flag = true;
                        string strGrdNum = el.Parent.Parent.FirstChild.InnerText.Trim();
                        if (Fn.GetDT_PgSQL("select * from ground_info where cityid='" + cityId + "' and groundnum=" + q + strGrdNum + q + " and clinchprice>0").Rows.Count > 0) continue;
                        listLink.Add(new KeyValuePair<string, object>(url, strGrdNum));
                        
                    }
                    catch { }
                   
                }
                if (flag) this.NavToNextPage();
                else this.NavToNext();
                return;
            }

            try
            {
                HtmlElement div = null;
                foreach (HtmlElement el in Document.GetElementsByTagName("p"))
                {
                    if (el.InnerText == "基本情况")
                    {
                        div = el.NextSibling;
                        break;
                    }
                }

                Model_Ground mg = new Model_Ground("南通");
                mg.GroundNum = this.linkCurrent.Value.ToString();
                mg.SourceLink = this.linkCurrent.Key;

                bool flagSold = false;
                string strImageLink = null;
                foreach (HtmlElement p in div.Children)
                {
                    if (p.TagName.ToLower() != "p") continue;
                    string txt=null;
                    try { txt = p.InnerText.Trim(); }
                    catch { continue; }
                    int index = txt.IndexOf("：");
                    if (index < 0) index = txt.IndexOf(":");
                    if (index < 0) continue;
                    string name = txt.Substring(0, index);
                    string val = null;
                    try { val = txt.Substring(index + 1); }
                    catch { }

                    if (val == null || val.Trim() == "") continue;
                    val = val.Trim();
                    #region 计算上市字段值
                    try
                    {
                        switch (name)
                        {
                            case "地块座落":
                                mg.GLocation = val;
                                break;

                            case "用地性质":
                                mg.GUsage = val;
                                break;

                            case "地块面积":
                                mg.GArea = Convert.ToDouble(val.Replace("平方米", ""));
                                break;

                            case "起 挂 价":
                                mg.StartingPrice = Convert.ToDouble(val.Replace("元/平方米", "").Trim()) * mg.GArea;
                                break;

                            case "容 积 率":
                                mg.PlotRatio = val;
                                break;

                            case "建筑密度":
                                mg.BuildingDensity = val;
                                break;

                            case "绿 化 率":
                                mg.GreeningRate = val;
                                break;

                            case "开发程度":
                                mg.DevelopLevel = val;
                                break;

                            case "备　　注":
                                mg.Remark = val;
                                break;

                            case "报价增幅":
                                mg.IncreaseRate = val;
                                break;

                            case "起始时间":
                                mg.ListingDate = Convert.ToDateTime(val);
                                break;

                            case "终止时间":
                                mg.ClinchDate = Convert.ToDateTime(val);
                                break;

                            case "挂牌情况":
                                if (val == "成交") flagSold = true;
                                else mg.ClinchDate = Convert.ToDateTime("1900-01-01");
                                break;

                        }
                    }
                    catch { }
                    #endregion
                }

                try
                {
                    strImageLink = div.Parent.FirstChild.Children[1].GetAttribute("src");
                }
                catch { }

                if (flagSold)
                {
                    div = div.GetElementsByTagName("div")[0];
                    mg.Buyer = div.Children[0].InnerText.Split('：')[1].Trim();
                    mg.ClinchPrice = Convert.ToDouble(div.Children[1].InnerText.Split('：')[1].Replace("元/平方米", "")) * mg.GArea;
                    mg.Save(true, null);
                }
                else
                {
                    
                    mg.Save();
                }

                
                Model_Ground_File mgf = new Model_Ground_File(mg.CityId, mg.GroundNum);
                if (strImageLink != null && strImageLink != "" && mgf.GroundId > 0)
                {
                    try
                    {
                        string strFileName = mg.CityId + "_ground_" + mg.GroundNum+".jpg";
                        if (!Fn.IsExistGroundFile(strFileName, FileType.Image))
                        {
                            bool b = Fn.DownloadFile(strImageLink, strFileName, FileType.Image);
                            if (b)
                            {
                                mgf.FileName = strFileName;
                                mgf.FileTitle = "土地位置图";
                                mgf.FileType = FileType.Image;
                                mgf.Save();
                            }
                        }

                    }
                    catch { }
                }
                
                
            }
            catch { }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            this.page++;
            this.Navigate("http://www.ntgt.gov.cn/gtzygl/app/gpcrController/listGpcrBySx?nowPage=" + page);
        }
    }
}
