using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class ContractInvoiceService : Service<ContractInvoice>, IContractInvoiceService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ContractInvoiceService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContractInvoice> CheckInsertOrUpdate(ContractInvoice model)
        {
            var ContractInvoiceObj = _mapper.Map<ContractInvoice>(model);
            var existingItem = _unitOfWork.ContractInvoiceRepository.GetMany(t => t.Id == model.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertContractInvoice(ContractInvoiceObj);
            }
            else
            {
                ContractInvoiceObj.Id = existingItem.Id;
                return await UpdateContractInvoice(ContractInvoiceObj, existingItem.Id);
            }
        }

        public async Task<ContractInvoice> UpdateContractInvoice(ContractInvoice updatedItem, long existingId)
        {
            var update = await _unitOfWork.ContractInvoiceRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<ContractInvoice> InsertContractInvoice(ContractInvoice ContractInvoiceObj)
        {
            var newItem = await _unitOfWork.ContractInvoiceRepository.AddAsync(ContractInvoiceObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<ContractInvoice> GetAll()
        {
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public List<ContractInvoice> GetAllByContract(long ContractId)
        {
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.ContractId == ContractId && t.DeletedOn == null && t.ClientInvoice.DeletedOn == null).Result.Include(t => t.ClientInvoice).ToList();
        }


        public ContractInvoice GetById(long Id)
        {
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public ContractInvoice GetByInvoiceId(long ClientInvoiceId)
        {
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.DeletedOn == null && t.ClientInvoiceId == ClientInvoiceId).Result.FirstOrDefault();
        }

        public List<ContractInvoice> GetByClient(long ClientId){
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.ClientInvoice.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
        }

        public async Task<ContractInvoice> DeleteContractInvoice(long Id)
        {
            var ContractInvoiceObj = _unitOfWork.ContractInvoiceRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (ContractInvoiceObj != null)
            {
                ContractInvoiceObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ContractInvoiceRepository.UpdateAsync(ContractInvoiceObj, ContractInvoiceObj.Id);
                await _unitOfWork.CommitAsync();            
            }
            return ContractInvoiceObj;
        }

        public List<ContractInvoice> GetListByIdList(List<long> ContractInvoiceIds)
        {
            return _unitOfWork.ContractInvoiceRepository.GetMany(t => t.ClientInvoice.DeletedOn == null && t.ClientInvoiceId != null && ContractInvoiceIds.Any(b => t.ClientInvoiceId.Value == b)).Result.Include(t => t.ClientInvoice).ToList();
        }
    }

    public partial interface IContractInvoiceService : IService<ContractInvoice>
    {
        Task<ContractInvoice> CheckInsertOrUpdate(ContractInvoice model);
        List<ContractInvoice> GetAll();
        Task<ContractInvoice> DeleteContractInvoice(long Id);
        ContractInvoice GetById(long Id);
        ContractInvoice GetByInvoiceId(long ClientInvoiceId);
        List<ContractInvoice> GetAllByContract(long ContractId);
        List<ContractInvoice> GetByClient(long ClientId);
        List<ContractInvoice> GetListByIdList(List<long> ContractInvoiceIds);
    }
}