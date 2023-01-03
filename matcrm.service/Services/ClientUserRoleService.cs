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
    public partial class ClientUserRoleService : Service<ClientUserRole>, IClientUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientUserRoleService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ClientUserRole> CheckInsertOrUpdate(ClientUserRole clientUserRoleObj)
        {
            var existingItem = _unitOfWork.ClientUserRoleRepository.GetMany(t => t.Id == clientUserRoleObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientUserRole(clientUserRoleObj);
            }
            else
            {
                clientUserRoleObj.CreatedBy = existingItem.CreatedBy;
                clientUserRoleObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClientUserRole(clientUserRoleObj, existingItem.Id);
            }
        }
        public async Task<ClientUserRole> InsertClientUserRole(ClientUserRole clientUserRoleObj)
        {
            clientUserRoleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientUserRoleRepository.AddAsync(clientUserRoleObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientUserRole> UpdateClientUserRole(ClientUserRole existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientUserRoleRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<ClientUserRole> DeleteById(long Id)
        {
            var clientUserRoleObj = _unitOfWork.ClientUserRoleRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientUserRoleObj != null)
            {
                clientUserRoleObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientUserRoleRepository.UpdateAsync(clientUserRoleObj, clientUserRoleObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return clientUserRoleObj;
        }
        public List<ClientUserRole> GetAll()
        {
            return _unitOfWork.ClientUserRoleRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public ClientUserRole GetById(long Id)
        {
            return _unitOfWork.ClientUserRoleRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<ClientUserRole> GetByTenant(long TenantId)
        {
            return _unitOfWork.ClientUserRoleRepository.GetMany(t => t.DeletedOn == null && (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId)).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IClientUserRoleService : IService<ClientUserRole>
    {
        Task<ClientUserRole> CheckInsertOrUpdate(ClientUserRole model);
        Task<ClientUserRole> DeleteById(long Id);
        List<ClientUserRole> GetAll();
        ClientUserRole GetById(long Id);
        List<ClientUserRole> GetByTenant(long TenantId);
    }
}