using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 广播manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class NoticeManager
    {
        private NoticeDao noticeDao = new NoticeDao();

        /// <summary>
        /// 设置广播进行轮播
        /// </summary>
        /// <param name="id">广播编号</param>
        /// <returns>广播信息</returns>
        [TransactionAspect]
        public NoticeInfo SetLoop(int id)
        {
            NoticeInfo info = noticeDao.SetNoticeAsLoop(id);
            return info;
        }

        /// <summary>
        /// 保存广播信息
        /// </summary>
        /// <param name="info">广播信息</param>
        /// <returns>广播编号</returns>
        [TransactionAspect]
        public int SaveNotice(NoticeInfo info)
        {
            if (info.ID>0)
                this.noticeDao.UpdateNotice(info);
            else
                info = this.noticeDao.AddNotice(info);
            if (info.IsLoop == true)
                this.noticeDao.SetNoticeAsLoop(info.ID);
            return info.ID;
        }
    }
}
