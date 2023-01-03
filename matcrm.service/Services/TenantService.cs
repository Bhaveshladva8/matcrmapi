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
    public partial class TenantService : Service<Tenant>, ITenantService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TenantService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Tenant> CheckInsertOrUpdate(TenantDto tenantDto)
        {
            var tenantObj = _mapper.Map<Tenant>(tenantDto);
            var existingItem = _unitOfWork.TenantRepository.GetMany(t => t.TenantName == tenantObj.TenantName && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault();

            if (existingItem != null)
            {
                return await UpdateTenant(existingItem);
            }
            else
            {
                return await InsertTenant(tenantObj);
            }
        }
        public async Task<Tenant> InsertTenant(Tenant tenantObj)
        {
            tenantObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TenantRepository.AddAsync(tenantObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Tenant> UpdateTenant(Tenant existingItem)
        {
            var newItem = await _unitOfWork.TenantRepository.UpdateAsync(existingItem, existingItem.TenantId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public async Task<Tenant> Block(int tenantId)
        {

            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.TenantId == tenantId).Result.FirstOrDefault();
            var adminObj = _unitOfWork.TenantRepository.GetMany(t => t.IsAdmin == true).Result.FirstOrDefault();

            if (alreadyTenant == null)
                return null;

            alreadyTenant.IsBlocked = true;
            alreadyTenant.BlockedOn = DateTime.UtcNow;
            if (adminObj != null)
            {
                alreadyTenant.BlockedBy = adminObj.TenantId;
            }
            return await UpdateTenant(alreadyTenant);
        }

        public async Task<Tenant> Revoke(int tenantId)
        {
            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.TenantId == tenantId).Result.FirstOrDefault();

            if (alreadyTenant == null)
                return null;

            alreadyTenant.IsBlocked = false;
            alreadyTenant.BlockedOn = null;
            alreadyTenant.BlockedBy = null;
            return await UpdateTenant(alreadyTenant);
        }

        public List<Tenant> RevokeAllBlocked()
        {
            var tenants = _unitOfWork.TenantRepository.GetMany(t => t.IsBlocked == true).Result.ToList();
            if (tenants != null && tenants.Count() > 0)
            {
                foreach (var tenant in tenants)
                {
                    tenant.IsBlocked = false;
                    tenant.BlockedOn = null;
                    tenant.BlockedOn = null;
                    tenant.BlockedBy = null;
                    UpdateTenant(tenant);
                }
            }
            List<Tenant> tenantList = new List<Tenant>(tenants);
            return tenantList;
        }

        public List<Tenant> GetAll()
        {
            return _unitOfWork.TenantRepository.GetMany(t => t.IsAdmin == false).Result.ToList();
        }

        public List<Tenant> GetAllTenantByAdmin()
        {
            return _unitOfWork.TenantRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public bool IsBlocked(string tenant)
        {
            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.TenantName.ToLower() == tenant.ToLower()).Result.FirstOrDefault();
            if (alreadyTenant == null)
                return true;

            if (alreadyTenant.IsBlocked)
                return true;
            else
                return false;
        }

        public Tenant GetTenant(string tenant)
        {
            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.TenantName.ToLower() == tenant.ToLower() && t.IsBlocked == false && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyTenant;
        }

        public Tenant GetAdmin()
        {
            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.IsBlocked == false && t.IsAdmin == true && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyTenant;
        }

        public Tenant GetTenantById(int tenantId)
        {
            var alreadyTenant = _unitOfWork.TenantRepository.GetMany(t => t.TenantId == tenantId && t.IsBlocked == false && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyTenant;
        }

        public Tenant DeleteTenant(int tenantId)
        {
            var tenantObj = _unitOfWork.TenantRepository.GetMany(u => u.TenantId == tenantId && u.IsDeleted == false).Result.FirstOrDefault();
            if (tenantObj != null)
            {
                tenantObj.IsDeleted = true;
                tenantObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.TenantRepository.UpdateAsync(tenantObj, tenantObj.TenantId);
                _unitOfWork.CommitAsync();
            }
            return tenantObj;
        }
    }

    public partial interface ITenantService : IService<Tenant>
    {
        Task<Tenant> CheckInsertOrUpdate(TenantDto tenant);

        // Tenant Update(Tenant tenant);
        Task<Tenant> Block(int tenantId);

        Task<Tenant> Revoke(int tenantId);
        List<Tenant> GetAll();
        List<Tenant> RevokeAllBlocked();
        bool IsBlocked(string tenant);
        Tenant GetTenant(string tenant);
        Tenant GetTenantById(int tenantId);
        Tenant GetAdmin();
        List<Tenant> GetAllTenantByAdmin();
        Tenant DeleteTenant(int tenantId);
    }
}