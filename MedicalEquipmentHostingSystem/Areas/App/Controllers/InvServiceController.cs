using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// InvServiceController
    /// </summary>
    public class InvServiceController : BaseController
    {
        private InvServiceDao invServiceDao = new InvServiceDao();

        private InvServiceManager invServiceManager = new InvServiceManager();

        /// <summary>
        /// 获取服务库列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="statusID">服务状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>服务库列表信息</returns>
        public JsonResult QueryServiceList(int userID, string sessionID, int statusID = 0, string filterField = "", string filterText = "", string sortField = "se.ID", bool sortDirection = false, int curRowNum = 0, int pageSize = 0, int fujiClass2ID = 0)
        {
            ServiceResultModel<List<InvServiceInfo>> result = new ServiceResultModel<List<InvServiceInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                    result.Data = this.invServiceDao.QueryServices(statusID, filterField, filterText, sortField, sortDirection, curRowNum, pageSize, fujiClass2ID);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据服务id获取服务信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="serviceID">服务id</param>
        /// <returns>服务信息</returns>
        public JsonResult GetServiceByID(int userID, string sessionID, int serviceID)
        {
            ServiceResultModel<InvServiceInfo> result = new ServiceResultModel<InvServiceInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.invServiceDao.GetServiceByID(serviceID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存服务信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">服务信息</param>
        /// <returns>服务id</returns>
        [HttpPost]
        public JsonResult SaveService(int userID, string sessionID, InvServiceInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.invServiceManager.SaveService(info, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
    }
}