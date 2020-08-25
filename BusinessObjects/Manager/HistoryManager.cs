using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 历史信息manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class HistoryManager
    {
        private HistoryDao historyDao = new HistoryDao();
    }
}
