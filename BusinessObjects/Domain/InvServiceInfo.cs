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
    /// 服务库info
    /// </summary>
    public class InvServiceInfo : EntityInfo
    {
        /// <summary>
        /// 富士II类
        /// </summary>
        public FujiClass2Info FujiClass2 { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务次数
        /// </summary>
        public int TotalTimes { get; set; }
        /// <summary>
        /// 金额
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
        /// 供应商
        /// </summary>
        public SupplierInfo Supplier { get; set; }
        /// <summary>
        /// 采购单
        /// </summary>
        public KeyValueInfo Purchase { get; set; }
        /// <summary>
        /// 购入日期
        /// </summary>
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 可用服务次数
        /// </summary>
        public int AvaibleTimes { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// OID
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.InvService, this.ID); } }

        /// <summary>
        /// 是否已入库
        /// </summary>
        public bool Inbounded { get; set; }

        //display only
        /// <summary>
        /// 根据合同起止时间计算合同状态
        /// </summary>
        public string Status
        {
            get
            {
                int statusId = 0;
                if ((this.EndDate - DateTime.Now).Days < 0)
                    statusId = Statuses.Expired;
                else if ((this.StartDate - DateTime.Now).Days > 0)
                    statusId = Statuses.Pending;
                else if ((this.StartDate - DateTime.Now).Days <= 0 && (int)Math.Ceiling((this.EndDate - DateTime.Now).TotalDays) <= ControlManager.GetSettingInfo().WillExpireTime)
                    statusId = Statuses.WillExpire;
                else
                    statusId = Statuses.Active;

                return Statuses.GetStatusDesc(statusId);
            }
        }

        /// <summary>
        /// 服务库info
        /// </summary>
        public InvServiceInfo() 
        {
            this.FujiClass2 = new FujiClass2Info();
            this.Supplier = new SupplierInfo();
            this.Purchase = new KeyValueInfo();
        }
        /// <summary>
        /// 服务库info
        /// </summary>
        /// <param name="dr">dr</param>
        public InvServiceInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("ID"))
                this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            if (dr.Table.Columns.Contains("FujiClass2Name"))
                this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.TotalTimes = SQLUtil.ConvertInt(dr["TotalTimes"]);
            this.Price = SQLUtil.ConvertDouble(dr["Price"]);
            this.StartDate = SQLUtil.ConvertDateTime(dr["StartDate"]);
            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            if (dr.Table.Columns.Contains("SupplierID"))
                this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            if (dr.Table.Columns.Contains("SupplierName"))
                this.Supplier.Name = SQLUtil.TrimNull(dr["SupplierName"]);
            this.Purchase.ID = SQLUtil.ConvertInt(dr["PurchaseID"]);
            this.Purchase.Name = LookupManager.GetObjectOID(ObjectTypes.PurchaseOrder, this.Purchase.ID);
            if (dr.Table.Columns.Contains("PurchaseDate"))
                this.PurchaseDate = SQLUtil.ConvertDateTime(dr["PurchaseDate"]);
            if (dr.Table.Columns.Contains("Comments"))
                this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            if (dr.Table.Columns.Contains("AddDate"))
                this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            if (dr.Table.Columns.Contains("AvaibleTimes"))
                this.AvaibleTimes = SQLUtil.ConvertInt(dr["AvaibleTimes"]);
            if (dr.Table.Columns.Contains("UpdateDate"))
                this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            if (dr.Table.Columns.Contains("Inbounded"))
                this.Inbounded = SQLUtil.ConvertBoolean(dr["Inbounded"]);
        }
        /// <summary>
        /// 服务状态
        /// </summary>
        public static class Statuses
        {
            /// <summary>
            /// 失效
            /// </summary>
            public const int Expired = 1;
            /// <summary>
            /// 生效
            /// </summary>
            public const int Active = 2;
            /// <summary>
            /// 未生效
            /// </summary>
            public const int Pending = 3;
            /// <summary>
            /// 即将失效
            /// </summary>
            public const int WillExpire = 4;
            /// <summary>
            /// 根据服务状态编号获取查询该状态服务的sql
            /// </summary>
            /// <param name="statusId">服务状态编号</param>
            /// <returns>查询该状态服务的sql</returns>
            public static string GetSqlFilter(int statusId)
            {
                switch (statusId)
                {
                    case Expired:
                        return " AND DATEDIFF(DAY,se.EndDate, GETDATE()) > 0";
                    case WillExpire:
                        return " AND DATEDIFF(DAY,se.StartDate, GETDATE()) >= 0 AND DATEDIFF(DAY, se.EndDate, GETDATE()) >= -" + ControlManager.GetSettingInfo().WillExpireTime + " AND DATEDIFF(DAY,se.EndDate, GETDATE()) <= 0 ";
                    case Active:
                        return " AND DATEDIFF(DAY,se.StartDate, GETDATE()) >= 0 AND DATEDIFF(DAY,se.EndDate, GETDATE()) <= 0";
                    case Pending:
                        return " AND (DATEDIFF(DAY,se.StartDate, GETDATE()) < 0 OR DATEDIFF(SECOND,se.StartDate, GETDATE()) < 0 )";
                    default:
                        return "";
                }
            }
            /// <summary>
            /// 获取服务状态信息
            /// </summary>
            /// <returns>服务状态信息</returns>
            public static List<KeyValueInfo> GetStatuses()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = Statuses.Expired, Name = GetStatusDesc(Statuses.Expired) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.Active, Name = GetStatusDesc(Statuses.Active) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.Pending, Name = GetStatusDesc(Statuses.Pending) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.WillExpire, Name = GetStatusDesc(Statuses.WillExpire) });
                return statuses;
            }
            /// <summary>
            /// 根据服务状态编号获取服务状态描述
            /// </summary>
            /// <param name="statusId">服务状态编号</param>
            /// <returns>服务状态描述</returns>
            public static string GetStatusDesc(int statusId)
            {
                switch (statusId)
                {
                    case Statuses.Expired:
                        return "失效";
                    case Statuses.Active:
                        return "生效";
                    case Statuses.Pending:
                        return "未生效";
                    case Statuses.WillExpire:
                        return "即将失效";
                    default:
                        return "全部";
                }
            }
        }
    }
}
