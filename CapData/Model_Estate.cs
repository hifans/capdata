using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace CapData
{
    /// <summary>
    /// 楼盘基本信息
    /// </summary>
    class Model_Estate : Model
    {
        #region 数据表字段
        /// <summary>
        /// 楼盘编号，通过该编号能区别同一城市内不同的楼盘。不同城市间的楼盘可以有相同的编号。该项值不能为空
        /// </summary>
        public string EstNum = string.Empty;

        /// <summary>
        /// 楼盘名称
        /// </summary>
        public string EstName = string.Empty;

        /// <summary>
        /// 推广名称。一些楼盘在房产局登记的名称和广告推广的名称不一致，因此该项数据记录其推广用的名称，以便于用户查询
        /// </summary>
        public string PublicName = string.Empty;

        /// <summary>
        /// 所在行政区
        /// </summary>
        public string DistrictName = string.Empty;

        /// <summary>
        /// 所在板块
        /// </summary>
        public string PlateName = string.Empty;

        /// <summary>
        /// 对应地块名称，该项数据只做备查用，不用于和地块关联
        /// </summary>
        public string GroundName = string.Empty;

        /// <summary>
        /// 楼盘地址
        /// </summary>
        public string EstAddr = string.Empty;

        /// <summary>
        /// 开盘时间，可以设置为该楼盘首次开盘的时间
        /// </summary>
        public string OpeningTime = string.Empty;

        /// <summary>
        /// 交付时间，实际上很多楼盘的不同许可证交付时间不一样
        /// </summary>
        public string HandoverTime = string.Empty;

        /// <summary>
        /// 预计竣工时间
        /// </summary>
        public string CompletionTime = string.Empty;

        /// <summary>
        /// 装修标准，如：毛坯、简装修、精装修等
        /// </summary>
        public string DecorationLevel = string.Empty;

        /// <summary>
        /// 物业类型
        /// </summary>
        public string EstUsage = string.Empty;

        /// <summary>
        /// 开发商
        /// </summary>
        public string Developer = string.Empty;

        /// <summary>
        /// 绿化率
        /// </summary>
        public string GreeningRate = string.Empty;

        /// <summary>
        /// 容积率
        /// </summary>
        public string PlotRatio = string.Empty;

        /// <summary>
        /// 建筑密度
        /// </summary>
        public string BuildingDensity = string.Empty;

        /// <summary>
        /// 物业公司
        /// </summary>
        public string PropertyCompany = string.Empty;

        /// <summary>
        /// 物业费
        /// </summary>
        public string PropertyFee = string.Empty;

        /// <summary>
        /// 销售代理公司
        /// </summary>
        public string SaleAgent = string.Empty;

        /// <summary>
        /// 楼盘销售地址
        /// </summary>
        public string SaleAddr = string.Empty;

        /// <summary>
        /// 楼盘销售电话
        /// </summary>
        public string SaleTele = string.Empty;

        /// <summary>
        /// 楼盘周边交通
        /// </summary>
        public string Traffic = string.Empty;

        /// <summary>
        /// 楼盘简介
        /// </summary>
        public string EstIntroduction = string.Empty;

        /// <summary>
        /// 开发企业简介
        /// </summary>
        public string DevIntroduction = string.Empty;

        /// <summary>
        /// 楼盘配套设施
        /// </summary>
        public string EstFacility = string.Empty;

        /// <summary>
        /// 周边配套设施
        /// </summary>
        public string SrdFacility = string.Empty;

        /// <summary>
        /// 基础设施
        /// </summary>
        public string Infrastructure = string.Empty;

        /// <summary>
        /// 轨道交通
        /// </summary>
        public string RailTransport = string.Empty;

        /// <summary>
        /// 周边商业设施
        /// </summary>
        public string CommercialFacilities = string.Empty;

        /// <summary>
        /// 银行
        /// </summary>
        public string Banks = string.Empty;

        /// <summary>
        /// 周边小区
        /// </summary>
        public string ResidentialBearBy = string.Empty;

        /// <summary>
        /// 学校
        /// </summary>
        public string School = string.Empty;

        /// <summary>
        /// 其他
        /// </summary>
        public string Others = string.Empty;

        /// <summary>
        /// 公益设施
        /// </summary>
        public string PublicFacilities = string.Empty;

        /// <summary>
        /// 设计单位
        /// </summary>
        public string Designer = string.Empty;

        /// <summary>
        /// 施工单位
        /// </summary>
        public string Builder = string.Empty;

        /// <summary>
        /// 建筑类别
        /// </summary>
        public string BuildingType = string.Empty;

        /// <summary>
        /// 社区规模
        /// </summary>
        public string CommunitySize = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark = string.Empty;

        /// <summary>
        /// 源链接
        /// </summary>
        public string SourceLink = string.Empty;

        /// <summary>
        /// 立项批文
        /// </summary>
        public string ProjectApprovals = string.Empty;

        /// <summary>
        /// 建设用地规划许可证号
        /// </summary>
        public string LandPermitNum = string.Empty;

        /// <summary>
        /// 建设工程规划许可证号
        /// </summary>
        public string ProjectPermitNum = string.Empty;

        /// <summary>
        /// 施工许可证号
        /// </summary>
        public string ConstructionPermitNum = string.Empty;

        /// <summary>
        /// 土地证号
        /// </summary>
        public string GroundPermitNum = string.Empty;

        /// <summary>
        /// 总用地面积
        /// </summary>
        public double GroundAreaAll = 0;

        /// <summary>
        /// 当期用地面积
        /// </summary>
        public double GroundAreaCurrent = 0;

        /// <summary>
        /// 总户数
        /// </summary>
        public int CountAll = 0;

        /// <summary>
        /// 停车位状况
        /// </summary>
        public string ParkingStatus = string.Empty;

        /// <summary>
        /// 总建筑面积
        /// </summary>
        public double BuildingAreaAll = 0;

        /// <summary>
        /// 已售面积
        /// </summary>
        public double AreaSold = 0;

        /// <summary>
        /// 已售套数，用于记录楼盘是否已售完；如果未开盘，则该值也有可能为0，但此时总户数值一般也为0
        /// </summary>
        public int CountSold = 0;

        #endregion

        /// <summary>
        /// 初始化一个楼盘实例
        /// </summary>
        /// <param name="CityName">楼盘所在城市名称</param>
        public Model_Estate(string CityName)
        {
            Init(CityName);
        }

        /// <summary>
        /// 保存数据，默认不覆盖
        /// </summary>
        public int Save()
        {
            return this.Save(false);
        }
        /// <summary>
        /// 保存数据，如果该数据已存在，则根据Cover值判断是否重新覆盖（只覆盖数据库中没有被审核的数据）
        /// </summary>
        /// <param name="Cover">是否重新覆盖</param>
        public int Save(bool Cover)
        {
            List<KeyValuePair<List<string>, int>> listStrLen = new List<KeyValuePair<List<string>, int>>() { 
                new KeyValuePair<List<string>, int>(new List<string>(){"DistrictName","PlateName","LandPermitNum","ProjectPermitNum","ConstructionPermitNum","GroundPermitNum","GreeningRate","BuildingDensity","PlotRatio","BuildingType","CommunitySize"},100),
                new KeyValuePair<List<string>, int>(new List<string>(){"SourceLink"},511)
            };

            if (EstNum == string.Empty || EstNum == null || EstNum == "") return 0;
            if (EstNum.Length > 50) EstNum = EstNum.Substring(0, 50);

            bool exist = IsExist(EstNum);
            if (!Cover && exist) return this.getId();

            Type type = typeof(Model_Estate);
            string sql = exist ? "update estate_info set updatetime=now()" : "insert into estate_info ";
            string sql_key = "cityid,estnum,adminid,updatetime";
            string sql_val = CityId.ToString() + "," + Quota + EstNum + Quota + ",0,now()";
            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "EstNum" || info.Name == "CityId" || info.Name == "Quota"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;

                int len = 255;
                try
                {
                    len = listStrLen.Find(delegate(KeyValuePair<List<string>, int> tmp) { return tmp.Key.Contains(info.Name); }).Value;
                    if (len == 0) len = 255;
                }
                catch { }

                string val = info.GetValue(this).ToString();
                if (val.Length > len) val = val.Substring(0, len);
                if (exist)
                {
                    sql += "," + info.Name.ToLower() + "=" + Quota + val + Quota;
                }
                else
                {
                    sql_key += "," + info.Name;
                    sql_val += "," + Quota + val + Quota;
                }
            }

            if (exist) sql += " where cityid=" + CityId + " and estnum=" + Quota + EstNum + Quota + " and isverified=false";
            else sql += "(" + sql_key + ") values (" + sql_val + ")";
            //MessageBox.Show(sql);
            try
            {
                Fn.ExecNonQuery_PgSQL(sql);
            }
            catch (Exception ex) { Program.MainForm.AddMessage(ex.Message + "。SQL：" + sql); return 0; }
            return this.getId();
        }

        /// <summary>
        /// 判断是否存在指定楼盘编号的数据
        /// </summary>
        /// <param name="estNum"></param>
        /// <returns></returns>
        public bool IsExist(string estNum)
        {
            return Fn.GetDT_PgSQL("select Id from estate_info where cityid=" + CityId + " and estnum=" + Quota + estNum + Quota).Rows.Count > 0;
        }

        private int getId()
        {
            try
            {
                return int.Parse(Fn.GetObj_PgSQL("select id from estate_info where cityid=" + CityId + " and estnum=" + Quota + EstNum + Quota).ToString());
            }
            catch { }
            return 0;
        }
    }

    /// <summary>
    /// 楼盘成交量
    /// </summary>
    class Model_EstateSold
    {
        #region 数据表字段
        /// <summary>
        /// 物业类型，默认为“商品房”。一个楼盘同一时间段内同一物业类型的成交数据唯一
        /// </summary>
        public string EstUsage = "商品房";

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

        /// <summary>
        /// 成交时间段的起始时间
        /// </summary>
        public DateTime SoldDateStart = DateTime.Today;

        /// <summary>
        /// 成交时间段的终止时间
        /// </summary>
        public DateTime SoldDateEnd = DateTime.Today;

        #endregion
        private int _estId;

        public int EstId
        {
            get { return _estId; }
        }


        public Model_EstateSold(int estId)
        {
            this._estId = estId;
        }

        public bool IsExistUsage(string usage)
        {
            return Fn.GetDT_PgSQL("select * from estate_sold where estid='" + EstId + "' and EstUsage='" + Fn.KW_Equal(usage) + "' and solddatestart='" + SoldDateStart.ToString("yyyy-MM-dd") + "' and solddateend='" + SoldDateEnd.ToString("yyyy-MM-dd") + "'").Rows.Count > 0;
        }

        /// <summary>
        /// 保存成交量。如果成交套数为0，则不保存
        /// <param name="Cover">如果数据已存在，是否覆盖已有数据</param>
        /// </summary>
        public void Save(bool Cover)
        {
            if (SoldCount == 0) return;
            if (EstUsage == null || EstUsage == "" || EstUsage == string.Empty) EstUsage = "商品房";
            bool exist = IsExistUsage(EstUsage);
            if (!Cover && exist) return;

            string Quota = "$" + Fn.GetRandomStr() + "$";
            Type type = typeof(Model_EstateSold);
            string sql = exist ? "update estate_sold set " : "insert into estate_sold ";
            string sql_key = "estid,estusage,solddatestart,solddateend";
            string sql_val = EstId.ToString() + "," + Quota + EstUsage + Quota + ",'" + SoldDateStart.ToString("yyyy-MM-dd") + "','" + SoldDateEnd.ToString("yyyy-MM-dd") + "'";
            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "_estId" || info.Name == "EstId" || info.Name == "EstUsage" || info.Name == "SoldDateStart" || info.Name == "SoldDateEnd"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;
                if (exist)
                {
                    sql += "," + info.Name.ToLower() + "=" + Quota + info.GetValue(this) + Quota;
                }
                else
                {
                    sql_key += "," + info.Name;
                    sql_val += "," + Quota + info.GetValue(this) + Quota;
                }
            }

            if (exist) sql += " where estid=" + EstId + " and estusage=" + Quota + EstUsage + Quota + " and solddatestart='" + SoldDateStart.ToString("yyyy-MM-dd") + "' and solddateend='" + SoldDateEnd.ToString("yyyy-MM-dd") + "'";
            else sql += "(" + sql_key + ") values (" + sql_val + ")";
            //MessageBox.Show(sql);
            Fn.ExecNonQuery_PgSQL(sql);
        }
    }

    class Model_Estate_File
    {
        public string Quota = "$"+Fn.GetRandomStr()+"$";
        private int _eId = 0;
        /// <summary>
        /// 楼盘Id
        /// </summary>
        public int EstateId { get { return _eId; } }

        /// <summary>
        /// 文件标题，在程序中显示的标题
        /// </summary>
        public string FileTitle = string.Empty;

        /// <summary>
        /// 文件名称，文件在磁盘中存储的名称
        /// </summary>
        public string FileName = string.Empty;

        public FileType FileType = FileType.Image;

        /// <summary>
        /// 定义一个地块相关文件数据模型，并尝试通过地块所属城市和地块编号确定其后台数据库Id
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="estateNum"></param>
        public Model_Estate_File(int cityId, string estateNum)
        {
            try
            {
                this._eId = int.Parse(Fn.GetDT_PgSQL("select Id from estate_info where cityid=" + cityId + " and estnum='" + estateNum + "'").Rows[0][0].ToString());
            }
            catch { }
        }

        public void Save()
        {
            if (EstateId <= 0 || FileName == null || FileName.Trim() == "") return;
            string sql = "insert into estate_file (estid,FileTitle,FileName,FileType) values(" + EstateId + "," + Quota + FileTitle + Quota + "," + Quota + FileName + Quota + "," + Convert.ToInt32(FileType) + ")";
            Fn.ExecNonQuery_PgSQL(sql);
        }
    }

    class Model_Estate_Price:Model
    {
        private int _eId = 0;
        /// <summary>
        /// 楼盘Id
        /// </summary>
        public int EstateId { get { return _eId; } }

        /// <summary>
        /// 报价日期
        /// </summary>
        public DateTime PriceDate = DateTime.Today;
        
        /// <summary>
        /// 物业类型，默认为“商品房”。一个楼盘同一时间段内同一物业类型的成交数据唯一
        /// </summary>
        public string EstUsage = "商品房";

        /// <summary>
        /// 最低价
        /// </summary>
        public double PriceMin = 0;

        /// <summary>
        /// 最高价
        /// </summary>
        public double PriceMax = 0;

        /// <summary>
        /// 装修程度
        /// </summary>
        public string Decoration = "";

        /// <summary>
        /// 朝向
        /// </summary>
        public string Orientation = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark = "";

        /// <summary>
        /// 数据源网站链接，如：http://www.wxhouse.com
        /// </summary>
        public string SourceLink = "";

        /// <summary>
        /// 数据源网站名称
        /// </summary>
        public string SourceName = "";


        /// <summary>
        /// 创建一个楼盘报价数据模型
        /// </summary>
        /// <param name="estId">楼盘在本系统数据库中的Id</param>
        public Model_Estate_Price(int estId)
        {
            this._eId = estId;
        }

        /// <summary>
        /// 创建一个楼盘报价数据模型
        /// </summary>
        /// <param name="cityId">所在城市Id</param>
        /// <param name="estateNum">楼盘编号</param>
        public Model_Estate_Price(int cityId, string estateNum)
        {
            this._cityId = cityId;
            try
            {
                this._eId = int.Parse(Fn.GetDT_PgSQL("select Id from estate_info where cityid=" + cityId + " and estnum='" + estateNum + "'").Rows[0][0].ToString());
            }
            catch { }
        }

        /// <summary>
        /// 保存数据，数据通过楼盘Id+日期+用途来判断唯一性
        /// </summary>
        /// <param name="Cover">当数据已经存在时，是否覆盖已有数据</param>
        /// <returns>当没有价格信息或者输入的楼盘信息不存在时，返回false，保存成功则返回true</returns>
        public bool Save(bool Cover)
        {
            if (PriceMax == 0) PriceMax = PriceMin;
            if (PriceMin == 0) PriceMin = PriceMax;
            if (PriceMax == 0) return false;
            if (Fn.GetDT_PgSQL("select * from estate_info where id='" + _eId + "'").Rows.Count == 0) return false;

            if (!this.SourceLink.StartsWith("http://")) this.SourceLink = "http://"+this.SourceLink;

            this.SourceLink = this.SourceLink.Trim();
            DataTable dtSourceId = Fn.GetDT_PgSQL("select id from estate_price_source where sourcelink=" + Quota + SourceLink + Quota);
            if (dtSourceId.Rows.Count == 0)
            {
                string sqlSource = "insert into estate_price_source (sourcelink, sourcename) values (" + Quota + SourceLink + Quota + "," + Quota + SourceName + Quota + ")";
                Fn.ExecNonQuery_PgSQL(sqlSource);
                dtSourceId = Fn.GetDT_PgSQL("select id from estate_price_source where sourcelink=" + Quota + SourceLink + Quota);

            }

            string filter = "estid=" + this._eId + " and pricedate='" + PriceDate.ToString("yyyy-MM-dd") + "' and estusage=" + Quota + EstUsage + Quota;
            List<KeyValuePair<string, string>> listParas = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("PriceMin",PriceMin.ToString()),
                new KeyValuePair<string, string>("PriceMax",PriceMax.ToString()),
                new KeyValuePair<string, string>("Decoration",Decoration),
                new KeyValuePair<string, string>("Orientation",Orientation),
                new KeyValuePair<string, string>("Remark",Remark),
                new KeyValuePair<string, string>("SourceId",dtSourceId.Rows[0][0].ToString())
            };


            if (Fn.GetDT_PgSQL("select * from estate_price where " + filter).Rows.Count == 0)
            {
                listParas.Add(new KeyValuePair<string, string>("estid", EstateId.ToString()));
                listParas.Add(new KeyValuePair<string, string>("pricedate", PriceDate.ToString("yyyy-MM-dd")));
                listParas.Add(new KeyValuePair<string, string>("estusage", EstUsage));
                Fn.ExecNonQuery_PgSQL(this.Insert("estate_price", listParas));
            }
            else if (Cover) Fn.ExecNonQuery_PgSQL(this.Update("estate_price", listParas, filter));
            return true;
        }
    }
}
