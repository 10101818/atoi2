using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 派工单controller
    /// </summary>
    public class DispatchController : BaseController
    {
        private DispatchDao dispatchDao = new DispatchDao();
        private DispatchReportDao dispatchReportDao = new DispatchReportDao();
        private DispatchJournalDao dispatchJournalDao = new DispatchJournalDao();
        private DispatchManager dispatchManager = new DispatchManager();
        private UserManager userManager = new UserManager();

        /// <summary>
        /// 派工单列表页面
        /// </summary>
        /// <param name="requestId">请求编号</param>
        /// <param name="status">派工单状态</param>
        /// <returns>派工单列表页面</returns>
        public ActionResult DispatchList(int requestId = 0,int status =0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestId = requestId;
            ViewBag.StatusID = status;

            return View();
        }
        /// <summary>
        /// 请求历史派工列表页面
        /// </summary>
        /// <param name="requestID">请求编号</param>
        /// <returns>请求历史派工列表页面</returns>
        public ActionResult RequestToDispatch(int requestID)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestID = requestID;
            ViewBag.StatusID = 0;
            return View("DispatchList");
        }
        /// <summary>
        /// 作业报告列表页面
        /// </summary>
        /// <returns>作业报告列表页面</returns>
        public ActionResult DispatchReportList()
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 服务凭证列表页面
        /// </summary>
        /// <returns>服务凭证列表页面</returns>
        public ActionResult DispatchJournalList()
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 服务工单页面
        /// </summary>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="requestType">派工类型</param>
        /// <returns>服务工单页面</returns>
        public ActionResult DispatchResponse(int dispatchID, int requestType)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ID = dispatchID;
            ViewBag.RequestTypeID = requestType;
            ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);
            return View();
        }
        
        /// <summary>
        /// 获取派工单列表信息
        /// </summary>
        /// <param name="status">派工单状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>派工单列表信息</returns>
        public JsonResult QueryDispatches(int status, int urgency, int type, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ResultModel<List<DispatchInfo>> result = new ResultModel<List<DispatchInfo>>();
            List<DispatchInfo> DispatchInfo = new List<DispatchInfo>();
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
                UserInfo userInfo = new UserInfo();
                userInfo = GetLoginUser();

                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                if (currentPage > 0)
                {
                    int totalNum = this.dispatchDao.QueryDispatchesCount(userInfo.ID,userInfo.Role.ID, status, urgency, type, filterField, filterText);

                    result.SetTotalPages(totalNum, pageSize);
                    result.Data = dispatchManager.QueryDispatches(userInfo.ID, userInfo.Role.ID, status, urgency, type, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                    result.Data = dispatchManager.QueryDispatches(userInfo.ID, userInfo.Role.ID, status, urgency, type, filterField, filterText, sortField, sortDirection, 0, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 根据派工单编号获取派工单信息
        /// </summary>
        /// <param name="dispatchID">派工单编号</param>
        /// <returns>派工单信息</returns>
        public JsonResult GetDispatchByID(int dispatchID)
        {
            ResultModel<DispatchInfo> result = new ResultModel<DispatchInfo>();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = this.dispatchManager.GetDispatchByID(dispatchID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 根据请求编号获取最新派工单信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <returns>最新一条派工单信息</returns>
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
                result.Data = this.dispatchManager.GetDispatchByRequestID(id);

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 获取工程师的各状态的派工单数量
        /// </summary>
        /// <param name="userID">工程师编号</param>
        /// <returns>获取取消、新建、已响应、待审批、已审批的派工单数量</returns>
        public JsonResult GetDispatchCount(int userID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<Dictionary<string, int>> result = new ResultModel<Dictionary<string, int>>();
            try
            {
                UserInfo userInfo = this.userManager.GetUser(userID);
                Dictionary<int, int> dicDispatchCount = this.dispatchDao.GetDispatchCount(userID,userInfo.Role.ID);

                Dictionary<string, int> resultData = new Dictionary<string, int>();
                if (dicDispatchCount.ContainsKey(DispatchInfo.Statuses.Cancelled) == false)
                    resultData.Add("Cancelled", 0);
                else
                    resultData.Add("Cancelled", dicDispatchCount[DispatchInfo.Statuses.Cancelled]);
                if (dicDispatchCount.ContainsKey(DispatchInfo.Statuses.New) == false)
                    resultData.Add("New", 0);
                else
                    resultData.Add("New", dicDispatchCount[DispatchInfo.Statuses.New]);
                if (dicDispatchCount.ContainsKey(DispatchInfo.Statuses.Responded) == false)
                    resultData.Add("Responded", 0);
                else
                    resultData.Add("Responded", dicDispatchCount[DispatchInfo.Statuses.Responded]);
                if (dicDispatchCount.ContainsKey(DispatchInfo.Statuses.Pending) == false)
                    resultData.Add("Pending", 0);
                else
                    resultData.Add("Pending", dicDispatchCount[DispatchInfo.Statuses.Pending]);
                if (dicDispatchCount.ContainsKey(DispatchInfo.Statuses.Approved) == false)
                    resultData.Add("Approved", 0);
                else
                    resultData.Add("Approved", dicDispatchCount[DispatchInfo.Statuses.Approved]);

                result.Data = resultData;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 开始作业
        /// </summary>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="requestID">请求id</param>
        /// <returns>开始作业返回编码</returns>
        [HttpPost]
        public JsonResult ResponseDispatch(int dispatchID, int requestID)
        {
            ResultModelBase result = new ResultModelBase();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                this.dispatchManager.ResponseDispatch(dispatchID, requestID, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 取消派工单
        /// </summary>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="requestID">请求编号</param>
        /// <returns>取消派工单返回编码</returns>
        [HttpPost]
        public JsonResult CancelDispatch(int dispatchID, int requestID)
        {
            ResultModelBase result = new ResultModelBase();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                this.dispatchManager.CancelDispatch(dispatchID, requestID, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 导出派工单列表
        /// </summary>
        /// <param name="status">派工单状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>派工单列表的excel</returns>
        public ActionResult ExportDispatches(int status,int urgency, int type, string filterField, string filterText, string sortField, bool sortDirection)
        {
            if (CheckSession() == false)
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
                UserInfo userInfo = new UserInfo();
                userInfo = GetLoginUser();
                List<DispatchInfo> dispatches = null;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                dispatches = this.dispatchManager.QueryDispatches(userInfo.ID, userInfo.Role.ID, status, urgency, type, filterField, filterText, sortField, sortDirection, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("派工单编号");
                dt.Columns.Add("请求编号");
                dt.Columns.Add("设备系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("派工类型");
                dt.Columns.Add("紧急程度");
                dt.Columns.Add("派工日期");
                dt.Columns.Add("结束日期");
                dt.Columns.Add("状态");


                foreach (DispatchInfo dispatch in dispatches)
                {
                    dt.Rows.Add(dispatch.OID, dispatch.Request.OID, dispatch.Request.EquipmentOID, dispatch.Request.EquipmentName, dispatch.RequestType.Name,
                        dispatch.Urgency.Name, dispatch.ScheduleDate.ToString("yyyy-MM-dd"), dispatch.EndDate == DateTime.MinValue ? "" : dispatch.EndDate.ToString("yyyy-MM-dd"), dispatch.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "派工单.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

	}
}