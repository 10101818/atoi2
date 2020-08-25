using BusinessObjects.DataAccess;
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
    /// SupplierController
    /// </summary>
    public class SupplierController : BaseController
    {
        private SupplierDao supplierDao = new SupplierDao();
        private UserDao userDao = new UserDao();

        private SupplierManager supplierManager = new SupplierManager();

        /// <summary>
        /// 通过供应商编号获取供应商信息
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>供应商信息</returns>
        public JsonResult GetSupplierById(int userID, string sessionID, int id)
        {
            ServiceResultModel<SupplierInfo> response = new ServiceResultModel<SupplierInfo>();

            try
            {
                if (CheckSessionID(userID, sessionID, response) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                SupplierInfo info = this.supplierManager.GetSupplier(id);
                if (info == null) response.SetFailed(ResultCodes.ParameterError, "供应商编号不正确");
                else response.Data = info;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存供应商信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="info">供应商信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>保存供应商信息返回编码</returns>
        [HttpPost]
        public JsonResult SaveSupplier(int userID, string sessionID, SupplierInfo info)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (CheckSessionID(userID, sessionID, response) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false)
                    return MyJson(response, JsonRequestBehavior.AllowGet);
                if (this.supplierDao.CheckSupplierName(info.ID, info.Name))
                {
                    response.SetFailed(ResultCodes.ParameterError, "供应商名称重复");
                    return MyJson(response, JsonRequestBehavior.AllowGet);
                }
                user = this.userDao.GetUser(userID);
                this.supplierManager.SaveSupplier(info, null, user);
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