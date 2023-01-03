using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappSubTaskService : Service<OneClappSubTask>, IOneClappSubTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappSubTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappSubTask CheckInsertOrUpdate(OneClappSubTaskDto model)
        {
            var oneClappSubTaskObj = _mapper.Map<OneClappSubTask>(model);
            var existingItem = _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.WeClappTimeRecordId == oneClappSubTaskObj.WeClappTimeRecordId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertSubTask(oneClappSubTaskObj);
            }
            else
            {
                oneClappSubTaskObj.CreatedBy = existingItem.CreatedBy;
                oneClappSubTaskObj.CreatedOn = existingItem.CreatedOn;
                oneClappSubTaskObj.Id = existingItem.Id;
                return UpdateSubTask(oneClappSubTaskObj, existingItem.Id);
            }
        }

        public OneClappSubTask InsertSubTask(OneClappSubTask oneClappSubTaskObj)
        {
            oneClappSubTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappSubTaskRepository.Add(oneClappSubTaskObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public OneClappSubTask UpdateSubTask(OneClappSubTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.OneClappSubTaskRepository.UpdateAsync(updatedItem, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public List<OneClappSubTask> GetAllActive()
        {
            return _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappSubTask> GetAllActiveByTaskIds(List<long> TaskIds)
        {
            return _unitOfWork.OneClappSubTaskRepository.GetMany(t => TaskIds.Contains(t.OneClappTaskId.Value) && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappSubTask> GetAll()
        {
            return _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappSubTask> GetAllSubTaskByTask(long TaskId)
        {
            return _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.OneClappTaskId == TaskId && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public OneClappSubTask GetSubTaskById(long SubTaskId)
        {
            return _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.Id == SubTaskId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OneClappSubTask Delete(long SubTaskId)
        {
            var oneClappSubTaskObj = _unitOfWork.OneClappSubTaskRepository.GetMany(t => t.Id == SubTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappSubTaskObj != null)
            {
                oneClappSubTaskObj.IsDeleted = true;
                oneClappSubTaskObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.OneClappSubTaskRepository.UpdateAsync(oneClappSubTaskObj, oneClappSubTaskObj.Id);
                _unitOfWork.CommitAsync();
            }
            return oneClappSubTaskObj;
        }
    }
    public partial interface IOneClappSubTaskService : IService<OneClappSubTask>
    {
        OneClappSubTask CheckInsertOrUpdate(OneClappSubTaskDto model);
        List<OneClappSubTask> GetAll();
        List<OneClappSubTask> GetAllActive();
        List<OneClappSubTask> GetAllActiveByTaskIds(List<long> TaskIds);
        List<OneClappSubTask> GetAllSubTaskByTask(long TaskId);
        OneClappSubTask Delete(long SubTaskId);
        OneClappSubTask GetSubTaskById(long SubTaskId);
    }
}