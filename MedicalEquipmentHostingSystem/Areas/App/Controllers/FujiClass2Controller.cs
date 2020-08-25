using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// FujiClass2Controller
    /// </summary>
    public class FujiClass2Controller : BaseController
    {
        private FujiClassDao fujiClassDao = new FujiClassDao();

        /// <summary>
        /// 获取富士II类信息
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>富士II类信息</returns>
        public JsonResult GetFujiClass2(int userID, string sessionID)
        {
            ServiceResultModel<List<FujiClass2Info>> result = new ServiceResultModel<List<FujiClass2Info>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false) return MyJson(result, JsonRequestBehavior.AllowGet);

                List<FujiClass2Info> data = this.fujiClassDao.GetFujiClass2();
                int index = data.FindIndex(info => info.ID == -1);
                if (index != -1) data.SwapOrder(index, data.Count - 1);
                result.Data = data;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
    }
}