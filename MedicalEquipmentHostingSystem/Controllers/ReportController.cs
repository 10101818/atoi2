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
    /// 报表controller
    /// </summary>
    public class ReportController : BaseController
    {
        private ReportDao reportDao = new ReportDao();
        private ServiceHisDao serviceHisDao = new ServiceHisDao();
        private ReportManager reportManager = new ReportManager();
        private FileDao fileDao = new FileDao();
        private UploadFileManager fileManager = new UploadFileManager();

        #region view
        /// <summary>
        /// 设备报表列表页面
        /// </summary>
        /// <returns>设备报表列表页面</returns>
        public ActionResult EquipmentReportList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 请求报表列表页面
        /// </summary>
        /// <returns>请求报表列表页面</returns>
        public ActionResult RequestReportList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }

        /// <summary>
        /// 请求数量统计页面
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <returns>请求数量统计页面</returns>
        public ActionResult RequestCountReport(int requestType, int status, string actionName, string inter, string reportName, string yName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestType = requestType;
            ViewBag.Status = status;
            ViewBag.ReportName = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            return View();
        }
        /// <summary>
        /// 请求比率页面
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="inter">接口名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <returns>请求比率页面</returns>
        public ActionResult RequestRatio(int requestType, int status, string inter, string actionName, string reportName = "", string curName = "", string lastName = "", string yName = "")
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestType = requestType;
            ViewBag.Status = status;
            ViewBag.Inter = inter;
            ViewBag.ReportName = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.YName = yName;
            return View();
        }
        /// <summary>
        /// 服务达标率页面
        /// </summary>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <returns>服务达标率页面</returns>
        public ActionResult ServiceRatio(string actionName, string inter, string reportName, string curName, string lastName, string yName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.Inter = inter;
            ViewBag.ReportName = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.YName = yName;
            return View();
        }
        /// <summary>
        /// 未完成请求数量统计页面
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <returns>未完成请求数量统计页面</returns>
        public ActionResult UnFinishedRequestCountReport(int requestType, int status, string actionName, string inter, string reportName, string yName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestType = requestType;
            ViewBag.Status = status;
            ViewBag.ReportName = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            return View();
        }
        /// <summary>
        /// 设备报表数量统计页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>设备报表数量统计页面</returns>
        public ActionResult AmountReport(string reportName, string inter, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ReportName = reportName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            ViewBag.ActionName = actionName;
            return View();
        }
        /// <summary>
        /// 设备比率页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>设备比率页面</returns>
        public ActionResult EqptRatio(string reportName, string inter, string curName, string lastName, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ReportName = reportName;
            ViewBag.Inter = inter;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.YName = yName;
            ViewBag.ActionName = actionName;
            return View();
        }
        /// <summary>
        /// 故障率/开机率报表页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="inter">接口名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>故障率/开机率报表页面</returns>
        public ActionResult RatioReport(string reportName, string inter, string curName, string lastName, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ReportName = reportName;
            ViewBag.Inter = inter;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.YName = yName;
            ViewBag.ActionName = actionName;
            return View();
        }
        /// <summary>
        /// 按年月统计设备数量页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="inter">接口</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>按年月统计设备数量页面</returns>
        public ActionResult RepairTimeReport(string reportName, string inter, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ReportName = reportName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            ViewBag.ActionName = actionName;

            return View();
        }
        /// <summary>
        /// 请求响应时间页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="inter">接口名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>请求响应时间页面</returns>
        public ActionResult RequestResponseTimeReport(string reportName, int requestType, string inter, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ReportName = reportName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            ViewBag.RequestType = requestType;
            ViewBag.ActionName = actionName;

            return View();
        }
        /// <summary>
        /// 服务合格率页面
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="inter">接口名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>服务合格率页面</returns>
        public ActionResult RequestFinishedRate(string reportName, int requestType, string inter, string curName, string lastName, string yName, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.ID = requestType;
            ViewBag.Name = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.Inter = inter;
            ViewBag.YName = yName;
            return View();
        }
        /// <summary>
        /// 内部/供应商 请求率页面
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="inter">接口名称</param>
        /// <param name="actionName">上级页面名称</param>
        /// <param name="reportName">报表名称</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <returns>内部/供应商 请求率页面</returns>
        public ActionResult SupplierRequestRatio(int requestType, int status, string inter, string actionName, string reportName = "", string curName = "", string lastName = "", string yName = "")
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.RequestType = requestType;
            ViewBag.Status = status;
            ViewBag.Inter = inter;
            ViewBag.ReportName = reportName;
            ViewBag.ActionName = actionName;
            ViewBag.CurName = curName;
            ViewBag.LastName = lastName;
            ViewBag.YName = yName;
            return View();
        }

        #endregion

        #region 设备数量统计报表
        /// <summary>
        /// 统计设备数量
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备数量</returns>
        public JsonResult EquipmentCountReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.EquipmentCountReport(type, year, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备数量excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备数量excel</returns>
        public ActionResult ExportEquipmentCountReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.EquipmentCountReport(type, year, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备开机率
        /// <summary>
        /// 报表设备开机率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备开机率</returns>
        public JsonResult EquipmentBootRatioReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentBootRatio(type, year, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("repairTime", item.Item2);
                    temp.Add("totalTime", item.Item3);
                    temp.Add("eqptCount", item.Item4);
                    temp.Add("value", Math.Round(item.Item5, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备开机率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备开机率excel</returns>
        public ActionResult ExportEquipmentBootRatioReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentBootRatio(type, year, false);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add("故障时间(Hour)");
                dt.Columns.Add("总时间(Day)");
                dt.Columns.Add("设备数量(s)");
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4, info.Item5);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备开机率同比
        /// <summary>
        /// 设备开机率同比报表
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备开机率同比</returns>
        public JsonResult BootRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportBootRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备开机率同比excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备开机率同比excel</returns>
        public ActionResult ExportBootRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportBootRatio(type, year, month, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备故障率
        /// <summary>
        /// 统计设备故障率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备故障率</returns>
        public JsonResult EquipmentRapirRatioReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentRepairTimeRatio(type, year, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("repairTime", item.Item2);
                    temp.Add("totalTime", item.Item3);
                    temp.Add("eqptCount", item.Item4);
                    temp.Add("value", Math.Round(item.Item5, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备故障率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备故障率excel</returns>
        public ActionResult ExportEquipmentRapirRatioReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentRepairTimeRatio(type, year, false);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add("故障时间(Hour)");
                dt.Columns.Add("总时间(Day)");
                dt.Columns.Add("设备数量(s)");
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4, info.Item5);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 服务合格率
        /// <summary>
        /// 统计服务合格率
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>服务合格率</returns>
        public JsonResult RequestFinishedRateReport(int year)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> Counts = new List<Dictionary<string, object>>();
                List<Tuple<double, double, double, double>> list = this.reportDao.QueryRequestFinishedRate(year);

                List<KeyValueInfo> axisList = null;
                if (year > 0)
                {
                    axisList = ReportDimension.GetMonthList(year);
                }
                else
                {
                    int startYear = 0; int endYear = 0;
                    if (list.Count != 0)
                    { 
                        startYear = SQLUtil.ConvertInt(list.Min(x => x.Item1));
                        endYear = SQLUtil.ConvertInt(list.Max(x => x.Item1));
                    }
                    if (startYear == 0) startYear = DateTime.Today.Year;
                    if (endYear == 0) startYear = DateTime.Today.Year;
                    
                    axisList = ReportDimension.GetYearList(startYear, endYear);
                }

                foreach (KeyValueInfo typeItem in axisList)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    Tuple<double, double, double, double> temp = (from Tuple<double, double, double, double> temp1 in list where temp1.Item1 == typeItem.ID select temp1).FirstOrDefault();
                    if (temp == null)
                        temp = new Tuple<double, double, double, double>(0, 0, 0, 0);
                    item.Add("key", typeItem.Name);
                    item.Add("total", temp.Item2);
                    item.Add("passed", temp.Item3);
                    item.Add("value", temp.Item4);
                    Counts.Add(item);
                }
                result.Data = Counts;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出服务合格率excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>服务合格率excel</returns>
        public ActionResult ExportRequestFinishedRateReport(int year, string curName, string lastName, string yName, string reportName)
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
                List<Tuple<double, double, double, double>> list = this.reportDao.QueryRequestFinishedRate(year);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("时间");
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                List<KeyValueInfo> axisList = null;
                if (year > 0)
                {
                    axisList = ReportDimension.GetMonthList(year);
                }
                else
                {
                    int startYear = SQLUtil.ConvertInt(list.Min(x => x.Item1));
                    if (startYear == 0) startYear = DateTime.Today.Year;
                    int endYear = SQLUtil.ConvertInt(list.Max(x => x.Item1));
                    axisList = ReportDimension.GetYearList(startYear, endYear);
                }

                foreach (KeyValueInfo typeItem in axisList)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    Tuple<double, double, double, double> temp = (from Tuple<double, double, double, double> temp1 in list where temp1.Item1 == typeItem.ID select temp1).FirstOrDefault();
                    if (temp == null)
                        temp = new Tuple<double, double, double, double>(0, 0, 0, 0);
                    dt.Rows.Add(typeItem.Name, temp.Item2, temp.Item3, temp.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 请求响应时间
        /// <summary>
        /// 统计请求响应时间
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="requestType">请求类型</param>
        /// <returns>请求响应时间</returns>
        public JsonResult RepairResponseTimeReport(int year, int month, int requestType)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ResponseTime(requestType, year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出请求响应时间excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>请求响应时间excel</returns>
        public ActionResult ExportRepairResponseTimeReport(int year, int month, int requestType, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ResponseTime(requestType, year, month);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("类别");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备数量增长率报表
        /// <summary>
        /// 统计设备数量增长率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备数量增长率</returns>
        public JsonResult EquipmentRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备数量增长率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子名称</param>
        /// <param name="lastName">分母名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备数量增长率excel</returns>
        public ActionResult ExportEquipmentRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentRatio(type, year, month, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备故障时间(天)
        /// <summary>
        /// 统计设备故障时间(天)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备故障时间(天)</returns>
        public JsonResult EquipmentRepairTimeDayReport(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentRepairTimeDay(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备故障时间(天)excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备故障时间(天)excel</returns>
        public ActionResult ExportEquipmentRepairTimeDayReport(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentRepairTimeDay(year, month);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("时间段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备故障时间(小时)
        /// <summary>
        /// 统计设备故障时间(小时)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备故障时间(小时)</returns>
        public JsonResult EquipmentRepairTimeHourReport(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentRepairTimeHour(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备故障时间(小时)excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备故障时间(小时)excel</returns>
        public ActionResult ExportEquipmentRepairTimeHourReport(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentRepairTimeHour(year, month);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("时间段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备故障率同比
        /// <summary>
        /// 统计设备故障率同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备故障率同比</returns>
        public JsonResult FailureRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportFailureRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备故障率同比excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子名称</param>
        /// <param name="lastName">分母名称</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备故障率同比excel</returns>
        public ActionResult ExportFailureRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportFailureRatio(type, year, month, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备采购价格
        /// <summary>
        /// 统计设备采购价格
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备采购价格</returns>
        public JsonResult EquipmentPurchase(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.EquipmentCountByPurchaseAmount(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备采购价格excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备采购价格excel</returns>
        public ActionResult ExportEquipmentPurchase(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.EquipmentCountByPurchaseAmount(year, month);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 服务合同金额
        /// <summary>
        /// 统计服务合同金额
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>服务合同金额</returns>
        public JsonResult ContractAmount(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportContractAmount(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 导出服务合同金额excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>服务合同金额excel</returns>
        public ActionResult ExportContractAmount(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportContractAmount(year, month);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);


                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);

                }




                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 服务合同年限
        /// <summary>
        /// 统计服务合同年限
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>服务合同年限</returns>
        public JsonResult ContractYears(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportContractMonth(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 导出服务合同年限
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>服务合同年限excel</returns>
        public ActionResult ExportContractYears(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportContractMonth(year, month);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 折旧剩余年限
        /// <summary>
        /// 统计折旧剩余年限
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>折旧剩余年限</returns>
        public JsonResult DepreciationYears(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportDepreciationYears(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 导出折旧剩余年限excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>折旧剩余年限excel</returns>
        public ActionResult ExportDepreciationYears(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportDepreciationYears(year, month);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备折旧率
        /// <summary>
        /// 统计设备折旧率
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备折旧率</returns>
        public JsonResult DepreciationRate(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.DepreciationRate(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 导出设备折旧率excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备折旧率excel</returns>
        public ActionResult ExportDepreciationRate(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.DepreciationRate(year, month);

                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);


                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备检查人次
        /// <summary>
        /// 统计设备检查人次
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备检查人次</returns>
        public JsonResult ServiceCountReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ServiceCountReport(type, year, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备检查人次
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备检查人次excel</returns>
        public ActionResult ExportServiceCountReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ServiceCountReport(type, year, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备检查收入
        /// <summary>
        /// 统计设备检查收入
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备检查收入</returns>
        public JsonResult EquipmentSumIncome(int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentCountByIncome(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 导出设备检查收入excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备检查收入excel</returns>
        public ActionResult ExportEquipmentSumIncome(int year, int month, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentCountByIncome(year, month);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("阶段");
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备零配件花费总额/设备备件花费总额
        /// <summary>
        /// 统计设备零配件花费总额/设备备件花费总额
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备零配件花费总额/设备备件花费总额</returns>
        public JsonResult PartExpenditureReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {

                result.Data = this.reportManager.PartExpenditureReport(type, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备零配件花费总额/设备备件花费总额excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备零配件花费总额/设备备件花费总额excel</returns>
        public ActionResult ExportPartExpenditureReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.PartExpenditureReport(type, year, 0, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备总支出同比
        /// <summary>
        /// 统计设备总支出同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总支出同比</returns>
        public JsonResult ExpenditureRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportExpenditureRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备总支出同比excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备总支出同比excel</returns>
        public ActionResult ExportExpenditureRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportExpenditureRatio(type, year, month, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备总收入
        /// <summary>
        /// 统计设备总收入
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <returns>设备总收入</returns>
        public JsonResult EquipmentIncomeReport(int type, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentIncome(type, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备总收入excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备总收入excel</returns>
        public ActionResult ExportEquipmentIncomeReport(int type, int year, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ReportEquipmentIncome(type, year, 0, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备总收入同比
        /// <summary>
        /// 统计设备总收入同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总收入同比</returns>
        public JsonResult EquipmentIncomeRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentIncomeRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备总收入同比excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备总收入同比excel</returns>
        public ActionResult ExportEquipmentIncomeRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentIncomeRatio(type, year, month, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 设备总收支比
        /// <summary>
        /// 统计设备总收支比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总收支比</returns>
        public JsonResult IncomeRatioExpenditureReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.IncomeRatioExpenditure(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出设备总收支比excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>设备总收支比excel</returns>
        public ActionResult ExportIncomeRatioExpenditureReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.IncomeRatioExpenditure(type, year, month, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 请求数量
        /// <summary>
        /// 统计请求数量
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求数量</returns>
        public JsonResult ReportRequestCount(int type, int requestType = 0, int status = 0, int year = 0, int month = 0)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.RequestCount(type, requestType, status, year, month, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出请求数量excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>请求数量excel</returns>
        public ActionResult ExportReportRequestCount(int type, int requestType = 0, int status = 0, int year = 0, int month = 0, string yName = "", string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.RequestCount(type, requestType, status, year, 0, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 请求比率
        /// <summary>
        /// 统计请求比率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年份比率</param>
        /// <returns>请求比率</returns>
        public JsonResult RequestRatioReport(int type, int requestType, int status, bool byYear)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;

                List<Tuple<string, double, double, double>> list = this.reportManager.RequestRatio(type, requestType, status, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("key", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("value", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出请求比率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年份比率</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>请求比率excel</returns>
        public ActionResult ExportRequestRatioReport(int type, int requestType, int status, bool byYear, string curName, string lastName, string yName = "", string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;

                List<Tuple<string, double, double, double>> list = this.reportManager.RequestRatio(type, requestType, status, year, month, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 维修请求数量增长率
        /// <summary>
        /// 统计维修请求数量增长率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>维修请求数量增长率</returns>
        public JsonResult RepairRequestGrowthRatioReport(int type, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.RequestGrowthRatioReport(type, RequestInfo.RequestTypes.Repair, 0, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("key", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("value", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出维修请求数量增长率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>维修请求数量增长率excel</returns>
        public ActionResult ExportRepairRequestGrowthRatioReport(int type, int year, int month, string curName, string lastName, string yName, string reportName)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.RequestGrowthRatioReport(type, RequestInfo.RequestTypes.Repair, 0, year, month, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 派工响应时间
        /// <summary>
        /// 统计派工响应时间
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工响应时间</returns>
        public JsonResult ResponseDispatchTime(int year, int month)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ResponseDispatchTime(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出派工响应时间excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>派工响应时间excel</returns>
        public ActionResult ExportResponseDispatchTime(int year, int month, string yName, string reportName)
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
                List<Tuple<string, double>> data = this.reportManager.ResponseDispatchTime(year, month);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("响应时间区间");
                dt.Columns.Add(yName);

                foreach (var info in data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
                ms.Dispose();

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 派工执行率
        /// <summary>
        /// 统计派工执行率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年比率</param>
        /// <returns>派工执行率</returns>
        public JsonResult DispatchRatio(int type, int requestType, int status, bool byYear)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;

                List<Tuple<string, object>> Counts = new List<Tuple<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.DispatchResponseRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("key", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("value", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出派工执行率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年比率</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>派工执行率excel</returns>
        public ActionResult ExportDispatchRatio(int type, int requestType, int status, bool byYear, string curName, string lastName, string yName, string reportName)
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
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;
                List<Tuple<string, double, double, double>> list = this.reportManager.DispatchResponseRatio(type, year, month, false);
                DataTable dt = new DataTable("Sheet1");

                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
                ms.Dispose();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 服务时间达标率
        /// <summary>
        /// 统计服务时间达标率
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>服务时间达标率</returns>
        public JsonResult PassServiceRatioReport(int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ServicePassRatio(year, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("key", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("value", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出服务时间达标率excel
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>服务时间达标率excel</returns>
        public ActionResult ExportPassServiceRatioReport(int year, string curName, string lastName, string yName = "", string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ServicePassRatio(year, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("类型");
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 供应商保养数
        /// <summary>
        /// 统计供应商保养数
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <returns>供应商保养数</returns>
        public JsonResult ResultCount_supplierReport(int type, int requestType, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ResultCount(type, requestType, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出供应商保养数excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>供应商保养数excel</returns>
        public ActionResult ExportResultCount_supplierReport(int type, int requestType, int year, string yName, string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.ResultCount(type, requestType, year, 0, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 内部保养数
        /// <summary>
        /// 统计内部保养数
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <returns>内部保养数</returns>
        public JsonResult ResultCount_self(int type, int requestType, int year)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.SelfResultCount(type, requestType, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出内部保养数excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>内部保养数excel</returns>
        public ActionResult ExportResultCount_self(int type, int requestType, int year, string yName, string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                result.Data = this.reportManager.SelfResultCount(type, requestType, year, 0, false);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(yName);

                foreach (var info in result.Data)
                {
                    dt.Rows.Add(info.Item1, info.Item2);

                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 供应商维修率
        /// <summary>
        /// 统计供应商维修率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>供应商维修率</returns>
        public JsonResult Supplier_RepairRatioReport(int type, int requestType, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_supplier(type, requestType, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("key", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("value", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出供应商维修率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>供应商维修率excel</returns>
        public ActionResult ExportSupplier_RepairRatioReport(int type, int requestType, int year, int month, string curName, string lastName, string yName = "", string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Tuple<string, double>>> result = new ResultModel<List<Tuple<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_supplier(type, requestType, year, month, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 自修率
        /// <summary>
        /// 统计自修率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>自修率</returns>
        public JsonResult RepairRatioReport(int type, int requestType, int year, int month)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_self(type, requestType, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("key", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("value", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出自修率excel
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="curName">分子描述</param>
        /// <param name="lastName">分母描述</param>
        /// <param name="yName">纵坐标名称</param>
        /// <param name="reportName">报表名称</param>
        /// <returns>自修率excel</returns>
        public ActionResult ExportRepairRatioReport(int type, int requestType, int year, int month, string curName, string lastName, string yName = "", string reportName = "")
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Dictionary<string, double>>> result = new ResultModel<List<Dictionary<string, double>>>();
            try
            {
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_self(type, requestType, year, month, false);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add(ReportDimension.GetDimensionDesc(type));
                dt.Columns.Add(curName);
                dt.Columns.Add(lastName);
                dt.Columns.Add(yName);

                foreach (var info in list)
                {
                    dt.Rows.Add(info.Item1, info.Item2, info.Item3, info.Item4);
                }
                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", reportName + ".xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 单台设备收入支出
        /// <summary>
        /// 单台设备收入、支出
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="type">维度类型</param>
        /// <param name="year">选择年份</param>
        /// <returns>设备收入、支出</returns>
        public JsonResult SingleEquipmentAuditing(int id, int type = ReportDimension.AcceptanceYear, int year = 0)
        {
            ServiceResultModel<List<Tuple<string, double, double>>> result = new ServiceResultModel<List<Tuple<string, double, double>>>();
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
                if (year <= 0)
                    year = DateTime.Today.Year;
                if (WebConfig.ISDEMO)
                {
                    year = 2019;
                }
                List<Tuple<string, double, double>> list = new List<Tuple<string, double, double>>();
                Dictionary<string, double> incomes = this.serviceHisDao.GetServiceHisIncomesByEquipmentID(id, type, year);
                Dictionary<string, double> costs = this.serviceHisDao.GetAccessoryExpenseByEquipmentID(id, type, year);
                foreach (string key in incomes.Keys.Union(costs.Keys))
                {
                    double income = 0, cost = 0;
                    if (incomes.ContainsKey(key))
                        income = incomes[key];
                    if (costs.ContainsKey(key))
                        cost = costs[key];
                    list.Add(new Tuple<string, double, double>(key, income, cost));
                }

                result.Data = list;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}