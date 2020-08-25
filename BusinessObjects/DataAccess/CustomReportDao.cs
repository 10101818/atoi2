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
    /// 自定义报表dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class CustomReportDao : BaseDao
    {
        /// <summary>
        /// 获取自定义报表数量
        /// </summary>
        /// <param name="typeID">自定义报表类型ID</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>自定义报表数量</returns>
        public int QueryCustRptCount(int typeID, string filterField, string filterText)
        {
            sqlStr = " SELECT COUNT(ID) FROM tblCustomReport" +
                " WHERE 1=1 ";
            if (typeID > 0)
                sqlStr += " AND TypeID = " + typeID;
            if (!string.IsNullOrEmpty(filterText))
            {
                sqlStr += GetFieldFilterClause(filterField);
            }
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取自定义报表信息
        /// </summary>
        /// <param name="typeID">自定义报表类型</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>自定义报表信息</returns>
        public List<CustomReportInfo> QueryCustRptsList(int typeID, string filterField, string filterText, int curRowNum = 0, int pageSize = 0)
        {
            List<CustomReportInfo> infos = new List<CustomReportInfo>();
            sqlStr = " SELECT * FROM tblCustomReport" +
                " WHERE 1=1";
            if (typeID > 0)
                sqlStr += " AND TypeID = " + typeID;
            if (!string.IsNullOrEmpty(filterText))
            {
                sqlStr += GetFieldFilterClause(filterField);
            }
            sqlStr += " ORDER BY ID ";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new CustomReportInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取设备自定义报表信息
        /// </summary>
        /// <param name="field">搜索时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>设备自定义报表信息</returns>
        public DataTable QueryCustRpt4Equipment(string field ,DateTime startTime, DateTime endTime)
        {
            sqlStr = "SELECT e.*,e.Comments as EquipmentComments,e.Name as EquipmentName, e.ID as EquipmentID, e.PurchaseAmount as EquipmentPurchaseAmount, f.Name AS EquipmentFujiClass2Name, '' as EquipmentLevelDesc, '' as EquipmentClass1Name,'' as EquipmentClass2Name,'' as EquipmentClass3Name,'' as DepartmentName,'' as UsageStatusDesc, '' as ServiceScopeDesc, " +
                " '' as EquipmentStatusDesc,'' as MaintenanceTypeDesc,'' as PatrolTypeDesc,'' as CorrectionTypeDesc,'' as CreateUserName,'' as MandatoryTestStatusDesc , '' as IsActiveDesc, '' as WarrantyStatus," +
                " su.Name as ManufacturerName,su.Province as ManufacturerProvince,su.Mobile as ManufacturerMobile,su.Address as ManufacturerAddress,su.Contact as ManufacturerContact,su.ContactMobile as ManufacturerContactMobile, " +
                " s.Name as DealerName,s.Province as DealerProvince,s.Mobile as DealerMobile,s.Address as DealerAddress,s.Contact as DealerContact,s.ContactMobile as DealerContactMobile, " +
                " c.ContractID as WarrantyStatusID " +
                " FROM tblEquipment AS e " +
                " LEFT JOIN tblSupplier AS s ON e.SupplierID=s.ID " +
                " LEFT JOIN tblSupplier AS su ON e.ManufacturerID=su.ID " +
                " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                " LEFT JOIN tblFujiClass2 f ON e.FujiClass2ID = f.ID " +
                " WHERE " + field + " >= '" + startTime +
                " ' AND " + field + " <= ' " + endTime + "' " +
                " ORDER BY e.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        /// <summary>
        /// 获取合同自定义报表信息
        /// </summary>
        /// <param name="field">搜索时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>合同自定义报表信息</returns>
        public DataTable QueryCustRpt4Contract(string field,DateTime startTime, DateTime endTime)
        {
            sqlStr = "SELECT c.Name ContractName , c.StartDate ContractStartDate,c.EndDate ContractEndDate, c.Comments ContractComments, " +
                    " c.TypeID,c.ContractNum,c.ScopeID,c.Amount,c.ProjectNum, '' TypeDesc, '' ScopeDesc,c.ScopeComments, " +
                    " e.*, e.ID EquipmentID, e.Name EquipmentName, e.EquipmentCode,e.SerialCode,su.Name EquipmentManufacturerName, " +
                    " '' EquipmentLevelDesc, '' EquipmentClass1Name, '' EquipmentClass2Name, '' EquipmentClass3Name, '' ServiceScopeDesc,  " +
                    " '' DepartmentName, " +
                    " s.Name SupplierName,s.Province SupplierProvince,s.Mobile SupplierMobile,s.Address SupplierAddress,s.Contact SupplierContact,s.ContactMobile SupplierContactMobile, " +
                    " s.TypeID SupplierTypeID, '' SupplierType, s.IsActive SupplierStatusID, '' SupplierStatus, " +
                    " vc.ContractID WarrantyStatusID  " +
                    " FROM tblContract c  " +
                    " LEFT JOIN jctContractEqpt ce on ce.ContractID = c.ID " +
                    " LEFT JOIN tblEquipment e on e.ID = ce.EquipmentID " +
                    " LEFT JOIN tblSupplier s on s.ID = c.SupplierID " +
                    " LEFT JOIN tblSupplier su ON su.ID = e.ManufacturerID " +
                    " LEFT JOIN v_ActiveContract vc on vc.EquipmentID = e.ID " + 
                    " WHERE " + field + " >= '" + startTime +
                    " ' AND " + field + " <= ' " + endTime + "' " +
                    " ORDER BY c.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        /// <summary>
        /// 获取请求自定义报表信息
        /// </summary>
        /// <param name="field">搜索时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>请求自定义报表信息</returns>
        public DataTable QueryCustRpt4Request(string field,DateTime startTime, DateTime endTime)
        {
            sqlStr = "SELECT r.*, r.ID RequestID, r.FaultDesc RequestFaultDesc, r.IsRecall RequestIsRecall,r.StatusID RequestStatusID, " +
                    " '' RequestTypeDesc,'' RequestUserName,'' RequestUserMobile,'' SourceDesc,'' FaultTypeDesc,'' RequestStatusDesc,'' DealTypeDesc,'' PriorityDesc,'' IsRecallDesc, " +
                    " e.*, e.ID EquipmentID,e.Name EquipmentName,su.Name EquipmentManufacturerName, " +
                    " '' EquipmentLevelDesc,'' EquipmentClass1Name,'' EquipmentClass2Name,'' EquipmentClass3Name,'' DepartmentName,'' ServiceScopeDesc, " +
                    " su.Name ManufacturerName,su.Province ManufacturerProvince,su.Mobile ManufacturerMobile,su.Address ManufacturerAddress,su.Contact ManufacturerContact,su.ContactMobile ManufacturerContactMobile, " +
                    " c.ContractID WarrantyStatusID " +
                    " FROM tblRequest r " +
                    " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " LEFT JOIN tblSupplier su ON e.ManufacturerID = su.ID " +
                    " LEFT JOIN v_ActiveContract  c on c.EquipmentID = e.ID  " + 
                    " WHERE " + field + " >= '" + startTime +
                    " ' AND " + field + " <= ' " + endTime + "' " +
                    " ORDER BY r.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        /// <summary>
        /// 获取派工单自定义报表信息
        /// </summary>
        /// <param name="field">搜索时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>派工单自定义报表信息</returns>
        public DataTable QueryCustRpt4Dispatch(string field,DateTime startTime, DateTime endTime)
        {
            sqlStr = "SELECT d.*, d.ID  DisID,d.RequestType DispatchRequestType,d.StatusID DispatchStatusID, " +
                    " '' DispatchTypeDesc,'' UrgencyDesc,'' MachineStatusDesc,'' EngineerName,'' DispatchStatusDesc, " +
                    " dr.*,dr.ID ReportID,dr.FaultCode DispatchReportFaultCode,dr.FaultDesc DispatchReportFaultDesc,dr.Comments ReportComments, dr.FujiComments ReportFujiComments, " +
                    " dr.EquipmentStatus reportEquipmentStatus, dr.IsRecall ReportIsRecall, " +
                    " '' ReportIsRecallDesc,'' ReportTypeDesc,''  ServiceProviderDesc, '' ReportEquipmentStatusDesc,'' ServiceScopeDesc, '' SolutionResultStatusDesc, " +
                    " '' SolutionResultStatusDesc,'' ReportStatusDesc, '' IsPrivateDesc, " +
                    " dj.*,dj.ID JournalID, dj.FujiComments JournalFujiComments, dj.StatusID DispatchJournalStatusID, " +
                    " '' ResultStatusDesc,'' SignedDesc,'' JournalStatusDesc, " +
                    " r.*, r.ID RequestID, r.IsRecall RequestIsRecall, r.StatusID RequestStatusID,r.FaultDesc RequestFaultDesc, " +
                    " '' RequestTypeDesc,'' FaultTypeDesc,'' RequestStatusDesc,'' DealTypeDesc,'' PriorityDesc,'' IsRecallDesc, '' SourceDesc " +
                    " FROM tblDispatch d " +
                    " LEFT JOIN tblDispatchJournal dj ON dj.DispatchID = d.ID " +
                    " LEFT JOIN tblDispatchReport dr ON dr.DispatchID = d.ID " +
                    " LEFT JOIN tblRequest r ON d.RequestID = r.ID " +
                    " WHERE " + field + " >= '" + startTime +
                    " ' AND " + field + " <= ' " + endTime + "' " +
                    " ORDER BY d.ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        /// <summary>
        /// 根据id获取自定义报表内容
        /// </summary>
        /// <param name="id">报表id</param>
        /// <returns>自定义报表内容</returns>
        public CustomReportInfo GetReportByID(int id)
        {
            CustomReportInfo info = null;
            sqlStr = "SELECT cr.*,crf.* FROM tblCustomReport cr" +
                " LEFT JOIN tblCustRptField crf ON cr.ID = crf.CustomReportID " +
                " WHERE cr.ID = " + id;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (info == null) info = new CustomReportInfo(dr);
                        info.Fields.Add(new CustRptFieldInfo(dr));
                    }
                }
            }
            return info;
        }
        /// <summary>
        /// 新增自定义报表
        /// </summary>
        /// <param name="info">自定义报表信息</param>
        /// <returns>自定义报表ID</returns>
        public int AddCustomRpt(CustomReportInfo info)
        {
            sqlStr = " INSERT INTO tblCustomReport(TypeID,Name,CreateUserID,CreateUserName,CreatedDate)" +
                " VALUES (@TypeID,@Name,@CreateUserID,@CreateUserName,GETDATE())" +
                " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = info.Type.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@CreateUserID", SqlDbType.Int).Value = info.CreateUser.ID;
                command.Parameters.Add("@CreateUserName", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.CreateUser.Name);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info.ID;
        }
        /// <summary>
        /// 更新自定义报表信息
        /// </summary>
        /// <param name="info">自定义报表信息</param>
        public void UpdateCustRpt(CustomReportInfo info)
        {
            sqlStr = " UPDATE tblCustomReport " +
                " SET Name=@Name,UpdateDate=@UpdateDate" +
                " WHERE ID=@ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新自定义报表上次运行日期
        /// </summary>
        /// <param name="id">自定义报表ID</param>
        public void UpdateCustRptRunDate(int id)
        {
            sqlStr = " UPDATE tblCustomReport " +
                " SET LastRunDate=@LastRunDate" +
                " WHERE ID=@ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@LastRunDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 新增自定义报表选择字段
        /// </summary>
        /// <param name="customRptID">自定义报表ID</param>
        /// <param name="tableName">自定义报表表名</param>
        /// <param name="fieldName">自定义报表字段名</param>
        public void AddCusrRptField(int customRptID, string tableName, string fieldName)
        {
            sqlStr = " INSERT INTO tblCustRptField(CustomReportID,TableName,FieldName)" +
                " VALUES(@CustomReportID,@TableName,@FieldName)";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@CustomReportID", SqlDbType.Int).Value = customRptID;
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = tableName;
                command.Parameters.Add("@FieldName", SqlDbType.NVarChar).Value = fieldName;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除选择的自定义报表字段
        /// </summary>
        /// <param name="id">自定义报表ID</param>
        public void DeleteCustomRptFields(int id)
        {
            sqlStr = " DELETE FROM tblCustRptField WHERE CustomReportID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除自定义报表
        /// </summary>
        /// <param name="id">自定义报表ID</param>
        public void DeleteCusRpt(int id)
        {
            {
                sqlStr = " DELETE FROM tblCustomReport WHERE ID = @ID";

                using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
                {
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// 根据自定义报表类型获取自定义报表模板表
        /// </summary>
        /// <param name="typeId">自定义报表类型</param>
        /// <returns>自定义报表模板表</returns>
        public List<CustRptTemplateTableInfo> GetCustRptTemplateTables(int typeId)
        {
            List<CustRptTemplateTableInfo> infos = new List<CustRptTemplateTableInfo>();

            sqlStr = "SELECT * FROM lkpCustRptTemplateTable Where TypeID = @TypeID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = typeId;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new CustRptTemplateTableInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据自定义报表类型获取自定义报表模板字段
        /// </summary>
        /// <param name="typeId">自定义报表类型</param>
        /// <returns>自定义报表模板字段</returns>
        public List<CustRptTemplateFieldInfo> GetCustRptTemplateFields(int typeId)
        {
            List<CustRptTemplateFieldInfo> infos = new List<CustRptTemplateFieldInfo>();

            sqlStr = "SELECT * FROM lkpCustRptTemplateField Where TypeID = @TypeID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = typeId;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new CustRptTemplateFieldInfo(dr));
                    }
                }
            }

            return infos;
        }

    }
}
