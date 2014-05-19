using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class HuaiAn_Estate : CapData.FormBrowser
    {
        public HuaiAn_Estate()
        {
            InitializeComponent();
        }

        private void HuaiAn_Estate_Load(object sender, EventArgs e)
        {
            Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != Document.Url.ToString()) return;
            if (e.Url.ToString().StartsWith("http://www.lousw.com/loupanfnewsearch.asp?pages0="))
            {
                HtmlElement tbody = Document.Body.Children[6].Children[1];
                if (tbody.Children[2].Children.Count == 1)
                {
                    this.NavToNext();
                    return;
                }
                for (int i = 2; i < tbody.Children.Count-1; i++)
                {
                    HtmlElement tr = tbody.Children[i];
                    string strLink = tr.Children[1].Children[0].GetAttribute("href");
                    if (!Fn.IsExistEstUrl(strLink) && !UrlInLinkList(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, tr.Children[0].InnerText));
                }

                this.NavToNextPage(); return;
            }

            try
            {
                Model_Estate me = new Model_Estate("淮安");
                me.SourceLink = linkCurrent.Key;
                me.EstNum = me.SourceLink.Split('=')[1];
                me.DistrictName = linkCurrent.Value.ToString();
                HtmlElement body = Document.Body;
                HtmlElement info = body.Children[14].GetElementsByTagName("table")[2].Children[0].Children[0].Children[0];
                string strTmp = info.InnerText;
                me.EstName = strTmp.Substring(0, strTmp.IndexOf("（")).Trim();
                me.Developer = strTmp.Substring(strTmp.IndexOf("：")+1).Trim('）').Trim();

                info = body.Children[16].Children[0].Children[0].Children[2].Children[0].Children[0];

                HtmlElement info1 = info.Children[3].GetElementsByTagName("tbody")[1];
                me.SaleTele = info1.Children[1].InnerText;
                me.SaleAddr = info1.Children[2].InnerText.Split('：')[1];
                me.EstAddr = info1.Children[3].InnerText.Split('：')[1];

                info1 = info.Children[7].Children[0].Children[0].Children[0].Children[1].Children[0];
                me.CountAll = int.Parse(info1.InnerText.Replace("套", ""));

                info1 = info.Children[17].Children[0].Children[0].Children[0];
                me.Traffic = info1.Children[1].Children[0].InnerText;
                me.SrdFacility = info1.Children[3].Children[0].InnerText;
                me.Save();
            }
            catch { }
            this.NavToNext();
        }

        int page = 0;
        private void NavToNextPage()
        {
            page++;
            //if (page > 1) { this.NavToNext(); return; }
            this.Navigate("http://www.lousw.com/loupanfnewsearch.asp?pages0="+page);
        }
    }
}
