using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;

namespace CapData
{
    public partial class YiXing_Ground : CapData.FormBrowser
    {
        public YiXing_Ground()
        {
            InitializeComponent();
        }

        List<KeyValuePair<string, string>> listColName = new List<KeyValuePair<string, string>>() { 
            new KeyValuePair<string, string>("编号","GroundNum"),
            new KeyValuePair<string, string>("地块编号","GroundNum"),
            new KeyValuePair<string, string>("挂牌编号","GroundNum"),
            new KeyValuePair<string, string>("土地位置","GLocation"),
            new KeyValuePair<string, string>("地块位置","GLocation"),
            new KeyValuePair<string, string>("土地面积","GArea"),
            new KeyValuePair<string, string>("土地用途","GUsage"),
            new KeyValuePair<string, string>("地块用途","GUsage"),
            new KeyValuePair<string, string>("出让年限","TransferYear"),
            new KeyValuePair<string, string>("交地条件","GCondition"),
            new KeyValuePair<string, string>("产业准入条件","GUsage_Detail"),
            new KeyValuePair<string, string>("容积率","PlotRatio"),
            new KeyValuePair<string, string>("建筑密度","BuildingDensity"),
            new KeyValuePair<string, string>("绿地率","GreeningRate"),
            new KeyValuePair<string, string>("投资强度","InvestmentIntensity"),
            new KeyValuePair<string, string>("起始价","StartingPrice"),
            new KeyValuePair<string, string>("起始总地价","StartingPrice"),
            new KeyValuePair<string, string>("挂牌起始价","StartingPrice"),
            new KeyValuePair<string, string>("出让年限","TransferYear"),
            new KeyValuePair<string, string>("保证金","BidBond"),
            new KeyValuePair<string, string>("竞买保证金","BidBond"),
            new KeyValuePair<string, string>("增价规则及幅度","IncreaseRate"),
            new KeyValuePair<string, string>("起始价保证金每次增幅价","MultiPrice"),//特殊情况
            new KeyValuePair<string, string>("绿地率容积率建筑密度","MultiRate") //特殊情况
        };

        Regex regLink = new Regex("http://www\\.yxlr\\.gov\\.cn/Article/ShowArticle\\.asp\\?ArticleID=\\d+", RegexOptions.IgnoreCase);

        Regex regColNameFilter = new Regex("[\\(（][^\\(（]+[\\)）]");

        int page = 0;

        private void YiXing_Ground_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            Model_Ground mg = new Model_Ground("宜兴");
            if (e.Url.ToString().StartsWith("http://www.yxlr.gov.cn/Article/ShowClass.asp?ClassID=42&page="))
            {
                HtmlElement td = null;
                foreach (HtmlElement tb in Document.GetElementsByTagName("table"))
                {
                    if (tb.InnerText!=null&&tb.InnerText.Trim() == "招拍挂公告")
                    {
                        try
                        {
                            td = tb.Parent.Parent.NextSibling.Children[0];
                        }
                        catch { }
                        break;
                    }
                }

                if (td == null)
                {
                    this.NavToNext();
                    return;
                }

                foreach (HtmlElement elLink in td.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = elLink.GetAttribute("href");
                        if (regLink.IsMatch(strLink))
                        {
                            string strDate = elLink.Parent.NextSibling.InnerText;
                            if (listLink.Contains(new KeyValuePair<string, object>(strLink, strDate)))
                            {
                                this.NavToNext();
                                return;
                            }
                            if (!mg.IsExistLink(strLink)) listLink.Add(new KeyValuePair<string, object>(strLink, strDate));
                        }
                    }
                    catch { }
                }

                this.NavToNextPage();
                return;
            }

            //开始抓取详细内容

