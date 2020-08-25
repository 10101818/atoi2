using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 服务凭证controller
    /// </summary>
    public class DispatchJournalController : BaseController
    {
        private DispatchDao dispatchDao = new DispatchDao();
        private HistoryDao historyDao = new HistoryDao();
        private DispatchManager dispatchManager = new DispatchManager();

        /// <summary>
        /// 服务凭证填写页面
        /// </summary>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="dispatchReportID">作业报告编号</param>
        /// <param name="requestType">派工类型</param>
        /// <returns>服务凭证填写页面</returns>
        public ActionResult DispatchJournalDetail(string actionName, int dispatchID, int dispatchJournalID=0, int dispatchReportID=0, int requestType = 0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ActionName = actionName;
            ViewBag.DispatchID = dispatchID;
            ViewBag.DispatchReportID = dispatchReportID;
            ViewBag.ID = dispatchJournalID;
            ViewBag.RequestTypeID = requestType;
            ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);

            DispatchInfo info = this.dispatchDao.GetDispatchByID(dispatchID);
            if (GetLoginUser().Role.ID == UserRole.SuperAdmin || GetLoginUser().Role.ID == UserRole.SuperUser)
            {
                return View("DispatchJournalApproveDetail");
            }
            if (info.DispatchJournal.Status.ID > DispatchJournalInfo.DispatchJournalStatus.New)
            {
                ViewBag.DispatchReportID = info.DispatchReport.ID;
                ViewBag.ID = info.DispatchJournal.ID;
                return View("DispatchJournalApproveDetail");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 服务凭证审批/查看页面
        /// </summary>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="dispatchReportID">作业报告编号</param>
        /// <param name="requestType">派工类型</param>
        /// <returns>服务凭证填写页面</returns>
        public ActionResult DispatchJournalApproveDetail(string actionName, int dispatchID, int dispatchJournalID=0, int dispatchReportID=0, int requestType = 0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ActionName = actionName;
            ViewBag.DispatchID = dispatchID;
            ViewBag.DispatchReportID = dispatchReportID;
            ViewBag.ID = dispatchJournalID;
            ViewBag.RequestTypeID = requestType;
            ViewBag.RequestTypeName = LookupManager.GetRequestTypeDesc(requestType);
            return View();
        }

        /// <summary>
        /// 获取服务凭证列表信息
        /// </summary>
        /// <param name="status">服务凭证状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>服务凭证列表信息</returns>
        public JsonResult QueryDispatchJournals(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ResultModel<List<DispatchJournalInfo>> result = new ResultModel<List<DispatchJournalInfo>>();
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
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                if (currentPage > 0)
                {
                    int totalNum = this.dispatchDao.QueryDispatchJournalsCount(status, urgency, filterField, filterText);

                    result.SetTotalPages(totalNum, pageSize);
                    result.Data = dispatchManager.QueryDispatchJournals(status, urgency, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    result.Data = dispatchManager.QueryDispatchJournals(status, urgency, filterField, filterText, sortField, sortDirection, 0, pageSize);
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
        /// 通过服务凭证编号获取服务凭证信息
        /// </summary>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <returns>服务凭证信息</returns>
        public JsonResult GetDispatchJournalByID(int dispatchJournalID)
        {
            ResultModel<DispatchJournalInfo> result = new ResultModel<DispatchJournalInfo>();
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
                DispatchJournalInfo info = this.dispatchManager.GetDispatchJournalByID(dispatchJournalID); 
                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 服务凭证各状态的数量
        /// </summary>
        /// <returns>服务凭证取消、新建、待审批、已审批的数量</returns>
        public JsonResult GetDispatchJournalCount()
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
                Dictionary<int, int> dicDispatchJournalCount = this.dispatchDao.GetDispatchJournalCount();

                Dictionary<string, int> resultData = new Dictionary<string, int>();
                if (dicDispatchJournalCount.ContainsKey(DispatchInfo.DocStatus.Cancelled) == false)
                    resultData.Add("Cancelled", 0);
                else
                    resultData.Add("Cancelled", dicDispatchJournalCount[DispatchInfo.DocStatus.Cancelled]);
                if (dicDispatchJournalCount.ContainsKey(DispatchInfo.DocStatus.New) == false)
                    resultData.Add("New", 0);
                else
                    resultData.Add("New", dicDispatchJournalCount[DispatchInfo.DocStatus.New]);
                if (dicDispatchJournalCount.ContainsKey(DispatchInfo.DocStatus.Pending) == false)
                    resultData.Add("Pending", 0);
                else
                    resultData.Add("Pending", dicDispatchJournalCount[DispatchInfo.DocStatus.Pending]);
                if (dicDispatchJournalCount.ContainsKey(DispatchInfo.DocStatus.Approved) == false)
                    resultData.Add("Approved", 0);
                else
                    resultData.Add("Approved", dicDispatchJournalCount[DispatchInfo.DocStatus.Approved]);

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
        /// 上传服务凭证
        /// </summary>
        /// <param name="dispatchJournal">服务凭证信息</param>
        /// <returns>服务凭证信息</returns>
        [HttpPost]
        public JsonResult SaveDispatchJournal(DispatchJournalInfo dispatchJournal)
        {
            ResultModel<int> result = new ResultModel<int>();
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
                dispatchJournal.FileContent = ParseBase64String(dispatchJournal.FileContent);

                dispatchJournal = this.dispatchManager.SaveDispatchJournal(dispatchJournal , GetLoginUser());
                result.Data = dispatchJournal.ID;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 审批通过服务凭证
        /// </summary>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="resultStatusID">服务凭证结果</param>
        /// <param name="followProblem">待跟进问题</param>
        /// <param name="comments">审批备注</param>
        /// <returns>审批通过服务凭证返回编码</returns>
        [HttpPost]
        public JsonResult PassDispatchJournal(int dispatchJournalID, int dispatchID, int resultStatusID, string followProblem = "", string comments = "")
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
                this.dispatchManager.PassDispatchJournal(dispatchJournalID, dispatchID, resultStatusID, GetLoginUser(), followProblem, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 审批拒绝服务凭证
        /// </summary>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="resultStatusID">服务凭证结果</param>
        /// <param name="followProblem">待跟进问题</param>
        /// <param name="comments">审批备注</param>
        /// <returns>审批拒绝服务凭证返回编码</returns>
        [HttpPost]
        public JsonResult RejectDispatchJournal(int dispatchJournalID, int dispatchID, int resultStatusID, string followProblem = "", string comments = "")
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
                this.dispatchManager.RejectDispatchJournal(dispatchJournalID, dispatchID, resultStatusID, GetLoginUser(), followProblem, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出服务凭证列表
        /// </summary>
        /// <param name="status">服务凭证状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>服务凭证列表excel</returns>
        public ActionResult ExportDispatchJournals(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection)
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
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<DispatchJournalInfo> dispatchJournals = this.dispatchManager.QueryDispatchJournals(status, urgency, filterField, filterText, sortField, sortDirection);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("服务凭证编号");
                dt.Columns.Add("请求编号");
                dt.Columns.Add("设备系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("派工类型");
                dt.Columns.Add("紧急程度");
                dt.Columns.Add("派工日期");
                dt.Columns.Add("结束日期");
                dt.Columns.Add("状态");

                foreach (DispatchJournalInfo dispatchJournal in dispatchJournals)
                {
                    dt.Rows.Add(dispatchJournal.OID, dispatchJournal.Dispatch.Request.OID, dispatchJournal.Dispatch.Request.EquipmentOID, dispatchJournal.Dispatch.Request.EquipmentName, dispatchJournal.Dispatch.RequestType.Name,
                        dispatchJournal.Dispatch.Urgency.Name, dispatchJournal.Dispatch.ScheduleDate.ToString("yyyy-MM-dd"), dispatchJournal.Dispatch.EndDate == DateTime.MinValue ? "" : dispatchJournal.Dispatch.EndDate.ToString("yyyy-MM-dd"), dispatchJournal.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "服务凭证.xlsx");
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