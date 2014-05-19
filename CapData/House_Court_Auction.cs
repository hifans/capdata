using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace CapData
{
    public partial class House_Court_Auction : CapData.FormBrowser
    {
        public House_Court_Auction()
        {
            InitializeComponent();
        }

        Regex regPageMax = new Regex("第\\d+/(\\d+)页");
        string fileDir = "";
        int page = 0;
        int pageMax = -1;
        private void House_Court_Auction_Load(object sender, EventArgs e)
        {
            this.fileDir = Fn.GetConfValue("fileDir") + "house_court_auction\\";
            if (!System.IO.Directory.Exists(fileDir)) System.IO.Directory.CreateDirectory(fileDir);
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://www1.rmfysszc.gov.cn/projects.shtml"))
            {
                if (pageMax < 0)
                {
                    Match m = regPageMax.Match(Document.Body.InnerText);
                    if (m.Success) pageMax = int.Parse(m.Groups[1].Value);
                }

                HtmlElement tBody = null;
                try
                {
                    tBody = GetElementByClassName("objlist", "div").Children[1].Children[0];
                }
                catch { }

                if (tBody == null) { this.NavToNext(); return; }

                for (int i = 1; i < tBody.Children.Count; i++)
                {
                    try
                    {
                        HtmlElement tr = tBody.Children[i];
                        Model_House_Court_Auction hca = new Model_House_Court_Auction(tr.Children[3].Children[0].GetAttribute("title").Trim().Split('/'));
                        if (hca.CityId == 0) continue;
                        hca.PrjName = tr.Children[0].GetAttribute("title");
                        hca.SourceLink = tr.Children[0].Children[0].GetAttribute("href");
                        hca.CourtPlace = tr.Children[3].Children[0].GetAttribute("title");
                        hca.CourtEntrust = tr.Children[1].Children[0].GetAttribute("title");
                        hca.AuctionResult = tr.Children[4].GetAttribute("title");
                        try
                        {
                            hca.StartingPrice = double.Parse(tr.Children[5].InnerText.Trim()) * 10000;
                        }
                        catch { }

                        try
                        {
                            hca.PubDate = DateTime.Parse(tr.Children[7].InnerText.Trim());
                        }
                        catch { }

                        int id = hca.Save();

                        string dir=fileDir + hca.PubDate.ToString("yyyy_MM") + "\\";
                        if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);

                        string fileName=hca.PubDate.ToString("yyyy_MM")+"\\"+hca.MD5+".txt";

                        if (id > 0 || !System.IO.File.Exists(dir + fileName))
                        {
                            listLink.Add(new KeyValuePair<string, object>(hca.SourceLink, fileName));
                        }
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                string content = GetElementByClassName("xmxx_titlemaincontent","div").InnerHtml;
                StreamWriter sw = new StreamWriter(fileDir + linkCurrent.Value.ToString(), false);
                sw.Write(content);
                sw.Close();
            }
            catch { }

            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            if (page >= pageMax && (pageMax > 0 || pageMax < 0 && page > 100))
            {
                this.NavToNext();
                return;
            }

            this.Navigate("http://www1.rmfysszc.gov.cn/projects.shtml?s=q&c=320000&tt=111&fid=%2c%2c&gpstate=2&page=" + page.ToString());
        }
    }
}
