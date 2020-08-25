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
    /// 请求信息
    /// </summary>
    public class RequestInfo : EntityInfo
    {
        /// <summary>
        /// 请求来源
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public KeyValueInfo Source { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public KeyValueInfo RequestType { get; set; }
        /// <summary>
        /// 请求人
        /// </summary>
        /// <value>
        /// The request user.
        /// </value>
        public UserInfo RequestUser { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public String Subject { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        /// <value>
        /// The fault desc.
        /// </value>
        public String FaultDesc { get; set; }
        /// <summary>
        /// 机器状态
        /// </summary>
        /// <value>
        /// The machine status.
        /// </value>
        public KeyValueInfo MachineStatus { get; set; }
        /// <summary>
        /// 故障分类
        /// </summary>
        /// <value>
        /// The type of the fault.
        /// </value>
        public KeyValueInfo FaultType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public KeyValueInfo Status { get; set; }
        /// <summary>
        /// 处理方式
        /// </summary>
        /// <value>
        /// The type of the deal.
        /// </value>
        public KeyValueInfo DealType { get; set; }
        /// <summary>
        /// 紧急程度
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public KeyValueInfo Priority { get; set; }
        /// <summary>
        /// 请求日期
        /// </summary>
        /// <value>
        /// The request date.
        /// </value>
        public DateTime RequestDate { get; set; }
        /// <summary>
        /// 首次分配时间
        /// </summary>
        /// <value>
        /// The distribute date.
        /// </value>
        public DateTime DistributeDate { get; set; }
        /// <summary>
        /// 首次派工时间
        /// </summary>
        /// <value>
        /// The dispatch date.
        /// </value>
        public DateTime DispatchDate { get; set; }
        /// <summary>
        /// 首次响应时间
        /// </summary>
        /// <value>
        /// The response date.
        /// </value>
        public DateTime ResponseDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        /// <value>
        /// The close date.
        /// </value>
        public DateTime CloseDate { get; set; }
        /// <summary>
        /// 派工前状态
        /// </summary>
        /// <value>
        /// The last status.
        /// </value>
        public KeyValueInfo LastStatus { get; set; }
        /// <summary>
        /// 是否召回
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is recall; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsRecall { get; set; }
        /// <summary>
        /// 择期日期
        /// </summary>
        /// <value>
        /// The selective date.
        /// </value>
        public DateTime SelectiveDate { get; set; }
        /// <summary>
        /// 流程信息
        /// </summary>
        /// <value>
        /// The histories.
        /// </value>
        public List<HistoryInfo> Histories { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        /// <value>
        /// The equipments.
        /// </value>
        public List<EquipmentInfo> Equipments { get; set; }
        /// <summary>
        ///附件信息
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<UploadFileInfo> Files { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value>
        public string SourceType { get { return this.Source.Name + "-" + this.RequestType.Name; } }
        /// <summary>
        /// 主题.
        /// </summary>
        /// <value>
        /// The name of the subject.
        /// </value>
        public string SubjectName { get { return this.RequestType.ID == RequestTypes.Others ? this.RequestType.Name : this.EquipmentName + "-" + this.RequestType.Name; } }
        /// <summary>
        /// 是否误工
        /// </summary>
        public bool IsDelay
        {
            get
            {
                if (this.RequestType.ID != RequestTypes.Repair || this.Equipments == null || this.Equipments.Count == 0 || this.Equipments[0] == null)
                {
                    return false;
                }
                else
                {
                    double delayMinuts = this.ResponseDate.Subtract(this.DistributeDate).TotalMinutes;
                    if (delayMinuts > this.Equipments[0].ResponseTimeLength && this.LastStatus.ID == RequestInfo.Statuses.New)
                        return true;
                    else
                        return false;
                }
            }
        }

        #region" Dispaly Only-Equipment、Department "

        private EquipmentInfo Equipment { get { if (this.Equipments == null || this.Equipments.Count != 1) return null; else return this.Equipments[0]; } }
        /// <summary>
        /// 设备编号
        /// </summary>
        /// <value>
        /// The equipment oid.
        /// </value>
        public string EquipmentOID
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return "多设备";
                else if(this.Equipment != null)
                    return this.Equipment.OID;
                else
                    return "";
            }
        }
        /// <summary>
        /// 设备编号
        /// </summary>
        /// <value>
        /// The equipment id.
        /// </value>
        public int EquipmentID
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count == 1 && this.Equipments[0] != null)
                    return this.Equipment.ID;
                else
                    return 0;
            }
        }
        /// <summary>
        /// 设备名称
        /// </summary>
        /// <value>
        /// The name of the equipment.
        /// </value>
        public string EquipmentName
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return "多设备";
                else if (this.Equipment != null)
                    return this.Equipment.Name;
                else
                    return "";
            }
        }
        /// <summary>
        /// 科室编号
        /// </summary>
        /// <value>
        /// The department identifier.
        /// </value>
        public int DepartmentID
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return this.Equipments[0].Department.ID;
                else if (this.Equipment != null)
                    return this.Equipment.Department.ID;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 科室名称
        /// </summary>
        /// <value>
        /// The name of the department.
        /// </value>
        public string DepartmentName
        {
            get
            {
                if (this.Equipments != null && this.Equipments.Count > 1)
                    return this.Equipments[0].Department.Name;
                else if (this.Equipment != null)
                    return this.Equipment.Department.Name;
                else
                    return "";
            }
        }

        #endregion        
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Request, this.ID); } }

        /// <summary>
        /// 是否有作业中的派工单
        /// </summary>
        //Display only
        public bool HasOpenDispatch { get; set; }
        /// <summary>
        /// 请求信息
        /// </summary>
        public RequestInfo()
        {
            this.Source = new KeyValueInfo();
            this.RequestType = new KeyValueInfo();
            this.RequestUser = new UserInfo();
            this.MachineStatus = new KeyValueInfo();
            this.FaultType = new KeyValueInfo();
            this.Status = new KeyValueInfo();
            this.DealType = new KeyValueInfo();
            this.Priority = new KeyValueInfo();
            this.LastStatus = new KeyValueInfo();
        }
        /// <summary>
        /// 获取请求信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public RequestInfo(DataRow dr)
            :this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Source.ID = SQLUtil.ConvertInt(dr["Source"]);
            this.Source.Name = Sources.GetSourceDesc(this.Source.ID);
            this.RequestType.ID = SQLUtil.ConvertInt(dr["RequestType"]);
            this.RequestType.Name = Manager.LookupManager.GetRequestTypeDesc(this.RequestType.ID);
            this.RequestUser.ID = SQLUtil.ConvertInt(dr["RequestUserID"]);
            this.RequestUser.Name = SQLUtil.TrimNull(dr["RequestUserName"]);
            this.RequestUser.Mobile = SQLUtil.TrimNull(dr["RequestUserMobile"]);
            if (dr.Table.Columns.Contains("RequestUserRoleID"))
                this.RequestUser.Role.Name = Manager.LookupManager.GetRoleDesc(SQLUtil.ConvertInt(dr["RequestUserRoleID"]));
            this.Subject = SQLUtil.TrimNull(dr["Subject"]); 
            this.FaultDesc = SQLUtil.TrimNull(dr["FaultDesc"]);
            this.MachineStatus.ID = SQLUtil.ConvertInt(dr["EquipmentStatus"]);
            this.MachineStatus.Name = MachineStatuses.GetMachineStatusesDesc(this.MachineStatus.ID);
            this.FaultType.ID = SQLUtil.ConvertInt(dr["FaultTypeID"]);
            this.FaultType.Name = RequestTypes.GetFaultTypeDesc(this.RequestType.ID, this.FaultType.ID);
            this.Status.ID = SQLUtil.ConvertInt(dr["StatusID"]);
            this.Status.Name = Manager.LookupManager.GetRequestStatusDesc(this.Status.ID);
            this.DealType.ID = SQLUtil.ConvertInt(dr["DealTypeID"]);
            this.DealType.Name = Manager.LookupManager.GetDealTypeDesc(this.DealType.ID);
            this.Priority.ID = SQLUtil.ConvertInt(dr["PriorityID"]);
            this.Priority.Name = Manager.LookupManager.GetUrgencyDesc(this.Priority.ID);
            this.RequestDate = SQLUtil.ConvertDateTime(dr["RequestDate"]);
            this.DistributeDate = SQLUtil.ConvertDateTime(dr["DistributeDate"]);
            this.DispatchDate = SQLUtil.ConvertDateTime(dr["DispatchDate"]);
            this.ResponseDate = SQLUtil.ConvertDateTime(dr["ResponseDate"]);
            this.CloseDate = SQLUtil.ConvertDateTime(dr["CloseDate"]);
            this.LastStatus.ID = SQLUtil.ConvertInt(dr["LastStatusID"]);
            this.LastStatus.Name = Manager.LookupManager.GetRequestStatusDesc(this.LastStatus.ID);
            this.IsRecall = SQLUtil.ConvertBoolean(dr["IsRecall"]);
            this.SelectiveDate = SQLUtil.ConvertDateTime(dr["SelectiveDate"]);
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
        /// 请求附件类型
        /// </summary>
        public static class FileTypes
        {
            /// <summary>
            /// 请求类型的附件
            /// </summary>
            public const int RequestFile = 1;

            /// <summary>
            /// 获取附件类型描述
            /// </summary>
            /// <param name="id">附件类型id</param>
            /// <returns>附件类型描述</returns>
            public static string GetFileName(int id)
            {
                switch (id)
                {
                    case RequestFile:
                        return "附件";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 请求状态
        /// </summary>
        public static class Statuses
        {
            /// <summary>
            /// 终止
            /// </summary>
            public const int Cancelled = -1;
            /// <summary>
            /// 新建
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 已分配
            /// </summary>
            public const int Allocated = 2;
            /// <summary>
            /// 已响应
            /// </summary>
            public const int Responded = 3;
            /// <summary>
            /// 待审批
            /// </summary>
            public const int Pending = 4;
            /// <summary>
            /// 待分配
            /// </summary>
            public const int Allocating = 5;
            /// <summary>
            /// 问题升级
            /// </summary>
            public const int ProblemEscalation = 6;
            /// <summary>
            /// 待第三方支持
            /// </summary>
            public const int WaittingThirdParty = 7;
            /// <summary>
            /// 关闭
            /// </summary>
            public const int Close = 99;
            /// <summary>
            /// 未完成
            /// </summary>
            //Display-Only
            public const int Unfinished = 98;
            /// <summary>
            /// 超期
            /// </summary>
            public const int OverDue = 1830;

            /// <summary>
            /// 获取超期搜索条件sql
            /// </summary>
            /// <returns>超期搜索条件sql</returns>
            public static string GetCurOverDueSQL()
            {
                return string.Format(" r.StatusID <> {0} AND r.StatusID <> {1} AND r.Source <> {2} AND r.RequestType = {3} AND" +
                                     " (( DATEDIFF(DAY,r.RequestDate, GETDATE()) >= {4}) " +
                                     " OR (r.StatusID={5} AND DATEDIFF(MINUTE,r.RequestDate,GETDATE())>e.ResponseTimeLength)) ", 
                                         RequestInfo.Statuses.Close,
                                         RequestInfo.Statuses.Cancelled,
                                         RequestInfo.Sources.SysRequest,
                                         RequestInfo.RequestTypes.Repair,                                         
                                         ControlManager.GetSettingInfo().OverDueTime,
                                         RequestInfo.Statuses.New
                                     );
            }

            /// <summary>
            /// 超期请求排序sql
            /// </summary>
            /// <returns>超期请求排序sql</returns>
            public static string GetCurOverDueField()
            {
                return string.Format("(Case When {0} Then 1 Else 0 End)", GetCurOverDueSQL());
            }
            /// <summary>
            /// 报表超期条件sql
            /// </summary>
            /// <returns>报表超期条件sql</returns>
            public static string GetHisOverDueSQL()
            {
                return string.Format(" r.StatusID <> {0} AND r.Source <> {1} AND r.RequestType = {2} AND" +
                                     " (DATEDIFF(DAY,r.RequestDate, ISNULL(r.CloseDate, GETDATE())) >= {3} OR DATEDIFF(MINUTE,r.RequestDate, ISNULL(r.DispatchDate, GETDATE())) > e.ResponseTimeLength) ",
                                         RequestInfo.Statuses.Cancelled,
                                         RequestInfo.Sources.SysRequest,
                                         RequestInfo.RequestTypes.Repair,
                                         ControlManager.GetSettingInfo().OverDueTime
                                     );
            }
        }
        /// <summary>
        /// (报表用)请求状态
        /// </summary>
        public static class ReportStatus
        {
            /// <summary>
            /// 未关闭
            /// </summary>
            public const int Unclosed = 1;
            /// <summary>
            /// 未响应
            /// </summary>
            public const int Unresponsed = 2;
            /// <summary>
            /// 关闭
            /// </summary>
            public const int Closed = 3;
            /// <summary>
            /// 已响应
            /// </summary>
            public const int Responsed = 4;
        }
        /// <summary>
        /// 请求类型
        /// </summary>
        public static class RequestTypes
        {
            /// <summary>
            /// 维修
            /// </summary>
            public const int Repair = 1;
            /// <summary>
            /// 保养
            /// </summary>
            public const int Maintain = 2;
            /// <summary>
            /// 强检
            /// </summary>
            public const int Inspection = 3;
            /// <summary>
            /// 巡检
            /// </summary>
            public const int OnSiteInspection = 4;
            /// <summary>
            /// 校准
            /// </summary>
            public const int Correcting = 5;
            /// <summary>
            /// 设备新增
            /// </summary>
            public const int AddEquipment = 6;
            /// <summary>
            /// 不良事件
            /// </summary>
            public const int AdverseEvent = 7;
            /// <summary>
            /// 合同档案
            /// </summary>
            public const int ContractArchives = 8;
            /// <summary>
            /// 验收安装
            /// </summary>
            public const int Accetance = 9;
            /// <summary>
            /// 调拨
            /// </summary>
            public const int Allocation = 10;
            /// <summary>
            /// 借用
            /// </summary>
            public const int Borrow = 11;
            /// <summary>
            /// 盘点
            /// </summary>
            public const int Inventory = 12;
            /// <summary>
            /// 报废
            /// </summary>
            public const int Scrap = 13;
            /// <summary>
            /// 其它服务
            /// </summary>
            public const int Others = 14;
            /// <summary>
            /// 待召回
            /// </summary>
            public const int Recall = -1;

            /// <summary>
            /// 根据请求类型、故障分类编号获取故障分类字段描述
            /// </summary>
            /// <param name="requestType">请求类型</param>
            /// <param name="faultType">故障分类编号</param>
            /// <returns>故障分类字段描述</returns>
            public static string GetFaultTypeDesc(int requestType,int faultType)
            {
                switch (requestType)
                {
                    case Repair:
                        return MachineStatuses.GetMachineStatusesDesc(faultType);
                    case Maintain:
                        return MaintainType.GetMaintainTypeDesc(faultType);
                    case Inspection:
                        return InspectionType.GetInspectionDesc(faultType);
                    case AdverseEvent:
                        return AdverseEventType.GetAdverseEventDesc(faultType);
                    default:
                        return "";
                }
            }
            /// <summary>
            /// 根据请求类型获取字段描述名称
            /// </summary>
            /// <param name="requestType">请求类型</param>
            /// <returns>字段名称</returns>
            public static string GetRequestDescTdHead(int requestType)
            {
                switch(requestType)
                {
                    case Repair:
                        return "故障描述";
                    case Maintain:
                        return "保养要求";
                    case Inspection:
                        return "强检要求";
                    case OnSiteInspection:
                        return "巡检要求";
                    case Correcting:
                        return "校准要求";
                    case AdverseEvent:
                        return "不良事件描述";
                    case ContractArchives:
                        return "合同档案备注";
                    case Accetance:
                        return "验收安装备注";
                    case Allocation:
                        return "调拨备注";
                    case Borrow:
                        return "借用备注";
                    case Inventory:
                        return "盘点备注";
                    case Scrap:
                        return "报废备注";
                    case AddEquipment:
                    case Others:
                        return "备注";
                    case Recall:
                        return "召回备注";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 请求紧急程度
        /// </summary>
        public static class UrgencyLevel 
        {
            /// <summary>
            /// 普通
            /// </summary>
            public const int Normal = 1;
            /// <summary>
            /// 紧急
            /// </summary>
            public const int Urgency = 2;
        }
        /// <summary>
        /// 请求来源
        /// </summary>
        public static class Sources
        {
            /// <summary>
            /// 客户请求
            /// </summary>
            public const int CustomerRequest = 1;
            /// <summary>
            /// 计划服务
            /// </summary>
            public const int SysRequest = 2;
            /// <summary>
            /// 服务请求
            /// </summary>
            public const int ManualRequest = 3;
            /// <summary>
            /// 获取请求来源信息
            /// </summary>
            /// <returns>请求来源信息</returns>
            public static List<KeyValueInfo> GetSource()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = Sources.CustomerRequest, Name = GetSourceDesc(Sources.CustomerRequest) });
                statuses.Add(new KeyValueInfo() { ID = Sources.SysRequest, Name = GetSourceDesc(Sources.SysRequest) });
                statuses.Add(new KeyValueInfo() { ID = Sources.ManualRequest, Name = GetSourceDesc(Sources.ManualRequest) });
                return statuses;
            }
            /// <summary>
            /// 根据请求来源编号获取请求来源描述
            /// </summary>
            /// <param name="statusId">来源编号</param>
            /// <returns>请求来源描述</returns>
            public static string GetSourceDesc(int statusId)
            {
                switch (statusId)
                {
                    case Sources.CustomerRequest:
                        return "客户请求";
                    case Sources.SysRequest:
                        return "计划服务";
                    case Sources.ManualRequest:
                        return "服务请求";
                    default:
                        return "";

                }
             }
         }
        /// <summary>
        /// 请求紧急程度
        /// </summary>
        public static class Prioritys 
        {
            /// <summary>
            /// 低
            /// </summary>
            public const int Low = 3;
            /// <summary>
            /// 中
            /// </summary>
            public const int Middle = 2;
            /// <summary>
            /// 高
            /// </summary>
            public const int Height = 1;
            /// <summary>
            /// 获取请求紧急程度信息
            /// </summary>
            /// <returns>请求紧急程度信息</returns>
            public static List<KeyValueInfo> GetPrioritys()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = Prioritys.Low, Name = GetPriorityDesc(Prioritys.Low) });
                statuses.Add(new KeyValueInfo() { ID = Prioritys.Middle, Name = GetPriorityDesc(Prioritys.Middle) });
                statuses.Add(new KeyValueInfo() { ID = Prioritys.Height, Name = GetPriorityDesc(Prioritys.Height) });
                return statuses;
            }
            /// <summary>
            /// 根据请求紧急程度编号获取紧急程度描述
            /// </summary>
            /// <param name="statusId">请求紧急程度编号</param>
            /// <returns>紧急程度描述</returns>
            public static  string GetPriorityDesc(int statusId)
            {
                switch (statusId)
                {
                    case Prioritys.Low:
                        return "低";
                    case Prioritys.Middle:
                        return "中";
                    case Prioritys.Height:
                        return "高";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 请求流程操作
        /// </summary>
        public static class Actions
        {
            /// <summary>
            /// 新增
            /// </summary>
            public const int New = 1;
            /// <summary>
            /// 派工
            /// </summary>
            public const int Allocate = 2;
            /// <summary>
            /// 更新状态
            /// </summary>
            public const int Update = 3;
            /// <summary>
            /// 终止
            /// </summary>
            public const int Cancelled = 4;

            /// <summary>
            /// 根据流程信息动作编号获取动作名称
            /// </summary>
            /// <param name="actionID">动作编号</param>
            /// <returns>动作名称</returns>
            public static string GetDesc(int actionID)
            {
                switch (actionID)
                {
                    case New:
                        return "新增";
                    case Allocate:
                        return "派工";
                    case Update:
                        return "更新状态";
                    case Cancelled:
                        return "终止";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 故障分类
        /// </summary>
        public static class FaultTypes
        {
            /// <summary>
            /// 未知
            /// </summary>
            public const int Others = 1;
            /// <summary>
            /// 获取故障分类信息
            /// </summary>
            /// <returns>故障分类信息</returns>
            public static List<KeyValueInfo> GetFaultType()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = FaultTypes.Others, Name = GetFaultTypeDesc(FaultTypes.Others) });
                return statuses;
            }

            /// <summary>
            /// 根据故障分类编号获取故障分类描述
            /// </summary>
            /// <param name="typeID">故障分类编号</param>
            /// <returns>故障分类描述</returns>
            public static string GetFaultTypeDesc(int typeID)
            {
                switch (typeID)
                {
                    case Others:
                        return "未知";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 保养类型
        /// </summary>
        public static class MaintainType
        {
            /// <summary>
            /// 原厂保养
            /// </summary>
            public const int OriginalMaintain = 1;
            /// <summary>
            /// 第三方保养
            /// </summary>
            public const int ThirdMaintain = 2;
            /// <summary>
            /// FMTS保养
            /// </summary>
            public const int FMTSMaintain = 3;

            /// <summary>
            /// 获取保养类型信息
            /// </summary>
            /// <returns>保养类型信息</returns>
            public static List<KeyValueInfo> GetMaintainType()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = MaintainType.OriginalMaintain, Name = GetMaintainTypeDesc(MaintainType.OriginalMaintain) });
                statuses.Add(new KeyValueInfo() { ID = MaintainType.ThirdMaintain, Name = GetMaintainTypeDesc(MaintainType.ThirdMaintain) });
                statuses.Add(new KeyValueInfo() { ID = MaintainType.FMTSMaintain, Name = GetMaintainTypeDesc(MaintainType.FMTSMaintain) });
                return statuses;
            }

            /// <summary>
            /// 根据保养类型编号获取保养类型描述
            /// </summary>
            /// <param name="typeID">保养类型编号</param>
            /// <returns>保养类型描述</returns>
            public static string GetMaintainTypeDesc(int typeID)
            {
                switch (typeID)
                {
                    case OriginalMaintain:
                        return "原厂保养";
                    case ThirdMaintain:
                        return "第三方保养";
                    case FMTSMaintain:
                        return "FMTS保养";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 强检原因
        /// </summary>
        public static class InspectionType
        {
            /// <summary>
            /// 政府要求
            /// </summary>
            public const int GovernmentInspection = 1;
            /// <summary>
            /// 医院要求
            /// </summary>
            public const int HospitalInspection = 2;
            /// <summary>
            /// 自主强检
            /// </summary>
            public const int IndependentInspection = 3;
            /// <summary>
            /// 获取强检原因信息
            /// </summary>
            /// <returns>强检原因信息</returns>
            public static List<KeyValueInfo> GetInspectionType()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = InspectionType.GovernmentInspection, Name = GetInspectionDesc(InspectionType.GovernmentInspection) });
                statuses.Add(new KeyValueInfo() { ID = InspectionType.HospitalInspection, Name = GetInspectionDesc(InspectionType.HospitalInspection) });
                statuses.Add(new KeyValueInfo() { ID = InspectionType.IndependentInspection, Name = GetInspectionDesc(InspectionType.IndependentInspection) });
                return statuses;
            }
            /// <summary>
            /// 根据强检原因编号获取强检原因描述
            /// </summary>
            /// <param name="typeID">强检原因编号</param>
            /// <returns>强检原因描述</returns>
            public static string GetInspectionDesc(int typeID)
            {
                switch (typeID)
                {
                    case GovernmentInspection:
                        return "政府要求";
                    case HospitalInspection:
                        return "医院要求";
                    case IndependentInspection:
                        return "自主强检";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 不良事件来源
        /// </summary>
        public static class AdverseEventType
        {
            /// <summary>
            /// 政府通报
            /// </summary>
            public const int GovernmentAdverseEvent = 1;
            /// <summary>
            /// 医院自检
            /// </summary>
            public const int HospitalAdverseEvent = 2;
            /// <summary>
            /// 召回事件
            /// </summary>
            public const int RecallAdverseEvent = 3;

            /// <summary>
            /// 获取不良事件来源信息
            /// </summary>
            /// <returns>不良事件来源信息</returns>
            public static List<KeyValueInfo> GetAdverseEventType()
            {
                List<KeyValueInfo> statuses = new List<KeyValueInfo>();
                statuses.Add(new KeyValueInfo() { ID = AdverseEventType.GovernmentAdverseEvent, Name = GetAdverseEventDesc(AdverseEventType.GovernmentAdverseEvent) });
                statuses.Add(new KeyValueInfo() { ID = AdverseEventType.HospitalAdverseEvent, Name = GetAdverseEventDesc(AdverseEventType.HospitalAdverseEvent) });
                statuses.Add(new KeyValueInfo() { ID = AdverseEventType.RecallAdverseEvent, Name = GetAdverseEventDesc(AdverseEventType.RecallAdverseEvent) });
                return statuses;
            }

            /// <summary>
            /// 根据不良事件来源编号获取来源描述
            /// </summary>
            /// <param name="typeID">不良事件来源编号</param>
            /// <returns>不良事件来源描述</returns>
            public static string GetAdverseEventDesc(int typeID)
            {
                switch (typeID)
                {
                    case GovernmentAdverseEvent:
                        return "政府通报";
                    case HospitalAdverseEvent:
                        return "医院自检";
                    case RecallAdverseEvent:
                        return "召回事件";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 根据请求类型及故障分类编号获取字段信息描述
        /// </summary>
        /// <param name="requestTypeID">请求类型</param>
        /// <param name="typeID">故障分类编号</param>
        /// <returns>字段信息描述</returns>
        public static string GetDescFaultType(int requestTypeID,int typeID)
        {
            switch (requestTypeID)
            {
                case RequestTypes.Repair:
                    return FaultTypes.GetFaultTypeDesc(typeID);
                case RequestTypes.Maintain:
                    return MaintainType.GetMaintainTypeDesc(typeID);
                case RequestTypes.Inspection:
                    return InspectionType.GetInspectionDesc(typeID);
                case RequestTypes.AdverseEvent:
                    return AdverseEventType.GetAdverseEventDesc(typeID);
                default:
                    return "";
            }
        }
        /// <summary>
        /// 请求处理方式
        /// </summary>
        public static class DealTypes
        {
            /// <summary>
            /// 现场服务
            /// </summary>
            public const int FieldService = 1;
            /// <summary>
            /// 电话解决
            /// </summary>
            public const int TelephoneService = 2;
            /// <summary>
            /// 远程解决
            /// </summary>
            public const int RemoteService = 3;
            /// <summary>
            /// 第三方支持
            /// </summary>
            public const int ThirdSupport = 4;
        }
    }
    /// <summary>
    /// 机器状态
    /// </summary>
    public static class MachineStatuses
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const int Normal = 1;
        /// <summary>
        /// 勉强使用
        /// </summary>
        public const int Barely = 2;
        /// <summary>
        /// 停机
        /// </summary>
        public const int Stopped = 3;

        /// <summary>
        /// 请求页面获取机器状态
        /// </summary>
        /// <returns>机器状态信息</returns>
        public static List<KeyValueInfo> GetMachineStatuses()
        {
            List<KeyValueInfo> statuses = new List<KeyValueInfo>();
            statuses.Add(new KeyValueInfo() { ID = MachineStatuses.Normal, Name = GetMachineStatusesDesc(MachineStatuses.Normal) });
            statuses.Add(new KeyValueInfo() { ID = MachineStatuses.Barely, Name = GetMachineStatusesDesc(MachineStatuses.Barely) });
            statuses.Add(new KeyValueInfo() { ID = MachineStatuses.Stopped, Name = GetMachineStatusesDesc(MachineStatuses.Stopped) });
            return statuses;
        }
        /// <summary>
        /// 根据机器状态编号获取机器状态描述
        /// </summary>
        /// <param name="statusId">机器状态编号</param>
        /// <returns>机器状态描述</returns>
        public static string GetMachineStatusesDesc(int statusId)
        {
            switch (statusId)
            {
                case MachineStatuses.Normal:
                    return "正常";
                case MachineStatuses.Barely:
                    return "勉强使用";
                case MachineStatuses.Stopped:
                    return "停机";
                default:
                    return "";
            }
        }
    }
    /// <summary>
    /// 请求设备关联信息
    /// </summary>
    public class RequestEqptInfo
    {
        /// <summary>
        /// 请求编号
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public int RequestID { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        /// <value>
        /// The equipment.
        /// </value>
        public EquipmentInfo Equipment { get; set; }
        /// <summary>
        /// 请求关联设备信息
        /// </summary>
        public RequestEqptInfo()
        {
            this.Equipment = new EquipmentInfo();
        }
    }
}
