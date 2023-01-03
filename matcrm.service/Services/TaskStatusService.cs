using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class TaskStatusService : Service<TaskStatus>, ITaskStatusService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TaskStatusService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public TaskStatus CheckInsertOrUpdate (TaskStatusDto model) {
            var taskStatusObj = _mapper.Map<TaskStatus> (model);
            var existingItem = _unitOfWork.TaskStatusRepository.GetMany (t => t.Id == taskStatusObj.Id && t.TenantId == taskStatusObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertTaskStatus (taskStatusObj);
            } else {
                return UpdateTaskStatus (taskStatusObj, existingItem.Id);
            }
        }

        public TaskStatus UpdateTaskStatus (TaskStatus updatedItem, int existingId) {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.TaskStatusRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public TaskStatus InsertTaskStatus (TaskStatus taskStatusObj) {
            taskStatusObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TaskStatusRepository.Add (taskStatusObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<TaskStatus> GetAll () {
            return _unitOfWork.TaskStatusRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public TaskStatus GetTaskStatusById (int statusId) {
            return _unitOfWork.TaskStatusRepository.GetMany (t => t.IsDeleted == false && t.Id == statusId).Result.FirstOrDefault ();
        }

        public List<TaskStatus> GetStatusByTenant (int tenantId) {
            return _unitOfWork.TaskStatusRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<TaskStatus> GetStatusByUser (int userId) {
            return _unitOfWork.TaskStatusRepository.GetMany (t => t.UserId == userId && t.IsDeleted == false).Result.ToList ();
        }

        public TaskStatus DeleteTaskStatus (int Id) {
            var existingItem = _unitOfWork.TaskStatusRepository.GetMany (t => t.Id == Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.TaskStatusRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
               return null;
            }
        }
    }

    public partial interface ITaskStatusService : IService<TaskStatus> {
        TaskStatus CheckInsertOrUpdate (TaskStatusDto model);
        List<TaskStatus> GetAll ();
        List<TaskStatus> GetStatusByTenant (int tenantId);
        TaskStatus DeleteTaskStatus (int Id);
        TaskStatus GetTaskStatusById (int statusId);
        List<TaskStatus> GetStatusByUser (int userId);
    }
}