using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using BusinessObjects.DataAccess;
using System.Data;


namespace BusinessObjects.Manager
{
    /// <summary>
    /// 自定义报表manager
    /// </summary>
    public class CustomReportManager
    {
        /// <summary>
        /// customReportDao
        /// </summary>
        public CustomReportDao custRptDao = new CustomReportDao();
        /// <summary>
        /// userDao
        /// </summary>
        public UserDao userDao = new UserDao();
        /// <summary>
        /// 保存自定义报表信息
        /// </summary>
        /// <param name="info">自定义报表信息</param>
        /// <returns>自定义报表编号</returns>
        public int SaveCusRpt(CustomReportInfo info)
        {
            if (info.ID == 0)
            {
                info.ID = this.custRptDao.AddCustomRpt(info);
                foreach (CustRptFieldInfo field in info.Fields)
                {
                    this.custRptDao.AddCusrRptField(info.ID, field.TableName, field.FieldName);
                }
            }
            else
            {
                this.custRptDao.DeleteCustomRptFields(info.ID);

                foreach (CustRptFieldInfo field in info.Fields)
                {                    
                    this.custRptDao.AddCusrRptField(info.ID, field.TableName, field.FieldName);
                }
                this.custRptDao.UpdateCustRpt(info);
            }

            return info.ID;
        }

        /// <summary>
        /// 获取自定义报表类型为设备的报表信息
        /// </summary>
        /// <param name="info">报表内容</param>
        /// <param name="field">筛选时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <returns>报表内容</returns>
        public DataTable QueryCustRpt4Equipment(CustomReportInfo info,string field,DateTime startTime,DateTime endTime)
        {
            DataTable dtEquipment = this.custRptDao.QueryCustRpt4Equipment(field, startTime, endTime);
            List<string> fieldsInRpt = SQLUtil.GetStringListFromObjectList(info.Fields, "FieldName");

            foreach (DataRow dr in dtEquipment.Rows)
            {
                GetEquipmentDesc(dr, fieldsInRpt);
            }

            return SortColumn(info, dtEquipment);
        }

        /// <summary>
        /// 获取自定义报表类型为合同的报表信息
        /// </summary>
        /// <param name="info">报表内容</param>
        /// <param name="field">筛选时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <returns>报表内容</returns>
        public DataTable QueryCustRpt4Contract(CustomReportInfo info, string field, DateTime startTime, DateTime endTime)
        {
            DataTable dtContract = this.custRptDao.QueryCustRpt4Contract(field, startTime, endTime);
            List<string> fieldsInRpt = SQLUtil.GetStringListFromObjectList(info.Fields, "FieldName");
            foreach (DataRow dr in dtContract.Rows)
            {
                GetContractDesc(dr, fieldsInRpt);
                GetEquipmentDesc(dr, fieldsInRpt);
            }

            return SortColumn(info, dtContract);
        }

        /// <summary>
        /// 获取自定义报表类型为请求的报表信息
        /// </summary>
        /// <param name="info">报表内容</param>
        /// <param name="field">筛选时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <returns>报表内容</returns>
        public DataTable QueryCustRpt4Request(CustomReportInfo info, string field, DateTime startTime, DateTime endTime)
        {
            DataTable dtRequest = this.custRptDao.QueryCustRpt4Request(field, startTime, endTime);
            List<string> fieldsInRpt = SQLUtil.GetStringListFromObjectList(info.Fields, "FieldName");
            foreach (DataRow dr in dtRequest.Rows)
            {
                GetRequestDesc(dr, fieldsInRpt);
                GetEquipmentDesc(dr, fieldsInRpt);
            }

            return SortColumn(info, dtRequest);
        }

        /// <summary>
        /// 获取自定义报表类型为派工单的报表信息
        /// </summary>
        /// <param name="info">报表内容</param>
        /// <param name="field">筛选时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <returns>报表内容</returns>
        public DataTable QueryCustRpt4Dispatch(CustomReportInfo info, string field, DateTime startTime, DateTime endTime)
        {
            DataTable dtDispatch = this.custRptDao.QueryCustRpt4Dispatch(field, startTime, endTime);
            List<string> fieldsInRpt = SQLUtil.GetStringListFromObjectList(info.Fields, "FieldName");
            foreach (DataRow dr in dtDispatch.Rows)
            {
                GetDispatchDesc(dr, fieldsInRpt);
                GetDispatchJournalDesc(dr, fieldsInRpt);
                GetDispatchReportDesc(dr, fieldsInRpt);
                GetRequestDesc(dr, fieldsInRpt);
            }

            return SortColumn(info, dtDispatch);
        }
        
