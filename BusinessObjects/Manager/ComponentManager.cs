using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 零件manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ComponentManager
    {
        private ComponentDao componentDao = new ComponentDao();
        private FaultRateDao faultRateDao = new FaultRateDao();
        private AuditDao auditDao = new AuditDao();
        private FujiClassDao fujiClassDao = new FujiClassDao();

        private AuditManager auditManager = new AuditManager();
        /// <summary>
        /// 根据富士II类id获取零件信息
        /// </summary>
        /// <param name="fujiClass2ID">富士II类id</param>
        /// <param name="componentTypeID">零件类型id</param>
        /// <param name="isIncluded">是否维保</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo> QueryComponentsByFujiClass2ID(int fujiClass2ID, int componentTypeID, bool isIncluded = false, string filterField = "", string filterText = "")
        {
            List<ComponentInfo> infos = this.componentDao.QueryComponentsByFujiClass2ID(fujiClass2ID, componentTypeID, isIncluded, filterField, filterText);
            Dictionary<int, List<FaultRateInfo>> faultRates = this.faultRateDao.GetFaultRateByObject(infos.Select(info => info.ID).ToList(), FaultRateInfo.ObjectType.Component);
            infos.ForEach(info => {
                if (faultRates.ContainsKey(info.ID))
                    info.FaultRates = faultRates[info.ID];  
            });
            return infos;
        }
        /// <summary>
        /// 根据零件id获取零件信息
        /// </summary>
        /// <param name="ids">零件id</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo> QueryComponentsByIDs(List<int> ids)
        {
            List<ComponentInfo> infos = this.componentDao.QueryComponentsByIDs(ids);
            Dictionary<int, List<FaultRateInfo>> faultRates = this.faultRateDao.GetFaultRateByObject(ids, FaultRateInfo.ObjectType.Component);
            infos.ForEach(info => {
                if (faultRates.ContainsKey(info.ID))
                    info.FaultRates = faultRates[info.ID];
            });
            return infos;
        }

        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>零件id</returns>
        [TransactionAspect]
        public int SaveComponent(ComponentInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.componentDao.AddComponent(info);
                if (info.FaultRates.Count == 0)
                    info.FaultRates = FaultRateInfo.GetInitList(info.ID, FaultRateInfo.ObjectType.Component);
                else
                    info.FaultRates.ForEach(faultRate => faultRate.ObjectID = info.ID);
                this.faultRateDao.AddFaultRates(FaultRateInfo.ConvertFaultRateDataTable(info.FaultRates));
            }
            else
            {
                ComponentInfo existingInfo = this.componentDao.GetComponentByID(info.ID);
                existingInfo.FaultRates = this.faultRateDao.GetFaultRateByObject(existingInfo.ID, FaultRateInfo.ObjectType.Component);
                info.FujiClass2.Name = this.fujiClassDao.GetFujiClass2ByID(info.FujiClass2.ID).Name;
                DataTable dtField = existingInfo.GetChangedFields(info);

                if (dtField.Rows.Count > 0)
                {
                    this.componentDao.UpdateComponent(info);

                    this.faultRateDao.UpdateFaultRates(FaultRateInfo.ConvertFaultRateDataTable(info.FaultRates));
                    // 转换故障率信息 
                    if(info.FaultRates.Count!=0 && existingInfo.FaultRates.Count!= 0)
                        dtField.Merge(FaultRateInfo.ConvertAuditDetail(existingInfo.FaultRates, info.FaultRates).ConvertAuditDetailDT(0));
                    this.auditManager.AddAuditLog(userInfo.ID, ObjectTypes.Component, info.ID, dtField);
                }
            }

            return info.ID;
        }

        /// <summary>
        /// 批量修改零件信息
        /// </summary>
        /// <param name="infos">零件信息列表</param>
        /// <param name="userInfo">操作者</param>
        [TransactionAspect]
        public void UpdateComponents(List<ComponentInfo> infos,UserInfo userInfo)
        {
            List<ComponentInfo> existsInfos = this.QueryComponentsByIDs(infos.Select(info => info.ID).ToList());
            foreach (ComponentInfo info in infos)
            {
                this.componentDao.UpdateComponent(info);
            }
            DataTable dt = FaultRateInfo.ConvertFaultRateDataTable(infos.SelectMany(component => component.FaultRates).ToList());
            this.faultRateDao.UpdateFaultRates(dt);

            // 零件转换日志信息

            infos.Sort();
            existsInfos.Sort();
            List<AuditHdrInfo> audits = new List<AuditHdrInfo>();
            int len = existsInfos.Count;
            for (int i = 0; i < len; i++)
            {
                AuditHdrInfo audit = existsInfos[i].ConvertAudit(infos[i], userInfo);
                audit.DetailInfo = audit.DetailInfo.Concat(FaultRateInfo.ConvertAuditDetail(existsInfos[i].FaultRates, infos[i].FaultRates)).ToList();
                //if(audit.DetailInfo.Count>0)
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
        /// 删除零件
        /// </summary>
        /// <param name="componentID">零件id</param>
        public void DeleteComponentByID(int componentID)
        {
            this.componentDao.DeleteComponentByID(componentID);
            this.faultRateDao.DeleteFaultRateByObjID(componentID, FaultRateInfo.ObjectType.Component);
        }
    }
}
