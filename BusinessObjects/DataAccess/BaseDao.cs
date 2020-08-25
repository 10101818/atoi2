using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects.Util;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// baseDao
    /// </summary>
    public class BaseDao
    {
        /// <summary>
        /// The SQL string
        /// </summary>
        protected string sqlStr;
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>sql语句</returns>
        protected string AppendLimitClause(string sqlStr, int curRowNum, int pageSize)
        {
            if (pageSize > 0)
                sqlStr += string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", curRowNum, pageSize);

            return sqlStr;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="direction">排序方式</param>
        /// <param name="field">排序字段名</param>
        /// <param name="idField">id字段名</param>
        /// <returns>sql语句</returns>
        protected string GenerateSortClause(bool direction, string field, string idField)
        {
            string sortDirection = null;
            if (direction)
                sortDirection = "ASC";
            else
                sortDirection = "DESC";

            if (field.Equals(idField))
                return string.Format("ORDER BY {0} {1}", field, sortDirection);
            else
                return string.Format("ORDER BY {0} {2}, {1} {2}", field, idField, sortDirection);
        }
        /// <summary>
        /// 获取DataTable类型数据库数据
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <param name="tableName">表名</param>
        /// <returns>DataTable类型数据库数据</returns>
        protected DataTable GetDataTable(SqlCommand command, string tableName = "")
        {
            DataTable dt = null;
            if (string.IsNullOrEmpty(tableName))
                dt = new DataTable();
            else
                dt = new DataTable(tableName);

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dt);
            }

            return dt;
        }
        /// <summary>
        /// 获取数据库第一行数据
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库第一行数据</returns>
        protected DataRow GetDataRow(SqlCommand command)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// 获取数据库中第一行第一列的值(int)
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库中第一行第一列的值(int)</returns>
        protected int GetCount(SqlCommand command)
        {
            return SQLUtil.ConvertInt(command.ExecuteScalar());
        }
        /// <summary>
        /// 获取数据库中第一行第一列的值(double)
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库中第一行第一列的值(int)</returns>
        protected double GetDouble(SqlCommand command)
        {
            return SQLUtil.ConvertDouble(command.ExecuteScalar());
        }
        /// <summary>
        /// 获取数据库中第一列的值(int)
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库中第一列的值(int)</returns>
        protected List<int> GetList(SqlCommand command)
        {
            List<int> list = new List<int>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(SQLUtil.ConvertInt(dr[0]));
                    }
                }
                return list;
            }
        }
        /// <summary>
        /// 获取数据库中第一列的值(string)
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库中第一列的值(string)</returns>
        protected List<string> GetStringList(SqlCommand command)
        {
            List<string> list = new List<string>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(SQLUtil.TrimNull(dr[0]));
                    }
                }
                return list;
            }
        }
        /// <summary>
        /// 获取数据库Dictionary类型数据
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库Dictionary类型数据</returns>
        protected Dictionary<int, int> GetDictionary(SqlCommand command)
        {
            Dictionary<int, int> dicCount = new Dictionary<int, int>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        dicCount.Add(SQLUtil.ConvertInt(dr[0]), SQLUtil.ConvertInt(dr[1]));
                    }
                }
                return dicCount;
            }
        }
        
        /// <summary>
        /// Gets the string dictionary.获取数据库Dictionary(string, int)类型数据
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>数据库Dictionary(string,int)类型数据</returns>
        protected Dictionary<string, int> GetStringDictionary(SqlCommand command)
        {
            Dictionary<string, int> dicCount = new Dictionary<string, int>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        dicCount.Add(SQLUtil.TrimNull(dr[0]), SQLUtil.ConvertInt(dr[1]));
                    }
                }
                return dicCount;
            }
        }

        /// <summary>
        /// 获取数据库Dictionary(string, double)public class Detail
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库Dictionary(string, double)类型数据</returns>
        protected Dictionary<string, double> GetStringDoubleDictionary(SqlCommand command)
        {
            Dictionary<string, double> dicCount = new Dictionary<string, double>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        dicCount.Add(SQLUtil.TrimNull(dr[0]), SQLUtil.ConvertDouble(dr[1]));
                    }
                }
                return dicCount;
            }
        }
        /// <summary>
        /// 获取数据库Dictionary(string, string）类型数据
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <returns>数据库Dictionary（string, string)类型数据</returns>
        protected Dictionary<string, string> GetStringsDictionary(SqlCommand command)
        {
            Dictionary<string, string> dicCount = new Dictionary<string, string>();
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        dicCount.Add(SQLUtil.TrimNull(dr[0]), SQLUtil.TrimNull(dr[1]));
                    }
                }
                return dicCount;
            }
        }
        /// <summary>
        /// 根据搜索条件返回对应sql语句
        /// </summary>
        /// <param name="filterField">搜索条件</param>
        /// <returns>sql语句</returns>
        protected string GetFieldFilterClause(string filterField)
        {
            if (filterField.EndsWith("ID"))
            {
                return string.Format(" AND {0} = @FilterText ", filterField);
            }
            else
            {
                return string.Format(" AND UPPER({0}) Like @FilterText ", filterField);
            }
        }
        /// <summary>
        /// 根据搜索条件获取对应字段类型
        /// </summary>
        /// <param name="command">SqlCommand</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        protected void AddFieldFilterParam(SqlCommand command, string filterField, string filterText)
        {
            if (filterField.EndsWith("ID"))
            {
                command.Parameters.Add("@FilterText", SqlDbType.Int).Value = filterText;
            }
            else
            {
                command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";
            }
        }
        /// <summary>
        /// 按照ID搜索时删除数字之外内容
        /// </summary>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        public static void ProcessFieldFilterValue(string filterField, ref string filterText)
        {
            if (filterField.EndsWith("ID"))
            {
                filterText = Regex.Replace(filterText, "[^0-9]", "", RegexOptions.IgnoreCase);
                int value = 0;
                if (Int32.TryParse(filterText, out value) == true)
                {
                    filterText = value.ToString();
                }
                else
                {
                    filterText = "";
                }
            }
        }
        /// <summary>
        /// 取字符串中的数字
        /// </summary>
        /// <param name="filterText">string</param>
        /// <returns>num</returns>
        public static int ProcessOID(string filterText)
        {            
            filterText = Regex.Replace(filterText, "[^0-9]", "", RegexOptions.IgnoreCase);
            int value = 0;
            Int32.TryParse(filterText, out value);

            return value;
        }
        /// <summary>
        ///SqlCommand获取数据并封装实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        protected List<T> GetList<T>(SqlCommand command)
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }

                List<T> l = new List<T>();

                DataRowInstantiation(l, dt);

                return l;
            }
        }
        /// <summary>
        /// 获取带DataRow参数的构造函数实例化填充集合
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="infos">集合</param>
        /// <param name="dt">数据表</param>
        public static void DataRowInstantiation<T>(List<T> infos, DataTable dt)
        {

            ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[] { typeof(DataRow) });

            if (constructorInfo == null) return;

            object[] consargs = new object[1];
            foreach (DataRow dr in dt.Rows)
            {
                consargs[0] = dr;
                infos.Add((T)constructorInfo.Invoke(consargs)); 
            }
        }
    }
}
