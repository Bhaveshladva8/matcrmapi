using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ServiceArticleHourService : Service<ServiceArticleHour>, IServiceArticleHourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ServiceArticleHourService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceArticleHour> CheckInsertOrUpdate(ServiceArticleHour serviceArticleHourObj)
        {            
            var existingItem = _unitOfWork.ServiceArticleHourRepository.GetMany(t => t.Id == serviceArticleHourObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertServiceArticleHour(serviceArticleHourObj);
            }
            else
            {                
                return await UpdateServiceArticleHour(serviceArticleHourObj, existingItem.Id);
            }
        }

        public async Task<ServiceArticleHour> InsertServiceArticleHour(ServiceArticleHour serviceArticleHourObj)
        {            
            var newItem = await _unitOfWork.ServiceArticleHourRepository.AddAsync(serviceArticleHourObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ServiceArticleHour> UpdateServiceArticleHour(ServiceArticleHour existingItem, long existingId)
        {            
            await _unitOfWork.ServiceArticleHourRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public ServiceArticleHour GetByServiceArticleId(long Id)
        {
            return _unitOfWork.ServiceArticleHourRepository.GetMany(t => t.ServiceArticleId == Id).Result.FirstOrDefault();
        }

    }
    public partial interface IServiceArticleHourService : IService<ServiceArticleHour>
    {
        Task<ServiceArticleHour> CheckInsertOrUpdate(ServiceArticleHour serviceArticleHourObj);
        ServiceArticleHour GetByServiceArticleId(long Id);
    }
}