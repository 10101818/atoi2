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
    /// 服务凭证信息
    /// </summary>
    public class DispatchJournalInfo:EntityInfo
    {
        /// <summary>
        /// 派工单信息
        /// </summary>
        /// <value>
        /// The dispatch.
        /// </value>
        public DispatchBaseInfo Dispatch { get; set; }
        /// <summary>
        /// 故障现象/错误代码/事由
        /// </summary>
        /// <value>
        /// The fault code.
        /// </value>
        public string FaultCode { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        /// <value>
        /// The content of the job.
        /// </value>
        public string JobContent { get; set; }
        /// <summary>
        /// 服务结果
        /// </summary>
        /// <value>
        /// The result status.
        /// </value>
        public KeyValueInfo ResultStatus { get; set; }
        /// <summary>
        /// 待跟进问题
        /// </summary>
        /// <value>
        /// The follow problem.
        /// </value>
        public string FollowProblem { get; set; }
        /// <summary>
        /// 建议留言
        /// </summary>
        /// <value>
        /// The advice.
        /// </value>
        public string Advice { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// 用户电话
        /// </summary>
        /// <value>
        /// The user mobile.
        /// </value>
        public string UserMobile { get; set; }
        /// <summary>
        /// 是否签名
        /// </summary>
        /// <value>
        ///   <c>true</c> if signed; otherwise, <c>false</c>.
        /// </value>
        public bool Signed { get; set; }
        /// <summary>
        /// 文件内容
        /// </summary>
        /// <value>
        /// The content of the file.
        /// </value>
        public string FileContent { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        /// <value>
        /// The fuji comments.
        /// </value>
        public string FujiComments { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public KeyValueInfo Status { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public int RequestID { get { return this.Dispatch.RequestID; } }
        /// <summary>
        /// 历史信息
        /// </summary>
        /// <value>
        /// The histories.
        /// </value>
        public List<HistoryInfo> Histories { get; set; }
        /// <summary>
        /// 服务凭证编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.DispatchJournal, this.ID); } }
        /// <summary>
        /// 签名文件名称
        /// </summary>
        /// <value>
        /// The name of the signature file.
        /// </value>
        public string SignatureFileName { get { return string.Format("{0}.png", this.ID); } }
        /// <summary>
        /// 服务凭证信息
        /// </summary>
        public DispatchJournalInfo()
        {
            this.Dispatch = new DispatchBaseInfo();
            this.Status = new KeyValueInfo();
            this.ResultStatus = new KeyValueInfo();
        }
        /// <summary>
        /// 获取服务凭证信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public DispatchJournalInfo(DataRow dr)
            :this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Dispatch.ID = SQLUtil.ConvertInt(dr["DispatchID"]);
            this.FaultCode = SQLUtil.TrimNull(dr["FaultCode"]);
            this.JobContent = SQLUtil.TrimNull(dr["JobContent"]);
            this.FollowProblem = SQLUtil.TrimNull(dr["FollowProblem"]); 
            this.Advice = SQLUtil.TrimNull(dr["Advice"]);
            this.UserName = SQLUtil.TrimNull(dr["UserName"]);
            this.UserMobile = SQLUtil.TrimNull(dr["UserMobile"]);
            this.Signed = SQLUtil.ConvertBoolean(dr["Signed"]);
            this.Status.ID = SQLUtil.ConvertInt(dr["StatusID"]);
            this.Status.Name = LookupManager.GetDispatchDocStatusDesc(this.Status.ID);
            this.ResultStatus.ID = SQLUtil.ConvertInt(dr["ResultStatusID"]);
            this.ResultStatus.Name = LookupManager.GetDispatchJournalResultStatusDesc(this.ResultStatus.ID);
            this.FujiComments = SQLUtil.TrimNull(dr["FujiComments"]);
        }
        /// <summary>
        /// 流程信息
        /// </summary>
        /// <value>
        /// The format history.
        /// </value>
        public string FormatHistory { get; set; }
        /// <summary>
        /// 设置流程信息
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
        /// 服务凭证状态
        /// </summary>
        public static class DispatchJournalStatus
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
            /// 终止
            /// </summary>
            public const int Cancelled = 99;
        }

        /// <summary>
        /// 服务凭证流程操作
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// 提交
            /// </summary>
            public const int Submit = 1;
            /// <summary>
            /// 通过
            /// </summary>
            public const int Pass = 2;
            /// <summary>
            /// 退回
            /// </summary>
            public const int Reject = 3;

            /// <summary>
            /// 根据服务凭证流程操作编号获取服务凭证流程操作描述信息
            /// </summary>
            /// <param name="actionID">服务凭证流程操作编号</param>
            /// <returns>服务凭证流程操作描述</returns>
            public static string GetDesc(int actionID)
            {
                switch (actionID)
                {
                    case Submit:
                        return "提交";
                    case Pass:
                        return "通过";
                    case Reject:
                        return "退回";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 服务凭证结果
        /// </summary>
        public static class ResultStatuses
        {
            /// <summary>
            /// 待跟进
            /// </summary>
            public const int Pending = 1;
            /// <summary>
            /// 已完成
            /// </summary>
            public const int Close = 2;
        }
    }
}
