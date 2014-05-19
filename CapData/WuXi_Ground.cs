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
    public partial class WuXi_Ground : FormBrowser
    {
        public WuXi_Ground()
        {
            InitializeComponent();
        }

        List<KeyValuePair<string, string>> listColName = new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("编号", "GroundNum"),
            new KeyValuePair<string, string>("地块编号", "GroundNum"),
            new KeyValuePair<string, string>("地块名称", "GroundName"),
            new KeyValuePair<string, string>("土地位置", "GLocation"),
            new KeyValuePair<string, string>("地块位置", "GLocation"),
            new KeyValuePair<string, string>("土地位", "GLocation"),
            new KeyValuePair<string, string>("土地面积", "GArea"),
            new KeyValuePair<string, string>("用地面积", "GArea"),
            new KeyValuePair<string, string>("土地用途", "GUsage"),
            new KeyValuePair<string, string>("用地性质", "GUsage"),
            new KeyValuePair<string, string>("容积率", "PlotRatio"),
            new KeyValuePair<string, string>("建筑密度", "BuildingDensity"),
            new KeyValuePair<string, string>("绿地率", "GreeningRate"),
            new KeyValuePair<string, string>("建筑限高", "MaxHeight"),
            new KeyValuePair<string, string>("投资强度", "InvestmentIntensity"),
            new KeyValuePair<string, string>("供地条件", "GCondition"),
            new KeyValuePair<string, string>("工地条件", "GCondition"),
            new KeyValuePair<string, string>("土地出让金起始价", "StartingPrice"),
            new KeyValuePair<string, string>("起始总价", "StartingPrice"),
            new KeyValuePair<string, string>("出让底价", "StartingPrice"),
            new KeyValuePair<string, string>("底价", "StartingPrice"),
            new KeyValuePair<string, string>("竞买保证金", "BidBond"),
            new KeyValuePair<string, string>("出让年限", "TransferYear"),
            new KeyValuePair<string, string>("出让方式", "TransferWay")
        };
        List<string>arrPlaning = new List<string>(){"规划条件","规划指标要求" };

        Regex regUrl = new Regex("http://gtj\\.wuxi\\.gov\\.cn/BA13/D/04/index(_\\d+)?\\.shtml", RegexOptions.IgnoreCase);
        Regex regLink = new Regex("http://gtj\\.wuxi\\.gov\\.cn/BA13/D/04/\\d+\\.shtml", RegexOptions.IgnoreCase);

        Regex regColNameFilter = new Regex("[\\(（][^\\(（]+[\\)）]");

        Regex regListingNum = new Regex("锡[工经]告字[（\\(\\[]\\d{4}[）\\)\\]]\\d+号", RegexOptions.IgnoreCase);
        Regex regGroundNum = new Regex("^(锡新?((国土)|(地))|XDG)([\\(（][工经][）\\)])?\\d{4}-\\d+号?", RegexOptions.IgnoreCase);

        int page = 1;

        private void WuXi_Ground_Load(object sender, EventArgs e)
        {          
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://gtj.wuxi.gov.cn/BA13/D/04/index.shtml"); return;
            //listLink.Add(new KeyValuePair<string, object>("http://gtj.wuxi.gov.cn/BA13/D/04/5586510.shtml", "无锡市2006年第三批国有土地使用权挂牌出让公告"));
            //this.NavToNext();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (regUrl.IsMatch(e.Url.ToString()))
            {
                foreach (HtmlElement elLink in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = elLink.GetAttribute("href");
                        if (regLink.IsMatch(strLink))
                        {
                            listLink.Add(new KeyValuePair<string, object>(strLink, elLink.InnerText));
                        }
                    }
                    catch { }
                }

                try
                {
                    HtmlElement btnNext = Document.Body.Children[2].GetElementsByTagName("input")[2];
                    if (bool.Parse(btnNext.GetAttribute("disabled"))) this.NavToNext();
                    else
                    {
                        page++;
                        this.Navigate("http://gtj.wuxi.gov.cn/BA13/D/04/index_" + page.ToString() + ".shtml");
                    }
                }
                catch { this.NavToNext(); }
                return;
            }

            Model_Ground mg = new Model_Ground("无锡");

            //开始计算信息包括的字段
            List<string> arrCol = new List<string>();
            HtmlElement tRow = null;
            HtmlElement tRowSub = null;
            try
            {
                int subIndex = 0;
                foreach (HtmlElement td in Document.GetElementById("Zoom").GetElementsByTagName("td"))
                {
                    try
                    {
                        string str = td.InnerText.Replace("\n","").Replace("\r","").Replace(" ","").Replace("　","").Trim();
                        if (str == "编号" || str == "地块编号")
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
                    string str = td.InnerText.Replace("\n","").Replace("\r","").Replace(" ","").Replace("　","").Trim();
                    str = regColNameFilter.Replace(str, "");
                    KeyValuePair<string, string> kvCol;
                    try
                    {
                        kvCol = listColName.FindAll(delegate(KeyValuePair<string, string> kv) { return kv.Key == str; })[0];
                        arrCol.Add(kvCol.Value);
                    }
                    catch
                    {
                        if (!arrPlaning.Contains(str))
                        {
                            arrCol.Add("");
                            continue;
                        }
                    }

                    int colSpan = int.Parse(td.GetAttribute("colspan"));
                    if (colSpan == 1||int.Parse(td.GetAttribute("rowspan"))>1) continue;
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
            //mg.SourceLink = linkCurrent.Key;
            mg.SourceLink = linkCurrent.Key;

            //开始计算挂牌日期
            foreach (HtmlElement td in Document.GetElementsByTagName("td"))
            {
                if (td.InnerText!=null&&td.InnerText.Trim() == "公开日期")
                {
                    try
                    {
                        mg.ListingDate = DateTime.Parse(td.NextSibling.InnerText);
                    }
                    catch { }
                    break;
                }
            }

            //开始计算挂牌编号
            Match mListingNum = regListingNum.Match(Document.Title);
            if (mListingNum.Success)
            {
                mg.ListingNum = mListingNum.Value;
            }

            //开始计算每条招拍挂信息
            int[] arrRowSpan = new int[arrCol.Count];
            try
            {
                Type type = typeof(Model_Ground);
                HtmlElement tr = (tRowSub == null ? tRow : tRowSub).NextSibling;
                while (tr!=null)
                {
                    if (tr.Children.Count == 0) break;
                    string strGroundNum = tr.Children[0].InnerText;
                    if (strGroundNum == null) break;
                    strGroundNum = strGroundNum.Replace("\r", "").Replace("\n", "").Replace(" ","").Replace("　","").Trim();
                    Match mGroundNum = regGroundNum.Match(strGroundNum);
                    if (!mGroundNum.Success)
                    {
                        tr = tr.NextSibling;
                        continue;
                    }
                    mg.GroundNum = mGroundNum.Value;

                    int offset = 0;

                    string strImageLink = null;
                    for (int i = 1; i < arrCol.Count; i++)
                    {
                        if (arrCol[i] == "" || arrCol[i] == null) continue;
                        if (arrRowSpan[i] > 0)
                        {
                            offset++;
                            arrRowSpan[i]--;
                            continue;
                        }

                        if (arrCol[i] == "GroundName")
                        {
                            try
                            {
                                strImageLink = tr.Children[i - offset].GetElementsByTagName("a")[0].GetAttribute("href");
                            }
                            catch { }
                        }

                        string strValue = tr.Children[i-offset].InnerText;
                        if (strValue != null)
                        {
                            strValue = strValue.Replace("\n", "").Replace("\r", "").Trim();
                        }
                        FieldInfo info = type.GetField(arrCol[i]);
                        Type t = info.FieldType;

                        int rowSpan = int.Parse(tr.Children[i-offset].GetAttribute("rowspan"))-1;
                        if (rowSpan > 0)
                        {
                            arrRowSpan[i] = rowSpan;
                        }
                        try
                        {
                            if (t == typeof(int))
                            {
                                info.SetValue(mg, int.Parse(strValue));
                            }
                            else if (t == typeof(double))
                            {
                                info.SetValue(mg, double.Parse(strValue));
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
                    mg.GroundName = mg.GroundName.Split('.')[0];
                    mg.StartingPrice *= 10000;
                    mg.BidBond *= 10000;
                    mg.Save();

                    Model_Ground_File mgf = new Model_Ground_File(mg.CityId, mg.GroundNum);
                    if (strImageLink != null && strImageLink != ""&&mgf.GroundId>0)
                    {
                        try
                        {
                            string[] tmp = strImageLink.Split('/');
                            string strFileName = mg.CityId + "_" + tmp[tmp.Length - 1];
                            if (!Fn.IsExistGroundFile(strFileName, FileType.Image))
                            {
                                bool b = Fn.DownloadFile(strImageLink, strFileName, FileType.Image);
                                if (b)
                                {
                                    mgf.FileName = strFileName;
                                    mgf.FileTitle = "土地红线图";
                                    mgf.FileType = FileType.Image;
                                    mgf.Save();
                                }
                            }

                        }
                        catch { }
                    }
                    tr = tr.NextSibling;
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.AddMessage("错误！无锡地块数据抓取错误。链接：" + mg.SourceLink + "。地块Id：" + mg.GroundNum + "。错误消息：" + ex.Message);
            }

            this.NavToNext();
            
        }
    }
}
