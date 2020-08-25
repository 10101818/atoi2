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
    /// 服务凭证dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class DispatchJournalDao:BaseDao
    {
        #region"tblDispatchJournal"
        /// <summary>
        /// 根据服务凭证id获取服务凭证信息
        /// </summary>
        /// <param name="dispatchJournalID">服务凭证ID</param>
        /// <returns>服务凭证信息</returns>
        public DispatchJournalInfo GetDispatchJournalByID(int dispatchJournalID)
        {
            sqlStr = "SELECT j.* FROM tblDispatchJournal j " +
                " WHERE j.ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchJournalID;
                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DispatchJournalInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据派工单id获取服务凭证信息
        /// </summary>
        /// <param name="dispatchID">派工单id</param>
        /// <returns>服务凭证信息</returns>
        public DispatchJournalInfo GetDispatchJournalByDispatchID(int dispatchID)
        {
            sqlStr = "SELECT j.* FROM tblDispatchJournal j " +
                " WHERE j.DispatchID=@DispatchID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchID", SqlDbType.Int).Value = dispatchID;
                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DispatchJournalInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 保存服务凭证信息
        /// </summary>
        /// <param name="dispatchJournal">服务凭证信息</param>
        /// <returns>服务凭证ID</returns>
        public int AddDispatchJournal(DispatchJournalInfo dispatchJournal)
        {
            sqlStr = "INSERT INTO tblDispatchJournal(DispatchID,FaultCode,JobContent,FujiComments,ResultStatusID, "+
                            " FollowProblem,Advice,UserName,UserMobile,Signed,StatusID) " +
                     " VALUES(@DispatchID,@FaultCode,@JobContent,@FujiComments,@ResultStatusID, " +
                            " @FollowProblem,@Advice,@UserName,@UserMobile,@Signed,@StatusID); " +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchID", SqlDbType.Int).Value = dispatchJournal.Dispatch.ID;
                command.Parameters.Add("@FaultCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FaultCode);
                command.Parameters.Add("@JobContent", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.JobContent);
                command.Parameters.Add("@ResultStatusID", SqlDbType.Int).Value = dispatchJournal.ResultStatus.ID;
                command.Parameters.Add("@FollowProblem", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FollowProblem);
                command.Parameters.Add("@Advice", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.Advice);
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(dispatchJournal.UserName);
                command.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(dispatchJournal.UserMobile);
                command.Parameters.Add("@Signed", SqlDbType.Bit).Value = dispatchJournal.Signed;
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = dispatchJournal.Status.ID;
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FujiComments);
                
                dispatchJournal.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return dispatchJournal.ID;
        }
        /// <summary>
        /// 修改服务凭证信息
        /// </summary>
        /// <param name="dispatchJournal">服务凭证信息</param>
        public void UpdateDispatchJournal(DispatchJournalInfo dispatchJournal)
        {
            sqlStr = "UPDATE tblDispatchJournal SET DispatchID = @DispatchID,FaultCode=@FaultCode,"+
                " FujiComments=@FujiComments,JobContent=@JobContent,ResultStatusID=@ResultStatusID,StatusID=@StatusID,FollowProblem=@FollowProblem," +
                " Advice=@Advice,UserName=@UserName,UserMobile=@UserMobile, Signed=@Signed";
            sqlStr += " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchID", SqlDbType.VarChar).Value = dispatchJournal.Dispatch.ID;
                command.Parameters.Add("@FaultCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FaultCode);
                command.Parameters.Add("@JobContent", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.JobContent);
                command.Parameters.Add("@ResultStatusID", SqlDbType.Int).Value = dispatchJournal.ResultStatus.ID;
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = dispatchJournal.Status.ID;
                command.Parameters.Add("@FollowProblem", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FollowProblem);
                command.Parameters.Add("@Advice", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.Advice);
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(dispatchJournal.UserName);
                command.Parameters.Add("@UserMobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(dispatchJournal.UserMobile);
                command.Parameters.Add("@Signed", SqlDbType.Bit).Value = dispatchJournal.Signed;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchJournal.ID;
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchJournal.FujiComments);

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 审批服务凭证
        /// </summary>
        /// <param name="journalID">服务凭证ID</param>
        /// <param name="statusID">审批后服务凭证状态</param>
        /// <param name="resultStatusID">服务结果ID</param>
        /// <param name="followProblem">待跟进问题</param>
        /// <param name="fujiComments">审批备注</param>
        public void UpdateDispatchJournalStatus(int journalID, int statusID, int resultStatusID, string followProblem = "", string fujiComments = "")
        {
            sqlStr = "UPDATE tblDispatchJournal SET FujiComments=@FujiComments ,StatusID=@StatusID, ResultStatusID=@ResultStatusID,FollowProblem=@FollowProblem " +
                     " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(fujiComments);
                command.Parameters.Add("@ResultStatusID", SqlDbType.Int).Value = resultStatusID;
                command.Parameters.Add("@FollowProblem", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(followProblem);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = journalID;

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
