using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapData
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        List<FormBrowser> listForm = new List<FormBrowser>();
        int index = -1;

        private void MainForm_Load(object sender, EventArgs e)
        {
            RASDisplay ras = new RASDisplay();
            ras.Connect("adsl");

            //Fn.UseProxy = false;
            DayOfWeek dow = DateTime.Today.DayOfWeek;
            
            listForm.Add(new WuXi_DailySold());
            listForm.Add(new WuXi_Ground_Presell());
            
            if (dow == DayOfWeek.Sunday) listForm.Add(new WuXi_DailySold2());

            if (dow == DayOfWeek.Thursday) listForm.Add(new WuXi_YanJiu(295, 1, "月度简报"));
            if (dow == DayOfWeek.Friday) listForm.Add(new WuXi_YanJiu(109, 2, "全市行情"));
            if (dow == DayOfWeek.Saturday) listForm.Add(new WuXi_YanJiu(110, 3, "指数图表"));
            if (dow == DayOfWeek.Sunday) listForm.Add(new WuXi_YanJiu(372, 4, "租赁指数"));
            
            if (dow == DayOfWeek.Tuesday) listForm.Add(new WuXi_Ground());
            if (dow == DayOfWeek.Tuesday) listForm.Add(new WuXi_Ground_Sold());
            if (dow == DayOfWeek.Wednesday) listForm.Add(new WuXi_Permit());
            if (dow == DayOfWeek.Thursday) listForm.Add(new JiangYin_Ground());
            if (dow == DayOfWeek.Thursday) listForm.Add(new JiangYin_Ground_Sold());
            if (dow == DayOfWeek.Friday) listForm.Add(new NanTong_Ground());
            if (dow == DayOfWeek.Saturday) listForm.Add(new YiXing_Estate_1());
            if (dow == DayOfWeek.Sunday) listForm.Add(new JiangYin_Estate());
            if (dow == DayOfWeek.Monday) listForm.Add(new NanJing_Estate());
            if (dow == DayOfWeek.Sunday || DateTime.Today.AddDays(1).Day == 1) listForm.Add(new NanJing_EstateSold());//周日或者月底抓一次数据
            if (dow == DayOfWeek.Tuesday) listForm.Add(new XuZhou_Estate());
            if (dow == DayOfWeek.Wednesday) listForm.Add(new NanTong_Estate());
            if (dow == DayOfWeek.Thursday) listForm.Add(new LianYunGang_Estate());
            if (dow == DayOfWeek.Friday) listForm.Add(new HuaiAn_Estate());
            if (dow == DayOfWeek.Saturday) listForm.Add(new ZhenJiang_Estate());
            if (dow == DayOfWeek.Sunday) listForm.Add(new SuZhou_Estate());

            //listForm.Add(new ChangZhou_Estate());//没有好的数据源，暂时不抓
            //listForm.Add(new ZhangJiaGang_Estate());//需要服务器浏览器运行javascript脚本，暂时不抓
            //listForm.Add(new YangZhou_Estate());//需要服务器浏览器运行javascript脚本，暂时不抓

            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320100, "南京"));

            //if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320200, "无锡"));
            //if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320281, "江阴"));
            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320282, "宜兴"));

            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320300, "徐州"));

            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(320400, "常州"));

            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(320500, "苏州"));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(320506, "苏州"));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(320507, "苏州"));
            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320582, "张家港"));
            if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320584, "吴江"));

            //if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320600, "南通"));

            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320700, "连云港"));

            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320800, "淮安"));

            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(321000, "扬州"));

            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(321100, "镇江"));

            //协议出让
            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320100, "南京", 2));
            if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320200, "无锡", 2));
            if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320281, "江阴", 2));
            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320282, "宜兴", 2));
            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320300, "徐州", 2));
            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(320400, "常州", 2));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(320500, "苏州", 2));

            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320582, "张家港", 2));
            if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320584, "吴江", 2));
            if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320600, "南通", 2));
            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320700, "连云港", 2));
            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320800, "淮安", 2));
            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(321000, "扬州", 2));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(321100, "镇江", 2));

            //划拨出让
            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320100, "南京", 3));
            if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320200, "无锡", 3));
            if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320281, "江阴", 3));
            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320282, "宜兴", 3));
            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320300, "徐州", 3));
            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(320400, "常州", 3));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(320500, "苏州", 3));

            if (dow == DayOfWeek.Monday) listForm.Add(new Ground_LandJS(320582, "张家港", 3));
            if (dow == DayOfWeek.Tuesday) listForm.Add(new Ground_LandJS(320584, "吴江", 3));
            if (dow == DayOfWeek.Wednesday) listForm.Add(new Ground_LandJS(320600, "南通", 3));
            if (dow == DayOfWeek.Thursday) listForm.Add(new Ground_LandJS(320700, "连云港", 3));
            if (dow == DayOfWeek.Friday) listForm.Add(new Ground_LandJS(320800, "淮安", 3));
            if (dow == DayOfWeek.Saturday) listForm.Add(new Ground_LandJS(321000, "扬州", 3));
            if (dow == DayOfWeek.Sunday) listForm.Add(new Ground_LandJS(321100, "镇江", 3));

            //抓取法院拍卖网数据
            if (dow == DayOfWeek.Monday) listForm.Add(new House_Court_Auction());

            //二手房数据，每天抓取
            listForm.Add(new WuXi_ErShouFang());


            //无锡新房数据，每天抓取
            listForm.Add(new WuXi_Estate());
            listForm.Add(new WuXi_House_List());
            listForm.Add(new WuXi_House_Info());
            listForm.Add(new WuXi_HouseSold_Check());
            

            this.timer1.Start();
            this.timer1_Tick(this.timer1, EventArgs.Empty);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Program.CountBrowserForm >= 2) return;

            index++;
            if (index >= listForm.Count)
            {
                this.timer1.Stop();
                return;
            }
            listForm[index].Show();
            this.Text = (index + 1).ToString() + " / " + listForm.Count.ToString();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.CountBrowserForm > 0 || index < listForm.Count-1)
            {
                e.Cancel = true;
                return;
            }

            if (this.txtStatus.Text != "")
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "\\"+DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".log");
                sw.Write(this.txtStatus.Text);
                sw.Close();
            }
            //ras.Disconnect();
        }

        public void AddMessage(string msgContent)
        {
            this.txtStatus.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：\n\r" + msgContent + "\n\r\n\r\n\r";
        }

        public bool CheckIsClosed(string className)
        {
            foreach (FormBrowser item in listForm)
            {
                if (item.Name == className)
                {
                    if (item.IsClosed == false) return false;
                    break;
                }
            }
            return true; 
        }
    }
}
