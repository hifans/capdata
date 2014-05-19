using System;
using System.Collections.Generic;
using System.Text;

namespace CapData
{
    /// <summary>
    /// 法院拍卖网数据模型
    /// </summary>
    class Model_House_Court_Auction:Model
    {
        /// <summary>
        /// 根据内容来源链接生成的MD5值
        /// </summary>
        public string MD5 { get { return Fn.GetMD5(SourceLink); } }

        /// <summary>
        /// 内容来源链接
        /// </summary>
        public string SourceLink = "";

        /// <summary>
        /// 项目名称
        /// </summary>
        public string PrjName = "";

        /// <summary>
        /// 委托法院
        /// </summary>
        public string CourtEntrust = "";

        /// <summary>
        /// 法院所在地
        /// </summary>
        public string CourtPlace = "";

        /// <summary>
        /// 成交结果
        /// </summary>
        public string AuctionResult = "";

        /// <summary>
        /// 起拍价（元）
        /// </summary>
        public double StartingPrice = 0;

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PubDate = DateTime.Today;

        /// <summary>
        /// 使用确定的城市名称
        /// </summary>
        /// <param name="cityName"></param>
        public Model_House_Court_Auction(string[] listCityName)
        {
            this.DBType = DataBaseType.MySQL;
            for (var i = listCityName.Length - 1; i >= 0; i--)
            {
                int cid = Fn.GetCityIdByName(listCityName[i].TrimEnd('市'));
                if (cid == -1) continue;

                this._cityId = cid;
                break;
            }
        }

        /// <summary>
        /// 保存数据，如果数据已存在，则不覆盖（返回Id值为0），如果数据Id为0，则表示无法找到合适的城市
        /// <returns>数据Id</returns>
        /// </summary>
        public int Save()
        {
            return Save(false);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="Cover">是否需要强制覆盖</param>
        /// <returns>数据Id；不需要覆盖且数据存在时，返回0，城市信息错误时，也返回0</returns>
        public int Save(bool Cover)
        {
            string tName = "house_court_auction";
            if (CityId == 0) return 0;
            int id = 0;
            try
            {
                id = int.Parse(Fn.GetObj_MySQL("select id from " + tName + " where md5='" + MD5 + "'").ToString());
            }
            catch { }
            if (Cover && id > 0) return 0;

            List<KeyValuePair<string, string>> listKV = new List<KeyValuePair<string, string>>() { 
                new KeyValuePair<string, string>("CityId",CityId.ToString()),
                new KeyValuePair<string, string>("MD5",MD5),
                new KeyValuePair<string, string>("SourceLink",SourceLink),
                new KeyValuePair<string, string>("PrjName",PrjName),
                new KeyValuePair<string, string>("CourtEntrust",CourtEntrust),
                new KeyValuePair<string, string>("CourtPlace",CourtPlace),
                new KeyValuePair<string, string>("AuctionResult",AuctionResult),
                new KeyValuePair<string, string>("StartingPrice",StartingPrice.ToString()),
                new KeyValuePair<string, string>("PubDate",PubDate.ToString("yyyy-MM-dd"))
            };

            if (id > 0) Fn.ExecNonQuery_MySQL(Update(tName, listKV, "id='" + id.ToString() + "'"));
            else
            {
                Fn.ExecNonQuery_MySQL(Insert(tName, listKV));
                try
                {
                    id = int.Parse(Fn.GetObj_MySQL("select id from " + tName + " where md5='" + MD5 + "'").ToString());
                }
                catch { }
            }

            return id;
        }
    }
}
