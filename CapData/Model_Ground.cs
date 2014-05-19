using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Data;

namespace CapData
{
    /// <summary>
    /// 土地基本信息
    /// </summary>
    class Model_Ground : Model
    {
        #region 数据表字段
        /// <summary>
        /// 地块编号。通过该编号能区别同一城市的不同地块。不同城市的地块可以有相同的编号。不能为空
        /// </summary>
        public string GroundNum = string.Empty;

        /// <summary>
        /// 出让方式，如“招拍挂”、“协议出让”等，如果不填，则默认为“招拍挂”
        /// </summary>
        public string TransferWay = string.Empty;

        /// <summary>
        /// 挂牌编号
        /// </summary>
        public string ListingNum = string.Empty;

        /// <summary>
        /// 地块名称。可以是官方公布的名称，也可以是大概地址
        /// </summary>
        public string GroundName = string.Empty;

        /// <summary>
        /// 地块地址。可以是详细地址和地块四至
        /// </summary>
        public string GLocation = string.Empty;

        /// <summary>
        /// 所在行政区
        /// </summary>
        public string DistrictName = string.Empty;

        /// <summary>
        /// 板块名称
        /// </summary>
        public string PlateName = string.Empty;

        /// <summary>
        /// 出让面积，以“平米(m2)”计算
        /// </summary>
        public double GArea = 0;

        /// <summary>
        /// 地块用途，如工业用地，商业用地等大类
        /// </summary>
        public string GUsage = string.Empty;

        /// <summary>
        /// 详细用途，如工业用地中的具体用于某个行业，以及商业用地中的住宅、别墅、办公、商业等等。
        /// </summary>
        public string GUsage_Detail = string.Empty;

        /// <summary>
        /// 容积率
        /// </summary>
        public string PlotRatio = string.Empty;

        /// <summary>
        /// 建筑密度
        /// </summary>
        public string BuildingDensity = string.Empty;

        /// <summary>
        /// 绿化率
        /// </summary>
        public string GreeningRate = string.Empty;

        /// <summary>
        /// 建筑限高
        /// </summary>
        public string MaxHeight = string.Empty;

        /// <summary>
        /// 投资强度，以“万元/亩”计算
        /// </summary>
        public double InvestmentIntensity = 0;

        /// <summary>
        /// 供地条件。该项数据目前见的最多的是“净地”
        /// </summary>
        public string GCondition = string.Empty;

        /// <summary>
        /// 挂牌价，以“元”计算！！！
        /// </summary>
        public double StartingPrice = 0;

        /// <summary>
        /// 保证金，以“元”计算！！！
        /// </summary>
        public double BidBond = 0;

        /// <summary>
        /// 出让年限
        /// </summary>
        public string TransferYear = string.Empty;

        /// <summary>
        /// 挂牌时间
        /// </summary>
        public DateTime ListingDate = DateTime.Today;

        /// <summary>
        /// 竞得人
        /// </summary>
        public string Buyer = string.Empty;

        /// <summary>
        /// 成交价，以“元”计算！！！
        /// </summary>
        public double ClinchPrice = 0;

        /// <summary>
        /// 成交日期
        /// </summary>
        public DateTime ClinchDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// 配套设施用地比例
        /// </summary>
        public string SupportFacilitiesRatio = string.Empty;

        /// <summary>
        /// 增价幅度
        /// </summary>
        public string IncreaseRate = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark = string.Empty;

        /// <summary>
        /// 源链接
        /// </summary>
        public string SourceLink = string.Empty;

        /// <summary>
        /// 距商服中心距离
        /// </summary>
        public string DisFromCS = string.Empty;

        /// <summary>
        /// 交通便捷度
        /// </summary>
        public string TrafficLevel = string.Empty;

        /// <summary>
        /// 道路通达度
        /// </summary>
        public string RoadAccessibility = string.Empty;

        /// <summary>
        /// 基础设施状况
        /// </summary>
        public string InfrastructureStatus = string.Empty;

        /// <summary>
        /// 公共服务设施
        /// </summary>
        public string PublicFacilities = string.Empty;

        /// <summary>
        /// 产业聚集程度及配套协作关系
        /// </summary>
        public string IndustryGatheringLevel = string.Empty;

        /// <summary>
        /// 自然环境状况
        /// </summary>
        public string EnviromentNatural = string.Empty;

        /// <summary>
        /// 人文环境状况
        /// </summary>
        public string EnviromentHuman = string.Empty;

