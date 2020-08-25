using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 附件controller
    /// </summary>
    public class UploadFileController : BaseController
    {
        private UploadFileManager fileManager = new UploadFileManager();
        private FileDao fileDao = new FileDao();

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="info">附件信息</param>
        /// <returns>附件信息</returns>
        public JsonResult UploadFile(UploadFileInfo info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<int> result = new ResultModel<int>();
            try
            {
                if (string.IsNullOrEmpty(info.FileContent))
                {
                    result.SetFailed(ResultCodes.BusinessError, "文件为空");
                }
                else
                {
                    if (info.ObjectID == 0)
                    {
                        if (info.ID != 0) DeleteUploadFileInSession(info.ID);

                        result.Data = SaveUploadFileInSession(info);
                    }
                    else
                    {
                        result.Data = this.fileManager.SaveUploadFile(info).ID;
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        ///  下载附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="id">附件编号</param>
        /// <returns>附件信息</returns>
        public ActionResult DownloadUploadFile(int objectTypeId, int id)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (id < 0)
                {
                    UploadFileInfo file = GetUploadFileInSession(id);

                    string fileName = new FileInfo(file.FileName).Name;
                    byte[] fileContent = Convert.FromBase64String(file.FileContent);

                    return File(fileContent, System.Web.MimeMapping.GetMimeMapping(fileName), fileName);
                }
                else
                {
                    string filePath = null;
                    UploadFileInfo file = this.fileDao.GetFileByID(objectTypeId, id);
                    filePath = Path.Combine(ObjectTypes.GetFileFolder(objectTypeId), file.GetFileName());
                    Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                    file.FileName = this.fileManager.GetDownloadFileName(file);
                    return File(filePath, System.Web.MimeMapping.GetMimeMapping(file.FileName), file.FileName);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="objectTypeId">附件对象类型ID</param>
        /// <param name="id">附件编号</param>
        /// <returns>删除附件返回信息</returns>
        public JsonResult DeleteUploadFile(int objectTypeId, int id)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModelBase result = new ResultModelBase();
            try
            {
                if (id < 0)
                {
                    DeleteUploadFileInSession(id);
                }
                else
                {
                    this.fileManager.DeleteUploadFileByID(objectTypeId, id);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
    }
}