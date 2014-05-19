using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace CapData
{

    class Model
    {
        protected int _cityId = 0;
        private string _quota = "$$";
        private DataBaseType _dbType = DataBaseType.Postgresql;
        public int CityId
        {
            get { return _cityId; }
        }

        /// <summary>
        /// 获取或设置该数据模型所对应的数据存储的数据库类型，默认为Postgresql。
        /// 如需修改，可在继承的数据模型声明函数中修改
        /// </summary>
        protected DataBaseType DBType { get { return _dbType; } set {
            this._dbType = value;
            switch (value)
            {
                case DataBaseType.MySQL:
                    this._quota = "'";
                    break;
                case DataBaseType.Postgresql:
                    this._quota = "$" + Fn.GetRandomStr() + "$";
                    break;
                default:
                    break;
            }
        } }

        /// <summary>
        /// 获取引号，当数据库类型为Postgresql时，该值为美元符号所包括的随机字符串；
        /// 当数据库类型为MySQL时，该值仅为简单的单引号，此时，在生成SQL时，程序会自动处理字符串值中的特殊字符
        /// </summary>
        public string Quota { get { return _quota; } }

        public void Init(string CityName)
        {
            string sql = "select Id from sys_City where cName='" + CityName + "' limit 1";
            try
            {
                object obj = Fn.GetObj_MySQL(sql);
                _cityId = int.Parse(obj.ToString());
            }
            catch { throw new Exception("数据库找不到城市“" + CityName + "”！请在MySQL数据库中的表sys_City中添加该城市基本信息之后再运行。"); }
            //Quota = "$" + Fn.GetRandomStr() + "$";
        }
        protected Model()
        {
        }

        /// <summary>
        /// 获取Insert SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="listParas">字段名称和值</param>
        /// <returns>SQL语句</returns>
        public string Insert(string tableName, List<KeyValuePair<string, string>> listParas)
        {
            if (listParas.Count == 0) return "";
            string sql = "insert into " + tableName;
            string key = "(";
            string val = "(";
            foreach (KeyValuePair<string, string> item in listParas)
            {
                key += (this.DBType == DataBaseType.MySQL ? "`" : "") + item.Key + (this.DBType == DataBaseType.MySQL ? "`" : "") + ",";
                string tmp = item.Value.Trim();
                if (this.DBType == DataBaseType.MySQL) tmp = Fn.KW_Equal(tmp);
                val += Quota + tmp + Quota + ",";
            }
            sql += key.Trim(',')+") values "+val.Trim(',')+");";
            return sql;
        }

        /// <summary>
        /// 返回Update SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="listParas">字段名称和值</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>SQL语句</returns>
        public string Update(string tableName, List<KeyValuePair<string, string>> listParas, string filter)
        {
            if (listParas.Count == 0) return "";
            string sql = "update "+tableName+" set ";
            foreach (KeyValuePair<string, string> item in listParas)
            {
                string tmp = item.Value.Trim();
                if (this.DBType == DataBaseType.MySQL) tmp = Fn.KW_Equal(tmp);
                sql += item.Key + "=" + Quota + tmp + Quota + ",";
            }
            return sql.Trim(',') + " where " + filter;
        }

        public void SetValue(string fldName, object value)
        {
            FieldInfo fi = this.GetType().GetField(fldName);
            this.GetType().GetField(fldName).SetValue(this, value);
        }
    }

    class Model_DailySold : Model
    {
        /// <summary>
        /// 成交日期，默认为今天
        /// </summary>
        public DateTime SoldDate = DateTime.Today;

        /// <summary>
        /// 物业类型，默认为“商品房”
        /// </summary>
        public string SoldUsage = "商品房";

        /// <summary>
        /// 成交套数
        /// </summary>
        public int SoldCount = 0;

        /// <summary>
        /// 成交面积
        /// </summary>
        public double SoldArea = 0;

        /// <summary>
        /// 成交均价
        /// </summary>
        public double avePrice = 0;

        public Model_DailySold(string cityName)
        {
            this.Init(cityName);
        }

        public void Save()
        {
            DataTable dt = Fn.GetDT_MySQL("select id from city_DailySold where cityid="+this.CityId + " and soldDate='"+SoldDate.ToString("yyyy-MM-dd")+"' and soldUsage='"+SoldUsage+"' limit 1");
            if (dt.Rows.Count > 0)
            {
                string sql = "update city_DailySold set soldCount="+SoldCount + ", soldArea="+SoldArea+", avePrice="+avePrice + " where id=" + dt.Rows[0]["id"].ToString();
                Fn.ExecNonQuery_MySQL(sql);
            }
            else
            {
                string sql = "insert into city_DailySold (cityid,soldDate,soldUsage,soldCount,soldArea,aveprice) values(";
                sql += this.CityId + ",'" + SoldDate.ToString("yyyy-MM-dd")+"','"+SoldUsage + "',"+SoldCount+","+SoldArea+","+avePrice+")";
                Fn.ExecNonQuery_MySQL(sql);
            }
        }
    }

}
