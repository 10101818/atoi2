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

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 科室controller
    /// </summary>
    public class DepartmentController : BaseController
    {
        private DepartmentManager departmentManager = new DepartmentManager();

        private DepartmentDao departmentDao = new DepartmentDao();
        /// <summary>
        /// 科室列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartmentList()
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }
        /// <summary>
        /// 获取科室列表信息
        /// </summary>
        /// <param name="departmentTypeID">科室分类编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>科室列表信息</returns>
        public JsonResult QueryDepartmentList(int departmentTypeID, string filterField, string filterText, string field, bool direction, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<DepartmentInfo>> result = new ResultModel<List<DepartmentInfo>>();
            try
            {
                List<DepartmentInfo> infos = new List<DepartmentInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.departmentDao.QueryDepartmentCount(departmentTypeID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.departmentDao.QueryDepartments(departmentTypeID, filterField, filterText, field, direction, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.departmentDao.QueryDepartments(departmentTypeID, filterField, filterText, field, direction, 0, ConstDefinition.PAGE_SIZE);
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
        /// 获取科室信息
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <returns>科室信息</returns>
        public JsonResult QueryDepartment4AutoComplete(string inputText)
        {
           if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
           if (CheckSessionID() == false)
           {
               return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
           }

            ResultModel<List<DepartmentInfo>> result = new ResultModel<List<DepartmentInfo>>();
            try
            {
                List<DepartmentInfo> infos = new List<DepartmentInfo>();

                infos = this.departmentDao.QueryDepartment4AutoComplete(inputText, 0, ConstDefinition.PAGE_SIZE);
                

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
        /// 获取科室信息
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <returns>科室信息</returns>
        public JsonResult QueryDepartment4AutoCompleteVal(string inputText)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<DepartmentInfo>> result = new ResultModel<List<DepartmentInfo>>();
            try
            {
                List<DepartmentInfo> infos = new List<DepartmentInfo>();

                infos = this.departmentDao.QueryDepartment4AutoCompleteVal(inputText, 0, ConstDefinition.PAGE_SIZE);


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
        /// 根据科室编号获取科室信息
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <returns>科室信息</returns>
        public JsonResult GetDepartmentByID(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<DepartmentInfo> result = new ResultModel<DepartmentInfo>();
            try
            {
                result.Data = this.departmentDao.GetDepartmentByID(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 判断科室名称是否存在
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <param name="departmentName">科室名称</param>
        /// <returns>科室名称是否存在</returns>
        public JsonResult CheckDepartmentName(int id, String departmentName)
        {
            if (CheckSession() == false)
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
                result.Data = this.departmentDao.CheckDepartmentName(id, departmentName);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 保存科室信息
        /// </summary>
        /// <param name="info">科室信息</param>
        /// <returns>科室id</returns>
        [HttpPost]
        public JsonResult SaveDepartment(DepartmentInfo info)
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
                info.ID = this.departmentManager.SaveDepartment(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出科室列表
        /// </summary>
        /// <param name="departmentTypeID">科室分类编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <returns>科室列表excel</returns>
        public ActionResult ExportDepartmentList(int departmentTypeID, string filterField, string filterText, string field, bool direction)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<DepartmentInfo>> result = new ResultModel<List<DepartmentInfo>>();
            try
            {
                List<DepartmentInfo> infos = this.departmentDao.QueryDepartments(departmentTypeID, filterField, filterText, field, direction, 0, 0);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("科室系统编号");
                dt.Columns.Add("科室排序");
                dt.Columns.Add("科室名称");
                dt.Columns.Add("科室拼音简称");
                dt.Columns.Add("科室分类");

                foreach (DepartmentInfo info in infos)
                {
                    dt.Rows.Add(info.OID, info.Seq, info.Description, info.Pinyin, info.DepartmentType.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "科室列表.xlsx");
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