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
    /// 供应商manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class SupplierManager
    {
        private SupplierDao supplierDao = new SupplierDao();
        private FileDao fileDao = new FileDao();
        private UploadFileManager fileManager = new UploadFileManager();
        private AuditManager auditManager = new AuditManager();

        /// <summary>
        /// 根据供应商编号获取供应商信息
        /// </summary>
        /// <param name="id">供应商编号</param>
        /// <returns>供应商信息</returns>
        public SupplierInfo GetSupplier(int id)
        {
            SupplierInfo info = this.supplierDao.GetSupplier(id);
            if (info != null)
            {
                info.SupplierFile = this.fileDao.GetFiles(ObjectTypes.Supplier, info.ID);
            }

            return info;
        }
        /// <summary>
        /// 保存供应商信息
        /// </summary>
        /// <param name="supplier">供应商信息</param>
        /// <param name="files">供应商附件信息</param>
        /// <param name="user">操作者</param>
        /// <returns>供应商编号</returns>
        [TransactionAspect]
        public int SaveSupplier(SupplierInfo supplier, List<UploadFileInfo> files, UserInfo user)
        {
            if (supplier.ID == 0)
            {
                supplier.ID = this.supplierDao.AddSupplier(supplier);
            }
            else
            {
                SupplierInfo existingInfo = this.supplierDao.GetSupplier(supplier.ID);
                DataTable dtField = existingInfo.GetChangedFields(supplier);
                if (dtField.Rows.Count > 0)
                {
                    this.supplierDao.UpdateSupplier(supplier);

                    this.auditManager.AddAuditLog(user.ID, ObjectTypes.Supplier, supplier.ID, dtField);
                }

            }
            if (files != null && files.Count > 0)
            {
                foreach (UploadFileInfo file in files)
                {
                    file.ObjectID = supplier.ID;
                    file.ObjectTypeId = ObjectTypes.Supplier;
                    this.fileManager.SaveUploadFile(file);
                }
            }
            return supplier.ID;
        }

    }

}
