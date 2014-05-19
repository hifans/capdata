using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace CapData
{
    public partial class ZhenJiang_Estate : CapData.FormBrowser
    {
        public ZhenJiang_Estate()
        {
            InitializeComponent();
        }
        int pageMax = -1;
        int page = 0;
        private void ZhenJiang_Estate_Load(object sender, EventArgs e)
        {
            Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("about:blank");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString() == "about:blank")
            {
                page++;
                if (pageMax > 0 && page > pageMax) { this.NavToNext(); return; }
                string para = "currentPage=" + page;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://222.186.197.52:9080/estate2/olestate/queryPjtList.action");
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] byteArray = Encoding.GetEncoding("GBK").GetBytes(para);
                req.ContentLength = byteArray.Length;
                Stream newStream = req.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();

                string result = string.Empty;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);

                    result = sr.ReadToEnd();
                    sr.Close();
                    response.Close();
                }
                catch { this.NavToNext(); return; }
                if (result == string.Empty) { this.NavToNext(); return; }

                Document.Write(result);

                if (pageMax < 0)
                {
                    try
                    {
                        Regex reg = new Regex("共\\((\\d+)\\)页");
                        Match m = reg.Match(GetElementByClassName("pages_btns").InnerText);
                        pageMax = int.Parse(m.Groups[1].Value);
                    }
                    catch { pageMax = 10; }
                }

                foreach (HtmlElement link in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = link.GetAttribute("href");
                        if (strLink.StartsWith("about:/estate2/olestate/getPjtPrep.action"))
                        {
                            strLink = strLink.Replace("about:", "http://222.186.197.52:9080");
                            if (!UrlInLinkList(strLink) && !Fn.IsExistEstUrl(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText));
                        }
                    }
                    catch { }
                }

                this.Navigate("about:blank");
                return;
            }

            try
            {
                Model_Estate me = new Model_Estate("镇江");
                me.SourceLink = linkCurrent.Key;
                me.EstName = linkCurrent.Value.ToString();
                me.EstNum = me.SourceLink.Split('=')[1].Split('&')[0];

                HtmlElement info = Document.GetElementById("table1").Children[0];
                me.DistrictName = info.Children[2].Children[3].InnerText;
                me.PlateName = info.Children[3].Children[1].InnerText;
                me.BuildingAreaAll = double.Parse(info.Children[3].Children[3].InnerText);
                me.GroundAreaAll = double.Parse(info.Children[5].Children[3].InnerText);
                me.CompletionTime = info.Children[6].Children[3].InnerText;
                me.EstIntroduction = info.Children[7].Children[1].InnerText;
                me.Save();
            }
            catch { }

            this.NavToNext();
        }

    }
}
