using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaskAttachmentService : Service<TaskAttachment>, ITaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public TaskAttachment CheckInsertOrUpdate(TaskAttachmentDto model)
        {
            var taskAttachmentObj = _mapper.Map<TaskAttachment>(model);
            var existingItem = _unitOfWork.TaskAttachmentRepository.GetMany(t => t.Name == taskAttachmentObj.Name && t.TaskId == taskAttachmentObj.TaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertTaskAttachment(taskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public TaskAttachment InsertTaskAttachment(TaskAttachment taskAttachmentObj)
        {
            taskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TaskAttachmentRepository.Add(taskAttachmentObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public TaskAttachment UpdateTaskAttachment(TaskAttachment existingItem, int existingId)
        {
            _unitOfWork.TaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<TaskAttachment> GetAllByTaskId(long taskId)
        {
            return _unitOfWork.TaskAttachmentRepository.GetMany(t => t.TaskId == taskId && t.IsDeleted == false).Result.ToList();
        }

        public TaskAttachment GetTaskAttachmentById(long Id)
        {
            return _unitOfWork.TaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public TaskAttachment DeleteTaskAttachmentById(long Id)
        {
            var taskAttachmentObj = _unitOfWork.TaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (taskAttachmentObj != null)
            {
                taskAttachmentObj.IsDeleted = true;
                taskAttachmentObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.TaskAttachmentRepository.UpdateAsync(taskAttachmentObj, taskAttachmentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return taskAttachmentObj;
        }


        public List<TaskAttachment> DeleteAttachmentByTaskId(long TaskId)
        {
            var taskAttachmentList = _unitOfWork.TaskAttachmentRepository.GetMany(t => t.TaskId == TaskId && t.IsDeleted == false).Result.ToList();
            if (taskAttachmentList != null && taskAttachmentList.Count() > 0)
            {
                foreach (var item in taskAttachmentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.TaskAttachmentRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return taskAttachmentList;
        }
    }
    public partial interface ITaskAttachmentService : IService<TaskAttachment>
    {
        TaskAttachment CheckInsertOrUpdate(TaskAttachmentDto model);
        List<TaskAttachment> GetAllByTaskId(long taskId);
        TaskAttachment GetTaskAttachmentById(long Id);
        TaskAttachment DeleteTaskAttachmentById(long Id);



        List<TaskAttachment> DeleteAttachmentByTaskId(long TaskId);
    }
}