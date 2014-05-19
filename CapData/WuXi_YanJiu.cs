using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace CapData
{
    public partial class WuXi_YanJiu : CapData.FormBrowser
    {
        int page = 0;
        Regex regLink = new Regex("http://www.wxhouse.com/YanJiu/\\d+\\.html");
        Regex regMonth = new Regex("^\\d{4}年\\d{1,2}月");

        Regex regFilterLinkStart = new Regex("<a[^>]*?>", RegexOptions.IgnoreCase);
        Regex regFilterLinkEnd = new Regex("</a>", RegexOptions.IgnoreCase);

        string fileDir = "";
        string imgDir = "";


        int UrlTypeId = 0;
        int DBTypeId = 0;
        string Title = "";

        int CityId = Fn.GetCityIdByName("无锡");
        /// <summary>
        /// 初始化一个抓取无锡房地产市场网研究数据的窗口
        /// </summary>
        /// <param name="urlTypeId">链接中的TypeId</param>
        /// <param name="dbTypeId">数据库中的TypeId，各值请查看系统const.php文件中以CAP_CONTENT_WUXI_开头的各值。</param>
        /// <param name="title">弹出窗口标题</param>
        public WuXi_YanJiu(int urlTypeId, int dbTypeId, string title)
        {
            this.UrlTypeId = urlTypeId;
            this.DBTypeId = dbTypeId;
            this.Text = title;
            InitializeComponent();
        }

        private void WuXi_YanJiu_Load(object sender, EventArgs e)
        {
            //fileDir = System.Configuration.ConfigurationSettings.AppSettings["fileDir"] + "WuXi_YanJiu\\" + DBTypeId + "\\";
            fileDir = Fn.GetConfValue("fileDir") + "WuXi_YanJiu\\" + DBTypeId + "\\";
            if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
            //imgDir = System.Configuration.ConfigurationSettings.AppSettings["WuXi_YanJiuImgDir"].TrimEnd('\\') + "\\";
            imgDir = Fn.GetConfValue("WuXi_YanJiuImgDir").TrimEnd('\\') + "\\";
            if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);

            this.Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            this.NavToNextPage();
        }

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != Browser.Url) return;
            if (e.Url.ToString().StartsWith("http://www.wxhouse.com/YanJiu/list.html?PageIndex="))
            {
                foreach (HtmlElement link in Document.GetElementsByTagName("a"))
                {
                    try
                    {
                        string strLink = link.GetAttribute("href");
                        if (regLink.IsMatch(strLink) && Fn.GetDT_MySQL("select * from city_capturecontent where SourceLink='" + strLink + "' limit 1").Rows.Count == 0) listLink.Add(new KeyValuePair<string, object>(strLink, link.InnerText.Replace("[图文]","").Trim()));
                    }
                    catch { }
                }
                this.NavToNextPage();
                return;
            }

            try
            {
                DateTime dateRelease = DateTime.Today;
                foreach (HtmlElement span in Document.GetElementsByTagName("span"))
                {
                    if (span.GetAttribute("classname") == "div_intro")
                    {
                        dateRelease = Convert.ToDateTime("20" + span.InnerText);
                        break;
                    }
                }

                string fileName = "";

                Random rand = new Random();
                do
                {
                    fileName = dateRelease.ToString("yyyy_MM_dd_") + rand.Next(10000, 100000) + ".txt";
                } while (File.Exists(fileDir + fileName));

                string filePath = fileDir + fileName;
                

                HtmlElement content = Document.GetElementById("content");
                foreach (HtmlElement img in content.GetElementsByTagName("img"))
                {
                    try
                    {
                        string src = img.GetAttribute("src");
                        string imgName = src.Replace("/UploadFiles/", "$").Split('$')[1];
                        string[] tmp = imgName.Split('/');
                        string imgDirTmp = imgDir;
                        for (int i = 0; i < tmp.Length - 1; i++)
                        {
                            imgDirTmp += tmp[i] + "\\";
                            
                        }
                        if (!Directory.Exists(imgDirTmp)) Directory.CreateDirectory(imgDirTmp);

                        if (!File.Exists(imgDirTmp + tmp[tmp.Length - 1])) Fn.DownloadFile(src, tmp[tmp.Length - 1], imgDirTmp);
                        img.SetAttribute("src", "{site_tfimg}".TrimEnd('/') + "/WuXi_YanJiu/" + imgName);
                    }
                    catch { }
                }

                string strCont = content.InnerHtml;
                strCont = regFilterLinkStart.Replace(strCont, "");
                strCont = regFilterLinkEnd.Replace(strCont, "");

                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(strCont);
                sw.Close();

                string sql = "insert into city_capturecontent(CityId, ContType,Title,FileName, IssueDate,SourceLink) values (";
                sql += "'" + this.CityId + "','" + this.DBTypeId + "','" + this.linkCurrent.Value.ToString()+"','"+fileName+"',";
                sql += "'" + dateRelease.ToString("yyyy-MM-dd") + "','" + Document.Url.ToString() + "');";
                Fn.ExecNonQuery_MySQL(sql);
                
            }
            catch { }

            this.NavToNext();
        }

        private void NavToNextPage()
        {
            page++;
            if (page > 5) { this.NavToNext(); return; }
            this.Navigate("http://www.wxhouse.com/YanJiu/list.html?PageIndex=" + page + "&TypeID="+this.UrlTypeId);
        }
    }
}
