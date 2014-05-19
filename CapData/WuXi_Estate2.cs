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
    public partial class WuXi_Estate2 : CapData.FormBrowser
    {
        public WuXi_Estate2()
        {
            InitializeComponent();
        }

        Regex regTimeSpan = new Regex("([\\d]{4})年(\\d)季度");

        private bool flagSearch = false;

        private void WuXi_Estate2_Load(object sender, EventArgs e)
        {
            //if (DateTime.Today.DayOfWeek != DayOfWeek.Saturday) { this.Close(); return; }
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://58.215.18.133:82/LandPriceWX/login.aspx");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = e.Url.ToString();
            if (url == "http://58.215.18.133:82/LandPriceWX/login.aspx")
            {
                Document.GetElementById("txtusername").SetAttribute("value", "PGJG");
                Document.GetElementById("txtpassword").SetAttribute("value", "123456");
                Document.GetElementById("Submit1").InvokeMember("click");
                return;
            }

            if (url == "http://58.215.18.133:82/LandPriceWX/index_main.aspx")
            {
                this.Navigate("http://58.215.18.133:82/LandPriceWX/mapmain.aspx?ss=0,0,0,1");
                return;
            }

            if (url == "http://58.215.18.133:82/LandPriceWX/mapquery.aspx?ss=0,0,0,1")
            {
                HtmlDocument doc = Document.Window.Frames[1].Document;
                HtmlElement tb = doc.GetElementById("table4").Children[0];
                tb.Children[tb.Children.Count - 1].Children[0].Children[0].InvokeMember("click");
                this.flagSearch = true;
                return;
            }

            if (flagSearch && url.StartsWith("http://58.215.18.133:82/LandPriceWX/mapresult_lp1.aspx"))
            {
                HtmlDocument doc = Document.Window.Frames[2].Document;
                foreach (HtmlElement el in doc.GetElementsByTagName("a"))
                {
                    if (el.InnerText!=null&&el.InnerText.Trim()=="详细")
                    {
                        listLink.Add(new KeyValuePair<string, object>(el.GetAttribute("href"), null));
                    }
                }
                this.NavToNext();
                return;
            }

            if (url.StartsWith("http://58.215.18.133:82/LandPriceWX/lp_zpg_edit.aspx?mode=show&id="))
            {
                try
                {
                    string strEstName = Document.GetElementById("LP_MC").GetAttribute("value");
                    HtmlElement tbPermit = Document.GetElementById("dg_xk").Children[0];
                    string strPermitId = tbPermit.Children[1].Children[1].InnerText.Trim();
                    string strPermitId1 = strPermitId;
                    if (strPermitId.IndexOf("(") >= 0)
                    {
                        strPermitId1 = strPermitId.Replace("(", "（").Replace(")", "）");
                    }
                    else if (strPermitId.IndexOf("（") >= 0)
                    {
                        strPermitId1 = strPermitId.Replace("（", "(").Replace("）", ")");
                    }
                    int cityId = Fn.GetCityIdByName("无锡");
                    DataTable dt = Fn.GetDT_MySQL("select estnum from permitinfo where (permitnum='"+strPermitId+"' or permitnum='"+strPermitId1+"') and cityid="+cityId);
                    string strEstNum = null;
                    if (dt.Rows.Count > 1)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                string tmp = Fn.GetObj_PgSQL("select estname from estate_info where cityid=" + cityId + " and estnum='" + dr[0].ToString() + "'").ToString();
                                if (strEstName.IndexOf(tmp) >= 0 || tmp.IndexOf(strEstName) >= 0)
                                {
                                    strEstNum = dr[0].ToString();
                                    break;
                                }
                            }
                            catch { }
                        }
                    }
                    else strEstNum = dt.Rows[0][0].ToString();

                    int estId = int.Parse(Fn.GetObj_PgSQL("select id from estate_info where cityid="+cityId+" and estnum='"+strEstNum+"'").ToString());
                    HtmlElement tbXS = Document.GetElementById("dg_xs").Children[0];

                    Model_EstateSold mes = new Model_EstateSold(estId);
                    for (int i = 1; i < tbXS.Children.Count; i++)
                    {
                        HtmlElement row = tbXS.Children[i];
                        string strTime = row.Children[1].InnerText;
                        Match m = regTimeSpan.Match(strTime);
                        int year = int.Parse(m.Groups[1].Value);
                        int season = int.Parse(m.Groups[2].Value);
                        mes.SoldDateStart = new DateTime(year, (season - 1) * 3 + 1, 1);
                        mes.SoldDateEnd = new DateTime(year, season * 3, DateTime.DaysInMonth(year, season * 3));
                        mes.EstUsage = row.Children[2].InnerText;
                        mes.SoldCount = int.Parse(row.Children[3].InnerText);
                        mes.SoldArea = double.Parse(row.Children[4].InnerText);
                        mes.avePrice = double.Parse(row.Children[6].InnerText);
                        mes.Save(true);
                    }

                    HtmlElement next = null;
                    foreach (HtmlElement el in Document.GetElementById("Pager2").GetElementsByTagName("a"))
                    {
                        if (el.InnerText == "下一页" && el.GetAttribute("href") != null&&el.GetAttribute("href")!="")
                        {
                            next = el;
                            break;
                        }
                    }
                    if (next != null) 
                        next.InvokeMember("click");
                    else this.NavToNext();
                }
                catch {
                    this.NavToNext();
                }
                
            }
        }
    }
}
