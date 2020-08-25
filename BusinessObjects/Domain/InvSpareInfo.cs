using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 备用机库info
    /// </summary>
    public class InvSpareInfo : EntityInfo
    {
        /// <summary>
        /// 富士II类
        /// </summary>
        public FujiClass2Info FujiClass2 { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialCode { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// OID
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.InvSpare, this.ID); } }

        //display only
        /// <summary>
        /// 根据备用机起止时间计算备用机状态
        /// </summary>
        public string Status
        {
            get
            {
                int statusId = 0;
                if ((this.EndDate - DateTime.Now).Days < 0)
                    statusId = SpareStatus.Expired;
                else if ((this.StartDate - DateTime.Now).Days > 0)
                    statusId = SpareStatus.Unused;
                else
                    statusId = SpareStatus.Used;

                return SpareStatus.GetTypeDesc(statusId);
            }
        }

        /// <summary>
        /// 备用机库info
        /// </summary>
        public InvSpareInfo() 
        {
            this.FujiClass2 = new FujiClass2Info();
        }
        /// <summary>
        /// 备用机库info
        /// </summary>
        /// <param name="dr">dr</param>
        public InvSpareInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            if (dr.Table.Columns.Contains("FujiClass2Name"))
                this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
            this.Price = SQLUtil.ConvertDouble(dr["Price"]);
            this.StartDate = SQLUtil.ConvertDateTime(dr["StartDate"]);
            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
        }
        /// <summary>
        /// 备用机状态
        /// </summary>
        public static class SpareStatus
        {
            /// <summary>
            /// 未使用
            /// </summary>
            public const int Unused = 1;
            /// <summary>
            /// 当前在用
            /// </summary>
            public const int Used = 2;
            /// <summary>
            /// 已经过期
            /// </summary>
            public const int Expired = 3;
            /// <summary>
            /// 根据备用机状态编号获取查询该状态备用机的sql
            /// </summary>
            /// <param name="statusId">备用机状态编号</param>
            /// <returns>查询该状态备用机的sql</returns>
            public static string GetSqlFilter(int statusId)
            {
                switch (statusId)
                {
                    case Expired:
                        return " AND DATEDIFF(DAY,sp.EndDate, GETDATE()) > 0";
                    case Used:
                        return " AND DATEDIFF(DAY,sp.StartDate, GETDATE()) >= 0 AND DATEDIFF(DAY,sp.EndDate, GETDATE()) <= 0";
                    case Unused:
                        return " AND (DATEDIFF(DAY,sp.StartDate, GETDATE()) < 0 OR DATEDIFF(SECOND,sp.StartDate, GETDATE()) < 0 )";
                    default:
                        return "";
                }
            }
            /// <summary>
            /// 获取备用机状态列表
            /// </summary>
            /// <returns>备用机状态列表</returns>
            public static List<KeyValueInfo> GetSpareStatus()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = SpareStatus.Unused, Name = GetTypeDesc(SpareStatus.Unused) });
                statuses.Add(new KeyValueInfo() { ID = SpareStatus.Used, Name = GetTypeDesc(SpareStatus.Used) });
                statuses.Add(new KeyValueInfo() { ID = SpareStatus.Expired, Name = GetTypeDesc(SpareStatus.Expired) });
                return statuses;
            }

            /// <summary>
            /// 根据备用机状态id获取描述
            /// </summary>
            /// <param name="id">备用机状态id</param>
            /// <returns>描述</returns>
            public static string GetTypeDesc(int id)
            {
                switch (id)
                {
                    case Unused:
                        return "未使用";
                    case Used:
                        return "当前在用";
                    case Expired:
                        return "已经过期";
                    default:
                        return "";
                }
            }
        }
    }
}
