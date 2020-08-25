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
    /// 合同dao
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    [ConnectionAspect(AspectPriority = 2, AttributeTargetTypeAttributes = MulticastAttributes.Public)]
    public class ContractDao : BaseDao
    {
        #region "tblContract"
        /// <summary>
        /// 获取合同列表
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>合同列表</returns>
        public List<ContractInfo> QueryContracts(int status, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum= 0, int pageSize = 0) 
        {
            List<ContractInfo> contracts = new List<ContractInfo>();
            sqlStr = "SELECT c.*,s.Name supplierName " +
                    " FROM tblContract c " +
                    " LEFT JOIN tblSupplier s ON c.SupplierID=s.ID " +
                    " WHERE 1=1 ";
            if (status != 0) sqlStr += ContractInfo.Statuses.GetSqlFilter(status);

            if (!string.IsNullOrEmpty(filterText))
            {
                if (filterField.Equals("e.ID") || filterField.Equals("e.Name") || filterField.Equals("e.SerialCode"))
                {
                    sqlStr += " AND EXISTS (SELECT j.EquipmentID FROM jctContractEqpt j INNER JOIN tblEquipment as e ON j.EquipmentID = e.ID WHERE j.ContractID = c.ID";
                    sqlStr += GetFieldFilterClause(filterField);
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += GetFieldFilterClause(filterField);
                }
            }

            sqlStr += GenerateSortClause(sortDirection, sortField, "c.ID");

            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);
            using(SqlCommand command=ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText))AddFieldFilterParam(command, filterField, filterText);

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        contracts.Add(new ContractInfo(dr));
                    }
                }
            }
            return contracts;
        }
        /// <summary>
        /// App获取合同列表信息
        /// </summary>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="curRowNum">当前页数第一个数据的位置</param>
        /// <param name="pageSize">一页几条数据</param>
        /// <returns>合同列表信息</returns>
        public List<ContractInfo> GetContracts(string filterText, int curRowNum = 0, int pageSize = 0)
        {
            List<ContractInfo> infos = new List<ContractInfo>();

            sqlStr = "SELECT c.*,s.Name supplierName " +
                    " FROM tblContract c " +
                    " LEFT JOIN tblSupplier s ON c.SupplierID=s.ID " +
                    " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(filterText))
                sqlStr += " AND ( UPPER(c.Name) LIKE @FilterText OR UPPER(c.ID) LIKE @FilterText OR UPPER(c.ContractNum) LIKE @FilterText ) ";
            sqlStr += " ORDER BY ID";
            sqlStr = AppendLimitClause(sqlStr, curRowNum, pageSize);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!string.IsNullOrEmpty(filterText))
                    command.Parameters.Add("@FilterText", SqlDbType.NVarChar).Value = "%" + filterText.ToUpper() + "%";

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ContractInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取合同数量
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索条件</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <returns>合同数量</returns>
        public int QueryContractsCount(int status, string filterField, string filterText)
        {
            List<ContractInfo> contracts = new List<ContractInfo>();
            sqlStr = "SELECT COUNT(c.ID) FROM tblContract c " +
                    " LEFT JOIN tblSupplier s ON c.SupplierID=s.ID " +
                    " WHERE 1=1 ";

            if (status != 0) sqlStr += ContractInfo.Statuses.GetSqlFilter(status);

            if (!string.IsNullOrEmpty(filterText))
            {
                if (filterField.Equals("e.ID") || filterField.Equals("e.Name") || filterField.Equals("e.SerialCode"))
                {
                    sqlStr += " AND EXISTS (SELECT j.EquipmentID FROM jctContractEqpt j INNER JOIN tblEquipment as e ON j.EquipmentID = e.ID WHERE j.ContractID = c.ID";
                    sqlStr += GetFieldFilterClause(filterField);
                    sqlStr += ")";
                }
                else
                {
                    sqlStr += GetFieldFilterClause(filterField);
                }
            }
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                if (!String.IsNullOrEmpty(filterText)) AddFieldFilterParam(command, filterField, filterText);

                return GetCount(command);
            }
        }
        /// <summary>
        /// 新增合同
        /// </summary>
        /// <param name="contract">合同信息</param>
        /// <returns>合同ID</returns>
        public int AddContract(ContractInfo contract)
        {
            sqlStr = "INSERT INTO tblContract(SupplierID,ContractNum,"+
                            "Name,TypeID,ScopeID,ScopeComments,Amount,ProjectNum,StartDate,EndDate,Comments)" +
                    "VALUES(@SupplierID,@ContractNum,@Name,@TypeID,@ScopeID,@ScopeComments,@Amount," +
                            "@ProjectNum,@StartDate,@EndDate,@Comments)"+
                            "SELECT @@IDENTITY";
            using(SqlCommand command=ConnectionUtil.GetCommand(sqlStr)){
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Supplier.ID);
                command.Parameters.Add("@ContractNum", SqlDbType.VarChar).Value = SQLUtil.TrimNull(contract.ContractNum);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.Name);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Type.ID);
                command.Parameters.Add("@ScopeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Scope.ID);
                command.Parameters.Add("@ScopeComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.ScopeComments);
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = contract.Amount;
                command.Parameters.Add("@ProjectNum", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.ProjectNum);
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(contract.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(contract.EndDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.Comments);

                contract.ID = SQLUtil.ConvertInt(command.ExecuteScalar());
            }
            return contract.ID;
        }
        /// <summary>
        /// 修改合同
        /// </summary>
        /// <param name="contract">合同信息</param>
        public void UpdateContract(ContractInfo contract)
        {
            sqlStr = " UPDATE tblContract SET SupplierID=@SupplierID, " +
                     " ContractNum=@ContractNum,Name=@Name,TypeID=@TypeID,ScopeID=@ScopeID,ScopeComments=@ScopeComments, " +
                     " Amount=@Amount,ProjectNum=@ProjectNum,StartDate=@StartDate,EndDate=@EndDate,Comments=@Comments " +
                     " WHERE ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Supplier.ID);
                command.Parameters.Add("@ContractNum", SqlDbType.VarChar).Value = SQLUtil.TrimNull(contract.ContractNum);
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.Name);
                command.Parameters.Add("@TypeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Type.ID);
                command.Parameters.Add("@ScopeID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.Scope.ID);
                command.Parameters.Add("@ScopeComments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.ScopeComments);
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = contract.Amount;
                command.Parameters.Add("@ProjectNum", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.ProjectNum);
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(contract.StartDate);
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = SQLUtil.ConvertDateTime(contract.EndDate);
                command.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = SQLUtil.TrimNull(contract.Comments);
                command.Parameters.Add("@ID", SqlDbType.Int).Value = SQLUtil.ConvertInt(contract.ID);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 根据id获取合同信息
        /// </summary>
        /// <param name="contractID">合同ID</param>
        /// <returns>合同信息</returns>
        public ContractInfo GetContractByID(int contractID)
        {
            sqlStr = "SELECT c.*,s.Name supplierName " +
                    " FROM tblContract c " +
                    " LEFT JOIN tblSupplier s ON c.SupplierID=s.ID " +
                    " WHERE c.ID=@ID ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ID", SqlDbType.Int).Value = contractID;
                DataRow dr = GetDataRow(command);
                if (dr != null)
                    return new ContractInfo(dr);
                else
                    return null;
            }
        }
        /// <summary>
        /// 根据合同状态编号获取合同数量
        /// </summary>
        /// <param name="statusID">合同状态ID</param>
        /// <returns>该状态合同数量</returns>
        public int GetContractCount(int statusID)
        {
            sqlStr = "SELECT COUNT(ID)" +
                    " FROM tblContract c " +
                    " WHERE 1=1 " + ContractInfo.Statuses.GetSqlFilter(statusID);

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                return GetCount(command);
            }
        }
        /// <summary>
        /// 删除合同绑定的设备信息
        /// </summary>
        /// <param name="contractID">合同ID</param>
        public void DeleteContractEqpt(int contractID)
        {
            sqlStr = " DELETE FROM jctContractEqpt WHERE ContractID = @ContractID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;

                command.ExecuteNonQuery();
            } 
        }

        #endregion

        #region jctContractEqpt
        /// <summary>
        /// 新增合同设备关联信息
        /// </summary>
        /// <param name="contractID">合同ID</param>
        /// <param name="equipmentID">设备ID</param>
        public void AddContractEqpt(int contractID, int equipmentID)
        {
            sqlStr = "INSERT INTO jctContractEqpt (ContractID,EquipmentID) " +
                     " VALUES(@ContractID,@EquipmentID); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 根据合同id获取有效的合同关联的设备信息
        /// </summary>
        /// <param name="contractID">合同ID</param>
        /// <returns>有效的合同及关联的设备信息</returns>
        public List<EquipmentInfo> GetContractEqpts(int contractID)
        {
            List<EquipmentInfo> infos = new List<EquipmentInfo>();

            sqlStr = "SELECT e.*, s.Name AS SupplierName, su.Name AS ManufacturerName, c.ContractID,ct.ScopeID,ct.ScopeComments FROM jctContractEqpt j " +
                    " INNER JOIN tblEquipment AS e ON e.ID = j.EquipmentID" +
                     " LEFT JOIN tblSupplier AS s ON e.SupplierID=s.ID " +
                     " LEFT JOIN tblSupplier AS su ON e.ManufacturerID=su.ID " +
                     " LEFT JOIN v_ActiveContract AS c on c.EquipmentID = e.ID" +
                     " LEFT JOIN tblContract AS ct on ct.ID = c.ContractID" +
                     " WHERE j.ContractID = @ContractID ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new EquipmentInfo(dr));
                    }
                }
            }

            return infos;
        }
        /// <summary>
        /// 获取合同及关联的设备信息
        /// </summary>
        /// <param name="contractIDs">合同ID</param>
        /// <returns>合同及关联的设备信息</returns>
        public List<ContractEqptInfo> GetContractEqpts(List<int> contractIDs)
        {
            List<ContractEqptInfo> infos = new List<ContractEqptInfo>();

            sqlStr = "SELECT e.ID ,e.Name ,e.DepartmentID ,e.SerialCode , j.ContractID FROM jctContractEqpt j " +
                    " INNER JOIN tblEquipment AS e ON e.ID = j.EquipmentID" +
                    " WHERE j.ContractID in (" + (string.IsNullOrEmpty(SQLUtil.ConvertToInStr(contractIDs)) ? "null" : SQLUtil.ConvertToInStr(contractIDs)) + ") ";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                using (DataTable dt = GetDataTable(command))
                {
                    ContractEqptInfo info = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        info = new ContractEqptInfo();
                        info.ContractID = SQLUtil.ConvertInt(dr["ContractID"]);
                        info.Equipment.ID = SQLUtil.ConvertInt(dr["ID"]);
                        info.Equipment.Name = SQLUtil.TrimNull(dr["Name"]);
                        info.Equipment.SerialCode = SQLUtil.TrimNull(dr["SerialCode"]);
                        info.Equipment.Department.ID = SQLUtil.ConvertInt(dr["DepartmentID"]);
                        info.Equipment.Department.Name = Manager.LookupManager.GetDepartmentDesc(info.Equipment.Department.ID);
                        infos.Add(info);
                    }
                }
            }

            return infos;
        }

        #endregion

        #region component
        /// <summary>
        /// 添加合同零件
        /// </summary>
        /// <param name="contractID">合同id</param>
        /// <param name="equipmentID">设备id</param>
        /// <param name="componentID">零件id</param>
        public void AddContractComponent(int contractID, int equipmentID, int componentID)
        {
            sqlStr = "INSERT INTO jctContractComponent (ContractID,EquipmentID,ComponentID) " +
                     " VALUES(@ContractID,@EquipmentID,@ComponentID); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                command.Parameters.Add("@ComponentID", SqlDbType.Int).Value = componentID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除零件信息
        /// </summary>
        /// <param name="contractID">合同id</param>
        public void DeleteContractComponent(int contractID)
        {
            sqlStr = " DELETE FROM jctContractComponent WHERE ContractID = @ContractID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取合同零件信息
        /// </summary>
        /// <param name="contractID">合同id</param>
        /// <param name="equipmentID">设备id</param>
        /// <returns>零件信息</returns>
        public List<ComponentInfo> GetContractComponents(int contractID, int equipmentID)
        {
            List<ComponentInfo> infos = new List<ComponentInfo>();

            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM jctContractComponent j " +
                    " INNER JOIN tblComponent AS c ON c.ID = j.ComponentID" +
                    " LEFT JOIN tblFujiClass2 f ON f.ID = c.FujiClass2ID " +
                    " WHERE j.ContractID = @ContractID " +
                    " AND j.EquipmentID = @EquipmentID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ComponentInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion

        #region consumable
        /// <summary>
        /// 添加合同耗材信息
        /// </summary>
        /// <param name="contractID">合同id</param>
        /// <param name="equipmentID">设备id</param>
        /// <param name="consumableID">耗材id</param>
        public void AddContractConsumable(int contractID, int equipmentID, int consumableID)
        {
            sqlStr = "INSERT INTO jctContractConsumable (ContractID,EquipmentID,ConsumableID) " +
                     " VALUES(@ContractID,@EquipmentID,@ConsumableID); ";
            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                command.Parameters.Add("@ConsumableID", SqlDbType.Int).Value = consumableID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除合同耗材
        /// </summary>
        /// <param name="contractID">合同id</param>
        public void DeleteContractConsumable(int contractID)
        {
            sqlStr = " DELETE FROM jctContractConsumable WHERE ContractID = @ContractID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;

                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 获取合同耗材信息
        /// </summary>
        /// <param name="contractID">合同id</param>
        /// <param name="equipmentID">设备id</param>
        /// <returns>合同耗材信息</returns>
        public List<ConsumableInfo> GetContractConsumables(int contractID, int equipmentID)
        {
            List<ConsumableInfo> infos = new List<ConsumableInfo>();

            sqlStr = "SELECT c.*, f.Name FujiClass2Name FROM jctContractConsumable j " +
                    " INNER JOIN tblConsumable AS c ON c.ID = j.ConsumableID " +
                    " LEFT JOIN tblFujiClass2 f ON f.ID = c.FujiClass2ID " +
                    " WHERE j.ContractID = @ContractID " +
                    " AND j.EquipmentID = @EquipmentID";

            using (SqlCommand command = ConnectionUtil.GetCommand(sqlStr))
            {
                command.Parameters.Add("@ContractID", SqlDbType.Int).Value = contractID;
                command.Parameters.Add("@EquipmentID", SqlDbType.Int).Value = equipmentID;

                using (DataTable dt = GetDataTable(command))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        infos.Add(new ConsumableInfo(dr));
                    }
                }
            }

            return infos;
        }
        #endregion

    }
}
