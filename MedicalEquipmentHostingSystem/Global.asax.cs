using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem;
using MedicalEquipmentHostingSystem.App_Start;

namespace MedicalEquipmentHostingSystem
{
    /// <summary>
    /// MvcApplication
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class MvcApplication : System.Web.HttpApplication
    {
        Timer eventTimer = null;
        /// <summary>
        /// Applications the start.
        /// </summary>
        protected void Application_Start()
        {
            ConnectionUtil.Init(WebConfig.SQL_CONNECTION_STRING);
            JPushManager.SetKeyInfo(WebConfig.JPUSH_APP_KEY, WebConfig.JPUSH_MASTER_SECRET);
            LookupManager.DoInit();
            Constants.SetFolders(Server.MapPath("~/"));

            CreateTimer();

            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private void CreateTimer()
        {
            int minutes = WebConfig.BACKGROUP_PROCESS_INTERVAL;

            eventTimer = new Timer(1000 * 60 * minutes);
            eventTimer.Elapsed += new ElapsedEventHandler(DoTimerCallback);
            eventTimer.AutoReset = true;
            eventTimer.Enabled = true;

            NLog.LogManager.GetCurrentClassLogger().Info(string.Format("Create event timer with Period: {0} Minute(s).", minutes));
        }

        private void DoTimerCallback(object source, ElapsedEventArgs e)
        {
            try
            {
                this.eventTimer.Enabled = false;

                string nowStr = DateTime.Now.ToString("HHmm");
                if (nowStr.CompareTo("00") >= 0 && nowStr.CompareTo("2400") <= 0)
                {
                    NLog.LogManager.GetCurrentClassLogger().Info(string.Format("DoTimerCallback run start at {0}", DateTime.Now));

                    new RequestGenerator().DoProcess();

                    NLog.LogManager.GetCurrentClassLogger().Info(string.Format("DoTimerCallback run end at {0}", DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "DoTimerCallback have error: " + ex.Message);
            }
            finally
            {
                this.eventTimer.Enabled = true;
            }
        }
    }
}
