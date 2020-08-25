using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;


namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 估价controller
    /// </summary>
    public class ValuationController : BaseController
    {
        private readonly ValuationDao valuationDao = new ValuationDao();
        private EquipmentDao equipmentDao = new EquipmentDao();
        private readonly ValuationManager valuationManager = new ValuationManager();

        /// <summary>
        /// 估价参数页面
        /// </summary>
        /// <returns>估价参数页面</returns>
        public ActionResult Parameter()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 运营实绩页面
        /// </summary>
        /// <returns>运营实绩页面</returns>
        public ActionResult Actual()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 估价执行页面
        /// </summary>
        /// <returns>估价执行页面</returns>
        public ActionResult Valuation()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            ClearValuationSession();
            return View();
        }
        /// <summary>
        /// 估价执行页面
        /// </summary>
        /// <returns>估价执行页面</returns>
        public ActionResult DecisionAids()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }

        #region tblParameter
        /// <summary>
        /// 获取估价参数信息
        /// </summary>
        /// <returns>估价参数信息</returns>
        public JsonResult GetParameter()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<ValParameterInfo> result = new ResultModel<ValParameterInfo>();

            try
            {
                result.Data = this.valuationDao.GetParameter();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 保存估价参数信息
        /// </summary>
        /// <param name="info">估价参数信息</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public JsonResult SaveParameter(ValParameterInfo info)
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
                this.valuationManager.UpdateParameter(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region tblValControl
        /// <summary>
        /// 获取估价前提条件信息
        /// </summary>
        /// <returns>估价前提条件信息</returns>
        public JsonResult GetValControl()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<ValControlInfo> result = new ResultModel<ValControlInfo>();

            try
            {
                result.Data = this.valuationManager.GetValControlInfo(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 获取预测工程师数量
        /// </summary>
        /// <returns>预测工程师数量</returns>
        public JsonResult GetComputeEngineer()
        {
            if (CheckSession() == false)
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
                this.valuationManager.UpdateEquipmentHours(GetLoginUser().ID);

                double totalHours = this.valuationDao.GetTotalHours(GetLoginUser().ID);
                ValParameterInfo paraInfo = this.valuationDao.GetParameter();

                result.Data = paraInfo.GetCalculatedEngineers(totalHours);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 保存估价前提条件信息
        /// </summary>
        /// <param name="info">估价前提条件信息</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public JsonResult SaveValControl(ValControlInfo info)
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
                info.User.ID = GetLoginUser().ID;
                this.valuationManager.SaveValControl(info);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        #endregion

        #region Session

        /// <summary>
        ///  存入缓存
        /// </summary>
        /// <param name="type">缓存类型</param>
        /// <returns>返回结果</returns>
        public JsonResult SaveValuation(ValuationType type)
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
                switch (type)
                {
                    case ValuationType.Equipment:
                        var equipments = GetValSession<ValEquipmentInfo>();
                        if(equipments!=null )
                            this.valuationManager.SaveValEquipments(GetLoginUser().ID, equipments.Values.ToList());
                        break;
                    case ValuationType.Spare:
                        var spares = GetValSession<ValSpareInfo>();
                        if(spares!=null )
                            this.valuationManager.SaveValSpares(spares.Values.ToList());
                        break;
                    case ValuationType.Consumable:
                        var consumables = GetValSession<ValConsumableInfo>();
                        if(consumables!=null )
                            this.valuationManager.SaveValConsumables(consumables.Values.ToList());
                        break;
                    case ValuationType.Component: 
                    case ValuationType.CTTube:
                        var components = GetValSession<ValComponentInfo>();
                        if(components!=null )
                            this.valuationManager.SaveValComponents(components.Values.ToList());
                        break;
                }
                ClearValSession();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 清除session
        /// </summary>
        /// <returns></returns>
        public JsonResult ClearValuationSession()
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
                ClearValSession();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion
        
        #region tblValEquipment
        /// <summary>
        /// 初始化设备信息
        /// </summary>
        /// <returns>初始化结果</returns>
        public JsonResult InitValEquipments()
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
                this.valuationDao.DeleteEquipments(GetLoginUser().ID);
                this.valuationDao.InitEquipments(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 获取设备列表信息
        /// </summary>
        /// <param name="infos">已更改集合</param>
        /// <param name="equipmentType">设备类别</param>
        /// <param name="amountType">金额</param>
        /// <param name="equipmentName">设备名称</param>
        /// <param name="serialCode">设备序列号</param>
        /// <param name="departmentName">科室名称</param>
        /// <param name="class1ID">富士I类id</param>
        /// <param name="class2ID">腐蚀II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>设备信息</returns>
        [HttpPost]
        public JsonResult QueryValEquipments(List<ValEquipmentInfo> infos, int equipmentType, int amountType, string equipmentName, string serialCode, string departmentName, int class1ID, int class2ID, string filterField, string filterText, string sortField, bool sortDirection, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValEquipmentInfo>> result = new ResultModel<List<ValEquipmentInfo>>();

            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                if (infos != null && infos.Count != 0)
                { 
                    SaveValSession(infos.ToDictionary(info => info.SessionKey));
                }				
				List<ValEquipmentInfo> datas = new List<ValEquipmentInfo>();                
				if (currentPage > 0)
                {
                    int totalRecord = this.valuationDao.GetEquipmentCount(GetLoginUser().ID, equipmentType, amountType, equipmentName, serialCode, departmentName, class1ID, class2ID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                        datas = this.valuationDao.QueryEquipmentList(GetLoginUser().ID, equipmentType, amountType, equipmentName, serialCode, departmentName, class1ID, class2ID, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                    datas = this.valuationDao.QueryEquipmentList(GetLoginUser().ID, equipmentType, amountType, equipmentName, serialCode, departmentName, class1ID, class2ID, filterField, filterText, sortField, sortDirection);

                Dictionary<string, ValEquipmentInfo> valSessions = GetValSession<ValEquipmentInfo>();
                if (valSessions != null)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        string key = datas[i].SessionKey;
                        if (valSessions.ContainsKey(key))
                            datas[i] = valSessions[key];
                    }
                }
                result.Data = datas;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导入设备
        /// </summary>
        /// <param name="base64String">设备信息</param>
        /// <returns>导入结果</returns>
        [HttpPost]
        public JsonResult ImportValEquipment(string base64String)
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
                base64String = ParseBase64String(base64String);
                if(string.IsNullOrEmpty(base64String))
                {
                    result.SetFailed(ResultCodes.ParameterError, "上传文件为空");
                }
                else
                {
                    UserInfo user = GetLoginUser();
                    byte[] excelFile = Convert.FromBase64String(base64String);
                    string message = null;
                    List<ValEquipmentInfo> importEquipments = null;
                    List<ValEquipmentInfo> updateEquipments = null;
                    int parseResult = this.valuationManager.ParseImportFile(user.ID, excelFile, out message, out importEquipments, out updateEquipments);
                    if(parseResult == 1)
                        result.SetFailed(ResultCodes.BusinessError, message);
                    else
                        this.valuationManager.SaveValEquipments(user.ID, updateEquipments, importEquipments);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出设备清单
        /// </summary>
        /// <param name="equipmentType">设备类别</param>
        /// <param name="amountType">金额</param>
        /// <param name="equipmentName">设备名称</param>
        /// <param name="serialCode">设备序列号</param>
        /// <param name="departmentName">科室名称</param>
        /// <param name="class1ID">富士I类id</param>
        /// <param name="class2ID">腐蚀II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>设备清单</returns>
        public ActionResult ExportValEquipments(int equipmentType, int amountType, string equipmentName, string serialCode, string departmentName, int class1ID, int class2ID, string filterField, string filterText, string sortField, bool sortDirection)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValEquipmentInfo>> result = new ResultModel<List<ValEquipmentInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<ValEquipmentInfo> infos = this.valuationDao.QueryEquipmentList(GetLoginUser().ID, equipmentType, amountType, equipmentName, serialCode, departmentName, class1ID, class2ID, filterField, filterText, sortField, sortDirection);

                DataTable dt = new DataTable();
                dt.Columns.Add("是否在系统中");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("资产编号");
                dt.Columns.Add("名称");
                dt.Columns.Add("设备序列号");
                dt.Columns.Add("厂商");
                dt.Columns.Add("科室");
                dt.Columns.Add("金额");
                dt.Columns.Add("设备类型");
                dt.Columns.Add("富士I类");
                dt.Columns.Add("富士II类");
                dt.Columns.Add("目前维保种类");
                dt.Columns.Add("维保到期日");
                dt.Columns.Add("下期维保种类");

                foreach(ValEquipmentInfo info in infos)
                {
                    dt.Rows.Add(info.InSystem ? 'Y' : 'N',info.Equipment.OID, info.Equipment.AssetCode, info.Equipment.Name, info.Equipment.SerialCode, info.Equipment.Manufacturer.Name, info.Equipment.Department.Name, info.Equipment.PurchaseAmount, info.Equipment.FujiClass2.EquipmentType.Name, info.Equipment.FujiClass2.FujiClass1.Name, info.Equipment.FujiClass2.Name, info.CurrentScope.Name, info.EndDate == DateTime.MinValue ? "" : info.EndDate.ToString("yyyy-MM-dd"), info.NextScope.Name);
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "估价执行:设备清单.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region tblValComponent
        /// <summary>
        /// 初始化零件
        /// </summary>
        /// <returns>初始化结果</returns>
        public JsonResult InitValComponents()
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
                this.valuationDao.DeleteComponents(GetLoginUser().ID, false);
                this.valuationDao.InitComponents(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 查询执行零件、CT球管
        /// </summary>
        /// <param name="infos">已更改集合</param>
        /// <param name="componentType">零件类型</param>
        /// <param name="equipmentType">设备类别</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>零件列表</returns>
        [HttpPost]
        public JsonResult QueryValComponents(List<ValComponentInfo> infos, int componentType, int equipmentType, string filterField, string filterText, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValComponentInfo>> result = new ResultModel<List<ValComponentInfo>>();

            try
            {
                if (infos != null && infos.Count != 0)
                { 
                    SaveValSession(infos.ToDictionary(info => info.SessionKey));
                }

                int currentRow = 0;

                if (currentPage > 0)
                {
                    int totalRecord = this.valuationDao.GetComponentCount(GetLoginUser().ID, componentType, equipmentType, filterField, filterText); 
                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                    {
                        result.SetTotalPages(totalRecord, pageSize);
                        currentRow = result.GetCurRowNum(currentPage, pageSize);
                    }
                }
                List<ValComponentInfo> datas = this.valuationDao.QueryComponentList(GetLoginUser().ID, componentType, equipmentType, filterField, filterText, false, currentRow, pageSize);
                Dictionary<string, ValComponentInfo> valSessions = GetValSession<ValComponentInfo>();
                if(valSessions !=null){
                    for (int i = 0; i < datas.Count; i++)
                    {
                        string key = datas[i].SessionKey;
                        if (valSessions.ContainsKey(key))
                            datas[i] = valSessions[key];
                    }
                }
                result.Data = datas;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 同步零件使用量
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateComponentUsage()
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
                this.valuationDao.UpdateComponentUsage(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion
        
        #region tblValConsumable
        /// <summary>
        /// 初始化耗材列表
        /// </summary>
        /// <returns>初始化结果</returns>
        public JsonResult InitValConsumables()
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
                this.valuationDao.DeleteConsumables(GetLoginUser().ID);
                this.valuationDao.InitConsumables(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 获取耗材列表信息
        /// </summary>
        /// <param name="infos">已更改集合</param>
        /// <param name="class2ID">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>耗材列表信息</returns>
        [HttpPost]
        public JsonResult QueryValConsumables(List<ValConsumableInfo> infos, int class2ID, string filterField, string filterText, int currentPage, int pageSize = ConstDefinition.PAGE_SIZE)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValConsumableInfo>> result = new ResultModel<List<ValConsumableInfo>>();

            try
            {

                if (infos != null && infos.Count != 0)
                { 
                    SaveValSession(infos.ToDictionary(info => info.SessionKey));
                }
				List<ValConsumableInfo> datas = new List<ValConsumableInfo>();                
                
                if (currentPage > 0)
                {
                    int totalRecord = this.valuationDao.GetConsumableCount(GetLoginUser().ID, class2ID, filterField, filterText);

                    result.SetTotalPages(totalRecord, pageSize);
                    if (totalRecord > 0)
                        datas = this.valuationDao.QueryConsumableList(GetLoginUser().ID, class2ID, filterField, filterText, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    datas = this.valuationDao.QueryConsumableList(GetLoginUser().ID, class2ID, filterField, filterText);
                }

                Dictionary<string, ValConsumableInfo> valSessions = GetValSession<ValConsumableInfo>();
                if (valSessions != null)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        string key = datas[i].SessionKey;
                        if (valSessions.ContainsKey(key))
                            datas[i] = valSessions[key];
            }
                }

                result.Data = datas;

            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        #endregion

        #region tblValSpare
        /// <summary>
        /// 初始化备用机
        /// </summary>
        /// <returns>初始化结果</returns>
        public JsonResult InitValSpares()
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
                this.valuationDao.DeleteSpares(GetLoginUser().ID);
                this.valuationDao.InitSpares(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        /// <summary>
        /// 获取备用机列表
        /// </summary>
        /// <param name="infos">已更改集合</param>
        /// <returns>备用机列表</returns>
        [HttpPost]
        public JsonResult QueryValSpares(List<ValSpareInfo> infos)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValSpareInfo>> result = new ResultModel<List<ValSpareInfo>>();

            try
            { 
				if (infos != null && infos.Count != 0)
                { 
                    SaveValSession(infos.ToDictionary(info => info.SessionKey));
                }
				List<ValSpareInfo> datas = new List<ValSpareInfo>();
                datas = this.valuationDao.QuerySpareList(GetLoginUser().ID);

                Dictionary<string, ValSpareInfo> valSessions = GetValSession<ValSpareInfo>();
                if (valSessions != null)
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        string key = datas[i].SessionKey;
                        if (valSessions.ContainsKey(key))
                            datas[i] = valSessions[key];            
					}
                }

                result.Data = datas;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        
        #endregion

        #region 成本明细表
        /// <summary>
        /// 执行估价
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RunVal()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<Dictionary<string, object>> result = new ResultModel<Dictionary<string, object>>();
            try
            {
                this.valuationManager.RunVal(GetLoginUser().ID);
                result.Data = this.valuationManager.GetValResultData(GetLoginUser().ID);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region 实绩
        public JsonResult GetActualData(int engineerCount, DateTime startDate, DateTime endDate)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<Dictionary<string, object>> result = new ResultModel<Dictionary<string, object>>();
            try
            {
                result.Data = this.valuationManager.GetValResultActualData(engineerCount, startDate, endDate.AddDays(1), DateTime.Now);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region 计算备用机
        /// <summary>
        /// 计算备用机数量
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="safeRate">安全率</param>
        /// <param name="valSpareList">备用机列表</param>
        /// <returns>备用机列表</returns>
        [HttpPost]
        public JsonResult CalculateSpareCount(DateTime startDate, DateTime endDate, double safeRate, List<ValSpareInfo> valSpareList)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValSpareInfo>> result = new ResultModel<List<ValSpareInfo>>();

            try
            {
                result.Data = this.valuationManager.CalculateSpareCount(startDate, endDate, safeRate, valSpareList);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 同步备用机参考值
        /// </summary>
        /// <param name="infos">备用机列表</param>
        /// <returns>备用机列表</returns>
        [HttpPost]
        public JsonResult SyncSpareCount(List<ValSpareInfo> infos)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<ValSpareInfo>> result = new ResultModel<List<ValSpareInfo>>();

            try
            {
                result.Data = this.valuationManager.SyncSpareCount(GetLoginUser().ID, infos);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }
        #endregion

        #region VaR
        /// <summary>
        /// 获取VaR数据源
        /// </summary>
        /// <param name="showType">展示类型：导入期、稳定期</param>
        /// <param name="assetsAmountRate">资产金额比例</param>
        /// <param name="riskRate">风险控制度</param>
        /// <param name="forecastQuantity">预测年数量</param>
        /// <returns></returns>
        public JsonResult GetVaRData(VaRType showType, double assetsAmountRate, int forecastQuantity=0, double riskRate = 0d)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<Dictionary<string, object>> result = new ResultModel<Dictionary<string, object>>();

            try
            {
                int userID = GetLoginUser().ID;
                var parameters = this.valuationDao.GetParameter();
                var control = this.valuationDao.GetControl(userID);
                if (forecastQuantity == 0)
                    forecastQuantity = control.Years;
                if (riskRate == 0)
                    riskRate = control.RiskRatio;
                double systemCost = parameters.SystemCost/12d;
                double labourCost_Forecast = parameters.UnitCost*control.ForecastEngineer;
                double labourCost_Actual = parameters.UnitCost * control.ForecastEngineer;
                double smallCost = parameters.SmallConsumableCost;
                double spareAmount = this.valuationDao.GetSpareAmount(userID);
                var eqptContractAmount = this.valuationDao.GetValEqptContractAmount(userID);
                var componentAmout = this.valuationDao.GetValComponentAmout(userID, -1);
                var qptService = this.valuationDao.GetValEqptService(userID, -1);
                var consumableAmount_Regular = this.valuationDao.GetValConsumableAmount(userID, ConsumableInfo.ConsumableTypes.RegularConsumable);
                var consumableAmount_Quantitative = this.valuationDao.GetValConsumableAmount(userID, ConsumableInfo.ConsumableTypes.QuantitativeConsumable);


                List<double> contractActualAmountList = new List<double>(), spareActualAmountList = new List<double>(), regularActualAmountList = new List<double>(), quanTityActualAmountList = new List<double>(), smallActualAmountList = new List<double>(), importantComponentActualAmountList = new List<double>(), generalComponentActualAmountList = new List<double>(), componentActualAmountList = new List<double>(), importantRepair3partyActualCostList = new List<double>(),generalRepair3partyActualCostList = new List<double>(), repair3partyActualCostList = new List<double>();

                double assetsAmount = this.valuationDao.QueryEquipmentList(userID, -1,-1, "", "", "", 0, 0, "", "", "f2.ID", true).Sum(e => e.Equipment.PurchaseAmount), deviationAvg = 0, stdDeviation = 0, length = forecastQuantity * 12;
                double vaR = assetsAmount * assetsAmountRate * 0.01;

                DateTime startDate = control.ContractStartDate.AddYears(-ValControlInfo.ActualYears.ActualYear);
                DateTime endDate = control.ContractStartDate;
                while (startDate < endDate)
                {
                    if (startDate < DateTime.Now)
                    {
                        //维保费
                        contractActualAmountList.Add(this.valuationDao.GetActualContractAmount(startDate));
                        //备用机
                        spareActualAmountList.Add(this.valuationDao.GetActualSpareAmount(startDate));
                        //耗材定期
                        regularActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.RegularConsumable, startDate));
                        //耗材定量
                        quanTityActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.QuantitativeConsumable, startDate));
                        //小额成本
                        smallActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.SmallCostConsumable, startDate));
                        //重点设备零件
                        importantComponentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                        //一般设备零件
                        generalComponentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                        //零件成本
                        componentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(-1, startDate));
                        //重点设备服务费
                        importantRepair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                        //一般设备服务费
                        generalRepair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                        //服务费
                        repair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(-1, startDate));
                    }

                    startDate = startDate.AddMonths(1);
                }






                List<double> deviations = new List<double>();
                List<Dictionary<string, object>> cost = new List<Dictionary<string, object>>();
                if (showType == VaRType.StablePeriod)
                {
                    Dictionary<string, object> costDetail;
                    Dictionary<string, object> forecastCost;
                    Dictionary<string, object> actualCost;
                    for (int i = 0; i < length; i++)
                    {
                        costDetail = new Dictionary<string, object>();
                        forecastCost = new Dictionary<string, object>();
                        actualCost = new Dictionary<string, object>();
                        DateTime date = control.ContractStartDate.AddMonths(i);
                        DateTime actualDate = startDate.AddMonths(i);
                        double forecastValue = systemCost + labourCost_Forecast + smallCost + spareAmount
                            + eqptContractAmount.Where(info => info.Year == date.Year && info.Month == date.Month).Sum(info => info.ContractAmount)
                            + componentAmout.Where(info => info.Year == date.Year && info.Month == date.Month).Sum(info => info.Amount)
                            + qptService.Where(info => info.Year == date.Year && info.Month == date.Month).Sum(info => info.Repair3partyCost)
                            + consumableAmount_Regular.Where(info => info.Year == date.Year && info.Month == date.Month).Sum(info => info.Amount)
                            + consumableAmount_Quantitative.Where(info => info.Year == date.Year && info.Month == date.Month).Sum(info => info.Amount);
                        double actualValue = i>=12?0:systemCost + labourCost_Actual
                            + contractActualAmountList[i]
                            + spareActualAmountList[i]
                            + regularActualAmountList[i]
                            + quanTityActualAmountList[i]
                            + smallActualAmountList[i]
                            + importantComponentActualAmountList[i]
                            + generalComponentActualAmountList[i]
                            + componentActualAmountList[i]
                            + importantRepair3partyActualCostList[i]
                            + generalRepair3partyActualCostList[i]
                            + repair3partyActualCostList[i];
                        forecastCost.Add("Year", date.Year);
                        forecastCost.Add("Month", MonthDesc.GetMonthDesc(date.Month));
                        forecastCost.Add("Value", forecastValue);

                        actualCost.Add("Year", i>=12?"":actualDate.Year.ToString());
                        actualCost.Add("Month", i >= 12 ? "" : MonthDesc.GetMonthDesc(actualDate.Month));
                        actualCost.Add("Value", i >= 12 ? "-" : actualValue.ToString());
                        costDetail.Add("ForecastCost", forecastCost);
                        costDetail.Add("ActualCost", actualCost);
                        costDetail.Add("Deviation", i >= 12 ? "-" : (actualValue - forecastValue).ToString());
                        if(i < 12 )deviations.Add(actualValue - forecastValue);
                        cost.Add(costDetail);
                    }
                    deviationAvg = (deviations.Sum()) / 12;
                    stdDeviation = Statistics.StandardDeviation(deviations);
                    double NORMINV = Normal.InvCDF(0, 1, assetsAmountRate * 0.01);
                    double SQRT = Math.Sqrt(forecastQuantity * 12);
                    vaR = NORMINV * SQRT * stdDeviation + deviationAvg * forecastQuantity * 12;
                    if (double.IsNaN(vaR) || vaR < 0)
                        vaR = 0;
                }
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("AssetsAmount",assetsAmount);
                data.Add("Cost", cost);
                data.Add("DeviationAvg", deviationAvg);
                data.Add("StdDeviation", stdDeviation);
                data.Add("VaR", vaR);
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

        #region 决策辅助

        /// <summary>
        /// 获取决策辅助数据源
        /// </summary> 
        /// <returns></returns>
        public JsonResult GetDecisionAidsData()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<Dictionary<string, object>>> result = new ResultModel<List<Dictionary<string, object>>>();

            try
            {
                List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                int userID = GetLoginUser().ID; 
                var control = this.valuationDao.GetControl(userID);
                double FailureCost = 0,OutsourcingMaintenanceCost = 0, WholeGuaranteeCost = 0,TechnicalGuaranteeCost = 0;
                List<ValEquipmentInfo> equipments = this.valuationDao.GetEquipmentList(userID);
                var componentAmout = this.valuationManager.CalculateComponentsAmountForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear,false); 
                var eqptService = this.valuationManager.CalculateRepair3partyCostForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear,false); 
                DateTime stdDate = new DateTime(control.ContractStartDate.Year,control.ContractStartDate.Month,1);

                equipments.ForEach(equipment =>
                {
                    Dictionary<string, object> temp = new Dictionary<string, object>();

                    FailureCost = componentAmout.AsEnumerable().Where(row => new DateTime((int)row["Year"], (int)row["Month"], 1) >= stdDate && new DateTime((int)row["Year"], (int)row["Month"], 1) <= stdDate.AddMonths(12).AddSeconds(-1) && (int)row["EquipmentID"] == equipment.Equipment.ID).Sum(row => SQLUtil.ConvertDouble(row["Amount"]));
                    OutsourcingMaintenanceCost = eqptService.AsEnumerable().Where(row => new DateTime((int)row["Year"], (int)row["Month"], 1) >= stdDate && new DateTime((int)row["Year"], (int)row["Month"], 1) <= stdDate.AddMonths(12).AddSeconds(-1) && (int)row["EquipmentID"] == equipment.Equipment.ID).Sum(row => SQLUtil.ConvertDouble(row["Repair3partyCost"])); 
                    WholeGuaranteeCost = equipment.Equipment.PurchaseAmount * equipment.Equipment.FujiClass2.FullCoveragePtg * 0.01;
                    TechnicalGuaranteeCost = equipment.Equipment.PurchaseAmount * equipment.Equipment.FujiClass2.TechCoveragePtg * 0.01;

                    temp.Add("ValEquipment", equipment);
                    temp.Add("FailureCost", FailureCost);
                    temp.Add("OutsourcingMaintenanceCost", OutsourcingMaintenanceCost);
                    temp.Add("WholeGuaranteeCost", WholeGuaranteeCost);
                    temp.Add("TechnicalGuaranteeCost", TechnicalGuaranteeCost);
                    data.Add(temp);
                });

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