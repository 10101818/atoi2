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
    /// 故障率dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class FaultRateDao : BaseDao
    {
        #region "tbFaultRate"
        /// <summary>
        /// 获取故障率
        /// </summary> 
        /// <param name="objectID">关联对象ID</param>
        /// <param name="type">类型</param>
        /// <returns>故障率</returns>
        public List<FaultRateInfo> GetFaultRateByObject(int objectID, FaultRateInfo.ObjectType type)
        {
            sqlStr = "SELECT fr.* FROM tblFaultRate fr  WHERE fr.ObjectID=@ObjectID AND ObjectTypeID = @ObjectTypeID ORDER BY fr.Year,fr.Month ASC";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = objectID;
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = type;
                return GetList<FaultRateInfo>(command);
            }
        }
        /// <summary>
        /// 批量获取故障率
        /// </summary>
        /// <param name="objectIDs">父对象ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public Dictionary<int, List<FaultRateInfo>> GetFaultRateByObject(List<int> objectIDs, FaultRateInfo.ObjectType type)
        {
            if (objectIDs == null || objectIDs.Count == 0)
                return null;
            sqlStr = string.Format("SELECT fr.* FROM tblFaultRate fr  WHERE fr.ObjectID in ({0}) AND ObjectTypeID = @ObjectTypeID ORDER BY fr.Year,fr.Month ASC", string.Join(",",objectIDs));
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = type;
                List<FaultRateInfo> result = GetList<FaultRateInfo>(command);
                return result.GroupBy(info => info.ObjectID).ToDictionary(g => g.Key, g => g.ToList()); 
            }
        }
        /// <summary>
        /// 添加故障率
        /// </summary>
        /// <param name="faultRateInfo">故障率信息</param>
        public void AddFaultRate(FaultRateInfo faultRateInfo){
            sqlStr = "INSERT INTO tblFaultRate(ObjectTypeID , ObjectID  , Year , Month , Rate) " +
                     " VALUES(@ObjectTypeID , @ObjectID  , @Year , @Month , @Rate) " ;

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            { 
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = faultRateInfo.ObjectTypeID;
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = faultRateInfo.ObjectID; 
                command.Parameters.Add("@Year", SqlDbType.Int).Value = faultRateInfo.Year;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = faultRateInfo.Month;
                command.Parameters.Add("@Rate", SqlDbType.Decimal).Value = faultRateInfo.Rate;

                command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 批量添加故障率
        /// </summary>
        /// <param name="dt">封装故障率信息的DataTable</param>
        public void AddFaultRates(DataTable dt)
        {
            sqlStr = @"INSERT INTO tblFaultRate(ObjectTypeID , ObjectID , Year , Month , Rate)
                VALUES(@ObjectTypeID , @ObjectID , @Year , @Month , @Rate) ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@ObjectTypeID", SqlDbType.Int);
                parameter.SourceColumn = "ObjectTypeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@ObjectID", SqlDbType.Int);
                parameter.SourceColumn = "ObjectID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Year", SqlDbType.Int);
                parameter.SourceColumn = "Year";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Month", SqlDbType.Int);
                parameter.SourceColumn = "Month";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Rate", SqlDbType.Decimal);
                parameter.SourceColumn = "Rate";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 更新故障率
        /// </summary>
        /// <param name="faultRate">故障率信息</param>
        public void UpdateFaultRate(FaultRateInfo faultRate)
        {
            sqlStr = @"UPDATE tblFaultRate SET Rate = @Rate
                       WHERE ObjectTypeID=@ObjectTypeID AND ObjectID=@ObjectID AND Year=@Year AND Month=@Month ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            { 
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = faultRate.ObjectTypeID;
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = faultRate.ObjectID;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = faultRate.Year;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = faultRate.Month;
                command.Parameters.Add("@Rate", SqlDbType.Decimal).Value = faultRate.Rate;

                command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 批量更新故障率
        /// </summary>
        /// <param name="dt">封装故障率信息的DataTable</param>
        public void UpdateFaultRates(DataTable dt)
        {
            sqlStr = @" IF EXISTS (SELECT * FROM tblFaultRate WHERE ObjectTypeID=@ObjectTypeID AND ObjectID=@ObjectID AND Year=@Year AND Month=@Month)
	                        UPDATE tblFaultRate SET Rate = @Rate
                            WHERE ObjectTypeID=@ObjectTypeID AND ObjectID=@ObjectID AND Year=@Year AND Month=@Month 
                        ELSE
	                        INSERT INTO tblFaultRate(ObjectTypeID , ObjectID , Year , Month , Rate)
                            VALUES(@ObjectTypeID , @ObjectID , @Year , @Month , @Rate) ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@ObjectTypeID", SqlDbType.Int);
                parameter.SourceColumn = "ObjectTypeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@ObjectID", SqlDbType.Int);
                parameter.SourceColumn = "ObjectID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Year", SqlDbType.Int);
                parameter.SourceColumn = "Year";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Month", SqlDbType.Int);
                parameter.SourceColumn = "Month";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Rate", SqlDbType.Decimal);
                parameter.SourceColumn = "Rate";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }



        /// <summary>
        /// 通过关联对象ID删除故障率
        /// </summary>
        /// <param name="objectID">关联对象ID</param>
        /// <param name="type">类型</param>
        public void DeleteFaultRateByObjID(int objectID, FaultRateInfo.ObjectType type)
        {
            sqlStr = "DELETE tblFaultRate WHERE ObjectID=@ObjectID AND ObjectTypeID = @ObjectTypeID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                // ObjectTypeID , ObjectID , MethodID , Year , Month , Rate 
                command.Parameters.Add("@ObjectID", SqlDbType.Int).Value = objectID;
                command.Parameters.Add("@ObjectTypeID", SqlDbType.Int).Value = type;

                command.ExecuteScalar();
            }
        }
        #endregion

        #region web
        public Dictionary<int, int> GetEqptContractMonth(int fujiClass2ID)
        {
            sqlStr = "SELECT e.ID,ISNULL(DATEDIFF(mm,e.AcceptanceDate,ISNULL(e.ScrapDate,GETDATE())),0) " +
                    " FROM tblEquipment e " +
                    " where e.FujiClass2ID = @FujiClass2";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2", SqlDbType.Int).Value = fujiClass2ID;
                return GetDictionary(command);
            }
        }

        public Dictionary<int, int> GetEqptRepairMonth(int fujiClass2ID)
        {
            sqlStr = "SELECT jre.RequestID,ISNULL(DATEDIFF(mm,e.AcceptanceDate,r.RequestDate),0) " +
                    " FROM tblRequest r " +
                    " LEFT JOIN jctRequestEqpt jre ON r.ID = jre.RequestID " +
                    " LEFT JOIN tblEquipment e ON jre.EquipmentID = e.ID " +
                    " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND e.FujiClass2ID = @FujiClass2";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2", SqlDbType.Int).Value = fujiClass2ID;
                return GetDictionary(command);
            }
        }

        public Dictionary<int, int> GetComponentRepairMonth(int componentID)
        {
            string incComponentSql = "(SELECT mh.* FROM tblMaterialHistory mh LEFT JOIN tblInvComponent ic ON ic.ID = mh.ObjectID  WHERE mh.ObjectType = {0} AND ic.ComponentID = @ComponentID)";

            sqlStr = "select ISNULL(mh1.ObjectID,mh2.ObjectID),DATEDIFF(MM,ISNULL(mh1.UsedDate, e.AcceptanceDate),ISNULL(mh2.UsedDate,GETDATE())) " +
                    " FROM {0} as mh1 " +
                    " FULL JOIN {1} as mh2 ON mh1.ObjectID = mh2.ObjectID " +
                    " LEFT JOIN tblEquipment e ON mh1.EquipmentID = e.ID OR mh2.EquipmentID = e.ID ";

            sqlStr = string.Format(sqlStr, string.Format(incComponentSql, ReportMaterialInfo.MaterialTypes.NewComponent), string.Format(incComponentSql, ReportMaterialInfo.MaterialTypes.OldComponent));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = componentID;
                return GetDictionary(command);
            }
        }
        #endregion
    }
}
