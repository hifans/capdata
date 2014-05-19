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
    public partial class WuXi_Ground_Presell : CapData.FormBrowser
    {
        public WuXi_Ground_Presell()
        {
            InitializeComponent();
        }

        Regex regLink = new Regex("javascript:tonext\\((\\d+),'([0-9a-f]+)'\\);");
        Regex regPageAll = new Regex("共(\\d+)页");
        string strIds = "";
        private void WuXi_Ground_Presell_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://www.landwx.com/wxmh/presell/presell!list.action");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString() == "http://www.landwx.com/wxmh/presell/presell!list.action")
            {
                HtmlElement list = GetElementByClassName("list_bg","table");
                if (list == null)
                {
                    this.NavToNext();
                    return;
                }
                foreach (HtmlElement link in list.GetElementsByTagName("a"))
                {
                    string href = link.GetAttribute("href");
                    string groundName = link.InnerText;
                    if (groundName == null) continue;
                    groundName = groundName.Trim();
                    Match m = regLink.Match(href);
                    if (!m.Success)
                    {
                        //MessageBox.Show(href+";"+groundName);
                        continue;
                    }
                    href = "http://www.landwx.com/wxmh/presell/presell!input.action?type=" + m.Groups[1].Value + "&presellId=" + m.Groups[2].Value;

                    if (!listLink.Contains(new KeyValuePair<string, object>(href, groundName))) listLink.Add(new KeyValuePair<string, object>(href, groundName));
                    else
                    {
                        //MessageBox.Show(href + ";" + groundName);
                    }
                }

                this.NavToNext();
                return;

                try
                {
                    string page = Document.GetElementById("pageNo").GetAttribute("value");
                    string strPage = Document.GetElementById("pageNo").Parent.InnerText.Replace(" ", "");
                    Match mPage = regPageAll.Match(strPage);
                    if (!mPage.Success || mPage.Groups[1].Value == page) this.NavToNext();
                    else
                    {
                        Document.GetElementById("pageNo").SetAttribute("value", (int.Parse(page)+1).ToString());
                        Document.GetElementById("pageForm").InvokeMember("submit");
                    }
                }
                catch { this.NavToNext(); }
                return;
            }

            try
            {
                Model_Ground_Presell mgp = new Model_Ground_Presell("无锡");
                mgp.GroundNum = linkCurrent.Value.ToString();
                mgp.SourceLink = linkCurrent.Key;

                HtmlElement tbody = GetElementByClassName("list_position", "table").NextSibling.NextSibling.NextSibling.Children[0];
                try { mgp.GroundName = tbody.Children[0].Children[1].InnerText.Trim(); }
                catch { }

                try { mgp.GLocation = tbody.Children[2].Children[1].InnerText.Trim(); }
                catch { }

                try { mgp.GArea = double.Parse(tbody.Children[3].Children[1].InnerText.Replace("平方米", "").Trim()); }
                catch { }

                try { mgp.GUsage = tbody.Children[4].Children[1].InnerText.Trim(); }
                catch { }

                try { mgp.PlotRatio = tbody.Children[5].Children[1].InnerText.Trim(); }
                catch { }

                try { mgp.BuildingDensity = tbody.Children[5].Children[3].InnerText.Trim(); }
                catch { }

                try { mgp.GreeningRate = tbody.Children[5].Children[5].InnerText.Trim(); }
                catch { }

                try { mgp.Surrounding = tbody.Children[6].Children[1].InnerText.Trim(); }
                catch { }

                this.strIds += mgp.Save(true).ToString() + ",";
            }
            catch { }

            if (this.IndexCurrent == this.listLink.Count - 1&& this.strIds!="")
            {
                try
                {
                    Fn.ExecNonQuery_PgSQL("update ground_presell set expired=true where id not in (" + strIds.Trim(',') + ")");
                }
                catch { }
            }
            this.NavToNext();
        }
    }
}
