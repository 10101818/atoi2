using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Domain
{
    /// <summary>
    /// 系统信息
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The system name
        /// </summary>
        public const string SYSTEM_NAME = "医疗设备管理系统";
        /// <summary>
        /// 返回列表描述
        /// </summary>
        public const string BACKLIST = "返回列表";

        #region "Folders"
        private const string FOLDER_DOCUMENTS = "Documents";
        private const string FolderContent = "Content";
        private const string FolderImage = "img";
        private const string FOLDER_EQUIPMENT = "Equipment";
        private const string FOLDER_CONTRACT = "Contract";
        private const string FOLDER_SUPPLIER = "Supplier";
        private const string FOLDER_REQUEST = "Request";
        private const string FOLDER_DISPATCHREPORT = "DispatchReport";
        private const string FOLDER_DISPATCHJOURNAL = "DispatchJournal";
        private const string FOLDER_REPORTACCESSORY = "ReportAccessory";

        /// <summary>
        /// 设备附件table
        /// </summary>
        public static string EquipmentFolder = null;
        /// <summary>
        /// 图片附件table
        /// </summary>
        public static string ImageFolder = null;
        /// <summary>
        /// 合同附件table
        /// </summary>
        public static string ContractFolder = null;
        /// <summary>
        /// 厂商附件table
        /// </summary>
        public static string SupplierFolder = null;
        /// <summary>
        /// 作业报告附件table
        /// </summary>
        public static string DispatchReportFolder = null;
        /// <summary>
        /// 服务凭证附件table
        /// </summary>
        public static string DispatchJournalFolder = null;
        /// <summary>
        /// 请求附件table
        /// </summary>
        public static string RequestFolder = null;
        /// <summary>
        /// 零配件附件table
        /// </summary>
        public static string ReportAccessoryFolder = null;
        /// <summary>
        /// 图片名称错误时展示图片
        /// </summary>
        public static string ImageErrorName = "imageNoFound.png";

        //please call this method when web server startup

        /// <summary>
        /// 初始化文件地址
        /// </summary>
        /// <param name="serverPath">服务器地址</param>
        public static void SetFolders(string serverPath)
        {
            EquipmentFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_EQUIPMENT));
            ContractFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_CONTRACT));
            SupplierFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_SUPPLIER));
            DispatchReportFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_DISPATCHREPORT));
            DispatchJournalFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_DISPATCHJOURNAL));
            RequestFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_REQUEST));
            ReportAccessoryFolder = Path.Combine(serverPath, Path.Combine(FOLDER_DOCUMENTS, FOLDER_REPORTACCESSORY));
            ImageFolder = Path.Combine(serverPath, Path.Combine(FolderContent, FolderImage));
        }
        #endregion

        #region "Email url inform"        
        /// <summary>
        /// The login URL
        /// </summary>
        public static string LoginUrl = null;
        /// <summary>
        /// The virtual directory
        /// </summary>
        public static string VirtualDirectory = null;

        //please call this method when web server startup

        /// <summary>
        /// 设置邮件
        /// </summary>
        /// <param name="sLoginUrl">LoginUrl</param>
        /// <param name="sVirtualDirectory">VirtualDirectory</param>
        public static void SetEmailUrlInfo(string sLoginUrl, string sVirtualDirectory)
        {
            LoginUrl = sLoginUrl;
            VirtualDirectory = sVirtualDirectory;
        }
        #endregion
    }
}
