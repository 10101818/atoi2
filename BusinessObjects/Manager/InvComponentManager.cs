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
    public class InvComponentManager
    {
        private InvComponentDao invComponentDao = new InvComponentDao();

        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="info">零件信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>零件id</returns>
        [TransactionAspect]
        public int SaveComponent(InvComponentInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.invComponentDao.AddComponent(info);
            }
            else
            {
                this.invComponentDao.UpdateComponent(info);
            }

            return info.ID;
        }

    }
}
