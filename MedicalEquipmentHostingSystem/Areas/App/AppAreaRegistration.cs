using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// App
/// </summary>
namespace MedicalEquipmentHostingSystem.Areas.App
{
    /// <summary>
    /// 注册Area路由
    /// </summary>
    public class AppAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// 重写AreaName方法
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "App";
            }
        }

        /// <summary>
        /// 注册Area路由
        /// </summary>
        /// <param name="context">命名空间</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "App_default",
                "App/{controller}/{action}/{id}",
                new {Controller = "User", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}