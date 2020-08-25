using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using MedicalEquipmentHostingSystem.App_Start;
using MedicalEquipmentHostingSystem.Areas.App.Models;
using MedicalEquipmentHostingSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalEquipmentHostingSystem.Areas.App.Controllers
{
    /// <summary>
    /// EquipmentController
    /// </summary>
    public class EquipmentController : BaseController
    {
        private EquipmentManager equipmentManager = new EquipmentManager();
        private UploadFileManager uploadFileManager = new UploadFileManager();

        private EquipmentDao equipmentDao = new EquipmentDao();
        private FileDao fileDao = new FileDao();
        private ServiceHisDao serviceHisDao = new ServiceHisDao();
        private DashboardProvider api = new DashboardProvider();

        /// <summary>
        /// 扫码获取设备信息
        /// </summary>
        /// <param name="codeContent">二维码信息</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备信息</returns>
        public JsonResult GetDeviceByQRCode(int userID, string sessionID, string codeContent)
        {
            ServiceResultModel<EquipmentInfo> result = new ServiceResultModel<EquipmentInfo>();

            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrEmpty(codeContent))
                {
                    result.SetFailed(ResultCodes.ParameterError, "二维码内容为空，请重新扫码");//您输入的codeContent参数不正确
                }
                else
                {
                    string strEquipmentID = codeContent.Split('=')[codeContent.Split('=').Count() - 1];
                    int equipmentID = 0;
                    if (Int32.TryParse(strEquipmentID, out equipmentID) == false)
                    {
                        result.SetFailed(ResultCodes.ParameterError, "二维码参数不正确，请重新扫码");//您输入的codeContent参数不正确
                        return MyJson(result, JsonRequestBehavior.AllowGet);
                    }

                    EquipmentInfo info = this.equipmentManager.GetEquipment(equipmentID);
                    if (info == null) result.SetFailed(ResultCodes.ParameterError, "设备系统编号不正确，请重新扫码");//您输入的设备系统编号不正确
                    else result.Data = info.Copy4App();
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 获取设备列表信息
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <param name="warrantyStatus">维保状态</param>
        /// <param name="departmentID">科室</param>
        /// <param name="filterTextName">设备名称</param>
        /// <param name="filterTextSerialCode">设备序列号</param>
        /// <param name="useStatus">是否停用</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备信息</returns>
        public JsonResult GetDevices(int userID, string sessionID, int status = 0, int warrantyStatus = 0, int departmentID = -1, int fujiClass2ID = 0, string filterTextName = "", string filterTextSerialCode = "", bool useStatus = false, string filterField = "e.ID", string filterText = "", int curRowNum = 0, int pageSize = 0)
        {
            ServiceResultModel<List<EquipmentInfo>> result = new ServiceResultModel<List<EquipmentInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                BaseDao.ProcessFieldFilterValue(filterField, ref filterText);
                List<EquipmentInfo> infos = new List<EquipmentInfo>();
                List<EquipmentInfo> equipments = this.equipmentDao.QueryEquipments(status, warrantyStatus, departmentID, fujiClass2ID, filterTextName, filterTextSerialCode, useStatus, filterField, filterText, "e.ID", false, curRowNum, pageSize);
                foreach (EquipmentInfo info in equipments)
                {
                    infos.Add(info.Copy4App());
                }
                result.Data = infos;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过设备编号获取二维码信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备二维码信息</returns>
        public JsonResult EquipmentLabel(int userID, string sessionID, int id)
        {
            ServiceResultModel<string> result = new ServiceResultModel<string>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.equipmentManager.GetEquipmentLabel(id, GetBaseUrl(), WebConfig.HOSPITAL_NAME);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过设备编号获取设备信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备信息</returns>
        public JsonResult GetDeviceById(int userID, string sessionID, int id)
        {
            ServiceResultModel<EquipmentInfo> result = new ServiceResultModel<EquipmentInfo>();

            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                EquipmentInfo info = this.equipmentManager.GetEquipment(id);
                if (info == null) result.SetFailed(ResultCodes.ParameterError, "设备系统编号不正确");//您输入的设备系统编号不正确
                else result.Data = info;
                
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="userID">用户编号</param>
        /// <param name="info">设备信息</param> 
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>保存设备信息返回编码</returns>
        [HttpPost]
        public JsonResult SaveEquipment(int userID, string sessionID, EquipmentInfo info)
        {
            ServiceResultModelBase result = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UserInfo user = null;
                if (CheckUser(userID, result, out user) == false)
                    return MyJson(result, JsonRequestBehavior.AllowGet);

                if (WebConfig.AutoAssetCode == false)
                {
                    if (string.IsNullOrEmpty(info.AssetCode))
                    {
                        result.SetFailed(ResultCodes.ParameterError, "设备资产编号不能为空");
                        return MyJson(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (this.equipmentDao.CheckAssetCode(info.ID, info.AssetCode))
                        {
                            result.SetFailed(ResultCodes.ParameterError, "设备资产编号重复");
                            return MyJson(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                this.equipmentManager.SaveEquipment4App(info, info.EquipmentFile, user);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据附件编号获取设备附件信息
        /// </summary>
        /// <param name="id">附件编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>获取设备附件信息返回编码</returns>
        public JsonResult DownloadUploadFile(int userID, string sessionID, int id)
        {
            ServiceResultModel<string> result = new ServiceResultModel<string>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                UploadFileInfo file = this.fileDao.GetFileByID(ObjectTypes.Equipment, id);

                if (file == null)
                {
                    result.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                }
                else
                {
                    string path = Path.Combine(Constants.EquipmentFolder, file.GetFileName());
                    if (!System.IO.File.Exists(path))
                    {
                        result.SetFailed(ResultCodes.ParameterError, "找不到上传文件");
                    }
                    else
                    {
                        byte[] arr = System.IO.File.ReadAllBytes(path);
                        result.Data = Convert.ToBase64String(arr);
                    }
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除设备附件
        /// </summary>
        /// <param name="fileID">附件编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>删除设备附件返回编码</returns>
        [HttpPost]
        public JsonResult DeleteEquipmentFile(int userID, string sessionID, int fileID)
        {
            ServiceResultModelBase result = new ServiceResultModelBase();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                if (fileID > 0)
                {
                    this.uploadFileManager.DeleteUploadFileByID(ObjectTypes.Equipment, fileID);
                }
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备类别信息
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="parentCode">父级分类编码</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>该条件下的所有信息</returns>
        public JsonResult GetEquipmentClass(int userID, string sessionID, int level, string parentCode = null)
        {
            ServiceResultModel<List<EquipmentClassInfo>> result = new ServiceResultModel<List<EquipmentClassInfo>>();

            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = LookupManager.GetEquipmentClass(level, parentCode);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }

            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过设备编号获取设备生命周期
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>生命周期</returns>
        public JsonResult GetTimeLine4App(int userID, string sessionID, int id)
        {
            ServiceResultModel<EquipmentInfo> result = new ServiceResultModel<EquipmentInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                EquipmentInfo info = this.equipmentManager.GetTimeLine(id);

                result.Data = info;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备的各类型请求数量
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="date">日期</param>
        /// <returns>设备申请的维修请求、保养请求、巡检请求、强检请求、校准请求数量</returns>
        public JsonResult GetRequestCountByID(int userID, string sessionID, int id, string date = "")
        {
            ServiceResultModel<Dictionary<string, int>> result = new ServiceResultModel<Dictionary<string, int>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.api.GetRequestCountByID(id, date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 单台设备收入、支出
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备收入、支出</returns>
        public JsonResult IncomeExpenseByID(int userID, string sessionID, int id, int type = ReportDimension.AcceptanceYear, int year = 0)
        {
            ServiceResultModel<Dictionary<string, List<Tuple<string, double, double>>>> result = new ServiceResultModel<Dictionary<string, List<Tuple<string, double, double>>>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                Dictionary<string, List<Tuple<string, double, double>>> dic = new Dictionary<string, List<Tuple<string, double, double>>>();
                if (year <= 0)
                    year = DateTime.Today.Year; 
                List<Tuple<string, double, double>> detailList = new List<Tuple<string, double, double>>();
                List<Tuple<string, double, double>> overAllList = new List<Tuple<string, double, double>>();
                Dictionary<string, double> incomes = this.serviceHisDao.GetServiceHisIncomesByEquipmentID(id, type, year);
                Dictionary<string, double> costs = this.serviceHisDao.GetAccessoryExpenseByEquipmentID(id, type, year);
                Dictionary<string, double> lastIncomes = this.serviceHisDao.GetServiceHisIncomesByEquipmentID(id, type, year - 1);
                Dictionary<string, double> lastCosts = this.serviceHisDao.GetAccessoryExpenseByEquipmentID(id, type, year - 1);
                foreach (string key in incomes.Keys.Union(costs.Keys))
                {
                    double income = 0, cost = 0;
                    if (incomes.ContainsKey(key))
                        income = incomes[key];
                    if (costs.ContainsKey(key))
                        cost = costs[key];
                    detailList.Add(new Tuple<string, double, double>(key, income, cost));
                }
                detailList.Sort((x, y) => SQLUtil.ConvertInt(x.Item1).CompareTo(SQLUtil.ConvertInt(y.Item1)));

                if (type == ReportDimension.AcceptanceYear)
                {
                    incomes = this.serviceHisDao.GetServiceHisIncomesByEquipmentID(id, ReportDimension.AcceptanceMonth, DateTime.Today.Year);
                    costs = this.serviceHisDao.GetAccessoryExpenseByEquipmentID(id, ReportDimension.AcceptanceMonth, DateTime.Today.Year);
                    lastIncomes = this.serviceHisDao.GetServiceHisIncomesByEquipmentID(id, ReportDimension.AcceptanceMonth, DateTime.Today.Year - 1);
                    lastCosts = this.serviceHisDao.GetAccessoryExpenseByEquipmentID(id, ReportDimension.AcceptanceMonth, DateTime.Today.Year - 1);
                }

                double sumIncomes = incomes.Values.Sum();
                double sumCosts = costs.Values.Sum();
                double lastsumIncomes = lastIncomes.Values.Sum();
                double lastsumCosts = lastCosts.Values.Sum();

                overAllList.Add(new Tuple<string, double, double>("current", sumIncomes, sumCosts));
                overAllList.Add(new Tuple<string, double, double>("last", lastsumIncomes, lastsumCosts));
                overAllList.Add(new Tuple<string, double, double>("rate", lastsumIncomes == 0 ? 0 : 100 * (sumIncomes - lastsumIncomes) / lastsumIncomes, lastsumCosts == 0 ? 0 : 100 * (sumCosts - lastsumCosts) / lastsumCosts));

                dic.Add("detail", detailList);
                dic.Add("overall", overAllList);


                result.Data = dic;
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备未来1年计划服务发生时间
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <param name="equipmentID">设备ID</param>
        /// <param name="typeID">计划服务类型</param>
        /// <returns>下次发生时间</returns>
        public JsonResult GetSysRequestList(int userID, string sessionID, int equipmentID, int typeID, EquipmentInfo equipmentInfo)
        {
            ServiceResultModel<List<DateTime>> result = new ServiceResultModel<List<DateTime>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.equipmentManager.GetSysRequestList(equipmentID, typeID, equipmentInfo);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);

        }


        #region "DashBoard"

        /// <summary>
        /// 设备信息总览
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>设备数量、设备金额、折旧率、当年服务人次</returns>
        public JsonResult QueryOverview(int userID, string sessionID)
        {
            ServiceResultModel<Dictionary<string, object>> result = new ServiceResultModel<Dictionary<string, object>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.EquipmentQueryOverview();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 各部门设备信息（包括收入、支出、服务人次）
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>各部门设备信息</returns>
        public JsonResult IncomeExpenseByDepartment(int userID, string sessionID, string date)
        {
            ServiceResultModel<List<DepartPerformanceModel>> result = new ServiceResultModel<List<DepartPerformanceModel>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.IncomeExpenseByDepartment(date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 科室下所有设备信息（包括收入、支出）
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <param name="date">日期</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>科室下所有设备信息</returns>
        public JsonResult EquipmentsDetailsByDepartment(int userID, string sessionID, int id, string date = "")
        {
            ServiceResultModel<List<EquipmentInfo>> result = new ServiceResultModel<List<EquipmentInfo>>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = api.EquipmentsDetailsByDepartment(id, date);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// App获取生命周期信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="date">日期</param>
        /// <param name="userID">用户ID</param>
        /// <param name="sessionID">当前请求所在设备储存的SessionID</param>
        /// <returns>生命周期信息</returns>
        public JsonResult GetTimeline4Dashboard(int userID, string sessionID, int id, string date = "")
        {
            ServiceResultModel<EquipmentInfo> result = new ServiceResultModel<EquipmentInfo>();
            try
            {
                if (!CheckSessionID(userID, sessionID, result)) return MyJson(result, JsonRequestBehavior.AllowGet);
                result.Data = this.equipmentManager.GetTimeLine(id);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, ex.Message);
                result.SetFailed(ResultCodes.SystemError, ControlManager.GetSettingInfo().ErrorMessage);
            }
            return MyJson(result, JsonRequestBehavior.AllowGet);
        }
        #endregion 
    }
}