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
    public partial class RoleService : Service<Role>, IRoleService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantService _tenantService;

        public RoleService(IUnitOfWork unitOfWork, ITenantService tenantService,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tenantService = tenantService;
        }
        public async Task<Role> CheckInsertOrUpdate(RoleDto model)
        {
            var roleObj = _mapper.Map<Role>(model);
            var existingItem = new Role();
            // if (obj.TenantId != null) {
            //     existingItem = _unitOfWork.RoleRepository.GetMany (t => t.RoleId == obj.RoleId && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            // } else {
            //     existingItem = _unitOfWork.RoleRepository.GetMany (t => t.RoleId == obj.RoleId && t.IsDeleted == false).Result.FirstOrDefault ();
            // }

            if (model.IsDefault == false)
            {
                existingItem = _unitOfWork.RoleRepository.GetMany(t => t.RoleId == roleObj.RoleId && t.TenantId == roleObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.RoleRepository.GetMany(t => t.RoleId == roleObj.RoleId && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertRole(roleObj);
            }
            else
            {

                existingItem.RoleName = roleObj.RoleName;
                existingItem.IsActive = roleObj.IsActive;
                // existingItem.TenantId = obj.TenantId;
                existingItem.UpdatedBy = model.userId;

                return await UpdateRole(existingItem, existingItem.RoleId);
            }
        }

        public async Task<Role> InsertRole(Role roleObj)
        {
            roleObj.CreatedOn = DateTime.UtcNow;
            roleObj.CreatedBy = roleObj.CreatedBy;
            var newItem = await _unitOfWork.RoleRepository.AddAsync(roleObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Role> UpdateRole(Role existingItem, int existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;

            await _unitOfWork.RoleRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<Role> GetAllByAdmin()
        {
            return _unitOfWork.RoleRepository.GetMany(t => t.TenantId == null && t.IsDeleted == false).Result.ToList();
        }

        public List<Role> GetAllByTenantAdmin(long? TenantId)
        {
            var tenantObj = _tenantService.GetTenantById(Convert.ToInt32(TenantId));

            if (tenantObj.TenantName.ToLower() == "admin")
            {
                return _unitOfWork.RoleRepository.GetMany(t => t.TenantId == TenantId || t.TenantId == null && t.IsDeleted == false).Result.ToList();
            }
            return _unitOfWork.RoleRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.ToList();
        }

        // public List<Role> GetAllRoleByAdmin (long? TenantId)
        // {
        //     return _unitOfWork.RoleRepository.GetMany (t => t.TenantId == TenantId).Result.ToList ();
        // }

        public List<Role> GetAllActive()
        {
            return _unitOfWork.RoleRepository.GetMany(t => t.IsDeleted == false && t.IsActive == true).Result.ToList();
        }

        public Role GetRole(string RoleName)
        {
            return _unitOfWork.RoleRepository.GetMany(t => t.RoleName == RoleName && t.IsDeleted == false && t.IsActive == true).Result.FirstOrDefault();
        }

        public Role GetRoleById(int RoleId)
        {
            return _unitOfWork.RoleRepository.GetMany(t => t.RoleId == RoleId && t.IsDeleted == false && t.IsActive == true).Result.FirstOrDefault();
        }

        public Role GetRoleByTenant(int RoleId, long TenantId)
        {
            return _unitOfWork.RoleRepository.GetMany(t => t.RoleId == RoleId && t.TenantId == TenantId && t.IsDeleted == false && t.IsActive == true).Result.FirstOrDefault();
        }

        public Role DeleteRole(int RoleId)
        {
            var roleObj = _unitOfWork.RoleRepository.GetMany(t => t.RoleId == RoleId).Result.FirstOrDefault();
            if (roleObj != null)
            {
                roleObj.IsDeleted = true;
                // RoleToDelete.DeletedBy = RoleToDelete.TenantId;
                roleObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.RoleRepository.UpdateAsync(roleObj, roleObj.RoleId);
                _unitOfWork.CommitAsync();
            }
            return roleObj;
        }

        // public Role DeleteRoleByTenantAdmin(int RoleId, long TenantId)
        // {
        //     var RoleToDelete = _unitOfWork.RoleRepository.GetMany(t => t.RoleId == RoleId && t.TenantId == TenantId).Result.FirstOrDefault();
        //     RoleToDelete.IsDeleted = true;
        //     RoleToDelete.DeletedBy = RoleToDelete.TenantId;
        //     RoleToDelete.DeletedOn = DateTime.UtcNow;

        //     _unitOfWork.RoleRepository.UpdateAsync(RoleToDelete, RoleToDelete.RoleId);
        //     _unitOfWork.CommitAsync();

        //     return RoleToDelete;
        // }

        public Role DeleteRoleByTenantAdmin(RoleDto role)
        {
            var roleObj = _unitOfWork.RoleRepository.GetMany(t => t.RoleId == role.RoleId).Result.FirstOrDefault();
            if (roleObj != null)
            {
                roleObj.IsDeleted = true;
                roleObj.DeletedBy = role.userId;
                roleObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.RoleRepository.UpdateAsync(roleObj, roleObj.RoleId);
                _unitOfWork.CommitAsync();
            }
            return roleObj;
        }
    }

    public partial interface IRoleService : IService<Role>
    {
        Task<Role> CheckInsertOrUpdate(RoleDto model);
        List<Role> GetAllByAdmin();
        List<Role> GetAllByTenantAdmin(long? TenantId);
        // List<Role> GetAllRoleByAdmin (long? TenantId);
        List<Role> GetAllActive();
        Role GetRole(string RoleName);
        Role GetRoleById(int RoleId);
        Role GetRoleByTenant(int RoleId, long TenantId);
        Role DeleteRole(int RoleId);
        Role DeleteRoleByTenantAdmin(RoleDto role);
    }
}