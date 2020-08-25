using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MedicalEquipmentHostingSystem.App_Start;

namespace MedicalEquipmentHostingSystem.Models
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public class ResultModelBase
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        /// <value>
        /// The result code.
        /// </value>
        public string ResultCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <value>
        /// The result message.
        /// </value>
        public string ResultMessage { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        public int TotalPages { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public ResultModelBase()
        {
            this.ResultCode = ResultCodes.Succeed;
            this.ResultMessage = string.Empty;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resultCode">错误编号</param>
        /// <param name="resultMessage">错误信息</param>
        public ResultModelBase(string resultCode, string resultMessage)
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

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="recordCount">数据总数</param>
        /// <param name="pageSize">每页展示数据条数</param>
        public void SetTotalPages(int recordCount, int pageSize)
        {
            int totalPages = recordCount / pageSize;

            if (totalPages * pageSize < recordCount)
                totalPages++;

            if (totalPages <= 0)
                totalPages = 1;

            this.TotalPages = totalPages;
        }

        /// <summary>
        /// 获取当前页数第一个数据的位置
        /// </summary>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>当前页数第一个数据的位置</returns>
        public int GetCurRowNum(int currentPage, int pageSize)
        {
            if (currentPage < 1) currentPage = 1;
            if (currentPage > this.TotalPages) currentPage = this.TotalPages;

            return (currentPage - 1) * pageSize;
        }

        /// <summary>
        /// 登录超时错误编号
        /// </summary>
        /// <returns>错误编号</returns>
        public static ResultModelBase CreateTimeoutModel()
        {
            return new ResultModelBase(ResultCodes.TimeoutError, "由于您长时间未进行操作，本页面已经失效，请重新登录！");
        }

        /// <summary>
        /// 异地错误编号
        /// </summary>
        /// <returns>错误编号</returns>
        public static ResultModelBase CreateLogoutModel()
        {
            return new ResultModelBase(ResultCodes.LogoutError, "由于异地登录，本页面已经失效，请重新登录！");
        }
    }

    /// <summary>
    /// ResultModel
    /// </summary>
    /// <typeparam name="T">data</typeparam>
    public class ResultModel<T> : ResultModelBase
    {
        /// <summary>
        /// data
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }
    }
}