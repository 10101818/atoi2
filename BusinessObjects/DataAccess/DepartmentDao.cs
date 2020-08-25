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
    /// 科室Dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class DepartmentDao : BaseDao
    {
        /// <summary>
        /// 获取科室列表信息
        /// </summary>
        /// <param name="departmentTypeID">科室分类编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>科室列表信息</returns>
        public int QueryDepartmentCount(int departmentTypeID, string filterField, string filterText)
        {
            List<DepartmentInfo> departments = new List<DepartmentInfo>();

            sqlStr = "SELECT Count(d.ID) FROM lkpDepartment d " +
                    " WHERE 1=1 ";
            if (departmentTypeID > 0)
                sqlStr += " AND d.TypeID = " + departmentTypeID;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += string.Format(" AND UPPER({0}) LIKE @FilterText OR UPPER(d.Pinyin) LIKE @FilterText ", filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                return GetCount(command);
            }

        }

        /// <summary>
        /// 获取科室列表信息
        /// </summary>
        /// <param name="departmentTypeID">科室分类编号</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>科室列表信息</returns>
        public List<DepartmentInfo> QueryDepartments(int departmentTypeID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<DepartmentInfo> departments = new List<DepartmentInfo>();

            sqlStr = "SELECT * FROM lkpDepartment d " +
                    " WHERE 1=1 ";
            if (departmentTypeID > 0)
                sqlStr += " AND d.TypeID = " + departmentTypeID;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += string.Format(" AND UPPER({0}) LIKE @FilterText OR UPPER(d.Pinyin) LIKE @FilterText ", filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "d.Seq");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        departments.Add(new DepartmentInfo(dr));
                    }
                }
            }

            return departments;
        }

        /// <summary>
        /// 获取科室
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>科室信息</returns>
        public List<DepartmentInfo> QueryDepartment4AutoComplete(string inputText, int curRowNum = 0, int pageSize = 0)
        {
            List<DepartmentInfo> infos = new List<DepartmentInfo>();

            sqlStr = "SELECT d.* FROM lkpDepartment AS d WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(inputText))
                sqlStr += " AND (UPPER(d.Description) LIKE @InputText OR UPPER(d.ID) LIKE @InputText OR UPPER(d.Pinyin) LIKE @InputText)";
            sqlStr += " ORDER BY d.Seq ";

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!string.IsNullOrEmpty(inputText))
                    command.Parameters.Add("@InputText", SqlDbType.NVarChar).Value = "%" + inputText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new DepartmentInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 获取科室
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>科室信息</returns>
        public List<DepartmentInfo> QueryDepartment4AutoCompleteVal(string inputText, int curRowNum = 0, int pageSize = 0)
        {
            List<DepartmentInfo> infos = new List<DepartmentInfo>();

            sqlStr = "SELECT DISTINCT(ve.Department) as Description, d.* FROM lkpDepartment AS d " +
                    " RIGHT JOIN tblValEquipment AS ve ON ve.Department = d.Description" +
                    " WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(inputText))
                sqlStr += " AND (UPPER(ve.Department) LIKE @InputText OR UPPER(d.ID) LIKE @InputText OR UPPER(d.Pinyin) LIKE @InputText)";
            sqlStr += " ORDER BY d.Seq ";

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!string.IsNullOrEmpty(inputText))
                    command.Parameters.Add("@InputText", SqlDbType.NVarChar).Value = "%" + inputText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new DepartmentInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据科室编号获取科室信息
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <returns>科室信息</returns>
        public DepartmentInfo GetDepartmentByID(int id)
        {
            sqlStr = "SELECT d.* FROM lkpDepartment AS d " +
                     " WHERE d.ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new DepartmentInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 判断科室名称是否存在
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <param name="departmentName">科室名称</param>
        /// <returns>科室名称是否存在</returns>
        public bool CheckDepartmentName(int id, string departmentName)
        {
            sqlStr = "SELECT COUNT(ID) FROM lkpDepartment WHERE ID <> @ID AND UPPER(Description) = @Description ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = departmentName.ToUpper();
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// 新增科室
        /// </summary>
        /// <param name="info">科室信息</param>
        /// <returns>科室id</returns>
        public int AddDepartment(DepartmentInfo info)
        {
            sqlStr = "INSERT INTO lkpDepartment(Description,Pinyin,TypeID,Seq) " +
                     " VALUES(@Description,@Pinyin,@TypeID,@Seq);" +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = info.Description;
                command.Parameters.Add("@Pinyin", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.Pinyin);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = info.DepartmentType.ID;
                command.Parameters.Add("@Seq", SqlDbType.Int).Value = info.Seq;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }
        /// <summary>
        /// 修改科室信息
        /// </summary>
        /// <param name="info">科室信息</param>
        /// <returns>科室id</returns>
        public int UpdateDepartment(DepartmentInfo info)
        {
            sqlStr = "UPDATE lkpDepartment SET Description=@Description,TypeID=@TypeID,Seq=@Seq,Pinyin=@Pinyin" +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = info.Description;
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = info.DepartmentType.ID;
                command.Parameters.Add("@Seq", SqlDbType.Int).Value = info.Seq;
                command.Parameters.Add("@Pinyin", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.Pinyin);

                command.ExecuteScalar();

                return info.ID;
            }
        }
        /// <summary>
        /// 修改科室排序
        /// </summary>
        /// <param name="fromSeq">排序起始序号</param>
        /// <param name="toSeq">排序截止序号</param>
        /// <param name="step">改动值</param>
        public void UpdateDepartmentSeq(int fromSeq, int toSeq, int step)
        {
            sqlStr = "UPDATE lkpDepartment SET Seq = Seq + @Step" +
                    " WHERE Seq BETWEEN @FromSeq AND @toSeq";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FromSeq", SqlDbType.Int).Value = fromSeq;
                command.Parameters.Add("@toSeq", SqlDbType.Int).Value = toSeq;
                command.Parameters.Add("@Step", SqlDbType.Int).Value = step;

                command.ExecuteScalar();
            }
        }
    }
}
