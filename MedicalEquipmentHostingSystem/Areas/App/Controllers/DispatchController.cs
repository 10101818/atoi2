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
    /// 派工单controller
    /// </summary>
    public class DispatchController : BaseController
    {
        private DispatchDao dispatchDao= new DispatchDao();
        private DispatchManager dispatchManager = new DispatchManager();
        private UserManager userManager = new UserManager();

        /// <summary>
        /// 获取派工单列表信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="statusIDs">派工单状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>、
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工单信息</returns>
        public JsonResult GetDispatchs(int userID, string sessionID, List<int> statusIDs, int urgency = 0, int type = 0, string filterField = "", string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<DispatchInfo>> result = new ServiceResultModel<List<DispatchInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                else result.Data = this.dispatchManager.QueryDispatches(user.ID, user.Role.ID, statusIDs, urgency, type, filterField, filterText, "init", false, curRowNum, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 通过派工单编号获取派工单信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工单信息</returns>
        public JsonResult GetDispatchByID(int userID, string sessionID, int dispatchID)
        {
            ServiceResultModel<DispatchInfo> response = new ServiceResultModel<DispatchInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                
                DispatchInfo info = this.dispatchManager.GetDispatchByID(dispatchID);

                if (info == null) response.SetFailed(ResultCodes.ParameterError, "派工单不存在");
                else response.Data = info.Copy4App();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过请求编号获取最新派工单信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工单信息</returns>
        public JsonResult GetDispatchByRequestID(int userID, string sessionID, int id)
        {
            ServiceResultModel<DispatchInfo> response = new ServiceResultModel<DispatchInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                DispatchInfo dispatchInfo = this.dispatchDao.GetDispatchByRequestID(id);
                if (dispatchInfo != null)
                {
                    UserInfo userInfo = userManager.GetUser(dispatchInfo.Engineer.ID);
                    dispatchInfo.Engineer.Name = userInfo.Name;
                    response.Data = dispatchInfo;
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过请求编号获取所有派工单信息
        /// </summary>
        /// <param name="id">请求编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工单信息</returns>
        public JsonResult GetDispatchesByRequestID(int userID, string sessionID, int id)
        {
            ServiceResultModel<List<DispatchInfo>> response = new ServiceResultModel<List<DispatchInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                List<DispatchInfo> dispatchInfos = this.dispatchDao.GetDispatchesByRequestID(id);
                if (dispatchInfos.Count > 0)
                {
                    foreach(DispatchInfo dispatchInfo in dispatchInfos)
                    {
                        UserInfo userInfo = userManager.GetUser(dispatchInfo.Engineer.ID);
                        dispatchInfo.Engineer.Name = userInfo.Name;
                    }
                }
                response.Data = dispatchInfos;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 开始作业
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>开始作业返回编码</returns>
        [HttpPost]
        public JsonResult StartDispatch(int userID, string sessionID, int dispatchID)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                
                List<DispatchInfo> infos = new List<DispatchInfo>();
                DispatchInfo dispatchInfo = new DispatchInfo();

                dispatchInfo = this.dispatchManager.GetDispatchByID(dispatchID);
                if (dispatchInfo == null)
                    response.SetFailed(ResultCodes.ParameterError, "派工单信息不存在");
                else if (dispatchInfo.Status.ID != BusinessObjects.Domain.DispatchInfo.Statuses.New)
                    response.SetFailed(ResultCodes.ParameterError, "派工单已响应");
                else if (userID != dispatchInfo.Engineer.ID)
                    response.SetFailed(ResultCodes.ParameterError, "不可操作他人派工单");
                else
                {
                    
                    this.dispatchManager.ResponseDispatch(dispatchID, dispatchInfo.Request.ID, user);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消派工单
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchID">派工单编号</param>
        /// <param name="requestID">请求编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>取消派工单返回编码</returns>
        [HttpPost]
        public JsonResult EndDispatch(int userID, string sessionID, int dispatchID, int requestID)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchInfo dispatch = this.dispatchManager.GetDispatchByID(dispatchID);

                if (dispatch == null)
                    response.SetFailed(ResultCodes.ParameterError, "派工单不存在");
                else if (dispatch.Status.ID == BusinessObjects.Domain.DispatchInfo.Statuses.Cancelled || dispatch.Status.ID == BusinessObjects.Domain.DispatchInfo.Statuses.Approved)
                    response.SetFailed(ResultCodes.ParameterError, "当前派工状态下不可终止");
                else if (dispatch.RequestID != requestID)
                    response.SetFailed(ResultCodes.ParameterError, "请求单号与派工单号不匹配");
                else this.dispatchManager.CancelDispatch(dispatchID, requestID, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }


        #region "DashBoard"
        /// <summary>
        /// 获取派工单列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="status">派工单状态</param>
        /// <param name="urgency">派工单紧急程度</param>
        /// <param name="type">派工类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>派工单列表信息</returns>
        public JsonResult QueryDispatches(int userID, string sessionID, int status, int urgency, int type, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = 0)
        {
            ServiceResultModel<List<DispatchInfo>> result = new ServiceResultModel<List<DispatchInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = userManager.GetUser(userID);
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                result.Data = dispatchManager.QueryDispatches(userID, user.Role.ID, status, urgency, type, filterField, filterText, sortField, sortDirection, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
        #endregion 
	}
}