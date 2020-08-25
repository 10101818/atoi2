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
    /// InvSpareController
    /// </summary>
    public class InvSpareController : BaseController
    {
        private InvSpareDao invSpareDao = new InvSpareDao();

        private InvSpareManager invSpareManager = new InvSpareManager();

        /// <summary>
        /// 获取备用机库列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="statusID">备用机状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>备用机库列表信息</returns>
        public JsonResult QuerySpareList(int userID, string sessionID, int statusID = 0, string filterField = "", string filterText = "", string sortField = "sp.ID", bool sortDirection = false, int curRowNum = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ServiceResultModel<List<InvSpareInfo>> result = new ServiceResultModel<List<InvSpareInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);
                else
                {
                    BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                    result.Data = this.invSpareDao.QuerySpares(statusID, filterField, filterText, sortField, sortDirection, curRowNum, pageSize);
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
        /// 根据备用机id获取备用机信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="spareID">备用机id</param>
        /// <returns>备用机信息</returns>
        public JsonResult GetSpareByID(int userID, string sessionID, int spareID)
        {
            ServiceResultModel<InvSpareInfo> result = new ServiceResultModel<InvSpareInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.invSpareDao.GetSpareByID(spareID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存备用机信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="info">备用机信息</param>
        /// <returns>备用机id</returns>
        [HttpPost]
        public JsonResult SaveSpare(int userID, string sessionID, InvSpareInfo info)
        {
            ServiceResultModel<int> result = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                if (this.invSpareDao.CheckSpareSerialCode(info.ID, info.FujiClass2.ID, info.SerialCode, info.StartDate))
                {
                    result.SetFailed(ResultCodes.ParameterError, "序列号已存在");
                    return MyJson(result, JsonRequestBehavior.AllowGet);
                }

                result.Data = this.invSpareManager.SaveSpare(info, user);
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