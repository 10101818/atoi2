using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using BusinessObjects.DataAccess;


namespace BusinessObjects.Manager
{
    /// <summary>
    /// 请求manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class RequestManager
    {
        private UploadFileManager uploadFileManager = new UploadFileManager();
        private UploadFileManager fileManager = new UploadFileManager();
        private RequestDao requestDao = new RequestDao();
        private FileDao fileDao = new FileDao();
        private HistoryDao historyDao = new HistoryDao();
        private DispatchDao dispatchDao = new DispatchDao();
        private UserDao userDao = new UserDao();

        /// <summary>
        /// 获取请求列表信息
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
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页首条数据下标</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间 </param>
        /// <param name="requestUserID">请求人编号</param>
        /// <returns>获取请求列表信息</returns>
        public List<RequestInfo> QueryRequestsList(int status, int requestType, bool isRecall, int department, int urgency, bool overDue, int source, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize, string startDate, string endDate, int requestUserID = 0)
        {
            List<int> statusList = new List<int>();
            if (status != 0)
            {
                statusList.Add(status);
            }

            return QueryRequestsList(statusList, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, sortField, sortDirection, curRowNum, pageSize, startDate, endDate, requestUserID);
        }

        /// <summary>
        /// 获取请求列表信息
        /// </summary>
        /// <param name="statusList">请求状态集</param>
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
        /// <param name="curRowNum">每页首条数据下标</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="requestUserID">请求人编号</param>
        /// <returns>请求列表信息</returns>
        public List<RequestInfo> QueryRequestsList(List<int> statusList, int requestType, bool isRecall, int department, int urgency, bool overDue, int source, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize, string startDate, string endDate, int requestUserID = 0)
        {
            List<RequestInfo> infos = this.requestDao.QueryRequestsList(statusList, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, sortField, sortDirection, startDate, endDate, curRowNum, pageSize, requestUserID);

            if (infos.Count > 0)
            {
                List<RequestEqptInfo> requestEqpts = this.requestDao.GetRequestEgpts(SQLUtil.GetIDListFromObjectList(infos));

                foreach (RequestInfo info in infos)
                {
                    info.Equipments = (from RequestEqptInfo temp in requestEqpts where temp.RequestID == info.ID select temp.Equipment).ToList();
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据请求编号获取请求信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <returns>请求信息</returns>
        public RequestInfo GetRequest(int id)
        {
            RequestInfo info = this.requestDao.QueryRequestByID(id);
            if (info != null)
            {
                if (info.Source.ID == RequestInfo.Sources.SysRequest)
                {
                    info.RequestUser.Name = "系统";
                }
                List<HistoryInfo> histories = this.historyDao.GetHistories(ObjectTypes.Request, info.ID);
                if (histories != null && histories.Count > 0)
                {
                    foreach (HistoryInfo history in histories)
                    {
                        history.Action.Name = RequestInfo.Actions.GetDesc(history.Action.ID);
                    }
                    info.Histories = histories;
                    info.SetHis4Comments();
                }
                info.Files = this.fileDao.GetFiles(ObjectTypes.Request, info.ID);
                info.Equipments = this.requestDao.GetRequestEgpts(info.ID);
            }
            return info;
        }
        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="info">请求信息</param>
        /// <param name="files">请求附件信息</param>
        /// <param name="user">操作的用户信息</param>
        /// <returns>请求编号</returns>
        [TransactionAspect]
        public int AddRequest(RequestInfo info, List<UploadFileInfo> files,UserInfo user)
        {
            info.ID = this.requestDao.AddRequest(info);

            if (info.Equipments != null)
            {
                foreach(EquipmentInfo eqptInfo in info.Equipments)
                {
                    this.requestDao.AddRequestEqpt(info.ID, eqptInfo.ID);
                }
            }

            if (files != null && files.Count > 0)
            {
                foreach (UploadFileInfo file in files)
                {
                    file.ObjectID = info.ID;
                    file.ObjectTypeId = ObjectTypes.Request;
                    this.fileManager.SaveUploadFile(file);
                }
            }

            //添加历史操作——请求 新增
            if (user != null)
            {
                HistoryInfo history = new HistoryInfo(info.ID, ObjectTypes.Request, user.ID, RequestInfo.Actions.New);
                this.historyDao.AddHistory(history);
            }

            //send app message
            if (info.Source.ID == RequestInfo.Sources.CustomerRequest && info.RequestType.ID == RequestInfo.RequestTypes.Repair)
            {
                info = GetRequest(info.ID);
                List<UserInfo> superAdminUsers = this.userDao.GetActiveUsers(new List<int> { UserRole.SuperAdmin });
                List<string> registrationIds = SQLUtil.GetStringListFromObjectList(superAdminUsers, "RegistrationID", false);
                if (registrationIds.Count > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        JPushManager.PushMessage(string.Format("您有新的请求,科室:{0},设备名称:{1},设备序列号:{2},请及时派工", info.Equipments[0].Department.Name, info.Equipments[0].Name, info.Equipments[0].SerialCode), registrationIds);
                    });
                }
            }

            return info.ID;
        }
        /// <summary>
        /// 修改择期日期
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="selectiveDate">择期日期</param>
        [TransactionAspect]
        public void UpdateSelectiveDate(int id, DateTime selectiveDate)
        {
            this.requestDao.UpdateSelectiveDate(id, selectiveDate);
        }
        /// <summary>
        /// 派工
        /// </summary>
        /// <param name="requestInfo">请求内容</param>
        /// <param name="dispatchInfo">派工单信息</param>
        /// <param name="user">操作的用户信息</param>
        [TransactionAspect]
        public void DispatchRequest(RequestInfo requestInfo, DispatchInfo dispatchInfo, UserInfo user)
        {
            RequestInfo info = GetRequest(requestInfo.ID);

            List<DispatchInfo> dispatchInfos = this.dispatchDao.GetOpenDispatchesByRequestID(requestInfo.ID);
            bool isExisted = dispatchInfos.Count > 0 ? true : false;

            if (info.DistributeDate == DateTime.MinValue)
            {
                this.requestDao.UpdateRequestDistributeDate(requestInfo.ID, dispatchInfo.ScheduleDate);
            }
            if (isExisted == false)
            {
                requestInfo.LastStatus.ID = requestInfo.Status.ID;
            }
            requestInfo.Status.ID = RequestInfo.Statuses.Allocated;
            if (info.Status.ID == RequestInfo.Statuses.Close || info.Status.ID == RequestInfo.Statuses.Pending || info.Status.ID == RequestInfo.Statuses.Responded)
            {
                requestInfo.Status.ID = info.Status.ID;
            }
            requestInfo.Priority.ID = dispatchInfo.Urgency.ID;

            this.requestDao.UpdateRequest(requestInfo);

            dispatchInfo.Status.ID = DispatchInfo.Statuses.New;
            dispatchInfo.ID = this.dispatchDao.AddDispatch(dispatchInfo).ID;
            //添加历史操作——请求 派工
            HistoryInfo historyRequest = new HistoryInfo(requestInfo.ID, ObjectTypes.Request, user.ID, RequestInfo.Actions.Allocate);
            this.historyDao.AddHistory(historyRequest);

            //添加历史操作——派工单 新增
            HistoryInfo historyDisptach = new HistoryInfo(dispatchInfo.ID, ObjectTypes.Dispatch, user.ID, DispatchInfo.Actions.New, dispatchInfo.LeaderComments);
            this.historyDao.AddHistory(historyDisptach);

            UserInfo adminUser = this.userDao.GetUser(dispatchInfo.Engineer.ID);
            if (!string.IsNullOrEmpty(adminUser.RegistrationID))
            {
                Task.Factory.StartNew(() =>
                {
                    JPushManager.PushMessage(string.Format("您有新的派工, 派工单号:{0},请及时响应", dispatchInfo.OID), new List<string> { adminUser.RegistrationID });
                }); 
                
            }
        }
        /// <summary>
        /// 取消请求
        /// </summary>
        /// <param name="requestInfo">请求信息</param>
        /// <param name="user">操作的用户信息</param>
        [TransactionAspect]
        public void CancelRequest(RequestInfo requestInfo, UserInfo user)
        {
            requestInfo.Status.ID = RequestInfo.Statuses.Cancelled;
            this.requestDao.UpdateRequest(requestInfo);
            this.requestDao.UpdateRequestCloseDate(requestInfo.ID);
            //添加历史操作——请求 终止
            HistoryInfo historyRequest = new HistoryInfo(requestInfo.ID, ObjectTypes.Request, user.ID, RequestInfo.Actions.Cancelled);
            this.historyDao.AddHistory(historyRequest);
        }
    }
}
