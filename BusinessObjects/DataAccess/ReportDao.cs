using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 报表dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ReportDao : BaseDao
    {
        /// <summary>
        /// 获取设备数量报表验收日期sql语言
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备数量报表验收日期sql语言</returns>
        private string GetAcceptanceDateFilter(int year = 0, int month = 0)
        {
            string sqlStr = "";
            if (year > 0)
            {
                if (month > 0)
                {
                    if (year == DateTime.Today.Year)
                        sqlStr += " AND (e.AcceptanceDate IS NULL OR DATEPART(YEAR,e.AcceptanceDate) < " + year + " OR (DATEPART(YEAR,e.AcceptanceDate) = " + year + " AND DATEPART(MONTH,e.AcceptanceDate) <= " + month + "))";
                    else
                        sqlStr += " AND (DATEPART(YEAR,e.AcceptanceDate) < " + year + " OR (DATEPART(YEAR,e.AcceptanceDate) = " + year + " AND DATEPART(MONTH,e.AcceptanceDate) <= " + month + "))";
                }
                else
                {
                    if (year == DateTime.Today.Year)
                        sqlStr += " AND (e.AcceptanceDate IS NULL OR DATEPART(YEAR,e.AcceptanceDate) <= " + year + ")";
                    else
                        sqlStr += " AND DATEPART(YEAR,e.AcceptanceDate) <= " + year;
                }

                sqlStr += " AND (e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap + " OR DATEPART(YEAR,e.ScrapDate) > " + year + ")";
            }
            else
            {
                sqlStr += " AND e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap;
            }

            return sqlStr;
        }

        #region 设备数量
        /// <summary>
        /// 获取设备最早验收年份
        /// </summary>
        /// <returns>设备最早验收年份</returns>
        public int ReportEquipmentCountMinYear()
        {
            sqlStr = "SELECT Min(DATEPART(YEAR,e.AcceptanceDate)) FROM tblEquipment e";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取设备各个资产类型的数量
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备各个资产类型的数量</returns>
        public Dictionary<string, double> ReportEquipmentCountByPurchaseAmount(int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + ", COUNT(e.ID) " +
                    " FROM tblEquipment e" +
                    " WHERE 1 = 1 ";

            sqlStr += GetAcceptanceDateFilter(year, month);

            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 获取设备各个设备年限的数量
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备各个设备年限的数量</returns>
        public Dictionary<string, double> ReportEquipmentCountByUsageTime(int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + ", COUNT(e.ID) " +
                    " FROM tblEquipment e " +
                    " WHERE 1 = 1 ";

            sqlStr += GetAcceptanceDateFilter(year, month);

            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型获取设备数量报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备数量报表信息</returns>
        public DataTable ReportEquipmentCount(int type, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ", COUNT(e.ID) " +
                    " FROM tblEquipment e" +
                    " WHERE 1 = 1";

            sqlStr += GetAcceptanceDateFilter(year, month);

            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);

            if (type == ReportDimension.ManufacturerType)
                sqlStr += " ORDER BY COUNT(e.ID) DESC";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        #endregion

        #region 设备总数
        /// <summary>
        /// 根据年月获取设备总数
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总数</returns>
        public int EquipmentCount(int year, int month)
        {
            sqlStr = "SELECT COUNT(e.ID) " +
                " FROM tblEquipment e " +
                " WHERE 1 = 1 ";
            if (month > 0)
            {
                sqlStr += "AND (DATEPART(YEAR,e.AcceptanceDate) < " + year + " OR " +
                            " (DATEPART(YEAR,e.AcceptanceDate) = " + year + " AND DATEPART(MONTH,e.AcceptanceDate) <= " + month + ")" +
                           ")";
                sqlStr += " AND (e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap + " OR (DATEPART(YEAR,e.ScrapDate) > " + year + "))";
            }
            else
            {
                if (year == DateTime.Today.Year)
                    sqlStr += " AND (e.AcceptanceDate IS NULL OR DATEPART(YEAR,e.AcceptanceDate) <= " + year + ")";
                else
                    sqlStr += " AND DATEPART(YEAR,e.AcceptanceDate) <= " + year;
                sqlStr += " AND (e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap + " OR DATEPART(YEAR,e.ScrapDate) > " + year + ")";
            }

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetCount(command);
            }
        }
        #endregion

        #region 故障时间
        /// <summary>
        /// 获取设备故障时间（天）报表信息
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>设备故障时间（天）报表信息</returns>
        public Dictionary<string, double> ReportEquipmentRepairTimeDay(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = " SELECT " + ReportDimension.GetRepairDaysSql("Hours") + ", count(ID)" +
                    " From (";
            sqlStr += " SELECT e.ID, SUM(" + ReportDimension.SqlRepairTimeHours + ") as Hours " +
                    " FROM tblRequest r " +
                    " INNER JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " INNER JOIN tblEquipment e ON j.EquipmentID = e.ID " +
                    " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)" +
                    " GROUP BY e.ID";
            sqlStr += ") as temp" +
                    " GROUP BY " + ReportDimension.GetRepairDaysSql("Hours");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;

                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 获取设备故障时间（小时）报表信息
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>设备故障时间（小时）报表信息</returns>
        public Dictionary<string, double> ReportEquipmentRepairTimeHour(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = " SELECT " + ReportDimension.GetRepairHoursSql("Minutes") + ", count(ID)" +
                    " From (";
            sqlStr += " SELECT e.ID, SUM(" + ReportDimension.SqlRepairTimeMinutes + ") as Minutes" +
                    " FROM tblRequest r " +
                    " INNER JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " INNER JOIN tblEquipment e ON j.EquipmentID = e.ID " +
                    " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)" +
                    " GROUP BY e.ID";
            sqlStr += ") as temp" +
                    " GROUP BY " + ReportDimension.GetRepairHoursSql("Minutes");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;

                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 故障率
        /// <summary>
        /// 获取故障率报表最早年限
        /// </summary>
        /// <returns>故障率报表最早年限</returns>
        public int ReportEquipmentRepairTimeRatioMinYear()
        {
            sqlStr = "SELECT Min(DATEPART(YEAR,RequestDate)) FROM tblRequest" +
                     " WHERE RequestType = " + RequestInfo.RequestTypes.Repair +
                     " AND StatusID <> " + RequestInfo.Statuses.Cancelled;

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetCount(command);
            }
        }
        /// <summary>
        /// “时间”维度获取设备故障时间
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>设备故障时间</returns>
        public int ReportEquipmentRepairTimeRatioByPurchaseDate(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = "SELECT SUM(" + ReportDimension.SqlRepairTimeHours + ")" +
                    " FROM tblRequest r " +
                    " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取设备各个资产类型故障时间
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>设备各个资产类型故障时间</returns>
        public Dictionary<string, double> ReportEquipmentRepairTimeRatioByPurchaseAmount(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = " SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + ", SUM(" + ReportDimension.SqlRepairTimeHours + ")" +
                     " FROM tblRequest r " +
                     " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                     " LEFT JOIN tblEquipment e ON j.EquipmentID = e.ID ";
            sqlStr += " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)";
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;

                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 获取设备各个设备年限故障时间
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>设备各个设备年限故障时间</returns>
        public Dictionary<string, double> ReportEquipmentRepairTimeRatioByUsageTime(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = " SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType)) + " , SUM(" + ReportDimension.SqlRepairTimeHours + ") " +
                     " FROM tblRequest r " +
                     " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                     " LEFT JOIN tblEquipment e ON j.EquipmentID = e.ID ";
            sqlStr += " WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)";
            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;

                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型获取设备故障率报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="dateFrom">年份</param>
        /// <param name="dateTo">月份</param>
        /// <returns>设备故障率报表信息</returns>
        public DataTable ReportEquipmentRepairTimeRatio(int type, DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = " SELECT " + ReportDimension.GetFieldDesc(type) + ", SUM(" + ReportDimension.SqlRepairTimeHours + ")" +
                     " FROM tblRequest r " +
                     " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                     " LEFT JOIN tblEquipment e ON j.EquipmentID = e.ID " ;
            sqlStr += "WHERE r.RequestType = " + RequestInfo.RequestTypes.Repair +
                    " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled +
                    " AND r.RequestDate < @EndDate And (r.CloseDate >= @StartDate OR r.CloseDate IS NULL)";
            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;
                return GetDataTable(command);
            }
        }
        #endregion

        #region 服务合格率
        /// <summary>
        /// 根据年份获取服务合格率报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>服务合格率报表信息</returns>
        public List<Tuple<double, double, double, double>> QueryRequestFinishedRate(int year)
        {
            List<Tuple<double, double, double, double>> count = new List<Tuple<double, double, double, double>>();
            string monthOrYear = (year > 0) ? "MONTH" : "YEAR";

            sqlStr = "SELECT DATEPART({0},r.RequestDate), COUNT(r.ID), ";
            if (year > 0)
                sqlStr += " SUM(CASE WHEN r.StatusID = " + RequestInfo.Statuses.Close + " AND DATEPART(YEAR,r.RequestDate) = DATEPART(YEAR,r.CloseDate) AND DATEPART(MONTH,r.RequestDate) = DATEPART(MONTH,r.CloseDate) THEN 1 ELSE 0 END)  ";
            else
                sqlStr += " SUM(CASE WHEN r.StatusID = " + RequestInfo.Statuses.Close + " AND DATEPART(YEAR,r.RequestDate) = DATEPART(YEAR,r.CloseDate) THEN 1 ELSE 0 END) ";

            sqlStr += " FROM tblRequest r " +
                      " WHERE r.StatusID <>" + RequestInfo.Statuses.Cancelled;
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR ,r.RequestDate) = " + year;
            sqlStr += " GROUP BY DATEPART({0}, r.RequestDate)";

            sqlStr = string.Format(sqlStr, monthOrYear);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        count.Add(new Tuple<double, double, double, double>(SQLUtil.ConvertInt(dr[0]), SQLUtil.ConvertDouble(dr[1]), SQLUtil.ConvertDouble(dr[2]), SQLUtil.ConvertDouble(dr[1]) == 0 ? 0 : SQLUtil.ConvertDouble(Math.Round(SQLUtil.ConvertDouble(dr[2]) * 100 / SQLUtil.ConvertDouble(dr[1]), 2))));
                    }
                };
            }
            return count;
        }
        #endregion

        #region 请求响应时间
        /// <summary>
        /// 获取请求响应时间报表信息
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求响应时间报表信息</returns>
        public Dictionary<string, double> ResponseTime(int requestType, int year, int month)
        {
            sqlStr = "SELECT " + ReportDimension.GetResponseTimeSql() + " 'type',COUNT(r.ID) sum " +
                    " FROM tblRequest r " +
                    " WHERE r.RequestType =  " + requestType;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            sqlStr += " GROUP BY " + ReportDimension.GetResponseTimeSql();

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 设备采购价格
        /// <summary>
        /// 获取设备采购价格报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备采购价格报表信息</returns>
        public Dictionary<string, double> EquipmentCountByPurchaseAmount(int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " 'price', COUNT(e.ID) " +
                    " FROM tblEquipment e" +
                    " WHERE DATEPART(YEAR,e.AcceptanceDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,e.AcceptanceDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }

        #endregion

        #region 合同金额
        /// <summary>
        /// 获取服务合同金额报表信息
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>服务合同金额报表信息</returns>
        public Dictionary<string, double> ReportContractAmount(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = "SELECT " + ReportDimension.GetContractAmountSql("c.Amount") + " as type,COUNT(c.ID)" +
                    " FROM tblContract c  " +
                    " WHERE (c.StartDate between @StartDate And @EndDate) OR (c.EndDate between @StartDate and @EndDate) OR (c.StartDate < @StartDate And c.EndDate > @EndDate)" +
                    " GROUP BY " + ReportDimension.GetContractAmountSql("c.Amount");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;
                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 合同年限
        /// <summary>
        /// 获取服务合同年限报表信息
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <returns>服务合同年限报表信息</returns>
        public Dictionary<string, double> ReportContractMonths(DateTime dateFrom, DateTime dateTo)
        {
            sqlStr = "SELECT " + ReportDimension.GetContractMonthSql("DATEDIFF(MONTH,GETDATE(), c.EndDate)") + " x,COUNT(c.ID)" +
                    " FROM tblContract c " +
                    " WHERE (c.StartDate between @StartDate And @EndDate) OR (c.EndDate between @StartDate and @EndDate) OR (c.StartDate < @StartDate And c.EndDate > @EndDate)" +
                    " GROUP BY " + ReportDimension.GetContractMonthSql("DATEDIFF(MONTH,GETDATE(), c.EndDate)");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = dateFrom;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = dateTo;
                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 设备剩余折旧年限
        /// <summary>
        /// 获取设备剩余折旧年限报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备剩余折旧年限报表信息</returns>
        public Dictionary<string, double> ReportDepreciationYears(int year, int month)
        {
            sqlStr = "SELECT " + ReportDimension.GetDepreciationYearsSql("@ReportDate") + " type,count(e.ID)" +
                    " From tblEquipment e " +
                    " WHERE e.DepreciationYears > 0 AND e.AcceptanceDate <= @ReportDate And e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap +
                    " GROUP BY " + ReportDimension.GetDepreciationYearsSql("@ReportDate");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ReportDate", SqlDbType.DateTime).Value = new DateTime(year, month == 0 ? 12 : month, 1).AddMonths(1).AddDays(-1);

                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 设备折旧率
        /// <summary>
        /// 获取设备折旧率限报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备折旧率限报表信息</returns>
        public Dictionary<string, double> ReportDepreciationRate(int year, int month)
        {
            sqlStr = " SELECT " + ReportDimension.GetDepreciationRationSql("@ReportDate") + " type,count(e.ID)" +
                    " FROM tblEquipment e " +
                    " WHERE e.DepreciationYears > 0 AND e.AcceptanceDate <= @ReportDate And e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap +
                    " GROUP BY " + ReportDimension.GetDepreciationRationSql("@ReportDate");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ReportDate", SqlDbType.DateTime).Value = new DateTime(year, month == 0 ? 12 : month, 1).AddMonths(1).AddDays(-1);

                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 设备检查人次
        /// <summary>
        /// 根据年份获取每月设备检查人次报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>按月份统计设备检查人次报表信息</returns>
        public Dictionary<string, double> ReportServiceCountByAcceptanceMonth(int year)
        {
            sqlStr = "SELECT DATEPART(month,s.TransDate) ,count(ID) " +
                    " FROM tblServiceHis AS s" +
                    " WHERE DATEPART(year,s.TransDate) = " + year +
                    " GROUP BY DATEPART(month,s.TransDate)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 获取每年的设备检查人次报表信息
        /// </summary>
        /// <returns>每年的设备检查人次报表信息</returns>
        public Dictionary<string, double> ReportServiecCountByAcceptanceYear()
        {
            sqlStr = "SELECT DATEPART(year,s.TransDate), count(ID) " +
                    " FROM tblServiceHis AS s" +
                    " GROUP BY DATEPART(year,s.TransDate)";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据资产类型获取设备检查人次报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>设备检查人次报表信息</returns>
        public Dictionary<string, double> ReportServiceCountByPurchaseAmount(int year)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " 'price', count(s.ID) emps " +
                     " FROM tblServiceHis AS s " +
                     " LEFT JOIN tblEquipment AS e ON e.ID = s.EquipmentID";
            if (year > 0) sqlStr += " WHERE DATEPART(year,s.TransDate) = " + year;
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备年限获取设备检查人次报表信息
        /// </summary>
        /// <param name="year">年限</param>
        /// <returns>设备检查人次报表信息</returns>
        public Dictionary<string, double> ReportServiceCountByDepreciationYears(int year)
        {
            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + " 'price', count(s.ID) emps " +
                     " FROM tblServiceHis AS s " +
                     " LEFT JOIN tblEquipment AS e ON e.ID = s.EquipmentID";
            if (year > 0) sqlStr += " WHERE DATEPART(year,s.TransDate) = " + year;
            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型获取设备检查人次报表信息
        /// </summary>
        /// <param name="typeID">维度类型ID</param>
        /// <param name="year">年份</param>
        /// <returns>设备检查人次报表信息</returns>
        public DataTable ReportServiceCount(int typeID, int year)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(typeID) + ",Count(s.ID) as count" +
                     " FROM tblServiceHis AS s " +
                     " LEFT JOIN tblEquipment AS e ON e.ID = s.EquipmentID" ;
            if (year > 0) sqlStr += " WHERE DATEPART(year,s.TransDate) = " + year;
            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(typeID);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }

        #endregion

        #region 设备检查收入
        /// <summary>
        /// 获取设备检查收入报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备检查收入报表信息</returns>
        public Dictionary<string, double> ReportEquipmentCountByIncome(int year, int month)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql("s.Income") + " 'price', COUNT(s.EquipmentID) " +
                    " FROM (SELECT EquipmentID, SUM(Income) as Income " +
                           " FROM tblServiceHis " +
                           " WHERE DATEPART(YEAR, TransDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,TransDate ) = " + month;
            sqlStr += " GROUP BY EquipmentID) AS s " +
                      " GROUP BY " + ReportDimension.GetAmountSql("s.Income");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 零配件/备件花费
        /// <summary>
        /// 根据年份获取每月设备零配件花费总额报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>每月设备零配件花费总额报表信息</returns>
        public Dictionary<string, double> ReportEquipmentExpenditureByAcceptanceMonth(int year)
        {
            sqlStr = "SELECT DATEPART(MONTH,di.ScheduleDate) ,SUM(ra.Qty * ra.Amount) " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID " +
                    " LEFT JOIN tblDispatch di ON dr.DispatchID = di.ID " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID = di.RequestID " +
                    " LEFT JOIN tblRequest r ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON e.ID = re.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.OnSiteInspection +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.Inventory +
                    " AND dr.StatusID = " + DispatchInfo.DocStatus.Approved +
                    " AND DATEPART(YEAR,di.ScheduleDate) = " + year +
                    " GROUP BY DATEPART(MONTH,di.ScheduleDate)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 获取每年设备零配件花费总额
        /// </summary>
        /// <returns>每年设备零配件花费总额</returns>
        public Dictionary<string, double> ReportEquipmentExpenditureByAcceptanceYear()
        {
            sqlStr = "SELECT DATEPART(YEAR,di.ScheduleDate), SUM(ra.Qty * ra.Amount) " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID " +
                    " LEFT JOIN tblDispatch di ON dr.DispatchID = di.ID " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID = di.RequestID " +
                    " LEFT JOIN tblRequest r ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON e.ID = re.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.OnSiteInspection +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.Inventory +
                    " AND dr.StatusID = " + DispatchInfo.DocStatus.Approved +
                    " GROUP BY DATEPART(YEAR,di.ScheduleDate)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据资产类型获取设备零配件花费总额报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备零配件花费总额报表信息</returns>
        public Dictionary<string, double> ReportEquipmentExpenditureByPurchaseAmount(int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " 'price', SUM(ra.Qty * ra.Amount) emps " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID " +
                    " LEFT JOIN tblDispatch di ON dr.DispatchID = di.ID " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID = di.RequestID " +
                    " LEFT JOIN tblRequest r ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON e.ID = re.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.OnSiteInspection +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.Inventory +
                    " AND dr.StatusID = " + DispatchInfo.DocStatus.Approved;

            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,di.ScheduleDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,di.ScheduleDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备年限获取设备零配件花费总额报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备零配件花费总额报表信息</returns>
        public Dictionary<string, double> ReportEquipmentExpenditureByUsageTime(int year = 0, int month = 0)
        {
            sqlStr = " SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType)) + " 'type', SUM(ra.Qty * ra.Amount) " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID " +
                    " LEFT JOIN tblDispatch di ON dr.DispatchID = di.ID " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID = di.RequestID " +
                    " LEFT JOIN tblRequest r ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON e.ID = re.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.OnSiteInspection +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.Inventory +
                    " AND dr.StatusID = " + DispatchInfo.DocStatus.Approved;

            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,di.ScheduleDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,di.ScheduleDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型获取设备零配件花费总额报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备零配件花费总额报表信息</returns>
        public DataTable ReportEquipmentExpenditure(int type, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ",SUM(ra.Qty * ra.Amount) " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID " +
                    " LEFT JOIN tblDispatch di ON dr.DispatchID = di.ID " +
                    " LEFT JOIN jctRequestEqpt re ON re.RequestID = di.RequestID " +
                    " LEFT JOIN tblRequest r ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON e.ID = re.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.OnSiteInspection +
                    " AND di.RequestType <> " + RequestInfo.RequestTypes.Inventory +
                    " AND dr.StatusID = " + DispatchInfo.DocStatus.Approved;
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,di.ScheduleDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,di.ScheduleDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        #endregion

        #region 设备总收入
        /// <summary>
        /// 获取每年/每月设备总收入报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>每年/每月设备总收入报表信息</returns>
        public Dictionary<string, double> ReportEquipmentIncomeByAcceptanceDate(int year = 0)
        {
            sqlStr = "SELECT DATEPART({0},seh.TransDate) ,SUM(seh.Income) " +
                    " FROM tblServiceHis seh" +
                    " LEFT JOIN tblEquipment e ON seh.EquipmentID = e.ID " +
                    " WHERE 1=1 ";
            if (year > 0) sqlStr += " AND DATEPART(YEAR,seh.TransDate) = " + year;
            sqlStr += " GROUP BY DATEPART({0},seh.TransDate)";

            sqlStr = string.Format(sqlStr, year > 0 ? "MONTH" : "YEAR");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据资产类型获取设备总收入报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总收入报表信息</returns>
        public Dictionary<string, double> ReportEquipmentIncomeByPurchaseAmount(int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " 'price', SUM(seh.Income) " +
                    " FROM tblServiceHis seh" +
                    " LEFT JOIN tblEquipment e ON seh.EquipmentID = e.ID " +
                    " WHERE 1=1 ";
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,seh.TransDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,seh.TransDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备年限获取设备总收入报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总收入报表信息</returns>
        public Dictionary<string, double> ReportEquipmentIncomeByUsageTime(int year = 0, int month = 0)
        {

            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + " 'price', SUM(seh.Income) " +
                 " FROM tblServiceHis seh" +
                 " LEFT JOIN tblEquipment e ON seh.EquipmentID = e.ID " +
                 " WHERE 1=1 ";
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,seh.TransDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,seh.TransDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型获取设备总收入报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备总收入报表信息</returns>
        public DataTable ReportEquipmentIncome(int type, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ",SUM(seh.Income) " +
                    " FROM tblServiceHis seh" +
                    " LEFT JOIN tblEquipment e ON seh.EquipmentID = e.ID " ;

            sqlStr += " WHERE 1=1 ";
            if (year > 0)
                sqlStr += " AND DATEPART(YEAR,seh.TransDate) = " + year;
            if (month > 0)
                sqlStr += " AND DATEPART(MONTH,seh.TransDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }

        #endregion

        #region 请求数量
        /// <summary>
        /// 根据请求状态返回sql语句
        /// </summary>
        /// <param name="status">请求状态</param>
        /// <returns>sql语句</returns>
        private string GetRequestStatusSql(int status)
        {
            switch (status)
            {
                case RequestInfo.ReportStatus.Unclosed:
                    return " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled + " AND r.StatusID <> " + RequestInfo.Statuses.Close;
                case RequestInfo.ReportStatus.Unresponsed:
                    return " AND r.StatusID = " + RequestInfo.Statuses.New;
                case RequestInfo.ReportStatus.Closed:
                    return " AND r.StatusID = " + RequestInfo.Statuses.Close;
                case RequestInfo.ReportStatus.Responsed:
                    return " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled + " AND r.StatusID <> " + RequestInfo.Statuses.New;
                default:
                    return " AND r.StatusID <> " + RequestInfo.Statuses.Cancelled;
            }
        }
        /// <summary>
        /// 根据请求类型返回sql语句
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <returns>sql语句</returns>
        private string GetRequestTypeSql(int requestType)
        {
            if (requestType != 0)
            {
                if (requestType == RequestInfo.RequestTypes.Recall)
                    return " AND r.IsRecall = 1 ";
                else
                    return " AND r.RequestType = " + requestType;
            }
            else return "";
        }
        /// <summary>
        /// 根据请求类型,请求状态和年份获取请求数量报表信息
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <returns>请求数量报表信息</returns>
        public Dictionary<string, double> RequestCountByDate(int requestType = 0, int status = 0, int year = 0)
        {
            sqlStr = "SELECT DATEPART({0},r.RequestDate), COUNT(r.ID)" +
                    " FROM tblRequest r " +
                    " WHERE 1=1 ";
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            sqlStr += GetRequestTypeSql(requestType);
            sqlStr += GetRequestStatusSql(status);
            sqlStr += " GROUP BY DATEPART({0},r.RequestDate)";

            sqlStr = string.Format(sqlStr, year > 0 ? "MONTH" : "YEAR");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据资产类型,请求类型,请求状态,年份和月份获取请求数量报表信息
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求数量报表信息</returns>
        public Dictionary<string, double> RequestCountByPurchaseAmount(int requestType, int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + ", COUNT(DISTINCT r.ID) " +
                " FROM tblRequest r " +
                " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                " WHERE 1=1 ";
            sqlStr += GetRequestTypeSql(requestType);
            sqlStr += GetRequestStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备年限,请求类型,请求状态,年份和月份获取请求数量报表信息
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求数量报表信息</returns>
        public Dictionary<string, double> RequestCountByUsageTime(int requestType, int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + " AS type, COUNT(DISTINCT r.ID) " +
                " FROM tblRequest r " +
                " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                " WHERE 1=1 ";
            sqlStr += GetRequestTypeSql(requestType);
            sqlStr += GetRequestStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型,请求类型,请求状态,年份和月份获取请求数量报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="status">请求状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>请求数量报表信息</returns>
        public DataTable ReportRequestCount(int type, int requestType = 0, int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ",COUNT(DISTINCT r.ID) " +
                    " FROM tblRequest r " +
                    " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " WHERE 1 = 1";

            sqlStr += GetRequestTypeSql(requestType);
            sqlStr += GetRequestStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);
            if (type == ReportDimension.ManufacturerType)
                sqlStr += " ORDER BY COUNT(r.ID) DESC";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }
        #endregion

        #region 派工响应时间
        /// <summary>
        /// 根据年份,月份获取派工响应时间报表信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工响应时间报表信息</returns>
        public Dictionary<string, double> QueryResponseDispatchTime(int year, int month)
        {
            sqlStr = " SELECT " + ReportDimension.GetDispatchResponseTimeSql() + " , COUNT(d.ID) " +
                    " FROM tblDispatch d " +
                    " WHERE d.StatusID <> @Cancelled " +
                    " AND d.RequestType <> " + RequestInfo.RequestTypes.Others;
            if (year > 0) sqlStr += " AND DATEPART(YEAR,d.CreateDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,d.CreateDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetDispatchResponseTimeSql();
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Cancelled", SqlDbType.Int).Value = DispatchInfo.Statuses.Cancelled;
                command.Parameters.Add("@New", SqlDbType.Int).Value = DispatchInfo.Statuses.New;

                using (DataTable dt = GetDataTable(command))
                {
                    return GetStringDoubleDictionary(command);
                };
            }
        }
        #endregion

        #region 派工执行率
        /// <summary>
        /// 根据派工单状态返回sql语句
        /// </summary>
        /// <param name="status">派工单状态</param>
        /// <returns>sql语句</returns>
        private string GetDispatchStatusSql(int status)
        {
            switch (status)
            {
                case DispatchInfo.Statuses.New:
                    return " AND di.StatusID = " + status + " AND di.StatusID <> " + DispatchInfo.Statuses.Cancelled;
                case DispatchInfo.Statuses.Responded:
                    return " AND di.Status.ID <> " + DispatchInfo.Statuses.New + " AND di.StatusID <> " + DispatchInfo.Statuses.Cancelled;
                case DispatchInfo.Statuses.Approved:
                    return " AND di.StatusID <> " + DispatchInfo.Statuses.Cancelled + " AND di.StatusID = " + DispatchInfo.Statuses.Approved;
                default:
                    return " AND di.StatusID <> " + DispatchInfo.Statuses.Cancelled;
            }
        }
        /// <summary>
        /// 根据资产类型,派工单状态,年份和月份获取派工执行率报表信息
        /// </summary>
        /// <param name="status">派工单状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工执行率报表信息</returns>
        public Dictionary<string, double> DispatchResponseCountByPurchaseAmount(int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " 'price', COUNT(DISTINCT di.ID) " +
                " FROM tblDispatch di " +
                " LEFT JOIN tblRequest r ON di.RequestID = r.ID " +
                " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others;
            sqlStr += GetDispatchStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,di.CreateDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,di.CreateDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据设备年限,派工单状态,年份和月份获取派工执行率报表信息
        /// </summary>
        /// <param name="status">派工单状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工执行率报表信息</returns>
        public Dictionary<string, double> DispatchResponseCountByUsageTime(int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + " AS type, COUNT(DISTINCT di.ID) " +
                " FROM tblDispatch di " +
                " LEFT JOIN tblRequest r ON di.RequestID = r.ID " +
                " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others;
            sqlStr += GetDispatchStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,di.CreateDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,di.CreateDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据维度类型,派工单状态,年份和月份获取派工执行率报表信息
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="status">派工单状态</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>派工执行率报表信息</returns>
        public DataTable DispatchResponseCount(int type, int status = 0, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ",COUNT(DISTINCT di.ID) " +
                    " FROM tblDispatch di " +
                    " LEFT JOIN tblRequest r ON di.RequestID = r.ID " +
                    " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " WHERE di.RequestType <> " + RequestInfo.RequestTypes.Others;
            sqlStr += GetDispatchStatusSql(status);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,di.CreateDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,di.CreateDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);
            if (type == ReportDimension.ManufacturerType)
                sqlStr += " ORDER BY COUNT(di.ID) DESC";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }

        #endregion

        #region 服务时间达标率
        /// <summary>
        /// 根据是否超期和年份获取每年/每月请求数量
        /// </summary>
        /// <param name="isOverdue">是否超期</param>
        /// <param name="year">年份</param>
        /// <returns>服务时间达标率报表信息</returns>
        public Dictionary<string, double> ServiceCountByDate(bool isOverdue, int year = 0)
        {
            sqlStr = "SELECT DATEPART({0},r.RequestDate), COUNT(r.ID) " +
                " FROM tblRequest r " +
                " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                " WHERE r.StatusID <>" + RequestInfo.Statuses.Cancelled +
                " AND r.RequestType = " + RequestInfo.RequestTypes.Repair;
            if (isOverdue)
                sqlStr += " AND " + RequestInfo.Statuses.GetHisOverDueSQL();

            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            sqlStr += " GROUP BY DATEPART({0},r.RequestDate)";
            sqlStr = string.Format(sqlStr, year > 0 ? "MONTH" : "YEAR");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        #endregion

        #region 供应商维修/保养
        /// <summary>
        /// 根据请求类型和年份获取设备供应商维修/保养数量
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <returns>设备供应商维修/保养数量</returns>
        public Dictionary<string, double> RequestCount_supplierByDate(int requestType, int year)
        {
            sqlStr = "SELECT DATEPART({0},r.RequestDate),COUNT(DISTINCT r.ID)" +
                    " FROM tblDispatchReport dr " +
                    " LEFT JOIN tblDispatch di ON di.ID = dr.DispatchID " +
                    " LEFT JOIN tblRequest r ON di.RequestID = r.ID" +
                    " WHERE dr.ServiceProvider = " + DispatchReportInfo.ServiceProviders.Third_Party +
                    " AND di.StatusID = " + DispatchInfo.Statuses.Approved;
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            sqlStr += GetRequestTypeSql(requestType);
            sqlStr += " GROUP BY DATEPART({0},r.RequestDate)";

            sqlStr = string.Format(sqlStr, year > 0 ? "MONTH" : "YEAR");

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据请求类型,资产类型,年份和月份获取设备供应商维修/保养数量
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备供应商维修/保养数量</returns>
        public Dictionary<string, double> RequestCount_supplierByPurchaseAmount(int requestType, int year, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType)) + " , COUNT(DISTINCT r.ID) " +
                    " FROM tblDispatchReport dr " +
                    " LEFT JOIN tblDispatch di ON di.ID = dr.DispatchID " +
                    " LEFT JOIN tblRequest r ON di.RequestID = r.ID" +
                    " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " WHERE dr.ServiceProvider = " + DispatchReportInfo.ServiceProviders.Third_Party +
                    " AND di.StatusID = " + DispatchInfo.Statuses.Approved;
            sqlStr += GetRequestTypeSql(requestType);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;
            sqlStr += " GROUP BY " + ReportDimension.GetAmountSql(ReportDimension.GetFieldDesc(ReportDimension.AmountType));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据请求类型,设备年限,年份和月份获取设备供应商维修/保养数量
        /// </summary>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备供应商维修/保养数量</returns>
        public Dictionary<string, double> RequestCount_supplierByUsageTime(int requestType, int year, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0) + " AS type, COUNT(DISTINCT e.ID) " +
                    " FROM tblDispatchReport dr " +
                    " LEFT JOIN tblDispatch di ON di.ID = dr.DispatchID " +
                    " LEFT JOIN tblRequest r ON di.RequestID = r.ID" +
                     " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " WHERE dr.ServiceProvider = " + DispatchReportInfo.ServiceProviders.Third_Party +
                    " AND di.StatusID = " + DispatchInfo.Statuses.Approved;
            sqlStr += GetRequestTypeSql(requestType);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetUsageTimeSql(ReportDimension.GetFieldDesc(ReportDimension.UsageTimeType), 0);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetStringDoubleDictionary(command);
            }
        }
        /// <summary>
        /// 根据请求类型,维度类型,年份和月份获取设备供应商维修/保养数量
        /// </summary>
        /// <param name="type">维度类型</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>设备供应商维修/保养数量</returns>
        public DataTable RequestCount_supplier(int type, int requestType, int year = 0, int month = 0)
        {
            sqlStr = "SELECT " + ReportDimension.GetFieldDesc(type) + ",COUNT(DISTINCT r.ID) " +
                    " FROM tblDispatchReport dr " +
                    " LEFT JOIN tblDispatch di ON di.ID = dr.DispatchID " +
                    " LEFT JOIN tblRequest r ON di.RequestID = r.ID" +
                    " LEFT JOIN jctRequestEqpt j ON j.RequestID = r.ID " +
                    " LEFT JOIN tblEquipment e ON e.ID = j.EquipmentID " +
                    " WHERE dr.ServiceProvider = " + DispatchReportInfo.ServiceProviders.Third_Party +
                    " AND di.StatusID = " + DispatchInfo.Statuses.Approved;
            sqlStr += GetRequestTypeSql(requestType);
            if (year > 0) sqlStr += " AND DATEPART(YEAR,r.RequestDate) = " + year;
            if (month > 0) sqlStr += " AND DATEPART(MONTH,r.RequestDate) = " + month;

            sqlStr += " GROUP BY " + ReportDimension.GetFieldDesc(type);
            if (type == ReportDimension.ManufacturerType)
                sqlStr += " ORDER BY COUNT(r.ID) DESC";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetDataTable(command);
            }
        }

        #endregion

    }
}
