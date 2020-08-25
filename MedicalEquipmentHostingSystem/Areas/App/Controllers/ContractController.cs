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
    /// 合同controller
    /// </summary>
    public class ContractController : BaseController 
    {
        private ContractDao contractDao = new ContractDao();
        private ContractManager contractManager = new ContractManager();

        /// <summary>
        /// 获取合同列表
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索条件</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">页面信息条数</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>合同信息</returns>
        public JsonResult GetContracts(int userID,string sessionID,int status = 0, string filterField = "c.ID" , string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<ContractInfo>> response = new ServiceResultModel<List<ContractInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<ContractInfo> infos = this.contractManager.QueryContracts(status, filterField, filterText, "c.ID", true, curRowNum, pageSize);

                response.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过合同编号获取合同信息
        /// </summary>
        /// <param name="id">合同编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>合同信息</returns>
        public JsonResult GetContractById(int userID, string sessionID, int id)
        {
            ServiceResultModel<ContractInfo> response = new ServiceResultModel<ContractInfo>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                ContractInfo info = this.contractManager.GetContract(id);
                if (info == null) response.SetFailed(ResultCodes.ParameterError, "合同编号不正确");
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
        /// 保存合同信息
        /// </summary>
        /// <param name="userID">操作的用户编号</param>
        /// <param name="info">合同信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>保存合同信息JsonResult</returns>
        [HttpPost]
        public JsonResult SaveContract(int userID, string sessionID, ContractInfo info)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (CheckSessionID(userID, sessionID, response) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                this.contractManager.SaveContract(info, null, user);
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