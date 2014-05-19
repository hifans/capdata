using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CapData
{
    public partial class FormBrowser : Form
    {
        public FormBrowser()
        {
            InitializeComponent();
        }

        public List<KeyValuePair<string, object>> listLink = new List<KeyValuePair<string, object>>();
        private int _index = -1;
        private string _title = "";

        /// <summary>
        /// 执行该界面是否需要使用到IE代理，默认为false，可在Form_Load函数中调用Navigate之前设置该值
        /// </summary>
        //protected bool needProxy = false;


        private bool _isClosed = false;
        /// <summary>
        /// 窗口是否已被执行关闭操作
        /// </summary>
        public bool IsClosed
        {
            get { return _isClosed; }
        }

        /// <summary>
        /// listLink中当前链接的Index，-1表示还没有开始访问该列表中内容
        /// </summary>
        public int IndexCurrent { get { return _index; } }

        public KeyValuePair<string, object> linkCurrent
        {
            get { return listLink[_index]; }
        }

        public HtmlDocument Document
        {
            get { return this.Browser.Document; }
        }

        private void FormBrowser_Load(object sender, EventArgs e)
        {
            Program.CountBrowserForm++;
            this._title = this.Text;
            
        }

        private void FormBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            this._isClosed = true;
            //if (this.needProxy) Fn.UseProxy = false;
            Program.CountBrowserForm--;
            Program.MainForm.Close();
        }

        /// <summary>
        /// 打开新的详情链接
        /// </summary>
        protected void NavToNext()
        {
            _index++;
            if (_index >= listLink.Count)
            {
                this.Close();
                return;
            }
            this.Navigate(listLink[_index].Key);
            this.Text = this._title + " - " + (_index + 1).ToString() + " / " + this.listLink.Count;
        }

        protected void Navigate(string Url)
        {     
            this.Browser.Navigate(Url);
            this.timer1.Start();
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.txtURL.Text = Browser.Url.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (this.Browser.Url != null) this.Navigate(this.Browser.Url.ToString());
            else this.NavToNext();
        }

        protected bool UrlInLinkList(string url)
        {
            return listLink.FindAll(delegate(KeyValuePair<string, object> item) { return item.Key == url; }).Count > 0;
        }

        private void Browser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }


        /// <summary>
        /// 获取类名为给定值的第一个元素，如果找不到返回null
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="tagName">元素名</param>
        /// <returns></returns>
        public HtmlElement GetElementByClassName(string className, string tagName)
        {
            return this.GetElementByClassName(className, tagName, 0);
        }

        /// <summary>
        /// 获取类名为给定值的第一个元素，如果找不到返回null
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public HtmlElement GetElementByClassName(string className)
        {
            return GetElementByClassName(className, null);
        }

        /// <summary>
        /// 获取给名为给定值的第n个元素，如果找不到则返回null
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="tagName"></param>
        /// <param name="index">第n个元素，n值从0开始</param>
        /// <returns></returns>
        public HtmlElement GetElementByClassName(string className, string tagName, int index)
        {
            if (Document == null) return null;


            HtmlElementCollection coll = (tagName == null || tagName == "") ? Document.All : Document.GetElementsByTagName(tagName);

            int i = 0;
            foreach (HtmlElement item in coll)
            {
                if (item.GetAttribute("classname") == className) {
                    if (i >= index) return item;
                    else i++;
                }
            }

            return null;
        }

        private int countRefresh = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            countRefresh++;
            if (countRefresh > 3)
            {
                countRefresh = 0;
                this.timer1.Stop();
                this.NavToNext();
            }
            else this.btnRefresh_Click(this.btnRefresh, EventArgs.Empty);
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (Document.Url != e.Url) return;
            this.timer1.Stop();
            countRefresh = 0;
        }
    }
}
