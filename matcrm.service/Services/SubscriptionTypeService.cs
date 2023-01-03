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
    public partial class SubscriptionTypeService : Service<SubscriptionType>, ISubscriptionTypeService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubscriptionType> CheckInsertOrUpdate(SubscriptionTypeDto model)
        {
            var subscriptionTypeObj = _mapper.Map<SubscriptionType>(model);
            var existingItem = _unitOfWork.SubscriptionTypeRepository.GetMany(t => t.Name == subscriptionTypeObj.Name && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubscriptionType(subscriptionTypeObj);
            }
            else
            {
                subscriptionTypeObj.CreatedOn = existingItem.CreatedOn;
                subscriptionTypeObj.CreatedBy = existingItem.CreatedBy;
                subscriptionTypeObj.Id = existingItem.Id;
                return await UpdateSubscriptionType(subscriptionTypeObj, existingItem.Id);
            }
        }

        public async Task<SubscriptionType> UpdateSubscriptionType(SubscriptionType updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<SubscriptionType> InsertSubscriptionType(SubscriptionType subscriptionTypeObj)
        {
            subscriptionTypeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SubscriptionTypeRepository.AddAsync(subscriptionTypeObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<SubscriptionType> GetAll()
        {
            return _unitOfWork.SubscriptionTypeRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

       public SubscriptionType GetByName(string Name)
        {
            return _unitOfWork.SubscriptionTypeRepository.GetMany(t => t.DeletedOn == null && t.Name == Name).Result.FirstOrDefault();
        } 

        public SubscriptionType GetById(long Id)
        {
            return _unitOfWork.SubscriptionTypeRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public SubscriptionType DeleteSubscriptionType(long Id)
        {
            var subscriptionTypeObj = _unitOfWork.SubscriptionTypeRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (subscriptionTypeObj != null)
            {
                subscriptionTypeObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.SubscriptionTypeRepository.UpdateAsync(subscriptionTypeObj, subscriptionTypeObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ISubscriptionTypeService : IService<SubscriptionType>
    {
        Task<SubscriptionType> CheckInsertOrUpdate(SubscriptionTypeDto model);
        List<SubscriptionType> GetAll();
        SubscriptionType DeleteSubscriptionType(long Id);
        SubscriptionType GetById(long Id);
        SubscriptionType GetByName(string Name);
    }
}