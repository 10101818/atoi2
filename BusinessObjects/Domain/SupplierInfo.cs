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
    /// 供应商信息
    /// </summary>
    public class SupplierInfo : EntityInfo
    {
        /// <summary>
        /// 供应商类型
        /// </summary>
        /// <value>
        /// The type of the supplier.
        /// </value>
        public KeyValueInfo SupplierType { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// 供应商省份
        /// </summary>
        /// <value>
        /// The province.
        /// </value>
        public string Province { get; set; }
        /// <summary>
        /// 供应商电话
        /// </summary>
        /// <value>
        /// The mobile.
        /// </value>
        public string Mobile { get; set; }
        /// <summary>
        /// 供应商地址
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
        /// <summary>
        ///  供应商联系人
        /// </summary>
        /// <value>
        /// The contact.
        /// </value>
        public string Contact { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        /// <value>
        /// The contact mobile.
        /// </value>
        public string ContactMobile { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        /// <value>
        /// The add date.
        /// </value>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
        /// <summary>
        /// 供应商类型
        /// </summary>
        /// <value>
        /// The supplier file.
        /// </value>
        public List<UploadFileInfo> SupplierFile { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Supplier, this.ID); } }
        /// <summary>
        /// 供应商信息
        /// </summary>
        public SupplierInfo()
        {
            this.SupplierType = new KeyValueInfo();
        }
        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public SupplierInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.SupplierType.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.SupplierType.Name = Manager.LookupManager.GetSupplierTypeDesc(this.SupplierType.ID);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Province = SQLUtil.TrimNull(dr["Province"]);
            this.Mobile = SQLUtil.TrimNull(dr["Mobile"]);
            this.Address = SQLUtil.TrimNull(dr["Address"]);
            this.Contact = SQLUtil.TrimNull(dr["Contact"]);
            this.ContactMobile = SQLUtil.TrimNull(dr["ContactMobile"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.IsActive = SQLUtil.ConvertBoolean(dr["IsActive"]);
        }
        /// <summary>
        /// 获取修改的字段
        /// </summary>
        /// <param name="newInfo">修改后的信息</param>
        /// <returns>修改的字段</returns>
        public DataTable GetChangedFields(SupplierInfo newInfo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");

            if (this.SupplierType.ID != newInfo.SupplierType.ID)
            {
                dt.Rows.Add("SupplierType", this.SupplierType.Name, Manager.LookupManager.GetSupplierTypeDesc(newInfo.SupplierType.ID));
            }

            if (this.Name != SQLUtil.TrimNull(newInfo.Name))
            {
                dt.Rows.Add("SupplierName", this.Name, newInfo.Name == null ? "" : newInfo.Name);
            }

            if (this.Province != SQLUtil.TrimNull(newInfo.Province))
            {
                dt.Rows.Add("SupplierProvince", this.Province, newInfo.Province == null ? "" : newInfo.Province);
            }

            if (this.Mobile != SQLUtil.TrimNull(newInfo.Mobile))
            {
                dt.Rows.Add("SupplierMobile", this.Mobile, newInfo.Mobile == null ? "" : newInfo.Mobile);
            }

            if (this.Address != SQLUtil.TrimNull(newInfo.Address))
            {
                dt.Rows.Add("SupplierAddress", this.Address, newInfo.Address == null ? "" : newInfo.Address);
            }

            if (this.Contact != SQLUtil.TrimNull(newInfo.Contact))
            {
                dt.Rows.Add("SupplierContact", this.Contact, newInfo.Contact == null ? "" : newInfo.Contact);
            }

            if (this.ContactMobile != SQLUtil.TrimNull(newInfo.ContactMobile))
            {
                dt.Rows.Add("SupplierContactMobile", this.ContactMobile, newInfo.ContactMobile == null ? "" : newInfo.ContactMobile);
            }            
                
            if (this.IsActive != newInfo.IsActive)
            {
                dt.Rows.Add("SupplierStatus", SQLUtil.ConvertBoolean(this.IsActive) ? "启用" : "停用", SQLUtil.ConvertBoolean(newInfo.IsActive) ? "启用" : "停用");
            }
            return dt;
        }

        /// <summary>
        /// 供应商附件信息
        /// </summary>
        public static class FileTypes
        {
            /// <summary>
            /// 许可证
            /// </summary>
            public const int ServiceSupplier = 1;
            /// <summary>
            /// 医疗器械经营证书
            /// </summary>
            public const int SupplierAttachment = 2;

            /// <summary>
            /// 获取附件类型描述
            /// </summary>
            /// <param name="id">附件类型id</param>
            /// <returns>附件类型描述</returns>
            public static string GetFileName(int id)
            {
                switch (id)
                {
                    //case ServiceSupplier:
                    //    return "许可证";
                    //case SupplierAttachment:
                    //    return "医疗器械经营证书";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 供应商类型
        /// </summary>
        public static class SupplierTypes
        {
            /// <summary>
            /// 厂商
            /// </summary>
            public const int Manufacturer = 1;
            /// <summary>
            /// 代理商
            /// </summary>
            public const int Agent = 2;
            /// <summary>
            /// 经销商
            /// </summary>
            public const int Dealer = 3;
            /// <summary>
            /// 其它供应商
            /// </summary>
            public const int Other = 4;
        }
    }
}
