using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using matcrm.data.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class ClientUserService : Service<ClientUser>, IClientUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientUserService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientUser> CheckInsertOrUpdate(ClientUser clientUserObj)
        {
            var existingItem = _unitOfWork.ClientUserRepository.GetMany(t => t.Id == clientUserObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientUser(clientUserObj);
            }
            else
            {
                clientUserObj.CreatedBy = existingItem.CreatedBy;
                clientUserObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClientUser(clientUserObj, existingItem.Id);
            }
        }
        public async Task<ClientUser> InsertClientUser(ClientUser clientUserObj)
        {
            clientUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientUserRepository.AddAsync(clientUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientUser> UpdateClientUser(ClientUser existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<ClientUser> DeleteById(long Id)
        {
            var clientUserObj = _unitOfWork.ClientUserRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientUserObj != null)
            {
                clientUserObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientUserRepository.UpdateAsync(clientUserObj, clientUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return clientUserObj;
        }
        public List<ClientUser> GetAll()
        {
            return _unitOfWork.ClientUserRepository.GetMany(t => t.DeletedOn == null).Result.Include(t => t.ClientUserRole).ToList();
        }
        public ClientUser GetById(long Id)
        {
            return _unitOfWork.ClientUserRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.ClientUserRole).Include(t => t.Department).Include(t => t.ReportToUser).Include(t => t.Salutation).FirstOrDefault();
        }
        public List<ClientUser> GetByClientId(long ClientId)
        {
            return _unitOfWork.ClientUserRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.Include(t => t.ClientUserRole).ToList();
        }
        // public List<ClientUser> GetByClientId(ClientUserListRequest model)
        // {
        //     if (!String.IsNullOrEmpty(model.SearchString))
        //     {
        //         var searchString = model.SearchString.ToLower();

        //         return _unitOfWork.ClientUserRepository.GetMany(t => t.ClientId == model.ClientId && t.DeletedOn == null && ((t.FirstName.ToLower().Contains(searchString)))).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
        //     }
        //     else
        //     {
        //         return _unitOfWork.ClientUserRepository.GetMany(t => t.ClientId == model.ClientId && t.DeletedOn == null).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
        //     }
        //     //eturn _unitOfWork.ClientUserRepository.GetMany(t => t.ClientId == t.ClientId && t.DeletedOn == null).Result.Include(t => t.ClientUserRole).ToList();
        // }
        public async Task<List<ClientUser>> DeleteByClientId(long ClientId)
        {
            var clientUserList = _unitOfWork.ClientUserRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (clientUserList != null && clientUserList.Count() > 0)
            {
                foreach (var existingItem in clientUserList)
                {                    
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientUserRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return clientUserList;
        }
        public List<ClientUser> GetByClientUserId(long ClientUserId)
        {
            return _unitOfWork.ClientUserRepository.GetMany(t => t.ReportTo == ClientUserId && t.DeletedOn == null).Result.Include(t => t.ClientUserRole).ToList();
        }
    }
    public partial interface IClientUserService : IService<ClientUser>
    {
        Task<ClientUser> CheckInsertOrUpdate(ClientUser model);
        Task<ClientUser> DeleteById(long Id);
        List<ClientUser> GetAll();
        ClientUser GetById(long Id);
        List<ClientUser> GetByClientId(long ClientId);
        //List<ClientUser> GetByClientId(ClientUserListRequest model);
        Task<ClientUser> InsertClientUser(ClientUser clientUserObj);
        Task<List<ClientUser>> DeleteByClientId(long ClientId);
        List<ClientUser> GetByClientUserId(long ClientUserId);
    }

}