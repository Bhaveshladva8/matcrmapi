using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ContractActivityService : Service<ContractActivity>, IContractActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ContractActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ContractActivity> CheckInsertOrUpdate(ContractActivity contractActivityObj)
        {
            return await InsertContractActivity(contractActivityObj);
        }
        public async Task<ContractActivity> InsertContractActivity(ContractActivity contractActivityObj)
        {
            contractActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ContractActivityRepository.AddAsync(contractActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ContractActivity> UpdateContractActivity(ContractActivity existingItem, int existingId)
        {
            await _unitOfWork.ContractActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ContractActivity> GetAllByContractId(long contractId)
        {
            return _unitOfWork.ContractActivityRepository.GetMany(t => t.ContractId == contractId).Result.ToList();
        }

        public ContractActivity GetById(long Id)
        {
            return _unitOfWork.ContractActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }
    }
    public partial interface IContractActivityService : IService<ContractActivity>
    {
        Task<ContractActivity> CheckInsertOrUpdate(ContractActivity model);
        List<ContractActivity> GetAllByContractId(long contractId);
        ContractActivity GetById(long Id);
    }
}