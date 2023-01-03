using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaskCommentService : Service<TaskComment>, ITaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public TaskComment CheckInsertOrUpdate(TaskCommentDto model)
        {
            var taskCommentObj = _mapper.Map<TaskComment>(model);
            // var existingItem = _unitOfWork.TaskCommentRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.TaskCommentRepository.GetMany(t => t.Id == taskCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertTaskComment(taskCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return UpdateTaskComment(existingItem, existingItem.Id);
            }
        }

        public TaskComment InsertTaskComment(TaskComment taskCommentObj)
        {
            taskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TaskCommentRepository.Add(taskCommentObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public TaskComment UpdateTaskComment(TaskComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.TaskCommentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<TaskComment> GetAllByTaskId(long taskId)
        {
            return _unitOfWork.TaskCommentRepository.GetMany(t => t.TaskId == taskId && t.IsDeleted == false).Result.ToList();
        }

        public TaskComment GetTaskCommenttById(long Id)
        {
            return _unitOfWork.TaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<TaskComment> DeleteTaskComment(long Id)
        {
            var taskCommentObj = _unitOfWork.TaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (taskCommentObj != null)
            {
                taskCommentObj.IsDeleted = true;
                taskCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.TaskCommentRepository.UpdateAsync(taskCommentObj, taskCommentObj.Id);
                await _unitOfWork.CommitAsync();

                return taskCommentObj;
            }
            else
            {
                return null;
            }
        }

        public List<TaskComment> DeleteCommentByTaskId(long TaskId)
        {
            var taskCommentList = _unitOfWork.TaskCommentRepository.GetMany(t => t.TaskId == TaskId && t.IsDeleted == false).Result.ToList();
            if (taskCommentList != null)
            {
                foreach (var item in taskCommentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.TaskCommentRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return taskCommentList;
        }
    }

    public partial interface ITaskCommentService : IService<TaskComment>
    {
        TaskComment CheckInsertOrUpdate(TaskCommentDto model);
        List<TaskComment> GetAllByTaskId(long taskId);
        TaskComment GetTaskCommenttById(long Id);
        Task<TaskComment> DeleteTaskComment(long Id);
        List<TaskComment> DeleteCommentByTaskId(long TaskId);
    }
}