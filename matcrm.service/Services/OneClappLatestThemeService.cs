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
    public partial class OneClappLatestThemeService : Service<OneClappLatestTheme>, IOneClappLatestThemeService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappLatestThemeService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneClappLatestTheme> CheckInsertOrUpdate(OneClappLatestThemeDto model)
        {
            var oneClappLatestThemeObj = _mapper.Map<OneClappLatestTheme>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeRepository.GetMany(t => t.Id == oneClappLatestThemeObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddOneClappLatestTheme(oneClappLatestThemeObj);
            }
            else
            {
                return await UpdateOneClappLatestTheme(oneClappLatestThemeObj, existingItem.Id);
            }
        }
        public async Task<OneClappLatestTheme> UpdateOneClappLatestTheme(OneClappLatestTheme updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappLatestThemeRepository.UpdateAsync(updatedItem, existingId);
           await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<OneClappLatestTheme> AddOneClappLatestTheme(OneClappLatestTheme oneClappLatestThemeObj)
        {
            oneClappLatestThemeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappLatestThemeRepository.AddAsync(oneClappLatestThemeObj);
           await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<OneClappLatestTheme> GetAll()
        {
            return _unitOfWork.OneClappLatestThemeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappLatestTheme GetByName(string Name)
        {
            return _unitOfWork.OneClappLatestThemeRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }
        public OneClappLatestTheme GetOneClappLatestThemeById(long Id)
        {
            return _unitOfWork.OneClappLatestThemeRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappLatestTheme DeleteOneClappLatestTheme(OneClappLatestThemeDto model)
        {
            var oneClappLatestThemeObj = _mapper.Map<OneClappLatestTheme>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeRepository.GetMany(t => t.Id == oneClappLatestThemeObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappLatestThemeRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }
    public partial interface IOneClappLatestThemeService : IService<OneClappLatestTheme>
    {
        OneClappLatestTheme GetByName(string Name);
        Task<OneClappLatestTheme> CheckInsertOrUpdate(OneClappLatestThemeDto model);
        List<OneClappLatestTheme> GetAll();
        OneClappLatestTheme DeleteOneClappLatestTheme(OneClappLatestThemeDto model);
        OneClappLatestTheme GetOneClappLatestThemeById(long Id);
    }
}