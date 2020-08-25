using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 系统设置controller
    /// </summary>
    public class SystemController : BaseController
    {
        private NoticeManager noticeManager = new NoticeManager();
        private UserManager userManager = new UserManager();
        private ControlDao controlDao = new ControlDao();
        private NoticeDao noticeDao = new NoticeDao();

        /// <summary>
        /// 邮件设置页面
        /// </summary>
        /// <returns>邮件设置页面</returns>
        public ActionResult SmptEmail()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 短信设置页面
        /// </summary>
        /// <returns>短信设置页面</returns>
        public ActionResult Sms()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// App设置页面
        /// </summary>
        /// <returns>App设置页面</returns>
        public ActionResult App()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 异常信息设置页面
        /// </summary>
        /// <returns>异常信息设置页面</returns>
        public ActionResult ErrorMessageEdit()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 预警时间设置页面
        /// </summary>
        /// <returns>预警时间设置页面</returns>
        public ActionResult WarningTimeEdit()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }

        #region "Smtp"
        /// <summary>
        /// 获取系统设置信息（短信、邮箱）
        /// </summary>
        /// <returns>系统设置信息</returns>
        public JsonResult GetSmtpInfo()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<SmtpInfo> result = new ResultModel<SmtpInfo>();

            try
            {
                result.Data = ControlManager.GetSmtpInfo();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存系统设置信息
        /// </summary>
        /// <param name="info">系统设置内容</param>
        /// <returns>保存系统设置信息返回信息</returns>
        [HttpPost]
        public JsonResult SaveSmtpInfo(SmtpInfo info)
        {
            if (CheckSession() == false)
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
                ControlManager.UpdateSmtpInfo(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 获取系统设置信息（异常设置、预警时间）
        /// </summary>
        /// <returns>系统设置信息</returns>
        public JsonResult GetSettingInfo()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<SettingInfo> result = new ResultModel<SettingInfo>();

            try
            {
                result.Data = ControlManager.GetSettingInfo();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存异常设置
        /// </summary>
        /// <param name="info">异常设置内容</param>
        /// <returns>保存异常设置返回信息</returns>
        [HttpPost]
        public JsonResult SaveErrorMessage(SettingInfo info)
        {
            if (CheckSession() == false)
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
                ControlManager.UpdateErrorMessageInfo(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存预警时间设置
        /// </summary>
        /// <param name="info">预警时间设置内容</param>
        /// <returns>保存预警时间设置返回信息</returns>
        [HttpPost]
        public JsonResult SaveWarningTime(SettingInfo info)
        {
            if (CheckSession() == false)
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
                ControlManager.UpdateSaveWarningTimeInfo(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 测试邮箱
        /// </summary>
        /// <param name="smtpInfo">系统设置内容</param>
        /// <returns>1/0 是否成功发送邮件</returns>
        [HttpPost]
        public JsonResult TestEmail(SmtpInfo smtpInfo)
        {

            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<String> result = new ResultModel<String>();

            try
            {
                string mailSubject = "测试邮件";
                string mailBody = "这是一封测试邮件";

                bool send = EmailUtil.TestMail(mailSubject, mailBody, smtpInfo.AdminEmail, smtpInfo);
                if (send)
                {
                    result.Data = "1";
                }
                else
                {
                    result.Data = "0";
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return JsonResult(result);
        }

        #endregion

        #region "Sms"
        /// <summary>
        /// 测试短信
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <returns>1/0 是否成功发送短信</returns>
        [HttpPost]
        public JsonResult TestSms(string mobilePhone)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<String> result = new ResultModel<String>();

            try
            {
                bool send = this.userManager.SendPhoneVerify(mobilePhone, WebConfig.SMS_SDK_APP_ID, WebConfig.SMS_SDK_APP_KEY, WebConfig.SMS_PERIOD, WebConfig.SMS_SIGNATURE, WebConfig.SMS_TEMPLATEID);
                if (send)
                {
                    result.Data = "1";
                }
                else
                {
                    result.Data = "0";
                }

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return JsonResult(result);
        }
        #endregion

        #region "NoticeList"
        /// <summary>
        /// 广播列表页面
        /// </summary>
        /// <returns>广播列表页面</returns>
        public ActionResult NoticeList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }

        /// <summary>
        /// 获取广播列表信息
        /// </summary>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>广播列表信息</returns>
        public JsonResult QueryNoticeList(string filterField, string filterText, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<List<NoticeInfo>> result = new ResultModel<List<NoticeInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<NoticeInfo> infos = new List<NoticeInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.noticeDao.QueryNoticeCount(filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        infos = this.noticeDao.QueryNotices(filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                    }
                }
                else
                {
                    infos = this.noticeDao.QueryNotices(filterField, filterText,0,0);
                }

                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return JsonResult(result);
        }

        /// <summary>
        /// 获取需要轮播的广播信息
        /// </summary>
        /// <returns>广播信息</returns>
        public JsonResult GetCurrentLoopNotice()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<NoticeInfo> result = new ResultModel<NoticeInfo>();
            try
            {
                NoticeInfo infos = new NoticeInfo();
                infos = this.noticeDao.GetCurrentLoopNotice();

                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return JsonResult(result);
        }

        /// <summary>
        /// 通过广播编号获取广播信息
        /// </summary>
        /// <param name="id">广播编号</param>
        /// <returns>广播信息</returns>
        [HttpPost]
        public JsonResult GetNoticeByID(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<NoticeInfo> result = new ResultModel<NoticeInfo>();
            try
            {
                NoticeInfo infos = new NoticeInfo();
                infos = this.noticeDao.GetNoticeByID(id);

                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return JsonResult(result);
        }
        
        /// <summary>
        /// 设置广播轮播
        /// </summary>
        /// <param name="id">广播编号</param>
        /// <returns>广播内容</returns>
        [HttpPost]
        public JsonResult SetLoop(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<NoticeInfo> result = new ResultModel<NoticeInfo>();
            try
            {
                NoticeInfo editNoticeInfo = this.noticeManager.SetLoop(id);
                result.Data = editNoticeInfo;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出广播列表excel
        /// </summary>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>广播列表excel</returns>
        public ActionResult ExportNoticesList( string filterField, string filterText)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<NoticeInfo>> result = new ResultModel<List<NoticeInfo>>();
            try
            {
                List<NoticeInfo> infos = null;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                infos = this.noticeDao.QueryNotices(filterField, filterText,0,0);

                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("广播编号");
                dt.Columns.Add("广播名称");
                dt.Columns.Add("添加时间");
                dt.Columns.Add("广播内容");
                dt.Columns.Add("备注");
                dt.Columns.Add("是否轮播");


                foreach (NoticeInfo noticeInfo in infos)
                {
                    dt.Rows.Add(noticeInfo.OID, noticeInfo.Name, noticeInfo.CreatedDate, noticeInfo.Content, noticeInfo.Comments, noticeInfo.IsLoop ? '是' : '否');
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "广播列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存广播信息
        /// </summary>
        /// <param name="info">广播信息</param>
        /// <returns>保存广播信息返回信息</returns>
        [HttpPost]
        public JsonResult SaveNotice(NoticeInfo info)
        {
            if (CheckSession() == false)
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
                this.noticeManager.SaveNotice(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        #endregion
    }
}