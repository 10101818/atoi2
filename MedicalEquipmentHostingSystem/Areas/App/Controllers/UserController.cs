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
    /// UserController
    /// </summary>
    public class UserController : BaseController
    {
        private UserManager userManager = new UserManager();

        private RequestDao requestDao = new RequestDao();
        private DispatchDao dispatchDao = new DispatchDao();
        private UserDao userDao = new UserDao();
        private DepartmentDao departmentDao = new DepartmentDao();

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="info">用户信息</param> 
        /// <returns>用户信息</returns>
        [HttpPost]
        public JsonResult Login(UserInfo info)
        {
            ServiceResultModel<UserInfo> response = new ServiceResultModel<UserInfo>();
            try
            {
                string sessionID = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(info.LoginID))
                {
                    response.SetFailed(ResultCodes.ParameterError, "用户名/手机号不能为空");//您输入的LoginID参数不正确
                }
                else if (string.IsNullOrEmpty(info.LoginPwd))
                {
                    response.SetFailed(ResultCodes.ParameterError, "密码不能为空");//您输入的LoginPwd参数不正确
                }
                else
                {
                    UserInfo userInfo = this.userManager.GetUserByLoginID(info.LoginID);
                    if (userInfo == null)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "您输入的用户名/手机号尚未注册");//您输入的手机号尚未注册
                    }
                    else if (userInfo.IsActive == false)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "用户已停用");//用户已停用
                    }
                    else if (userInfo.LoginPwd != info.LoginPwd)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "您输入的密码不正确");//您输入的密码不正确
                    }
                    else if (userInfo.VerifyStatus.ID != BusinessObjects.Domain.VerifyStatus.Pass)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "用户尚未通过审批");//用户尚未通过审批
                    }else
                    {
                        info.ID = userInfo.ID;
                        userInfo.SessionID = sessionID;
                        info.SessionID = sessionID; 
                        this.userManager.Login4App(info);
                        userInfo.RegistrationID = info.RegistrationID;
                        userInfo.OSName = info.OSName;

                        response.Data = userInfo;
                    }
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
        /// 用户注册
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <param name="VerificationCode">验证码</param> 
        /// <returns>用户信息</returns>
        [HttpPost]
        public JsonResult Register(UserInfo info, string VerificationCode)
        {
            ServiceResultModel<UserInfo> response = new ServiceResultModel<UserInfo>();

            try
            {
                if (string.IsNullOrEmpty(info.LoginID))
                {
                    response.SetFailed(ResultCodes.ParameterError, "手机号码不能为空");//您输入的LoginID参数不正确
                }
                else if (info.LoginID.Length != 11)
                {
                    response.SetFailed(ResultCodes.ParameterError, "请正确输入11位数手机号码");//您输入的MobilePhone参数不正确
                }
                else if (string.IsNullOrEmpty(info.LoginPwd))
                {
                    response.SetFailed(ResultCodes.ParameterError, "密码不能为空");//您输入的LoginPwd参数不正确
                }
                else if (string.IsNullOrEmpty(info.Name))
                {
                    response.SetFailed(ResultCodes.ParameterError, "用户名不能为空");//您输入的Name参数不正确
                }
                else
                {
                    UserInfo userInfo = this.userManager.GetUserByLoginID(info.LoginID);

                    VerificationCodeInfo verificationCodeInfo = this.userDao.GetLastVerificationCode(info.LoginID);
                    if (userInfo != null)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "您输入的手机号已注册");//您输入的手机号已注册
                    }
                    else if (verificationCodeInfo == null)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "请点击获取验证码");//请点击获取验证码
                    }
                    else if (SQLUtil.TrimNull(VerificationCode) != verificationCodeInfo.VerificationCode)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "验证码错误，请重新输入");//验证码错误，请重新输入
                    }
                    else if ((DateTime.Now - verificationCodeInfo.CreatedTime).TotalMinutes > WebConfig.SMS_PERIOD)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "验证码已失效，请重新获取"); //验证码已失效，请重新获取
                    }
                    else
                    {
                        info = userManager.RegisterUser(info, verificationCodeInfo.ID);

                        response.Data = info;
                    }
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
        /// 修改用户信息
        /// </summary>
        /// <param name="info">用户信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>用户信息</returns>
        [HttpPost]
        public JsonResult UpdateUserInfo(UserInfo info,string sessionID)
        {
            ServiceResultModel<UserInfo> response = new ServiceResultModel<UserInfo>();

            UserInfo user = new UserInfo();

            try
            {
                if (!CheckSessionID(info.ID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                if (CheckUser(info.ID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                else
                {
                    if (info.LoginPwd == "" || info.LoginPwd == null)
                    {
                        info.LoginPwd = EncryptionUtil.Decrypt(user.LoginPwd);
                    }
                    
                    info.IsActive = user.IsActive;
                    info.VerifyStatus = user.VerifyStatus;
                    info.Role = user.Role;
                    info.LoginID = user.LoginID;
                    info.Comments = user.Comments;

                    this.userManager.SaveUser(info);

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
        /// 获取验证码
        /// </summary>
        /// <param name="mobilePhone">手机号</param> 
        /// <returns>验证码</returns>
        public JsonResult GetVerificationCode(string mobilePhone)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();

            try
            {
                if (string.IsNullOrEmpty(mobilePhone))
                {
                    response.SetFailed(ResultCodes.ParameterError, "手机号码不能为空");//您输入的MobilePhone参数不正确
                }
                else if (mobilePhone.Length != 11)
                {
                    response.SetFailed(ResultCodes.ParameterError, "请正确输入11位数手机号码");//您输入的MobilePhone参数不正确
                }
                else
                {
                    UserInfo userInfo = userManager.GetUserByLoginID(mobilePhone);
                    VerificationCodeInfo verificationCodeInfo = userDao.GetLastVerificationCode(mobilePhone);

                    if (userInfo != null)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "您输入的手机号已注册");//您输入的手机号已注册
                    }
                    else if (verificationCodeInfo != null && (DateTime.Now - verificationCodeInfo.CreatedTime).TotalMinutes <= WebConfig.SMS_PERIOD)
                    {
                        response.SetFailed(ResultCodes.ParameterError, "短信验证码已发送，请耐心等待");//短信验证码已发送，请耐心等待
                    }
                    else
                    {
                        if (this.userManager.SendPhoneVerify(mobilePhone, WebConfig.SMS_SDK_APP_ID, WebConfig.SMS_SDK_APP_KEY, WebConfig.SMS_PERIOD, WebConfig.SMS_SIGNATURE, WebConfig.SMS_TEMPLATEID) == false)
                        {
                            response.SetFailed(ResultCodes.ParameterError, "发送短信验证码失败，请重试");//发送短信验证码失败，请重试
                        }
                        else
                        {
                            response.SetFailed(ResultCodes.Succeed, "短信验证码已发送，请耐心等待");//短信验证码已发送，请耐心等待
                        }
                    }
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
        /// 获取科室信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>科室信息</returns>
        public JsonResult GetDepartments(int userID, string sessionID, string filterText = "")
        {
            ServiceResultModel<List<DepartmentInfo>> response = new ServiceResultModel<List<DepartmentInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                response.Data = this.departmentDao.QueryDepartment4AutoComplete(filterText);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取工程师信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>工程师信息</returns>
        public JsonResult GetAdmins(int userID,string sessionID)
        {
            ServiceResultModel<List<UserInfo>> response = new ServiceResultModel<List<UserInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                response.Data = this.userDao.QueryUsers(1, UserRole.Admin, VerifyStatus.Pass, "", "", "u.LoginID", true, 0, 0);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取超管的请求数量
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>获取待派工、待审批、未完成的请求数量</returns>
        public JsonResult GetAdminCount(int userID, string sessionID)
        {
            ServiceResultModel<Dictionary<string, int>> response = new ServiceResultModel<Dictionary<string, int>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                Dictionary<string, int> resultData = new Dictionary<string, int>();
                List<int> dispatchStatusId = new List<int>();
                dispatchStatusId.Add(DispatchInfo.Statuses.Pending);
                List<int> unfinishedList = new List<int>();
                unfinishedList.Add(RequestInfo.Statuses.Unfinished);
                int unfinishedCount = this.requestDao.GetRequestCount4App(unfinishedList);
                int newCount = unfinishedCount;
                int dispatchCount = this.dispatchDao.GetDispatchCount4App(dispatchStatusId);

                resultData.Add("newCount", newCount);
                resultData.Add("unfinishedCount", unfinishedCount);
                resultData.Add("dispatchCount", dispatchCount);

                response.Data = resultData;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取工程师的未完成派工单数量
        /// </summary>
        /// <param name="userId">工程师编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>获取待开始工单,作业中工单的数量</returns>
        public JsonResult GetEngineerCount(int userId, string sessionID)
        {
            ServiceResultModel<Dictionary<string, int>> response = new ServiceResultModel<Dictionary<string, int>>();

            try
            {
                if (!CheckSessionID(userId, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                Dictionary<string, int> resultData = new Dictionary<string, int>();
                List<int> newDispatchStatusId = new List<int>();
                newDispatchStatusId.Add(DispatchInfo.Statuses.New);
                List<int> pendingDispatchStatusId = new List<int>();
                pendingDispatchStatusId.Add(DispatchInfo.Statuses.Responded);
                pendingDispatchStatusId.Add(DispatchInfo.Statuses.Pending);
                int pendingDispatchCount = this.dispatchDao.GetDispatchCount4App(pendingDispatchStatusId, userId);
                int newDispatchCount = this.dispatchDao.GetDispatchCount4App(newDispatchStatusId, userId);

                resultData.Add("pendingDispatchCount", pendingDispatchCount);
                resultData.Add("newdispatchCount", newDispatchCount);

                response.Data = resultData;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取可派工的工程师
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>可派工的工程师</returns>
        public JsonResult GetUsers4Dispatch(int userID, string sessionID)
        {
            ServiceResultModel<List<UserInfo>> result = new ServiceResultModel<List<UserInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.userDao.GetUsers4Dispatch();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取静态常量(因用于手机测试服务器地址,取消session检测)
        /// </summary>
        /// <returns>静态常量</returns>
        public JsonResult GetConstants()
        {
            ServiceResultModel<Dictionary<string, List<KeyValueInfo>>> result = new ServiceResultModel<Dictionary<string, List<KeyValueInfo>>>();
            try
            {
                //if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                Dictionary<string, List<KeyValueInfo>> dic = new Dictionary<string, List<KeyValueInfo>>();

                dic.Add("UserRole", LookupManager.GetRoles());
                dic.Add("AssetsLevel", EquipmentInfo.AssetLevels.GetAssetLevels());
                dic.Add("ContractScope", LookupManager.GetContractScope());
                dic.Add("SolutionStatus", LookupManager.GetSolutionResultStatus());
                dic.Add("ReportStatus", LookupManager.GetDispatchDocStatus());
                dic.Add("AccessorySourceType", LookupManager.GetAccessorySourceType());
                dic.Add("AccessoryFileType", ReportComponentInfo.FileTypes.GetFileName());
                dic.Add("RequestType", LookupManager.GetRequestTypes());
                dic.Add("RequestStatus", LookupManager.GetRequestStatus());
                dic.Add("DealType", LookupManager.GetDealType());
                dic.Add("PriorityID", LookupManager.GetUrgency());
                dic.Add("FaultRepair", LookupManager.GetFaultType());
                dic.Add("FaultMaintain", RequestInfo.MaintainType.GetMaintainType());
                dic.Add("FaultCheck", RequestInfo.InspectionType.GetInspectionType());
                dic.Add("MachineStatus", MachineStatuses.GetMachineStatuses());
                dic.Add("FaultBad", RequestInfo.AdverseEventType.GetAdverseEventType());
                dic.Add("ServiceProviders", DispatchReportInfo.ServiceProviders.GetServiceProviders());
                dic.Add("UrgencyID", LookupManager.GetUrgency());
                dic.Add("EquipmentStatus", LookupManager.GetEquipmentStatus());
                dic.Add("DispatchStatus", LookupManager.GetDispatchStatus());
                dic.Add("ResultStatusID", LookupManager.GetDispatchJournalResultStatus());
                dic.Add("JournalStatusID", LookupManager.GetDispatchDocStatus());
                dic.Add("SupplierType", LookupManager.GetSupplierType());
                dic.Add("UsageStatus", LookupManager.GetUsageStatus());
                dic.Add("PeriodType", LookupManager.GetPeriodType());
                dic.Add("ContractType", LookupManager.GetContractType());
                dic.Add("EquipmentLevel", EquipmentInfo.EquipmentLevels.GetEquipmentLevels());
                dic.Add("MandatoryTestStatus", EquipmentInfo.MandatoryTestStatuses.GetMandatoryTestStatuses());
                dic.Add("Source", RequestInfo.Sources.GetSource());
                dic.Add("ComponentStatus", LookupManager.GetComponentType());
                dic.Add("ConsumableStatus", LookupManager.GetConsumableType());
                dic.Add("InvComponentStatus", InvComponentInfo.ComponentStatus.GetComponentStatus());
                dic.Add("InvServiceStatus", InvServiceInfo.Statuses.GetStatuses());
                dic.Add("InvSpareStatus", InvSpareInfo.SpareStatus.GetSpareStatus());
                dic.Add("PurchaseOrderStatus", LookupManager.GetPurchaseOrderStatus());

                result.Data = dic;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取系统设置
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>系统设置</returns>
        public JsonResult GetSystemSetting(int userID, string sessionID)
        {
            ServiceResultModel<Dictionary<string, object>> result = new ServiceResultModel<Dictionary<string, object>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                SmtpInfo smtpInfo = ControlManager.GetSmtpInfo();
                dic.Add("AppValidVersion", smtpInfo.AppValidVersion);
                dic.Add("AutoAssetCode", WebConfig.AutoAssetCode);

                result.Data = dic;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }


        #region "DashBoard"
        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="status">用户状态</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="verifyStatusID">审批状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>用户列表信息</returns>
        public JsonResult QueryUserList(int userID, string sessionID, int status, int roleId, int verifyStatusID, string filterField, string filterText, string field, bool direction, int currentPage = 0, int pageSize = 0)
        {
            ServiceResultModel<List<UserInfo>> result = new ServiceResultModel<List<UserInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<UserInfo> infos = this.userDao.QueryUsers(status, roleId, verifyStatusID, filterField, filterText, field, direction, currentPage, pageSize); 
                result.Data = infos;
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