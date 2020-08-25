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
    /// 耗材info
    /// </summary>
    public class ConsumableInfo : EntityInfo,IComparable<ConsumableInfo>
    { 
        /// <summary>
        /// 富士II类信息
        /// </summary>
        public KeyValueInfo FujiClass2 { get; set; }
        /// <summary>
        /// 耗材名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 耗材描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 耗材类型
        /// </summary>
        public KeyValueInfo Type { get; set; }
        /// <summary>
        /// 更换频率
        /// </summary>
        public double ReplaceTimes { get; set; }
        /// <summary>
        /// 单次保养耗材成本
        /// </summary>
        public double CostPer { get; set; }
        /// <summary>
        /// 标准单价
        /// </summary>
        public double StdPrice { get; set; }
        /// <summary>
        /// 是否参与估值
        /// </summary>
        public bool IsIncluded { get; set; }
        /// <summary>
        /// 是否维保
        /// </summary>
        public bool IncludeContract { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
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
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Consumable, this.ID); } }
        /// <summary>
        /// 耗材info
        /// </summary>
        public ConsumableInfo() 
        {
            this.Type = new KeyValueInfo();
            this.FujiClass2 = new KeyValueInfo();
        }
        /// <summary>
        /// 耗材info
        /// </summary>
        /// <param name="dr">dr</param>
        public ConsumableInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.Type.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.Type.Name = LookupManager.GetConsumabletypeDesc(this.Type.ID);
            this.ReplaceTimes = SQLUtil.ConvertDouble(dr["ReplaceTimes"]);
            this.CostPer = SQLUtil.ConvertDouble(dr["CostPer"]);
            this.StdPrice = SQLUtil.ConvertDouble(dr["StdPrice"]);
            this.IsIncluded = SQLUtil.ConvertBoolean(dr["IsIncluded"]);
            this.IncludeContract = SQLUtil.ConvertBoolean(dr["IncludeContract"]);
            this.IsActive = SQLUtil.ConvertBoolean(dr["IsActive"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
        }
        /// <summary>
        /// 耗材类型
        /// </summary>
        public static class ConsumableTypes
        {
            /// <summary>
            /// 定期
            /// </summary>
            public const int RegularConsumable = 1;
            /// <summary>
            /// 定量
            /// </summary>
            public const int QuantitativeConsumable = 2;
            /// <summary>
            /// 小额成本
            /// </summary>
            public const int SmallCostConsumable = 3;
            /// <summary>
            /// 根据耗材类型id获取耗材类型描述
            /// </summary>
            /// <param name="id">耗材类型id</param>
            /// <returns>耗材类型描述</returns>
            public static string GetTypeDesc(int id)
            {
                switch (id)
                {
                    case RegularConsumable:
                        return "定期";
                    case QuantitativeConsumable:
                        return "定量";
                    case SmallCostConsumable:
                        return "小额成本";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 获取耗材修改字段dt
        /// </summary>
        /// <param name="newInfo">修改后的耗材信息</param>
        /// <returns>耗材修改字段dt</returns>
        public DataTable GetChangedFields(ConsumableInfo newInfo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");

            if (this.FujiClass2.ID != newInfo.FujiClass2.ID)
                dt.Rows.Add("ConsumableFujiClass2", SQLUtil.TrimNull(this.FujiClass2.Name), SQLUtil.TrimNull(newInfo.FujiClass2.Name));

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
                dt.Rows.Add("ConsumableName", this.Name, newInfo.Name);

            if (this.Description != SQLUtil.TrimNull(newInfo.Description))
                dt.Rows.Add("ConsumableDescription", this.Description, SQLUtil.TrimNull(newInfo.Description));

            if (this.Type.ID != newInfo.Type.ID)
                dt.Rows.Add("ConsumableType", this.Type.Name, LookupManager.GetConsumabletypeDesc(newInfo.Type.ID));

            if (this.StdPrice != newInfo.StdPrice)
                dt.Rows.Add("ConsumableStdPrice", SQLUtil.ConvertDouble(this.StdPrice), SQLUtil.ConvertDouble(newInfo.StdPrice));

            if (this.ReplaceTimes != newInfo.ReplaceTimes)
                dt.Rows.Add("ConsumableReplaceTimes", SQLUtil.ConvertDouble(this.ReplaceTimes), SQLUtil.ConvertDouble(newInfo.ReplaceTimes));

            if (this.CostPer != newInfo.CostPer)
                dt.Rows.Add("ConsumableCostPer", SQLUtil.ConvertDouble(this.CostPer), SQLUtil.ConvertDouble(newInfo.CostPer));

            if (this.IsIncluded != newInfo.IsIncluded)
                dt.Rows.Add("ConsumableIsIncluded", SQLUtil.ConvertBoolean(this.IsIncluded) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.IsIncluded) ? "是" : "否");

            if (this.IncludeContract != newInfo.IncludeContract)
                dt.Rows.Add("ConsumableIncludeContract", SQLUtil.ConvertBoolean(this.IncludeContract) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.IncludeContract) ? "是" : "否");

            if (this.IsActive != newInfo.IsActive)
                dt.Rows.Add("ConsumableIsActive", SQLUtil.ConvertBoolean(this.IsActive) ? "启用" : "停用", SQLUtil.ConvertBoolean(newInfo.IsActive) ? "启用" : "停用");

            return dt;
        }

        /// <summary>
        /// 耗材修改日志
        /// </summary>
        /// <param name="newInfo">修改后的耗材信息</param>
        /// <param name="user">操作者</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo ConvertAudit(ConsumableInfo newInfo, UserInfo user)
        {
            AuditHdrInfo audit = new AuditHdrInfo();
            audit.ObjectType.ID = ObjectTypes.Consumable;
            audit.ObjectID = this.ID;
            audit.Operation.ID = AuditHdrInfo.AuditOperations.Update;
            audit.TransUser = user;
            List<AuditDetailInfo> infos = audit.DetailInfo;

            if (this.FujiClass2.ID != newInfo.FujiClass2.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableFujiClass2", OldValue = this.FujiClass2.Name, NewValue = newInfo.FujiClass2.Name });

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableName", OldValue = this.Name, NewValue = newInfo.Name });

            if (this.Description != SQLUtil.TrimNull(newInfo.Description))
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableDescription", OldValue = this.Description, NewValue = newInfo.Description });

            if (this.Type.ID != newInfo.Type.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableType", OldValue = this.Type.Name, NewValue = newInfo.Type.Name });

            if (this.ReplaceTimes != newInfo.ReplaceTimes)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableReplaceTimes", OldValue = this.ReplaceTimes.ToString(), NewValue = newInfo.ReplaceTimes.ToString() }); 

            if (this.CostPer != newInfo.CostPer)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableCostPer", OldValue = this.CostPer.ToString(), NewValue = newInfo.CostPer.ToString() }); 

            if (this.StdPrice != newInfo.StdPrice)
                infos.Add(new AuditDetailInfo() { FieldName = "CompomemtStdPrice", OldValue = this.StdPrice.ToString(), NewValue = newInfo.StdPrice.ToString() });

            if (this.IsIncluded != newInfo.IsIncluded)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableIsIncluded", OldValue = this.IsIncluded ? "是" : "否", NewValue = newInfo.IsIncluded ? "是" : "否" });

            if (this.IncludeContract != newInfo.IncludeContract)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableIncludeContract", OldValue = this.IncludeContract ? "是" : "否", NewValue = newInfo.IncludeContract ? "是" : "否", });

            if (this.IsActive != newInfo.IsActive)
                infos.Add(new AuditDetailInfo() { FieldName = "ConsumableIsActive", OldValue = this.IsActive ? "是" : "否", NewValue = newInfo.IsActive ? "是" : "否" });

            return audit;
        }

        public int CompareTo(ConsumableInfo other)
        {
            return this.ID.CompareTo(other.ID);
        }
    }
}
