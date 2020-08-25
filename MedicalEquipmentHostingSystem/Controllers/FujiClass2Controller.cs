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
    /// 
    /// </summary>
    public class FujiClass2Controller : BaseController
    {
        private FaultRateDao faultRateDao = new FaultRateDao();
        private FujiClassDao fujiClassDao = new FujiClassDao();
        private FujiClassManager fujiClassManager = new FujiClassManager();
        private LookupDao lookDao = new LookupDao();
        /// <summary>
        /// 富士II类页面
        /// </summary>
        /// <param name="selectedClass1">富士I类id</param>
        /// <returns>富士II类页面</returns>
        public ActionResult FujiClass2List(int selectedClass1 = 0)
        { 
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null; 
            }
            ViewBag.SelectedClass1 = selectedClass1;
            List<KeyValueInfo> class1 = this.fujiClassDao.QueryKeyValueFujiClass1();
            List<Tuple<KeyValueInfo, int>> class2 = this.fujiClassDao.QueryKeyValueFujiClass2(); 
            int index1 = class1.FindIndex(info => info.ID == -1);
            if(index1!=-1)class1.SwapOrder(index1,class1.Count-1);
            int index2 = class2.FindIndex(info => info.Item1.ID == -1);
            if(index2!=-1)class2.SwapOrder(index2,class2.Count-1);
            ViewBag.Class1 = class1;
            ViewBag.Class2 = class2;
            return View();
        }
        /// <summary>
        /// 富士II类详情页
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>富士II类详情页</returns>
        public ActionResult Detail(int id = 0)
        {
            if (!CheckSession())
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ViewBag.Class1 = this.lookDao.GetLookups("tblFujiClass1", "Name");
            ViewBag.Class2 = this.fujiClassDao.QueryKeyValueFujiClass2();
            ViewBag.Equipment1 = LookupManager.GetEquipmentClass(1);
            ViewBag.Equipment2 = LookupManager.GetEquipmentClass(2);
            ViewBag.ID = id;
            return View("FujiClass2Detail");
        }
        /// <summary>
        /// 获取富士II类信息
        /// </summary>
        /// <param name="class1">富士I类id</param>
        /// <param name="class2">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>富士II类信息</returns>
        public JsonResult QueryFujiClass2s(int class1, int class2, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass2Info>> result = new ResultModel<List<FujiClass2Info>>();

            try
            { 
                if (currentPage > 0)
                {
                    int totalNum = this.fujiClassDao.QueryFujiClass2Count(class1,class2, filterField, filterText);
                    result.SetTotalPages(totalNum, pageSize); 
                    result.Data = this.fujiClassDao.QueryFujiClass2s(class1, class2, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    result.Data = this.fujiClassManager.QueryFujiClass2s(class1, class2, filterField, filterText, sortField, sortDirection);
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
        /// 获取富士II类信息
        /// </summary>
        /// <param name="inputText">搜索内容</param>
        /// <param name="fujiClass1ID">富士I类id</param>
        /// <returns>富士II类信息</returns>
        public JsonResult QueryFujiClass24AutoComplete(string inputText, int fujiClass1ID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass2Info>> result = new ResultModel<List<FujiClass2Info>>();
            try
            {
                List<FujiClass2Info> infos = new List<FujiClass2Info>();

                infos = this.fujiClassDao.QueryFujiClass24AutoComplete(inputText, fujiClass1ID);

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
        /// 获取富士II类信息
        /// </summary>
        /// <returns>富士II类信息</returns>
        public JsonResult GetFujiClass2()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass2Info>> result = new ResultModel<List<FujiClass2Info>>();

            try
            {
                List<FujiClass2Info> data = this.fujiClassDao.GetFujiClass2();
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
        /// 根据富士II类id获取II类信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>富士II类信息</returns>
        public JsonResult GetFujiClass2ByID(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<FujiClass2Info> result = new ResultModel<FujiClass2Info>();

            try
            { 
                    result.Data = this.fujiClassManager.GetFujiClass2ByID(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        } 
        /// <summary>
        /// 根据富士II类id获取关联信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>关联信息</returns>
        public JsonResult GetFujiLinkByClass2ID(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<FujiClassLink> result = new ResultModel<FujiClassLink>();

            try
            {
                result.Data = this.fujiClassManager.GetFujiLinkByClass2ID(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 导出富士II类信息
        /// </summary>
        /// <param name="class1">富士I类id</param>
        /// <param name="class2">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>导出富士II类信息excel</returns>
        [HttpPost]
        public ActionResult ExportFujiClass2s(int class1, int class2, string filterField, string filterText, string sortField, bool sortDirection)
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
                List<FujiClass2Info> fujiClass2s = this.fujiClassManager.QueryFujiClass2s(class1, class2, filterField, filterText, sortField, sortDirection);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("富士I类");
                dt.Columns.Add("富士II类简称");
                dt.Columns.Add("富士II类描述");
                dt.Columns.Add("人工费");
                dt.Columns.Add("维保服务费");
                dt.Columns.Add("备用机成本");
                dt.Columns.Add("维保额外维修费");
                dt.Columns.Add("零件");
                dt.Columns.Add("耗材");

                foreach (FujiClass2Info fujiClass2 in fujiClass2s)
                {
                    dt.Rows.Add(fujiClass2.FujiClass1.Name, fujiClass2.Name,fujiClass2.Description, fujiClass2.IncludeLabour?"有":"无", fujiClass2.IncludeContract ? "有" : "无", fujiClass2.IncludeSpare ? "有" : "无", fujiClass2.IncludeRepair ? "有" : "无",(fujiClass2.Components!=null&&fujiClass2.Components.Count>0)?"有" : "无",(fujiClass2.Consumables!=null&&fujiClass2.Consumables.Count>0)?"有" : "无");
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "富士II类列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取故障率信息
        /// </summary>
        /// <param name="objID">故障对象id</param>
        /// <param name="type">故障对象类型</param>
        /// <returns>故障率信息</returns>
        public JsonResult GetFaultRatesByObjID(int objID,FaultRateInfo.ObjectType type)
        { 
            if (!CheckSession())
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FaultRateInfo>> result = new ResultModel<List<FaultRateInfo>>();

            try
            {
                List<FaultRateInfo> infos = new List<FaultRateInfo>();
                infos = this.faultRateDao.GetFaultRateByObject(objID,type);

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
        /// 根据设备类别获取富士II类信息
        /// </summary>
        /// <param name="equipmentClass1">设备1类</param>
        /// <param name="equipmentClass2">设备2类</param>
        /// <returns>富士II类信息</returns>
        public JsonResult GetFujiClass2ByEqptClass(string equipmentClass1, string equipmentClass2)
        {
            if (!CheckSession(false))
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<FujiClass2Info>> result = new ResultModel<List<FujiClass2Info>>();

            try
            {
                List<FujiClass2Info> infos = this.fujiClassDao.GetFujiClass2ByEqptClass(equipmentClass1, equipmentClass2);

                if (infos.Count == 0)
                {
                    infos.Add(this.fujiClassDao.GetFujiClass2ByID(-1));
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
        /// 保存富士II类信息
        /// </summary>
        /// <param name="info">关联信息</param>
        /// <param name="isUpdate">是否更新</param>
        /// <param name="copyID">复制的II类id</param>
        /// <returns>富士II类id</returns>
        public JsonResult SaveFujiClass2(FujiClassLink info, bool isUpdate, int copyID = 0)
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
                this.fujiClassManager.SaveFujiClass2(info, GetLoginUser(), isUpdate, copyID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        } 
        /// <summary>
        /// 批量保存富士II类信息
        /// </summary>
        /// <param name="infos">富士II类信息</param>
        /// <param name="type">费用类型</param>
        /// <returns>保存结果</returns>
        public JsonResult SaveFujiClass2s(List<FujiClass2Info> infos, FujiClass2Info.SectionType type)
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
                this.fujiClassManager.SaveFujiClass2s(infos,type,GetLoginUser());
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断富士II类名称是否重复
        /// </summary>
        /// <param name="info">富士II类</param>
        /// <returns>富士II类名称是否重复</returns>
        [HttpPost]
        public JsonResult CheckFujiClass2Name(FujiClass2Info info)
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
                result.Data = this.fujiClassDao.CheckFujiClass2Name(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断富士II类关联关系是否重复
        /// </summary>
        /// <param name="info">富士II类关联信息</param>
        /// <returns>富士II类关联关系是否重复</returns>
        [HttpPost]
        public JsonResult CheckFujiClass2EqpExisted(FujiClassLink info)
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
                result.Data = this.fujiClassDao.CheckFujiClass2EqpExisted(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 删除富士II类
        /// </summary>
        /// <param name="info">富士II类关联信息</param>
        [HttpPost]
        public JsonResult DeleteFujiClass2ByLink(FujiClassLink info)
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
                this.fujiClassManager.DeleteFujiClass2ByLink(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        #region web
        public JsonResult GetWebData(int fujiClass2ID, int months, int componentID = 0)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            ResultModel<Dictionary<string, object>> result = new ResultModel<Dictionary<string, object>>();
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                if (componentID == 0)
                    data = this.fujiClassManager.GetRepairWebData(fujiClass2ID, months);
                else
                    data = this.fujiClassManager.GetComponentWebData(fujiClass2ID, componentID, months);

                result.Data = data;
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