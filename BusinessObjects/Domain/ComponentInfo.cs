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
    /// 零件info
    /// </summary>
    public class ComponentInfo : EntityInfo, IComparable<ComponentInfo>
    { 
        /// <summary>
        /// 富士II类信息
        /// </summary>
        public KeyValueInfo FujiClass2 { get; set; }
        /// <summary>
        /// 零件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 零件描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 零件类型
        /// </summary>
        public KeyValueInfo Type { get; set; }
        /// <summary>
        /// 零件标准单价
        /// </summary>
        public double StdPrice { get; set; }
        /// <summary>
        /// 标准使用量
        /// </summary>
        public int Usage { get; set; }
        /// <summary>
        /// CT球管 理论使用寿命
        /// </summary>
        public int TotalSeconds { get; set; }
        /// <summary>
        /// CT球管 秒次/人
        /// </summary>
        public double SecondsPer { get; set; }
        /// <summary>
        /// 是否参与估值
        /// </summary>
        public bool IsIncluded { get; set; }
        /// <summary>
        /// 是否维保
        /// </summary>
        public bool IncludeContract { get; set; }
        /// <summary>
        /// 故障率计算方式
        /// </summary>
        public FujiClass2Info.Method Method { get; set; }
        /// <summary>
        /// 定期零件更换月
        /// </summary>
        public int LifeTime { get; set; }
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
        /// 故障率
        /// </summary>
        public List<FaultRateInfo> FaultRates { get; set; }
        /// <summary>
        /// OID
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Component, this.ID); } }
        /// <summary>
        /// 零件info
        /// </summary>
        public ComponentInfo() 
        {
            this.Type = new KeyValueInfo();
            this.FujiClass2 = new KeyValueInfo(); 
            this.FaultRates = new List<FaultRateInfo>();
        }
        /// <summary>
        /// 零件info
        /// </summary>
        /// <param name="dr">dr</param>
        public ComponentInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.Type.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.Type.Name = LookupManager.GetComponentTypeDesc(this.Type.ID);
            this.StdPrice = SQLUtil.ConvertDouble(dr["StdPrice"]);
            this.Usage = SQLUtil.ConvertInt(dr["Usage"]);
            this.TotalSeconds = SQLUtil.ConvertInt(dr["TotalSeconds"]);
            this.SecondsPer = SQLUtil.ConvertDouble(dr["SecondsPer"]);
            this.IsIncluded = SQLUtil.ConvertBoolean(dr["IsIncluded"]);
            this.IncludeContract = SQLUtil.ConvertBoolean(dr["IncludeContract"]);
            this.Method = SQLUtil.ConvertEnum<FujiClass2Info.Method>(dr["MethodID"]); 
            this.LifeTime = SQLUtil.ConvertInt(dr["LifeTime"]);
            this.IsActive = SQLUtil.ConvertBoolean(dr["IsActive"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
        }
        /// <summary>
        /// 零件类型
        /// </summary>
        public static class ComponentTypes
        {
            /// <summary>
            /// 重要零件
            /// </summary>
            public const int ImportantComponent = 1;
            /// <summary>
            /// 一般零件
            /// </summary>
            public const int GeneralComponent = 2;
            /// <summary>
            /// CT球管
            /// </summary>
            public const int CT = 3;
            /// <summary>
            /// 根据零件类型id获取描述
            /// </summary>
            /// <param name="id">零件类型id</param>
            /// <returns>零件类型描述</returns>
            public static string GetTypeDesc(int id)
            {
                switch(id)
                {
                    case ImportantComponent:
                        return "重要零件";
                    case GeneralComponent:
                        return "一般零件";
                    case CT:
                        return "CT球管";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 获取修改的字段（日志）
        /// </summary>
        /// <param name="newInfo">修改后的信息</param>
        /// <returns>修改字段dt</returns>
        public DataTable GetChangedFields(ComponentInfo newInfo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");

            if(this.FujiClass2.ID != newInfo.FujiClass2.ID)
                dt.Rows.Add("ComponentFujiClass2", SQLUtil.TrimNull(this.FujiClass2.Name), SQLUtil.TrimNull(newInfo.FujiClass2.Name));

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
                dt.Rows.Add("ComponentName", this.Name, newInfo.Name);

            if (this.Description != SQLUtil.TrimNull(newInfo.Description))
                dt.Rows.Add("ComponentDescription", this.Description, SQLUtil.TrimNull(newInfo.Description));

            if (this.Type.ID != newInfo.Type.ID)
                dt.Rows.Add("ComponentType", this.Type.Name, LookupManager.GetComponentTypeDesc(newInfo.Type.ID));

            if (this.StdPrice != newInfo.StdPrice)
                dt.Rows.Add("CompomemtStdPrice", SQLUtil.ConvertDouble(this.StdPrice), SQLUtil.ConvertDouble(newInfo.StdPrice));

            if (this.Usage != newInfo.Usage)
                dt.Rows.Add("ComponentUsage", SQLUtil.ConvertInt(this.Usage), SQLUtil.ConvertInt(newInfo.Usage));

            if (this.TotalSeconds != newInfo.TotalSeconds)
                dt.Rows.Add("ComponentTotalSeconds", SQLUtil.ConvertDouble(this.TotalSeconds), SQLUtil.ConvertDouble(newInfo.TotalSeconds));

            if (this.SecondsPer != newInfo.SecondsPer)
                dt.Rows.Add("ComponentSecondsPer", SQLUtil.ConvertDouble(this.SecondsPer), SQLUtil.ConvertDouble(newInfo.SecondsPer));

            if (this.IsIncluded != newInfo.IsIncluded)
                dt.Rows.Add("ComponentIsIncluded", SQLUtil.ConvertBoolean(this.IsIncluded) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.IsIncluded) ? "是" : "否");

            if (this.IncludeContract != newInfo.IncludeContract)
                dt.Rows.Add("ComponentIncludeContract", SQLUtil.ConvertBoolean(this.IncludeContract) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.IncludeContract) ? "是" : "否");

            if (this.Method != newInfo.Method) 
                dt.Rows.Add("ComponentMethodID", this.Method.GetDescription(),  newInfo.Method.GetDescription());

            if (this.IsActive != newInfo.IsActive)
                dt.Rows.Add("ComponentIsActive", SQLUtil.ConvertBoolean(this.IsActive) ? "启用" : "停用", SQLUtil.ConvertBoolean(newInfo.IsActive) ? "启用" : "停用");

            return dt;
        }
         /// <summary>
         /// 更新日志
         /// </summary>
         /// <param name="newInfo">修改后的零件信息</param>
         /// <param name="user">操作者</param>
         /// <returns>日志信息</returns>
        public AuditHdrInfo ConvertAudit(ComponentInfo newInfo, UserInfo user)
        {
            AuditHdrInfo audit = new AuditHdrInfo();
            audit.ObjectType.ID = ObjectTypes.Component;
            audit.ObjectID = this.ID;
            audit.Operation.ID = AuditHdrInfo.AuditOperations.Update;
            audit.TransUser = user;
            List<AuditDetailInfo> infos = audit.DetailInfo;

            if (this.FujiClass2.ID != newInfo.FujiClass2.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentFujiClass2", OldValue = this.FujiClass2.Name, NewValue = newInfo.FujiClass2.Name });

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentName", OldValue = this.Name, NewValue = newInfo.Name });

            if (this.Description != SQLUtil.TrimNull(newInfo.Description))
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentDescription", OldValue = this.Description, NewValue = newInfo.Description });

            if (this.Type.ID != newInfo.Type.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentType", OldValue = this.Type.Name, NewValue = LookupManager.GetComponentTypeDesc(newInfo.Type.ID) });

            if (this.StdPrice != newInfo.StdPrice)
                infos.Add(new AuditDetailInfo() { FieldName = "CompomemtStdPrice", OldValue = this.StdPrice.ToString(), NewValue = newInfo.StdPrice.ToString() });

            if (this.Usage != newInfo.Usage)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentUsage", OldValue = this.Usage.ToString(), NewValue = newInfo.Usage.ToString() });

            if (this.Method != newInfo.Method)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentMethodID", OldValue = this.Method.GetDescription(), NewValue = newInfo.Method.GetDescription() });

            if (this.TotalSeconds != newInfo.TotalSeconds)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentTotalSeconds", OldValue = this.TotalSeconds.ToString(), NewValue = newInfo.TotalSeconds.ToString() });

            if (this.SecondsPer != newInfo.SecondsPer)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentSecondsPer", OldValue = this.SecondsPer.ToString(), NewValue = newInfo.SecondsPer.ToString() });

            if (this.IsIncluded != newInfo.IsIncluded)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentIsIncluded", OldValue = this.IsIncluded ? "是" : "否", NewValue = newInfo.IsIncluded ? "是" : "否" });

            if (this.IncludeContract != newInfo.IncludeContract)
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentIncludeContract", OldValue = this.IncludeContract ? "是" : "否", NewValue = newInfo.IncludeContract ? "是" : "否", });

            if (this.IsActive != newInfo.IsActive) 
                infos.Add(new AuditDetailInfo() { FieldName = "ComponentIsActive", OldValue = this.IsActive ? "是" : "否", NewValue = newInfo.IsActive ? "是" : "否" });

            return audit;
        }

        public int CompareTo(ComponentInfo other)
        {
            return this.ID.CompareTo(other.ID);
        }
    }
}
