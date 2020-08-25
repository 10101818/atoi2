using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// DispatchReportController
    /// </summary>
    public class DispatchReportController : BaseController
    {
        private DispatchManager dispatchManager = new DispatchManager();
        private UserDao userDao = new UserDao();
        private DispatchReportDao dispatchReportDao = new DispatchReportDao();
        private FileDao fileDao = new FileDao();
        private UploadFileManager uploadFileManager = new UploadFileManager();
        private SupplierDao supplierDao = new SupplierDao();

        /// <summary>
        /// 通过作业报告编号获取作业报告信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchReportID">作业报告编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>作业报告信息</returns>
        public JsonResult GetDispatchReport(int userID, string sessionID, int dispatchReportID)
        {
            ServiceResultModel<DispatchReportInfo> response = new ServiceResultModel<DispatchReportInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                DispatchReportInfo info = this.dispatchManager.GetDispatchReportByID(dispatchReportID);

                UserInfo user = null;
                if (CheckUser(userID, response, out user) == false) return MyJson(response, JsonRequestBehavior.AllowGet);


                if (info == null) response.SetFailed(ResultCodes.ParameterError, "作业报告不存在");
                else response.Data = info.Copy4App();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审批通过作业报告
        /// </summary>
        /// <param name="info">作业报告信息</param>
        /// <param name="userID">用户编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>审批通过作业报告返回编码</returns>
        [HttpPost]
        public JsonResult ApproveDispatchReport(DispatchReportInfo info, int userID, string sessionID) //comments是审批备注
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                if(info == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业报告不存在");
                }
                else if (info.Status.ID != DispatchReportInfo.DispatchReportStatus.Pending)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业报告状态非待审批");
                }
                else if (info.SolutionResultStatus.ID < DispatchReportInfo.SolutionResultStatuses.Allocating || info.SolutionResultStatus.ID > DispatchReportInfo.SolutionResultStatuses.Resolved)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业结果不存在");
                }
                else
                {
                    if (info.FileInfo != null)
                        info.FileInfo.FileContent = ParseBase64String(info.FileInfo.FileContent);
                    if (info.ReportComponent != null)
                    {
                        foreach (ReportComponentInfo item in info.ReportComponent)
                        {
                            if (item.FileInfos != null && item.FileInfos.Count > 0)
                            {
                                foreach (UploadFileInfo fileInfo in item.FileInfos)
                                {
                                    fileInfo.FileContent = ParseBase64String(fileInfo.FileContent);
                                }
                            }
                        }
                    }
                    this.dispatchManager.PassDispatchReport4App(info, user);
                }
                
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 审批拒绝作业报告
        /// </summary>
        /// <param name="info">作业报告信息</param>
        /// <param name="userID">用户编号</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>审批拒绝作业报告返回编码</returns>
        [HttpPost]
        public JsonResult RejectDispatchReport(DispatchReportInfo info,int userID, string sessionID) //comments是审批备注
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.SuperAdmin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);
                                                
                if (info == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业报告不存在");
                }
                else if (info.Status.ID != DispatchReportInfo.DispatchReportStatus.Pending)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业报告状态非待审批");
                }
                else if (string.IsNullOrEmpty(info.FujiComments))
                {
                    response.SetFailed(ResultCodes.ParameterError, "审批备注不能为空");
                }
                else
                {
                    if (info.FileInfo != null)
                        info.FileInfo.FileContent = ParseBase64String(info.FileInfo.FileContent);
                    if (info.ReportComponent != null)
                    {
                        foreach (ReportComponentInfo item in info.ReportComponent)
                        {
                            if (item.FileInfos != null && item.FileInfos.Count > 0)
                            {
                                foreach (UploadFileInfo fileInfo in item.FileInfos)
                                {
                                    fileInfo.FileContent = ParseBase64String(fileInfo.FileContent);
                                }
                            }
                        }
                    }
                    this.dispatchManager.RejectDispatchReport4App(info,user);
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存作业报告信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="dispatchReport">作业报告信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>作业报告信息</returns>
        [HttpPost]
        public JsonResult SaveDispatchReport(int userID, string sessionID, DispatchReportInfo dispatchReport)
        {
            ServiceResultModel<int> response = new ServiceResultModel<int>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, response, out user, UserRole.Admin) == false) return MyJson(response, JsonRequestBehavior.AllowGet);

                DispatchInfo dispatchInfo = this.dispatchManager.GetDispatchByID(dispatchReport.Dispatch.ID);

                if (dispatchInfo == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "派工单不存在");
                }
                else if (dispatchInfo.Engineer.ID != userID)
                {
                    response.SetFailed(ResultCodes.ParameterError, "派工人不正确");
                }
                else if (dispatchInfo.Status.ID != BusinessObjects.Domain.DispatchInfo.Statuses.Responded && dispatchInfo.Status.ID != BusinessObjects.Domain.DispatchInfo.Statuses.Pending)
                {
                    response.SetFailed(ResultCodes.ParameterError, "派工单当前状态不可进行该操作");
                }
                else if (dispatchInfo.DispatchReport.ID != dispatchReport.ID)
                {
                    response.SetFailed(ResultCodes.ParameterError, "派工单作业报告编号不匹配");
                }
                else if (dispatchReport.SolutionResultStatus.ID < DispatchReportInfo.SolutionResultStatuses.Allocating || dispatchReport.SolutionResultStatus.ID > DispatchReportInfo.SolutionResultStatuses.Resolved)
                {
                    response.SetFailed(ResultCodes.ParameterError, "作业结果不存在");
                }
                else if (dispatchInfo.DispatchReport.Status.ID != DispatchReportInfo.DispatchReportStatus.New && dispatchInfo.DispatchReport.Status.ID != 0)
                {
                    response.SetFailed(ResultCodes.ParameterError, "当前作业报告状态非新建");
                }
                else if (dispatchReport.Status.ID != DispatchReportInfo.DispatchReportStatus.Pending && dispatchReport.Status.ID != DispatchReportInfo.DispatchReportStatus.New)
                {
                    response.SetFailed(ResultCodes.ParameterError, "更新作业报告状态非待审批或新建");
                }
                else
                {
                    if (dispatchReport.FileInfo != null)
                        dispatchReport.FileInfo.FileContent = ParseBase64String(dispatchReport.FileInfo.FileContent);
                    if (dispatchReport.ReportComponent != null)
                    {
                        foreach (ReportComponentInfo info in dispatchReport.ReportComponent)
                        {
                            if (info.FileInfos != null && info.FileInfos.Count > 0)
                            {
                                foreach (UploadFileInfo fileInfo in info.FileInfos)
                                {
                                    fileInfo.FileContent = ParseBase64String(fileInfo.FileContent);
                                }
                            }
                        }
                    }
                    response.Data = this.dispatchManager.SaveDispatchReport4App(dispatchReport, user);
                }
                
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过附件编号删除作业报告附件
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="fileID">附件编号</param>
        /// <returns>删除作业报告附件返回编码</returns>
        [HttpPost]
        public JsonResult DeleteFileByID(int userID, string sessionID, int fileID)
        {
            ServiceResultModelBase response = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                if (fileID > 0)
                {
                    this.uploadFileManager.DeleteUploadFileByID(ObjectTypes.DispatchReport, fileID);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过作业报告附件编号下载附件
        /// </summary>
        /// <param name="id">附件编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>下载附件返回编码</returns>
        public JsonResult DownloadUploadFile(int userID, string sessionID, int id)
        {
            ServiceResultModel<string> response = new ServiceResultModel<string>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UploadFileInfo file = this.fileDao.GetFileByID(ObjectTypes.DispatchReport, id);

                if (file == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                }
                else
                {
                    string path = Path.Combine(Constants.DispatchReportFolder, file.GetFileName()); 
                    if (!System.IO.File.Exists(path))
                    {
                        response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                    }
                    else
                    {
                        byte[] arr = System.IO.File.ReadAllBytes(path);
                        response.Data = Convert.ToBase64String(arr);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过零配件附件编号下载零配件附件
        /// </summary>
        /// <param name="id">零配件附件编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param> 
        /// <returns>下载零配件附件返回编码</returns>
        public JsonResult DownloadAccessoryFile(int userID, string sessionID, int id)
        {
            ServiceResultModel<string> response = new ServiceResultModel<string>();
            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                UploadFileInfo file = this.fileDao.GetFileByID(ObjectTypes.ReportAccessory, id);

                if (file == null)
                {
                    response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                }
                else
                {
                    string path = Path.Combine(Constants.ReportAccessoryFolder, file.GetFileName());
                    if (!System.IO.File.Exists(path))
                    {
                        response.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                    }
                    else
                    {
                        byte[] arr = System.IO.File.ReadAllBytes(path);
                        response.Data = Convert.ToBase64String(arr);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="typeID">供应商类型</param>
        /// <param name="status">供应商状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页展示信息条数</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>供应商信息</returns>
        public JsonResult GetSuppliers(int userID, string sessionID, int typeID = 0, int status = -1, string filterField = "s.ID", string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<SupplierInfo>> response = new ServiceResultModel<List<SupplierInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                response.Data = this.supplierDao.QuerySupplier(typeID, status, filterField, filterText, "s.ID", true, curRowNum, pageSize);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据派工类型获取作业报告类型
        /// </summary>
        /// <param name="id">派工类型</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>可选的作业报告类型</returns>
        public JsonResult GetDispatchReportType(int userID, string sessionID, int id)
        {
            ServiceResultModel<List<KeyValueInfo>> response = new ServiceResultModel<List<KeyValueInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, response)) return MyJson(response, JsonRequestBehavior.AllowGet);
                response.Data = DispatchReportInfo.DispatchReportTypes.GetDispatchReportTypes(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                response.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(response, JsonRequestBehavior.AllowGet);
        }
    }
}