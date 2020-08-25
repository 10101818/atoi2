using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 合同信息
    /// </summary>
    public class ContractInfo : EntityInfo
    {
        /// <summary>
        /// 设备信息集合
        /// </summary>
        /// <value>
        /// 设备信息
        /// </value>
        public List<EquipmentInfo> Equipments { get; set; }
        /// <summary>
        /// 零件信息
        /// </summary>
        public List<ContractComponentInfo> Components { get; set; }
        /// <summary>
        /// 耗材信息
        /// </summary>
        public List<ContractConsumableInfo> Consumables { get; set; }

        /// <summary>
        /// 厂商信息
        /// </summary>
        /// <value>
        /// 厂商信息
        /// </value>
        public SupplierInfo Supplier { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        /// <value>
        /// 合同编号
        /// </value>
        public string ContractNum { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        /// <value>
        /// 合同名称
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// 合同类型
        /// </summary>
        /// <value>
        /// 合同类型
        /// </value>
        public KeyValueInfo Type { get; set; }
        /// <summary>
        /// 服务范围
        /// </summary>
        /// <value>
        /// 服务范围
        /// </value>
        public KeyValueInfo Scope { get; set; }
        /// <summary>
        /// 其它范围备注
        /// </summary>
        /// <value>
        /// 其它范围备注
        /// </value>
        public string ScopeComments { get; set; }
        /// <summary>
        /// 合同金额
        /// </summary>
        /// <value>
        /// 合同金额
        /// </value>
        public double Amount { get; set; }
        /// <summary>
        /// 投标项目编号
        /// </summary>
        /// <value>
        /// 投标项目编号
        /// </value>
        public string ProjectNum { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <value>
        /// 开始日期
        /// </value>
        public DateTime StartDate { get; set; }
        /// <summary>
        ///  结束日期
        /// </summary>
        /// <value>
        ///  结束日期
        /// </value>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <value>
        /// 备注
        /// </value>
        public string Comments { get; set; }

        //display only
        /// <summary>
        /// 根据合同起止时间计算合同状态
        /// </summary>
        public string Status
        {
            get
            {
                int statusId = 0;
                if ((this.EndDate - DateTime.Now).Days < 0)
                    statusId = Statuses.Expired;
                else if ((this.StartDate - DateTime.Now).Days > 0)
                    statusId = Statuses.Pending;
                else if ((this.StartDate - DateTime.Now).Days <= 0 && (int)Math.Ceiling((this.EndDate - DateTime.Now).TotalDays) <= ControlManager.GetSettingInfo().WillExpireTime)
                    statusId = Statuses.WillExpire;
                else
                    statusId = Statuses.Active;

                return Statuses.GetStatusDesc(statusId);
            }
        }

        /// <summary>
        /// 合同附件
        /// </summary>
        /// <value>
        /// 合同附件
        /// </value>
        public UploadFileInfo ContractFile { get; set; }

        #region" Dispaly Only-Equipment、Department "

        private EquipmentInfo Equipment { get { if (this.Equipments == null || this.Equipments.Count != 1) return null; else return this.Equipments[0]; } }
        /// <summary>
        /// 设备编号
        /// </summary>
        /// <value>
        /// 设备系统编号
        /// </value>
        public string EquipmentOID
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return "多设备";
                else if (this.Equipment != null)
                    return this.Equipment.OID;
                else
                    return "";
            }
        }
        /// <summary>
        /// 设备序列号
        /// </summary>
        /// <value>
        /// 设备序列号
        /// </value>
        public string EquipmentSerialCode
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return "多设备";
                else if (this.Equipment != null)
                    return this.Equipment.SerialCode;
                else
                    return "";
            }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        /// <value>
        /// 设备名称
        /// </value>
        public string EquipmentName
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return "多设备";
                else if (this.Equipment != null)
                    return this.Equipment.Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// 设备科室编号
        /// </summary>
        /// <value>
        /// 设备科室编号
        /// </value>
        public int DepartmentID
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return this.Equipments[0].Department.ID;
                else if (this.Equipment != null)
                    return this.Equipment.Department.ID;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 设备科室名称
        /// </summary>
        /// <value>
        /// 设备科室名称
        /// </value>
        public string DepartmentName
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return this.Equipments[0].Department.Name;
                else if (this.Equipment != null)
                    return this.Equipment.Department.Name;
                else
                    return "";
            }
        }

        #endregion

        /// <summary>
        /// 合系系统同编号
        /// </summary>
        /// <value>
        /// 合同系统编号
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Contract, this.ID); } }

        /// <summary>
        /// 合同信息
        /// </summary>
        public ContractInfo()
        {
            this.Supplier = new SupplierInfo();
            this.Type = new KeyValueInfo();
            this.Scope = new KeyValueInfo();

        }

        /// <summary>
        /// 获取合同信息
        /// </summary>
        /// <param name="dr">dataRow</param>
        public ContractInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.ContractNum = SQLUtil.TrimNull(dr["ContractNum"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Type.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.Type.Name = Manager.LookupManager.GetContractTypeDesc(this.Type.ID);
            this.Scope.ID = SQLUtil.ConvertInt(dr["ScopeID"]);
            this.Scope.Name = LookupManager.GetContractScopeDesc(this.Scope.ID);
            this.ScopeComments = SQLUtil.TrimNull(dr["ScopeComments"]);
            this.Amount = SQLUtil.ConvertDouble(dr["Amount"]);
            this.ProjectNum = SQLUtil.TrimNull(dr["ProjectNum"]);
            this.StartDate = SQLUtil.ConvertDateTime(dr["StartDate"]);
            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.Supplier.ID = SQLUtil.ConvertInt(dr["SupplierID"]);
            this.Supplier.Name = SQLUtil.TrimNull(dr["supplierName"]);
        }

        /// <summary>
        /// 获取修改的字段
        /// </summary>
        /// <param name="newInfo">修改后的信息</param>
        /// <returns>修改的字段</returns>
        public DataTable GetChangedFields(ContractInfo newInfo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");

            if (this.Supplier.ID != newInfo.Supplier.ID)
            {
                dt.Rows.Add("SupplierName", this.Supplier.Name, newInfo.Supplier.Name);
            }

            if (GetStringFromObjectList(this.Equipments, "OID") != GetStringFromObjectList(newInfo.Equipments, "OID"))
            {
                dt.Rows.Add("EquipmentID", GetStringFromObjectList(this.Equipments, "OID"), GetStringFromObjectList(newInfo.Equipments, "OID"));
            }

            if (this.ContractNum != SQLUtil.TrimNull(newInfo.ContractNum))
            {
                dt.Rows.Add("ContractNum", this.ContractNum, newInfo.ContractNum == null ? "" : newInfo.ContractNum);
            }

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
            {
                dt.Rows.Add("ContractName", this.Name, newInfo.Name == null ? "" : newInfo.Name);
            }

            if (this.Type.ID != newInfo.Type.ID)
            {
                dt.Rows.Add("TypeDesc", this.Type.Name, Manager.LookupManager.GetContractTypeDesc(newInfo.Type.ID));
            }

            if (this.Scope.ID != newInfo.Scope.ID)
            {
                dt.Rows.Add("ScopeDesc", this.Scope.Name, LookupManager.GetContractScopeDesc(newInfo.Scope.ID));
            }

            if (this.ScopeComments != SQLUtil.TrimNull(newInfo.ScopeComments))
            {
                dt.Rows.Add("ScopeComments", this.ScopeComments, newInfo.ScopeComments == null ? "" : newInfo.ScopeComments);
            }

            if (this.Amount != newInfo.Amount)
            {
                dt.Rows.Add("Amount", this.Amount, newInfo.Amount);
            }

            if (this.ProjectNum != SQLUtil.TrimNull(newInfo.ProjectNum))
            {
                dt.Rows.Add("ProjectNum", this.ProjectNum, newInfo.ProjectNum == null ? "" : newInfo.ProjectNum);
            }

            if (this.StartDate != newInfo.StartDate)
            {
                dt.Rows.Add("ContractStartDate", SQLUtil.ConvertDateTime(this.StartDate) == DateTime.MinValue ? "" : this.StartDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.StartDate) == DateTime.MinValue ? "" : newInfo.StartDate.ToString("yyyy-MM-dd"));
            }

            if (this.EndDate != newInfo.EndDate)
            {
                dt.Rows.Add("ContractEndDate", SQLUtil.ConvertDateTime(this.EndDate) == DateTime.MinValue ? "" : this.EndDate.ToString("yyyy-MM-dd"), SQLUtil.ConvertDateTime(newInfo.EndDate) == DateTime.MinValue ? "" : newInfo.EndDate.ToString("yyyy-MM-dd"));
            }

            if (this.Comments != SQLUtil.TrimNull(newInfo.Comments))
            {
                dt.Rows.Add("ContractComments", this.Comments, newInfo.Comments == null ? "" : newInfo.Comments);
            }

            return dt;
        }

        private string GetStringFromObjectList(List<EquipmentInfo> data, string field)
        {
            
            if (data == null || data.Count == 0 ) return "";
            else if (data.Count == 1) return data[0].OID;
            else
            {
                string info = data[0].OID;
                for (int i = 1; i < data.Count; i++)
                {
                    info = info + ',' + data[i].OID;
                }

                return info;
            }
        }
        private int JudgeDateStatus(DateTime startDate, DateTime endDate)
        {
            if ((endDate - DateTime.Now).Days < 0) return Statuses.Expired;
            else if ((startDate - DateTime.Now).Days >= 0) return Statuses.Pending;
            else if ((startDate - DateTime.Now).Days <= 0 && (int)Math.Ceiling((endDate - DateTime.Now).TotalDays) <= ControlManager.GetSettingInfo().WillExpireTime) return Statuses.WillExpire;
            else return Statuses.Active;
        }
        /// <summary>
        /// 合同附件类型定义
        /// </summary>
        public static class FileTypes
        {
            /// <summary>
            /// 服务合同
            /// </summary>
            public const int ServiceContract = 1;
            /// <summary>
            /// 其它合同
            /// </summary>
            public const int Other = 2;

            /// <summary>
            /// 获取附件类型描述信息
            /// </summary>
            /// <param name="id">附件类型id</param>
            /// <returns>附件类型描述信息</returns>
            public static string GetFileName(int id)
            {
                switch (id)
                {
                    //case ServiceContract:
                    //    return "服务合同";
                    //case Other:
                    //    return "附件";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 合同服务范围
        /// </summary>
        public static class Scopes
        {
            /// <summary>
            /// The other
            /// </summary>
            public const int Other = 3;
        }
        /// <summary>
        /// 合同状态
        /// </summary>
        public static class Statuses
        {
            /// <summary>
            /// 失效
            /// </summary>
            public const int Expired = 1;
            /// <summary>
            /// 生效
            /// </summary>
            public const int Active = 2;
            /// <summary>
            /// 未生效
            /// </summary>
            public const int Pending = 3;
            /// <summary>
            /// 即将失效
            /// </summary>
            public const int WillExpire = 4;
            /// <summary>
            /// 根据合同状态编号获取查询该状态合同的sql
            /// </summary>
            /// <param name="statusId">合同状态编号</param>
            /// <returns>查询该状态合同的sql</returns>
            public static string GetSqlFilter(int statusId)
            {
                switch (statusId)
                {
                    case Expired:
                        return " AND DATEDIFF(DAY,c.EndDate, GETDATE()) > 0";
                    case WillExpire:
                        return " AND DATEDIFF(DAY,c.StartDate, GETDATE()) >= 0 AND DATEDIFF(DAY, c.EndDate, GETDATE()) >= -" + ControlManager.GetSettingInfo().WillExpireTime + " AND DATEDIFF(DAY,c.EndDate, GETDATE()) <= 0 ";
                    case Active:
                        return " AND DATEDIFF(DAY,c.StartDate, GETDATE()) >= 0 AND DATEDIFF(DAY,c.EndDate, GETDATE()) <= 0";
                    case Pending:
                        return " AND (DATEDIFF(DAY,c.StartDate, GETDATE()) < 0 OR DATEDIFF(SECOND,c.StartDate, GETDATE()) < 0 )";
                    default:
                        return "";
                }
            }
            /// <summary>
            /// 获取合同状态信息
            /// </summary>
            /// <returns>合同状态信息</returns>
            public static List<KeyValueInfo> GetStatuses()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = Statuses.Expired, Name = GetStatusDesc(Statuses.Expired) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.Active, Name = GetStatusDesc(Statuses.Active) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.Pending, Name = GetStatusDesc(Statuses.Pending) });
                statuses.Add(new KeyValueInfo() { ID = Statuses.WillExpire, Name = GetStatusDesc(Statuses.WillExpire) });
                return statuses;
            }
            /// <summary>
            /// 根据合同状态编号获取合同状态描述
            /// </summary>
            /// <param name="statusId">合同状态编号</param>
            /// <returns>合同状态描述</returns>
            public static string GetStatusDesc(int statusId)
            {
                switch (statusId)
                {
                    case Statuses.Expired:
                        return "失效";
                    case Statuses.Active:
                        return "生效";
                    case Statuses.Pending:
                        return "未生效";
                    case Statuses.WillExpire:
                        return "即将失效";
                    default:
                        return "全部";
                }
            }
        }
    }
    /// <summary>
    /// 合同设备关联信息
    /// </summary>
    public class ContractEqptInfo
    {
        /// <summary>
        /// 合同编号
        /// </summary>
        /// <value>
        /// 合同编号
        /// </value>
        public int ContractID { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        /// <value>
        /// 设备信息
        /// </value>
        public EquipmentInfo Equipment { get; set; }

        /// <summary>
        /// 合同关联的设备信息
        /// </summary>
        public ContractEqptInfo()
        {
            this.Equipment = new EquipmentInfo();
        }
    }
    /// <summary>
    /// 合同零件关联信息info
    /// </summary>
    public class ContractComponentInfo
    {
        /// <summary>
        /// 合同id
        /// </summary>
        public int ContractID { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 零件信息
        /// </summary>
        public ComponentInfo Component { get; set; }
        /// <summary>
        /// 合同零件信息info
        /// </summary>
        public ContractComponentInfo()
        {
            this.Component = new ComponentInfo();
            this.Equipment = new EquipmentInfo();
        }
    }
    /// <summary>
    /// 合同耗材关联信息
    /// </summary>
    public class ContractConsumableInfo
    {
        /// <summary>
        /// 合同id
        /// </summary>
        public int ContractID { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 耗材信息
        /// </summary>
        public ConsumableInfo Consumable { get; set; }
        /// <summary>
        /// 合同耗材关联信息
        /// </summary>
        public ContractConsumableInfo()
        {
            this.Consumable = new ConsumableInfo();
            this.Equipment = new EquipmentInfo();
        }
    }
}
