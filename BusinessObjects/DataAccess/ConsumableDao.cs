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
    /// 耗材dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ConsumableDao : BaseDao
    {
        /// <summary>
        /// 获取耗材信息数量
        /// </summary>
        /// <param name="statusID">耗材状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>耗材信息数量</returns>
        public int QueryConsumableCount(int statusID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT c.ID) FROM tblConsumable c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (statusID >= 0)
                sqlStr += " AND c.IsActive = " + statusID;
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
        /// 获取耗材列表信息
        /// </summary>
        /// <param name="statusID">耗材状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>耗材信息</returns>
        public List<ConsumableInfo> QueryConsumables(int statusID, string filterField, string filterText, int curRowNum, int pageSize)
        {
            List<ConsumableInfo> infos = new List<ConsumableInfo>();

            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM tblConsumable c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 ";
            if (statusID >= 0)
                sqlStr += " AND c.IsActive = " + statusID;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += " ORDER BY c.ID ";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ConsumableInfo(dr));
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
        public ConsumableInfo GetConsumableByID(int consumableID)
        {
            sqlStr = "SELECT c.*,f.Name FujiClass2Name FROM tblConsumable c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 " +
                    " AND c.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = consumableID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new ConsumableInfo(dr);
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// 根据富士II类获取耗材信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>耗材信息</returns>
        public List<ConsumableInfo> QueryConsumablesByFujiClass2ID(int fujiClass2ID, bool isIncluded = false, string filterField = "", string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            List<ConsumableInfo> infos = new List<ConsumableInfo>();

            sqlStr = "SELECT c.*,f2.Name FujiClass2Name FROM tblConsumable c " +
                    " LEFT JOIN tblFujiClass2 f2 ON c.FujiClass2ID = f2.ID " +
                    " WHERE 1=1 AND c.IsActive = 1 AND c.FujiClass2ID = " + fujiClass2ID;

            if (isIncluded)
                sqlStr += " AND c.IsIncluded = 1 ";

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += " ORDER BY c.ID ";

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ConsumableInfo(dr));
                    }
                }
            }

            return infos;
        }
        
        /// <summary>
        /// 根据设备id获取耗材数量
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>耗材数量</returns>
        public int QueryConsumableCountByFujiClass2ID(int fujiClass2ID, string filterField = "", string filterText = "")
        {
            sqlStr = "SELECT COUNT(DISTINCT c.ID) FROM tblConsumable c " +
                    " LEFT JOIN tblFujiClass2 f2 ON c.FujiClass2ID = f2.ID " +
                    " WHERE 1=1 AND c.IsActive = 1 AND c.FujiClass2ID = " + fujiClass2ID;

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
        /// 判断耗材名称是否重复
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <param name="name">耗材名称</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <returns>耗材名称是否重复</returns>
        public bool CheckConsumableName(int id, string name, int fujiClass2ID)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblConsumable WHERE ID <> @ID AND UPPER(Name) = UPPER(@Name) AND FujiClass2ID = @FujiClass2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = fujiClass2ID;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 耗材是否被使用
        /// </summary>
        /// <param name="id">耗材id</param>
        /// <returns>耗材是否被使用</returns>
        public bool CheckConsumableInUse(int id)
        {
            sqlStr = " SELECT COUNT(DISTINCT ConsumableID) " +
                    " FROM jctContractConsumable " +
                    " WHERE ConsumableID = @ConsumableID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 添加耗材
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        public int AddConsumable(ConsumableInfo info)
        {
            sqlStr = "INSERT INTO tblConsumable(FujiClass2ID,Name,Description,TypeID,ReplaceTimes, CostPer,StdPrice,IsIncluded,IncludeContract,IsActive,AddDate) " +
                    " VALUES(@FujiClass2ID,@Name,@Description,@TypeID,@ReplaceTimes,@CostPer,@StdPrice,@IsIncluded,@IncludeContract,@IsActive,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Type.ID);
                command.Parameters.Add("@ReplaceTimes", SqlDbType.Decimal).Value = info.ReplaceTimes;
                command.Parameters.Add("@CostPer", SqlDbType.Decimal).Value = info.CostPer;
                command.Parameters.Add("@StdPrice", SqlDbType.Decimal).Value = info.StdPrice;
                command.Parameters.Add("@IsIncluded", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsIncluded);
                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IncludeContract);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsActive);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        public int UpdateConsumable(ConsumableInfo info)
        {
            sqlStr = "UPDATE tblConsumable Set FujiClass2ID=@FujiClass2ID,Name=@Name,Description=@Description,TypeID=@TypeID,StdPrice=@StdPrice, " +
                    " ReplaceTimes=@ReplaceTimes,CostPer=@CostPer,IsIncluded=@IsIncluded,IncludeContract=@IncludeContract," +
                    " IsActive=@IsActive,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Type.ID);
                command.Parameters.Add("@ReplaceTimes", SqlDbType.Decimal).Value = info.ReplaceTimes;
                command.Parameters.Add("@CostPer", SqlDbType.Decimal).Value = info.CostPer;
                command.Parameters.Add("@StdPrice", SqlDbType.Decimal).Value = info.StdPrice;
                command.Parameters.Add("@IsIncluded", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsIncluded);
                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IncludeContract);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsActive);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }

        /// <summary>
        /// 根据耗材id删除耗材信息
        /// </summary>
        /// <param name="consumableID">耗材id</param>
        public void DeleteConsumableByID(int consumableID)
        {
            sqlStr = "DELETE FROM tblConsumable WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = consumableID;

                command.ExecuteNonQuery();
            } 
        }

        /// <summary>
        /// 根据耗材富士II类id删除耗材信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        public void DeleteConsumableByFujiClass2ID(int fujiClass2ID)
        {
            sqlStr = "DELETE FROM tblConsumable WHERE FujiClass2ID = @FujiClass2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = fujiClass2ID;

                command.ExecuteNonQuery();
            }
        }

    }
}
