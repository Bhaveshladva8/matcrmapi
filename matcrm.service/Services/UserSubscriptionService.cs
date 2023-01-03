using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class UserSubscriptionService : Service<UserSubscription>, IUserSubscriptionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public UserSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserSubscription> CheckInsertOrUpdate(UserSubscriptionDto model)
        {
            var userSubscriptionObj = _mapper.Map<UserSubscription>(model);
            var existingItem = _unitOfWork.UserSubscriptionRepository.GetMany(t => t.UserId == userSubscriptionObj.UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertUserSubscription(userSubscriptionObj);
            }
            else
            {
                userSubscriptionObj.CreatedOn = existingItem.CreatedOn;
                userSubscriptionObj.UserId = existingItem.UserId;
                userSubscriptionObj.Id = existingItem.Id;
                return await UpdateUserSubscription(userSubscriptionObj, existingItem.Id);
            }
        }

        public async Task<UserSubscription> UpdateUserSubscription(UserSubscription updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.UserSubscriptionRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<UserSubscription> UpdateIsSubscribed(UserSubscription updatedItem, long existingId)
        {
            var update = await _unitOfWork.UserSubscriptionRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<UserSubscription> InsertUserSubscription(UserSubscription userSubscriptionObj)
        {
            userSubscriptionObj.CreatedOn = DateTime.UtcNow;
            userSubscriptionObj.SubscribedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.UserSubscriptionRepository.AddAsync(userSubscriptionObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<UserSubscription> GetAll()
        {
            return _unitOfWork.UserSubscriptionRepository.GetMany(t => t.DeletedOn == null).Result.Include(t => t.User).ToList();
        }

        public UserSubscription GetById(long Id)
        {
            return _unitOfWork.UserSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public UserSubscription GetByUser(long UserId)
        {
            return _unitOfWork.UserSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.UserId == UserId).Result.Include(t => t.SubscriptionPlan).Include(t => t.SubscriptionType).FirstOrDefault();
        }

        public UserSubscription DeleteUserSubscription(long Id)
        {
            var userSubscriptionObj = _unitOfWork.UserSubscriptionRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (userSubscriptionObj != null)
            {
                userSubscriptionObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.UserSubscriptionRepository.UpdateAsync(userSubscriptionObj, userSubscriptionObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IUserSubscriptionService : IService<UserSubscription>
    {
        Task<UserSubscription> CheckInsertOrUpdate(UserSubscriptionDto model);
        List<UserSubscription> GetAll();
        UserSubscription DeleteUserSubscription(long Id);
        UserSubscription GetById(long Id);
        UserSubscription GetByUser(long UserId);
        Task<UserSubscription> UpdateIsSubscribed(UserSubscription updatedItem, long existingId);
    }
}