using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 估价参数Info
    /// </summary>
    public class ValParameterInfo
    {
        /// <summary>
        /// 医院等级
        /// </summary>
        public HospitalLevelInfo HospitalLevel { get; set; }
        /// <summary>
        /// 医院等级1系统参数
        /// </summary>
        public double HospitalFactor1 { get; set; }
        /// <summary>
        /// 医院等级2系统参数
        /// </summary>
        public double HospitalFactor2 { get; set; }
        /// <summary>
        /// 医院等级3系统参数
        /// </summary>
        public double HospitalFactor3 { get; set; }
        /// <summary>
        /// 信息系统使用费
        /// </summary>
        public double SystemCost { get; set; }
        /// <summary>
        /// 每月工作时长
        /// </summary>
        public double MonthlyHours { get; set; }
        /// <summary>
        /// 单位人工成本
        /// </summary>
        public double UnitCost { get; set; }
        /// <summary>
        /// 小额成本耗材标准年费用
        /// </summary>
        public double SmallConsumableCost { get; set; }
        /// <summary>
        /// 估价参数info
        /// </summary>
        public ValParameterInfo()
        {
            this.HospitalLevel = new HospitalLevelInfo();
        }
        /// <summary>
        /// 估价参数info
        /// </summary>
        /// <param name="dr">数据</param>
        public ValParameterInfo(DataRow dr)
            : this()
        {
            this.SystemCost = SQLUtil.ConvertDouble(dr["SystemCost"]);
            this.MonthlyHours = SQLUtil.ConvertDouble(dr["MonthlyHours"]);
            this.UnitCost = SQLUtil.ConvertDouble(dr["UnitCost"]);
            this.SmallConsumableCost = SQLUtil.ConvertDouble(dr["SmallConsumableCost"]);
            this.HospitalLevel.ID = SQLUtil.ConvertInt(dr["HospitalLevel"]);
            this.HospitalLevel = LookupManager.GetHospitalLevel(this.HospitalLevel.ID);
            this.HospitalFactor1 = SQLUtil.ConvertDouble(dr["HospitalFactor1"]);
            this.HospitalFactor2 = SQLUtil.ConvertDouble(dr["HospitalFactor2"]);
            this.HospitalFactor3 = SQLUtil.ConvertDouble(dr["HospitalFactor3"]);
        }
        /// <summary>
        /// 获取预测工程师数量
        /// </summary>
        /// <param name="totalHours">总工时</param>
        /// <returns>工程师数量</returns>
        public int GetCalculatedEngineers(double totalHours)
        {
            return (int)Math.Ceiling(totalHours / this.MonthlyHours);
        }
    }

    /// <summary>
    /// 医院等级Info
    /// </summary>
    public class HospitalLevelInfo
    {
        /// <summary>
        /// 等级id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 等级描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 医院等级系统参数
        /// </summary>
        public double Factor { get; set; }
        /// <summary>
        /// 医院等级info
        /// </summary>
        public HospitalLevelInfo() { }
        /// <summary>
        /// 医院等级info
        /// </summary>
        /// <param name="dr">数据</param>
        public HospitalLevelInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.Factor = SQLUtil.ConvertDouble(dr["Factor"]);
        }
    }

    /// <summary>
    /// 估价前提条件Info
    /// </summary>
    public class ValControlInfo
    {
        /// <summary>
        /// flag
        /// </summary>
        public string CtlFlag { get; set; }
        /// <summary>
        /// 操作用户
        /// </summary>
        public UserInfo User { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 是否已执行
        /// </summary>
        public bool IsExecuted { get; set; }

        /// <summary>
        /// 运营实际截至月
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 合同开始月
        /// </summary>
        public DateTime ContractStartDate { get; set; }
        /// <summary>
        /// 预测年数
        /// </summary>
        public int Years { get; set; }
        /// <summary>
        /// 医院等级
        /// </summary>
        public HospitalLevelInfo HospitalLevel { get; set; }
        /// <summary>
        /// 导入期成本
        /// </summary>
        public double ImportCost { get; set; }
        /// <summary>
        /// 边际利润率
        /// </summary>
        public double ProfitMargins { get; set; }
        /// <summary>
        /// 风险控制度
        /// </summary>
        public double RiskRatio { get; set; }
        /// <summary>
        /// VaR资产金额比例
        /// </summary>
        public double VarAmount { get; set; }
        /// <summary>
        /// 预测工程师数量
        /// </summary>
        public int ComputeEngineer { get; set; }
        /// <summary>
        /// 预定工程师数量
        /// </summary>
        public int ForecastEngineer { get; set; }

        public double HospitalFactor1 { get; set; }
        public double HospitalFactor2 { get; set; }
        public double HospitalFactor3 { get; set; }
        /// <summary>
        /// 估价前提条件Info
        /// </summary>
        public ValControlInfo()
        {
            this.User = new UserInfo();
            this.HospitalLevel = new HospitalLevelInfo();
        }
        /// <summary>
        /// 估价前提条件Info
        /// </summary>
        /// <param name="dr">数据</param>
        public ValControlInfo(DataRow dr)
            : this()
        {
            this.CtlFlag = SQLUtil.TrimNull(dr["CtlFlag"]);
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            this.IsExecuted = SQLUtil.ConvertBoolean(dr["IsExecuted"]);

            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            this.ContractStartDate = SQLUtil.ConvertDateTime(dr["ContractStartDate"]);
            this.Years = SQLUtil.ConvertInt(dr["Years"]);
            this.HospitalLevel.ID = SQLUtil.ConvertInt(dr["HospitalLevel"]);
            this.ImportCost = SQLUtil.ConvertDouble(dr["ImportCost"]);
            this.ProfitMargins = SQLUtil.ConvertDouble(dr["ProfitMargins"]);
            this.RiskRatio = SQLUtil.ConvertDouble(dr["RiskRatio"]);
            this.VarAmount = SQLUtil.ConvertDouble(dr["VarAmount"]);
            this.ComputeEngineer = SQLUtil.ConvertInt(dr["ComputeEngineer"]);
            this.ForecastEngineer = SQLUtil.ConvertInt(dr["ForecastEngineer"]);
            this.HospitalFactor1 = SQLUtil.ConvertDouble(dr["HospitalFactor1"]);
            this.HospitalFactor2 = SQLUtil.ConvertDouble(dr["HospitalFactor2"]);
            this.HospitalFactor3 = SQLUtil.ConvertDouble(dr["HospitalFactor3"]);
        }


        public static class ForecastYears
        {
            public const int ForecastYear = 5;

            public static List<KeyValueInfo> GetForecastYearsList()
            {
                List<KeyValueInfo> ForecastList = new List<KeyValueInfo>();
                for (int i = 1; i <= ForecastYears.ForecastYear; i++)
                {
                    ForecastList.Add(new KeyValueInfo() { ID = i, Name = SQLUtil.TrimNull(i) });
                }

                return ForecastList;
            }
        }

        public static class ActualYears
        {
            public const int ActualYear = 1;

            public static List<KeyValueInfo> GetActualYearsList()
            {
                List<KeyValueInfo> ActualList = new List<KeyValueInfo>();
                for (int i = 1; i <= ActualYears.ActualYear; i++)
                {
                    ActualList.Add(new KeyValueInfo() { ID = i, Name = SQLUtil.TrimNull(i) });
                }

                return ActualList;
            }
        }
    }

    /// <summary>
    /// 估价执行条件：设备清单Info
    /// </summary>
    public class ValEquipmentInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 是否在系统中
        /// </summary>
        public bool InSystem { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 当前维保情况
        /// </summary>
        public KeyValueInfo CurrentScope { get; set; }
        /// <summary>
        /// 下次维保情况
        /// </summary>
        public KeyValueInfo NextScope { get; set; }
        /// <summary>
        /// 维保结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 所需巡检小时数
        /// </summary>
        public Double PatrolHours { get; set; }
        /// <summary>
        /// 所需保养工时
        /// </summary>
        public Double MaintenanceHours { get; set; }
        /// <summary>
        /// 所需维修工时
        /// </summary>
        public Double RepairHours { get; set; }
        /// <summary>
        /// Session标记
        /// </summary>
        public string SessionKey
        {
            get { return string.Format("{0}_{1}_{2}_{3}", ValuationType.Equipment.GetDescription(), this.Equipment.ID, this.InSystem, this.User.ID); }
        }
        /// <summary>
        /// 估价执行条件：设备清单Info
        /// </summary>
        public ValEquipmentInfo()
        {
            this.User = new UserInfo();
            this.Equipment = new EquipmentInfo();
            this.CurrentScope = new KeyValueInfo();
            this.NextScope = new KeyValueInfo();
        }
        /// <summary>
        /// 估价执行条件：设备清单Info
        /// </summary>
        /// <param name="dr"></param>
        public ValEquipmentInfo(DataRow dr)
            : this()
        {
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            this.Equipment.AssetCode = SQLUtil.TrimNull(dr["AssetCode"]);
            this.Equipment.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Equipment.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
            this.Equipment.Manufacturer.Name = SQLUtil.TrimNull(dr["Manufacturer"]);
            this.Equipment.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            this.Equipment.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Equipment.FujiClass2.FujiClass1.ID = SQLUtil.ConvertInt(dr["FujiClass1ID"]);
            this.Equipment.FujiClass2.FujiClass1.Name = SQLUtil.TrimNull(dr["FujiClass1Name"]);
            if (dr.Table.Columns.Contains("EquipmentTypeID"))
            {
                this.Equipment.FujiClass2.EquipmentType.ID = SQLUtil.ConvertInt(dr["EquipmentTypeID"]);
                this.Equipment.FujiClass2.EquipmentType.Name = LookupManager.GetEquipmentTypeDesc(this.Equipment.FujiClass2.EquipmentType.ID);
            }
            if (dr.Table.Columns.Contains("FullCoveragePtg"))
                this.Equipment.FujiClass2.FullCoveragePtg = SQLUtil.ConvertDouble(dr["FullCoveragePtg"]);
            if (dr.Table.Columns.Contains("TechCoveragePtg"))
                this.Equipment.FujiClass2.TechCoveragePtg = SQLUtil.ConvertDouble(dr["TechCoveragePtg"]);

            this.Equipment.Department.Name = SQLUtil.TrimNull(dr["Department"]);
            this.Equipment.PurchaseAmount = SQLUtil.ConvertDouble(dr["PurchaseAmount"]);
            this.Equipment.UseageDate = SQLUtil.ConvertDateTime(dr["UseageDate"]);
            this.CurrentScope.ID = SQLUtil.ConvertInt(dr["CurrentScopeID"]);
            this.CurrentScope.Name = ScopeTypes.GetScopeDesc(this.CurrentScope.ID);
            this.NextScope.ID = SQLUtil.ConvertInt(dr["NextScopeID"]);
            this.NextScope.Name = ScopeTypes.GetScopeDesc(this.NextScope.ID);
            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            this.InSystem = SQLUtil.ConvertBoolean(dr["InSystem"]);
            this.PatrolHours = SQLUtil.ConvertDouble(dr["PatrolHours"]);
            this.MaintenanceHours = SQLUtil.ConvertDouble(dr["MaintenanceHours"]);
            this.RepairHours = SQLUtil.ConvertDouble(dr["RepairHours"]);
        }
        /// <summary>
        /// 维保类型
        /// </summary>
        public static class ScopeTypes
        {
            /// <summary>
            /// 全保
            /// </summary>
            public const int FullCoverage = 1;
            /// <summary>
            /// 技术保
            /// </summary>
            public const int TechCoverage = 2;
            /// <summary>
            /// 不保
            /// </summary>
            public const int NoneCoverage = 3;
            /// <summary>
            /// 获取维保类型列表
            /// </summary>
            /// <returns>维保类型列表</returns>
            public static List<KeyValueInfo> GetScopeTypes()
            {
                List<KeyValueInfo> types = new List<KeyValueInfo>();
                types.Add(new KeyValueInfo() { ID = FullCoverage, Name = GetScopeDesc(FullCoverage) });
                types.Add(new KeyValueInfo() { ID = TechCoverage, Name = GetScopeDesc(TechCoverage) });
                types.Add(new KeyValueInfo() { ID = NoneCoverage, Name = GetScopeDesc(NoneCoverage) });

                return types;
            }
            /// <summary>
            /// 根据维保类型id获取描述
            /// </summary>
            /// <param name="typeID">维保类型id</param>
            /// <returns>维保类型描述</returns>
            public static string GetScopeDesc(int typeID)
            {
                switch (typeID)
                {
                    case FullCoverage:
                        return "全保";
                    case TechCoverage:
                        return "技术保";
                    case NoneCoverage:
                        return "不保";
                    default:
                        return "";
                }
            }
            /// <summary>
            /// 根据维保类型描述获取维保类型id
            /// </summary>
            /// <param name="scopeDesc">维保类型描述</param>
            /// <returns>维保类型id</returns>
            public static int GetScopeID(string scopeDesc)
            {
                switch (scopeDesc)
                {
                    case "全保":
                        return FullCoverage;
                    case "技术保":
                        return TechCoverage;
                    case "不保":
                        return NoneCoverage;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// 获取估价设备清单信息DataTable
        /// </summary>
        /// <param name="infos">设备信息</param>
        /// <returns>设备清单DataTable</returns>
        public static DataTable ConvertValEquipmentTable(List<ValEquipmentInfo> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("AssetCode", typeof(System.String));
            dt.Columns.Add("Name", typeof(System.String));
            dt.Columns.Add("SerialCode", typeof(System.String));
            dt.Columns.Add("Manufacturer", typeof(System.String));
            dt.Columns.Add("Department", typeof(System.String));
            dt.Columns.Add("FujiClass2ID", typeof(System.Int32));
            dt.Columns.Add("PurchaseAmount", typeof(System.Double));
            dt.Columns.Add("CurrentScopeID", typeof(System.Int32));
            dt.Columns.Add("NextScopeID", typeof(System.Int32));
            dt.Columns.Add("EndDate", typeof(System.DateTime));

            infos.ForEach(info =>
            {
                DataRow dr = dt.NewRow();
                dr["UserID"] = info.User.ID;
                dr["Insystem"] = info.InSystem;
                dr["EquipmentID"] = info.Equipment.ID;
                dr["AssetCode"] = info.Equipment.AssetCode;
                dr["Name"] = info.Equipment.Name;
                dr["SerialCode"] = info.Equipment.SerialCode;
                dr["Manufacturer"] = info.Equipment.Manufacturer.Name;
                dr["Department"] = info.Equipment.Department.Name;
                dr["FujiClass2ID"] = info.Equipment.FujiClass2.ID;
                dr["PurchaseAmount"] = info.Equipment.PurchaseAmount;
                dr["CurrentScopeID"] = info.CurrentScope.ID;
                dr["NextScopeID"] = info.NextScope.ID;
                dr["EndDate"] = SQLUtil.MinDateToNull(info.EndDate);            

                dt.Rows.Add(dr);
            });
            return dt;
        }
    }

    /// <summary>
    /// 估价执行条件:维保对耗材的覆盖情况Info
    /// </summary>
    public class ValConsumableInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 耗材信息
        /// </summary>
        public ConsumableInfo Consumable { get; set; }
        /// <summary>
        /// 是否维保
        /// </summary>
        public bool IncludeContract { get; set; }
        /// <summary>
        /// session
        /// </summary>
        public string SessionKey
        {
            get{return string.Format("{0}_{1}_{2}", ValuationType.Consumable.GetDescription(), this.Consumable.ID, this.User.ID);}
        }
        /// <summary>
        /// 估价执行条件:维保对耗材的覆盖情况Info
        /// </summary>
        public ValConsumableInfo()
        {
            this.User = new UserInfo();
            this.Consumable = new ConsumableInfo();
        }

        /// <summary>
        /// 估价执行条件:维保对耗材的覆盖情况Info
        /// </summary>
        /// <param name="dr">数据</param>
        public ValConsumableInfo(DataRow dr)
            : this()
        {
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            this.Consumable.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            this.Consumable.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Consumable.ID = SQLUtil.ConvertInt(dr["ConsumableID"]);
            this.Consumable.Name = SQLUtil.TrimNull(dr["ConsumableName"]);
            this.Consumable.CostPer = SQLUtil.ConvertDouble(dr["CostPer"]);
            this.Consumable.ReplaceTimes = SQLUtil.ConvertDouble(dr["ReplaceTimes"]);
            this.IncludeContract = SQLUtil.ConvertBoolean(dr["IncludeContract"]);
        }

        /// <summary>
        /// 获取估价耗材清单信息DataTable
        /// </summary>
        /// <param name="infos">耗材信息</param>
        /// <returns>耗材清单信息DataTable</returns>
        public static DataTable ConvertValConsumableTable(List<ValConsumableInfo> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("ConsumableID", typeof(System.Int32));
            dt.Columns.Add("IncludeContract", typeof(System.Boolean));

            infos.ForEach(info =>
            {
                DataRow dr = dt.NewRow();
                dr["UserID"] = info.User.ID;
                dr["ConsumableID"] = info.Consumable.ID;
                dr["IncludeContract"] = info.IncludeContract;

                dt.Rows.Add(dr);
            });
            return dt;
        }
    }

    /// <summary>
    /// 估价执行条件：备用机清单Info
    /// </summary>
    public class ValSpareInfo 
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 富士II类信息
        /// </summary>
        public FujiClass2Info FujiClass2 { get; set; }
        /// <summary>
        /// 备用金价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 月租输入数量
        /// </summary>
        public int QtyEnter { get; set; }
        /// <summary>
        /// 月租评估数量
        /// </summary>
        public int QtyEval { get; set; }
        /// <summary>
        /// 维修期间
        /// </summary>
        public int AdjustRepairDays { get; set; }
        /// <summary>
        /// 计算备用机台数
        /// </summary>
        public int CalculatedCount { get; set; }

        /// <summary>
        /// Session标记
        /// </summary>
        public string SessionKey
        {
            get{return string.Format("{0}_{1}_{2}", ValuationType.Spare.GetDescription(), this.FujiClass2.ID, this.User.ID);}
        }
        /// <summary>
        /// 估价执行条件：备用机清单Info
        /// </summary>
        public ValSpareInfo()
        {
            this.User = new UserInfo();
            this.FujiClass2 = new FujiClass2Info();
        }
        /// <summary>
        /// 估价执行条件：备用机清单Info
        /// </summary>
        /// <param name="dr">数据</param>
        public ValSpareInfo(DataRow dr)
            : this()
        {
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Price = SQLUtil.ConvertDouble(dr["Price"]);
            this.QtyEnter = SQLUtil.ConvertInt(dr["QtyEnter"]);
            this.QtyEval = SQLUtil.ConvertInt(dr["QtyEval"]);
        }

        /// <summary>
        /// 获取估价备用机信息DataTable
        /// </summary>
        /// <param name="infos">备用机信息</param>
        /// <returns>备用机信息DataTable</returns>
        public static DataTable ConvertValSpareTable(List<ValSpareInfo> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("FujiClass2ID", typeof(System.Int32));
            dt.Columns.Add("Price", typeof(System.Double));
            dt.Columns.Add("QtyEnter", typeof(System.Int32));
            dt.Columns.Add("QtyEval", typeof(System.Int32));
            dt.Columns.Add("CalculatedCount", typeof(System.Int32));

            infos.ForEach(info =>
            {
                DataRow dr = dt.NewRow();
                dr["UserID"] = info.User.ID;
                dr["FujiClass2ID"] = info.FujiClass2.ID;
                dr["Price"] = info.Price;
                dr["QtyEnter"] = info.QtyEnter;
                dr["QtyEval"] = info.QtyEval;
                dr["CalculatedCount"] = info.CalculatedCount;

                dt.Rows.Add(dr);
            });
            return dt;
        }
    }

    /// <summary>
    /// 估价执行：零件清单Info
    /// </summary>
    public class ValComponentInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 零件信息
        /// </summary>
        public ComponentInfo Component { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 使用量
        /// </summary>
        public int Usage { get; set; }
        /// <summary>
        /// CT已使用秒次
        /// </summary>
        public double Seconds { get; set; }
        /// <summary>
        /// 以后是否维保
        /// </summary>
        public bool IncludeContract { get; set; }
        /// <summary>
        /// 设备类别
        /// </summary>
        public KeyValueInfo EquipmentType { get; set; }
        /// <summary>
        /// 是否在系统
        /// </summary>
        public Boolean InSystem { get; set; }
        /// <summary>
        /// 已使用秒次参考值
        /// </summary>
        public double UsedSeconds { get; set; }

        public int UsageRefere { get; set; }
        /// <summary>
        /// Session标记
        /// </summary>
        public string SessionKey
        {
            get{return string.Format("{0}_{1}_{2}_{3}_{4}", ValuationType.Component.GetDescription(), this.Equipment.ID, this.Component.ID, this.InSystem, this.User.ID);}
        }
        /// <summary>
        /// 零件id
        /// </summary>
        public int ComponentID { get { return this.Component.ID; } }

        public DateTime UseageDate { get; set; }
        /// <summary>
        /// 估价执行：零件清单Info
        /// </summary>
        public ValComponentInfo()
        {
            this.User = new UserInfo();
            this.Equipment = new EquipmentInfo();
            this.Component = new ComponentInfo();
            this.EquipmentType = new KeyValueInfo();
        }
        /// <summary>
        /// 估价执行：零件清单Info
        /// </summary>
        /// <param name="dr">数据</param>
        public ValComponentInfo(DataRow dr)
            : this()
        {
            this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            this.InSystem = SQLUtil.ConvertBoolean(dr["InSystem"]);
            this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            this.Equipment.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Equipment.AssetCode = SQLUtil.TrimNull(dr["AssetCode"]);
            this.EquipmentType.ID = SQLUtil.ConvertInt(dr["EquipmentType"]);
            this.EquipmentType.Name = LookupManager.GetEquipmentTypeDesc(this.EquipmentType.ID);
            this.Equipment.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            this.Component.ID = SQLUtil.ConvertInt(dr["ComponentID"]);
            this.Component.Name = this.Component.ID == 0 ? ComponentType.WholeMachine : SQLUtil.TrimNull(dr["ComponentName"]);
            this.Component.Type.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.Component.StdPrice = SQLUtil.ConvertDouble(dr["StdPrice"]);
            this.Component.SecondsPer = SQLUtil.ConvertDouble(dr["SecondsPer"]);
            this.Component.TotalSeconds = SQLUtil.ConvertInt(dr["TotalSeconds"]);
            this.Component.Usage = SQLUtil.ConvertInt(dr["ComponentUsage"]);
            this.Qty = SQLUtil.ConvertInt(dr["Qty"]);
            this.Usage = SQLUtil.ConvertInt(dr["Usage"]);
            this.Seconds = SQLUtil.ConvertDouble(dr["Seconds"]);
            this.UsedSeconds = SQLUtil.ConvertDouble(dr["UsedSeconds"]);
            this.IncludeContract = SQLUtil.ConvertBoolean(dr["IncludeContract"]);
            this.UsageRefere = SQLUtil.ConvertInt(dr["UsageRefere"]);
            if (dr.Table.Columns.Contains("UseageDate"))
                this.UseageDate = SQLUtil.ConvertDateTime(dr["UseageDate"]);
        }
        /// <summary>
        /// 获取估价零件信息DataTable
        /// </summary>
        /// <param name="infos">零件信息</param>
        /// <returns>零件信息DataTable</returns>
        public static DataTable ConvertValComponentTable(List<ValComponentInfo> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("ComponentID", typeof(System.Int32));
            dt.Columns.Add("Qty", typeof(System.Int32));
            dt.Columns.Add("Usage", typeof(System.Int32));
            dt.Columns.Add("Seconds", typeof(System.Double));
            dt.Columns.Add("IncludeContract", typeof(System.Boolean));

            infos.ForEach(info =>
            {
                DataRow dr = dt.NewRow();
                dr["UserID"] = info.User.ID;
                dr["Insystem"] = info.InSystem;
                dr["EquipmentID"] = info.Equipment.ID;
                dr["ComponentID"] = info.Component.ID;
                dr["Qty"] = info.Qty;
                dr["Usage"] = info.Usage;
                dr["Seconds"] = info.Seconds;
                dr["IncludeContract"] = info.IncludeContract;

                dt.Rows.Add(dr);
            });
            return dt;
        }
        /// <summary>
        /// 零件类型
        /// </summary>
        public static class ComponentType
        {
            /// <summary>
            /// 整机
            /// </summary>
            public const string WholeMachine = "整机";
        }
    }

    /// <summary>
    /// 清单类型
    /// </summary>
    public enum ValuationType
    {
        /// <summary>
        /// 设备清单
        /// </summary>
        [Description("Equipment")]
        Equipment,
        /// <summary>
        /// 备用机清单
        /// </summary>
        [Description("Spare")]
        Spare,
        /// <summary>
        /// 维保对耗材的覆盖
        /// </summary>
        [Description("Consumable")]
        Consumable,
        /// <summary>
        /// 零件清单
        /// </summary>
        [Description("Component")]
        Component,
        /// <summary>
        /// CT清单
        /// </summary>
        [Description("Component")]//correct
        CTTube,
    }

    /// <summary>
    /// 设备成本info
    /// </summary>
    public class ValEqptOutputInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 维保金额
        /// </summary>
        public double ContractAmount { get; set; }
        /// <summary>
        /// 外来服务费金额
        /// </summary>
        public double Repair3partyCost { get; set; }
        /// <summary>
        /// 设备成本info
        /// </summary>
        public ValEqptOutputInfo()
        {
            this.User = new UserInfo();
            this.Equipment = new EquipmentInfo();
        }
        /// <summary>
        /// 设备成本info
        /// </summary>
        /// <param name="dr">dr</param>
        public ValEqptOutputInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("UserID"))
                this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            if (dr.Table.Columns.Contains("UserName"))
                this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            if (dr.Table.Columns.Contains("EquipmentID"))
                this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            this.Year = SQLUtil.ConvertInt(dr["Year"]);
            this.Month = SQLUtil.ConvertInt(dr["Month"]);
            if (dr.Table.Columns.Contains("ContractAmount"))
                this.ContractAmount = SQLUtil.ConvertDouble(dr["ContractAmount"]);
            if (dr.Table.Columns.Contains("Repair3partyCost"))
                this.Repair3partyCost = SQLUtil.ConvertDouble(dr["Repair3partyCost"]);
        }
    }

    /// <summary>
    /// 耗材成本info
    /// </summary>
    public class ValConsumableOutputInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 耗材
        /// </summary>
        public ConsumableInfo Consumable { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 成本
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 耗材成本info
        /// </summary>
        public ValConsumableOutputInfo()
        {
            this.User = new UserInfo();
            this.Equipment = new EquipmentInfo();
            this.Consumable = new ConsumableInfo();
        }
        /// <summary>
        /// 耗材成本info
        /// </summary>
        /// <param name="dr">dr</param>
        public ValConsumableOutputInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("UserID"))
                this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            if (dr.Table.Columns.Contains("UserName"))
                this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            if (dr.Table.Columns.Contains("EquipmentID"))
                this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            if (dr.Table.Columns.Contains("ConsumableID"))
                this.Consumable.ID = SQLUtil.ConvertInt(dr["ConsumableID"]);
            this.Year = SQLUtil.ConvertInt(dr["Year"]);
            this.Month = SQLUtil.ConvertInt(dr["Month"]);
            this.Amount = SQLUtil.ConvertDouble(dr["Amount"]);
        }
    }
    /// <summary>
    /// 零件成本info
    /// </summary>
    public class ValComponentOutputInfo
    {
        public UserInfo User { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 零件
        /// </summary>
        public ComponentInfo Component { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// 零件成本
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 零件成本info
        /// </summary>
        public ValComponentOutputInfo()
        {
            this.User = new UserInfo();
            this.Equipment = new EquipmentInfo();
            this.Component = new ComponentInfo();
        }
        /// <summary>
        /// 零件成本info
        /// </summary>
        /// <param name="dr">dr</param>
        public ValComponentOutputInfo(DataRow dr)
            : this()
        {
            if (dr.Table.Columns.Contains("UserID"))
                this.User.ID = SQLUtil.ConvertInt(dr["UserID"]);
            if (dr.Table.Columns.Contains("UserName"))
                this.User.Name = SQLUtil.TrimNull(dr["UserName"]);
            if (dr.Table.Columns.Contains("EquipmentID"))
                this.Equipment.ID = SQLUtil.ConvertInt(dr["EquipmentID"]);
            if (dr.Table.Columns.Contains("ComponentID"))
                this.Component.ID = SQLUtil.ConvertInt(dr["ComponentID"]);
            this.Year = SQLUtil.ConvertInt(dr["Year"]);
            this.Month = SQLUtil.ConvertInt(dr["Month"]);
            this.Amount = SQLUtil.ConvertDouble(dr["Amount"]);
        }

    }

    public enum VaRType
    {
        [Display(Name ="导入期")]
        [Description("导入期")]
        LeadinPeriod ,
        [Display(Name = "稳定期")]
        [Description("稳定期")]
        StablePeriod
    }

    
}
