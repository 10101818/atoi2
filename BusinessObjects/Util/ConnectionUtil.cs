using System;
using System.Data;
using System.Data.SqlClient;

namespace BusinessObjects.Util
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class ConnectionUtil : IDisposable
    {
        /// <summary>
        /// sql
        /// </summary>
        public static string connectionStr = null;

        [ThreadStatic] // static per thread
        private static SqlConnection conn = null;

        [ThreadStatic] // static per thread
        private static SqlTransaction trans = null; 

        private bool connOwner = false;
        private bool transOwner = false; 

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public ConnectionUtil()
        {
            if (conn == null)
            {
                conn = new SqlConnection(connectionStr);
                conn.Open();

                this.connOwner = true;
            }
        }

        /// <summary>
        /// 初始化数据库地址
        /// </summary>
        /// <param name="configString">数据库地址</param>
        public static void Init(string configString)
        {
            connectionStr = configString;
        }

        /// <summary>
        /// 获取数据库名称
        /// </summary>
        /// <returns>数据库名称</returns>
        public static string GetDatabaseName()
        {
            return conn.Database;
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sqlStr">sql</param>
        public static void ExecuteSQL(string sqlStr)
        {
            using (SqlCommand command = GetCommand(sqlStr))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取SqlCommand
        /// </summary>
        /// <param name="sqlStr">sql</param>
        /// <param name="type">CommandType</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand GetCommand(string sqlStr, CommandType type = CommandType.Text)
        {
            SqlCommand command = new SqlCommand(sqlStr, conn);
            command.CommandTimeout = 300;
            command.CommandType = type;
            if (trans != null)
                command.Transaction = trans;

            return command;
        }



        /// <summary>
        /// 获取SqlCommand
        /// </summary> 
        /// <returns>SqlCommand</returns>
        public static SqlBulkCopy GetBulkCopy()
        { 
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans);
            bulkCopy.BulkCopyTimeout = 300; 
            return bulkCopy;
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        public void BeginTransaction()
        {
            if (trans == null)
            {
                trans = conn.BeginTransaction();

                this.transOwner = true;
            }
        }

        /// <summary>
        /// 提交数据库事务
        /// </summary>
        public void CommitTransaction()
        {
            if (this.transOwner == true)
            {
                trans.Commit();
                trans = null;
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            if (this.transOwner == true)
            {
                trans.Rollback();
                trans = null;
            }
        }
        
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Dispose()
        {
            if (this.connOwner == true)
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();

                    conn = null;
                }
            }
        }
    }
}
