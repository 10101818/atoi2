using System;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;

namespace BusinessObjects.Util
{
    /// <summary>
    /// 邮件
    /// </summary>
    public static class EmailUtil
    {
        #region "Email cache"
        private static Dictionary<string, DateTime> SendEmailList = new Dictionary<string, DateTime>();
        
        /// <summary>
        /// 检查邮件是否已经包含
        /// </summary>
        /// <param name="emailBody">邮件内容</param>
        /// <returns>是否已经包含</returns>
        private static Boolean CheckEmail(string emailBody)
        {
            DateTime cutOffDate = DateTime.Now.AddHours(-1);
            List<string> removeList = new List<string>();

            foreach (string content in SendEmailList.Keys)
            {
                if (SendEmailList[content].CompareTo(cutOffDate) < 0)
                    removeList.Add(content);
            }

            foreach (string remove in removeList)
            {
                SendEmailList.Remove(remove);
            }

            return SendEmailList.ContainsKey(emailBody);
        }
        #endregion

        /// <summary>
        /// 给工程师发邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        public static void SendMailToAdmin(string mailSubject, string mailBody)
        {
            try
            {
                if (CheckEmail(mailBody) == true) return;

                SmtpInfo smtpInfo = ControlManager.GetSmtpInfo();

                if (SentMailWithRetry(mailSubject, mailBody, smtpInfo.AdminEmail, smtpInfo) == true)
                {
                    SendEmailList.Add(mailBody, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, string.Format("Error happend in SendMail Method: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="emailRecipient">收件人</param>
        /// <param name="attachments">附件</param>
        public static void SendMailAsyn(string mailSubject, string mailBody, string emailRecipient = "", params string[] attachments)
        {
            try
            {
                SmtpInfo smtpInfo = ControlManager.GetSmtpInfo();

                if (string.IsNullOrEmpty(emailRecipient)) emailRecipient = smtpInfo.AdminEmail;

                Task.Factory.StartNew(() =>
                {
                    SentMailWithRetry(mailSubject, mailBody, emailRecipient, smtpInfo, attachments);
                });     
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, string.Format("Error happend in SendMail Method: {0}", ex.Message));
            }
        }
        
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="emailRecipient">收件人</param>
        /// <param name="attachments">附件</param>
        /// <returns>是否发送成功</returns>
        public static Boolean SendMail(string mailSubject, string mailBody, string emailRecipient = "", params string[] attachments)
        {
            try
            {
                SmtpInfo smtpInfo = ControlManager.GetSmtpInfo();

                if (string.IsNullOrEmpty(emailRecipient)) emailRecipient = smtpInfo.AdminEmail;

                return SentMailWithRetry(mailSubject, mailBody, emailRecipient, smtpInfo, attachments);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, string.Format("Error happend in SendMail Method: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 测试发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="emailRecipient">收件人</param>
        /// <param name="smtpInfo">邮件信息</param>
        /// <returns>是否发送成功</returns>
        public static Boolean TestMail(string mailSubject, string mailBody, string emailRecipient, SmtpInfo smtpInfo)
        {
            return SentMailInternal(mailSubject, mailBody, emailRecipient, smtpInfo); 
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="emailRecipient">收件人</param>
        /// <param name="smtpInfo">邮件信息</param>
        /// <param name="attachments">附件</param>
        /// <returns>是否发送成功</returns>
        private static Boolean SentMailWithRetry(string mailSubject, string mailBody, string emailRecipient, SmtpInfo smtpInfo, params string[] attachments)
        {
            bool result = SentMailInternal(mailSubject, mailBody, emailRecipient, smtpInfo, attachments);

            if (result == false)
            {
                Thread.Sleep(2000);
                result = SentMailInternal(mailSubject, mailBody, emailRecipient, smtpInfo, attachments);
            }

            return result;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubject">邮件主题</param>
        /// <param name="mailBody">邮件内容</param>
        /// <param name="emailRecipient">收件人</param>
        /// <param name="smtpInfo">邮件信息</param>
        /// <param name="attachments">附件</param>
        /// <returns>是否发送成功</returns>
        private static Boolean SentMailInternal(string mailSubject, string mailBody, string emailRecipient, SmtpInfo smtpInfo, params string[] attachments)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                delegate(object s, X509Certificate certificate,
                         X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                // Instantiate a new instance of SmtpClient
                using (SmtpClient smtpClient = new SmtpClient(smtpInfo.Host, smtpInfo.Port))
                {
                    smtpClient.EnableSsl = smtpInfo.UseSsl;
                    smtpClient.UseDefaultCredentials = false;

                    NetworkCredential networkCredential = new NetworkCredential(smtpInfo.UserName, smtpInfo.Pwd);
                    smtpClient.Credentials = networkCredential;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        //the mail message
                        mailMessage.From = new MailAddress(smtpInfo.EmailFrom);

                        IList<MailAddress> mailAddress = new List<MailAddress>();
                        foreach (string addr in emailRecipient.Split(';'))
                        {
                            mailMessage.To.Add(addr);
                        }
                        mailMessage.Subject = Constants.SYSTEM_NAME + " - " + mailSubject;

                        foreach (string attachment in attachments)
                        {
                            mailMessage.Attachments.Add(new Attachment(attachment));
                        }

                        // Set the body of the mail message
                        mailMessage.Body = mailBody;

                        // Set the format of the mail message body as HTML
                        mailMessage.IsBodyHtml = true;

                        // Set the priority of the mail message to normal
                        mailMessage.Priority = MailPriority.Normal;

                        smtpClient.Send(mailMessage);
                    }
                }

                NLog.LogManager.GetCurrentClassLogger().Info(string.Format("Successfully sent email to {0} with Subject: {1}, Body: {2}. ", emailRecipient, mailSubject, mailBody));
                return true;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, string.Format("Error happend in sending email with Subject: {0}, Body: {1}, Recipients; {2}. The error is {3}", mailSubject, mailBody, emailRecipient, ex.Message));
                return false;
            }
        }
    }
}
