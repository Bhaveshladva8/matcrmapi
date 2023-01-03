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
    public partial class SubscriptionPlanService : Service<SubscriptionPlan>, ISubscriptionPlanService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionPlanService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionPlan> CheckInsertOrUpdate(SubscriptionPlanDto model)
        {
            var subscriptionPlanObj = _mapper.Map<SubscriptionPlan>(model);
            var existingItem = _unitOfWork.SubscriptionPlanRepository.GetMany(t => t.Name == subscriptionPlanObj.Name && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubscriptionPlan(subscriptionPlanObj);
            }
            else
            {
                subscriptionPlanObj.CreatedOn = existingItem.CreatedOn;
                subscriptionPlanObj.CreatedBy = existingItem.CreatedBy;
                subscriptionPlanObj.Id = existingItem.Id;
                return await UpdateSubscriptionPlan(subscriptionPlanObj, existingItem.Id);
            }
        }

        public async Task<SubscriptionPlan> UpdateSubscriptionPlan(SubscriptionPlan updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.SubscriptionPlanRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<SubscriptionPlan> InsertSubscriptionPlan(SubscriptionPlan subscriptionPlanObj)
        {
            subscriptionPlanObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SubscriptionPlanRepository.AddAsync(subscriptionPlanObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<SubscriptionPlan> GetAll()
        {
            return _unitOfWork.SubscriptionPlanRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public SubscriptionPlan GetById(long Id)
        {
            return _unitOfWork.SubscriptionPlanRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public SubscriptionPlan DeleteSubscriptionPlan(long Id)
        {
            var subscriptionPlanObj = _unitOfWork.SubscriptionPlanRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (subscriptionPlanObj != null)
            {
                subscriptionPlanObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.SubscriptionPlanRepository.UpdateAsync(subscriptionPlanObj, subscriptionPlanObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ISubscriptionPlanService : IService<SubscriptionPlan>
    {
        Task<SubscriptionPlan> CheckInsertOrUpdate(SubscriptionPlanDto model);
        List<SubscriptionPlan> GetAll();
        SubscriptionPlan DeleteSubscriptionPlan(long Id);
        SubscriptionPlan GetById(long Id);
    }
}