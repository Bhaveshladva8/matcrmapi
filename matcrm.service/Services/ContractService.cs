using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;
using matcrm.data;
using matcrm.data.Models.Request;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class ContractService : Service<Contract>, IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ContractService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Contract> CheckInsertOrUpdate(Contract ContractObj)
        {
            var existingItem = _unitOfWork.ContractRepository.GetMany(t => t.Id == ContractObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertContract(ContractObj);
            }
            else
            {
                ContractObj.CreatedBy = existingItem.CreatedBy;
                ContractObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateContract(ContractObj, existingItem.Id);
            }
        }

        public async Task<Contract> InsertContract(Contract ContractObj)
        {
            ContractObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ContractRepository.AddAsync(ContractObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<Contract> UpdateContract(Contract existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ContractRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<Contract> GetByTenant(int tenantId)
        {
            return _unitOfWork.ContractRepository.GetMany(t => t.CreatedUser != null && t.CreatedUser.TenantId == tenantId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).ToList();
        }

        public List<Contract> GetAll()
        {
            return _unitOfWork.ContractRepository.GetMany(t => t.DeletedOn == null).Result.Include(t => t.Client).Include(t => t.InvoiceInterval).ToList();
        }

        public List<Contract> GetByClient(long clientId)
        {
            return _unitOfWork.ContractRepository.GetMany(t => t.ClientId == clientId && t.DeletedOn == null).Result.Include(t => t.ContractType).Include(t => t.InvoiceInterval).Include(t => t.Currency).Include(t => t.Status).ToList();
        }
        public Contract GetById(long Id)
        {
            return _unitOfWork.ContractRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.ContractType).Include(t => t.Currency).Include(t => t.Status).Include(t => t.InvoiceInterval).FirstOrDefault();
        }
        public async Task<Contract> DeleteContract(long Id, int deletedby)
        {
            var ContractObj = _unitOfWork.ContractRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (ContractObj != null)
            {
                ContractObj.DeletedBy = deletedby;
                ContractObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ContractRepository.UpdateAsync(ContractObj, ContractObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return ContractObj;
        }

        public bool IsExistOrNot(string contractName, long clientId)
        {
            var existingItem = _unitOfWork.ContractRepository.GetMany(t => t.DeletedOn == null && t.ClientId == clientId && t.Name.ToLower() == contractName.ToLower()).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public partial interface IContractService : IService<Contract>
    {
        Task<Contract> CheckInsertOrUpdate(Contract ContractObj);
        List<Contract> GetByTenant(int tenantId);
        Contract GetById(long Id);
        List<Contract> GetAll();
        Task<Contract> DeleteContract(long Id, int deletedby);
        List<Contract> GetByClient(long clientId);
        bool IsExistOrNot(string contractName, long clientId);

    }
}