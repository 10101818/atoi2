using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using PostSharp.Extensibility;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// LookupDao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class LookupDao : BaseDao
    {
        /// <summary>
        /// 根据表名获取数据库信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="descField">字段名</param>
        /// <returns>数据库信息</returns>
        public List<KeyValueInfo> GetLookups(string tableName, string descField = null)
        {
            string desc = string.IsNullOrEmpty(descField) ? "Description" : descField; 

            List<KeyValueInfo> infos = new List<KeyValueInfo>();

            sqlStr = string.Format("SELECT * FROM {0} ORDER BY ID", tableName);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new KeyValueInfo() { ID = SQLUtil.ConvertInt(dr["ID"]), Name = SQLUtil.TrimNull(dr[desc]) });
                    }
                }
            }

            return infos;
        }
        
        /// <summary>
        /// 获取设备类别信息
        /// </summary>
        /// <returns>设备类别信息</returns>
        public List<EquipmentClassInfo> GetEquipmentClasses()
        {
            List<EquipmentClassInfo> infos = new List<EquipmentClassInfo>();

            sqlStr = "SELECT * FROM lkpEquipmentClass";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentClassInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 获取医院等级信息
        /// </summary>
        /// <returns>医院等级信息</returns>
        public List<HospitalLevelInfo> GetHospitalLevels()
        {
            List<HospitalLevelInfo> infos = new List<HospitalLevelInfo>();

            sqlStr = "SELECT * from lkpHospitalLevel ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new HospitalLevelInfo(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        /// <returns>对象类型</returns>
        public List<ObjectTypeInfo> GetObjectTypes()
        {
            List<ObjectTypeInfo> infos = new List<ObjectTypeInfo>();

            sqlStr = "SELECT * from lkpObjectType ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ObjectTypeInfo(dr));
                    }
                }
            }

            return infos;
        }
    }
}
