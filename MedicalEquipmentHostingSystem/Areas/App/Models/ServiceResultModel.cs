using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MedicalEquipmentHostingSystem.App_Start;

namespace MedicalEquipmentHostingSystem.Areas.App.Models
{
    /// <summary>
    /// 转换器
    /// </summary>
    public class ServiceResultModelBase
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        /// <value>
        /// 错误编号
        /// </value>
        public string ResultCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <value>
        /// 错误信息
        /// </value>
        public string ResultMessage { get; set; }
        /// <summary>
        /// 成功返回信息
        /// </summary>
        public ServiceResultModelBase()
        {
            this.ResultCode = ResultCodes.Succeed;
            this.ResultMessage = string.Empty;
        }
        /// <summary>
        /// 返回信息
        /// </summary>
        /// <param name="resultCode">错误编号</param>
        /// <param name="resultMessage">错误信息</param>
        public ServiceResultModelBase(string resultCode, string resultMessage)
        {
            this.ResultCode = resultCode;
            this.ResultMessage = resultMessage;
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="resultCode">错误编号</param>
        /// <param name="resultMessage">错误信息</param>
        public void SetFailed(string resultCode, string resultMessage)
        {
            this.ResultCode = resultCode;
            this.ResultMessage = resultMessage;
        }
    }

    /// <summary>
    /// ServiceResultModel
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class ServiceResultModel<T> : ServiceResultModelBase
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }
    }
}