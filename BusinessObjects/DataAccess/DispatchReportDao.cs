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
    /// 作业报告dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class DispatchReportDao : BaseDao
    {
        #region"tblDispatchReport"
        /// <summary>
        /// 根据作业报告ID获取作业报告信息
        /// </summary>
        /// <param name="dispatchReportID">作业报告ID</param>
        /// <returns>作业报告信息</returns>
        public DispatchReportInfo GetDispatchReportByID(int dispatchReportID)
        {
            sqlStr = "SELECT re.* FROM tblDispatchReport re " +
                " WHERE re.ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchReportID;
                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DispatchReportInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增作业报告
        /// </summary>
        /// <param name="dispatchReport">作业报告信息</param>
        /// <returns>作业报告ID</returns>
        public int AddDispatchReport(DispatchReportInfo dispatchReport)
        {
            sqlStr = "INSERT INTO tblDispatchReport(DispatchID ,TypeID ,FaultCode ," +
                " FaultDesc ,SolutionCauseAnalysis ,SolutionWay,IsPrivate, ServiceProvider,SolutionResultStatusID ," +
                " SolutionUnsolvedComments ,DelayReason,Comments,FujiComments ,StatusID, " +
                " EquipmentStatus, PurchaseAmount, ServiceScope, Result, IsRecall,AcceptanceDate) " +
                " VALUES(@DispatchID ,@TypeID ,@FaultCode ," +
                " @FaultDesc ,@SolutionCauseAnalysis , @SolutionWay,@IsPrivate,@ServiceProvider,@SolutionResultStatusID ," +
                " @SolutionUnsolvedComments ,@DelayReason,@Comments,@FujiComments,@StatusID,  " +
                " @EquipmentStatus, @PurchaseAmount, @ServiceScope, @Result, @IsRecall,@AcceptanceDate); " +
                " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchID", SqlDbType.Int).Value = dispatchReport.Dispatch.ID;
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = dispatchReport.Type.ID;
                command.Parameters.Add("@FaultCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FaultCode);
                command.Parameters.Add("@FaultDesc", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FaultDesc);
                command.Parameters.Add("@SolutionCauseAnalysis", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionCauseAnalysis);
                command.Parameters.Add("@SolutionWay", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionWay);
                command.Parameters.Add("@IsPrivate", SqlDbType.Bit).Value = dispatchReport.IsPrivate;
                command.Parameters.Add("@ServiceProvider", SqlDbType.Int).Value = SQLUtil.ZeroToNull(dispatchReport.ServiceProvider.ID);
                command.Parameters.Add("@SolutionResultStatusID", SqlDbType.Int).Value = dispatchReport.SolutionResultStatus.ID;
                command.Parameters.Add("@SolutionUnsolvedComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionUnsolvedComments);
                command.Parameters.Add("@DelayReason", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.DelayReason);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.Comments);
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FujiComments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = dispatchReport.Status.ID;

                command.Parameters.Add("@EquipmentStatus", SqlDbType.Int).Value = dispatchReport.EquipmentStatus.ID;
                command.Parameters.Add("@PurchaseAmount", SqlDbType.Decimal).Value = dispatchReport.PurchaseAmount;
                command.Parameters.Add("@ServiceScope", SqlDbType.Bit).Value = dispatchReport.ServiceScope;
                command.Parameters.Add("@Result", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.Result);
                command.Parameters.Add("@IsRecall", SqlDbType.Bit).Value = dispatchReport.IsRecall;
                command.Parameters.Add("@AcceptanceDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(dispatchReport.AcceptanceDate);

                dispatchReport.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return dispatchReport.ID;
        }
        /// <summary>
        /// 更新作业报告
        /// </summary>
        /// <param name="dispatchReport">作业报告信息</param>
        public void UpdateDispatchReport(DispatchReportInfo dispatchReport)
        {
            sqlStr = "UPDATE tblDispatchReport SET DispatchID=@DispatchID ,TypeID=@TypeID ," +
                " FaultCode=@FaultCode ,FaultDesc=@FaultDesc ," +
                " SolutionCauseAnalysis=@SolutionCauseAnalysis,SolutionWay=@SolutionWay," +
                " IsPrivate=@IsPrivate,ServiceProvider=@ServiceProvider,SolutionResultStatusID=@SolutionResultStatusID ," +
                " SolutionUnsolvedComments=@SolutionUnsolvedComments ,DelayReason=@DelayReason,Comments=@Comments," +
                " FujiComments=@FujiComments ,StatusID=@StatusID, " +
                " EquipmentStatus=@EquipmentStatus, PurchaseAmount=@PurchaseAmount, ServiceScope=@ServiceScope, Result= @Result, IsRecall=@IsRecall,AcceptanceDate=@AcceptanceDate";
            sqlStr += " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchID", SqlDbType.Int).Value = dispatchReport.Dispatch.ID;
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = dispatchReport.Type.ID;
                command.Parameters.Add("@FaultCode", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FaultCode);
                command.Parameters.Add("@FaultDesc", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FaultDesc);
                command.Parameters.Add("@SolutionCauseAnalysis", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionCauseAnalysis);
                command.Parameters.Add("@SolutionWay", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionWay);
                command.Parameters.Add("@IsPrivate", SqlDbType.Bit).Value = dispatchReport.IsPrivate;
                command.Parameters.Add("@ServiceProvider", SqlDbType.Int).Value = SQLUtil.ZeroToNull(dispatchReport.ServiceProvider.ID);
                command.Parameters.Add("@SolutionResultStatusID", SqlDbType.Int).Value = dispatchReport.SolutionResultStatus.ID;
                command.Parameters.Add("@SolutionUnsolvedComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.SolutionUnsolvedComments);
                command.Parameters.Add("@DelayReason", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.DelayReason);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.Comments);
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.FujiComments);
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = dispatchReport.Status.ID;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = dispatchReport.ID;

                command.Parameters.Add("@EquipmentStatus", SqlDbType.Int).Value = dispatchReport.EquipmentStatus.ID;
                command.Parameters.Add("@PurchaseAmount", SqlDbType.Decimal).Value = dispatchReport.PurchaseAmount;
                command.Parameters.Add("@ServiceScope", SqlDbType.Bit).Value = dispatchReport.ServiceScope;
                command.Parameters.Add("@Result", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(dispatchReport.Result);
                command.Parameters.Add("@IsRecall", SqlDbType.Bit).Value = dispatchReport.IsRecall;
                command.Parameters.Add("@AcceptanceDate", SqlDbType.DateTime).Value = SQLUtil.MinDateToNull(dispatchReport.AcceptanceDate);

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新作业报告审批信息
        /// </summary>
        /// <param name="reportID">作业报告ID</param>
        /// <param name="statusID">审批后作业报告状态</param>
        /// <param name="fujiComments">审批备注</param>
        /// <param name="solutionResultStatusID">作业报告结果ID</param>
        /// <param name="solutionUnsolvedComments">问题升级</param>
        public void UpdateDispatchReportStatus(int reportID, int statusID, string fujiComments = "", int solutionResultStatusID = 0, string solutionUnsolvedComments = "")
        {
            sqlStr = "UPDATE tblDispatchReport SET FujiComments=@FujiComments ,StatusID=@StatusID, SolutionResultStatusID=@SolutionResultStatusID, " +
                " SolutionUnsolvedComments=@SolutionUnsolvedComments" +
                     " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = statusID;
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(fujiComments);
                command.Parameters.Add("@SolutionResultStatusID", SqlDbType.Int).Value = solutionResultStatusID;
                command.Parameters.Add("@SolutionUnsolvedComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(solutionUnsolvedComments);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = reportID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 审批通过作业报告信息
        /// </summary>
        /// <param name="reportID">作业报告ID</param>
        /// <param name="solutionResultStatusID">作业报告结果ID</param>
        /// <param name="solutionUnsolvedComments">问题升级</param>
        /// <param name="fujiComments">审批备注</param>
        public void PassDispatchReport(int reportID, int solutionResultStatusID, string solutionUnsolvedComments, string fujiComments)
        {
            sqlStr = "UPDATE tblDispatchReport SET SolutionResultStatusID=@SolutionResultStatusID, FujiComments=@FujiComments ,StatusID=@StatusID, " +
                " SolutionUnsolvedComments=@SolutionUnsolvedComments " +
                     " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(fujiComments);
                command.Parameters.Add("@SolutionResultStatusID", SqlDbType.Int).Value = solutionResultStatusID;
                command.Parameters.Add("@StatusID", SqlDbType.Int).Value = DispatchReportInfo.DispatchReportStatus.Approved;
                command.Parameters.Add("@SolutionUnsolvedComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(solutionUnsolvedComments);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = reportID;

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region tblReportComponent
        public List<ReportComponentInfo> GetReportComponentByDispatchReportID(int dispatchReportID)
        {
            List<ReportComponentInfo> infos = new List<ReportComponentInfo>();
            sqlStr = "SELECT rc.*, new.ID as NewID, new.Price as NewPrice, old.ID as OldID ,old.Price as OldPrice, c.Name as ComponentName " +
                    " FROM tblReportComponent rc " +
                    " LEFT JOIN tblInvComponent new ON rc.NewSerialCode = new.SerialCode " +
                    " LEFT JOIN tblInvComponent old ON rc.OldSerialCode = old.SerialCode " +
                    " LEFT JOIN tblComponent c ON rc.ComponentID = c.ID " +
                    " WHERE rc.DispatchReportID = @DispatchReportID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = dispatchReportID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ReportComponentInfo(dr));
                    }
                }
                return infos;
            }
        }

        public bool CheckComponent(string serialCode, int type)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblReportComponent WHERE 1=1 ";

            if (type == ReportComponentInfo.FileTypes.New)
                sqlStr += " AND NewSerialCode = @SerialCode ";
            if (type == ReportComponentInfo.FileTypes.Old)
                sqlStr += " AND OldSerialCode = @SerialCode ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = serialCode;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        public int AddReportComponent(ReportComponentInfo info)
        {
            sqlStr = "INSERT INTO tblReportComponent(DispatchReportID,ComponentID,NewSerialCode,OldSerialCode) " +
                    " VALUES(@DispatchReportID,@ComponentID,@NewSerialCode,@OldSerialCode)" +
                    " SELECT @@IDENTITY";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = info.DispatchReportID;
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = info.NewComponent.Component.ID;
                command.Parameters.Add("@NewSerialCode", SqlDbType.NVarChar).Value = info.NewComponent.SerialCode;
                command.Parameters.Add("@OldSerialCode", SqlDbType.NVarChar).Value = info.OldComponent.SerialCode;
                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info.ID;
        }

        public void DeleteReportComponent(int reportComponentID)
        {
            sqlStr = " DELETE FROM tblReportComponent WHERE ID=@ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = reportComponentID;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region tblReportConsumable
        public List<ReportConsumableInfo> GetReportConsumableByDispatchReportID(int dispatchReportID)
        {
            List<ReportConsumableInfo> infos = new List<ReportConsumableInfo>();
            sqlStr = "SELECT rc.*,c.Name as ConsumableName,s.Name as SupplierName,ic.Price, ic.ID as InvConsumableID " +
                    " FROM tblReportConsumable rc " +
                    " LEFT JOIN tblInvConsumable ic ON rc.LotNum = ic.LotNum " +
                    " LEFT JOIN tblSupplier s ON ic.SupplierID = s.ID " +
                    " LEFT JOIN tblConsumable c ON ic.ConsumableID = c.ID " +
                    " WHERE rc.DispatchReportID = @DispatchReportID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = dispatchReportID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ReportConsumableInfo(dr));
                    }
                }
                return infos;
            }
        }

        public bool CheckConsumable(int dispatchReportID, int consumableID, string lotNum)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblReportConsumable WHERE DispatchReportID = @DispatchReportID AND ConsumableID = @ConsumableID AND LotNum = @LotNum ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = dispatchReportID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = consumableID;
                command.Parameters.Add("@LotNum", SqlDbType.NVarChar).Value = lotNum;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        public int AddReportConsumable(ReportConsumableInfo info)
        {
            sqlStr = "INSERT INTO tblReportConsumable(DispatchReportID,ConsumableID,LotNum,Qty) " +
                    " VALUES(@DispatchReportID,@ConsumableID,@LotNum,@Qty)" +
                    " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = info.DispatchReportID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = info.InvConsumable.Consumable.ID;
                command.Parameters.Add("@LotNum", SqlDbType.NVarChar).Value = info.InvConsumable.LotNum;
                command.Parameters.Add("@Qty", SqlDbType.Decimal).Value = info.Qty;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info.ID;
        }

        public void DeleteReportConsumable(int reportConsumableID)
        {
            sqlStr = " DELETE FROM tblReportConsumable WHERE ID=@ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = reportConsumableID;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region tblReportService
        public List<ReportServiceInfo> GetReportServiceByDispatchReportID(int dispatchReportID)
        {
            List<ReportServiceInfo> infos = new List<ReportServiceInfo>();
            sqlStr = "SELECT rs.*,ise.Name,s.Name as SupplierName,ise.Price " +
                    " FROM tblReportService rs " +
                    " LEFT JOIN tblInvService ise ON rs.ServiceID = ise.ID " +
                    " LEFT JOIN tblSupplier s ON ise.SupplierID = s.ID " +
                    " WHERE rs.DispatchReportID = @DispatchReportID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = dispatchReportID;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ReportServiceInfo(dr));
                    }
                }
                return infos;
            }
        }

        public bool CheckService(int dispatchReportID, int serviceID)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblReportService WHERE DispatchReportID = @DispatchReportID AND ServiceID = @ServiceID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = dispatchReportID;
                command.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceID;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }
        public int AddReportService(ReportServiceInfo info)
        {
            sqlStr = "INSERT INTO tblReportService(DispatchReportID,ServiceID) " +
                    " VALUES(@DispatchReportID,@ServiceID)" +
                    " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@DispatchReportID", SqlDbType.Int).Value = info.DispatchReportID;
                command.Parameters.Add("@ServiceID", SqlDbType.Int).Value = info.Service.ID;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return info.ID;
        }

        public void DeleteReportService(int reportServiceID)
        {
            sqlStr = " DELETE FROM tblReportService WHERE ID=@ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = reportServiceID;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region tblMaterialHistory

        public void AddMaterialHistory(DataTable dt)
        {
            sqlStr = "INSERT INTO tblMaterialHistory (DispatchReportID,ObjectType,ObjectID,EquipmentID,UserID,Qty,UsedDate) " +
                    " VALUES(@DispatchReportID,@ObjectType,@ObjectID,@EquipmentID,@UserID,@Qty,GetDate())";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@DispatchReportID", SqlDbType.Int);
                parameter.SourceColumn = "DispatchReportID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@ObjectType", SqlDbType.Int);
                parameter.SourceColumn = "ObjectType";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@ObjectID", SqlDbType.Int);
                parameter.SourceColumn = "ObjectID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@EquipmentID", SqlDbType.Int);
                parameter.SourceColumn = "EquipmentID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Qty", SqlDbType.Decimal);
                parameter.SourceColumn = "Qty";
                parameter.SourceVersion = DataRowVersion.Original;

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
