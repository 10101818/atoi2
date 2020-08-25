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
    /// 派工单dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class DispatchDao:BaseDao
    {
        /// <summary>
        /// 新增派工单
        /// </summary>
        /// <param name="info">派工单信息</param>
        /// <returns>派工单信息</returns>
        public DispatchInfo AddDispatch(DispatchInfo info)
        {
            sqlStr = "INSERT INTO tblDispatch(RequestID,RequestType,UrgencyID,EquipmentStatus,EngineerID," +
                     " ScheduleDate,LeaderComments,StatusID,CreateDate) " +
                     " VALUES(@RequestID,@RequestType,@UrgencyID,@EquipmentStatus,@EngineerID," +
                     " @ScheduleDate,@LeaderComments,@StatusID,GETDATE()); " +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = info.Request.ID;
                command.Parameters.Add("@RequestType", SqlDbType.Int).Value = info.RequestType.ID;
                command.Parameters.Add("@UrgencyID", SqlDbType.Int).Value = SQLUtil.ZeroToNull(info.Urgency.ID);
                command.Parameters.Add("@EquipmentStatus", SqlDbType.Int).Value = info.MachineStatus.ID;
                command.Parameters.Add("@EngineerID", SqlDbType.Int).Value = info.Engineer.ID;
                command.Parameters.Add("@ScheduleDate", SqlDbType.DateTime).Value = info.ScheduleDate;
                command.Parameters.Add("@LeaderComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.LeaderComments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = info.Status.ID;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info;
        }
        /// <summary>
        /// 获取派工单列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userRoleID">用户角色ID</param>
        /// <param name="statusList">状态ID</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>派工单列表</returns>
        public List<DispatchInfo> QueryDispatches(int userID,int userRoleID, List<int> statusList, int urgency, int type, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            List<DispatchInfo> dispatches = new List<DispatchInfo>();
            sqlStr = "SELECT DISTINCT d.*, CONVERT(VARCHAR(10),d.CreateDate,112),j.ID as DispatchJournalID,dr.ID as DispatchReportID , j.StatusID AS DispatchJournalStatusID,dr.StatusID AS DispatchReportStatusID, " + DispatchReportInfo.GetOverDueSQL() + string.Format(", CASE WHEN d.StatusID = {0} THEN -1 ELSE d.StatusID END AS newStatusID ", DispatchInfo.Statuses.Responded) +
                " FROM tblDispatch d " + 
                " LEFT JOIN tblDispatchJournal  j ON d.ID = j.DispatchID " +
                " LEFT JOIN tblDispatchReport dr ON d.ID = dr.DispatchID " +
                " LEFT JOIN tblRequest as r on r.ID = d.RequestID " +
                " LEFT JOIN jctRequestEqpt jc ON jc.RequestID=r.ID  " +
                " LEFT JOIN tblEquipment e ON e.ID=jc.EquipmentID " +
                " WHERE 1=1 ";
            if (statusList != null && statusList.Count > 1) sqlStr += " AND d.StatusID IN (" + SQLUtil.ConvertToInStr(statusList) + ")";
            else if (statusList != null && statusList.Count == 1 && statusList[0] != 0) sqlStr += " AND d.StatusID IN (" + SQLUtil.ConvertToInStr(statusList) + ")";
            else sqlStr += " AND d.StatusID <> " + DispatchInfo.Statuses.Cancelled;
            
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;
            if (type != 0) sqlStr += " AND d.RequestType=" + type;
            if (userRoleID == BusinessObjects.Domain.UserRole.Admin) sqlStr += " AND d.EngineerID=" + userID;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            if (sortField.Equals("init"))
            {
                if (userRoleID == BusinessObjects.Domain.UserRole.Admin)
                    sqlStr += string.Format(" ORDER BY newStatusID, d.RequestType, CONVERT(VARCHAR(10),d.CreateDate,112) DESC , d.ID ", DispatchInfo.Statuses.Responded, DispatchReportInfo.GetOverDueSQL());
                else
                    sqlStr += string.Format(" ORDER BY {0} DESC, d.RequestType, CONVERT(VARCHAR(10),d.CreateDate,112) DESC, d.StatusID, d.ID ", DispatchReportInfo.GetOverDueSQL());
            }
            else
                sqlStr += GenerateSortClause(sortDirection, sortField, "d.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatches.Add(new DispatchInfo(dr));
                    }
                }
            }
            return dispatches;
        }
        /// <summary>
        /// 获取派工单数量
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userRoleID">用户角色ID</param>
        /// <param name="status">派工单状态ID</param>
        /// <param name="urgency">派工紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>派工单数量</returns>
        public int QueryDispatchesCount(int userID,int userRoleID,int status, int urgency, int type, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(d.ID) FROM tblDispatch d " +
                    " LEFT JOIN tblDispatchJournal  j ON d.ID = j.DispatchID " +
                    " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                    " WHERE 1=1 ";

            if (status != 0) sqlStr += " AND d.StatusID=" + status;
            else sqlStr += " AND d.StatusID <> " + DispatchInfo.Statuses.Cancelled;
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;
            if (type != 0) sqlStr += " AND d.RequestType=" + type;
            if (userRoleID == BusinessObjects.Domain.UserRole.Admin) sqlStr += " AND d.EngineerID=" + userID;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取作业报告列表
        /// </summary>
        /// <param name="status">作业报告状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>作业报告列表</returns>
        public List<DispatchReportInfo> QueryDispatchReports(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            List<DispatchReportInfo> dispatches = new List<DispatchReportInfo>();
            sqlStr = "SELECT DISTINCT d.*,r.ID AS DispatchReportID , r.StatusID AS DispatchReportStatusID ,j.ID as DispatchJournalID, j.StatusID AS DispatchJournalStatusID " +
                " FROM tblDispatchReport r " +
                " LEFT JOIN tblDispatch d ON d.ID = r.DispatchID " +
                " LEFT JOIN tblDispatchJournal j ON d.ID = j.DispatchID " +
                " WHERE 1=1 ";
            if (status != 0) sqlStr += " AND r.StatusID=" + status;
            else sqlStr += " AND r.StatusID <> " + DispatchReportInfo.DispatchReportStatus.Cancelled;
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "r.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DispatchReportInfo dispatchReport = new DispatchReportInfo();
                        dispatchReport.ID = SQLUtil.ConvertInt(dr["DispatchReportID"]);
                        dispatchReport.Status.ID = SQLUtil.ConvertInt(dr["DispatchReportStatusID"]);
                        dispatchReport.Status.Name = LookupManager.GetDispatchDocStatusDesc(dispatchReport.Status.ID);

                        dispatchReport.Dispatch.ID = SQLUtil.ConvertInt(dr["ID"]);
                        dispatchReport.Dispatch.Request.ID = SQLUtil.ConvertInt(dr["RequestID"]);
                        dispatchReport.Dispatch.RequestType.ID = SQLUtil.ConvertInt(dr["RequestType"]);
                        dispatchReport.Dispatch.RequestType.Name = LookupManager.GetRequestTypeDesc(dispatchReport.Dispatch.RequestType.ID);
                        dispatchReport.Dispatch.Urgency.ID = SQLUtil.ConvertInt(dr["UrgencyID"]);
                        dispatchReport.Dispatch.Urgency.Name = LookupManager.GetUrgencyDesc(dispatchReport.Dispatch.Urgency.ID);
                        dispatchReport.Dispatch.ScheduleDate = SQLUtil.ConvertDateTime(dr["ScheduleDate"]); 
                        dispatchReport.Dispatch.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
                        dispatches.Add(dispatchReport);
                    }
                }
            }
            return dispatches;
        }
        /// <summary>
        /// 获取作业报告数量
        /// </summary>
        /// <param name="status">作业报告状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>作业报告数量</returns>
        public int QueryDispatchReportsCount(int status, int urgency, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(r.ID) FROM tblDispatchReport r " +
                    " LEFT JOIN tblDispatch d ON d.ID = r.DispatchID " +
                    " LEFT JOIN tblDispatchJournal j ON d.ID = j.DispatchID " +
                    " WHERE 1=1 ";

            if (status != 0) sqlStr += " AND r.StatusID= " + status;
            else sqlStr += " AND r.StatusID <> " + DispatchReportInfo.DispatchReportStatus.Cancelled;
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取服务凭证列表
        /// </summary>
        /// <param name="status">服务凭证状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>服务凭证列表</returns>
        public List<DispatchJournalInfo> QueryDispatchJournals(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            List<DispatchJournalInfo> dispatchJournals = new List<DispatchJournalInfo>();
            sqlStr = "SELECT DISTINCT d.*,j.ID as DispatchJournalID, j.StatusID AS DispatchJournalStatusID ,r.ID AS DispatchReportID , r.StatusID AS DispatchReportStatusID " +
                " FROM tblDispatchJournal j " +
                " LEFT JOIN tblDispatch d ON d.ID = j.DispatchID " +
                " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                " WHERE 1=1 ";
            if (status != 0) sqlStr += " AND j.StatusID=" + status;
            else sqlStr += " AND j.StatusID <> " + DispatchJournalInfo.DispatchJournalStatus.Cancelled;
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "j.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DispatchJournalInfo dispatchJournal = new DispatchJournalInfo();
                        dispatchJournal.ID = SQLUtil.ConvertInt(dr["DispatchJournalID"]);
                        dispatchJournal.Status.ID = SQLUtil.ConvertInt(dr["DispatchJournalStatusID"]);
                        dispatchJournal.Status.Name = LookupManager.GetDispatchDocStatusDesc(dispatchJournal.Status.ID);

                        dispatchJournal.Dispatch.ID = SQLUtil.ConvertInt(dr["ID"]);
                        dispatchJournal.Dispatch.Request.ID = SQLUtil.ConvertInt(dr["RequestID"]);
                        dispatchJournal.Dispatch.RequestType.ID = SQLUtil.ConvertInt(dr["RequestType"]);
                        dispatchJournal.Dispatch.RequestType.Name = LookupManager.GetRequestTypeDesc(dispatchJournal.Dispatch.RequestType.ID);
                        dispatchJournal.Dispatch.Urgency.ID = SQLUtil.ConvertInt(dr["UrgencyID"]);
                        dispatchJournal.Dispatch.Urgency.Name = LookupManager.GetUrgencyDesc(dispatchJournal.Dispatch.Urgency.ID);
                        dispatchJournal.Dispatch.ScheduleDate = SQLUtil.ConvertDateTime(dr["ScheduleDate"]); 
                        dispatchJournal.Dispatch.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);

                        dispatchJournals.Add(dispatchJournal);
                    }
                }
            }
            return dispatchJournals;
        }
        /// <summary>
        /// 获取服务凭证数量
        /// </summary>
        /// <param name="status">服务凭证状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>服务凭证数量</returns>
        public int QueryDispatchJournalsCount(int status, int urgency, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(j.ID) FROM tblDispatchJournal j " +
                    " LEFT JOIN tblDispatch d ON d.ID = j.DispatchID " +
                    " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                    " WHERE 1=1 ";
            if (status != 0) sqlStr += " AND j.StatusID= " + status;
            else sqlStr += " AND j.StatusID <> " + DispatchJournalInfo.DispatchJournalStatus.Cancelled;
            if (urgency != 0) sqlStr += " AND d.UrgencyID=" + urgency;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 根据请求id获取最新派工单信息
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <returns>最新派工单信息</returns>
        public DispatchInfo GetDispatchByRequestID(int requestID)
        {
            sqlStr = "SELECT top 1 d.*,j.ID as DispatchJournalID,r.ID as DispatchReportID ," +
                     " j.StatusID AS DispatchJournalStatusID,r.StatusID AS DispatchReportStatusID FROM tblDispatch d " +
                     " LEFT JOIN tblDispatchJournal j ON d.ID = j.DispatchID " +
                     " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                     " WHERE d.RequestID = @RequestID " +
                     " ORDER BY d.ID DESC ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = requestID;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DispatchInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据派工单ID获取派工单信息
        /// </summary>
        /// <param name="id">派工单ID</param>
        /// <returns>派工单信息</returns>
        public DispatchInfo GetDispatchByID(int id)
        {
            sqlStr = "SELECT DISTINCT d.*,j.ID as DispatchJournalID,r.ID as DispatchReportID ," +
                " j.StatusID AS DispatchJournalStatusID,r.StatusID AS DispatchReportStatusID FROM tblDispatch d " +
                " LEFT JOIN tblDispatchJournal j ON d.ID = j.DispatchID " +
                " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                " WHERE d.ID = @ID ;";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DispatchInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据用户获取派工单各状态数量
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roleID">角色ID</param>
        /// <returns>派工单各状态数量</returns>
        public Dictionary<int, int> GetDispatchCount(int userID,int roleID)
        {
            sqlStr = "SELECT StatusID, Count(ID) " +
                " FROM tblDispatch " +
                " WHERE 1=1 ";
            if (roleID == BusinessObjects.Domain.UserRole.Admin)
                sqlStr += " AND EngineerID = " + userID;
             sqlStr += " GROUP BY StatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDictionary(command);
            }
        }
        /// <summary>
        /// 获取各状态作业报告数量
        /// </summary>
        /// <returns>各状态作业报告数量</returns>
        public Dictionary<int, int> GetDispatchReportCount()
        {
            sqlStr = "SELECT StatusID, Count(ID) FROM tblDispatchReport GROUP BY StatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDictionary(command);
            }
        }
        /// <summary>
        /// 获取各状态服务凭证数量
        /// </summary>
        /// <returns>获取各状态服务凭证数量</returns>
        public Dictionary<int, int> GetDispatchJournalCount()
        {
            sqlStr = "SELECT StatusID, Count(ID) FROM tblDispatchJournal GROUP BY StatusID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDictionary(command);
            }
        }
        /// <summary>
        /// 更新派工单状态
        /// </summary>
        /// <param name="dispatchID">派工单ID</param>
        /// <param name="status">状态ID</param>
        public void UpdateDispatchStatus(int dispatchID, int status)
        {
            sqlStr = "UPDATE tblDispatch SET StatusID=@StatusID " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新派工单结束日期
        /// </summary>
        /// <param name="dispatchID">派工单ID</param>
        public void UpdateDispatchEndDate(int dispatchID)
        {
            sqlStr = "UPDATE tblDispatch SET EndDate=@EndDate " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新派工单开始日期
        /// </summary>
        /// <param name="dispatchID">派工单ID</param>
        public void UpdateDispatchStartDate(int dispatchID)
        {
            sqlStr = "UPDATE tblDispatch SET StartDate=@StartDate " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchID;
                command.ExecuteNonQuery();
            }
        }

        #region "APP"
        /// <summary>
        /// 获取派工单列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="status">派工单状态</param>
        /// <returns>派工单列表</returns>
        public List<DispatchInfo> GetDispatches(int userID,int status)
        {
            List<DispatchInfo> dispatches = new List<DispatchInfo>();
            sqlStr = "SELECT DISTINCT d.*,j.ID as DispatchJournalID,r.ID as DispatchReportID ," +
                " j.StatusID AS DispatchJournalStatusID,r.StatusID AS DispatchReportStatusID FROM tblDispatch d " +
                " LEFT JOIN tblDispatchJournal  j ON d.ID = j.DispatchID " +
                " LEFT JOIN tblDispatchReport r ON d.ID = r.DispatchID " +
                " WHERE 1=1 ";
            if (status != 0) sqlStr += " AND d.StatusID=" + status;
            sqlStr += " AND d.EngineerID = " + userID;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatches.Add(new DispatchInfo(dr));
                    }
                }
            }
            return dispatches;
        }
        /// <summary>
        /// 根据派工单状态获取派工单数量
        /// </summary>
        /// <param name="statusList">派工单状态</param>
        /// <param name="userId">用户ID</param>
        /// <returns>派工单数量</returns>
        public int GetDispatchCount4App(List<int> statusList, int userId = 0)
        {
            sqlStr = "SELECT COUNT(d.ID) FROM tblDispatch AS d " +
                     " WHERE d.StatusID IN (" + SQLUtil.ConvertToInStr(statusList) + ")";
            if (userId > 0) sqlStr += " AND d.EngineerID=" + userId;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetCount(command);
            }
        }
        /// <summary>
        /// 根据请求ID获取派工单信息
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <returns>派工单信息</returns>
        public List<DispatchInfo> GetDispatchesByRequestID(int requestID)
        {
            List<DispatchInfo> dispatches = new List<DispatchInfo>();

            sqlStr = "SELECT d.* FROM tblDispatch d " +
                     " WHERE d.RequestID = @RequestID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = requestID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatches.Add(new DispatchInfo(dr).Copy4App());
                    }
                }
            }
            return dispatches;
        }
        /// <summary>
        /// 根据请求ID获取未完成派工单信息
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <returns>未完成派工单信息</returns>
        public List<DispatchInfo> GetOpenDispatchesByRequestID(int requestID)
        {
            List<DispatchInfo> dispatches = new List<DispatchInfo>();

            sqlStr = "SELECT d.* FROM tblDispatch d " +
                     " WHERE d.StatusID not in (@Cancelled,@Approved) AND d.RequestID = @RequestID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = requestID;
                command.Parameters.Add("@Cancelled", SqlDbType.Int).Value = DispatchInfo.Statuses.Cancelled;
                command.Parameters.Add("@Approved", SqlDbType.Int).Value = DispatchInfo.Statuses.Approved;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatches.Add(new DispatchInfo(dr).Copy4App());
                    }
                }
            }
            return dispatches;
        }
        #endregion
    }
}
