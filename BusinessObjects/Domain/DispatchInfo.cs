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
    /// 派工单基础信息
    /// </summary>
    public class DispatchBaseInfo : EntityInfo
    {
        /// <summary>
        /// 请求信息
        /// </summary>
        /// <value>
        /// 请求信息
        /// </value>
        public RequestInfo Request { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        /// <value>
        /// 请求类型
        /// </value>
        public KeyValueInfo RequestType { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        /// <value>
        /// 紧急程度
        /// </value>
        public KeyValueInfo Urgency { get; set; }
        /// <summary>
        /// 机器状态
        /// </summary>
        /// <value>
        ///  机器状态
        /// </value>
        public KeyValueInfo MachineStatus { get; set; }
        /// <summary>
        /// 工程师信息
        /// </summary>
        /// <value>
        /// 工程师信息
        /// </value>
        public UserInfo Engineer { get; set; }
        /// <summary>
        /// 择期日期
        /// </summary>
        /// <value>
        /// 择期日期
        /// </value>
        public DateTime ScheduleDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <value>
        /// 备注
        /// </value>
        public string LeaderComments { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value>
        /// 状态
        /// </value>
        public KeyValueInfo Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public int RequestID { get { return this.Request == null ? 0 : this.Request.ID; } }
        /// <summary>
        /// 派工单编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Dispatch, this.ID); } }
        /// <summary>
        /// 派工单信息
        /// </summary>
        public DispatchBaseInfo()
        {
            this.Request = new RequestInfo();
            this.RequestType = new KeyValueInfo();
            this.Urgency = new KeyValueInfo();
            this.MachineStatus = new KeyValueInfo();
            this.Engineer = new UserInfo();
            this.Status = new KeyValueInfo();
        }
    }

    /// <summary>
    /// 派工单信息
    /// </summary>
    public class DispatchInfo : DispatchBaseInfo
    {
        /// <summary>
        /// 历史信息
        /// </summary>
        /// <value>
        /// The histories.
        /// </value>
        public List<HistoryInfo> Histories { get; set; }
        /// <summary>
        /// 服务工单信息
        /// </summary>
        /// <value>
        /// The dispatch journal.
        /// </value>
        public DispatchJournalInfo DispatchJournal { get; set; }
        /// <summary>
        /// 作业报告信息
        /// </summary>
        /// <value>
        /// The dispatch report.
        /// </value>
        public DispatchReportInfo DispatchReport { get; set; }
        /// <summary>
        /// 生命周期描述
        /// </summary>
        public string TimelineDesc
        {
            get
            {
                if (this.DispatchReport != null)
                    return string.Format("{0}{1}{2}{3}", this.RequestType.ID != 0 ? this.RequestType.Name : this.RequestType.Name,
                     this.Request.ID != 0 ? "；" + this.Request.OID : "",
                     string.IsNullOrEmpty(this.DispatchReport.SolutionWay) ? "" : "；" + this.DispatchReport.SolutionWay,
                     string.IsNullOrEmpty(this.DispatchReport.Comments) ? "" : "；" + this.DispatchReport.Comments);
                else
                    return "";
            }
        }
        /// <summary>
        /// 派工单信息
        /// </summary>
        public DispatchInfo() 
        {
            this.DispatchJournal = new DispatchJournalInfo();
            this.DispatchReport = new DispatchReportInfo();
        }
        /// <summary>
        /// 获取派工单信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DispatchInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Request.ID = SQLUtil.ConvertInt(dr["RequestID"]);
            this.RequestType.ID = SQLUtil.ConvertInt(dr["RequestType"]);
            this.RequestType.Name = LookupManager.GetRequestTypeDesc(this.RequestType.ID);
            this.Urgency.ID = SQLUtil.ConvertInt(dr["UrgencyID"]);
            this.Urgency.Name = LookupManager.GetUrgencyDesc(this.Urgency.ID);
            this.MachineStatus.ID = SQLUtil.ConvertInt(dr["EquipmentStatus"]);
            this.MachineStatus.Name = MachineStatuses.GetMachineStatusesDesc(this.MachineStatus.ID);
            this.Engineer.ID = SQLUtil.ConvertInt(dr["EngineerID"]);
            this.ScheduleDate = SQLUtil.ConvertDateTime(dr["ScheduleDate"]);
            this.LeaderComments = SQLUtil.TrimNull(dr["LeaderComments"]);
            this.Status.ID = SQLUtil.ConvertInt(dr["StatusID"]);
            this.Status.Name = LookupManager.GetDispatchStatusDesc(this.Status.ID);
            this.CreateDate = SQLUtil.ConvertDateTime(dr["CreateDate"]);
            this.StartDate = SQLUtil.ConvertDateTime(dr["StartDate"]);
            this.EndDate = SQLUtil.ConvertDateTime(dr["EndDate"]);
            if (dr.Table.Columns.Contains("DispatchJournalID"))
                this.DispatchJournal.ID = SQLUtil.ConvertInt(dr["DispatchJournalID"]);
            if (dr.Table.Columns.Contains("DispatchJournalStatusID"))
            {
                this.DispatchJournal.Status.ID = SQLUtil.ConvertInt(dr["DispatchJournalStatusID"]);
                this.DispatchJournal.Status.Name = LookupManager.GetDispatchDocStatusDesc(this.DispatchJournal.Status.ID);
            }
            if (dr.Table.Columns.Contains("DispatchReportID"))
                this.DispatchReport.ID = SQLUtil.ConvertInt(dr["DispatchReportID"]);
            if (dr.Table.Columns.Contains("DispatchReportStatusID"))
            {
                this.DispatchReport.Status.ID = SQLUtil.ConvertInt(dr["DispatchReportStatusID"]);
                this.DispatchReport.Status.Name = LookupManager.GetDispatchDocStatusDesc(this.DispatchReport.Status.ID);
            }
        }
        /// <summary>
        /// Copy4s the application.
        /// </summary>
        /// <returns>派工单信息</returns>
        public DispatchInfo Copy4App()
        {
            this.DispatchJournal = null;
            this.DispatchReport = null;
            this.Histories = null;
            this.FormatHistory = null;
            return this;
        }
        /// <summary>
        /// Copy4s the base.
        /// </summary>
        /// <returns>DispatchBaseInfo</returns>
        public DispatchBaseInfo Copy4Base()
        {
            DispatchBaseInfo baseInfo = new DispatchBaseInfo();
            baseInfo.Request = this.Request;
            baseInfo.RequestType = this.RequestType;
            baseInfo.Urgency = this.Urgency;
            baseInfo.MachineStatus = this.MachineStatus;
            baseInfo.Engineer = this.Engineer;
            baseInfo.ScheduleDate = this.ScheduleDate;
            baseInfo.LeaderComments = this.LeaderComments;
            baseInfo.Status = this.Status;
            baseInfo.CreateDate = this.CreateDate;
            baseInfo.StartDate = this.StartDate;
            baseInfo.EndDate = this.EndDate;
            return baseInfo;
        }
        /// <summary>
        /// 流程信息
        /// </summary>
        /// <value>
        /// The format history.
        /// </value>
        public string FormatHistory { get; set; }
        /// <summary>
        /// 设置派工单流程信息
        /// </summary>
        public void SetHis4Comments()
        {
            StringBuilder sb = new StringBuilder();
            foreach (HistoryInfo history in this.Histories)
            {
                sb.AppendLine(history.VerifyHistory);
            }
            this.FormatHistory = sb.ToString();
        }
        /// <summary>
        /// 派工单状态
        /// </summary>
        public static class Statuses
        {
            /// <summary>
            /// 取消
            /// </summary>
            public const int Cancelled = -1;
            /// <summary>
            /// 新建
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 已响应
            /// </summary>
            public const int Responded = 2;
            /// <summary>
            /// 待审批
            /// </summary>
            public const int Pending = 3;
            /// <summary>
            /// 已审批
            /// </summary>
            public const int Approved = 4;

            /// <summary>
            /// 根据派工单状态获取请求状态
            /// </summary>
            /// <param name="statuses">派工单状态</param>
            /// <returns>请求状态</returns>
            public static int GetRequestStatusByStatuses(int statuses)
            {
                switch (statuses)
                {
                    case New:
                        return RequestInfo.Statuses.Allocated;
                    case Responded:
                        return RequestInfo.Statuses.Responded;
                    case Pending:
                        return RequestInfo.Statuses.Pending;
                    default:
                        return 0;
                }
            }
        }
        /// <summary>
        /// 服务凭证、作业报告状态
        /// </summary>
        public static class DocStatus
        {
            /// <summary>
            /// 新建
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 待审批
            /// </summary>
            public const int Pending = 2;
            /// <summary>
            /// 已审批
            /// </summary>
            public const int Approved = 3;
            /// <summary>
            /// 取消
            /// </summary>
            public const int Cancelled = 99;
        }
        /// <summary>
        /// 流程操作状态
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// 新增
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 响应
            /// </summary>
            public const int Response = 2;
            /// <summary>
            /// 完成
            /// </summary>
            public const int Finish = 3;
            /// <summary>
            /// 终止
            /// </summary>
            public const int Cancelled = 4;

            /// <summary>
            /// 根据流程信息操作编号获取流程信息操作描述
            /// </summary>
            /// <param name="actionID">流程信息操作编号</param>
            /// <returns>流程信息操作描述</returns>
            public static string GetDesc(int actionID)
            {
                switch (actionID)
                {
                    case New:
                        return "新增";
                    case Response:
                        return "响应";
                    case Finish:
                        return "完成";
                    case Cancelled:
                        return "终止";
                    default:
                        return "";
                }
            }
        }
    }
}
