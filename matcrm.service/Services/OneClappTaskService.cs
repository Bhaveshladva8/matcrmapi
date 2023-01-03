using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappTaskService : Service<OneClappTask>, IOneClappTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappTask CheckInsertOrUpdate(OneClappTaskDto model)
        {
            var oneClappTaskObj = _mapper.Map<OneClappTask>(model);
            var existingItem = _unitOfWork.OneClappTaskRepository.GetMany(t => t.WeClappTimeRecordId == oneClappTaskObj.WeClappTimeRecordId && t.TenantId == oneClappTaskObj.TenantId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertTask(oneClappTaskObj);
            }
            else
            {
                oneClappTaskObj.CreatedBy = existingItem.CreatedBy;
                oneClappTaskObj.CreatedOn = existingItem.CreatedOn;
                oneClappTaskObj.Id = existingItem.Id;
                return UpdateTask(oneClappTaskObj, existingItem.Id);
            }
        }

        public OneClappTask InsertTask(OneClappTask oneClappTaskObj)
        {
            oneClappTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappTaskRepository.Add(oneClappTaskObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public OneClappTask UpdateTask(OneClappTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.OneClappTaskRepository.UpdateAsync(updatedItem, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public List<OneClappTask> GetAllActiveByTenant(long TenantId)
        {
            return _unitOfWork.OneClappTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappTask> GetAllTaskByTenantWithOutSection(long TenantId)
        {
            return _unitOfWork.OneClappTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsActive == true && t.IsDeleted == false && t.SectionId == null).Result.ToList();
        }

        public List<OneClappTask> GetAll()
        {
            return _unitOfWork.OneClappTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappTask> GetAllTaskBySectionId(long sectionId)
        {
            return _unitOfWork.OneClappTaskRepository.GetMany(t => t.SectionId == sectionId && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public OneClappTask GetTaskById(long TaskId)
        {
            return _unitOfWork.OneClappTaskRepository.GetMany(t => t.Id == TaskId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OneClappTask Delete(long TaskId)
        {
            var oneClappTaskObj = _unitOfWork.OneClappTaskRepository.GetMany(t => t.Id == TaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappTaskObj != null)
            {
                oneClappTaskObj.IsDeleted = true;
                oneClappTaskObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.OneClappTaskRepository.UpdateAsync(oneClappTaskObj, oneClappTaskObj.Id);
                _unitOfWork.CommitAsync();
            }
            return oneClappTaskObj;
        }
    }
    public partial interface IOneClappTaskService : IService<OneClappTask>
    {
        OneClappTask CheckInsertOrUpdate(OneClappTaskDto model);
        List<OneClappTask> GetAll();
        List<OneClappTask> GetAllTaskBySectionId(long sectionId);
        List<OneClappTask> GetAllTaskByTenantWithOutSection(long tenantId);
        List<OneClappTask> GetAllActiveByTenant(long TenantId);
        OneClappTask GetTaskById(long TaskId);
        OneClappTask Delete(long TaskId);
        OneClappTask UpdateTask(OneClappTask updatedItem, long existingId);
    }
}