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
    /// 零件库dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class InvComponentDao : BaseDao
    {
        /// <summary>
        /// 获取零件库信息数量
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>零件信息数量</returns>
        public int QueryComponentCount(int statusID, string filterField, string filterText, string eqptID)
        {
            sqlStr = "SELECT COUNT(DISTINCT ic.ID) FROM tblInvComponent ic " +
                    " LEFT JOIN tblEquipment AS e ON ic.EquipmentID = e.ID " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblComponent AS c ON ic.ComponentID = c.ID " +
                    " WHERE 1=1 ";
            if (statusID == 0)
                sqlStr += " AND ic.StatusID <> " + InvComponentInfo.ComponentStatus.Scrap;
            else if (statusID != 0)
                sqlStr += " AND ic.StatusID = " + statusID;

            BaseDao.ProcessFieldFilterValue("e.ID", ref eqptID);
            if (SQLUtil.ConvertInt(eqptID) != 0)
                sqlStr += " AND e.ID = " + SQLUtil.ConvertInt(eqptID);
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
        /// 获取零件库信息
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>零件库信息</returns>
        public List<InvComponentInfo> QueryComponents(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize, string eqptID)
        {
            List<InvComponentInfo> infos = new List<InvComponentInfo>();

            sqlStr = "SELECT ic.*,c.Name AS ComponentName,c.Description AS ComponentDescription,c.TypeID AS ComponentTypeID,e.Name AS EquipmentName,s.Name AS SupplierName FROM tblInvComponent ic" +
                    " LEFT JOIN tblEquipment AS e ON ic.EquipmentID = e.ID " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblComponent AS c ON ic.ComponentID = c.ID " +
                    " WHERE 1=1 ";
            if (statusID == 0)
                sqlStr += " AND ic.StatusID <> " + InvComponentInfo.ComponentStatus.Scrap;
            else if (statusID != 0)
                sqlStr += " AND ic.StatusID = " + statusID;
            BaseDao.ProcessFieldFilterValue("e.ID", ref eqptID);
            if (SQLUtil.ConvertInt(eqptID) != 0)
                sqlStr += " AND e.ID = " + SQLUtil.ConvertInt(eqptID);
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "ic.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

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
        /// 根据采购单ID获取零件
        /// </summary>
        /// <param name="purchaseID">采购单ID</param>
        /// <returns>零件</returns>
        public List<InvComponentInfo> QueryComponentsByPurchaseID(int purchaseID)
        {
            List<InvComponentInfo> infos = new List<InvComponentInfo>();

            sqlStr = "SELECT ic.*,c.Name AS ComponentName,c.Description AS ComponentDescription,c.TypeID AS ComponentTypeID,e.Name AS EquipmentName,s.Name AS SupplierName FROM tblInvComponent ic" +
                    " LEFT JOIN tblEquipment AS e ON ic.EquipmentID = e.ID " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblComponent AS c ON ic.ComponentID = c.ID " +
                    " WHERE ic.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseID;
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
        /// 根据零件id获取零件信息
        /// </summary>
        /// <param name="componentID">零件id</param>
        /// <returns>零件信息</returns>
        public InvComponentInfo GetComponentByID(int componentID)
        {
            sqlStr = "SELECT ic.*,c.Name AS ComponentName,c.Description AS ComponentDescription,c.TypeID AS ComponentTypeID,e.Name AS EquipmentName,e.FujiClass2ID AS EquipmentFujiClass2ID,s.Name AS SupplierName FROM tblInvComponent ic " +
                    " LEFT JOIN tblEquipment AS e ON ic.EquipmentID = e.ID " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblComponent AS c ON ic.ComponentID = c.ID " +
                    " WHERE 1=1 " +
                    " AND ic.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = componentID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new InvComponentInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 判断零件序列号是否重复
        /// </summary>
        /// <param name="id">零件id</param>
        /// <param name="serialCode">零件名称</param>
        /// <returns>是否重复</returns>
        public bool CheckComponentSerialCode(int id, string serialCode)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblInvComponent WHERE ID <> @ID AND UPPER(SerialCode) = UPPER(@SerialCode)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = serialCode;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 添加零件
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        public int AddComponent(InvComponentInfo info)
        {
            sqlStr = "INSERT INTO tblInvComponent(ComponentID,EquipmentID,SerialCode,Specification,Model,SupplierID,Price,PurchaseDate,PurchaseID,Comments,StatusID,AddDate) " +
                    " VALUES(@ComponentID,@EquipmentID,@SerialCode,@Specification,@Model,@SupplierID,@Price,@PurchaseDate,@PurchaseID,@Comments,@StatusID,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Component.ID);
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Equipment.ID);
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Model);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Status.ID);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改零件信息
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        public int UpdateComponent(InvComponentInfo info)
        {
            sqlStr = "UPDATE tblInvComponent Set ComponentID=@ComponentID,EquipmentID=@EquipmentID,SerialCode=@SerialCode,Specification=@Specification, " +
                    " Model=@Model,SupplierID=@SupplierID,Price=@Price,PurchaseDate=@PurchaseDate," +
                    " PurchaseID=@PurchaseID,Comments=@Comments,StatusID=@StatusID,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Component.ID);
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Equipment.ID);
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Model);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Status.ID);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }

        public void UpdateComponentStatus(List<int> componentIDs, int status)
        {
            sqlStr = "UPDATE tblInvComponent SET StatusID = @StatusID,UpdateDate = GetDate() " +
                    " WHERE ID IN (" + string.Join(",", componentIDs) + ")";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = status;
                command.ExecuteNonQuery();
            }
        }
    }
}
