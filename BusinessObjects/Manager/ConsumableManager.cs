using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 耗材manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ConsumableManager
    {
        private ConsumableDao consumableDao = new ConsumableDao();
        private FujiClassDao fujiClassDao = new FujiClassDao();

        private AuditManager auditManager = new AuditManager();

        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <param name="userInfo">操作者</param>
        /// <returns>耗材id</returns>
        public int SaveConsumable(ConsumableInfo info, UserInfo userInfo)
        {
            if (info.ID == 0) info.ID = this.consumableDao.AddConsumable(info);
            else
            {
                ConsumableInfo existingInfo = this.consumableDao.GetConsumableByID(info.ID);
                info.FujiClass2.Name = this.fujiClassDao.GetFujiClass2ByID(info.FujiClass2.ID).Name;
                DataTable dtField = existingInfo.GetChangedFields(info);

                if (dtField.Rows.Count > 0)
                {
                    this.consumableDao.UpdateConsumable(info);

                    this.auditManager.AddAuditLog(userInfo.ID, ObjectTypes.Component, info.ID, dtField);
                }
            }

            return info.ID;
        }

        /// <summary>
        /// 批量修改耗材信息
        /// </summary>
        /// <param name="infos">耗材信息列表</param>
        /// <param name="userInfo">操作者</param>
        [TransactionAspect]
        public void UpdateConsumables(List<ConsumableInfo> infos, UserInfo userInfo)
        {
            foreach (ConsumableInfo info in infos)
            {
                this.SaveConsumable(info, userInfo);
            }
        }
    }
}
