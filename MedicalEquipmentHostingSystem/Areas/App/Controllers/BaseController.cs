using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
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
    /// BaseController
    /// </summary>
    [AllowCrossSiteJson]
    [ValidateInput(false)]
    public class BaseController : Controller
    {
        private UserDao userDao = new UserDao();

        /// <summary>
        /// JsonResult
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>JsonResult</returns>
        protected JsonResult MyJson(object data, JsonRequestBehavior behavior)
         {
             return new JsonNetResult
             {
                 Data = data,
                 JsonRequestBehavior = behavior,
             };
         }

        /// <summary>
        /// 判断用户账号是否可以正常使用
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="response">ServiceResultModelBase</param>
        /// <param name="user">用户信息</param>
        /// <param name="roleID">角色编号</param>
        /// <returns>账号是否可以正常使用</returns>
        protected bool CheckUser(int userID, ServiceResultModelBase response, out UserInfo user, int roleID = 0)
        {
            user = this.userDao.GetUser(userID); 
            if (user == null)
            {
                response.SetFailed(ResultCodes.ParameterError, "用户不存在");
                return false;
            }

            if (user.IsActive == false )
            {
                response.SetFailed(ResultCodes.ParameterError, "用户已停用");
                return false;
            }

            if (user.VerifyStatus.ID != VerifyStatus.Pass)
            {
                response.SetFailed(ResultCodes.ParameterError, "用户未通过审批");
                return false;
            }

            if (roleID != 0)
            {
                if (roleID != user.Role.ID)
                {
                    response.SetFailed(ResultCodes.ParameterError, "角色不正确");
                    return false;
                }
            } 

            return true;
        }

        /// <summary>
        /// 判断用户账号当前登录是否一致
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="sessionID">当前请求所在App设备保存的SessionID</param>
        /// <param name="response">ServiceResultModelBase</param> 
        /// <returns>账号登录位置是否一致</returns>
        protected bool CheckSessionID(int userID, string sessionID, ServiceResultModelBase response)
        {
            bool flag = true;
            if (WebConfig.GLOBAL_SESSIONID_FLAG)
            {
                string _sessionID = this.userDao.GetSessionID(userID);
                flag = string.IsNullOrEmpty(_sessionID) || _sessionID.Equals(sessionID);

                if (!flag)
                    response.SetFailed(ResultCodes.LogoutError, "用户已在其他设备登录");
            }
            return flag;
        }
        /// <summary>
        /// 获取url
        /// </summary>
        /// <returns>url</returns>
        public string GetBaseUrl()
        {
            return string.Format("{0}/{1}", Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.Unescaped).TrimEnd('/'), Request.ApplicationPath.TrimStart('/'));
        }

        /// <summary>
        /// 转换base64数据的格式
        /// </summary>
        /// <param name="base64Str">原始base64数据</param>
        /// <returns>转换后base64数据</returns>
        public string ParseBase64String(string base64Str)
        {
            if (!string.IsNullOrEmpty(base64Str) && base64Str.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                if (base64Str.IndexOf("base64,") >= 0)
                    return base64Str.Substring(base64Str.IndexOf("base64,") + ("base64,").Length).Trim();
                else
                    return string.Empty;
            }
            else
            {
                return base64Str;
            }
        }
    }
}