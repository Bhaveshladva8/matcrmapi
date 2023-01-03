using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TenantActivityService : Service<TenantActivity>, ITenantActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TenantActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TenantActivity> CheckInsertOrUpdate(TenantActivity tenantActivityObj)
        {
            // var existingItem = _unitOfWork.TenantActivityRepository.GetMany (t => t.UserId == obj.UserId && t.TenantId == obj.TenantId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertTenantActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateTenantActivity (existingItem, existingItem.Id);
            // }
            return await InsertTenantActivity(tenantActivityObj);
        }

        public async Task<TenantActivity> InsertTenantActivity(TenantActivity tenantActivityObj)
        {
            tenantActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TenantActivityRepository.AddAsync(tenantActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<TenantActivity> UpdateTenantActivity(TenantActivity existingItem, int existingId)
        {
            await _unitOfWork.TenantActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<TenantActivity> GetAllByTenantId(long tenantId)
        {
            return _unitOfWork.TenantActivityRepository.GetMany(t => t.TenantId == tenantId).Result.ToList();
        }

        public TenantActivity GetTenantActivitytById(long Id)
        {
            return _unitOfWork.TenantActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public List<TenantActivity> GetTenantActivitytByUser(long UserId)
        {
            return _unitOfWork.TenantActivityRepository.GetMany(t => t.UserId == UserId).Result.ToList();
        }

    }

    public partial interface ITenantActivityService : IService<TenantActivity>
    {
        Task<TenantActivity> CheckInsertOrUpdate(TenantActivity obj);
        List<TenantActivity> GetAllByTenantId(long tenantId);
        TenantActivity GetTenantActivitytById(long Id);
        List<TenantActivity> GetTenantActivitytByUser(long UserId);
    }
}