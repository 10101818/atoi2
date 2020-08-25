using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// ReportController
    /// </summary>
    public class ReportController : BaseController
    {
        private ReportManager reportManager = new ReportManager();
        private ReportDao reportDao = new ReportDao();

        /// <summary>
        /// 获取维度列表
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>维度列表</returns>
        public ActionResult GetDimensionList(int userID, string sessionID)
        {
            ServiceResultModel<List<KeyValueInfo>> result = new ServiceResultModel<List<KeyValueInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<KeyValueInfo> list = ReportDimension.GetDimensionList();

                result.Data = list;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 统计设备数量
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备数量</returns>
        public ActionResult EquipmentCountReport(int userID, string sessionID,int  type, int year)
        {

            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.EquipmentCountReport(type, year,true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备故障率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备故障率</returns>
        public ActionResult EquipmentRapirRatioReport(int userID, string sessionID,int  type, int year)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentRepairTimeRatio(type, year,true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("repairTime", item.Item2);
                    temp.Add("totalTime", item.Item3);
                    temp.Add("eqptCount", item.Item4);
                    temp.Add("ratio", Math.Round(item.Item5, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备数量增长率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备数量增长率</returns>
        public ActionResult EquipmentRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 报表设备开机率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备开机率</returns>
        public ActionResult EquipmentBootRatioReport(int userID, string sessionID,int  type, int year)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, int, int, double>> list = this.reportManager.ReportEquipmentBootRatio(type, year, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("repairTime", item.Item2);
                    temp.Add("totalTime", item.Item3);
                    temp.Add("eqptCount", item.Item4);
                    temp.Add("ratio", Math.Round(item.Item5, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设备开机率同比报表
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备开机率同比</returns>
        public ActionResult BootRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportBootRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计服务合格率
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务合格率</returns>
        public ActionResult RequestFinishedRateReport(int userID, string sessionID,int  year)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> Counts = new List<Dictionary<string, object>>();
                List<Tuple<double, double, double, double>> list = this.reportDao.QueryRequestFinishedRate(year);

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
                    item.Add("type", typeItem.Name);
                    item.Add("total", temp.Item2);
                    item.Add("passed", temp.Item3);
                    item.Add("ratio", temp.Item4);
                    Counts.Add(item);
                }
                result.Data = Counts;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计请求响应时间
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求响应时间</returns>
        public ActionResult RepairResponseTimeReport(int userID, string sessionID,int  year, int month, int requestType)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ResponseTime(requestType, year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备故障时间(天)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备故障时间(天)</returns>
        public ActionResult EquipmentRepairTimeDayReport(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportEquipmentRepairTimeDay(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备故障时间(小时)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备故障时间(小时)</returns>
        public ActionResult EquipmentRepairTimeHourReport(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportEquipmentRepairTimeHour(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备故障率同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备故障率同比</returns>
        public ActionResult FailureRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportFailureRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备采购价格
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备采购价格</returns>
        public ActionResult EquipmentPurchase(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.EquipmentCountByPurchaseAmount(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计服务合同金额
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务合同金额</returns>
        public ActionResult ContractAmount(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportContractAmount(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 统计服务合同年限
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务合同年限</returns>
        public ActionResult ContractYears(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportContractMonth(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 统计折旧剩余年限
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>折旧剩余年限</returns>
        public ActionResult DepreciationYears(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportDepreciationYears(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 统计设备折旧率
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备折旧率</returns>
        public ActionResult DepreciationRate(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.DepreciationRate(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 统计设备检查人次
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备检查人次</returns>
        public ActionResult ServiceCountReport(int userID, string sessionID,int  type, int year)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ServiceCountReport(type, year, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备检查收入
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备检查收入</returns>
        public ActionResult EquipmentSumIncome(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportEquipmentCountByIncome(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 统计设备零配件花费总额/设备备件花费总额
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备零配件花费总额/设备备件花费总额</returns>
        public ActionResult PartExpenditureReport(int userID, string sessionID,int  type, int year)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);

                result.Data = this.reportManager.PartExpenditureReport(type, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备总支出同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备总支出同比</returns>
        public ActionResult ExpenditureRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportExpenditureRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备总收入
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备总收入</returns>
        public ActionResult EquipmentIncomeReport(int userID, string sessionID,int  type, int year)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ReportEquipmentIncome(type, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备总收入同比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备总收入同比</returns>
        public ActionResult EquipmentIncomeRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.ReportEquipmentIncomeRatio(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计设备总收支比
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备总收支比</returns>
        public ActionResult IncomeRatioExpenditureReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.IncomeRatioExpenditure(type, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", item.Item4);
                    count.Add(temp);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计请求数量
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求数量</returns>
        public ActionResult ReportRequestCount(int userID, string sessionID,int  type, int requestType = 0, int status = 0, int year = 0)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.RequestCount(type, requestType, status, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计请求比率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年份比率</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>请求比率</returns>
        public ActionResult RequestRatioReport(int userID, string sessionID,int  type, int requestType, int status, bool byYear)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;

                List<Tuple<string, double, double, double>> list = this.reportManager.RequestRatio(type, requestType, status, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("type", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("ratio", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计维修请求数量增长率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>维修请求数量增长率</returns>
        public ActionResult RepairRequestGrowthRatioReport(int userID, string sessionID,int  type, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.RequestGrowthRatioReport(type, RequestInfo.RequestTypes.Repair, 0, year, month, true);
                foreach (var item in list)
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();
                    temp.Add("type", item.Item1);
                    temp.Add("cur", item.Item2);
                    temp.Add("last", item.Item3);
                    temp.Add("ratio", Math.Round(item.Item4, 2));
                    count.Add(temp);
                }
                result.Data = count;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计派工响应时间
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工响应时间</returns>
        public ActionResult ResponseDispatchTime(int userID, string sessionID,int  year, int month)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ResponseDispatchTime(year, month);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计派工执行率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="byYear">是否按年比率</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>派工执行率</returns>
        public ActionResult DispatchRatio(int userID, string sessionID,int  type, int status, bool byYear)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                int year = DateTime.Today.Year;
                int month = byYear ? 0 : DateTime.Today.Month;

                List<Tuple<string, object>> Counts = new List<Tuple<string, object>>();
                List<Tuple<string, double, double, double>> list = this.reportManager.DispatchResponseRatio(type, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("type", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("ratio", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计服务时间达标率
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>服务时间达标率</returns>
        public ActionResult PassServiceRatioReport(int userID, string sessionID,int  year)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Tuple<string, double, double, double>> list = this.reportManager.ServicePassRatio(year, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("type", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("ratio", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计供应商保养数
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>供应商保养数</returns>
        public ActionResult ResultCount_supplierReport(int userID, string sessionID,int  type, int requestType, int year)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.ResultCount(type, requestType, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计内部保养数
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>内部保养数</returns>
        public ActionResult ResultCount_self(int userID, string sessionID,int  type, int requestType, int year)
        {
            ServiceResultModel<List<Tuple<string, double>>> result = new ServiceResultModel<List<Tuple<string, double>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.reportManager.SelfResultCount(type, requestType, year, 0, true);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计供应商维修率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>供应商维修率</returns>
        public ActionResult Supplier_RepairRatioReport(int userID, string sessionID,int  type, int requestType, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_supplier(type, requestType, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("type", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("ratio", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 统计自修率
        /// </summary>
        /// <param name="type">统计维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>自修率</returns>
        public ActionResult RepairRatioReport(int userID, string sessionID,int  type, int requestType, int year, int month)
        {
            ServiceResultModel<List<Dictionary<string, object>>> result = new ServiceResultModel<List<Dictionary<string, object>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                List<Tuple<string, double, double, double>> list = this.reportManager.ResultRatio_self(type, requestType, year, month, true);
                List<Dictionary<string, object>> count = new List<Dictionary<string, object>>();
                foreach (var info in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("type", info.Item1);
                    item.Add("cur", info.Item2);
                    item.Add("last", info.Item3);
                    item.Add("ratio", info.Item4);
                    count.Add(item);
                }
                result.Data = count;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }        
    }
}