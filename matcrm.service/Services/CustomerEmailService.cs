using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerEmailService : Service<CustomerEmail>, ICustomerEmailService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerEmailService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public CustomerEmail CheckInsertOrUpdate (CustomerEmailDto model) {
            var customerEmailObj = _mapper.Map<CustomerEmail> (model);
            var existingItem = _unitOfWork.CustomerEmailRepository.GetMany (t => t.CustomerId == customerEmailObj.CustomerId && t.TenantId == customerEmailObj.TenantId && t.Id == customerEmailObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertCustomerEmail (customerEmailObj);
            } else {
                customerEmailObj.CreatedOn = existingItem.CreatedOn;
                customerEmailObj.CreatedBy = existingItem.CreatedBy;
                customerEmailObj.TenantId = existingItem.TenantId;
                customerEmailObj.CustomerId = existingItem.CustomerId;
                customerEmailObj.Id = existingItem.Id;
                return UpdateCustomerEmail (customerEmailObj, existingItem.Id);
            }
        }

        public CustomerEmail UpdateCustomerEmail (CustomerEmail updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.CustomerEmailRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public CustomerEmail InsertCustomerEmail (CustomerEmail customerEmailObj) {
            customerEmailObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.CustomerEmailRepository.Add (customerEmailObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerEmail> GetAll () {
            return _unitOfWork.CustomerEmailRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerEmail GetById (long Id) {
            return _unitOfWork.CustomerEmailRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerEmail> GetByTenant (int tenantId) {
            return _unitOfWork.CustomerEmailRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerEmail> GetByCustomer (long customerId) {
            return _unitOfWork.CustomerEmailRepository.GetMany (t => t.CustomerId == customerId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerEmail> GetByUser (int userId) {
            return _unitOfWork.CustomerEmailRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerEmail DeleteCustomerEmail (CustomerEmailDto model) {
            var customerEmailObj = _mapper.Map<CustomerEmail> (model);
            var existingItem = _unitOfWork.CustomerEmailRepository.GetMany (t => t.Id == customerEmailObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CustomerEmailRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

        public List<CustomerEmail> DeleteByCustomer (long CustomerId) {
            var customerEmailList = _unitOfWork.CustomerEmailRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
            if(customerEmailList != null && customerEmailList.Count() > 0)
            {
                foreach (var existingItem in customerEmailList) {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.CustomerEmailRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync ();
            }
            return customerEmailList;
        }
    }

    public partial interface ICustomerEmailService : IService<CustomerEmail> {
        CustomerEmail CheckInsertOrUpdate (CustomerEmailDto model);
        List<CustomerEmail> GetAll ();
        List<CustomerEmail> GetByTenant (int tenantId);
        List<CustomerEmail> GetByCustomer (long customerId);
        CustomerEmail DeleteCustomerEmail (CustomerEmailDto model);
        CustomerEmail GetById (long Id);
        List<CustomerEmail> GetByUser (int userId);
        List<CustomerEmail> DeleteByCustomer (long CustomerId);
    }
}