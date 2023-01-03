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
    public partial class ServiceArticlePriceService : Service<ServiceArticlePrice>, IServiceArticlePriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ServiceArticlePriceService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceArticlePrice> CheckInsertOrUpdate(ServiceArticlePrice serviceArticlePriceObj)
        {
            var existingItem = _unitOfWork.ServiceArticlePriceRepository.GetMany(t => t.Id == serviceArticlePriceObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertServiceArticlePrice(serviceArticlePriceObj);
            }
            else
            {
                serviceArticlePriceObj.LoggedInUserId = existingItem.LoggedInUserId;
                serviceArticlePriceObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateServiceArticlePrice(serviceArticlePriceObj, existingItem.Id);
            }
        }

        public async Task<ServiceArticlePrice> InsertServiceArticlePrice(ServiceArticlePrice serviceArticlePriceObj)
        {
            serviceArticlePriceObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ServiceArticlePriceRepository.AddAsync(serviceArticlePriceObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ServiceArticlePrice> UpdateServiceArticlePrice(ServiceArticlePrice existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ServiceArticlePriceRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<ServiceArticlePrice> GetAllByClientId(long ClientId)
        {
            return _unitOfWork.ServiceArticlePriceRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.Include(t => t.ServiceArticle).ToList();
        }
        public ServiceArticlePrice GetById(long Id)
        {
            return _unitOfWork.ServiceArticlePriceRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.ServiceArticle).FirstOrDefault();
        }
        public async Task<ServiceArticlePrice> DeleteById(int Id)
        {
            var serviceArticlePriceObj = _unitOfWork.ServiceArticlePriceRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (serviceArticlePriceObj != null)
            {
                serviceArticlePriceObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ServiceArticlePriceRepository.UpdateAsync(serviceArticlePriceObj, serviceArticlePriceObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return serviceArticlePriceObj;
        }
        public async Task<List<ServiceArticlePrice>> DeleteByClientId(long ClientId)
        {
            var serviceArticlePriceList = _unitOfWork.ServiceArticlePriceRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (serviceArticlePriceList != null && serviceArticlePriceList.Count() > 0)
            {
                foreach (var existingItem in serviceArticlePriceList)
                {                    
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ServiceArticlePriceRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return serviceArticlePriceList;
        }
    }
    public partial interface IServiceArticlePriceService : IService<ServiceArticlePrice>
    {
        Task<ServiceArticlePrice> CheckInsertOrUpdate(ServiceArticlePrice model);
        List<ServiceArticlePrice> GetAllByClientId(long ClientId);
        ServiceArticlePrice GetById(long Id);
        Task<ServiceArticlePrice> DeleteById(int Id);
        Task<List<ServiceArticlePrice>> DeleteByClientId(long ClientId);
    }
}