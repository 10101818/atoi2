using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 采购单manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class PurchaseOrderManager
    {
        private PurchaseOrderDao purchaseOrderDao = new PurchaseOrderDao();
        private HistoryDao historyDao = new HistoryDao();

        private FujiClassDao fujiClassDao = new FujiClassDao();
        private EquipmentDao equipmentDao = new EquipmentDao();
        private InvComponentDao invComponentDao = new InvComponentDao();
        private InvConsumableDao invConsumableDao = new InvConsumableDao();
        private InvServiceDao invServiceDao = new InvServiceDao();

        /// <summary>
        /// 保存采购单信息
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>采购单id</returns>
        [TransactionAspect]
        public int SavePurchaseOrder(PurchaseOrderInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.purchaseOrderDao.AddPurchaseOrder(info);
            }
            else
            {
                this.purchaseOrderDao.UpdatePurchaseOrder(info);

                this.purchaseOrderDao.DeleteComponent(info.ID);
                this.purchaseOrderDao.DeleteConsumable(info.ID);
                this.purchaseOrderDao.DeleteService(info.ID);

            }

            if (info.Components != null)
            {
                foreach (InvComponentInfo component in info.Components)
                {
                    this.purchaseOrderDao.AddComponent(info.ID, component);
                }
            }
            if (info.Consumables != null)
            {
                foreach (InvConsumableInfo consumable in info.Consumables)
                {
                    this.purchaseOrderDao.AddConsumable(info.ID, consumable);
                }
            }
            if (info.Services != null)
            {
                foreach (InvServiceInfo service in info.Services)
                {
                    this.purchaseOrderDao.AddService(info.ID, service);
                }
            }

            if (info.Status.ID == PurchaseOrderInfo.PurchaseOrderStatus.Pending)
            {
                HistoryInfo history = new HistoryInfo(info.ID, ObjectTypes.PurchaseOrder, userInfo.ID, PurchaseOrderInfo.Actions.Submit);
                this.historyDao.AddHistory(history);
            }

            return info.ID;
        }

        /// <summary>
        /// 根据ID获取采购单信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单ID</param>
        /// <returns>采购单信息</returns>
        public PurchaseOrderInfo GetPurchaseOrderByID(int purchaseOrderID)
        {
            PurchaseOrderInfo info = this.purchaseOrderDao.GetPurchaseOrderByID(purchaseOrderID);
            info.Components = new List<InvComponentInfo>();
            info.Consumables = new List<InvConsumableInfo>();

            List<InvComponentInfo> components = this.purchaseOrderDao.GetComponents(purchaseOrderID);

            info.Components = components;
            foreach (InvComponentInfo component in components)
            {
                component.Equipment = this.equipmentDao.GetEquipmentByID(component.Equipment.ID);
            }

            List<InvConsumableInfo> consumables = this.purchaseOrderDao.GetConsumables(purchaseOrderID);

            info.Consumables = consumables;

            info.Services = this.purchaseOrderDao.GetServices(purchaseOrderID);
            foreach (InvServiceInfo service in info.Services)
            {
                service.FujiClass2 = this.fujiClassDao.GetFujiClass2ByID(service.FujiClass2.ID);
            }

            List<HistoryInfo> histories = this.historyDao.GetHistories(ObjectTypes.PurchaseOrder, purchaseOrderID);
            if (histories != null && histories.Count > 0)
            {
                foreach (HistoryInfo history in histories)
                {
                    history.Action.Name = PurchaseOrderInfo.Actions.GetDesc(history.Action.ID);
                }
                info.Histories = histories;
                info.SetHis4Comments();
            }

            return info;
        }

        /// <summary>
        /// 审批通过采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="user">操作的用户信息</param>
        /// <param name="comments">审批备注</param>
        [TransactionAspect]
        public void PassPurchaseOrder(int purchaseOrderID, UserInfo user, string comments = "")
        {
            this.purchaseOrderDao.UpdatePurchaseOrderStatus(purchaseOrderID, PurchaseOrderInfo.PurchaseOrderStatus.Stocking, comments);

            HistoryInfo history = new HistoryInfo(purchaseOrderID, ObjectTypes.PurchaseOrder, user.ID, PurchaseOrderInfo.Actions.Pass, comments);
            this.historyDao.AddHistory(history);
        }

        /// <summary>
        /// 审批退回采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="user">操作的用户信息</param>
        /// <param name="comments">审批备注</param>
        [TransactionAspect]
        public void RejectPurchaseOrder(int purchaseOrderID, UserInfo user, string comments = "")
        {
            this.purchaseOrderDao.UpdatePurchaseOrderStatus(purchaseOrderID, PurchaseOrderInfo.PurchaseOrderStatus.New, comments);

            HistoryInfo history = new HistoryInfo(purchaseOrderID, ObjectTypes.PurchaseOrder, user.ID, PurchaseOrderInfo.Actions.Reject, comments);
            this.historyDao.AddHistory(history);
        }

        /// <summary>
        /// 终止采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="user">操作的用户信息</param>
        /// <param name="comments">审批备注</param>
        [TransactionAspect]
        public void CancelPurchaseOrder(int purchaseOrderID, UserInfo user, string comments = "")
        {
            this.purchaseOrderDao.UpdatePurchaseOrderStatus(purchaseOrderID, PurchaseOrderInfo.PurchaseOrderStatus.Cancelled, comments);

            HistoryInfo history = new HistoryInfo(purchaseOrderID, ObjectTypes.PurchaseOrder, user.ID, PurchaseOrderInfo.Actions.Cancelled, comments);
            this.historyDao.AddHistory(history);
        }

        /// <summary>
        /// 完成采购单
        /// </summary>
        /// <param name="purchaseOrderID">采购单编号</param>
        /// <param name="user">操作的用户信息</param>
        /// <param name="comments">审批备注</param>
        [TransactionAspect]
        public void EndPurchaseOrder(int purchaseOrderID, UserInfo user, string comments = "")
        {
            this.purchaseOrderDao.UpdatePurchaseOrderStatus(purchaseOrderID, PurchaseOrderInfo.PurchaseOrderStatus.Ended, comments);

            HistoryInfo history = new HistoryInfo(purchaseOrderID, ObjectTypes.PurchaseOrder, user.ID, PurchaseOrderInfo.Actions.End, comments);
            this.historyDao.AddHistory(history);
        }

        /// <summary>
        /// 采购单入库
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>采购单id</returns>
        [TransactionAspect]
        public int InboundPurchaseOrder(PurchaseOrderInfo info, UserInfo userInfo)
        {
            if (info.Components != null && info.Components.Count > 0)
            {
                this.purchaseOrderDao.UpdatePurchaseComponentInboundQty(info.Components[0].ID, info.Components.Count);
                foreach (InvComponentInfo component in info.Components)
                {
                    component.Supplier = info.Supplier;
                    component.PurchaseDate = DateTime.Now;
                    component.Purchase.ID = info.ID;
                    component.Status.ID = InvComponentInfo.ComponentStatus.InStock;
                    this.invComponentDao.AddComponent(component);
                }
            }
            if (info.Consumables != null)
            {
                foreach (InvConsumableInfo consumable in info.Consumables)
                {
                    consumable.ReceiveQty = consumable.Qty;
                    consumable.AvaibleQty = consumable.Qty;
                    this.purchaseOrderDao.UpdatePurchaseConsumableInboundQty(consumable.ID, consumable.ReceiveQty);
                    consumable.Supplier = info.Supplier;
                    consumable.PurchaseDate = DateTime.Now;
                    consumable.Purchase.ID = info.ID;
                    this.invConsumableDao.AddConsumable(consumable);
                }
            }
            if (info.Services != null)
            {
                foreach (InvServiceInfo service in info.Services)
                {
                    this.purchaseOrderDao.UpdateService(service);
                    service.Supplier = info.Supplier;
                    service.PurchaseDate = DateTime.Now;
                    service.AvaibleTimes = service.TotalTimes;
                    this.invServiceDao.AddService(service);
                }
            }

            HistoryInfo history = new HistoryInfo(info.ID, ObjectTypes.PurchaseOrder, userInfo.ID, PurchaseOrderInfo.Actions.Inbound);
            this.historyDao.AddHistory(history);

            return info.ID;
        }

        /// <summary>
        /// 根据ID获取采购单信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单ID</param>
        /// <returns>采购单信息</returns>
        public PurchaseOrderInfo GetPurchaseOrder4Ended(int purchaseOrderID)
        {
            PurchaseOrderInfo info = this.purchaseOrderDao.GetPurchaseOrderByID(purchaseOrderID);

            info.Components = this.invComponentDao.QueryComponentsByPurchaseID(purchaseOrderID);

            info.Consumables = this.invConsumableDao.QueryConsumablesByPurchaseID(purchaseOrderID);

            info.Services = this.invServiceDao.QueryServicesByPurchaseID(purchaseOrderID);

            List<HistoryInfo> histories = this.historyDao.GetHistories(ObjectTypes.PurchaseOrder, purchaseOrderID);
            if (histories != null && histories.Count > 0)
            {
                foreach (HistoryInfo history in histories)
                {
                    history.Action.Name = PurchaseOrderInfo.Actions.GetDesc(history.Action.ID);
                }
                info.Histories = histories;
                info.SetHis4Comments();
            }

            return info;
        }
    }
}
