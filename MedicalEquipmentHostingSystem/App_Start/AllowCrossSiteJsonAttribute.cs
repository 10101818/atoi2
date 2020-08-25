using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.App_Start
{
    /// <summary>
    /// AllowCrossSiteJsonAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 重写OnActionExecuting
        /// </summary>
        /// <param name="filterContext">new instance</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*"); 

            base.OnActionExecuting(filterContext);
        }
    } 
}