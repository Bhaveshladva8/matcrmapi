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
    public partial class ServiceArticleService : Service<ServiceArticle>, IServiceArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ServiceArticleService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceArticle> CheckInsertOrUpdate(ServiceArticle serviceArticleObj)
        {
            var existingItem = _unitOfWork.ServiceArticleRepository.GetMany(t => t.Id == serviceArticleObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertServiceArticle(serviceArticleObj);
            }
            else
            {
                serviceArticleObj.CreatedBy = existingItem.CreatedBy;
                serviceArticleObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateServiceArticle(serviceArticleObj, existingItem.Id);
            }
        }

        public async Task<ServiceArticle> InsertServiceArticle(ServiceArticle serviceArticleObj)
        {
            serviceArticleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ServiceArticleRepository.AddAsync(serviceArticleObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ServiceArticle> UpdateServiceArticle(ServiceArticle existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ServiceArticleRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<ServiceArticle> GetByTenant(int tenantId)
        {
            return _unitOfWork.ServiceArticleRepository.GetMany(t => t.User.TenantId == tenantId && t.DeletedOn == null).Result.Include(t => t.ServiceArticleCategory).Include(t => t.Currency).ToList();
        }
        public ServiceArticle GetById(long Id)
        {
            return _unitOfWork.ServiceArticleRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.ServiceArticleCategory).Include(t => t.Currency).Include(t => t.Tax).FirstOrDefault();
        }
        public async Task<ServiceArticle> DeleteServiceArticle(long Id)
        {
            var serviceArticleObj = _unitOfWork.ServiceArticleRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (serviceArticleObj != null)
            {                
                serviceArticleObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ServiceArticleRepository.UpdateAsync(serviceArticleObj, serviceArticleObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return serviceArticleObj;
        }
    }
    public partial interface IServiceArticleService : IService<ServiceArticle>
    {
        Task<ServiceArticle> CheckInsertOrUpdate(ServiceArticle model);
        List<ServiceArticle> GetByTenant(int tenantId);
        ServiceArticle GetById(long Id);
        Task<ServiceArticle> DeleteServiceArticle(long Id);
    }
}