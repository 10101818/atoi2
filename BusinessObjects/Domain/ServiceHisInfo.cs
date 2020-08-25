using BusinessObjects.Domain;
using BusinessObjects.Manager;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 医院信息
    /// </summary>
    public class ServiceHisCountInfo
    {
        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        /// <value>
        /// The object identifier.
        /// </value>
        public int ObjectID { get; set; }
        /// <summary>
        /// 服务数量
        /// </summary>
        /// <value>
        /// The service count.
        /// </value>
        public int ServiceCount { get; set; }
        /// <summary>
        /// 收入
        /// </summary>
        /// <value>
        /// The incomes.
        /// </value>
        public double Incomes { get; set; }
        /// <summary>
        /// 支出
        /// </summary>
        /// <value>
        /// The expenses.
        /// </value>
        public double Expenses { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHisCountInfo"/> class.
        /// </summary>
        public ServiceHisCountInfo()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHisCountInfo"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public ServiceHisCountInfo(DataRow dr)
            :this()
        {
            this.ObjectID = SQLUtil.ConvertInt(dr["ObjectID"]);
            if (dr.Table.Columns.Contains("ServiceCount"))
                this.ServiceCount = SQLUtil.ConvertInt(dr["ServiceCount"]);
            if (dr.Table.Columns.Contains("Incomes"))
                this.Incomes = SQLUtil.ConvertDouble(dr["Incomes"]);
            if (dr.Table.Columns.Contains("Expenses"))
                this.Expenses = SQLUtil.ConvertDouble(dr["Expenses"]);
        }
    }
}