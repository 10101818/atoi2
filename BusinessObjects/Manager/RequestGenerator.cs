using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 计划任务manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class RequestGenerator
    {
        private EquipmentDao equipmentDao = new EquipmentDao();
        private EquipmentManager equipmentManager = new EquipmentManager();
        private RequestDao requestDao = new RequestDao();
        private RequestManager requestManager = new RequestManager();
        private HistoryDao historyDao = new HistoryDao();

        /// <summary>
        /// 执行计划任务
        /// </summary>
        public void DoProcess()
        {
            this.CreateMaintenanceRequest();
            this.CreateCorrectionRequest();
            this.CreatePatrolRequest();
        }
        /// <summary>
        /// 获取计划服务下次生成时间
        /// </summary>
        /// <param name="typeID">周期类型</param>
        /// <param name="period">周期</param>
        /// <param name="lastChekcDate">上次计划服务时间</param>
        /// <param name="createDate">设备添加时间</param>
        /// <returns>下次计划服务时间</returns>
        private static DateTime GetExecDate(int typeID, int period, DateTime lastChekcDate, DateTime createDate)
        {
            DateTime checkDate = new DateTime();
            if (typeID == EquipmentInfo.PeriodTypes.Month && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddMonths(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Month && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddMonths(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Year && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddYears(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Year && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddYears(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Day && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddDays(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Day && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddDays(period);
            return checkDate.Date;
        }

        /// <summary>
        /// 创建保养请求的计划任务
        /// </summary>
        private void CreateMaintenanceRequest()
        {
            try
            {
                List<EquipmentInfo> equipmentInfos = this.equipmentDao.GetScheduledEquipments("MaintenanceType");
                if (equipmentInfos.Count > 0) equipmentInfos = CheckRequestExsist(equipmentInfos, RequestInfo.RequestTypes.Maintain);

                foreach (EquipmentInfo info in equipmentInfos)
                {
                    DateTime execDate = GetExecDate(info.MaintenanceType.ID, info.MaintenancePeriod, info.LastMaintenanceDate, info.CreateDate);

                    DateTime checkDate = execDate.AddMonths(-1);
                    if (checkDate < DateTime.Now) checkDate = DateTime.Now.Date;

                    if (DateTime.Now >= checkDate)
                    {
                        AddRequest(info, RequestInfo.RequestTypes.Maintain, checkDate, execDate);
                        this.equipmentManager.UpdateEquipmentLastMaintenanceCheck(info);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }
        }
        /// <summary>
        /// 创建校准请求的计划任务
        /// </summary>
        private void CreateCorrectionRequest()
        {
            try
            {
                List<EquipmentInfo> equipmentInfos = this.equipmentDao.GetScheduledEquipments("CorrectionType");
                if (equipmentInfos.Count > 0) equipmentInfos = CheckRequestExsist(equipmentInfos, RequestInfo.RequestTypes.Correcting);

                foreach (EquipmentInfo info in equipmentInfos)
                {
                    DateTime execDate = GetExecDate(info.CorrectionType.ID, info.CorrectionPeriod, info.LastCorrectionDate, info.CreateDate);

                    DateTime checkDate = execDate.AddMonths(-1);
                    if (checkDate < DateTime.Now) checkDate = DateTime.Now.Date;

                    if (DateTime.Now >= checkDate)
                    {
                        AddRequest(info, RequestInfo.RequestTypes.Correcting, checkDate, execDate);
                        this.equipmentManager.UpdateEquipmentLastCorrectionCheck(info);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }
        }
        /// <summary>
        /// 创建巡检请求的计划任务
        /// </summary>
        private void CreatePatrolRequest()
        {
            try
            {
                List<EquipmentInfo> equipmentInfos = this.equipmentDao.GetScheduledEquipments("PatrolType");
                if (equipmentInfos.Count > 0) equipmentInfos = CheckRequestExsist(equipmentInfos, RequestInfo.RequestTypes.OnSiteInspection);
                Dictionary<DateTime, List<EquipmentInfo>> dic = new Dictionary<DateTime, List<EquipmentInfo>>();

                foreach (EquipmentInfo info in equipmentInfos)
                {
                    DateTime checkDate, requestDate;
                    if (info.LastPatrolDate == DateTime.MinValue)
                        info.LastPatrolDate = info.CreateDate;
                    if (info.PatrolType.ID == EquipmentInfo.PeriodTypes.Month)
                    {
                        checkDate = info.LastPatrolDate.AddMonths(info.PatrolPeriod).AddDays(-info.LastPatrolDate.Day + 1);
                        requestDate = info.LastPatrolDate.AddMonths(info.PatrolPeriod).Date;
                    }
                    else if (info.PatrolType.ID == EquipmentInfo.PeriodTypes.Year)
                    {
                        checkDate = info.LastPatrolDate.AddYears(info.PatrolPeriod).AddDays(-info.LastPatrolDate.Day + 1);
                        requestDate = info.LastPatrolDate.AddYears(info.PatrolPeriod).Date;
                    }
                    else
                    {
                        checkDate = info.LastPatrolDate.AddDays(info.PatrolPeriod).AddDays(-info.LastPatrolDate.AddDays(info.PatrolPeriod).Day + 1);
                        requestDate = info.LastPatrolDate.AddDays(info.PatrolPeriod).Date;
                    }
                    if (requestDate < DateTime.Now) requestDate = DateTime.Now.Date;
                    if (DateTime.Now >= checkDate.Date)
                    {
                        if (!dic.ContainsKey(requestDate)) dic.Add(requestDate, new List<EquipmentInfo>());

                        dic[requestDate].Add(info);
                    }
                }
                foreach(var info in dic)
                {
                    AddRequest4Patrol(info.Value, info.Key);
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }
        }
        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="info">创建请求的设备信息</param>
        /// <param name="requestType">请求类型</param>
        /// <param name="requestDate">请求日期</param>
        private void AddRequest(EquipmentInfo info, int requestType, DateTime requestDate, DateTime execDate)
        {
            RequestInfo requestInfo = new RequestInfo();
            requestInfo.Source.ID = RequestInfo.Sources.SysRequest;
            requestInfo.RequestType.ID = requestType;
            requestInfo.RequestType.Name = Manager.LookupManager.GetRequestTypeDesc(requestInfo.RequestType.ID);
            requestInfo.Subject = requestInfo.RequestType.Name;
            requestInfo.FaultType.ID = RequestInfo.MaintainType.OriginalMaintain;
            requestInfo.DealType.ID = RequestInfo.DealTypes.FieldService;
            requestInfo.Priority.ID = RequestInfo.UrgencyLevel.Normal;
            requestInfo.FaultDesc = "系统定时生成；执行日期：" + execDate.ToShortDateString();
            requestInfo.Status.ID = RequestInfo.Statuses.New;
            requestInfo.RequestDate = requestDate;
            requestInfo.Equipments = new List<EquipmentInfo>();
            requestInfo.Equipments.Add(info);
            requestInfo.Subject = requestInfo.SubjectName;

            UserInfo user = new UserInfo();
            this.requestManager.AddRequest(requestInfo, null, user);

        }
        /// <summary>
        /// 创建巡检请求
        /// </summary>
        /// <param name="infos">创建请求的设备信息</param>
        /// <param name="requestDate">请求日期</param>
        private void AddRequest4Patrol(List<EquipmentInfo> infos, DateTime requestDate)
        {
            RequestInfo requestInfo = new RequestInfo();
            requestInfo.Source.ID = RequestInfo.Sources.SysRequest;
            requestInfo.RequestType.ID = RequestInfo.RequestTypes.OnSiteInspection;
            requestInfo.RequestType.Name = Manager.LookupManager.GetRequestTypeDesc(requestInfo.RequestType.ID);
            requestInfo.FaultType.ID = RequestInfo.MaintainType.OriginalMaintain;
            requestInfo.DealType.ID = RequestInfo.DealTypes.FieldService;
            requestInfo.Priority.ID = RequestInfo.UrgencyLevel.Normal;
            requestInfo.FaultDesc = "系统定时生成";
            requestInfo.Status.ID = RequestInfo.Statuses.New;
            requestInfo.RequestDate = requestDate;
            requestInfo.Equipments = new List<EquipmentInfo>();
            foreach(EquipmentInfo info in infos)
            {
                requestInfo.Equipments.Add(info);
                this.equipmentManager.UpdateEquipmentLastPatrolCheck(info.ID, requestDate);
            }
            requestInfo.Subject = requestInfo.SubjectName;
            
            UserInfo user = new UserInfo();
            this.requestManager.AddRequest(requestInfo, null, user);

        }

        /// <summary>
        /// 检测设备该类型的请求是否已创建
        /// </summary>
        /// <param name="equipments">设备信息</param>
        /// <param name="requestType">请求类型</param>
        /// <returns>未创建请求的设备信息</returns>
        private List<EquipmentInfo> CheckRequestExsist(List<EquipmentInfo> equipments, int requestType)
        {
            List<EquipmentInfo> exsistInfos = this.equipmentDao.CheckRequestExsist(SQLUtil.GetIDListFromObjectList(equipments), requestType);

            foreach(EquipmentInfo info in exsistInfos)
            {
                equipments = (from temp in equipments where temp.ID != info.ID select temp).ToList();
            }

            return equipments;
        }
    }
}
