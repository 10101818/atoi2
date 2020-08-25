using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.Models;
using MedicalEquipmentHostingSystem.App_Start;
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
    /// 供应商controller
    /// </summary>
    public class SupplierController : BaseController
    {
        private SupplierManager supplierManager = new SupplierManager();
        private SupplierDao supplierDao = new SupplierDao();
        private FileDao fileDao = new FileDao();
        private UploadFileManager fileManager = new UploadFileManager();

        /// <summary>
        /// 供应商列表页面
        /// </summary>
        /// <returns>供应商列表页面</returns>
        public ActionResult SupplierList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 供应商详情页面（新增、修改、查看）
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <returns>供应商详情页面（新增、修改、查看）</returns>
        public ActionResult SupplierDetail(int id, bool isReadOnly)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ID = id;
            ViewBag.ReadOnly = isReadOnly;
            return View();
        }

        /// <summary>
        /// 获取供应商列表信息
        /// </summary>
        /// <param name="typeID">供应商类型</param>
        /// <param name="status">供应商状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>供应商列表信息</returns>
        public JsonResult QuerySupplierList(int typeID, int status, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<SupplierInfo>> result = new ResultModel<List<SupplierInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<SupplierInfo> infos = new List<SupplierInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.supplierDao.QuerySupplierCount(typeID,status, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.supplierDao.QuerySupplier(typeID,status, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.supplierDao.QuerySupplier(typeID,status, filterField, filterText, sortField, sortDirection, 0, ConstDefinition.PAGE_SIZE);
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
        /// 获取供应商各个类别数量
        /// </summary>
        /// <returns>供应商各个类别数量</returns>
        public JsonResult GetSupplierCount()
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
                Dictionary<int, int> dicSupplierCount = this.supplierDao.GetSupplierCount();

                Dictionary<string, int> resultData = new Dictionary<string, int>();
                if (dicSupplierCount.ContainsKey(SupplierInfo.SupplierTypes.Manufacturer) == false)
                    resultData.Add("Manufacturer", 0);
                else
                    resultData.Add("Manufacturer", dicSupplierCount[SupplierInfo.SupplierTypes.Manufacturer]);
                if (dicSupplierCount.ContainsKey(SupplierInfo.SupplierTypes.Agent) == false)
                    resultData.Add("Agent", 0);
                else
                    resultData.Add("Agent", dicSupplierCount[SupplierInfo.SupplierTypes.Agent]);
                if (dicSupplierCount.ContainsKey(SupplierInfo.SupplierTypes.Dealer) == false)
                    resultData.Add("Dealer", 0);
                else
                    resultData.Add("Dealer", dicSupplierCount[SupplierInfo.SupplierTypes.Dealer]);
                if (dicSupplierCount.ContainsKey(SupplierInfo.SupplierTypes.Other) == false)
                    resultData.Add("Other", 0);
                else
                    resultData.Add("Other", dicSupplierCount[SupplierInfo.SupplierTypes.Other]);

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
        /// 通过供应商编号获取供应商信息
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <returns>供应商信息</returns>
        public JsonResult GetSupplier(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<SupplierInfo> result = new ResultModel<SupplierInfo>();
            try
            {
                SupplierInfo editSupplierInfo = this.supplierManager.GetSupplier(id);
                result.Data = editSupplierInfo;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断是否有重复的供应商名称
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="Name">供应商名称</param>
        /// <returns>true/false 是否有重复的供应商名称</returns>
        public JsonResult CheckSupplierName(int id, String Name)
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
                result.Data = this.supplierDao.CheckSupplierName(id, Name);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }

        /// <summary>
        /// 保存供应商
        /// </summary>
        /// <param name="info">供应商信息</param>
        /// <returns>供应商信息</returns>
        [HttpPost]
        public JsonResult SaveSupplier(SupplierInfo info)
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
                List<UploadFileInfo> files = GetUploadFilesInSession();

                result.Data = this.supplierManager.SaveSupplier(info, files, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出供应商列表信息
        /// </summary>
        /// <param name="typeID">供应商类型</param>
        /// <param name="status">供应商状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>供应商列表excel</returns>
        public ActionResult ExportSupplierList(int typeID, int status, string filterField, string filterText, string sortField, bool sortDirection)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<SupplierInfo>> result = new ResultModel<List<SupplierInfo>>();
            try
            {
                List<SupplierInfo> infos = null;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                infos = this.supplierDao.QuerySupplier(typeID,status, filterField, filterText, sortField, sortDirection, 0, 0);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("供应商名称");
                dt.Columns.Add("类型");
                dt.Columns.Add("省份");
                dt.Columns.Add("地址");
                dt.Columns.Add("联系人");
                dt.Columns.Add("联系人电话");
                dt.Columns.Add("添加日期");
                dt.Columns.Add("状态");
                foreach (SupplierInfo supplierInfo in infos)
                {
                    dt.Rows.Add(supplierInfo.OID, supplierInfo.Name, supplierInfo.SupplierType.Name,
                         supplierInfo.Province, supplierInfo.Address, supplierInfo.Contact, supplierInfo.ContactMobile, supplierInfo.AddDate.ToString("yyyy-MM-dd"), (supplierInfo.IsActive ? "启用" : "停用"));
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "供应商列表.xlsx");
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