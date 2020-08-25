using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using BusinessObjects.DataAccess;
using BCWS.BusinessObjects.Util;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 用户manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class UserManager
    {
        private UserDao userDao = new UserDao();

        /// <summary>
        /// 通过用户名获取用户信息
        /// </summary>
        /// <param name="loginID">用户名</param>
        /// <returns>用户信息</returns>
        public UserInfo GetUserByLoginID(string loginID)
        {
            UserInfo info = this.userDao.GetUserByLoginID(loginID);
            if (info != null)
            {
                if (!string.IsNullOrEmpty(info.LoginPwd))
                    info.LoginPwd = EncryptionUtil.Decrypt(info.LoginPwd);
            }

            return info;
        }
        /// <summary>
        /// 通过编号获取用户信息
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <returns>用户信息</returns>
        public UserInfo GetUser(int id)
        {
            UserInfo info = this.userDao.GetUser(id);

            if (info != null && !string.IsNullOrEmpty(info.LoginPwd))
                info.LoginPwd = EncryptionUtil.Decrypt(info.LoginPwd);
            return info;
        }
        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>用户编号</returns>
        [TransactionAspect]
        public int SaveUser(UserInfo info)
        {
            string password = SQLUtil.TrimNull(info.LoginPwd);
            if (password.Length > 0)
                password = EncryptionUtil.Encrypt(password);

            info.LoginPwd = password;
            if (info.ID>0)
                this.userDao.UpdateUser(info);
            else
                info = this.userDao.AddUser(info);

            return info.ID;
        }
        /// <summary>
        /// 修改用户上次登录时间
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="sessionID">用户当前SessionID</param>
        [TransactionAspect]
        public void UpdateLoginDate(int id,string sessionID)
        {
            this.userDao.UpdateLastLoginInfo(id, sessionID);
        }
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <param name="loginPwd">新密码</param>
        [TransactionAspect]
        public void UpdatePassword(int id, string loginPwd)
        {
            string password = SQLUtil.TrimNull(loginPwd);
            if (password.Length > 0)
                password = EncryptionUtil.Encrypt(password);
            this.userDao.UpdatePassword(id, password);
        }

        #region "App"
        /// <summary>
        /// 用户登录修改上次登录记录
        /// </summary>
        /// <param name="info">用户信息</param>
        [TransactionAspect]
        public void Login4App(UserInfo info)
        {
            this.userDao.Login4App(info);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <param name="phoneVerifyID">手机验证码</param>
        /// <returns>用户信息</returns>
        [TransactionAspect]
        public UserInfo RegisterUser(UserInfo info, int phoneVerifyID)
        {
            info.LoginPwd = EncryptionUtil.Encrypt(info.LoginPwd);
            info.Role.ID = UserRole.User;
            info.IsActive = true;
            info.VerifyStatus.ID = BusinessObjects.Domain.VerifyStatus.Pending;

            info = this.userDao.AddUser(info);

            this.userDao.UpdatePhoneVerifyIsUsed(phoneVerifyID);

            return info;
        }
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <param name="appID">appID</param>
        /// <param name="appKey">appKey</param>
        /// <param name="period">短信有效期间隔</param>
        /// <param name="signature">短信签名</param>
        /// <param name="templateID">短信模板id</param>
        /// <returns>是否发送成功</returns>
        [TransactionAspect]
        public bool SendPhoneVerify(string mobilePhone, int appID, string appKey, int period, string signature, int templateID)
        {
            Random random = new Random();
            int verificationCode = random.Next(100000, 999999);

            //and the code to sms here.
            List<string> smsMessage = new List<string>();
            smsMessage.Add(verificationCode.ToString());
            smsMessage.Add(period.ToString());
            bool isSended = SendMsg2User(mobilePhone, smsMessage, appID, appKey, signature, templateID); 

            if (isSended == true)
                this.userDao.AddPhoneVerify(mobilePhone, verificationCode.ToString());

            return isSended;
        }
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="mobilePhone">手机号</param>
        /// <param name="smsMessage">短信内容</param>
        /// <param name="appID">appID</param>
        /// <param name="appKey">appKey</param>
        /// <param name="signature">短信签名</param>
        /// <param name="templateID">短信模板id</param>
        /// <returns>是否成功发送</returns>
        private bool SendMsg2User(string mobilePhone, List<string> smsMessage, int appID, string appKey, string signature, int templateID)
        {
            try
            {
                SmsSingleSender smsSender = new SmsSingleSender(appID, appKey);
                SmsSingleSenderResult smsResult = smsSender.SendWithParam("86", mobilePhone, templateID, smsMessage, signature, "", "");
                if (smsResult.result != 0)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error("SmsMultiSender has error: " + smsResult);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                return false;
            }
        }
        #endregion
    }
}
