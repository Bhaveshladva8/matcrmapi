using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class EmployeeProjectStatusService : Service<EmployeeProjectStatus>, IEmployeeProjectStatusService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeProjectStatusService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public EmployeeProjectStatus CheckInsertOrUpdate (EmployeeProjectStatusDto model) {
            var employeeProjectStatusObj = _mapper.Map<EmployeeProjectStatus> (model);
            var existingItem = _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.Id == employeeProjectStatusObj.Id && t.TenantId == employeeProjectStatusObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertEmployeeProjectStatus (employeeProjectStatusObj);
            } else {
                employeeProjectStatusObj.CreatedOn = existingItem.CreatedOn;
                employeeProjectStatusObj.TenantId = existingItem.TenantId;
                employeeProjectStatusObj.UserId = existingItem.UserId;
                return UpdateEmployeeProjectStatus (employeeProjectStatusObj, existingItem.Id);
            }
        }

        public EmployeeProjectStatus UpdateEmployeeProjectStatus (EmployeeProjectStatus updatedItem, long existingId) {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var employeeProjectStatus = _unitOfWork.EmployeeProjectStatusRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return employeeProjectStatus;
        }

        public EmployeeProjectStatus InsertEmployeeProjectStatus (EmployeeProjectStatus employeeProjectStatusObj) {
            employeeProjectStatusObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.EmployeeProjectStatusRepository.Add (employeeProjectStatusObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<EmployeeProjectStatus> GetAll () {
            return _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public EmployeeProjectStatus GetEmployeeProjectStatusById (long statusId) {
            return _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.IsDeleted == false && t.Id == statusId).Result.FirstOrDefault ();
        }

        public List<EmployeeProjectStatus> GetStatusByTenant (int tenantId) {
            return _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<EmployeeProjectStatus> GetStatusByUser (int userId) {
            return _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.UserId == userId && t.IsDeleted == false).Result.ToList ();
        }

        public EmployeeProjectStatus DeleteEmployeeProjectStatus (EmployeeProjectStatusDto model) {
            var employeeProjectStatusObj = _mapper.Map<EmployeeProjectStatus> (model);
            var existingItem = _unitOfWork.EmployeeProjectStatusRepository.GetMany (t => t.Id == employeeProjectStatusObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.EmployeeProjectStatusRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
               return null;
            }
        }
    }

    public partial interface IEmployeeProjectStatusService : IService<EmployeeProjectStatus> {
        EmployeeProjectStatus CheckInsertOrUpdate (EmployeeProjectStatusDto model);
        List<EmployeeProjectStatus> GetAll ();
        List<EmployeeProjectStatus> GetStatusByTenant (int tenantId);
        EmployeeProjectStatus DeleteEmployeeProjectStatus (EmployeeProjectStatusDto model);
        EmployeeProjectStatus GetEmployeeProjectStatusById (long statusId);
        List<EmployeeProjectStatus> GetStatusByUser (int userId);
    }
}