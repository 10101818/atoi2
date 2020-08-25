using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// RequestController
    /// </summary>
    public class RequestController : BaseController
    {
        private RequestManager requestManager = new RequestManager();
        private UserManager userManager = new UserManager();
        private EquipmentManager equipmentManager = new EquipmentManager();
        private RequestDao requestDao = new RequestDao();
        private DispatchDao dispatchDao = new DispatchDao();
        private EquipmentDao equipmentDao = new EquipmentDao();
        private FileDao fileDao = new FileDao();
        private DashboardProvider api = new DashboardProvider();

        /// <summary>
        /// 扫码报修
        /// </summary>
        /// <param name="id">设备编号</param> 
        /// <returns>扫码报修页面</returns>
        public ActionResult Repair(int id)
        {
            ResultModelBase result = new ResultModelBase();
            try
            { 
                EquipmentInfo info = this.equipmentManager.GetEquipment(id);
                if (info == null) return View("Error", result);
                return View("Repair", info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }

        /// <summary>
        /// 获取请求信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="statusID">请求状态</param>
        /// <param name="typeID">请求类型</param>
        /// <param name="isRecall">是否召回</param>
        /// <param name="department">科室ID</param>
        /// <param name="urgency">紧急程度ID</param>
        /// <param name="overDue">是否超期</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="startDate">请求开始时间</param>
        /// <param name="endDate">请求结束时间</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页展示数据条数</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求信息</returns>
        public JsonResult GetRequests(int userID, string sessionID, List<int> statusID, int typeID, bool isRecall = false, int department = -1, int urgency = 0, int source = 0, bool overDue = false, string filterField = "", string filterText = "", string sortField = "init", bool sortDirection = false, string startDate = "", string endDate = "", int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<RequestInfo>> result = new ServiceResultModel<List<RequestInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                int requestUserID = 0;
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    if (user.Role.ID == UserRole.User) requestUserID = userID;

                    BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                    List<RequestInfo> infos = this.requestManager.QueryRequestsList(statusID, typeID, isRecall, department, urgency, overDue, source, filterField, filterText, sortField, sortDirection, curRowNum, pageSize, startDate, endDate, requestUserID);

                    if (pageSize != 0)
                    {
                        foreach (RequestInfo info in infos)
                        {
                            if (info.Status.ID != RequestInfo.Statuses.New ||
                                info.Status.ID != RequestInfo.Statuses.Cancelled)
                            {
                                info.HasOpenDispatch = (this.dispatchDao.GetOpenDispatchesByRequestID(info.ID).Count > 0);
                            }
                        }
                    }

                    result.Data = infos;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过请求编号获取请求信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="requestID">请求编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求信息</returns>
        public JsonResult GetRequestByID(int userID, string sessionID, int requestID)
        {
            ServiceResultModel<RequestInfo> result = new ServiceResultModel<RequestInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    RequestInfo info  = this.requestManager.GetRequest(requestID);
                    info.HasOpenDispatch = this.dispatchDao.GetOpenDispatchesByRequestID(info.ID).Count > 0;

                    result.Data = info;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增请求信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="requestInfo">请求内容</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求信息</returns>
        [HttpPost]
        public JsonResult AddRequest(int userID, string sessionID, RequestInfo requestInfo)
        {
            ServiceResultModel<int> response = new ServiceResultModel<int>();
            EquipmentInfo equipmentInfo = new EquipmentInfo();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                requestInfo.RequestUser = user;
                requestInfo.Status.ID = RequestInfo.Statuses.New;
                if (user.Role.ID == UserRole.User)
                {
                    if (requestInfo.RequestType.ID != RequestInfo.RequestTypes.Repair && requestInfo.RequestType.ID != RequestInfo.RequestTypes.Others)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "请求类型不正确");
                        return MyJson(response, JsonRequestBehavior.AllowGet);
                    }
                    requestInfo.Source.ID = RequestInfo.Sources.CustomerRequest;
                }
                else requestInfo.Source.ID = RequestInfo.Sources.ManualRequest;
                requestInfo.Source.Name = RequestInfo.Sources.GetSourceDesc(requestInfo.Source.ID);
                requestInfo.Priority.ID = RequestInfo.UrgencyLevel.Normal;
                requestInfo.DealType.ID = RequestInfo.DealTypes.FieldService;
                if (requestInfo.Files != null && requestInfo.Files.Count > 0)
                {
                    foreach (UploadFileInfo info in requestInfo.Files)
                    {
                        info.FileContent = ParseBase64String(info.FileContent);
                    }
                }

                if (requestInfo.RequestType.ID == 0 && requestInfo.RequestType.ID > RequestInfo.RequestTypes.Others)
                    response.SetFailed(ResultCodes.ParameterError, "请选择请求类型");
                else if (requestInfo.RequestType.ID != RequestInfo.RequestTypes.Others && requestInfo.Equipments == null)
                    response.SetFailed(ResultCodes.ParameterError, "请选择设备");
                else if (requestInfo.FaultType.ID == 0 && (requestInfo.RequestType.ID == RequestInfo.RequestTypes.Maintain || requestInfo.RequestType.ID == RequestInfo.RequestTypes.Inspection || requestInfo.RequestType.ID == RequestInfo.RequestTypes.AdverseEvent))
                    response.SetFailed(ResultCodes.ParameterError, "请选择FaultType类型");
                else if (requestInfo.FaultType.ID > 0 && ((requestInfo.RequestType.ID == RequestInfo.RequestTypes.Maintain && requestInfo.FaultType.ID > RequestInfo.MaintainType.FMTSMaintain) || (requestInfo.RequestType.ID == RequestInfo.RequestTypes.Inspection && requestInfo.FaultType.ID > RequestInfo.InspectionType.IndependentInspection) || (requestInfo.RequestType.ID == RequestInfo.RequestTypes.AdverseEvent && requestInfo.FaultType.ID > RequestInfo.AdverseEventType.RecallAdverseEvent)))
                    response.SetFailed(ResultCodes.ParameterError, "请选择范围内的FaultType类型");
                else if (requestInfo.RequestType.ID != RequestInfo.RequestTypes.Others && requestInfo.Equipments.Count > 0)
                {
                    foreach (var i in requestInfo.Equipments)
                    {
                        equipmentInfo = this.equipmentDao.GetEquipmentByID(i.ID);
                        if (equipmentInfo == null)
                        {
                            response.SetFailed(ResultCodes.ParameterError, "设备不存在,请选择有效设备");
                            return MyJson(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if ((requestInfo.RequestType.ID != RequestInfo.RequestTypes.OnSiteInspection && requestInfo.RequestType.ID != RequestInfo.RequestTypes.Inventory) && requestInfo.Equipments.Count > 1)
                        response.SetFailed(ResultCodes.ParameterError, "当前请求状态只可选择一台设备");
                    else
                        response.Data = this.requestManager.AddRequest(requestInfo, requestInfo.Files, user);
                }
                else
                    response.Data = this.requestManager.AddRequest(requestInfo, requestInfo.Files, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 普通用户新增请求
        /// </summary>
        /// <param name="requestInfo">请求内容</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求内容</returns>
        [HttpPost]
        public JsonResult AddRequest4User(int userID, string sessionID, RequestInfo requestInfo)
        {
            ServiceResultModel<int> response = new ServiceResultModel<int>();
            EquipmentInfo equipmentInfo = new EquipmentInfo();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                requestInfo.Status.ID = RequestInfo.Statuses.New;
                requestInfo.Source.ID = RequestInfo.Sources.CustomerRequest;
                requestInfo.Source.Name = RequestInfo.Sources.GetSourceDesc(requestInfo.Source.ID);
                requestInfo.Priority.ID = RequestInfo.UrgencyLevel.Normal;
                requestInfo.DealType.ID = RequestInfo.DealTypes.FieldService;
                requestInfo.RequestType.ID = RequestInfo.RequestTypes.Repair;

                foreach (var i in requestInfo.Equipments)
                {
                    equipmentInfo = this.equipmentDao.GetEquipmentByID(i.ID);
                    if (equipmentInfo == null)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "设备不存在");
                        return MyJson(response, JsonRequestBehavior.AllowGet);
                    }
                }
                if (requestInfo.Files != null && requestInfo.Files.Count > 0)
                {
                    foreach (UploadFileInfo info in requestInfo.Files)
                    {
                        info.FileContent = ParseBase64String(info.FileContent);
                    }
                }

                response.Data = this.requestManager.AddRequest(requestInfo, requestInfo.Files, null);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 终止请求
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="requestID">请求编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>终止请求返回编码</returns>
        [HttpPost]
        public JsonResult EndRequest(int userID, string sessionID, int requestID)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();

            RequestInfo requestInfo = new RequestInfo();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                requestInfo = this.requestManager.GetRequest(requestID);

                if (requestInfo == null)
                    response.SetFailed(ResultCodes.ParameterError, "请求信息不存在");
                else if (requestInfo.Status.ID == RequestInfo.Statuses.Cancelled || requestInfo.Status.ID == RequestInfo.Statuses.Close)
                    response.SetFailed(ResultCodes.ParameterError, "当前请求状态下不可终止");
                else this.requestManager.CancelRequest(requestInfo, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 派工
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchInfo">派工单内容</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工单编号</returns>
        [HttpPost]
        public JsonResult CreateDispatch(int userID, string sessionID,DispatchInfo dispatchInfo)
        {
            ServiceResultModel<int> response = new ServiceResultModel<int>();

            UserInfo userInfo = new UserInfo();
            RequestInfo requestInfo = new RequestInfo();
            
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                requestInfo = this.requestManager.GetRequest(dispatchInfo.Request.ID);
                dispatchInfo.Request.RequestType = requestInfo.RequestType;
                dispatchInfo.Request.Status = requestInfo.Status;
                dispatchInfo.Request.LastStatus = requestInfo.LastStatus;

                List<DispatchInfo> dispatchInfos = this.dispatchDao.GetOpenDispatchesByRequestID(requestInfo.ID);
                foreach (DispatchInfo dispatch in dispatchInfos)
                {
                    if (dispatch.Engineer.ID == dispatchInfo.Engineer.ID)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "该工程师已被派工且未完成");
                        return MyJson(response, JsonRequestBehavior.AllowGet);
                    }
                }

                if (requestInfo == null)
                    response.SetFailed(ResultCodes.ParameterError, "请求信息不存在");
                else if (dispatchInfo.Request.FaultType.ID == 0 && (requestInfo.RequestType.ID == RequestInfo.RequestTypes.Maintain || requestInfo.RequestType.ID == RequestInfo.RequestTypes.Inspection || requestInfo.RequestType.ID == RequestInfo.RequestTypes.AdverseEvent))
                    response.SetFailed(ResultCodes.ParameterError, "请选择FaultType类型");
                else if (dispatchInfo.Request.FaultType.ID > 0 && ((requestInfo.RequestType.ID == RequestInfo.RequestTypes.Maintain && dispatchInfo.Request.FaultType.ID > RequestInfo.MaintainType.FMTSMaintain) || (requestInfo.RequestType.ID == RequestInfo.RequestTypes.Inspection && dispatchInfo.Request.FaultType.ID > RequestInfo.InspectionType.IndependentInspection) || (requestInfo.RequestType.ID == RequestInfo.RequestTypes.AdverseEvent && dispatchInfo.Request.FaultType.ID > RequestInfo.AdverseEventType.RecallAdverseEvent)))
                    response.SetFailed(ResultCodes.ParameterError, "请选择范围内的FaultType类型");
                else if (dispatchInfo.Request.Priority.ID <= 0 || dispatchInfo.Request.Priority.ID > RequestInfo.UrgencyLevel.Urgency)
                    response.SetFailed(ResultCodes.ParameterError, "请选择请求紧急程度");
                else if (dispatchInfo.Request.DealType.ID <= 0 || dispatchInfo.Request.DealType.ID > RequestInfo.DealTypes.ThirdSupport)
                    response.SetFailed(ResultCodes.ParameterError, "请选择范围内的处理方式");
                else if (dispatchInfo.RequestType.ID <= 0)
                    response.SetFailed(ResultCodes.ParameterError, "请选择派工类型");
                else if (dispatchInfo.Urgency.ID <= 0 || dispatchInfo.Urgency.ID > RequestInfo.UrgencyLevel.Urgency)
                    response.SetFailed(ResultCodes.ParameterError, "请选择派工单紧急程度");
                else if (dispatchInfo.MachineStatus.ID <= 0)
                    response.SetFailed(ResultCodes.ParameterError, "请选择机器状态");
                else if (dispatchInfo.Engineer.ID == 0)
                    response.SetFailed(ResultCodes.ParameterError, "请选择工程师");
                else if (CheckUser(dispatchInfo.Engineer.ID, response, out userInfo, UserRole.Admin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                else if (dispatchInfo.RequestType.ID != RequestInfo.RequestTypes.Others && dispatchInfo.Request.RequestType.ID == RequestInfo.RequestTypes.Others)
                    response.SetFailed(ResultCodes.ParameterError, "当前请求类型不可转换派工类型");
                else
                {
                    this.requestManager.DispatchRequest(dispatchInfo.Request, dispatchInfo, user);
                    response.Data = dispatchInfo.ID;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过附件编号下载请求附件
        /// </summary>
        /// <param name="id">附件编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>下载请求附件返回编码</returns>
        public JsonResult DownloadUploadFile(int userID, string sessionID, int id)
        {
            ServiceResultModel<string> response = new ServiceResultModel<string>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UploadFileInfo file = this.fileDao.GetFileByID(ObjectTypes.Request, id);

                if (file == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                }
                else
                {
                    string path = Path.Combine(Constants.RequestFolder, file.GetFileName());
                    if (!System.IO.File.Exists(path))
                    {
                        response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                    }
                    else
                    {
                        byte[] arr = System.IO.File.ReadAllBytes(path);
                        response.Data = Convert.ToBase64String(arr);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 择期
        /// </summary>
        /// <param name="requestId">请求编号</param>
        /// <param name="selectiveDate">择期日期</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>择期返回编码</returns>
        [HttpPost]
        public JsonResult UpdateSelectiveDate(int userID, string sessionID, int requestId, DateTime selectiveDate)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                RequestInfo requestInfo = this.requestManager.GetRequest(requestId);
                if (requestInfo == null)
                    response.SetFailed(ResultCodes.ParameterError, "请求信息不存在");
                else
                    this.requestManager.UpdateSelectiveDate(requestId, selectiveDate);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }


        #region"DashBoard"

        /// <summary>
        /// 请求信息总览
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求信息总览</returns>
        public JsonResult QueryOverview(int userID, string sessionID)
        {
            ServiceResultModel<Dictionary<string, List<RequestInfo>>> result = new ServiceResultModel<Dictionary<string, List<RequestInfo>>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.RequestQueryOverview();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 今日总报修
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>今日总报修</returns>
        public JsonResult Todays(int userID, string sessionID,string date = "")
        {
            ServiceResultModel<List<RequestInfo>> result = new ServiceResultModel<List<RequestInfo>>(); 
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.Todays(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 开机率,校准率,保养率,巡检率信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>开机率,校准率,保养率,巡检率信息</returns>
        public JsonResult KPI(int userID, string sessionID,string date = "")
        {
            ServiceResultModel<Dictionary<string, Dictionary<string, double>>> result = new ServiceResultModel<Dictionary<string, Dictionary<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.KPI(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取请求列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="status">请求状态</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="isRecall">是否召回</param>
        /// <param name="department">科室编号</param>
        /// <param name="urgency">请求紧急程度</param>
        /// <param name="overDue">是否超期</param>
        /// <param name="source">请求来源</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>请求列表信息</returns>
        public JsonResult QueryRequestList(int userID, string sessionID, int status, int requestType, bool isRecall, int department, int urgency, bool overDue, int source, string filterField, string filterText, string field, bool direction, int currentPage = 0, string startDate = "", string endDate = "", int pageSize =0)
        {
            ServiceResultModel<List<RequestInfo>> result = new ServiceResultModel<List<RequestInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<RequestInfo> infos = this.requestManager.QueryRequestsList(status, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, field, direction, currentPage, pageSize, startDate, endDate);
                
                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
	}
}