        /// <summary>
        /// 城市规划限制
        /// </summary>
        public string PlanningLimit = string.Empty;

        /// <summary>
        /// 地形条件
        /// </summary>
        public string Terrain = string.Empty;

        /// <summary>
        /// 地质条件
        /// </summary>
        public string Geological = string.Empty;

        /// <summary>
        /// 宗地形状
        /// </summary>
        public string GroundShape = string.Empty;

        /// <summary>
        /// 临路状况
        /// </summary>
        public string RoadNearBy = string.Empty;

        /// <summary>
        /// 特殊限制
        /// </summary>
        public string SpecialLimit = string.Empty;

        /// <summary>
        /// 宗地内开发程度
        /// </summary>
        public string DevelopLevel = string.Empty;

        /// <summary>
        /// 各用途比例
        /// </summary>
        public string RateOfEachUsage = string.Empty;

        /// <summary>
        /// 成交数据链接
        /// </summary>
        public string ClinchLink = string.Empty;

        #endregion

        /// <summary>
        /// 初始化一个地块实例
        /// </summary>
        /// <param name="CityName"></param>
        public Model_Ground(string CityName)
        {
            Init(CityName);
        }

        /// <summary>
        /// 保存数据，默认不覆盖
        /// </summary>
        public void Save()
        {
            Save(false, null);
        }

        /// <summary>
        /// 保存数据，如果该数据已存在，则根据Cover值判断是否覆盖，注意，该方法不会覆盖已经经过认证的数据，如要更新已经过认证的数据，请用Update方法
        /// </summary>
        /// <param name="Cover">如果数据已存在的话，是否需要覆盖</param>
        /// <param name="listField">需要保存的字段，如果该参数问null或者长度为0，则保存所有字段</param>
        public void Save(bool Cover, List<string> listField)
        {
            if (GroundNum == string.Empty || GroundNum == null || GroundNum == "") return;

            bool exist = IsExist(GroundNum);
            if (!Cover && exist)
                return;

            //if (TransferWay != "招拍挂" && TransferWay != "协议出让") TransferWay = "招拍挂";
            //if (TransferWay == string.Empty || TransferWay == null) TransferWay = "招拍挂";

            Type type = typeof(Model_Ground);
            string sql = exist ? "update ground_info set updatetime=now()" : "insert into ground_info ";
            string sql_key = "cityid,groundnum,adminid,updatetime";
            string sql_val = CityId.ToString() + "," + Quota + GroundNum + Quota + ",0,now()";
            foreach (FieldInfo info in type.GetFields())
            {
                if (info.Name == "GroundNum" || info.Name == "CityId" || info.Name == "Quota"
                    || info.GetValue(this) == null || info.GetValue(this).ToString() == "") continue;
                if (listField != null && listField.Count > 0)
                {
                    if (!listField.Contains(info.Name)) continue;
                }
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

            if (exist) sql += " where cityid=" + CityId + " and groundnum=" + Quota + GroundNum + Quota + " and isverified=false";
            else sql += "(" + sql_key + ") values (" + sql_val + ")";
            //MessageBox.Show(sql);
            Fn.ExecNonQuery_PgSQL(sql);
        }

        /// <summary>
        /// 更新指定的字段
        /// </summary>
        /// <param name="listField"></param>
        public void Update(List<string> listField)
        {
            Type type = typeof(Model_Ground);
            string sql = "Update ground_info set updatetime=now()";
            foreach (string item in listField)
            {
                try
                {
                    FieldInfo info = type.GetField(item);
                    sql += "," + item + "=" + Quota + info.GetValue(this) + Quota;
                }
                catch { }
            }

            sql += " where cityid=" + CityId + " and groundnum=" + Quota + GroundNum + Quota;
            Fn.ExecNonQuery_PgSQL(sql);
        }

        /// <summary>
        /// 判断是否存在指定地块编号的数据
        /// </summary>
        /// <param name="groundNum"></param>
        /// <returns></returns>
        public bool IsExist(string groundNum)
        {
            return Fn.GetDT_PgSQL("select Id from ground_info where cityid=" + CityId + " and groundnum=" + Quota + groundNum + Quota).Rows.Count > 0;
        }

        /// <summary>
        /// 判断是否存在指定源链接的数据
        /// </summary>
        /// <param name="Link">源连接</param>
        /// <returns></returns>
        public bool IsExistLink(string Link)
        {
            return Fn.GetDT_PgSQL("select Id from ground_info where cityid=" + CityId + " and sourcelink=" + Quota + Link + Quota).Rows.Count > 0;
        }
    }


