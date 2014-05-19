using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class WuXi_HouseSold_Check : CapData.FormBrowser
    {
        public WuXi_HouseSold_Check()
        {
            InitializeComponent();
        }

        private void WuXi_HouseSold_Check_Load(object sender, EventArgs e)
        {
            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            string tName = Fn.GetHouseInfoTable(Fn.GetCityIdByName("无锡"));
            DataTable dt = Fn.GetDT_MySQL("select a.id, a.houseid, b.estnum, b.id as pid from " + tName + " as a left join permitinfo as b on a.permitid=b.id where a.solddate>='" + DateTime.Today.AddDays(-3).ToString("yyyy-MM-dd") + "' and soldLock=false;");

            foreach (DataRow dr in dt.Rows)
            {
                string sql = "update " + tName + " set SoldDate='1900-01-01',SoldPrice=0 where Id='" + dr[0].ToString() + "';";
                sql += "update permitinfo set SoldOut=0 where id=" + dr["pid"].ToString()+";";
                listLink.Add(new KeyValuePair<string, object>("http://www.wxhouse.com/house/hsview.html?houseid=" + dr[2].ToString() + "&hsid=" + dr[1].ToString(), sql));
            }

            this.NavToNext();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Document.Url) return;

            if (Document.Body.GetElementsByTagName("td").Count>0)
            {
                try
                {
                    Fn.ExecNonQuery_MySQL(linkCurrent.Value.ToString());
                }
                catch { }
            }

            this.NavToNext();
        }
    }
}
