using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CapData
{
    public partial class JiangYin_Ground_Sold : CapData.FormBrowser
    {
        public JiangYin_Ground_Sold()
        {
            InitializeComponent();
        }

        Regex regPage = new Regex("当前第  (\\d+)  页");
        Regex regLink = new Regex("http://gtj\\.jiangyin\\.gov\\.cn/jygt/sitePages/subPages/\\d+\\.jsp\\?sourceChannelId=7155&did=253122", RegexOptions.IgnoreCase);
        Regex regColNameFilter = new Regex("[\\(（][^\\(（]+[\\)）]");

        List<string> listUpdateFields = new List<string>() { "ClinchDate", "ClinchPrice", "Buyer", "ClinchLink" };

        private int pageCurrent = 1;

        List<KeyValuePair<string, string>> listColName = new List<KeyValuePair<string, string>>() {
            new KeyValuePair<string, string>("地块编号","GroundNum"),
            new KeyValuePair<string, string>("宗地编号","GroundNum"),
            new KeyValuePair<string, string>("竞买时间","ClinchDate"),
            new KeyValuePair<string, string>("成交时间","ClinchDate"),
            new KeyValuePair<string, string>("成交价","ClinchPrice"),
            new KeyValuePair<string, string>("成交价总价","ClinchPrice"),
            new KeyValuePair<string, string>("成交价总价(万元)","ClinchPrice"),
            new KeyValuePair<string, string>("竞得人","Buyer"),
            new KeyValuePair<string, string>("竞得单位","Buyer")
        };

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(JiangYin_Ground).Name)) return;
            this.timer1.Stop();
            this.Text = "JiangYin_Ground_Sold";
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://gtj.jiangyin.gov.cn/jygt/sitePages/channelPages/135034050062404.jsp");
            //this.Navigate("http://gtj.jiangyin.gov.cn/jygt/sitePages/subPages/1350340007342917.jsp?sourceChannelId=7155&did=253122");
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;
            if (e.Url.ToString() == "http://gtj.jiangyin.gov.cn/jygt/sitePages/channelPages/135034050062404.html")
            {
                this.timer2.Interval = 3000;
                this.timer2.Start();
                return;
            }

            Model_Ground mg = new Model_Ground("江阴");
            //开始计算信息包括的字段
            List<string> arrCol = new List<string>();
            HtmlElement tRow = null;

            List<HtmlElement> listTRowSub = new List<HtmlElement>();
            List<int> listSubIndex = new List<int>();

            int headRowSpan = 1;
            try
            {
                foreach (HtmlElement td in Document.GetElementById("lk_wcms_edit_pictext701content").GetElementsByTagName("td"))
                {
                    try
                    {
                        string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        if (str == "宗地编号" || str == "地块编号")
                        {
                            tRow = td.Parent;
                            break;
                        }
                    }
                    catch { }
                }

                foreach (HtmlElement td in tRow.Children)
                {
                    int rowSpan = int.Parse(td.GetAttribute("rowspan"));
                    if (rowSpan > headRowSpan) headRowSpan = rowSpan;
                }

                for (int i = 1; i < headRowSpan; i++)
                {
                    listTRowSub.Add(listTRowSub.Count == 0 ? tRow.NextSibling : listTRowSub[listTRowSub.Count - 1].NextSibling);
                    listSubIndex.Add(0);
                }

                
                foreach (HtmlElement td in tRow.Children)
                {
                    int rowSpan = int.Parse(td.GetAttribute("rowspan"));
                    int colSpan = int.Parse(td.GetAttribute("colspan"));
                    string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                    KeyValuePair<string, string> kvCol;
                    string strTop = regColNameFilter.Replace(str, "");
                    if (colSpan == 1)
                    {
                        str = strTop;
                        for (int i = rowSpan-1; i < headRowSpan-1; i++)
                        {
                            str += listTRowSub[i].Children[listSubIndex[i]].InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                            listSubIndex[i]++;
                        }
                        try
                        {
                            kvCol = listColName.FindAll(delegate(KeyValuePair<string, string> kv) { return kv.Key == str; })[0];
                            arrCol.Add(kvCol.Value);
                        }
                        catch
                        {
                            arrCol.Add("");
                        }
                        continue;
                    }

                    HtmlElement tRowSub = listTRowSub[rowSpan - 1];
                    for (int i = 0; i < colSpan; i++)
                    {
                        str = strTop;
                        for (int j = rowSpan - 1; j < headRowSpan - 1; j++)
                        {
                            str += listTRowSub[j].Children[listSubIndex[j]].InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                            listSubIndex[j]++;
                        }
                        //str = strTop + tRowSub.Children[i + listSubIndex[rowSpan-1]].InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        //str = regColNameFilter.Replace(str, "");
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
                    //listSubIndex[rowSpan - 1] += colSpan;
                }
            }
            catch { this.NavToNext(); return; }

            mg.ClinchLink = Document.Url.ToString().Replace(".html",".jsp");

            int[] arrRowSpan = new int[arrCol.Count];

            try
            {
                Type type = typeof(Model_Ground);
                HtmlElement tr = (listTRowSub.Count == 0 ? tRow : listTRowSub[listTRowSub.Count-1]).NextSibling;
                while (tr != null)
                {
                    int offset = 0;
                    for (int i = 0; i < arrCol.Count; i++)
                    {
                        int rowSpan = 0;
                        try
                        {
                            rowSpan = int.Parse(tr.Children[i - offset].GetAttribute("rowspan")) - 1;
                        }
                        catch { }

                        if (arrCol[i] == "" || arrCol[i] == null)
                        {
                            if (arrRowSpan[i] > 0)
                            {
                                offset++;
                                arrRowSpan[i]--;
                            }
                            if (rowSpan > 0) arrRowSpan[i] = rowSpan;
                            continue;
                        }

                        if (arrRowSpan[i] > 0)
                        {
                            offset++;
                            arrRowSpan[i]--;
                            continue;
                        }

                        if (rowSpan > 0) arrRowSpan[i] = rowSpan;
 
                        string strValue = tr.Children[i - offset].InnerText;
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
                                catch {
                                    info.SetValue(mg, 0);
                                }
                            }
                            else if (t == typeof(double))
                            {
                                try
                                {
                                    info.SetValue(mg, double.Parse(strValue));
                                }
                                catch {
                                    info.SetValue(mg, 0);
                                }
                            }
                            else if (t == typeof(DateTime))
                            {
                                if (strValue.IndexOf("～") >= 0) strValue = strValue.Split('～')[1];
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
                    mg.ClinchPrice *= 10000;
                    if(mg.ClinchPrice>0) mg.Update(listUpdateFields);
                    tr = tr.NextSibling;
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.AddMessage("错误！江阴地块成交数据抓取错误。链接：" + mg.ClinchLink + "错误Id：" + mg.GroundNum + "。错误消息：" + ex.Message);
            }

            this.NavToNext();
        }

        private void JiangYin_Ground_Sold_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
            this.Text += "（注意：地块挂牌数据抓取完毕之后该部分会自动运行，如无必要，请不要关闭该窗口！）";
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            HtmlElement shutPages = Document.GetElementById("shutPages");
            if (shutPages == null)
            {
                this.timer1.Stop();
                this.NavToNext();
                return;
            }
            string strShutPages = shutPages.InnerText;
            if (strShutPages == null) strShutPages = "";
            Match mPage = regPage.Match(strShutPages);
            if (shutPages.InnerText == null || shutPages.InnerText.Trim() == "" || !mPage.Success)
            {
                GetLinks();
                this.timer2.Stop();
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
                    this.timer2.Stop();
                    this.NavToNext();
                }
                else next.InvokeMember("click");
            }
            catch { }
        }

        private void GetLinks()
        {
            try
            {
                HtmlElement tbody = Document.GetElementById("linkTable").Children[0];
                foreach (HtmlElement elLink in tbody.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = elLink.GetAttribute("href");
                        if (regLink.IsMatch(strLink))
                        {
                            listLink.Add(new KeyValuePair<string, object>(strLink, null));
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Program.MainForm.AddMessage("错误！抓取江阴土地成交链接时出错。链接：" + Document.Url.ToString() + "，第" + pageCurrent + "页。错误消息：" + ex.Message);
            }
        }
    }
}