        /// <summary>
        /// 排序报表显示字段
        /// </summary>
        /// <param name="info">选取报表信息</param>
        /// <param name="dt">报表全部信息</param>
        /// <returns>排序后的报表</returns>
        public DataTable SortColumn(CustomReportInfo info, DataTable dt)
        {
            foreach (string columnName in SQLUtil.GetTableColumnNames(dt))
            {
                if (!info.ContainField(columnName))
                {
                    dt.Columns.Remove(columnName);
                }
            }

            List<CustRptTemplateFieldInfo> fields = this.custRptDao.GetCustRptTemplateFields(info.Type.ID);
            string fieldDesc = null;
            for (int i = 0; i < info.Fields.Count; i++)
            {
                dt.Columns[info.Fields[i].FieldName].SetOrdinal(i);

                fieldDesc = (from CustRptTemplateFieldInfo temp in fields where temp.FieldName.Equals(info.Fields[i].FieldName) select temp.FieldDesc).FirstOrDefault();
                if (!string.IsNullOrEmpty(fieldDesc)) dt.Columns[info.Fields[i].FieldName].ColumnName = fieldDesc;
            }

            if (info.ID > 0) this.custRptDao.UpdateCustRptRunDate(info.ID);

            return dt;
        }

        /// <summary>
        /// 获取设备报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetEquipmentDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (SQLUtil.ConvertInt(dr["EquipmentID"]) != 0)
            {
                if (fieldsInRpt.Contains("EquipmentLevelDesc")) dr["EquipmentLevelDesc"] = (dr["EquipmentLevel"] == null) ? "" : EquipmentInfo.EquipmentLevels.GetEquipmentLevelDesc(SQLUtil.ConvertInt(dr["EquipmentLevel"]));
                if (fieldsInRpt.Contains("ServiceScopeDesc")) dr["ServiceScopeDesc"] = SQLUtil.ConvertBoolean(dr["ServiceScope"]) ? "是" : "否";
                if (fieldsInRpt.Contains("EquipmentClass1Name")) dr["EquipmentClass1Name"] = (SQLUtil.TrimNull(dr["EquipmentClass1"]) == "00" || SQLUtil.TrimNull(dr["EquipmentClass1"]) == null) ? "" : LookupManager.GetEquipmentClassDesc(SQLUtil.TrimNull(dr["EquipmentClass1"]), 1);
                if (fieldsInRpt.Contains("EquipmentClass2Name")) dr["EquipmentClass2Name"] = (SQLUtil.TrimNull(dr["EquipmentClass2"]) == "00") ? "" : LookupManager.GetEquipmentClassDesc(SQLUtil.TrimNull(dr["EquipmentClass2"]), 2, SQLUtil.TrimNull(dr["EquipmentClass1"]));
                if (fieldsInRpt.Contains("EquipmentClass3Name")) dr["EquipmentClass3Name"] = (SQLUtil.TrimNull(dr["EquipmentClass3"]) == "00") ? "" : LookupManager.GetEquipmentClassDesc(SQLUtil.TrimNull(dr["EquipmentClass3"]), 3, SQLUtil.TrimNull(dr["EquipmentClass1"]) + SQLUtil.TrimNull(dr["EquipmentClass2"]));
                if (fieldsInRpt.Contains("DepartmentName")) dr["DepartmentName"] = LookupManager.GetDepartmentDesc(SQLUtil.ConvertInt(dr["DepartmentID"]));
                if (fieldsInRpt.Contains("UsageStatusDesc")) dr["UsageStatusDesc"] = LookupManager.GetUsageStatusDesc(SQLUtil.ConvertInt(dr["UsageStatusID"]));
                if (fieldsInRpt.Contains("EquipmentStatusDesc")) dr["EquipmentStatusDesc"] = LookupManager.GetEquipmentStatusDesc(SQLUtil.ConvertInt(dr["EquipmentStatusID"]));
                if (fieldsInRpt.Contains("MandatoryTestStatusDesc")) dr["MandatoryTestStatusDesc"] = EquipmentInfo.MandatoryTestStatuses.GetMandatoryTestStatusDesc(SQLUtil.ConvertInt(dr["MandatoryTestStatus"]));
                if (fieldsInRpt.Contains("MaintenanceTypeDesc")) dr["MaintenanceTypeDesc"] = LookupManager.GetPeriodTypeDesc(SQLUtil.ConvertInt(dr["MaintenanceTypeID"]));
                if (fieldsInRpt.Contains("PatrolTypeDesc")) dr["PatrolTypeDesc"] = LookupManager.GetPeriodTypeDesc(SQLUtil.ConvertInt(dr["PatrolTypeID"]));
                if (fieldsInRpt.Contains("CorrectionTypeDesc")) dr["CorrectionTypeDesc"] = LookupManager.GetPeriodTypeDesc(SQLUtil.ConvertInt(dr["CorrectionTypeID"]));
                if (fieldsInRpt.Contains("CreateUserName")) dr["CreateUserName"] = (SQLUtil.ConvertInt(dr["CreateUserID"]) == 0) ? "" : this.userDao.GetUser(SQLUtil.ConvertInt(dr["CreateUserID"])).Name;
                if (fieldsInRpt.Contains("WarrantyStatus")) dr["WarrantyStatus"] = SQLUtil.ConvertInt(dr["WarrantyStatusID"]) == 0 ? "保外" : "保内";
            }
            
