using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class TenantModuleService : Service<TenantModule>, ITenantModuleService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TenantModuleService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TenantModule> CheckInsertOrUpdate (TenantModuleDto model) {
            var tenantModuleObj = _mapper.Map<TenantModule> (model);
            var existingItem = _unitOfWork.TenantModuleRepository.GetMany (t => t.ModuleId == tenantModuleObj.ModuleId && t.TenantId == tenantModuleObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertTenantModule (tenantModuleObj);
            } else {
                existingItem.ModuleId = tenantModuleObj.ModuleId;
                return await UpdateTenantModule (existingItem, existingItem.Id);
            }
        }

        public async Task<TenantModule> InsertTenantModule (TenantModule tenantModuleObj) {
            tenantModuleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TenantModuleRepository.AddAsync (tenantModuleObj);
           await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<TenantModule> UpdateTenantModule (TenantModule existingItem, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
           await _unitOfWork.TenantModuleRepository.UpdateAsync (existingItem, existingId);
           await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public TenantModule GetTenantModule (long ModuleId, long TenantId) {
            return _unitOfWork.TenantModuleRepository.GetMany (t => t.ModuleId == ModuleId && t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
        }
    }

    public partial interface ITenantModuleService : IService<TenantModule> {
        Task<TenantModule> CheckInsertOrUpdate (TenantModuleDto model);
        TenantModule GetTenantModule (long ModuleId, long TenantId);
    }
}