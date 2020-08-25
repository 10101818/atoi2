using BusinessObjects.Aspect;
using BusinessObjects.Domain;
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
    /// 医院信息dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ServiceHisDao:BaseDao
    {

        /// <summary>
        /// 根据年份获取总服务人数  
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>总服务人数</returns>
        public int GetServiceCount(int year)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblServiceHis WHERE DATEPART(YEAR,TransDate) = @Year ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                return GetCount(command);
            }
        }

        /// <summary>
        /// 根据年份获取各个科室收入、服务人次 
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>各个科室收入、服务人次</returns>
        public List<ServiceHisCountInfo> GetServiceHisCountsByDepartment(int year)
        {
            List<ServiceHisCountInfo> result = new List<ServiceHisCountInfo>();

            sqlStr = "SELECT e.DepartmentID AS ObjectID, COUNT(s.ID) AS ServiceCount , SUM(s.Income) AS Incomes" +
					" FROM tblServiceHis s"+
					" LEFT JOIN tblEquipment e ON e.ID=s.EquipmentID"+
					" WHERE  DATEPART(YEAR,s.TransDate) = @Year"+
                    " GROUP BY e.DepartmentID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        result.Add(new ServiceHisCountInfo(dr));
                    }

                    return result;
                }
            }
        }
        /// <summary>
        /// 根据年份获取各个科室支出
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>各个科室支出</returns>
        public List<ServiceHisCountInfo> GetAccessoryExpenseByDepartment(int year)
        {
            List<ServiceHisCountInfo> result = new List<ServiceHisCountInfo>();

            sqlStr = "SELECT e.DepartmentID AS ObjectID, SUM(ra.Qty *  ra.Amount) AS Expenses " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID" +
                    " LEFT JOIN tblDispatch d ON dr.DispatchID = d.ID" +
                    " LEFT JOIN tblRequest r ON d.RequestID = r.ID " +
                    " LEFT JOIN jctRequestEqpt re ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON re.EquipmentID = e.ID" +
                    " WHERE  DATEPART(YEAR,d.StartDate) = @Year  AND d.StatusID = @Approved " +
                    " GROUP BY e.DepartmentID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@Approved", SqlDbType.Int).Value = DispatchInfo.Statuses.Approved;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(new ServiceHisCountInfo(dr));
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 根据年份获取各个设备收入、服务人次 
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>各个设备收入、服务人次</returns>
        public List<ServiceHisCountInfo> GetServiceHisCountsByEquipment(int year)
        {
            List<ServiceHisCountInfo> result = new List<ServiceHisCountInfo>();

            sqlStr = "SELECT s.EquipmentID AS ObjectID, COUNT(s.ID) AS ServiceCount , SUM(s.Income) AS Incomes " +
                    " FROM tblServiceHis s" +
                    " LEFT JOIN tblEquipment e ON e.ID=s.EquipmentID" +
                    " WHERE  DATEPART(YEAR,s.TransDate) = @Year" +
                    " GROUP BY s.EquipmentID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(new ServiceHisCountInfo(dr));
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 根据年份获取各个设备支出
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>各个设备支出</returns>
        public List<ServiceHisCountInfo> GetAccessoryExpenseByEquipment(int year)
        {
            List<ServiceHisCountInfo> result = new List<ServiceHisCountInfo>();

            sqlStr = "SELECT e.ID AS ObjectID, SUM(ra.Qty * ra.Amount) AS Expenses " +
                    " FROM tblReportAccessory ra" +
                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID" +
                    " LEFT JOIN tblDispatch d ON dr.DispatchID = d.ID" +
                    " LEFT JOIN tblRequest r ON d.RequestID = r.ID " +
                    " LEFT JOIN jctRequestEqpt re ON r.ID = re.RequestID " +
                    " LEFT JOIN tblEquipment e ON re.EquipmentID = e.ID" +
                    " WHERE  DATEPART(YEAR,d.StartDate) = @Year AND d.StatusID = @Approved " +
                    " GROUP BY e.ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@Approved", SqlDbType.Int).Value = DispatchInfo.Statuses.Approved;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(new ServiceHisCountInfo(dr));
                    }

                    return result;
                }
            }
        }
        
        /// <summary>
        /// 根据类型获取设备收入
        /// </summary>
        /// <param name="id">设备id</param>
        /// <param name="type">类型</param>
        /// <param name="year">年份</param>
        /// <returns>各个设备收入、服务人次</returns>
        public Dictionary<string,double> GetServiceHisIncomesByEquipmentID(int id,int type,int year)
        {
            Dictionary<string,double> result = new Dictionary<string,double>();

            sqlStr = string.Format("SELECT {0} AS ObjectID , SUM(s.Income) AS Incomes " +
                     " FROM tblServiceHis s " +
                     " WHERE s.EquipmentID = @ID ",(type == ReportDimension.AcceptanceMonth? " DATEPART(MONTH,s.TransDate) " : "DATEPART(YEAR,s.TransDate) ") );
            if (type == ReportDimension.AcceptanceMonth )
                sqlStr += " AND DATEPART(YEAR,s.TransDate) = @Year  GROUP BY DATEPART(MONTH,s.TransDate) ";
            else
                sqlStr +=  " GROUP BY DATEPART(YEAR,s.TransDate) ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result.Add(SQLUtil.TrimNull(dr[0]),SQLUtil.ConvertDouble(dr[1]));
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 根据类型获取设备支出分组
        /// </summary>
        /// <param name="id">设备id</param>
        /// <param name="type">展示类型</param>
        /// <param name="year">年份</param>
        /// <returns>设备支出</returns>
        public Dictionary<string,double> GetAccessoryExpenseByEquipmentID(int id,int type,int year)
        {
            //List<ServiceHisCountInfo> result = new List<ServiceHisCountInfo>();
            Dictionary<string,double> result = new Dictionary<string,double>();
            sqlStr =  string.Format("SELECT {0} AS ObjectID, SUM(ra.Qty * ra.Amount) AS Expenses " +
                                    " FROM tblReportAccessory ra" +
                                    " LEFT JOIN tblDispatchReport dr ON ra.DispatchReportID = dr.ID" +
                                    " LEFT JOIN tblDispatch d ON dr.DispatchID = d.ID" +
                                    " LEFT JOIN tblRequest r ON d.RequestID = r.ID " +
                                    " LEFT JOIN jctRequestEqpt re ON r.ID = re.RequestID " +
                                    " LEFT JOIN tblEquipment e ON re.EquipmentID = e.ID" +
                                    " WHERE d.StatusID = @Approved AND e.ID = @ID ",(type == ReportDimension.AcceptanceMonth? " DATEPART(MONTH,d.StartDate) " : "DATEPART(YEAR,d.StartDate) ") );
            
            if (type == ReportDimension.AcceptanceMonth )
                sqlStr += " AND DATEPART(YEAR,d.StartDate) = @Year  GROUP BY DATEPART(MONTH,d.StartDate) ";
            else
                sqlStr +=  " GROUP BY DATEPART(YEAR,d.StartDate) ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {                
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id; 
                command.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@Approved", SqlDbType.Int).Value = DispatchInfo.Statuses.Approved;
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //result.Add(new ServiceHisCountInfo(dr));                        
                        result.Add(SQLUtil.TrimNull(dr[0]),SQLUtil.ConvertDouble(dr[1]));

                    }

                    return result;
                }
            }
        }

     }
}
