using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class InvoiceMollieSubscriptionService : Service<InvoiceMollieSubscription>, IInvoiceMollieSubscriptionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceMollieSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceMollieSubscription> CheckInsertOrUpdate(InvoiceMollieSubscription model)
        {
            var InvoiceMollieSubscriptionObj = _mapper.Map<InvoiceMollieSubscription>(model);
            // var existingItem = _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.ClientId == obj.ClientId && t.SubscriptionId == obj.SubscriptionId && t.DeletedOn == null).Result.FirstOrDefault();
            var existingItem = _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.ClientId == InvoiceMollieSubscriptionObj.ClientId && t.ClientInvoiceId == InvoiceMollieSubscriptionObj.ClientInvoiceId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertInvoiceMollieSubscription(InvoiceMollieSubscriptionObj);
            }
            else
            {
                return await UpdateInvoiceMollieSubscription(existingItem, existingItem.Id);
            }
        }

        public async Task<InvoiceMollieSubscription> UpdateInvoiceMollieSubscription(InvoiceMollieSubscription updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.InvoiceMollieSubscriptionRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<InvoiceMollieSubscription> InsertInvoiceMollieSubscription(InvoiceMollieSubscription InvoiceMollieSubscriptionObj)
        {
            InvoiceMollieSubscriptionObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.InvoiceMollieSubscriptionRepository.AddAsync(InvoiceMollieSubscriptionObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<InvoiceMollieSubscription> GetAll()
        {
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public List<InvoiceMollieSubscription> GetAllStatusNotPaid(){
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.Status != OneClappContext.PaidPaymentStatus).Result.ToList();
        }

        public InvoiceMollieSubscription GetById(long Id)
        {
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public List<InvoiceMollieSubscription> GetByClient(long ClientId)
        {
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.ToList();
        }

        public InvoiceMollieSubscription GetByInvoiceId(long InvoiceId)
        {
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.ClientInvoiceId == InvoiceId).Result.FirstOrDefault();
        }

        public InvoiceMollieSubscription DeleteInvoiceMollieSubscription(long Id)
        {
            var InvoiceMollieSubscriptionObj = _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (InvoiceMollieSubscriptionObj != null)
            {
                InvoiceMollieSubscriptionObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.InvoiceMollieSubscriptionRepository.UpdateAsync(InvoiceMollieSubscriptionObj, InvoiceMollieSubscriptionObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<InvoiceMollieSubscription> DeleteByInvoiceId(long InvoiceId)
        {
            var existingItem = _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.ClientInvoiceId == InvoiceId).Result.FirstOrDefault();
            if (existingItem != null)
            {                
                existingItem.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.InvoiceMollieSubscriptionRepository.UpdateAsync(existingItem, existingItem.Id);
                await _unitOfWork.CommitAsync();
            }
            return existingItem;
        }

        public List<InvoiceMollieSubscription> GetListByInvoiceIdList(List<long> invoiceIds)
        {
            return _unitOfWork.InvoiceMollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.ClientInvoiceId != null && invoiceIds.Any(b => t.ClientInvoiceId.Value == b)).Result.ToList();
        }
    }

    public partial interface IInvoiceMollieSubscriptionService : IService<InvoiceMollieSubscription>
    {
        Task<InvoiceMollieSubscription> CheckInsertOrUpdate(InvoiceMollieSubscription model);
        List<InvoiceMollieSubscription> GetAll();
        InvoiceMollieSubscription DeleteInvoiceMollieSubscription(long Id);
        InvoiceMollieSubscription GetById(long Id);
        List<InvoiceMollieSubscription> GetByClient(long ClientId);
        InvoiceMollieSubscription GetByInvoiceId(long InvoiceId);
        List<InvoiceMollieSubscription> GetAllStatusNotPaid();
        Task<InvoiceMollieSubscription> DeleteByInvoiceId(long InvoiceId);
        List<InvoiceMollieSubscription> GetListByInvoiceIdList(List<long> invoiceIds);
    }
}