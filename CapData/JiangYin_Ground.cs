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
    public partial class JiangYin_Ground : CapData.FormBrowser
    {
        public JiangYin_Ground()
        {
            InitializeComponent();
        }
        Regex regPage = new Regex("当前第  (\\d+)  页");
        Regex regLink = new Regex("http://gtj\\.jiangyin\\.gov\\.cn/jygt/sitePages/subPages/\\d+\\.jsp\\?sourceChannelId=7154&did=253120", RegexOptions.IgnoreCase);
        Regex regListingDate = new Regex("([一二三四五六七八九0o]{4})年([一二三四五六七八九十]{1,2})月([一二三四五六七八九十]{1,3})日", RegexOptions.IgnoreCase);

        Regex regColNameFilter = new Regex("[\\(（][^\\(（]+[\\)）]");

        Regex regListingNum = new Regex("澄国土告字\\[\\d{4}\\]\\d+号", RegexOptions.IgnoreCase);

        int pageCurrent = 1;

        List<KeyValuePair<string, string>> listColName = new List<KeyValuePair<string, string>>() { 
            new KeyValuePair<string,string>("地块编号","GroundNum"),
            new KeyValuePair<string,string>("宗地编号","GroundNum"),
            new KeyValuePair<string,string>("土地座落","GLocation"),
            new KeyValuePair<string,string>("地块座落","GLocation"),
            new KeyValuePair<string,string>("土地用途","GUsage"),
            new KeyValuePair<string,string>("出让年限","TransferYear"),
            new KeyValuePair<string,string>("用地面积","GArea"),
            new KeyValuePair<string,string>("出让面积","GArea"),
            new KeyValuePair<string,string>("容积率","PlotRatio"),
            new KeyValuePair<string,string>("建筑密度","BuildingDensity"),
            new KeyValuePair<string,string>("绿地率","GreeningRate"),
            new KeyValuePair<string,string>("宗地总投资","InvestmentIntensity"),
            new KeyValuePair<string,string>("办公及生活设施用地比例","SupportFacilitiesRatio"),
            new KeyValuePair<string,string>("挂牌起始价","StartingPrice"),
            new KeyValuePair<string,string>("竞买保证金","BidBond"),
            new KeyValuePair<string,string>("竞价加价幅度","IncreaseRate"),
            new KeyValuePair<string,string>("建筑限高","MaxHeight"),
            new KeyValuePair<string,string>("准入行业","GUsage_Detail")
        };

        List<string> arrPlaning = new List<string>() { "规划设计条件", "集约用地指标" };

        private void JiangYin_Ground_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://gtj.jiangyin.gov.cn/jygt/sitePages/channelPages/135034050062403.jsp");
            //this.Navigate("http://gtj.jiangyin.gov.cn/jygt/sitePages/subPages/1350340007276310.jsp?sourceChannelId=7154&did=253120");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString() == "http://gtj.jiangyin.gov.cn/jygt/sitePages/channelPages/135034050062403.html")
            {
                this.timer1.Interval = 3000;
                this.timer1.Start();
                return;
            }

            Model_Ground mg = new Model_Ground("江阴");

            //开始计算信息包括的字段
            List<string> arrCol = new List<string>();
            HtmlElement tRow = null;
            HtmlElement tRowSub = null;
            try
            {
                int subIndex = 0;
                foreach (HtmlElement td in Document.GetElementById("lk_wcms_edit_pictext701content").GetElementsByTagName("td"))
                {
                    try
                    {
                        string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        if (str == "宗地编号" || str == "地块编号")
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
                    KeyValuePair<string, string> kvCol;
                    int colSpan = int.Parse(td.GetAttribute("colspan"));
                    try
                    {
                        kvCol = listColName.FindAll(delegate(KeyValuePair<string, string> kv) { return kv.Key == str; })[0];
                        arrCol.Add(kvCol.Value);
                    }
                    catch
                    {
                        if (colSpan==1)
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
            //mg.SourceLink = linkCurrent.Key;
            mg.SourceLink = Document.Url.ToString().Replace(".html",".jsp");

            string strContent = Document.GetElementById("lk_wcms_edit_pictext701content").InnerText;
            if (strContent == null) strContent = "";
            //开始计算挂牌日期
            Match mListingDate = regListingDate.Match(strContent);
            if (mListingDate.Success)
            {
                string strYear = ReplaceChineseToNum(mListingDate.Groups[1].Value);
                string strMonth = ReplaceChineseToNum(mListingDate.Groups[2].Value);
                string strDay = ReplaceChineseToNum(mListingDate.Groups[3].Value);
                try
                {
                    mg.ListingDate = DateTime.Parse(strYear + "-" + strMonth + "-" + strDay);
                }
                catch { }
            }

            //开始计算挂牌编号
            Match mListingNum = regListingNum.Match(strContent);
            if (mListingNum.Success)
            {
                mg.ListingNum = mListingNum.Value;
            }

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
                        if (strValue != null)
                        {
                            strValue = strValue.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        }
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
                    mg.GroundNum = mg.GroundNum.Replace("澄地", "").Trim();
                    mg.StartingPrice *= 10000;
                    mg.BidBond *= 10000;
                    mg.InvestmentIntensity = mg.InvestmentIntensity / mg.GArea / 0.0015;

                    mg.Save();
                    tr = tr.NextSibling;
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.AddMessage("错误！江阴地块数据抓取错误。链接：" + mg.SourceLink + "。地块Id：" + mg.GroundNum + "。错误消息：" + ex.Message);
            }

            this.NavToNext();
        }

        List<KeyValuePair<string, string>> listNum = new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("一","1"),
            new KeyValuePair<string, string>("二","2"),
            new KeyValuePair<string, string>("三","3"),
            new KeyValuePair<string, string>("四","4"),
            new KeyValuePair<string, string>("五","5"),
            new KeyValuePair<string, string>("六","6"),
            new KeyValuePair<string, string>("七","7"),
            new KeyValuePair<string, string>("八","8"),
            new KeyValuePair<string, string>("九","9"),
            new KeyValuePair<string, string>("o","0"),
            new KeyValuePair<string, string>("O","0"),
            new KeyValuePair<string, string>("〇","0")
        };

        /// <summary>
        /// 把小雨100的汉字数字转换成阿拉伯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ReplaceChineseToNum(string str)
        {
            if (str == "十") str = "10";
            else if (str.StartsWith("十")) str = str.Replace("十", "1");
            else str.Replace("十", "");

            foreach (var item in listNum)
            {
                str = str.Replace(item.Key, item.Value);
            }

            return str;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HtmlElement shutPages = Document.GetElementById("shutPages");
            if (shutPages == null||shutPages.InnerText==null)
            {
                this.timer1.Stop();
                this.NavToNext();
                return;
            }
            string strShutPages = shutPages.InnerText;
            if (strShutPages == null) strShutPages = "";
            Match mPage = regPage.Match(strShutPages);
            if (shutPages.InnerText == null || shutPages.InnerText.Trim() == ""||!mPage.Success)
            {
                GetLinks();
                this.timer1.Stop();
                this.NavToNext();
                return;
            }

            if (int.Parse(mPage.Groups[1].Value) < pageCurrent) return;

            GetLinks();

            pageCurrent++;

            try
            {
                HtmlElement next = null;
                foreach (HtmlElement elLink in shutPages.GetElementsByTagName("a"))
                {
                    if (elLink.InnerText.Trim() == "下一页")
                    {
                        next = elLink;
                        break;
                    }
                }
                if (next == null)
                {
                    this.timer1.Stop();
                    this.NavToNext();
                }
                else next.InvokeMember("click");
            }
            catch { }
        }

        private void GetLinks()
        {
            Model_Ground mg = new Model_Ground("江阴");
            try
            {
                HtmlElement tbody = Document.GetElementById("linkTable").Children[0];
                foreach (HtmlElement elLink in tbody.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = elLink.GetAttribute("href");
                        if (regLink.IsMatch(strLink)&&!mg.IsExistLink(strLink))
                        {
                            listLink.Add(new KeyValuePair<string, object>(strLink, null));
                        }
                    }
                    catch { }
                }
            }
            catch(Exception ex) {
                Program.MainForm.AddMessage("错误！抓取江阴挂牌土地链接时出错。链接："+Document.Url.ToString() +"，第"+ pageCurrent + "页。错误消息："+ ex.Message);
            }
        }
    }
}
