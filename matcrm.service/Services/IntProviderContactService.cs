using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class IntProviderContactService : Service<IntProviderContact>, IIntProviderContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public IntProviderContactService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntProviderContact> CheckInsertOrUpdate(IntProviderContact intProviderContactObj)
        {
            var existingItem = _unitOfWork.IntProviderContactRepository.GetMany(t => t.Id == intProviderContactObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertIntProviderContact(intProviderContactObj);
            }
            else
            {
                intProviderContactObj.LoggedInUserId = existingItem.LoggedInUserId;
                intProviderContactObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateIntProviderContact(intProviderContactObj, existingItem.Id);
            }
        }
        public async Task<IntProviderContact> InsertIntProviderContact(IntProviderContact intProviderContactObj)
        {
            intProviderContactObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.IntProviderContactRepository.AddAsync(intProviderContactObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<IntProviderContact> UpdateIntProviderContact(IntProviderContact existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.IntProviderContactRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<IntProviderContact> DeleteByClientUserId(long ClientUserId)
        {
            var intProviderContactObj = _unitOfWork.IntProviderContactRepository.GetMany(u => u.ClientUserId == ClientUserId && u.DeletedOn == null).Result.FirstOrDefault();
            if (intProviderContactObj != null)
            {
                intProviderContactObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.IntProviderContactRepository.UpdateAsync(intProviderContactObj, intProviderContactObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return intProviderContactObj;
        }
        public async Task<List<IntProviderContact>> DeleteByClientId(long ClientId)
        {
            var intProviderContactList = _unitOfWork.IntProviderContactRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (intProviderContactList != null && intProviderContactList.Count() > 0)
            {
                foreach (var existingItem in intProviderContactList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.IntProviderContactRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return intProviderContactList;
        }
    }
    public partial interface IIntProviderContactService : IService<IntProviderContact>
    {
        Task<IntProviderContact> CheckInsertOrUpdate(IntProviderContact model);
        Task<IntProviderContact> DeleteByClientUserId(long ClientUserId);
        Task<List<IntProviderContact>> DeleteByClientId(long ClientId);
    }
}