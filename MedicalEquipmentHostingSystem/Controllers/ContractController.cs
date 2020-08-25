using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// ContractController
    /// </summary>
    public class ContractController : BaseController
    {
        private ContractManager contractManager = new ContractManager();
        private ContractDao contractDao = new ContractDao();
        private UploadFileManager fileManager = new UploadFileManager();

        /// <summary>
        /// 合同列表页面
        /// </summary>
        /// <param name="equipmentId">设备编号</param>
        /// <returns>合同列表页面</returns>
        public ActionResult ContractList(int equipmentId = 0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.EquipmentId = equipmentId;

            return View();
        }
        /// <summary>
        /// 添加/修改合同页面
        /// </summary>
        /// <param name="contractID">合同编号</param>
        /// <returns>添加/修改合同页面</returns>
        public ActionResult ContractDetail(int contractID=0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ID = contractID;
            return View();
        }
        /// <summary>
        /// 查看合同页面
        /// </summary>
        /// <param name="contractID">合同编号</param>
        /// <returns>查看合同页面</returns>
        public ActionResult ContractDetailView(int contractID = 0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ID = contractID;
            return View();
        }

        /// <summary>
        /// 获取合同列表信息
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>列表合同信息</returns>
        public JsonResult QueryContract(int status, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            ResultModel<List<ContractInfo>> result = new ResultModel<List<ContractInfo>>();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                if (currentPage > 0)
                {
                    int totalNum = this.contractDao.QueryContractsCount(status, filterField, filterText);
                    result.SetTotalPages(totalNum, pageSize);
                    result.Data = contractManager.QueryContracts(status, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    result.Data = contractManager.QueryContracts(status, filterField, filterText, sortField, sortDirection, 0, ConstDefinition.PAGE_SIZE);
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
        /// 根据合同编号获取单个合同信息
        /// </summary>
        /// <param name="id">合同编号</param>
        /// <returns>单个合同信息</returns>
        public JsonResult GetContractByID(int id)
        {
            ResultModel<ContractInfo> result = new ResultModel<ContractInfo>();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = this.contractManager.GetContract(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 获取各状态合同数量
        /// </summary>
        /// <returns>获取即将失效、失效、生效、未生效的合同数量</returns>
        public JsonResult GetContractCount()
        {
            ResultModel<Dictionary<string, int>> result = new ResultModel<Dictionary<string, int>>();
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = this.contractManager.GetContractCount();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存合同信息
        /// </summary>
        /// <param name="contract">合同信息</param>
        /// <returns>返回合同信息</returns>
        [HttpPost]
        public JsonResult SaveContract(ContractInfo contract)
        {
            ResultModel<int> result = new ResultModel<int>();
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                List<UploadFileInfo> fileContractInfos = GetUploadFilesInSession();

                contract.ID = this.contractManager.SaveContract(contract,fileContractInfos, GetLoginUser());
                result.Data = contract.ID;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 下载合同列表信息
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>合同列表信息的excel文档</returns>
        [HttpPost]
        public ActionResult ExportContracts(int status, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<ContractInfo> contracts = null;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                contracts = this.contractManager.QueryContracts(status, filterField, filterText, sortField, sortDirection);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("合同编号");
                dt.Columns.Add("设备编号");
                dt.Columns.Add("设备序列号");
                dt.Columns.Add("合同名称");
                dt.Columns.Add("合同类型");
                dt.Columns.Add("供应商");
                dt.Columns.Add("开始时间");
                dt.Columns.Add("结束时间");
                dt.Columns.Add("状态");

                foreach (ContractInfo contract in contracts)
                {
                    dt.Rows.Add(contract.OID, contract.ContractNum, contract.EquipmentOID,contract.EquipmentSerialCode, contract.Name, contract.Type.Name, contract.Supplier.Name, contract.StartDate.ToString("yyyy-MM-dd"), contract.EndDate.ToString("yyyy-MM-dd"), contract.Status);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "合同列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}