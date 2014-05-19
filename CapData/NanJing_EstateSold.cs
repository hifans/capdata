using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class NanJing_EstateSold : CapData.FormBrowser
    {
        public NanJing_EstateSold()
        {
            InitializeComponent();
        }

        private void NanJing_EstateSold_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            DataTable dt = Fn.GetDT_PgSQL("select distinct a.id, a.estnum, a.sourcelink,sum(b.soldcount) as countsold,sum(b.soldarea) as areasold,sum(soldarea*aveprice) as tprice, max(b.solddateend) as dateend from estate_info as a left join estate_sold as b on a.id=b.estid where cityid=" + Fn.GetCityIdByName("南京") + " and a.countall>countsold group by a.id, a.estnum, a.sourcelink");
            foreach (DataRow dr in dt.Rows)
            {
                List<object> arrInfo = new List<object>();
                arrInfo.Add(dr[0]);//id
                arrInfo.Add(dr[3]);//销售套数
                arrInfo.Add(dr[4]);//销售面积
                arrInfo.Add(dr[5]);//销售总价
                arrInfo.Add(dr[6]);//最后抓取数据日期
                try
                {
                    if (dr[6].ToString() != "" && DateTime.Parse(dr[6].ToString()) >= DateTime.Today) continue;
                }
                catch { }
                if (!dr[2].ToString().StartsWith("http://newhouse.njhouse.com.cn/detail.php?id=")) continue;
                this.listLink.Add(new KeyValuePair<string, object>(dr[2].ToString(), arrInfo));
            }
            this.NavToNext();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://newhouse.njhouse.com.cn/detail.php?id="))
            {
                try
                {
                    this.Navigate(Document.GetElementsByTagName("iframe")[3].GetAttribute("src"));
                }
                catch { this.NavToNext(); }
            }
            else
            {
                try
                {
                    List<object> arrInfo = linkCurrent.Value as List<object>;
                    Model_EstateSold mes = new Model_EstateSold(int.Parse(arrInfo[0].ToString()));
                    try
                    {
                        mes.SoldDateStart = DateTime.Parse(arrInfo[4].ToString()).AddDays(1);
                    }
                    catch { mes.SoldDateStart = new DateTime(1900, 1, 1); }
                    mes.SoldDateEnd = DateTime.Today;

                    int countAll = 0, countNew = 0;
                    double areaAll = 0, areaNew = 0;
                    double tPrice = 0, tPriceNew = 0;

                    try { countAll = int.Parse(arrInfo[1].ToString()); }
                    catch { }

                    try { areaAll = double.Parse(arrInfo[2].ToString()); }
                    catch { }

                    try { tPrice = double.Parse(arrInfo[3].ToString()); }
                    catch { }

                    HtmlElement tbody = Document.Window.Frames[0].Document.GetElementsByTagName("table")[1].Children[0];

                    try { countNew = int.Parse(tbody.Children[2].Children[1].InnerText.Replace("套", "")); }
                    catch { }
                    try { areaNew = double.Parse(tbody.Children[2].Children[3].InnerText.Replace("m2", "")); }
                    catch { }
                    try { tPriceNew = double.Parse(tbody.Children[3].Children[1].InnerText.Replace("元/m2", "")); }
                    catch { }

                    tPriceNew *= areaNew;

                    mes.SoldCount = countNew - countAll;
                    mes.SoldArea = areaNew - areaAll;
                    mes.avePrice = (tPriceNew - tPrice) / (areaNew - (tPrice == 0 ? 0 : areaAll));
                    mes.Save(false);

                    if (tPrice == 0 && mes.avePrice != 0) Fn.ExecNonQuery_PgSQL("update estate_sold set aveprice=" + mes.avePrice + " where estid=" + mes.EstId + " and aveprice=0");
                }
                catch { }

                this.NavToNext();
            }
        }
    }
}
