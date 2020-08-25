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
    /// 采购单info
    /// </summary>
    public class PurchaseOrderInfo : EntityInfo
    {
        /// <summary>
        /// 请求者
        /// </summary>
        public UserInfo User { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public SupplierInfo Supplier { get; set; }
        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 到货日期
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        /// <value>
        /// The fuji comments.
        /// </value>
        public string FujiComments { get; set; }
        /// <summary>
        /// 采购单状态
        /// </summary>
        public KeyValueInfo Status { get; set; }
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
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.PurchaseOrder, this.ID); } }
        /// <summary>
        /// 零件
        /// </summary>
        public List<InvComponentInfo> Components { get; set; }
        /// <summary>
        /// 耗材
        /// </summary>
        public List<InvConsumableInfo> Consumables { get; set; }
        /// <summary>
        /// 服务
        /// </summary>
        public List<InvServiceInfo> Services { get; set; }

        /// <summary>
        /// 历史信息
        /// </summary>
        /// <value>
        /// The histories.
        /// </value>
        public List<HistoryInfo> Histories { get; set; }

        /// <summary>
        /// 采购单info
        /// </summary>
        public PurchaseOrderInfo() 
        {
            this.User = new UserInfo();
            this.Supplier = new SupplierInfo();
            this.Status = new KeyValueInfo();
        }
        /// <summary>
        /// 采购单info
        /// </summary>
        /// <param name="dr">dr</param>
        public PurchaseOrderInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            if (dr.Table.Columns.Contains("UserName"))
                this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            if (dr.Table.Columns.Contains("SupplierName"))
                this.Supplier.Name = SQLUtil.TrimNull(dr["SupplierName"]);
            this.OrderDate = SQLUtil.ConvertDateTime(dr["OrderDate"]);
            this.DueDate = SQLUtil.ConvertDateTime(dr["DueDate"]);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.Status.ID = SQLUtil.ConvertInt(dr["StatusID"]);
            this.Status.Name = LookupManager.GetPurchaseOrderStatusDesc(this.Status.ID);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
        }

        /// <summary>
        /// 采购单状态
        /// </summary>
        public static class PurchaseOrderStatus
        {
            /// <summary>
            /// 已终止
            /// </summary>
            public const int Cancelled = -1;
            /// <summary>
            /// 新建
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 待审批
            /// </summary>
            public const int Pending = 2;
            /// <summary>
            /// 待入库
            /// </summary>
            public const int Stocking = 3;
            /// <summary>
            /// 已完成
            /// </summary>
            public const int Ended = 9;
        }

        /// <summary>
        /// 流程信息
        /// </summary>
        /// <value>
        /// The format history.
        /// </value>
        public string FormatHistory { get; set; }
        /// <summary>
        /// 设置流程历史信息
        /// </summary>
        public void SetHis4Comments()
        {
            StringBuilder sb = new StringBuilder();
            foreach (HistoryInfo history in this.Histories)
            {
                sb.AppendLine(history.VerifyHistory);
            }
            this.FormatHistory = sb.ToString();
        }

        /// <summary>
        /// 采购单流程操作
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// 提交
            /// </summary>
            public const int Submit = 1;
            /// <summary>
            /// 通过
            /// </summary>
            public const int Pass = 2;
            /// <summary>
            /// 退回
            /// </summary>
            public const int Reject = 3;
            /// <summary>
            /// 入库
            /// </summary>
            public const int Inbound = 4;
            /// <summary>
            /// 完成
            /// </summary>
            public const int End = 5;
            /// <summary>
            /// 终止
            /// </summary>
            public const int Cancelled = 9;
            /// <summary>
            /// 根据采购单流程操作编号获取采购单流程操作描述信息
            /// </summary>
            /// <param name="actionID">采购单流程操作编号</param>
            /// <returns>采购单流程操作描述</returns>
            public static string GetDesc(int actionID)
            {
                switch (actionID)
                {
                    case Submit:
                        return "提交";
                    case Pass:
                        return "通过";
                    case Reject:
                        return "退回";
                    case Inbound:
                        return "入库";
                    case Cancelled:
                        return "终止";
                    case End:
                        return "完成";
                    default:
                        return "";
                }
            }
        }
    }
}
