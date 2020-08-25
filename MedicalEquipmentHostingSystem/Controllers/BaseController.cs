using BusinessObjects.Domain;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Web.Mvc;
using MedicalEquipmentHostingSystem.App_Start;
using BusinessObjects.Util;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MedicalEquipmentHostingSystem.Areas.App.Models;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// BaseController
    /// </summary>
    [NoCache]
    [AllowCrossSiteJson]
    [ValidateInput(false)]
    public class BaseController : Controller
    {
        #region "session"        
        /// <summary>
        /// The session key user
        /// </summary>
        public const string SESSION_KEY_USER = "LoginUser";
        private const string SESSION_KEY_FILES = "UploadedFiles";
        private const string SESSION_KEY_VALUATION = "ValuationInfos";
        private BusinessObjects.DataAccess.UserDao userDao = new BusinessObjects.DataAccess.UserDao();

        /// <summary>
        /// 是否删除登录信息
        /// </summary>
        /// <param name="clearCache">是否删除登录信息</param>
        /// <returns>是否删除登录信息</returns>
        public bool CheckSession(bool clearCache = true)
        {
            if (clearCache) Session.Remove(SESSION_KEY_FILES);

            if (Session[SESSION_KEY_USER] == null)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 是否删除登录信息
        /// </summary>
        /// <param name="clearCache">是否删除登录信息</param>
        /// <returns>是否删除登录信息</returns>
        public bool CheckSessionID()
        {
            bool flag = true;//debugger
            if (WebConfig.GLOBAL_SESSIONID_FLAG)
            {
                UserInfo user = GetLoginUser();
                string lastSessionID = userDao.GetWebSessionID(user.ID);
                flag = string.IsNullOrEmpty(lastSessionID) || lastSessionID.Equals(user.WebSessionID);
            }
            return flag;
        }
        /// <summary>
        /// 保存登录信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public void SaveSession(UserInfo userInfo)
        {
            Session[SESSION_KEY_USER] = userInfo;
        }
        /// <summary>
        /// 获取登录的用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        public UserInfo GetLoginUser()
        {
            return (UserInfo)Session[SESSION_KEY_USER];
        }
        #endregion

        #region "cache temp files"        
        /// <summary>
        /// 保存上传的附件信息至session
        /// </summary>
        /// <param name="file">附件信息</param>
        /// <returns>附件编号</returns>
        public int SaveUploadFileInSession(UploadFileInfo file)
        {
            if (Session[SESSION_KEY_FILES] == null) Session[SESSION_KEY_FILES] = new List<UploadFileInfo>();

            int id = -1;
            List<UploadFileInfo> files = (List<UploadFileInfo>)Session[SESSION_KEY_FILES];
            if (files.Count > 0) id = (from UploadFileInfo temp in files select temp.ID).Min() - 1;

            file.ID = id;
            files.Add(file);

            return file.ID;   
        }
        /// <summary>
        /// 获取session中上传的附件信息
        /// </summary>
        /// <param name="id">附件编号</param>
        /// <returns>附件信息</returns>
        public UploadFileInfo GetUploadFileInSession(int id)
        {
            if (Session[SESSION_KEY_FILES] == null) return null;

            List<UploadFileInfo> files = (List<UploadFileInfo>)Session[SESSION_KEY_FILES];

            return (from UploadFileInfo temp in files where temp.ID == id select temp).FirstOrDefault();
        }
        /// <summary>
        /// 获取session中上传的附件信息
        /// </summary>
        /// <returns>附件信息</returns>
        public List<UploadFileInfo> GetUploadFilesInSession()
        {
            if (Session[SESSION_KEY_FILES] == null) return null;

            List<UploadFileInfo> infos = (List<UploadFileInfo>)Session[SESSION_KEY_FILES];

            int i = -1;
            infos.ForEach(info => info.ID = i--);

            return infos;
        }
        /// <summary>
        /// 删除session中上传的附件信息
        /// </summary>
        /// <param name="id">附件编号</param>
        public void DeleteUploadFileInSession(int id)
        {
            if (Session[SESSION_KEY_FILES] == null) return;

            List<UploadFileInfo> files = (List<UploadFileInfo>)Session[SESSION_KEY_FILES];
            for (int i = 0; i < files.Count ; i++)
            {
                if (files[i].ID == id)
                {
                    files.RemoveAt(i);
                    return;
                }
            }
        }

        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="module">ResultModelBase</param>
        /// <returns>JsonResult</returns>
        public JsonResult JsonResult(ResultModelBase module)
        {
            JsonResult jr = Json(module, JsonRequestBehavior.AllowGet);
            jr.MaxJsonLength = Int32.MaxValue;
            return jr;
        }

        /// <summary>
        /// 转换base64数据的格式
        /// </summary>
        /// <param name="base64Str">原始base64数据</param>
        /// <returns>转换后base64数据</returns>
        public string ParseBase64String(string base64Str)
        {
            if (!string.IsNullOrEmpty(base64Str) && base64Str.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                if (base64Str.IndexOf("base64,") >= 0)
                    return base64Str.Substring(base64Str.IndexOf("base64,") + ("base64,").Length).Trim();
                else
                    return string.Empty;
            }
            else
            {
                return base64Str;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="base64String">base64数据</param>
        /// <returns>文件信息</returns>
        public ActionResult OpenLocalFile(string fileName, string base64String)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = new FileInfo(fileName).Name;
                    base64String = ParseBase64String(base64String);

                    byte[] fileContent = Convert.FromBase64String(base64String);

                    Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                    return File(fileContent, System.Web.MimeMapping.GetMimeMapping(fileName), fileName);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }

            return null;
        }

        /// <summary>
        /// 获取url
        /// </summary>
        /// <returns>url</returns>
        public string GetBaseUrl()
        {
            return string.Format("{0}/{1}", Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.Unescaped).TrimEnd('/'), Request.ApplicationPath.TrimStart('/'));
        }

        /// <summary>
        /// 获取邮件地址
        /// </summary>
        /// <param name="controller">controller</param>
        /// <param name="action">方法名称</param>
        /// <returns>邮件地址</returns>
        public string GetLink4Email(string controller, string action)
        {
            string baseUrl = GetBaseUrl();

            string token = EncryptionUtil.Encrypt(Url.Action(action, controller));

            string fullUrl = string.Format("{0}/{1}/{2}?token={3}", baseUrl.TrimEnd('/'), ConstDefinition.HOME_CONTROLLER, ConstDefinition.HOME_ACTION, token);

            return fullUrl;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns>解密后密码</returns>
        public string CheckToken()
        {
            try
            {
                string token = Request.QueryString["token"];
                if (!string.IsNullOrEmpty(token))
                    return EncryptionUtil.Decrypt(token);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
            }
            return null;
        }

        ///use json.net to generate json string        
        /// <summary>
        /// JsonResult
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>JsonResult</returns>
        protected JsonResult JsonNetResult(object data, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
            };
        }

        #region 估价执行缓存已修改元素操作
        /// <summary>
        /// 保存session
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="infos">session信息</param>
        public void SaveValSession<T>(Dictionary<string, T> infos)
        {
            if (Session[SESSION_KEY_VALUATION] == null)
                Session[SESSION_KEY_VALUATION] = infos;
            else
            {
                Dictionary<string, T> valuations = (Dictionary<string, T>)Session[SESSION_KEY_VALUATION];
                foreach(string key in infos.Keys)
                { 
                    valuations[key] = infos[key]; 
                }
                Session[SESSION_KEY_VALUATION] = valuations;
            }
        }
        /// <summary>
        /// 获取session数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>session数据</returns>
        public Dictionary<string, T> GetValSession<T>()
        {
            if (Session[SESSION_KEY_VALUATION] == null) return null;
            Dictionary<string, T> valuations = (Dictionary<string, T>)Session[SESSION_KEY_VALUATION];
            return valuations;
        }
        /// <summary>
        /// 清除session
        /// </summary>
        public void ClearValSession()
        {
            Session.Remove(SESSION_KEY_VALUATION);
        }
        #endregion
    }
}
