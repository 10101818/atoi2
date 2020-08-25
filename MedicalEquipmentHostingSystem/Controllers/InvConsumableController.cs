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
    /// 耗材库controller
    /// </summary>
    public class InvConsumableController : BaseController
    {
        private InvConsumableDao invConsumableDao = new InvConsumableDao();

        private InvConsumableManager invConsumableManager = new InvConsumableManager();

        /// <summary>
        /// 耗材库列表页面
        /// </summary>
        /// <returns>耗材库列表页面</returns>
        public ActionResult InvConsumableList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 获取耗材库列表信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>耗材库列表信息</returns>
        public JsonResult QueryConsumableList(int fujiClass2ID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<InvConsumableInfo>> result = new ResultModel<List<InvConsumableInfo>>();

            try
            {
                List<InvConsumableInfo> infos = new List<InvConsumableInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.invConsumableDao.QueryConsumableCount(fujiClass2ID, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.invConsumableDao.QueryConsumables(fujiClass2ID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.invConsumableDao.QueryConsumables(fujiClass2ID, filterField, filterText, sortField, sortDirection, 0, 0);
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
        /// 根据耗材id获取耗材信息
        /// </summary>
        /// <param name="consumableID">耗材id</param>
        /// <returns>耗材信息</returns>
        public JsonResult GetConsumableByID(int consumableID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<InvConsumableInfo> result = new ResultModel<InvConsumableInfo>();
            try
            {
                result.Data = this.invConsumableDao.GetConsumableByID(consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        [HttpPost]
        public JsonResult SaveConsumable(InvConsumableInfo info)
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
                result.Data = this.invConsumableManager.SaveConsumable(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断耗材批次号是否重复
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <param name="lotNum">耗材批次号</param>
        /// <returns>耗材批次号是否重复</returns>
        public JsonResult CheckConsumableLotNum(int id, string lotNum, int consumableID)
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
                result.Data = this.invConsumableDao.CheckConsumableLotNum(id, lotNum, consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出耗材信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>耗材列表excel</returns>
        public ActionResult ExportConsumables(int fujiClass2ID, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<InvConsumableInfo> invConsumables = this.invConsumableDao.QueryConsumables(fujiClass2ID, filterField, filterText, sortField, sortDirection, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("批次号");
                dt.Columns.Add("简称");
                dt.Columns.Add("描述");
                dt.Columns.Add("供应商");
                dt.Columns.Add("富士II类");
                dt.Columns.Add("单价（元）");
                dt.Columns.Add("购入日期");
                dt.Columns.Add("采购单号");
                dt.Columns.Add("数量");

                foreach (InvConsumableInfo invConsumable in invConsumables)
                {
                    dt.Rows.Add(invConsumable.OID, invConsumable.LotNum, invConsumable.Consumable.Name, invConsumable.Consumable.Description, invConsumable.Supplier.Name,
                        invConsumable.Consumable.FujiClass2.Name, invConsumable.Price, invConsumable.PurchaseDate.ToString("yyyy-MM-dd"), invConsumable.Purchase.ID == 0 ? "" : invConsumable.Purchase.Name, invConsumable.AvaibleQty);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "耗材库列表.xlsx");
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