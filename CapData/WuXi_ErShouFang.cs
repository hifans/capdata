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
    public partial class WuXi_ErShouFang : CapData.FormBrowser
    {
        Regex regPrice = new Regex("\\((\\d+)元\\/㎡\\)");
        Regex regLayer = new Regex("第(\\d+)层\\(共(\\d+)层\\)");
        Regex regPostTime = new Regex("\\d{4}-\\d{1,2}-\\d{1,2} \\d{1,2}:\\d{1,2}:\\d{1,2}");
        List<KeyValuePair<string, string>> listWordFld = new List<KeyValuePair<string, string>>() { 
            new KeyValuePair<string, string>("年代","BuildingYear"),
            new KeyValuePair<string, string>("朝向","Orientation"),
            new KeyValuePair<string, string>("楼层","Orientation"),
            new KeyValuePair<string, string>("结构","Structure"),
            new KeyValuePair<string, string>("装修","Decoration"),
            new KeyValuePair<string, string>("住宅类别","HouseUsage"),
            new KeyValuePair<string, string>("建筑类别","BuildingType"),
            new KeyValuePair<string, string>("产权性质","PropertyRight"),
            new KeyValuePair<string, string>("楼盘名称","EstName")
        };

        public WuXi_ErShouFang()
        {
            InitializeComponent();
        }

        string tName = "";
        int page = 0;
        private void NavToNextPage()
        {
            page++;
            if (page > 100)
            {
                this.NavToNext();
                return;
            }
            this.Navigate("http://esf.wuxi.soufun.com/house/h316-w31-i3" + page + "/");
        }

        private void WuXi_ErShouFang_Load(object sender, EventArgs e)
        {
            tName = Fn.GetHouseSecTable(Fn.GetCityIdByName("无锡"), DateTime.Today);
            if (tName == "")
            {
                this.Close();
                return;
            }

            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString().StartsWith("http://esf.wuxi.soufun.com/house/"))
            {
                for (int i = 1; i <= 30; i++)
                {
                    try
                    {
                        HtmlElement dt = Document.GetElementById("list_" + i).Children[0].Children[1].Children[0].Children[0];
                        string href = dt.Children[0].Children[0].GetAttribute("href");
                        if (Fn.GetDT_MySQL("select * from "+tName+" where sourceLink='" + Fn.KW_Equal(href) + "'").Rows.Count > 0)
                        {
                            continue;
                        }
                        string[] val = new string[2];
                        val[0]=dt.Children[1].Children[1].GetAttribute("title");
                        val[1]="";
                        foreach (HtmlElement a in dt.Children[3].Children)
                        {
                            val[1] += a.InnerText + " ";
                        }
                        listLink.Add(new KeyValuePair<string, object>(href, val));
                    }
                    catch {
                        this.NavToNext();
                        return;
                    }
                }
                this.NavToNextPage();
                return;
            }

            Model_House_Sec mhs = new Model_House_Sec("无锡");
            mhs.SourceLink = linkCurrent.Key;
            mhs.HouseAddr = (linkCurrent.Value as string[])[0];
            mhs.PostBy = (linkCurrent.Value as string[])[1];
            try
            {
                mhs.Title = GetElementByClassName("icon_tag20120517", "h1").InnerText;
            }
            catch { }

            HtmlElement dl = null;
            try
            {
                dl = GetElementByClassName("base_info", "div").Children[0];
                Match mPrice = regPrice.Match(dl.Children[0].InnerText);
                if (mPrice.Success) mhs.avePrice = double.Parse(mPrice.Groups[1].Value);
                mhs.BuildingArea = double.Parse(dl.Children[2].InnerText.Split('：')[1].Replace("㎡", ""));
                mhs.HouseType = dl.Children[1].InnerText.Split('：')[1];
            }
            catch {
                this.NavToNext();
                return;
            }

            try
            {
                Match mPostTime = regPostTime.Match(Document.GetElementById("Time").Parent.InnerText);
                if (mPostTime.Success) mhs.PostTime = DateTime.Parse(mPostTime.Value);
            }
            catch { }

            try
            {
                mhs.PhoneNum = Document.GetElementById("mobilecode").InnerText;
            }
            catch { }

            dl = GetElementByClassName("borderb mb10", "dl");
            if (dl != null)
            {
                foreach (HtmlElement dd in dl.Children)
                {
                    try
                    {
                        string word = dd.Children[0].InnerText.TrimEnd('：').Replace("\n", "").Replace("\r", "");
                        var fld = listWordFld.Find(delegate(KeyValuePair<string, string> kvp) { return kvp.Key == word; });
                        if (fld.Key == null || fld.Key == "") continue;
                        string value = dd.InnerText.Split('：')[1].Trim();
                        if (fld.Key == "年代") mhs.BuildingYear = int.Parse(value.TrimEnd('年'));
                        else if (fld.Key == "楼层")
                        {
                            Match mLayer = regLayer.Match(dd.InnerText);
                            if (mLayer.Success)
                            {
                                mhs.layer = int.Parse(mLayer.Groups[1].Value);
                                mhs.layerAll = int.Parse(mLayer.Groups[2].Value);
                            }
                        }
                        else if (fld.Key == "楼盘名称")
                        {
                            mhs.EstName = dd.Children[1].InnerText.Trim();
                        }
                        else mhs.SetValue(fld.Value, value);
                    }
                    catch { }
                }

            }

            if (mhs.avePrice > 0) mhs.Save();

            this.NavToNext();
        }
    }
}
