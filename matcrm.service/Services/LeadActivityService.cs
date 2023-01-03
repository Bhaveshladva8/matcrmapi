using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class LeadActivityService : Service<LeadActivity>, ILeadActivityService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LeadActivityService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public LeadActivity CheckInsertOrUpdate (LeadActivityDto model) {
            var leadActivityObj = _mapper.Map<LeadActivity> (model);
            // var existingItem = _unitOfWork.LeadActivityRepository.GetMany (t => t.LeadId == obj.LeadId && t.ScheduleStartDate == obj.ScheduleStartDate && t.StartTime == obj.StartTime &&
            //     t.EndTime == obj.EndTime && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();

            var existingItem = _unitOfWork.LeadActivityRepository.GetMany (t => t.Id == leadActivityObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();

            if (existingItem == null) {
                return InsertLeadActivity (leadActivityObj);
            } else {
                leadActivityObj.CreatedOn = existingItem.CreatedOn;
                leadActivityObj.CreatedBy = existingItem.CreatedBy;
                leadActivityObj.TenantId = existingItem.TenantId;
                leadActivityObj.LeadId = existingItem.LeadId;
                return UpdateLeadActivity (leadActivityObj, existingItem.Id);
            }
        }

        public LeadActivity UpdateLeadActivity (LeadActivity leadActivityobj, long existingId) {
            leadActivityobj.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.LeadActivityRepository.UpdateAsync (leadActivityobj, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public LeadActivity InsertLeadActivity (LeadActivity leadActivityObj) {
            leadActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.LeadActivityRepository.Add (leadActivityObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<LeadActivity> GetAll () {
            return _unitOfWork.LeadActivityRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public LeadActivity GetById (long Id) {
            return _unitOfWork.LeadActivityRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<LeadActivity> GetByTenant (int tenantId) {
            return _unitOfWork.LeadActivityRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<LeadActivity> GetByLead (long LeadId) {
            return _unitOfWork.LeadActivityRepository.GetMany (t => t.LeadId == LeadId && t.IsDeleted == false).Result.OrderByDescending (t => t.CreatedOn).ToList ();
        }

        public List<LeadActivity> GetByUser (int userId) {
            return _unitOfWork.LeadActivityRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public async Task<LeadActivity> DeleteLeadActivity (LeadActivityDto model) {
            var leadActivityObj = _mapper.Map<LeadActivity> (model);
            var leadActivity = _unitOfWork.LeadActivityRepository.GetMany (t => t.Id == leadActivityObj.Id).Result.FirstOrDefault ();
            if (leadActivity != null) {
                leadActivity.IsDeleted = true;
                leadActivity.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.LeadActivityRepository.UpdateAsync (leadActivity, leadActivity.Id);
                await _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }

        public async Task<List<LeadActivity>> DeleteByLead (long LeadId) {
            var leadActivities = _unitOfWork.LeadActivityRepository.GetMany (t => t.LeadId == LeadId && t.IsDeleted == false).Result.ToList ();
            if(leadActivities != null && leadActivities.Count() > 0)
            {
                foreach (var existingItem in leadActivities) {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.LeadActivityRepository.UpdateAsync (existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync ();
            }
            return leadActivities;
        }
    }

    public partial interface ILeadActivityService : IService<LeadActivity> {
        LeadActivity CheckInsertOrUpdate (LeadActivityDto model);
        List<LeadActivity> GetAll ();
        List<LeadActivity> GetByTenant (int tenantId);
        List<LeadActivity> GetByLead (long LeadId);
        Task<LeadActivity> DeleteLeadActivity (LeadActivityDto model);
        LeadActivity GetById (long Id);
        List<LeadActivity> GetByUser (int userId);
        Task<List<LeadActivity>> DeleteByLead (long LeadId);
    }
}