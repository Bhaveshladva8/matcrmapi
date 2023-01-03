
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
    public partial class ContractArticleService : Service<ContractArticle>, IContractArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ContractArticleService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContractArticle> CheckInsertOrUpdate(ContractArticle ContractArticleObj)
        {
            ContractArticle? existingItem = null;
            if (ContractArticleObj.Id != null)
            {
                existingItem = _unitOfWork.ContractArticleRepository.GetMany(t => t.Id == ContractArticleObj.Id).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.ContractArticleRepository.GetMany(t => t.ContractId == ContractArticleObj.ContractId && t.ServiceArticleId == ContractArticleObj.ServiceArticleId).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertContractArticle(ContractArticleObj);
            }
            else
            {
                return await UpdateContractArticle(ContractArticleObj, existingItem.Id);
            }
        }

        public async Task<ContractArticle> InsertContractArticle(ContractArticle ContractArticleObj)
        {
            var newItem = await _unitOfWork.ContractArticleRepository.AddAsync(ContractArticleObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<ContractArticle> UpdateContractArticle(ContractArticle existingItem, long existingId)
        {
            await _unitOfWork.ContractArticleRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<ContractArticle> GetByContract(long contractId)
        {
            return _unitOfWork.ContractArticleRepository.GetMany(t => t.ContractId == contractId && t.ServiceArticle.DeletedOn == null).Result.Include(t => t.ServiceArticle).ToList();
        }

        public List<ContractArticle> GetByContractIds(List<long> contractIds)
        {
            return _unitOfWork.ContractArticleRepository.GetMany(t => t.Contract != null && t.Contract.DeletedOn == null && contractIds.Contains(t.ContractId.Value)).Result.Include(t => t.ServiceArticle).Include(t => t.Contract).ToList();
        }
        public ContractArticle GetById(long Id)
        {
            return _unitOfWork.ContractArticleRepository.GetMany(t => t.Id == Id).Result.Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).Include(t => t.Contract).Include(t => t.Contract.Currency).FirstOrDefault();
        }
        public async Task<List<ContractArticle>> DeleteByContract(long contractId)
        {
            var ExistingItemList = _unitOfWork.ContractArticleRepository.GetMany(u => u.ContractId == contractId).Result.ToList();
            foreach (var ExistingItem in ExistingItemList)
            {
                await _unitOfWork.ContractArticleRepository.DeleteAsync(ExistingItem);
            }
            await _unitOfWork.CommitAsync();
            return ExistingItemList;
        }

        public async Task<ContractArticle> Delete(long ContractId, long ServiceArticleId)
        {
            var ExistingItem = _unitOfWork.ContractArticleRepository.GetMany(u => u.ContractId == ContractId && u.ServiceArticleId == ServiceArticleId).Result.FirstOrDefault();

            await _unitOfWork.ContractArticleRepository.DeleteAsync(ExistingItem);

            await _unitOfWork.CommitAsync();
            return ExistingItem;
        }
        // public async Task<ContractArticle> DeleteByServiceArticle(long ServiceArticleId)
        // {
        //     var contractArticleObj = _unitOfWork.ContractArticleRepository.GetMany(u => u.ServiceArticleId == ServiceArticleId).Result.FirstOrDefault();
        //     if (contractArticleObj != null)
        //     {
        //         await _unitOfWork.ContractArticleRepository.DeleteAsync(contractArticleObj);
        //         await _unitOfWork.CommitAsync();
        //     }
        //     return contractArticleObj;
        // }

        public List<ContractArticle> GetByServiceArticle(long ServiceArticleId)
        {
            return _unitOfWork.ContractArticleRepository.GetMany(t => t.ServiceArticleId == ServiceArticleId).Result.Include(t => t.ServiceArticle).ToList();
        }
        public async Task<ContractArticle> DeletebyId(long Id)
        {
            var ExistingItem = _unitOfWork.ContractArticleRepository.GetMany(u => u.Id == Id).Result.FirstOrDefault();

            await _unitOfWork.ContractArticleRepository.DeleteAsync(ExistingItem);

            await _unitOfWork.CommitAsync();
            return ExistingItem;
        }
        public ContractArticle GetByContractAndServiceArticle(long contractId, long ServiceArticleId)
        {
            return _unitOfWork.ContractArticleRepository.GetMany(t => t.ServiceArticleId == ServiceArticleId && t.ContractId == contractId && t.ServiceArticle.DeletedOn == null).Result.Include(t => t.ServiceArticle).FirstOrDefault();
        }
    }

    public partial interface IContractArticleService : IService<ContractArticle>
    {
        Task<ContractArticle> CheckInsertOrUpdate(ContractArticle ContractArticleObj);
        List<ContractArticle> GetByContract(long contractId);
        List<ContractArticle> GetByContractIds(List<long> contractIds);
        ContractArticle GetById(long Id);
        Task<List<ContractArticle>> DeleteByContract(long contractId);
        Task<ContractArticle> Delete(long ContractId, long ServiceArticleId);
        //Task<ContractArticle> DeleteByServiceArticle(long ServiceArticleId);
        Task<ContractArticle> DeletebyId(long Id);
        List<ContractArticle> GetByServiceArticle(long ServiceArticleId);
        ContractArticle GetByContractAndServiceArticle(long contractId, long ServiceArticleId);
    }
}