            List<string> arrCol = new List<string>();
            HtmlElement tRow = null;
            HtmlElement tRowSub = null;
            try
            {
                int subIndex = 0;
                foreach (HtmlElement td in Document.GetElementsByTagName("td"))
                {
                    try
                    {
                        string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        if (str == "编号" || str == "地块编号"||str=="挂牌编号")
                        {
                            tRow = td.Parent;
                            if (int.Parse(td.GetAttribute("rowspan")) > 1) tRowSub = tRow.NextSibling;
                            break;
                        }
                    }
                    catch { }
                }

                foreach (HtmlElement td in tRow.Children)
                {
                    string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                    str = regColNameFilter.Replace(str, "");
                    int colSpan = int.Parse(td.GetAttribute("colspan"));
                    KeyValuePair<string, string> kvCol;
                    try
                    {
                        kvCol = listColName.FindAll(delegate(KeyValuePair<string, string> kv) { return kv.Key == str; })[0];
                        arrCol.Add(kvCol.Value);
                    }
                    catch
                    {
                        if (colSpan == 1)
                        {
                            arrCol.Add("");
                            continue;
                        }
                    }
                    
                    if (colSpan == 1) continue;
                    for (int i = 0; i < colSpan; i++)
                    {
                        str = tRowSub.Children[i + subIndex].InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        str = regColNameFilter.Replace(str, "");
                        try
                        {
                            kvCol = listColName.FindAll(delegate(KeyValuePair<string, string> kv) { return kv.Key == str; })[0];
                            arrCol.Add(kvCol.Value);
                        }
                        catch
                        {
                            arrCol.Add("");
                            continue;
                        }
                    }
                }
            }
            catch { this.NavToNext(); return; }

            mg.SourceLink = Document.Url.ToString();

            //开始计算挂牌日期
            try
            {
                mg.ListingDate = DateTime.Parse(linkCurrent.Value.ToString());
            }
            catch { }

            //开始计算每条招拍挂信息
            try
            {
                Type type = typeof(Model_Ground);
                HtmlElement tr = (tRowSub == null ? tRow : tRowSub).NextSibling;
                while (tr != null)
                {
                    for (int i = 0; i < arrCol.Count; i++)
                    {
                        if (arrCol[i] == "" || arrCol[i] == null) continue;

                        string strValue = "";
                        try
                        {
                            strValue = tr.Children[i].InnerText;
                        }
                        catch { }
                        if (strValue != null)  strValue = strValue.Trim();

                        if (arrCol[i] == "MultiPrice")
                        {
                            string[] tmp = strValue.Split('\n');
                            try { mg.StartingPrice = double.Parse(tmp[0].Trim()); }
                            catch { }
                            try { mg.BidBond = double.Parse(tmp[1].Trim()); }
                            catch { }
                            try { mg.IncreaseRate = tmp[2].Trim() + "万元"; }
                            catch { }
                        }
                        else if (arrCol[i] == "MultiRate")
                        {
                            string[] tmp = strValue.Split('\n');
                            try 
                            { 
                                mg.GreeningRate = tmp[0].Trim();
                                mg.PlotRatio = tmp[1].Trim();
                                mg.BuildingDensity = tmp[2].Trim();
                            }
                            catch { }
                        }
                        else
                        {
                            FieldInfo info = type.GetField(arrCol[i]);
                            Type t = info.FieldType;
                            try
                            {
                                if (t == typeof(int))
                                {
                                    try
                                    {
                                        info.SetValue(mg, int.Parse(strValue));
                                    }
                                    catch
                                    {
                                        info.SetValue(mg, 0);
                                    }
                                }
                                else if (t == typeof(double))
                                {
                                    try
                                    {
                                        info.SetValue(mg, double.Parse(strValue));
                                    }
                                    catch
                                    {
                                        info.SetValue(mg, 0);
                                    }
                                }
                                else if (t == typeof(DateTime))
                                {
                                    info.SetValue(mg, DateTime.Parse(strValue));
                                }
                                else
                                {
                                    info.SetValue(mg, strValue);
                                }
                            }
                            catch { }
                        }
                    }
                    mg.StartingPrice *= 10000;
                    mg.BidBond *= 10000;

                    mg.Save();
                    tr = tr.NextSibling;
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.AddMessage("错误！宜兴地块数据抓取错误。链接：" + mg.SourceLink + "。地块Id：" + mg.GroundNum + "。错误消息：" + ex.Message);
            }

            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            this.Navigate("http://www.yxlr.gov.cn/Article/ShowClass.asp?ClassID=42&page="+page);
        }
    }
}
