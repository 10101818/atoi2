using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using PostSharp.Extensibility;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 广播dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class NoticeDao : BaseDao
    {
        /// <summary>
        /// 获取广播数量
        /// </summary>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>广播数量</returns>
        public int QueryNoticeCount(string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT ID) FROM tblNotice  ";
            sqlStr += " WHERE 1=1 ";
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
        /// 获取广播列表
        /// </summary>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>广播列表</returns>
        public List<NoticeInfo> QueryNotices(string filterField, string filterText, int curRowNum, int pageSize)
        {
            List<NoticeInfo> infos = new List<NoticeInfo>();

            sqlStr = "SELECT DISTINCT * FROM tblNotice ";
            sqlStr += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += " ORDER BY IsLoop DESC , CreatedDate DESC, ID";

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new NoticeInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据广播ID获取广播信息
        /// </summary>
        /// <param name="id">广播ID</param>
        /// <returns>广播信息</returns>
        public NoticeInfo GetNoticeByID(int id)
        {
            sqlStr = "SELECT * FROM tblNotice WHERE ID = @ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new NoticeInfo(dr);
                    else
                        return null;
                }
            }

        }
        /// <summary>
        /// 获取当前循环广播信息
        /// </summary>
        /// <returns>广播信息</returns>
        public NoticeInfo GetCurrentLoopNotice()
        {
            sqlStr = "SELECT * FROM tblNotice WHERE IsLoop = 1";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new NoticeInfo(dr);
                    else
                        return null;
                }
            }

        }
        /// <summary>
        /// 设置广播循环
        /// </summary>
        /// <param name="id">广播ID</param>
        /// <returns>广播信息</returns>
        public NoticeInfo SetNoticeAsLoop(int id)
        {
            sqlStr = "UPDATE tblNotice SET IsLoop = 1 where ID = @ID; " +
                    " UPDATE tblNotice SET IsLoop = 0 where ID <> @ID AND IsLoop = 1; ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new NoticeInfo(dr);
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// 新增广播信息
        /// </summary>
        /// <param name="info">广播信息</param>
        /// <returns>广播信息</returns>
        public NoticeInfo AddNotice(NoticeInfo info)
        {
            sqlStr += "INSERT INTO tblNotice(Name,Content,Comments,IsLoop,CreatedDate)" +
                        "VALUES(@Name,@Content,@Comments,@IsLoop,GETDATE());" +
                        "SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Name", SqlDbType.VarChar).Value = info.Name;
                command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = info.Content;
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Comments);
                command.Parameters.Add("@IsLoop", SqlDbType.Bit).Value = info.IsLoop;
                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info;
            }
        }
        /// <summary>
        /// 更新广播信息
        /// </summary>
        /// <param name="info">广播信息</param>
        public void UpdateNotice(NoticeInfo info)
        {
            sqlStr = "UPDATE tblNotice Set Name = @Name,Content = @Content,Comments = @Comments,IsLoop=@IsLoop " +
                     " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Name", SqlDbType.VarChar).Value = info.Name;
                command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = info.Content;
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Comments);
                command.Parameters.Add("@IsLoop", SqlDbType.Bit).Value = info.IsLoop;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;

                command.ExecuteNonQuery();
            }
        }
        
    }
}
