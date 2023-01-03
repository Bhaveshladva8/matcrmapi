using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappChildTaskService : Service<OneClappChildTask>, IOneClappChildTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappChildTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappChildTask CheckInsertOrUpdate(OneClappChildTaskDto model)
        {
            var oneClappChildTaskObj = _mapper.Map<OneClappChildTask>(model);
            var existingItem = _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.WeClappTimeRecordId == oneClappChildTaskObj.WeClappTimeRecordId && t.OneClappSubTaskId == oneClappChildTaskObj.OneClappSubTaskId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertChildTask(oneClappChildTaskObj);
            }
            else
            {
                oneClappChildTaskObj.CreatedBy = existingItem.CreatedBy;
                oneClappChildTaskObj.CreatedOn = existingItem.CreatedOn;
                oneClappChildTaskObj.Id = existingItem.Id;
                return UpdateChildTask(oneClappChildTaskObj, existingItem.Id);
            }
        }

        public OneClappChildTask InsertChildTask(OneClappChildTask oneClappChildTaskObj)
        {
            oneClappChildTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappChildTaskRepository.Add(oneClappChildTaskObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public OneClappChildTask UpdateChildTask(OneClappChildTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.OneClappChildTaskRepository.UpdateAsync(updatedItem, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public List<OneClappChildTask> GetAllActive()
        {
            return _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappChildTask> GetAllActiveBySubTaskIds(List<long> SubTaskIds)
        {
            return _unitOfWork.OneClappChildTaskRepository.GetMany(t => SubTaskIds.Contains(t.OneClappSubTaskId.Value) && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappChildTask> GetAll()
        {
            return _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappChildTask> GetAllChildTaskBySubTask(long SubTaskId)
        {
            return _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.OneClappSubTaskId == SubTaskId && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public OneClappChildTask GetChildTaskById(long ChildTaskId)
        {
            return _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.Id == ChildTaskId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OneClappChildTask Delete(long ChildTaskId)
        {
            var oneClappChildTaskObj = _unitOfWork.OneClappChildTaskRepository.GetMany(t => t.Id == ChildTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappChildTaskObj != null)
            {
                oneClappChildTaskObj.IsDeleted = true;
                oneClappChildTaskObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.OneClappChildTaskRepository.UpdateAsync(oneClappChildTaskObj, oneClappChildTaskObj.Id);
                _unitOfWork.CommitAsync();
            }
            return oneClappChildTaskObj;
        }
    }

    public partial interface IOneClappChildTaskService : IService<OneClappChildTask>
    {
        OneClappChildTask CheckInsertOrUpdate(OneClappChildTaskDto model);
        List<OneClappChildTask> GetAll();
        List<OneClappChildTask> GetAllActive();
        List<OneClappChildTask> GetAllActiveBySubTaskIds(List<long> SubTaskIds);
        List<OneClappChildTask> GetAllChildTaskBySubTask(long SubTaskId);
        OneClappChildTask Delete(long ChildTaskId);
        OneClappChildTask GetChildTaskById(long ChildTaskId);
    }
}