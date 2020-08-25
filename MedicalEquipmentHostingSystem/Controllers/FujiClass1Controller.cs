using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 富士I类controller
    /// </summary>
    public class FujiClass1Controller : BaseController
    {
        private FujiClassDao fujiClassDao = new FujiClassDao();
        private FujiClassManager fujiClassManager = new FujiClassManager();

        /// <summary>
        /// 富士类别页面
        /// </summary>
        /// <returns></returns>
        public ActionResult FujiClassList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            return View();
        }

        /// <summary>
        /// 展示富士类别
        /// </summary>
        /// <param name="EquipmentClass1Name">设备类别1</param>
        /// <param name="EquipmentClass2Name">设备类别2</param>
        /// <param name="FujiClass1ID">富士I类id</param>
        /// <param name="FujiClass2ID">富士II类id</param>
        /// <returns>富士类别信息</returns>
        public JsonResult QueryFujiClass(string EquipmentClass1Name, string EquipmentClass2Name, int FujiClass1ID, int FujiClass2ID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClassLink>> result = new ResultModel<List<FujiClassLink>>();

            try
            {
                List<FujiClassLink> infos = new List<FujiClassLink>();
                infos = this.fujiClassDao.QueryFujiClass(EquipmentClass1Name, EquipmentClass2Name, FujiClass1ID, FujiClass2ID);

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
        /// 获取富士I类信息
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <returns>富士I类信息</returns>
        public JsonResult QueryFujiClass14AutoComplete(string inputText)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass1Info>> result = new ResultModel<List<FujiClass1Info>>();
            try
            {
                List<FujiClass1Info> infos = new List<FujiClass1Info>();

                infos = this.fujiClassDao.QueryFujiClass14AutoComplete(inputText);


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
        /// 获取富士I类信息
        /// </summary>
        /// <returns>富士I类信息</returns>
        public JsonResult GetFujiClass1s()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass1Info>> result = new ResultModel<List<FujiClass1Info>>();

            try
            {
                List<FujiClass1Info> data = this.fujiClassDao.GetFujiClass1s();
                int index = data.FindIndex(info => info.ID == -1);
                if(index!=-1)data.SwapOrder(index,data.Count-1);
                result.Data = data;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 获取设备1类
        /// </summary>
        /// <returns>设备1类</returns>
        public JsonResult GetEquipmentClass1s()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<string>> result = new ResultModel<List<string>>();

            try
            {
                result.Data = this.fujiClassDao.GetEquipmentClass1s();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 获取设备2类
        /// </summary>
        /// <returns>设备2类</returns>
        public JsonResult GetEquipmentClass2s()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<string>> result = new ResultModel<List<string>>();

            try
            {
                result.Data = this.fujiClassDao.GetEquipmentClass2s();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 保存富士I类信息
        /// </summary>
        /// <param name="fujiClass1Info">富士I类信息</param>
        /// <param name="isUpdate">是否修改</param>
        /// <returns>富士I类信息</returns>
        [HttpPost]
        public JsonResult SaveFujiClass1(FujiClass1Info fujiClass1Info, bool isUpdate)
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
                result.Data = this.fujiClassManager.SaveFujiClass1(fujiClass1Info, isUpdate);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断富士I类名称是否重复
        /// </summary>
        /// <param name="info">富士I类</param>
        /// <returns>富士I类名称是否重复</returns>
        [HttpPost]
        public JsonResult CheckFujiClass1Name(FujiClass1Info info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                result.Data = this.fujiClassDao.CheckFujiClass1Name(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断设备类型是否重复
        /// </summary>
        /// <param name="info">富士I类</param>
        /// <returns>设备类型是否重复</returns>
        [HttpPost]
        public JsonResult CheckEquipmentClassExisted(FujiClass1Info info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                result.Data = this.fujiClassDao.CheckEquipmentClassExisted(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 通过富士I类编号获取富士I类信息
        /// </summary>
        /// <param name="fujiClass1ID">富士I类编号</param>
        /// <returns>富士I类信息</returns>
        public JsonResult GetFujiClass1ByID(int fujiClass1ID)
        {
            ResultModel<FujiClass1Info> result = new ResultModel<FujiClass1Info>();
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
                FujiClass1Info info = this.fujiClassDao.GetFujiClass1ByID(fujiClass1ID);

                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 根据设备类别获取设备的富士I类
        /// </summary>
        /// <param name="equipmentClass1">设备类别I</param>
        /// <param name="equipmentClass2">设备类别II</param>
        /// <returns>设备的富士I类</returns>
        public JsonResult GetFujiClass1ByEquipmentClass(string equipmentClass1, string equipmentClass2)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<FujiClass1Info> result = new ResultModel<FujiClass1Info>();
            try
            {
                FujiClass1Info info = this.fujiClassDao.GetFujiClass1ByEquipmentClass(equipmentClass1, equipmentClass2);

                if (info == null)
                {
                    info = this.fujiClassDao.GetFujiClass1ByID(-1);
                }
                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }

        /// <summary>
        /// 删除富士I类与设备类型关联
        /// </summary>
        /// <param name="info">富士I类关联信息</param>
        [HttpPost]
        public JsonResult DeleteFujiClassLink(FujiClassLink info)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                this.fujiClassManager.DeleteFujiClassLink(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 删除富士I类
        /// </summary>
        /// <param name="id">富士I类ID</param>
        [HttpPost]
        public JsonResult DeleteFujiClass1ByID(int id)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<bool> result = new ResultModel<bool>();
            try
            {
                this.fujiClassManager.DeleteFujiClass1ByID(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出富士类别
        /// </summary>
        /// <param name="EquipmentClass1Name">设备类别1</param>
        /// <param name="EquipmentClass2Name">设备类别2</param>
        /// <param name="FujiClass1ID">富士I类id</param>
        /// <param name="FujiClass2ID">富士II类id</param>
        /// <returns>零件列表excel</returns>
        public ActionResult ExportFujiClass(string EquipmentClass1Name, string EquipmentClass2Name, int FujiClass1ID, int FujiClass2ID)
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
                List<FujiClassLink> infos = this.fujiClassDao.QueryFujiClass(EquipmentClass1Name, EquipmentClass2Name, FujiClass1ID, FujiClass2ID);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("设备类型I简称");
                dt.Columns.Add("设备类型II简称");
                dt.Columns.Add("富士I类简称");
                dt.Columns.Add("富士II类简称");

                foreach (FujiClassLink info in infos)
                {
                    dt.Rows.Add(info.EquipmentType1.Description, info.EquipmentType2.Description, info.FujiClass2.FujiClass1.Name, info.FujiClass2.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "富士类别.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}