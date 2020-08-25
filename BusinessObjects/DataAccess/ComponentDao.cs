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
    /// 零件dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ComponentDao : BaseDao
    {
        /// <summary>
        /// 获取零件信息数量
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>零件信息数量</returns>
        public int QueryComponentCount(int statusID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT c.ID) FROM tblComponent c " +
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
        /// 获取零件信息
        /// </summary>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">每页第一条数据</param>
        /// <param name="pageSize">页码</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo> QueryComponents(int statusID, string filterField, string filterText, int curRowNum, int pageSize)
        {
            List<ComponentInfo> infos = new List<ComponentInfo>();

            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM tblComponent c " +
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
                        infos.Add(new ComponentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据富士II类获取零件信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="componentTypeID">零件类型</param>
        /// <param name="IsIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo> QueryComponentsByFujiClass2ID(int fujiClass2ID, int componentTypeID, bool IsIncluded = false, string filterField = "", string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            List<ComponentInfo> infos = new List<ComponentInfo>();

            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM tblComponent c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID" +
                    " WHERE 1=1 AND c.IsActive = 1 " +
                    " AND c.FujiClass2ID = " + fujiClass2ID;
            if(componentTypeID != -1)
                    sqlStr += " AND c.TypeID = " + componentTypeID;
            if (IsIncluded)
                sqlStr += " AND c.IsIncluded = 1 ";

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += " ORDER BY c.ID ";

            if (pageSize != 0)
                sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ComponentInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据富士II类获取二类零件数量
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>零件数量</returns>
        public int QueryComponentCountByFujiClass2ID(int fujiClass2ID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT c.ID) FROM tblComponent c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID" +
                    " WHERE 1=1 AND c.IsActive = 1 " +
                    " AND c.FujiClass2ID = " + fujiClass2ID;

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
        /// 根据零件id列表获取零件信息
        /// </summary>
        /// <param name="ids">零件id列表</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo>  QueryComponentsByIDs(List<int> ids)
        {
            if (ids.Count == 0)
                return new List<ComponentInfo>();
            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM tblComponent c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID" +
                    " WHERE  c.ID IN ("+string.Join(",", ids) +")";
            
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetList<ComponentInfo>(command);
            }

        }

        /// <summary>
        /// 根据零件id获取零件信息
        /// </summary>
        /// <param name="componentID">零件id</param>
        /// <returns>零件信息</returns>
        public ComponentInfo GetComponentByID(int componentID)
        {
            sqlStr = "SELECT c.*,f.Name FujiClass2Name FROM tblComponent c " +
                    " LEFT JOIN tblFujiClass2 f ON c.FujiClass2ID = f.ID " +
                    " WHERE 1=1 " +
                    " AND c.ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = componentID;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new ComponentInfo(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 判断零件名称是否重复
        /// </summary>
        /// <param name="id">零件id</param>
        /// <param name="name">零件名称</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <returns>是否重复</returns>
        public bool CheckComponentName(int id, string name, int fujiClass2ID)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblComponent WHERE ID <> @ID AND UPPER(Name) = UPPER(@Name) AND FujiClass2ID = @FujiClass2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = fujiClass2ID;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 判断零件是否在使用中
        /// </summary>
        /// <param name="id">零件id</param>
        /// <returns>零件是否在使用中</returns>
        public bool CheckComponentInUse(int id)
        {
            sqlStr = "select sum(component.count) " +
                    " FROM (SELECT count(ComponentID) as count FROM jctContractComponent  where ComponentID = @ComponentID " +
                    " Union all " +
                    " select count(distinct ID) as count from tblInvComponent  where ComponentID = @ComponentID) as component ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// 添加零件
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        public int AddComponent(ComponentInfo info)
        {
            sqlStr = "INSERT INTO tblComponent(FujiClass2ID,Name,Description,TypeID,StdPrice,Usage,TotalSeconds,SecondsPer,IsIncluded,IncludeContract,MethodID,LifeTime,IsActive,AddDate) " +
                    " VALUES(@FujiClass2ID,@Name,@Description,@TypeID,@StdPrice,@Usage,@TotalSeconds,@SecondsPer,@IsIncluded,@IncludeContract,@MethodID,@LifeTime,@IsActive,GetDate()); " +
                    " SELECT @@IDENTITY ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Type.ID);
                command.Parameters.Add("@StdPrice", SqlDbType.Decimal).Value = info.StdPrice;
                command.Parameters.Add("@Usage", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Usage);
                command.Parameters.Add("@TotalSeconds", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.TotalSeconds);
                command.Parameters.Add("@SecondsPer", SqlDbType.Decimal).Value = info.SecondsPer;
                command.Parameters.Add("@IsIncluded", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsIncluded);
                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IncludeContract);
                command.Parameters.Add("@MethodID", SqlDbType.Int).Value = (int)info.Method == 0 ? FujiClass2Info.Method.Manual : info.Method;
                command.Parameters.Add("@LifeTime", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.LifeTime);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsActive);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 修改零件信息
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        public int UpdateComponent(ComponentInfo info)
        {
            sqlStr = "UPDATE tblComponent Set FujiClass2ID=@FujiClass2ID,Name=@Name,Description=@Description,TypeID=@TypeID,StdPrice=@StdPrice, " + 
                    " Usage=@Usage,TotalSeconds=@TotalSeconds,SecondsPer=@SecondsPer,IsIncluded=@IsIncluded,IncludeContract=@IncludeContract," + 
                    " MethodID=@MethodID,LifeTime=@LifeTime,IsActive=@IsActive,UpdateDate=GetDate() " +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Type.ID);
                command.Parameters.Add("@StdPrice", SqlDbType.Decimal).Value = info.StdPrice;
                command.Parameters.Add("@Usage", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.Usage);
                command.Parameters.Add("@TotalSeconds", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.TotalSeconds);
                command.Parameters.Add("@SecondsPer", SqlDbType.Decimal).Value = info.SecondsPer;
                command.Parameters.Add("@IsIncluded", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsIncluded);
                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IncludeContract);
                command.Parameters.Add("@MethodID", SqlDbType.Int).Value = (int)info.Method == 0 ? FujiClass2Info.Method.Manual : info.Method;
                command.Parameters.Add("@LifeTime", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.LifeTime);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = SQLUtil.ConvertBoolean(info.IsActive);

                command.ExecuteNonQuery();

                return info.ID;
            }
        }

        /// <summary>
        /// 根据零件id删除零件信息
        /// </summary>
        /// <param name="componentID">零件id</param>
        public void DeleteComponentByID(int componentID)
        {
            sqlStr = "DELETE FROM tblComponent WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = componentID;

                command.ExecuteNonQuery();
            }
        }
    }
}
