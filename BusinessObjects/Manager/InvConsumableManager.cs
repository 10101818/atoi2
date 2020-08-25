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
    /// 耗材manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class InvConsumableManager
    {
        private InvConsumableDao invConsumableDao = new InvConsumableDao();

        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="info">耗材信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>耗材id</returns>
        [TransactionAspect]
        public int SaveConsumable(InvConsumableInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.invConsumableDao.AddConsumable(info);
            }
            else
            {
                this.invConsumableDao.UpdateConsumable(info);
            }

            return info.ID;
        }

    }
}
