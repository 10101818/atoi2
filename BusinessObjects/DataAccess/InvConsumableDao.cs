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
    /// 耗材库dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class InvConsumableDao : BaseDao
    {
        /// <summary>
        /// 获取耗材库信息数量
        /// </summary>
        /// <param name="fujiClass2ID">富士II类ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>耗材信息数量</returns>
        public int QueryConsumableCount(int fujiClass2ID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT ic.ID) FROM tblInvConsumable ic " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable AS c ON ic.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (fujiClass2ID != 0)
                sqlStr += " AND c.FujiClass2ID = " + fujiClass2ID;
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
        /// 获取耗材库信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>耗材库信息</returns>
        public List<InvConsumableInfo> QueryConsumables(int fujiClass2ID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<InvConsumableInfo> infos = new List<InvConsumableInfo>();

            sqlStr = "SELECT ic.*,f.Name AS FujiClass2Name,c.Name AS ConsumableName,c.Description AS ConsumableDescription,s.Name AS SupplierName FROM tblInvConsumable ic" +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable AS c ON ic.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (fujiClass2ID != 0)
                sqlStr += " AND c.FujiClass2ID = " + fujiClass2ID;
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
                        infos.Add(new InvConsumableInfo(dr));
                    }
                }
            }

            return infos;
        }

        public List<InvConsumableInfo> QueryConsumablesByPurchaseID(int purchaseID)
        {
            List<InvConsumableInfo> infos = new List<InvConsumableInfo>();

            sqlStr = "SELECT ic.*,f.Name AS FujiClass2Name,c.Name AS ConsumableName,c.Description AS ConsumableDescription,s.Name AS SupplierName FROM tblInvConsumable ic" +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable AS c ON ic.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE ic.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseID;
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
        /// 根据耗材id获取耗材信息
        /// </summary>
        /// <param name="consumableID">耗材id</param>
        /// <returns>耗材信息</returns>
        public InvConsumableInfo GetConsumableByID(int consumableID)
        {
            sqlStr = "SELECT ic.*,f.ID AS FujiClass2ID,f.Name AS FujiClass2Name,c.Name AS ConsumableName,c.Description AS ConsumableDescription,s.Name AS SupplierName FROM tblInvConsumable ic " +
                    " LEFT JOIN tblSupplier AS s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable AS c ON ic.ConsumableID = c.ID " +
                    " LEFT JOIN tblFujiClass2 AS f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 " +
                    " AND ic.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = consumableID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new InvConsumableInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 判断耗材批次号是否重复
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <param name="lotNum">耗材名称</param>
        /// <returns>是否重复</returns>
        public bool CheckConsumableLotNum(int id, string lotNum, int consumableID)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblInvConsumable WHERE ID <> @ID AND UPPER(LotNum) = UPPER(@LotNum) AND ConsumableID = @ConsumableID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@LotNum", SqlDbType.NVarChar).Value = lotNum;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = consumableID;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 添加耗材
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        public int AddConsumable(InvConsumableInfo info)
        {
            sqlStr = "INSERT INTO tblInvConsumable(ConsumableID,LotNum,Specification,Model,SupplierID,Price,ReceiveQty,PurchaseDate,PurchaseID,AvaibleQty,Comments,AddDate) " +
                    " VALUES(@ConsumableID,@LotNum,@Specification,@Model,@SupplierID,@Price,@ReceiveQty,@PurchaseDate,@PurchaseID,@ReceiveQty,@Comments,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Consumable.ID);
                command.Parameters.Add("@LotNum", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.LotNum);
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Model);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@ReceiveQty", SqlDbType.Decimal).Value = info.ReceiveQty;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        public int UpdateConsumable(InvConsumableInfo info)
        {
            sqlStr = "UPDATE tblInvConsumable Set ConsumableID=@ConsumableID,LotNum=@LotNum,Specification=@Specification, " +
                    " Model=@Model,SupplierID=@SupplierID,Price=@Price,ReceiveQty=@ReceiveQty,PurchaseDate=@PurchaseDate," +
                    " PurchaseID=@PurchaseID,Comments=@Comments,AvaibleQty=@AvaibleQty,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Consumable.ID);
                command.Parameters.Add("@LotNum", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.LotNum);
                command.Parameters.Add("@Specification", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Specification);
                command.Parameters.Add("@Model", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Model);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@ReceiveQty", SqlDbType.Decimal).Value = info.ReceiveQty;
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@AvaibleQty", SqlDbType.Decimal).Value = info.AvaibleQty;
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }


        public void UpdateConsumableQty(int consumableID, double usedQty)
        {
            sqlStr = "UPDATE tblInvConsumable SET AvaibleQty = AvaibleQty-@usedQty,UpdateDate = GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = consumableID;
                command.Parameters.Add("@usedQty", SqlDbType.Decimal).Value = usedQty;
                command.ExecuteNonQuery();
            }
        }
    }
}
