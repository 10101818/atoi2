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
using System.Data.SqlTypes;


namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class AuditController : BaseController
    {
        private AuditDao auditLogDao = new AuditDao();
        private UserDao userDao = new UserDao();

        private AuditManager auditLogManager = new AuditManager();

        /// <summary>
        /// 日志列表页面
        /// </summary>
        /// <returns>日志列表页面</returns>
        public ActionResult SysAuditLogList(int objectTypeID = 0, int objectID = 0)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            if (objectID > 0)
            {
                ViewBag.ObjectTypeID = objectTypeID;
                ViewBag.ObjectID = objectID;
                ViewBag.FilterField = "h.ObjectID";
                ViewBag.StartDate = DateTime.MinValue;
                ViewBag.EndDate = DateTime.MaxValue;
            }

            return View();
        }

        /// <summary>
        /// 获取日志列表信息
        /// </summary>
        /// <param name="objectTypeID">操作对象</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="startDate">请求开始日期</param>
        /// <param name="endDate">请求结束日期</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>日志列表信息</returns>
        public JsonResult QuerySysAuditLogList(int objectTypeID, string filterField, string filterText, string field, bool direction, string startDate, string endDate, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<AuditHdrInfo>> result = new ResultModel<List<AuditHdrInfo>>();
            try
            { 
                DateTime _startDate = (DateTime)SqlDateTime.MinValue;
                DateTime _endDate = DateTime.Now;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);

                if (!string.IsNullOrEmpty(startDate))
                    _startDate = Convert.ToDateTime(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    _endDate = Convert.ToDateTime(endDate);
                List<AuditHdrInfo> infos = new List<AuditHdrInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.auditLogDao.QuerySysAuditLogsCount(objectTypeID, filterField, filterText, _startDate, _endDate);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.auditLogDao.QuerySysAuditLogs(objectTypeID, filterField, filterText, field, direction, _startDate, _endDate, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.auditLogDao.QuerySysAuditLogs(objectTypeID, filterField, filterText, field, direction, _startDate, _endDate, 0, 0);
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
        /// 通过日志编号获取日志信息
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="type">父对象类型ID</param>
        /// <returns>用户信息</returns>
        public JsonResult GetSysAuditLog(int id,int type)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<AuditHdrInfo> result = new ResultModel<AuditHdrInfo>();
            try
            {                
                result.Data = this.auditLogManager.GetAuditInfo(id, type);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出日志列表
        /// </summary>
        /// <param name="objectTypeID">角色ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="startDate">请求开始日期</param>
        /// <param name="endDate">请求结束日期</param>
        /// <returns>日志列表excel</returns>
        public ActionResult ExportSysAuditLogList(int objectTypeID, string filterField, string filterText, string field, bool direction, string startDate, string endDate )
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
                DateTime _startDate = (DateTime)SqlDateTime.MinValue;
                DateTime _endDate = DateTime.Now;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);

                if (!string.IsNullOrEmpty(startDate))
                    _startDate = Convert.ToDateTime(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    _endDate = Convert.ToDateTime(endDate);
                UserInfo userInfo = GetLoginUser();
                List<AuditHdrInfo> infos = this.auditLogDao.QuerySysAuditLogs(objectTypeID, filterField, filterText, field, direction, _startDate, _endDate, 0, 0);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("用户姓名");
                dt.Columns.Add("操作时间");
                dt.Columns.Add("操作类型");
                dt.Columns.Add("对象类型");
                dt.Columns.Add("对象系统编号");

                foreach (AuditHdrInfo info in infos)
                {
                    dt.Rows.Add(info.OID, info.TransUser.Name, info.UpdateDate.ToString("yyyy-MM-dd"), info.Operation.Name, info.ObjectType.Description, info.ObjectOID);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "日志列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
    }
}