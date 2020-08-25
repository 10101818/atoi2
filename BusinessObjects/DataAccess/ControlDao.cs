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
    /// 系统信息dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ControlDao : BaseDao
    {
        /// <summary>
        /// 获取邮件、短信信息
        /// </summary>
        /// <returns>邮件、短信信息</returns>
        public SmtpInfo GetSmtpInfo()
        {
            sqlStr = "SELECT * from tblControl ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                DataRow dr = GetDataRow(command);
                return new SmtpInfo(dr);
            }
        }
        /// <summary>
        /// 更新邮件、短信信息
        /// </summary>
        /// <param name="info">邮件、短信信息</param>
        public void UpdateSmtpInfo(SmtpInfo info)
        {
            sqlStr = "UPDATE tblControl SET AdminEmail = @AdminEmail, SmtpHost = @SmtpHost, SmtpPort = @SmtpPort, SmtpUseSsl = @SmtpUseSsl," +
                     " SmtpUserName = @SmtpUserName, SmtpPwd = @SmtpPwd, SmtpEmailFrom = @SmtpEmailFrom, MessageEnabled = @MessageEnabled, MessageKey = @MessageKey,AppValidVersion = @AppValidVersion,MobilePhone=@MobilePhone ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@AdminEmail", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.AdminEmail);
                command.Parameters.Add("@SmtpHost", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Host);
                command.Parameters.Add("@SmtpPort", SqlDbType.Int).Value = info.Port;
                command.Parameters.Add("@SmtpUseSsl", SqlDbType.Bit).Value = info.UseSsl;
                command.Parameters.Add("@SmtpUserName", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.UserName);
                command.Parameters.Add("@SmtpPwd", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.Pwd);
                command.Parameters.Add("@SmtpEmailFrom", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.EmailFrom);
                command.Parameters.Add("@MessageKey", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.MessageKey);
                command.Parameters.Add("@MessageEnabled", SqlDbType.Bit).Value = info.MessageEnabled;
                command.Parameters.Add("@AppValidVersion", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.AppValidVersion);
                command.Parameters.Add("@MobilePhone", SqlDbType.VarChar).Value = SQLUtil.EmptyStringToNull(info.MobilePhone);
                

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取报错信息和预警时间信息
        /// </summary>
        /// <returns>报错信息和预警时间信息</returns>
        public SettingInfo GetSettingInfo()
        {
            sqlStr = "SELECT * from tblControl ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                DataRow dr = GetDataRow(command);
                return new SettingInfo(dr);
            }
        }
        /// <summary>
        /// 更新报错信息
        /// </summary>
        /// <param name="info">报错信息</param>
        public void UpdateErrorMessageInfo(SettingInfo info)
        {
            sqlStr = "UPDATE tblControl SET ErrorMessage = @ErrorMessage";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar).Value = SQLUtil.EmptyStringToNull(info.ErrorMessage);


                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 更新预警时间信息
        /// </summary>
        /// <param name="info">预警时间信息</param>
        public void UpdateSaveWarningTimeInfo(SettingInfo info)
        {
            sqlStr = "UPDATE tblControl SET OverDueTime = @OverDueTime,WillExpireTime = @WillExpireTime";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@OverDueTime", SqlDbType.NVarChar).Value = SQLUtil.ConvertInt(info.OverDueTime);
                command.Parameters.Add("@WillExpireTime", SqlDbType.NVarChar).Value = SQLUtil.ConvertInt(info.WillExpireTime);


                command.ExecuteNonQuery();
            }
        }
    }
}
