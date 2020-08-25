using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 科室manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class DepartmentManager
    {
        private DepartmentDao departmentDao = new DepartmentDao();
        /// <summary>
        /// 保存科室信息
        /// </summary>
        /// <param name="info">科室信息</param>
        /// <returns>科室id</returns>
        public int SaveDepartment(DepartmentInfo info)
        {
            if (info.ID >= 0){
                DepartmentInfo existingInfo = this.departmentDao.GetDepartmentByID(info.ID);
                if (existingInfo.Seq > info.Seq)
                    this.departmentDao.UpdateDepartmentSeq(info.Seq, existingInfo.Seq, 1);
                else if (existingInfo.Seq < info.Seq)
                    this.departmentDao.UpdateDepartmentSeq(existingInfo.Seq, info.Seq, -1);

                this.departmentDao.UpdateDepartment(info);
            }                
            else
            {
                this.departmentDao.UpdateDepartmentSeq(info.Seq, Int16.MaxValue, 1);

                info.ID = this.departmentDao.AddDepartment(info);
            }

            LookupManager.RecacheDepartments();

            return info.ID;
        }
    }
}
