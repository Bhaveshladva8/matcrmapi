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
    public partial class OneClappLatestThemeConfigService : Service<OneClappLatestThemeConfig>, IOneClappLatestThemeConfigService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappLatestThemeConfigService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappLatestThemeConfig> CheckInsertOrUpdate(OneClappLatestThemeConfigDto model)
        {
            var oneClappLatestThemeConfigObj = _mapper.Map<OneClappLatestThemeConfig>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeConfigRepository.GetMany(t => t.CreatedBy == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddOneClappLatestThemeConfig(oneClappLatestThemeConfigObj);
            }
            else
            {
                oneClappLatestThemeConfigObj.Id = existingItem.Id;
                oneClappLatestThemeConfigObj.CreatedOn = existingItem.CreatedOn;
                oneClappLatestThemeConfigObj.CreatedBy = existingItem.CreatedBy;
                return await UpdateOneClappLatestThemeConfig(oneClappLatestThemeConfigObj, existingItem.Id);
            }
        }
        public async Task<OneClappLatestThemeConfig> UpdateOneClappLatestThemeConfig(OneClappLatestThemeConfig updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappLatestThemeConfigRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<OneClappLatestThemeConfig> AddOneClappLatestThemeConfig(OneClappLatestThemeConfig oneClappLatestThemeConfigObj)
        {
            oneClappLatestThemeConfigObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappLatestThemeConfigRepository.AddAsync(oneClappLatestThemeConfigObj);
           await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<OneClappLatestThemeConfig> GetAll()
        {
            return _unitOfWork.OneClappLatestThemeConfigRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappLatestThemeConfig GetOneClappLatestThemeConfigById(long Id)
        {
            return _unitOfWork.OneClappLatestThemeConfigRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappLatestThemeConfig DeleteOneClappLatestThemeConfig(OneClappLatestThemeConfigDto model)
        {
            var OneClappLatestThemeConfigObj = _mapper.Map<OneClappLatestThemeConfig>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeConfigRepository.GetMany(t => t.Id == OneClappLatestThemeConfigObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappLatestThemeConfigRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IOneClappLatestThemeConfigService : IService<OneClappLatestThemeConfig>
    {
        Task<OneClappLatestThemeConfig> CheckInsertOrUpdate(OneClappLatestThemeConfigDto model);
        List<OneClappLatestThemeConfig> GetAll();
        OneClappLatestThemeConfig DeleteOneClappLatestThemeConfig(OneClappLatestThemeConfigDto model);
        OneClappLatestThemeConfig GetOneClappLatestThemeConfigById(long Id);
    }
}