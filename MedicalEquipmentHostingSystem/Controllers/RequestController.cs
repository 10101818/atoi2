using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.DataAccess;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using MedicalEquipmentHostingSystem.Areas.App.Models;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// RequestController
    /// </summary>
    /// <seealso cref="MedicalEquipmentHostingSystem.Controllers.BaseController" />
    public class RequestController : BaseController
    {
        /// <summary>
        /// 请求controller
        /// </summary>
        private RequestDao requestDao = new RequestDao();
        private DispatchDao dispatchDao = new DispatchDao();
        private EquipmentDao equipmentDao = new EquipmentDao();
        private UserManager userManager = new UserManager();
        private RequestManager requestManager = new RequestManager();
        private UploadFileManager fileManager = new UploadFileManager();
        private DashboardProvider api = new DashboardProvider();

        /// <summary>
        /// 请求列表页面
        /// </summary>
        /// <param name="equipmentId">设备编号</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="urgency">请求紧急程度</param>
        /// <param name="isRecall">是否召回</param>
        /// <param name="overDue">超期</param>
        /// <param name="source">请求来源</param>
        /// <param name="status">请求状态</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>请求列表页面</returns>
        public ActionResult RequestList(int equipmentId = 0, int requestType = 0, int urgency = 0, bool isRecall = false, bool overDue = false, int status = 0,int source = 0 , string startDate = "", string endDate = "")
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.EquipmentId = equipmentId;
            ViewBag.RequestType = requestType;
            ViewBag.Urgency = urgency;
            ViewBag.IsRecall = isRecall;
            ViewBag.OverDue = overDue;
            ViewBag.Status = status;
            ViewBag.Source = source;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.IsDemo = WebConfig.ISDEMO;

            return View();
        }
        /// <summary>
        /// 请求详情页面
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="requestStatus">请求状态</param>
        /// <returns>请求详情页面</returns>
        public ActionResult RequestDetail(int id,string actionName,int requestType ,int requestStatus)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.ActionName = actionName;
                ViewBag.RequestTypeID = requestType;
                ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);
                ViewBag.RequestStatus = requestStatus;
                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }
        /// <summary>
        /// 择期页面
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="requestType">请求类型</param>
        /// <returns>择期页面</returns>
        public ActionResult RequestSelective(int id, string actionName, int requestType)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.RequestTypeID = requestType;
                ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);
                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }
        /// <summary>
        /// 派工页面
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="requestType">请求类型</param>
        /// <returns>派工页面</returns>
        public ActionResult RequestDispatch(int id, string actionName, int requestType)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.ActionName = actionName;
                ViewBag.RequestTypeID = requestType;
                ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);

                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }

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
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">截至日期</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>请求列表信息</returns>
        public JsonResult QueryRequestList(int status, int requestType, bool isRecall, int department, int urgency, bool overDue, int source , string filterField, string filterText, string field, bool direction, int currentPage = 0, string startDate = "", string endDate = "", int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<RequestInfo>> result = new ResultModel<List<RequestInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);

                List<RequestInfo> infos = new List<RequestInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.requestDao.QueryRequestsCount(status, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, startDate, endDate);

                    result.SetTotalPages(totalRecord, pageSize);
                    infos = this.requestManager.QueryRequestsList(status, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, field, direction, result.GetCurRowNum(currentPage, pageSize), pageSize, startDate, endDate);
                }
                else
                {
                    infos = this.requestManager.QueryRequestsList(status, requestType, isRecall, department, urgency, overDue, source, filterField, filterText, field, direction, 0, 0, startDate, endDate);
                }
                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 通过请求编号获取请求信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <returns>请求信息</returns>
        public JsonResult QueryRequestByID(int id)
        {
            ResultModel<RequestInfo> result = new ResultModel<RequestInfo>();
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                RequestInfo requestInfo = this.requestManager.GetRequest(id);

                result.Data = requestInfo;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 通过请求编号获取最新派工单信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <returns>最新派工单信息</returns>
        public JsonResult GetDispatchByRequestID(int id)
        {
            ResultModel<DispatchInfo> result = new ResultModel<DispatchInfo>();
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                DispatchInfo dispatchInfo = this.dispatchDao.GetDispatchByRequestID(id);
                if (dispatchInfo != null)
                {
                    UserInfo userInfo = userManager.GetUser(dispatchInfo.Engineer.ID);
                    if (userInfo != null) dispatchInfo.Engineer.Name = userInfo.Name;
                    result.Data = dispatchInfo;
                }
                
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 判断请求是否有作业中的派工单
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <returns>true/false 是否有作业中的派工单</returns>
        public JsonResult CheckOpenDispatchesExist(int id)
        {
            ResultModel<bool> result = new ResultModel<bool>();
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                List<DispatchInfo> dispatchInfos = this.dispatchDao.GetOpenDispatchesByRequestID(id);
                if (dispatchInfos.Count > 0)
                    result.Data = true;
                else
                    result.Data = false;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 择期
        /// </summary>
        /// <param name="info">请求内容</param>
        /// <returns>择期返回信息</returns>
        [HttpPost]
        public JsonResult UpdateSelectiveDate(RequestInfo info)
        {
            ResultModelBase result = new ResultModelBase();
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                this.requestManager.UpdateSelectiveDate(info.ID, info.SelectiveDate);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出请求列表信息excel
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
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>请求信息excel</returns>
        [HttpPost]
        public ActionResult ExportRequestsList(int status, int requestType, bool isRecall, int department, int urgency, bool overDue, int source , string filterField, string filterText, string field, bool direction, string startDate = "", string endDate = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<RequestInfo>> result = new ResultModel<List<RequestInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<RequestInfo> infos = new List<RequestInfo>();
                infos = this.requestManager.QueryRequestsList(status, requestType,isRecall, department, urgency, overDue, source, filterField, filterText, field, direction, 0, 0,startDate,endDate);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("请求编号");
                dt.Columns.Add("设备系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("科室");
                dt.Columns.Add("请求人");
                dt.Columns.Add("请求日期");
                dt.Columns.Add("择期日期");
                dt.Columns.Add("请求来源");
                dt.Columns.Add("类型");
                dt.Columns.Add("状态");


                foreach (RequestInfo requestInfo in infos)
                {
                    dt.Rows.Add(requestInfo.OID, requestInfo.EquipmentOID, requestInfo.EquipmentName,requestInfo.Equipments.Count>0?requestInfo.Equipments[0].Department.Name:"",
                        requestInfo.RequestUser.Name, requestInfo.RequestDate.ToString("yyyy-MM-dd"), requestInfo.SelectiveDate == DateTime.MinValue ? "" : requestInfo.SelectiveDate.ToString("yyyy-MM-dd"), requestInfo.Source.Name, requestInfo.RequestType.Name, requestInfo.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "客户请求列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="info">请求内容</param>
        /// <returns>新增请求返回信息</returns>
        [HttpPost]
        public JsonResult AddRequest(RequestInfo info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModelBase result = new ResultModelBase();

            try
            {
                List<UploadFileInfo> files = GetUploadFilesInSession();

                info.RequestUser = GetLoginUser();
                info.Status.ID = RequestInfo.Statuses.New;
                info.Source.ID = RequestInfo.Sources.ManualRequest;
                info.Source.Name = RequestInfo.Sources.GetSourceDesc(info.Source.ID);

                info.ID = this.requestManager.AddRequest(info,files, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 派工
        /// </summary>
        /// <param name="requestInfo">请求内容</param>
        /// <param name="dispatchInfo">派工单内容</param>
        /// <returns>派工返回信息</returns>
        [HttpPost]
        public JsonResult DispatchRequest(RequestInfo requestInfo, DispatchInfo dispatchInfo)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModelBase result = new ResultModelBase();

            try
            {
                List<DispatchInfo> dispatchInfos = this.dispatchDao.GetOpenDispatchesByRequestID(requestInfo.ID);
                foreach (DispatchInfo dispatch in dispatchInfos)
                {
                    if (dispatch.Engineer.ID == dispatchInfo.Engineer.ID)
                    {
                        result.SetFailed(ResultCodes.ParameterError, "该工程师已被派工且未完成");
                        return JsonResult(result);
                    }
                }
                this.requestManager.DispatchRequest(requestInfo, dispatchInfo, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 取消请求
        /// </summary>
        /// <param name="requestInfo">请求内容</param>
        /// <returns>取消请求返回信息</returns>
        [HttpPost]
        public JsonResult CancelRequest(RequestInfo requestInfo)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModelBase result = new ResultModelBase();

            try
            {
                this.requestManager.CancelRequest(requestInfo,GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        #region"DashBoard"

        /// <summary>
        /// 请求信息总览
        /// </summary>
        /// <returns>请求信息总览</returns>
        public JsonResult QueryOverview()
        {
            ServiceResultModel<Dictionary<string, List<RequestInfo>>> result = new ServiceResultModel<Dictionary<string, List<RequestInfo>>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.RequestQueryOverview();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 今日总报修
        /// </summary>
        /// <returns>今日总报修</returns>
        public JsonResult Todays(string date = "")
        {
            ServiceResultModel<List<RequestInfo>> result = new ServiceResultModel<List<RequestInfo>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.Todays(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 开机率,校准率,保养率,巡检率信息
        /// </summary>
        /// <returns>开机率,校准率,保养率,巡检率信息</returns>
        public JsonResult KPI(string date ="")
        {
            ServiceResultModel<Dictionary<string, Dictionary<string, double>>> result = new ServiceResultModel<Dictionary<string, Dictionary<string, double>>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.KPI(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
	}
}