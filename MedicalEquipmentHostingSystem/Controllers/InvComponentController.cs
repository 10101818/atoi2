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
    /// 零件库controller
    /// </summary>
    public class InvComponentController : BaseController
    {
        private InvComponentDao invComponentDao = new InvComponentDao();

        private InvComponentManager invComponentManager = new InvComponentManager();

        /// <summary>
        /// 零件库列表页面
        /// </summary>
        /// <returns>零件库列表页面</returns>
        public ActionResult InvComponentList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 获取零件库列表信息
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件库列表信息</returns>
        public JsonResult QueryComponentList(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE, string eqptID = "")
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<InvComponentInfo>> result = new ResultModel<List<InvComponentInfo>>();

            try
            {
                List<InvComponentInfo> infos = new List<InvComponentInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.invComponentDao.QueryComponentCount(statusID, filterField, filterText, eqptID);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.invComponentDao.QueryComponents(statusID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize, eqptID);
                    }
                }
                else
                {
                    infos = this.invComponentDao.QueryComponents(statusID, filterField, filterText, sortField, sortDirection, 0, 0, eqptID);
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
        /// 根据零件id获取零件信息
        /// </summary>
        /// <param name="componentID">零件id</param>
        /// <returns>零件信息</returns>
        public JsonResult GetComponentByID(int componentID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<InvComponentInfo> result = new ResultModel<InvComponentInfo>();
            try
            {
                result.Data = this.invComponentDao.GetComponentByID(componentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        [HttpPost]
        public JsonResult SaveComponent(InvComponentInfo info)
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
                result.Data = this.invComponentManager.SaveComponent(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断零件序列号是否重复
        /// </summary>
        /// <param name="id">零件id</param>
        /// <param name="serialCode">零件序列号</param>
        /// <returns>零件序列号是否重复</returns>
        public JsonResult CheckComponentSerialCode(int id, string serialCode)
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
                result.Data = this.invComponentDao.CheckComponentSerialCode(id, serialCode);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出零件信息
        /// </summary>
        /// <param name="statusID">状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>零件列表excel</returns>
        public ActionResult ExportComponents(int statusID, string filterField, string filterText, string sortField, bool sortDirection, string eqptID = "")
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
                List<InvComponentInfo> invComponents = this.invComponentDao.QueryComponents(statusID, filterField, filterText, sortField, sortDirection, 0, 0, eqptID);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("序列号");
                dt.Columns.Add("简称");
                dt.Columns.Add("描述");
                dt.Columns.Add("类型");
                dt.Columns.Add("设备系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("供应商");
                dt.Columns.Add("单价（元）");
                dt.Columns.Add("购入日期");
                dt.Columns.Add("采购单号");
                dt.Columns.Add("状态");

                foreach (InvComponentInfo invComponent in invComponents)
                {
                    dt.Rows.Add(invComponent.OID, invComponent.SerialCode, invComponent.Component.Name, invComponent.Component.Description, invComponent.Component.Type.Name,
                        invComponent.Equipment.OID, invComponent.Equipment.Name, invComponent.Supplier.Name, invComponent.Price, invComponent.PurchaseDate.ToString("yyyy-MM-dd"), invComponent.Purchase.ID == 0 ? "" : invComponent.Purchase.Name, invComponent.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "零件库列表.xlsx");
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