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
    /// 零件controller
    /// </summary>
    public class ComponentController : BaseController
    {
        private ComponentManager componentManager = new ComponentManager();
        private FujiClassManager fujiManager = new FujiClassManager();
        private ComponentDao componentDao = new ComponentDao();

        /// <summary>
        /// 零件列表页面
        /// </summary>
        /// <returns>零件列表页面</returns>
        public ActionResult ComponentList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 获取零件列表信息
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件列表信息</returns>
        public JsonResult QueryComponentList(int statusID, string filterField, string filterText, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if(CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ComponentInfo>> result = new ResultModel<List<ComponentInfo>>();

            try
            {
                List<ComponentInfo> infos = new List<ComponentInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.componentDao.QueryComponentCount(statusID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.componentDao.QueryComponents(statusID, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.componentDao.QueryComponents(statusID, filterField, filterText, 0, 0);
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
        /// 根据富士II类获取零件信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="componentTypeID">零件类型</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件信息</returns>
        public JsonResult QueryComponentsByFujiClass2ID(int fujiClass2ID, int componentTypeID = -1, bool isIncluded = false, string filterField = "", string filterText = "", int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ComponentInfo>> result = new ResultModel<List<ComponentInfo>>();
            try
            {
                List<ComponentInfo> infos = new List<ComponentInfo>();
                if(currentPage != 0)
                {
                    int totalRecord = this.componentDao.QueryComponentCountByFujiClass2ID(fujiClass2ID, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.componentDao.QueryComponentsByFujiClass2ID(fujiClass2ID, componentTypeID, false, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                    infos = this.componentManager.QueryComponentsByFujiClass2ID(fujiClass2ID, componentTypeID, isIncluded, filterField, filterText);

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
            ResultModel<ComponentInfo> result = new ResultModel<ComponentInfo>();
            try
            {
                result.Data = this.componentDao.GetComponentByID(componentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        public JsonResult GetComponentsRateByFujiClass2(int fujiClass2ID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<ComponentInfo>> result = new ResultModel<List<ComponentInfo>>();
            try
            {
                result.Data = this.fujiManager.GetComponentsRateByFujiClass2(fujiClass2ID);
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
        public JsonResult SaveComponent(ComponentInfo info)
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
                result.Data = this.componentManager.SaveComponent(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 富士II类保存零件信息
        /// </summary>
        /// <param name="infos">零件信息</param>
        /// <returns>返回信息</returns>
        [HttpPost]
        public JsonResult SaveComponents(List<ComponentInfo> infos)
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
                this.componentManager.UpdateComponents(infos,GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据零件id删除零件
        /// </summary>
        /// <param name="componentID">零件id</param>
        /// <returns>返回信息</returns>
        [HttpPost]
        public JsonResult DeleteComponentByID(int componentID)
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
                this.componentManager.DeleteComponentByID(componentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 判断零件名称是否重复
        /// </summary>
        /// <param name="id">零件id</param>
        /// <param name="name">零件名称</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <returns>零件名称是否重复</returns>
        public JsonResult CheckComponentName(int id, string name, int fujiClass2ID)
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
                result.Data = this.componentDao.CheckComponentName(id, name, fujiClass2ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断零件是否在使用中
        /// </summary>
        /// <param name="id">零件id</param>
        /// <returns>零件是否在使用中</returns>
        public JsonResult CheckComponentInUse(int id)
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
                result.Data = this.componentDao.CheckComponentInUse(id);
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
        /// <returns>零件列表excel</returns>
        public ActionResult ExportComponents(int statusID, string filterField, string filterText)
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
                List<ComponentInfo> components = this.componentDao.QueryComponents(statusID, filterField, filterText, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("富士II类");
                dt.Columns.Add("简称");
                dt.Columns.Add("描述");
                dt.Columns.Add("类型");
                dt.Columns.Add("标准单价(元)");
                dt.Columns.Add("是否参与估值");
                dt.Columns.Add("标准使用量");
                dt.Columns.Add("状态");

                foreach(ComponentInfo component in components)
                {
                    dt.Rows.Add(component.FujiClass2.Name, component.Name, component.Description, component.Type.Name, component.StdPrice, component.IsIncluded ? "是" : "否", component.Usage, component.IsActive ? "启用" : "停用");
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "零件定义列表.xlsx");
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