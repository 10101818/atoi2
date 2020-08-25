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
    /// 备用机库dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class InvSpareDao : BaseDao
    {
        /// <summary>
        /// 获取备用机库信息数量
        /// </summary>
        /// <param name="statusID">备用机状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>备用机信息数量</returns>
        public int QuerySpareCount(int statusID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT sp.ID) FROM tblInvSpare sp " +
                    " LEFT JOIN tblFujiClass2 AS f ON sp.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0) sqlStr += InvSpareInfo.SpareStatus.GetSqlFilter(statusID);

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
        /// 获取备用机库信息
        /// </summary>
        /// <param name="statusID">备用机状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>备用机库信息</returns>
        public List<InvSpareInfo> QuerySpares(int statusID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<InvSpareInfo> infos = new List<InvSpareInfo>();

            sqlStr = "SELECT sp.*,f.Name AS FujiClass2Name FROM tblInvSpare sp" +
                    " LEFT JOIN tblFujiClass2 AS f ON sp.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (statusID != 0) sqlStr += InvSpareInfo.SpareStatus.GetSqlFilter(statusID);

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "sp.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new InvSpareInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据备用机id获取备用机信息
        /// </summary>
        /// <param name="SpareID">备用机id</param>
        /// <returns>备用机信息</returns>
        public InvSpareInfo GetSpareByID(int SpareID)
        {
            sqlStr = "SELECT sp.*,f.Name AS FujiClass2Name FROM tblInvSpare sp " +
                    " LEFT JOIN tblFujiClass2 AS f ON sp.FujiClass2ID = f.ID " +
                    " WHERE 1=1 " +
                    " AND sp.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SpareID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new InvSpareInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 判断备用机序列号是否重复
        /// </summary>
        /// <param name="id">备用机id</param>
        /// <param name="fujiClass2ID">备用机富士II类id</param>
        /// <param name="serialCode">备用机序列号</param>
        /// <param name="startDate">备用机开始日期</param>
        /// <returns>是否重复</returns>
        public bool CheckSpareSerialCode(int id, int fujiClass2ID, string serialCode, DateTime startDate)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblInvSpare WHERE ID <> @ID AND (UPPER(SerialCode) = UPPER(@SerialCode) AND FujiClass2ID =@FujiClass2ID AND StartDate = @StartDate)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = serialCode;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = fujiClass2ID;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 添加备用机
        /// </summary>
        /// <param name="info">备用机信息</param>
        /// <returns>备用机id</returns>
        public int AddSpare(InvSpareInfo info)
        {
            sqlStr = "INSERT INTO tblInvSpare(FujiClass2ID,SerialCode,Price,StartDate,EndDate,AddDate) " +
                    " VALUES(@FujiClass2ID,@SerialCode,@Price,@StartDate,@EndDate,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.EndDate);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改备用机信息
        /// </summary>
        /// <param name="info">备用机信息</param>
        /// <returns>备用机id</returns>
        public int UpdateSpare(InvSpareInfo info)
        {
            sqlStr = "UPDATE tblInvSpare Set FujiClass2ID=@FujiClass2ID,SerialCode=@SerialCode,Price=@Price,StartDate=@StartDate,EndDate=@EndDate,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.SerialCode);
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = info.Price;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(info.EndDate);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }

    }
}
