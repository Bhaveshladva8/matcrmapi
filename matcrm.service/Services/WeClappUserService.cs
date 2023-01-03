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
    public partial class WeClappUserService : Service<WeClappUser>, IWeClappUserService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantService _tenantService;

        public WeClappUserService(IUnitOfWork unitOfWork, ITenantService tenantService,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tenantService = tenantService;
        }
        public async Task<WeClappUser> CheckInsertOrUpdate(WeClappUserDto model)
        {
            var weClappUserObj = _mapper.Map<WeClappUser>(model);

            var existingItem = _unitOfWork.WeClappUserRepository.GetMany(t => t.UserId == weClappUserObj.UserId && t.IsDeleted == false).Result.FirstOrDefault();

            if (existingItem == null)
            {
                return await InsertWeClappUser(weClappUserObj);
            }
            else
            {
                existingItem.TenantName = weClappUserObj.TenantName;
                existingItem.ApiKey = weClappUserObj.ApiKey;
                return await UpdateWeClappUser(existingItem, existingItem.Id);
            }
        }

        public async Task<WeClappUser> InsertWeClappUser(WeClappUser weClappUserObj)
        {
            weClappUserObj.CreatedOn = DateTime.UtcNow;
            weClappUserObj.UserId = weClappUserObj.UserId;
            var newItem = await _unitOfWork.WeClappUserRepository.AddAsync(weClappUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<WeClappUser> UpdateWeClappUser(WeClappUser existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.WeClappUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public WeClappUser GetByUser(int UserId)
        {
            return _unitOfWork.WeClappUserRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public WeClappUser GetById(long Id)
        {
            return _unitOfWork.WeClappUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public WeClappUser GetByTenantName(string TenantName)
        {
            return _unitOfWork.WeClappUserRepository.GetMany(t => t.TenantName == TenantName && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<WeClappUser> DeleteById(long Id)
        {
            var weClappUserObj = _unitOfWork.WeClappUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (weClappUserObj != null)
            {
                weClappUserObj.IsDeleted = true;
                weClappUserObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.WeClappUserRepository.UpdateAsync(weClappUserObj, Id);
                await _unitOfWork.CommitAsync();
            }
            return weClappUserObj;
        }

        public WeClappUser DeleteByUser(int UserId)
        {
            var weClappUserObj = _unitOfWork.WeClappUserRepository.GetMany(t => t.UserId == UserId).Result.FirstOrDefault();
            if (weClappUserObj != null)
            {
                weClappUserObj.IsDeleted = true;
                weClappUserObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.WeClappUserRepository.UpdateAsync(weClappUserObj, weClappUserObj.Id);
                _unitOfWork.CommitAsync();
            }
            return weClappUserObj;
        }
    }

    public partial interface IWeClappUserService : IService<WeClappUser>
    {
        Task<WeClappUser> CheckInsertOrUpdate(WeClappUserDto model);
        WeClappUser GetByUser(int UserId);
        WeClappUser GetById(long Id);
        WeClappUser GetByTenantName(string TenantName);
        Task<WeClappUser> DeleteById(long Id);
        WeClappUser DeleteByUser(int UserId);
    }
}