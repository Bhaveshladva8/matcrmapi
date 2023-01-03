using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerPhoneService : Service<CustomerPhone>, ICustomerPhoneService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerPhoneService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerPhone> CheckInsertOrUpdate (CustomerPhoneDto model) {
            var customerPhoneObj = _mapper.Map<CustomerPhone> (model);
            var existingItem = _unitOfWork.CustomerPhoneRepository.GetMany (t => t.Id == customerPhoneObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomerPhone (customerPhoneObj);
            } else {
                customerPhoneObj.CreatedOn = existingItem.CreatedOn;
                customerPhoneObj.CreatedBy = existingItem.CreatedBy;
                customerPhoneObj.TenantId = existingItem.TenantId;
                customerPhoneObj.CustomerId = existingItem.CustomerId;
                return await UpdateCustomerPhone (customerPhoneObj, existingItem.Id);
            }
        }

        public async Task<CustomerPhone> UpdateCustomerPhone (CustomerPhone updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CustomerPhoneRepository.UpdateAsync (updatedItem, existingId);
            await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<CustomerPhone> InsertCustomerPhone (CustomerPhone customerPhoneObj) {
            customerPhoneObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerPhoneRepository.AddAsync (customerPhoneObj);
            await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerPhone> GetAll () {
            return _unitOfWork.CustomerPhoneRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerPhone GetById (long Id) {
            return _unitOfWork.CustomerPhoneRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerPhone> GetByTenant (int tenantId) {
            return _unitOfWork.CustomerPhoneRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerPhone> GetByCustomer (long customerId) {
            return _unitOfWork.CustomerPhoneRepository.GetMany (t => t.CustomerId == customerId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerPhone> GetByUser (int userId) {
            return _unitOfWork.CustomerPhoneRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerPhone DeleteCustomerPhone (CustomerPhoneDto model) {
            var customerPhoneObj = _mapper.Map<CustomerPhone> (model);
            var existingItem = _unitOfWork.CustomerPhoneRepository.GetMany (t => t.Id == customerPhoneObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CustomerPhoneRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

         public async Task<List<CustomerPhone>> DeleteByCustomer (long CustomerId) {
            var customerPhoneList = _unitOfWork.CustomerPhoneRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
            if(customerPhoneList != null && customerPhoneList.Count() > 0)
            {
                foreach (var existingItem in customerPhoneList) {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CustomerPhoneRepository.UpdateAsync (existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync ();
            }
            return customerPhoneList;
        }
    }

    public partial interface ICustomerPhoneService : IService<CustomerPhone> {
        Task<CustomerPhone> CheckInsertOrUpdate (CustomerPhoneDto model);
        List<CustomerPhone> GetAll ();
        List<CustomerPhone> GetByTenant (int tenantId);
        List<CustomerPhone> GetByCustomer (long customerId);
        CustomerPhone DeleteCustomerPhone (CustomerPhoneDto model);
        CustomerPhone GetById (long Id);
        List<CustomerPhone> GetByUser (int userId);
        Task<List<CustomerPhone>> DeleteByCustomer (long CustomerId);
    }
}