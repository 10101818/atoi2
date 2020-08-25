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
    /// InvComponentController
    /// </summary>
    public class InvComponentController : BaseController
    {
        private InvComponentDao invComponentDao = new InvComponentDao();
        private ComponentDao componentDao = new ComponentDao();

        private InvComponentManager invComponentManager = new InvComponentManager();

        /// <summary>
        /// 获取零件库列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="statusID">零件状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件库列表信息</returns>
        public JsonResult QueryComponentList(int userID, string sessionID, int statusID, string filterField, string filterText, string sortField = "ic.ID", bool sortDirection = false, int curRowNum = 0, int pageSize = 0, string eqptID = "")
        {
            ServiceResultModel<List<InvComponentInfo>> result = new ServiceResultModel<List<InvComponentInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    result.Data = this.invComponentDao.QueryComponents(statusID, filterField, filterText, sortField, sortDirection, curRowNum, pageSize, eqptID);
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
        /// 根据零件id获取零件信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="componentID">零件id</param>
        /// <returns>零件信息</returns>
        public JsonResult GetComponentByID(int userID, string sessionID, int componentID)
        {
            ServiceResultModel<InvComponentInfo> result = new ServiceResultModel<InvComponentInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.invComponentDao.GetComponentByID(componentID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据富士II类获取零件信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="componentTypeID">零件类型</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件信息</returns>
        public JsonResult QueryComponentsByFujiClass2ID(int userID, string sessionID, int fujiClass2ID, int componentTypeID = -1, bool isIncluded = false, string filterField = "", string filterText = "", int curRowNum = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ServiceResultModel<List<ComponentInfo>> result = new ServiceResultModel<List<ComponentInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.componentDao.QueryComponentsByFujiClass2ID(fujiClass2ID, componentTypeID, isIncluded, filterField, filterText, curRowNum, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">零件信息</param>
        /// <returns>零件id</returns>
        [HttpPost]
        public JsonResult SaveComponent(int userID, string sessionID, InvComponentInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (this.invComponentDao.CheckComponentSerialCode(info.ID, info.SerialCode))
                {
                    result.SetFailed(ResultCodes.ParameterError, "序列号已存在");
                    return MyJson(result, JsonRequestBehavior.AllowGet);
                }

                result.Data = this.invComponentManager.SaveComponent(info, user);
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