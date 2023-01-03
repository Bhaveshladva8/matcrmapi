using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerLabelService : Service<CustomerLabel>, ICustomerLabelService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerLabelService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerLabel> CheckInsertOrUpdate (CustomerLabelDto model) {
            var customerLabelObj = _mapper.Map<CustomerLabel> (model);
            // var existingItem = _unitOfWork.CustomerLabelRepository.GetMany (t => t.Id == obj.Id && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.CustomerLabelRepository.GetMany (t => t.CustomerId == customerLabelObj.CustomerId && t.TenantId == customerLabelObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertLabel (customerLabelObj);
            } else {
                customerLabelObj.CreatedOn = existingItem.CreatedOn;
                // obj.CreatedBy = existingItem.CreatedBy;
                // obj.TenantId = existingItem.TenantId;
                customerLabelObj.Id = existingItem.Id;
                return await UpdateLabel (customerLabelObj, existingItem.Id);
            }
        }

        public async Task<CustomerLabel> UpdateLabel (CustomerLabel updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CustomerLabelRepository.UpdateAsync (updatedItem, existingId);
            await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<CustomerLabel> InsertLabel (CustomerLabel customerLabelObj) {
            customerLabelObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerLabelRepository.AddAsync (customerLabelObj);
           await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerLabel> GetAll () {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerLabel GetById (long Id) {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerLabel> GetByTenant (int tenantId) {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerLabel> GetAllDefault () {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerLabel> GetByUser (int userId) {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerLabel> GetByUserAndDefault (int userId) {
            return _unitOfWork.CustomerLabelRepository.GetMany (t => (t.CreatedBy == userId || t.CreatedBy == null) && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerLabel GetByCustomer (long CustomerId){
            return _unitOfWork.CustomerLabelRepository.GetMany (t => t.IsDeleted == false && t.CustomerId == CustomerId).Result.FirstOrDefault (); 
         }

        public CustomerLabel DeleteLabel (CustomerLabelDto model) {
            var customerLabelObj = _mapper.Map<CustomerLabel> (model);
            var existingItem = _unitOfWork.CustomerLabelRepository.GetMany (t => t.Id == customerLabelObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CustomerLabelRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

         public async Task<List<CustomerLabel>> DeleteByLabel (long LabelId) {
            var customerLabelList = _unitOfWork.CustomerLabelRepository.GetMany (t => t.LabelId == LabelId && t.IsDeleted == false).Result.ToList ();
            if(customerLabelList != null && customerLabelList.Count() > 0)
            {
                foreach (var existingItem in customerLabelList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CustomerLabelRepository.UpdateAsync (existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync ();
            }
             return customerLabelList;
        }
    }

    public partial interface ICustomerLabelService : IService<CustomerLabel> {
        Task<CustomerLabel> CheckInsertOrUpdate (CustomerLabelDto model);
        List<CustomerLabel> GetAll ();
        List<CustomerLabel> GetByTenant (int tenantId);
        CustomerLabel DeleteLabel (CustomerLabelDto model);
        CustomerLabel GetById (long Id);
        CustomerLabel GetByCustomer (long CustomerId);
        List<CustomerLabel> GetByUser (int userId);
        List<CustomerLabel> GetAllDefault ();
        List<CustomerLabel> GetByUserAndDefault (int userId);
        Task<List<CustomerLabel>> DeleteByLabel (long LabelId);
    }
}