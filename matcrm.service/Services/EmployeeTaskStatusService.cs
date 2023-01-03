using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class EmployeeTaskStatusService : Service<EmployeeTaskStatus>, IEmployeeTaskStatusService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTaskStatusService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public EmployeeTaskStatus CheckInsertOrUpdate (EmployeeTaskStatusDto model) {
            var employeeTaskStatusobj = _mapper.Map<EmployeeTaskStatus> (model);
            var existingItem = _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.Id == employeeTaskStatusobj.Id && t.TenantId == employeeTaskStatusobj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertEmployeeTaskStatus (employeeTaskStatusobj);
            } else {
                return UpdateEmployeeTaskStatus (employeeTaskStatusobj, existingItem.Id);
            }
        }

        public EmployeeTaskStatus UpdateEmployeeTaskStatus (EmployeeTaskStatus updatedItem, long existingId) {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.EmployeeTaskStatusRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public EmployeeTaskStatus InsertEmployeeTaskStatus (EmployeeTaskStatus employeeTaskStatusObj) {
            employeeTaskStatusObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.EmployeeTaskStatusRepository.Add (employeeTaskStatusObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<EmployeeTaskStatus> GetAll () {
            return _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public EmployeeTaskStatus GetEmployeeTaskStatusById (long statusId) {
            return _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.IsDeleted == false && t.Id == statusId).Result.FirstOrDefault ();
        }

        public List<EmployeeTaskStatus> GetStatusByTenant (int tenantId) {
            return _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<EmployeeTaskStatus> GetStatusByUser (int userId) {
            return _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.UserId == userId && t.IsDeleted == false).Result.ToList ();
        }

        public EmployeeTaskStatus DeleteEmployeeTaskStatus (EmployeeTaskStatusDto model) {
            var employeeTaskStatusObj = _mapper.Map<EmployeeTaskStatus> (model);
            var existingItem = _unitOfWork.EmployeeTaskStatusRepository.GetMany (t => t.Id == employeeTaskStatusObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.EmployeeTaskStatusRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
               return null;
            }
        }
    }

    public partial interface IEmployeeTaskStatusService : IService<EmployeeTaskStatus> {
        EmployeeTaskStatus CheckInsertOrUpdate (EmployeeTaskStatusDto model);
        List<EmployeeTaskStatus> GetAll ();
        List<EmployeeTaskStatus> GetStatusByTenant (int tenantId);
        EmployeeTaskStatus DeleteEmployeeTaskStatus (EmployeeTaskStatusDto model);
        EmployeeTaskStatus GetEmployeeTaskStatusById (long statusId);
        List<EmployeeTaskStatus> GetStatusByUser (int userId);
    }
}