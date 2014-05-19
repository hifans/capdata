using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Npgsql;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CapData
{
    /// <summary>
    /// 本软件使用到的数据库类型
    /// </summary>
    public enum DataBaseType
    {
        /// <summary>
        /// MySQL数据库
        /// </summary>
        MySQL,
        /// <summary>
        /// Postgresql
        /// </summary>
        Postgresql
    }

    public enum FileType:int
    {
        /// <summary>
        /// 普通图件
        /// </summary>
        Image = 1,
        /// <summary>
        /// 普通文件
        /// </summary>
        File = 2,
        /// <summary>
        /// 楼盘套型图
        /// </summary>
        Image_Struct=3,
        /// <summary>
        /// 楼盘效果图
        /// </summary>
        Image_Virtual=4
    }
    public static class Fn
    {
        private static MySqlConnection connMy = null;
        private static NpgsqlConnection connPg = null;
        private static MySqlCommand cmdMy = null;
        private static NpgsqlCommand cmdPg = null;


        ///// <summary>
        ///// 获取或设置是否使用IE代理
        ///// </summary>
        //public static bool UseProxy
        //{
        //    get { return _useProxy; }
        //    set
        //    {
        //        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
        //        rk.SetValue("ProxyEnable", value ? 1 : 0);
        //        if (value) rk.SetValue("ProxyServer", Fn.GetConfValue("ProxyServer"));
        //        rk.Close();
        //        _useProxy = value;
        //} }

        public static bool InitConn()
        {
            try
            {
                //connMy = new MySqlConnection(System.Configuration.ConfigurationSettings.AppSettings["connStrMy"]);
                //connPg = new NpgsqlConnection(System.Configuration.ConfigurationSettings.AppSettings["connStrPg"]);
                string strConnMy = GetConfValue("connStrMy");
                string strConnPg = GetConfValue("connStrPG");
                connMy = new MySqlConnection(strConnMy);
                connPg = new NpgsqlConnection(strConnPg);
                connMy.Open();
                connPg.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接初始化失败！\n" + ex.Message, "提示");
                return false;
            }


            cmdMy = new MySqlCommand("", connMy);
            cmdPg = new NpgsqlCommand("set names 'gbk';", connPg);
            cmdPg.ExecuteNonQuery();

            return true;
        }

        public static void CloseConn()
        {
            connMy.Close();
            connPg.Close();
        }

        public static string GetConfValue(string key)
        {
            string path = Application.StartupPath + "\\conf.xml";
            if (!File.Exists(path)) path = Application.StartupPath + "\\..\\..\\conf.xml";
            if (!File.Exists(path)) return "";

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode node = doc.SelectSingleNode("configuration").ChildNodes[0];
            foreach (XmlNode item in node.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;
                if (xe.GetAttribute("key") == key) return xe.GetAttribute("value");
            }
            return "";
        }

        public static DataTable GetDT_MySQL(string SQL)
        {
            MySqlDataAdapter mda = new MySqlDataAdapter(SQL, connMy);
            DataTable dt = new DataTable();
            mda.Fill(dt);
            return dt;
        }

        public static object GetObj_MySQL(string SQL)
        {
            cmdMy.CommandText = SQL;
            return cmdMy.ExecuteScalar();
        }

        public static int ExecNonQuery_MySQL(string SQL)
        {
            cmdMy.CommandText = SQL;
            return cmdMy.ExecuteNonQuery();
        }

        public static DataTable GetDT_PgSQL(string SQL)
        {
            NpgsqlDataAdapter dpt = new NpgsqlDataAdapter(SQL, connPg);
            DataTable dt = new DataTable();
            dpt.Fill(dt);
            return dt;
        }

        public static int GetCount_MySQL(string tableName, string sFilter)
        {
            return int.Parse(Fn.GetDT_MySQL("select count(*) from " + tableName + " where " + sFilter).Rows[0][0].ToString());
        }

        public static int GetCount_PgSQL(string tableName, string sFilter)
        {

            return int.Parse(Fn.GetDT_PgSQL("select count(*) from " + tableName + " where " + sFilter).Rows[0][0].ToString());
        }

        public static object GetObj_PgSQL(string SQL)
        {
            cmdPg.CommandText = SQL;
            return cmdPg.ExecuteScalar();
        }

        public static int ExecNonQuery_PgSQL(string SQL)
        {
            cmdPg.CommandText = SQL;
            return cmdPg.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取长度为8位的随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRandomStr()
        {
            return GetRandomStr(8);
        }

        /// <summary>
        /// 获取指定长度的字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GetRandomStr(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            string str = "";
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars[rand.Next(26)].ToString();
            }
            return str;
        }

        /// <summary>
        /// MySQL数据库中处理判断条件为Equal时的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string KW_Equal(string str)
        {
            if (str == null) return str;
            string[] cl = { "\\", "'" };
            foreach (var item in cl)
            {
                str = str.Replace(item, "\\" + item);
            }
            return str;
        }

        /// <summary>
        /// 根据城市名称获取城市编号，如果城市不存在，则返回-1
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public static int GetCityIdByName(string cityName)
        {
            try
            {
                return Convert.ToInt32(Fn.GetDT_MySQL("select Id from sys_city where cname='" + Fn.KW_Equal(cityName) + "'").Rows[0][0].ToString());
            }
            catch { return -1; }
        }

        /// <summary>
        /// MySQL数据库中处理判断条件为Like时的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string KW_Like(string str)
        {
            if (str == null) return str;
            string[] cl = { "\\", "'", "%", "_" };
            foreach (var item in cl)
            {
                str = str.Replace(item, "\\" + item);
            }
            return str;
        }

        public static bool DownloadFile(string fileLink, string filename, string fileDir)
        {
            if (!System.IO.Directory.Exists(fileDir)) System.IO.Directory.CreateDirectory(fileDir);
            //string filename = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + (new Random()).Next(1000, 10000);
            WebRequest req = null;
            try
            {
                req = HttpWebRequest.Create(fileLink);
                req.Timeout = 20000;

                WebResponse res = req.GetResponse();
                if (res.ContentType.ToLower().StartsWith("text/")) { res.Close(); return false; }
                //string[] tmp = res.ResponseUri.AbsolutePath.Split('.');
                //filename += "." + tmp[tmp.Length - 1];
                System.IO.Stream stream = res.GetResponseStream();
                int fileLength = (int)res.ContentLength;

                BinaryReader br = new System.IO.BinaryReader(stream);
                FileStream fs = File.Create(fileDir + filename);
                try
                {
                    if (fileLength > 0) fs.Write(br.ReadBytes(fileLength), 0, fileLength);
                    else
                    {
                        while (true)
                        {
                            byte[] bt = br.ReadBytes(10000);
                            if (bt.Length > 0)
                            {
                                fs.Write(bt, 0, bt.Length);
                            }
                            if (bt.Length < 10000) break;
                        }
                    }
                }
                catch { }
                br.Close();
                fs.Close();

                return true;

            }
            catch { return false; }
        }

        public static bool DownloadFile(string fileLink, string filename, FileType ft)
        {
            //string strFileDir = System.Configuration.ConfigurationSettings.AppSettings["fileDir"].Trim('\\')+"\\"+ft.ToString()+"\\";
            string strFileDir = Fn.GetConfValue("fileDir").Trim('\\') + "\\" + ft.ToString() + "\\";
            return DownloadFile(fileLink, filename, strFileDir);
        }

        public static bool IsExistGroundFile(string fileName, FileType ft)
        {
            return Fn.GetDT_PgSQL("select * from ground_file where FileName='" + fileName + "' and FileType=" + Convert.ToInt32(ft)).Rows.Count > 0;
            //string strFileDir = System.Configuration.ConfigurationSettings.AppSettings["fileDir"].Trim('\\') + "\\" + ft.ToString() + "\\";
            //return File.Exists(strFileDir + fileName);
        }

        public static bool IsExistEstateFile(string fileName, FileType ft)
        {
            return Fn.GetDT_PgSQL("select * from estate_file where FileName='" + fileName + "' and FileType=" + Convert.ToInt32(ft)).Rows.Count > 0;
            //string strFileDir = System.Configuration.ConfigurationSettings.AppSettings["fileDir"].Trim('\\') + "\\" + ft.ToString() + "\\";
            //return File.Exists(strFileDir + fileName);
        }

        /// <summary>
        /// 楼盘数据库中是否存在给定的链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsExistEstUrl(string url)
        {
            return Fn.GetDT_PgSQL("select * from estate_info where sourcelink='" + url + "'").Rows.Count > 0;
        }

        /// <summary>
        /// 保存楼盘相关文件
        /// </summary>
        /// <param name="cityId">城市Id</param>
        /// <param name="estNum">楼盘编号</param>
        /// <param name="imageLink">图片链接</param>
        public static void SaveEstateFile(int cityId, string estNum, string fileLink, string fileTitle, FileType ft)
        {
            Model_Estate_File mef = new Model_Estate_File(cityId, estNum);
            if (fileLink != null && fileLink != "" && mef.EstateId > 0)
            {
                try
                {
                    string[] tmp = fileLink.Split('/');
                    string strFileName = cityId + "_" + tmp[tmp.Length - 1];
                    if (!Fn.IsExistEstateFile(strFileName, ft))
                    {
                        bool b = Fn.DownloadFile(fileLink, strFileName, ft);
                        if (b)
                        {
                            mef.FileName = strFileName;
                            mef.FileTitle = fileTitle;
                            mef.FileType = ft;
                            mef.Save();
                        }
                    }

                }
                catch { }
            }
        }

        /// <summary>
        /// 获取字符串的MD5编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(str);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>
        /// 获取二手房数据表名称，如果不存在则创建一个
        /// </summary>
        /// <param name="cityId">城市Id</param>
        /// <param name="date">日期</param>
        /// <returns>二手房数据表名称</returns>
        public static string GetHouseSecTable(int CityId, DateTime date)
        {
            string tName = "house_sec_info_" + CityId + "_" + date.ToString("yyyy_MM");
            if (Fn.GetDT_MySQL("SHOW TABLES LIKE '" + tName + "';").Rows.Count == 0)
            {
                string sql = "";
                try
                {
                    var obj=Fn.GetDT_MySQL("SHOW CREATE TABLE house_sec_info").Rows[0][1];
                    if (obj.GetType() == typeof(byte[]))
                    {
                        sql = System.Text.Encoding.Default.GetString(obj as byte[]);
                    }
                    else sql = obj.ToString();
                    sql = sql.Replace("`house_sec_info`", "`" + tName + "`").Replace("InnoDB", "MyISAM");
                    sql = (new Regex("AUTO_INCREMENT=\\d+")).Replace(sql, "AUTO_INCREMENT=1");
                
                    Fn.ExecNonQuery_MySQL(sql);
                }
                catch (Exception e) {
                    Program.MainForm.AddMessage("创建二手房数据表失败！错误消息："+e.Message+"；SQL："+sql);
                    return "";
                }
            }
            return tName;
        }

        /// <summary>
        /// 获取房屋详情数据表名称，如果不存在则创建一个
        /// </summary>
        /// <param name="cityId">城市Id</param>
        /// <returns>房屋详情数据表名称</returns>
        public static string GetHouseInfoTable(int CityId)
        {
            string tName = "house_info_" + CityId;
            if (Fn.GetDT_MySQL("SHOW TABLES LIKE '" + tName + "';").Rows.Count == 0)
            {
                string sql = "";
                try
                {
                    var obj = Fn.GetDT_MySQL("SHOW CREATE TABLE house_info").Rows[0][1];
                    if (obj.GetType() == typeof(byte[]))
                    {
                        sql = System.Text.Encoding.Default.GetString(obj as byte[]);
                    }
                    else sql = obj.ToString();
                    sql = sql.Replace("`house_info`", "`" + tName + "`").Replace("InnoDB", "MyISAM");
                    sql = (new Regex("AUTO_INCREMENT=\\d+")).Replace(sql, "AUTO_INCREMENT=1");

                    Fn.ExecNonQuery_MySQL(sql);
                }
                catch (Exception e)
                {
                    Program.MainForm.AddMessage("创建房屋详情数据表失败！错误消息：" + e.Message + "；SQL：" + sql);
                    return "";
                }
            }
            return tName;
        }
    }
}
