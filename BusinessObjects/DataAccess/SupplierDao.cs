using System;
using System.Collections.Generic;
using BusinessObjects.Aspect;
using PostSharp.Extensibility;
using BusinessObjects.Domain;
using System.Data.SqlClient;
using System.Data;
using BusinessObjects.Util;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 供应商dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class SupplierDao : BaseDao
    {
        #region "Supplier"
        /// <summary>
        /// 判断是否有重复的供应商名称
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="Name">供应商名称</param>
        /// <returns>true/false 是否有重复的供应商名称</returns>
        public bool CheckSupplierName(int id, string Name)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblSupplier WHERE ID <> @ID AND UPPER(Name) = @Name ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name.ToUpper();
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// 获取供应商列表信息
        /// </summary>
        /// <param name="typeID">供应商类型</param>
        /// <param name="status">供应商状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>供应商列表信息</returns>
        public List<SupplierInfo> QuerySupplier(int typeID,int status, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<SupplierInfo> infos = new List<SupplierInfo>();

            sqlStr = "SELECT s.* FROM tblSupplier s WHERE 1=1 ";
            if (typeID != 0)
                sqlStr += "AND s.TypeID = " + typeID;

            if (status > -1)
                sqlStr += " AND s.IsActive = " + status;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "s.ID");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new SupplierInfo(dr));
                    }
                }
            }
            return infos;
        }
        /// <summary>
        /// 获取供应商数量
        /// </summary>
        /// <param name="typeID">供应商类型</param>
        /// <param name="status">供应商状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>供应商数量</returns>
        public int QuerySupplierCount(int typeID,int status, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT s.ID) FROM tblSupplier s ";
            sqlStr += " WHERE 1=1 ";
            if (typeID != 0)
                sqlStr += "AND s.TypeID = " + typeID;
            if (status > -1)
                sqlStr += " AND s.IsActive = " + status;
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
        /// 获取供应商
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>供应商信息</returns>
        public List<SupplierInfo> QuerySuppliers4AutoComplete(string inputText, int curRowNum = 0, int pageSize = 0)
        {
            List<SupplierInfo> infos = new List<SupplierInfo>();

            sqlStr = "SELECT s.* FROM tblSupplier AS s WHERE s.IsActive = 1 ";
            if (!string.IsNullOrEmpty(inputText))
                sqlStr += " AND (UPPER(s.Name) LIKE @InputText OR UPPER(s.ID) LIKE @InputText)";
            sqlStr += " ORDER BY s.ID ";

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!string.IsNullOrEmpty(inputText))
                    command.Parameters.Add("@InputText", SqlDbType.NVarChar).Value = "%" + inputText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new SupplierInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据供应商ID获取供应商信息
        /// </summary>
        /// <param name="id">供应商ID</param>
        /// <returns>供应商信息</returns>
        public SupplierInfo GetSupplier(int id)
        {
            sqlStr = "SELECT * FROM tblSupplier WHERE ID=@ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new SupplierInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 获取供应商各个类型数量
        /// </summary>
        /// <returns>供应商各个类型数量</returns>
        public Dictionary<int, int> GetSupplierCount()
        {
            sqlStr = "SELECT TypeID, Count(ID) FROM tblSupplier WHERE IsActive = 1 GROUP BY TypeID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDictionary(command);
            }
        }
        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="info">供应商信息</param>
        /// <returns>供应商ID</returns>
        public int AddSupplier(SupplierInfo info)
        {
            sqlStr = "INSERT INTO tblSupplier(TypeID,Name,Province,Mobile, " +
                     " Address,Contact,ContactMobile,AddDate,IsActive) " +
                     " VALUES(@TypeID,@Name,@Province,@Mobile, " +
                     " @Address,@Contact,@ContactMobile,GETDATE(),@IsActive);" +
                     " SELECT @@IDENTITY";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.SupplierType.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Province", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Province);
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Mobile);
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Address);
                command.Parameters.Add("@Contact ", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Contact);
                command.Parameters.Add("@ContactMobile ", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.ContactMobile);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = info.IsActive;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }
        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="info">供应商信息</param>
        public void UpdateSupplier(SupplierInfo info)
        {
            sqlStr = "UPDATE tblSupplier SET TypeID = @TypeID, Name = @Name, Province = @Province, " +
                     " Mobile = @Mobile, Address = @Address,Contact = @Contact, ContactMobile = @ContactMobile, " +
                      "IsActive = @IsActive ";

            sqlStr += "WHERE ID = @ID";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;

                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.SupplierType.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Province", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Province);
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Mobile);
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Address);
                command.Parameters.Add("@Contact ", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Contact);
                command.Parameters.Add("@ContactMobile ", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.ContactMobile);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = info.IsActive;

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
