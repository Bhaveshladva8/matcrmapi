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
    public partial class OneClappLatestThemeSchemeService : Service<OneClappLatestThemeScheme>, IOneClappLatestThemeSchemeService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappLatestThemeSchemeService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappLatestThemeScheme> CheckInsertOrUpdate(OneClappLatestThemeSchemeDto model)
        {
            var oneClappLatestThemeSchemeObj = _mapper.Map<OneClappLatestThemeScheme>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeSchemeRepository.GetMany(t => t.Id == oneClappLatestThemeSchemeObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddOneClappLatestThemeScheme(oneClappLatestThemeSchemeObj);
            }
            else
            {
                return await UpdateOneClappLatestThemeScheme(oneClappLatestThemeSchemeObj, existingItem.Id);
            }
        }
        public async Task<OneClappLatestThemeScheme> UpdateOneClappLatestThemeScheme(OneClappLatestThemeScheme updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappLatestThemeSchemeRepository.UpdateAsync(updatedItem, existingId);
           await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<OneClappLatestThemeScheme> AddOneClappLatestThemeScheme(OneClappLatestThemeScheme oneClappLatestThemeSchemeObj)
        {
            oneClappLatestThemeSchemeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappLatestThemeSchemeRepository.AddAsync(oneClappLatestThemeSchemeObj);
           await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<OneClappLatestThemeScheme> GetAll()
        {
            return _unitOfWork.OneClappLatestThemeSchemeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappLatestThemeScheme GetOneClappLatestThemeSchemeById(long Id)
        {
            return _unitOfWork.OneClappLatestThemeSchemeRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappLatestThemeScheme GetByName(string Name)
        {
            return _unitOfWork.OneClappLatestThemeSchemeRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }
        public OneClappLatestThemeScheme DeleteOneClappLatestThemeScheme(OneClappLatestThemeSchemeDto model)
        {
            var oneClappLatestThemeSchemeObj = _mapper.Map<OneClappLatestThemeScheme>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeSchemeRepository.GetMany(t => t.Id == oneClappLatestThemeSchemeObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappLatestThemeSchemeRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }
    public partial interface IOneClappLatestThemeSchemeService : IService<OneClappLatestThemeScheme>
    {
        Task<OneClappLatestThemeScheme> CheckInsertOrUpdate(OneClappLatestThemeSchemeDto model);
        List<OneClappLatestThemeScheme> GetAll();
        OneClappLatestThemeScheme GetByName(string Name);
        OneClappLatestThemeScheme DeleteOneClappLatestThemeScheme(OneClappLatestThemeSchemeDto model);
        OneClappLatestThemeScheme GetOneClappLatestThemeSchemeById(long Id);
    }
}