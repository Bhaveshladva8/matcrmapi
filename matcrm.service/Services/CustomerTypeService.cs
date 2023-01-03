using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerTypeService : Service<CustomerType>, ICustomerTypeService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerTypeService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public CustomerType CheckInsertOrUpdate (CustomerTypeDto model) {
            var customerTypeObj = _mapper.Map<CustomerType> (model);
            var existingItem = _unitOfWork.CustomerTypeRepository.GetMany (t => t.Id == customerTypeObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertCustomerType (customerTypeObj);
            } else {
                return UpdateCustomerType (customerTypeObj, existingItem.Id);
            }
        }

        public CustomerType UpdateCustomerType (CustomerType updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.CustomerTypeRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public CustomerType InsertCustomerType (CustomerType customerTypeObj) {
            customerTypeObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.CustomerTypeRepository.Add (customerTypeObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<CustomerType> GetAll () {
            return _unitOfWork.CustomerTypeRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomerType GetCustomerTypeById (long Id) {
            return _unitOfWork.CustomerTypeRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<CustomerType> GetCustomerTypeByName (string Name) {
            return _unitOfWork.CustomerTypeRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.ToList ();
        }

        public List<CustomerType> GetCustomerTypesByTenant (int TenantId) {
            return _unitOfWork.CustomerTypeRepository.GetMany (t => t.TenantId == TenantId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerType DeleteCustomerType (long Id, int UserId) {
            var existingItem = _unitOfWork.CustomerTypeRepository.GetMany (t => t.Id == Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                existingItem.DeletedBy = UserId;
                var newItem = _unitOfWork.CustomerTypeRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }
    }

    public partial interface ICustomerTypeService : IService<CustomerType> {
        CustomerType CheckInsertOrUpdate (CustomerTypeDto model);
        List<CustomerType> GetAll ();
        CustomerType DeleteCustomerType (long Id, int UserId);
        CustomerType GetCustomerTypeById (long Id);
        List<CustomerType> GetCustomerTypeByName (string Name);
        List<CustomerType> GetCustomerTypesByTenant (int TenantId);
    }
}