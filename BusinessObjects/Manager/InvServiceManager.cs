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
    /// 服务manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class InvServiceManager
    {
        private InvServiceDao invServiceDao = new InvServiceDao();

        /// <summary>
        /// 保存服务信息
        /// </summary>
        /// <param name="info">服务信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>服务id</returns>
        [TransactionAspect]
        public int SaveService(InvServiceInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.invServiceDao.AddService(info);
            }
            else
            {
                this.invServiceDao.UpdateService(info);
            }

            return info.ID;
        }

    }
}