            return dr;
        }
        
        /// <summary>
        /// 获取请求报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetRequestDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (SQLUtil.ConvertInt(dr["RequestID"]) != 0)
            {
                if (fieldsInRpt.Contains("RequestTypeDesc")) dr["RequestTypeDesc"] = LookupManager.GetRequestTypeDesc(SQLUtil.ConvertInt(dr["RequestType"]));
                if (fieldsInRpt.Contains("SourceDesc")) dr["SourceDesc"] = RequestInfo.Sources.GetSourceDesc(SQLUtil.ConvertInt(dr["Source"]));
                if (fieldsInRpt.Contains("FaultTypeDesc")) dr["FaultTypeDesc"] = RequestInfo.GetDescFaultType(SQLUtil.ConvertInt(dr["RequestType"]), SQLUtil.ConvertInt(dr["FaultTypeID"]));
                if (fieldsInRpt.Contains("RequestStatusID")) dr["RequestStatusID"] = LookupManager.GetRequestStatusDesc(SQLUtil.ConvertInt(dr["StatusID"]));
                if (fieldsInRpt.Contains("DealTypeDesc")) dr["DealTypeDesc"] = LookupManager.GetDealTypeDesc(SQLUtil.ConvertInt(dr["DealTypeID"]));
                if (fieldsInRpt.Contains("PriorityDesc")) dr["PriorityDesc"] = LookupManager.GetUrgencyDesc(SQLUtil.ConvertInt(dr["PriorityID"]));
                if (fieldsInRpt.Contains("IsRecallDesc")) dr["IsRecallDesc"] = SQLUtil.ConvertBoolean(dr["RequestIsRecall"]) ? "是" : "否";
            }            

