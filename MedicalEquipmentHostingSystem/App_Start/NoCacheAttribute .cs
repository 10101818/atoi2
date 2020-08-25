using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.App_Start
{
    /// <summary>
    /// 结果筛选器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class NoCacheAttribute : FilterAttribute, IResultFilter
    {
        /// <summary>
        /// 操作结果执行之前调用
        /// </summary>
        /// <param name="filterContext">筛选器上下文</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        /// <summary>
        /// 操作结果执行后调用
        /// </summary>
        /// <param name="filterContext">筛选器上下文</param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetRevalidation(HttpCacheRevalidation.ProxyCaches);
            cache.SetExpires(DateTime.Now.AddYears(-5));
            cache.AppendCacheExtension("private");
            cache.AppendCacheExtension("no-cache=Set-Cookie");
            cache.SetProxyMaxAge(TimeSpan.Zero);
        }
    }
}