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
    /// 富士I类info
    /// </summary>
    public class FujiClass1Info
    {
        /// <summary>
        /// 富士I类id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 富士I类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 设备类别1
        /// </summary>
        public EquipmentClassInfo EquipmentType1 { get; set; }
        /// <summary>
        /// 设备类别2
        /// </summary>
        public EquipmentClassInfo EquipmentType2 { get; set; }
        /// <summary>
        /// 关联富士II类数量
        /// </summary>
        public int FujiClass2Count { get; set; }

        /// <summary>
        /// 富士I类info
        /// </summary>
        public FujiClass1Info()
        {
            this.EquipmentType1 = new EquipmentClassInfo();
            this.EquipmentType2 = new EquipmentClassInfo();
        }
        /// <summary>
        /// 富士I类info
        /// </summary>
        /// <param name="dr">dr</param>
        public FujiClass1Info(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);

            if (dr.Table.Columns.Contains("EquipmentType1ID"))
            {
                this.EquipmentType1.Code = SQLUtil.TrimNull(dr["EquipmentType1ID"]);
                this.EquipmentType1.Description = LookupManager.GetEquipmentClassDesc(this.EquipmentType1.Code, 1);
            }

            if (dr.Table.Columns.Contains("EquipmentType1ID"))
            {
                this.EquipmentType2.Code = SQLUtil.TrimNull(dr["EquipmentType2ID"]);
                this.EquipmentType2.Description = LookupManager.GetEquipmentClassDesc(this.EquipmentType2.Code, 2, this.EquipmentType1.Code);
            }

            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Description = SQLUtil.TrimNull(dr["Description"]);
            this.AddDate = SQLUtil.ConvertDateTime(dr["AddDate"]);
            this.UpdateDate = SQLUtil.ConvertDateTime(dr["UpdateDate"]);

            if (dr.Table.Columns.Contains("FujiClass2Count"))
            {
                this.FujiClass2Count = SQLUtil.ConvertInt(dr["FujiClass2Count"]);
            }
        }
    }
    /// <summary>
    /// 富士类别关联信息
    /// </summary>
    public class FujiClassLink
    {
        /// <summary>
        /// 设备类别1
        /// </summary>
        public EquipmentClassInfo EquipmentType1 { get; set; }
        /// <summary>
        /// 设备类别2
        /// </summary>
        public EquipmentClassInfo EquipmentType2 { get; set; }
        /// <summary>
        /// 富士I类
        /// </summary>
        public FujiClass1Info FujiClass1 { get; set; }
        /// <summary>
        /// 富士II类
        /// </summary>
        public FujiClass2Info FujiClass2 { get; set; }
        /// <summary>
        /// 富士类别关联信息
        /// </summary>
        public FujiClassLink()
        {
            this.EquipmentType1 = new EquipmentClassInfo();
            this.EquipmentType2 = new EquipmentClassInfo();
            this.FujiClass1 = new FujiClass1Info();
            this.FujiClass2 = new FujiClass2Info();
        }
        /// <summary>
        /// 富士类别关联信息
        /// </summary>
        /// <param name="dr">dr</param>
        public FujiClassLink(DataRow dr)
            : this()
        {
            this.FujiClass2.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.FujiClass2.Name = SQLUtil.TrimNull(dr["Name"]);
            this.FujiClass2.Description = SQLUtil.TrimNull(dr["Description"]);
            if (dr.Table.Columns.Contains("EquipmentType1ID"))
            {
                this.EquipmentType1.Code = SQLUtil.TrimNull(dr["EquipmentType1ID"]);
                this.EquipmentType1.Description = LookupManager.GetEquipmentClassDesc(this.EquipmentType1.Code, 1);
            }
            if (dr.Table.Columns.Contains("EquipmentType2ID"))
            {
                this.EquipmentType2.Code = SQLUtil.TrimNull(dr["EquipmentType2ID"]);
                this.EquipmentType2.Description = LookupManager.GetEquipmentClassDesc(this.EquipmentType2.Code, 2, this.EquipmentType1.Code);
            }
            if (dr.Table.Columns.Contains("FujiClass1ID"))
            {
                this.FujiClass2.FujiClass1.ID = SQLUtil.ConvertInt(dr["FujiClass1ID"]);
            }
            if (dr.Table.Columns.Contains("FujiClass1Name"))
            {
                this.FujiClass2.FujiClass1.Name = SQLUtil.TrimNull(dr["FujiClass1Name"]);
            }
            if (dr.Table.Columns.Contains("FujiClass1Description"))
            {
                this.FujiClass2.FujiClass1.Description = SQLUtil.TrimNull(dr["FujiClass1Description"]);
            }
            if (dr.Table.Columns.Contains("FujiClass2Count"))
            {
                this.FujiClass2.FujiClass1.FujiClass2Count = SQLUtil.ConvertInt(dr["FujiClass2Count"]);
            }
            if (dr.Table.Columns.Contains("hasEdited"))
            {
                this.FujiClass2.hasEdited = SQLUtil.ConvertBoolean(dr["hasEdited"]);
            }
        }
        /// <summary>
        /// 日志信息
        /// </summary>
        /// <param name="user">修改者</param>
        /// <param name="newInfo">修改后的关联信息</param>
        /// <returns>日志信息</returns>
        public AuditHdrInfo ConvertAudit(UserInfo user ,FujiClassLink newInfo = null)
        {
            bool isUpdate = newInfo != null;
            AuditHdrInfo audit = new AuditHdrInfo();
            audit.ObjectType.ID = ObjectTypes.FujiClass2;
            audit.ObjectID = this.FujiClass2.ID;
            audit.Operation.ID = isUpdate ? AuditHdrInfo.AuditOperations.Update : AuditHdrInfo.AuditOperations.Add;
            audit.TransUser = user;
            List<AuditDetailInfo> infos = audit.DetailInfo;
            if(isUpdate)
            { 
                if (this.EquipmentType1.Code.Equals(newInfo.EquipmentType1.Code)) 
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Equipment1", OldValue = this.EquipmentType1.Code, NewValue = newInfo.EquipmentType1.Code });

                if (this.EquipmentType2.Code.Equals(newInfo.EquipmentType2.Code))
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Equipment2", OldValue = this.EquipmentType2.Code, NewValue = newInfo.EquipmentType2.Code });

                if (this.FujiClass2.FujiClass1.ID != newInfo.FujiClass2.FujiClass1.ID)
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2FujiClass1", OldValue = this.FujiClass2.FujiClass1.Name, NewValue = newInfo.FujiClass2.FujiClass1.Name });
            }
            else
            {
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Equipment1", OldValue = "" , NewValue = this.EquipmentType1.Description });
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Equipment2", OldValue = "" , NewValue = this.EquipmentType2.Description });
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2FujiClass1", OldValue = "", NewValue = this.FujiClass2.FujiClass1.Name });
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Name", OldValue = "", NewValue = this.FujiClass2.Name });
                    infos.Add(new AuditDetailInfo() { FieldName = "FujiClass2Description", OldValue = "", NewValue = this.FujiClass2.Description });
            } 
            return audit;
        }
    }
}
