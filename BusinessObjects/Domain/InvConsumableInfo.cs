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
    /// 耗材库info
    /// </summary>
    public class InvConsumableInfo : EntityInfo
    {
        /// <summary>
        /// 耗材
        /// </summary>
        public ConsumableInfo Consumable { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string LotNum { get; set; }
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
        /// 入库数量
        /// </summary>
        public double ReceiveQty { get; set; }
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
        /// 可用数量
        /// </summary>
        public double AvaibleQty { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// OID
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.InvConsumable, this.ID); } }

        /// <summary>
        /// 采购数量
        /// </summary>
        public double Qty { get; set; }
        /// <summary>
        /// 已入库数量
        /// </summary>
        public double InboundQty { get; set; }

        /// <summary>
        /// 耗材库info
        /// </summary>
        public InvConsumableInfo() 
        {
            this.Consumable = new ConsumableInfo();
            this.Supplier = new SupplierInfo();
            this.Purchase = new KeyValueInfo();
        }
        /// <summary>
        /// 耗材库info
        /// </summary>
        /// <param name="dr">dr</param>
        public InvConsumableInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("ID"))
                this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Consumable.ID = SQLUtil.ConvertInt(dr["ConsumableID"]);
            if (dr.Table.Columns.Contains("ConsumableName"))
                this.Consumable.Name = SQLUtil.TrimNull(dr["ConsumableName"]);
            if (dr.Table.Columns.Contains("ConsumableDescription"))
                this.Consumable.Description = SQLUtil.TrimNull(dr["ConsumableDescription"]);
            if (dr.Table.Columns.Contains("LotNum"))
                this.LotNum = SQLUtil.TrimNull(dr["LotNum"]);
            this.Specification = SQLUtil.TrimNull(dr["Specification"]);
            this.Model = SQLUtil.TrimNull(dr["Model"]);
            if (dr.Table.Columns.Contains("SupplierID"))
                this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            if (dr.Table.Columns.Contains("SupplierName"))
                this.Supplier.Name = SQLUtil.TrimNull(dr["SupplierName"]);
            if (dr.Table.Columns.Contains("FujiClass2ID"))
                this.Consumable.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            if (dr.Table.Columns.Contains("FujiClass2Name"))
                this.Consumable.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Price = SQLUtil.ConvertDouble(dr["Price"]);
            if (dr.Table.Columns.Contains("ReceiveQty"))
                this.ReceiveQty = SQLUtil.ConvertDouble(dr["ReceiveQty"]);
            if (dr.Table.Columns.Contains("PurchaseDate"))
                this.PurchaseDate = SQLUtil.ConvertDateTime(dr["PurchaseDate"]);
            this.Purchase.ID = SQLUtil.ConvertInt(dr["PurchaseID"]);
            this.Purchase.Name = LookupManager.GetObjectOID(ObjectTypes.PurchaseOrder, this.Purchase.ID);
            if (dr.Table.Columns.Contains("Comments"))
                this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            if (dr.Table.Columns.Contains("AddDate"))
                this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            if (dr.Table.Columns.Contains("AvaibleQty"))
                this.AvaibleQty = SQLUtil.ConvertDouble(dr["AvaibleQty"]);
            if (dr.Table.Columns.Contains("UpdateDate"))
                this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            if (dr.Table.Columns.Contains("Qty"))
                this.Qty = SQLUtil.ConvertDouble(dr["Qty"]);
            if (dr.Table.Columns.Contains("InboundQty"))
                this.InboundQty = SQLUtil.ConvertDouble(dr["InboundQty"]);
        }
    }
}
