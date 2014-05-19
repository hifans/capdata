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
    public partial class WuXi_House_Info : CapData.FormBrowser
    {
        int cityId = Fn.GetCityIdByName("无锡");
        string tName_HouseInfo = "";

        Regex regSalePrice = new Regex(@"(\d+(\.\d+)?)元/平米");

        public WuXi_House_Info()
        {
            InitializeComponent();
        }

        private void WuXi_House_Info_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.timerWait.Start();
            this.Text += "（WuXi_House_List窗口运行结束之后该窗口将自动运行，如无必要，请勿关闭窗口）";
        }

        

        private void Start()
        {
            this.Text = "WuXi_House_Info";
            this.tName_HouseInfo = Fn.GetHouseInfoTable(cityId);
            DataTable dt = Fn.GetDT_MySQL("select t1.id, t1.HouseId, t2.EstNum from " + tName_HouseInfo + " as t1 left join permitinfo as t2 on t1.PermitId=t2.Id  where t1.area=-1");
            foreach (DataRow dr in dt.Rows)
            {
                this.listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/house/hsview.html?houseid="+dr[2].ToString()+"&hsid="+dr[1].ToString(), dr[0]));
            }

            this.NavToNext();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != Document.Url.ToString()) return;
            string sql = "update "+ tName_HouseInfo + " set ";
            try
            {
                HtmlElement tbody = GetElementByClassName("color_960", "td").Parent.Parent;
                foreach (HtmlElement tr in tbody.Children)
                {
                    if (tr.Children.Count < 2) continue;
                    switch (tr.Children[0].InnerText.Trim())
                    {
                        case "幢号：":
                            sql += "BuildNum='" + tr.Children[1].InnerText + "',";
                            break;

                        case "单元：":
                            sql += "UnitNum='" + tr.Children[1].InnerText + "',";
                            break;

                        case "室号：":
                            sql += "RoomNum='" + tr.Children[1].InnerText + "',";
                            break;

                        case "所在层：":
                            try { sql += "Layer='" + int.Parse(tr.Children[1].InnerText) + "',"; }
                            catch { }
                            break;

                        case "总层数：":
                            try { sql += "LayerAll='" + int.Parse(tr.Children[1].InnerText) + "',"; }
                            catch { }
                            break;

                        case "户型：":
                            sql += "Structure='" + tr.Children[1].InnerText + "',";
                            break;

                        case "用途：":
                            sql += "`Usage`='" + tr.Children[1].InnerText + "',";
                            break;

                        case "建筑面积：":
                            try { sql += "Area='" + double.Parse(tr.Children[1].InnerText.Replace("㎡", "").Trim()) + "',"; }
                            catch { }
                            break;

                        case "套内面积：":
                            try { sql += "AreaInner='" + double.Parse(tr.Children[1].InnerText.Replace("㎡", "").Trim()) + "',"; }
                            catch { }
                            break;

                        case "分摊面积：":
                            try { sql += "AreaOuter='" + double.Parse(tr.Children[1].InnerText.Replace("㎡", "").Trim()) + "',"; }
                            catch { }
                            break;
                    }
                }

                foreach (HtmlElement td in Document.GetElementsByTagName("td"))
                {
                    if (td.InnerText == null) continue;
                    Match m = regSalePrice.Match(td.InnerText);
                    if (!m.Success) continue;
                    sql += "ListPrice='" + double.Parse(m.Groups[1].Value) + "',";
                    break;
                }
                sql = sql.Trim(',')+" where id='"+linkCurrent.Value.ToString()+"';";
                Fn.ExecNonQuery_MySQL(sql);
            }
            catch { }

            this.NavToNext();
        }

        private void timerWait_Tick(object sender, EventArgs e)
        {
            if (!Program.MainForm.CheckIsClosed(typeof(WuXi_House_List).Name)) return;
            this.timerWait.Stop();
            
            Start();
        }
    }
}
