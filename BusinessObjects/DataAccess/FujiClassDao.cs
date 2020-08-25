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
    /// 富士类别Dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class FujiClassDao : BaseDao
    {
        #region"FujiClass2"
        /// <summary>
        /// 获取富士II类列表
        /// </summary>
        /// <param name="class1">富士I类id</param>
        /// <param name="class2">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">页码</param>
        /// <param name="pageSize">每页展示数据条数</param>
        /// <returns>富士II类列表</returns>
        public List<FujiClass2Info> QueryFujiClass2s(int class1, int class2, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum , int pageSize )
        {
            sqlStr = "SELECT f2.*,f1.Name FujiClass1Name FROM tblFujiClass2 f2 " +
                    " LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID " +
                    " WHERE 1=1 ";
            if (class1 != 0)
                sqlStr += " AND f2.FujiClass1ID = @Class1ID ";
            if (class2 != 0)
                sqlStr += " AND f2.ID = @Class2ID ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            sqlStr += GenerateSortClause(sortDirection, sortField, "f1.ID");
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                AddFieldFilterParam(command, filterField, filterText);
                command.Parameters.Add("@Class1ID", SqlDbType.Int).Value = class1;
                command.Parameters.Add("@Class2ID", SqlDbType.Int).Value = class2;
                return GetList<FujiClass2Info>(command); 
            } 
        }

        /// <summary>
        /// 获取富士II类数量
        /// </summary>
        /// <param name="class1">富士I类id</param>
        /// <param name="class2">富士II类id</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索内容</param>
        /// <returns>富士II类数量</returns>
        public int QueryFujiClass2Count(int class1, int class2, string filterField, string filterText)
        { 
            sqlStr = "SELECT COUNT(DISTINCT f2.ID) FROM tblFujiClass2 f2 " +
                    " LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID " +
                    " WHERE 1=1 "; 
            if (class1 != 0)
                sqlStr += " AND f2.FujiClass1ID = @Class1ID ";
            if (class2 != 0)
                sqlStr += " AND f2.ID = @Class2ID ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += GetFieldFilterClause(filterField);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                AddFieldFilterParam(command, filterField, filterText);

                command.Parameters.Add("@Class1ID", SqlDbType.Int).Value = class1;
                command.Parameters.Add("@Class2ID", SqlDbType.Int).Value = class2;
                return GetCount(command);
            }
        }

        /// <summary>
        /// 根据输入内容搜索富士I类信息
        /// </summary>
        /// <param name="inputText">输入内容</param>
        /// <param name="fujiClass1ID">一类ID</param>
        /// <returns>富士I类信息</returns>
        public List<FujiClass2Info> QueryFujiClass24AutoComplete(string inputText, int fujiClass1ID)
        {
            List<FujiClass2Info> infos = new List<FujiClass2Info>();
            sqlStr = "SELECT f.* " +
                    " FROM tblFujiClass2 f " +
                    " WHERE 1=1 AND FujiClass1ID = @FujiClass1ID";
            if (!string.IsNullOrEmpty(inputText))
                sqlStr += " AND UPPER(f.Name) LIKE @InputText ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = fujiClass1ID;
                if (!string.IsNullOrEmpty(inputText))
                    command.Parameters.Add("@InputText", SqlDbType.NVarChar).Value = "%" + inputText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new FujiClass2Info(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 通过富士II类id列表获取富士II类信息
        /// </summary>
        /// <param name="ids">富士II类id列表</param>
        /// <returns>富士II类信息</returns>
        public List<FujiClass2Info> QueryFujiClass2sByIDs(List<int> ids)
        {
            if (ids.Count == 0)
                return new List<FujiClass2Info>();
            sqlStr = "SELECT f2.*,f1.Name FujiClass1Name FROM tblFujiClass2 f2 " +
                    " LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID " +
                    " WHERE f2.ID IN ("+ string.Join(",", ids)+")";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {  
                return GetList<FujiClass2Info>(command);
            }
        }

        /// <summary>
        /// 根据设备类别获取富士II类
        /// </summary>
        /// <param name="equipmentClass1">设备类别 (I)</param>
        /// <param name="equipmentClass2">设备类别 (II)</param>
        /// <returns>富士II类</returns>
        public List<FujiClass2Info> GetFujiClass2ByEqptClass(string equipmentClass1, string equipmentClass2)
        {
            sqlStr = "SELECT f2.* FROM tblFujiClass2 f2 " +
                    " LEFT JOIN jctFujiClass2EqpType AS jct2 ON jct2.FujiClass2ID = f2.ID " +
                    " WHERE jct2.EquipmentType1ID = @EquipmentType1ID AND jct2.EquipmentType2ID = @EquipmentType2ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.NVarChar).Value = equipmentClass1;
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.NVarChar).Value = equipmentClass2;
                return GetList<FujiClass2Info>(command);
            } 
        }
        /// <summary>
        /// 获取富士II类信息
        /// </summary>
        /// <returns></returns>
        public List<FujiClass2Info> GetFujiClass2()
        {
            List<FujiClass2Info> infos = new List<FujiClass2Info>();
            sqlStr = @"SELECT f2.*,f1.Name FujiClass1Name FROM tblFujiClass2 f2  
                          LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID  
                          WHERE 1=1 ORDER BY f2.Name ASC";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new FujiClass2Info(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取富士类别信息
        /// </summary>
        /// <returns>富士类别信息</returns>
        public List<Tuple<KeyValueInfo, int>> QueryKeyValueFujiClass2()
        { 
            List<Tuple<KeyValueInfo, int>> infos = new List<Tuple<KeyValueInfo, int>>();
            sqlStr = @" SELECT f2.ID,f2.Name,f2.FujiClass1ID FROM tblFujiClass2 f2 
                        WHERE 1=1 ORDER BY f2.Name ASC ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    { 
                        infos.Add(new Tuple<KeyValueInfo, int>(new KeyValueInfo() { ID = SQLUtil.ConvertInt(dr[0]), Name = SQLUtil.TrimNull(dr[1]) }, SQLUtil.ConvertInt(dr[2])));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 通过id获取富士II类信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>富士II类信息</returns>
        public FujiClass2Info GetFujiClass2ByID(int id)
        {
            sqlStr = "SELECT f2.*,f1.Name FujiClass1Name FROM tblFujiClass2 f2 " +
                    " LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID " +
                    " WHERE f2.ID = @ID "; 
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            { 
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new FujiClass2Info(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据富士II类id获取关联信息
        /// </summary>
        /// <param name="id">富士II类id</param>
        /// <returns>富士II类关联信息</returns>
        public FujiClassLink GetFujiLinkByClass2ID(int id)
        { 
            sqlStr = @"SELECT f2.*,f1.Name FujiClass1Name ,e1.Code EquipmentType1ID,e2.Code EquipmentType2ID FROM tblFujiClass2 f2 
                         LEFT JOIN tblFujiClass1 f1 ON f2.FujiClass1ID = f1.ID 
                         LEFT JOIN jctFujiClass2EqpType as jct2 ON jct2.FujiClass2ID = f2.ID 
                         LEFT JOIN lkpEquipmentClass as e1 ON jct2.EquipmentType1ID = e1.Code and e1.Level = 1 
                         LEFT JOIN lkpEquipmentClass as e2 ON jct2.EquipmentType2ID = e2.Code and jct2.EquipmentType1ID = e2.ParentCode and e2.Level = 2 
                         WHERE f2.ID = @ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                DataRow dr = GetDataRow(command);
                if (dr != null)
                {
                    FujiClassLink result = new FujiClassLink(dr);
                    result.FujiClass2 = new FujiClass2Info(dr);
                    return result;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 判断富士II类名称是否重复
        /// </summary>
        /// <param name="info">富士II类信息</param>
        /// <returns>是否重复</returns>
        public bool CheckFujiClass2Name(FujiClass2Info info)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblFujiClass2 " +
                    " WHERE UPPER(Name)=@Name AND ID<>@ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name.ToUpper());

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }

        }

        /// <summary>
        /// 判断富士II类关联关系是否重复
        /// </summary>
        /// <param name="info">富士II类关联信息</param>
        /// <returns>富士II类关联关系是否重复</returns>
        public bool CheckFujiClass2EqpExisted(FujiClassLink info)
        {
            sqlStr = "SELECT COUNT(FujiClass2ID) FROM jctFujiClass2EqpType " +
                    " WHERE FujiClass2ID = @FujiClass2ID and EquipmentType1ID = @EquipmentType1ID and EquipmentType2ID = @EquipmentType2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }

        }
        /// <summary>
        /// 添加富士II类信息
        /// </summary>
        /// <param name="info">富士II类信息</param>
        /// <returns>富士II类id</returns>
        public int AddFujiClass2(FujiClass2Info info)
        {
            sqlStr = @" INSERT INTO tblFujiClass2 (FujiClass1ID,Name,Description, 
                                            IncludeLabour,PatrolTimes,PatrolHours,MaintenanceTimes,MaintenanceHours,RepairHours, 
                                            IncludeContract,FullCoveragePtg,TechCoveragePtg, IncludeSpare,SparePrice,SpareRentPtg, 
                                            IncludeRepair,Usage,EquipmentType,RepairComponentCost,Repair3partyRatio,Repair3partyCost,RepairCostRatio, 
                                            MethodID, AddDate)
                        VALUES (@FujiClass1ID,@Name,@Description,
                                            @IncludeLabour,@PatrolTimes,@PatrolHours,@MaintenanceTimes,@MaintenanceHours,@RepairHours,
                                            @IncludeContract,@FullCoveragePtg,@TechCoveragePtg,@IncludeSpare,@SparePrice,@SpareRentPtg,
                                            @IncludeRepair,@Usage,@EquipmentType,@RepairComponentCost,@Repair3partyRatio,@Repair3partyCost,@RepairCostRatio,
                                            @MethodID, GETDATE())
                        SELECT @@IDENTITY ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = info.FujiClass1.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);

                command.Parameters.Add("@IncludeLabour", SqlDbType.Bit).Value = info.IncludeLabour;
                command.Parameters.Add("@PatrolTimes", SqlDbType.Decimal).Value = info.PatrolTimes;
                command.Parameters.Add("@PatrolHours", SqlDbType.Decimal).Value = info.PatrolHours;
                command.Parameters.Add("@MaintenanceTimes", SqlDbType.Decimal).Value = info.MaintenanceTimes;
                command.Parameters.Add("@MaintenanceHours", SqlDbType.Decimal).Value = info.MaintenanceHours;
                command.Parameters.Add("@RepairHours", SqlDbType.Decimal).Value = info.RepairHours;

                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = info.IncludeContract;
                command.Parameters.Add("@FullCoveragePtg", SqlDbType.Decimal).Value = info.FullCoveragePtg;
                command.Parameters.Add("@TechCoveragePtg", SqlDbType.Decimal).Value = info.TechCoveragePtg;

                command.Parameters.Add("@IncludeSpare", SqlDbType.Bit).Value = info.IncludeSpare;
                command.Parameters.Add("@SparePrice", SqlDbType.Decimal).Value = info.SparePrice;
                command.Parameters.Add("@SpareRentPtg", SqlDbType.Decimal).Value = info.SpareRentPtg;

                command.Parameters.Add("@IncludeRepair", SqlDbType.Bit).Value = true;
                command.Parameters.Add("@Usage", SqlDbType.Int).Value = info.Usage;
                command.Parameters.Add("@EquipmentType", SqlDbType.Int).Value = info.EquipmentType.ID ;
                command.Parameters.Add("@RepairComponentCost", SqlDbType.Decimal).Value = info.RepairComponentCost;
                command.Parameters.Add("@Repair3partyRatio", SqlDbType.Decimal).Value = info.Repair3partyRatio;
                command.Parameters.Add("@Repair3partyCost", SqlDbType.Decimal).Value = info.Repair3partyCost;
                command.Parameters.Add("@RepairCostRatio", SqlDbType.Decimal).Value = info.RepairCostRatio;
                command.Parameters.Add("@MethodID", SqlDbType.Int).Value = ((int)info.MethodID ==0 )? FujiClass2Info.Method.Manual: info.MethodID;
                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
                return info.ID;
            } 
        }

        /// <summary>
        /// 添加富士II类关联表信息
        /// </summary>
        /// <param name="info">富士II类关联信息</param>
        public void AddFujiClass2EqpType(FujiClassLink info)
        {
            sqlStr = "INSERT INTO jctFujiClass2EqpType(EquipmentType1ID,EquipmentType2ID,FujiClass2ID) " +
                    " VALUES(@EquipmentType1ID,@EquipmentType2ID,@FujiClass2ID)";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 修改富士II类信息
        /// </summary>
        /// <param name="info">富士II类信息</param>
        public void UpdateFujiClass2(FujiClass2Info info)
        {
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            FujiClass1ID = @FujiClass1ID,Name = @Name,Description = @Description,
                            IncludeLabour = @IncludeLabour,PatrolTimes = @PatrolTimes,PatrolHours = @PatrolHours,MaintenanceTimes = @MaintenanceTimes,MaintenanceHours = @MaintenanceHours,RepairHours = @RepairHours,
                            IncludeContract = @IncludeContract,FullCoveragePtg = @FullCoveragePtg,TechCoveragePtg = @TechCoveragePtg,
                            IncludeSpare = @IncludeSpare,SparePrice = @SparePrice,SpareRentPtg = @SpareRentPtg,
                            IncludeRepair = @IncludeRepair,Usage = @Usage,EquipmentType = @EquipmentType,RepairComponentCost = @RepairComponentCost,Repair3partyRatio = @Repair3partyRatio,Repair3partyCost = @Repair3partyCost,
                            RepairCostRatio = @RepairCostRatio,MethodID = @MethodID,UpdateDate = GETDATE() 
                        WHERE ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = info.FujiClass1.ID;
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);

                command.Parameters.Add("@IncludeLabour", SqlDbType.Bit).Value = info.IncludeLabour;
                command.Parameters.Add("@PatrolTimes", SqlDbType.Decimal).Value = info.PatrolTimes;
                command.Parameters.Add("@PatrolHours", SqlDbType.Decimal).Value = info.PatrolHours;
                command.Parameters.Add("@MaintenanceTimes", SqlDbType.Decimal).Value = info.MaintenanceTimes;
                command.Parameters.Add("@MaintenanceHours", SqlDbType.Decimal).Value = info.MaintenanceHours;
                command.Parameters.Add("@RepairHours", SqlDbType.Decimal).Value = info.RepairHours;

                command.Parameters.Add("@IncludeContract", SqlDbType.Bit).Value = info.IncludeContract;
                command.Parameters.Add("@FullCoveragePtg", SqlDbType.Decimal).Value = info.FullCoveragePtg;
                command.Parameters.Add("@TechCoveragePtg", SqlDbType.Decimal).Value = info.TechCoveragePtg;

                command.Parameters.Add("@IncludeSpare", SqlDbType.Bit).Value = info.IncludeSpare;
                command.Parameters.Add("@SparePrice", SqlDbType.Decimal).Value = info.SparePrice;
                command.Parameters.Add("@SpareRentPtg", SqlDbType.Decimal).Value = info.SpareRentPtg;

                command.Parameters.Add("@IncludeRepair", SqlDbType.Bit).Value = info.IncludeRepair;
                command.Parameters.Add("@Usage", SqlDbType.Int).Value = info.Usage;
                command.Parameters.Add("@EquipmentType", SqlDbType.Int).Value = info.EquipmentType.ID;
                command.Parameters.Add("@RepairComponentCost", SqlDbType.Decimal).Value = info.RepairComponentCost;
                command.Parameters.Add("@Repair3partyRatio", SqlDbType.Decimal).Value = info.Repair3partyRatio;
                command.Parameters.Add("@Repair3partyCost", SqlDbType.Decimal).Value = info.Repair3partyCost;
                command.Parameters.Add("@RepairCostRatio", SqlDbType.Decimal).Value = info.RepairCostRatio;
                command.Parameters.Add("@MethodID", SqlDbType.Int).Value = ((int)info.MethodID == 0) ? FujiClass2Info.Method.Manual : info.MethodID;
                command.Parameters.Add("@ID", SqlDbType.Int).Value = info.ID;
                command.ExecuteNonQuery(); 
            }
        }

        /// <summary>
        /// 批量修改富士II类信息
        /// </summary>
        /// <param name="dt">富士II类信息</param>
        public void UpdateFujiClass2s(DataTable dt)
        { 
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            FujiClass1ID = @FujiClass1ID,Name = @Name,Description = @Description,
                            IncludeLabour = @IncludeLabour,PatrolTimes = @PatrolTimes,PatrolHours = @PatrolHours,MaintenanceTimes = @MaintenanceTimes,MaintenanceHours = @MaintenanceHours,RepairHours = @RepairHours,
                            IncludeContract = @IncludeContract,FullCoveragePtg = @FullCoveragePtg,TechCoveragePtg = @TechCoveragePtg,
                            IncludeSpare = @IncludeSpare,SparePrice = @SparePrice,SpareRentPtg = @SpareRentPtg,
                            IncludeRepair = @IncludeRepair,Usage = @Usage,EquipmentType = @EquipmentType,RepairComponentCost = @RepairComponentCost,Repair3partyRatio = @Repair3partyRatio,Repair3partyCost = @Repair3partyCost,
                            RepairCostRatio = @RepairCostRatio,MethodID = @MethodID,UpdateDate = GETDATE()
                        WHERE ID = @ID ";
              
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@FujiClass1ID", SqlDbType = SqlDbType.Int, SourceColumn ="FujiClass1ID", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, SourceColumn ="Name" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Description", SqlDbType = SqlDbType.NVarChar, SourceColumn ="Description" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeLabour", SqlDbType = SqlDbType.Bit, SourceColumn ="IncludeLabour" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@PatrolTimes", SqlDbType = SqlDbType.Decimal, SourceColumn ="PatrolTimes" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@PatrolHours", SqlDbType = SqlDbType.Decimal, SourceColumn ="PatrolHours" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MaintenanceTimes", SqlDbType = SqlDbType.Decimal, SourceColumn ="MaintenanceTimes" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MaintenanceHours", SqlDbType = SqlDbType.Decimal, SourceColumn ="MaintenanceHours" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairHours", SqlDbType = SqlDbType.Decimal, SourceColumn ="RepairHours" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeContract", SqlDbType = SqlDbType.Bit, SourceColumn ="IncludeContract" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@FullCoveragePtg", SqlDbType = SqlDbType.Decimal, SourceColumn ="FullCoveragePtg" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@TechCoveragePtg", SqlDbType = SqlDbType.Decimal, SourceColumn ="TechCoveragePtg" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeSpare", SqlDbType = SqlDbType.Bit, SourceColumn ="IncludeSpare" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@SparePrice", SqlDbType = SqlDbType.Decimal, SourceColumn ="SparePrice" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@SpareRentPtg", SqlDbType = SqlDbType.Decimal, SourceColumn ="SpareRentPtg" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeRepair", SqlDbType = SqlDbType.Bit, SourceColumn ="IncludeRepair" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Usage", SqlDbType = SqlDbType.Int, SourceColumn ="Usage" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@EquipmentType", SqlDbType = SqlDbType.Int, SourceColumn ="EquipmentType" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairComponentCost", SqlDbType = SqlDbType.Decimal, SourceColumn ="RepairComponentCost" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Repair3partyRatio", SqlDbType = SqlDbType.Decimal, SourceColumn ="Repair3partyRatio" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Repair3partyCost", SqlDbType = SqlDbType.Decimal, SourceColumn ="Repair3partyCost" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairCostRatio", SqlDbType = SqlDbType.Decimal, SourceColumn ="RepairCostRatio" , SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MethodID", SqlDbType = SqlDbType.Int, SourceColumn ="MethodID" , SourceVersion = DataRowVersion.Original }); 
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original });
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 批量修改富士II类人工费信息
        /// </summary>
        /// <param name="dt">富士II类人工费信息</param>
        public void UpdateFujiClass2Labour(DataTable dt)
        {
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            IncludeLabour = @IncludeLabour,PatrolTimes = @PatrolTimes,PatrolHours = @PatrolHours,MaintenanceTimes = @MaintenanceTimes,MaintenanceHours = @MaintenanceHours,RepairHours = @RepairHours,
                            UpdateDate = GETDATE()
                        WHERE ID = @ID ";
             
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeLabour", SqlDbType = SqlDbType.Bit, SourceColumn = "IncludeLabour", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@PatrolTimes", SqlDbType = SqlDbType.Decimal, SourceColumn = "PatrolTimes", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@PatrolHours", SqlDbType = SqlDbType.Decimal, SourceColumn = "PatrolHours", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MaintenanceTimes", SqlDbType = SqlDbType.Decimal, SourceColumn = "MaintenanceTimes", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MaintenanceHours", SqlDbType = SqlDbType.Decimal, SourceColumn = "MaintenanceHours", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairHours", SqlDbType = SqlDbType.Decimal, SourceColumn = "RepairHours", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original });
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 批量修改富士II类维保信息
        /// </summary>
        /// <param name="dt">富士II类维保信息</param>
        public void UpdateFujiClass2Contract(DataTable dt)
        {
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            IncludeContract = @IncludeContract,FullCoveragePtg = @FullCoveragePtg,TechCoveragePtg = @TechCoveragePtg,
                            UpdateDate = GETDATE()
                        WHERE ID = @ID ";
            
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeContract", SqlDbType = SqlDbType.Bit, SourceColumn = "IncludeContract", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@FullCoveragePtg", SqlDbType = SqlDbType.Decimal, SourceColumn = "FullCoveragePtg", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@TechCoveragePtg", SqlDbType = SqlDbType.Decimal, SourceColumn = "TechCoveragePtg", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original });
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 批量修改富士II类备用机信息
        /// </summary>
        /// <param name="dt">富士II类备用机信息</param>
        public void UpdateFujiClass2Spare(DataTable dt)
        {
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            IncludeSpare = @IncludeSpare,SparePrice = @SparePrice,SpareRentPtg = @SpareRentPtg,
                            UpdateDate = GETDATE()
                        WHERE ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeSpare", SqlDbType = SqlDbType.Bit, SourceColumn = "IncludeSpare", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@SparePrice", SqlDbType = SqlDbType.Decimal, SourceColumn = "SparePrice", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@SpareRentPtg", SqlDbType = SqlDbType.Decimal, SourceColumn = "SpareRentPtg", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original });
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }
        /// <summary>
        /// 批量修改富士II类维修费信息
        /// </summary>
        /// <param name="dt">富士II类维修费信息</param>
        public void UpdateFujiClass2Repair(DataTable dt)
        {
            sqlStr = @" UPDATE tblFujiClass2 SET 
                            IncludeRepair = @IncludeRepair,Usage = @Usage,EquipmentType = @EquipmentType,RepairComponentCost = @RepairComponentCost,Repair3partyRatio = @Repair3partyRatio,Repair3partyCost = @Repair3partyCost,
                            RepairCostRatio = @RepairCostRatio,MethodID = @MethodID,UpdateDate = GETDATE()
                        WHERE ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add(new SqlParameter() { ParameterName = "@IncludeRepair", SqlDbType = SqlDbType.Bit, SourceColumn = "IncludeRepair", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Usage", SqlDbType = SqlDbType.Int, SourceColumn = "Usage", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@EquipmentType", SqlDbType = SqlDbType.Int, SourceColumn = "EquipmentType", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairComponentCost", SqlDbType = SqlDbType.Decimal, SourceColumn = "RepairComponentCost", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Repair3partyRatio", SqlDbType = SqlDbType.Decimal, SourceColumn = "Repair3partyRatio", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@Repair3partyCost", SqlDbType = SqlDbType.Decimal, SourceColumn = "Repair3partyCost", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@RepairCostRatio", SqlDbType = SqlDbType.Decimal, SourceColumn = "RepairCostRatio", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@MethodID", SqlDbType = SqlDbType.Int, SourceColumn = "MethodID", SourceVersion = DataRowVersion.Original });
                command.Parameters.Add(new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.Int, SourceColumn = "ID", SourceVersion = DataRowVersion.Original });
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.InsertCommand = command;

                    da.Update(dt);
                }
            }
        }

        /// <summary>
        /// 删除富士II类与设备类型关联信息
        /// </summary>
        /// <param name="info">富士II类与设备类型关联信息</param>
        public void DeleteFujiClass2Link(FujiClassLink info)
        {
            sqlStr = " DELETE FROM jctFujiClass2EqpType WHERE EquipmentType1ID = @EquipmentType1ID AND EquipmentType2ID = @EquipmentType2ID AND FujiClass2ID = @FujiClass2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据ID删除富士II类信息
        /// </summary>
        /// <param name="id">富士II类ID</param>
        public void DeleteFujiClass2ByID(int id)
        {
            sqlStr = " DELETE FROM tblFujiClass2 WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(id);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 判断富士II类设备类型是否存在
        /// </summary>
        /// <param name="info">富士II类信息</param>
        /// <returns>是否存在</returns>
        public bool CheckFujiClass2LinkExisted(FujiClassLink info)
        {
            sqlStr = "SELECT COUNT(FujiClass2ID) FROM jctFujiClass2EqpType " +
                    " WHERE FujiClass2ID=@FujiClass2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.FujiClass2.ID);

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }

        }
        #endregion

        #region"FujiClass1"
        /// <summary>
        /// 展示富士类别
        /// </summary>
        /// <param name="EquipmentClass1Name">设备类别1名称</param>
        /// <param name="EquipmentClass2Name">设备类别2名称</param>
        /// <param name="FujiClass1ID">富士1类名称</param>
        /// <param name="FujiClass2ID">富士2类名称</param>
        /// <returns>富士类别信息</returns>
        public List<FujiClassLink> QueryFujiClass(string EquipmentClass1Name, string EquipmentClass2Name, int FujiClass1ID, int FujiClass2ID)
        {
            List<FujiClassLink> infos = new List<FujiClassLink>();
            sqlStr = "SELECT jct1.*, f2.*, f1.Name as FujiClass1Name, f1.Description as FujiClass1Description, t.FujiClass2Count, " +
                    " (CASE WHEN f2.IncludeContract = 0 AND f2.IncludeLabour = 0 AND f2.IncludeRepair = 0 AND f2.IncludeSpare = 0 THEN 0 ELSE 1 END) as hasEdited " +
                    " FROM jctFujiClass1EqpType as jct1 " +
                    " INNER JOIN tblFujiClass1 as f1 ON f1.ID = jct1.FujiClass1ID " +
                    " INNER JOIN lkpEquipmentClass as e1 ON jct1.EquipmentType1ID = e1.Code and e1.Level = 1 " +
                    " INNER JOIN lkpEquipmentClass as e2 ON jct1.EquipmentType2ID = e2.Code and jct1.EquipmentType1ID = e2.ParentCode and e2.Level = 2 " +
                    " LEFT JOIN jctFujiClass2EqpType as jct2 ON jct2.EquipmentType1ID = jct1.EquipmentType1ID and jct2.EquipmentType2ID = jct1.EquipmentType2ID " +
                    " LEFT JOIN tblFujiClass2 as f2 ON jct2.FujiClass2ID = f2.ID " + 
                    " LEFT JOIN (Select FujiClass1ID, count(ID) as FujiClass2Count from tblFujiClass2 Group By FujiClass1ID) as t on t.FujiClass1ID = jct1.FujiClass1ID" +
                    " WHERE 1=1 ";

            if (EquipmentClass1Name != "0")
                sqlStr += " AND e1.Description = @EquipmentClass1Name ";
            if (EquipmentClass2Name != "0")
                sqlStr += " AND e2.Description = @EquipmentClass2Name ";
            if (FujiClass1ID != 0)
                sqlStr += " AND f1.ID = @FujiClass1ID ";
            if (FujiClass2ID != 0)
                sqlStr += " AND f2.ID = @FujiClass2ID ";

            sqlStr += "ORDER BY jct1.FujiClass1ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (EquipmentClass1Name != "0")
                    command.Parameters.Add("@EquipmentClass1Name", SqlDbType.NVarChar).Value = EquipmentClass1Name;
                if (EquipmentClass2Name != "0")
                    command.Parameters.Add("@EquipmentClass2Name", SqlDbType.NVarChar).Value = EquipmentClass2Name;
                if (FujiClass1ID != 0)
                    command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(FujiClass1ID);
                if (FujiClass2ID != 0)
                    command.Parameters.Add("@FujiClass2ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(FujiClass2ID);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new FujiClassLink(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据输入内容搜索富士I类信息
        /// </summary>
        /// <param name="inputText">输入内容</param>
        /// <returns>富士I类信息</returns>
        public List<FujiClass1Info> QueryFujiClass14AutoComplete(string inputText)
        {
            List<FujiClass1Info> infos = new List<FujiClass1Info>();
            sqlStr = "SELECT f.* " +
                    " FROM tblFujiClass1 f " +
                    " WHERE f.ID > 0 ";
            if (!string.IsNullOrEmpty(inputText))
                sqlStr += " AND UPPER(f.Name) LIKE @InputText ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!string.IsNullOrEmpty(inputText))
                    command.Parameters.Add("@InputText", SqlDbType.NVarChar).Value = "%" + inputText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new FujiClass1Info(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取富士I类信息
        /// </summary>
        /// <returns>富士I类信息</returns>
        public List<KeyValueInfo> QueryKeyValueFujiClass1()
        { 
            List<KeyValueInfo> infos = new List<KeyValueInfo>();
            sqlStr = @" SELECT f1.ID,f1.Name FROM tblFujiClass1 f1 
                        WHERE 1=1 ORDER BY f1.Name ASC ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    { 
                        infos.Add(new KeyValueInfo() { ID = SQLUtil.ConvertInt(dr[0]), Name = SQLUtil.TrimNull(dr[1]) });
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 根据富士I类id获取富士I类信息
        /// </summary>
        /// <param name="id">富士I类id</param>
        /// <returns>富士I类信息</returns>
        public FujiClass1Info GetFujiClass1ByID(int id)
        {
            sqlStr = "SELECT * FROM tblFujiClass1 WHERE ID = " + id;

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new FujiClass1Info(dr);
                    else
                        return null;
                }
            }
        }
        /// <summary>
        /// 获取设备类别1信息
        /// </summary>
        /// <returns>设备类别1信息</returns>
        public List<string> GetEquipmentClass1s()
        {
            List<string> infos = new List<string>();
            sqlStr = "SELECT DISTINCT e1.Description " +
                    " FROM jctFujiClass1EqpType AS jct1 " +
                    " INNER JOIN lkpEquipmentClass AS e1 ON jct1.EquipmentType1ID = e1.Code AND e1.Level = 1 ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(SQLUtil.TrimNull(dr[0]));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 获取设备类别2信息
        /// </summary>
        /// <returns>设备类别2信息</returns>
        public List<string> GetEquipmentClass2s()
        {
            List<string> infos = new List<string>();
            sqlStr = "SELECT DISTINCT e2.Description " +
                    " FROM jctFujiClass1EqpType AS jct1 " +
                    " INNER JOIN lkpEquipmentClass as e2 ON jct1.EquipmentType2ID = e2.Code and jct1.EquipmentType1ID = e2.ParentCode and e2.Level = 2  ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(SQLUtil.TrimNull(dr[0]));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取富士I类信息
        /// </summary>
        /// <returns>富士I类信息</returns>
        public List<FujiClass1Info> GetFujiClass1s()
        {
            List<FujiClass1Info> infos = new List<FujiClass1Info>();
            sqlStr = @" SELECT f1.* FROM tblFujiClass1 f1 
                        WHERE 1=1 ORDER BY f1.Name ASC ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new FujiClass1Info(dr));
                    }
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据设备类别获取设备的富士I类
        /// </summary>
        /// <param name="equipmentClass1">设备类别I</param>
        /// <param name="equipmentClass2">设备类别II</param>
        /// <returns>设备的富士I类</returns>
        public FujiClass1Info GetFujiClass1ByEquipmentClass(string equipmentClass1, string equipmentClass2)
        {
            sqlStr = "SELECT f1.* FROM tblFujiClass1 f1 " +
                    " LEFT JOIN jctFujiClass1EqpType AS jct1 ON jct1.FujiClass1ID = f1.ID" +
                    " WHERE jct1.EquipmentType1ID = " + equipmentClass1 +
                    " AND jct1.EquipmentType2ID = " + equipmentClass2;

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    DataRow dr = GetDataRow(command);
                    if (dr != null)
                        return new FujiClass1Info(dr);
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 判断富士I类名称是否重复
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <returns>是否重复</returns>
        public bool CheckFujiClass1Name(FujiClass1Info info)
        {
            sqlStr = "SELECT COUNT(ID) FROM tblFujiClass1 " +
                    " WHERE UPPER(Name)=@Name AND ID<>@ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name.ToUpper());

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }

        }

		/// <summary>
        /// 判断富士I类设备类型是否重复
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <returns>是否重复</returns>
        public bool CheckEquipmentClassExisted(FujiClass1Info info)
        {
            sqlStr = "SELECT COUNT(FujiClass1ID) FROM jctFujiClass1EqpType " +
                    " WHERE EquipmentType1ID=@EquipmentType1 AND EquipmentType2ID=@EquipmentType2";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentType1", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);

                return SQLUtil.ConvertInt(command.ExecuteScalar()) > 0;
            }

        }

        /// <summary>
        /// 添加富士I类信息
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <returns>富士I类id</returns>
        public int AddFujiClass1(FujiClass1Info info)
        {
            sqlStr = "INSERT INTO tblFujiClass1(Name,Description,AddDate) " +
                    " VALUES(@Name,@Description,GetDate())" +
                    " SELECT @@IDENTITY ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);

                info.ID = SQLUtil.ConvertInt(command.ExecuteScalar());

                return info.ID;
            }
        }

        /// <summary>
        /// 添加富士I类关联表信息
        /// </summary>
        /// <param name="info">富士I类信息</param>
        public void AddFujiClass1EqpType(FujiClass1Info info)
        {
            sqlStr = "INSERT INTO jctFujiClass1EqpType(EquipmentType1ID,EquipmentType2ID,FujiClass1ID) " +
                    " VALUES(@EquipmentType1ID,@EquipmentType2ID,@FujiClass1ID)";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);
                command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.ID);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 修改富士I类信息
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <returns>富士I类id</returns>
        public int UpdateFujiClass1(FujiClass1Info info)
        {
            sqlStr = "UPDATE tblFujiClass1 SET Name=@Name,Description=@Description,UpdateDate=GetDate() " +
                    " WHERE ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);

                command.ExecuteScalar();

                return info.ID;
            }

        }

        /// <summary>
        /// 修改富士I类信息
        /// </summary>
        /// <param name="info">富士I类信息</param>
        /// <returns>富士I类id</returns>
        public void UpdateFujiClass1EqpType(FujiClass1Info info)
        {
            sqlStr = "UPDATE jctFujiClass1EqpType SET EquipmentType1ID=@EquipmentType1ID,EquipmentType2ID=@EquipmentType2ID" +
                    " WHERE ID = @ID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(info.ID);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Name);
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(info.Description);

                command.ExecuteScalar();
            }

        }

        /// <summary>
        /// 删除富士I类与设备类型关联信息
        /// </summary>
        /// <param name="info">富士I类与设备类型关联信息</param>
        public void DeleteFujiClassLink(FujiClassLink info)
        {
            sqlStr = " DELETE FROM jctFujiClass1EqpType WHERE EquipmentType1ID = @EquipmentType1ID AND EquipmentType2ID = @EquipmentType2ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@EquipmentType1ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType1.Code);
                command.Parameters.Add("@EquipmentType2ID", SqlDbType.VarChar).Value = SQLUtil.TrimNull(info.EquipmentType2.Code);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据ID删除富士I类与设备类型关联信息
        /// </summary>
        /// <param name="id">富士I类ID</param>
        public void DeleteFujiClass1LinkByID(int id)
        {
            sqlStr = " DELETE FROM jctFujiClass1EqpType WHERE FujiClass1ID = @FujiClass1ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@FujiClass1ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(id);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据ID删除富士I类信息
        /// </summary>
        /// <param name="id">富士I类ID</param>
        public void DeleteFujiClass1ByID(int id)
        {
            sqlStr = " DELETE FROM tblFujiClass1 WHERE ID = @ID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(id);

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
