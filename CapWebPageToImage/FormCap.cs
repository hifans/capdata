using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CapWebPageToImage
{
    public partial class FormCap : Form
    {
        string[] args;
        public FormCap(string[] args)
        {
            this.args = args;
            InitializeComponent();
        }

        private void FormCap_Load(object sender, EventArgs e)
        {
            string url = "";
            string saveFile = "";
            int height = 4000;

            //url = "http://esf.wuxi.soufun.com/chushou/3_150247682.htm";
            //saveFile = "D:/raisFiles/house_sec/1/2014_03/1.jpg";

            if (args != null && args.Length >= 2)
            {
                try
                {
                    url = args[0];
                    saveFile = args[1];
                    try
                    {
                        height = Convert.ToInt32(args[2]);
                    }
                    catch { }
                    string dir = new System.Text.RegularExpressions.Regex("[^\\\\/]+$").Replace(saveFile, "");
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    GetImage thumb = new GetImage(url, 1024, height, 1024, height);
                    System.Drawing.Bitmap x = thumb.GetBitmap();
                    x.Save(saveFile);
                    Program.Result = 1;
                }
                catch(Exception ex)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter("error_" + DateTime.Today.ToString("yyyy_MM_dd") + ".txt", true);
                    MessageBox.Show(ex.Message);
                    string msg = DateTime.Now.ToString("HH:mm:ss") + "\t\t"+url+"\t\t"+saveFile+"\t\t"+ex.Message+"\r\n\r\n";
                    sw.Write(msg);
                    sw.Close();
                }
            }
            this.Close();
        }
    }
}
