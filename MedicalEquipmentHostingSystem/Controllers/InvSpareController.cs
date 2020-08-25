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
    /// 备用机controller
    /// </summary>
    public class InvSpareController : BaseController
    {
        private InvSpareDao invSpareDao = new InvSpareDao();

        private InvSpareManager invSpareManager = new InvSpareManager();

        /// <summary>
        /// 备用机库列表页面
        /// </summary>
        /// <returns>备用机库列表页面</returns>
        public ActionResult InvSpareList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 获取备用机库列表信息
        /// </summary>
        /// <param name="statusID">备用机状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>备用机库列表信息</returns>
        public JsonResult QuerySpareList(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<InvSpareInfo>> result = new ResultModel<List<InvSpareInfo>>();

            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<InvSpareInfo> infos = new List<InvSpareInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.invSpareDao.QuerySpareCount(statusID, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.invSpareDao.QuerySpares(statusID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.invSpareDao.QuerySpares(statusID, filterField, filterText, sortField, sortDirection, 0, 0);
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
        /// 根据备用机id获取备用机信息
        /// </summary>
        /// <param name="spareID">备用机id</param>
        /// <returns>备用机信息</returns>
        public JsonResult GetSpareByID(int spareID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<InvSpareInfo> result = new ResultModel<InvSpareInfo>();
            try
            {
                result.Data = this.invSpareDao.GetSpareByID(spareID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存备用机信息
        /// </summary>
        /// <param name="info">备用机信息</param>
        /// <returns>备用机id</returns>
        [HttpPost]
        public JsonResult SaveSpare(InvSpareInfo info)
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
                result.Data = this.invSpareManager.SaveSpare(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断备用机序列号是否重复
        /// </summary>
        /// <param name="id">备用机id</param>
        /// <param name="fujiClass2ID">备用机富士II类id</param>
        /// <param name="serialCode">备用机序列号</param>
        /// <param name="startDate">备用机开始日期</param>
        /// <returns>备用机序列号是否重复</returns>
        public JsonResult CheckSpareSerialCode(int id, int fujiClass2ID, string serialCode, DateTime startDate)
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
                result.Data = this.invSpareDao.CheckSpareSerialCode(id, fujiClass2ID, serialCode, startDate);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出备用机信息
        /// </summary>
        /// <param name="statusID">状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>备用机列表excel</returns>
        public ActionResult ExportSpares(int statusID, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<InvSpareInfo> invSpares = this.invSpareDao.QuerySpares(statusID, filterField, filterText, sortField, sortDirection, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("富士II类");
                dt.Columns.Add("序列号");
                dt.Columns.Add("月租（元）");
                dt.Columns.Add("开始日期");
                dt.Columns.Add("结束日期");

                foreach (InvSpareInfo invSpare in invSpares)
                {
                    dt.Rows.Add(invSpare.OID, invSpare.FujiClass2.Name, invSpare.SerialCode, invSpare.Price, invSpare.AddDate.ToString("yyyy-MM-dd"), invSpare.EndDate.ToString("yyyy-MM-dd"));
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "备用机库列表.xlsx");
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