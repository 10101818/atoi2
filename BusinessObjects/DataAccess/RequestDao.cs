using System;
using System.Collections.Generic;
using BusinessObjects.Aspect;
using PostSharp.Extensibility;
using BusinessObjects.Domain;
using System.Data.SqlClient;
using System.Data;
using BusinessObjects.Util;
using BusinessObjects.Manager;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 请求dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
   public class RequestDao : BaseDao
   {
        #region "tblRequest"
        /// <summary>
        /// 根据搜索条件获取请求数量
        /// </summary>
        /// <param name="status">请求状态</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="isRecall">是否召回</param>
        /// <param name="department">科室编号</param>
        /// <param name="urgency">请求紧急程度</param>
        /// <param name="overDue">是否超期</param>
        /// <param name="source">请求来源</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <returns>请求数量</returns>
        public int QueryRequestsCount(int status, int requestType,bool isRecall, int department, int urgency, bool overDue, int source, string filterField, string filterText, string startDate, string endDate)
        {
            sqlStr = "SELECT COUNT(DISTINCT r.ID) FROM tblRequest AS r " +
                     " LEFT JOIN jctRequestEqpt re ON re.RequestID=r.ID" +
                     " LEFT JOIN tblEquipment e ON e.ID=re.EquipmentID" +
                     " LEFT JOIN tblDispatch d ON d.RequestID=r.ID";
            sqlStr += " WHERE 1=1 ";
            if (status == RequestInfo.Statuses.Unfinished)
                sqlStr += " AND  r.StatusID <> " + RequestInfo.Statuses.Cancelled + " AND r.StatusID <> " + RequestInfo.Statuses.Close;
            else if (status != 0)
                sqlStr += " AND r.StatusID = " + status;
            else sqlStr += " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled;
            if (department >= 0)
                sqlStr += " AND e.DepartmentID = " + department;
            if (urgency != 0)
                sqlStr += " AND r.PriorityID = " + urgency;
            if (overDue)
                sqlStr += " AND " + RequestInfo.Statuses.GetCurOverDueSQL();
            if (source != 0)
                sqlStr += " AND r.Source = " + source;

            if (requestType > 0)
                sqlStr += " AND r.RequestType = " + requestType;
            if (!string.IsNullOrEmpty(startDate))
                sqlStr += " AND r.RequestDate >= @StartDate ";
            if (!string.IsNullOrEmpty(endDate))
                sqlStr += " AND r.RequestDate < @EndDate ";
            if (isRecall)
                sqlStr += " AND r.IsRecall = 1 ";
            if (!string.IsNullOrEmpty(filterText))
            {
                if (filterField.Equals("e.ID") || filterField.Equals("e.Name"))
                {
                    sqlStr += " AND EXISTS (SELECT j.EquipmentID FROM jctRequestEqpt j INNER JOIN tblEquipment as e ON j.EquipmentID = e.ID WHERE j.RequestID = r.ID";
                    sqlStr += GetFieldFilterClause(filterField);
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += GetFieldFilterClause(filterField);
                }
            }

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (!string.IsNullOrEmpty(startDate))
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(endDate).AddDays(1);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取请求列表信息
        /// </summary>
        /// <param name="statusList">请求状态</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="isRecall">是否召回</param>
        /// <param name="department">科室编号</param>
        /// <param name="urgency">请求紧急程度</param>
        /// <param name="overDue">是否超期</param>
        /// <param name="source">请求来源</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <param name="requestUserID">请求用户ID</param>
        /// <returns>请求列表信息</returns>
        public List<RequestInfo> QueryRequestsList(List<int> statusList, int requestType, bool isRecall, int department, int urgency, bool overDue, int source, string filterField, string filterText, string sortField, bool sortDirection, string startDate, string endDate, int curRowNum = 0, int pageSize = 0, int requestUserID = 0)
        {
            List<RequestInfo> infos = new List<RequestInfo>();
            sqlStr = " SELECT DISTINCT r.*,CONVERT(VARCHAR(10),r.RequestDate,112), " + RequestInfo.Statuses.GetCurOverDueField() + 
                     " FROM tblRequest r  " +
                     " LEFT JOIN jctRequestEqpt re ON re.RequestID=r.ID" +
                     " LEFT JOIN tblEquipment e ON e.ID=re.EquipmentID" +
                     " LEFT JOIN tblDispatch d ON d.RequestID=r.ID" +
                     " WHERE 1=1 ";
            if (statusList != null && statusList.Count > 1) sqlStr += " AND r.StatusID IN (" + SQLUtil.ConvertToInStr(statusList) + ")";
            else if (statusList != null && statusList.Count == 1 && statusList[0] == RequestInfo.Statuses.Unfinished)
                sqlStr += " AND  r.StatusID <> "+RequestInfo.Statuses.Cancelled+" AND r.StatusID <> "+RequestInfo.Statuses.Close;
            else if (statusList != null && statusList.Count == 1 && statusList[0] != 0)
                sqlStr += " AND r.StatusID = " + statusList[0];
            else sqlStr += " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled;
            if (department >= 0)
                sqlStr += " AND e.DepartmentID = " + department;
            if (urgency != 0)
                sqlStr += " AND r.PriorityID = " + urgency;
            if (overDue)
                sqlStr += " AND " + RequestInfo.Statuses.GetCurOverDueSQL(); 
            if (source != 0)
                sqlStr += " AND r.Source = " + source;

            if (!string.IsNullOrEmpty(startDate))
                sqlStr += " AND r.RequestDate >= @StartDate ";
            if (!string.IsNullOrEmpty(endDate))
                sqlStr += " AND r.RequestDate < @EndDate ";
            if (requestType > 0)
                sqlStr += " AND r.RequestType = " + requestType;
            if (isRecall)
                sqlStr += " AND r.IsRecall = 1 ";
            if (requestUserID != 0)
                sqlStr += " AND r.RequestUserID = " + requestUserID;
            if (!string.IsNullOrEmpty(filterText))
            {
                if (filterField.Equals("e.ID") || filterField.Equals("e.Name"))
                {
                    sqlStr += " AND EXISTS (SELECT j.EquipmentID FROM jctRequestEqpt j INNER JOIN tblEquipment as e ON j.EquipmentID = e.ID WHERE j.RequestID = r.ID";
                    sqlStr += GetFieldFilterClause(filterField);
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += GetFieldFilterClause(filterField);
                }               
            }
            if (sortField.Equals("init"))
                sqlStr += string.Format(" ORDER BY {0} DESC, r.RequestType ASC, CONVERT(VARCHAR(10),r.RequestDate,112) DESC,r.StatusID, r.ID ", RequestInfo.Statuses.GetCurOverDueField());
            else
                sqlStr += GenerateSortClause(sortDirection, sortField, "r.ID");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (!string.IsNullOrEmpty(startDate))
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(endDate).AddDays(1);
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new RequestInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据请求ID获取请求信息
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <returns>请求信息</returns>
        public RequestInfo QueryRequestByID(int id)
        {
            sqlStr = "SELECT r.* From tblRequest as r " +
                    " WHERE r.ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new RequestInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="info">请求信息</param>
        /// <returns>请求ID</returns>
        public int AddRequest(RequestInfo info)
        {
            sqlStr = "INSERT INTO tblRequest(Source,RequestType,RequestUserID,RequestUserName,RequestUserMobile," +
                     " Subject,EquipmentStatus,FaultTypeID,FaultDesc,StatusID,DealTypeID,PriorityID,RequestDate,LastStatusID,IsRecall) " +
                     " VALUES(@Source,@RequestType,@RequestUserID,@RequestUserName,@RequestUserMobile," +
                     " @Subject,@EquipmentStatus,@FaultTypeID,@FaultDesc,@StatusID,@DealTypeID,@PriorityID,@RequestDate,@LastStatusID,@IsRecall); " +
                     " SELECT @@IDENTITY";
                     
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Source", SqlDbType.Int).Value = info.Source.ID;
                command.Parameters.Add("@RequestType", SqlDbType.Int).Value = info.RequestType.ID;
                command.Parameters.Add("@RequestUserID", SqlDbType.Int).Value = SQLUtil.ZeroToNull(info.RequestUser.ID);
                command.Parameters.Add("@RequestUserName", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.RequestUser.Name);
                command.Parameters.Add("@RequestUserMobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.RequestUser.Mobile);
                command.Parameters.Add("@EquipmentStatus", SqlDbType.Int).Value = info.MachineStatus.ID;
                command.Parameters.Add("@Subject", SqlDbType.NVarChar).Value = info.EquipmentName + '-' + info.RequestType.Name;
                command.Parameters.Add("@FaultTypeID", SqlDbType.Int).Value = SQLUtil.ZeroToNull(info.FaultType.ID);
                command.Parameters.Add("@FaultDesc", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.FaultDesc);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = info.Status.ID;
                command.Parameters.Add("@DealTypeID", SqlDbType.Int).Value = info.DealType.ID;
                command.Parameters.Add("@PriorityID", SqlDbType.Int).Value = info.Priority.ID;
                command.Parameters.Add("@RequestDate", SqlDbType.DateTime).Value = info.RequestDate == DateTime.MinValue ? DateTime.Now : info.RequestDate;
                command.Parameters.Add("@LastStatusID", SqlDbType.Int).Value = info.LastStatus.ID;
                command.Parameters.Add("@IsRecall", SqlDbType.Bit).Value = info.IsRecall;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info.ID;
        }
        /// <summary>
        /// 更新请求信息
        /// </summary>
        /// <param name="info">请求信息</param>
        public void UpdateRequest(RequestInfo info)
        {
            sqlStr = " UPDATE tblRequest SET Subject=@Subject,EquipmentStatus=@EquipmentStatus,FaultTypeID=@FaultTypeID,FaultDesc=@FaultDesc,StatusID=@StatusID, " +
                     " LastStatusID=@LastStatusID,DealTypeID=@DealTypeID,PriorityID=@PriorityID,IsRecall=@IsRecall " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Subject", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Subject);
                command.Parameters.Add("@EquipmentStatus", SqlDbType.Int).Value = info.MachineStatus.ID;
                command.Parameters.Add("@FaultTypeID", SqlDbType.Int).Value = SQLUtil.ZeroToNull(info.FaultType.ID);
                command.Parameters.Add("@FaultDesc", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.FaultDesc);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = info.Status.ID;
                command.Parameters.Add("@LastStatusID", SqlDbType.Int).Value = info.LastStatus.ID;
                command.Parameters.Add("@DealTypeID", SqlDbType.Int).Value = info.DealType.ID;
                command.Parameters.Add("@PriorityID", SqlDbType.Int).Value = info.Priority.ID;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@IsRecall", SqlDbType.Bit).Value = info.IsRecall;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新上次请求状态
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="lastStatusID">上次请求状态ID</param>
        public void UpdateLastStatusID(int id, int lastStatusID)
        {
            sqlStr = " UPDATE tblRequest SET LastStatusID=@LastStatusID " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LastStatusID", SqlDbType.Int).Value = lastStatusID;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新择期日期
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="selectiveDate">择期日期</param>
        public void UpdateSelectiveDate(int id, DateTime selectiveDate)
        {
            sqlStr = " UPDATE tblRequest SET SelectiveDate=@SelectiveDate " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@SelectiveDate", SqlDbType.DateTime).Value = selectiveDate;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新请求状态和首次响应时间
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="status">请求状态</param>
        public void UpdateRequest4DispatchResponse(int requestID, int status)
        {
            sqlStr = "UPDATE tblRequest SET StatusID=@StatusID,ResponseDate=@ResponseDate " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status;
                command.Parameters.Add("@ResponseDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = requestID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新请求状态
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="status">请求状态</param>
        public void UpdateRequestStatus(int requestID, int status)
        {
            sqlStr = "UPDATE tblRequest SET StatusID=@StatusID WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = requestID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新请求结束日期
        /// </summary>
        /// <param name="requestID">请求ID</param>
        public void UpdateRequestCloseDate(int requestID)
        {
            sqlStr = "UPDATE tblRequest SET CloseDate=@CloseDate WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@CloseDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = requestID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新请求首次派工时间,首次分配时间
        /// </summary>
        /// <param name="requestID">请求ID</param>
        /// <param name="distributeDate">首次分配时间</param>
        public void UpdateRequestDistributeDate(int requestID, DateTime distributeDate)
        {
            sqlStr = "UPDATE tblRequest SET DistributeDate=@DistributeDate,DispatchDate=@DispatchDate " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DistributeDate", SqlDbType.DateTime).Value = distributeDate;
                command.Parameters.Add("@DispatchDate", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = requestID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 根据请求类型获取请求数量
        /// </summary>
        /// <param name="statusIdList">请求类型</param>
        /// <returns>请求数量</returns>
        public int GetRequestCount4App(List<int> statusIdList)
        {
            sqlStr = "SELECT COUNT(r.ID) FROM tblRequest AS r " +
                     " WHERE 1=1 ";
            if (statusIdList[0] == RequestInfo.Statuses.Unfinished)
                sqlStr += " AND r.StatusID != -1 AND r.StatusID != 99 ";
            else
                sqlStr += " AND r.StatusID IN (" + SQLUtil.ConvertToInStr(statusIdList) + ")";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {

                return GetCount(command);
            }
        }
        #endregion

        #region "jctRequestEqpt"
        /// <summary>
        /// 新增请求设备关联信息
        /// </summary>
        /// <param name="requestId">请求ID</param>
        /// <param name="equipmentId">设备ID</param>
        public void AddRequestEqpt(int requestId,int equipmentId)
        {
            sqlStr = "INSERT INTO jctRequestEqpt (RequestID,EquipmentID) " +
                     " VALUES(@RequestID,@EquipmentID); " ;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = requestId;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentId;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取请求关联设备信息
        /// </summary>
        /// <param name="requestId">请求ID</param>
        /// <returns>请求关联设备信息</returns>
        public List<EquipmentInfo> GetRequestEgpts(int requestId)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT e.*, s.Name AS SupplierName, su.Name AS ManufacturerName, c.ContractID,ct.ScopeID,ct.ScopeComments FROM jctRequestEqpt j " +
                    " INNER JOIN tblEquipment AS e ON e.ID = j.EquipmentID" +
                     " LEFT JOIN tblSupplier AS s ON e.SupplierID=s.ID " +
                     " LEFT JOIN tblSupplier AS su ON e.ManufacturerID=su.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblContract AS ct on ct.ID = c.ContractID" +
                     " WHERE j.RequestID = @RequestID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestID", SqlDbType.Int).Value = requestId;

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
        /// 获取请求关联设备信息
        /// </summary>
        /// <param name="requestIds">请求ID</param>
        /// <returns>请求关联设备信息</returns>
        public List<RequestEqptInfo> GetRequestEgpts(List<int> requestIds)
        {
            List<RequestEqptInfo> infos = new List<RequestEqptInfo>();

            sqlStr = "SELECT e.ID ,e.Name ,e.DepartmentID ,e.SerialCode , j.RequestID FROM jctRequestEqpt j " +
                    " INNER JOIN tblEquipment AS e ON e.ID = j.EquipmentID" +
                    " WHERE j.RequestID in (" + (string.IsNullOrEmpty(SQLUtil.ConvertToInStr(requestIds)) ? "null" : SQLUtil.ConvertToInStr(requestIds)) + ") ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    RequestEqptInfo info = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        info = new RequestEqptInfo();
                        info.RequestID = SQLUtil.ConvertInt(dr["RequestID"]);
                        info.Equipment.ID = SQLUtil.ConvertInt(dr["ID"]);
                        info.Equipment.Name = SQLUtil.TrimNull(dr["Name"]);
                        info.Equipment.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
                        info.Equipment.Department.ID = SQLUtil.ConvertInt(dr["DepartmentID"]);
                        info.Equipment.Department.Name = Manager.LookupManager.GetDepartmentDesc(info.Equipment.Department.ID);
                        infos.Add(info);
                    }
                }
            }

            return infos;
        }

        #endregion

        #region "Dashboard"

        /// <summary>
        /// 获取今日报修请求
        /// </summary>
        /// <returns>今日报修请求</returns>
        public List<RequestInfo> GetTodayRepair(DateTime today)
        {
            List<RequestInfo> repairs = new List<RequestInfo>();
            sqlStr = "SELECT r.* FROM tblRequest r " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID=r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID=re.EquipmentID " +
                    " WHERE r.RequestDate BETWEEN @Today AND @Tomorrow" +
                    " AND r.RequestType= @RequestType ORDER BY RequestDate DESC, r.ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Today", SqlDbType.DateTime).Value = today.Date;
                command.Parameters.Add("@Tomorrow", SqlDbType.DateTime).Value = today.AddDays(1).AddMilliseconds(-1);
                command.Parameters.Add("@RequestType", SqlDbType.Int).Value = RequestInfo.RequestTypes.Repair;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        repairs.Add(new RequestInfo(dr));
                    }
                }
            }
            return repairs;
        }

        /// <summary>
        /// 根据请求类型获取当月校准、保养、巡检 占比 KPI
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="today">目标日期</param>
        /// <returns>当月校准、保养、巡检 占比 KPI</returns>
        public Tuple<int, int> GetRequestPlanActual(int requestType,DateTime today)
        {
            sqlStr = " SELECT COUNT(ID) AS Plans, SUM(CASE WHEN StatusID = @StatusClose THEN 1 ELSE 0 END) AS Actuals " +
                    " FROM tblRequest " +
                    " WHERE DATEPART(YEAR, RequestDate) = @Year And DATEPART(MONTH, RequestDate) = @Month " +
                    " AND RequestType = @RequestType AND Source=@Source ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusClose", SqlDbType.Int).Value = RequestInfo.Statuses.Close;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = today.Year;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = today.Month;
                command.Parameters.Add("@RequestType", SqlDbType.Int).Value = requestType;
                command.Parameters.Add("@Source", SqlDbType.Int).Value = RequestInfo.Sources.SysRequest;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new Tuple<int, int>(SQLUtil.ConvertInt(dr["Plans"]), SQLUtil.ConvertInt(dr["Actuals"]));
                else
                    return new Tuple<int, int>(0, 0);
            }
        }

       #endregion
   }
}
