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
    /// 历史信息dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class HistoryDao:BaseDao
    {
        #region "tblXXXXHistory"
        /// <summary>
        /// 添加历史信息
        /// </summary>
        /// <param name="history">历史信息</param>
        public void AddHistory(HistoryInfo history){
            sqlStr = string.Format("INSERT INTO tbl{0}History({0}ID ,OperatorID ,Action ,Comments ,TransDate ) " +
                     " VALUES(@ObjectID ,@OperatorID ,@Action ,@Comments ,@TransDate ); " +
                     " SELECT @@IDENTITY ", LookupManager.GetObjectTypeKey(history.ObjectTypeID));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = history.ObjectID;
                command.Parameters.Add("@OperatorID", SqlDbType.Int).Value = history.Operator.ID;
                command.Parameters.Add("@Action", SqlDbType.Int).Value = history.Action.ID;
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(history.Comments);
                command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now;

                command.ExecuteScalar();
            }
        }
        /// <summary>
        /// 获取历史信息
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="objectID">历史关联ID</param>
        /// <returns>历史信息</returns>
        public List<HistoryInfo> GetHistories(int objectTypeId, int objectID)
        {
            List<HistoryInfo> histories = new List<HistoryInfo>();

            sqlStr = string.Format("SELECT h.ID, h.{0}ID ObjectID, h.OperatorID,u.Name OperatorName ,u.RoleID OperatorRoleID,h.Action, h.Comments, h.TransDate FROM tbl{0}History h " +
            " LEFT JOIN tblUser u ON u.ID=h.OperatorID WHERE h.{0}ID=@ObjectID ORDER BY h.ID DESC", LookupManager.GetObjectTypeKey(objectTypeId));
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = objectID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        histories.Add(new HistoryInfo(dr));
                    }
                }
            }
            return histories;
        }
        #endregion
    }
}
