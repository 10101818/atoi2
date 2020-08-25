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
    /// 用户dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class UserDao : BaseDao
    {
        #region "tblUser"
        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        public UserInfo GetUser(int id)
        {
            sqlStr = "SELECT u.* FROM tblUser AS u " +
                     " WHERE u.ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new UserInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="loginID">用户名</param>
        /// <returns>用户信息</returns>
        public UserInfo GetUserByLoginID(string loginID)
        {
            sqlStr = "SELECT u.* FROM tblUser AS u " +
                     " WHERE UPPER(u.LoginID) = @LoginID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LoginID", SqlDbType.VarChar).Value = loginID.ToUpper();

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new UserInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="verifyStatusID">审批状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>用户数量</returns>
        public int QueryUsersCount(int status, int roleId, int verifyStatusID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(DISTINCT u.ID) FROM tblUser AS u ";
            sqlStr += " WHERE 1=1 ";
            if (status > -1)
                sqlStr += " AND u.IsActive = " + status;
            if (roleId != -1)
                sqlStr += " AND u.RoleID = " + roleId ;
            if (verifyStatusID > 0)
                sqlStr += " AND u.VerifyStatus = " + verifyStatusID;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += string.Format(" AND UPPER({0}) LIKE @FilterText", filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="verifyStatusID">审批状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>用户列表信息</returns>
        public List<UserInfo> QueryUsers(int status, int roleId, int verifyStatusID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum, int pageSize)
        {
            List<UserInfo> infos = new List<UserInfo>();

            sqlStr = "SELECT DISTINCT u.* FROM tblUser AS u " ;
            sqlStr += " WHERE 1=1 ";
            if (status > -1)
                sqlStr += " AND u.IsActive = " + status;
            if (roleId > -1)
                sqlStr += " AND u.RoleID IN ( " + roleId + " )";
            if (verifyStatusID > 0)
                sqlStr += " AND u.VerifyStatus = " + verifyStatusID;
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += string.Format(" AND UPPER({0}) LIKE @FilterText ", filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "u.LoginID");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new UserInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取工程师信息
        /// </summary>
        /// <returns>工程师信息</returns>
        public List<UserInfo> GetUsers4Dispatch()
        {
            List<UserInfo> infos = new List<UserInfo>();

            sqlStr = "SELECT DISTINCT u.*, (Case when p.ID is null then 0 else 1 end) as HasOpenDispatch FROM tblUser AS u  " + 
                    " LEFT JOIN tblDispatch AS p on p.EngineerID = u.ID And p.StatusID <> @Cancelled AND p.StatusID <> @Approved" + 
                    " WHERE u.IsActive = 1  AND u.RoleID = " + UserRole.Admin;
            sqlStr += " ORDER BY u.Name, u.ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Cancelled", SqlDbType.Int).Value = DispatchInfo.Statuses.Cancelled;
                command.Parameters.Add("@Approved", SqlDbType.Int).Value = DispatchInfo.Statuses.Approved;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new UserInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 判断用户名是否存在
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="loginId">用户名</param>
        /// <returns>用户名是否存在</returns>
        public bool CheckUserLoginID(int id, string loginId)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblUser WHERE ID <> @ID AND UPPER(LoginID) = @LoginID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LoginID", SqlDbType.VarChar).Value = loginId.ToUpper();
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// 根据用户角色ID获取用户信息
        /// </summary>
        /// <param name="roleIds">用户角色ID</param>
        /// <returns>用户信息</returns>
        public List<UserInfo> GetActiveUsers(List<int> roleIds)
        {
            List<UserInfo> infos = new List<UserInfo>();

            sqlStr = "SELECT DISTINCT u.*  FROM tblUser AS u ";
            sqlStr += " WHERE u.IsActive = 1 ";
            if (roleIds != null && roleIds.Count > 0)
                sqlStr += " AND u.RoleID IN ( " + SQLUtil.ConvertToInStr(roleIds) + " )";
            sqlStr += " ORDER BY u.Name, u.ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new UserInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据用户ID获取用户最近一次登录Session
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        public string GetWebSessionID(int id)
        {
            sqlStr = "SELECT u.WebSessionID FROM tblUser AS u ";
            sqlStr += " WHERE u.ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                return (dr != null) ? SQLUtil.TrimNull(dr[0]) : "";
            }
        }
        /// <summary>
        /// 根据用户ID获取用户最近一次App登录Session
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        public string GetSessionID(int id)
        {
            sqlStr = "SELECT u.SessionID FROM tblUser AS u ";
            sqlStr += " WHERE u.ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                return (dr != null) ? SQLUtil.TrimNull(dr[0]) : "";
            }
        }
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>用户信息</returns>
        public UserInfo AddUser(UserInfo info)
        {
            sqlStr = "INSERT INTO tblUser(LoginID,LoginPwd,Name,RoleID,Address, " +
                     " Mobile,Email,IsActive,LastLoginDate,CreatedDate,VerifyStatus,Department ) " +
                     " VALUES(@LoginID,@LoginPwd,@Name,@RoleID,@Address, " +
                     " @Mobile,@Email,@IsActive,null,GETDATE(),@VerifyStatus,@Department );" +
                     " SELECT @@IDENTITY";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LoginID", SqlDbType.VarChar).Value = info.LoginID;
                command.Parameters.Add("@LoginPwd", SqlDbType.VarChar).Value = info.LoginPwd;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = info.Name;
                command.Parameters.Add("@RoleID", SqlDbType.Int).Value = info.Role.ID;

                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Address);
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Mobile);
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Email);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = info.IsActive;
                command.Parameters.Add("@VerifyStatus", SqlDbType.Int).Value = info.VerifyStatus.ID;
                command.Parameters.Add("@Department", SqlDbType.Int).Value = info.Department.ID;

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info;
            }
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        public void UpdateUser(UserInfo info)
        {
            sqlStr = "UPDATE tblUser SET LoginID = @LoginID, Name = @Name, RoleID = @RoleID, " +
                     " Mobile = @Mobile,Email=@Email,Address = @Address, IsActive = @IsActive,LoginPwd = @LoginPwd,Department = @Department,VerifyStatus = @VerifyStatus,Comments = @Comments ";

            sqlStr += "WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LoginID", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.LoginID);
                command.Parameters.Add("@LoginPwd", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.LoginPwd);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = info.Name;
                command.Parameters.Add("@RoleID", SqlDbType.Int).Value = info.Role.ID;
                command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Mobile);
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Address);
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Email);
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = info.IsActive;
                command.Parameters.Add("@Department", SqlDbType.Int).Value = info.Department.ID;
                command.Parameters.Add("@VerifyStatus", SqlDbType.Int).Value = info.VerifyStatus.ID;
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.Comments);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新用户上次登录时间
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="sessionID">用户当前SessionID</param>
        public void UpdateLastLoginInfo(int id, string sessionID)
        {
            sqlStr = "UPDATE tblUser SET LastLoginDate = GETDATE() ,WebSessionID = @WebSessionID WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@WebSessionID", SqlDbType.VarChar).Value = sessionID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="loginPwd">密码</param>
        public void UpdatePassword(int id, string loginPwd)
        {
            sqlStr = "UPDATE tblUser SET LoginPwd = @LoginPwd WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@LoginPwd", SqlDbType.VarChar).Value = loginPwd;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region "App"
        /// <summary>
        /// 手机用户登录更新用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        public void Login4App(UserInfo info)
        {
            sqlStr = "UPDATE tblUser SET LastLoginDate = GETDATE(), RegistrationID = @RegistrationID, OSName = @OSName  ,SessionID = @SessionID" +
                    " WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RegistrationID", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.RegistrationID);
                command.Parameters.Add("@OSName", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.OSName);
                command.Parameters.Add("@SessionID", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.SessionID);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取最新验证信息
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <returns>验证信息</returns>
        public VerificationCodeInfo GetLastVerificationCode(string mobilePhone)
        {
            sqlStr = "SELECT * FROM tblPhoneVerify WHERE MobilePhone = @MobilePhone AND IsUsed IS NULL ORDER BY CreatedTime DESC, ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@MobilePhone", SqlDbType.VarChar).Value = mobilePhone;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new VerificationCodeInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 新增手机验证信息
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <param name="verificationCode">验证码</param>
        public void AddPhoneVerify(string mobilePhone, string verificationCode)
        {
            sqlStr = "INSERT INTO tblPhoneVerify (MobilePhone, VerificationCode, CreatedTime) " +
                    " VALUES(@MobilePhone, @VerificationCode, GETDATE())";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@MobilePhone", SqlDbType.VarChar).Value = mobilePhone;
                command.Parameters.Add("@VerificationCode", SqlDbType.VarChar).Value = verificationCode;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新验证码已用
        /// </summary>
        /// <param name="id">验证信息ID</param>
        public void UpdatePhoneVerifyIsUsed(int id)
        {
            sqlStr = "UPDATE tblPhoneVerify SET IsUsed = 1 WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
