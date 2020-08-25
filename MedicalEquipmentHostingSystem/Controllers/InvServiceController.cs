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
    /// 服务controller
    /// </summary>
    public class InvServiceController : BaseController
    {
        private InvServiceDao invServiceDao = new InvServiceDao();

        private InvServiceManager invServiceManager = new InvServiceManager();

        /// <summary>
        /// 服务库列表页面
        /// </summary>
        /// <returns>服务库列表页面</returns>
        public ActionResult InvServiceList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 服务库详情页面
        /// </summary>
        /// <returns>服务库详情页面</returns>
        public ActionResult InvServiceDetail(int serviceID)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ViewBag.ID = serviceID;

            return View();
        }

        /// <summary>
        /// 获取服务库列表信息
        /// </summary>
        /// <param name="statusID">服务状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>服务库列表信息</returns>
        public JsonResult QueryServiceList(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE, int fujiClass2ID = 0)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<InvServiceInfo>> result = new ResultModel<List<InvServiceInfo>>();

            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<InvServiceInfo> infos = new List<InvServiceInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.invServiceDao.QueryServiceCount(statusID, filterField, filterText, fujiClass2ID);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.invServiceDao.QueryServices(statusID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize, fujiClass2ID);
                    }
                }
                else
                {
                    infos = this.invServiceDao.QueryServices(statusID, filterField, filterText, sortField, sortDirection, 0, 0,fujiClass2ID);
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
        /// 根据服务id获取服务信息
        /// </summary>
        /// <param name="serviceID">服务id</param>
        /// <returns>服务信息</returns>
        public JsonResult GetServiceByID(int serviceID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<InvServiceInfo> result = new ResultModel<InvServiceInfo>();
            try
            {
                result.Data = this.invServiceDao.GetServiceByID(serviceID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存服务信息
        /// </summary>
        /// <param name="info">服务信息</param>
        /// <returns>服务id</returns>
        [HttpPost]
        public JsonResult SaveService(InvServiceInfo info)
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
                result.Data = this.invServiceManager.SaveService(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出服务信息
        /// </summary>
        /// <param name="statusID">状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>服务列表excel</returns>
        public ActionResult ExportServices(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int fujiClass2ID = 0)
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
                List<InvServiceInfo> invServices = this.invServiceDao.QueryServices(statusID, filterField, filterText, sortField, sortDirection, 0, 0, fujiClass2ID);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("服务名称");
                dt.Columns.Add("关联富士II类");
                dt.Columns.Add("供应商");
                dt.Columns.Add("服务次数");
                dt.Columns.Add("服务开始日期");
                dt.Columns.Add("服务结束日期");
                dt.Columns.Add("金额");
                dt.Columns.Add("剩余服务次数");
                dt.Columns.Add("采购单号");
                dt.Columns.Add("状态");

                foreach (InvServiceInfo invService in invServices)
                {
                    dt.Rows.Add(invService.OID, invService.Name, invService.FujiClass2.Name, invService.Supplier.Name, invService.TotalTimes,
                        invService.AddDate.ToString("yyyy-MM-dd"), invService.EndDate.ToString("yyyy-MM-dd"), invService.Price, invService.AvaibleTimes, invService.Purchase.ID == 0 ? "" : invService.Purchase.Name, invService.Status);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "服务库列表.xlsx");
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