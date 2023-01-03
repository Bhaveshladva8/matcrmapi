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
    public partial class OrganizationActivityService : Service<OrganizationActivity>, IOrganizationActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationActivityService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationActivity> CheckInsertOrUpdate(OrganizationActivityDto model)
        {
            var organizationActivityObj = _mapper.Map<OrganizationActivity>(model);
            // var existingItem = _unitOfWork.OrganizationActivityRepository.GetMany (t => t.OrganizationId == obj.OrganizationId && t.ScheduleStartDate == obj.ScheduleStartDate && t.StartTime == obj.StartTime &&
            //     t.EndTime == obj.EndTime && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();

            var existingItem = _unitOfWork.OrganizationActivityRepository.GetMany(t => t.Id == organizationActivityObj.Id && t.IsDeleted == false).Result.FirstOrDefault();

            if (existingItem == null)
            {
                return await InsertOrganizationActivity(organizationActivityObj);
            }
            else
            {
                organizationActivityObj.CreatedOn = existingItem.CreatedOn;
                organizationActivityObj.CreatedBy = existingItem.CreatedBy;
                organizationActivityObj.TenantId = existingItem.TenantId;
                organizationActivityObj.OrganizationId = existingItem.OrganizationId;
                return await UpdateOrganizationActivity(organizationActivityObj, existingItem.Id);
            }
        }

        public async Task<OrganizationActivity> UpdateOrganizationActivity(OrganizationActivity updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OrganizationActivityRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OrganizationActivity> InsertOrganizationActivity(OrganizationActivity organizationActivityObj)
        {
            organizationActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationActivityRepository.AddAsync(organizationActivityObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OrganizationActivity> GetAll()
        {
            return _unitOfWork.OrganizationActivityRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OrganizationActivity GetById(long Id)
        {
            return _unitOfWork.OrganizationActivityRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<OrganizationActivity> GetByTenant(int tenantId)
        {
            return _unitOfWork.OrganizationActivityRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<OrganizationActivity> GetByOrganization(long organizationId)
        {
            return _unitOfWork.OrganizationActivityRepository.GetMany(t => t.OrganizationId == organizationId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedOn).ToList();
        }

        public List<OrganizationActivity> GetByUser(int userId)
        {
            return _unitOfWork.OrganizationActivityRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public async Task<OrganizationActivity> DeleteOrganizationActivity(OrganizationActivityDto model)
        {
            var organizationActivityObj = _mapper.Map<OrganizationActivity>(model);
            var existingItem = _unitOfWork.OrganizationActivityRepository.GetMany(t => t.Id == organizationActivityObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.OrganizationActivityRepository.UpdateAsync(existingItem, existingItem.Id);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<OrganizationActivity>> DeleteByOrganization(long OrganizationId)
        {
            var organizationActivities = _unitOfWork.OrganizationActivityRepository.GetMany(t => t.OrganizationId == OrganizationId && t.IsDeleted == false).Result.ToList();
            if (organizationActivities != null && organizationActivities.Count() > 0)
            {
                foreach (var existingItem in organizationActivities)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.OrganizationActivityRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return organizationActivities;
        }
    }

    public partial interface IOrganizationActivityService : IService<OrganizationActivity>
    {
        Task<OrganizationActivity> CheckInsertOrUpdate(OrganizationActivityDto model);
        List<OrganizationActivity> GetAll();
        List<OrganizationActivity> GetByTenant(int tenantId);
        List<OrganizationActivity> GetByOrganization(long organizationId);
        Task<OrganizationActivity> DeleteOrganizationActivity(OrganizationActivityDto model);
        OrganizationActivity GetById(long Id);
        List<OrganizationActivity> GetByUser(int userId);
        Task<List<OrganizationActivity>> DeleteByOrganization(long OrganizationId);
    }
}