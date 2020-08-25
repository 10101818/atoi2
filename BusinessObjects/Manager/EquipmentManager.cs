using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 设备manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class EquipmentManager
    {
        private EquipmentDao equipmentDao = new EquipmentDao();
        private FileDao fileDao = new FileDao();
        private SupplierDao supplierDao = new SupplierDao();
        private FujiClassDao fujiClassDao = new FujiClassDao();

        private UploadFileManager fileManager = new UploadFileManager();
        private RequestManager requestManager = new RequestManager();
        private AuditManager auditManager = new AuditManager();

        /// <summary>
        /// 根据设备编号获取设备信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <returns>设备信息</returns>
        public EquipmentInfo GetEquipment(int id)
        {
            EquipmentInfo info = this.equipmentDao.GetEquipmentByID(id);
            if (info != null)
            {
                info.EquipmentFile = this.fileDao.GetFiles(ObjectTypes.Equipment, info.ID);
                info.HisComponentList = this.equipmentDao.GetComponentByEpqtID(id);
                info.HisConsumableList = this.equipmentDao.GetConsumableByEqptID(id);
            }

            return info;
        }

        /// <summary>
        /// 根据设备编号获取生命周期
        /// </summary>
        /// <param name="equipmentID">设备编号</param>
        /// <returns>生命周期信息</returns>
        public EquipmentInfo GetTimeLine(int equipmentID)
        {
            EquipmentInfo info = this.GetEquipment(equipmentID);
            if (info != null)
            {
                List<DispatchInfo> dispatchs = this.equipmentDao.GetTimeLine(equipmentID);

                DispatchInfo dispatch = null;
                if (info.InstalDate != DateTime.MinValue)
                {
                    dispatch = new DispatchInfo();
                    dispatch.EndDate = info.InstalDate;
                    dispatch.RequestType = new KeyValueInfo() { Name = "装机 - 用户培训完成" };
                    dispatchs.Add(dispatch);
                }
                if (info.PurchaseDate != DateTime.MinValue)
                {
                    dispatch = new DispatchInfo();
                    dispatch.EndDate = info.PurchaseDate;
                    dispatch.RequestType = new KeyValueInfo() { Name = "完成采购" };
                    dispatchs.Add(dispatch);
                }

                info.Dispatches = dispatchs;
            }

            return info;
        }

        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <param name="files">设备附件信息</param>
        /// <param name="user">操作的用户信息</param>
        /// <returns>设备信息</returns>
        [TransactionAspect]
        public EquipmentInfo SaveEquipment(EquipmentInfo info,List<UploadFileInfo> files, UserInfo user)
        {
            if (info.EquipmentStatus.ID != EquipmentInfo.EquipmentStatuses.Scrap)
            {
                info.ScrapDate = DateTime.MinValue;
            }
            else if (info.ScrapDate == DateTime.MinValue)
            {
                info.ScrapDate = DateTime.Today;
            }

            if (string.IsNullOrEmpty(info.AssetCode))
            {
                string date = DateTime.Today.Year + DateTime.Today.Month.ToString("D2") + DateTime.Today.Day.ToString("D2");
                info.AssetCode = this.equipmentDao.GetNextAssetCode(date);
            }

            if (info.ID == 0)
            {
                info = this.equipmentDao.AddEquipment(info, user);
                if (files != null && files.Count > 0)
                {
                    foreach (UploadFileInfo file in files)
                    {
                        file.ObjectID = info.ID;
                        file.ObjectTypeId = ObjectTypes.Equipment;
                        this.fileManager.SaveUploadFile(file);
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("FieldName");
                dt.Columns.Add("OldValue");
                dt.Columns.Add("NewValue");

                dt.Rows.Add("EquipmentName", "", info.Name);
                dt.Rows.Add("AssetCode", "", info.AssetCode);

                this.auditManager.AddAuditLog(user.ID, ObjectTypes.Equipment, info.ID, dt, AuditHdrInfo.AuditOperations.Add);
            }
            else
            {
                EquipmentInfo existingInfo = this.equipmentDao.GetEquipmentByID(info.ID);
                info.FujiClass2.Name = this.fujiClassDao.GetFujiClass2ByID(info.FujiClass2.ID).Name;
                DataTable dtField = existingInfo.GetChangedFields(info);
                if (dtField.Rows.Count > 0)
                {
                    this.equipmentDao.UpdateEquipment(info, user);

                    this.auditManager.AddAuditLog(user.ID, ObjectTypes.Equipment, info.ID, dtField);
                }
            }

            return info;
        }

        /// <summary>
        /// app保存设备信息
        /// </summary>
        /// <param name="info">设备信息</param>
        /// <param name="files">设备附件信息</param>
        /// <param name="user">操作的用户信息</param>
        /// <returns>设备信息</returns>
        [TransactionAspect]
        public EquipmentInfo SaveEquipment4App(EquipmentInfo info, List<UploadFileInfo> files, UserInfo user)
        {
            if (info.EquipmentStatus.ID != EquipmentInfo.EquipmentStatuses.Scrap)
            {
                info.ScrapDate = DateTime.MinValue;
            }
            else if (info.ScrapDate == DateTime.MinValue)
            {
                info.ScrapDate = DateTime.Today;
            }

            if (string.IsNullOrEmpty(info.AssetCode))
            {
                string date = DateTime.Today.Year + DateTime.Today.Month.ToString("D2") + DateTime.Today.Day.ToString("D2");
                info.AssetCode = this.equipmentDao.GetNextAssetCode(date);
            }

            if (info.ID == 0)
            {
                info = this.equipmentDao.AddEquipment(info, user);

                DataTable dt = new DataTable();
                dt.Columns.Add("FieldName");
                dt.Columns.Add("OldValue");
                dt.Columns.Add("NewValue");

                dt.Rows.Add("EquipmentName", "", info.Name);
                dt.Rows.Add("AssetCode", "", info.AssetCode);

                this.auditManager.AddAuditLog(user.ID, ObjectTypes.Equipment, info.ID, dt, AuditHdrInfo.AuditOperations.Add);
            }
            else
            {
                EquipmentInfo existingInfo = this.equipmentDao.GetEquipmentByID(info.ID);
                info.Supplier.Name = info.Supplier.ID == 0 ? "" : this.supplierDao.GetSupplier(info.Supplier.ID).Name;
                info.Manufacturer.Name = info.Manufacturer.ID == 0 ? "" : this.supplierDao.GetSupplier(info.Manufacturer.ID).Name;
                info.EquipmentClass1.Code = SQLUtil.TrimNull(info.EquipmentClass1.Code == null ? "00" : info.EquipmentClass1.Code);
                info.EquipmentClass2.Code = SQLUtil.TrimNull(info.EquipmentClass2.Code == null ? "00" : info.EquipmentClass2.Code);
                info.EquipmentClass3.Code = SQLUtil.TrimNull(info.EquipmentClass3.Code == null ? "00" : info.EquipmentClass3.Code);
                info.FujiClass2.Name = this.fujiClassDao.GetFujiClass2ByID(info.FujiClass2.ID).Name;
                DataTable dtField = existingInfo.GetChangedFields(info);
                if (dtField.Rows.Count > 0)
                {
                    this.equipmentDao.UpdateEquipment(info, user);

                    this.auditManager.AddAuditLog(user.ID, ObjectTypes.Equipment, info.ID, dtField);
                }
            }
            if (files != null && files.Count > 0)
            {
                foreach (UploadFileInfo file in files)
                {
                    file.ObjectID = info.ID;
                    file.ObjectTypeId = ObjectTypes.Equipment;
                    this.fileManager.SaveUploadFile(file);
                }
            }

            return info;
        }

        /// <summary>
        /// 获取设备二维码信息
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="baseUrl">url地址</param>
        /// <param name="hospitalName">医院名称</param>
        /// <returns>二维码信息</returns>
        public string GetEquipmentLabel(int id,string baseUrl,string hospitalName)
        {
            //baseUrl = "http://159.226.128.250/MEMS";
            int dpi = 360;
            int width_mm = 50;
            int height_mm = 32;
            string url = string.Format("{0}/App/Request/Repair?id={1}", baseUrl.TrimEnd('/'), id);

            EquipmentInfo equipment = this.equipmentDao.GetEquipmentByID(id);
            if (equipment == null)
            {
                byte[] arr = File.ReadAllBytes(Path.Combine(Constants.ImageFolder, Constants.ImageErrorName));
                return Convert.ToBase64String(arr);
            }

            //int width = 585, height = 405;
            int width = (int)(width_mm / 25.4 * dpi), height = (int)(height_mm / 25.4 * dpi);//708,453
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);
                    Pen blackPen = new Pen(Color.Black, 5);
                    g.DrawRectangle(blackPen, 0, 0, width-1, height-1);

                    QRCodeGenerator qrcodeGenerater = new QRCodeGenerator();
                    QRCodeData qrcodeData = qrcodeGenerater.CreateQrCode(url, QRCodeGenerator.ECCLevel.H);
                    QRCode qrcode = new QRCode(qrcodeData);
                    using (Bitmap qrcodeStr = qrcode.GetGraphic(4))
                    {
                        g.DrawImage(qrcodeStr, 509, 210, 196, 196);
                    }
                    using (Image logo = Image.FromFile(Path.Combine(Constants.ImageFolder, "logo_block.png")))
                    {
                        using (Bitmap mapLogo = new Bitmap(logo))
                        {
                            g.DrawImage(mapLogo, 479, 130, (int)(logo.Width * 0.052), (int)(logo.Height * 0.052));
                        }
                    }

                    //Font fontHospital = new Font("Microsoft YaHei", 26, FontStyle.Bold);
                    Font fontHead = new Font("Microsoft YaHei", 24, FontStyle.Bold);
                    Font fontValue = new Font("Microsoft YaHei", 22, FontStyle.Bold);
                    Font fontTips = new Font("Microsoft YaHei", 20, FontStyle.Bold);
                    SolidBrush black = new SolidBrush(Color.Black);

                    //g.DrawString("埃　特　欧", new Font("Microsoft YaHei", 18, FontStyle.Bold), black, new PointF(559, 175));
                    int xHeadPoint = 10;
                    int xValuePoint = (int)Math.Ceiling(g.MeasureString("序  列  号:", fontHead).Width);
                    int yPoint = 20;
                    int interval = 60;
                    //g.DrawString("医　  院：", fontHospital, black, new PointF(xHeadPoint, yPoint + 0 * interval));
                    //g.DrawString("科　  室：", fontHospital, black, new PointF(xHeadPoint, yPoint + 1 * interval));
                    g.DrawString("医　　院:",       fontHead, black, new PointF(xHeadPoint, yPoint + 0 * interval));
                    g.DrawString("科　　室:",       fontHead, black, new PointF(xHeadPoint, yPoint + 1 * interval));
                    g.DrawString("放置地点:",       fontHead, black, new PointF(xHeadPoint, yPoint + 2 * interval));
                    g.DrawString("名　　称:",       fontHead, black, new PointF(xHeadPoint, yPoint + 3 * interval));
                    g.DrawString("型　　号:",       fontHead, black, new PointF(xHeadPoint, yPoint + 4 * interval));
                    g.DrawString("序  列  号:",     fontHead, black, new PointF(xHeadPoint, yPoint + 5 * interval));
                    g.DrawString("资产编号:",       fontHead, black, new PointF(xHeadPoint, yPoint + 6 * interval));

                    yPoint += 2;
                    g.DrawString(hospitalName,                   fontValue, black, new PointF(xValuePoint, yPoint + 0 * interval));//hospitalName//"湖南省中医药大学附属第一医院"
                    g.DrawString(equipment.Department.Name,      fontValue, black, new PointF(xValuePoint, yPoint + 1 * interval));
                    g.DrawString(equipment.InstalSite,           fontValue, black, new PointF(xValuePoint, yPoint + 2 * interval));
                    g.DrawString(equipment.Name,                 fontValue, black, new PointF(xValuePoint, yPoint + 3 * interval));
                    g.DrawString(equipment.EquipmentCode,        fontValue, black, new PointF(xValuePoint, yPoint + 4 * interval));
                    g.DrawString(equipment.SerialCode,           fontValue, black, new PointF(xValuePoint, yPoint + 5 * interval));
                    g.DrawString(equipment.AssetCode,            fontValue, black, new PointF(xValuePoint, yPoint + 6 * interval));//equipment.AssetCode//"LSRM0020211234567892"
                    g.DrawString("扫一扫报修", fontTips, black, new PointF(534, 400));
                }

                using(MemoryStream ms = new MemoryStream())
                {
                    //bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    return Convert.ToBase64String(ms.ToArray());
                }               
            }            
            
        }

        /// <summary>
        /// 获取设备未来1年计划服务发生时间
        /// </summary>
        /// <param name="equipmentID">设备ID</param>
        /// <param name="typeID">计划服务类型</param>
        /// <returns>下次发生时间</returns>
        public List<DateTime> GetSysRequestList(int equipmentID, int typeID, EquipmentInfo equipmentInfo)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime nextDate = new DateTime();
            EquipmentInfo info = new EquipmentInfo();
            if (equipmentID == 0)
            {
                info = equipmentInfo;
                info.LastMaintenanceDate = DateTime.Now.Date;
                info.LastCorrectionDate = DateTime.Now.Date;
                info.LastPatrolDate = DateTime.Now.Date;
            }
            else
            {
                info = this.equipmentDao.GetEquipmentByID(equipmentID);
                if (typeID == RequestInfo.RequestTypes.Maintain)
                {
                    info.MaintenancePeriod = equipmentInfo.MaintenancePeriod;
                    info.MaintenanceType = equipmentInfo.MaintenanceType;
                }
                else if (typeID == RequestInfo.RequestTypes.Correcting)
                {
                    info.CorrectionPeriod = equipmentInfo.CorrectionPeriod;
                    info.CorrectionType = equipmentInfo.CorrectionType;
                }
                else if (typeID == RequestInfo.RequestTypes.OnSiteInspection)
                {
                    info.PatrolPeriod = equipmentInfo.PatrolPeriod;
                    info.PatrolType = equipmentInfo.PatrolType;
                }
            }

            List<EquipmentInfo> infos = this.equipmentDao.CheckRequestExsist(new List<int> { equipmentID }, typeID);
            if (infos.Count > 0)
            {
                info.LastMaintenanceDate = DateTime.Now.Date;
                info.LastCorrectionDate = DateTime.Now.Date;
                info.LastPatrolDate = DateTime.Now.Date;
            }

            while (nextDate < DateTime.Now.AddYears(1))
            {
                if (typeID == RequestInfo.RequestTypes.Maintain)
                {
                    nextDate = this.GetCheckDate(info.MaintenanceType.ID, info.MaintenancePeriod, info.LastMaintenanceDate, info.CreateDate);
                    dates.Add(nextDate);
                    info.LastMaintenanceDate = nextDate;
                }
                else if (typeID == RequestInfo.RequestTypes.Correcting)
                {
                    nextDate = this.GetCheckDate(info.CorrectionType.ID, info.CorrectionPeriod, info.LastCorrectionDate, info.CreateDate);
                    dates.Add(nextDate);
                    info.LastCorrectionDate = nextDate;
                }
                else if (typeID == RequestInfo.RequestTypes.OnSiteInspection)
                {
                    if (info.LastPatrolDate == DateTime.MinValue)
                        info.LastPatrolDate = info.CreateDate;
                    if (info.PatrolType.ID == EquipmentInfo.PeriodTypes.Month)
                    {
                        nextDate = info.LastPatrolDate.AddMonths(info.PatrolPeriod).Date;
                    }
                    else if (info.PatrolType.ID == EquipmentInfo.PeriodTypes.Year)
                    {
                        nextDate = info.LastPatrolDate.AddYears(info.PatrolPeriod).Date;
                    }
                    else
                    {
                        nextDate = info.LastPatrolDate.AddDays(info.PatrolPeriod).Date;
                    }
                    if (nextDate < DateTime.Now) nextDate = DateTime.Now.Date;
                    dates.Add(nextDate);
                    info.LastPatrolDate = nextDate;
                }
                else
                {
                    return dates;
                }
            }

            if (dates.Count > 0) dates.RemoveAt(dates.Count - 1);

            return dates;
        }

        /// <summary>
        /// 获取计划服务下次生成时间
        /// </summary>
        /// <param name="typeID">周期类型</param>
        /// <param name="period">周期</param>
        /// <param name="lastChekcDate">上次计划服务时间</param>
        /// <param name="createDate">设备添加时间</param>
        /// <returns>下次计划服务时间</returns>
        public DateTime GetCheckDate(int typeID, int period, DateTime lastChekcDate, DateTime createDate)
        {
            DateTime checkDate = new DateTime();
            if (typeID == EquipmentInfo.PeriodTypes.Month && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddMonths(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Month && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddMonths(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Year && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddYears(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Year && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddYears(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Day && lastChekcDate != DateTime.MinValue)
                checkDate = lastChekcDate.AddDays(period);
            else if (typeID == EquipmentInfo.PeriodTypes.Day && lastChekcDate == DateTime.MinValue)
                checkDate = createDate.AddDays(period);

            if (checkDate < DateTime.Now) checkDate = DateTime.Now.Date;
            if (checkDate <= lastChekcDate) checkDate = lastChekcDate.AddDays(1);
            return checkDate.Date;
        }

        #region "UpdateLastCheckDate"

        /// <summary>
        /// 修改设备上次保养时间
        /// </summary>
        /// <param name="info">设备信息</param>
        [TransactionAspect]
        public void UpdateEquipmentLastMaintenanceCheck(EquipmentInfo info)
        {
            this.equipmentDao.UpdateEquipmentLastMaintenanceCheck(info);
        }

        /// <summary>
        /// 修改设备上次校准时间
        /// </summary>
        /// <param name="info">设备信息</param>
        [TransactionAspect]
        public void UpdateEquipmentLastCorrectionCheck(EquipmentInfo info)
        {
            this.equipmentDao.UpdateEquipmentLastCorrectionCheck(info);
        }

        /// <summary>
        /// 修改设备上次巡检时间
        /// </summary>
        /// <param name="id">设备编号</param>
        /// <param name="date">巡检时间</param>
        [TransactionAspect]
        public void UpdateEquipmentLastPatrolCheck(int id,DateTime date)
        {
            this.equipmentDao.UpdateEquipmentLastPatrolCheck(id,date);
        }
        #endregion
    }
}
