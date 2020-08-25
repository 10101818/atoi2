using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MedicalEquipmentHostingSystem.App_Start
{
    public class DashboardProvider
    {
        
        private EquipmentDao equipmentDao = new EquipmentDao();
        private ServiceHisDao serviceHisDao = new ServiceHisDao();   
        private RequestDao requestDao = new RequestDao();  
        private RequestManager requestManager = new RequestManager(); 

        #region "Equipment"

        /// <summary>
        /// 设备信息总览
        /// </summary>
        /// <returns>设备数量、设备金额、折旧率、当年服务人次</returns>
        public Dictionary<string, object> EquipmentQueryOverview()
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            Tuple<int, double> Counts = this.equipmentDao.GetEquipmentInfoCounts();
            response.Add("EquipmentCount", Counts.Item1);
            response.Add("EquipmentAmount", Counts.Item2);
            response.Add("DepreciationRate", Math.Round(this.equipmentDao.GetEquipmentDepreciationRate(), 4));
            if (WebConfig.ISDEMO)
            {
                response.Add("ServiceCount", 13224);
            }
            else
                response.Add("ServiceCount", this.serviceHisDao.GetServiceCount(DateTime.Today.Year));

            return response;
        }

        /// <summary>
        /// 各部门设备信息（包括收入、支出、服务人次）
        /// </summary>
        /// <returns>各部门设备信息</returns>
        public List<DepartPerformanceModel> IncomeExpenseByDepartment(string date)
        {
            List<DepartPerformanceModel> models = new List<DepartPerformanceModel>();
            DepartPerformanceModel model = null;

            int curYear = DateTime.Today.Year;
            if (!string.IsNullOrEmpty(date))
                curYear = Convert.ToDateTime(date).Year;
            if (WebConfig.ISDEMO)
                curYear = new DateTime(2019, 11, 12).Year;
            int lastYear = curYear - 1;

            DataTable dtEquipmentCount = this.equipmentDao.GetEquipmentInfoCountsByDepartment();
            List<ServiceHisCountInfo> serviceInfosThisYear = this.serviceHisDao.GetServiceHisCountsByDepartment(curYear);
            List<ServiceHisCountInfo> accessoryExpensesThisYear = this.serviceHisDao.GetAccessoryExpenseByDepartment(curYear);
            List<ServiceHisCountInfo> serviceInfosLastYear = this.serviceHisDao.GetServiceHisCountsByDepartment(lastYear);
            List<ServiceHisCountInfo> accessoryExpensesLastYear = this.serviceHisDao.GetAccessoryExpenseByDepartment(lastYear);
            ServiceHisCountInfo serviceInfo = null;
            foreach (DataRow dr in dtEquipmentCount.Rows)
            {
                model = new DepartPerformanceModel();
                model.Department = LookupManager.GetDepartments(SQLUtil.ConvertInt(dr["DepartmentID"]));
                model.EquipmentCount = SQLUtil.ConvertInt(dr["Counts"]);
                model.EquipmentAmount = SQLUtil.ConvertDouble(dr["Amounts"]);

                serviceInfo = (from ServiceHisCountInfo temp in serviceInfosThisYear where temp.ObjectID == model.Department.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    model.ServiceCount = serviceInfo.ServiceCount;
                    model.Incomes = serviceInfo.Incomes;
                }
                serviceInfo = (from ServiceHisCountInfo temp in accessoryExpensesThisYear where temp.ObjectID == model.Department.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    model.Expenses = serviceInfo.Expenses;
                }

                serviceInfo = (from ServiceHisCountInfo temp in serviceInfosLastYear where temp.ObjectID == model.Department.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    model.LastIncomes = serviceInfo.Incomes;
                }
                serviceInfo = (from ServiceHisCountInfo temp in accessoryExpensesLastYear where temp.ObjectID == model.Department.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    model.LastExpenses = serviceInfo.Expenses;
                }
                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// 科室下所有设备信息（包括收入、支出）
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <param name="date">日期</param>
        /// <returns>科室下所有设备信息</returns>
        public List<EquipmentInfo> EquipmentsDetailsByDepartment(int id, string date = "")
        {
            int curYear = DateTime.Today.Year;
            if (!string.IsNullOrEmpty(date))
                curYear = SQLUtil.ConvertDateTime(date).Year;
            int lastYear = curYear - 1;
            if (WebConfig.ISDEMO)
            {
                curYear = 2019;
                lastYear = 2018;
            }

            List<EquipmentInfo> equipmentInfos = this.equipmentDao.GetEquipmentsByDepartmentID(id);
            List<ServiceHisCountInfo> serviceInfosThisYear = this.serviceHisDao.GetServiceHisCountsByEquipment(curYear);
            List<ServiceHisCountInfo> accessoryExpensesThisYear = this.serviceHisDao.GetAccessoryExpenseByEquipment(curYear);
            List<ServiceHisCountInfo> serviceInfosLastYear = this.serviceHisDao.GetServiceHisCountsByEquipment(lastYear);
            List<ServiceHisCountInfo> accessoryExpensesLastYear = this.serviceHisDao.GetAccessoryExpenseByEquipment(lastYear);
            ServiceHisCountInfo serviceInfo = null;
            foreach (EquipmentInfo info in equipmentInfos)
            {
                serviceInfo = (from ServiceHisCountInfo temp in serviceInfosThisYear where temp.ObjectID == info.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    info.Incomes = serviceInfo.Incomes;
                }
                serviceInfo = (from ServiceHisCountInfo temp in accessoryExpensesThisYear where temp.ObjectID == info.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    info.Expenses = serviceInfo.Expenses;
                }

                serviceInfo = (from ServiceHisCountInfo temp in serviceInfosLastYear where temp.ObjectID == info.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    info.LastIncomes = serviceInfo.Incomes;
                }
                serviceInfo = (from ServiceHisCountInfo temp in accessoryExpensesLastYear where temp.ObjectID == info.ID select temp).FirstOrDefault();
                if (serviceInfo != null)
                {
                    info.LastExpenses = serviceInfo.Expenses;
                }
            }
            equipmentInfos.RemoveAll(t => ((t.EquipmentStatus.ID == EquipmentInfo.EquipmentStatuses.Scrap) && t.Incomes == 0 && t.Expenses == 0));
             
            return equipmentInfos;
        }

        /// <summary>
        /// 获取设备的各类型请求数量
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="date">日期</param>
        /// <returns>设备申请的维修请求、保养请求、巡检请求、强检请求、校准请求数量</returns>
        public Dictionary<string, int> GetRequestCountByID(int id, string date = "")
        { 
            int year = DateTime.Today.Year;
            if (!string.IsNullOrEmpty(date))
                year = SQLUtil.ConvertDateTime(date).Year;
            if (WebConfig.ISDEMO)
                year = 2019;
            Dictionary<int, int> dicRequestCount = this.equipmentDao.GetRequestCountByID(id, year);

            Dictionary<string, int> resultData = new Dictionary<string, int>();
            if (dicRequestCount.ContainsKey(RequestInfo.RequestTypes.Repair) == false)
                resultData.Add("Repair", 0);
            else
                resultData.Add("Repair", dicRequestCount[RequestInfo.RequestTypes.Repair]);

            if (dicRequestCount.ContainsKey(RequestInfo.RequestTypes.Maintain) == false)
                resultData.Add("Maintain", 0);
            else
                resultData.Add("Maintain", dicRequestCount[RequestInfo.RequestTypes.Maintain]);

            if (dicRequestCount.ContainsKey(RequestInfo.RequestTypes.Inspection) == false)
                resultData.Add("Inspection", 0);
            else
                resultData.Add("Inspection", dicRequestCount[RequestInfo.RequestTypes.Inspection]);
            if (dicRequestCount.ContainsKey(RequestInfo.RequestTypes.OnSiteInspection) == false)
                resultData.Add("OnSiteInspection", 0);
            else
                resultData.Add("OnSiteInspection", dicRequestCount[RequestInfo.RequestTypes.OnSiteInspection]);
            if (dicRequestCount.ContainsKey(RequestInfo.RequestTypes.Correcting) == false)
                resultData.Add("Correcting", 0);
            else
                resultData.Add("Correcting", dicRequestCount[RequestInfo.RequestTypes.Correcting]);

            return resultData;
        }
        #endregion 

        #region"Request"

        /// <summary>
        /// 请求信息总览
        /// </summary>
        /// <returns>请求信息总览</returns>
        public Dictionary<string, List<RequestInfo>> RequestQueryOverview()
        {
            Dictionary<string, List<RequestInfo>> response = new Dictionary<string, List<RequestInfo>>();
            List<RequestInfo> requests = this.requestManager.QueryRequestsList(RequestInfo.Statuses.Unfinished, RequestInfo.RequestTypes.Repair, false, -1, RequestInfo.UrgencyLevel.Urgency, false, 0, "", "", "r.ID", true, 0, ConstDefinition.PAGE_SIZE_MAX, "", "");
            foreach (RequestInfo info in requests)
            {
                info.RequestUser = null;
            }
            response.Add("Repair", requests);

            requests = this.requestManager.QueryRequestsList(RequestInfo.Statuses.Unfinished, RequestInfo.RequestTypes.Inspection, true, -1, 0, false, 0, "", "", "r.ID", true, 0, ConstDefinition.PAGE_SIZE_MAX, "", "");
            foreach (RequestInfo info in requests)
            {
                info.RequestUser = null;
            }
            response.Add("Recall", requests);

            requests = this.requestManager.QueryRequestsList(RequestInfo.Statuses.Unfinished, RequestInfo.RequestTypes.Inspection, false, -1, 0, false, 0, "", "", "r.ID", true, 0, ConstDefinition.PAGE_SIZE_MAX, "", "");
            foreach (RequestInfo info in requests)
            {
                info.RequestUser = null;
            }
            response.Add("MandatoryTest", requests);

            requests = this.requestManager.QueryRequestsList(RequestInfo.Statuses.Unfinished, 0, false, -1, 0, true, 0, "", "", "r.ID", true, 0, ConstDefinition.PAGE_SIZE_MAX, "", "");
            foreach (RequestInfo info in requests)
            {
                info.RequestUser = null;
            }
            response.Add("OverDue", requests);
            return response;
        }

        /// <summary>
        /// 今日总报修
        /// </summary>
        /// <returns>今日总报修</returns>
        public List<RequestInfo> Todays(string date = "")
        {
            DateTime today = DateTime.Today;
            if (!string.IsNullOrEmpty(date))
                today = Convert.ToDateTime(date);
            if (WebConfig.ISDEMO)
                today = new DateTime(2019, 11, 12);

            List<RequestInfo> response = this.requestDao.GetTodayRepair(today);
            List<RequestEqptInfo> requestEqpts = this.requestDao.GetRequestEgpts(SQLUtil.GetIDListFromObjectList(response));
            foreach (RequestInfo request in response)
            {
                request.Equipments = (from RequestEqptInfo temp in requestEqpts where temp.RequestID == request.ID select temp.Equipment).ToList();
                request.RequestUser = null;
            }
            return response;
        }

        /// <summary>
        /// 开机率,校准率,保养率,巡检率信息
        /// </summary>
        /// <returns>开机率,校准率,保养率,巡检率信息</returns>
        public Dictionary<string, Dictionary<string, double>> KPI(string date = "")
        {
            DateTime today = DateTime.Today;
            if (!string.IsNullOrEmpty(date))
                today = Convert.ToDateTime(date);
            if (WebConfig.ISDEMO)
                today = new DateTime(2019, 11, 12);
            Dictionary<string, Dictionary<string, double>> response = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, double> dPlanDone = null;

            if (WebConfig.ISDEMO)
            {
                Tuple<int, int> tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.Correcting, today);
                int doneCount;
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", 31);
                doneCount = (int)Math.Ceiling(31 * DateTime.Today.Day / 28.0);
                dPlanDone.Add("Done", doneCount > 31 ? 31 : doneCount);
                response.Add("Correcting", dPlanDone);

                tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.Maintain, today);
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", 56);
                dPlanDone.Add("Done", 48);
                response.Add("Maintain", dPlanDone);

                tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.OnSiteInspection, today);
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", 232);
                doneCount = (int)Math.Ceiling(232 * DateTime.Today.Day / 15.0);
                doneCount = doneCount < 187 ? 187 : doneCount;
                dPlanDone.Add("Done", doneCount > 232 ? 232 : doneCount);
                response.Add("OnSiteInspection", dPlanDone);
            }
            else
            {
                Tuple<int, int> tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.Correcting, today);
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", tPlanDone.Item1);
                dPlanDone.Add("Done", tPlanDone.Item2);
                response.Add("Correcting", dPlanDone);

                tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.Maintain, today);
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", tPlanDone.Item1);
                dPlanDone.Add("Done", tPlanDone.Item2);
                response.Add("Maintain", dPlanDone);

                tPlanDone = this.requestDao.GetRequestPlanActual(RequestInfo.RequestTypes.OnSiteInspection, today);
                dPlanDone = new Dictionary<string, double>();
                dPlanDone.Add("Plans", tPlanDone.Item1);
                dPlanDone.Add("Done", tPlanDone.Item2);
                response.Add("OnSiteInspection", dPlanDone);
            }


            Dictionary<string, double> startupRate = new Dictionary<string, double>();
            startupRate.Add("Default", Math.Round(WebConfig.BOOT_RATE, 4));
            startupRate.Add("Present", Math.Round(this.equipmentDao.GetEquipmentBootRate(), 4));
            response.Add("BootRate", startupRate);

            return response;
        }

        #endregion
    }
}