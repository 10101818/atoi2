using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 文件manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public  class UploadFileManager
    {
        private FileDao fileDao = new FileDao();
        private EquipmentDao equipmentDao = new EquipmentDao();

        /// <summary>
        /// 获取下载附件时的文件名
        /// </summary>
        /// <param name="info">附件信息</param>
        /// <returns>格式化后的附件名</returns>
        public string GetDownloadFileName(UploadFileInfo info)
        {
            string name = info.GetDisplayFileName();

            if (name == "") return info.FileName;

            if (ObjectTypes.Equipment.Equals(info.ObjectTypeId))
            {
                EquipmentInfo equipmentInfo = this.equipmentDao.GetEquipmentByID(info.ObjectID);
                name = string.Format("{0}_{1}", name, equipmentInfo.AssetCode);
            }
            else
            {
                name = string.Format("{0}_{1}", LookupManager.GetObjectOID(info.ObjectTypeId, info.ObjectID), name);
            }
           
            return name + new FileInfo(info.FileName).Extension;
        }

        /// <summary>
        /// 上传附件信息
        /// </summary>
        /// <param name="info">附件信息</param>
        /// <returns>附件信息</returns>
        [TransactionAspect]
        public UploadFileInfo SaveUploadFile(UploadFileInfo info)
        {
            info.FileName = new FileInfo(info.FileName).Name;
            if (info.ID > 0)
            {
                UploadFileInfo existingInfo = this.fileDao.GetFileByID(info.ObjectTypeId, info.ID);
                FileUtil.DeleteFile(ObjectTypes.GetFileFolder(info.ObjectTypeId), existingInfo.GetFileName());

                fileDao.UpdateFile(info.ObjectTypeId, info);
            }
            else
            {
                info = fileDao.AddFile(info.ObjectTypeId, info);               
            }

            byte[] fileContent = Convert.FromBase64String(info.FileContent);
            FileUtil.SaveFile(fileContent, ObjectTypes.GetFileFolder(info.ObjectTypeId), info.GetFileName());

            return info;
        }

        /// <summary>
        /// 根据附件类型及编号删除附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="ID">附件编号</param>
        [TransactionAspect]
        public void DeleteUploadFileByID(int objectTypeId, int ID)
        {
            UploadFileInfo existingInfo = this.fileDao.GetFileByID(objectTypeId, ID);
            if (existingInfo == null) return;

            this.fileDao.DeleteFileByID(objectTypeId, existingInfo.ID);
            FileUtil.DeleteFile(ObjectTypes.GetFileFolder(objectTypeId), existingInfo.GetFileName());
        }
    }
}
