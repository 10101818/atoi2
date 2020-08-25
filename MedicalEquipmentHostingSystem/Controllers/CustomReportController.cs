using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
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
    /// 自定义报表controller
    /// </summary>
    public class CustomReportController : BaseController
    {
        private CustomReportManager customRptManager = new CustomReportManager();
        private CustomReportDao customRptDao = new CustomReportDao();

        /// <summary>
        /// 自定义报表列表页面
        /// </summary>
        /// <returns>自定义报表列表页面</returns>
        public ActionResult CustomReport()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        
        /// <summary>
        /// 获取自定义报表列表
        /// </summary>
        /// <param name="typeID">自定义报表类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>自定义报表列表信息</returns>
        public JsonResult QueryCustReptList(int typeID, string filterField, string filterText, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<CustomReportInfo>> result = new ResultModel<List<CustomReportInfo>>();

            try{
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);

                List<CustomReportInfo> infos = new List<CustomReportInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.customRptDao.QueryCustRptCount(typeID,filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    infos = this.customRptDao.QueryCustRptsList(typeID, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);

                }
                else
                {
                    infos = this.customRptDao.QueryCustRptsList(typeID, filterField, filterText, 0,0);
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
        /// 根据自定义报表编号获取报表内容
        /// </summary>
        /// <param name="id">自定义报表编号</param>
        /// <returns>自定义报表内容</returns>
        public JsonResult GetReportByID(int id)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<CustomReportInfo> result = new ResultModel<CustomReportInfo>();

            try
            {
                result.Data = this.customRptDao.GetReportByID(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 保存自定义报表
        /// </summary>
        /// <param name="info">自定义报表内容</param>
        /// <returns>自定义报表信息</returns>
        [HttpPost]
        public JsonResult SaveCustRept(CustomReportInfo info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<Int32> result = new ResultModel<Int32>();
            try
            {
                result.Data = this.customRptManager.SaveCusRpt(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 获取报表可选字段列表
        /// </summary>
        /// <param name="typeID">自定义报表类型</param>
        /// <returns>可选字段列表</returns>
        public JsonResult GetCustRptTemplateTableList(int typeID)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<CustRptTemplateTableInfo>> result = new ResultModel<List<CustRptTemplateTableInfo>>();
            try
            {
                List<CustRptTemplateTableInfo> tables = this.customRptDao.GetCustRptTemplateTables(typeID);
                List<CustRptTemplateFieldInfo> fields = this.customRptDao.GetCustRptTemplateFields(typeID);

                foreach(CustRptTemplateTableInfo table in tables)
                {
                    table.Fields = (from CustRptTemplateFieldInfo temp in fields where temp.TableName.Equals(table.TableName) select temp).ToList();
                }

                result.Data = tables;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 删除自定义报表
        /// </summary>
        /// <param name="id">自定义报表编号</param>
        /// <returns>删除自定义报表返回编码</returns>
        [HttpPost]
        public JsonResult DeleteCustRpt(int id)
        {
            if (CheckSession(false) == false)
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
                this.customRptDao.DeleteCustomRptFields(id);
                this.customRptDao.DeleteCusRpt(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        /// <summary>
        /// 运行自定义报表
        /// </summary>
        /// <param name="info">自定义报表内容</param>
        /// <param name="field">筛选时间字段</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <returns>自定义报表信息excel</returns>
        [HttpPost]
        public ActionResult RunCustRpt(CustomReportInfo info,string field,DateTime startTime,DateTime endTime)
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
                endTime = endTime.AddDays(1);
                DataTable infoList = new DataTable();
                if(info.Type.ID == CustomReportInfo.CustRptType.Equipment)
                    infoList = this.customRptManager.QueryCustRpt4Equipment(info, field, startTime, endTime);
                if (info.Type.ID == CustomReportInfo.CustRptType.Contract)
                    infoList = this.customRptManager.QueryCustRpt4Contract(info, field, startTime, endTime);
                if (info.Type.ID == CustomReportInfo.CustRptType.Request)
                    infoList = this.customRptManager.QueryCustRpt4Request(info, field, startTime, endTime);
                if (info.Type.ID == CustomReportInfo.CustRptType.Dispatch)
                    infoList = this.customRptManager.QueryCustRpt4Dispatch(info, field, startTime, endTime);

                MemoryStream ms = ExportUtil.ToExcel(infoList);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", info.Name + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 下载自定义报表列表
        /// </summary>
        /// <param name="typeID">自定义报表类型</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>自定义报表列表excel</returns>
        [HttpPost]
        public ActionResult ExportCustRpt(int typeID, string filterField, string filterText)
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
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<CustomReportInfo> CustRpts = this.customRptDao.QueryCustRptsList(typeID, filterField, filterText, 0, 0);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("编号");
                dt.Columns.Add("类别");
                dt.Columns.Add("名称");
                dt.Columns.Add("创建人");
                dt.Columns.Add("创建日期");
                dt.Columns.Add("更改日期");
                dt.Columns.Add("上次运行日期");
                foreach (CustomReportInfo custRpt in CustRpts)
                {
                    dt.Rows.Add(custRpt.OID, custRpt.Type.Name, custRpt.Name, custRpt.CreateUser.Name, custRpt.CreatedDate.ToString("yyyy-MM-dd"),
                        custRpt.UpdateDate == DateTime.MinValue ? "" : custRpt.UpdateDate.ToString("yyyy-MM-dd"), custRpt.LastRunDate == DateTime.MinValue ? "" : custRpt.LastRunDate.ToString("yyyy-MM-dd"));
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "自定义报表列表.xlsx");
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