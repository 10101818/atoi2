using BusinessObjects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalEquipmentHostingSystem.Models
{
    /// <summary>
    /// DepartPerformanceModel
    /// </summary>
    public class DepartPerformanceModel
    {
        /// <summary>
        /// 科室信息
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public DepartmentInfo Department { get; set; }
        /// <summary>
        /// 服务数量
        /// </summary>
        /// <value>
        /// The service count.
        /// </value>
        public int ServiceCount { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        /// <value>
        /// The equipment count.
        /// </value>
        public int EquipmentCount { get; set; }
        /// <summary>
        /// 设备资产
        /// </summary>
        /// <value>
        /// The equipment amount.
        /// </value>
        public double EquipmentAmount { get; set; }
        /// <summary>
        /// 收入
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        public double Incomes { get; set; }
        /// <summary>
        /// 上次收入
        /// </summary>
        /// <value>
        /// The last incomes.
        /// </value>
        public double LastIncomes { get; set; }
        /// <summary>
        /// 花费
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        public double Expenses { get; set; }
        /// <summary>
        /// 上次花费
        /// </summary>
        /// <value>
        /// The last expenses.
        /// </value>
        public double LastExpenses { get; set; }

        /// <summary>
        /// 科室信息
        /// </summary>
        public DepartPerformanceModel()
        {
            this.Department = new DepartmentInfo();
        }
    }
}