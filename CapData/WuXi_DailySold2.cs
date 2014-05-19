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
    public partial class WuXi_DailySold2 : CapData.FormBrowser
    {
        public WuXi_DailySold2()
        {
            InitializeComponent();
        }
        int page = 0;
        Regex regLink = new Regex("http://www.wxhouse.com/YanJiu/\\d+\\.html");

        Regex regAll = new Regex("(\\d{1,2}月\\d{1,2}日)，全市商品房[\\(（]不含经济适用房和定销商品房[）\\)]成交量为([\\d\\.]+)平方米，环比[^，]*，环比[^；;]*[;；]全日成交套数为?(\\d+)套");
        Regex regRes = new Regex("(\\d{1,2}月\\d{1,2}日)，全市商品住宅成交量为([\\d\\.]+)平方米，环比[^，]*，环比[^;；]*[;；]全日成交套数为?(\\d+)套");
        Regex regResPrice = new Regex("(\\d{1,2}月\\d{1,2}日)，全市商品住宅[^。]*。全市住宅[^，]*，环比[^，]*，环比[^。;；]*[。;；]商品住宅成交均价为([\\d\\.]+)元");

        private void WuXi_DailySold2_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://www.wxhouse.com/YanJiu/list.html?PageIndex="))
            {
                foreach (HtmlElement link in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = link.GetAttribute("href");
                        if (regLink.IsMatch(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, null));
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                DateTime dateRelease = DateTime.Today;
                foreach (HtmlElement span in Document.GetElementsByTagName("span"))
                {
                    if (span.GetAttribute("classname") == "div_intro")
                    {
                        dateRelease = Convert.ToDateTime("20" + span.InnerText);
                        break;
                    }
                }

                string cont = Document.GetElementById("content").InnerText;
                MatchCollection mcAll = regAll.Matches(cont);
                MatchCollection mcRes = regRes.Matches(cont);
                MatchCollection mcResPrice = regResPrice.Matches(cont);

                Model_DailySold mds = new Model_DailySold("无锡");
                foreach (Match mAll in mcAll)
                {
                    DateTime date = Convert.ToDateTime(DateTime.Today.Year + "年" + mAll.Groups[1].Value);
                    while(date > dateRelease) date = date.AddYears(-1);

                    mds.SoldDate = date;
                    double areaAll = double.Parse(mAll.Groups[2].Value);
                    int countAll = int.Parse(mAll.Groups[3].Value);
                    double priceAll = 0;

                    double areaRes = 0;
                    int countRes = 0;
                    double priceRes = 0;

                    string sFilter = "cityId='" + mds.CityId + "' and soldDate='" + date.ToString("yyyy-MM-dd") + "'";
                    try
                    {
                        priceAll = double.Parse(Fn.GetObj_MySQL("select sum(soldArea*avePrice)/sum(soldArea) from city_dailysold where " + sFilter + " limit 1").ToString());
                    }
                    catch { }

                    foreach (Match m in mcRes)
                    {
                        DateTime dateTmp = Convert.ToDateTime(DateTime.Today.Year + "年" + m.Groups[1].Value);
                        while (dateTmp > dateRelease) dateTmp = dateTmp.AddYears(-1);
                        if (dateTmp == date)
                        {
                            areaRes = double.Parse(m.Groups[2].Value);
                            countRes = int.Parse(m.Groups[3].Value);
                            break;
                        }
                    }

                    foreach (Match m in mcResPrice)
                    {
                        DateTime dateTmp = Convert.ToDateTime(DateTime.Today.Year + "年" + m.Groups[1].Value);
                        while(dateTmp > dateRelease) dateTmp = dateTmp.AddYears(-1);
                        if (dateTmp == date)
                        {
                            priceRes = double.Parse(m.Groups[2].Value);
                            break;
                        }
                    }

                    if (countRes > 0)
                    {
                        Fn.ExecNonQuery_MySQL("delete from city_dailysold where " + sFilter+" and soldUsage='商品房'");
                        if (priceRes == 0) priceRes = priceAll;
                        mds.SoldUsage = "住宅";
                        mds.SoldCount = countRes;
                        mds.SoldArea = areaRes;
                        mds.avePrice = priceRes;
                        mds.Save();

                        mds.SoldUsage = "其他";
                        mds.SoldCount = countAll - countRes;
                        mds.SoldArea = areaAll - areaRes;
                        mds.avePrice = priceAll == 0 ? 0 : ((priceAll * areaAll - priceRes * areaRes) / mds.SoldArea);
                        mds.Save();
                    }
                    else
                    {
                        mds.SoldUsage = "商品房";
                        mds.SoldArea = areaAll;
                        mds.SoldCount = countAll;
                        mds.avePrice = priceAll;
                        mds.Save();
                    }
                }

            }
            catch { }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            //this.listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/YanJiu/40784.html", null));this.NavToNext(); return;
            page++;
            if (page > 20)
            {
                this.NavToNext();
                return;
            }
            this.Navigate("http://www.wxhouse.com/YanJiu/list.html?PageIndex="+page.ToString()+"&TypeID=208");
        }
    }
}
