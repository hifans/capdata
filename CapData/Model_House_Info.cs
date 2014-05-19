using System;
using System.Collections.Generic;
using System.Text;

namespace CapData
{
    class Model_House_Info : Model
    {
        #region 字段
        /// <summary>
        /// 房屋Id，可用网站上的Id或者其他方式生成，用作唯一性识别，为必填项，如果不填则无法保存数据
        /// </summary>
        public string HouseId = "";

        /// <summary>
        /// 幢号
        /// </summary>
        public string BuildNum = "";
        
        /// <summary>
        /// 单元号
        /// </summary>
        public string UnitNum = "";

        /// <summary>
        /// 门牌号
        /// </summary>
        public string RoomNum = "";

        /// <summary>
        /// 所在楼层
        /// </summary>
        public int Layer = 0;

        /// <summary>
        /// 总楼层
        /// </summary>
        public int LayerAll = 0;

        /// <summary>
        /// 套型
        /// </summary>
        public string Structure = "";

        /// <summary>
        /// 物业类型
        /// </summary>
        public string Usage = "";

        /// <summary>
        /// 建筑面积
        /// </summary>
        public double Area = 0;

        /// <summary>
        /// 套内面积
        /// </summary>
        public double AreaInner = 0;

        /// <summary>
        /// 公摊面积
        /// </summary>
        public double AreaOuter = 0;

        /// <summary>
        /// 上市日期（默认为“今天”）
        /// </summary>
        public DateTime ListDate = DateTime.Today;

        /// <summary>
        /// 上市挂牌均价（一房一价的挂牌价）
        /// </summary>
        public double ListPrice = 0;

        /// <summary>
        /// 成交日期（如果未成交不填即可）
        /// </summary>
        public DateTime SoldDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// 实际成交均价（如果未成交不填即可）
        /// </summary>
        public double SoldPrice = 0;

        #endregion

        private int _permitId = 0;
        private int _estId = 0;

        /// <summary>
        /// 声明一个房屋详情数据模型
        /// </summary>
        /// <param name="cityId">所属城市Id（必须要有效）</param>
        /// <param name="estId">所属楼盘Id（必须要有效）</param>
        /// <param name="permitId">所属许可证Id（可不填）</param>
        public Model_House_Info(int cityId, int estId, int permitId)
        {
            this._cityId = cityId;
            this._permitId = permitId;
            this._estId = estId;
            if (estId <= 0)
            {
                throw new Exception("楼盘Id必须大于0。");
            }

            this.DBType = DataBaseType.MySQL;
        }

        /// <summary>
        /// 声明一个房屋详情数据模型
        /// </summary>
        /// <param name="cityId">所属城市Id（必须要有效）</param>
        /// <param name="estId">所属楼盘Id（必须要有效）</param>
        public Model_House_Info(int cityId, int estId)
        {
            this._cityId = cityId;
            this._estId = estId;
            if (estId <= 0)
            {
                throw new Exception("楼盘Id必须大于0。");
            }

            this.DBType = DataBaseType.MySQL;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void Save()
        {
            if (HouseId.Trim() == "")
            {
                throw new Exception("房屋Id不能为空！");
            }
            string tName = Fn.GetHouseInfoTable(CityId);

            if (Fn.GetCount_MySQL(tName, "HouseId=" + Quota + Fn.KW_Equal(HouseId) + Quota) > 0) return;

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>() { 
                new KeyValuePair<string, string>("EstId", _estId.ToString()),
                new KeyValuePair<string, string>("HouseId", HouseId),
                new KeyValuePair<string, string>("BuildNum", BuildNum),
                new KeyValuePair<string, string>("UnitNum", UnitNum),
                new KeyValuePair<string, string>("RoomNum", RoomNum),
                new KeyValuePair<string, string>("Layer", Layer.ToString()),
                new KeyValuePair<string, string>("LayerAll", LayerAll.ToString()),
                new KeyValuePair<string, string>("Structure", Structure),
                new KeyValuePair<string, string>("Usage", Usage),
                new KeyValuePair<string, string>("Area", Area.ToString()),
                new KeyValuePair<string, string>("AreaInner", AreaInner.ToString()),
                new KeyValuePair<string, string>("AreaOuter", AreaOuter.ToString()),
                new KeyValuePair<string, string>("ListDate", ListDate.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("ListPrice", ListPrice.ToString()),
                new KeyValuePair<string, string>("SoldDate", SoldDate.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("SoldPrice", SoldPrice.ToString())
            };

            if (_permitId > 0) list.Add(new KeyValuePair<string, string>("PermitId", _permitId.ToString()));

            Fn.ExecNonQuery_MySQL(Insert(tName, list));
        }
    }
}
