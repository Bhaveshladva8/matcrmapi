using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class ClientSocialMediaService : Service<ClientSocialMedia>, IClientSocialMediaService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ClientSocialMediaService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ClientSocialMedia> CheckInsertOrUpdate(ClientSocialMedia clientSocialMediaObj)
        {
            var existingItem = _unitOfWork.ClientSocialMediaRepository.GetMany(t => t.Id == clientSocialMediaObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientSocialMedia(clientSocialMediaObj);
            }
            else
            {
                clientSocialMediaObj.CreatedOn = existingItem.CreatedOn;
                clientSocialMediaObj.CreatedBy = existingItem.CreatedBy;                
                return await UpdateClientSocialMedia(clientSocialMediaObj, existingItem.Id);
            }
        }
        public async Task<ClientSocialMedia> InsertClientSocialMedia(ClientSocialMedia ClientSocialMediaObj)
        {
            ClientSocialMediaObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientSocialMediaRepository.AddAsync(ClientSocialMediaObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public async Task<ClientSocialMedia> UpdateClientSocialMedia(ClientSocialMedia updatedItem, int existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.ClientSocialMediaRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }        
        public List<ClientSocialMedia> GetByClientId(long ClientId)
        {
            return _unitOfWork.ClientSocialMediaRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.Include(t => t.SocialMedia).ToList();
        }
        public async Task<ClientSocialMedia> DeleteById(int Id)
        {
            var clientSocialMediaObj = _unitOfWork.ClientSocialMediaRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientSocialMediaObj != null)
            {
                clientSocialMediaObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientSocialMediaRepository.UpdateAsync(clientSocialMediaObj, clientSocialMediaObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return clientSocialMediaObj;
        }
        public ClientSocialMedia GetById(int Id)
        {
            return _unitOfWork.ClientSocialMediaRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.SocialMedia).FirstOrDefault();
        }
        public async Task<List<ClientSocialMedia>> DeleteByClientId(long ClientId)
        {
            var clientSocialMediaList = _unitOfWork.ClientSocialMediaRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (clientSocialMediaList != null && clientSocialMediaList.Count() > 0)
            {
                foreach (var existingItem in clientSocialMediaList)
                {                    
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientSocialMediaRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return clientSocialMediaList;
        }
    }
    public partial interface IClientSocialMediaService : IService<ClientSocialMedia>
    {
        Task<ClientSocialMedia> CheckInsertOrUpdate(ClientSocialMedia model);
        List<ClientSocialMedia> GetByClientId(long ClientId);
        Task<ClientSocialMedia> DeleteById(int Id);
        ClientSocialMedia GetById(int Id);
        Task<List<ClientSocialMedia>> DeleteByClientId(long ClientId);
    }
}