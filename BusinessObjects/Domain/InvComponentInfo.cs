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
    /// 零件库info
    /// </summary>
    public class InvComponentInfo : EntityInfo
    {
        /// <summary>
        /// 零件
        /// </summary>
        public ComponentInfo Component { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 零件序列号
        /// </summary>
        public string SerialCode { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public SupplierInfo Supplier { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 购入日期
        /// </summary>
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// 采购单
        /// </summary>
        public KeyValueInfo Purchase { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public KeyValueInfo Status { get; set; }
        /// <summary>
        /// OID
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.InvComponent, this.ID); } }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 已入库数量
        /// </summary>
        public int InboundQty { get; set; }

        /// <summary>
        /// 零件库info
        /// </summary>
        public InvComponentInfo() 
        {
            this.Component = new ComponentInfo();
            this.Equipment = new EquipmentInfo();
            this.Supplier = new SupplierInfo();
            this.Purchase = new KeyValueInfo();
            this.Status = new KeyValueInfo();
        }
        /// <summary>
        /// 零件库info
        /// </summary>
        /// <param name="dr">dr</param>
        public InvComponentInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("ID"))
                this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Component.ID = SQLUtil.ConvertInt(dr["ComponentID"]);
            if (dr.Table.Columns.Contains("ComponentName"))
                this.Component.Name = SQLUtil.TrimNull(dr["ComponentName"]);
            if (dr.Table.Columns.Contains("ComponentDescription"))
                this.Component.Description = SQLUtil.TrimNull(dr["ComponentDescription"]);
            if (dr.Table.Columns.Contains("ComponentTypeID"))
            {
                this.Component.Type.ID = SQLUtil.ConvertInt(dr["ComponentTypeID"]);
                this.Component.Type.Name = LookupManager.GetComponentTypeDesc(this.Component.Type.ID);
            }
            this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            if (dr.Table.Columns.Contains("EquipmentName"))
                this.Equipment.Name = SQLUtil.TrimNull(dr["EquipmentName"]);
            if (dr.Table.Columns.Contains("EquipmentFujiClass2ID"))
                this.Equipment.FujiClass2.ID = SQLUtil.ConvertInt(dr["EquipmentFujiClass2ID"]);
            if (dr.Table.Columns.Contains("SerialCode"))
                this.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
            this.Specification = SQLUtil.TrimNull(dr["Specification"]);
            this.Model = SQLUtil.TrimNull(dr["Model"]);
            if (dr.Table.Columns.Contains("SupplierID"))
                this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            if (dr.Table.Columns.Contains("SupplierName"))
                this.Supplier.Name = SQLUtil.TrimNull(dr["SupplierName"]);
            this.Price = SQLUtil.ConvertDouble(dr["Price"]);
            if (dr.Table.Columns.Contains("PurchaseDate"))
                this.PurchaseDate = SQLUtil.ConvertDateTime(dr["PurchaseDate"]);
            this.Purchase.ID = SQLUtil.ConvertInt(dr["PurchaseID"]);
            this.Purchase.Name = LookupManager.GetObjectOID(ObjectTypes.PurchaseOrder, this.Purchase.ID);
            if (dr.Table.Columns.Contains("Comments"))
                this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            if (dr.Table.Columns.Contains("AddDate"))
                this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            if (dr.Table.Columns.Contains("UpdateDate"))
                this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            if (dr.Table.Columns.Contains("StatusID"))
            {
                this.Status.ID = SQLUtil.ConvertInt(dr["StatusID"]);
                this.Status.Name = ComponentStatus.GetTypeDesc(this.Status.ID);
            }
            if (dr.Table.Columns.Contains("Qty"))
                this.Qty = SQLUtil.ConvertInt(dr["Qty"]);
            if (dr.Table.Columns.Contains("InboundQty"))
                this.InboundQty = SQLUtil.ConvertInt(dr["InboundQty"]);

        }
        /// <summary>
        /// 状态
        /// </summary>
        public static class ComponentStatus
        {
            /// <summary>
            /// 在库
            /// </summary>
            public const int InStock = 1;
            /// <summary>
            /// 已用
            /// </summary>
            public const int Used = 2;
            /// <summary>
            /// 报废
            /// </summary>
            public const int Scrap = 3;

            /// <summary>
            /// 获取零件状态列表
            /// </summary>
            /// <returns>零件状态列表</returns>
            public static List<KeyValueInfo> GetComponentStatus()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = ComponentStatus.InStock, Name = GetTypeDesc(ComponentStatus.InStock) });
                statuses.Add(new KeyValueInfo() { ID = ComponentStatus.Used, Name = GetTypeDesc(ComponentStatus.Used) });
                statuses.Add(new KeyValueInfo() { ID = ComponentStatus.Scrap, Name = GetTypeDesc(ComponentStatus.Scrap) });
                return statuses;
            }
            /// <summary>
            /// 根据零件状态id获取描述
            /// </summary>
            /// <param name="id">零件状态id</param>
            /// <returns>描述</returns>
            public static string GetTypeDesc(int id)
            {
                switch(id)
                {
                    case InStock:
                        return "在库";
                    case Used:
                        return "已用";
                    case Scrap:
                        return "报废";
                    default:
                        return "";
                }
            }
        }
    }
}
