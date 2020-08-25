using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// PurchaseOrderController
    /// </summary>
    public class PurchaseOrderController : BaseController
    {
        private PurchaseOrderDao purchaseOrderDao = new PurchaseOrderDao();
        private ComponentDao componentDao = new ComponentDao();
        private ConsumableDao consumableDao = new ConsumableDao();
        private InvComponentDao invComponentDao = new InvComponentDao();
        private InvConsumableDao invConsumableDao = new InvConsumableDao();

        private PurchaseOrderManager purchaseOrderManager = new PurchaseOrderManager();
        private ComponentManager componentManager = new ComponentManager();
        private ConsumableManager consumableManager = new ConsumableManager();

        /// <summary>
        /// 获取采购单列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="statusID">采购单状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>采购单列表信息</returns>
        public JsonResult QueryPurchaseOrderList(int userID, string sessionID, int statusID = 0, string filterField = "", string filterText = "", string sortField = "po.ID", bool sortDirection = false, int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<PurchaseOrderInfo>> result = new ServiceResultModel<List<PurchaseOrderInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                    result.Data = this.purchaseOrderDao.QueryPurchaseOrders(statusID, filterField, filterText, sortField, sortDirection, curRowNum, pageSize);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据采购单id获取采购单信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <returns>采购单信息</returns>
        public JsonResult GetPurchaseOrderByID(int userID, string sessionID, int purchaseOrderID)
        {
            ServiceResultModel<PurchaseOrderInfo> result = new ServiceResultModel<PurchaseOrderInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.purchaseOrderManager.GetPurchaseOrderByID(purchaseOrderID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存采购单信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        [HttpPost]
        public JsonResult SavePurchaseOrder(int userID, string sessionID, PurchaseOrderInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.purchaseOrderManager.SavePurchaseOrder(info, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        [HttpPost]
        public JsonResult SaveComponent(int userID, string sessionID, ComponentInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (this.componentDao.CheckComponentName(info.ID, info.Name, info.FujiClass2.ID))
                {
                    result.SetFailed(ResultCodes.ParameterError, "名称已存在");
                    return MyJson(result, JsonRequestBehavior.AllowGet);
                }
                info.IsActive = true;
                result.Data = this.componentManager.SaveComponent(info, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        [HttpPost]
        public JsonResult SaveConsumable(int userID, string sessionID, ConsumableInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (this.consumableDao.CheckConsumableName(info.ID, info.Name, info.FujiClass2.ID))
                {
                    result.SetFailed(ResultCodes.ParameterError, "名称已存在");
                    return MyJson(result, JsonRequestBehavior.AllowGet);
                }
                info.IsActive = true;
                result.Data = this.consumableManager.SaveConsumable(info, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审批通过采购单
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult PassPurchaseOrder(int userID, string sessionID, int purchaseOrderID, string comments = "")
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                this.purchaseOrderManager.PassPurchaseOrder(purchaseOrderID, user, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 退回采购单
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult RejectPurchaseOrder(int userID, string sessionID, int purchaseOrderID, string comments = "")
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                this.purchaseOrderManager.RejectPurchaseOrder(purchaseOrderID, user, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 终止采购单
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult CancelPurchaseOrder(int userID, string sessionID, int purchaseOrderID, string comments = "")
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                this.purchaseOrderManager.CancelPurchaseOrder(purchaseOrderID, user, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 完成采购单
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="comments">审批备注</param>
        [HttpPost]
        public JsonResult EndPurchaseOrder(int userID, string sessionID, int purchaseOrderID, string comments = "")
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                this.purchaseOrderManager.EndPurchaseOrder(purchaseOrderID, user, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 入库采购单信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        [HttpPost]
        public JsonResult InboundPurchaseOrder(int userID, string sessionID, PurchaseOrderInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (info.Components != null && info.Components.Count > 0)
                {
                    foreach (InvComponentInfo component in info.Components)
                    {
                        if (this.invComponentDao.CheckComponentSerialCode(component.ID, component.SerialCode))
                        {
                            result.SetFailed(ResultCodes.ParameterError, "序列号已存在");
                            return MyJson(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (info.Consumables != null && info.Consumables.Count > 0)
                {
                    foreach (InvConsumableInfo consumable in info.Consumables)
                    {
                        if (this.invConsumableDao.CheckConsumableLotNum(consumable.ID, consumable.LotNum, consumable.Consumable.ID))
                        {
                            result.SetFailed(ResultCodes.ParameterError, "批次号已存在");
                            return MyJson(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                result.Data = this.purchaseOrderManager.InboundPurchaseOrder(info, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

    }
}