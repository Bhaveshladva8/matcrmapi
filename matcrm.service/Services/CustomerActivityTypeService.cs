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
    public partial class ActivityTypeService : Service<ActivityType>, IActivityTypeService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ActivityTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivityType> CheckInsertOrUpdate(ActivityTypeDto model)
        {
            var activityTypeObj = _mapper.Map<ActivityType>(model);
            var existingItem = _unitOfWork.ActivityTypeRepository.GetMany(t => t.Id == activityTypeObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertActivityType(activityTypeObj);
            }
            else
            {
                activityTypeObj.CreatedOn = existingItem.CreatedOn;
                activityTypeObj.CreatedBy = existingItem.CreatedBy;
                // obj.TenantId = existingItem.TenantId;
                return await UpdateActivityType(activityTypeObj, existingItem.Id);
            }
        }

        public async Task<ActivityType> UpdateActivityType(ActivityType updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.ActivityTypeRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<ActivityType> InsertActivityType(ActivityType activityTypeObj)
        {
            activityTypeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ActivityTypeRepository.AddAsync(activityTypeObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<ActivityType> GetAll()
        {
            return _unitOfWork.ActivityTypeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public ActivityType GetById(long Id)
        {
            return _unitOfWork.ActivityTypeRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        // public List<ActivityType> GetByTenant (int tenantId) {
        //     return _unitOfWork.ActivityTypeRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        // }

        public List<ActivityType> GetAllDefault()
        {
            return _unitOfWork.ActivityTypeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<ActivityType> GetByUser(int userId)
        {
            return _unitOfWork.ActivityTypeRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public ActivityType DeleteActivityType(ActivityTypeDto model)
        {
            var activityTypeObj = _mapper.Map<ActivityType>(model);
            var existingItem = _unitOfWork.ActivityTypeRepository.GetMany(t => t.Id == activityTypeObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.ActivityTypeRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IActivityTypeService : IService<ActivityType>
    {
       Task<ActivityType> CheckInsertOrUpdate(ActivityTypeDto model);
        List<ActivityType> GetAll();
        // List<ActivityType> GetByTenant (int tenantId);
        ActivityType DeleteActivityType(ActivityTypeDto model);
        ActivityType GetById(long Id);
        List<ActivityType> GetByUser(int userId);
        List<ActivityType> GetAllDefault();
    }
}