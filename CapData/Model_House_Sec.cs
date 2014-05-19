using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CapData
{
    /// <summary>
    /// 二手房数据模型
    /// </summary>
    class Model_House_Sec:Model
    {
        private bool _autoMatchEstate = true;
        private string _estName = "";
        private int _estId = 0;

        #region 字段

        /// <summary>
        /// 房源标题
        /// </summary>
        public string Title = "";

        /// <summary>
        /// 房源地址
        /// </summary>
        public string HouseAddr = "";

        /// <summary>
        /// 均价
        /// </summary>
        public double avePrice = 0;

        /// <summary>
        /// 建筑面积
        /// </summary>
        public double BuildingArea = 0;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNum = "";

        /// <summary>
        /// 户型
        /// </summary>
        public string HouseType = "";

        /// <summary>
        /// 建筑年代
        /// </summary>
        public int BuildingYear = 0;

        /// <summary>
        /// 朝向
        /// </summary>
        public string Orientation = "";

        /// <summary>
        /// 所在楼层
        /// </summary>
        public int layer = 0;

        /// <summary>
        /// 总楼层
        /// </summary>
        public int layerAll = 0;

        /// <summary>
        /// 房屋结构
        /// </summary>
        public string Structure = "";

        /// <summary>
        /// 装修程度
        /// </summary>
        public string Decoration = "";

        /// <summary>
        /// 物业类型（用途）
        /// </summary>
        public string HouseUsage = "";

        /// <summary>
        /// 建筑类别
        /// </summary>
        public string BuildingType = "";

        /// <summary>
        /// 产权性质
        /// </summary>
        public string PropertyRight = "";

        /// <summary>
        /// 配套设施
        /// </summary>
        public string Facilities = "";

        /// <summary>
        /// 小区名称，当类声明过程中autoMatchEstate值为true时，设置或更改该值可以同时计算所关联的楼盘Id
        /// </summary>
        public string EstName { get { return _estName; } set {
            this._estName = value.Trim();
            if (this._estName == null || this._estName == "")
            {
                this._estId = 0;
            }
            else if (_autoMatchEstate)
            {
                object id = Fn.GetObj_PgSQL("select id from estate_info where CityId='" + this.CityId + "' and EstName=" + Quota + this._estName + Quota + " and deleted=false limit 1");
                string[] arrFilter = {
                    "a.alias=" + Quota + value + Quota,
                    "a.alias like " + Quota + value + "%" + Quota + " and a.like_right=true",
                    "a.alias like " + Quota + "%" + value + Quota + " and a.like_left=true",
                    "a.alias like " + Quota + "%" + value + "%" + Quota + " and a.like_left=true and a.like_right=true"
                };

                int index = 0;
                while (id == null&&index<arrFilter.Length) id = Fn.GetObj_PgSQL("select a.EstId from estate_aliases as a left join estate_info as b on a.EstId=b.Id where b.CityId="+CityId+" and "+ arrFilter[index++]+" order by a.Id desc limit 1");
                try
                {
                    this._estId = int.Parse(id.ToString());
                }
                catch { }
            }
        } }

        /// <summary>
        /// 房源描述
        /// </summary>
        public string Remark = "";

        /// <summary>
        /// 房源信息发布人
        /// </summary>
        public string PostBy = "";

        /// <summary>
        /// 数据发布时间，默认为当前时间
        /// </summary>
        public DateTime PostTime = DateTime.Now;

        /// <summary>
        /// 内容来源链接
        /// </summary>
        public string SourceLink = "";

        #endregion

        /// <summary>
        /// 楼盘Id，仅当类声明时autoMatchEstate值为true才有可能获得有效值（>0）
        /// </summary>
        public int EstId { get { return _estId; } }

        /// <summary>
        /// 创建一个二手房数据模型
        /// </summary>
        /// <param name="cityName">所属城市名称</param>
        public Model_House_Sec(string cityName)
        {
            this._cityId = Fn.GetCityIdByName(cityName);
            this.DBType = DataBaseType.MySQL;
        }

        /// <summary>
        /// 创建一个二手房数据模型
        /// </summary>
        /// <param name="cityName">所属城市名称</param>
        /// <param name="autoMatchEstate">是否根据输入的小区名称自动匹配所属小区，如果自动匹配，则会使用输入的小区名称和系统库中的楼盘名称（Equal）以及楼盘别名数据（匹配方式根据estate_aliases表中数据中定义）进行匹配，默认为true</param>
        public Model_House_Sec(string cityName, bool autoMatchEstate)
        {
            this._cityId = Fn.GetCityIdByName(cityName);
            this._autoMatchEstate = autoMatchEstate;
            this.DBType = DataBaseType.MySQL;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="Cover">当数据已经存在时，是否覆盖（根据数据的SourceLink判断数据是否存在）</param>
        /// <returns>数据Id</returns>
        public int Save(bool Cover)
        {
            string tName = Fn.GetHouseSecTable(CityId, this.PostTime);
            
            var id = 0;
            try
            {
                id = int.Parse(Fn.GetObj_MySQL("select id from "+tName+" where sourcelink=" + Quota + SourceLink + Quota).ToString());
            }
            catch { }

            List<KeyValuePair<string, string>> listParas = new List<KeyValuePair<string, string>>() { 
                new KeyValuePair<string, string>("CityId", CityId.ToString()),
                new KeyValuePair<string, string>("EstId", EstId.ToString()),
                new KeyValuePair<string, string>("Title", Title),
                new KeyValuePair<string, string>("HouseAddr", HouseAddr),
                new KeyValuePair<string, string>("avePrice", avePrice.ToString()),
                new KeyValuePair<string, string>("BuildingArea", BuildingArea.ToString()),
                new KeyValuePair<string, string>("PhoneNum", PhoneNum),
                new KeyValuePair<string, string>("HouseType", HouseType),
                new KeyValuePair<string, string>("BuildingYear", BuildingYear.ToString()),
                new KeyValuePair<string, string>("Orientation", Orientation),
                new KeyValuePair<string, string>("layer", layer.ToString()),
                new KeyValuePair<string, string>("layerAll", layerAll.ToString()),
                new KeyValuePair<string, string>("Structure", Structure),
                new KeyValuePair<string, string>("Decoration", Decoration),
                new KeyValuePair<string, string>("HouseUsage", HouseUsage),
                new KeyValuePair<string, string>("BuildingType", BuildingType),
                new KeyValuePair<string, string>("PropertyRight", PropertyRight),
                new KeyValuePair<string, string>("Facilities", Facilities),
                new KeyValuePair<string, string>("EstName", EstName),
                new KeyValuePair<string, string>("Remark", Remark),
                new KeyValuePair<string, string>("PostBy", PostBy),
                new KeyValuePair<string, string>("PostTime", PostTime.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("SourceLink", SourceLink),
                new KeyValuePair<string, string>("EditTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("AdminId", "1")
            };

            if (id > 0)
            {
                if (Cover)
                {
                    string sql = Update(tName, listParas, "Id=" + id.ToString());
                    Fn.ExecNonQuery_MySQL(sql);
                }
                
            }
            else
            {
                try
                {
                    string sql = Insert(tName, listParas);
                    Fn.ExecNonQuery_MySQL(sql);
                    id = int.Parse(Fn.GetObj_MySQL("select id from "+tName+" where sourcelink=" + Quota + SourceLink + Quota).ToString());
                }
                catch { }
            }



            return id;
        }

        /// <summary>
        /// 保存数据（不覆盖已有数据）
        /// </summary>
        /// <returns>数据Id</returns>
        public int Save()
        {
            return Save(false);
        }
    }
}
