using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
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
    /// 日志dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class AuditDao : BaseDao
    {
        #region 'auditDetail'
        /// <summary>
        /// 根据日志ID获取日志信息
        /// </summary>
        /// <param name="id">日志ID</param>
        /// <param name="type">父对象类型ID</param>
        /// <returns>日志信息</returns>
        public List<AuditDetailInfo> GetAuditDetailLog(int id,int type)
        {
            sqlStr = "SELECT distinct(d.FieldName),lkp.Seq, d.*,lkp.FieldDesc as FieldNameDesc FROM tblAuditDetail AS d " +
                    " LEFT JOIN lkpCustRptTemplateField lkp on d.FieldName = lkp.FieldName And " + CustomReportInfo.CustRptType.GetSqlStr4ObjectType("lkp", type) +
                    " WHERE d.AuditID = @ID" + 
                    " ORDER BY lkp.Seq ASC";
              
             using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = type; 
                return GetList<AuditDetailInfo>(command);
            }
        }
        
        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="auditID">日志id</param>
        /// <param name="dt">日志dt</param>
        /// <returns>日志信息</returns>
        public void AddAuditDetail(int auditID, DataTable dt)
        {
            sqlStr = "INSERT INTO tblAuditDetail(AuditID,FieldName,OldValue,NewValue) " +
                     " VALUES(@AuditID,@FieldName,@OldValue,@NewValue);" +
                     " SELECT @@IDENTITY";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@AuditID", SqlDbType.Int).Value = auditID;

                parameter = command.Parameters.Add("@FieldName", SqlDbType.VarChar);
                parameter.SourceColumn = "FieldName";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@OldValue", SqlDbType.NVarChar);
                parameter.SourceColumn = "OldValue";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@NewValue", SqlDbType.NVarChar);
                parameter.SourceColumn = "NewValue";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 大数据量新增日志
        /// </summary>
        /// <param name="dt">日志信息</param>
        public void AddAuditDetail( DataTable dt)
        {
            sqlStr = "INSERT INTO tblAuditDetail(AuditID,FieldName,OldValue,NewValue) " +
                     " VALUES(@AuditID,@FieldName,@OldValue,@NewValue);" +
                     " SELECT @@IDENTITY";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            { 
                parameter = command.Parameters.Add("@AuditID", SqlDbType.Int);
                parameter.SourceColumn = "AuditID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@FieldName", SqlDbType.VarChar);
                parameter.SourceColumn = "FieldName";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@OldValue", SqlDbType.NVarChar);
                parameter.SourceColumn = "OldValue";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@NewValue", SqlDbType.NVarChar);
                parameter.SourceColumn = "NewValue";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        #endregion
        #region 'tblAuditHdr'
        /// <summary>
        /// 获取日志数量
        /// </summary>
        /// <param name="objectTypeID">操作类型编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="startDate">搜索开始日期</param>
        /// <param name="endDate">搜索截止日期</param>
        /// <returns>日志数量</returns>
        public int QuerySysAuditLogsCount(int objectTypeID, string filterField, string filterText, DateTime startDate, DateTime endDate)
        {
            sqlStr = "SELECT COUNT(DISTINCT h.ID) FROM tblAuditHdr AS h " +
                    " LEFT JOIN tblUser AS u ON u.ID = h.UserID";
            sqlStr += " WHERE 1=1 ";
            if (objectTypeID != -1)
                sqlStr += " AND h.ObjectTypeID = " + objectTypeID;
            if (startDate != DateTime.MinValue)
                sqlStr += " AND h.UpdateDate >= @StartDate ";
            if (endDate != DateTime.MinValue)
                sqlStr += " AND h.UpdateDate < @EndDate ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (startDate != DateTime.MinValue) command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                if (endDate != DateTime.MinValue) command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate.AddDays(1);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取日志列表信息
        /// </summary>
        /// <param name="objectTypeID">操作类型编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="startDate">搜索开始日期</param>
        /// <param name="endDate">搜索截止日期</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>日志列表信息</returns>
        public List<AuditHdrInfo> QuerySysAuditLogs(int objectTypeID, string filterField, string filterText, string sortField, bool sortDirection, DateTime startDate, DateTime endDate, int curRowNum, int pageSize)
        {
            List<AuditHdrInfo> infos = new List<AuditHdrInfo>();

            sqlStr = "SELECT DISTINCT * FROM tblAuditHdr AS h " +
                    " LEFT JOIN tblUser AS u ON u.ID = h.UserID";
            sqlStr += " WHERE 1=1 ";
            if (objectTypeID != -1)
                sqlStr += " AND h.ObjectTypeID = " + objectTypeID;
            if (startDate != DateTime.MinValue)
                sqlStr += " AND h.UpdateDate >= @StartDate ";
            if (endDate != DateTime.MinValue)
                sqlStr += " AND h.UpdateDate < @EndDate ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "h.ID");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (startDate != DateTime.MinValue) command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                if (endDate != DateTime.MinValue) command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate.AddDays(1);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new AuditHdrInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据日志ID获取日志信息
        /// </summary>
        /// <param name="id">日志ID</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo GetAuditHdrLog(int id)
        {
            sqlStr = "SELECT * FROM tblAuditHdr AS h " +
                    " LEFT JOIN tblUser AS u ON u.ID = h.UserID" +
                    " WHERE h.ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new AuditHdrInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="info">日志信息</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo AddAuditHdr(AuditHdrInfo info)
        {
            sqlStr = "INSERT INTO tblAuditHdr(UserID,Operation,ObjectTypeID,ObjectID,UpdateDate) " +
                     " VALUES(@UserID,@Operation,@ObjectTypeID,@ObjectID,GETDATE() );" +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = info.TransUser.ID;
                command.Parameters.Add("@Operation", SqlDbType.Int).Value = info.Operation.ID;
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = info.ObjectType.ID;
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = info.ObjectID;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info;
            }
        }

        /// <summary>
        /// 批量新增日志
        /// </summary>
        /// <param name="dt">日志信息</param> 
        public void AddAuditHdr(DataTable dt)
        {
            sqlStr = "INSERT INTO tblAuditHdr(UserID,Operation,ObjectTypeID,ObjectID,UpdateDate) " +
                     " VALUES(@UserID,@Operation,@ObjectTypeID,@ObjectID,GETDATE() );" +
                     " SELECT @ID =@@IDENTITY";
             
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, SourceColumn = "UserID", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Operation", SqlDbType = SqlDbType.Int, SourceColumn = "Operation", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ObjectTypeID", SqlDbType = SqlDbType.Int, SourceColumn = "ObjectTypeID", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ObjectID", SqlDbType = SqlDbType.Int, SourceColumn = "ObjectID", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original , Direction = ParameterDirection.Output });

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }

        #endregion

    }
}
