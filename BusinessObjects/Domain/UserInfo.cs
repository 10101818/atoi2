using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo : EntityInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        /// <value>
        /// The login identifier.
        /// </value>
        public string LoginID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        /// <value>
        /// The login password.
        /// </value>
        public string LoginPwd { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public KeyValueInfo Role { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        /// <value>
        /// The mobile.
        /// </value>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        /// <value>
        /// The last login date.
        /// </value>
        public DateTime LastLoginDate { get; set; }
        /// <summary>
        /// App上次登录位置标识
        /// </summary>
        /// <value>
        /// The last login SessionID.
        /// </value>
        public string SessionID { get; set; }
        /// <summary>
        /// Web上次登录位置标识
        /// </summary>
        /// <value>
        /// The last login SessionID of App.
        /// </value>
        public string WebSessionID { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 注册编号
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public string RegistrationID { get; set; }
        /// <summary>
        /// 手机名称
        /// </summary>
        /// <value>
        /// The name of the os.
        /// </value>
        public string OSName { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <value>
        /// The verify status.
        /// </value>
        public KeyValueInfo VerifyStatus { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string Comments { get; set; }
        /// <summary>
        /// 科室
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public KeyValueInfo Department { get; set; }

        /// <summary>
        /// 是否有作业中的派工单
        /// </summary>
        //have open dispatch for admin role
        public bool HasOpenDispatch { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo() 
        {
            this.Role = new KeyValueInfo();
            this.VerifyStatus = new KeyValueInfo();
            this.Department = new KeyValueInfo();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public UserInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.LoginID = SQLUtil.TrimNull(dr["LoginID"]);
            this.LoginPwd = SQLUtil.TrimNull(dr["LoginPwd"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Role.ID = SQLUtil.ConvertInt(dr["RoleID"]);
            this.Role.Name = Manager.LookupManager.GetRoleDesc(this.Role.ID); 
            this.Mobile = SQLUtil.TrimNull(dr["Mobile"]);
            this.Email = SQLUtil.TrimNull(dr["Email"]);
            this.Address = SQLUtil.TrimNull(dr["Address"]);
            this.IsActive = SQLUtil.ConvertBoolean(dr["IsActive"]);
            this.LastLoginDate = SQLUtil.ConvertDateTime(dr["LastLoginDate"]);
            this.SessionID = SQLUtil.TrimNull(dr["SessionID"]);
            this.WebSessionID = SQLUtil.TrimNull(dr["WebSessionID"]);
            this.CreatedDate = SQLUtil.ConvertDateTime(dr["CreatedDate"]);
            this.RegistrationID = SQLUtil.TrimNull(dr["RegistrationID"]);
            this.OSName = SQLUtil.TrimNull(dr["OSName"]);
            this.VerifyStatus.ID = SQLUtil.ConvertInt(dr["VerifyStatus"]);
            this.VerifyStatus.Name = BusinessObjects.Domain.VerifyStatus.GetVerifyStatusDesc(this.VerifyStatus.ID);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.Department.ID = SQLUtil.ConvertInt(dr["Department"]);
            this.Department.Name = Manager.LookupManager.GetDepartmentDesc(this.Department.ID);

            if (dr.Table.Columns.Contains("HasOpenDispatch")) this.HasOpenDispatch = SQLUtil.ConvertBoolean(dr["HasOpenDispatch"]);
        }  
    }

    /// <summary>
    /// 用户验证码
    /// </summary>
    public class VerificationCodeInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        /// <value>
        /// The mobile phone.
        /// </value>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <value>
        /// The verification code.
        /// </value>
        public string VerificationCode { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        /// <value>
        /// The created time.
        /// </value>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 验证信息
        /// </summary>
        public VerificationCodeInfo()
        {
        }
        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public VerificationCodeInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.MobilePhone = SQLUtil.TrimNull(dr["MobilePhone"]);
            this.VerificationCode = SQLUtil.TrimNull(dr["VerificationCode"]);
            this.CreatedTime = SQLUtil.ConvertDateTime(dr["CreatedTime"]);
        }
    }
    /// <summary>
    /// 用户审批状态
    /// </summary>
    public class VerifyStatus
    {
        /// <summary>
        /// 已通过
        /// </summary>
        public const int Pass = 1;
        /// <summary>
        /// 待审批
        /// </summary>
        public const int Pending = 2;
        /// <summary>
        /// 已拒绝
        /// </summary>
        public const int Refused = 3;
        /// <summary>
        /// 获取用户审批状态
        /// </summary>
        /// <returns>用户审批状态</returns>
        public static List<KeyValueInfo> GetVerifyStatus()
        {
            List<KeyValueInfo> statuses = new List<KeyValueInfo>();
            statuses.Add(new KeyValueInfo() { ID = VerifyStatus.Pass, Name = GetVerifyStatusDesc(VerifyStatus.Pass) });
            statuses.Add(new KeyValueInfo() { ID = VerifyStatus.Pending, Name = GetVerifyStatusDesc(VerifyStatus.Pending) });
            statuses.Add(new KeyValueInfo() { ID = VerifyStatus.Refused, Name = GetVerifyStatusDesc(VerifyStatus.Refused) });
            return statuses;
        }
        /// <summary>
        /// 根据用户审批状态编号获取审批状态描述
        /// </summary>
        /// <param name="actionID">用户审批状态编号</param>
        /// <returns>审批状态描述</returns>
        public static string GetVerifyStatusDesc(int actionID)
        {
            switch (actionID)
            {
                case Pass:
                    return "已通过";
                case Pending:
                    return "待审批";
                case Refused:
                    return "已拒绝";
                default:
                    return "";
            }
        }
    }
}
