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
    /// 耗材controller
    /// </summary>
    public class ConsumableController : BaseController
    {
        private ConsumableManager consumableManager = new ConsumableManager();
        private ConsumableDao consumableDao = new ConsumableDao();

        /// <summary>
        /// 耗材列表页面
        /// </summary>
        /// <returns>耗材列表页面</returns>
        public ActionResult ConsumableList()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        /// <summary>
        /// 获取耗材列表信息
        /// </summary>
        /// <param name="statusID">耗材状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>耗材列表信息</returns>
        public JsonResult QueryConsumableList(int statusID, string filterField, string filterText, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ConsumableInfo>> result = new ResultModel<List<ConsumableInfo>>();

            try
            {
                List<ConsumableInfo> infos = new List<ConsumableInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.consumableDao.QueryConsumableCount(statusID, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.consumableDao.QueryConsumables(statusID, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.consumableDao.QueryConsumables(statusID, filterField, filterText, 0, 0);
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
            ResultModel<ConsumableInfo> result = new ResultModel<ConsumableInfo>();
            try
            {
                result.Data = this.consumableDao.GetConsumableByID(consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据富士II类id获取耗材信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>耗材信息</returns>
        public JsonResult QueryConsumablesByFujiClass2ID(int fujiClass2ID, bool isIncluded = false, string filterField = "", string filterText = "", int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ConsumableInfo>> result = new ResultModel<List<ConsumableInfo>>();
            try
            {
                List<ConsumableInfo> infos = new List<ConsumableInfo>();
                if (currentPage != 0)
                {
                    int totalRecord = this.consumableDao.QueryConsumableCountByFujiClass2ID(fujiClass2ID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.consumableDao.QueryConsumablesByFujiClass2ID(fujiClass2ID, isIncluded, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                    infos = this.consumableDao.QueryConsumablesByFujiClass2ID(fujiClass2ID, isIncluded, filterField, filterText);

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
        /// 保存耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        [HttpPost]
        public JsonResult SaveConsumable(ConsumableInfo info)
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
                result.Data = this.consumableManager.SaveConsumable(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 富士II类保存耗材信息
        /// </summary>
        /// <param name="infos">耗材信息</param>
        /// <returns>返回信息</returns>
        [HttpPost]
        public JsonResult SaveConsumables(List<ConsumableInfo> infos)
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
                this.consumableManager.UpdateConsumables(infos, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据耗材id删除耗材信息
        /// </summary>
        /// <param name="consumableID">耗材id</param>
        /// <returns>返回信息</returns>
        [HttpPost]
        public JsonResult DeleteConsumableByID(int consumableID)
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
                this.consumableDao.DeleteConsumableByID(consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断耗材名称是否重复
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <param name="name">耗材名称</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <returns>耗材名称是否重复</returns>
        public JsonResult CheckConsumableName(int id, string name, int fujiClass2ID)
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
                result.Data = this.consumableDao.CheckConsumableName(id, name, fujiClass2ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 耗材是否被使用
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <returns>耗材是否被使用</returns>
        public JsonResult CheckConsumableInUse(int id)
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
                result.Data = this.consumableDao.CheckConsumableInUse(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出耗材列表
        /// </summary>
        /// <param name="statusID">耗材状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>耗材定义excel</returns>
        public ActionResult ExportConsumables(int statusID, string filterField, string filterText)
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
                List<ConsumableInfo> consumables = this.consumableDao.QueryConsumables(statusID, filterField, filterText, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("富士II类");
                dt.Columns.Add("简称");
                dt.Columns.Add("描述");
                dt.Columns.Add("类型");
                dt.Columns.Add("标准单价(元)");
                dt.Columns.Add("是否参与估值");
                dt.Columns.Add("更换频率(次/年)");
                dt.Columns.Add("单次保养耗材成本(元)");
                dt.Columns.Add("状态");

                foreach (ConsumableInfo consumable in consumables)
                {
                    dt.Rows.Add(consumable.FujiClass2.Name, consumable.Name, consumable.Description, consumable.Type.Name, consumable.StdPrice, consumable.IsIncluded ? "是" : "否", consumable.ReplaceTimes, consumable.CostPer, consumable.IsActive ? "启用" : "停用");
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "耗材定义列表.xlsx");
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