using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 科室信息
    /// </summary>
    public class DepartmentInfo : EntityInfo
    {
        /// <summary>
        /// 科室名称
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 科室拼音简称
        /// </summary>
        public string Pinyin { get; set; }
        /// <summary>
        /// 排序位置
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// 科室类型
        /// </summary>
        public KeyValueInfo DepartmentType { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Department, this.ID); } }

        /// <summary>
        /// 科室信息
        /// </summary>
        public DepartmentInfo()
        {
            this.DepartmentType = new KeyValueInfo();
        }

        /// <summary>
        /// 获取科室信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DepartmentInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Seq = SQLUtil.ConvertInt(dr["Seq"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.Pinyin = SQLUtil.TrimNull(dr["Pinyin"]);
            this.DepartmentType.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.DepartmentType.Name = Manager.LookupManager.GetDepartmentTypeDesc(this.DepartmentType.ID);
        }
    }

}
