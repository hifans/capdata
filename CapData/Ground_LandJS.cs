using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class Ground_LandJS : CapData.FormBrowser
    {
        private int CityCode = 0;
        private string CityName = "";

        int page = 0;
        int cityId = 0;
        int xmlx = 1;
        List<string> listTransferWay = new List<string>() { 
            "招拍挂","招拍挂","协议","划拨"
        };

        string q = "$" + Fn.GetRandomStr() + "$";

        /// <summary>
        /// 抓取江苏土地市场网数据
        /// </summary>
        /// <param name="cityCode">城市编号</param>
        /// <param name="cityName">城市名称</param>
        /// <param name="xmlx">项目类型，1表示招拍挂（默认值），2表示协议，3表示划拨</param>
        public Ground_LandJS(int cityCode, string cityName, int xmlx)
        {
            this.CityCode = cityCode;
            this.CityName = cityName;
            this.xmlx = xmlx;
            cityId = Fn.GetCityIdByName(CityName);
            this.Text = "土地成交（" + listTransferWay[xmlx] + "） - " + CityName;
            InitializeComponent();
        }

        /// <summary>
        /// 抓取江苏土地市场网招拍挂数据
        /// </summary>
        /// <param name="cityCode">城市编号</param>
        /// <param name="cityName">城市名称</param>
        public Ground_LandJS(int cityCode, string cityName)
        {
            this.CityCode = cityCode;
            this.CityName = cityName;
            cityId = Fn.GetCityIdByName(CityName);
            this.Text = "土地成交 - " + CityName;
            InitializeComponent();
        }

        private void Ground_LandJS_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Browser.Url) return;
            if (e.Url.ToString().StartsWith("http://www.landjs.com/web/cjgs_list.aspx"))
            {
                HtmlElement tbody=null;
                try { tbody = Document.GetElementById("GridView1").Children[0]; }
                catch { }
                if (tbody == null || tbody.Children.Count <= 1)
                {
                    this.NavToNext(); return;
                }

                for (int i = 1; i < tbody.Children.Count; i++)
                {
                    try
                    {
                        HtmlElement a = tbody.Children[i].Children[2].Children[0];
                        string strLink = a.GetAttribute("href");
                        string groundNum = a.InnerText.Trim();

                        if (Fn.GetDT_PgSQL("select * from ground_info where cityid='" + cityId + "' and groundnum=" + q + groundNum + q).Rows.Count > 0) continue;
                        listLink.Add(new KeyValuePair<string, object>(strLink, groundNum));
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }
            else if (e.Url.ToString().StartsWith("http://www.landjs.com/web/cjgs_view.aspx"))
            {
                try
                {
                    Model_Ground mg = new Model_Ground(CityName);
                    try { mg.GroundNum = Document.GetElementById("PARCEL_NO").InnerText.Trim(); }
                    catch { }

                    try { mg.ListingNum = Document.GetElementById("AFFICHE_NO").InnerText.Trim(); }
                    catch { }

                    try { mg.GLocation = Document.GetElementById("LAND_POSITION").InnerText.Trim(); }
                    catch { }

                    try { mg.DistrictName = Document.GetElementById("XZQ_DM").InnerText.Trim(); }
                    catch { }

                    try { mg.GArea = Convert.ToDouble(Document.GetElementById("REMISE_AREA").InnerText.Trim()); }
                    catch { }

                    try { mg.GUsage = Document.GetElementById("LAND_USE").InnerText.Trim(); }
                    catch { }

                    try { mg.TransferYear = Document.GetElementById("USE_YEAR").InnerText.Trim(); }
                    catch { }

                    try { mg.TransferWay = Document.GetElementById("REMISE_TYPE").InnerText.Trim(); }
                    catch { }

                    try { mg.StartingPrice = Convert.ToDouble(Document.GetElementById("START_PRICE").InnerText.Trim()) * 10000; }
                    catch { }

                    try { mg.ClinchPrice = Convert.ToDouble(Document.GetElementById("PRICE").InnerText.Trim()) * 10000; }
                    catch { }

                    try { mg.ClinchDate = Convert.ToDateTime(Document.GetElementById("BARGAIN_DATE").InnerText.Trim()); }
                    catch { }

                    try { mg.Buyer = Document.GetElementById("ALIENEE").InnerText.Trim(); }
                    catch { }

                    try { mg.ListingDate = Convert.ToDateTime(Document.GetElementById("KSRQ").InnerText.Trim()); }
                    catch { }

                    try { mg.GCondition = Document.GetElementById("DELIVER_TERM").InnerText.Trim(); }
                    catch { }

                    string strSourceLink = "";
                    try 
                    { 
                        strSourceLink = Document.GetElementById("AFFICHE_NO").FirstChild.GetAttribute("href");
                        mg.SourceLink = strSourceLink;
                    }
                    catch { }

                    mg.ClinchLink = this.linkCurrent.Key;
                    if (mg.TransferWay == "") mg.TransferWay = listTransferWay[xmlx];

                    mg.Save();

                    if (strSourceLink != null && strSourceLink != "")
                    {
                        this.Navigate(strSourceLink);
                    }
                    else this.NavToNext();

                }
                catch { this.NavToNext(); }
            }
            else
            {
                try
                {
                    HtmlElement tbody = Document.Window.Frames["f_view"].Document.GetElementById("tblparcel").FirstChild;
                    Model_Ground mg = new Model_Ground(CityName);
                    mg.GroundNum = linkCurrent.Value.ToString();
                    for (int i = 2; i < tbody.Children.Count - 1; i++)
                    {
                        try
                        {
                            HtmlElement tr = tbody.Children[i];
                            string groundNum = tr.Children[0].InnerText.Trim();
                            if (groundNum != mg.GroundNum) continue;

                            if (!mg.IsExist(mg.GroundNum)) break;

                            mg.PlotRatio = tr.Children[4].InnerText;
                            mg.BuildingDensity = tr.Children[5].InnerText;
                            mg.GreeningRate = tr.Children[6].InnerText;
                            mg.BidBond = Convert.ToDouble(tr.Children[8].InnerText.Trim()) * 10000;
                            mg.StartingPrice = Convert.ToDouble(tr.Children[9].InnerText.Trim()) * 10000;
                            mg.IncreaseRate = tr.Children[10].InnerText.Trim();

                            mg.Save(true, new List<string>() { "PlotRatio", "BuildingDensity", "GreeningRate", "BidBond", "StartingPrice", "IncreaseRate" });
                        }
                        catch { }

                    }
                }
                catch { }
                this.NavToNext();
            }
        }

        private void NavToNextPage()
        {
            page++;
            this.Navigate("http://www.landjs.com/web/cjgs_list.aspx?xmlx=" + xmlx + "&xzq=" + CityCode + "&page=" + page);
        }
    }
}
