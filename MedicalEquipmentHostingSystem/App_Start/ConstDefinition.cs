using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MedicalEquipmentHostingSystem.App_Start
{
    /// <summary>
    /// 常量信息
    /// </summary>
    public class ConstDefinition
    {
        /// <summary>
        /// The home controller
        /// </summary>
        public const string HOME_CONTROLLER = "Home";
        /// <summary>
        /// The home action
        /// </summary>
        public const string HOME_ACTION = "Login";
        /// <summary>
        /// 列表页面信息条数
        /// </summary>
        public const int PAGE_SIZE = 10;
        /// <summary>
        /// The page size maximum
        /// </summary>
        public const int PAGE_SIZE_MAX = 1000;
        /// <summary>
        /// The automatic complete maximum
        /// </summary>
        public const int AUTO_COMPLETE_MAX = 30;
        /// <summary>
        /// 日期格式
        /// </summary>
        public const string DATEFORMAT_DATEPICKER = "yyyy-MM-dd";
        /// <summary>
        /// 年月格式
        /// </summary>
        public const string DATEFORMAT_DATEPICKER_YM = "yyyy-MM";
        /// <summary>
        /// 时间格式
        /// </summary>
        public const string DATEFORMAT_DATETIMEPICKER = "HH:mm";
    }

    /// <summary>
    /// 错误编号
    /// </summary>
    public static class ResultCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string Succeed = "00";
        /// <summary>
        /// 超时
        /// </summary>
        public const string TimeoutError = "01";
        /// <summary>
        /// 参数错误
        /// </summary>
        public const string ParameterError = "02";
        /// <summary>
        /// 业务逻辑错误
        /// </summary>
        public const string BusinessError = "03";
        /// <summary>
        /// 已在其他设备登录
        /// </summary>
        public const string LogoutError = "04";
        /// <summary>
        /// 系统错误
        /// </summary>
        public const string SystemError = "09";

    }
}