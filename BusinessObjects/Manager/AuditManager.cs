using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using BusinessObjects.DataAccess;
using System.IO;
using System.Data;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// AuditManager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class AuditManager
    {
        private AuditDao auditLogDao = new AuditDao();
        private SupplierDao supplierDao = new SupplierDao();
        private EquipmentDao equipmentDao = new EquipmentDao();

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="userID">操作者id</param>
        /// <param name="objectTypeID">操作对象类型</param>
        /// <param name="objectID">对象ID</param>
        /// <param name="dt">修改字段信息</param>
        /// <param name="operation">操作方式</param>
        public void AddAuditLog(int userID, int objectTypeID, int objectID, DataTable dt, int operation = AuditHdrInfo.AuditOperations.Update)
        {
            AuditHdrInfo auditHdrInfo = new AuditHdrInfo();
            auditHdrInfo.TransUser.ID = userID;
            auditHdrInfo.ObjectType.ID = objectTypeID;
            auditHdrInfo.ObjectID = objectID;

            auditHdrInfo.Operation.ID = operation;
            auditHdrInfo = this.auditLogDao.AddAuditHdr(auditHdrInfo);

            this.auditLogDao.AddAuditDetail(auditHdrInfo.ID,dt);
        }
        /// <summary>
        /// 根据日志ID获取日志信息
        /// </summary>
        /// <param name="auditID">日志ID</param>
        /// <param name="type">父对象类型ID</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo GetAuditInfo(int auditID,int type)
        {
            AuditHdrInfo info = this.auditLogDao.GetAuditHdrLog(auditID);
            info.DetailInfo = this.auditLogDao.GetAuditDetailLog(auditID, type);

            return info;
        }

    }
}