            return dr;
        }

        /// <summary>
        /// 获取派工单报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetDispatchDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (SQLUtil.ConvertInt(dr["DisID"]) != 0)
            {
                if (fieldsInRpt.Contains("DispatchTypeDesc")) dr["DispatchTypeDesc"] = LookupManager.GetRequestTypeDesc(SQLUtil.ConvertInt(dr["DispatchRequestType"]));
                if (fieldsInRpt.Contains("UrgencyDesc")) dr["UrgencyDesc"] = LookupManager.GetUrgencyDesc(SQLUtil.ConvertInt(dr["UrgencyID"]));
                if (fieldsInRpt.Contains("MachineStatusDesc")) dr["MachineStatusDesc"] = MachineStatuses.GetMachineStatusesDesc(SQLUtil.ConvertInt(dr["EquipmentStatus"]));
                if (fieldsInRpt.Contains("EngineerName")) dr["EngineerName"] = (SQLUtil.ConvertInt(dr["EngineerID"]) == 0) ? "" : (this.userDao.GetUser(SQLUtil.ConvertInt(dr["EngineerID"])) == null ? "" : this.userDao.GetUser(SQLUtil.ConvertInt(dr["EngineerID"])).Name);
                if (fieldsInRpt.Contains("DispatchStatusDesc")) dr["DispatchStatusDesc"] = LookupManager.GetDispatchStatusDesc(SQLUtil.ConvertInt(dr["DispatchStatusID"]));
            }            

            return dr;
        }

        /// <summary>
        /// 获取作业报告报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetDispatchReportDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (SQLUtil.ConvertInt(dr["ReportID"]) != 0)
            {
                if (fieldsInRpt.Contains("ReportTypeDesc")) dr["ReportTypeDesc"] = LookupManager.GetDispatchReportType(SQLUtil.ConvertInt(dr["TypeID"])).Name;
                if (fieldsInRpt.Contains("ReportStatusDesc")) dr["ReportStatusDesc"] = LookupManager.GetDispatchDocStatusDesc(SQLUtil.ConvertInt(dr["DispatchReportStatusID"]));
                if (fieldsInRpt.Contains("ServiceProviderDesc")) dr["ServiceProviderDesc"] = DispatchReportInfo.ServiceProviders.GetDescByID(SQLUtil.ConvertInt(dr["ServiceProvider"]));
                if (fieldsInRpt.Contains("ReportEquipmentStatusDesc")) dr["ReportEquipmentStatusDesc"] = LookupManager.GetEquipmentStatusDesc(SQLUtil.ConvertInt(dr["reportEquipmentStatus"]));
                if (fieldsInRpt.Contains("ServiceScopeDesc")) dr["ServiceScopeDesc"] = SQLUtil.ConvertBoolean(dr["ServiceScope"]) ? "是" : "否";
                if (fieldsInRpt.Contains("SolutionResultStatusDesc")) dr["SolutionResultStatusDesc"] = LookupManager.GetSolutionResultStatusDesc(SQLUtil.ConvertInt(dr["SolutionResultStatusID"]));
                if (fieldsInRpt.Contains("ReportIsRecallDesc")) dr["ReportIsRecallDesc"] = SQLUtil.ConvertBoolean(dr["ReportIsRecall"]) ? "是" : "否";


                if (fieldsInRpt.Contains("IsPrivateDesc")) dr["IsPrivateDesc"] = SQLUtil.ConvertBoolean(dr["IsPrivate"]) ? "是" : "否";
            }
            return dr;
        }

        /// <summary>
        /// 获取服务凭证报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetDispatchJournalDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (SQLUtil.ConvertInt(dr["JournalID"]) != 0)
            {
                if (fieldsInRpt.Contains("ResultStatusDesc")) dr["ResultStatusDesc"] = LookupManager.GetDispatchJournalResultStatusDesc(SQLUtil.ConvertInt(dr["ResultStatusID"]));
                if (fieldsInRpt.Contains("SignedDesc")) dr["SignedDesc"] = (SQLUtil.ConvertInt(dr["JournalID"]) > 0) ? (SQLUtil.ConvertBoolean(dr["Signed"]) ? "是" : "否") : "";
                if (fieldsInRpt.Contains("JournalStatusDesc")) dr["JournalStatusDesc"] = LookupManager.GetDispatchDocStatusDesc(SQLUtil.ConvertInt(dr["DispatchJournalStatusID"]));
            }
            

            return dr;
        }

        /// <summary>
        /// 获取合同报表的部分字段描述信息
        /// </summary>
        /// <param name="dr">报表所有字段信息</param>
        /// <param name="fieldsInRpt">选取的报表字段</param>
        /// <returns>报表信息</returns>
        public DataRow GetContractDesc(DataRow dr, List<string> fieldsInRpt)
        {
            if (fieldsInRpt.Contains("TypeDesc")) dr["TypeDesc"] = LookupManager.GetContractTypeDesc(SQLUtil.ConvertInt(dr["TypeID"]));
            if (fieldsInRpt.Contains("ScopeDesc")) dr["ScopeDesc"] = LookupManager.GetContractScopeDesc(SQLUtil.ConvertInt(dr["ScopeID"]));
            if (fieldsInRpt.Contains("SupplierType")) dr["SupplierType"] = LookupManager.GetSupplierTypeDesc(SQLUtil.ConvertInt(dr["SupplierTypeID"]));
            if (fieldsInRpt.Contains("SupplierStatus")) dr["SupplierStatus"] = SQLUtil.ConvertBoolean(dr["SupplierStatusID"]) ? "启用" : "停用";

            return dr;
        }
    }
}
