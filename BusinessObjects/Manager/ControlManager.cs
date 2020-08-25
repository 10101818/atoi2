using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 系统设置manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ControlManager
    {
        private static SmtpInfo _SmtpInfo = null;
        private static SettingInfo _SettingInfo = null;
        // syncLock object, used to lock the code block
        private static object syncLock = new object();

        #region "SmtpEmail"
        /// <summary>
        /// 获取短信设置、邮件设置信息
        /// </summary>
        /// <returns>短信设置、邮件设置信息</returns>
        public static SmtpInfo GetSmtpInfo()
        {
            //To ensure thread safety
            lock (syncLock)
            {
                if (_SmtpInfo == null)
                {
                    _SmtpInfo = new ControlDao().GetSmtpInfo();
                }
            }

            return _SmtpInfo;
        }

        /// <summary>
        /// 修改短信设置、邮件设置信息
        /// </summary>
        /// <param name="info">短信设置、邮件设置信息</param>
        [TransactionAspect]
        public static void  UpdateSmtpInfo(SmtpInfo info)
        {
            lock (syncLock)
            {
                new ControlDao().UpdateSmtpInfo(info);

                _SmtpInfo = null;
            }
        }
        #endregion
                
        #region "Setting"
        /// <summary>
        /// 获取预警时间、异常设置内容
        /// </summary>
        /// <returns>预警时间、异常设置内容</returns>
        public static SettingInfo GetSettingInfo()
        {
            //To ensure thread safety
            lock (syncLock)
            {
                if (_SettingInfo == null)
                {
                    _SettingInfo = new ControlDao().GetSettingInfo();
                }
            }

            return _SettingInfo;
        }

        #region "ErrorMessage"
        /// <summary>
        /// 修改异常设置内容
        /// </summary>
        /// <param name="info">异常设置内容</param>
        [TransactionAspect]
        public static void UpdateErrorMessageInfo(SettingInfo info)
        {
            lock (syncLock)
            {
                new ControlDao().UpdateErrorMessageInfo(info);

                _SettingInfo = null;
            }
        }
        #endregion

        #region "WarningTime"
        /// <summary>
        /// 修改预警时间设置内容
        /// </summary>
        /// <param name="info">预警时间设置内容</param>
        [TransactionAspect]
        public static void UpdateSaveWarningTimeInfo(SettingInfo info)
        {
            lock (syncLock)
            {
                new ControlDao().UpdateSaveWarningTimeInfo(info);

                _SettingInfo = null;
            }
        }
        #endregion

        #endregion
    }
}
