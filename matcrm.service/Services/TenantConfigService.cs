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
    public class TenantConfigService  : Service<TenantConfig>, ITenantConfigService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TenantConfigService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<TenantConfig> CheckInsertOrUpdate (TenantConfigDto model) {
            var tenantConfigObj = _mapper.Map<TenantConfig> (model);
            TenantConfig existingItem = _unitOfWork.TenantConfigRepository.GetMany(t => t.TenantId == tenantConfigObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();

            if (existingItem == null) {
                return await InsertTenantConfig (tenantConfigObj);
            } else {
                tenantConfigObj.CreatedOn = existingItem.CreatedOn;
                tenantConfigObj.TenantId = existingItem.TenantId;
                return await UpdateTenantConfig (tenantConfigObj, existingItem.Id);
            }
        }

        public async Task<TenantConfig> InsertTenantConfig (TenantConfig tenantConfigObj) {
            tenantConfigObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TenantConfigRepository.AddAsync (tenantConfigObj);
           await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<TenantConfig> UpdateTenantConfig (TenantConfig existingItem, long existingId) {
            existingItem.UpdatedOn = DateTime.UtcNow;

            await _unitOfWork.TenantConfigRepository.UpdateAsync (existingItem, existingId);
            await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<TenantConfig> GetAll () {
            return _unitOfWork.TenantConfigRepository.GetMany (t => t.IsDeleted == false).Result.ToList();
        }

        public TenantConfig GetTenantConfigById (long Id) {
           return _unitOfWork.TenantConfigRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public TenantConfig GetConfigByTenant (long TenantId) {
            return _unitOfWork.TenantConfigRepository.GetMany (t => t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public TenantConfig DeleteTenantConfig (long Id) {
            var tenantConfigObj = GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
            tenantConfigObj.IsDeleted = true;
            tenantConfigObj.DeletedOn = DateTime.UtcNow;

            _unitOfWork.TenantConfigRepository.UpdateAsync (tenantConfigObj, tenantConfigObj.Id);
            _unitOfWork.CommitAsync();

            return tenantConfigObj;
        }
    }

    public partial interface ITenantConfigService : IService<TenantConfig> {
        Task<TenantConfig> CheckInsertOrUpdate (TenantConfigDto model);
        List<TenantConfig> GetAll ();
        TenantConfig GetTenantConfigById (long Id);
        TenantConfig GetConfigByTenant (long tenantId);
        Task<TenantConfig> UpdateTenantConfig (TenantConfig existingItem, long existingId);
        TenantConfig DeleteTenantConfig (long Id);
    }
}