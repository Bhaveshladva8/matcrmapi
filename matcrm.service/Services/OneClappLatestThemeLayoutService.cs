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
    public partial class OneClappLatestThemeLayoutService : Service<OneClappLatestThemeLayout>, IOneClappLatestThemeLayoutService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappLatestThemeLayoutService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneClappLatestThemeLayout> CheckInsertOrUpdate(OneClappLatestThemeLayoutDto model)
        {
            var oneClappLatestThemeLayoutObj = _mapper.Map<OneClappLatestThemeLayout>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeLayoutRepository.GetMany(t => t.Id == oneClappLatestThemeLayoutObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddOneClappLatestThemeLayout(oneClappLatestThemeLayoutObj);
            }
            else
            {
                return await UpdateOneClappLatestThemeLayout(oneClappLatestThemeLayoutObj, existingItem.Id);
            }
        }
        public async Task<OneClappLatestThemeLayout> UpdateOneClappLatestThemeLayout(OneClappLatestThemeLayout updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappLatestThemeLayoutRepository.UpdateAsync(updatedItem, existingId);
           await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<OneClappLatestThemeLayout> AddOneClappLatestThemeLayout(OneClappLatestThemeLayout oneClappLatestThemeLayoutObj)
        {
            oneClappLatestThemeLayoutObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappLatestThemeLayoutRepository.AddAsync(oneClappLatestThemeLayoutObj);
           await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<OneClappLatestThemeLayout> GetAll()
        {
            return _unitOfWork.OneClappLatestThemeLayoutRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappLatestThemeLayout GetOneClappLatestThemeLayoutById(long Id)
        {
            return _unitOfWork.OneClappLatestThemeLayoutRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappLatestThemeLayout GetByName(string Name)
        {
            return _unitOfWork.OneClappLatestThemeLayoutRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }
        public OneClappLatestThemeLayout DeleteOneClappLatestThemeLayout(OneClappLatestThemeLayoutDto model)
        {
            var oneClappLatestThemeLayoutObj = _mapper.Map<OneClappLatestThemeLayout>(model);
            var existingItem = _unitOfWork.OneClappLatestThemeLayoutRepository.GetMany(t => t.Id == oneClappLatestThemeLayoutObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappLatestThemeLayoutRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }
    public partial interface IOneClappLatestThemeLayoutService : IService<OneClappLatestThemeLayout>
    {
        Task<OneClappLatestThemeLayout> CheckInsertOrUpdate(OneClappLatestThemeLayoutDto model);
        List<OneClappLatestThemeLayout> GetAll();
        OneClappLatestThemeLayout DeleteOneClappLatestThemeLayout(OneClappLatestThemeLayoutDto model);
        OneClappLatestThemeLayout GetOneClappLatestThemeLayoutById(long Id);
        OneClappLatestThemeLayout GetByName(string Name);
    }
}