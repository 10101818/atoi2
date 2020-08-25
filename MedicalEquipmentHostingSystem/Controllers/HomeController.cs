using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 登录、注册controller
    /// </summary>
    public class HomeController : BaseController
    {
        private UserManager userManager = new UserManager();

        #region "Login"
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns>登录页面</returns>
        public ActionResult Login()
        {
            Session.Clear();

            return View();
        }
        /// <summary>
        /// dashboard的title
        /// </summary>
        /// <returns>dashboard的title</returns>
        public ActionResult Home()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginID">登录用户名</param>
        /// <param name="loginPwd">密码</param>
        /// <returns>用户信息</returns>
        [HttpPost]
        public JsonResult Login(string loginID, string loginPwd)
        {
            ResultModel<UserInfo> result = new ResultModel<UserInfo>();

            try
            {
                UserInfo existingInfo = this.userManager.GetUserByLoginID(loginID);
                if (existingInfo == null)
                    result.SetFailed(ResultCodes.BusinessError, "用户尚未注册");
                else if (existingInfo.LoginPwd != SQLUtil.TrimNull(loginPwd))
                    result.SetFailed(ResultCodes.BusinessError, "用户名或密码不正确");
                else if (existingInfo.IsActive == false)
                    result.SetFailed(ResultCodes.BusinessError, "用户已停用");
                else if (existingInfo.VerifyStatus.ID != BusinessObjects.Domain.VerifyStatus.Pass)
                    result.SetFailed(ResultCodes.BusinessError, "用户尚未通过审批");
                else if (existingInfo.Role.ID == UserRole.User)
                    result.SetFailed(ResultCodes.BusinessError, "用户权限不足");
                else
                {
                    existingInfo.WebSessionID = Guid.NewGuid().ToString();
                    existingInfo.LastLoginDate = DateTime.Now;
                    this.userManager.UpdateLoginDate(existingInfo.ID, existingInfo.WebSessionID);
                    SaveSession(existingInfo);
                    result.Data = existingInfo;
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="loginID">用户名</param>
        /// <returns>忘记密码返回信息</returns>
        [HttpPost]
        public JsonResult ForgetPassword(string loginID)
        {
            ResultModelBase result = new ResultModelBase();

            try
            {
                UserInfo existingInfo = this.userManager.GetUserByLoginID(loginID);

                if (existingInfo == null)
                    result.SetFailed(ResultCodes.BusinessError, "用户尚未注册");
                else if (string.IsNullOrEmpty(existingInfo.Email))
                    result.SetFailed(ResultCodes.BusinessError, "无邮箱账号，请联系管理员");
                else if (existingInfo.IsActive == false)
                    result.SetFailed(ResultCodes.BusinessError, "用户已停用");
                else if (existingInfo.VerifyStatus.ID != BusinessObjects.Domain.VerifyStatus.Pass)
                    result.SetFailed(ResultCodes.BusinessError, "用户尚未通过审批");
                else
                {
                    SendNoticeEmail(existingInfo);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region "Clear Session"
        /// <summary>
        /// 清除附件缓存
        /// </summary>
        /// <returns>清除附件缓存返回信息</returns>
        public JsonResult ClearSession()
        {
            CheckSession();

            return JsonResult(new ResultModelBase());
        }
        #endregion

        #region "SendNoticeEmail"
        /// <summary>
        /// 发送邮件用户密码
        /// </summary>
        /// <param name="info">用户信息</param>
        private void SendNoticeEmail(UserInfo info)
        {
            try
            {
                string mailReceipt = null, mailSubject = null;
                StringBuilder sbMailContent = new StringBuilder();

                mailSubject = Constants.SYSTEM_NAME + " - 忘记密码";

                sbMailContent.AppendLine(string.Format("{0},您好<br/>", info.Name));
                sbMailContent.AppendLine(string.Format("您的密码为{0}<br/>", info.LoginPwd));
                mailReceipt = info.Email;

                EmailUtil.SendMailAsyn(mailSubject, sbMailContent.ToString(), mailReceipt);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }
        }
        #endregion
    }
}