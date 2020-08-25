using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 故障率Info
    /// </summary>
    public class FaultRateInfo
    {
        /// <summary>
        /// 故障对象类型
        /// </summary>
        public ObjectType ObjectTypeID { get; set; }
        /// <summary>
        /// 故障对象id
        /// </summary>
        public int ObjectID { get; set; }
        /// <summary>
        /// 第几年
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// 第几月
        /// </summary>
        public DateTimeMonth Month { get; set; }
        /// <summary>
        /// 故障率
        /// </summary>
        public double Rate { get; set; }
        /// <summary>
        /// 故障率Info
        /// </summary>
        public FaultRateInfo() { 

        }
        /// <summary>
        /// 故障率Info
        /// </summary>
        /// <param name="dr">dr</param>
        public FaultRateInfo(DataRow dr)
            :this()
        {
            this.ObjectTypeID = SQLUtil.ConvertEnum<ObjectType>(dr["ObjectTypeID"]);
            this.ObjectID = SQLUtil.ConvertInt(dr["ObjectID"]); 
            this.Year = SQLUtil.ConvertInt(dr["Year"]);
            this.Month = SQLUtil.ConvertEnum<DateTimeMonth>(dr["Month"]);
            this.Rate = SQLUtil.ConvertDouble(dr["Rate"]);
        }
        /// <summary>
        /// 获取初始的故障率信息
        /// </summary>
        /// <param name="objectID">故障对象id</param>
        /// <param name="type">故障对象类型</param>
        /// <returns>故障率信息</returns>
        public static List<FaultRateInfo> GetInitList(int objectID, ObjectType type)
        {
            List<FaultRateInfo> infos = new List<FaultRateInfo>(); 
            foreach(FaultRateWholeSection year in Enum.GetValues(typeof(FaultRateWholeSection)))
            { 
                foreach(DateTimeMonth month in Enum.GetValues(typeof(DateTimeMonth)))
                {
                    FaultRateInfo info = new FaultRateInfo();
                    info.ObjectID = objectID;
                    info.ObjectTypeID = type;
                    info.Year = (int)year;
                    info.Month = month;
                    info.Rate = 0d;
                    infos.Add(info); 
                }
            }
            return infos;
        }
        /// <summary>
        /// 故障率修改日志信息
        /// </summary>
        /// <param name="oldDatas">修改前数据</param>
        /// <param name="newDatas">修改后数据</param>
        /// <returns>日志信息</returns>
        public static List<AuditDetailInfo> ConvertAuditDetail( List<FaultRateInfo> oldDatas, List<FaultRateInfo> newDatas)
        {
            List<AuditDetailInfo> infos = new List<AuditDetailInfo> (); 
            int len = oldDatas.Count;
            for (int i = 0;i< len; i++)
            {
                FaultRateInfo oldInfo = oldDatas[i], newInfo = newDatas[i];
                if (oldInfo.Rate != newInfo.Rate)
                    infos.Add(new AuditDetailInfo() { FieldName = "FaultRateRate-" + oldInfo.Year + "-" + (int)oldInfo.Month, OldValue = oldInfo.Rate.ToString(), NewValue = newInfo.Rate.ToString() });
            }
            return infos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditID"></param>
        /// <param name="oldDatas"></param>
        /// <param name="newDatas"></param>
        /// <returns></returns>
        public static DataTable ConvertDataTable(int auditID, List<FaultRateInfo> oldDatas, List<FaultRateInfo> newDatas)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AuditID");
            dt.Columns.Add("FieldName");
            dt.Columns.Add("OldValue");
            dt.Columns.Add("NewValue");
            int len = oldDatas.Count;
            for (int i = 0; i < len; i++)
            {
                FaultRateInfo oldInfo = oldDatas[i], newInfo = newDatas[i];
                if (oldInfo.Rate != newInfo.Rate)
                    //infos.Add(new AuditDetailInfo() { AuditID = auditID, FieldName = "FaultRateRate-" + oldInfo.Year + "-" + oldInfo.Month, OldValue = oldInfo.Rate.ToString(), NewValue = newInfo.Rate.ToString() });
                    dt.Rows.Add(auditID, "FaultRateRate-"+ oldInfo.Year+"-"+oldInfo.Month, oldInfo.Rate, newInfo.Rate);
            }
            return dt;
        }

        /// <summary>
        /// 故障率dt
        /// </summary>
        /// <param name="infos">故障率信息</param>
        /// <returns>故障率dt</returns>
        public static DataTable ConvertFaultRateDataTable(List<FaultRateInfo> infos)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ObjectTypeID", typeof(System.Int32));
            dt.Columns.Add("ObjectID", typeof(System.Int32));
            dt.Columns.Add("Year", typeof(System.Int32));
            dt.Columns.Add("Month", typeof(System.Int32));
            dt.Columns.Add("Rate", typeof(System.Double));

            infos.ForEach(faultRate => {
                DataRow dr = dt.NewRow();
                dr["ObjectTypeID"] = faultRate.ObjectTypeID;
                dr["ObjectID"] = faultRate.ObjectID;
                dr["Year"] = faultRate.Year;
                dr["Month"] = faultRate.Month;
                dr["Rate"] = faultRate.Rate;

                dt.Rows.Add(dr);
            });
            return dt;
        }

        /// <summary>
        /// 故障对象类型
        /// </summary>
        public enum ObjectType
        {
            /// <summary>
            /// 维保额外
            /// </summary>
            [Description("维保额外")]
            Repair = 1,
            /// <summary>
            /// 零件
            /// </summary>
            [Description("零件")]
            Component,
        }

        /// <summary>
        /// 故障月份
        /// </summary>
        public enum DateTimeMonth
        {
            /// <summary>
            /// 1月
            /// </summary>
            [Description("1月")]
            Jan = 1,   // January  , //
            /// <summary>
            /// 2月
            /// </summary>
            [Description("2月")]
            Feb,   // February    , //
            /// <summary>
            /// 3月
            /// </summary>
            [Description("3月")]
            Mar,   // March       , //
            /// <summary>
            /// 4月
            /// </summary>
            [Description("4月")]
            Apr,   // April       , //
            /// <summary>
            /// 5月
            /// </summary>
            [Description("5月")]
            May,   // May         , //
            /// <summary>
            /// 6月
            /// </summary>
            [Description("6月")]
            Jun,   // June        , //
            /// <summary>
            /// 7月
            /// </summary>
            [Description("7月")]
            Jul,   // July        , //
            /// <summary>
            /// 8月
            /// </summary>
            [Description("8月")]
            Aug,   // August      , //
            /// <summary>
            /// 9月
            /// </summary>
            [Description("9月")]
            Sep,   // September   , //
            /// <summary>
            /// 10月
            /// </summary>
            [Description("10月")]
            Oct,   // October     , //
            /// <summary>
            /// 11月
            /// </summary>
            [Description("11月")]
            Nov,   // November    , //
            /// <summary>
            /// 12月
            /// </summary>
            [Description("12月")]
            Dec    // December      //
        }
        /// <summary>
        /// 故障年份
        /// </summary>
        public enum FaultRateShortSection
        {
            /// <summary>
            /// 1年
            /// </summary>
            [Description("1年")]
            One = 1,
            /// <summary>
            /// 2年
            /// </summary>
            [Description("2年")]
            Two,
            /// <summary>
            /// 3年
            /// </summary>
            [Description("3年")]
            Three,
            /// <summary>
            /// 4年
            /// </summary>
            [Description("4年")]
            Four,
            /// <summary>
            /// 5-9年
            /// </summary>
            [Description("5-9年")]
            FiveToNine,
            /// <summary>
            /// 10年及以上
            /// </summary>
            [Description("10年及以上")]
            TenAndAbove = 10
        }
        /// <summary>
        /// 故障年份
        /// </summary>
        public enum FaultRateSubSection
        {
            /// <summary>
            /// 5年
            /// </summary>
            [Description("5年")]
            Five = 5,
            /// <summary>
            /// 6年
            /// </summary>
            [Description("6年")]
            Six,
            /// <summary>
            /// 7年
            /// </summary>
            [Description("7年")]
            Seven,
            /// <summary>
            /// 8年
            /// </summary>
            [Description("8年")]
            Eight,
            /// <summary>
            /// 9年
            /// </summary>
            [Description("9年")]
            Nine
        }
        /// <summary>
        /// 故障年份
        /// </summary>
        public enum FaultRateWholeSection
        {
            /// <summary>
            /// 1年
            /// </summary>
            [Description("1年")]
            One = 1,
            /// <summary>
            /// 2年
            /// </summary>
            [Description("2年")]
            Two,
            /// <summary>
            /// 3年
            /// </summary>
            [Description("3年")]
            Three,
            /// <summary>
            /// 4年
            /// </summary>
            [Description("4年")]
            Four,
            /// <summary>
            /// 5年
            /// </summary>
            [Description("5年")]
            Five,
            /// <summary>
            /// 6年
            /// </summary>
            [Description("6年")]
            Six,
            /// <summary>
            /// 7年
            /// </summary>
            [Description("7年")]
            Seven,
            /// <summary>
            /// 8年
            /// </summary>
            [Description("8年")]
            Eight,
            /// <summary>
            /// 9年
            /// </summary>
            [Description("9年")]
            Nine,
            /// <summary>
            /// 10年
            /// </summary>
            [Description("10年")]
            TenAndAbove
        }
    }
}
