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
    public partial class SubscriptionPlanDetailService : Service<SubscriptionPlanDetail>, ISubscriptionPlanDetailService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionPlanDetailService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionPlanDetail> CheckInsertOrUpdate(SubscriptionPlanDetailDto model)
        {
            var subscriptionPlanDetailObj = _mapper.Map<SubscriptionPlanDetail>(model);
            var existingItem = _unitOfWork.SubscriptionPlanDetailRepository.GetMany(t => t.SubScriptionPlanId == subscriptionPlanDetailObj.SubScriptionPlanId && t.FeatureName == subscriptionPlanDetailObj.FeatureName && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubscriptionPlanDetail(subscriptionPlanDetailObj);
            }
            else
            {
                subscriptionPlanDetailObj.CreatedOn = existingItem.CreatedOn;
                subscriptionPlanDetailObj.CreatedBy = existingItem.CreatedBy;
                subscriptionPlanDetailObj.Id = existingItem.Id;
                return await UpdateSubscriptionPlanDetail(subscriptionPlanDetailObj, existingItem.Id);
            }
        }

        public async Task<SubscriptionPlanDetail> UpdateSubscriptionPlanDetail(SubscriptionPlanDetail updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.SubscriptionPlanDetailRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<SubscriptionPlanDetail> InsertSubscriptionPlanDetail(SubscriptionPlanDetail subscriptionPlanDetailObj)
        {
            subscriptionPlanDetailObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SubscriptionPlanDetailRepository.AddAsync(subscriptionPlanDetailObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<SubscriptionPlanDetail> GetAll()
        {
            return _unitOfWork.SubscriptionPlanDetailRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public List<SubscriptionPlanDetail> GetAllByPlan(long SubscriptionPlanId)
        {
            return _unitOfWork.SubscriptionPlanDetailRepository.GetMany(t => t.SubScriptionPlanId == SubscriptionPlanId && t.DeletedOn == null).Result.ToList();
        }

        public SubscriptionPlanDetail GetById(long Id)
        {
            return _unitOfWork.SubscriptionPlanDetailRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public SubscriptionPlanDetail DeleteSubscriptionPlanDetail(long Id)
        {
            var subscriptionPlanDetailObj = _unitOfWork.SubscriptionPlanDetailRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (subscriptionPlanDetailObj != null)
            {
                subscriptionPlanDetailObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.SubscriptionPlanDetailRepository.UpdateAsync(subscriptionPlanDetailObj, subscriptionPlanDetailObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ISubscriptionPlanDetailService : IService<SubscriptionPlanDetail>
    {
        Task<SubscriptionPlanDetail> CheckInsertOrUpdate(SubscriptionPlanDetailDto model);
        List<SubscriptionPlanDetail> GetAll();
        SubscriptionPlanDetail DeleteSubscriptionPlanDetail(long Id);
        SubscriptionPlanDetail GetById(long Id);
        List<SubscriptionPlanDetail> GetAllByPlan(long SubscriptionPlanId);
    }
}