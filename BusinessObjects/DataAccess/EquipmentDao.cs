using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 设备dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class EquipmentDao : BaseDao
    {

        #region "tblEquipment"
        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <param name="warrantyStatus">维保状态</param>
        /// <param name="departmentID">科室</param>
        /// <param name="filterTextName">设备名称</param>
        /// <param name="filterTextSerialCode">设备序列号</param>
        /// <param name="useStatus">是否停用</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>设备数量</returns>
        public int QueryEquipmentsCount(int status, int warrantyStatus, int departmentID, int fujiClass2ID, string filterTextName, string filterTextSerialCode, bool useStatus, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(e.ID) FROM tblEquipment AS e " +
                    " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID " +
                     " LEFT JOIN tblFujiClass2 f2 ON e.FujiClass2ID = f2.ID ";
            sqlStr += " WHERE 1=1 ";
            if (status == 0)
                sqlStr += " AND EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap;
            if (status > 0)
                sqlStr += " AND EquipmentStatusID = " + status;
            if (useStatus)
                sqlStr += " AND UsageStatusID = " + EquipmentInfo.UsageStatuses.Stop;
            if (warrantyStatus == EquipmentInfo.WarrantyStatuses.Active)
                sqlStr += " AND c.ContractID IS NOT NULL ";
            if (warrantyStatus == EquipmentInfo.WarrantyStatuses.Expired)
                sqlStr += " AND c.ContractID IS NULL ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);
            if (departmentID >= 0)
                sqlStr += " AND e.DepartmentID = " + departmentID;
            if (fujiClass2ID != 0)
                sqlStr += " AND e.FujiClass2ID = " + fujiClass2ID;
            if (!string.IsNullOrEmpty(filterTextName))
                sqlStr += " AND UPPER(e.Name) Like @FilterTextName ";
            if (!string.IsNullOrEmpty(filterTextSerialCode))
                sqlStr += " AND UPPER(e.SerialCode) Like @FilterTextSerialCode ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);
                if (!string.IsNullOrEmpty(filterTextName))
                    command.Parameters.Add("@FilterTextName", SqlDbType.NVarChar).Value = "%" + filterTextName.ToUpper() + "%";
                if (!string.IsNullOrEmpty(filterTextSerialCode))
                    command.Parameters.Add("@FilterTextSerialCode", SqlDbType.NVarChar).Value = "%" + filterTextSerialCode.ToUpper() + "%";

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <param name="warrantyStatus">维保状态</param>
        /// <param name="departmentID">科室</param>
        /// <param name="filterTextName">设备名称</param>
        /// <param name="filterTextSerialCode">设备序列号</param>
        /// <param name="useStatus">是否停用</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段名</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>设备列表</returns>
        public List<EquipmentInfo> QueryEquipments(int status, int warrantyStatus, int departmentID, int fujiClass2ID, string filterTextName, string filterTextSerialCode, bool useStatus, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT e.*, f2.Name AS FujiClass2Name, f1.ID AS FujiClass1ID, f1.Name AS FujiClass1Name, s.Name AS SupplierName, su.Name AS ManufacturerName, c.ContractID, ct.ScopeID, ct.ScopeComments FROM tblEquipment AS e " +
                     " LEFT JOIN tblFujiClass2 AS f2 ON e.FujiClass2ID=f2.ID " +
                     " LEFT JOIN tblFujiClass1 AS f1 ON f2.FujiClass1ID=f1.ID " +
                     " LEFT JOIN tblSupplier AS s ON e.SupplierID=s.ID " +
                     " LEFT JOIN tblSupplier AS su ON e.ManufacturerID=su.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblContract AS ct on ct.ID = c.ContractID" +
                     " WHERE 1=1 ";
            if (status == 0)
                sqlStr += " AND EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap;
            if (status > 0)
                sqlStr += " AND e.EquipmentStatusID = " + status;
            if (useStatus)
                sqlStr += " AND UsageStatusID = " + EquipmentInfo.UsageStatuses.Stop;
            if (warrantyStatus == EquipmentInfo.WarrantyStatuses.Active)
                sqlStr += " AND c.ContractID IS NOT NULL ";
            if (warrantyStatus == EquipmentInfo.WarrantyStatuses.Expired)
                sqlStr += " AND c.ContractID IS NULL ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);
            if (departmentID >= 0)
                sqlStr += " AND e.DepartmentID = " + departmentID;
            if (fujiClass2ID != 0)
                sqlStr += " AND e.FujiClass2ID = " + fujiClass2ID;
            if (!string.IsNullOrEmpty(filterTextName))
                sqlStr += " AND UPPER(e.Name) Like @FilterTextName ";
            if (!string.IsNullOrEmpty(filterTextSerialCode))
                sqlStr += " AND UPPER(e.SerialCode) Like @FilterTextSerialCode ";

            sqlStr += GenerateSortClause(sortDirection, sortField, "e.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);
                if (!string.IsNullOrEmpty(filterTextName))
                    command.Parameters.Add("@FilterTextName", SqlDbType.NVarChar).Value = "%" + filterTextName.ToUpper() + "%";
                if (!string.IsNullOrEmpty(filterTextSerialCode))
                    command.Parameters.Add("@FilterTextSerialCode", SqlDbType.NVarChar).Value = "%" + filterTextSerialCode.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>设备信息</returns>
        public List<EquipmentInfo> GetEquipments(string filterText, int curRowNum = 0, int pageSize = 0)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT e.*, s.Name AS ManufacturerName, c.ContractID, ct.ScopeID, ct.ScopeComments FROM tblEquipment AS e" +
                     " LEFT JOIN tblSupplier AS s ON e.ManufacturerID=s.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblContract AS ct on ct.ID = c.ContractID" +
                     " WHERE 1=1 AND EquipmentStatusID <> @EquipmentStatusID ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += " AND ( UPPER(e.Name) LIKE @FilterText OR UPPER(e.EquipmentCode) LIKE @FilterText OR UPPER(e.SerialCode) LIKE @FilterText ) ";
            sqlStr += " ORDER BY ID Desc";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.NVarChar).Value = EquipmentInfo.EquipmentStatuses.Scrap;
                if (!string.IsNullOrEmpty(filterText))
                    command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据科室获取设备信息
        /// </summary>
        /// <param name="id">部门ID</param>
        /// <returns>设备信息</returns>
        public List<EquipmentInfo> GetEquipmentsByDepartmentID(int id)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT e.*, s.Name AS ManufacturerName, c.ContractID, f.ID AS ConfigLicenceID, f.FileName AS ConfigLicenceName FROM tblEquipment AS e" +
                     " LEFT JOIN tblSupplier AS s ON e.ManufacturerID=s.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblEquipmentFile AS f on f.EquipmentID = e.ID and f.FileType = @FileType" +
                     " WHERE DepartmentID = @DepartmentID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FileType", SqlDbType.NVarChar).Value = EquipmentInfo.FileTypes.ConfigLicence;
                command.Parameters.Add("@DepartmentID", SqlDbType.NVarChar).Value = id;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据计划服务类型获取设备信息
        /// </summary>
        /// <param name="type">计划服务类型</param>
        /// <returns>设备信息</returns>
        public List<EquipmentInfo> GetScheduledEquipments(string type)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = string.Format("SELECT * FROM tblEquipment WHERE {0}ID <> 0 AND EquipmentStatusID <> {1} ", type, EquipmentInfo.EquipmentStatuses.Scrap);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据设备id和请求类型获取已存在该类型计划服务请求的设备信息
        /// </summary>
        /// <param name="equipmentIds">设备ID</param>
        /// <param name="requestType">请求类型</param>
        /// <returns>设备信息</returns>
        public List<EquipmentInfo> CheckRequestExsist(List<int> equipmentIds, int requestType)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT DISTINCT e.* FROM tblRequest AS r " +
                     " LEFT JOIN jctRequestEqpt AS j ON j.RequestID = r.ID " +
                     " LEFT JOIN tblEquipment AS e ON e.ID = j.EquipmentID " +
                     " WHERE ((r.Source = @Source AND r.RequestType = @RequestType) AND (r.CloseDate IS NULL OR r.RequestDate = CONVERT(varchar(100), GETDATE(), 23)) ) AND e.ID IN (" + SQLUtil.ConvertToInStr(equipmentIds) + ")";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Source", SqlDbType.Int).Value = RequestInfo.Sources.SysRequest;
                command.Parameters.Add("@RequestType", SqlDbType.Int).Value = requestType;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取各个状态的设备数量
        /// </summary>
        /// <returns>各个状态的设备数量</returns>
        public Dictionary<int, int> GetEquipmentCount()
        {
            sqlStr = "SELECT EquipmentStatusID, Count(ID) FROM tblEquipment GROUP BY EquipmentStatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备ID获取设备信息
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>设备信息</returns>
        public EquipmentInfo GetEquipmentByID(int id)
        {
            sqlStr = "SELECT e.*, s.Name AS SupplierName, su.Name AS ManufacturerName, c.ContractID,ct.*, f2.Name FujiClass2Name, f.ID AS ConfigLicenceID, f.FileName AS ConfigLicenceName FROM tblEquipment AS e " +
                     " LEFT JOIN tblSupplier AS s ON e.SupplierID=s.ID " +
                     " LEFT JOIN tblSupplier AS su ON e.ManufacturerID=su.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblContract AS ct on ct.ID = c.ContractID" +
                     " LEFT JOIN tblFujiClass2 f2 ON e.FujiClass2ID = f2.ID " +
                     " LEFT JOIN tblEquipmentFile AS f on f.EquipmentID = e.ID and f.FileType = @FileType" +
                     " WHERE e.ID = @ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FileType", SqlDbType.NVarChar).Value = EquipmentInfo.FileTypes.ConfigLicence;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new EquipmentInfo(dr);
                    else
                        return null;
                }
            }

        }
        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <param name="user">用户信息</param>
        /// <returns>设备信息</returns>
        public EquipmentInfo AddEquipment(EquipmentInfo info, UserInfo user)
        {
            sqlStr = "INSERT INTO tblEquipment(EquipmentLevel,Name,EquipmentCode,SerialCode,ManufacturerID,EquipmentClass1,EquipmentClass2,EquipmentClass3,ServiceScope,Brand,Comments,ManufacturingDate, " +
                     " FixedAsset,AssetCode,AssetLevel,DepreciationYears,ValidityStartDate,ValidityEndDate, " +
                     " SaleContractName,SupplierID,PurchaseWay,PurchaseAmount,PurchaseDate,IsImport, " +
                     " DepartmentID,InstalSite,InstalDate,UseageDate,Accepted,AcceptanceDate,UsageStatusID,EquipmentStatusID,ScrapDate,MaintenancePeriod, " +
                     " MaintenanceTypeID,PatrolPeriod,PatrolTypeID,CorrectionPeriod,CorrectionTypeID,MandatoryTestStatus,MandatoryTestDate,RecallFlag, " +
                     " RecallDate,CreateDate,CreateUserID,UpdateDate,ResponseTimeLength,FujiClass2ID,CTSerialCode,CTUsedSeconds ) " +
                     " VALUES(@EquipmentLevel,@Name,@EquipmentCode,@SerialCode,@ManufacturerID,@EquipmentClass1,@EquipmentClass2,@EquipmentClass3,@ServiceScope,@Brand,@Comments,@ManufacturingDate, " +
                     " @FixedAsset,@AssetCode,@AssetLevel,@DepreciationYears,@ValidityStartDate,@ValidityEndDate, " +
                     " @SaleContractName,@SupplierID,@PurchaseWay,@PurchaseAmount,@PurchaseDate,@IsImport, " +
                     " @DepartmentID,@InstalSite,@InstalDate,@UseageDate,@Accepted,@AcceptanceDate,@UsageStatusID,@EquipmentStatusID,@ScrapDate,@MaintenancePeriod, " +
                     " @MaintenanceTypeID,@PatrolPeriod,@PatrolTypeID,@CorrectionPeriod,@CorrectionTypeID,@MandatoryTestStatus,@MandatoryTestDate,@RecallFlag, " +
                     " @RecallDate,GETDATE(),@CreateUserID,GETDATE(),@ResponseTimeLength,@FujiClass2ID,@CTSerialCode,@CTUsedSeconds);" +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentLevel", SqlDbType.Int).Value = info.EquipmentLevel.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@EquipmentCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentCode);
                command.Parameters.Add("@SerialCode", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@ManufacturerID", SqlDbType.Int).Value = info.Manufacturer.ID;
                command.Parameters.Add("@EquipmentClass1", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass1.Code == null ? "00" : info.EquipmentClass1.Code);
                command.Parameters.Add("@EquipmentClass2", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass2.Code == null ? "00" : info.EquipmentClass2.Code);
                command.Parameters.Add("@EquipmentClass3", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass3.Code == null ? "00" : info.EquipmentClass3.Code);
                command.Parameters.Add("@ServiceScope", SqlDbType.Bit).Value = info.ServiceScope;
                command.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Brand);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@ManufacturingDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ManufacturingDate);

                command.Parameters.Add("@FixedAsset", SqlDbType.Bit).Value = info.FixedAsset;
                command.Parameters.Add("@AssetCode", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.AssetCode);
                command.Parameters.Add("@AssetLevel", SqlDbType.Int).Value = info.AssetLevel.ID;
                command.Parameters.Add("@DepreciationYears", SqlDbType.Int).Value = info.DepreciationYears;
                command.Parameters.Add("@ValidityStartDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ValidityStartDate);
                command.Parameters.Add("@ValidityEndDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ValidityEndDate);

                command.Parameters.Add("@SaleContractName", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SaleContractName);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = info.Supplier.ID;
                command.Parameters.Add("@PurchaseWay", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.PurchaseWay);
                command.Parameters.Add("@PurchaseAmount", SqlDbType.Decimal).Value = info.PurchaseAmount;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.PurchaseDate);
                command.Parameters.Add("@IsImport", SqlDbType.Bit).Value = info.IsImport;

                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = info.Department.ID;
                command.Parameters.Add("@InstalSite", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.InstalSite);
                command.Parameters.Add("@InstalDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.InstalDate);
                command.Parameters.Add("@UseageDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.UseageDate);
                command.Parameters.Add("@Accepted", SqlDbType.Bit).Value = info.Accepted;
                command.Parameters.Add("@AcceptanceDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.AcceptanceDate);
                command.Parameters.Add("@UsageStatusID", SqlDbType.Int).Value = info.UsageStatus.ID;
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.Int).Value = info.EquipmentStatus.ID;
                command.Parameters.Add("@ScrapDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ScrapDate);
                command.Parameters.Add("@MaintenancePeriod", SqlDbType.Int).Value = info.MaintenancePeriod;
                command.Parameters.Add("@MaintenanceTypeID", SqlDbType.Int).Value = info.MaintenanceType.ID;
                command.Parameters.Add("@PatrolPeriod", SqlDbType.Int).Value = info.PatrolPeriod;
                command.Parameters.Add("@PatrolTypeID", SqlDbType.Int).Value = info.PatrolType.ID;
                command.Parameters.Add("@CorrectionPeriod", SqlDbType.Int).Value = info.CorrectionPeriod;
                command.Parameters.Add("@CorrectionTypeID", SqlDbType.Int).Value = info.CorrectionType.ID;
                command.Parameters.Add("@MandatoryTestStatus", SqlDbType.Int).Value = info.MandatoryTestStatus.ID;
                command.Parameters.Add("@MandatoryTestDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.MandatoryTestDate);
                command.Parameters.Add("@RecallFlag", SqlDbType.Bit).Value = info.RecallFlag;
                command.Parameters.Add("@RecallDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.RecallDate);
                command.Parameters.Add("@CreateUserID", SqlDbType.Int).Value = user.ID;
                command.Parameters.Add("@ResponseTimeLength", SqlDbType.Int).Value = info.ResponseTimeLength;

                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = info.FujiClass2.ID;
                command.Parameters.Add("@CTSerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.CTSerialCode);
                command.Parameters.Add("@CTUsedSeconds", SqlDbType.Decimal).Value = info.CTUsedSeconds;
                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info;
            }
        }
        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <param name="user">用户信息</param>
        public void UpdateEquipment(EquipmentInfo info, UserInfo user)
        {
            sqlStr = "UPDATE tblEquipment SET EquipmentLevel = @EquipmentLevel, Name = @Name, EquipmentCode = @EquipmentCode, SerialCode = @SerialCode, " +
                     " ManufacturerID = @ManufacturerID, EquipmentClass1 = @EquipmentClass1, EquipmentClass2 = @EquipmentClass2, EquipmentClass3 = @EquipmentClass3,ServiceScope = @ServiceScope, " +
                     " Brand = @Brand,Comments = @Comments,ManufacturingDate = @ManufacturingDate, " +
                     " FixedAsset = @FixedAsset, AssetCode = @AssetCode,AssetLevel = @AssetLevel,DepreciationYears = @DepreciationYears, ValidityStartDate = @ValidityStartDate, ValidityEndDate = @ValidityEndDate, " +
                     " SaleContractName = @SaleContractName, SupplierID = @SupplierID, PurchaseWay = @PurchaseWay, PurchaseAmount = @PurchaseAmount, PurchaseDate = @PurchaseDate, IsImport = @IsImport, " +
                     " DepartmentID = @DepartmentID, InstalSite = @InstalSite, InstalDate = @InstalDate, UseageDate = @UseageDate, " +
                     " Accepted = @Accepted, AcceptanceDate = @AcceptanceDate, UsageStatusID = @UsageStatusID, EquipmentStatusID = @EquipmentStatusID, ScrapDate = @ScrapDate, MaintenancePeriod = @MaintenancePeriod, " +
                     " MaintenanceTypeID = @MaintenanceTypeID, PatrolPeriod = @PatrolPeriod, PatrolTypeID = @PatrolTypeID, CorrectionPeriod = @CorrectionPeriod, " +
                     " CorrectionTypeID = @CorrectionTypeID, MandatoryTestStatus = @MandatoryTestStatus, MandatoryTestDate = @MandatoryTestDate, RecallFlag = @RecallFlag,  " +
                     " RecallDate = @RecallDate, CreateUserID = @CreateUserID, UpdateDate = @UpdateDate, ResponseTimeLength = @ResponseTimeLength, FujiClass2ID = @FujiClass2ID, CTSerialCode = @CTSerialCode, CTUsedSeconds = @CTUsedSeconds ";

            sqlStr += " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {

                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@EquipmentLevel", SqlDbType.Int).Value = info.EquipmentLevel.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@EquipmentCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentCode);
                command.Parameters.Add("@SerialCode", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@ManufacturerID", SqlDbType.Int).Value = info.Manufacturer.ID;
                command.Parameters.Add("@EquipmentClass1", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass1.Code == null ? "00" : info.EquipmentClass1.Code);
                command.Parameters.Add("@EquipmentClass2", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass2.Code == null ? "00" : info.EquipmentClass2.Code);
                command.Parameters.Add("@EquipmentClass3", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentClass3.Code == null ? "00" : info.EquipmentClass3.Code);
                command.Parameters.Add("@ServiceScope", SqlDbType.Bit).Value = info.ServiceScope;
                command.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Brand);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@ManufacturingDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ManufacturingDate);

                command.Parameters.Add("@FixedAsset", SqlDbType.Bit).Value = info.FixedAsset;
                command.Parameters.Add("@AssetCode", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.AssetCode);
                command.Parameters.Add("@AssetLevel", SqlDbType.Int).Value = info.AssetLevel.ID;
                command.Parameters.Add("@DepreciationYears", SqlDbType.Int).Value = info.DepreciationYears;
                command.Parameters.Add("@ValidityStartDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ValidityStartDate);
                command.Parameters.Add("@ValidityEndDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ValidityEndDate);

                command.Parameters.Add("@SaleContractName", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SaleContractName);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = info.Supplier.ID;
                command.Parameters.Add("@PurchaseWay", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.PurchaseWay);
                command.Parameters.Add("@PurchaseAmount", SqlDbType.Decimal).Value = info.PurchaseAmount;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.PurchaseDate);
                command.Parameters.Add("@IsImport", SqlDbType.Int).Value = info.IsImport;

                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = info.Department.ID;
                command.Parameters.Add("@InstalSite", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.InstalSite);
                command.Parameters.Add("@InstalDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.InstalDate);
                command.Parameters.Add("@UseageDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.UseageDate);
                command.Parameters.Add("@Accepted", SqlDbType.Bit).Value = info.Accepted;
                command.Parameters.Add("@AcceptanceDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.AcceptanceDate);
                command.Parameters.Add("@UsageStatusID", SqlDbType.Int).Value = info.UsageStatus.ID;
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.Int).Value = info.EquipmentStatus.ID;
                command.Parameters.Add("@ScrapDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.ScrapDate);
                command.Parameters.Add("@MaintenancePeriod", SqlDbType.Int).Value = info.MaintenancePeriod;
                command.Parameters.Add("@MaintenanceTypeID", SqlDbType.Int).Value = info.MaintenanceType.ID;
                command.Parameters.Add("@PatrolPeriod", SqlDbType.Int).Value = info.PatrolPeriod;
                command.Parameters.Add("@PatrolTypeID", SqlDbType.Int).Value = info.PatrolType.ID;
                command.Parameters.Add("@CorrectionPeriod", SqlDbType.Int).Value = info.CorrectionPeriod;
                command.Parameters.Add("@CorrectionTypeID", SqlDbType.Int).Value = info.CorrectionType.ID;
                command.Parameters.Add("@MandatoryTestStatus", SqlDbType.Int).Value = info.MandatoryTestStatus.ID;
                command.Parameters.Add("@MandatoryTestDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.MandatoryTestDate);
                command.Parameters.Add("@RecallFlag", SqlDbType.Bit).Value = info.RecallFlag;
                command.Parameters.Add("@RecallDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(info.RecallDate);
                command.Parameters.Add("@CreateUserID", SqlDbType.Int).Value = user.ID;
                command.Parameters.Add("@ResponseTimeLength", SqlDbType.Int).Value = info.ResponseTimeLength;
                command.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;

                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = info.FujiClass2.ID;
                command.Parameters.Add("@CTSerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.CTSerialCode);
                command.Parameters.Add("@CTUsedSeconds", SqlDbType.Decimal).Value = info.CTUsedSeconds;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新设备上次保养日期
        /// </summary>
        /// <param name="info">设备信息</param>
        public void UpdateEquipmentLastMaintenanceCheck(EquipmentInfo info)
        {
            sqlStr = "UPDATE tblEquipment SET LastMaintenanceDate = @LastMaintenanceDate WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@LastMaintenanceDate", SqlDbType.DateTime).Value = DateTime.Now;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新设备上次校准日期
        /// </summary>
        /// <param name="info">设备信息</param>
        public void UpdateEquipmentLastCorrectionCheck(EquipmentInfo info)
        {
            sqlStr = "UPDATE tblEquipment SET LastCorrectionDate = @LastCorrectionDate WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@LastCorrectionDate", SqlDbType.DateTime).Value = DateTime.Now;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新设备上次巡检日期
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="lastDate">上次巡检日期</param>
        public void UpdateEquipmentLastPatrolCheck(int id,DateTime lastDate)
        {
            sqlStr = "UPDATE tblEquipment SET LastPatrolDate = @LastPatrolDate WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@LastPatrolDate", SqlDbType.DateTime).Value = lastDate;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 检测序列号是否已存在
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="serialCode">设备序列号</param>
        /// <returns>序列号是否已存在</returns>
        public bool CheckSerialCode(int id, string serialCode)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblEquipment WHERE ID <> @ID AND UPPER(SerialCode) = @SerialCode ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@SerialCode", SqlDbType.VarChar).Value = serialCode.ToUpper();
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// 检测资产编号是否已存在
        /// </summary>
        /// <param name="id">设备</param>
        /// <param name="assetCode">资产编号</param>
        /// <returns>资产编号是否已存在</returns>
        public bool CheckAssetCode(int id, string assetCode)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblEquipment WHERE ID <> @ID AND UPPER(AssetCode) = @AssetCode ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@AssetCode", SqlDbType.VarChar).Value = assetCode.ToUpper();
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 根据设备编号获取生命周期信息
        /// </summary>
        /// <param name="equipmentID">设备ID</param>
        /// <returns>生命周期信息</returns>
        public List<DispatchInfo> GetTimeLine(int equipmentID)
        {
            List<DispatchInfo> dispatchInfo = new List<DispatchInfo>();

            sqlStr = "SELECT dr.SolutionWay, dr.Comments, d.* FROM tblRequest AS r " +
                     " LEFT JOIN jctRequestEqpt AS jc ON jc.RequestID = r.ID " +
                     " LEFT JOIN tblDispatch AS d ON d.RequestID = r.ID " +
                     " LEFT JOIN tblDispatchReport AS dr ON dr.DispatchID = d.ID " +
                     " WHERE jc.EquipmentID = @EquipmentID AND d.EndDate IS NOT NULL " +
                     " ORDER BY d.EndDate DESC, d.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DispatchInfo info = new DispatchInfo();
                        
                        info.ID = SQLUtil.ConvertInt(dr["ID"]);
                        info.Request.ID = SQLUtil.ConvertInt(dr["RequestID"]);
                        info.RequestType.ID = SQLUtil.ConvertInt(dr["RequestType"]);
                        info.RequestType.Name = LookupManager.GetRequestTypeDesc(info.RequestType.ID);
                        info.ScheduleDate = SQLUtil.ConvertDateTime(dr["ScheduleDate"]);
                        info.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
                        info.DispatchReport.SolutionWay = SQLUtil.TrimNull(dr["SolutionWay"]);
                        info.DispatchReport.Comments = SQLUtil.TrimNull(dr["Comments"]);
                        dispatchInfo.Add(info);
                    }
                }
            }

            return dispatchInfo;
        }
        /// <summary>
        /// 获取所有设备厂商信息
        /// </summary>
        /// <returns>所有设备厂商信息</returns>
        public List<SupplierInfo> QueryManufacturer()
        {
            List<SupplierInfo> infos = new List<SupplierInfo>();

            sqlStr = "SELECT s.* FROM tblEquipment e " +
                    " LEFT JOIN tblSupplier s ON e.ManufacturerID = s.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new SupplierInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion

        #region "EqptComponent"
        public List<EqptComponentInfo> GetComponentByEpqtID(int eqptID)
        {
            string getcomponentSql = "(SELECT ic.SerialCode,c.Name,c.Description,mh.UsedDate,mh.DispatchReportID,dr.DispatchID " +
                                    " FROM tblMaterialHistory mh " +
                                    " LEFT JOIN tblInvComponent ic ON mh.ObjectID = ic.ID " +
                                    " LEFT JOIN tblComponent c ON ic.ComponentID = c.ID " +
                                    " LEFT JOIN tblDispatchReport dr ON mh.DispatchReportID = dr.ID " +
                                    " WHERE mh.EquipmentID = @EqptID AND mh.ObjectType = {0}) ";
            sqlStr = "SELECT ISNULL(mh1.SerialCode,mh2.SerialCode) as SerialCode,ISNULL(mh1.Name,mh2.Name) as Name, ISNULL(mh1.Description,mh2.Description) as Description,mh1.UsedDate as InstalDate,mh2.UsedDate as RemoveDate,mh1.DispatchReportID as NewDispatchReportID,mh1.DispatchID as NewDispatchID,mh2.DispatchReportID as OldDispatchReportID,mh2.DispatchID as OldDispatchID " +
                    " FROM {0} as mh1 " +
                    " FULL JOIN {1} as mh2 ON mh1.SerialCode = mh2.SerialCode " +
                    " ORDER BY InstalDate DESC,RemoveDate DESC";

            sqlStr = string.Format(sqlStr, string.Format(getcomponentSql, ReportMaterialInfo.MaterialTypes.NewComponent), string.Format(getcomponentSql, ReportMaterialInfo.MaterialTypes.OldComponent));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EqptID", SqlDbType.Int).Value = eqptID;
                command.Parameters.Add("@NewComponent", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.NewComponent;
                command.Parameters.Add("@OldComponent", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.OldComponent;

                return GetList<EqptComponentInfo>(command); 
            }
        }
        #endregion

        #region "EqptConsumable"
        public List<EqptConsumableInfo> GetConsumableByEqptID(int eqptID)
        {
            sqlStr = "SELECT ic.LotNum,c.Name,c.Description,mh.Qty,mh.UsedDate,mh.DispatchReportID,dr.DispatchID " +
                    " FROM tblMaterialHistory mh " +
                    " LEFT JOIN tblInvConsumable ic ON ic.ID = mh.ObjectID " +
                    " LEFT JOIN tblConsumable c ON c.ID = ic.ConsumableID " +
                    " LEFT JOIN tblDispatchReport dr ON mh.DispatchReportID = dr.ID " +
                    " WHERE mh.EquipmentID = @EqptID AND mh.ObjectType = @Consumable";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EqptID", SqlDbType.Int).Value = eqptID;
                command.Parameters.Add("@Consumable", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.Consumable;

                return GetList<EqptConsumableInfo>(command);
            }
        }
        #endregion

        #region'tblEquipmentCtl'
        /// <summary>
        /// 获取下一个资产编号序号
        /// </summary>
        /// <param name="date">当前日期</param>
        /// <returns>下一个资产编号序号</returns>
        public string GetNextAssetCode(string date)
        {
            int seq = 1;

            sqlStr = "Select Seq From tblEquipmentCtl WHERE Date = @Date";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Date", SqlDbType.VarChar).Value = date;

                Object objSeq = command.ExecuteScalar();

                if (objSeq == DBNull.Value || objSeq == null)
                {
                    command.CommandText = "INSERT INTO tblEquipmentCtl (Date, Seq) VALUES (@Date, 1)";
                }
                else
                {
                    seq = SQLUtil.ConvertInt(objSeq) + 1;
                    command.CommandText = "UPDATE tblEquipmentCtl SET Seq = Seq + 1 WHERE Date = @Date";
                }

                command.ExecuteNonQuery();
            }

            return "HRY" + date + seq.ToString("D3");
        }
        #endregion

        #region"DashBoard"

        /// <summary>
        /// 获取未报废的设备总数量、采购金额总数
        /// </summary>
        /// <returns>设备总数量、采购金额总数</returns>
        public Tuple<int, double> GetEquipmentInfoCounts()
        {
            sqlStr = "SELECT Count(ID) AS Counts,SUM(PurchaseAmount) AS Amounts FROM tblEquipment WHERE EquipmentStatusID <> @EquipmentStatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.NVarChar).Value = EquipmentInfo.EquipmentStatuses.Scrap;

                DataRow dr = GetDataRow(command);

                return new Tuple<int, double>(SQLUtil.ConvertInt(dr["Counts"]), SQLUtil.ConvertDouble(dr["Amounts"]));
            }
        }

        /// <summary>
        /// 获取开机率 
        /// </summary>
        /// <returns>开机率</returns>
        public double GetEquipmentBootRate()
        {
            sqlStr = "SELECT SUM(CASE WHEN UsageStatusID=@Running THEN 1 ELSE 0 END )*1.0/COUNT(ID) FROM tblEquipment WHERE EquipmentStatusID <> @EquipmentStatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("Running", SqlDbType.Int).Value = EquipmentInfo.UsageStatuses.Running;
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.NVarChar).Value = EquipmentInfo.EquipmentStatuses.Scrap;

                DataRow dr = GetDataRow(command);

                return SQLUtil.ConvertDouble(dr[0]);
            }
        }

        /// <summary>
        /// 获取各科室未报废设备数量、金额
        /// </summary>
        /// <returns>各科室设备数量、金额</returns>
        public DataTable GetEquipmentInfoCountsByDepartment()
        {
            sqlStr = "SELECT DepartmentID, Count(ID) AS Counts,SUM(PurchaseAmount) AS Amounts FROM tblEquipment WHERE EquipmentStatusID <> @EquipmentStatusID " +
                    " GROUP BY DepartmentID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.NVarChar).Value = EquipmentInfo.EquipmentStatuses.Scrap;

                return GetDataTable(command);
            }
        }

        /// <summary>
        /// 获取折旧率(采购日期的下个月开始计算折旧率、若折旧率超过1 返回 1.否返回采购日期下月第一天与当前时间“月数”差值 比 预定折旧年限*12)
        /// </summary>
        /// <returns>折旧率</returns>
        public double GetEquipmentDepreciationRate()
        {
            string sqlDiffMonth = "(DATEDIFF(MONTH, PurchaseDate, GETDATE())-1)";

            sqlStr = " SELECT AVG(CASE WHEN  PurchaseDate IS NULL OR {0} <= 0 THEN 0.0 ELSE (CASE WHEN {0} >= (DepreciationYears*12) THEN 1.0 ELSE {0}*1.0/(DepreciationYears*12) END)END ) "+
                    " FROM tblEquipment WHERE EquipmentStatusID <> @EquipmentStatusID ";

            sqlStr = string.Format(sqlStr, sqlDiffMonth);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentStatusID", SqlDbType.NVarChar).Value = EquipmentInfo.EquipmentStatuses.Scrap;

                return SQLUtil.ConvertDouble(GetDataRow(command)[0]);
            }
        }
        /// <summary>
        /// 获取设备当年各个类型的请求数量
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="year">年份</param>
        /// <returns>设备当年各个类型的请求数量</returns>
        public Dictionary<int, int> GetRequestCountByID(int id,int year)
        {
            sqlStr = "SELECT r.RequestType, COUNT(r.ID) FROM tblRequest AS r " +
                     " LEFT JOIN jctRequestEqpt AS jc ON jc.RequestID = r.ID " +
                     " WHERE DATEPART(YEAR,r.RequestDate) = @Year AND jc.EquipmentID = @EquipmentID " +
                     " GROUP BY r.RequestType";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("Year", SqlDbType.Int).Value = year;
                command.Parameters.Add("EquipmentID", SqlDbType.Int).Value = id;

                return GetDictionary(command);
            }
        }

        #endregion
    }
}
