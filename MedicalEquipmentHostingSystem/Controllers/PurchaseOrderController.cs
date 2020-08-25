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
    /// 采购单controller
    /// </summary>
    public class PurchaseOrderController : BaseController
    {
        private PurchaseOrderDao purchaseOrderDao = new PurchaseOrderDao();

        private PurchaseOrderManager purchaseOrderManager = new PurchaseOrderManager();

        /// <summary>
        /// 采购单列表页面
        /// </summary>
        /// <returns>采购单列表页面</returns>
        public ActionResult PurchaseOrderList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 采购单详情页面
        /// </summary>
        /// <returns>采购单详情页面</returns>
        public ActionResult PurchaseOrderDetail(int purchaseOrderID)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ViewBag.ID = purchaseOrderID;

            return View();
        }

        /// <summary>
        /// 采购单查看页面
        /// </summary>
        /// <returns>采购单查看页面</returns>
        public ActionResult PurchaseOrderDetailView(int purchaseOrderID)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ViewBag.ID = purchaseOrderID;

            return View();
        }
        /// <summary>
        /// 采购单入库页面
        /// </summary>
        /// <returns>采购单入库页面</returns>
        public ActionResult PurchaseOrderStock(int purchaseOrderID)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ViewBag.ID = purchaseOrderID;

            return View();
        }
        /// <summary>
        /// 获取采购单列表信息
        /// </summary>
        /// <param name="statusID">采购单状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>采购单列表信息</returns>
        public JsonResult QueryPurchaseOrderList(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<PurchaseOrderInfo>> result = new ResultModel<List<PurchaseOrderInfo>>();

            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<PurchaseOrderInfo> infos = new List<PurchaseOrderInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.purchaseOrderDao.QueryPurchaseOrderCount(statusID, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.purchaseOrderDao.QueryPurchaseOrders(statusID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.purchaseOrderDao.QueryPurchaseOrders(statusID, filterField, filterText, sortField, sortDirection, 0, 0);
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
        /// 根据采购单id获取采购单信息
        /// </summary>
        /// <param name="PurchaseOrderID">采购单id</param>
        /// <returns>采购单信息</returns>
        public JsonResult GetPurchaseOrderByID(int PurchaseOrderID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<PurchaseOrderInfo> result = new ResultModel<PurchaseOrderInfo>();
            try
            {
                result.Data = this.purchaseOrderManager.GetPurchaseOrderByID(PurchaseOrderID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存采购单信息
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        [HttpPost]
        public JsonResult SavePurchaseOrder(PurchaseOrderInfo info)
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
                result.Data = this.purchaseOrderManager.SavePurchaseOrder(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 审批通过采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult PassPurchaseOrder(int purchaseOrderID, string comments = "")
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
                this.purchaseOrderManager.PassPurchaseOrder(purchaseOrderID, GetLoginUser(), comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 退回采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult RejectPurchaseOrder(int purchaseOrderID, string comments = "")
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
                this.purchaseOrderManager.RejectPurchaseOrder(purchaseOrderID, GetLoginUser(), comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 终止采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult CancelPurchaseOrder(int purchaseOrderID, string comments = "")
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
                this.purchaseOrderManager.CancelPurchaseOrder(purchaseOrderID, GetLoginUser(), comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 完成采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult EndPurchaseOrder(int purchaseOrderID, string comments = "")
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
                this.purchaseOrderManager.EndPurchaseOrder(purchaseOrderID, GetLoginUser(), comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据采购单id,零件id获取入库零件信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="componentID">零件id</param>
        /// <returns>入库零件信息</returns>
        public JsonResult GetComponent4Inbound(int purchaseOrderID, int componentID)
        {
            if (CheckSession() == false)
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
                result.Data = this.purchaseOrderDao.GetComponent4Inbound(purchaseOrderID, componentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据采购单id,耗材id获取入库耗材信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="consumableID">耗材id</param>
        /// <returns>入库耗材信息</returns>
        public JsonResult GetConsumable4Inbound(int purchaseOrderID, int consumableID)
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
                result.Data = this.purchaseOrderDao.GetConsumable4Inbound(purchaseOrderID, consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 入库采购单信息
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        [HttpPost]
        public JsonResult InboundPurchaseOrder(PurchaseOrderInfo info)
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
                result.Data = this.purchaseOrderManager.InboundPurchaseOrder(info, GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出采购单信息
        /// </summary>
        /// <param name="statusID">状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>采购单列表excel</returns>
        public ActionResult ExportPurchaseOrders(int statusID, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<PurchaseOrderInfo> purchaseOrders = this.purchaseOrderDao.QueryPurchaseOrders(statusID, filterField, filterText, sortField, sortDirection, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("请求人");
                dt.Columns.Add("供应商");
                dt.Columns.Add("采购日期");
                dt.Columns.Add("到货日期");
                dt.Columns.Add("状态");

                foreach (PurchaseOrderInfo purchaseOrder in purchaseOrders)
                {
                    dt.Rows.Add(purchaseOrder.OID, purchaseOrder.User.Name, purchaseOrder.Supplier.Name, purchaseOrder.OrderDate.ToString("yyyy-MM-dd"), purchaseOrder.DueDate.ToString("yyyy-MM-dd"), purchaseOrder.Status.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "采购单列表.xlsx");
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