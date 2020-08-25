using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Util;
using BusinessObjects.Manager;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 广播信息
    /// </summary>
    public class NoticeInfo : EntityInfo
    {
        /// <summary>
        /// 广播名称
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// 广播内容
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }
        /// <summary>
        /// 广播备注
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public string Comments { get; set; }
        /// <summary>
        /// 是否轮播
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loop; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoop { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        /// <value>
        /// The oid.
        /// </value>
        public string OID { get { return LookupManager.GetObjectOID(ObjectTypes.Notice, this.ID); } }
        /// <summary>
        /// 广播信息
        /// </summary>
        public NoticeInfo() { }
        /// <summary>
        /// 获取广播信息
        /// </summary>
        /// <param name="dr">The dr.</param>
        public NoticeInfo(DataRow dr)
            : this()
        {
            this.ID = SQLUtil.ConvertInt(dr["ID"]);
            this.Name = SQLUtil.TrimNull(dr["Name"]);
            this.Content = SQLUtil.TrimNull(dr["Content"]);
            this.Comments = SQLUtil.TrimNull(dr["Comments"]);
            this.IsLoop = SQLUtil.ConvertBoolean(dr["IsLoop"]);
            this.CreatedDate = SQLUtil.ConvertDateTime(dr["CreatedDate"]);
        }

    }


}
