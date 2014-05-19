using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace CapData
{
    /// <summary>
    /// 许可证基本信息
    /// </summary>
    class Model_Permit : Model
    {
        #region 数据表字段
        /// <summary>
        /// 预售许可证号，不能为空，同一城市许可证数据该编号必须唯一，不同城市之间可以相同（可以为该城市许可证抓取网站所提供的该许可证Id号）
        /// </summary>
        public string PermitNum = string.Empty;

        /// <summary>
        /// 许可证名称
        /// </summary>
        public string PermitName = string.Empty;

        /// <summary>
        /// 对应的楼盘编号
        /// </summary>
        public string EstNum = string.Empty;

        /// <summary>
        /// 发证日期，默认为今天
        /// </summary>
        public DateTime IssueDate = DateTime.Today;

        /// <summary>
        /// 开工日期
        /// </summary>
        public DateTime ConstructionDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// 预计竣工日期
        /// </summary>
        public DateTime CompletionDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// 预售门号
        /// </summary>
        public string SaleBuildingNum = string.Empty;

        /// <summary>
        /// 预售总面积
        /// </summary>
        public double SaleArea = 0;

        /// <summary>
        /// 预售总套数
        /// </summary>
        public int SaleCount = 0;

        /// <summary>
        /// 源链接
        /// </summary>
        public string SourceLink = string.Empty;

        #endregion

        /// <summary>
        /// 当前许可证在数据库中的Id，如果不存在，则为-1
        /// </summary>
        public int Id
        {
            get
            {
                try
                {
                    return int.Parse(Fn.GetObj_MySQL("select Id from permitinfo where cityid='" + CityId + "' and permitnum='" + Fn.KW_Equal(PermitNum) + "'").ToString());
                }
                catch { }
                return -1;
            }
        }
        /// <summary>
        /// 初始化一个许可证实例
        /// </summary>
        /// <param name="CityName">许可证所在城市</param>
        public Model_Permit(string CityName)
        {
            Init(CityName);
        }

        /// <summary>
        /// 保存许可证信息，默认不覆盖
        /// </summary>
        public void Save()
        {
            Save(false);
        }

        /// <summary>
        /// 保存许可证信息，根据参数来确定是否覆盖。注意，该方法不会更新已经认证过的数据，如要更新已经认证过的数据，请用Update方法
        /// </summary>
        /// <param name="Cover"></param>
        public void Save(bool Cover)
        {
            if (PermitNum == null || PermitNum == "" || PermitNum == string.Empty) return;

            bool exist = IsExist(PermitNum);
            if (!Cover && exist) return;

            Type type = typeof(Model_Permit);
            string sql = exist ? "update permitinfo set updatetime=now()" : "insert into permitinfo ";
            string sql_key = "cityid,permitnum,adminid,updatetime";
            string sql_val = CityId.ToString() + ",'" + Fn.KW_Equal(PermitNum) + "',1,now()";
            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "PermitNum" || info.Name == "CityId" || info.Name == "Quota"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;
                if (exist)
                {
                    sql += "," + info.Name.ToLower() + "='" + Fn.KW_Equal(info.GetValue(this).ToString()) + "'";
                }
                else
                {
                    sql_key += "," + info.Name;
                    sql_val += ",'" + Fn.KW_Equal(info.GetValue(this).ToString()) + "'";
                }
            }

            if (exist) sql += " where cityid=" + CityId + " and permitnum='" + Fn.KW_Equal(PermitNum) + "' and isverified=false";
            else sql += "(" + sql_key + ") values (" + sql_val + ")";
            Fn.ExecNonQuery_MySQL(sql);
        }

        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <param name="listField">需要更新的字段列表</param>
        public void Update(List<string> listField)
        {
            Type type = typeof(Model_Permit);
            string sql = "Update permitinfo set set updatetime=now()";
            foreach (string item in listField)
            {
                try
                {
                    FieldInfo info = type.GetField(item);
                    sql += "," + item + "='" + Fn.KW_Equal(info.GetValue(this).ToString()) + "'";
                }
                catch { }
            }

            sql += " where cityid=" + CityId + " and permitnum='" + Fn.KW_Equal(PermitNum) + "'";
            Fn.ExecNonQuery_MySQL(sql);
        }

        /// <summary>
        /// 判断是否存在指定许可证编号的数据
        /// </summary>
        /// <param name="permitNum"></param>
        /// <returns></returns>
        public bool IsExist(string permitNum)
        {
            return Fn.GetDT_MySQL("select Id from permitInfo where CityId=" + CityId + " and PermitNum='" + Fn.KW_Equal(permitNum) + "'").Rows.Count > 0;
        }

        public bool IsExistLink(string Link)
        {
            return Fn.GetDT_MySQL("select Id from permitInfo where CityId=" + CityId + " and SourceLink='" + Fn.KW_Equal(Link) + "'").Rows.Count > 0;
        }
    }

    /// <summary>
    /// 许可证上市量
    /// </summary>
    class Model_PermitSale
    {
        #region 数据表字段
        /// <summary>
        /// 物业类型，默认为“商品房”。一个许可证同种物业类型的上市量唯一
        /// </summary>
        public string Usage = "商品房";

        /// <summary>
        /// 上市套数
        /// </summary>
        public int SaleCount = 0;

        /// <summary>
        /// 上市面积
        /// </summary>
        public double SaleArea = 0;

        /// <summary>
        /// 预售均价
        /// </summary>
        public double avePrice = 0;
        #endregion
        private Model_Permit _mp;

        public Model_Permit Permit
        {
            get { return _mp; }
        }

        /// <summary>
        /// 初始化许可证上市量类
        /// </summary>
        /// <param name="mp">许可证</param>
        public Model_PermitSale(Model_Permit mp)
        {
            this._mp = mp;
        }

        public bool IsExistUsage(string usage)
        {
            return Fn.GetDT_MySQL("select * from permitsale where permitid='" + Permit.Id + "' and `usage`='" + Fn.KW_Equal(usage) + "'").Rows.Count > 0;
        }

        /// <summary>
        /// 保存上市量。如果上市套数为0，则不保存
        /// <param name="Cover">如果数据已存在，是否覆盖已有数据</param>
        /// </summary>
        public void Save(bool Cover)
        {
            if (SaleCount == 0) return;
            if (Usage == null || Usage == "" || Usage == string.Empty) Usage = "商品房";
            bool exist = IsExistUsage(Usage);
            if (!Cover && exist) return;

            Type type = typeof(Model_PermitSale);

            string sql = exist ? "update permitsale" : "insert into permitsale";
            sql += " set `Usage`='" + Fn.KW_Equal(Usage) + "'";

            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "Permit" || info.Name == "_mp" || info.Name == "Usage"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;
                sql += "," + info.Name.ToLower() + "='" + Fn.KW_Equal(info.GetValue(this).ToString()) + "'";
            }

            if (exist) sql += " where permitid='" + Permit.Id + "' and `Usage`='" + Fn.KW_Equal(Usage) + "'";
            else sql += ", permitid='" + Permit.Id + "'";
            Fn.ExecNonQuery_MySQL(sql);
        }
    }

    /// <summary>
    /// 许可证成交量
    /// </summary>
    class Model_PermitSold
    {
        #region 数据表字段
        /// <summary>
        /// 物业类型，默认为“商品房”。一个许可证同种物业类型的上市量唯一
        /// </summary>
        public string Usage = "商品房";

        /// <summary>
        /// 成交套数
        /// </summary>
        public int SoldCount = 0;

        /// <summary>
        /// 成交面积
        /// </summary>
        public double SoldArea = 0;

        /// <summary>
        /// 销售均价
        /// </summary>
        public double avePrice = 0;
        #endregion
        private Model_Permit _mp;

        public Model_Permit Permit
        {
            get { return _mp; }
        }

        /// <summary>
        /// 初始化许可证成交量类
        /// </summary>
        /// <param name="mp">许可证</param>
        public Model_PermitSold(Model_Permit mp)
        {
            this._mp = mp;
        }

        public bool IsExistUsage(string usage)
        {
            return Fn.GetDT_MySQL("select * from permitsoldtotal where permitid='" + Permit.Id + "' and `usage`='" + Fn.KW_Equal(usage) + "'").Rows.Count > 0;
        }

        /// <summary>
        /// 保存成交量。如果成交套数为0，则不保存
        /// <param name="Cover">如果数据已存在，是否覆盖已有数据</param>
        /// </summary>
        public void Save(bool Cover)
        {
            if (SoldCount == 0) return;
            if (Usage == null || Usage == "" || Usage == string.Empty) Usage = "商品房";
            bool exist = IsExistUsage(Usage);
            if (!Cover && exist) return;

            Type type = typeof(Model_PermitSold);

            string sql = exist ? "update permitsoldtotal" : "insert into permitsoldtotal";
            sql += " set `Usage`='" + Fn.KW_Equal(Usage) + "'";

            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "Permit" || info.Name == "_mp" || info.Name == "Usage"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;
                sql += "," + info.Name.ToLower() + "='" + Fn.KW_Equal(info.GetValue(this).ToString()) + "'";
            }

            if (exist) sql += " where permitid='" + Permit.Id + "' and `Usage`='" + Fn.KW_Equal(Usage) + "'";
            else sql += ", permitid='" + Permit.Id + "'";
            Fn.ExecNonQuery_MySQL(sql);
        }
    }
}
