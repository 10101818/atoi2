using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// lookupManager
    /// </summary>
    public static class LookupManager
    {
        private static LookupDao lookupDao = new LookupDao();
        private static DepartmentDao departmentDao = new DepartmentDao();

        /// <summary>
        /// 初始化字段信息
        /// </summary>
        public static void DoInit()
        {
            _Roles = lookupDao.GetLookups("lkpRole");
            _SupplierType = lookupDao.GetLookups("lkpSupplierType");
            _EquipmentClass = lookupDao.GetEquipmentClasses();
            _UsageStatus = lookupDao.GetLookups("lkpUsageStatus");
            _EquipmentStatus = lookupDao.GetLookups("lkpEquipmentStatus");
            _PeriodType = lookupDao.GetLookups("lkpPeriodType");
            _ContractType = lookupDao.GetLookups("lkpContractType");
            _ContractScope = lookupDao.GetLookups("lkpContractScope");	
            _RequestStatus = lookupDao.GetLookups("lkpRequestStatus");
            _RequestType = lookupDao.GetLookups("lkpRequestType");
            _DispatchStatus = lookupDao.GetLookups("lkpDispatchStatus");
            _FaultType = lookupDao.GetLookups("lkpFaultType");
            _DealType = lookupDao.GetLookups("lkpDealType");
            _Urgency = lookupDao.GetLookups("lkpUrgency");
            _DepartmentTypes = lookupDao.GetLookups("lkpDepartmentType");
            RecacheDepartments();
            _DispatchDocStatus = lookupDao.GetLookups("lkpDispatchDocStatus");
            _DispatchJournalResultStatus = lookupDao.GetLookups("lkpDispatchJournalResultStatus");
            _DispatchReportTypes = lookupDao.GetLookups("lkpDispatchReportType");
            _SolutionResultStatus = lookupDao.GetLookups("lkpSolutionResultStatus");
            _AccessorySourceType = lookupDao.GetLookups("lkpAccessorySourceType");
            _CustRptType = lookupDao.GetLookups("lkpCustomReportType");

            #region atoi2
            _ComponentType = lookupDao.GetLookups("lkpComponentType");
            _ConsumableType = lookupDao.GetLookups("lkpConsumableType");
            _EquipmentType = lookupDao.GetLookups("lkpEquipmentType");

            _HospitalLevel = lookupDao.GetHospitalLevels();

            _ObjectType = lookupDao.GetObjectTypes();

            _PurchaseOrderStatus = lookupDao.GetLookups("lkpPurchaseOrderStatus");
            #endregion
        }

        #region atoi1
        #region "CustRptType"
        private static List<KeyValueInfo> _CustRptType = null;

        /// <summary>
        /// 获取自定义报表类型信息
        /// </summary>
        /// <returns>自定义报表类型集</returns>
        public static List<KeyValueInfo> GetCustRptType()
        {
            return _CustRptType;
        }

        /// <summary>
        /// 根据自定义报表类型编号获取报表类型描述
        /// </summary>
        /// <param name="id">自定义报表类型id</param>
        /// <returns>自定义报表类型名称</returns>
        public static string GetCustRptTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetCustRptType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "Role"
        private static List<KeyValueInfo> _Roles = null;
        /// <summary>
        /// 获取用户角色信息
        /// </summary>
        /// <returns>用户角色信息</returns>
        public static List<KeyValueInfo> GetRoles()
        {
            return _Roles;
        }
        /// <summary>
        /// 根据角色编号获取角色名称
        /// </summary>
        /// <param name="id">角色编号</param>
        /// <returns>角色名称</returns>
        public static string GetRoleDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetRoles();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "GetSupplierTypeDesc"
        private static List<KeyValueInfo> _SupplierType = null;
        /// <summary>
        /// 获取供应商类型信息
        /// </summary>
        /// <returns>供应商类型集</returns>
        public static List<KeyValueInfo> GetSupplierType()
        {
            return _SupplierType;
        }
        /// <summary>
        /// 根据供应商类型编号获取供应商类型名称
        /// </summary>
        /// <param name="id">供应商类型编号</param>
        /// <returns>供应商类型名称</returns>
        public static string GetSupplierTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetSupplierType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "EquipmentClass"
        private static List<EquipmentClassInfo> _EquipmentClass = null;
        /// <summary>
        /// 根据设备类别等级和父级编码获取设备类别信息
        /// </summary>
        /// <param name="level">类别等级</param>
        /// <param name="parentCode">父级编码</param>
        /// <returns>设备类别信息</returns>
        public static List<EquipmentClassInfo> GetEquipmentClass(int level, string parentCode = null)
        {
            return (from temp in _EquipmentClass where temp.Level == level && (string.IsNullOrEmpty(parentCode) || temp.ParentCode.Equals(parentCode)) orderby temp.Code select temp).ToList();
        }
        /// <summary>
        /// 根据设备类别编码、类别等级、父级编码获取设备类别名称
        /// </summary>
        /// <param name="code">类别编码</param>
        /// <param name="level">类别等级</param>
        /// <param name="parentCode">父级编码</param>
        /// <returns>设备类别名称</returns>
        public static string GetEquipmentClassDesc(string code, int level, string parentCode = null)
        {
            List<EquipmentClassInfo> keyValues = GetEquipmentClass(level, parentCode);

            string desc = (from EquipmentClassInfo temp in keyValues where temp.Code == code select temp.Description).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }

        #endregion

        #region "UsageStatus"
        private static List<KeyValueInfo> _UsageStatus = null;
        /// <summary>
        /// 获取设备使用状态信息
        /// </summary>
        /// <returns>设备使用状态集</returns>
        public static List<KeyValueInfo> GetUsageStatus()
        {
            return _UsageStatus;
        }
        /// <summary>
        /// 根据使用状态编号获取设备使用状态名称
        /// </summary>
        /// <param name="id">使用状态编号</param>
        /// <returns>设备使用状态名称</returns>
        public static string GetUsageStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetUsageStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "EquipmentStatus"
        private static List<KeyValueInfo> _EquipmentStatus = null;
        /// <summary>
        /// 获取设备状态信息
        /// </summary>
        /// <returns>设备状态信息集</returns>
        public static List<KeyValueInfo> GetEquipmentStatus()
        {
            return _EquipmentStatus;
        }
        /// <summary>
        /// 根据设备状态编号获取设备状态名称
        /// </summary>
        /// <param name="id">设备状态编号</param>
        /// <returns>设备状态名称</returns>
        public static string GetEquipmentStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetEquipmentStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "PeriodType"
        private static List<KeyValueInfo> _PeriodType = null;
        /// <summary>
        /// 获取周期类型
        /// </summary>
        /// <returns>周期类型</returns>
        public static List<KeyValueInfo> GetPeriodType()
        {
            return _PeriodType;
        }
        /// <summary>
        /// 根据周期类型编号获取周期类型描述
        /// </summary>
        /// <param name="id">周期类型编号</param>
        /// <returns>周期类型描述</returns>
        public static string GetPeriodTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetPeriodType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion
        
		#region "ContractType"
        private static List<KeyValueInfo> _ContractType = null;
        /// <summary>
        /// 获取合同类型
        /// </summary>
        /// <returns>合同类型</returns>
        public static List<KeyValueInfo> GetContractType()
        {
            return _ContractType;
        }
        /// <summary>
        /// 根据合同类型编号获取合同类型描述
        /// </summary>
        /// <param name="id">合同类型编号</param>
        /// <returns>合同类型描述</returns>
        public static string GetContractTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetContractType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "ContractScope"
        private static List<KeyValueInfo> _ContractScope = null;
        /// <summary>
        /// 获取合同服务范围
        /// </summary>
        /// <returns>合同服务范围</returns>
        public static List<KeyValueInfo> GetContractScope()
        {
            return _ContractScope;
        }
        /// <summary>
        /// 根据合同服务范围编号获取合同服务范围描述
        /// </summary>
        /// <param name="id">合同服务范围编号</param>
        /// <returns>合同服务范围描述</returns>
        public static string GetContractScopeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetContractScope();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "Department Type"
        private static List<KeyValueInfo> _DepartmentTypes = null;
        /// <summary>
        /// 获取科室分类信息
        /// </summary>
        /// <returns>科室分类信息</returns>
        public static List<KeyValueInfo> GetDepartmentTypes()
        {
            return _DepartmentTypes;
        }
        /// <summary>
        /// 根据科室分类编号获取科室分类名称
        /// </summary>
        /// <param name="id">科室分类编号</param>
        /// <returns>科室分类名称</returns>
        public static string GetDepartmentTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetDepartmentTypes();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "Department"
        private static List<DepartmentInfo> _Departments = null;
        /// <summary>
        /// 更新科室信息
        /// </summary>
        public static void RecacheDepartments()
        {
            _Departments = departmentDao.QueryDepartments(0, "", "", "d.Seq", true, 0, 0);
        }

        /// <summary>
        /// 获取科室信息
        /// </summary>
        /// <returns>科室信息</returns>
        public static List<DepartmentInfo> GetDepartments()
        {
            return _Departments;
        }
        /// <summary>
        /// 根据科室编号获取科室名称
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <returns>科室名称</returns>
        public static string GetDepartmentDesc(int id)
        {
            List<DepartmentInfo> keyValues = GetDepartments();

            string desc = (from DepartmentInfo temp in keyValues where temp.ID == id select temp.Description).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        /// <summary>
        /// 根据科室编号获取科室信息
        /// </summary>
        /// <param name="id">科室编号</param>
        /// <returns>科室信息</returns>
        public static DepartmentInfo GetDepartments(int id)
        {
            List<DepartmentInfo> keyValues = GetDepartments();

            return (from DepartmentInfo temp in keyValues where temp.ID == id select temp).FirstOrDefault();
        }
        #endregion
        
        #region "RequestStatus"
        private static List<KeyValueInfo> _RequestStatus = null;
        /// <summary>
        /// 获取请求状态
        /// </summary>
        /// <returns>请求状态</returns>
        public static List<KeyValueInfo> GetRequestStatus()
        {
            return _RequestStatus;
        }
        /// <summary>
        /// 根据请求状态编号获取请求状态描述
        /// </summary>
        /// <param name="id">请求状态编号</param>
        /// <returns>请求状态描述</returns>
        public static string GetRequestStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetRequestStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "RequestType"
        private static List<KeyValueInfo> _RequestType = null;
        /// <summary>
        /// 获取请求类型
        /// </summary>
        /// <returns>请求类型</returns>
        public static List<KeyValueInfo> GetRequestTypes()
        {
            return _RequestType;
        }
        /// <summary>
        /// 根据请求类型编号获取请求类型描述
        /// </summary>
        /// <param name="id">请求类型编号</param>
        /// <returns>请求类型描述</returns>
        public static string GetRequestTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetRequestTypes();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "DispatchStatus"
        private static List<KeyValueInfo> _DispatchStatus = null;
        /// <summary>
        /// 获取派工单状态信息
        /// </summary>
        /// <returns>派工单状态信息</returns>
        public static List<KeyValueInfo> GetDispatchStatus()
        {
            return _DispatchStatus;
        }
        /// <summary>
        /// 根据派工单状态编号获取派工单状态描述
        /// </summary>
        /// <param name="id">派工单那状态编号</param>
        /// <returns>派工单状态描述</returns>
        public static string GetDispatchStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetDispatchStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "FaultType"
        private static List<KeyValueInfo> _FaultType = null;
        /// <summary>
        /// 获取维修请求故障分类
        /// </summary>
        /// <returns>故障分类信息</returns>
        public static List<KeyValueInfo> GetFaultType()
        {
            return _FaultType;
        }
        /// <summary>
        /// 根据故障分类编号获取故障分类描述
        /// </summary>
        /// <param name="id">故障分类编号</param>
        /// <returns>故障分类描述</returns>
        public static string GetFaultTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetFaultType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "DealType"
        private static List<KeyValueInfo> _DealType = null;
        /// <summary>
        /// 获取请求处理方式信息
        /// </summary>
        /// <returns>请求处理方式信息</returns>
        public static List<KeyValueInfo> GetDealType()
        {
            return _DealType;
        }
        /// <summary>
        /// 根据请求处理方式编号获取请求处理方式描述
        /// </summary>
        /// <param name="id">请求处理方式编号</param>
        /// <returns>请求处理方式描述</returns>
        public static string GetDealTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetDealType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "Urgency"
        private static List<KeyValueInfo> _Urgency = null;
        /// <summary>
        /// 获取派工单紧急程度信息
        /// </summary>
        /// <returns>派工单紧急程度信息</returns>
        public static List<KeyValueInfo> GetUrgency()
        {
            return _Urgency;
        }
        /// <summary>
        /// 根据派工单紧急程度编号获取描述
        /// </summary>
        /// <param name="id">紧急程度编号</param>
        /// <returns>紧急程度描述</returns>
        public static string GetUrgencyDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetUrgency();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region "DispatchDocStatus"
        private static List<KeyValueInfo> _DispatchDocStatus = null;
        /// <summary>
        /// 获取服务凭证、作业报告审批状态
        /// </summary>
        /// <returns>审批状态类型</returns>
        public static List<KeyValueInfo> GetDispatchDocStatus()
        {
            return _DispatchDocStatus;

        }
        /// <summary>
        /// 根据审批状态编号获取审批状态描述
        /// </summary>
        /// <param name="id">审批状态编号</param>
        /// <returns>审批状态描述</returns>
        public static string GetDispatchDocStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetDispatchDocStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }

        #endregion

        #region "DispatchJournalResultStatus"
        private static List<KeyValueInfo> _DispatchJournalResultStatus = null;
        /// <summary>
        /// 获取服务凭证结果
        /// </summary>
        /// <returns>服务凭证结果类型</returns>
        public static List<KeyValueInfo> GetDispatchJournalResultStatus()
        {
            return _DispatchJournalResultStatus;

        }
        /// <summary>
        /// 根据服务凭证结果编号获取服务凭证结果描述信息
        /// </summary>
        /// <param name="id">服务凭证结果编号</param>
        /// <returns>服务凭证结果描述</returns>
        public static string GetDispatchJournalResultStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetDispatchJournalResultStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }

        #endregion

        #region "DispatchReportType do not used in controller directly, please use DispatchReportTypes class instead"
        private static List<KeyValueInfo> _DispatchReportTypes = null;
        /// <summary>
        /// 获取作业报告类型
        /// </summary>
        /// <returns>作业报告类型信息</returns>
        public static List<KeyValueInfo> GetDispatchReportTypes()
        {
            return _DispatchReportTypes;

        }
        /// <summary>
        /// 根据作业报告类型编号获取作业报告类型
        /// </summary>
        /// <param name="id">作业报告类型</param>
        /// <returns>作业报告类型</returns>
        public static KeyValueInfo GetDispatchReportType(int id)
        {
            List<KeyValueInfo> keyValues = GetDispatchReportTypes();

            return (from KeyValueInfo temp in keyValues where temp.ID == id select temp).FirstOrDefault();
        }

        #endregion
        
        #region "SolutionResultStatus"
        private static List<KeyValueInfo> _SolutionResultStatus = null;
        /// <summary>
        /// 获取作业报告结果类型
        /// </summary>
        /// <returns>作业报告结果类型</returns>
        public static List<KeyValueInfo> GetSolutionResultStatus()
        {
            return _SolutionResultStatus;

        }
        /// <summary>
        /// 根据作业报告结果编号获取作业报告结果描述
        /// </summary>
        /// <param name="id">作业报告结果编号</param>
        /// <returns>作业报告结果描述</returns>
        public static string GetSolutionResultStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetSolutionResultStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }

        #endregion

        #region "AccessorySourceType"
        private static List<KeyValueInfo> _AccessorySourceType = null;
        /// <summary>
        /// 获取零配件来源信息
        /// </summary>
        /// <returns>零配件来源信息</returns>
        public static List<KeyValueInfo> GetAccessorySourceType()
        {
            return _AccessorySourceType;

        }
        /// <summary>
        /// 根据零配件来源编号获取零配件来源描述
        /// </summary>
        /// <param name="id">零配件来源编号</param>
        /// <returns>零配件来源描述</returns>
        public static string GetAccessorySourceTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetAccessorySourceType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }

        #endregion

        #endregion

        #region atoi2

        #region componentType

        private static List<KeyValueInfo> _ComponentType = null;
        /// <summary>
        /// 获取零件类型
        /// </summary>
        /// <returns>零配件来源信息</returns>
        public static List<KeyValueInfo> GetComponentType()
        {
            return _ComponentType;

        }
        /// <summary>
        /// 根据零件类型编号获取描述信息
        /// </summary>
        /// <param name="id">零件类型编号</param>
        /// <returns>零件类型描述</returns>
        public static string GetComponentTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetComponentType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region consumableType

        private static List<KeyValueInfo> _ConsumableType = null;
        /// <summary>
        /// 获取耗材类型信息
        /// </summary>
        /// <returns>耗材类型信息</returns>
        public static List<KeyValueInfo> GetConsumableType()
        {
            return _ConsumableType;

        }
        /// <summary>
        /// 根据耗材类型编号获取描述信息
        /// </summary>
        /// <param name="id">耗材类型编号</param>
        /// <returns>耗材类型描述</returns>
        public static string GetConsumabletypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetConsumableType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region EquipmentType

        private static List<KeyValueInfo> _EquipmentType = null;
        /// <summary>
        /// 获取设备类别1信息
        /// </summary>
        /// <returns>设备类别1信息</returns>
        public static List<KeyValueInfo> GetEquipmentType()
        {
            return _EquipmentType;

        }
        /// <summary>
        /// 根据设备类别1编号获取描述信息
        /// </summary>
        /// <param name="id">设备类别1编号</param>
        /// <returns>设备类别1描述</returns>
        public static string GetEquipmentTypeDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetEquipmentType();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #region HospitalLevel


        private static List<HospitalLevelInfo> _HospitalLevel = null;
        /// <summary>
        /// 获取医院等级信息
        /// </summary>
        /// <returns>医院等级信息</returns>
        public static List<HospitalLevelInfo> GetHospitalLevels()
        {
            return _HospitalLevel;

        }
        /// <summary>
        /// 根据医院等级编号获取描述信息
        /// </summary>
        /// <param name="id">医院等级编号</param>
        /// <returns>医院等级描述</returns>
        public static HospitalLevelInfo GetHospitalLevel(int id)
        {
            List<HospitalLevelInfo> keyValues = GetHospitalLevels();

            return (from HospitalLevelInfo temp in keyValues where temp.ID == id select temp).FirstOrDefault();
        }
        #endregion

        #region ObjectType


        private static List<ObjectTypeInfo> _ObjectType = null;
        /// <summary>
        /// 获取对象类型信息
        /// </summary>
        /// <returns>对象类型信息</returns>
        public static List<ObjectTypeInfo> GetObjectTypes()
        {
            return _ObjectType;

        }
        /// <summary>
        /// 根据对象类型编号获取对象
        /// </summary>
        /// <param name="id">对象类型编号</param>
        /// <returns>对象类型对象</returns>
        public static ObjectTypeInfo GetObjectType(int id)
        {
            List<ObjectTypeInfo> keyValues = GetObjectTypes();

            return (from ObjectTypeInfo temp in keyValues where temp.ID == id select temp).FirstOrDefault();
        }

        /// <summary>
        /// 根据对象类型编号获取Table key
        /// </summary>
        /// <param name="id">对象类型编号</param>
        /// <returns>对象类型Table key</returns>
        public static string GetObjectTypeKey(int id)
        {
            List<ObjectTypeInfo> keyValues = GetObjectTypes();

            return (from ObjectTypeInfo temp in keyValues where temp.ID == id select temp.TableKey).FirstOrDefault();
        }

        /// <summary>
        /// 根据对象类型编号获取Description
        /// </summary>
        /// <param name="id">对象类型编号</param>
        /// <returns>对象类型Description</returns>
        public static string GetObjectTypeDescription(int id)
        {
            List<ObjectTypeInfo> keyValues = GetObjectTypes();

            return (from ObjectTypeInfo temp in keyValues where temp.ID == id select temp.Description).FirstOrDefault();
        }

        /// <summary>
        /// 根据对象类型编号获取Object ID
        /// </summary>
        /// <param name="typeId">对象类型编号</param>
        /// <param name="objectId">对象编号</param>
        /// <returns>对象Object ID</returns>
        public static string GetObjectOID(int typeId, int objectId)
        {
            List<ObjectTypeInfo> keyValues = GetObjectTypes();

            ObjectTypeInfo info = (from ObjectTypeInfo temp in keyValues where temp.ID == typeId select temp).FirstOrDefault();

            if (info == null)
                return "";
            else
                return info.GenerateOID(objectId);
        }
        #endregion

        #region PurchaseOrderStatus

        private static List<KeyValueInfo> _PurchaseOrderStatus = null;
        /// <summary>
        /// 获取采购单状态
        /// </summary>
        /// <returns>采购单状态信息</returns>
        public static List<KeyValueInfo> GetPurchaseOrderStatus()
        {
            return _PurchaseOrderStatus;

        }
        /// <summary>
        /// 根据采购单状态编号获取描述信息
        /// </summary>
        /// <param name="id">采购单状态编号</param>
        /// <returns>采购单状态描述</returns>
        public static string GetPurchaseOrderStatusDesc(int id)
        {
            List<KeyValueInfo> keyValues = GetPurchaseOrderStatus();

            string desc = (from KeyValueInfo temp in keyValues where temp.ID == id select temp.Name).FirstOrDefault();

            if (desc == null)
                return "";
            else
                return desc;
        }
        #endregion

        #endregion

    }
}