    class Model_Ground_File
    {
        public string Quota = "";
        private int _gId = 0;
        /// <summary>
        /// 土地编号
        /// </summary>
        public int GroundId { get { return _gId; } }

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
        /// <param name="groundNum"></param>
        public Model_Ground_File(int cityId, string groundNum)
        {
            Quota = "$" + Fn.GetRandomStr() + "$";
            try
            {
                this._gId = int.Parse(Fn.GetDT_PgSQL("select Id from ground_info where cityid=" + cityId + " and groundnum='" + groundNum + "'").Rows[0][0].ToString());
            }
            catch { }
        }

        public void Save()
        {
            if (GroundId <= 0 || FileName == null || FileName.Trim() == "") return;
            string sql = "insert into ground_file (GroundId,FileTitle,FileName,FileType) values(" + GroundId + "," + Quota + FileTitle + Quota + "," + Quota + FileName + Quota + "," + Convert.ToInt32(FileType) + ")";
            Fn.ExecNonQuery_PgSQL(sql);
        }
    }

    /// <summary>
    /// 预公告土地数据
    /// </summary>
    class Model_Ground_Presell : Model
    {
        #region 数据表字段

        /// <summary>
        /// 地块编号，该字段和城市编号组成Unique Index
        /// </summary>
        public string GroundNum = "";

        /// <summary>
        /// 地块名称
        /// </summary>
        public string GroundName = "";

        /// <summary>
        /// 土地位置
        /// </summary>
        public string GLocation = "";

        /// <summary>
        /// 土地面积（平米）
        /// </summary>
        public double GArea = 0;

        /// <summary>
        /// 土地用途
        /// </summary>
        public string GUsage = "";

        /// <summary>
        /// 容积率
        /// </summary>
        public string PlotRatio = "";

        /// <summary>
        /// 建设密度
        /// </summary>
        public string BuildingDensity = "";

        /// <summary>
        /// 绿化率
        /// </summary>
        public string GreeningRate = "";

        /// <summary>
        /// 周边情况
        /// </summary>
        public string Surrounding = "";

        /// <summary>
        /// 源内容链接
        /// </summary>
        public string SourceLink = "";


        #endregion

        public Model_Ground_Presell(string cityName)
        {
            this._cityId = Fn.GetCityIdByName(cityName);
        }

        /// <summary>
        /// 保存数据，并返回数据保存之后的Id号
        /// </summary>
        /// <param name="Cover">是否覆盖已有数据</param>
        /// <returns>数据Id</returns>
        public int Save(bool Cover)
        {
            string tableName="Ground_Presell";
            if (GroundNum == null || GroundNum.Trim() == "") return 0;
            string sFilter = "CityId='" + CityId + "' and GroundNum=" + Quota + GroundNum.Trim() + Quota;
            
            int id=0;
            try
            {
                id = int.Parse(Fn.GetDT_PgSQL("select id from " + tableName + " where " + sFilter).Rows[0][0].ToString());
            }
            catch { }
            if (id > 0 && !Cover) return id;

            List<KeyValuePair<string, string>> listParas = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("GroundName", GroundName.Trim()),
                new KeyValuePair<string, string>("GLocation", GLocation.Trim()),
                new KeyValuePair<string, string>("GArea", GArea.ToString()),
                new KeyValuePair<string, string>("GUsage", GUsage.Trim()),
                new KeyValuePair<string, string>("PlotRatio", PlotRatio.Trim()),
                new KeyValuePair<string, string>("BuildingDensity", BuildingDensity.Trim()),
                new KeyValuePair<string, string>("GreeningRate", GreeningRate.Trim()),
                new KeyValuePair<string, string>("Surrounding", Surrounding.Trim()),
                new KeyValuePair<string, string>("SourceLink", SourceLink.Trim())
            };

            if (id > 0) Fn.ExecNonQuery_PgSQL(this.Update(tableName, listParas, "id=" + id.ToString()));
            else
            {
                listParas.Add(new KeyValuePair<string, string>("CityId", CityId.ToString()));
                listParas.Add(new KeyValuePair<string, string>("GroundNum", GroundNum));
                Fn.ExecNonQuery_PgSQL(this.Insert(tableName, listParas));
                try
                {
                    id = int.Parse(Fn.GetDT_PgSQL("select id from " + tableName + " where " + sFilter).Rows[0][0].ToString());
                }
                catch { }
            }

            return id;
        }
    }
}
