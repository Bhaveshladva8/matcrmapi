using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ClientPhoneService : Service<ClientPhone>, IClientPhoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientPhoneService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ClientPhone> CheckInsertOrUpdate(ClientPhone ClientPhoneObj)
        {            
            var existingItem = _unitOfWork.ClientPhoneRepository.GetMany(t => t.Id == ClientPhoneObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientPhone(ClientPhoneObj);
            }
            else
            {
                ClientPhoneObj.CreatedBy = existingItem.CreatedBy;
                ClientPhoneObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClientPhone(ClientPhoneObj, existingItem.Id);
            }
        }

        public async Task<ClientPhone> InsertClientPhone(ClientPhone ClientPhoneObj)
        {
            ClientPhoneObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientPhoneRepository.AddAsync(ClientPhoneObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientPhone> UpdateClientPhone(ClientPhone existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientPhoneRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public async Task<List<ClientPhone>> DeleteByClientId(long clientId)
        {
            var clientPhoneList = _unitOfWork.ClientPhoneRepository.GetMany(u => u.ClientId == clientId && u.DeletedOn == null).Result.ToList();
            if (clientPhoneList != null && clientPhoneList.Count() > 0)
            {
                foreach (var existingItem in clientPhoneList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientPhoneRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return clientPhoneList;   
        }
        public ClientPhone GetByClientIdWithPrimary(long clientId)
        {
            return _unitOfWork.ClientPhoneRepository.GetMany(t => t.ClientId == clientId && t.IsPrimary == true && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public async Task<ClientPhone> DeleteById(long Id)
        {
            var clientPhoneObj = _unitOfWork.ClientPhoneRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientPhoneObj != null)
            {
                clientPhoneObj.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.ClientPhoneRepository.UpdateAsync(clientPhoneObj, clientPhoneObj.Id);

                await _unitOfWork.CommitAsync();
            }
            return clientPhoneObj;
        }
        public List<ClientPhone> GetByClientId(long clientId)
        {
            return _unitOfWork.ClientPhoneRepository.GetMany(t => t.ClientId == clientId && t.DeletedOn == null).Result.ToList();
        }
        public bool IsExistOrNot(ClientPhone model)
        {
            var existingItem = _unitOfWork.ClientPhoneRepository.GetMany(t => t.ClientId == model.ClientId && t.PhoneNo == model.PhoneNo && t.PhoneNoTypeId == model.PhoneNoTypeId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public partial interface IClientPhoneService : IService<ClientPhone>
    {
        Task<ClientPhone> CheckInsertOrUpdate(ClientPhone model);
        Task<List<ClientPhone>> DeleteByClientId(long clientId);
        ClientPhone GetByClientIdWithPrimary(long clientId);
        List<ClientPhone> GetByClientId(long clientId);
        Task<ClientPhone> DeleteById(long Id);
        bool IsExistOrNot(ClientPhone model);
    }
}