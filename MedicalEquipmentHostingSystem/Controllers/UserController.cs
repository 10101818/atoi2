using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.DataAccess;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 用户controller
    /// </summary>
    public class UserController : BaseController
    {
        private UserManager userManager = new UserManager();
        private UserDao userDao = new UserDao();
       
        /// <summary>
        /// 用户列表页面
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <returns>用户列表页面</returns>
        public ActionResult UserList(int status=0)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.Status = status;
            return View();
        }

        /// <summary>
        /// 获取用户列表信息
        /// </summary>
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
        public JsonResult QueryUserList(int status, int roleId, int verifyStatusID, string filterField, string filterText, string field, bool direction, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<UserInfo>> result = new ResultModel<List<UserInfo>>();
            try
            {
                List<UserInfo> infos = new List<UserInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.userDao.QueryUsersCount(status, roleId, verifyStatusID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.userDao.QueryUsers(status, roleId, verifyStatusID, filterField, filterText, field, direction, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.userDao.QueryUsers(status, roleId, verifyStatusID, filterField, filterText, field, direction, 0, 0);
                }

                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 通过用户编号获取用户信息
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <returns>用户信息</returns>
        public JsonResult GetUser(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<UserInfo> result = new ResultModel<UserInfo>();
            try
            {
                UserInfo editUserInfo = this.userManager.GetUser(id);
                result.Data = editUserInfo;
            }
            catch(Exception ex){
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }


        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>保存用户信息返回信息</returns>
        [HttpPost]
        public JsonResult SaveUser(UserInfo info)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

           ResultModelBase result = new ResultModelBase();

            try
            {
                info.ID = this.userManager.SaveUser(info);
            }
            catch(Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        
        /// <summary>
        /// 判断用户名是否存在
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="loginID">用户名</param>
        /// <returns>用户名是否存在</returns>
        public JsonResult CheckUserLoginID(int id, String loginID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                result.Data = this.userDao.CheckUserLoginID(id, loginID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
            
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="loginPwd">新密码</param>
        /// <returns>修改密码返回信息</returns>
        [HttpPost]
        public JsonResult UpdatePassword(string loginPwd)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModelBase result = new ResultModelBase();
            try
            {
                int ID = GetLoginUser().ID;
                string pwd = this.userManager.GetUser(ID).LoginPwd;
                if (loginPwd == pwd)
                {
                    result.SetFailed(ResultCodes.ParameterError, "与原密码相同，请重新输入");
                    return JsonResult(result);
                }
                this.userManager.UpdatePassword(ID, loginPwd);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出用户List
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <param name="roleId">角色编号</param>
        /// <param name="verifyStatusID">审批状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="field">排序字段</param>
        /// <param name="direction">排序方式</param>
        /// <returns>用户列表excel</returns>
        [HttpPost]
        public ActionResult ExportUsersList(int status, int roleId, int verifyStatusID, string filterField, string filterText, string field, bool direction)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<UserInfo>> result = new ResultModel<List<UserInfo>>();
            try
            {
                List<UserInfo> infos = null;
                infos = this.userDao.QueryUsers(status, roleId, verifyStatusID, filterField, filterText, field, direction, 0, 0);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("用户名");
                dt.Columns.Add("角色");
                dt.Columns.Add("姓名");
                dt.Columns.Add("电话");
                dt.Columns.Add("邮箱");
                dt.Columns.Add("地址");
                dt.Columns.Add("添加日期");
                dt.Columns.Add("状态");
                dt.Columns.Add("审批状态");


                foreach (UserInfo userInfo in infos)
                {
                    dt.Rows.Add(userInfo.LoginID, userInfo.Role.Name, userInfo.Name,
                         userInfo.Mobile, userInfo.Email, userInfo.Address, userInfo.CreatedDate.ToShortDateString(), (userInfo.IsActive ? "启用" : "停用"), userInfo.VerifyStatus.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "用户列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取工程师
        /// </summary>
        /// <returns>管理员信息</returns>
        public JsonResult GetUsers4Dispatch()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<UserInfo>> result = new ResultModel<List<UserInfo>>();
            try
            {
                result.Data = this.userDao.GetUsers4Dispatch();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
    }
}