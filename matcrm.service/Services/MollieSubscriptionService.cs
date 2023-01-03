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
    public partial class MollieSubscriptionService : Service<MollieSubscription>, IMollieSubscriptionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MollieSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MollieSubscription> CheckInsertOrUpdate(MollieSubscriptionDto model)
        {
            var mollieSubscriptionObj = _mapper.Map<MollieSubscription>(model);
            // var existingItem = _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.UserId == obj.UserId && t.SubscriptionId == obj.SubscriptionId && t.DeletedOn == null).Result.FirstOrDefault();
            var existingItem = _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.UserId == mollieSubscriptionObj.UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMollieSubscription(mollieSubscriptionObj);
            }
            else
            {
                mollieSubscriptionObj.CreatedOn = existingItem.CreatedOn;
                mollieSubscriptionObj.UserId = existingItem.UserId;
                mollieSubscriptionObj.Id = existingItem.Id;
                return await UpdateMollieSubscription(mollieSubscriptionObj, existingItem.Id);
            }
        }

        public async Task<MollieSubscription> UpdateMollieSubscription(MollieSubscription updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.MollieSubscriptionRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<MollieSubscription> InsertMollieSubscription(MollieSubscription mollieSubscriptionObj)
        {
            mollieSubscriptionObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MollieSubscriptionRepository.AddAsync(mollieSubscriptionObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<MollieSubscription> GetAll()
        {
            return _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public MollieSubscription GetById(long Id)
        {
            return _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public MollieSubscription GetByUser(long UserId)
        {
            return _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.DeletedOn == null && t.UserId == UserId).Result.FirstOrDefault();
        }

        public MollieSubscription DeleteMollieSubscription(long Id)
        {
            var mollieSubscriptionObj = _unitOfWork.MollieSubscriptionRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mollieSubscriptionObj != null)
            {
                mollieSubscriptionObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MollieSubscriptionRepository.UpdateAsync(mollieSubscriptionObj, mollieSubscriptionObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IMollieSubscriptionService : IService<MollieSubscription>
    {
        Task<MollieSubscription> CheckInsertOrUpdate(MollieSubscriptionDto model);
        List<MollieSubscription> GetAll();
        MollieSubscription DeleteMollieSubscription(long Id);
        MollieSubscription GetById(long Id);
        MollieSubscription GetByUser(long UserId);
    }
}