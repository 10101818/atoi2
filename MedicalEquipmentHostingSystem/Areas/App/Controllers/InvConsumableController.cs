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
    /// InvConsumableController
    /// </summary>
    public class InvConsumableController : BaseController
    {
        private InvConsumableDao invConsumableDao = new InvConsumableDao();
        private ConsumableDao consumableDao = new ConsumableDao();

        private InvConsumableManager invConsumableManager = new InvConsumableManager();

        /// <summary>
        /// 获取耗材库列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="fujiClass2ID">富士II类ID</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>耗材库列表信息</returns>
        public JsonResult QueryConsumableList(int userID, string sessionID,int fujiClass2ID = 0, string filterField = "", string filterText = "", string sortField = "ic.ID", bool sortDirection = false, int curRowNum = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ServiceResultModel<List<InvConsumableInfo>> result = new ServiceResultModel<List<InvConsumableInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    result.Data = this.invConsumableDao.QueryConsumables(fujiClass2ID, filterField, filterText, sortField, sortDirection, curRowNum, pageSize);
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
        /// 根据耗材id获取耗材信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="consumableID">耗材id</param>
        /// <returns>耗材信息</returns>
        public JsonResult GetConsumableByID(int userID, string sessionID, int consumableID)
        {
            ServiceResultModel<InvConsumableInfo> result = new ServiceResultModel<InvConsumableInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.invConsumableDao.GetConsumableByID(consumableID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据富士II类获取耗材信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>耗材信息</returns>
        public JsonResult QueryConsumablesByFujiClass2ID(int userID, string sessionID, int fujiClass2ID, bool isIncluded = false, string filterField = "", string filterText = "", int curRowNum = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ServiceResultModel<List<ConsumableInfo>> result = new ServiceResultModel<List<ConsumableInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.consumableDao.QueryConsumablesByFujiClass2ID(fujiClass2ID, isIncluded, filterField, filterText, curRowNum, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">耗材信息</param>
        /// <returns>耗材id</returns>
        [HttpPost]
        public JsonResult SaveConsumable(int userID, string sessionID, InvConsumableInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (this.invConsumableDao.CheckConsumableLotNum(info.ID, info.LotNum, info.Consumable.ID))
                {
                    result.SetFailed(ResultCodes.ParameterError, "批次号已存在");
                    return MyJson(result, JsonRequestBehavior.AllowGet);
                }

                result.Data = this.invConsumableManager.SaveConsumable(info, user);
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