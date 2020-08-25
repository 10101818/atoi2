using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// ValuationManager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ValuationManager
    {
        private ValuationDao valuationDao = new ValuationDao();
        private FujiClassDao FujiClassDao = new FujiClassDao();
        private FaultRateDao faultRateDao = new FaultRateDao();

        /// <summary>
        /// 修改估价参数
        /// </summary>
        /// <param name="info">估价参数信息</param>
        [TransactionAspect]
        public void UpdateParameter(ValParameterInfo info)
        {
            this.valuationDao.UpdateParameter(info);
        }

        #region ValControl
        /// <summary>
        /// 保存估价前提条件信息
        /// </summary>
        /// <param name="info">估价前提条件</param>
        [TransactionAspect]
        public void SaveValControl(ValControlInfo info)
        {
            this.valuationDao.UpdateControl(info);
        }

        public ValControlInfo GetValControlInfo(int userID)
        {
            ValControlInfo info = this.valuationDao.GetControl(userID);

            if (info == null)
            {
                info = new ValControlInfo();
                info.ContractStartDate = DateTime.Now;
                info.EndDate = DateTime.Now;
                ValParameterInfo parameterInfo = this.valuationDao.GetParameter();
                info.HospitalLevel.ID = parameterInfo.HospitalLevel.ID;
                info.HospitalFactor1 = parameterInfo.HospitalFactor1;
                info.HospitalFactor2 = parameterInfo.HospitalFactor2;
                info.HospitalFactor3 = parameterInfo.HospitalFactor3;
                info.User.ID = userID;

                this.valuationDao.AddControl(info);
                this.valuationDao.InitEquipments(userID);
                this.valuationDao.InitComponents(userID);
                this.valuationDao.InitConsumables(userID);
                this.valuationDao.InitSpares(userID);
            }

            return info;
        }
        #endregion

        #region ValEquipment
        /// <summary>
        /// 保存设备
        /// </summary>
        /// <param name="updateEquipments">更改的设备信息</param>
        /// <param name="importEquipments">导入的设备信息</param>
        [TransactionAspect]
        public void SaveValEquipments(int userID, List<ValEquipmentInfo> updateEquipments, List<ValEquipmentInfo> importEquipments = null)
        {
            if (updateEquipments != null && updateEquipments.Count > 0)
            {                
                DataTable dt = ValEquipmentInfo.ConvertValEquipmentTable(updateEquipments);
                this.valuationDao.UpdateEquipments(dt);
            }

            if (importEquipments != null && importEquipments.Count > 0)
            {
                this.valuationDao.DeleteEquipments(userID, true);
                this.valuationDao.DeleteConsumables(userID, true);
                this.valuationDao.DeleteComponents(userID, true);
                this.valuationDao.DeleteSpares(userID, true);

                DataTable dt = ValEquipmentInfo.ConvertValEquipmentTable(importEquipments);
                this.valuationDao.ImportEquipments(dt);

                this.valuationDao.InitConsumables(userID);
                this.valuationDao.InitComponents(userID);
                this.valuationDao.InitSpares(userID);
            }            
        }

        /// <summary>
        /// 格式化导入的设备信息
        /// </summary>
        /// <param name="fileBytes">设备信息</param>
        /// <param name="message">导入状态信息</param>
        /// <param name="importEquipments">新增设备</param>
        /// <param name="updateEquipments">修改设备</param>
        /// <returns>导入状态</returns>
        public int ParseImportFile(int userID, byte[] fileBytes, out string message, out List<ValEquipmentInfo> importEquipments, out List<ValEquipmentInfo> updateEquipments)
        {
            List<int> inSystemEquipmentIDs = this.valuationDao.GetInSystemEquipmentID(userID);
            List<FujiClass2Info> existFujiClass2List = this.FujiClassDao.GetFujiClass2();

            message = "";
            importEquipments = new List<ValEquipmentInfo>();
            updateEquipments = new List<ValEquipmentInfo>();
            DataTable dt = null;
            if(Util.ImportUtil.ReadExcelFile(fileBytes, out dt) == false)
            {
                message = "读取Excel文件失败";
                return 1;
            }
            if(dt.Rows.Count == 0)
            {
                message = "Excel文件为空";
                return 1;
            }
            if(dt.Columns.Count < 14)
            {
                message = "Excel文件小于14列";
                return 1;
            }

            string OID = null;
            dt.RemoveEmptyRows();
            foreach(DataRow drRow in dt.Rows)
            {
                ValEquipmentInfo info = new ValEquipmentInfo();
                info.InSystem = SQLUtil.TrimNull(drRow[0]) == "Y" ? true : false;
                OID = SQLUtil.TrimNull(drRow[1]);
                if (info.InSystem) info.Equipment.ID = BaseDao.ProcessOID(OID);
                else info.Equipment.ID = importEquipments.Count + 1;
                info.Equipment.AssetCode = SQLUtil.TrimNull(drRow[2]);
                info.Equipment.Name = SQLUtil.TrimNull(drRow[3]);
                info.Equipment.SerialCode = SQLUtil.TrimNull(drRow[4]);
                info.Equipment.Manufacturer.Name = SQLUtil.TrimNull(drRow[5]);
                info.Equipment.Department.Name = SQLUtil.TrimNull(drRow[6]);
                double amount = 0;
                if (!Double.TryParse(SQLUtil.TrimNull(drRow[7]), out amount))
                {
                    message = string.Format("系统编号为{0}的金额无效", OID);
                    return 1;
                }
                info.Equipment.PurchaseAmount = SQLUtil.ConvertDouble(drRow[7]);
                info.Equipment.FujiClass2.Name = SQLUtil.TrimNull(drRow[10]);
                info.Equipment.FujiClass2.ID = (from FujiClass2Info fujiClass2 in existFujiClass2List where fujiClass2.Name == info.Equipment.FujiClass2.Name select fujiClass2.ID).FirstOrDefault();
                info.CurrentScope.Name = SQLUtil.TrimNull(drRow[11]);
                info.CurrentScope.ID = ValEquipmentInfo.ScopeTypes.GetScopeID(info.CurrentScope.Name);
                info.EndDate = SQLUtil.ConvertDateTime(drRow[12]);
                info.NextScope.Name = SQLUtil.TrimNull(drRow[13]);
                info.NextScope.ID = ValEquipmentInfo.ScopeTypes.GetScopeID(info.NextScope.Name);
                info.User.ID = userID;

                if (info.InSystem == false)
                {
                    if (info.Equipment.FujiClass2.ID == 0)
                    {
                        message = string.Format("设备富士II类:'{0}'不存在", info.Equipment.FujiClass2.Name);
                        return 1;
                    }
                    if (info.Equipment.AssetCode.Length == 0 || info.Equipment.AssetCode.Length > 30)
                    {
                        message = string.Format("系统编号为{0}的设备资产编号无效", OID);
                        return 1;
                    }
                    if (info.Equipment.Name.Length == 0 || info.Equipment.Name.Length > 30)
                    {
                        message = string.Format("系统编号为{0}的设备名称无效", OID);
                        return 1;
                    }
                    if (info.Equipment.SerialCode.Length == 0 || info.Equipment.SerialCode.Length > 30)
                    {
                        message = string.Format("系统编号为{0}的设备序列号无效", OID);
                        return 1;
                    }
                    if (info.Equipment.Manufacturer.Name.Length == 0 || info.Equipment.Manufacturer.Name.Length > 50)
                    {
                        message = string.Format("系统编号为{0}的设备厂商无效", OID);
                        return 1;
                    }                  
                    if (info.Equipment.Department.Name.Length == 0 || info.Equipment.Department.Name.Length > 20)
                    {
                        message = string.Format("系统编号为{0}的设备科室无效", OID);
                        return 1;
                    }
                    if (info.Equipment.PurchaseAmount > 99999999.99)
                    {
                        message = string.Format("系统编号为{0}的设备金额最大为99999999.99", OID);
                        return 1;
                    }
                }

                if (info.CurrentScope.ID == 0)
                {
                    message = string.Format("目前维保种类:'{0}'不存在", info.CurrentScope.Name);
                    return 1;
                }
                if(info.CurrentScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage && info.EndDate == DateTime.MinValue)
                {
                    message = string.Format("系统编号为{0}的设备维保到期日不能为空", OID);
                    return 1;
                }
                if(info.NextScope.ID == 0)
                {
                    message = string.Format("下期维保种类:'{0}'不存在", info.NextScope.Name);
                    return 1;
                }

                if (info.InSystem)
                {
                    if (inSystemEquipmentIDs.Contains(info.Equipment.ID))
                    {
                        updateEquipments.Add(info);
                    }
                    else
                    {
                        message = string.Format("系统中不存在系统编号为{0}的设备", OID);
                        return 1;
                    }
                }
                else
                {
                    importEquipments.Add(info);
                }
            }
            if (updateEquipments.Count == 0 && importEquipments.Count == 0)
            {
                message = "Excel文件为空";
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 更新设备工时
        /// </summary>
        [TransactionAspect]
        public void UpdateEquipmentHours(int userID)
        {
            this.valuationDao.UpdateEquipmentHours(userID);
        }

        #endregion

        #region ValSpare
        /// <summary>
        /// 保存备用机信息
        /// </summary>
        /// <param name="infos">备用机信息</param>
        [TransactionAspect]
        public void SaveValSpares(List<ValSpareInfo> infos)
        {
            DataTable dt = ValSpareInfo.ConvertValSpareTable(infos);
            this.valuationDao.UpdateSpares(dt);
        }
        #endregion

        #region ValComponent
        /// <summary>
        /// 保存零件信息
        /// </summary>
        /// <param name="infos">零件信息</param>
        [TransactionAspect]
        public void SaveValComponents(List<ValComponentInfo> infos)
        {
            DataTable dt = ValComponentInfo.ConvertValComponentTable(infos);
            this.valuationDao.UpdateComponents(dt);
        }
        #endregion

        #region ValConsumable
        /// <summary>
        /// 保存耗材信息
        /// </summary>
        /// <param name="infos">耗材信息</param>
        [TransactionAspect]
        public void SaveValConsumables(List<ValConsumableInfo> infos)
        {
            DataTable dt = ValConsumableInfo.ConvertValConsumableTable(infos);
            this.valuationDao.UpdateConsumables(dt);
        }
        #endregion

        #region 成本明细
        /// <summary>
        /// 估价执行
        /// </summary>
        [TransactionAspect]
        public void RunVal(int userID)
        {
            this.valuationDao.DeleteValEqptOutput(userID);
            this.valuationDao.DeleteValConsumableOutput(userID);
            this.valuationDao.DeleteValComponentOutput(userID);

            ValControlInfo control = this.valuationDao.GetControl(userID);

            DataTable dtContractAmount = CalculateContractAmountForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear);
            DataTable dtRepair3partyCost = CalculateRepair3partyCostForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear);
            dtContractAmount.Merge(dtRepair3partyCost);
            this.valuationDao.InsertValEquipmentOutput(dtContractAmount);

            DataTable dtComponentAmount = CalculateComponentsAmountForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear);
            this.valuationDao.InsertValComponentOutput(dtComponentAmount);

            //耗材
            CalculateConsumableAmountForcast(userID, control.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear);

            this.valuationDao.UpdateControlStatus(control.User.ID);
        }
        //维保费
        private DataTable CalculateContractAmountForcast(int userID, DateTime startDate, int years)
        {
            List<ValEquipmentInfo> valEquipmentInfos = this.valuationDao.QueryEquipmentList(userID, -1, -1, "", "", "", 0, 0, "", "", "ve.EquipmentID", true);
            List<FujiClass2Info> fujiClass2Infos = this.FujiClassDao.GetFujiClass2();

            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("Year", typeof(System.Int32));
            dt.Columns.Add("Month", typeof(System.Int32));
            dt.Columns.Add("ContractAmount", typeof(System.Double));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0], dt.Columns[1], dt.Columns[2], dt.Columns[3], dt.Columns[4] };

            double amount, baseAmount;
            DateTime curDate;
            foreach(ValEquipmentInfo valEquip in valEquipmentInfos)
            {
                baseAmount = 0;

                FujiClass2Info fujiClass2 = (from FujiClass2Info temp in fujiClass2Infos where temp.ID == valEquip.Equipment.FujiClass2.ID select temp).FirstOrDefault();
                //if (fujiClass2.IncludeContract)
                {
                    if (valEquip.NextScope.ID == ValEquipmentInfo.ScopeTypes.FullCoverage)
                        baseAmount = Math.Round(valEquip.Equipment.PurchaseAmount * fujiClass2.FullCoveragePtg / 100.0 / 12.0, 2);
                    if (valEquip.NextScope.ID == ValEquipmentInfo.ScopeTypes.TechCoverage)
                        baseAmount = Math.Round(valEquip.Equipment.PurchaseAmount * fujiClass2.TechCoveragePtg / 100.0 / 12.0, 2);
                }

                for (int i = 0; i < years * 12; i++)
                {
                    curDate = startDate.AddMonths(i);
                    if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                    {
                        if (curDate <= valEquip.EndDate)
                            amount = 0;
                        else
                            amount = baseAmount;

                        dt.Rows.Add(userID, valEquip.InSystem, valEquip.Equipment.ID, curDate.Year, curDate.Month, amount);
                    }
                }      
            }

            return dt;
        }
        //外来服务费
        public DataTable CalculateRepair3partyCostForcast(int userID, DateTime startDate, int years, bool contractFlag=true)
        {
            List<ValEquipmentInfo> valEquipmentInfos = this.valuationDao.QueryEquipmentList(userID, -1, -1, "", "", "", 0, 0, "", "", "ve.EquipmentID", true);
            List<FujiClass2Info> fujiClass2Infos = this.FujiClassDao.GetFujiClass2();
            Dictionary<int, List<FaultRateInfo>> dicFaultRates = this.faultRateDao.GetFaultRateByObject(SQLUtil.GetIDListFromObjectList(fujiClass2Infos), FaultRateInfo.ObjectType.Repair);

            List<ValComponentInfo> valComponentInfos = this.valuationDao.QueryComponentList(userID, 0, 0, "", "", true);

            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("Year", typeof(System.Int32));
            dt.Columns.Add("Month", typeof(System.Int32));
            dt.Columns.Add("Repair3partyCost", typeof(System.Double));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0], dt.Columns[1], dt.Columns[2], dt.Columns[3], dt.Columns[4] };

            double amount, baseAmount, usageRate;
            DateTime curDate;
            int monthPassed;
            foreach (ValEquipmentInfo valEquip in valEquipmentInfos)
            {
                baseAmount = 0;
                usageRate = 0;

                FujiClass2Info fujiClass2 = (from FujiClass2Info temp in fujiClass2Infos where temp.ID == valEquip.Equipment.FujiClass2.ID select temp).FirstOrDefault();
                if (!contractFlag || fujiClass2.IncludeRepair)
                {
                    if (!contractFlag || (fujiClass2.EquipmentType.ID != FujiClass2Info.LKPEquipmentType.General && 
                        (valEquip.CurrentScope.ID == ValEquipmentInfo.ScopeTypes.NoneCoverage || valEquip.NextScope.ID == ValEquipmentInfo.ScopeTypes.NoneCoverage)))
                        baseAmount = fujiClass2.Repair3partyCost * fujiClass2.Repair3partyRatio / 100.0 / 12;

                    ValComponentInfo valComponentInfo = (from ValComponentInfo temp in valComponentInfos where temp.InSystem == valEquip.InSystem && temp.Equipment.ID == valEquip.Equipment.ID select temp).FirstOrDefault();
                    usageRate = Math.Round(fujiClass2.Usage == 0 ? 0 : valComponentInfo == null || valComponentInfo.Usage == 0 ? 0 : valComponentInfo.Usage * 1.0 / fujiClass2.Usage, 2);
                }

                monthPassed = GetEquipUsageMonths(startDate, valEquip.Equipment.UseageDate);

                List<FaultRateInfo> faultRates = new List<FaultRateInfo>();
                if (valEquip.Equipment.FujiClass2.ID != -1) faultRates = dicFaultRates[valEquip.Equipment.FujiClass2.ID];
                for (int i = 0; i < years * 12; i++)
                {
                    curDate = startDate.AddMonths(i);
                    if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                    {
                        if (faultRates.Count == 0 || (contractFlag && (baseAmount == 0 || 
                            (curDate <= valEquip.EndDate && valEquip.CurrentScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage) ||
                            (curDate > valEquip.EndDate && valEquip.NextScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage))))
                        {
                            amount = 0;
                        }
                        else
                        {
                            amount = CalculateAmountByFaultRate(faultRates, baseAmount, monthPassed, usageRate, i);
                        }

                        dt.Rows.Add(userID, valEquip.InSystem, valEquip.Equipment.ID, curDate.Year, curDate.Month, amount);
                    }
                }
            }
            return dt;
        }
        //零件
        public DataTable CalculateComponentsAmountForcast(int userID, DateTime startDate, int years, bool contractFlag = true)
        {
            List<ValEquipmentInfo> valEquipmentInfos = this.valuationDao.QueryEquipmentList(userID, -1, -1, "", "", "", 0, 0, "", "", "ve.EquipmentID", true);
            List<FujiClass2Info> fujiClass2Infos = this.FujiClassDao.GetFujiClass2();
            Dictionary<int, List<FaultRateInfo>> dicFaultRatesEquip = this.faultRateDao.GetFaultRateByObject(SQLUtil.GetIDListFromObjectList(fujiClass2Infos), FaultRateInfo.ObjectType.Repair);

            List<ValComponentInfo> valComponentInfos = this.valuationDao.QueryComponentList(userID, -1, 0, "", "", false);
            Dictionary<int, List<FaultRateInfo>> dicFaultRatesComp = this.faultRateDao.GetFaultRateByObject(SQLUtil.GetIDListFromObjectList(valComponentInfos, "ComponentID"), FaultRateInfo.ObjectType.Component);

            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("ComponentID", typeof(System.Int32));
            dt.Columns.Add("Year", typeof(System.Int32));
            dt.Columns.Add("Month", typeof(System.Int32));
            dt.Columns.Add("Amount", typeof(System.Double));

            foreach (ValComponentInfo valComponentInfo in valComponentInfos)
            {
                ValEquipmentInfo valEquip = (from ValEquipmentInfo temp in valEquipmentInfos where temp.InSystem == valComponentInfo.InSystem && temp.Equipment.ID == valComponentInfo.Equipment.ID select temp).FirstOrDefault();
                FujiClass2Info fujiClass2 = (from FujiClass2Info temp in fujiClass2Infos where temp.ID == valEquip.Equipment.FujiClass2.ID select temp).FirstOrDefault();

                if(valComponentInfo.Component.ID == 0)
                {
                    if (fujiClass2.EquipmentType.ID == FujiClass2Info.LKPEquipmentType.General)
                    {
                        CalculateComponentAmountForcast1(userID, startDate, years, dt, valEquip, fujiClass2, valComponentInfo, contractFlag);
                    }
                    else
                    {
                        CalculateComponentAmountForcast2(userID, startDate, years, dt, valEquip, fujiClass2, valComponentInfo, dicFaultRatesEquip[valEquip.Equipment.FujiClass2.ID], contractFlag);
                    }
                }
                else if (valComponentInfo.Component.Type.ID == ComponentInfo.ComponentTypes.CT)
                {
                    CalculateComponentAmountForcast4(userID, startDate, years, dt, valEquip, fujiClass2, valComponentInfo, contractFlag);
                }
                else
                {
                    CalculateComponentAmountForcast3(userID, startDate, years, dt, valEquip, fujiClass2, valComponentInfo, dicFaultRatesComp[valComponentInfo.Component.ID], contractFlag);
                }
            }

            return dt;
        }

        //CT球管
        private static void CalculateComponentAmountForcast4(int userID, DateTime startDate, int years, DataTable dt, ValEquipmentInfo valEquip, FujiClass2Info fujiClass2, ValComponentInfo valComponentInfo, bool contractFlag = true)
        {
            double amount;            
            double currentSeconds = valComponentInfo.Seconds;
            for (int i = 0; i < years * 12; i++)
            {
                DateTime curDate = startDate.AddMonths(i);
                if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                {
                    amount = 0;
                    currentSeconds += Math.Round(valComponentInfo.Usage * valComponentInfo.Component.SecondsPer * DateTime.DaysInMonth(curDate.Year,curDate.Month), 2);
                    var rate = Math.Floor(currentSeconds / valComponentInfo.Component.TotalSeconds);
                    if (currentSeconds > valComponentInfo.Component.TotalSeconds)
                    {
                        if (!contractFlag || 
                            !valComponentInfo.IncludeContract ||
                            (curDate <= valEquip.EndDate && valEquip.CurrentScope.ID == ValEquipmentInfo.ScopeTypes.NoneCoverage) ||
                            (curDate > valEquip.EndDate && valEquip.NextScope.ID == ValEquipmentInfo.ScopeTypes.NoneCoverage))
                        {
                            amount = valComponentInfo.Component.StdPrice * rate;
                        }

                        currentSeconds = Math.Round(currentSeconds - valComponentInfo.Component.TotalSeconds * rate, 2);
                    }

                    dt.Rows.Add(userID, valComponentInfo.InSystem, valComponentInfo.Equipment.ID, valComponentInfo.Component.ID, curDate.Year, curDate.Month, amount);
                }
            }
        }

        //其他零件
        private void CalculateComponentAmountForcast3(int userID, DateTime startDate, int years, DataTable dt, ValEquipmentInfo valEquip, FujiClass2Info fujiClass2, ValComponentInfo valComponentInfo, List<FaultRateInfo> faultRates, bool contractFlag = true)
        {
            //TODO 因为作业报告零配件尚未开发，目前使用默认值 baseAmount and monthPassed
            double amount;
            double baseAmount = valComponentInfo.Component.StdPrice;
            int monthPassed = 0;
            monthPassed = GetEquipUsageMonths(startDate, valComponentInfo.UseageDate);
            double usageRate = Math.Round(valComponentInfo.Component.Usage == 0 ? 0 : valComponentInfo.Usage * valComponentInfo.Qty * 1.0 / valComponentInfo.Component.Usage, 2);

            for (int i = 0; i < years * 12; i++)
            {
                DateTime curDate = startDate.AddMonths(i);
                if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                {
                    if (contractFlag && (!fujiClass2.IncludeRepair ||
                        (curDate <= valEquip.EndDate && valEquip.CurrentScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage && valComponentInfo.IncludeContract) ||
                        (curDate > valEquip.EndDate && valEquip.NextScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage && valComponentInfo.IncludeContract)))
                    {
                        amount = 0;
                    }
                    else
                    {
                        amount = CalculateAmountByFaultRate(faultRates, baseAmount, monthPassed, usageRate, i);
                    }

                    dt.Rows.Add(userID, valComponentInfo.InSystem, valComponentInfo.Equipment.ID, valComponentInfo.Component.ID, curDate.Year, curDate.Month, amount);
                }
            }
        }

        //重点， 次重点设备 整机的计算
        private void CalculateComponentAmountForcast2(int userID, DateTime startDate, int years, DataTable dt, ValEquipmentInfo valEquip, FujiClass2Info fujiClass2, ValComponentInfo valComponentInfo, List<FaultRateInfo> faultRates, bool contractFlag = true)
        {
            double amount;
            double baseAmount = fujiClass2.RepairComponentCost;
            double usageRate = Math.Round(fujiClass2.Usage == 0 ? 0 : valComponentInfo.Usage * 1.0 / fujiClass2.Usage, 2);
            int monthPassed = GetEquipUsageMonths(startDate, valEquip.Equipment.UseageDate);

            for (int i = 0; i < years * 12; i++)
            {
                DateTime curDate = startDate.AddMonths(i);
                if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                {
                    if (contractFlag && (!fujiClass2.IncludeRepair ||
                        (curDate <= valEquip.EndDate && valEquip.CurrentScope.ID == ValEquipmentInfo.ScopeTypes.FullCoverage) ||
                        (curDate > valEquip.EndDate && valEquip.NextScope.ID == ValEquipmentInfo.ScopeTypes.FullCoverage)))
                    {
                        amount = 0;
                    }
                    else
                    {
                        amount = CalculateAmountByFaultRate(faultRates, baseAmount, monthPassed, usageRate, i);
                    }

                    dt.Rows.Add(userID, valComponentInfo.InSystem, valComponentInfo.Equipment.ID, valComponentInfo.Component.ID, curDate.Year, curDate.Month, amount);
                }
            }
        }

        //一般设备 整机的计算
        private static void CalculateComponentAmountForcast1(int userID, DateTime startDate, int years, DataTable dt, ValEquipmentInfo valEquip, FujiClass2Info fujiClass2, ValComponentInfo valComponentInfo, bool contractFlag = true)
        {
            double amount;
            double baseAmount = Math.Round(valEquip.Equipment.PurchaseAmount * fujiClass2.RepairCostRatio / 100.0, 2);

            for (int i = 0; i < years * 12; i++)
            {
                DateTime curDate = startDate.AddMonths(i);
                if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                {
                    if (contractFlag && (!fujiClass2.IncludeRepair ||
                        (curDate <= valEquip.EndDate && valEquip.CurrentScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage) ||
                        (curDate > valEquip.EndDate && valEquip.NextScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage)))
                    {
                        amount = 0;
                    }
                    else
                    {
                        amount = baseAmount;
                    }

                    dt.Rows.Add(userID, valComponentInfo.InSystem, valComponentInfo.Equipment.ID, valComponentInfo.Component.ID, curDate.Year, curDate.Month, amount);
                }
            }
        }
        //耗材
        private void CalculateConsumableAmountForcast(int userID, DateTime startDate, int years)
        {
            List<ValEquipmentInfo> valEquipmentInfos = this.valuationDao.QueryEquipmentList(userID, -1, -1, "", "", "", 0, 0, "", "", "ve.EquipmentID", true);
            List<ValConsumableInfo> valConsumableInfos = this.valuationDao.QueryConsumableList(userID, 0, null, null);

            DataTable dt = new DataTable();
            dt.Columns.Add("UserID", typeof(System.Int32));
            dt.Columns.Add("Insystem", typeof(System.Boolean));
            dt.Columns.Add("EquipmentID", typeof(System.Int32));
            dt.Columns.Add("ConsumableID", typeof(System.Int32));
            dt.Columns.Add("Year", typeof(System.Int32));
            dt.Columns.Add("Month", typeof(System.Int32));
            dt.Columns.Add("Amount", typeof(System.Double));

            double amount, baseAmount;
            DateTime curDate;
            foreach (ValConsumableInfo valConsumableInfo in valConsumableInfos)
            {
                baseAmount = Math.Round(valConsumableInfo.Consumable.CostPer * valConsumableInfo.Consumable.ReplaceTimes / 12.0, 2);

                foreach (ValEquipmentInfo valEquip in (from ValEquipmentInfo temp in valEquipmentInfos where temp.Equipment.FujiClass2.ID == valConsumableInfo.Consumable.FujiClass2.ID select temp))
                {
                    for (int i = 0; i < years * 12; i++)
                    {
                        curDate = startDate.AddMonths(i);
                        if (curDate >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day))
                        {
                            if (valConsumableInfo.IncludeContract &&
                                ((curDate <= valEquip.EndDate && valEquip.CurrentScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage) ||
                                    (curDate > valEquip.EndDate && valEquip.NextScope.ID != ValEquipmentInfo.ScopeTypes.NoneCoverage)))
                            {
                                amount = 0;
                            }
                            else
                            {
                                amount = baseAmount;
                            }

                            dt.Rows.Add(userID, valEquip.InSystem, valEquip.Equipment.ID, valConsumableInfo.Consumable.ID, curDate.Year, curDate.Month, amount);
                        }
                    }
                }
            }

            this.valuationDao.InsertValConsumableOutput(dt);
        }
        //用故障率计算金额
        private double CalculateAmountByFaultRate(List<FaultRateInfo> faultRates, double baseAmount, int monthPassed, double usageRate, int months)
        {
            int faultRateIndex = (int)Math.Round(usageRate * (monthPassed + months), 0);

            if (faultRateIndex >= faultRates.Count) faultRateIndex = faultRates.Count - 1;

            return Math.Round(baseAmount * faultRates[faultRateIndex].Rate / 100.0, 2);
        }

        //设备已使用月数
        private int GetEquipUsageMonths(DateTime endDate, DateTime startDate)
        {
            startDate = (startDate == DateTime.MinValue ? endDate : startDate);

            return (endDate.Year * 12 + endDate.Month) - (startDate.Year * 12 + startDate.Month);
        }

        #endregion

        #region 获取估价执行结果
        public Dictionary<string, object> GetValResultData(int userID)
        {
            ValControlInfo controlInfo = this.valuationDao.GetControl(userID);
            ValParameterInfo parameterInfo = this.valuationDao.GetParameter();

            DateTime ActualStartDate = controlInfo.ContractStartDate.AddYears(-ValControlInfo.ActualYears.ActualYear);

            Dictionary<string, object> data = new Dictionary<string, object>();
            List<KeyValueInfo> ForecastDateList = MonthDesc.GetValForecastDateDescList(controlInfo.ContractStartDate, ValControlInfo.ForecastYears.ForecastYear);
            data.Add("ForecastDate", ForecastDateList);

            //计算小计的时间范围
            int startIndex, count;
            DateTime ForecastEndDate = controlInfo.ContractStartDate.AddYears(ValControlInfo.ForecastYears.ForecastYear);
            DateTime TotalEndDate = controlInfo.ContractStartDate.AddYears(controlInfo.Years);
            if (TotalEndDate > ForecastEndDate) TotalEndDate = ForecastEndDate;

            if (controlInfo.EndDate > TotalEndDate || TotalEndDate == controlInfo.EndDate)
            {
                startIndex = 0;
                count = 0;
            }
            else if (controlInfo.EndDate < controlInfo.ContractStartDate)
            {
                startIndex = 0;
                count = GetEquipUsageMonths(TotalEndDate, controlInfo.ContractStartDate);
            }
            else
            {
                startIndex = GetEquipUsageMonths(controlInfo.EndDate, controlInfo.ContractStartDate);
                count = GetEquipUsageMonths(TotalEndDate, controlInfo.EndDate);
            }

            //计算预测与实绩重叠的月数
            DateTime curMonth = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
            int actualForecastMonth = 0;
            if (ForecastEndDate < curMonth)
                actualForecastMonth = ValControlInfo.ForecastYears.ForecastYear * 12;
            else if (controlInfo.ContractStartDate < curMonth)
                actualForecastMonth = GetEquipUsageMonths(curMonth, controlInfo.ContractStartDate);

            List<double> contractForecastAmountList4Actual = new List<double>();//维保重叠
            List<double> spareForecastAmountList4Actual = new List<double>();//备用机
            List<double> regularForecastAmountList4Actual = new List<double>();//耗材定期
            List<double> quanTityForecastAmountList4Actual = new List<double>();//耗材定量
            List<double> smallForecastAmountList4Actual = new List<double>();//小额成本
            List<double> importantComponentForecastAmountList4Actual = new List<double>();//重点设备零件
            List<double> generalComponentForecastAmountList4Actual = new List<double>();//一般设备零件
            List<double> componentForecastAmountList4Actual = new List<double>();//零件
            List<double> importantRepair3partyForecastAmountList4Actual = new List<double>();//重点设备服务费
            List<double> generalRepair3partyForecastAmountList4Actual = new List<double>();//一般设备服务费
            List<double> repair3partyForecastAmountList4Actual = new List<double>();//服务费

            if (actualForecastMonth > 0)
            {
                DateTime startDate = controlInfo.ContractStartDate;
                while (startDate < curMonth)
                {
                    contractForecastAmountList4Actual.Add(this.valuationDao.GetActualContractAmount(startDate));
                    spareForecastAmountList4Actual.Add(this.valuationDao.GetActualSpareAmount(startDate));
                    regularForecastAmountList4Actual.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.RegularConsumable, startDate));
                    quanTityForecastAmountList4Actual.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.QuantitativeConsumable, startDate));
                    smallForecastAmountList4Actual.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.SmallCostConsumable, startDate));
                    importantComponentForecastAmountList4Actual.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                    generalComponentForecastAmountList4Actual.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                    componentForecastAmountList4Actual.Add(this.valuationDao.GetActulComponentAmount(-1, startDate));
                    importantRepair3partyForecastAmountList4Actual.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                    generalRepair3partyForecastAmountList4Actual.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                    repair3partyForecastAmountList4Actual.Add(this.valuationDao.GetActulServiceAmount(-1, startDate));

                    startDate = startDate.AddMonths(1);
                }
            }

            #region 成本明细
            //信息系统使用费
            List<double> systemForecastAmountList = InitAmountList(ForecastDateList.Count, Math.Round(parameterInfo.SystemCost / 12, 2));
            double systemAmountTotal = systemForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("SystemForecastAmount", InitAmountListByYear(systemForecastAmountList));
            data.Add("SystemAmountTotal", Math.Round(systemAmountTotal, 0));
            //人工费
            List<double> labourForecastAmountList = InitAmountList(ForecastDateList.Count, controlInfo.ForecastEngineer * parameterInfo.UnitCost);
            double labourAmountTotal = labourForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("LabourForecastAmount", InitAmountListByYear(labourForecastAmountList));
            data.Add("LabourAmountTotal", Math.Round(labourAmountTotal, 0));
            //维保费            
            List<double> contractForecastAmountList = this.valuationDao.GetValEqptContractAmount(userID).Select(info => info.ContractAmount).ToList();
            if (contractForecastAmountList.Count == 0) contractForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            contractForecastAmountList = contractForecastAmountList4Actual.Concat(contractForecastAmountList).ToList();

            double contractAmountTotal = contractForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("ContractForecastAmount", InitAmountListByYear(contractForecastAmountList));
            data.Add("ContractAmountTotal", Math.Round(contractAmountTotal, 0));
            //备用机
            List<double> spareForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, this.valuationDao.GetSpareAmount(userID));
            spareForecastAmountList = spareForecastAmountList4Actual.Concat(spareForecastAmountList).ToList();

            double spareAmountTotal = spareForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("SpareForecastAmount", InitAmountListByYear(spareForecastAmountList));
            data.Add("SpareAmountTotal", Math.Round(spareAmountTotal, 0));
            //固定类
            List<double> fixedForecastAmountList = SQLUtil.MergeList(systemForecastAmountList, labourForecastAmountList, contractForecastAmountList, spareForecastAmountList);
            double fixedAmountTotal = systemAmountTotal + labourAmountTotal + contractAmountTotal + spareAmountTotal;
            data.Add("FixedForecastAmount", InitAmountListByYear(fixedForecastAmountList));
            data.Add("FixedAmountTotal", Math.Round(fixedAmountTotal, 0));

            //耗材定期
            List<double> regularForecastAmountList = this.valuationDao.GetValConsumableAmount(userID, ConsumableInfo.ConsumableTypes.RegularConsumable).Select(info => info.Amount).ToList();
            if (regularForecastAmountList.Count == 0) regularForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            regularForecastAmountList = regularForecastAmountList4Actual.Concat(regularForecastAmountList).ToList();

            double regularAmountTotal = regularForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("RegularForecastAmount", InitAmountListByYear(regularForecastAmountList));
            data.Add("RegularAmountTotal", Math.Round(regularAmountTotal, 0));
            //耗材定量
            List<double> quanTityForecastAmountList = this.valuationDao.GetValConsumableAmount(userID, ConsumableInfo.ConsumableTypes.QuantitativeConsumable).Select(info => info.Amount).ToList();
            if (quanTityForecastAmountList.Count == 0) quanTityForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            quanTityForecastAmountList = quanTityForecastAmountList4Actual.Concat(quanTityForecastAmountList).ToList();

            double quanTityAmountTotal = quanTityForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("QuanTityForecastAmount", InitAmountListByYear(quanTityForecastAmountList));
            data.Add("QuanTityAmountTotal", Math.Round(quanTityAmountTotal, 0));
            //小额成本
            List<double> smallForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, Math.Round(parameterInfo.SmallConsumableCost/12.0, 2));
            smallForecastAmountList = smallForecastAmountList4Actual.Concat(smallForecastAmountList).ToList();
            double smallAmountTotal = smallForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("SmallForecastAmount", InitAmountListByYear(smallForecastAmountList));
            data.Add("SmallAmountTotal", Math.Round(smallAmountTotal, 0));
            //变动-保养 
            List<double> consumableForecastAmountList = SQLUtil.MergeList(regularForecastAmountList, quanTityForecastAmountList, smallForecastAmountList);
            double consumableAmountTotal = regularAmountTotal + quanTityAmountTotal + smallAmountTotal;
            data.Add("ConsumableForecastAmount", InitAmountListByYear(consumableForecastAmountList));
            data.Add("ConsumableAmountTotal", Math.Round(consumableAmountTotal, 0));

            //重点设备零件
            List<double> importantComponentForecastAmountList = this.valuationDao.GetValComponentAmout(userID, FujiClass2Info.LKPEquipmentType.Import).Select(info => info.Amount).ToList();
            if (importantComponentForecastAmountList.Count == 0) importantComponentForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            importantComponentForecastAmountList = importantComponentForecastAmountList4Actual.Concat(importantComponentForecastAmountList).ToList();

            double importantComponentAmountTotal = importantComponentForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("ImportantComponentForecastAmount", InitAmountListByYear(importantComponentForecastAmountList));
            data.Add("ImportantComponentAmountTotal", Math.Round(importantComponentAmountTotal, 0));
            //一般设备零件
            List<double> generalComponentForecastAmountList = this.valuationDao.GetValComponentAmout(userID, FujiClass2Info.LKPEquipmentType.General).Select(info => info.Amount).ToList();
            if (generalComponentForecastAmountList.Count == 0) generalComponentForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            generalComponentForecastAmountList = generalComponentForecastAmountList4Actual.Concat(generalComponentForecastAmountList).ToList();

            double generalComponentAmountTotal = generalComponentForecastAmountList.GetRange(startIndex, count).Sum();
            data.Add("GeneralComponentForecastAmount", InitAmountListByYear(generalComponentForecastAmountList));
            data.Add("GeneralComponentAmountTotal", Math.Round(generalComponentAmountTotal, 0));
            //零件成本
            List<double> componentForecastAmountList = this.valuationDao.GetValComponentAmout(userID, -1).Select(info => info.Amount).ToList();
            if (componentForecastAmountList.Count == 0) componentForecastAmountList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            componentForecastAmountList = componentForecastAmountList4Actual.Concat(componentForecastAmountList).ToList();

            double componentAmountTotal = importantComponentAmountTotal + generalComponentAmountTotal;

            data.Add("ComponentForecastAmount", InitAmountListByYear(componentForecastAmountList));
            data.Add("ComponentAmountTotal", Math.Round(componentAmountTotal, 0));

            //重点设备服务费
            List<double> importantRepair3partyForecastCostList = this.valuationDao.GetValEqptService(userID, FujiClass2Info.LKPEquipmentType.Import).Select(info => info.Repair3partyCost).ToList();
            if (importantRepair3partyForecastCostList.Count == 0) importantRepair3partyForecastCostList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            importantRepair3partyForecastCostList = importantRepair3partyForecastAmountList4Actual.Concat(importantRepair3partyForecastCostList).ToList();

            double importantRepair3partyCostTotal = importantRepair3partyForecastCostList.GetRange(startIndex, count).Sum();

            data.Add("ImportantRepair3partyForecastCost", InitAmountListByYear(importantRepair3partyForecastCostList));
            data.Add("ImportantRepair3partyCostTotal", Math.Round(importantRepair3partyCostTotal, 0));


            //一般设备服务费
            List<double> generalRepair3partyForecastCostList = this.valuationDao.GetValEqptService(userID, FujiClass2Info.LKPEquipmentType.General).Select(info => info.Repair3partyCost).ToList();
            if (generalRepair3partyForecastCostList.Count == 0) generalRepair3partyForecastCostList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            generalRepair3partyForecastCostList = generalRepair3partyForecastAmountList4Actual.Concat(generalRepair3partyForecastCostList).ToList();

            double generalRepair3partyCostTotal = generalRepair3partyForecastCostList.GetRange(startIndex, count).Sum();

            data.Add("GeneralRepair3partyForecastCost", InitAmountListByYear(generalRepair3partyForecastCostList));
            data.Add("GeneralRepair3partyCostTotal", Math.Round(generalRepair3partyCostTotal, 0));

            //服务费
            List<double> repair3partyForecastCostList = this.valuationDao.GetValEqptService(userID, -1).Select(info => info.Repair3partyCost).ToList();
            if (repair3partyForecastCostList.Count == 0) repair3partyForecastCostList = InitAmountList(ForecastDateList.Count - actualForecastMonth, 0);
            repair3partyForecastCostList = repair3partyForecastAmountList4Actual.Concat(repair3partyForecastCostList).ToList();

            double repair3partyCostTotal = importantRepair3partyCostTotal + generalRepair3partyCostTotal;

            data.Add("Repair3partyForecastCost", InitAmountListByYear(repair3partyForecastCostList));
            data.Add("Repair3partyCostTotal", Math.Round(repair3partyCostTotal, 0));

            //变动类-维修
            List<double> repairForecastAmountList = SQLUtil.MergeList(componentForecastAmountList, repair3partyForecastCostList);
            double repairAmountTotal = componentAmountTotal + repair3partyCostTotal;
            data.Add("RepairForecastAmount", InitAmountListByYear(repairForecastAmountList));
            data.Add("RepairAmountTotal", Math.Round(repairAmountTotal, 0));


            //总计
            List<double> amountForecastList = SQLUtil.MergeList(fixedForecastAmountList, consumableForecastAmountList, repairForecastAmountList);
            double amountTotal = fixedAmountTotal + consumableAmountTotal + repairAmountTotal;
            data.Add("AmountForecast", InitAmountListByYear(amountForecastList));
            data.Add("AmountTotal", Math.Round(amountTotal, 0));
            #endregion

            #region 最终定价表
            //导入期成本
            List<double> importAmountList = InitAmountList(ForecastDateList.Count, controlInfo.ImportCost);
            double importAmountTotal = importAmountList.GetRange(startIndex, count).Sum();
            data.Add("ImportAmount", InitAmountListByYear(importAmountList.ToList()));
            data.Add("ImportAmountTotal", importAmountTotal);

            //总成本
            List<double> totalAmountList = SQLUtil.MergeList(amountForecastList, importAmountList);
            double totalAmountTotal = amountTotal + importAmountTotal;
            data.Add("TotalAmount", InitAmountListByYear(totalAmountList));
            data.Add("TotalAmountTotal", Math.Round(totalAmountTotal, 0));

            //边际利润率
            List<double> vaRRate = InitAmountList(ForecastDateList.Count, controlInfo.ProfitMargins);
            data.Add("VaRRate", InitAmountListByYear(vaRRate));
            data.Add("VaRRates", controlInfo.ProfitMargins);

            //边际利润
            List<double> vaRProfitList = SQLUtil.GetPercentageAmount(totalAmountList, controlInfo.ProfitMargins / (100 - controlInfo.ProfitMargins) * 100.0);
            double vaRProfitTotal = vaRProfitList.GetRange(startIndex, count).Sum();
            data.Add("VaRProfit", InitAmountListByYear(vaRProfitList));
            data.Add("VaRProfitTotal", Math.Round(vaRProfitTotal, 0));

            //安全额
            List<double> safeProfitList = InitAmountList(ForecastDateList.Count, 0);//to-do
            double safeProfitTotal = 0;
            data.Add("SafeProfit", InitAmountListByYear(safeProfitList));
            data.Add("SafeProfitTotal", Math.Round(safeProfitTotal, 0));

            //报价
            List<double> priceList = SQLUtil.MergeList(totalAmountList, vaRProfitList);
            double priceTotal = priceList.GetRange(startIndex, count).Sum();
            data.Add("Price", InitAmountListByYear(priceList));
            data.Add("PriceTotal", Math.Round(priceTotal, 0));

            #endregion


            Dictionary<string,object> actual = GetValResultActualData(controlInfo.ForecastEngineer, ActualStartDate, controlInfo.ContractStartDate, new DateTime(DateTime.Now.Year,DateTime.Now.Month,1));
            data = data.Concat(actual).ToDictionary(k => k.Key, v => v.Value);
            return data;
        }
        public Dictionary<string, object> GetValResultActualData(int engineerCount, DateTime startDate, DateTime endDate, DateTime now)
        {
            ValParameterInfo parameterInfo = this.valuationDao.GetParameter();
            Dictionary<string, object> data = new Dictionary<string, object>();

            List<KeyValueInfo> ActualDateList = MonthDesc.GetValActualDateDescList(startDate, endDate);
            data.Add("ActualDate", ActualDateList);
            //信息系统使用费
            List<double> systemActualAmountList = InitAmountList(ActualDateList.Count, Math.Round(parameterInfo.SystemCost / 12, 2));
            data.Add("SystemActualAmount", InitAmountListByYear(systemActualAmountList));
            //人工费
            List<double> labourActualAmountList = InitAmountList(ActualDateList.Count, engineerCount * parameterInfo.UnitCost);
            data.Add("LabourActualAmount", InitAmountListByYear(labourActualAmountList));
            //维保费
            List<double> contractActualAmountList = new List<double>();
            //备用机
            List<double> spareActualAmountList = new List<double>();
            //耗材定期
            List<double> regularActualAmountList = new List<double>();
            //耗材定量
            List<double> quanTityActualAmountList = new List<double>();
            //小额成本
            List<double> smallActualAmountList = new List<double>();
            //重点设备零件
            List<double> importantComponentActualAmountList = new List<double>();
            //一般设备零件
            List<double> generalComponentActualAmountList = new List<double>();
            //零件成本
            List<double> componentActualAmountList = new List<double>();
            //重点设备服务费
            List<double> importantRepair3partyActualCostList = new List<double>();
            //一般设备服务费
            List<double> generalRepair3partyActualCostList = new List<double>();
            //服务费
            List<double> repair3partyActualCostList = new List<double>();

            while (startDate < endDate)
            {
                if (startDate < now)
                {
                    contractActualAmountList.Add(this.valuationDao.GetActualContractAmount(startDate));
                    spareActualAmountList.Add(this.valuationDao.GetActualSpareAmount(startDate));
                    regularActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.RegularConsumable, startDate));
                    quanTityActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.QuantitativeConsumable, startDate));
                    smallActualAmountList.Add(this.valuationDao.GetActulConsumableAmount(ConsumableInfo.ConsumableTypes.SmallCostConsumable, startDate));
                    importantComponentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                    generalComponentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                    componentActualAmountList.Add(this.valuationDao.GetActulComponentAmount(-1, startDate));
                    importantRepair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.Import, startDate));
                    generalRepair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(FujiClass2Info.LKPEquipmentType.General, startDate));
                    repair3partyActualCostList.Add(this.valuationDao.GetActulServiceAmount(-1, startDate));
                }
                else
                {
                    contractActualAmountList.Add(0);
                    spareActualAmountList.Add(0);
                    regularActualAmountList.Add(0);
                    quanTityActualAmountList.Add(0);
                    smallActualAmountList.Add(0);
                    importantComponentActualAmountList.Add(0);
                    generalComponentActualAmountList.Add(0);
                    componentActualAmountList.Add(0);
                    importantRepair3partyActualCostList.Add(0);
                    generalRepair3partyActualCostList.Add(0);
                    repair3partyActualCostList.Add(0);
                }

                startDate = startDate.AddMonths(1);
            }

            data.Add("ContractActualAmount", InitAmountListByYear(contractActualAmountList));
            data.Add("SpareActualAmount", InitAmountListByYear(spareActualAmountList));
            //固定类
            List<double> fixedActualAmountList = SQLUtil.MergeList(systemActualAmountList, labourActualAmountList, contractActualAmountList, spareActualAmountList);
            data.Add("FixedActualAmount", InitAmountListByYear(fixedActualAmountList));
            data.Add("RegularActualAmount", InitAmountListByYear(regularActualAmountList));
            data.Add("QuanTityActualAmount", InitAmountListByYear(quanTityActualAmountList));
            data.Add("SmallActualAmount", InitAmountListByYear(smallActualAmountList));
            //变动-保养 
            List<double> consumableActualAmountList = SQLUtil.MergeList(regularActualAmountList, quanTityActualAmountList, smallActualAmountList);
            data.Add("ConsumableActualAmount", InitAmountListByYear(consumableActualAmountList));
            data.Add("ImportantComponentActualAmount", InitAmountListByYear(importantComponentActualAmountList));
            data.Add("GeneralComponentActualAmount", InitAmountListByYear(generalComponentActualAmountList));
            data.Add("ComponentActualAmount", InitAmountListByYear(componentActualAmountList));
            data.Add("ImportantRepair3partyActualCost", InitAmountListByYear(importantRepair3partyActualCostList));
            data.Add("GeneralRepair3partyActualCost", InitAmountListByYear(generalRepair3partyActualCostList));
            data.Add("Repair3partyActualCost", InitAmountListByYear(repair3partyActualCostList));
            //变动类-维修
            List<double> repairActualAmountList = SQLUtil.MergeList(componentActualAmountList, repair3partyActualCostList);
            data.Add("RepairActualAmount", InitAmountListByYear(repairActualAmountList));
            //总计
            List<double> amountActualList = SQLUtil.MergeList(fixedActualAmountList, consumableActualAmountList, repairActualAmountList);
            data.Add("AmountActual", InitAmountListByYear(amountActualList));

            return data;
        }

        private List<double> InitAmountList(int length, double amount)
        {
            List<double> amountList = new List<double>();
            for (int i = 0; i < length; i++)
            {
                amountList.Add(amount);
            }

            return amountList;
        }    

        private List<object> InitAmountListByYear(List<double> list)
        {
            List<object> amountList = new List<object>();

            Dictionary<string, object> item = new Dictionary<string,object>();
            List<double> data = new List<double>();
            double sum = 0;

            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
                data.Add(list[i]);
                if ((((i + 1) % 12) == 0 && i != 0) || i == list.Count-1)
                {
                    item.Add("year", Math.Floor((double)(i / 12 + 1)));
                    item.Add("data", data);
                    item.Add("sum", Math.Round(sum,0));
                    amountList.Add(item);
                    item = new Dictionary<string, object>();
                    data = new List<double>();
                    sum = 0;
                }
            }
            return amountList;
        }
        #endregion
        #region 备用机计算工具
        /// <summary>
        /// 计算备用机数量
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="safeRate">安全率</param>
        /// <param name="valSpareList">备用机列表</param>
        /// <returns>备用机列表</returns>
        public List<ValSpareInfo> CalculateSpareCount(DateTime startDate, DateTime endDate, double safeRate, List<ValSpareInfo> valSpareList)
        {
            DataTable dtRaw = this.valuationDao.GetRepairDataByFujiClass2(startDate, endDate);
            double safeRate_InvCDF = Normal.InvCDF(0, 1, safeRate / 100.0);

            foreach(ValSpareInfo valSpareInfo in valSpareList)
            {
                CalculateSpareCount(startDate, endDate, safeRate_InvCDF, dtRaw, valSpareInfo);
            }

            return valSpareList;
        }
        /// <summary>
        /// 计算备用机数量
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="safeRate_InvCDF">安全系数</param>
        /// <param name="dtRaw">维修数据</param>
        /// <param name="valSpareInfo">备用机信息</param>
        private void CalculateSpareCount(DateTime startDate, DateTime endDate, double safeRate_InvCDF, DataTable dtRaw, ValSpareInfo valSpareInfo)
        {
            List<double> requestCounts = new List<double>();
            double totalMinutes = 0;

            DateTime curDate = startDate;
            while(curDate <= endDate)
            {
                DataRow drRow = (from DataRow temp in dtRaw.Rows where SQLUtil.ConvertInt(temp["FujiClass2ID"]) == valSpareInfo.FujiClass2.ID && SQLUtil.ConvertDateTime(temp["RequestDate"]) == curDate select temp).FirstOrDefault();
                if (drRow != null)
                {
                    requestCounts.Add(SQLUtil.ConvertDouble(drRow["RepairCount"]));
                    totalMinutes += SQLUtil.ConvertDouble(drRow["Minutes"]);
                }
                else
                    requestCounts.Add(0);

                curDate = curDate.AddDays(1);
            }

            double stdDev = Statistics.PopulationStandardDeviation(requestCounts);
            double totalCounts = Math.Round(requestCounts.Sum(),0);
            if(totalCounts != 0)
            {
                double avgRepairDays = Math.Round(totalMinutes / requestCounts.Sum() / 60.0 / 24.0, 8);

                int calCount = (int)Math.Ceiling(stdDev * safeRate_InvCDF * Math.Sqrt(valSpareInfo.AdjustRepairDays == 0 ? avgRepairDays : valSpareInfo.AdjustRepairDays));

                valSpareInfo.CalculatedCount = calCount > 0 ? calCount : 0;
            }            
        }
        /// <summary>
        /// 同步备用机
        /// </summary>
        /// <param name="infos">备用机列表</param>
        /// <returns>备用机列表</returns>
        public List<ValSpareInfo> SyncSpareCount(int userID, List<ValSpareInfo> infos)
        {
            DataTable dt = ValSpareInfo.ConvertValSpareTable(infos);
            this.valuationDao.SyncSpareCount(dt);

            return this.valuationDao.QuerySpareList(userID);
        }
        #endregion
    }
}
