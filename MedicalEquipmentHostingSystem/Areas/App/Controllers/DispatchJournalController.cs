using BusinessObjects.Domain;
using BusinessObjects.Manager;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// DispatchJournalController
    /// </summary>
    public class DispatchJournalController : BaseController
    {
        private DispatchManager dispatchManager = new DispatchManager();
        private UserManager userManager = new UserManager();

        /// <summary>
        /// 通过服务凭证编号获取服务凭证信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务凭证信息</returns>
        public JsonResult GetDispatchJournal(int userID, string sessionID, int dispatchJournalID)
        {
            ServiceResultModel<DispatchJournalInfo> response = new ServiceResultModel<DispatchJournalInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchJournalInfo info = this.dispatchManager.GetDispatchJournalByID(dispatchJournalID);

                if (info == null) response.SetFailed(ResultCodes.ParameterError, "服务凭证不存在");
                else
                {
                    response.Data = info;
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
        /// 审批通过服务凭证
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="resultStatusID">服务凭证结果</param>
        /// <param name="followProblem">待跟进问题</param>
        /// <param name="comments">审批备注</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>审批通过服务凭证返回编码</returns>
        [HttpPost]
        public JsonResult ApproveDispatchJournal(int userID, string sessionID, int dispatchJournalID, int resultStatusID, string followProblem, string comments)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchJournalInfo info = this.dispatchManager.GetDispatchJournalByID(dispatchJournalID);

                if (info == null)
                    response.SetFailed(ResultCodes.ParameterError, "服务凭证不存在");
                else if (info.Status.ID != BusinessObjects.Domain.DispatchInfo.DocStatus.Pending)
                    response.SetFailed(ResultCodes.ParameterError, "当前服务凭证状态不可审批");
                else if (resultStatusID < DispatchJournalInfo.ResultStatuses.Pending || resultStatusID > DispatchJournalInfo.ResultStatuses.Close)
                    response.SetFailed(ResultCodes.ParameterError, "服务结果不存在");
                else
                    this.dispatchManager.PassDispatchJournal(dispatchJournalID, info.Dispatch.ID, resultStatusID, user,followProblem, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审批拒绝服务凭证
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchJournalID">服务凭证编号</param>
        /// <param name="resultStatusID">服务凭证结果</param>
        /// <param name="followProblem">待跟进问题</param>
        /// <param name="comments">审批备注</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>审批拒绝服务凭证返回编码</returns>
        [HttpPost]
        public JsonResult RejectDispatchJournal(int userID, string sessionID, int dispatchJournalID, int resultStatusID, string followProblem = "", string comments = "")
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchJournalInfo info = this.dispatchManager.GetDispatchJournalByID(dispatchJournalID);

                if (info == null)
                    response.SetFailed(ResultCodes.ParameterError, "服务凭证不存在");
                else if (info.Status.ID != BusinessObjects.Domain.DispatchInfo.DocStatus.Pending)
                    response.SetFailed(ResultCodes.ParameterError, "当前服务凭证状态不可审批");
                else if (string.IsNullOrEmpty(comments))
                    response.SetFailed(ResultCodes.ParameterError, "备注不能为空");
                else
                    this.dispatchManager.RejectDispatchJournal(dispatchJournalID, info.Dispatch.ID, resultStatusID, user, followProblem, comments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存服务凭证信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchJournalInfo">服务凭证信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务凭证编号</returns>
        [HttpPost]
        public JsonResult SaveDispatchJournal(int userID, string sessionID, DispatchJournalInfo dispatchJournalInfo)
        {
            ServiceResultModel<int> response = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.Admin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchInfo dispatchInfo = this.dispatchManager.GetDispatchByID(dispatchJournalInfo.Dispatch.ID);

                if (dispatchInfo == null)
                    response.SetFailed(ResultCodes.ParameterError, "派工单信息不存在");
                else if (userID != dispatchInfo.Engineer.ID)
                    response.SetFailed(ResultCodes.ParameterError, "不可操作他人派工单");
                else if (dispatchInfo.Status.ID != BusinessObjects.Domain.DispatchInfo.Statuses.Responded && dispatchInfo.Status.ID != BusinessObjects.Domain.DispatchInfo.Statuses.Pending)
                    response.SetFailed(ResultCodes.ParameterError, "派工单当前状态不可进行该操作");
                else if(dispatchInfo.DispatchJournal.ID != dispatchJournalInfo.ID)
                    response.SetFailed(ResultCodes.ParameterError,"派工单已提交过服务凭证");
                else if (dispatchInfo.DispatchJournal.Status.ID != DispatchJournalInfo.DispatchJournalStatus.New && dispatchInfo.DispatchJournal.Status.ID != 0)
                    response.SetFailed(ResultCodes.ParameterError, "当前服务凭证状态非新建");
                else if (dispatchJournalInfo.ResultStatus.ID == 0 || dispatchJournalInfo.ResultStatus.ID > BusinessObjects.Manager.LookupManager.GetDispatchJournalResultStatus().Count)
                    response.SetFailed(ResultCodes.ParameterError, "请选择范围内的服务结果");
                else if (dispatchJournalInfo.FileContent == null)
                    response.SetFailed(ResultCodes.ParameterError, "签名文件不能为空");
                else
                {
                    dispatchJournalInfo.Status.ID = BusinessObjects.Domain.DispatchInfo.DocStatus.Pending;
                    dispatchJournalInfo.FileContent = ParseBase64String(dispatchJournalInfo.FileContent);
                    dispatchJournalInfo = this.dispatchManager.SaveDispatchJournal(dispatchJournalInfo, user);
                    response.Data = dispatchJournalInfo.ID;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);   
        }
	}
}