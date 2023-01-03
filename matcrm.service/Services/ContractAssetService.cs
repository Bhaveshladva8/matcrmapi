using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class ContractAssetService : Service<ContractAsset>, IContractAssetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ContractAssetService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ContractAsset> CheckInsertOrUpdate(ContractAsset ContractAssetObj)
        {
            var existingItem = _unitOfWork.ContractAssetRepository.GetMany(t => t.Id == ContractAssetObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertContractAsset(ContractAssetObj);
            }
            else
            {
                ContractAssetObj.CreatedBy = existingItem.CreatedBy;
                ContractAssetObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateContractAsset(ContractAssetObj, existingItem.Id);
            }
        }
        public async Task<ContractAsset> InsertContractAsset(ContractAsset ContractAssetObj)
        {
            ContractAssetObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ContractAssetRepository.AddAsync(ContractAssetObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ContractAsset> UpdateContractAsset(ContractAsset existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ContractAssetRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<ContractAsset> GetByContractId(long ContractId)
        {
            return _unitOfWork.ContractAssetRepository.GetMany(t => t.ContractId == ContractId && t.DeletedOn == null).Result.Include(t => t.AssetsManufacturer).ToList();
        }
        public ContractAsset GetbyId(long Id)
        {
            return _unitOfWork.ContractAssetRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.AssetsManufacturer).FirstOrDefault();
        }
        public async Task<ContractAsset> DeleteById(long Id)
        {
            var existingItem = _unitOfWork.ContractAssetRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ContractAssetRepository.UpdateAsync(existingItem, existingItem.Id);
                await _unitOfWork.CommitAsync();
            }
            return existingItem;
        }
        public async Task<List<ContractAsset>> DeleteByContractId(long ContractId)
        {
            var contractAssetList = _unitOfWork.ContractAssetRepository.GetMany(t => t.ContractId == ContractId && t.DeletedOn == null).Result.ToList();
            if (contractAssetList != null && contractAssetList.Count() > 0)
            {
                foreach (var existingItem in contractAssetList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ContractAssetRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return contractAssetList;
        }
    }
    public partial interface IContractAssetService : IService<ContractAsset>
    {
        Task<ContractAsset> CheckInsertOrUpdate(ContractAsset model);
        List<ContractAsset> GetByContractId(long ContractId);
        ContractAsset GetbyId(long Id);
        Task<ContractAsset> DeleteById(long Id);
        Task<List<ContractAsset>> DeleteByContractId(long ContractId);
    }

}