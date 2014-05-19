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
    public partial class WuXi_House_List : CapData.FormBrowser
    {
        int cityId = Fn.GetCityIdByName("无锡");
        Regex regNum = new Regex("^\\d+$");
        Regex regSaleCount = new Regex("入网总套数：(\\d+)套");
        Regex regSoldCount = new Regex("成交套数：(\\d+)套");

        Regex regXKList = new Regex(@"http://www\.wxhouse\.com/house/xk_(\d+)\.html\?blid=0");
        Regex regXKLink = new Regex(@"http://www\.wxhouse\.com/house/xk_(\d+)\.html\?blid=([1-9]\d+)");
        Regex regHSLink = new Regex(@"http://www\.wxhouse\.com/house/hsview\.html\?houseid=\d+&hsid=(\d+)");

        List<KeyValuePair<string, int[]>> listEstSaleCount = new List<KeyValuePair<string, int[]>>();
        int indexEst = -1;


        string tName_HouseInfo = "house_info";

        public WuXi_House_List()
        {
            InitializeComponent();
        }

        private void WuXi_HouseInfo_Load(object sender, EventArgs e)
        {
            this.tName_HouseInfo = Fn.GetHouseInfoTable(this.cityId);
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);

            this.timerWait.Start();
        }

        private void Start()
        {
            //计算网站可售量和数据库可售量之间的差别
            
            DataTable dt = Fn.GetDT_PgSQL("select estnum, countAll, countSold from estate_info where cityid='" + cityId + "' and deleted=false and forsechandhouse=false and forcase=false");
            foreach (DataRow dr in dt.Rows)
            {
                string num = dr[0].ToString();
                if (!regNum.IsMatch(num)) continue;
                int countAll = 0, countSold=0;
                try { countAll = int.Parse(dr[1].ToString()); }
                catch { }
                try { countSold = int.Parse(dr[2].ToString()); }
                catch { }
                listEstSaleCount.Add(new KeyValuePair<string, int[]>(num, new int[]{countAll, countSold}));
            }
            this.NavToNextEstSaleCount();
            //this.listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/house/xk_25.html?blid=100153", new string[] { "25", "1229" }));
            //this.NavToNext();
        }

        private void NavToNextEstSaleCount()
        {
            this.indexEst++;
            if (indexEst >=this.listEstSaleCount.Count)
            {
                this.NavToNext();
                return;
            }
            this.Navigate("http://www.wxhouse.com/newhouse/hs_bainfo.html?hsid=" + this.listEstSaleCount[indexEst].Key);
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://www.wxhouse.com/newhouse/hs_bainfo.html?hsid="))//检查楼盘上市套数和成交套数
            {
                string strBody = Document.Body.InnerText.Replace(" ","");
                int countAll = 0, countSold = 0;
                Match mAll = regSaleCount.Match(strBody);
                
                if (mAll.Success)
                {
                    countAll = int.Parse(mAll.Groups[1].Value);
                }
                Match mSold = regSoldCount.Match(strBody);
                if (mSold.Success)
                {
                    countSold = int.Parse(mSold.Groups[1].Value);
                }
                int[] tmp = listEstSaleCount[indexEst].Value;

                if (countAll != 0 && (countAll != tmp[0] || countSold != tmp[1])) this.listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/house/xk_" + listEstSaleCount[indexEst].Key + ".html?blid=0", listEstSaleCount[indexEst].Key));
                this.NavToNextEstSaleCount();
                return;
            }
            else if (regXKList.IsMatch(e.Url.ToString())) //许可证列表
            {
                foreach (HtmlElement td in Document.GetElementsByTagName("td"))
                {
                    try
                    {
                        if (td.GetAttribute("classname") != "yxsxkz") continue;
                        string href = td.Children[0].GetAttribute("href");
                        if (href == null || href == "") continue;
                        string permitNum = "";
                        try
                        {
                            permitNum = href.Split('=')[1];
                        }
                        catch { }

                        string permitName = td.Children[0].InnerText.Trim();
                        string sFilter = "cityid=" + this.cityId + " and PermitNum='" + permitNum + "' and EstNum='" + linkCurrent.Value.ToString() + "'";
                        if (Fn.GetCount_MySQL("permitinfo", sFilter) == 0)
                        {
                            string sql = "";
                            if (Fn.GetCount_MySQL("permitinfo", "cityid='" + this.cityId + "' and PermitNum='"+permitNum+"'") > 0)
                            {
                                sql = "update permitinfo set EstNum='" + linkCurrent.Value.ToString() + "', SaleCount=-1 where cityid='" + this.cityId + "' and PermitNum='" + permitNum + "';";
                            }
                            else sql = "insert into permitinfo(cityid,PermitNum,PermitName, EstNum, IssueDate, SaleCount,SourceLink) values (" + this.cityId + ",'" + permitNum + "','" + permitName + "','" + linkCurrent.Value.ToString() + "',current_date, -1,'http://pub.wxlife.cn/BuildInfo.pub?blid=" + permitNum + "');";
                            Fn.ExecNonQuery_MySQL(sql);
                        }

                        string pId = Fn.GetObj_MySQL("select id from permitinfo where " + sFilter + " limit 1").ToString();
                        string estId = Fn.GetObj_PgSQL("select id from estate_info where cityid=" + this.cityId + " and estnum='" + linkCurrent.Value.ToString() + "' limit 1").ToString();
                        if (Fn.GetCount_MySQL("permitinfo", "cityid='"+cityId+"' and permitnum='"+permitNum+"' and SoldOut=1") == 0) listLink.Add(new KeyValuePair<string, object>(href, new string[] { estId, pId }));
                    }
                    catch { }
                }
            }
            else if (regXKLink.IsMatch(e.Url.ToString())) //许可证房产列表页面
            {
                string[] tmp = linkCurrent.Value as string[];
                int estId = int.Parse(tmp[0]);
                int permitId = int.Parse(tmp[1]);
                HtmlElement div = Document.GetElementById("hsinfo1");
                string strHouseIdSale = "'-1'";
                foreach (HtmlElement item in div.GetElementsByTagName("a"))
                {
                    try
                    {
                        string href = item.GetAttribute("href");
                        Match mLink = regHSLink.Match(href);
                        if (!mLink.Success) continue;
                        string strNum = mLink.Groups[1].Value;
                        strHouseIdSale += ",'" + strNum + "'";
                        if (Fn.GetCount_MySQL(tName_HouseInfo, "HouseId='" + strNum + "'") == 0)
                        {
                            Model_House_Info mhi = new Model_House_Info(cityId, estId, permitId);
                            mhi.HouseId = strNum;
                            mhi.Area = -1;
                            mhi.Save();
                        }

                    }
                    catch { }
                }
                string sql = "update " + tName_HouseInfo + " set SoldDate='" + DateTime.Today.AddDays(DateTime.Now.Hour >= 12 ? 0 : -1).ToString("yyyy-MM-dd") + "' where PermitId='" + permitId + "' and HouseId not in (" + strHouseIdSale + ") and SoldDate='1900-01-01';";
                sql += "update " + tName_HouseInfo + " set SoldDate='1900-01-01',SoldPrice=0 where PermitId='" + permitId + "' and HouseId in (" + strHouseIdSale + ") and SoldDate>'1900-01-01' and SoldLock=false;";
                if (strHouseIdSale == "'-1'")
                {
                    sql += "update permitinfo set soldOut=1 where id='"+permitId+"';";
                }
                Fn.ExecNonQuery_MySQL(sql);
            }

            this.NavToNext();
        }

        private void timerWait_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(WuXi_Estate).Name)) return;
            this.timerWait.Stop();

            Start();
        }
    }
}
