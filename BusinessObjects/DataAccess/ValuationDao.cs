using BusinessObjects.Aspect;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DataAccess
{
    /// <summary>
    /// 估价Dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ValuationDao : BaseDao
    {
        string hospitalFactorPara = " (case when vp.HospitalLevel = 1 then vp.HospitalFactor1 when vp.HospitalLevel = 2 then vp.HospitalFactor2 when vp.HospitalLevel = 3 then vp.HospitalFactor3 end) ";
        string hospitalFactorVal = "(case when v.HospitalLevel = vp.HospitalLevel then {0} else (case when v.HospitalLevel =1 then vp.HospitalFactor1 when v.HospitalLevel = 2 then vp.HospitalFactor2 when v.HospitalLevel = 3 then vp.HospitalFactor3 else 0 end)* {0} end)";

        #region parameter
        /// <summary>
        /// 获取估价参数信息
        /// </summary>
        /// <returns>估价参数信息</returns>
        public ValParameterInfo GetParameter()
        {
            sqlStr = "SELECT * from tblValParameter";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                DataRow dr = GetDataRow(command);

                return new ValParameterInfo(dr);
            }
        } 
        /// <summary>
        /// 修改估价参数
        /// </summary>
        /// <param name="info">估价参数</param>
        public void UpdateParameter(ValParameterInfo info)
        {
            sqlStr = "UPDATE tblValParameter SET HospitalLevel = @HospitalLevel,HospitalFactor1 = @HospitalFactor1,HospitalFactor2 = @HospitalFactor2,HospitalFactor3 = @HospitalFactor3, SystemCost = @SystemCost, " +
                    " MonthlyHours = @MonthlyHours, UnitCost = @UnitCost, SmallConsumableCost = @SmallConsumableCost";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@HospitalLevel", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.HospitalLevel.ID);
                command.Parameters.Add("@HospitalFactor1", SqlDbType.Float).Value = info.HospitalFactor1;
                command.Parameters.Add("@HospitalFactor2", SqlDbType.Float).Value = info.HospitalFactor2;
                command.Parameters.Add("@HospitalFactor3", SqlDbType.Float).Value = info.HospitalFactor3;
                command.Parameters.Add("@SystemCost", SqlDbType.Float).Value = info.SystemCost;
                command.Parameters.Add("@MonthlyHours", SqlDbType.Float).Value = info.MonthlyHours;
                command.Parameters.Add("@UnitCost", SqlDbType.Float).Value = info.UnitCost;
                command.Parameters.Add("@SmallConsumableCost", SqlDbType.Float).Value = info.SmallConsumableCost;

                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region ValControl
        /// <summary>
        /// 获取估价前提条件
        /// </summary>
        /// <returns>估价前提条件信息</returns>
        public ValControlInfo GetControl(int userID)
        {
              sqlStr = "SELECT vc.* ,vp.HospitalFactor1,vp.HospitalFactor2,vp.HospitalFactor3 " +
                    " FROM tblValControl vc " +
                    " LEFT JOIN tblValParameter vp ON 1=1" + 
                    " WHERE UserID = @UserID "; 

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                DataRow dr = GetDataRow(command);
                if (dr == null) return null;
                else return new ValControlInfo(dr);
            }
        }
        /// <summary>
        /// 预测所需总工时数
        /// </summary>
        /// <returns>预测所需总工时数</returns>
        public double GetTotalHours(int userID)
        {
            sqlStr = "SELECT SUM(PatrolHours + MaintenanceHours + RepairHours) " +
                    " FROM tblValEquipment " +
                    " WHERE UserID = @UserID "; 

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetDouble(command);
            }
        }
        /// <summary>
        /// 保存估价前提条件信息
        /// </summary>
        /// <param name="info">估价前提条件信息</param>
        public void AddControl(ValControlInfo info)
        {
            sqlStr = "INSERT INTO tblValControl(CtlFlag,UserID,UpdateDate,IsExecuted,EndDate,ContractStartDate,Years,HospitalLevel,ImportCost,ProfitMargins, " +
                    " RiskRatio,VarAmount,ComputeEngineer,ForecastEngineer) " +
                    " VALUES('CTL',@UserID,GetDate(),@IsExecuted, @EndDate,@ContractStartDate,@Years,@HospitalLevel,@ImportCost,@ProfitMargins, " +
                    " @RiskRatio,@VarAmount,@ComputeEngineer,@ForecastEngineer)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = info.User.ID;
                command.Parameters.Add("@IsExecuted", SqlDbType.Bit).Value = info.IsExecuted;

                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = info.EndDate;
                command.Parameters.Add("@ContractStartDate", SqlDbType.DateTime).Value = info.ContractStartDate;
                command.Parameters.Add("@Years", SqlDbType.Int).Value = info.Years;
                command.Parameters.Add("@HospitalLevel", SqlDbType.Int).Value = info.HospitalLevel.ID;
                command.Parameters.Add("@ImportCost", SqlDbType.Decimal).Value = info.ImportCost;
                command.Parameters.Add("@ProfitMargins", SqlDbType.Decimal).Value = info.ProfitMargins;
                command.Parameters.Add("@RiskRatio", SqlDbType.Decimal).Value = info.RiskRatio;
                command.Parameters.Add("@VarAmount", SqlDbType.Decimal).Value = info.VarAmount;
                command.Parameters.Add("@ComputeEngineer", SqlDbType.Int).Value = info.ComputeEngineer;
                command.Parameters.Add("@ForecastEngineer", SqlDbType.Int).Value = info.ForecastEngineer;

                command.ExecuteScalar();
            }
        }

        public void UpdateControl(ValControlInfo info)
        {
            sqlStr = "UPDATE tblValControl SET UpdateDate = GETDATE(),IsExecuted = @IsExecuted,EndDate = @EndDate,ContractStartDate = @ContractStartDate,Years = @Years,HospitalLevel = @HospitalLevel,ImportCost = @ImportCost,ProfitMargins = @ProfitMargins,RiskRatio = @RiskRatio,VarAmount = @VarAmount,ComputeEngineer = @ComputeEngineer,ForecastEngineer = @ForecastEngineer " +
                " WHERE UserID = @UserID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = info.User.ID;
                command.Parameters.Add("@IsExecuted", SqlDbType.Bit).Value = false;

                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = info.EndDate;
                command.Parameters.Add("@ContractStartDate", SqlDbType.DateTime).Value = info.ContractStartDate;
                command.Parameters.Add("@Years", SqlDbType.Int).Value = info.Years;
                command.Parameters.Add("@HospitalLevel", SqlDbType.Int).Value = info.HospitalLevel.ID;
                command.Parameters.Add("@ImportCost", SqlDbType.Decimal).Value = info.ImportCost;
                command.Parameters.Add("@ProfitMargins", SqlDbType.Decimal).Value = info.ProfitMargins;
                command.Parameters.Add("@RiskRatio", SqlDbType.Decimal).Value = info.RiskRatio;
                command.Parameters.Add("@VarAmount", SqlDbType.Decimal).Value = info.VarAmount;
                command.Parameters.Add("@ComputeEngineer", SqlDbType.Int).Value = info.ComputeEngineer;
                command.Parameters.Add("@ForecastEngineer", SqlDbType.Int).Value = info.ForecastEngineer;

                command.ExecuteScalar();
            }
        }

        public void UpdateControlStatus(int userID)
        {
            sqlStr = "UPDATE tblValControl SET UpdateDate = GETDATE(),IsExecuted = @IsExecuted " +
                    " WHERE UserID = @UserID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.Parameters.Add("@IsExecuted", SqlDbType.Bit).Value = true;

                command.ExecuteScalar();
            }
        }
        /// <summary>
        /// 删除估价前提条件信息
        /// </summary>
        public void DeleteControl(int userID)
        {
            sqlStr = "DELETE tblValControl" +
                    " WHERE UserID = @UserID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region tblValEquipment
        /// <summary>
        /// 初始化估价设备清单
        /// </summary>
        public void InitEquipments(int userID)
        {
            sqlStr = "INSERT INTO tblValEquipment (UserID,InSystem,EquipmentID, AssetCode,Name,SerialCode,Manufacturer,FujiClass2ID,Department,PurchaseAmount,CurrentScopeID,EndDate,NextScopeID)" +
                    " SELECT @UserID as UserID,1 AS InSystem,e.ID EquipmentID,e.AssetCode,e.Name,e.SerialCode,s.Name Manufacturer,ISNULL(e.FujiClass2ID,0) AS FujiClass2ID,d.Description Department,e.PurchaseAmount, " +
                    " ISNULL(c.ScopeID,@ScopeTypes) CurrentScopeID,c.EndDate, @ScopeTypes " +
                    " FROM tblEquipment e " +
                    " LEFT JOIN v_ActiveContract ac ON e.ID = ac.EquipmentID " +
                    " INNER JOIN tblFujiClass2 f2 ON e.FujiClass2ID = f2.ID " +
                    " LEFT JOIN tblContract c ON c.ID = ac.ContractID " +
                    " LEFT JOIN lkpDepartment d ON d.ID = e.DepartmentID " +
                    " LEFT JOIN tblSupplier s ON e.ManufacturerID = s.ID " +
                    " WHERE e.EquipmentStatusID <> " + EquipmentInfo.EquipmentStatuses.Scrap;

            using(SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.Parameters.Add("@ScopeTypes", SqlDbType.Int).Value = ValEquipmentInfo.ScopeTypes.NoneCoverage;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取估价设备清单数量
        /// </summary>
        /// <param name="equipmentType">设备类型</param>
        /// <param name="amountType">金额类型</param>
        /// <param name="equipmentName">设备名称</param>
        /// <param name="serialCode">设备序列号</param>
        /// <param name="departmentName">科室</param>
        /// <param name="class1ID">富士I类</param>
        /// <param name="class2ID">富士II类</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>设备数量</returns>
        public int GetEquipmentCount(int userID, int equipmentType, int amountType, string equipmentName, string serialCode, string departmentName, int class1ID, int class2ID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(ve.EquipmentID) FROM tblValEquipment ve " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " WHERE ve.UserID = @UserID ";
            if (equipmentType != -1)
                sqlStr += " AND f2.EquipmentType = " + equipmentType;
            if (amountType != -1)
                sqlStr += UIFilterEquipmentAmount.GetAmountSql(amountType);
            if (!string.IsNullOrEmpty(equipmentName))
                sqlStr += " AND UPPER(ve.Name) Like @EquipmentName ";
            if (!string.IsNullOrEmpty(serialCode))
                sqlStr += " AND UPPER(ve.SerialCode) Like @SerialCode ";
            if (!string.IsNullOrEmpty(departmentName))
                sqlStr += " AND UPPER(ve.Department) Like @Department ";
            if (class1ID != 0)
                sqlStr += " AND f2.FujiClass1ID = " + class1ID;
            if (class2ID != 0)
                sqlStr += "AND f2.ID = " + class2ID;

            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (!string.IsNullOrEmpty(equipmentName))
                    command.Parameters.Add("@EquipmentName", SqlDbType.NVarChar).Value = "%" + equipmentName.ToUpper() + "%";
                if (!string.IsNullOrEmpty(serialCode))
                    command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = "%" + serialCode.ToUpper() + "%";
                if (!string.IsNullOrEmpty(departmentName))
                    command.Parameters.Add("@Department", SqlDbType.NVarChar).Value = "%" + departmentName.ToUpper() + "%";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取估价设备清单信息
        /// </summary>
        /// <param name="equipmentType">设备类型</param>
        /// <param name="amountType">金额类型</param>
        /// <param name="equipmentName">设备名称</param>
        /// <param name="serialCode">设备序列号</param>
        /// <param name="departmentName">科室</param>
        /// <param name="class1ID">富士I类</param>
        /// <param name="class2ID">富士II类</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当页第一条数据下标</param>
        /// <param name="pageSize">数据条数</param>
        /// <returns>设备清单信息 </returns>
        public List<ValEquipmentInfo> QueryEquipmentList(int userID, int equipmentType, int amountType, string equipmentName, string serialCode, string departmentName, int class1ID, int class2ID, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            sqlStr = "SELECT ve.*,f2.Name as FujiClass2Name, f2.EquipmentType EquipmentTypeID, f1.ID FujiClass1ID,f1.Name as FujiClass1Name,e.UseageDate,u.Name as UserName " + 
                        " FROM tblValEquipment ve " +
                        " LEFT JOIN tblEquipment e ON e.ID = ve.EquipmentID AND ve.InSystem = 1 " +
                        " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                        " INNER JOIN tblFujiClass1 f1 ON f1.ID = f2.FujiClass1ID " +
                        " LEFT JOIN tblUser u ON u.ID = @UserID " +
                        " WHERE UserID = @UserID ";
                if (equipmentType != -1)
                    sqlStr += " AND f2.EquipmentType = " + equipmentType;
                if (amountType != -1)
                    sqlStr += UIFilterEquipmentAmount.GetAmountSql(amountType);
                if (!string.IsNullOrEmpty(equipmentName))
                    sqlStr += " AND UPPER(ve.Name) Like @EquipmentName ";
                if (!string.IsNullOrEmpty(serialCode))
                    sqlStr += " AND UPPER(ve.SerialCode) Like @SerialCode ";
                if (!string.IsNullOrEmpty(departmentName))
                    sqlStr += " AND UPPER(ve.Department) Like @Department ";
                if (class1ID != 0)
                    sqlStr += " AND f2.FujiClass1ID = " + class1ID;
                if (class2ID != 0)
                    sqlStr += "AND f2.ID = " + class2ID;

                if (!string.IsNullOrEmpty(filterText))
                    sqlStr += GetFieldFilterClause(filterField);

                sqlStr += GenerateSortClause(sortDirection, sortField, "f2.ID");
                sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                if (!string.IsNullOrEmpty(equipmentName))
                    command.Parameters.Add("@EquipmentName", SqlDbType.NVarChar).Value = "%" + equipmentName.ToUpper() + "%";
                if (!string.IsNullOrEmpty(serialCode))
                    command.Parameters.Add("@SerialCode", SqlDbType.NVarChar).Value = "%" + serialCode.ToUpper() + "%";
                if (!string.IsNullOrEmpty(departmentName))
                    command.Parameters.Add("@Department", SqlDbType.NVarChar).Value = "%" + departmentName.ToUpper() + "%";

                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                return GetList<ValEquipmentInfo>(command); 
            }
        }

        /// <summary>
        /// 获取估价设备清单信息
        /// </summary>
        /// <param name="userID">用户ID</param> 
        /// <returns>设备清单信息 </returns>
        public List<ValEquipmentInfo> GetEquipmentList(int userID)
        {
            sqlStr = "SELECT ve.*,f2.Name as FujiClass2Name, f2.EquipmentType EquipmentTypeID,f2.FullCoveragePtg, f2.TechCoveragePtg, f1.ID FujiClass1ID,f1.Name as FujiClass1Name,e.UseageDate,u.Name as UserName " +
                        " FROM tblValEquipment ve " +
                        " LEFT JOIN tblEquipment e ON e.ID = ve.EquipmentID AND ve.InSystem = 1 " +
                        " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                        " INNER JOIN tblFujiClass1 f1 ON f1.ID = f2.FujiClass1ID " +
                        " LEFT JOIN tblUser u ON u.ID = @UserID " +
                        " WHERE UserID = @UserID ";

            sqlStr += GenerateSortClause(true, "f2.ID", "f2.ID"); 

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {  
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                return GetList<ValEquipmentInfo>(command);
            }
        }

        /// <summary>
        /// 获取设备清单中在系统的设备id
        /// </summary>
        /// <returns>设备清单中在系统的设备id</returns>
        public List<int> GetInSystemEquipmentID(int userID)
        {
            sqlStr = "SELECT EquipmentID FROM tblValEquipment " +
                    " WHERE InSystem = 1 AND UserID = @UserID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList(command);
            }
        }

        /// <summary>
        /// 批量保存设备信息
        /// </summary>
        /// <param name="dt">设备信息列表</param>
        public void UpdateEquipments(DataTable dt)
        {
            sqlStr = "UPDATE tblValEquipment SET CurrentScopeID = @CurrentScopeID, EndDate = @EndDate, NextScopeID = @NextScopeID " +
                     " WHERE EquipmentID = @EquipmentID AND InSystem = @InSystem AND UserID = @UserID ";

            SqlParameter parameter = null;
            using (SqlCommand updateCommand = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = updateCommand.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = updateCommand.Parameters.Add("@EquipmentID", SqlDbType.Int);
                parameter.SourceColumn = "EquipmentID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = updateCommand.Parameters.Add("@CurrentScopeID", SqlDbType.Int);
                parameter.SourceColumn = "CurrentScopeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = updateCommand.Parameters.Add("@NextScopeID", SqlDbType.Int);
                parameter.SourceColumn = "NextScopeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = updateCommand.Parameters.Add("@EndDate", SqlDbType.DateTime);
                parameter.SourceColumn = "EndDate";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = updateCommand.Parameters.Add("@InSystem", SqlDbType.Bit);
                parameter.SourceColumn = "InSystem";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = updateCommand;

                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 导入不在系统中的设备
        /// </summary>
        /// <param name="dt">设备信息</param>
        public void ImportEquipments(DataTable dt)
        {
            sqlStr = "INSERT INTO tblValEquipment(UserID,InSystem,EquipmentID,AssetCode,Name,SerialCode,Manufacturer,FujiClass2ID,Department,PurchaseAmount,CurrentScopeID,EndDate,NextScopeID) " +
                " VALUES(@UserID,0,@EquipmentID,@AssetCode,@Name,@SerialCode,@Manufacturer,@FujiClass2ID,@Department,@PurchaseAmount,@CurrentScopeID,@EndDate,@NextScopeID) ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {

                parameter = command.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@EquipmentID", SqlDbType.Int);
                parameter.SourceColumn = "EquipmentID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@AssetCode", SqlDbType.NVarChar);
                parameter.SourceColumn = "AssetCode";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Name", SqlDbType.NVarChar);
                parameter.SourceColumn = "Name";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@SerialCode", SqlDbType.NVarChar);
                parameter.SourceColumn = "SerialCode";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Manufacturer", SqlDbType.NVarChar);
                parameter.SourceColumn = "Manufacturer";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@FujiClass2ID", SqlDbType.Int);
                parameter.SourceColumn = "FujiClass2ID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Department", SqlDbType.NVarChar);
                parameter.SourceColumn = "Department";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@PurchaseAmount", SqlDbType.Decimal);
                parameter.SourceColumn = "PurchaseAmount";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@CurrentScopeID", SqlDbType.Int);
                parameter.SourceColumn = "CurrentScopeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@NextScopeID", SqlDbType.Int);
                parameter.SourceColumn = "NextScopeID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@EndDate", SqlDbType.DateTime);
                parameter.SourceColumn = "EndDate";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 删除设备信息
        /// </summary>
        /// <param name="notInSystemOnly">是否仅删除不在系统中的设备</param>
        public void DeleteEquipments(int userID, bool notInSystemOnly = false)
        {
            sqlStr = "DELETE tblValEquipment WHERE UserID = @UserID ";
            if (notInSystemOnly) sqlStr += " AND InSystem = 0";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 修改设备工时
        /// </summary>
        public void UpdateEquipmentHours(int userID)
        {
            sqlStr = @"UPDATE e set e.MaintenanceHours = f.MaintenanceHours * f.MaintenanceTimes / 12.0,
                        e.PatrolHours = f.PatrolHours * f.PatrolTimes / 12.0, e.RepairHours = f.RepairHours
                        FROM tblValEquipment as e 
                        INNER JOIN tblFujiClass2 as f on e.FujiClass2ID = f.ID AND f.IncludeLabour=1
                        WHERE e.UserID = @UserID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region tblValConsumable
        /// <summary>
        /// 初始化耗材
        /// </summary>
        public void InitConsumables(int userID)
        {
            sqlStr = "INSERT INTO tblValConsumable(ConsumableID,UserID,IncludeContract) " +
                    " SELECT Distinct(c.ID),@UserID as UserID,c.IncludeContract " +
                    " FROM tblValEquipment ve " +
                    " INNER JOIN tblConsumable c ON ve.FujiClass2ID = c.FujiClass2ID " +
                    " WHERE c.ID NOT IN (SELECT ConsumableID FROM tblValConsumable WHERE UserID = @UserID ) AND (c.IsIncluded = 1 AND c.IsActive=1 ) AND ve.UserID = @UserID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取耗材列表数量
        /// </summary>
        /// <param name="class2ID">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>耗材数量</returns>
        public int GetConsumableCount(int userID, int class2ID, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(c.ID) FROM tblValConsumable vc " +
                    " INNER JOIN tblConsumable c ON vc.ConsumableID = c.ID " +
                    " INNER JOIN tblFujiClass2 f2 ON c.FujiClass2ID = f2.ID " +
                    " WHERE 1=1 AND vc.UserID = @UserID ";

            if (class2ID != 0)
                sqlStr += " AND f2.ID = " + class2ID+" ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);
            
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取耗材列表信息
        /// </summary>
        /// <param name="class2ID">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="curRowNum">当页第一条数据的下标</param>
        /// <param name="pageSize">数据条数</param>
        /// <returns>耗材列表信息</returns>
        public List<ValConsumableInfo> QueryConsumableList(int userID, int class2ID, string filterField, string filterText, int curRowNum = 0, int pageSize = 0)
        {
            sqlStr = "SELECT vc.*,c.Name ConsumableName,f2.ID FujiClass2ID,f2.Name FujiClass2Name,c.CostPer,c.ReplaceTimes,u.Name as UserName " +
                    " FROM tblValConsumable vc" +
                    " INNER JOIN tblConsumable c ON vc.ConsumableID = c.ID " +
                    " INNER JOIN tblFujiClass2 f2 ON c.FujiClass2ID = f2.ID " +
                    " LEFT JOIN tblUser u ON u.ID = vc.UserID " +
                    " WHERE vc.UserID = @UserID ";

            if (class2ID != 0)
                sqlStr += " AND f2.ID = " + class2ID+" ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += " ORDER BY f2.ID ";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                if (!String.IsNullOrEmpty(filterText))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetList<ValConsumableInfo>(command);
            }
        }
        /// <summary>
        /// 修改耗材信息
        /// </summary>
        /// <param name="dt">修改的耗材列表</param>
        public void UpdateConsumables(DataTable dt)
        {
            sqlStr = "UPDATE tblValConsumable SET IncludeContract = @IncludeContract " +
                    " WHERE ConsumableID = @ConsumableID AND UserID = @UserID ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@ConsumableID", SqlDbType.Int);
                parameter.SourceColumn = "ConsumableID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@IncludeContract", SqlDbType.Bit);
                parameter.SourceColumn = "IncludeContract";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 删除耗材
        /// </summary>
        /// <param name="notInSystemOnly">是否只删除不在系统中的耗材</param>
        public void DeleteConsumables(int userID, bool notInSystemOnly = false)
        {
            sqlStr = "DELETE tblValConsumable WHERE UserID = @UserID ";
            if (notInSystemOnly)
                sqlStr += " AND ConsumableID NOT IN (SELECT DISTINCT(c.ID) FROM tblValEquipment ve " +
                        " INNER JOIN tblConsumable c ON ve.FujiClass2ID = c.FujiClass2ID) ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        #endregion
        
        #region tblValComponent
        /// <summary>
        /// 初始化零件信息
        /// </summary>
        public void InitComponents(int userID)
        {
            sqlStr = "INSERT INTO tblValComponent(UserID,InSystem,EquipmentID,ComponentID,Qty,Usage,Seconds,IncludeContract, UsageRefere) " +
                    " (SELECT @UserID as UserID,ve.InSystem,ve.EquipmentID,c.ID as ComponentID,1 as Qty, c.Usage * {0} as Usage, " +
                    " (case when c.TypeID <> @CT OR ve.InSystem = 0 then 0 else (case when r.CloseDate is null then DATEDIFF(mm,ISNULL(e.UseageDate,e.InstalDate),GETDATE()) * c.SecondsPer*c.Usage* {0} + e.CTUsedSeconds else DATEDIFF(mm,r.CloseDate, GETDATE()) * c.SecondsPer*c.Usage* {0} END ) END) as Seconds, " +
                    " c.IncludeContract, c.Usage * {0} as UsageRefere " +
                    " FROM tblValEquipment ve " +
                    " LEFT JOIN tblEquipment e ON e.ID = ve.EquipmentID AND ve.InSystem = 1" +
                    " INNER JOIN tblComponent c ON ve.FujiClass2ID = c.FujiClass2ID " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " INNER JOIN tblValControl v ON v.UserID = ve.UserID " +
                    " LEFT JOIN tblValParameter vp ON 1=1 " +
                    " LEFT JOIN tblReportComponent rc ON c.ID = rc.ComponentID " +
                    " LEFT JOIN tblDispatchReport dr ON dr.ID = rc.DispatchReportID " +
                    " LEFT JOIN tblDispatch d ON d.ID = dr.ID " +
                    " LEFT JOIN tblRequest r ON d.RequestID = r.ID " +
                    " WHERE 1=1 AND f2.IncludeRepair = 1 and c.IsActive = 1 AND c.IsIncluded =1 AND ve.UserID = @UserID " +
                    " AND (ve.EquipmentID NOT IN (SELECT EquipmentID FROM tblValComponent WHERE UserID = @UserID) OR ve.InSystem = 0 ) " +
                    " ) UNION ALL " +
                    " (SELECT @UserID as UserID, ve.InSystem,ve.EquipmentID,0,1,f2.Usage * {0},0,0,f2.Usage * {0} " +
                    " FROM tblValEquipment ve " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " INNER JOIN tblValControl v ON v.UserID = ve.UserID " +
                    " LEFT JOIN tblValParameter vp ON 1=1 " +
                    " WHERE f2.IncludeRepair = 1 AND ve.UserID = @UserID AND (ve.EquipmentID NOT IN (SELECT EquipmentID FROM tblValComponent WHERE UserID = @UserID) OR ve.InSystem = 0))";

            sqlStr = string.Format(sqlStr, hospitalFactorVal);
            sqlStr = string.Format(sqlStr, hospitalFactorPara);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.Parameters.Add("@CT", SqlDbType.Int).Value = ComponentInfo.ComponentTypes.CT;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取零件列表数据总数
        /// </summary>
        /// <param name="componentType">零件类型</param>
        /// <param name="equipmentType">设备类别</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>零件列表数据总数</returns>
        public int GetComponentCount(int userID, int componentType, int equipmentType, string filterField, string filterText)
        {
            sqlStr = "SELECT COUNT(vc.ComponentID) " +
                    " FROM tblValComponent vc " +
                    " LEFT JOIN tblComponent c ON vc.ComponentID = c.ID " +
                    " INNER JOIN tblValEquipment ve ON vc.EquipmentID = ve.EquipmentID AND vc.UserID = ve.UserID " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " WHERE f2.IncludeRepair = 1  AND ve.InSystem = vc.InSystem AND vc.UserID = @UserID ";
            if (componentType == ComponentInfo.ComponentTypes.CT)
                sqlStr += " AND (c.TypeID = " + componentType +") ";
            else
                sqlStr += " AND ((c.TypeID <> " + ComponentInfo.ComponentTypes.CT +
                          ") OR vc.ComponentID = 0) ";

            if (equipmentType != 0)
                sqlStr += " AND f2.EquipmentType = " + equipmentType;

            if (!string.IsNullOrEmpty(filterText))
            {
                if ("c.Name".Equals(filterField) && ValComponentInfo.ComponentType.WholeMachine.Equals(filterText))
                    sqlStr += " AND vc.ComponentID=0 ";
                else
                    sqlStr += GetFieldFilterClause(filterField);
            }

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                if (!String.IsNullOrEmpty(filterText) && !("c.Name".Equals(filterField) && ValComponentInfo.ComponentType.WholeMachine.Equals(filterText)))
                    AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取零件列表数据
        /// </summary>
        /// <param name="componentType">零件类型</param>
        /// <param name="equipmentType">设备类别</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="wholeMachineonly">是否只查询整机</param>
        /// <param name="curRowNum">当页第一条数据的下标</param>
        /// <param name="pageSize">数据条数</param>
        /// <returns>零件列表数据</returns>
        public List<ValComponentInfo> QueryComponentList(int userID, int componentType, int equipmentType, string filterField, string filterText, bool wholeMachineonly = false, int curRowNum = 0, int pageSize = 0)
        {
            sqlStr = "SELECT vc.*,ve.*, c.Name as ComponentName,c.Usage as ComponentUsage, f2.Name as FujiClass2Name,f2.EquipmentType,c.TypeID,c.SecondsPer,c.TotalSeconds,u.Name as UserName, " +
                    " (case when c.TypeID <> @CT OR ve.InSystem = 0 then 0 else (case when r.CloseDate is null then DATEDIFF(mm,ISNULL(e.UseageDate,e.InstalDate),GETDATE()) * c.SecondsPer*c.Usage* {0} + e.CTUsedSeconds else DATEDIFF(mm,r.CloseDate, GETDATE()) * c.SecondsPer*c.Usage* {0} END ) END) as UsedSeconds, " +
                    " ISNULL(mh.Price,c.StdPrice) as StdPrice, ISNULL(mh.UsedDate,e.UseageDate) UsageDate" +
                    " FROM tblValComponent vc " +
                    " LEFT JOIN tblComponent c ON vc.ComponentID = c.ID " +
                    " INNER JOIN tblValEquipment ve ON vc.EquipmentID = ve.EquipmentID AND vc.UserID = ve.UserID " +
                    " LEFT JOIN tblEquipment e ON e.ID = ve.EquipmentID  AND ve.InSystem = 1 " +
                    " LEFT JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " LEFT JOIN tblReportComponent rc on c.ID = rc.ComponentID " +
                    " LEFT JOIN tblDispatchReport dr on dr.ID = rc.DispatchReportID " +
                    " LEFT JOIN tblDispatch  d on d.ID = dr.ID " +
                    " LEFT JOIN tblRequest r on d.RequestID = r.ID " +
                    " INNER JOIN tblValControl v ON v.UserID = vc.UserID " +
                    " LEFT JOIN tblValParameter vp ON 1=1 " + 
                    " LEFT JOIN tblUser u ON u.ID = vc.UserID " +
                    " LEFT JOIN (select a.*,ic.ID,ic.Price from (select a.EquipmentID,a.ComponentID,MAX(a.UsedDate) as UsedDate from (select top 1000 mh.EquipmentID,ic.ComponentID,mh.UsedDate from tblMaterialHistory mh LEFT JOIN tblInvComponent ic on ic.ID = mh.ObjectID WHERE mh.ObjectType = 1 ORDER BY mh.UsedDate DESC) as a GROUP by a.EquipmentID,a.ComponentID) a " +
                    " INNER JOIN tblMaterialHistory mh ON mh.EquipmentID = a.EquipmentID and mh.ObjectType = 1 AND mh.UsedDate = a.UsedDate " +
                    " INNER JOIN tblInvComponent ic ON ic.ComponentID = a.ComponentID and ic.ID = mh.ObjectID) mh ON mh.EquipmentID = ve.EquipmentID AND mh.ComponentID = vc.ComponentID AND ve.InSystem = 1 " +
                    " WHERE ve.InSystem = vc.InSystem AND vc.UserID = @UserID";
            if (componentType == ComponentInfo.ComponentTypes.CT)
                sqlStr += " AND (c.TypeID = " + componentType + ") ";
            else if (componentType == 0)
                sqlStr += " AND ((c.TypeID <> @CT) OR vc.ComponentID = 0) ";

            if (equipmentType != 0)
                sqlStr += " AND f2.EquipmentType = " + equipmentType +" ";

            if (!string.IsNullOrEmpty(filterText))
            {
                if ("c.Name".Equals(filterField) && ValComponentInfo.ComponentType.WholeMachine.Equals(filterText))
                    sqlStr += " AND vc.ComponentID=0 ";
                else
                    sqlStr += GetFieldFilterClause(filterField);
            }
            if (wholeMachineonly)
                sqlStr += " AND vc.ComponentID = 0";

            sqlStr += " ORDER BY f2.ID ASC,ve.EquipmentID,ve.AssetCode ";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            sqlStr = string.Format(sqlStr, hospitalFactorVal);
            sqlStr = string.Format(sqlStr, hospitalFactorPara);
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText) && !("c.Name".Equals(filterField) && ValComponentInfo.ComponentType.WholeMachine.Equals(filterText)))
                    AddFieldFilterParam(command, filterField, filterText);

                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.Parameters.Add("@CT", SqlDbType.Int).Value = ComponentInfo.ComponentTypes.CT;

                return GetList<ValComponentInfo>(command);
            }
        }
        /// <summary>
        /// 修改零件信息
        /// </summary>
        /// <param name="dt">修改的零件信息</param>
        public void UpdateComponents(DataTable dt)
        {
            sqlStr = "UPDATE tblValComponent SET Qty = @Qty,Usage = @Usage, Seconds = @Seconds, IncludeContract = @IncludeContract " +
                    " WHERE ComponentID = @ComponentID AND EquipmentID = @EquipmentID AND InSystem = @InSystem AND UserID = @UserID";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@EquipmentID", SqlDbType.Int);
                parameter.SourceColumn = "EquipmentID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@ComponentID", SqlDbType.Int);
                parameter.SourceColumn = "ComponentID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Qty", SqlDbType.Int);
                parameter.SourceColumn = "Qty";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Usage", SqlDbType.Int);
                parameter.SourceColumn = "Usage";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Seconds", SqlDbType.Decimal);
                parameter.SourceColumn = "Seconds";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@IncludeContract", SqlDbType.Bit);
                parameter.SourceColumn = "IncludeContract";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@InSystem", SqlDbType.Bit);
                parameter.SourceColumn = "InSystem";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 更新零件使用量
        /// </summary>
        public void UpdateComponentUsage(int userID)
        {
            sqlStr = " UPDATE tblValComponent SET Usage =(CASE WHEN vc.ComponentID = 0 THEN f2.Usage * con.HospitalFactor ELSE  c.Usage * con.HospitalFactor END) " +
                    " FROM tblValComponent vc " +
                    " LEFT JOIN tblComponent c ON vc.ComponentID = c.ID " +
                    " INNER JOIN tblValEquipment ve ON vc.EquipmentID = ve.EquipmentID AND vc.UserID = ve.UserID " +
                    " LEFT JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " INNER JOIN tblValControl con ON 1=1 " +
                    " WHERE vc.UserID = @UserID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除零件信息
        /// </summary>
        /// <param name="notInSystemOnly">是否仅删除不在系统中的零件</param>
        public void DeleteComponents(int userID, bool notInSystemOnly = false)
        {
            sqlStr = "DELETE tblValComponent WHERE UserID = @UserID ";
            if (notInSystemOnly)
                sqlStr += " AND InSystem = 0 ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }
        
        #endregion
        
        #region tblValSpare
        /// <summary>
        /// 初始化备用机
        /// </summary>
        public void InitSpares(int userID)
        {
            sqlStr = "INSERT tblValSpare(FujiClass2ID,Price,QtyEnter,QtyEval,UserID)" +
                    " SELECT Distinct(ve.FujiClass2ID),ISNULL(f2.SparePrice*0.01*f2.SpareRentPtg,0),0, 0,@UserID as UserID " +
                    " FROM tblValEquipment ve " + 
                    " INNER JOIN tblEquipment e ON ve.EquipmentID = e.ID " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " WHERE ve.FujiClass2ID NOT IN (SELECT FujiClass2ID FROM tblValSpare WHERE UserID = @UserID) AND f2.IncludeSpare = 1 AND ve.UserID = @UserID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取备用机数据总数
        /// </summary>
        /// <returns>备用机列表数据总数</returns>
        public int GetSpareCount(int userID)
        {
            sqlStr = "SELECT COUNT(vs.FujiClass2ID) " +
                    " FROM tblValSpare vs " +
                    " LEFT JOIN tblFujiClass2 f2 ON vs.FujiClass2ID = f2.ID " +
                    " WHERE vs.UserID = @UserID " ;

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetCount(command);
            }
        }
        /// <summary>
        /// 获取备用机列表信息
        /// </summary>
        /// <param name="curRowNum">当页第一条数据下标</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>备用机列表信息</returns>
        public List<ValSpareInfo> QuerySpareList(int userID, int curRowNum = 0, int pageSize = 0)
        {
            sqlStr = @"SELECT vs.*,f2.Name FujiClass2Name,u.Name as UserName FROM tblValSpare vs 
                      LEFT JOIN tblFujiClass2 f2 ON vs.FujiClass2ID = f2.ID 
                      LEFT JOIN tblUser u ON u.ID = vs.UserID
                      WHERE vs.UserID = @UserID ";

            sqlStr += " ORDER BY vs.FujiClass2ID ";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValSpareInfo>(command);
            }
        }
        /// <summary>
        /// 批量修改备用机信息
        /// </summary>
        /// <param name="dt">备用机列表</param>
        public void UpdateSpares(DataTable dt)
        {
            sqlStr = "UPDATE tblValSpare SET Price = @Price,QtyEnter = @QtyEnter,QtyEval = @QtyEval " +
                    " WHERE FujiClass2ID = @FujiClass2ID AND UserID = @UserID ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@UserID", SqlDbType.Int);
                parameter.SourceColumn = "UserID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@FujiClass2ID", SqlDbType.Int);
                parameter.SourceColumn = "FujiClass2ID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@Price", SqlDbType.Decimal);
                parameter.SourceColumn = "Price";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@QtyEnter", SqlDbType.Int);
                parameter.SourceColumn = "QtyEnter";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@QtyEval", SqlDbType.Int);
                parameter.SourceColumn = "QtyEval";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 删除备用机信息
        /// </summary>
        /// <param name="notInSystemOnly">是否仅删除不在系统中的备用机</param>
        public void DeleteSpares(int userID, bool notInSystemOnly = false)
        {
            sqlStr = "DELETE tblValSpare WHERE UserID = @UserID ";
            if (notInSystemOnly)
                sqlStr += " AND FujiClass2ID NOT IN (SELECT FujiClass2ID FROM tblValEquipment) ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region 成本明细表
        /// <summary>
        /// 获取预测备用机成本
        /// </summary>
        /// <returns>备用机成本</returns>
        public double GetSpareAmount(int userID)
        {
            sqlStr = "SELECT sum(Price*QtyEnter) " +
                    " FROM tblValSpare WHERE UserID = @UserID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetDouble(command);
            }
        }

        #region 维保费
        /// <summary>
        /// 获取维保费成本
        /// </summary>
        /// <returns>维保费成本</returns>
        public List<ValEqptOutputInfo> GetValEqptContractAmount(int userID)
        {
            sqlStr = "SELECT eo.Year,eo.Month,SUM(eo.ContractAmount) ContractAmount " +
                    " FROM tblValEqptOutput eo " +
                    " WHERE eo.UserID = @UserID " +
                    " GROUP BY eo.Year,eo.Month " +
                    " ORDER BY eo.Year,eo.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValEqptOutputInfo>(command);
            }
        }

        /// <summary>
        /// 获取维保费成本
        /// </summary>
        /// <returns>维保费成本</returns>
        public List<ValEqptOutputInfo> GetValEqptMaintenanceCost(int userID)
        {
            sqlStr = @"SELECT MAX(eo.EquipmentID) as EquipmentID,eo.Year,eo.Month,SUM(eo.ContractAmount) ContractAmount,0 Repair3partyCost,eo.UserID, u.Name as UserName 
                     FROM tblValEqptOutput eo 
                     LEFT JOIN tblUser u ON eo.UserID = u.ID 
                     WHERE eo.UserID = @UserID 
                     GROUP BY eo.Year,eo.Month,eo.UserID,u.Name 
                     ORDER BY eo.Year,eo.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValEqptOutputInfo>(command);
            }
        }

        /// <summary>
        /// 添加设备维保成本、外来服务费成本
        /// </summary>
        /// <param name="dt">设备信息</param>
        public void InsertValEquipmentOutput(DataTable dt)
        { 
            using (SqlBulkCopy bulkCopy = ConnectionUtil.GetBulkCopy())
            { 
                bulkCopy.DestinationTableName = "tblValEqptOutput";
                bulkCopy.BatchSize = dt.Rows.Count;   
                bulkCopy.ColumnMappings.Add("UserID","UserID");
                bulkCopy.ColumnMappings.Add("Insystem", "InSystem");  
                bulkCopy.ColumnMappings.Add("EquipmentID","EquipmentID");  
                bulkCopy.ColumnMappings.Add("Year","Year");  
                bulkCopy.ColumnMappings.Add("Month","Month");
                bulkCopy.ColumnMappings.Add("ContractAmount", "ContractAmount");
                bulkCopy.ColumnMappings.Add("Repair3partyCost", "Repair3partyCost");  
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);  
                } 
            }

        }

        /// <summary>
        /// 清空设备成本表
        /// </summary>
        public void DeleteValEqptOutput(int userID)
        {
            sqlStr = "DELETE tblValEqptOutput WHERE UserID = @UserID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            } 
        }

        #endregion

        #region 耗材
        /// <summary>
        /// 获取耗材成本
        /// </summary>
        /// <param name="consumableType">耗材类型</param>
        /// <returns>耗材成本</returns>
        public List<ValConsumableOutputInfo> GetValConsumableAmount(int userID, int consumableType)
        {
            sqlStr = " SELECT vco.Year, vco.Month, sum(Amount) as Amount " +
                    " FROM tblValConsumableOutput vco " +
                    " INNER JOIN tblEquipment e ON vco.EquipmentID = e.ID " +
                    " INNER JOIN tblConsumable c ON vco.ConsumableID = c.ID " +
                    " WHERE vco.UserID = @UserID ";
            if (consumableType != -1)
                sqlStr += " AND c.TypeID = " + consumableType;

            sqlStr += " GROUP BY vco.Year,vco.Month " +
                      " ORDER BY vco.Year,vco.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValConsumableOutputInfo>(command);
            }
        }
        /// <summary>
        /// 添加耗材成本信息
        /// </summary>
        /// <param name="dt">耗材信息</param>
        public void InsertValConsumableOutput(DataTable dt)
        { 
            using (SqlBulkCopy bulkCopy = ConnectionUtil.GetBulkCopy())
            {
                bulkCopy.DestinationTableName = "tblValConsumableOutput";
                bulkCopy.BatchSize = dt.Rows.Count; 
                bulkCopy.ColumnMappings.Add("UserID", "UserID");
                bulkCopy.ColumnMappings.Add("Insystem", "InSystem");
                bulkCopy.ColumnMappings.Add("EquipmentID", "EquipmentID");
                bulkCopy.ColumnMappings.Add("ConsumableID", "ConsumableID");
                bulkCopy.ColumnMappings.Add("Year", "Year");
                bulkCopy.ColumnMappings.Add("Month", "Month");
                bulkCopy.ColumnMappings.Add("Amount", "Amount"); 
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                } 
            }
        }
        /// <summary>
        /// 清空耗材成本表
        /// </summary>
        public void DeleteValConsumableOutput(int userID)
        {
            sqlStr = "DELETE tblValConsumableOutput WHERE UserID = @UserID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            } 
        }
        #endregion

        #region 零件
        /// <summary>
        /// 获取零件成本
        /// </summary>
        /// <param name="eqptType">设备类别</param>
        /// <returns>零件成本</returns>
        public List<ValComponentOutputInfo> GetValComponentAmout(int userID, int eqptType)
        {
            sqlStr = "SELECT vc.Year,vc.Month,sum(Amount) as Amount " +
                    " FROM tblValComponentOutput vc " +
                    " INNER JOIN tblValEquipment ve ON vc.EquipmentID = ve.EquipmentID and ve.InSystem = vc.InSystem AND vc.UserID = ve.UserID " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " INNER JOIN tblUser u ON u.ID = vc.UserID " +
                    " WHERE vc.UserID = @UserID " ;
            if (eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = " + FujiClass2Info.LKPEquipmentType.Import;
            if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> " + FujiClass2Info.LKPEquipmentType.Import;

            sqlStr += " GROUP BY vc.Year,vc.Month " +
                      " ORDER BY vc.Year,vc.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValComponentOutputInfo>(command);
            }
        } 
         
        /// <summary>
        /// 获取零件成本
        /// </summary>
        /// <param name="eqptType">设备类别</param>
        /// <returns>零件成本</returns>
        public List<ValComponentOutputInfo> GetValComponentCost(int userID, int eqptType)
        {
            sqlStr =@"SELECT MAX(vc.EquipmentID) as EquipmentID, MAX(vc.ComponentID) as ComponentID, vc.Year,vc.Month,sum(Amount) as Amount, vc.UserID, u.Name as UserName 
                     FROM tblValComponentOutput vc 
                     INNER JOIN tblValEquipment ve ON vc.EquipmentID = ve.EquipmentID and ve.InSystem = vc.InSystem AND vc.UserID = ve.UserID 
                     INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID 
                     INNER JOIN tblUser u ON u.ID = vc.UserID 
                     WHERE vc.UserID = @UserID ";
            if (eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = " + FujiClass2Info.LKPEquipmentType.Import;
            if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> " + FujiClass2Info.LKPEquipmentType.Import;

            sqlStr += " GROUP BY vc.Year,vc.Month,vc.UserID,u.Name " +
                      " ORDER BY vc.Year,vc.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValComponentOutputInfo>(command);
            }
        }

        /// <summary>
        /// 添加零件成本信息
        /// </summary>
        /// <param name="dt">零件信息</param>
        public void InsertValComponentOutput(DataTable dt)
        { 
            using (SqlBulkCopy bulkCopy = ConnectionUtil.GetBulkCopy())
            { 
                bulkCopy.DestinationTableName = "tblValComponentOutput";
                bulkCopy.BatchSize = dt.Rows.Count; 
                bulkCopy.ColumnMappings.Add("UserID", "UserID");
                bulkCopy.ColumnMappings.Add("Insystem", "InSystem");
                bulkCopy.ColumnMappings.Add("EquipmentID", "EquipmentID");
                bulkCopy.ColumnMappings.Add("ComponentID", "ComponentID");
                bulkCopy.ColumnMappings.Add("Year", "Year");
                bulkCopy.ColumnMappings.Add("Month", "Month");
                bulkCopy.ColumnMappings.Add("Amount", "Amount"); 
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt); 
                } 
            }
        }
        /// <summary>
        /// 删除零件成本表
        /// </summary>
        public void DeleteValComponentOutput(int userID)
        {
            sqlStr = "DELETE tblValComponentOutput WHERE UserID = @UserID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region 外来服务费
        /// <summary>
        /// 获取设备外来服务费
        /// </summary>
        /// <param name="eqptType">设备类别</param>
        /// <returns>设备外来服务费</returns>
        public List<ValEqptOutputInfo> GetValEqptService(int userID, int eqptType)
        {
            sqlStr = "SELECT eo.Year,eo.Month,sum(eo.Repair3partyCost) as Repair3partyCost " +
                    " FROM tblValEqptOutput eo " +
                    " INNER JOIN tblValEquipment ve ON eo.EquipmentID = ve.EquipmentID AND eo.InSystem = ve.InSystem AND eo.UserID = ve.UserID " +
                    " INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID " +
                    " INNER JOIN tblUser u ON u.ID = eo.UserID " +
                    " WHERE eo.UserID = @UserID ";
            if(eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = " + FujiClass2Info.LKPEquipmentType.Import;
            if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> " + FujiClass2Info.LKPEquipmentType.Import;

            sqlStr += " GROUP BY eo.Year,eo.Month " +
                      " ORDER BY eo.Year,eo.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValEqptOutputInfo>(command);
            }
        }

        /// <summary>
        /// 获取设备外来服务费
        /// </summary>
        /// <param name="eqptType">设备类别</param>
        /// <returns>设备外来服务费</returns>
        public List<ValEqptOutputInfo> GetValEqptServiceCost(int userID, int eqptType)
        {
            sqlStr = @"SELECT MAX(eo.EquipmentID) as EquipmentID,eo.Year,eo.Month,0 as ContractAmount,sum(eo.Repair3partyCost) as Repair3partyCost, eo.UserID, u.Name as UserName 
                     FROM tblValEqptOutput eo 
                     INNER JOIN tblValEquipment ve ON eo.EquipmentID = ve.EquipmentID AND eo.InSystem = ve.InSystem AND eo.UserID = ve.UserID 
                     INNER JOIN tblFujiClass2 f2 ON ve.FujiClass2ID = f2.ID 
                     INNER JOIN tblUser u ON u.ID = eo.UserID 
                     WHERE eo.UserID = @UserID ";
            if (eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = " + FujiClass2Info.LKPEquipmentType.Import;
            if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> " + FujiClass2Info.LKPEquipmentType.Import;

            sqlStr += " GROUP BY eo.Year,eo.Month,eo.UserID,u.Name" +
                      " ORDER BY eo.Year,eo.Month";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                return GetList<ValEqptOutputInfo>(command);
            }
        }
        #endregion
        #endregion

        #region 实绩
        public double GetActualContractAmount(DateTime startDate)
        {
            sqlStr = "SELECT ISNULL(ROUND(SUM({0}),2),0) " +
                    " FROM tblContract c " +
                    " WHERE c.StartDate <@EndDate AND c.EndDate > @StartDate";

            sqlStr = string.Format(sqlStr, ActualSql.GetMonthActualAmount("c.StartDate", "c.EndDate", "c.Amount/DATEDIFF(Day,c.StartDate,c.EndDate)"));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = startDate.AddMonths(1);

                return GetDouble(command);
            }
        }

        public double GetActualSpareAmount(DateTime startDate)
        {
            sqlStr = "SELECT ISNULL(ROUND(SUM({0}),2),0) " +
                    " FROM tblInvSpare s " +
                    " WHERE s.StartDate < @EndDate AND s.EndDate > @StartDate";

            sqlStr = string.Format(sqlStr, ActualSql.GetMonthActualAmount("s.StartDate", "s.EndDate", "s.Price/DATEDIFF(Day,@StartDate,@EndDate)"));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = startDate.AddMonths(1);

                return GetDouble(command);
            }
        }

        public double GetActulConsumableAmount(int consumableType, DateTime startDate)
        {
            sqlStr = "SELECT ISNULL(ROUND(SUM(ic.Price/ic.ReceiveQty * mh.Qty),2),0) " +
                    " FROM tblMaterialHistory mh " +
                    " LEFT JOIN tblInvConsumable ic ON mh.ObjectID = ic.ID " +
                    " LEFT JOIN tblConsumable c ON ic.ConsumableID = c.ID " +
                    " WHERE mh.ObjectType = @ObjectType AND mh.UsedDate BETWEEN @StartDate AND @EndDate";
            if (consumableType == ConsumableInfo.ConsumableTypes.QuantitativeConsumable)
                sqlStr += " AND c.TypeID = @QuantyConsumable ";
            else if (consumableType == ConsumableInfo.ConsumableTypes.RegularConsumable)
                sqlStr += " AND c.TypeID = @RegularConsumable ";
            else if (consumableType == ConsumableInfo.ConsumableTypes.SmallCostConsumable)
                sqlStr += " AND c.TypeID = @SmallConsumable ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectType", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.Consumable;
                command.Parameters.Add("@QuantyConsumable", SqlDbType.Int).Value = ConsumableInfo.ConsumableTypes.QuantitativeConsumable;
                command.Parameters.Add("@RegularConsumable", SqlDbType.Int).Value = ConsumableInfo.ConsumableTypes.RegularConsumable;
                command.Parameters.Add("@SmallConsumable", SqlDbType.Int).Value = ConsumableInfo.ConsumableTypes.SmallCostConsumable;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = startDate.AddMonths(1);

                return GetDouble(command);
            }
        }
        
        public double GetActulComponentAmount(int eqptType, DateTime startDate)
        {
            sqlStr = "SELECT ISNULL(ROUND(SUM(ic.Price),2),0) " +
                    " FROM tblMaterialHistory mh " +
                    " LEFT JOIN tblInvComponent ic ON mh.ObjectID = ic.ID " +
                    " LEFT JOIN tblComponent c ON ic.ComponentID = c.ID " +
                    " LEFT JOIN tblEquipment e ON mh.EquipmentID = e.ID " +
                    " LEFT JOIN tblFujiClass2 f2 ON e.FujiClass2ID = f2.ID " +
                    " WHERE mh.ObjectType = @ObjectType AND mh.UsedDate BETWEEN @StartDate AND @EndDate";
            if (eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = @EqptType ";
            else if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> @EqptType ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectType", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.NewComponent;
                command.Parameters.Add("@EqptType", SqlDbType.Int).Value = FujiClass2Info.LKPEquipmentType.Import;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = startDate.AddMonths(1);

                return GetDouble(command);
            }
        }

        public double GetActulServiceAmount(int eqptType, DateTime startDate)
        {
            sqlStr = "SELECT ISNULL(ROUND(SUM({0}),2),0) " +
                    " FROM tblInvService ins " +
                    " LEFT JOIN tblFujiClass2 f2 ON ins.FujiClass2ID = f2.ID " +
                    " WHERE ins.StartDate < @EndDate AND ins.EndDate > @StartDate";
            if (eqptType == FujiClass2Info.LKPEquipmentType.Import)
                sqlStr += " AND f2.EquipmentType = @EqptType ";
            else if (eqptType == FujiClass2Info.LKPEquipmentType.General)
                sqlStr += " AND f2.EquipmentType <> @EqptType ";

            sqlStr = string.Format(sqlStr, ActualSql.GetMonthActualAmount("ins.StartDate", "ins.EndDate", "ins.Price/DATEDIFF(Day,@StartDate,@EndDate)"));

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ObjectType", SqlDbType.Int).Value = ReportMaterialInfo.MaterialTypes.Service;
                command.Parameters.Add("@EqptType", SqlDbType.Int).Value = FujiClass2Info.LKPEquipmentType.Import;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = startDate.AddMonths(1);

                return GetDouble(command);
            }
        }

        #endregion
        
        #region 计算备用机数量
        /// <summary>
        /// 查询维修请求数据
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维修请求数据</returns>
        public DataTable GetRepairDataByFujiClass2(DateTime startDate, DateTime endDate)
        {
            sqlStr = "SELECT e.FujiClass2ID, CONVERT(DATE, r.RequestDate) as RequestDate,COUNT(r.ID) as RepairCount, sum(DATEDIFF(MINUTE, r.RequestDate, ISNULL(r.CloseDate, @EndDate))) as Minutes " +
                    " FROM tblRequest r " +
                    " LEFT JOIN jctRequestEqpt jre ON r.ID = jre.RequestID " +
                    " INNER JOIN tblEquipment e ON e.ID = jre.EquipmentID " +
                    " WHERE r.RequestType = @RequestTypeRepair AND r.RequestDate Between @StartDate AND @EndDate " +
                    " AND r.StatusID != @StatusCancelled " +
                    " GROUP BY e.FujiClass2ID, CONVERT(DATE, r.RequestDate)";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@RequestTypeRepair", SqlDbType.Int).Value = RequestInfo.RequestTypes.Repair;
                command.Parameters.Add("@StatusCancelled", SqlDbType.Int).Value = RequestInfo.Statuses.Cancelled;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate.AddDays(1).AddSeconds(-1);

                return GetDataTable(command);
            }
        }
        /// <summary>
        /// 同步备用机数量
        /// </summary>
        /// <param name="dt">备用机列表</param>
        public void SyncSpareCount(DataTable dt)
        {
            sqlStr = "UPDATE tblValSpare SET QtyEval = @CalculatedCount " +
                    " WHERE FujiClass2ID = @FujiClass2ID ";

            SqlParameter parameter = null;
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                parameter = command.Parameters.Add("@FujiClass2ID", SqlDbType.Int);
                parameter.SourceColumn = "FujiClass2ID";
                parameter.SourceVersion = DataRowVersion.Original;

                parameter = command.Parameters.Add("@CalculatedCount", SqlDbType.Int);
                parameter.SourceColumn = "CalculatedCount";
                parameter.SourceVersion = DataRowVersion.Original;

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        #endregion
    }
}
