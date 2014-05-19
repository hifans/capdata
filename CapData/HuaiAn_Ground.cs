using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class HuaiAn_Ground : FormBrowser
    {
        public HuaiAn_Ground()
        {
            InitializeComponent();
        }

        int page = 0;
        int cityId = Fn.GetCityIdByName("淮安");

        string q = "$" + Fn.GetRandomStr() + "$";

        private void HuaiAn_Ground_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Browser.Url) return;
            if (e.Url.ToString().StartsWith("http://www.landjs.com/web/cjgs_list.aspx"))
            {
                HtmlElement tbody = Document.GetElementById("GridView1").Children[0];
                if (tbody == null||tbody.Children.Count<=1)
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

            try
            {
                Model_Ground mg = new Model_Ground("淮安");
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

                try { mg.SourceLink = Document.GetElementById("AFFICHE_NO").FirstChild.GetAttribute("href"); }
                catch { }

                mg.ClinchLink = this.linkCurrent.Key;

                mg.Save();
            }
            catch { }
            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            this.Navigate("http://www.landjs.com/web/cjgs_list.aspx?xmlx=1&xzq=3208&page="+page);
        }
    }
}
