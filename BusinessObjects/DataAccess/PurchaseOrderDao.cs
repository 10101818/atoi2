using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 采购单dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class PurchaseOrderDao : BaseDao
    {
        /// <summary>
        /// 获取采购单信息数量
        /// </summary>
        /// <param name="statusID">采购单状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>采购单信息数量</returns>
        public int QueryPurchaseOrderCount(int statusID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT po.ID) FROM tblPurchaseOrder po " +
                    " LEFT JOIN tblUser AS u ON po.UserID = u.ID " +
                    " LEFT JOIN tblSupplier AS s ON po.SupplierID = s.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0)
                sqlStr += " AND po.StatusID = " + statusID;
            else
                sqlStr += " AND po.StatusID <> " + PurchaseOrderInfo.PurchaseOrderStatus.Cancelled;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }

        /// <summary>
        /// 获取采购单信息
        /// </summary>
        /// <param name="statusID">采购单状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>采购单信息</returns>
        public List<PurchaseOrderInfo> QueryPurchaseOrders(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<PurchaseOrderInfo> infos = new List<PurchaseOrderInfo>();

            sqlStr = "SELECT po.*,u.Name AS UserName,s.Name AS SupplierName FROM tblPurchaseOrder po" +
                    " LEFT JOIN tblUser AS u ON po.UserID = u.ID " +
                    " LEFT JOIN tblSupplier AS s ON po.SupplierID = s.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0)
                sqlStr += " AND po.StatusID = " + statusID;
            else
                sqlStr += " AND po.StatusID <> " + PurchaseOrderInfo.PurchaseOrderStatus.Cancelled;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "po.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new PurchaseOrderInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据采购单id获取采购单信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <returns>采购单信息</returns>
        public PurchaseOrderInfo GetPurchaseOrderByID(int purchaseOrderID)
        {
            sqlStr = "SELECT po.*,u.Name AS UserName,s.Name AS SupplierName FROM tblPurchaseOrder po " +
                    " LEFT JOIN tblUser AS u ON po.UserID = u.ID " +
                    " LEFT JOIN tblSupplier AS s ON po.SupplierID = s.ID " +
                    " WHERE 1=1 " +
                    " AND po.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = purchaseOrderID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new PurchaseOrderInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 根据采购单id获取入库零件信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="componentID">零件id</param>
        /// <returns>入库零件信息</returns>
        public List<InvComponentInfo> GetComponent4Inbound(int purchaseOrderID, int componentID)
        {
            List<InvComponentInfo> infos = new List<InvComponentInfo>();

            sqlStr = "SELECT ic.*,c.Name AS ComponentName,c.Description AS ComponentDescription,c.TypeID AS ComponentTypeID,e.Name AS EquipmentName,s.Name AS SupplierName FROM tblInvComponent ic" +
                    " LEFT JOIN tblEquipment AS e ON ic.EquipmentID = e.ID " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblComponent AS c ON ic.ComponentID = c.ID " +
                    " WHERE ic.PurchaseID = @PurchaseID and ic.ComponentID = @ComponentID";
                    
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = componentID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvComponentInfo(dr));
                    }
                }
            }
            return infos;
        }

        /// <summary>
        /// 根据采购单id获取入库耗材信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="consumableID">耗材id</param>
        /// <returns>入库耗材信息</returns>
        public List<InvConsumableInfo> GetConsumable4Inbound(int purchaseOrderID, int consumableID)
        {
            List<InvConsumableInfo> infos = new List<InvConsumableInfo>();

            sqlStr = "SELECT ic.*,f.Name AS FujiClass2Name,c.Name AS ConsumableName,c.Description AS ConsumableDescription,s.Name AS SupplierName FROM tblInvConsumable ic" +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable AS c ON ic.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE ic.PurchaseID = @PurchaseID and ic.ConsumableID = @ConsumableID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = consumableID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvConsumableInfo(dr));
                    }
                }
            }
            return infos;
        }

        /// <summary>
        /// 添加采购单
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        public int AddPurchaseOrder(PurchaseOrderInfo info)
        {
            sqlStr = "INSERT INTO tblPurchaseOrder(UserID,SupplierID,OrderDate,DueDate,Comments,StatusID,AddDate) " +
                    " VALUES(@UserID,@SupplierID,@OrderDate,@DueDate,@Comments,@StatusID,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.User.ID);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.OrderDate);
                command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.DueDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Status.ID);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改采购单信息
        /// </summary>
        /// <param name="info">采购单信息</param>
        /// <returns>采购单id</returns>
        public int UpdatePurchaseOrder(PurchaseOrderInfo info)
        {
            sqlStr = "UPDATE tblPurchaseOrder Set UserID=@UserID,SupplierID=@SupplierID,OrderDate=@OrderDate,DueDate=@DueDate,Comments=@Comments,StatusID=@StatusID,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.User.ID);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.OrderDate);
                command.Parameters.Add("@DueDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.DueDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Status.ID);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }

        /// <summary>
        /// 更新采购单状态
        /// </summary>
        /// <param name="purchaseOrderID">请求ID</param>
        /// <param name="statusID">采购单状态ID</param>
        /// <param name="comments">审批备注</param>
        public void UpdatePurchaseOrderStatus(int purchaseOrderID, int statusID, string comments)
        {
            sqlStr = "UPDATE tblPurchaseOrder SET StatusID=@StatusID, FujiComments=@FujiComments WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = comments;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = purchaseOrderID;
                command.ExecuteNonQuery();
            }
        }

        #region component
        /// <summary>
        /// 添加采购单零件
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="componentID">零件id</param>
        public void AddComponent(int purchaseOrderID, InvComponentInfo componentInfo)
        {
            sqlStr = "INSERT INTO tblPurchaseComponent (PurchaseID,ComponentID,EquipmentID,Specification,Model,Price,Qty,InboundQty) " +
                     " VALUES(@PurchaseID,@ComponentID,@EquipmentID,@Specification,@Model,@Price,@Qty,0); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = componentInfo.Equipment.ID;
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = componentInfo.Component.ID;
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(componentInfo.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(componentInfo.Model);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = componentInfo.Price;
                command.Parameters.Add("@Qty", SqlDbType.Int).Value = componentInfo.Qty;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除零件信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        public void DeleteComponent(int purchaseOrderID)
        {
            sqlStr = " DELETE FROM tblPurchaseComponent WHERE PurchaseID = @PurchaseID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新零件入库数量
        /// </summary>
        /// <param name="id">零件ID</param>
        /// <param name="inboundQty">入库数量</param>
        public void UpdatePurchaseComponentInboundQty(int id, int inboundQty)
        {
            sqlStr = "UPDATE tblPurchaseComponent Set InboundQty += @InboundQty " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@InboundQty", SqlDbType.Int).Value = SQLUtil.ConvertInt(inboundQty);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取采购单零件信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <returns>零件信息</returns>
        public List<InvComponentInfo> GetComponents(int purchaseOrderID)
        {
            List<InvComponentInfo> infos = new List<InvComponentInfo>();

            sqlStr = "SELECT pc.*,c.Name AS ComponentName,c.Description AS ComponentDescription,c.TypeID AS ComponentTypeID,e.Name AS EquipmentName FROM tblPurchaseComponent pc " +
                    " LEFT JOIN tblComponent AS c ON c.ID = pc.ComponentID " +
                    " LEFT JOIN tblEquipment AS e ON pc.EquipmentID = e.ID " +
                    " WHERE pc.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvComponentInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion

        #region consumable
        /// <summary>
        /// 添加采购单耗材
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="consumableID">耗材id</param>
        public void AddConsumable(int purchaseOrderID, InvConsumableInfo consumableInfo)
        {
            sqlStr = "INSERT INTO tblPurchaseConsumable (PurchaseID,ConsumableID,Specification,Model,Price,Qty,InboundQty) " +
                     " VALUES(@PurchaseID,@ConsumableID,@Specification,@Model,@Price,@Qty,0); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = consumableInfo.Consumable.ID;
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(consumableInfo.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(consumableInfo.Model);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = consumableInfo.Price;
                command.Parameters.Add("@Qty", SqlDbType.Decimal).Value = consumableInfo.Qty;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除耗材信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        public void DeleteConsumable(int purchaseOrderID)
        {
            sqlStr = " DELETE FROM tblPurchaseConsumable WHERE PurchaseID = @PurchaseID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新耗材入库数量
        /// </summary>
        /// <param name="id">耗材ID</param>
        /// <param name="inboundQty">入库数量</param>
        public void UpdatePurchaseConsumableInboundQty(int id, double inboundQty)
        {
            sqlStr = "UPDATE tblPurchaseConsumable Set InboundQty += @InboundQty " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@InboundQty", SqlDbType.Int).Value = inboundQty;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取采购单耗材信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <returns>耗材信息</returns>
        public List<InvConsumableInfo> GetConsumables(int purchaseOrderID)
        {
            List<InvConsumableInfo> infos = new List<InvConsumableInfo>();

            sqlStr = "SELECT pc.*,f.ID AS FujiClass2ID,f.Name AS FujiClass2Name,c.Name AS ConsumableName,c.Description AS ConsumableDescription FROM tblPurchaseConsumable pc " +
                    " LEFT JOIN tblConsumable AS c ON pc.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE pc.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvConsumableInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion

        #region service
        /// <summary>
        /// 添加采购单服务
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <param name="serviceID">服务id</param>
        public void AddService(int purchaseOrderID, InvServiceInfo serviceInfo)
        {
            sqlStr = "INSERT INTO tblPurchaseService (PurchaseID,FujiClass2ID,Name,TotalTimes,Price,StartDate,EndDate) " +
                     " VALUES(@PurchaseID,@FujiClass2ID,@Name,@TotalTimes,@Price,@StartDate,@EndDate); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = serviceInfo.FujiClass2.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(serviceInfo.Name);
                command.Parameters.Add("@TotalTimes", SqlDbType.Int).Value = serviceInfo.TotalTimes;
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = serviceInfo.Price;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(serviceInfo.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(serviceInfo.EndDate);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateService(InvServiceInfo service)
        {
            sqlStr = "UPDATE tblPurchaseService Set TotalTimes = @TotalTimes " +
                    " WHERE PurchaseID = @PurchaseID AND FujiClass2ID = @FujiClass2ID AND Name = @Name ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@TotalTimes", SqlDbType.Int).Value = service.TotalTimes;
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = service.Purchase.ID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = service.FujiClass2.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = service.Name;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 删除服务信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        public void DeleteService(int purchaseOrderID)
        {
            sqlStr = " DELETE FROM tblPurchaseService WHERE PurchaseID = @PurchaseID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取采购单服务信息
        /// </summary>
        /// <param name="purchaseOrderID">采购单id</param>
        /// <returns>服务信息</returns>
        public List<InvServiceInfo> GetServices(int purchaseOrderID)
        {
            List<InvServiceInfo> infos = new List<InvServiceInfo>();

            sqlStr = "SELECT ps.*,f.Name AS FujiClass2Name,Case WHEN se.ID IS NULL THEN 0 ELSE 1 END AS Inbounded FROM tblPurchaseService ps " +
                    " LEFT JOIN tblFujiClass2 AS f ON ps.FujiClass2ID = f.ID " +
                    " LEFT JOIN tblInvService AS se ON ps.PurchaseID = se.PurchaseID AND se.Name = ps.Name AND se.FujiClass2ID = ps.FujiClass2ID " +
                    " WHERE ps.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseOrderID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvServiceInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion
    }
}
