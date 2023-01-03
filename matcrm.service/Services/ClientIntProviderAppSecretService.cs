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
    public partial class ClientIntProviderAppSecretService : Service<ClientIntProviderAppSecret>, IClientIntProviderAppSecretService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ClientIntProviderAppSecretService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientIntProviderAppSecret> CheckInsertOrUpdate(ClientIntProviderAppSecret intProviderAppSecretObj, bool IsOtherMail = false)
        {            
            ClientIntProviderAppSecret? existingItem = null;
            if (IsOtherMail == true)
            {
                existingItem = _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.LoggedInUserId == intProviderAppSecretObj.LoggedInUserId && t.Email == intProviderAppSecretObj.Email && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {                
                existingItem = _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.LoggedInUserId == intProviderAppSecretObj.LoggedInUserId && t.Email == intProviderAppSecretObj.Email && t.IntProviderAppId == intProviderAppSecretObj.IntProviderAppId && t.DeletedOn == null).Result.FirstOrDefault();
            }
            if (existingItem == null)
            {
                return await InsertIntProviderAppSecret(intProviderAppSecretObj);
            }
            else
            {
                existingItem.Access_Token = intProviderAppSecretObj.Access_Token;
                existingItem.Expires_In = intProviderAppSecretObj.Expires_In;
                existingItem.Token_Type = intProviderAppSecretObj.Token_Type;
                existingItem.Id_Token = intProviderAppSecretObj.Id_Token;
                return await UpdateIntProviderAppSecret(existingItem, existingItem.Id);
            }
        }

        public async Task<ClientIntProviderAppSecret> InsertIntProviderAppSecret(ClientIntProviderAppSecret intProviderAppSecretObj)
        {
            intProviderAppSecretObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientIntProviderAppSecretRepository.AddAsync(intProviderAppSecretObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientIntProviderAppSecret> UpdateIntProviderAppSecret(ClientIntProviderAppSecret existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientIntProviderAppSecretRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public ClientIntProviderAppSecret GetActiveSecretByUserAndEmail(int UserId, string email, long appId)
        {
            return _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.LoggedInUserId == UserId && t.Email == email && t.IntProviderAppId == appId && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public List<ClientIntProviderAppSecret> GetAllByClientId(long ClientId)
        {
            return _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.Include(t => t.IntProviderApp).Include(t => t.IntProviderApp.IntProvider).ToList();
        }
        public ClientIntProviderAppSecret GetActiveByClientAndEmail(long ClientId, string email, long appId)
        {
            return _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.ClientId == ClientId && t.Email == email && t.IntProviderAppId == appId && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public async Task<List<ClientIntProviderAppSecret>> DeleteByClientId(long ClientId)
        {
            var intProviderAppSecretList = _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (intProviderAppSecretList != null && intProviderAppSecretList.Count() > 0)
            {
                foreach (var existingItem in intProviderAppSecretList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientIntProviderAppSecretRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return intProviderAppSecretList;
        }
        public ClientIntProviderAppSecret GetAllByUserId(int UserId)
        {
            return _unitOfWork.ClientIntProviderAppSecretRepository.GetMany(t => t.LoggedInUserId == UserId && t.DeletedOn == null).Result.Include(t => t.IntProviderApp).Include(t => t.IntProviderApp.IntProvider).FirstOrDefault();
        }
    }
    public partial interface IClientIntProviderAppSecretService : IService<ClientIntProviderAppSecret>
    {
        Task<ClientIntProviderAppSecret> CheckInsertOrUpdate(ClientIntProviderAppSecret model, bool IsOtherMail = false);
        ClientIntProviderAppSecret GetActiveSecretByUserAndEmail(int UserId, string email, long appId);
        List<ClientIntProviderAppSecret> GetAllByClientId(long ClientId);
        ClientIntProviderAppSecret GetActiveByClientAndEmail(long ClientId, string email, long appId);
        Task<List<ClientIntProviderAppSecret>> DeleteByClientId(long ClientId);
        ClientIntProviderAppSecret GetAllByUserId(int UserId);
    }
}