using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class EquipmentInfo : EntityInfo
    {
        /// <summary>
        /// 1,2,3类医疗器械
        /// </summary>
        /// <value>
        /// The equipment level.
        /// </value>
        public KeyValueInfo EquipmentLevel { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        /// <value>
        /// The equipment code.
        /// </value>
        public string EquipmentCode { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        /// <value>
        /// The serial code.
        /// </value>
        public string SerialCode { get; set; }
        /// <summary>
        /// 设备厂商
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public SupplierInfo Manufacturer { get; set; }
        /// <summary>
        /// 设备分类一级
        /// </summary>
        /// <value>
        /// The equipment class1.
        /// </value>
        public EquipmentClassInfo EquipmentClass1 { get; set; }
        /// <summary>
        /// 设备分类二级
        /// </summary>
        /// <value>
        /// The equipment class2.
        /// </value>
        public EquipmentClassInfo EquipmentClass2 { get; set; }
        /// <summary>
        /// 设备分类三级
        /// </summary>
        /// <value>
        /// The equipment class3.
        /// </value>
        public EquipmentClassInfo EquipmentClass3 { get; set; }
        /// <summary>
        /// 标准响应时间时长
        /// </summary>
        /// <value>
        /// The length of the response time.
        /// </value>
        public int ResponseTimeLength { get; set; }
        /// <summary>
        /// 整包范围
        /// </summary>
        /// <value>
        ///   <c>true</c> if [service scope]; otherwise, <c>false</c>.
        /// </value>
        public bool ServiceScope { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        public DateTime ManufacturingDate { get; set; }
        /// <summary>
        /// 固定资产
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed asset]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedAsset { get; set; }
        /// <summary>
        /// 医院系统资产编号
        /// </summary>
        /// <value>
        /// The asset code.
        /// </value>
        public string AssetCode { get; set; }
        /// <summary>
        /// 资产等级
        /// </summary>
        /// <value>
        /// The asset level.
        /// </value>
        public KeyValueInfo AssetLevel { get; set; }
        /// <summary>
        /// 折旧年限
        /// </summary>
        /// <value>
        /// The depreciation years.
        /// </value>
        public int DepreciationYears { get; set; }
        /// <summary>
        /// 注册证有效开始日期
        /// </summary>
        /// <value>
        /// The validity start date.
        /// </value>
        public DateTime ValidityStartDate { get; set; }
        /// <summary>
        /// 注册证有效结束日期
        /// </summary>
        /// <value>
        /// The validity end date.
        /// </value>
        public DateTime ValidityEndDate { get; set; }
        /// <summary>
        /// 销售合同名称
        /// </summary>
        /// <value>
        /// The name of the sale contract.
        /// </value>
        public string SaleContractName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        /// <value>
        /// The supplier.
        /// </value>
        public SupplierInfo Supplier { get; set; }
        /// <summary>
        /// 采购方式
        /// </summary>
        /// <value>
        /// The purchase way.
        /// </value>
        public string PurchaseWay { get; set; }
        /// <summary>
        /// 采购金额
        /// </summary>
        /// <value>
        /// The purchase amount.
        /// </value>
        public double PurchaseAmount { get; set; }
        /// <summary>
        ///采购日期
        /// </summary>
        /// <value>
        /// The purchase date.
        /// </value>
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// 设备产地
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is import; otherwise, <c>false</c>.
        /// </value>
        public bool IsImport { get; set; }
        /// <summary>
        /// 设备科室
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public KeyValueInfo Department { get; set; }
        /// <summary>
        /// 安装地点
        /// </summary>
        /// <value>
        /// The instal site.
        /// </value>
        public string InstalSite { get; set; }
        /// <summary>
        /// 安装日期
        /// </summary>
        /// <value>
        /// The instal date.
        /// </value>
        public DateTime InstalDate { get; set; }
        /// <summary>
        /// 启用日期
        /// </summary>
        public DateTime UseageDate { get; set; }
        /// <summary>
        /// 验收状态
        /// </summary>
        /// <value>
        ///   <c>true</c> if accepted; otherwise, <c>false</c>.
        /// </value>
        public bool Accepted { get; set; }
        /// <summary>
        /// 验收日期
        /// </summary>
        /// <value>
        /// The acceptance date.
        /// </value>
        public DateTime AcceptanceDate { get; set; }
        /// <summary>
        /// 使用状态
        /// </summary>
        /// <value>
        /// The usage status.
        /// </value>
        public KeyValueInfo UsageStatus { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        /// <value>
        /// The equipment status.
        /// </value>
        public KeyValueInfo EquipmentStatus { get; set; }
        /// <summary>
        /// 报废时间
        /// </summary>
        /// <value>
        /// The scrap date.
        /// </value>
        public DateTime ScrapDate { get; set; }
        /// <summary>
        /// 保养周期
        /// </summary>
        /// <value>
        /// The maintenance period.
        /// </value>
        public int MaintenancePeriod { get; set; }
        /// <summary>
        /// 保养周期类型
        /// </summary>
        /// <value>
        /// The type of the maintenance.
        /// </value>
        public KeyValueInfo MaintenanceType { get; set; }
        /// <summary>
        /// 上次保养日期
        /// </summary>
        /// <value>
        /// The last maintenance date.
        /// </value>
        public DateTime LastMaintenanceDate { get; set; }
        /// <summary>
        /// 巡检周期
        /// </summary>
        /// <value>
        /// The patrol period.
        /// </value>
        public int PatrolPeriod { get; set; }
        /// <summary>
        /// 巡检周期类型
        /// </summary>
        /// <value>
        /// The type of the patrol.
        /// </value>
        public KeyValueInfo PatrolType { get; set; }
        /// <summary>
        /// 上次巡检日期
        /// </summary>
        /// <value>
        /// The last patrol date.
        /// </value>
        public DateTime LastPatrolDate { get; set; }
        /// <summary>
        /// 校准周期
        /// </summary>
        /// <value>
        /// The correction period.
        /// </value>
        public int CorrectionPeriod { get; set; }
        /// <summary>
        /// 校准周期类型
        /// </summary>
        /// <value>
        /// The type of the correction.
        /// </value>
        public KeyValueInfo CorrectionType { get; set; }
        /// <summary>
        /// 校准周期
        /// </summary>
        /// <value>
        /// The last correction date.
        /// </value>
        public DateTime LastCorrectionDate { get; set; }
        /// <summary>
        /// 强检标记
        /// </summary>
        /// <value>
        /// The mandatory test status.
        /// </value>
        public KeyValueInfo MandatoryTestStatus { get; set; }
        /// <summary>
        /// 强检时间
        /// </summary>
        /// <value>
        /// The mandatory test date.
        /// </value>
        public DateTime MandatoryTestDate { get; set; }
        /// <summary>
        /// 召回标记
        /// </summary>
        /// <value>
        /// </value>
        public bool RecallFlag { get; set; }
        /// <summary>
        /// 召回时间
        /// </summary>
        /// <value>
        /// The recall date.
        /// </value>
        public DateTime RecallDate { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <value>
        /// The create user.
        /// </value>
        public UserInfo CreateUser { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        /// <value>
        /// The update date.
        /// </value>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        public double Incomes { get; set; }
        /// <summary>
        /// 上次收入
        /// </summary>
        /// <value>
        /// The last incomes.
        /// </value>
        public double LastIncomes { get; set; }
        /// <summary>
        /// 花费
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        public double Expenses { get; set; }
        /// <summary>
        /// 上次花费
        /// </summary>
        /// <value>
        /// The last expenses.
        /// </value>
        public double LastExpenses { get; set; }
        /// <summary>
        /// 合同来源
        /// </summary>
        /// <value>
        /// The contract scope.
        /// </value>
        public KeyValueInfo ContractScope { get; set; }
        /// <summary>
        /// 维保状态
        /// </summary>
        /// <value>
        /// The warranty status.
        /// </value>
        public string WarrantyStatus { get; set; }
        /// <summary>
        /// 设备附件
        /// </summary>
        /// <value>
        /// The equipment file.
        /// </value>
        public List<UploadFileInfo> EquipmentFile { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Equipment, this.ID); } }
        /// <summary>
        /// 产地
        /// </summary>
        /// <value>
        /// The type of the origin.
        /// </value>
        public string OriginType { get { return GetOriginType(this.IsImport); } }
        /// <summary>
        /// 产地
        /// </summary>
        /// <param name="isImport">if set to <c>true</c> [is import].</param>
        /// <returns>是否进口</returns>
        public static string GetOriginType(bool isImport)
        {
            return isImport ? "进口" : "国产";
        }
        /// <summary>
        /// 分类编码
        /// </summary>
        public string ClassCode { get { return this.EquipmentClass1.Code + this.EquipmentClass2.Code + this.EquipmentClass3.Code; } }

        /// <summary>
        /// 派工单信息
        /// </summary>
        /// <value>
        /// The dispatches.
        /// </value>
        public List<DispatchInfo> Dispatches { get; set; }
        /// <summary>
        /// 富士II类信息
        /// </summary>
        public FujiClass2Info FujiClass2 { get; set; }
        /// <summary>
        /// CT序列号
        /// </summary>
        public string CTSerialCode { get; set; }
        /// <summary>
        /// CT已使用秒数
        /// </summary>
        public double CTUsedSeconds { get; set; }

        public List<EqptComponentInfo> HisComponentList { get; set; }
        public List<EqptConsumableInfo> HisConsumableList { get; set; }

        //4Dashboard display ConfigLicence
        /// <summary>
        /// 文件id
        /// </summary>
        public int ConfigLicenceID { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string ConfigLicenceName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string url { get { return ConfigLicenceID == 0 ? "" : File.Exists(Path.Combine(Constants.EquipmentFolder, string.Format("{0}_{1}{2}", this.ID, this.ConfigLicenceID, new FileInfo(this.ConfigLicenceName).Extension))) ? string.Format("{0}_{1}{2}", this.ID, this.ConfigLicenceID, new FileInfo(this.ConfigLicenceName).Extension) : ""; } }

        /// <summary>
        /// 设备信息
        /// </summary>
        public EquipmentInfo()
        {
            this.EquipmentLevel = new KeyValueInfo();
            this.Manufacturer = new SupplierInfo();
            this.EquipmentClass1 = new EquipmentClassInfo();
            this.EquipmentClass2 = new EquipmentClassInfo();
            this.EquipmentClass3 = new EquipmentClassInfo();
            this.AssetLevel = new KeyValueInfo();
            this.Supplier = new SupplierInfo();
            this.Department = new KeyValueInfo();
            this.UsageStatus = new KeyValueInfo();
            this.EquipmentStatus = new KeyValueInfo();
            this.MaintenanceType = new KeyValueInfo();
            this.PatrolType = new KeyValueInfo();
            this.CorrectionType = new KeyValueInfo();
            this.MandatoryTestStatus = new KeyValueInfo();
            this.ContractScope = new KeyValueInfo();
            this.CreateUser = new UserInfo();
            this.FujiClass2 = new FujiClass2Info();

            this.HisComponentList = new List<EqptComponentInfo>();
            this.HisConsumableList = new List<EqptConsumableInfo>();
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        /// <param name="dr">dataRow</param>
        public EquipmentInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.EquipmentLevel.ID = SQLUtil.ConvertInt(dr["EquipmentLevel"]);
            this.EquipmentLevel.Name = EquipmentLevels.GetEquipmentLevelDesc(this.EquipmentLevel.ID);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.EquipmentCode = SQLUtil.TrimNull(dr["EquipmentCode"]);
            this.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
            this.Manufacturer.ID = SQLUtil.ConvertInt(dr["ManufacturerID"]);
            if (dr.Table.Columns.Contains("ManufacturerName"))
                this.Manufacturer.Name = SQLUtil.TrimNull(dr["ManufacturerName"]);
            this.EquipmentClass1.Code = SQLUtil.TrimNull(dr["EquipmentClass1"]);
            this.EquipmentClass1.Description = Manager.LookupManager.GetEquipmentClassDesc(this.EquipmentClass1.Code, 1);
            this.EquipmentClass2.Code = SQLUtil.TrimNull(dr["EquipmentClass2"]);
            this.EquipmentClass2.Description = Manager.LookupManager.GetEquipmentClassDesc(this.EquipmentClass2.Code, 2, this.EquipmentClass1.Code);
            this.EquipmentClass3.Code = SQLUtil.TrimNull(dr["EquipmentClass3"]);
            this.EquipmentClass3.Description = Manager.LookupManager.GetEquipmentClassDesc(this.EquipmentClass3.Code, 3, this.EquipmentClass1.Code + this.EquipmentClass2.Code);
            this.ResponseTimeLength = SQLUtil.ConvertInt(dr["ResponseTimeLength"]);
            this.ServiceScope = SQLUtil.ConvertBoolean(dr["ServiceScope"]);
            this.Brand = SQLUtil.TrimNull(dr["Brand"]);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.ManufacturingDate = SQLUtil.ConvertDateTime(dr["ManufacturingDate"]);
            this.FixedAsset = SQLUtil.ConvertBoolean(dr["FixedAsset"]);
            this.AssetCode = SQLUtil.TrimNull(dr["AssetCode"]);
            this.AssetLevel.ID = SQLUtil.ConvertInt(dr["AssetLevel"]);
            this.AssetLevel.Name = AssetLevels.GetAssetLevelDesc(this.AssetLevel.ID);
            this.DepreciationYears = SQLUtil.ConvertInt(dr["DepreciationYears"]);
            this.ValidityStartDate = SQLUtil.ConvertDateTime(dr["ValidityStartDate"]);
            this.ValidityEndDate = SQLUtil.ConvertDateTime(dr["ValidityEndDate"]);
            this.SaleContractName = SQLUtil.TrimNull(dr["SaleContractName"]);
            this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            if (dr.Table.Columns.Contains("SupplierName"))
                this.Supplier.Name = SQLUtil.TrimNull(dr["SupplierName"]);
            this.PurchaseWay = SQLUtil.TrimNull(dr["PurchaseWay"]);
            this.PurchaseAmount = SQLUtil.ConvertDouble(dr["PurchaseAmount"]);
            this.PurchaseDate = SQLUtil.ConvertDateTime(dr["PurchaseDate"]);
            this.IsImport = SQLUtil.ConvertBoolean(dr["IsImport"]);
            this.Department.ID = SQLUtil.ConvertInt(dr["DepartmentID"]);
            this.Department.Name = Manager.LookupManager.GetDepartmentDesc(this.Department.ID);
            this.InstalSite = SQLUtil.TrimNull(dr["InstalSite"]);
            this.InstalDate = SQLUtil.ConvertDateTime(dr["InstalDate"]);
            this.UseageDate = SQLUtil.ConvertDateTime(dr["UseageDate"]);
            this.Accepted = SQLUtil.ConvertBoolean(dr["Accepted"]);
            this.AcceptanceDate = SQLUtil.ConvertDateTime(dr["AcceptanceDate"]);
            this.UsageStatus.ID = SQLUtil.ConvertInt(dr["UsageStatusID"]);
            this.UsageStatus.Name = Manager.LookupManager.GetUsageStatusDesc(this.UsageStatus.ID);
            this.EquipmentStatus.ID = SQLUtil.ConvertInt(dr["EquipmentStatusID"]);
            this.EquipmentStatus.Name = Manager.LookupManager.GetEquipmentStatusDesc(this.EquipmentStatus.ID);
            this.ScrapDate = SQLUtil.ConvertDateTime(dr["ScrapDate"]);
            this.MaintenancePeriod = SQLUtil.ConvertInt(dr["MaintenancePeriod"]);
            this.MaintenanceType.ID = SQLUtil.ConvertInt(dr["MaintenanceTypeID"]);
            this.MaintenanceType.Name = Manager.LookupManager.GetPeriodTypeDesc(this.MaintenanceType.ID);
            this.LastMaintenanceDate = SQLUtil.ConvertDateTime(dr["LastMaintenanceDate"]);
            this.PatrolPeriod = SQLUtil.ConvertInt(dr["PatrolPeriod"]);
            this.PatrolType.ID = SQLUtil.ConvertInt(dr["PatrolTypeID"]);
            this.PatrolType.Name = Manager.LookupManager.GetPeriodTypeDesc(this.PatrolType.ID);
            this.LastPatrolDate = SQLUtil.ConvertDateTime(dr["LastPatrolDate"]);
            this.CorrectionPeriod = SQLUtil.ConvertInt(dr["CorrectionPeriod"]);
            this.CorrectionType.ID = SQLUtil.ConvertInt(dr["CorrectionTypeID"]);
            this.CorrectionType.Name = Manager.LookupManager.GetPeriodTypeDesc(this.CorrectionType.ID);
            this.LastCorrectionDate = SQLUtil.ConvertDateTime(dr["LastCorrectionDate"]);
            this.MandatoryTestStatus.ID = SQLUtil.ConvertInt(dr["MandatoryTestStatus"]);
            this.MandatoryTestStatus.Name = MandatoryTestStatuses.GetMandatoryTestStatusDesc(this.MandatoryTestStatus.ID);
            this.MandatoryTestDate = SQLUtil.ConvertDateTime(dr["MandatoryTestDate"]);
            this.RecallFlag = SQLUtil.ConvertBoolean(dr["RecallFlag"]);
            this.RecallDate = SQLUtil.ConvertDateTime(dr["RecallDate"]);
            this.CreateDate = SQLUtil.ConvertDateTime(dr["CreateDate"]);
            this.CreateUser.ID = SQLUtil.ConvertInt(dr["CreateUserID"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            if (dr.Table.Columns.Contains("ScopeID"))
            {
                this.ContractScope.ID = SQLUtil.ConvertInt(dr["ScopeID"]);
                this.ContractScope.Name = this.ContractScope.ID == 0 ? "" : Manager.LookupManager.GetContractScopeDesc(this.ContractScope.ID);
                if (this.ContractScope.ID == ContractInfo.Scopes.Other) this.ContractScope.Name = SQLUtil.TrimNull(dr["ScopeComments"]);
            }
            if (dr.Table.Columns.Contains("ContractID"))
                this.WarrantyStatus = SQLUtil.ConvertInt(dr["ContractID"]) == 0 ? "保外" : "保内";
            if (dr.Table.Columns.Contains("ConfigLicenceID"))
            {
                this.ConfigLicenceID = SQLUtil.ConvertInt(dr["ConfigLicenceID"]);
            }
            if (dr.Table.Columns.Contains("ConfigLicenceName"))
            {
                this.ConfigLicenceName = SQLUtil.TrimNull(dr["ConfigLicenceName"]);
            }

            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["FujiClass2ID"]);
            if (dr.Table.Columns.Contains("FujiClass2Name"))
                this.FujiClass2.Name = SQLUtil.TrimNull(dr["FujiClass2Name"]);
            if (dr.Table.Columns.Contains("FujiClass1Name"))
                this.FujiClass2.FujiClass1.Name = SQLUtil.TrimNull(dr["FujiClass1Name"]);
            if (dr.Table.Columns.Contains("FujiClass1ID"))
                this.FujiClass2.FujiClass1.ID = SQLUtil.ConvertInt(dr["FujiClass1ID"]);
            this.CTSerialCode = SQLUtil.TrimNull(dr["CTSerialCode"]);
            this.CTUsedSeconds = SQLUtil.ConvertDouble(dr["CTUsedSeconds"]);
        }
        /// <summary>
        /// Copy4s the application.
        /// </summary>
        /// <returns>EquipmentInfo</returns>
        public EquipmentInfo Copy4App()
        {
            EquipmentInfo info = new EquipmentInfo();
            info.ID = this.ID;
            info.ServiceScope = this.ServiceScope;
            info.AssetLevel = this.AssetLevel;
            info.AssetCode = this.AssetCode;
            info.Name = this.Name;
            info.Department = this.Department;
            info.EquipmentCode = this.EquipmentCode;
            info.InstalSite = this.InstalSite;
            info.SerialCode = this.SerialCode;
            info.WarrantyStatus = this.WarrantyStatus;
            info.Manufacturer = this.Manufacturer;
            info.ContractScope = this.ContractScope;
            info.EquipmentLevel = null;
            info.EquipmentClass1 = this.EquipmentClass1;
            info.EquipmentClass2 = this.EquipmentClass2;
            info.EquipmentClass3 = this.EquipmentClass3;
            info.Supplier = null;
            info.UsageStatus = null;
            info.EquipmentStatus = this.EquipmentStatus;
            info.MaintenanceType = null;
            info.PatrolType = null;
            info.CorrectionType = null;
            info.MandatoryTestStatus = null;
            info.CreateUser = null;
            info.FujiClass2 = this.FujiClass2;
            info.CTSerialCode = this.CTSerialCode;
            info.CTUsedSeconds = this.CTUsedSeconds;

            return info;
        }
        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <param name="newInfo">设备信息</param>
        /// <returns>设备信息</returns>
        public DataTable GetChangedFields(EquipmentInfo newInfo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");

            if (this.EquipmentLevel.ID != newInfo.EquipmentLevel.ID)
            {
                dt.Rows.Add("EquipmentLevelDesc", this.EquipmentLevel.Name, EquipmentLevels.GetEquipmentLevelDesc(newInfo.EquipmentLevel.ID));
            }

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
            {
                dt.Rows.Add("EquipmentName", this.Name, newInfo.Name);
            }

            if (this.EquipmentCode != SQLUtil.TrimNull(newInfo.EquipmentCode))
            {
                dt.Rows.Add("EquipmentCode", this.EquipmentCode, newInfo.EquipmentCode);
            }

            if (this.SerialCode != SQLUtil.TrimNull(newInfo.SerialCode))
            {
                dt.Rows.Add("SerialCode", this.SerialCode, newInfo.SerialCode);
            }

            if (this.Manufacturer.ID != newInfo.Manufacturer.ID)
            {
                dt.Rows.Add("EquipmentManufacturerName", this.Manufacturer.Name, newInfo.Manufacturer.Name);
            }

            if (this.EquipmentClass1.Code != SQLUtil.TrimNull(newInfo.EquipmentClass1.Code))
            {
                dt.Rows.Add("EquipmentClass1Name", this.EquipmentClass1.Description, Manager.LookupManager.GetEquipmentClassDesc(newInfo.EquipmentClass1.Code, 1));
            }
            
            if (this.EquipmentClass2.Code != SQLUtil.TrimNull(newInfo.EquipmentClass2.Code) || this.EquipmentClass1.Code != SQLUtil.TrimNull(newInfo.EquipmentClass1.Code))
            {
                dt.Rows.Add("EquipmentClass2Name", this.EquipmentClass2.Description, Manager.LookupManager.GetEquipmentClassDesc(newInfo.EquipmentClass2.Code, 2, newInfo.EquipmentClass1.Code));
            }
            
            if (this.EquipmentClass3.Code != SQLUtil.TrimNull(newInfo.EquipmentClass3.Code) || (this.EquipmentClass2.Code != SQLUtil.TrimNull(newInfo.EquipmentClass2.Code) || this.EquipmentClass1.Code != SQLUtil.TrimNull(newInfo.EquipmentClass1.Code)))
            {
                dt.Rows.Add("EquipmentClass3Name", this.EquipmentClass3.Description, Manager.LookupManager.GetEquipmentClassDesc(newInfo.EquipmentClass3.Code, 3, newInfo.EquipmentClass1.Code + newInfo.EquipmentClass2.Code));
            }

            if (this.ResponseTimeLength != newInfo.ResponseTimeLength)
            {
                dt.Rows.Add("ResponseTimeLength", this.ResponseTimeLength, newInfo.ResponseTimeLength);
            }

            if (this.ServiceScope != newInfo.ServiceScope)
            {
                dt.Rows.Add("ServiceScopeDesc", SQLUtil.ConvertBoolean(this.ServiceScope) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.ServiceScope) ? "是" : "否");
            }

            if (this.Brand != SQLUtil.TrimNull(newInfo.Brand))
            {
                dt.Rows.Add("Brand", this.Brand, newInfo.Brand == null ? "" : newInfo.Brand);
            }

            if (this.Comments != SQLUtil.TrimNull(newInfo.Comments))
            {
                dt.Rows.Add("EquipmentComments", this.Comments, newInfo.Comments == null ? "" : newInfo.Comments);
            }

            if (this.ManufacturingDate != newInfo.ManufacturingDate)
            {
                dt.Rows.Add("ManufacturingDate", SQLUtil.ConvertDateTime(this.ManufacturingDate) == DateTime.MinValue ? "" : this.ManufacturingDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.ManufacturingDate) == DateTime.MinValue ? "" : newInfo.ManufacturingDate.ToString("yyyy-MM-dd"));
            }

            if (this.FixedAsset != newInfo.FixedAsset)
            {
                dt.Rows.Add("FixedAsset", SQLUtil.ConvertBoolean(this.FixedAsset) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.FixedAsset) ? "是" : "否");
            }

            if (this.AssetCode != SQLUtil.TrimNull(newInfo.AssetCode))
            {
                dt.Rows.Add("AssetCode", this.AssetCode, newInfo.AssetCode);
            }

            if (this.AssetLevel.ID != newInfo.AssetLevel.ID)
            {
                dt.Rows.Add("AssetLevel", this.AssetLevel.Name, AssetLevels.GetAssetLevelDesc(newInfo.AssetLevel.ID));
            }

            if (this.DepreciationYears != newInfo.DepreciationYears)
            {
                dt.Rows.Add("DepreciationYears", this.DepreciationYears, newInfo.DepreciationYears);
            }

            if (this.ValidityStartDate != newInfo.ValidityStartDate)
            {
                dt.Rows.Add("ValidityStartDate", SQLUtil.ConvertDateTime(this.ValidityStartDate) == DateTime.MinValue ? "" : this.ValidityStartDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.ValidityStartDate) == DateTime.MinValue ? "" : newInfo.ValidityStartDate.ToString("yyyy-MM-dd"));
            }

            if (this.ValidityEndDate != newInfo.ValidityEndDate)
            {
                dt.Rows.Add("ValidityEndDate", SQLUtil.ConvertDateTime(this.ValidityEndDate) == DateTime.MinValue ? "" : this.ValidityEndDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.ValidityEndDate) == DateTime.MinValue ? "" : newInfo.ValidityEndDate.ToString("yyyy-MM-dd"));
            }

            if (this.SaleContractName != SQLUtil.TrimNull(newInfo.SaleContractName))
            {
                dt.Rows.Add("SaleContractName", this.SaleContractName, newInfo.SaleContractName == null ? "" : newInfo.SaleContractName);
            }

            if (this.Supplier.ID != newInfo.Supplier.ID)
            {
                dt.Rows.Add("DealerName", this.Supplier.Name, newInfo.Supplier.Name);
            }

            if (this.PurchaseWay != SQLUtil.TrimNull(newInfo.PurchaseWay))
            {
                dt.Rows.Add("PurchaseWay", this.PurchaseWay, newInfo.PurchaseWay == null ? "" : newInfo.PurchaseWay);
            }

            if (this.PurchaseAmount != newInfo.PurchaseAmount)
            {
                dt.Rows.Add("EquipmentPurchaseAmount", this.PurchaseAmount, newInfo.PurchaseAmount);
            }

            if (this.PurchaseDate != newInfo.PurchaseDate)
            {
                dt.Rows.Add("PurchaseDate", SQLUtil.ConvertDateTime(this.PurchaseDate) == DateTime.MinValue ? "" : this.PurchaseDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.PurchaseDate) == DateTime.MinValue ? "" : newInfo.PurchaseDate.ToString("yyyy-MM-dd"));
            }

            if (this.IsImport != newInfo.IsImport)
            {
                dt.Rows.Add("IsImport", SQLUtil.ConvertBoolean(this.IsImport) ? "国产" : "进口", SQLUtil.ConvertBoolean(newInfo.IsImport) ? "国产" : "进口");
            }

            if (this.Department.ID != newInfo.Department.ID)
            {
                dt.Rows.Add("DepartmentName", this.Department.Name, Manager.LookupManager.GetDepartmentDesc(newInfo.Department.ID));
            }

            if (this.InstalSite != SQLUtil.TrimNull(newInfo.InstalSite))
            {
                dt.Rows.Add("InstalSite", this.InstalSite, newInfo.InstalSite == null ? "" : newInfo.InstalSite);
            }

            if (this.InstalDate != newInfo.InstalDate)
            {
                dt.Rows.Add("InstalDate", SQLUtil.ConvertDateTime(this.InstalDate) == DateTime.MinValue ? "" : this.InstalDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.InstalDate) == DateTime.MinValue ? "" : newInfo.InstalDate.ToString("yyyy-MM-dd"));
            }

            if (this.UseageDate != newInfo.UseageDate)
            {
                dt.Rows.Add("UseageDate", SQLUtil.ConvertDateTime(this.UseageDate) == DateTime.MinValue ? "" : this.UseageDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.UseageDate) == DateTime.MinValue ? "" : newInfo.UseageDate.ToString("yyyy-MM-dd"));
            }

            if (this.Accepted != newInfo.Accepted)
            {
                dt.Rows.Add("Accepted", SQLUtil.ConvertBoolean(this.Accepted) ? "已验收" : "未验收", SQLUtil.ConvertBoolean(newInfo.Accepted) ? "已验收" : "未验收");
            }

            if (this.AcceptanceDate != newInfo.AcceptanceDate)
            {
                dt.Rows.Add("AcceptanceDate", SQLUtil.ConvertDateTime(this.AcceptanceDate) == DateTime.MinValue ? "" : this.AcceptanceDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.AcceptanceDate) == DateTime.MinValue ? "" : newInfo.AcceptanceDate.ToString("yyyy-MM-dd"));
            }

            if (this.UsageStatus.ID != newInfo.UsageStatus.ID)
            {
                dt.Rows.Add("UsageStatusDesc", this.UsageStatus.Name, Manager.LookupManager.GetUsageStatusDesc(newInfo.UsageStatus.ID));
            }

            if (this.EquipmentStatus.ID != newInfo.EquipmentStatus.ID)
            {
                dt.Rows.Add("EquipmentStatusDesc", this.EquipmentStatus.Name, Manager.LookupManager.GetEquipmentStatusDesc(newInfo.EquipmentStatus.ID));
            }

            if (this.ScrapDate != newInfo.ScrapDate)
            {
                dt.Rows.Add("ScrapDate", SQLUtil.ConvertDateTime(this.ScrapDate) == DateTime.MinValue ? "" : this.ScrapDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.ScrapDate) == DateTime.MinValue ? "" : newInfo.ScrapDate.ToString("yyyy-MM-dd"));
            }

            if (this.MaintenancePeriod != newInfo.MaintenancePeriod || this.MaintenanceType.ID != newInfo.MaintenanceType.ID)
            {
                dt.Rows.Add("MaintenancePeriod", this.MaintenancePeriod == 0 ? "无保养" : this.MaintenancePeriod + this.MaintenanceType.Name, newInfo.MaintenancePeriod == 0 ? "无保养" : SQLUtil.ConvertInt(newInfo.MaintenancePeriod) + Manager.LookupManager.GetPeriodTypeDesc(newInfo.MaintenanceType.ID));
            }

            if (this.PatrolPeriod != newInfo.PatrolPeriod || this.PatrolType.ID != newInfo.PatrolType.ID)
            {
                dt.Rows.Add("PatrolPeriod", this.PatrolPeriod == 0 ? "无巡检" : this.PatrolPeriod + this.PatrolType.Name, newInfo.PatrolPeriod == 0 ? "无巡检" : newInfo.PatrolPeriod + Manager.LookupManager.GetPeriodTypeDesc(newInfo.PatrolType.ID));
            }

            if (this.CorrectionPeriod != newInfo.CorrectionPeriod || this.CorrectionType.ID != newInfo.CorrectionType.ID)
            {
                dt.Rows.Add("CorrectionPeriod", this.CorrectionPeriod == 0 ? "无校准" : this.CorrectionPeriod + this.CorrectionType.Name, newInfo.CorrectionPeriod == 0 ? "无校准" : newInfo.CorrectionPeriod + Manager.LookupManager.GetPeriodTypeDesc(newInfo.CorrectionType.ID));
            }

            if (this.MandatoryTestStatus.ID != newInfo.MandatoryTestStatus.ID)
            {
                dt.Rows.Add("MandatoryTestStatusDesc", this.MandatoryTestStatus.Name, MandatoryTestStatuses.GetMandatoryTestStatusDesc(newInfo.MandatoryTestStatus.ID));
            }

            if (this.MandatoryTestDate != newInfo.MandatoryTestDate)
            {
                dt.Rows.Add("MandatoryTestDate", SQLUtil.ConvertDateTime(this.MandatoryTestDate) == DateTime.MinValue ? "" : this.MandatoryTestDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.MandatoryTestDate) == DateTime.MinValue ? "" : newInfo.MandatoryTestDate.ToString("yyyy-MM-dd"));
            }

            if (this.RecallFlag != newInfo.RecallFlag)
            {
                dt.Rows.Add("RecallFlag", SQLUtil.ConvertBoolean(this.RecallFlag) ? "是" : "否", SQLUtil.ConvertBoolean(newInfo.RecallFlag) ? "是" : "否");
            }

            if (this.RecallDate != newInfo.RecallDate)
            {
                dt.Rows.Add("RecallDate", SQLUtil.ConvertDateTime(this.RecallDate) == DateTime.MinValue ? "" : this.RecallDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.RecallDate) == DateTime.MinValue ? "" : newInfo.RecallDate.ToString("yyyy-MM-dd"));
            }

            if(this.FujiClass2.ID != newInfo.FujiClass2.ID)
            {
                dt.Rows.Add("EquipmentFujiClass2Name", SQLUtil.TrimNull(this.FujiClass2.Name), SQLUtil.TrimNull(newInfo.FujiClass2.Name));
            }

            if (this.CTSerialCode != SQLUtil.TrimNull(newInfo.CTSerialCode))
            {
                dt.Rows.Add("CTSerialCode", this.CTSerialCode, SQLUtil.TrimNull(newInfo.CTSerialCode));
            }

            if (this.CTUsedSeconds != newInfo.CTUsedSeconds)
            {
                dt.Rows.Add("CTUsedSeconds", this.CTUsedSeconds, newInfo.CTUsedSeconds);
            }

            return dt;
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public static class FileTypes
        {
            /// <summary>
            /// 销售合同
            /// </summary>
            public const int SaleContract = 1;
            /// <summary>
            /// 注册证文件
            /// </summary>
            public const int Registration = 2;
            /// <summary>
            /// 采购配置清单
            /// </summary>
            public const int PurchaseDetail = 3;
            /// <summary>
            /// 设备外观
            /// </summary>
            public const int ConfigLicence = 4;
            /// <summary>
            /// 设备铭牌
            /// </summary>
            public const int Nameplate = 5;
            /// <summary>
            /// 设备标签
            /// </summary>
            public const int Label = 6;
            /// <summary>
            /// 技术文档
            /// </summary>
            public const int TechDocu = 7;
            /// <summary>
            /// 其它
            /// </summary>
            public const int OtherFile = 8;

            /// <summary>
            /// 获取附件类型描述
            /// </summary>
            /// <param name="id">附件类型id</param>
            /// <returns>附件类型描述</returns>
            public static string GetFileName(int id)
            {
                switch (id)
                {
                   // case SaleContract:
                   //     return "销售合同文件";
                   // case Registration:
                   //     return "注册证文件";
                   // case PurchaseDetail:
                   //     return "采购配置清单";
                    case ConfigLicence:
                        return "设备外观";
                    case Nameplate:
                        return "设备铭牌";
                    case Label:
                        return "设备标签";
                    //case TechDocu:
                    //    return "技术文档";
                    //case OtherFile:
                    //    return "附件";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 设备状态
        /// </summary>
        public static class EquipmentStatuses
        {
            /// <summary>
            /// 正常
            /// </summary>
            public const int Normal = 1;
            /// <summary>
            /// 故障
            /// </summary>
            public const int Fault = 2;
            /// <summary>
            /// 报废
            /// </summary>
            public const int Scrap = 3;
        }
        /// <summary>
        /// 设备使用状态
        /// </summary>
        public static class UsageStatuses
        {
            /// <summary>
            /// 使用
            /// </summary>
            public const int Running = 1;
            /// <summary>
            /// 停用
            /// </summary>
            public const int Stop = 2;
        }
        /// <summary>
        /// 设备资产等级
        /// </summary>
        public static class AssetLevels
        {
            /// <summary>
            /// 重要
            /// </summary>
            public const int Important = 1;
            /// <summary>
            /// 一般
            /// </summary>
            public const int Normal = 2;
            /// <summary>
            /// 特殊
            /// </summary>
            public const int Special = 3;

            /// <summary>
            /// 获取设备资产等级列表
            /// </summary>
            /// <returns>设备资产等级列表</returns>
            public static List<KeyValueInfo> GetAssetLevels()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = AssetLevels.Important, Name = GetAssetLevelDesc(AssetLevels.Important) });
                statuses.Add(new KeyValueInfo() { ID = AssetLevels.Normal, Name = GetAssetLevelDesc(AssetLevels.Normal) });
                statuses.Add(new KeyValueInfo() { ID = AssetLevels.Special, Name = GetAssetLevelDesc(AssetLevels.Special) });
                return statuses;
            }
            /// <summary>
            /// 根据设备资产等级编号获取设备资产等级描述
            /// </summary>
            /// <param name="statusId">设备资产等级编号</param>
            /// <returns>设备资产等级描述</returns>
            public static string GetAssetLevelDesc(int statusId)
            {
                switch (statusId)
                {
                    case AssetLevels.Important:
                        return "重要";
                    case AssetLevels.Normal:
                        return "一般";
                    case AssetLevels.Special:
                        return "特殊";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 设备等级
        /// </summary>
        public static class EquipmentLevels
        {
            /// <summary>
            /// 1类
            /// </summary>
            public const int One = 1;
            /// <summary>
            /// 2类
            /// </summary>
            public const int Two = 2;
            /// <summary>
            /// 3类
            /// </summary>
            public const int Three = 3;

            /// <summary>
            /// 获取设备等级列表
            /// </summary>
            /// <returns>设备等级列表</returns>
            public static List<KeyValueInfo> GetEquipmentLevels()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = EquipmentLevels.One, Name = GetEquipmentLevelDesc(EquipmentLevels.One) });
                statuses.Add(new KeyValueInfo() { ID = EquipmentLevels.Two, Name = GetEquipmentLevelDesc(EquipmentLevels.Two) });
                statuses.Add(new KeyValueInfo() { ID = EquipmentLevels.Three, Name = GetEquipmentLevelDesc(EquipmentLevels.Three) });
                return statuses;
            }
            /// <summary>
            /// 根据设备等级编号获取设备等级描述
            /// </summary>
            /// <param name="statusId">设备等级编号</param>
            /// <returns>设备等级描述</returns>
            public static string GetEquipmentLevelDesc(int statusId)
            {
                switch (statusId)
                {
                    case EquipmentLevels.One:
                        return "1类";
                    case EquipmentLevels.Two:
                        return "2类";
                    case EquipmentLevels.Three:
                        return "3类";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 强检标记
        /// </summary>
        public static class MandatoryTestStatuses
        {
            /// <summary>
            /// 待强检
            /// </summary>
            public const int Waiting = 1;
            /// <summary>
            /// 已强检
            /// </summary>
            public const int Already = 2;
            /// <summary>
            /// 获取强检标记列表
            /// </summary>
            /// <returns>强检标记列表</returns>
            public static List<KeyValueInfo> GetMandatoryTestStatuses()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = MandatoryTestStatuses.Waiting, Name = GetMandatoryTestStatusDesc(MandatoryTestStatuses.Waiting) });
                statuses.Add(new KeyValueInfo() { ID = MandatoryTestStatuses.Already, Name = GetMandatoryTestStatusDesc(MandatoryTestStatuses.Already) });
                return statuses;
            }
            /// <summary>
            /// 根据强检标记编号获取强检标记描述
            /// </summary>
            /// <param name="statusId">强检标记编号</param>
            /// <returns>强检标记描述</returns>
            public static string GetMandatoryTestStatusDesc(int statusId)
            {
                switch (statusId)
                {
                    case MandatoryTestStatuses.Waiting:
                        return "待强检";
                    case MandatoryTestStatuses.Already:
                        return "已强检";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 维保状态
        /// </summary>
        public static class WarrantyStatuses
        {
            /// <summary>
            /// 保外
            /// </summary>
            public const int Expired = 1;
            /// <summary>
            /// 保内
            /// </summary>
            public const int Active = 2;
            /// <summary>
            /// 获取维保状态列表
            /// </summary>
            /// <returns>维保状态列表</returns>
            public static List<KeyValueInfo> GetWarrantyStatuses()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = WarrantyStatuses.Expired, Name = GetWarrantyStatusesDesc(WarrantyStatuses.Expired) });
                statuses.Add(new KeyValueInfo() { ID = WarrantyStatuses.Active, Name = GetWarrantyStatusesDesc(WarrantyStatuses.Active) });
                return statuses;
            }
            /// <summary>
            /// 根据维保状态编号获取维保状态描述
            /// </summary>
            /// <param name="statusId">维保状态编号</param>
            /// <returns>维保状态描述</returns>
            public static string GetWarrantyStatusesDesc(int statusId)
            {
                switch (statusId)
                {
                    case WarrantyStatuses.Expired:
                        return "保外";
                    case WarrantyStatuses.Active:
                        return "保内";
                    default:
                        return "全部";
                }
            }
        }
        /// <summary>
        /// 周期
        /// </summary>
        public static class PeriodTypes
        {
            /// <summary>
            /// 天
            /// </summary>
            public const int Day = 1;
            /// <summary>
            /// 月
            /// </summary>
            public const int Month = 3;
            /// <summary>
            /// 年
            /// </summary>
            public const int Year = 4;
        }
    }

    /// <summary>
    /// 设备类别信息
    /// </summary>
    public class EquipmentClassInfo
    {
        /// <summary>
        /// 等级
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }
        /// <summary>
        /// 父级编号
        /// </summary>
        /// <value>
        /// The parent code.
        /// </value>
        public string ParentCode { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// 设备类别信息
        /// </summary>
        public EquipmentClassInfo() { }
        /// <summary>
        /// 获取设备类别信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public EquipmentClassInfo(DataRow dr)
            : this()
        {
            this.Level = SQLUtil.ConvertInt(dr["Level"]);
            this.ParentCode = SQLUtil.TrimNull(dr["ParentCode"]);
            this.Code = SQLUtil.TrimNull(dr["Code"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
        }
    }

    public class EqptComponentInfo
    {
        public InvComponentInfo InvComponent { get; set; }
        public DispatchReportInfo NewDispatchReport { get; set; }
        public DispatchReportInfo OldDispatchReport { get; set; }
        public DateTime InstalDate { get; set; }
        public DateTime RemoveDate { get; set; }

        public EqptComponentInfo()
        {
            this.InvComponent = new InvComponentInfo();
            this.NewDispatchReport = new DispatchReportInfo();
            this.OldDispatchReport = new DispatchReportInfo();
        }

        public EqptComponentInfo(DataRow dr)
            :this()
        {
            this.InvComponent.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
            this.InvComponent.Component.Name = SQLUtil.TrimNull(dr["Name"]);
            this.InvComponent.Component.Description = SQLUtil.TrimNull(dr["Description"]);
            this.InstalDate = SQLUtil.ConvertDateTime(dr["InstalDate"]);
            this.RemoveDate = SQLUtil.ConvertDateTime(dr["RemoveDate"]);

            this.NewDispatchReport.ID = SQLUtil.ConvertInt(dr["NewDispatchReportID"]);
            this.NewDispatchReport.Dispatch.ID = SQLUtil.ConvertInt(dr["NewDispatchID"]);
            this.OldDispatchReport.ID = SQLUtil.ConvertInt(dr["OldDispatchReportID"]);
            this.OldDispatchReport.Dispatch.ID = SQLUtil.ConvertInt(dr["OldDispatchID"]);
        }
    }

    public class EqptConsumableInfo
    {
        public InvConsumableInfo InvConsumable { get; set; }
        public DispatchReportInfo DispatchReport { get; set; }
        public Double Qty { get; set; }
        public DateTime UsedDate { get; set; }

        public EqptConsumableInfo()
        {
            this.InvConsumable = new InvConsumableInfo();
            this.DispatchReport = new DispatchReportInfo();
        }

        public EqptConsumableInfo(DataRow dr)
            : this()
        {
            this.InvConsumable.LotNum = SQLUtil.TrimNull(dr["LotNum"]);
            this.InvConsumable.Consumable.Name = SQLUtil.TrimNull(dr["Name"]);
            this.InvConsumable.Consumable.Description = SQLUtil.TrimNull(dr["Description"]);
            this.Qty = SQLUtil.ConvertDouble(dr["Qty"]);
            this.UsedDate = SQLUtil.ConvertDateTime(dr["UsedDate"]);

            this.DispatchReport.ID = SQLUtil.ConvertInt(dr["DispatchReportID"]);
            this.DispatchReport.Dispatch.ID = SQLUtil.ConvertInt(dr["DispatchID"]);
        }
    }
}
