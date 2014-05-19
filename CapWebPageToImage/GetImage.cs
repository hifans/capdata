using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CapWebPageToImage
{
    public class GetImage
    {
        private int S_Height;
        private int S_Width;
        private int F_Height;
        private int F_Width;
        private string MyURL;
        //private WebBrowser browser;

        public int ScreenHeight
        {
            get { return S_Height; }
            set { S_Height = value; }
        }

        public int ScreenWidth
        {
            get { return S_Width; }
            set { S_Width = value; }
        }

        public int ImageHeight
        {
            get { return F_Height; }
            set { F_Height = value; }
        }

        public int ImageWidth
        {
            get { return F_Width; }
            set { F_Width = value; }
        }

        public string WebSite
        {
            get { return MyURL; }
            set { MyURL = value; }
        }

        public GetImage(string WebSite, int ScreenWidth, int ScreenHeight, int ImageWidth, int ImageHeight)
        {
            this.WebSite = WebSite;
            this.ScreenWidth = ScreenWidth;
            this.ScreenHeight = ScreenHeight;
            this.ImageHeight = ImageHeight;
            this.ImageWidth = ImageWidth;
            //this.browser = browser;
        }

        public Bitmap GetBitmap()
        {
            WebPageBitmap Shot = new WebPageBitmap(this.WebSite, this.ScreenWidth, this.ScreenHeight);
            Shot.GetIt();
            Bitmap Pic = Shot.DrawBitmap(this.ImageHeight, this.ImageWidth);
            return Pic;
        }
    }

    class WebPageBitmap
    {
        WebBrowser MyBrowser;
        string URL;
        int Height;
        int Width;
        Timer timer1 = new Timer();
        bool flagFail = false;

        public WebPageBitmap(string url, int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.URL = url;
            MyBrowser = new WebBrowser();
            MyBrowser.ScriptErrorsSuppressed = true;
            MyBrowser.ScrollBarsEnabled = false;
            MyBrowser.Size = new Size(this.Width, this.Height);
            this.timer1.Interval = 25000;
            this.timer1.Tick += new EventHandler(timer1_Tick);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.flagFail = true;
        }

        public void GetIt()
        {
            MyBrowser.Navigate(this.URL);
            this.timer1.Start();
            while (MyBrowser.ReadyState != WebBrowserReadyState.Complete&&flagFail==false)
            {
                Application.DoEvents();
            }

            if (flagFail)
            {
                MyBrowser.Stop();
                MyBrowser.Dispose();
                MyBrowser = null;
            }
        }

        public Bitmap DrawBitmap(int theight, int twidth)
        {
            if (flagFail) return null;
            Bitmap myBitmap = new Bitmap(Width, Height);
            Rectangle DrawRect = new Rectangle(0, 0, Width, Height);
            MyBrowser.DrawToBitmap(myBitmap, DrawRect);
            System.Drawing.Image imgOutput = myBitmap;
            System.Drawing.Image oThumbNail = new Bitmap(twidth, theight, imgOutput.PixelFormat);
            Graphics g = Graphics.FromImage(oThumbNail);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            Rectangle oRectangle = new Rectangle(0, 0, twidth, theight);
            g.DrawImage(imgOutput, oRectangle);
            try
            {
                return (Bitmap)oThumbNail;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                imgOutput.Dispose();
                imgOutput = null;
                MyBrowser.Dispose();
                MyBrowser = null;
            }
        }
    }
}
