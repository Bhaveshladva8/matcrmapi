using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaskActivityService : Service<TaskActivity>, ITaskActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TaskActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public TaskActivity CheckInsertOrUpdate(TaskActivity taskActivityObj)
        {
            // var existingItem = _unitOfWork.TaskActivityRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertTaskActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateTaskActivity (existingItem, existingItem.Id);
            // }
            return InsertTaskActivity(taskActivityObj);
        }

        public TaskActivity InsertTaskActivity(TaskActivity taskActivityObj)
        {
            taskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TaskActivityRepository.Add(taskActivityObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public TaskActivity UpdateTaskActivity(TaskActivity existingItem, int existingId)
        {
            _unitOfWork.TaskActivityRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<TaskActivity> GetAllByTaskId(long taskId)
        {
            return _unitOfWork.TaskActivityRepository.GetMany(t => t.TaskId == taskId).Result.ToList();
        }

        public TaskActivity GetTaskActivitytById(long Id)
        {
            return _unitOfWork.TaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public TaskActivity DeleteTaskActivityById(long Id)
        {
            var taskActivityObj = _unitOfWork.TaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (taskActivityObj != null)
            {
                _unitOfWork.TaskActivityRepository.DeleteAsync(taskActivityObj);
                _unitOfWork.CommitAsync();
            }
            return taskActivityObj;
        }

        public List<TaskActivity> DeleteTaskActivityByTaskId(long taskId)
        {
            var taskActivityList = _unitOfWork.TaskActivityRepository.GetMany(t => t.TaskId == taskId).Result.ToList();
            if (taskActivityList != null && taskActivityList.Count() > 0)
            {
                foreach (var item in taskActivityList)
                {
                    _unitOfWork.TaskActivityRepository.DeleteAsync(item);
                }
                _unitOfWork.CommitAsync();
            }
            return taskActivityList;
        }

    }

    public partial interface ITaskActivityService : IService<TaskActivity>
    {
        TaskActivity CheckInsertOrUpdate(TaskActivity model);
        List<TaskActivity> GetAllByTaskId(long taskId);
        TaskActivity GetTaskActivitytById(long Id);
        TaskActivity DeleteTaskActivityById(long Id);
        List<TaskActivity> DeleteTaskActivityByTaskId(long taskId);
    }
}