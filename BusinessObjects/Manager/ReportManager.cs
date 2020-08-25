using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 报表manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ReportManager
    {
        private ReportDao reportDao = new ReportDao();
        private EquipmentDao equipmentDao = new EquipmentDao();
        private SupplierDao supplierDao = new SupplierDao();
        private const int RECORD_LIMIT_MANUFACTURE = 10;

        #region axisX
        /// <summary>
        /// 获取维度下数据列表
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="startYear">年份</param>
        /// <returns>维度下数据列表</returns>
        public List<string> GetAxisXList(int type, int startYear = 0)
        {
            List<string> axisXList = null;
            switch (type)
            {
                case ReportDimension.AcceptanceYear:
                    axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetYearList(startYear), "ID");
                    break;
                case ReportDimension.AcceptanceMonth:
                    axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetMonthList(startYear), "ID");
                    break;
                case ReportDimension.AmountType:
                    axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetAmountList(), "Name");
                    break;
                case ReportDimension.UsageTimeType:
                    axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetUsageTimeList(), "Name");
                    break;
                case ReportDimension.EquipmentType:
                    axisXList = SQLUtil.GetStringListFromObjectList(LookupManager.GetEquipmentClass(1), "Description");
                    axisXList.Add("");
                    break;
                case ReportDimension.OriginType:
                    axisXList = new List<string>();
                    axisXList.Add(EquipmentInfo.GetOriginType(false));
                    axisXList.Add(EquipmentInfo.GetOriginType(true));
                    break;
                case ReportDimension.DepartmentType:
                    axisXList = SQLUtil.GetStringListFromObjectList(LookupManager.GetDepartments(), "Description");
                    break;
                case ReportDimension.ManufacturerType:
                    axisXList = SQLUtil.GetStringListFromObjectList(this.equipmentDao.QueryManufacturer(), "Name");
                    axisXList.Add("");
                    break;
            }
            return axisXList;
        }
        /// <summary>
        /// 获取字段描述信息
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="dr">字段信息</param>
        /// <returns>字段名称</returns>
        public string GetAxisX(int type, DataRow dr)
        {
            string axisX = "";
            switch (type)
            {
                case ReportDimension.AcceptanceYear:
                    axisX = SQLUtil.TrimNull(dr[0]);
                    break;
                case ReportDimension.AcceptanceMonth:
                    axisX = SQLUtil.TrimNull(dr[0]);
                    break;
                case ReportDimension.DepartmentType:
                    axisX = LookupManager.GetDepartmentDesc(SQLUtil.ConvertInt(dr[0]));
                    break;
                case ReportDimension.EquipmentType:
                    axisX = LookupManager.GetEquipmentClassDesc(SQLUtil.TrimNull(dr[0]), 1);
                    break;
                case ReportDimension.ManufacturerType:
                    SupplierInfo info = supplierDao.GetSupplier(SQLUtil.ConvertInt(dr[0]));
                    axisX = (info == null ? "" : info.Name);
                    break;
                case ReportDimension.OriginType:
                    axisX = EquipmentInfo.GetOriginType(SQLUtil.ConvertBoolean(dr[0]));
                    break;
            }
            return axisX;
        }
        /// <summary>
        /// 计算增长率
        /// </summary>
        /// <param name="cur">本年数据</param>
        /// <param name="last">去年数据</param>
        /// <returns>增长率</returns>
        public double GetGrowthRatio(double cur, double last)
        {
            double ratio = last == 0 ? (cur == 0 ? 0 : 100.0) : Math.Round((cur - last) * 100.0 / last, 2);
            return ratio;
        }
        /// <summary>
        /// 计算同比
        /// </summary>
        /// <param name="cur">本年数据</param>
        /// <param name="last">去年数据</param>
        /// <returns>同比比率</returns>
        public double GetVSRatio(double cur, double last)
        {
            return (last == 0 ? (cur == 0 ? 0 : 100.0) : Math.Round(cur * 100.0 / last, 2));
        }
        /// <summary>
        /// 获取日期段
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="checkToday">是否检测时间大于当前时间</param>
        /// <returns>时间段</returns>
        public Tuple<DateTime, DateTime> GetDateRange(int year, int month, bool checkToday = false)
        {
            DateTime dateFrom, dateTo;
            if (month == 0)
            {
                dateFrom = new DateTime(year, 1, 1);
                dateTo = dateFrom.AddYears(1);
            }
            else
            {
                dateFrom = new DateTime(year, month, 1);
                dateTo = dateFrom.AddMonths(1);
            }

            if (checkToday && dateTo > DateTime.Today.AddDays(1)) dateTo = DateTime.Today.AddDays(1);

            return  new Tuple<DateTime, DateTime>(dateFrom, dateTo);
        }
        #endregion

        /// <summary>
        /// dictionary类型数据补充缺失数据
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="result">原始数据</param>
        /// <param name="axisXList">维度下数据列表</param>
        /// <returns>补漏后的数据</returns>
        private List<Tuple<string, double>> FillMissingAxis(int type, Dictionary<string, double> result, List<string> axisXList)
        {
            List<Tuple<string, double>> raw = new List<Tuple<string, double>>();
            foreach (var axisX in axisXList)
            {
                if (result.ContainsKey(axisX))
                {
                    raw.Add(new Tuple<string,double>(axisX, result[axisX]));
                }
                else
                {
                    if (type != ReportDimension.ManufacturerType && type != ReportDimension.EquipmentType)
                    {
                        raw.Add(new Tuple<string,double>(axisX, 0));
                    }
                }
            }
            return raw;
        }
        /// <summary>
        /// 合并且补充缺失数据
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="resultCur">当年数据</param>
        /// <param name="resultLast">去年数据</param>
        /// <param name="axisList">数据列表</param>
        /// <returns>合并补充后的数据</returns>
        private List<Tuple<string, double, double, double>> FillMissingAxis(int type, List<Tuple<string, double>> resultCur, List<Tuple<string, double>> resultLast, List<string> axisList)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            foreach (string axisX in axisList)
            {
                Tuple<string, double> lastInfo = resultLast.SingleOrDefault(t => t.Item1.Equals(axisX));
                Tuple<string, double> curInfo = resultCur.SingleOrDefault(t => t.Item1.Equals(axisX));
                if (lastInfo != null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item2, lastInfo.Item2, GetGrowthRatio(curInfo.Item2, lastInfo.Item2)));
                else if (lastInfo != null && curInfo == null)
                    result.Add(new Tuple<string, double, double, double>(axisX, 0, lastInfo.Item2, lastInfo.Item2 == 0 ? 0 : - 100));
                else if (lastInfo == null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item2, 0, curInfo.Item2 == 0 ? 0 : 100));
                else
                {
                    if (type != ReportDimension.ManufacturerType && type != ReportDimension.EquipmentType)
                        result.Add(new Tuple<string, double, double, double>(axisX, 0, 0, 0));
                }
            }
            return result;
        }
        /// <summary>
        /// 维度为‘厂商分布’的合并补缺的数据取前10个
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="result">合并补缺的数据</param>
        /// <param name="limitRecord">是否需要取前10</param>
        /// <returns>统计后的数据</returns>
        private static List<Tuple<string, double, double, double>> GetTopManufacturer(int type, List<Tuple<string, double, double, double>> result, bool limitRecord)
        {
            if (limitRecord == true && type == ReportDimension.ManufacturerType)
            {
                List<Tuple<string, double, double, double>> resort = new List<Tuple<string, double, double, double>>();

                foreach (var item in result.OrderByDescending(x => x.Item4))
                {
                    resort.Add(item);

                    if (resort.Count == 10) break;
                }
                result = resort;
            }
            return result;
        }
        /// <summary>
        /// 维度为‘厂商分布’的补缺后的数据取前10个
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="result">补缺的数据</param>
        /// <param name="limitRecord">是否需要取前10</param>
        /// <returns>统计后的数据</returns>
        private static List<Tuple<string, double>> GetTopManufacturer(int type, List<Tuple<string, double>> result, bool limitRecord)
        {
            if (limitRecord == true && type == ReportDimension.ManufacturerType)
            {
                List<Tuple<string, double>> manu = new List<Tuple<string, double>>();
                foreach (var item in result.OrderByDescending(x => x.Item2))
                {
                    manu.Add(new Tuple<string,double>(item.Item1, item.Item2));
                    if (manu.Count == 10) break;
                }
                result = manu;
            }
            return result;
        }
        /// <summary>
        /// 计算开机率/故障率同比
        /// </summary>
        /// <param name="type">维度编号</param>
        /// <param name="resultA">当年数据</param>
        /// <param name="resultB">去年数据</param>
        /// <returns>同比后的数据</returns>
        private List<Tuple<string, double, double, double>> GetVSRatio(int type, List<Tuple<string, double, int, int, double>> resultA, List<Tuple<string, double, int, int, double>> resultB)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            List<string> axisXList = GetAxisXList(type);
            Tuple<string, double, int, int, double> lastInfo = null;
            Tuple<string, double, int, int, double> curInfo = null;
            foreach (string axisX in axisXList)
            {
                lastInfo = resultB.SingleOrDefault(t => t.Item1.Equals(axisX));
                curInfo = resultA.SingleOrDefault(t => t.Item1.Equals(axisX));
                if (lastInfo != null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item5, lastInfo.Item5, GetVSRatio(curInfo.Item5, lastInfo.Item5)));
                else if (lastInfo != null && curInfo == null)
                    result.Add(new Tuple<string, double, double, double>(axisX, 0, lastInfo.Item5, 0));
                else if (lastInfo == null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item5, 0, curInfo.Item5 == 0 ? 0 : 100));
                else
                {
                    if (type != ReportDimension.ManufacturerType && type != ReportDimension.EquipmentType)
                        result.Add(new Tuple<string, double, double, double>(axisX, 0, 0, 0));
                }
            }
            return result;
        }
        /// <summary>
        /// Tuple类型的数据同比、补缺
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="income">分子数据</param>
        /// <param name="expenditure">分母数据</param>
        /// <param name="axisXList">数据列表</param>
        /// <returns>同比、补缺后的数据</returns>
        private List<Tuple<string, double, double, double>> GetVSRatio(int type, List<Tuple<string, double>> income, List<Tuple<string, double>> expenditure, List<string> axisXList)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            foreach (var axisX in axisXList)
            {
                Tuple<string, double> curInfo = income.SingleOrDefault(t => t.Item1.Equals(axisX));
                Tuple<string, double> lastInfo = expenditure.SingleOrDefault(t => t.Item1.Equals(axisX));
                if (lastInfo != null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item2, lastInfo.Item2, GetVSRatio(curInfo.Item2, lastInfo.Item2)));
                else if (lastInfo != null && curInfo == null)
                    result.Add(new Tuple<string, double, double, double>(axisX, 0.00, lastInfo.Item2, 0));
                else if (lastInfo == null && curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(axisX, curInfo.Item2, 0, curInfo.Item2 == 0 ? 0 : 100.0));
                else
                {
                    if (type != ReportDimension.ManufacturerType && type != ReportDimension.EquipmentType)
                        result.Add(new Tuple<string, double, double, double>(axisX, 0, 0, 0));
                }
            }

            return result;
        }
        /// <summary>
        /// Dictionary类型的数据同比、补缺
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="income">分子数据</param>
        /// <param name="expenditure">分母数据</param>
        /// <param name="axisXList">数据列表</param>
        /// <returns>同比、补缺后的数据</returns>
        private List<Tuple<string, double, double, double>> GetVSRatio(int type, Dictionary<string, double> income, Dictionary<string, double> expenditure, List<string> axisXList)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            foreach (var axisX in axisXList)
            {
                if (income.ContainsKey(axisX) && expenditure.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, income[axisX], expenditure[axisX], GetVSRatio(income[axisX], expenditure[axisX])));
                else if (income.ContainsKey(axisX) && !expenditure.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, income[axisX], 0, income[axisX] == 0 ? 0 : 100.0));
                else if (!income.ContainsKey(axisX) && expenditure.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, 0.00, expenditure[axisX], 0));
                else
                {
                    if (type != ReportDimension.ManufacturerType && type != ReportDimension.EquipmentType)
                        result.Add(new Tuple<string, double, double, double>(axisX, 0, 0, 0));
                }           
            }

            return result;
        }     
        
        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>维度下设备数量列表</returns>
        public List<Tuple<string, double>> GetEquipmentCount(int type, int year, int month)
        {
            Dictionary<string, double> result = new Dictionary<string, double>(); 
            List<string> axisXList = GetAxisXList(type, year);
            switch (type)
            {
                case ReportDimension.AcceptanceMonth:
                    foreach (var axis in axisXList)
                    {
                        result.Add(axis, this.reportDao.EquipmentCount(year, SQLUtil.ConvertInt(axis)));
                    }
                    break;
                case ReportDimension.AcceptanceYear:
                    year = this.reportDao.ReportEquipmentCountMinYear();
                    if (year == 0) year = DateTime.Today.Year;
                    axisXList = GetAxisXList(type, year);
                    foreach (var axis in axisXList)
                    {
                        result.Add(axis, this.reportDao.EquipmentCount(SQLUtil.ConvertInt(axis), 0));
                    }
                    break;
                case ReportDimension.AmountType:
                    result = this.reportDao.ReportEquipmentCountByPurchaseAmount(year, month);
                    break;
                case ReportDimension.UsageTimeType:
                    result = this.reportDao.ReportEquipmentCountByUsageTime(year, month);
                    break;
                default:
                    DataTable dtCount = this.reportDao.ReportEquipmentCount(type, year, month);
                    string axisX = null;
                    foreach (DataRow dr in dtCount.Rows)
                    {
                        axisX = GetAxisX(type, dr);
                        result.Add(axisX, SQLUtil.ConvertInt(dr[1]));
                    }
                    break;
            }

            return FillMissingAxis(type, result, axisXList);
        }
        /// <summary>
        /// 设备数量报表(type,eqptCount)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="limitRecord">是否需要获取前10的厂商</param>
        /// <returns>设备数量列表</returns>
        public List<Tuple<string, double>> EquipmentCountReport(int type, int year, bool limitRecord = false)
        {
            List<Tuple<string, double>> result = GetEquipmentCount(type, year, 0);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 设备数量增长率报表(type,cur,last,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否需要厂商维度下取前10</param>
        /// <returns>设备数量增长率统计</returns>
        public List<Tuple<string, double, double, double>> ReportEquipmentRatio(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            List<Tuple<string, double>> resultCur = GetEquipmentCount(type, year, month);
            List<Tuple<string, double>> resultLast = GetEquipmentCount(type, year - 1, month);

            List<string> axisList = GetAxisXList(type);
            result = FillMissingAxis(type, resultCur, resultLast, axisList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 计算故障率
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>故障率</returns>
        private List<Tuple<string, double, int, int, double>> ReportEquipmentRepairTimeRatioInternal(int type, int year, int month = 0)
        {
            List<Tuple<string, double, int, int, double>> result = new List<Tuple<string, double, int, int, double>>();
            Dictionary<string, double> dicRepairTime = new Dictionary<string,double>();
            Dictionary<string, double> dicCount = new Dictionary<string,double>();

            List<string> axisXList = GetAxisXList(type, year);
            if (year == 0) year = DateTime.Today.Year;
            Tuple<DateTime, DateTime> dateRange = GetDateRange(year, month, true);
            DateTime yearBegin = new DateTime(year, 1, 1);
            int thisYear = 0;
            int totalDays = (dateRange.Item2 - dateRange.Item1).Days;

            switch (type)
            {
                case ReportDimension.AcceptanceYear:
                    year = this.reportDao.ReportEquipmentRepairTimeRatioMinYear();
                    if (year == 0) year = DateTime.Today.Year;
                    axisXList = GetAxisXList(type, year);
                    foreach (var axis in axisXList)
                    {
                        thisYear = SQLUtil.ConvertInt(axis);
                        yearBegin = new DateTime(thisYear, 1, 1);
                        totalDays = (yearBegin.AddYears(1) - yearBegin).Days;
                        int equipCount = this.reportDao.EquipmentCount(thisYear, 0);
                        int repairTime = this.reportDao.ReportEquipmentRepairTimeRatioByPurchaseDate(yearBegin, yearBegin.AddYears(1));
                        double repairTimeRatio = equipCount == 0 ? 0 : Math.Round(repairTime * 100.00 / totalDays / 24 / equipCount, 2);

                        result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(thisYear), SQLUtil.ConvertDouble(repairTime), totalDays, equipCount, repairTimeRatio));
                    }
                    break;
                case ReportDimension.AcceptanceMonth:
                    foreach (var axis in axisXList)
                    {
                        int thisMonth = SQLUtil.ConvertInt(axis);
                        int equipCount = this.reportDao.EquipmentCount(year, thisMonth);
                        DateTime monthBegin = new DateTime(year, thisMonth, 1);
                        int repairTime = this.reportDao.ReportEquipmentRepairTimeRatioByPurchaseDate(monthBegin, monthBegin.AddMonths(1));

                        double repairTimeRatio = equipCount == 0 ? 0 : Math.Round(repairTime * 100.00 / monthBegin.AddMonths(1).AddDays(-1).Day / 24 / equipCount, 2);
                        result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(thisMonth), SQLUtil.ConvertDouble(repairTime), monthBegin.AddMonths(1).AddDays(-1).Day, equipCount, repairTimeRatio));
                    }
                    break;
                case ReportDimension.UsageTimeType:
                    dicRepairTime = this.reportDao.ReportEquipmentRepairTimeRatioByUsageTime(dateRange.Item1, dateRange.Item2);
                    dicCount = this.reportDao.ReportEquipmentCountByUsageTime(year, month);
                    foreach (var axisX in axisXList)
                    {
                        if (dicRepairTime.ContainsKey(axisX) && dicRepairTime[axisX] != 0 &&
                            dicCount.ContainsKey(axisX) && dicCount[axisX] != 0)
                        {
                            double repairTimeRatio = Math.Round(dicRepairTime[axisX] * 100.00 / totalDays / 24 / SQLUtil.ConvertInt(dicCount[axisX]), 2);
                            result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(axisX), SQLUtil.ConvertDouble(dicRepairTime[axisX]), totalDays, SQLUtil.ConvertInt(dicCount[axisX]), repairTimeRatio));
                        }
                        else
                            result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(axisX), 0, totalDays, 0, 0));
                    }
                    break;
                case ReportDimension.AmountType:
                    dicRepairTime = this.reportDao.ReportEquipmentRepairTimeRatioByPurchaseAmount(dateRange.Item1, dateRange.Item2);
                    dicCount = this.reportDao.ReportEquipmentCountByPurchaseAmount(year, month);
                    foreach (var axisX in axisXList)
                    {
                        if (dicRepairTime.ContainsKey(axisX) && dicRepairTime[axisX] != 0 &&
                            dicCount.ContainsKey(axisX) && dicCount[axisX] != 0)
                        {
                            double repairTimeRatio = Math.Round(dicRepairTime[axisX] * 100.00 / totalDays / 24 / SQLUtil.ConvertInt(dicCount[axisX]), 2);
                            result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(axisX), SQLUtil.ConvertDouble(dicRepairTime[axisX]), totalDays, SQLUtil.ConvertInt(dicCount[axisX]), repairTimeRatio));
                        }
                        else
                            result.Add(new Tuple<string, double, int, int, double>(SQLUtil.TrimNull(axisX), 0, totalDays, 0, 0));
                    }
                    break;
                default:
                    DataTable dtRepairTime = this.reportDao.ReportEquipmentRepairTimeRatio(type, dateRange.Item1, dateRange.Item2);
                    DataTable dtCount = this.reportDao.ReportEquipmentCount(type, year, month);
                    foreach (string axisX in axisXList)
                    {
                        DataRow drCount = (from DataRow temp in dtCount.Rows where GetAxisX(type, temp) == axisX select temp).FirstOrDefault();
                        DataRow drRepair = (from DataRow temp in dtRepairTime.Rows where GetAxisX(type, temp) == axisX select temp).FirstOrDefault();
                        if (drCount == null || SQLUtil.ConvertInt(drCount[1]) == 0 || drRepair == null || SQLUtil.ConvertInt(drRepair[1]) == 0)
                        {
                            if(type != ReportDimension.EquipmentType && type != ReportDimension.ManufacturerType)
                                result.Add(new Tuple<string, double, int, int, double>(axisX, 0, totalDays, drCount == null ? 0 : SQLUtil.ConvertInt(drCount[1]), 0));
                        }
                        else
                            result.Add(new Tuple<string, double, int, int, double>(axisX, SQLUtil.ConvertDouble(drRepair[1]), totalDays,
                                    SQLUtil.ConvertInt(drCount[1]), Math.Round(SQLUtil.ConvertDouble(SQLUtil.ConvertInt(drRepair[1]) * 100.0 / 24 / totalDays / SQLUtil.ConvertDouble(drCount[1])), 2)));
                    }
                    break;
            }

            return result;
        }
        /// <summary>
        /// 故障率报表(type,repairTime,totalTime,eqptCount,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>故障率统计</returns>
        public List<Tuple<string, double, int, int, double>> ReportEquipmentRepairTimeRatio(int type, int year, bool limitRecord = false)
        {
            List<Tuple<string, double, int, int, double>> result = new List<Tuple<string, double, int, int, double>>();
            
            result = ReportEquipmentRepairTimeRatioInternal(type, year, 0);

            if (limitRecord == true && type == ReportDimension.ManufacturerType)
            {
                List<Tuple<string, double, int, int, double>> resort = new List<Tuple<string, double, int, int, double>>();

                foreach (var item in result.OrderByDescending(x => x.Item5))
                {
                    resort.Add(item);

                    if (resort.Count == 10) break;
                }

                result = resort;
            }

            return result;
        }
        /// <summary>
        /// 故障率同比报表(type,curRatio,lastRatio, Ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>故障率同比统计</returns>
        public List<Tuple<string, double, double, double>> ReportFailureRatio(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double, int, int, double>> resultA = ReportEquipmentRepairTimeRatioInternal(type, year, month);
            List<Tuple<string, double, int, int, double>> resultB = ReportEquipmentRepairTimeRatioInternal(type, year - 1, month);

            List<Tuple<string, double, double, double>> result = GetVSRatio(type, resultA, resultB);
            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 开机率报表(type,repairTime,totalTime,eqptCount,ratio)
       /// </summary>
       /// <param name="type">维度</param>
       /// <param name="year">年份</param>
       /// <param name="limitRecord">是否厂商维度取前10</param>
       /// <returns>开机率统计</returns>
        public List<Tuple<string, double, int, int, double>> ReportEquipmentBootRatio(int type, int year, bool limitRecord = false)
        {
            List<Tuple<string, double, int, int, double>> result = ReportEquipmentRepairTimeRatioInternal(type, year, 0);
            result = GetBoot(result);

            if (limitRecord == true && type == ReportDimension.ManufacturerType)
            {
                List<Tuple<string, double, int, int, double>> resort = new List<Tuple<string, double, int, int, double>>();

                foreach (var item in result.OrderByDescending(x => x.Item5))
                {
                    resort.Add(item);

                    if (resort.Count == 10) break;
                }

                result = resort;
            }

            return result;
        }
        /// <summary>
        /// 根据故障率计算开机率(type,repairTime,totalTime,eqptCount,ratio)
        /// </summary>
        /// <param name="dic">故障率统计</param>
        /// <returns>设备开机率</returns>
        public List<Tuple<string, double, int, int, double>> GetBoot(List<Tuple<string, double, int, int, double>> dic)
        {
            List<Tuple<string, double, int, int, double>> result = new List<Tuple<string, double, int, int, double>>();
            foreach (var item in dic)
            {
                result.Add(new Tuple<string, double, int, int, double>(item.Item1, item.Item2, item.Item3, item.Item4, Math.Round(100 - item.Item5,2)));
            }
            dic = result;
            return dic;
        }
        /// <summary>
        /// 开机率同比报表(type,curRatio,lastRatio, Ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>开机率同比统计</returns>
        public List<Tuple<string, double, double, double>> ReportBootRatio(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double, int, int, double>> resultA = ReportEquipmentRepairTimeRatioInternal(type, year, month);
            resultA = GetBoot(resultA);

            List<Tuple<string, double, int, int, double>> resultB = ReportEquipmentRepairTimeRatioInternal(type, year - 1, month);
            resultB = GetBoot(resultB);

            List<Tuple<string, double, double, double>> result = GetVSRatio(type, resultA, resultB);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 故障时间（天）(type,eqptCount)报表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>故障时间(天)报表统计</returns>
        public List<Tuple<string, double>> ReportEquipmentRepairTimeDay(int year, int month)
        {
            Tuple<DateTime, DateTime> dateRange = GetDateRange(year, month, true);
            Dictionary<string, double> raw = this.reportDao.ReportEquipmentRepairTimeDay(dateRange.Item1, dateRange.Item2);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetRepairDaysList(),"Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 故障时间（小时）(type,eqptCount)报表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>故障时间(小时)报表统计</returns>
        public List<Tuple<string, double>> ReportEquipmentRepairTimeHour(int year, int month)
        {
            Tuple<DateTime, DateTime> dateRange = GetDateRange(year, month, true);
            Dictionary<string, double> raw = this.reportDao.ReportEquipmentRepairTimeHour(dateRange.Item1, dateRange.Item2);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetRepairHoursList(), "Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 设备采购价格报表(type,eqptCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备采购价格</returns>
        public List<Tuple<string, double>> EquipmentCountByPurchaseAmount(int year, int month)
        {
            Dictionary<string, double> raw = this.reportDao.EquipmentCountByPurchaseAmount(year, month);
            List<string> axisXList = GetAxisXList(ReportDimension.AmountType);

            return FillMissingAxis(ReportDimension.AmountType, raw, axisXList);
        }
        /// <summary>
        /// 合同金额报表(type,eqptCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>合同金额</returns>
        public List<Tuple<string, double>> ReportContractAmount(int year, int month)
        {
            Tuple<DateTime, DateTime> dateRange = GetDateRange(year, month);
            Dictionary<string, double> raw = this.reportDao.ReportContractAmount(dateRange.Item1, dateRange.Item2.AddDays(-1));
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetContractAmountList(), "Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 合同年限报表(type,eqptCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>合同年限</returns>
        public List<Tuple<string, double>> ReportContractMonth(int year, int month)
        {
            Tuple<DateTime, DateTime> dateRange = GetDateRange(year, month);
            Dictionary<string, double> raw = this.reportDao.ReportContractMonths(dateRange.Item1, dateRange.Item2.AddDays(-1));
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetContractMonthList(), "Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 请求响应时间报表(type,requestCount)
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求响应时间</returns>
        public List<Tuple<string, double>> ResponseTime(int requestType, int year, int month)
        {
            Dictionary<string, double> res = this.reportDao.ResponseTime(requestType, year, month);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetResponseTimeList(), "Name");

            return FillMissingAxis(0, res, axisXList);
        }
        /// <summary>
        /// 折旧剩余年限报表(type,eqptCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>折旧剩余年限</returns>
        public List<Tuple<string, double>> ReportDepreciationYears(int year, int month)
        {
            Dictionary<string, double> raw = this.reportDao.ReportDepreciationYears(year, month);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetDepreciationYearsList(), "Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 折旧率报表(type,eqptCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>折旧率统计</returns>
        public List<Tuple<string, double>> DepreciationRate(int year, int month)
        {
            Dictionary<string, double> raw = this.reportDao.ReportDepreciationRate(year, month);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetDepreciationRationList(), "Name");

            return FillMissingAxis(0, raw, axisXList);
        }
        /// <summary>
        /// 服务人次报表(type,number)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>服务人次统计</returns>
        public List<Tuple<string, double>> ServiceCountReport(int type, int year, bool limitRecord = false)
        {
            Dictionary<string, double> raw = new Dictionary<string, double>();

            if ( type == ReportDimension.AcceptanceMonth)
                raw = this.reportDao.ReportServiceCountByAcceptanceMonth(year);
            else if(type == ReportDimension.AcceptanceYear){
                raw = this.reportDao.ReportServiecCountByAcceptanceYear();
                if(raw.Count != 0)
                    year = SQLUtil.ConvertInt(raw.Keys.Min());
                if (year == 0) year = DateTime.Today.Year;
            }
            else if (type == ReportDimension.AmountType)
                raw = this.reportDao.ReportServiceCountByPurchaseAmount(year);
            else if (type == ReportDimension.UsageTimeType)
                raw = this.reportDao.ReportServiceCountByDepreciationYears(year);
            else
            {
                DataTable count = null;
                count = this.reportDao.ReportServiceCount(type, year);
                string axisX = null;
                foreach (DataRow dr in count.Rows)
                {
                    axisX = GetAxisX(type, dr);
                    raw.Add(axisX, SQLUtil.ConvertDouble(dr[1]));
                }
            }

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double>> result = FillMissingAxis(type, raw, axisXList);

            result = GetTopManufacturer(type, result, limitRecord);

            return result;
        }        
        /// <summary>
        /// 设备检查收入报表(type,amount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备检查收入统计</returns>
        public List<Tuple<string, double>> ReportEquipmentCountByIncome(int year, int month)
        {
            Dictionary<string, double> result = this.reportDao.ReportEquipmentCountByIncome(year, month);
            return FillMissingAxis(ReportDimension.AmountType, result, GetAxisXList(ReportDimension.AmountType));
        }
        /// <summary>
        /// 零备件花费报表(type,amount)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>零配件花费统计</returns>
        public List<Tuple<string, double>> PartExpenditureReport(int type, int year, int month = 0, bool limitRecord = false)
        {
            Dictionary<string, double> raw = new Dictionary<string, double>();
            if (type == ReportDimension.AcceptanceMonth)
                raw = this.reportDao.ReportEquipmentExpenditureByAcceptanceMonth(year);
            else if (type == ReportDimension.AcceptanceYear)
            {
                raw = this.reportDao.ReportEquipmentExpenditureByAcceptanceYear();
                if (raw.Count != 0)
                    year = SQLUtil.ConvertInt(raw.Keys.Min());
                if (year == 0) year = DateTime.Today.Year;
            }
            else if (type == ReportDimension.AmountType)
                raw = this.reportDao.ReportEquipmentExpenditureByPurchaseAmount(year, month);
            else if (type == ReportDimension.UsageTimeType)
                raw = this.reportDao.ReportEquipmentExpenditureByUsageTime(year, month);
            else
            {
                DataTable count = null;
                count = this.reportDao.ReportEquipmentExpenditure(type, year, month);
                string axisX = null;

                foreach (DataRow dr in count.Rows)
                {
                    axisX = GetAxisX(type, dr);
                    raw.Add(axisX, SQLUtil.ConvertDouble(dr[1]));
                }
            }

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double>> result = FillMissingAxis(type, raw, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 总支出同比报表(type,cur,last,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>总支出同比统计</returns>
        public List<Tuple<string, double, double, double>> ReportExpenditureRatio(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double>> resultCur = PartExpenditureReport(type, year, month, false);
            List<Tuple<string, double>> resultLast = PartExpenditureReport(type, year - 1, month, false);

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double, double, double>> result = GetVSRatio(type, resultCur, resultLast, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 设备总收入报表(type,amount)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>设备总收入统计</returns>
        public List<Tuple<string, double>> ReportEquipmentIncome(int type, int year = 0, int month = 0, bool limitRecord = false)
        {
            Dictionary<string, double> raw = new Dictionary<string, double>();
            if ( type == ReportDimension.AcceptanceMonth)
                raw = this.reportDao.ReportEquipmentIncomeByAcceptanceDate(year);
            else if(type == ReportDimension.AcceptanceYear){
                raw = this.reportDao.ReportEquipmentIncomeByAcceptanceDate();
                if (raw.Count != 0)
                    year = SQLUtil.ConvertInt(raw.Keys.Min());
                if (year == 0) year = DateTime.Today.Year;
            }
            else if (type == ReportDimension.AmountType)
                raw = this.reportDao.ReportEquipmentIncomeByPurchaseAmount(year, month);
            else if (type == ReportDimension.UsageTimeType)
                raw = this.reportDao.ReportEquipmentIncomeByUsageTime(year, month);
            else
            {
                DataTable count = null;
                count = this.reportDao.ReportEquipmentIncome(type, year, month);
                string axisX = null;
                foreach (DataRow dr in count.Rows)
                {
                    axisX = GetAxisX(type, dr);

                    raw.Add(axisX, SQLUtil.ConvertDouble(dr[1]));
                }
            }

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double>> result = FillMissingAxis(type, raw, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 设备总收入同比报表(type,cur,last,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>设备总收入同比统计</returns>
        public List<Tuple<string, double, double, double>> ReportEquipmentIncomeRatio(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double>> resultCur = ReportEquipmentIncome(type, year, month, false);
            List<Tuple<string, double>> resultLast = ReportEquipmentIncome(type, year - 1, month, false);
            List<string> axisXList = GetAxisXList(type, year);

            List<Tuple<string, double, double, double>> result = GetVSRatio(type, resultCur, resultLast, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 收支比报表(type,income,expenditure,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>设备收支比统计</returns>
        public List<Tuple<string, double, double, double>> IncomeRatioExpenditure(int type, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double>> income = ReportEquipmentIncome(type, year, month);
            List<Tuple<string, double>> expenditure = PartExpenditureReport(type, year, month);
            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double, double, double>> result = GetVSRatio(type, income, expenditure, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        
        /// <summary>
        /// 请求数量(type,requestType)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>请求数量统计</returns>
        public List<Tuple<string, double>> RequestCount(int type, int requestType = 0, int status = 0, int year = 0, int month = 0, bool limitRecord = false)
        {
            Dictionary<string,double> raw = new Dictionary<string,double>();
            switch(type){
                case ReportDimension.AcceptanceYear:
                    raw = this.reportDao.RequestCountByDate(requestType, status);
                    if (raw.Count != 0)
                        year = SQLUtil.ConvertInt(raw.Keys.Min());
                    if (year == 0) year = DateTime.Today.Year;
                    break;
                case ReportDimension.AcceptanceMonth:
                    raw = this.reportDao.RequestCountByDate(requestType, status, year);
                    break;
                case ReportDimension.AmountType:
                    raw = this.reportDao.RequestCountByPurchaseAmount(requestType, status, year, month);
                    break;
                case ReportDimension.UsageTimeType:
                    raw = this.reportDao.RequestCountByUsageTime(requestType, status, year, month);
                    break;
                default:
                    DataTable dtCount = this.reportDao.ReportRequestCount(type,requestType,status,year,month);
                    
                    string axisX = null;
                    foreach (DataRow dr in dtCount.Rows)
                    {
                        axisX = GetAxisX(type, dr);
                        raw.Add(axisX, SQLUtil.ConvertInt(dr[1]));
                    }
                    break;
            }

            List<string> axisXList = GetAxisXList(type,year);
            List<Tuple<string, double>> result = FillMissingAxis(type, raw, axisXList);
            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 请求比率(type,statusRequestCount,totalCount,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>请求比率统计</returns>
        public List<Tuple<string, double, double, double>> RequestRatio(int type, int requestType, int status, int year = 0, int month = 0, bool limitRecord = false)
        {

            List<Tuple<string, double>> resultByStatus = RequestCount(type, requestType, status, year, month, false);
            List<Tuple<string, double>> resultTotle = RequestCount(type, requestType, 0, year, month, false);

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double, double, double>> result = GetVSRatio(type, resultByStatus, resultTotle, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 请求数量增长率报表(type,cur,last,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>请求数量增长率</returns>
        public List<Tuple<string, double, double, double>> RequestGrowthRatioReport(int type, int requestType, int status, int year, int month, bool limitRecord = false)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            List<Tuple<string, double>> resultCur = RequestCount(type, requestType, status, year, month, false);
            List<Tuple<string, double>> resultLast = RequestCount(type, requestType, status, year - 1, month, false);

            List<string> axisList = GetAxisXList(type);
            result = FillMissingAxis(type, resultCur, resultLast, axisList);
           
            return GetTopManufacturer(type, result, limitRecord);
        }

        /// <summary>
        /// 供应商保养/维修数量(type,requestCount)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>供应商保养/维修数量</returns>
        public List<Tuple<string, double>> ResultCount(int type, int requestType, int year, int month = 0, bool limitRecord = false)
        {
            Dictionary<string, double> raw = new Dictionary<string, double>();
            switch (type)
            {
                case ReportDimension.AcceptanceYear:
                    raw = this.reportDao.RequestCount_supplierByDate(requestType, year);
                    if (raw.Count != 0)
                        year = SQLUtil.ConvertInt(raw.Keys.Min());
                    if (year == 0) year = DateTime.Today.Year;
                    break;
                case ReportDimension.AcceptanceMonth:
                    raw = this.reportDao.RequestCount_supplierByDate(requestType, year);
                    break;
                case ReportDimension.AmountType:
                    raw = this.reportDao.RequestCount_supplierByPurchaseAmount(requestType,  year, month);
                    break;
                case ReportDimension.UsageTimeType:
                    raw = this.reportDao.RequestCount_supplierByUsageTime(requestType, year, month);
                    break;
                default:
                    DataTable dtCount = this.reportDao.RequestCount_supplier(type, requestType, year, month);
                    
                    string axisX = null;
                    foreach (DataRow dr in dtCount.Rows)
                    {
                        axisX = GetAxisX(type, dr);
                        raw.Add(axisX, SQLUtil.ConvertInt(dr[1]));
                    }
                    break;
            }

            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double>> result = FillMissingAxis(type, raw, axisXList);

            result = GetTopManufacturer(type, result, limitRecord);
            return result;
        }
        /// <summary>
        /// 内部保养/维修数量(type,requestCount)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>内部保养/维修数量</returns>
        public List<Tuple<string, double>> SelfResultCount(int type, int requestType, int year, int month = 0, bool limitRecord = false)
        {
            List<Tuple<string, double>> result = new List<Tuple<string, double>>();
            List<Tuple<string, double>> BySupplier = ResultCount(type, requestType, year, month, false);
            List<Tuple<string, double>> total = RequestCount(type, requestType, 0, year, month, false);
            if(type == ReportDimension.AcceptanceYear) year = SQLUtil.ConvertInt(total[0].Item1);
            List<string> axisXList = GetAxisXList(type, year);
            foreach (string axisX in axisXList)
            {
                Tuple<string,double> supplierInfo = BySupplier.SingleOrDefault(t => t.Item1.Equals(axisX));
                Tuple<string,double> totalInfo = total.SingleOrDefault(t => t.Item1.Equals(axisX));
                if (supplierInfo != null && totalInfo != null)
                {
                    result.Add(new Tuple<string, double>(axisX,Math.Round(totalInfo.Item2 - supplierInfo.Item2,2)));
                }
                else if(supplierInfo == null && totalInfo != null)
                {
                    result.Add(new Tuple<string, double>(axisX, totalInfo.Item2));
                }
                else
                {
                    if(type != ReportDimension.EquipmentType && type != ReportDimension.ManufacturerType)
                        result.Add(new Tuple<string, double>(axisX, 0));
                }
            }

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 供应商维修/保养率(type,requestCount,totalCount,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>供应商维修/保养率统计</returns>
        public List<Tuple<string, double, double, double>> ResultRatio_supplier(int type, int requestType, int year, int month = 0, bool limitRecord = false)
        {
            List<Tuple<string, double>> supplierResult = ResultCount(type, requestType, year, month, false);
            List<Tuple<string, double>> totalResult = RequestCount(type, requestType, 0, year, month, false);

            List<string> axisXList = GetAxisXList(type, year);

            List<Tuple<string, double, double, double>> result = GetVSRatio(type, supplierResult, totalResult, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 自修/保养率(type,requestCount,totalCount,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>自修/保养率</returns>
        public List<Tuple<string, double, double, double>> ResultRatio_self(int type, int requestType, int year, int month = 0, bool limitRecord = false)
        {
            List<string> axisXList = GetAxisXList(type, year);
            List<Tuple<string, double, double, double>> resultSupplier = ResultRatio_supplier(type, requestType, year, month,false);
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            foreach (string axisX in axisXList)
            {
                Tuple<string, double, double, double> curInfo = resultSupplier.SingleOrDefault(t => t.Item1.Equals(axisX));
                if (curInfo != null)
                    result.Add(new Tuple<string, double, double, double>(curInfo.Item1, Math.Round(curInfo.Item3 - curInfo.Item2,2), curInfo.Item3, Math.Round(100 - curInfo.Item4,2)));
                else
                {
                    if(type != ReportDimension.EquipmentType && type != ReportDimension.ManufacturerType)
                        result.Add(new Tuple<string, double, double, double>(axisX, 0, 0, 0));
                }
            }

            return GetTopManufacturer(type, result, limitRecord);
        }
        /// <summary>
        /// 派工响应时间报表(type,dispatchCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工响应时间统计</returns>
        public List<Tuple<string,double>> ResponseDispatchTime(int year, int month)
        {
            Dictionary<string,double> raw = this.reportDao.QueryResponseDispatchTime(year, month);
            List<string> axisXList = SQLUtil.GetStringListFromObjectList(ReportDimension.GetDispatchResponseTimeList(), "Name");
            List<Tuple<string, double>> result = FillMissingAxis(0, raw, axisXList);

            return result;
        }
        /// <summary>
        /// 派工执行率报表(type,approveCount,totalDispatchCount,ratio)
        /// </summary>
        /// <param name="type">维度</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>派工执行率统计</returns>
        public List<Tuple<string, double, double, double>> DispatchResponseRatio(int type, int year, int month, bool limitRecord)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            Dictionary<string, double> approved = new Dictionary<string, double>();
            Dictionary<string, double> total = new Dictionary<string, double>();
            switch (type)
            {
                case ReportDimension.AmountType:
                    total = this.reportDao.DispatchResponseCountByPurchaseAmount(0, year, month);
                    approved = this.reportDao.DispatchResponseCountByPurchaseAmount(DispatchInfo.Statuses.Approved, year, month);
                    break;
                case ReportDimension.UsageTimeType:
                    total = this.reportDao.DispatchResponseCountByUsageTime(0, year, month);
                    approved = this.reportDao.DispatchResponseCountByUsageTime(DispatchInfo.Statuses.Approved, year, month);
                    break;
                default:
                    DataTable dtTotal = this.reportDao.DispatchResponseCount(type, 0, year, month);
                    DataTable dtResponse = this.reportDao.DispatchResponseCount(type, DispatchInfo.Statuses.Approved, year, month);
                    string axisX = null;
                    foreach (DataRow dr in dtTotal.Rows)
                    {
                        axisX = GetAxisX(type, dr);
                        total.Add(axisX, SQLUtil.ConvertInt(dr[1]));
                    }
                    foreach (DataRow dr in dtResponse.Rows)
                    {
                        axisX = GetAxisX(type, dr);
                        approved.Add(axisX, SQLUtil.ConvertInt(dr[1]));
                    }
                    break;
            }
            List<string> axisXList = GetAxisXList(type, year);

            result = GetVSRatio(type, approved, total, axisXList);

            return GetTopManufacturer(type, result, limitRecord);
        }
        
        /// <summary>
        /// 服务时间达标率报表(type,overdueCount,requestCount)
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="limitRecord">是否厂商维度取前10</param>
        /// <returns>服务时间达标率统计</returns>
        public List<Tuple<string, double, double, double>> ServicePassRatio(int year, bool limitRecord)
        {
            List<Tuple<string, double, double, double>> result = new List<Tuple<string, double, double, double>>();
            Dictionary<string, double> overdue = this.reportDao.ServiceCountByDate(true, year);
            Dictionary<string, double> total = this.reportDao.ServiceCountByDate(false, year);
            List<string> axisXList;
            if (year > 0)
            {
                 axisXList = GetAxisXList(ReportDimension.AcceptanceMonth, year);
            }else{
                int yearMin = 0;
                if (total.Count != 0)
                    yearMin = SQLUtil.ConvertInt(total.Keys.Min());
                if (yearMin == 0) yearMin = DateTime.Today.Year;
                axisXList = GetAxisXList(ReportDimension.AcceptanceYear, yearMin);
            }
            
            foreach (var axisX in axisXList)
            {
                if (overdue.ContainsKey(axisX) && total.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, Math.Round(total[axisX] - overdue[axisX],2), total[axisX], GetVSRatio(total[axisX] - overdue[axisX], total[axisX])));
                else if (overdue.ContainsKey(axisX) && !total.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, overdue[axisX], 0, 100.0));
                else if (!overdue.ContainsKey(axisX) && total.ContainsKey(axisX))
                    result.Add(new Tuple<string, double, double, double>(axisX, total[axisX], total[axisX], 100));
                else
                {
                    result.Add(new Tuple<string, double, double, double>(axisX, 0, 0,100.0));
                }
            }

            return result;
        }
    
    }
}
