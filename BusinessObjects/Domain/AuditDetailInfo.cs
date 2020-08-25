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
    public class AuditDetailInfo : EntityInfo
    {
        /// <summary>
        /// 日志id
        /// </summary>
        public int AuditID { get; set; }
        /// <summary>
        /// 操作字段
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 操作字段描述
        /// </summary>
        public string FieldNameDesc { get; set; }
        /// <summary>
        /// 编辑前字段信息
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 编辑后字段信息
        /// </summary>
        public string NewValue { get; set; }
        /// <summary>
        /// 日志系统编号
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.SysAuditLog, this.AuditID); } }

        /// <summary>
        /// 日志信息
        /// </summary>
        public AuditDetailInfo() { }

        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public AuditDetailInfo(DataRow dr)
            : this()
        {
            this.AuditID = SQLUtil.ConvertInt(dr["AuditID"]);
            this.FieldName = SQLUtil.TrimNull(dr["FieldName"]);
            this.FieldNameDesc = SQLUtil.TrimNull(dr["FieldNameDesc"]);
            this.OldValue = SQLUtil.TrimNull(dr["OldValue"]);
            this.NewValue = SQLUtil.TrimNull(dr["NewValue"]);
        }

    }
}
