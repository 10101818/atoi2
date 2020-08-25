using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Util;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 邮箱、短信信息
    /// </summary>
    public class SmtpInfo
    {
        /// <summary>
        /// 管理员邮箱
        /// </summary>
        /// <value>
        /// The admin email.
        /// </value>
        public string AdminEmail { get; set; }
        /// <summary>
        /// 邮件服务器地址
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; set; }
        /// <summary>
        /// 启用SSL
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Pwd { get; set; }
        /// <summary>
        /// 邮件发送者
        /// </summary>
        /// <value>
        /// The email from.
        /// </value>
        public string EmailFrom { get; set; }
        /// <summary>
        /// 短信APPKEY
        /// </summary>
        /// <value>
        /// The message key.
        /// </value>
        public string MessageKey { get; set; }
        /// <summary>
        /// 是否开启短信平台
        /// </summary>
        /// <value>
        ///   <c>true</c> if [message enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool MessageEnabled { get; set; }
        /// <summary>
        /// Gets or sets the application valid version.
        /// </summary>
        /// <value>
        /// The application valid version.
        /// </value>
        public string AppValidVersion { get; set; }
        /// <summary>
        /// 接收提醒手机
        /// </summary>
        /// <value>
        /// The mobile phone.
        /// </value>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 系统信息
        /// </summary>
        public SmtpInfo()
        { }
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public SmtpInfo(DataRow dr)
            : this()
        {
            this.AdminEmail = SQLUtil.TrimNull(dr["AdminEmail"]);
            this.Host = SQLUtil.TrimNull(dr["SmtpHost"]);
            this.Port = SQLUtil.ConvertInt(dr["SmtpPort"]);
            this.UseSsl = SQLUtil.ConvertBoolean(dr["SmtpUseSsl"]);
            this.UserName = SQLUtil.TrimNull(dr["SmtpUserName"]);
            this.Pwd = SQLUtil.TrimNull(dr["SmtpPwd"]);
            this.EmailFrom = SQLUtil.TrimNull(dr["SmtpEmailFrom"]);
            this.MessageKey = SQLUtil.TrimNull(dr["MessageKey"]);
            this.MessageEnabled = SQLUtil.ConvertBoolean(dr["MessageEnabled"]);
            this.AppValidVersion = SQLUtil.TrimNull(dr["AppValidVersion"]);
            this.MobilePhone = SQLUtil.TrimNull(dr["MobilePhone"]);
        }
    }
    /// <summary>
    /// 预警时间、异常信息
    /// </summary>
    public class SettingInfo
    {
        /// <summary>
        /// 即将到期时间(天)
        /// </summary>
        /// <value>
        /// The will expire time.
        /// </value>
        public int WillExpireTime { get; set; }  //即将到期时间(天)        
        /// <summary>
        /// 超期时间(天)
        /// </summary>
        /// <value>
        /// The over due time.
        /// </value>
        public int OverDueTime { get; set; }     //超期时间(天)        
        /// <summary>
        /// 报错信息
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }  //报错信息

        /// <summary>
        /// 系统信息
        /// </summary>
        public SettingInfo()
        { }
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public SettingInfo(DataRow dr)
            : this()
        {
            this.WillExpireTime = SQLUtil.ConvertInt(dr["WillExpireTime"]);
            this.OverDueTime = SQLUtil.ConvertInt(dr["OverDueTime"]);
            this.ErrorMessage = SQLUtil.TrimNull(dr["ErrorMessage"]);
        }
    }
}
