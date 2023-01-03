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
    public partial class ContractTypeService : Service<ContractType>, IContractTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ContractTypeService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContractType> CheckInsertOrUpdate(ContractType ContractTypeObj)
        {
            var existingItem = _unitOfWork.ContractTypeRepository.GetMany(t => t.Id == ContractTypeObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertContractType(ContractTypeObj);
            }
            else
            {
                ContractTypeObj.CreatedBy = existingItem.CreatedBy;
                ContractTypeObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateContractType(ContractTypeObj, existingItem.Id);
            }
        }

        public async Task<ContractType> InsertContractType(ContractType ContractTypeObj)
        {
            ContractTypeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ContractTypeRepository.AddAsync(ContractTypeObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<ContractType> UpdateContractType(ContractType existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ContractTypeRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<ContractType> GetByTenant(int tenantId)
        {
            return _unitOfWork.ContractTypeRepository.GetMany(t => t.CreatedUser != null && t.CreatedUser.TenantId == tenantId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).ToList();
        }
        public ContractType GetById(long Id)
        {
            return _unitOfWork.ContractTypeRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }
        public async Task<ContractType> DeleteContractType(long Id, int deletedby)
        {
            var ContractTypeObj = _unitOfWork.ContractTypeRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (ContractTypeObj != null)
            {
                ContractTypeObj.DeletedBy = deletedby;
                ContractTypeObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ContractTypeRepository.UpdateAsync(ContractTypeObj, ContractTypeObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return ContractTypeObj;
        }
        public List<ContractType> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.ContractTypeRepository.GetMany(t => t.DeletedOn == null && (t.CreatedBy == null || t.CreatedUser.TenantId == tenantId)).Result.Include(t => t.CreatedUser).ToList();
        }

    }

    public partial interface IContractTypeService : IService<ContractType>
    {
        Task<ContractType> CheckInsertOrUpdate(ContractType ContractTypeObj);
        List<ContractType> GetByTenant(int tenantId);
        ContractType GetById(long Id);
        Task<ContractType> DeleteContractType(long Id, int deletedby);
        List<ContractType> GetAllByTenant(int tenantId);
    }
}