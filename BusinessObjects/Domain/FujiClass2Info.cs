using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 富士II类info
    /// </summary>
    public class FujiClass2Info : EntityInfo, IComparable<FujiClass2Info>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 富士I类
        /// </summary>
        public FujiClass1Info FujiClass1 { get; set; }
        /// <summary>
        /// 是否包含人工费
        /// </summary>
        public Boolean IncludeLabour { get; set; }
        /// <summary>
        /// 巡检次数
        /// </summary>
        public double PatrolTimes { get; set; }
        /// <summary>
        /// 巡检工时
        /// </summary>
        public double PatrolHours { get; set; }
        /// <summary>
        /// 保养次数
        /// </summary>
        public double MaintenanceTimes { get; set; }
        /// <summary>
        /// 保养工时
        /// </summary>
        public double MaintenanceHours { get; set; }
        /// <summary>
        /// 维修平均工时
        /// </summary>
        public double RepairHours { get; set; }
        /// <summary>
        /// 是否包含维保服务费
        /// </summary>
        public Boolean IncludeContract { get; set; }
        /// <summary>
        /// 全保单价占设备金额百分比
        /// </summary>
        public double FullCoveragePtg { get; set; }
        /// <summary>
        /// TechCoveragePtg
        /// </summary>
        public double TechCoveragePtg { get; set; }
        /// <summary>
        /// 是否包含备用机成本
        /// </summary>
        public Boolean IncludeSpare { get; set; }
        /// <summary>
        /// 备用机标准单价
        /// </summary>
        public double SparePrice { get; set; }
        /// <summary>
        /// 月租占标准单价比率
        /// </summary>
        public double SpareRentPtg { get; set; }
        /// <summary>
        /// 是否包含维保额外维修费
        /// </summary>
        public Boolean IncludeRepair { get; set; }
        /// <summary>
        /// 使用量
        /// </summary>
        public int Usage { get; set; }
        /// <summary>
        /// 设备等级
        /// </summary>
        public KeyValueInfo EquipmentType { get; set; }
        /// <summary>
        /// 单次维修平均零件成本
        /// </summary>
        public double RepairComponentCost { get; set; }
        /// <summary>
        /// 工程师无法修复概率
        /// </summary>
        public double Repair3partyRatio { get; set; }
        /// <summary>
        /// 购维修服务平均成本
        /// </summary>
        public double Repair3partyCost { get; set; }
        /// <summary>
        /// 故障成本占设备金额比例
        /// </summary>
        public double RepairCostRatio { get; set; }
        /// <summary>
        /// 故障率计算方式
        /// </summary>
        public Method MethodID { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 整机故障率
        /// </summary>
        public List<FaultRateInfo> Repairs { get; set; }
        /// <summary>
        /// 零件信息
        /// </summary>
        public List<ComponentInfo> Components { get; set; }
        /// <summary>
        /// 耗材信息
        /// </summary>
        public List<ConsumableInfo> Consumables { get; set; }

        // display only
        /// <summary>
        /// 是否已完善信息
        /// </summary>
        public bool hasEdited { get; set; }
        /// <summary>
        /// 富士II类info
        /// </summary>
        public FujiClass2Info()
        {
            this.FujiClass1 = new FujiClass1Info();
            this.EquipmentType = new KeyValueInfo();
            this.Repairs = new List<FaultRateInfo>();
            this.Components = new List<ComponentInfo>();
            this.Consumables = new List<ConsumableInfo>();
        }
        /// <summary>
        /// 富士II类info
        /// </summary>
        /// <param name="dr">dr</param>
        public FujiClass2Info(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.FujiClass1.ID = SQLUtil.ConvertInt(dr["FujiClass1ID"]);
            if (dr.Table.Columns.Contains("FujiClass1Name"))
                this.FujiClass1.Name = SQLUtil.TrimNull(dr["FujiClass1Name"]);

            this.IncludeLabour = SQLUtil.ConvertBoolean(dr["IncludeLabour"]);
            this.PatrolTimes = SQLUtil.ConvertDouble(dr["PatrolTimes"]);
            this.PatrolHours = SQLUtil.ConvertDouble(dr["PatrolHours"]);
            this.MaintenanceTimes = SQLUtil.ConvertDouble(dr["MaintenanceTimes"]);
            this.MaintenanceHours = SQLUtil.ConvertDouble(dr["MaintenanceHours"]);
            this.RepairHours = SQLUtil.ConvertDouble(dr["RepairHours"]);

            this.IncludeContract = SQLUtil.ConvertBoolean(dr["IncludeContract"]);
            this.FullCoveragePtg = SQLUtil.ConvertDouble(dr["FullCoveragePtg"]);
            this.TechCoveragePtg = SQLUtil.ConvertDouble(dr["TechCoveragePtg"]);

            this.IncludeSpare = SQLUtil.ConvertBoolean(dr["IncludeSpare"]);
            this.SparePrice = SQLUtil.ConvertDouble(dr["SparePrice"]);
            this.SpareRentPtg = SQLUtil.ConvertDouble(dr["SpareRentPtg"]);

            this.IncludeRepair = SQLUtil.ConvertBoolean(dr["IncludeRepair"]);
            this.Usage = SQLUtil.ConvertInt(dr["Usage"]);
            this.EquipmentType.ID = SQLUtil.ConvertInt(dr["EquipmentType"]);
            this.EquipmentType.Name = LookupManager.GetEquipmentTypeDesc(this.EquipmentType.ID);
            this.RepairComponentCost = SQLUtil.ConvertDouble(dr["RepairComponentCost"]);
            this.Repair3partyRatio = SQLUtil.ConvertDouble(dr["Repair3partyRatio"]);
            this.Repair3partyCost = SQLUtil.ConvertDouble(dr["Repair3partyCost"]);
            this.RepairCostRatio = SQLUtil.ConvertDouble(dr["RepairCostRatio"]);
            this.MethodID = SQLUtil.ConvertEnum<Method>(dr["MethodID"]);

            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
        }
        /// <summary>
        /// 判断设备类别
        /// </summary>
        public void CheckEquipmentTypeRelatedFields()
        {
            if (this.EquipmentType.ID == LKPEquipmentType.General) {
                this.RepairComponentCost = 0d;
                this.Repair3partyRatio = 0d;
                this.Repair3partyCost = 0d;
            }
            else
            {
                this.RepairCostRatio = 0d;
            }
        } 
        /// <summary>
        /// 日志信息
        /// </summary>
        /// <param name="newInfo">修改后的信息</param>
        /// <param name="user">修改者</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo ConvertAudit(FujiClass2Info newInfo,UserInfo user)//DataTable
        {

            AuditHdrInfo audit = new AuditHdrInfo();
            audit.ObjectType.ID = ObjectTypes.FujiClass2;
            audit.ObjectID = this.ID;
            audit.Operation.ID = AuditHdrInfo.AuditOperations.Update;
            audit.TransUser = user;
            List<AuditDetailInfo> infos = audit.DetailInfo;

            if (this.FujiClass1.ID != newInfo.FujiClass1.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2FujiClass1", OldValue = this.FujiClass1.Name, NewValue = newInfo.FujiClass1.Name }); 

            if (!this.Name.Equals(newInfo.Name))
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Name", OldValue = this.Name, NewValue = newInfo.Name });

            if (!this.Description.Equals(newInfo.Description))
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Description", OldValue = this.Description, NewValue = newInfo.Description });

            if (this.IncludeLabour != newInfo.IncludeLabour)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2IncludeLabour", OldValue = this.IncludeLabour ? "是" : "否", NewValue = newInfo.IncludeLabour ? "是" : "否" });

            if (this.PatrolTimes != newInfo.PatrolTimes)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2PatrolTimes", OldValue = this.PatrolTimes.ToString(), NewValue = newInfo.PatrolTimes.ToString() });

            if (this.PatrolHours != newInfo.PatrolHours)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2PatrolHours", OldValue = this.PatrolHours.ToString(), NewValue = newInfo.PatrolHours.ToString() });

            if (this.MaintenanceTimes != newInfo.MaintenanceTimes)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2MaintenanceTimes", OldValue = this.MaintenanceTimes.ToString(), NewValue = newInfo.MaintenanceTimes.ToString() });

            if (this.MaintenanceHours != newInfo.MaintenanceHours)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2MaintenanceHours", OldValue = this.MaintenanceHours.ToString(), NewValue = newInfo.MaintenanceHours.ToString() });

            if (this.RepairHours != newInfo.RepairHours)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2RepairHours", OldValue = this.RepairHours.ToString(), NewValue = newInfo.RepairHours.ToString() });

            if (this.IncludeContract != newInfo.IncludeContract)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2IncludeContract", OldValue = this.IncludeContract ? "是" : "否", NewValue = newInfo.IncludeContract ? "是" : "否" });

            if (this.FullCoveragePtg != newInfo.FullCoveragePtg)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2FullCoveragePtg", OldValue = this.FullCoveragePtg.ToString(), NewValue = newInfo.FullCoveragePtg.ToString() });

            if (this.TechCoveragePtg != newInfo.TechCoveragePtg)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2TechCoveragePtg", OldValue = this.TechCoveragePtg.ToString(), NewValue = newInfo.TechCoveragePtg.ToString() });

            if (this.IncludeSpare != newInfo.IncludeSpare)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2IncludeSpare", OldValue = this.IncludeSpare ? "是" : "否", NewValue = newInfo.IncludeSpare ? "是" : "否" });

            if (this.SparePrice != newInfo.SparePrice)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2SparePrice", OldValue = this.SparePrice.ToString(), NewValue = newInfo.SparePrice.ToString() });

            if (this.SpareRentPtg != newInfo.SpareRentPtg)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2SpareRentPtg", OldValue = this.SpareRentPtg.ToString(), NewValue = newInfo.SpareRentPtg.ToString() });

            if (this.IncludeRepair != newInfo.IncludeRepair)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2IncludeRepair", OldValue = this.IncludeRepair ? "是" : "否", NewValue = newInfo.IncludeRepair ? "是" : "否" });

            if (this.Usage != newInfo.Usage)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Usage", OldValue = this.Usage.ToString(), NewValue = newInfo.Usage.ToString() });

            if (this.EquipmentType.ID != newInfo.EquipmentType.ID)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2EquipmentType", OldValue = this.EquipmentType.Name, NewValue = LookupManager.GetEquipmentTypeDesc(newInfo.EquipmentType.ID) });

            if (this.RepairComponentCost != newInfo.RepairComponentCost)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2RepairComponentCost", OldValue = this.RepairComponentCost.ToString(), NewValue = newInfo.RepairComponentCost.ToString() });

            if (this.Repair3partyRatio != newInfo.Repair3partyRatio)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Repair3partyRatio", OldValue = this.Repair3partyRatio.ToString(), NewValue = newInfo.Repair3partyRatio.ToString() });

            if (this.Repair3partyCost != newInfo.Repair3partyCost)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Repair3partyCost", OldValue = this.Repair3partyCost.ToString(), NewValue = newInfo.Repair3partyCost.ToString() });

            if (this.RepairCostRatio != newInfo.RepairCostRatio)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2RepairCostRatio", OldValue = this.RepairCostRatio.ToString(), NewValue = newInfo.RepairCostRatio.ToString() });

            if (this.MethodID != newInfo.MethodID)
                infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2MethodID", OldValue = this.MethodID.GetDescription(), NewValue = newInfo.MethodID.GetDescription() });

            return audit;
        }
        /// <summary>
        /// 富士II类dt
        /// </summary>
        /// <param name="infos">富士II类信息</param>
        /// <returns>富士II类dt</returns>
        public static DataTable ConvertFujiClass2DataTable(List<FujiClass2Info> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("FujiClass1ID", typeof(int));
            dt.Columns.Add("Name");
            dt.Columns.Add("Description");
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IncludeLabour", typeof(bool));
            dt.Columns.Add("PatrolTimes", typeof(double));
            dt.Columns.Add("PatrolHours", typeof(double));
            dt.Columns.Add("MaintenanceTimes", typeof(double));
            dt.Columns.Add("MaintenanceHours", typeof(double));
            dt.Columns.Add("RepairHours", typeof(double));
            dt.Columns.Add("IncludeContract", typeof(bool));
            dt.Columns.Add("FullCoveragePtg", typeof(double));
            dt.Columns.Add("TechCoveragePtg", typeof(double));
            dt.Columns.Add("IncludeSpare", typeof(bool));
            dt.Columns.Add("SparePrice", typeof(double));
            dt.Columns.Add("SpareRentPtg", typeof(double));
            dt.Columns.Add("IncludeRepair", typeof(bool));
            dt.Columns.Add("Usage", typeof(int));
            dt.Columns.Add("EquipmentType", typeof(int));
            dt.Columns.Add("RepairComponentCost", typeof(double));
            dt.Columns.Add("Repair3partyRatio", typeof(double));
            dt.Columns.Add("Repair3partyCost", typeof(double));
            dt.Columns.Add("RepairCostRatio", typeof(double));
            dt.Columns.Add("MethodID", typeof(int));

            infos.ForEach(info => {
                DataRow dr = dt.NewRow();
                dr["ID"] = info.ID;
                dr["FujiClass1ID"] = info.FujiClass1.ID;
                dr["Name"] = info.Name;
                dr["Description"] = info.Description;
                dr["IncludeLabour"] = info.IncludeLabour;
                dr["PatrolTimes"] = info.PatrolTimes;
                dr["PatrolHours"] = info.PatrolHours;
                dr["MaintenanceTimes"] = info.MaintenanceTimes;
                dr["MaintenanceHours"] = info.MaintenanceHours;
                dr["RepairHours"] = info.RepairHours;
                dr["IncludeContract"] = info.IncludeContract;
                dr["FullCoveragePtg"] = info.FullCoveragePtg;
                dr["TechCoveragePtg"] = info.TechCoveragePtg;
                dr["IncludeSpare"] = info.IncludeSpare;
                dr["SparePrice"] = info.SparePrice;
                dr["SpareRentPtg"] = info.SpareRentPtg;
                dr["IncludeRepair"] = info.IncludeRepair;
                dr["Usage"] = info.Usage;
                dr["EquipmentType"] = info.EquipmentType.ID;
                dr["RepairComponentCost"] = info.RepairComponentCost;
                dr["Repair3partyRatio"] = info.Repair3partyRatio;
                dr["Repair3partyCost"] = info.Repair3partyCost;
                dr["RepairCostRatio"] = info.RepairCostRatio;
                dr["MethodID"] = info.MethodID;
                dt.Rows.Add(dr);
            });
            return dt;
        }

        public int CompareTo(FujiClass2Info other)
        {
            return this.ID.CompareTo(other.ID);
        }
        /// <summary>
        /// 设备类别
        /// </summary>
        public static class LKPEquipmentType{
            /// <summary>
            /// 重点设备
            /// </summary>
            public const int Import = 1;
            /// <summary>
            /// 一般设备
            /// </summary>
            public const int General = 3;
        }
        /// <summary>
        /// 故障率计算方式
        /// </summary>
        public enum Method
        {
            /// <summary>
            /// 未选
            /// </summary>
            [Description("")]
            Null,
            /// <summary>
            /// 手动
            /// </summary>
            [Description("手动")]
            Manual ,
            /// <summary>
            /// 韦伯
            /// </summary>
            [Description("韦伯")]
            WebbStatistics,
            /// <summary>
            /// 定期
            /// </summary>
            [Description("定期")]
            Regular
        }

        /// <summary>
        /// 费用类别
        /// </summary>
        public enum SectionType
        {
            /// <summary>
            /// 人工费
            /// </summary>
            [Description("人工费")]
            Labour = 1,
            /// <summary>
            /// 服务费
            /// </summary>
            [Description("服务费")]
            Contract,
            /// <summary>
            /// 备用机
            /// </summary>
            [Description("备用机")]
            Spare,
            /// <summary>
            /// 额外维保
            /// </summary>
            [Description("额外维保")]
            Repair,
            /// <summary>
            /// 零件
            /// </summary>
            [Description("零件")]
            Component,
            /// <summary>
            /// 耗材
            /// </summary>
            [Description("耗材")]
            Consumable
        }
    }
     
}
