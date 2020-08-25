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
    /// 作业报告controller
    /// </summary>
    public class DispatchReportController : BaseController
    {

        private DispatchManager dispatchManager = new DispatchManager();

        private DispatchDao dispatchDao = new DispatchDao();
        private HistoryDao historyDao = new HistoryDao();
        private FileDao fileDao = new FileDao();
        private DispatchReportDao dispatchReportDao = new DispatchReportDao();

        /// <summary>
        /// 作业报告填写/审批/查看页面
        /// </summary>
        /// <param name="actionName">父级页面名称</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="dispatchReportID">作业报告编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="requestType">派工类型</param>
        /// <returns>作业报告填写/审批/查看页面</returns>
        public ActionResult DispatchReportDetail(string actionName = "", int dispatchID = 0, int dispatchReportID = 0, int dispatchJournalID = 0, int requestType=0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ActionName = actionName;
            ViewBag.DispatchID = dispatchID;
            ViewBag.DispatchJournalID = dispatchJournalID;
            ViewBag.RequestType = requestType;
            if (dispatchReportID == 0) {
                int existsReportID = this.dispatchDao.GetDispatchByID(dispatchID).DispatchReport.ID;
                if (existsReportID == 0)
                {
                    DispatchReportInfo dispatchReport = new DispatchReportInfo();
                    dispatchReport.Type = DispatchReportInfo.DispatchReportTypes.GetDispatchReportType(requestType);
                    dispatchReport.Status.ID = DispatchReportInfo.DispatchReportStatus.New;
                    dispatchReport.Status.Name = LookupManager.GetDispatchDocStatusDesc(dispatchReport.Status.ID);
                    dispatchReport.Dispatch.ID = dispatchID;
                    dispatchReportID = this.dispatchManager.SaveDispatchReport(dispatchReport,GetLoginUser());
                }
                else dispatchReportID = existsReportID;
            }
            ViewBag.ID = dispatchReportID;
            return View();   
        }

        /// <summary>
        /// 作业报告列表信息
        /// </summary>
        /// <param name="status">作业报告状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>作业报告列表信息</returns>
        public JsonResult QueryDispatchReports(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ResultModel<List<DispatchReportInfo>> result = new ResultModel<List<DispatchReportInfo>>();
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
                    int totalNum = this.dispatchDao.QueryDispatchReportsCount(status, urgency, filterField, filterText);

                    result.SetTotalPages(totalNum, pageSize);
                    result.Data = dispatchManager.QueryDispatchReports(status, urgency, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    result.Data = dispatchManager.QueryDispatchReports(status, urgency, filterField, filterText, sortField, sortDirection, 0, pageSize);
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
        /// 获取各个状态的作业报告数量
        /// </summary>
        /// <returns>作业报告已取消、新建、待审批、已审批的数量</returns>
        public JsonResult GetDispatchReportCount()
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
                Dictionary<int, int> dicDispatchReportCount = this.dispatchDao.GetDispatchReportCount();

                Dictionary<string, int> resultData = new Dictionary<string, int>();
                if (dicDispatchReportCount.ContainsKey(DispatchInfo.DocStatus.Cancelled) == false)
                    resultData.Add("Cancelled", 0);
                else
                    resultData.Add("Cancelled", dicDispatchReportCount[DispatchInfo.DocStatus.Cancelled]);
                if (dicDispatchReportCount.ContainsKey(DispatchInfo.DocStatus.New) == false)
                    resultData.Add("New", 0);
                else
                    resultData.Add("New", dicDispatchReportCount[DispatchInfo.DocStatus.New]);
                if (dicDispatchReportCount.ContainsKey(DispatchInfo.DocStatus.Pending) == false)
                    resultData.Add("Pending", 0);
                else
                    resultData.Add("Pending", dicDispatchReportCount[DispatchInfo.DocStatus.Pending]);
                if (dicDispatchReportCount.ContainsKey(DispatchInfo.DocStatus.Approved) == false)
                    resultData.Add("Approved", 0);
                else
                    resultData.Add("Approved", dicDispatchReportCount[DispatchInfo.DocStatus.Approved]);

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
        /// 通过作业报告编号获取作业报告信息
        /// </summary>
        /// <param name="dispatchReportID">作业报告编号</param>
        /// <returns>作业报告信息</returns>
        public JsonResult GetDispatchReportByID(int dispatchReportID)
        {
            ResultModel<DispatchReportInfo> result = new ResultModel<DispatchReportInfo>();
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
                result.Data = this.dispatchManager.GetDispatchReportByID(dispatchReportID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据附件类型及作业报告编号获取附件信息
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="id">作业报告编号</param>
        /// <returns>附件信息</returns>
        public JsonResult GetFileInfo(int objectTypeId, int id)
        {
            ResultModel<UploadFileInfo> result = new ResultModel<UploadFileInfo>();
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
                result.Data = this.fileDao.GetFile(objectTypeId, id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存作业报告信息
        /// </summary>
        /// <param name="dispatchReport">作业报告信息</param>
        /// <returns>保存作业报告信息返回编码</returns>
        [HttpPost]
        public JsonResult SaveDispatchReport(DispatchReportInfo dispatchReport)
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
                DispatchInfo dispatch = this.dispatchManager.GetDispatchByID(dispatchReport.Dispatch.ID);
                if (dispatch.DispatchReport.Status.ID != DispatchInfo.DocStatus.New)
                {
                    result.SetFailed(ResultCodes.ParameterError, "不可重复提交");
                }
                else
                    this.dispatchManager.SaveDispatchReport(dispatchReport, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 审批通过作业报告
        /// </summary>
        /// <param name="info">作业报告信息</param>
        /// <returns>审批通过作业报告返回编码</returns>
        [HttpPost]
        public JsonResult PassDispatchReport(DispatchReportInfo info)
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
                this.dispatchManager.PassDispatchReport(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 审批拒绝作业报告
        /// </summary>
        /// <param name="info">作业报告信息</param>
        /// <returns>审批拒绝作业报告返回编码</returns>
        [HttpPost]
        public JsonResult RejectDispatchReport(DispatchReportInfo info)
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
                this.dispatchManager.RejectDispatchReport(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出作业报告列表信息
        /// </summary>
        /// <param name="status">作业报告状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>作业报告列表信息excel</returns>
        public ActionResult ExportDispatchReports(int status, int urgency, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<DispatchReportInfo> dispatchReports = this.dispatchManager.QueryDispatchReports(status, urgency, filterField, filterText, sortField, sortDirection);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("作业报告编号");
                dt.Columns.Add("请求编号");
                dt.Columns.Add("设备系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("派工类型");
                dt.Columns.Add("紧急程度");
                dt.Columns.Add("派工日期");
                dt.Columns.Add("结束日期");
                dt.Columns.Add("状态");


                foreach (DispatchReportInfo dispatchReport in dispatchReports)
                {
                    dt.Rows.Add(dispatchReport.OID, dispatchReport.Dispatch.Request.OID, dispatchReport.Dispatch.Request.EquipmentOID, dispatchReport.Dispatch.Request.EquipmentName, dispatchReport.Dispatch.RequestType.Name,
                        dispatchReport.Dispatch.Urgency.Name, dispatchReport.Dispatch.ScheduleDate.ToString("yyyy-MM-dd"), dispatchReport.Dispatch.EndDate == DateTime.MinValue ? "" : dispatchReport.Dispatch.EndDate.ToString("yyyy-MM-dd"), dispatchReport.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "作业报告.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region ReportComponent

        public JsonResult CheckComponent(string serialCode, int type)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();

            try
            {
                result.Data = this.dispatchReportDao.CheckComponent(serialCode, type);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        [HttpPost]
        public JsonResult SaveReportComponent(ReportComponentInfo info)
        {
            ResultModel<ReportComponentInfo> result = new ResultModel<ReportComponentInfo>();
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
                List<UploadFileInfo> fileReportComponent = GetUploadFilesInSession();

                if (fileReportComponent != null)
                {
                    foreach (UploadFileInfo file in fileReportComponent)
                    {
                        file.FileContent = ParseBase64String(file.FileContent);
                    }
                }

                info = dispatchManager.SaveReportComponent(info, fileReportComponent);
                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        [HttpPost]
        public JsonResult DeleteReportComponent(int reportComponentID)
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
                this.dispatchManager.DeleteReportComponent(reportComponentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region ReportConsumable
        [HttpPost]
        public JsonResult SaveReportConsumable(ReportConsumableInfo info)
        {
            ResultModel<ReportConsumableInfo> result = new ResultModel<ReportConsumableInfo>();
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
                info = dispatchManager.SaveReportConsumable(info);
                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        public JsonResult CheckConsumable(int dispatchReportID, int consumableID, string lotNum)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();

            try
            {
                result.Data = this.dispatchReportDao.CheckConsumable(dispatchReportID, consumableID, lotNum);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        [HttpPost]
        public JsonResult DeleteReportConsumable(int reportConsumableID)
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
                this.dispatchManager.DeleteReportConsumable(reportConsumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region ReportService
        [HttpPost]
        public JsonResult SaveReportService(ReportServiceInfo info)
        {
            ResultModel<ReportServiceInfo> result = new ResultModel<ReportServiceInfo>();
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
                info = dispatchManager.SaveReportService(info);
                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        public JsonResult CheckService(int dispatchReportID, int serviceID)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();

            try
            {
                result.Data = this.dispatchReportDao.CheckService(dispatchReportID, serviceID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        [HttpPost]
        public JsonResult DeleteReportService(int reportServiceID)
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
                this.dispatchManager.DeleteReportService(reportServiceID);
            }
            catch (Exception ex) 
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

    }
}