

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class InvoiceMollieCustomerService : Service<InvoiceMollieCustomer>, IInvoiceMollieCustomerService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceMollieCustomerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceMollieCustomer> CheckInsertOrUpdate(InvoiceMollieCustomer model)
        {
            var InvoiceMollieCustomerObj = _mapper.Map<InvoiceMollieCustomer>(model);
            var existingItem = _unitOfWork.InvoiceMollieCustomerRepository.GetMany(t => t.ClientId == InvoiceMollieCustomerObj.ClientId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertInvoiceMollieCustomer(InvoiceMollieCustomerObj);
            }
            else
            {
                InvoiceMollieCustomerObj.CreatedOn = existingItem.CreatedOn;
                InvoiceMollieCustomerObj.ClientId = existingItem.ClientId;
                InvoiceMollieCustomerObj.CustomerId = existingItem.CustomerId;
                InvoiceMollieCustomerObj.Id = existingItem.Id;
                return await UpdateInvoiceMollieCustomer(InvoiceMollieCustomerObj, existingItem.Id);
            }
        }

        public async Task<InvoiceMollieCustomer> UpdateInvoiceMollieCustomer(InvoiceMollieCustomer updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.InvoiceMollieCustomerRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<InvoiceMollieCustomer> InsertInvoiceMollieCustomer(InvoiceMollieCustomer InvoiceMollieCustomerObj)
        {
            InvoiceMollieCustomerObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.InvoiceMollieCustomerRepository.AddAsync(InvoiceMollieCustomerObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<InvoiceMollieCustomer> GetAll()
        {
            return _unitOfWork.InvoiceMollieCustomerRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public InvoiceMollieCustomer GetById(long Id)
        {
            return _unitOfWork.InvoiceMollieCustomerRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.Client).FirstOrDefault();
        }

        public InvoiceMollieCustomer GetByClient(long ClientId)
        {
            return _unitOfWork.InvoiceMollieCustomerRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.Include(t => t.Client).FirstOrDefault();
        }

        public InvoiceMollieCustomer DeleteInvoiceMollieCustomer(long Id)
        {
            var InvoiceMollieCustomerObj = _unitOfWork.InvoiceMollieCustomerRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (InvoiceMollieCustomerObj != null)
            {
                InvoiceMollieCustomerObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.InvoiceMollieCustomerRepository.UpdateAsync(InvoiceMollieCustomerObj, InvoiceMollieCustomerObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<InvoiceMollieCustomer>> DeleteByClientId(long ClientId)
        {
            var invoiceMollieCustomerList = _unitOfWork.InvoiceMollieCustomerRepository.GetMany(u => u.ClientId == ClientId && u.DeletedOn == null).Result.ToList();
            if (invoiceMollieCustomerList != null && invoiceMollieCustomerList.Count > 0)
            {
                foreach (var item in invoiceMollieCustomerList)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.InvoiceMollieCustomerRepository.UpdateAsync(item, item.Id);
                    await _unitOfWork.CommitAsync();
                }
            }
            return invoiceMollieCustomerList;
        }
    }

    public partial interface IInvoiceMollieCustomerService : IService<InvoiceMollieCustomer>
    {
        Task<InvoiceMollieCustomer> CheckInsertOrUpdate(InvoiceMollieCustomer model);
        List<InvoiceMollieCustomer> GetAll();
        InvoiceMollieCustomer DeleteInvoiceMollieCustomer(long Id);
        InvoiceMollieCustomer GetById(long Id);
        InvoiceMollieCustomer GetByClient(long ClientId);
        Task<List<InvoiceMollieCustomer>> DeleteByClientId(long ClientId);
    }
}