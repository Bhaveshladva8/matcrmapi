using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;
using matcrm.data;
using matcrm.data.Models.Request;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class ServiceArticleCategoryService : Service<ServiceArticleCategory>, IServiceArticleCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ServiceArticleCategoryService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceArticleCategory> CheckInsertOrUpdate(ServiceArticleCategory serviceArticleCategoryObj)
        {            
            var existingItem = _unitOfWork.ServiceArticleCategoryRepository.GetMany(t => t.Id == serviceArticleCategoryObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertServiceArticleCategory(serviceArticleCategoryObj);
            }
            else
            {
                serviceArticleCategoryObj.CreatedBy = existingItem.CreatedBy;
                serviceArticleCategoryObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateServiceArticleCategory(serviceArticleCategoryObj, existingItem.Id);
            }
        }

        public async Task<ServiceArticleCategory> InsertServiceArticleCategory(ServiceArticleCategory serviceArticleCategoryObj)
        {
            serviceArticleCategoryObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ServiceArticleCategoryRepository.AddAsync(serviceArticleCategoryObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<ServiceArticleCategory> UpdateServiceArticleCategory(ServiceArticleCategory existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ServiceArticleCategoryRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<ServiceArticleCategory> GetByTenant(int tenantId)
        {
            return _unitOfWork.ServiceArticleCategoryRepository.GetMany(t => t.TenantId == tenantId && t.DeletedOn == null).Result.ToList();
        }
        public ServiceArticleCategory GetById(long Id)
        {
            return _unitOfWork.ServiceArticleCategoryRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }
        public async Task<ServiceArticleCategory> DeleteServiceArticleCategory(long Id,int deletedby)
        {
            var serviceArticleCategoryObj = _unitOfWork.ServiceArticleCategoryRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (serviceArticleCategoryObj != null)
            {                
                serviceArticleCategoryObj.DeletedBy = deletedby;
                serviceArticleCategoryObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ServiceArticleCategoryRepository.UpdateAsync(serviceArticleCategoryObj, serviceArticleCategoryObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return serviceArticleCategoryObj;
        }

    }

    public partial interface IServiceArticleCategoryService : IService<ServiceArticleCategory>
    {
        Task<ServiceArticleCategory> CheckInsertOrUpdate(ServiceArticleCategory serviceArticleCategoryObj);
        List<ServiceArticleCategory> GetByTenant(int tenantId);
        ServiceArticleCategory GetById(long Id);
        Task<ServiceArticleCategory> DeleteServiceArticleCategory(long Id,int deletedby);
    }
}