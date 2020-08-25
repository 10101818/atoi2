using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using DocumentFormat.OpenXml.EMMA;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Statistics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 富士二类事务处理
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class FujiClassManager
    {
        private FaultRateDao faultRateDao = new FaultRateDao();
        private LookupDao lookupDao = new LookupDao();
        private AuditDao auditDao = new AuditDao();
        private FujiClassDao fujiClassDao = new FujiClassDao();
        private ComponentDao componentDao = new ComponentDao();
        private ConsumableDao consumableDao = new ConsumableDao();

        private ComponentManager componentManager = new ComponentManager();
        #region"FujiClass2"
        /// <summary>
        /// 获取富士2类信息包含故障率信息
        /// </summary>
        /// <param name="class1">富士类别1</param>
        /// <param name="class2">富士类别2</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param> 
        /// <returns>富士2类信息包含故障率信息</returns>
        public List<FujiClass2Info> QueryFujiClass2s(int class1, int class2, string filterField, string filterText, string sortField, bool sortDirection)
        {
            List < FujiClass2Info > infos = this.fujiClassDao.QueryFujiClass2s(class1, class2, filterField, filterText, sortField, sortDirection, 0, 0);
            Dictionary<int, List<FaultRateInfo>> faultRates = this.faultRateDao.GetFaultRateByObject(infos.Select(info => info.ID).ToList(), FaultRateInfo.ObjectType.Repair);
            infos.ForEach(info => {
                if (faultRates.ContainsKey(info.ID))
                    info.Repairs = faultRates[info.ID]; 

                SetFujiClass2Rate(info);
                
            });
            return infos;
        }

        private List<FujiClass2Info> QueryFujiClass2sByIDs(List<int> ids)
        {
            List<FujiClass2Info> infos = this.fujiClassDao.QueryFujiClass2sByIDs(ids);
            Dictionary<int, List<FaultRateInfo>> faultRates = this.faultRateDao.GetFaultRateByObject(ids, FaultRateInfo.ObjectType.Repair);
            infos.ForEach(info => {
                if (faultRates.ContainsKey(info.ID))
                    info.Repairs = faultRates[info.ID];

                // SetFujiClass2Rate(info);

            });
            return infos;
        }
        /// <summary>
        /// 根据富士II类id获取信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>富士II类信息</returns>
        public FujiClass2Info GetFujiClass2ByID(int id)
        {
            FujiClass2Info info = this.fujiClassDao.GetFujiClass2ByID(id); 
            info.Repairs = this.faultRateDao.GetFaultRateByObject(id, FaultRateInfo.ObjectType.Repair);

            SetFujiClass2Rate(info);
            return info;
        }
        /// <summary>
        /// 根据富士II类id获取关联信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>关联信息</returns>
        public FujiClassLink GetFujiLinkByClass2ID(int id)
        {
            FujiClassLink info = this.fujiClassDao.GetFujiLinkByClass2ID(id);
            info.FujiClass2.Repairs = this.faultRateDao.GetFaultRateByObject(id, FaultRateInfo.ObjectType.Repair);

            SetFujiClass2Rate(info.FujiClass2);
            return info;
        }

        /// <summary>
        /// 保存富士II类信息
        /// </summary>
        /// <param name="info">富士II类信息</param>
        /// <param name="userInfo">操作者</param>
        /// <param name="isUpdate">是否修改</param>
        /// <param name="copyID">复制的富士II类id</param>
        /// <returns>富士II类id</returns>
        [TransactionAspect]
        public int SaveFujiClass2(FujiClassLink info, UserInfo userInfo, bool isUpdate, int copyID = 0)
        { 
            List<AuditHdrInfo> audits = new List<AuditHdrInfo>();
            AuditHdrInfo audit;
            if (info.FujiClass2.ID == 0)
            {
                if (copyID != 0)
                {
                    FujiClass2Info copyInfo = this.GetFujiClass2ByID(copyID);
                    copyInfo.FujiClass1 = info.FujiClass2.FujiClass1;
                    copyInfo.Name = info.FujiClass2.Name;
                    copyInfo.Description = info.FujiClass2.Description;
                    info.FujiClass2.ID = this.fujiClassDao.AddFujiClass2(copyInfo);
                    this.fujiClassDao.AddFujiClass2EqpType(info);
                    copyInfo.Repairs.ForEach(rate => rate.ObjectID = info.FujiClass2.ID);
                    this.AddFaultRates(copyInfo.Repairs);
                    if (copyInfo.Components.Count > 0)
                    {
                        foreach(ComponentInfo componentInfo in copyInfo.Components)
                        {
                            componentInfo.ID = 0;
                            componentInfo.FujiClass2.ID = info.FujiClass2.ID;
                            this.componentManager.SaveComponent(componentInfo, userInfo);
                        }
                    }
                    if (copyInfo.Consumables.Count > 0)
                    {
                        foreach(ConsumableInfo consumableInfo in copyInfo.Consumables)
                        {
                            consumableInfo.ID = 0;
                            consumableInfo.FujiClass2.ID = info.FujiClass2.ID;
                            this.consumableDao.AddConsumable(consumableInfo);
                        }
                    }
                }
                else
                {
                    this.fujiClassDao.AddFujiClass2(info.FujiClass2);
                    this.fujiClassDao.AddFujiClass2EqpType(info);
                    info.FujiClass2.Repairs = FaultRateInfo.GetInitList(info.FujiClass2.ID, FaultRateInfo.ObjectType.Repair);
                    this.AddFaultRates(info.FujiClass2.Repairs);
                }
                audit = info.ConvertAudit(userInfo);
                if(audit.DetailInfo.Count>0)
                    audits.Add(audit);
            }
            else
            {
                FujiClassLink existsInfo = this.GetFujiLinkByClass2ID(info.FujiClass2.ID);
                if (isUpdate)
                {
                    info.FujiClass2.CheckEquipmentTypeRelatedFields();
                    this.fujiClassDao.UpdateFujiClass2(info.FujiClass2); 
                    this.UpdateFaultRates(info.FujiClass2.Repairs);
                     
                    audit = existsInfo.FujiClass2.ConvertAudit(info.FujiClass2, userInfo); 
                    audit.DetailInfo = audit.DetailInfo.Concat(FaultRateInfo.ConvertAuditDetail(existsInfo.FujiClass2.Repairs, info.FujiClass2.Repairs)).ToList();
                    if (audit.DetailInfo.Count>0)
                        audits.Add(audit);

                    int componentLen = info.FujiClass2.Components.Count;
                    info.FujiClass2.Components.Sort();
                    existsInfo.FujiClass2.Components.Sort();
                    info.FujiClass2.Consumables.Sort();
                    existsInfo.FujiClass2.Consumables.Sort();
                    for(int i= 0; i < componentLen; i++)
                    {
                        this.componentDao.UpdateComponent(info.FujiClass2.Components[i]);
                        audit = existsInfo.FujiClass2.Components[i].ConvertAudit(info.FujiClass2.Components[i], userInfo);
                        if (audit.DetailInfo.Count > 0)
                            audits.Add(audit);
                    }
                    componentLen = info.FujiClass2.Consumables.Count;
                    for (int i = 0; i < componentLen; i++)
                    {
                        this.consumableDao.UpdateConsumable(info.FujiClass2.Consumables[i]);
                        audit = existsInfo.FujiClass2.Consumables[i].ConvertAudit(info.FujiClass2.Consumables[i], userInfo);
                        if (audit.DetailInfo.Count > 0)
                            audits.Add(audit);
                    }
                }
                else
                {
                    this.fujiClassDao.AddFujiClass2EqpType(info);
                    audit = info.ConvertAudit(userInfo);
                    if (audit.DetailInfo.Count > 0)
                        audits.Add(audit);
                }
            }

            int len = audits.Count;
            if (len == 0)
                return info.FujiClass2.ID;
            DataTable auditDT = audits.ConvertAuditDT();
            DataTable auditDetailDT = null;
            this.auditDao.AddAuditHdr(auditDT);
            for (int i = 0; i < len; i++)
            {
                if (auditDetailDT == null)
                    auditDetailDT = audits[i].DetailInfo.ConvertAuditDetailDT(SQLUtil.ConvertInt(auditDT.Rows[i]["ID"]));
                else
                    auditDetailDT.Merge(audits[i].DetailInfo.ConvertAuditDetailDT(SQLUtil.ConvertInt(auditDT.Rows[i]["ID"])));
            }
            this.auditDao.AddAuditDetail(auditDetailDT);

            return info.FujiClass2.ID;
        }
		/// <summary>
        /// 设置富士II类零件、耗材信息
        /// </summary>
        /// <param name="info">富士II类</param>
        /// <returns>获取富士II类零件/耗材信息</returns>
        private FujiClass2Info SetFujiClass2Rate(FujiClass2Info info)
        {
            info.Components = GetComponentsRateByFujiClass2(info.ID);
            info.Consumables = this.consumableDao.QueryConsumablesByFujiClass2ID(info.ID, true);
            return info;
        }
        /// <summary>
        /// 根据富士II类获取零件故障率信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>零件故障率信息</returns>
        public List<ComponentInfo> GetComponentsRateByFujiClass2(int id)
        {
            List<ComponentInfo> components = this.componentDao.QueryComponentsByFujiClass2ID(id, -1, true);

            Dictionary<int, List<FaultRateInfo>> faultRates = this.faultRateDao.GetFaultRateByObject(components.Select(component => component.ID).ToList(), FaultRateInfo.ObjectType.Component);
            components.ForEach(component =>
            {
                if (faultRates.ContainsKey(component.ID))
                    component.FaultRates = faultRates[component.ID];
            });
            return components;
        }
        /// <summary>
        /// 批量保存富士II类信息
        /// </summary>
        /// <param name="infos">富士II类信息</param>
        /// <param name="type">费用类别</param>
        /// <param name="userInfo">操作者</param>
		[TransactionAspect]
        public void SaveFujiClass2s(List<FujiClass2Info> infos, FujiClass2Info.SectionType type,UserInfo userInfo)
        {
            List<FujiClass2Info> existsInfos = this.QueryFujiClass2sByIDs(infos.Select(info => info.ID).ToList());
            DataTable dt = FujiClass2Info.ConvertFujiClass2DataTable(infos); 
            switch (type)
            {
                case FujiClass2Info.SectionType.Labour:
                    this.fujiClassDao.UpdateFujiClass2Labour(dt);
                    break;
                case FujiClass2Info.SectionType.Contract:
                    this.fujiClassDao.UpdateFujiClass2Contract(dt);
                    break;
                case FujiClass2Info.SectionType.Spare:
                    this.fujiClassDao.UpdateFujiClass2Spare(dt);
                    break;
                case FujiClass2Info.SectionType.Repair:
                    this.fujiClassDao.UpdateFujiClass2Repair(dt);
                    this.UpdateFaultRates(infos.SelectMany(info => info.Repairs).ToList());
                    break;
            }

            //转换日志信息
            infos.Sort(); 
            existsInfos.Sort();
            List<AuditHdrInfo> audits = new List<AuditHdrInfo>();
            int len = existsInfos.Count;
            for (int i = 0; i<len; i++)
            {
                AuditHdrInfo audit = existsInfos[i].ConvertAudit(infos[i], userInfo); 
                audit.DetailInfo = audit.DetailInfo.Concat(FaultRateInfo.ConvertAuditDetail(existsInfos[i].Repairs, infos[i].Repairs)).ToList();
                audits.Add(audit);
            }
            DataTable auditDT = audits.ConvertAuditDT();
            DataTable auditDetailDT = null;
            this.auditDao.AddAuditHdr(auditDT); 
            for (int i = 0; i < len; i++)
            {
                if (auditDetailDT == null)
                    auditDetailDT = audits[i].DetailInfo.ConvertAuditDetailDT(SQLUtil.ConvertInt(auditDT.Rows[i]["ID"]));
                else
                    auditDetailDT.Merge(audits[i].DetailInfo.ConvertAuditDetailDT(SQLUtil.ConvertInt(auditDT.Rows[i]["ID"])));
            }
            this.auditDao.AddAuditDetail(auditDetailDT);
        }       

        /// <summary>
        /// 添加故障率
        /// </summary>
        /// <param name="infos">故障率信息</param>
        [TransactionAspect]
        public void AddFaultRates(List<FaultRateInfo> infos)
        {
            DataTable dt = FaultRateInfo.ConvertFaultRateDataTable(infos);
            this.faultRateDao.AddFaultRates(dt);
        }
        /// <summary>
        /// 修改故障率
        /// </summary>
        /// <param name="infos">故障率信息</param>
        [TransactionAspect]
        public void UpdateFaultRates(List<FaultRateInfo> infos)
        { 
            DataTable dt = FaultRateInfo.ConvertFaultRateDataTable(infos);
            this.faultRateDao.UpdateFaultRates(dt); 
        }

        /// <summary>
        /// 删除富士I类设备类型关联
        /// </summary>
        /// <param name="info">富士关联信息</param>
        [TransactionAspect]
        public void DeleteFujiClass2ByLink(FujiClassLink info)
        {
            this.fujiClassDao.DeleteFujiClass2Link(info);

            if (!this.fujiClassDao.CheckFujiClass2LinkExisted(info))
            {
                this.faultRateDao.DeleteFaultRateByObjID(info.FujiClass2.ID, FaultRateInfo.ObjectType.Repair);
                this.consumableDao.DeleteConsumableByFujiClass2ID(info.FujiClass2.ID);
                FujiClass2Info fujiClass2 = this.GetFujiClass2ByID(info.FujiClass2.ID);
                foreach(ComponentInfo component in fujiClass2.Components)
                {
                    this.componentManager.DeleteComponentByID(component.ID);
                }
                this.fujiClassDao.DeleteFujiClass2ByID(info.FujiClass2.ID);
            }

        }

        #endregion

        #region"FujiClass1"
        /// <summary>
        /// 保存富士I类
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <param name="isUpdate">是否更新</param>
        /// <returns>富士I类ID</returns>
        [TransactionAspect]
        public int SaveFujiClass1(FujiClass1Info info, bool isUpdate)
        {
            if (info.ID == 0)
            {
                info.ID = this.fujiClassDao.AddFujiClass1(info);
                this.fujiClassDao.AddFujiClass1EqpType(info);
            }
            else
            {
                //引用富士I类
                if (!isUpdate)
                {
                    this.fujiClassDao.AddFujiClass1EqpType(info);
                }
                else
                {
                    this.fujiClassDao.UpdateFujiClass1(info);
                }
            }

            return info.ID;
        }

        /// <summary>
        /// 删除富士I类设备类型关联
        /// </summary>
        /// <param name="info">富士关联信息</param>
        [TransactionAspect]
        public void DeleteFujiClassLink(FujiClassLink info)
        {
            this.fujiClassDao.DeleteFujiClassLink(info);
        }

        /// <summary>
        /// 根据ID删除富士I类
        /// </summary>
        /// <param name="id">富士I类ID</param>
        [TransactionAspect]
        public void DeleteFujiClass1ByID(int id)
        {
            this.fujiClassDao.DeleteFujiClass1ByID(id);
            this.fujiClassDao.DeleteFujiClass1LinkByID(id);
        }
        #endregion

        #region web
        public Dictionary<string, object> GetRepairWebData(int fujiClass2ID, int months)
        {
            Dictionary<int, int> eqptContractCount = this.faultRateDao.GetEqptContractMonth(fujiClass2ID);
            Dictionary<int, int> eqptRepairCount = this.faultRateDao.GetEqptRepairMonth(fujiClass2ID);

            Dictionary<string, object> result = GetWebData(fujiClass2ID,FaultRateInfo.ObjectType.Repair, months, eqptContractCount, eqptRepairCount);

            return result;
        }

        public Dictionary<string, object> GetComponentWebData(int fujiClass2ID, int componentID, int months)
        {
            Dictionary<int, int> eqptContractCount = this.faultRateDao.GetEqptContractMonth(fujiClass2ID);
            Dictionary<int, int> componentRepairCount = this.faultRateDao.GetComponentRepairMonth(componentID);

            Dictionary<string, object> result = GetWebData(componentID, FaultRateInfo.ObjectType.Component, months, eqptContractCount, componentRepairCount);

            return result;
        }

        private Dictionary<string, object> GetWebData(int objectID, FaultRateInfo.ObjectType objectType, int months, Dictionary<int, int> eqptContractCount, Dictionary<int, int> repairCount)
        {
            months = months > 120 ? 120 - 1 : months - 1;
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<FaultRateInfo> ForecastRateList = new List<FaultRateInfo>();
            double slope = 0, intercept = 0, slope1 = 0, intercept1 = 0;            

            List<FaultRateInfo> RateList = new List<FaultRateInfo>();
            List<FaultRateInfo> RateList1 = new List<FaultRateInfo>();

            GetRateList(objectID, objectType, eqptContractCount, repairCount, months, out RateList, out RateList1);

            if (RateList.Count >= 2)
            {
                Tuple<double, double> solpAndIntercept1 = SimpleRegression.Fit(RateList.Select(item => SQLUtil.ConvertDouble((item.Year - 1) * 12 + (int)item.Month)).ToArray(), RateList.Select(item => item.Rate).ToArray());
                slope = solpAndIntercept1.Item1;
                intercept = solpAndIntercept1.Item2;
            }

            if (RateList1.Count >= 2)
            {
                Tuple<double, double> solpAndIntercept2 = SimpleRegression.Fit(RateList1.Select(item => SQLUtil.ConvertDouble((item.Year - 1) * 12 + (int)item.Month)).ToArray(), RateList1.Select(item => item.Rate).ToArray());
                slope1 = solpAndIntercept2.Item1;
                intercept1 = solpAndIntercept2.Item2;
            }

            ForecastRateList = GetForecastRate(objectID, objectType, slope, intercept, slope1, intercept1, months);

            result.Add("RateList", RateList);
            result.Add("RateList1", RateList1);
            result.Add("ForecastRateList", ForecastRateList);
            result.Add("WebData", ForecastRateList.Concat(RateList).Concat(RateList1));
            
            return result;
        }

        private void GetRateList(int objectID, FaultRateInfo.ObjectType objectType, Dictionary<int, int> eqptContractCount, Dictionary<int, int> eqptRepairCount, int months, out List<FaultRateInfo> RateList, out List<FaultRateInfo> RateList1)
        {
            RateList = new List<FaultRateInfo>();
            RateList1 = new List<FaultRateInfo>();
            for (int i = 0; i < 120; i++)
            {
                int eqptCount = (from eqpt in eqptContractCount where eqpt.Value >= i select eqpt).Count();
                int repairCount = (from repair in eqptRepairCount where repair.Value == i select repair).Count();

                if (eqptCount > 0 && repairCount > 0)
                {
                    double rate = Math.Round(repairCount * 100.0 / eqptCount, 2);
                    FaultRateInfo item = new FaultRateInfo();
                    item.Year = SQLUtil.ConvertInt(i / 12 + 1);
                    item.Month = SQLUtil.ConvertEnum<FaultRateInfo.DateTimeMonth>(i % 12 + 1);
                    item.ObjectID = objectID;
                    item.ObjectTypeID = objectType;
                    item.Rate = rate;

                    if (i < months)
                        RateList.Add(item);
                    else
                        RateList1.Add(item); 
                }
            }

        }

        public List<FaultRateInfo> GetForecastRate(int objectID, FaultRateInfo.ObjectType objectType, double slope, double intercept, double slope1, double intercept1, int month = 0)
        {
            List<FaultRateInfo> ForecastRate = new List<FaultRateInfo>();

            for (int i = 0; i < 120; i++)
            {
                FaultRateInfo item = new FaultRateInfo();
                item.Year = SQLUtil.ConvertInt(i / 12 + 1);
                item.Month = SQLUtil.ConvertEnum<FaultRateInfo.DateTimeMonth>(i % 12 + 1);
                item.ObjectID = objectID;
                item.ObjectTypeID = objectType;
                if (i > month)
                {
                    double rate = Math.Round((((i + 1) * slope1 + intercept1) * 100.0), 2);
                    item.Rate = rate;
                    ForecastRate.Add(item);
                }
                else
                {
                    double rate = Math.Round((((i + 1) * slope + intercept) * 100.0), 2);
                    item.Rate = rate;
                    ForecastRate.Add(item);
                }
            }

            return ForecastRate;
        } 

        #endregion

    }
}
