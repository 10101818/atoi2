using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Controllers
{
    /// <summary>
    /// 设备controller
    /// </summary>
    public class EquipmentController : BaseController
    {
        private EquipmentDao equipmentDao = new EquipmentDao();
        private ServiceHisDao serviceHisDao = new ServiceHisDao();
        private EquipmentManager equipmentManager = new EquipmentManager();
        private FileDao fileDao = new FileDao();
        private UploadFileManager fileManager = new UploadFileManager();
        private DashboardProvider api = new DashboardProvider();

        /// <summary>
        /// 设备列表页面
        /// </summary>
        /// <returns>设备列表页面</returns>
        public ActionResult EquipmentList()
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }
            return View();
        }
        /// <summary>
        /// 设备填写/编辑页面
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>设备填写/编辑页面</returns>
        public ActionResult EquipmentDetail(int id, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.ActionName = actionName;

                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }
        /// <summary>
        /// 设备查看页面
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>设备查看页面</returns>
        public ActionResult EquipmentView(int id, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.ActionName = actionName;

                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }
        /// <summary>
        /// 设备生命周期页面
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="actionName">上级页面名称</param>
        /// <returns>设备生命周期页面</returns>
        public ActionResult EquipmentTimeLine(int id, string actionName)
        {
            if (CheckSession() == false)
            {
                Response.Redirect(Url.Action(ConstDefinition.HOME_ACTION, ConstDefinition.HOME_CONTROLLER), true);
                return null;
            }

            ResultModelBase result = new ResultModelBase();
            try
            {
                ViewBag.Id = id;
                ViewBag.ActionName = actionName;

                return View();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("Error", result);
        }

        /// <summary>
        /// 获取设备列表信息
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <param name="warrantyStatus">设备维保状态</param>
        /// <param name="departmentID">科室编号</param>
        /// <param name="filterTextName">设备名称</param>
        /// <param name="filterTextSerialCode">设备序列号</param>
        /// <param name="useStatus">设备使用状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="currentPage">页码</param>
        /// <param name="pageSize">每页信息条数</param>
        /// <returns>设备列表信息</returns>
        public JsonResult QueryEquipments(int status, int warrantyStatus, int departmentID, string filterTextName, string filterTextSerialCode, bool useStatus, string filterField, string filterText, string sortField, bool sortDirection, int currentPage = 0, int pageSize = ConstDefinition.PAGE_SIZE, int fujiClass2ID = 0)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<EquipmentInfo>> result = new ResultModel<List<EquipmentInfo>>();
            try
            {
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<EquipmentInfo> infos = new List<EquipmentInfo>();
                if (currentPage > 0)
                {
                    int totalRecord = this.equipmentDao.QueryEquipmentsCount(status, warrantyStatus, departmentID, fujiClass2ID, filterTextName, filterTextSerialCode, useStatus, filterField, filterText);
                    result.SetTotalPages(totalRecord, pageSize);

                    if (totalRecord > 0)
                        infos = this.equipmentDao.QueryEquipments(status, warrantyStatus, departmentID, fujiClass2ID, filterTextName, filterTextSerialCode, useStatus, filterField, filterText, sortField, sortDirection, result.GetCurRowNum(currentPage, pageSize), pageSize);
                }
                else
                {
                    infos = this.equipmentDao.QueryEquipments(status, warrantyStatus, departmentID, fujiClass2ID, filterTextName, filterTextSerialCode, useStatus, filterField, filterText, sortField, sortDirection);
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
        /// 获取各个状态的设备数量
        /// </summary>
        /// <returns>设备正常、故障、报废的设备数量</returns>
        public JsonResult GetEquipmentCount()
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<Dictionary<string, int>> result = new ResultModel<Dictionary<string, int>>();
            try
            {
                Dictionary<int,int> dicEquipmentCount = this.equipmentDao.GetEquipmentCount();

                Dictionary<string, int> resultData = new Dictionary<string,int>();
                if (dicEquipmentCount.ContainsKey(EquipmentInfo.EquipmentStatuses.Normal) == false)
                    resultData.Add("Normal", 0);
                else
                    resultData.Add("Normal", dicEquipmentCount[EquipmentInfo.EquipmentStatuses.Normal]);
                if (dicEquipmentCount.ContainsKey(EquipmentInfo.EquipmentStatuses.Fault) == false)
                    resultData.Add("Fault", 0);
                else
                    resultData.Add("Fault", dicEquipmentCount[EquipmentInfo.EquipmentStatuses.Fault]);
                if (dicEquipmentCount.ContainsKey(EquipmentInfo.EquipmentStatuses.Scrap) == false)
                    resultData.Add("Scrap", 0);
                else
                    resultData.Add("Scrap", dicEquipmentCount[EquipmentInfo.EquipmentStatuses.Scrap]);

                result.Data = resultData;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }

        /// <summary>
        /// 获取设备的生命周期
        /// </summary>
        /// <param name="equipmentID">设备编号</param>
        /// <returns>设备生命周期信息</returns>
        public JsonResult GetTimeLine(int equipmentID)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<EquipmentInfo> result = new ResultModel<EquipmentInfo>();
            try
            {
                EquipmentInfo info = this.equipmentManager.GetTimeLine(equipmentID);
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
        /// 获取设备类别信息
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="parentCode">父级分类编码</param>
        /// <returns>该条件下的所有信息</returns>
        public JsonResult GetEquipmentClass(int level, string parentCode = null)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<EquipmentClassInfo>> result = new ResultModel<List<EquipmentClassInfo>>();
            try
            {
                result.Data = LookupManager.GetEquipmentClass(level, parentCode);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 通过设备编号获取设备信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <returns>设备信息</returns>
        public JsonResult GetEquipmentByID(int id)
        {
            ResultModel<EquipmentInfo> result = new ResultModel<EquipmentInfo>();
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
                result.Data = this.equipmentManager.GetEquipment(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 预览图片
        /// </summary>
        /// <param name="equipmentID">设备id</param>
        /// <param name="fileID">附件id</param>
        /// <returns>图片预览</returns>
        [Route("{id}")]
        public ActionResult ViewPDF(int equipmentID, int fileID)
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
                UploadFileInfo info = this.fileDao.GetFileByID(ObjectTypes.Equipment, fileID);

                FileUtil.CheckDirectory(Constants.EquipmentFolder);

                string filePath = Path.Combine(Constants.EquipmentFolder, string.Format("{0}_{1}{2}", equipmentID, fileID, new FileInfo(info.FileName).Extension));


                if (info != null && System.IO.File.Exists(filePath))
                {
                    Stream stream = new FileStream(filePath, FileMode.Open);
                    return File(stream, info.FileName);
                }
                else
                    result.SetFailed(ResultCodes.BusinessError, "文件不存在");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return View("ErrorPage", null, result.ResultMessage);
        }
        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="equipmentInfo">设备信息</param>
        /// <returns>设备信息</returns>
        [HttpPost]
        public JsonResult SaveEquipmentInfo(EquipmentInfo equipmentInfo)
        {
            if (CheckSession(false) == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<EquipmentInfo> result = new ResultModel<EquipmentInfo>();
            try
            {
                List<UploadFileInfo> files = GetUploadFilesInSession();

                equipmentInfo = this.equipmentManager.SaveEquipment(equipmentInfo, files, GetLoginUser());

                result.Data = equipmentInfo;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 判断序列号是否存在
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="serialCode">序列号</param>
        /// <returns>序列号是否存在</returns>
        public JsonResult CheckSerialCode(int id, String serialCode)
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
                result.Data = this.equipmentDao.CheckSerialCode(id, serialCode);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }
        /// <summary>
        /// 判断设备资产编号是否重复
        /// </summary>
        /// <param name="id">设备id</param>
        /// <param name="assetCode">资产编号</param>
        /// <returns>设备资产编号是否重复</returns>
        public JsonResult CheckAssetCode(int id, string assetCode)
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
                result.Data = this.equipmentDao.CheckAssetCode(id, assetCode);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);
        }

        /// <summary>
        /// 导出设备列表信息
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <param name="warrantyStatus">维保状态</param>
        /// <param name="departmentID">设备科室编号</param>
        /// <param name="filterTextName">设备名称</param>
        /// <param name="filterTextSerialCode">设备序列号</param>
        /// <param name="useStatus">设备使用状态</param>
        /// <param name="searchField">搜索字段</param>
        /// <param name="searchValue">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <returns>设备列表信息excel</returns>
        public ActionResult ExportEquipments(int status, int warrantyStatus, int departmentID, string filterTextName, string filterTextSerialCode, bool useStatus, string filterField, string filterText, string sortField, bool sortDirection, int fujiClass2ID = 0)
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
                List<EquipmentInfo> equipments = null;
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                equipments = this.equipmentDao.QueryEquipments(status, warrantyStatus, departmentID, fujiClass2ID, filterTextName, filterTextSerialCode, useStatus, filterField, filterText, sortField, sortDirection);
                DataTable dt = new DataTable("Sheet1");
                dt.Columns.Add("系统编号");
                dt.Columns.Add("设备名称");
                dt.Columns.Add("设备型号");
                dt.Columns.Add("设备序列号");
                dt.Columns.Add("厂商");
                dt.Columns.Add("标准响应时间(分)");
                dt.Columns.Add("等级");
                dt.Columns.Add("设备类别 (I)");
                dt.Columns.Add("设备类别 (II)");
                dt.Columns.Add("设备类别 (III)");
                dt.Columns.Add("分类编码");
                dt.Columns.Add("整包范围");
                dt.Columns.Add("品牌");
                dt.Columns.Add("出厂日期");
                dt.Columns.Add("备注");
                dt.Columns.Add("富士I类");
                dt.Columns.Add("富士II类");

                dt.Columns.Add("固定资产");
                dt.Columns.Add("资产等级");
                dt.Columns.Add("注册证有效日期");
                dt.Columns.Add("资产编号");
                dt.Columns.Add("折旧年限(年)");

                dt.Columns.Add("销售合同名称");
                dt.Columns.Add("购入方式");
                dt.Columns.Add("采购日期");
                dt.Columns.Add("经销商");
                dt.Columns.Add("采购金额(元)");
                dt.Columns.Add("设备产地");

                dt.Columns.Add("使用科室");
                dt.Columns.Add("安装地点");
                dt.Columns.Add("安装日期");
                dt.Columns.Add("启用日期");
                dt.Columns.Add("验收状态");
                dt.Columns.Add("验收时间");
                dt.Columns.Add("使用状态");
                dt.Columns.Add("设备状态");
                dt.Columns.Add("维保状态");
                dt.Columns.Add("强检标记");
                dt.Columns.Add("强检时间");
                dt.Columns.Add("召回标记");
                dt.Columns.Add("召回时间");
                dt.Columns.Add("巡检周期");
                dt.Columns.Add("保养周期");
                dt.Columns.Add("校准周期");

                foreach (EquipmentInfo equipment in equipments)
                {
                    dt.Rows.Add(equipment.OID, equipment.Name, equipment.EquipmentCode, equipment.SerialCode, equipment.Manufacturer.Name, equipment.ResponseTimeLength, equipment.EquipmentLevel.Name,
                        equipment.EquipmentClass1.Description, equipment.EquipmentClass2.Description, equipment.EquipmentClass3.Description, equipment.ClassCode, equipment.ServiceScope == true? "是":"否",
                        equipment.Brand, equipment.ManufacturingDate == DateTime.MinValue ? "" : equipment.ManufacturingDate.ToString("yyyy-MM-dd"), equipment.Comments, equipment.FujiClass2.FujiClass1.Name, equipment.FujiClass2.Name,
                        equipment.FixedAsset == true? "是":"否", equipment.AssetLevel.Name, equipment.ValidityStartDate == DateTime.MinValue ? "" : equipment.ValidityStartDate.ToString("yyyy-MM-dd") + " - " + equipment.ValidityEndDate.ToString("yyyy-MM-dd"),
                        equipment.AssetCode, equipment.DepreciationYears,
                        equipment.SaleContractName, equipment.PurchaseWay, equipment.PurchaseDate.ToString("yyyy-MM-dd"), equipment.Supplier.Name, equipment.PurchaseAmount, equipment.IsImport == true ? "进口" : "国产",
                        equipment.Department.Name, equipment.InstalSite, equipment.InstalDate.ToString("yyyy-MM-dd"), equipment.UseageDate == DateTime.MinValue ? "" : equipment.UseageDate.ToString("yyyy-MM-dd"), 
                        equipment.Accepted == true ? "已验收" : "未验收", equipment.AcceptanceDate == DateTime.MinValue ? "" : equipment.AcceptanceDate.ToString("yyyy-MM-dd"), equipment.UsageStatus.Name, equipment.EquipmentStatus.Name,
                        equipment.WarrantyStatus, equipment.MandatoryTestStatus.Name == "" ? "无" : equipment.MandatoryTestStatus.Name, equipment.MandatoryTestDate == DateTime.MinValue ? "" : equipment.MandatoryTestDate.ToString("yyyy-MM-dd"),
                        equipment.RecallFlag == true ? "是" : "否", equipment.RecallDate == DateTime.MinValue ? "" : equipment.RecallDate.ToString("yyyy-MM-dd"), equipment.PatrolType.Name != "" ? equipment.PatrolPeriod + equipment.PatrolType.Name : "无巡检",
                        equipment.MaintenanceType.Name != "" ? equipment.MaintenancePeriod + equipment.MaintenanceType.Name : "无保养", equipment.CorrectionType.Name != "" ? equipment.CorrectionPeriod + equipment.CorrectionType.Name : "无巡检"
                        );
                }

                MemoryStream ms = ExportUtil.ToExcel(dt);
                Response.AddHeader("Set-Cookie", "fileDownload=true; path=/");
                return File(ms, "application/excel", "设备列表.xlsx");
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备二维码信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <returns>二维码信息</returns>
        public JsonResult EquipmentLabel(int id)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<string> result = new ResultModel<string>();
            try
            {
                result.Data = this.equipmentManager.GetEquipmentLabel(id, GetBaseUrl(), WebConfig.HOSPITAL_NAME);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备未来1年计划服务发生时间
        /// </summary>
        /// <param name="equipmentID">设备ID</param>
        /// <param name="typeID">计划服务类型</param>
        /// <returns>下次发生时间</returns>
        [HttpPost]
        public JsonResult GetSysRequestList(int equipmentID, int typeID, EquipmentInfo equipmentInfo)
        {
            if (CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet);
            }
            if (CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }

            ResultModel<List<DateTime>> result = new ResultModel<List<DateTime>>();
            try
            {
                result.Data = this.equipmentManager.GetSysRequestList(equipmentID, typeID, equipmentInfo);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonResult(result);

        }

        #region "DashBoard"
        
        /// <summary>
        /// 设备信息总览
        /// </summary>
        /// <returns>设备数量、设备金额、折旧率、当年服务人次</returns>
        public JsonResult QueryOverview()
        {
            ServiceResultModel<Dictionary<string, object>> result = new ServiceResultModel<Dictionary<string, object>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.EquipmentQueryOverview();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 各部门设备信息（包括收入、支出、服务人次）
        /// </summary>
        /// <returns>各部门设备信息</returns>
        public JsonResult IncomeExpenseByDepartment(string date)
        {
            ServiceResultModel<List<DepartPerformanceModel>> result = new ServiceResultModel<List<DepartPerformanceModel>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.IncomeExpenseByDepartment(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 科室下所有设备信息（包括收入、支出）
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <param name="date">日期</param>
        /// <returns>科室下所有设备信息</returns>
        public JsonResult EquipmentsDetailsByDepartment(int id,string date="")
        {
            ServiceResultModel<List<EquipmentInfo>> result = new ServiceResultModel<List<EquipmentInfo>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.EquipmentsDetailsByDepartment(id, date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备的各类型请求数量
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="date">日期</param>
        /// <returns>设备申请的维修请求、保养请求、巡检请求、强检请求、校准请求数量</returns>
        public JsonResult GetRequestCountByID(int id,string date="")
        {
            ServiceResultModel<Dictionary<string, int>> result = new ServiceResultModel<Dictionary<string, int>>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                result.Data = api.GetRequestCountByID(id, date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// App获取生命周期信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="date">日期</param>
        /// <returns>生命周期信息</returns>
        public JsonResult GetTimeline4Dashboard(int id,string date="")
        {
            ServiceResultModel<EquipmentInfo> result = new ServiceResultModel<EquipmentInfo>();
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSession() == false)
            {
                return Json(ResultModelBase.CreateTimeoutModel(), JsonRequestBehavior.AllowGet); 
            }
            if (WebConfig.CHECK_SESSION_ON_DASHBORAD_API == true && CheckSessionID() == false)
            {
                return Json(ResultModelBase.CreateLogoutModel(), JsonRequestBehavior.AllowGet);
            }
            try
            { 
                result.Data = this.equipmentManager.GetTimeLine(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return JsonNetResult(result, JsonRequestBehavior.AllowGet);
        }
        #endregion 
    }
}