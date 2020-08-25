using BusinessObjects.Manager;
using BusinessObjects.Util;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 重写方法
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// id
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }

        /// <summary>
        /// 重写equals方法
        /// </summary>
        /// <param name="obj">传入值</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (this.ID == ((EntityInfo)obj).ID)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 重写GetHashCode方法
        /// </summary>
        /// <returns>GetHashCode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>克隆对象</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 定义对象类型数据
    /// </summary>
    public class ObjectTypeInfo
    {
        /// <summary>
        /// key
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }
        /// <summary>
        /// Table Key
        /// </summary>
        /// <value>
        /// The Table Key.
        /// </value>
        public string TableKey { get; set; }
        /// <summary>
        /// Prefix
        /// </summary>
        /// <value>
        /// The object Prefix.
        /// </value>
        public string Prefix { get; set; }
        /// <summary>
        /// Leading Zeros
        /// </summary>
        /// <value>
        /// The Leading Zeros.
        /// </value>
        public int LeadingZeros { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        /// <value>
        /// The Description
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        public ObjectTypeInfo()
        {
        }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        /// <param name="dr">The dr.</param>
        public ObjectTypeInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.TableKey = SQLUtil.TrimNull(dr["TableKey"]);
            this.Prefix = SQLUtil.TrimNull(dr["Prefix"]);
            this.LeadingZeros = SQLUtil.ConvertInt(dr["LeadingZeros"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
        }

        /// <summary>
        /// 获取系统编号
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>系统编号</returns>
        public string GenerateOID(int id)
        {
            return string.Format("{0}{1}", this.Prefix, id.ToString("D" + this.LeadingZeros));
        }
    }

    /// <summary>
    /// 定义KeyValueInfo类型数据
    /// </summary>
    public class KeyValueInfo
    {
        /// <summary>
        /// key
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }
        /// <summary>
        /// value
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
    /// <summary>
    /// 定义文件流信息
    /// </summary>
    public class FileStreamInfo
    {
        /// <summary>
        /// key
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
        /// <summary>
        /// value
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        public Stream Stream { get; set; }
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    public class UploadFileInfo : EntityInfo
    {
        /// <summary>
        /// 文件编号
        /// </summary>
        /// <value>
        /// The object identifier.
        /// </value>
        public int ObjectID { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        /// <value>
        /// The type of the file.
        /// </value>
        public int FileType { get; set; }
        /// <summary>
        /// 文件描述
        /// </summary>
        /// <value>
        /// The file desc.
        /// </value>
        public string FileDesc { get; set; }
        /// <summary>
        /// Gets or sets the add date.
        /// </summary>
        /// <value>
        /// The add date.
        /// </value>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The type id of the object.
        /// </value>
        public int ObjectTypeId { get; set; }
        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        /// <value>
        /// The content of the file.
        /// </value>
        public string FileContent { get; set; }

        private int Seq { get; set; }

        /// <summary>
        /// 文件信息
        /// </summary>
        public UploadFileInfo() { }
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public UploadFileInfo(DataRow dr)
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.ObjectTypeId = SQLUtil.ConvertInt(dr["ObjectTypeId"]);
            this.ObjectID = SQLUtil.ConvertInt(dr["ObjectID"]);
            this.FileName = SQLUtil.TrimNull(dr["FileName"]);
            this.FileDesc = SQLUtil.TrimNull(dr["FileDesc"]);
            this.FileType = SQLUtil.ConvertInt(dr["FileType"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.Seq = SQLUtil.ConvertInt(dr["Seq"]);

            SetDisplayFileName();
        }

        private void SetDisplayFileName()
        {
            string name = GetDisplayFileName();

            if(name != "") this.FileName = name + new FileInfo(this.FileName).Extension;
        }

        /// <summary>
        /// 获取显示的文件名称
        /// </summary>
        /// <returns>文件名称</returns>
        public string GetDisplayFileName()
        {
            string name = "";

            switch(this.ObjectTypeId)
            {
                case ObjectTypes.Equipment:
                    name = EquipmentInfo.FileTypes.GetFileName(this.FileType);
                    break;
               //case ObjectTypes.Contract:
               //    name = ContractInfo.FileTypes.GetFileName(this.FileType);
               //    break;
               //case ObjectTypes.Supplier:
               //    name = SupplierInfo.FileTypes.GetFileName(this.FileType);
               //    break;
                case ObjectTypes.Request:
                    name = string.Format("{0}_{1}", RequestInfo.FileTypes.GetFileName(this.FileType), this.Seq);
                    break;
                case ObjectTypes.DispatchReport:
                    name = DispatchReportInfo.FileTypes.GetFileName(this.FileType);
                    break;
                case ObjectTypes.ReportAccessory:
                    name = ReportComponentInfo.FileTypes.GetFileName(this.FileType);
                    break;
                default:
                    break;
            }

            return name;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <returns>文件名称</returns>
        public string GetFileName()
        {
            return string.Format("{0}_{1}{2}", this.ObjectID, this.ID, new FileInfo(this.FileName).Extension);
        }
    }

    /// <summary>
    /// 流程信息
    /// </summary>
    public class HistoryInfo : EntityInfo
    {
        /// <summary>
        /// The object type.
        /// </summary>
        /// <value>
        /// The object type id. 
        /// </value>
        public int ObjectTypeID { get; set; }

        /// <summary>
        /// The object.
        /// </summary>
        /// <value>
        /// The object Id.
        /// </value>
        public int ObjectID { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public UserInfo Operator { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public KeyValueInfo Action { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string Comments { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        /// <value>
        /// The trans date.
        /// </value>
        public DateTime TransDate { get; set; }

        /// <summary>
        /// 获取流程信息
        /// </summary>
        public string VerifyHistory { get { return string.Format("{0} {1} - {2}: {3}{4}", TransDate.ToString("yyyy-MM-dd HH:mm:ss"), Operator.Role.Name == "" ? "计划服务" : Operator.Role.Name, Operator.Name == "" ? "系统" : Operator.Name, Action.Name, (Comments == "" ? "" : " - " + Comments)); } }
        /// <summary>
        /// 历史信息
        /// </summary>
        public HistoryInfo()
        {
            this.Operator = new UserInfo();
            this.Action = new KeyValueInfo();
        }

        /// <summary>
        /// 历史信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public HistoryInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.ObjectID = SQLUtil.ConvertInt(dr["ObjectID"]);
            this.Operator.ID = SQLUtil.ConvertInt(dr["OperatorID"]);
            this.Operator.Name = this.Operator.ID == 0 ? "" : SQLUtil.TrimNull(dr["OperatorName"]);
            this.Operator.Role.ID = SQLUtil.ConvertInt(dr["OperatorRoleID"]);
            this.Operator.Role.Name = this.Operator.Role.ID==0?"":LookupManager.GetRoleDesc(this.Operator.Role.ID);
            this.Action.ID = SQLUtil.ConvertInt(dr["Action"]);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.TransDate = SQLUtil.ConvertDateTime(dr["TransDate"]);
        }

        /// <summary>
        /// 历史信息
        /// </summary>
        /// <param name="objectID">The object identifier.</param>
        /// <param name="objectTypeId">The object type ID.</param>
        /// <param name="operatorID">角色编号</param>
        /// <param name="actionID">操作</param>
        /// <param name="comments">备注</param>
        public HistoryInfo(int objectID, int objectTypeId, int operatorID, int actionID = 0, string comments = "")
            : this()
        {
            this.ObjectTypeID = objectTypeId;
            this.ObjectID = objectID;
            this.Operator.ID = operatorID;
            this.Action.ID = actionID;
            this.Comments = comments;
        }
    }
    
    public static partial class ListHelper
    {
        public static void Swap<T>(this List<T> list, int begin, int end)
        {
            int _begin = begin, _end = end;
            T t = list[_begin];
            list[_begin] = list[_end];
            list[_end] =  t;
        }
        public static void SwapOrder<T>(this List<T> list, int begin, int end)
        {
            int _begin = begin, _end = end;
            if (_begin > _end)
            {
                _begin = end;
                _end = begin;
            }
            else if (_begin == _end)
                return;
            else { 
                T t = list[_begin];
                list[_begin] = list[_begin+1];
                list[_begin+1] = t;
                list.SwapOrder(++_begin, _end);
            }
        }
    }
}
