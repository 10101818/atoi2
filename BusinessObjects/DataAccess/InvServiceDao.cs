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
    /// 服务库dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class InvServiceDao : BaseDao
    {
        /// <summary>
        /// 获取服务库信息数量
        /// </summary>
        /// <param name="statusID">服务状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>服务信息数量</returns>
        public int QueryServiceCount(int statusID, string filterField, string filterText, int fujiClass2ID)
        {
            sqlStr = "SELECT COUNT(DISTINCT se.ID) FROM tblInvService se " +
                    " LEFT JOIN tblFujiClass2 AS f ON se.FujiClass2ID = f.ID " +
                    " LEFT JOIN tblSupplier AS s ON se.SupplierID = s.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0) sqlStr += InvServiceInfo.Statuses.GetSqlFilter(statusID);
            if (fujiClass2ID != 0) sqlStr += " AND f.ID = " + fujiClass2ID;

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
        /// 获取服务库信息
        /// </summary>
        /// <param name="statusID">服务状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>服务库信息</returns>
        public List<InvServiceInfo> QueryServices(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize, int fujiClass2ID)
        {
            List<InvServiceInfo> infos = new List<InvServiceInfo>();

            sqlStr = "SELECT se.*,f.Name AS FujiClass2Name,s.Name AS SupplierName FROM tblInvService se" +
                    " LEFT JOIN tblFujiClass2 AS f ON se.FujiClass2ID = f.ID " +
                    " LEFT JOIN tblSupplier AS s ON se.SupplierID = s.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0) sqlStr += InvServiceInfo.Statuses.GetSqlFilter(statusID);
            if (fujiClass2ID != 0) sqlStr += " AND f.ID = " + fujiClass2ID;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "se.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

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


        public List<InvServiceInfo> QueryServicesByPurchaseID(int purchaseID)
        {
            List<InvServiceInfo> infos = new List<InvServiceInfo>();

            sqlStr = "SELECT se.*,f.Name AS FujiClass2Name,s.Name AS SupplierName FROM tblInvService se" +
                    " LEFT JOIN tblFujiClass2 AS f ON se.FujiClass2ID = f.ID " +
                    " LEFT JOIN tblSupplier AS s ON se.SupplierID = s.ID " +
                    " WHERE se.PurchaseID = @PurchaseID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = purchaseID;

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

        /// <summary>
        /// 根据服务id获取服务信息
        /// </summary>
        /// <param name="ServiceID">服务id</param>
        /// <returns>服务信息</returns>
        public InvServiceInfo GetServiceByID(int ServiceID)
        {
            sqlStr = "SELECT se.*,f.Name AS FujiClass2Name,s.Name AS SupplierName FROM tblInvService se " +
                    " LEFT JOIN tblFujiClass2 AS f ON se.FujiClass2ID = f.ID " +
                    " LEFT JOIN tblSupplier AS s ON se.SupplierID = s.ID " +
                    " WHERE 1=1 " +
                    " AND se.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = ServiceID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new InvServiceInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="info">服务信息</param>
        /// <returns>服务id</returns>
        public int AddService(InvServiceInfo info)
        {
            sqlStr = "INSERT INTO tblInvService(FujiClass2ID,Name,TotalTimes,Price,StartDate,EndDate,SupplierID,PurchaseID,PurchaseDate,Comments,AvaibleTimes,AddDate) " +
                    " VALUES(@FujiClass2ID,@Name,@TotalTimes,@Price,@StartDate,@EndDate,@SupplierID,@PurchaseID,@PurchaseDate,@Comments,@AvaibleTimes,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@TotalTimes", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.TotalTimes);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.EndDate);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@AvaibleTimes", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.AvaibleTimes);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改服务信息
        /// </summary>
        /// <param name="info">服务信息</param>
        /// <returns>服务id</returns>
        public int UpdateService(InvServiceInfo info)
        {
            sqlStr = "UPDATE tblInvService Set FujiClass2ID=@FujiClass2ID,Name=@Name,TotalTimes=@TotalTimes,Price=@Price,StartDate=@StartDate,EndDate=@EndDate, " +
                    " SupplierID=@SupplierID,PurchaseID=@PurchaseID,PurchaseDate=@PurchaseDate,Comments=@Comments,AvaibleTimes=@AvaibleTimes,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@TotalTimes", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.TotalTimes);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.EndDate);
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Supplier.ID);
                command.Parameters.Add("@PurchaseID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Purchase.ID);
                command.Parameters.Add("@PurchaseDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.PurchaseDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Comments);
                command.Parameters.Add("@AvaibleTimes", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.AvaibleTimes);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }


        public void UpdateServiceTimes(List<int> serviceIDs)
        {
            sqlStr = "UPDATE tblInvService SET AvaibleTimes = AvaibleTimes-1, UpdateDate = GetDate() " +
                    " WHERE ID IN(" + string.Join(",",serviceIDs) + " )";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
