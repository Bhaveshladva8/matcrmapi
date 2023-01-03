using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerActivityService : Service<CustomerActivity>, ICustomerActivityService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerActivityService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerActivity> CheckInsertOrUpdate (CustomerActivityDto model) {
            var customerActivityObj = _mapper.Map<CustomerActivity> (model);
            // var existingItem = _unitOfWork.CustomerActivityRepository.GetMany (t => t.CustomerId == obj.CustomerId && t.ScheduleStartDate == obj.ScheduleStartDate && t.StartTime == obj.StartTime &&
            //     t.EndTime == obj.EndTime && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.CustomerActivityRepository.GetMany (t => t.Id == customerActivityObj.Id &&
                t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomerActivity (customerActivityObj);
            } else {
                customerActivityObj.CreatedOn = existingItem.CreatedOn;
                customerActivityObj.CreatedBy = existingItem.CreatedBy;
                customerActivityObj.TenantId = existingItem.TenantId;
                customerActivityObj.CustomerId = existingItem.CustomerId;
                return await UpdateCustomerActivity (customerActivityObj, existingItem.Id);
            }
        }

        public async Task<CustomerActivity> UpdateCustomerActivity (CustomerActivity updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CustomerActivityRepository.UpdateAsync (updatedItem, existingId);
            await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<CustomerActivity> InsertCustomerActivity (CustomerActivity customerActivityObj) {
            customerActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerActivityRepository.AddAsync (customerActivityObj);
            await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerActivity> GetAll () {
            return _unitOfWork.CustomerActivityRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerActivity GetById (long Id) {
            return _unitOfWork.CustomerActivityRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerActivity> GetByTenant (int tenantId) {
            return _unitOfWork.CustomerActivityRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerActivity> GetByCustomer (long customerId) {
            return _unitOfWork.CustomerActivityRepository.GetMany (t => t.CustomerId == customerId && t.IsDeleted == false).Result.OrderByDescending (t => t.CreatedOn).ToList ();
        }

        public List<CustomerActivity> GetByUser (int userId) {
            return _unitOfWork.CustomerActivityRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerActivity DeleteCustomerActivity (CustomerActivityDto model) {
            var customerActivityObj = _mapper.Map<CustomerActivity> (model);
            var existingItem = _unitOfWork.CustomerActivityRepository.GetMany (t => t.Id == customerActivityObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CustomerActivityRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

          public async Task<List<CustomerActivity>> DeleteByCustomer (long CustomerId) {
            var customerActivities = _unitOfWork.CustomerActivityRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
            if(customerActivities != null && customerActivities.Count() > 0)
            {
                foreach (var existingItem in customerActivities) {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CustomerActivityRepository.UpdateAsync (existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync ();
            }
            return customerActivities;
        }
    }

    public partial interface ICustomerActivityService : IService<CustomerActivity> {
        Task<CustomerActivity> CheckInsertOrUpdate (CustomerActivityDto model);
        List<CustomerActivity> GetAll ();
        List<CustomerActivity> GetByTenant (int tenantId);
        List<CustomerActivity> GetByCustomer (long customerId);
        CustomerActivity DeleteCustomerActivity (CustomerActivityDto model);
        CustomerActivity GetById (long Id);
        List<CustomerActivity> GetByUser (int userId);
        Task<List<CustomerActivity>> DeleteByCustomer (long CustomerId);
    }
}