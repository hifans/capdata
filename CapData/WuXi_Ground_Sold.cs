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
    public partial class WuXi_Ground_Sold : CapData.FormBrowser
    {
        public WuXi_Ground_Sold()
        {
            InitializeComponent();
        }

        List<KeyValuePair<string, string>> listColName = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("地块编号","GroundNum"),
            new KeyValuePair<string, string>("编号","GroundNum"),
            new KeyValuePair<string, string>("竞得人","Buyer"),
            new KeyValuePair<string, string>("竞得单位","Buyer"),
            new KeyValuePair<string, string>("成交价格","ClinchPrice"),
            new KeyValuePair<string, string>("成交价","ClinchPrice")
        };

        Regex regUrl = new Regex("http://gtj\\.wuxi\\.gov\\.cn/BA13/D/05/index(_\\d+)?\\.shtml", RegexOptions.IgnoreCase);
        Regex regLink = new Regex("http://gtj\\.wuxi\\.gov\\.cn/BA13/D/05/\\d+\\.shtml", RegexOptions.IgnoreCase);

        Regex regColNameFilter = new Regex("[\\(（][^\\(（]+[\\)）]");

        Regex regGroundNum = new Regex("^(锡新?((国土)|(地))|XDG)([\\(（][工经][）\\)])?\\d{4}-\\d+号?", RegexOptions.IgnoreCase);

        List<string> listSaveFields = new List<string>() { 
            "ClinchDate","ClinchPrice","Buyer","ClinchLink"
        };

        Regex regSoldInfo = new Regex("((锡新?((国土)|(地))|XDG)([\\(（][工经][）\\)])?\\d{4}-\\d+)(.*?)以(([\\d]+)([万亿])元)", RegexOptions.IgnoreCase);
        int page = 1;
        private void WuXi_Ground_Sold_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
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
                        this.Navigate("http://gtj.wuxi.gov.cn/BA13/D/05/index_"+page.ToString()+".shtml");
                    }
                }
                catch { this.NavToNext(); }
                return;
            }

            Model_Ground mg = new Model_Ground("无锡");
            mg.ClinchLink = Document.Url.ToString();
            //开始计算成交日期
            foreach (HtmlElement td in Document.GetElementsByTagName("td"))
            {
                if (td.InnerText!=null&&td.InnerText.Trim() == "公开日期")
                {
                    try
                    {
                        mg.ClinchDate = DateTime.Parse(td.NextSibling.InnerText);
                    }
                    catch { }
                    break;
                }
            }

            HtmlElement zoom = Document.GetElementById("Zoom");
            HtmlElement tRow = null;
            foreach (HtmlElement td in zoom.GetElementsByTagName("td"))
            {
                try
                {
                    string str = td.InnerText.Replace("\n","").Replace("\r","").Replace(" ","").Replace("　","").Trim();
                    if (str == "编号" || str == "地块编号")
                    {
                        tRow = td.Parent;
                        break;
                    }
                }
                catch { }
            }

            //开始计算每一条成交信息
            mg.ClinchLink = linkCurrent.Key;
            try
            {
                if (tRow != null)
                {

                    List<string> arrCol = new List<string>();
                    foreach (HtmlElement td in tRow.Children)
                    {
                        string str = td.InnerText.Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("　", "").Trim();
                        str = regColNameFilter.Replace(str, "");
                        KeyValuePair<string, string> kvCol;
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

                    Type type = typeof(Model_Ground);
                    HtmlElement tr = tRow.NextSibling;
                    while (tr != null)
                    {
                        if (tr.Children.Count == 0) break;
                        string strGroundNum = tr.Children[0].InnerText;
                        if (strGroundNum == null) break;
                        strGroundNum = strGroundNum.Replace("\r", "").Replace("\n", "").Trim();
                        Match mGroundNum = regGroundNum.Match(strGroundNum);
                        if (!mGroundNum.Success)
                        {
                            tr = tr.NextSibling;
                            continue;
                        }
                        mg.GroundNum = mGroundNum.Value;
                        mg.ClinchPrice = 0;
                        for (int i = 0; i < arrCol.Count; i++)
                        {
                            if (arrCol[i] == "" || arrCol[i] == null) continue;
                            string strValue = tr.Children[i].InnerText;
                            if (strValue != null)
                            {
                                strValue = strValue.Replace("\n", "").Replace("\r", "").Trim();
                            }
                            FieldInfo info = type.GetField(arrCol[i]);
                            Type t = info.FieldType;
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
                            catch { continue; }
                        }
                        mg.ClinchPrice *= 10000;
                        if (mg.ClinchPrice > 0) mg.Save(true, listSaveFields);

                        tr = tr.NextSibling;
                    }


                }
                else
                {
                    MatchCollection mc = regSoldInfo.Matches(zoom.InnerText);
                    foreach (Match m in mc)
                    {
                        try
                        {
                            mg.GroundNum = m.Groups[1].Value;
                            mg.Buyer = m.Groups[7].Value;
                            if (mg.Buyer.IndexOf("由") >= 0) mg.Buyer = mg.Buyer.Split('由')[1];
                            mg.ClinchPrice = double.Parse(m.Groups[9].Value)*10000;
                            if (m.Groups[10].Value == "亿") mg.ClinchPrice *= 10000;
                            mg.Update(listSaveFields);
                        }
                        catch { }
                    }
                }
            }
            catch { }
            this.NavToNext();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(WuXi_Ground).Name)) return;
            this.timer1.Stop();
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.Navigate("http://gtj.wuxi.gov.cn/BA13/D/05/index.shtml");
        }
    }
}
