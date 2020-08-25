using BusinessObjects.Aspect;
using BusinessObjects.DataAccess;
using BusinessObjects.Domain;
using BusinessObjects.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// 合同manager
    /// </summary>
    [LoggingAspect(AspectPriority = 1)]
    public class ContractManager
    {
        private UploadFileManager uploadFileManager = new UploadFileManager();
        UploadFileManager fileManager = new UploadFileManager();
        private AuditManager auditManager = new AuditManager();

        private ContractDao contractDao = new ContractDao();
        private FileDao fileDao = new FileDao();
        private SupplierDao supplierDao = new SupplierDao();

        /// <summary>
        /// 获取合同列表信息
        /// </summary>
        /// <param name="status">合同状态</param>
        /// <param name="filterField">搜索字段</param>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="curRowNum">每页首条数据下标</param>
        /// <param name="pageSize">每页展示信息条数</param>
        /// <returns>合同列表信息</returns>
        public List<ContractInfo> QueryContracts(int status, string filterField, string filterText, string sortField, bool sortDirection, int curRowNum = 0, int pageSize = 0)
        {
            List<ContractInfo> infos = this.contractDao.QueryContracts(status, filterField, filterText, sortField, sortDirection, curRowNum, pageSize);
            if (infos.Count > 0)
            {
                List<ContractEqptInfo> contractEqpts = this.contractDao.GetContractEqpts(SQLUtil.GetIDListFromObjectList(infos));

                foreach (ContractInfo info in infos)
                {
                    info.Equipments = (from ContractEqptInfo temp in contractEqpts where temp.ContractID == info.ID select temp.Equipment).ToList();
                }
            }

            return infos;
        }

        /// <summary>
        /// App获取合同列表信息
        /// </summary>
        /// <param name="filterText">搜索框填写内容</param>
        /// <param name="curRowNum">每页首条数据下标</param>
        /// <param name="pageSize">每页展示信息条数</param>
        /// <returns>合同列表信息</returns>
        public List<ContractInfo> GetContracts(string filterText, int curRowNum = 0, int pageSize = 0)
        {
            List<ContractInfo> infos = this.contractDao.GetContracts(filterText, curRowNum, pageSize);
            if (infos.Count > 0)
            {
                List<ContractEqptInfo> contractEqpts = this.contractDao.GetContractEqpts(SQLUtil.GetIDListFromObjectList(infos));

                foreach (ContractInfo info in infos)
                {
                    info.Equipments = (from ContractEqptInfo temp in contractEqpts where temp.ContractID == info.ID select temp.Equipment).ToList();
                }
            }

            return infos;
        }

        /// <summary>
        /// 根据合同id获取合同信息
        /// </summary>
        /// <param name="id">合同编号</param>
        /// <returns>合同信息</returns>
        public ContractInfo GetContract(int id)
        {
            ContractInfo info = this.contractDao.GetContractByID(id);
            if (info != null)
            {
                UploadFileInfo fileInfo = this.fileDao.GetFile(ObjectTypes.Contract, info.ID);
                if (fileInfo != null)
                {
                    info.ContractFile = fileInfo;
                }
                else
                {
                    info.ContractFile = new UploadFileInfo();
                }
                info.Equipments = this.contractDao.GetContractEqpts(info.ID);
                info.Components = new List<ContractComponentInfo>();
                info.Consumables = new List<ContractConsumableInfo>();

                foreach(EquipmentInfo eqpt in info.Equipments)
                {
                    List<ComponentInfo> components = this.contractDao.GetContractComponents(info.ID, eqpt.ID);
                    
                    if(components.Count >= 1)
                    {
                        foreach(ComponentInfo comp in components)
                        {
                            ContractComponentInfo component = new ContractComponentInfo();
                            component.Equipment = eqpt;
                            component.ContractID = info.ID;
                            component.Component = comp;
                            info.Components.Add(component);
                        }
                    }

                    List<ConsumableInfo> consumables = this.contractDao.GetContractConsumables(info.ID, eqpt.ID);
                    if(consumables.Count >=1 )
                    {
                        foreach (ConsumableInfo cons in consumables)
                        {
                            ContractConsumableInfo consumable = new ContractConsumableInfo();
                            consumable.Equipment = eqpt;
                            consumable.ContractID = info.ID;
                            consumable.Consumable = cons;
                            info.Consumables.Add(consumable);
                        }
                    }
                        
                }
                
            }

            return info;
        }

        /// <summary>
        /// 获取各状态的合同数量
        /// </summary>
        /// <returns>各状态的合同数量</returns>
        public Dictionary<string, int> GetContractCount()
        {

            Dictionary<string, int> counts = new Dictionary<string, int>();
            counts.Add("Expired", contractDao.GetContractCount(ContractInfo.Statuses.Expired));
            counts.Add("Active", contractDao.GetContractCount(ContractInfo.Statuses.Active));
            counts.Add("WillExpire", contractDao.GetContractCount(ContractInfo.Statuses.WillExpire));
            counts.Add("Pending", contractDao.GetContractCount(ContractInfo.Statuses.Pending));

            return counts;
        }

        /// <summary>
        /// 保存合同
        /// </summary>
        /// <param name="contract">合同信息</param>
        /// <param name="files">合同附件信息</param>
        /// <param name="userInfo">操作用户信息</param>
        /// <returns>合同编号</returns>
        [TransactionAspect]
        public int SaveContract(ContractInfo contract, List<UploadFileInfo> files, UserInfo userInfo)
        {
            if (contract.ID > 0)
            {
                ContractInfo existingInfo = this.contractDao.GetContractByID(contract.ID);
                existingInfo.Equipments = this.contractDao.GetContractEqpts(contract.ID);
                contract.Supplier.Name = contract.Supplier.ID == 0 ? "" : this.supplierDao.GetSupplier(contract.Supplier.ID).Name;
                DataTable dtField = existingInfo.GetChangedFields(contract);

                this.contractDao.DeleteContractEqpt(contract.ID);
                this.contractDao.DeleteContractComponent(contract.ID);
                this.contractDao.DeleteContractConsumable(contract.ID);
                if (dtField.Rows.Count > 0)
                {
                    this.contractDao.UpdateContract(contract);

                    this.auditManager.AddAuditLog(userInfo.ID, ObjectTypes.Contract, contract.ID, dtField);
                }
            }
            else
            {
                contract.ID = this.contractDao.AddContract(contract);
                if (files != null && files.Count > 0)
                {
                    foreach (UploadFileInfo file in files)
                    {
                        file.ObjectID = contract.ID;
                        file.ObjectTypeId = ObjectTypes.Contract;
                        this.fileManager.SaveUploadFile(file);
                    }
                }
            }
            if (contract.Equipments != null)
            {
                foreach (EquipmentInfo eqptInfo in contract.Equipments)
                {
                    this.contractDao.AddContractEqpt(contract.ID, eqptInfo.ID);
                }
            }

            if (contract.Components != null)
            {
                foreach (ContractComponentInfo component in contract.Components)
                {
                    this.contractDao.AddContractComponent(contract.ID,component.Equipment.ID, component.Component.ID);
                }
            }

            if (contract.Consumables != null)
            {
                foreach (ContractConsumableInfo consumable in contract.Consumables)
                {
                    this.contractDao.AddContractConsumable(contract.ID, consumable.Equipment.ID, consumable.Consumable.ID);
                }
            }

            return contract.ID;
        }
    }
}
