using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessObjects.Domain
{
    /// <summary>
    /// 日志信息
    /// </summary>
    public class AuditHdrInfo : EntityInfo
    {
        /// <summary>
        /// 操作用户
        /// </summary>
        public UserInfo TransUser { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 操作对象类型
        /// </summary>
        public ObjectTypeInfo ObjectType { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public KeyValueInfo Operation { get; set; }
        /// <summary>
        /// 操作对象id
        /// </summary>
        public int ObjectID { get; set; }
        /// <summary>
        /// 操作对象系统编号
        /// </summary>
        public string ObjectOID { get {return LookupManager.GetObjectOID(this.ObjectType.ID, this.ObjectID);}}
        /// <summary>
        /// 日志系统编号
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.SysAuditLog, this.ID); } }
        /// <summary>
        /// 日志详情信息
        /// </summary>
        public List<AuditDetailInfo> DetailInfo { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public AuditHdrInfo()
        {
            this.TransUser = new UserInfo();
            this.ObjectType = new ObjectTypeInfo();
            this.DetailInfo = new List<AuditDetailInfo>();
            this.Operation = new KeyValueInfo();
        }

        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public AuditHdrInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.TransUser.ID = SQLUtil.ConvertInt(dr["UserID"]);
            this.TransUser.LoginID = SQLUtil.TrimNull(dr["LoginID"]);
            this.TransUser.Name = SQLUtil.TrimNull(dr["Name"]);
            this.TransUser.Role.ID = SQLUtil.ConvertInt(dr["RoleID"]);
            this.TransUser.Role.Name = Manager.LookupManager.GetRoleDesc(this.TransUser.Role.ID);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            this.ObjectType.ID = SQLUtil.ConvertInt(dr["ObjectTypeID"]);
            this.ObjectType = LookupManager.GetObjectType(this.ObjectType.ID);
            this.ObjectID = SQLUtil.ConvertInt(dr["ObjectID"]);
            this.Operation.ID = SQLUtil.ConvertInt(dr["Operation"]);
            this.Operation.Name = AuditOperations.GetOperationName(this.Operation.ID);
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public class AuditOperations
        {
            /// <summary>
            /// 新增
            /// </summary>
            public const int Add = 1;
            /// <summary>
            /// 编辑
            /// </summary>
            public const int Update = 2;

            /// <summary>
            /// 获取系统日志类型
            /// </summary>
            /// <returns>合同状态信息</returns>
            public static List<KeyValueInfo> GetOperationTypes()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = AuditOperations.Add, Name = GetOperationName(AuditOperations.Add) });
                statuses.Add(new KeyValueInfo() { ID = AuditOperations.Update, Name = GetOperationName(AuditOperations.Update) });
                return statuses;
            }
            /// <summary>
            /// 获取操作对象
            /// </summary>
            /// <param name="objectTypeID">操作对象id</param>
            /// <returns>操作对象描述</returns>
            public static string GetOperationName(int objectTypeID)
            {
                switch (objectTypeID)
                {
                    case Add:
                        return "新增";
                    case Update:
                        return "编辑";
                    default:
                        return "";
                }
            }
        } 
    }

    public static partial class ListHelper
    {
        /// <summary>
        /// 获取日志详情dt
        /// </summary>
        /// <param name="list">日志详情信息</param>
        /// <param name="auditID">日志id</param>
        /// <returns>dt</returns>
        public static DataTable ConvertAuditDetailDT(this List<AuditDetailInfo> list, int auditID)
        {
            DataTable dt = new DataTable();
            if(auditID!=0)
                dt.Columns.Add("AuditID");
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");
            list.ForEach(info =>
            {
                if (auditID != 0)
                    dt.Rows.Add(auditID, info.FieldName, info.OldValue, info.NewValue);
                else
                    dt.Rows.Add(info.FieldName, info.OldValue, info.NewValue);
            });
            return dt;
        }

        /// <summary>
        /// 获取日志dt
        /// </summary>
        /// <param name="list">AuditHdrInfo</param>
        /// <returns>日志dt</returns>
        public static DataTable ConvertAuditDT(this List<AuditHdrInfo> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserID");
            dt.Columns.Add("Operation");
            dt.Columns.Add("ObjectTypeID");
            dt.Columns.Add("ObjectID");
            dt.Columns.Add("ID");
            list.ForEach(info =>
            {
                dt.Rows.Add(info.TransUser.ID, info.Operation.ID, info.ObjectType.ID, info.ObjectID);
            });
            return dt;
        }
    }
}
