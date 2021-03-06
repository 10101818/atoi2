﻿using MedicalEquipmentHostingSystem.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MedicalEquipmentHostingSystem
{
    /// <summary>
    /// 备份全局路径下的路由
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = ConstDefinition.HOME_CONTROLLER, action = ConstDefinition.HOME_ACTION, id = UrlParameter.Optional },
                namespaces: new string[] { "MedicalEquipmentHostingSystem.Controllers" }
            );
        }
    }
}