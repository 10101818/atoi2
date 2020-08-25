using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MedicalEquipmentHostingSystem.App_Start
{
    /// <summary>
    /// app WebConfig
    /// </summary>
    public class WebConfig
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        public static readonly string SQL_CONNECTION_STRING = ConfigurationManager.AppSettings["SqlConnectionString"].ToString().Trim();
        /// <summary>
        /// The backgroup process interval
        /// </summary>
        public static readonly int BACKGROUP_PROCESS_INTERVAL = Convert.ToInt32(ConfigurationManager.AppSettings["BackgroupProcessInterval"].ToString().Trim());
        /// <summary>
        /// The SMS period
        /// </summary>
        public static readonly int SMS_PERIOD = Convert.ToInt32(ConfigurationManager.AppSettings["SMSPeriod"]);
        /// <summary>
        /// sms内容
        /// </summary>
        public static readonly string SMS_CONTENT = SQLUtil.TrimNull(ConfigurationManager.AppSettings["SMSContent"]);
        /// <summary>
        /// SMS签名
        /// </summary>
        public static readonly string SMS_SIGNATURE = SQLUtil.TrimNull(ConfigurationManager.AppSettings["SMSSignature"]);
        /// <summary>
        /// The SMS templateid
        /// </summary>
        public static readonly int SMS_TEMPLATEID = Convert.ToInt32(ConfigurationManager.AppSettings["SMSTemplate"]);
        /// <summary>
        /// SMS_SDK_APP_ID
        /// </summary>
        public static readonly int SMS_SDK_APP_ID = SQLUtil.ConvertInt(ConfigurationManager.AppSettings["SMS_SdkAppId"]);
        /// <summary>
        /// SMS_SDK_APP_KEY
        /// </summary>
        public static readonly string SMS_SDK_APP_KEY = SQLUtil.TrimNull(ConfigurationManager.AppSettings["SMS_AppKey"]);
        /// <summary>
        /// 极光推送JPUSH_APP_KEY
        /// </summary>
        public static readonly string JPUSH_APP_KEY = SQLUtil.TrimNull(ConfigurationManager.AppSettings["JPush_AppKey"]);
        /// <summary>
        /// 极光推送JPUSH_MASTER_SECRET
        /// </summary>
        public static readonly string JPUSH_MASTER_SECRET = SQLUtil.TrimNull(ConfigurationManager.AppSettings["JPush_MasterSecret"]);
        /// <summary>
        /// 系统名称
        /// </summary>
        public static readonly string SYSTEM_NAME = SQLUtil.TrimNull(ConfigurationManager.AppSettings["SystemName"]);
        /// <summary>
        ///  医院名称
        /// </summary>
        public static readonly string HOSPITAL_NAME = SQLUtil.TrimNull(ConfigurationManager.AppSettings["HospitalName"]);
        /// <summary>
        /// 开机率
        /// </summary>
        public static readonly double BOOT_RATE = Convert.ToDouble(ConfigurationManager.AppSettings["BootRate"]);
        /// <summary>
        /// The check session on dashborad API
        /// </summary>
        public static readonly bool CHECK_SESSION_ON_DASHBORAD_API = Convert.ToBoolean(ConfigurationManager.AppSettings["CheckSessionOnDashboradAPI"]);
        /// <summary>
        /// 全局验证SessionID开关
        /// </summary>
        public static readonly bool GLOBAL_SESSIONID_FLAG = Convert.ToBoolean(ConfigurationManager.AppSettings["GlobalSessionIDFlag"]);

        public static readonly bool ISDEMO = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDemo"]);

        public static readonly bool AutoAssetCode = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoAssetCode"]);

    }
}