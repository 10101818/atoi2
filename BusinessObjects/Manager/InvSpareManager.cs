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
    /// 备用机manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class InvSpareManager
    {
        private InvSpareDao invSpareDao = new InvSpareDao();

        /// <summary>
        /// 保存备用机信息
        /// </summary>
        /// <param name="info">备用机信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>备用机id</returns>
        [TransactionAspect]
        public int SaveSpare(InvSpareInfo info, UserInfo userInfo)
        {
            if (info.ID == 0)
            {
                info.ID = this.invSpareDao.AddSpare(info);
            }
            else
            {
                this.invSpareDao.UpdateSpare(info);
            }

            return info.ID;
        }

    }
}
