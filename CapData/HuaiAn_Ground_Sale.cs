using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class HuaiAn_Ground_Sale : CapData.FormBrowser
    {
        public HuaiAn_Ground_Sale()
        {
            InitializeComponent();
        }

        private void HuaiAn_Ground_Sale_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(HuaiAn_Ground).Name)) return;
            this.timer1.Stop();
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            DataTable dt = Fn.GetDT_PgSQL("select distinct sourcelink from ground_info where cityid=12 and (plotratio is null or plotratio='') and isverified=false");
            foreach (DataRow dr in dt.Rows)
            {
                listLink.Add(new KeyValuePair<string, object>(dr[0].ToString(), null));
            }

            this.NavToNext();
        }

        

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            try
            {
                HtmlElement tbody = Document.Window.Frames["f_view"].Document.GetElementById("tblparcel").FirstChild;
                Model_Ground mg = new Model_Ground("淮安");
                for (int i = 2; i < tbody.Children.Count-1; i++)
                {
                    try
                    {
                        HtmlElement tr = tbody.Children[i];
                        mg.GroundNum = tr.Children[0].InnerText.Trim();

                        if (!mg.IsExist(mg.GroundNum)) continue;

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
}
