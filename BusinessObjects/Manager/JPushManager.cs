using Jiguang.JPush;
using Jiguang.JPush.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 极光推送manager
    /// </summary>
    public class JPushManager
    {
        private static string AppKey;
        private static string MasterSecret;

        /// <summary>
        /// 设置极光推送的appKey和
        /// </summary>
        /// <param name="appKey">appKey</param>
        /// <param name="masterSecret">masterSecret</param>
        public static void SetKeyInfo(string appKey, string masterSecret)
        {
            AppKey = appKey;
            MasterSecret = masterSecret;
        }

        /// <summary>
        /// 设置短信推送信息
        /// </summary>
        /// <param name="message">推送内容</param>
        /// <param name="registrationIds">管理员的手机注册码</param>
        public static void PushMessage(string message, List<string> registrationIds)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            
            PushPayload pushPayload = new PushPayload()
            {
                Platform = new List<string> { "android", "ios" },
                Audience = new Audience() {RegistrationId = registrationIds},
                Notification = new Notification
                {
                    Alert = message,
                },
                Options = new Options
                {
                    //IsApnsProduction = true // 设置 iOS 推送生产环境。不设置默认为开发环境。
                }
            };
            var response = client.SendPush(pushPayload);

            NLog.LogManager.GetCurrentClassLogger().Info(response.Content);
        }
    }
}
