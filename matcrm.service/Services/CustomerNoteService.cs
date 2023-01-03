using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerNoteService : Service<CustomerNote>, ICustomerNoteService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerNoteService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerNote> CheckInsertOrUpdate (CustomerNoteDto model) {
            var customerNoteObj = _mapper.Map<CustomerNote> (model);
            var existingItem = _unitOfWork.CustomerNoteRepository.GetMany (t => t.Id == customerNoteObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomerNote (customerNoteObj);
            } else {
                customerNoteObj.CreatedOn = existingItem.CreatedOn;
                customerNoteObj.CreatedBy = existingItem.CreatedBy;
                customerNoteObj.TenantId = existingItem.TenantId;
                customerNoteObj.CustomerId = existingItem.CustomerId;
                return await UpdateCustomerNote (customerNoteObj, existingItem.Id);
            }
        }

        public async Task<CustomerNote> UpdateCustomerNote (CustomerNote updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CustomerNoteRepository.UpdateAsync (updatedItem, existingId);
            await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<CustomerNote> InsertCustomerNote (CustomerNote customerNoteObj) {
            customerNoteObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerNoteRepository.AddAsync (customerNoteObj);
            await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerNote> GetAll () {
            return _unitOfWork.CustomerNoteRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerNote GetById (long Id) {
            return _unitOfWork.CustomerNoteRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerNote> GetByTenant (int tenantId) {
            return _unitOfWork.CustomerNoteRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerNote> GetByCustomer (long customerId) {
            return _unitOfWork.CustomerNoteRepository.GetMany (t => t.CustomerId == customerId && t.IsDeleted == false).Result.OrderByDescending (t => t.CreatedOn).ToList ();
        }

        public List<CustomerNote> GetByUser (int userId) {
            return _unitOfWork.CustomerNoteRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerNote DeleteCustomerNote (CustomerNoteDto model) {
            var customerNoteObj = _mapper.Map<CustomerNote> (model);
            var existingItem = _unitOfWork.CustomerNoteRepository.GetMany (t => t.Id == customerNoteObj.Id  && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CustomerNoteRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

        public async Task<List<CustomerNote>> DeleteByCustomer (long CustomerId) {
            var customerNoteList = _unitOfWork.CustomerNoteRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
            if(customerNoteList != null && customerNoteList.Count() > 0)
            {
                foreach (var existingItem in customerNoteList) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.CustomerNoteRepository.UpdateAsync (existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync ();
            }
            
            return customerNoteList;
        }
    }

    public partial interface ICustomerNoteService : IService<CustomerNote> {
        Task<CustomerNote> CheckInsertOrUpdate (CustomerNoteDto model);
        List<CustomerNote> GetAll ();
        List<CustomerNote> GetByTenant (int tenantId);
        List<CustomerNote> GetByCustomer (long customerId);
        CustomerNote DeleteCustomerNote (CustomerNoteDto model);
        Task<List<CustomerNote>> DeleteByCustomer (long CustomerId);
        CustomerNote GetById (long Id);
        List<CustomerNote> GetByUser (int userId);
    }
}