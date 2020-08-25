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
    /// 自定义报表信息
    /// </summary>
    public class CustomReportInfo : EntityInfo
    {
        /// <summary>
        /// 自定义报表类型
        /// </summary>
        /// <value>
        /// 自定义报表类型
        /// </value>
        public KeyValueInfo Type { get; set; }
        /// <summary>
        /// 添加者信息
        /// </summary>
        /// <value>
        /// 添加者信息
        /// </value>
        public UserInfo CreateUser { get; set; }
        /// <summary>
        /// 自定义报表名称
        /// </summary>
        /// <value>
        /// 自定义报表名称
        /// </value>
        public String Name { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        /// <value>
        ///  添加日期
        /// </value>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 上一次修改日期
        /// </summary>
        /// <value>
        /// 上一次修改日期
        /// </value>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 上次运行日期
        /// </summary>
        /// <value>
        /// 上次运行日期
        /// </value>
        public DateTime LastRunDate { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        /// <value>
        /// 报表字段
        /// </value>
        public List<CustRptFieldInfo> Fields { get; set; }
        /// <summary>
        /// 报表编号
        /// </summary>
        /// <value>
        /// 报表编号
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.CustomReport, this.ID); } }

        /// <summary>
        ///自定义报表信息
        /// </summary>
        public CustomReportInfo()
        {
            this.Type = new KeyValueInfo();
            this.CreateUser = new UserInfo();
            this.Fields = new List<CustRptFieldInfo>();
        }

        /// <summary>
        /// 获取自定义报表信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public CustomReportInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.CreateUser.ID = SQLUtil.ConvertInt(dr["CreateUserID"]);
            this.CreateUser.Name = SQLUtil.TrimNull(dr["CreateUserName"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.CreatedDate = SQLUtil.ConvertDateTime(dr["CreatedDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);
            this.LastRunDate = SQLUtil.ConvertDateTime(dr["LastRunDate"]);
            this.Type.ID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.Type.Name = LookupManager.GetCustRptTypeDesc(this.Type.ID);            
        }
        /// <summary>
        /// 是否包含字段
        /// </summary>
        /// <param name="columnName">字段名称</param>
        /// <returns>是否包含字段</returns>
        public bool ContainField(string columnName)
        {
            return (from CustRptFieldInfo temp in this.Fields where temp.FieldName.Equals(columnName) select temp).Count() > 0;
        }

        /// <summary>
        /// 自定义报表类型定义
        /// </summary>
        public static class CustRptType
        {
            /// <summary>
            /// 设备报表
            /// </summary>
            public const int Equipment = 1;
            /// <summary>
            /// 合同报表
            /// </summary>
            public const int Contract = 2;
            /// <summary>
            /// 请求报表
            /// </summary>
            public const int Request = 3;
            /// <summary>
            /// 派工单报表
            /// </summary>
            public const int Dispatch = 4;
            /// <summary>
            /// 获取报表类型
            /// </summary>
            /// <param name="tableAlias">lkp</param>
            /// <param name="objectTypeId">报表类型id</param>
            /// <returns>报表类型</returns>
            public static string GetSqlStr4ObjectType(string tableAlias, int objectTypeId)
            {
                int custRptTypeId = 99;
                switch(objectTypeId)
                {
                    case ObjectTypes.Equipment:
                        custRptTypeId = Equipment;
                        break;
                    case ObjectTypes.Contract:
                    case ObjectTypes.Supplier:
                        custRptTypeId = Contract;
                        break;
                    case ObjectTypes.Request:
                        custRptTypeId = Request;
                        break;
                    case ObjectTypes.Dispatch:
                        custRptTypeId = Dispatch;
                        break;
                    default:
                         break;
                }

                return string.Format("{0}.TypeID = {1}", tableAlias, custRptTypeId);
            }

        }
    }
    /// <summary>
    /// 自定义报表绑定信息
    /// </summary>
    public class CustRptFieldInfo
    {
        /// <summary>
        /// 自定义报表编号
        /// </summary>
        /// <value>
        /// 自定义报表编号
        /// </value>
        public int CustomReportID { get; set; }
        /// <summary>
        /// 字段所属表
        /// </summary>
        /// <value>
        /// 字段所属表名
        /// </value>
        public string TableName { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        /// <value>
        /// 字段名
        /// </value>
        public string FieldName { get; set; }
        /// <summary>
        /// 自定义报表字段信息
        /// </summary>
        public CustRptFieldInfo(){}

        /// <summary>
        /// 获取自定义报表字段内容
        /// </summary>
        /// <param name="dr">The dr.</param>
        public CustRptFieldInfo(DataRow dr)
            : this()
        {
            this.CustomReportID = SQLUtil.ConvertInt(dr["CustomReportID"]);
            this.TableName = SQLUtil.TrimNull(dr["TableName"]);
            this.FieldName = SQLUtil.TrimNull(dr["FieldName"]);
        }
    }
    /// <summary>
    /// 自定义报表绑定table信息
    /// </summary>
    public class CustRptTemplateTableInfo
    {
        /// <summary>
        /// 自定义报表类型
        /// </summary>
        /// <value>
        /// 自定义报表类型
        /// </value>
        public int TypeID { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        /// <value>
        /// 表名
        /// </value>
        public string TableName { get; set; }
        /// <summary>
        /// 表中文描述
        /// </summary>
        /// <value>
        /// 表中文描述
        /// </value>
        public string TableDesc { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        /// <value>
        /// 字段
        /// </value>
        public List<CustRptTemplateFieldInfo> Fields { get; set; }
        /// <summary>
        /// 自定义报表表信息
        /// </summary>
        public CustRptTemplateTableInfo() { }
        /// <summary>
        /// 获取自定义报表表信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public CustRptTemplateTableInfo(DataRow dr)
            : this()
        {
            this.TypeID = SQLUtil.ConvertInt(dr["TypeID"]);
            this.TableName = SQLUtil.TrimNull(dr["TableName"]);
            this.TableDesc = SQLUtil.TrimNull(dr["TableDesc"]);
        }
    }
    /// <summary>
    /// 自定义报表绑定字段信息
    /// </summary>
    public class CustRptTemplateFieldInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段中文名称
        /// </summary>
        /// <value>
        /// 字段中文名称
        /// </value>
        public string FieldDesc { get; set; }
        /// <summary>
        /// 自定义报表字段信息
        /// </summary>
        public CustRptTemplateFieldInfo() { }
        /// <summary>
        /// 获取自定义报表字段信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public CustRptTemplateFieldInfo(DataRow dr)
            : this()
        {
            this.TableName = SQLUtil.TrimNull(dr["TableName"]);
            this.FieldName = SQLUtil.TrimNull(dr["FieldName"]);
            this.FieldDesc = SQLUtil.TrimNull(dr["FieldDesc"]);
        }
    }
}
