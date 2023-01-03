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
    public partial class OneClappFormLayoutBackgroundService : Service<OneClappFormLayoutBackground>, IOneClappFormLayoutBackgroundService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappFormLayoutBackgroundService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormLayoutBackground> CheckInsertOrUpdate(OneClappFormLayoutBackgroundDto model)
        {
            var oneClappFormLayoutBackgroundObj = _mapper.Map<OneClappFormLayoutBackground>(model);
            var existingItem = _unitOfWork.OneClappFormLayoutBackgroundRepository.GetMany(t => t.Id == oneClappFormLayoutBackgroundObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappFormLayoutBackground(oneClappFormLayoutBackgroundObj);
            }
            else
            {
                oneClappFormLayoutBackgroundObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormLayoutBackgroundObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormLayoutBackgroundObj.Id = existingItem.Id;
                return await UpdateOneClappFormLayoutBackground(oneClappFormLayoutBackgroundObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormLayoutBackground> UpdateOneClappFormLayoutBackground(OneClappFormLayoutBackground updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappFormLayoutBackgroundRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OneClappFormLayoutBackground> InsertOneClappFormLayoutBackground(OneClappFormLayoutBackground oneClappFormLayoutBackgroundObj)
        {
            oneClappFormLayoutBackgroundObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappFormLayoutBackgroundRepository.AddAsync(oneClappFormLayoutBackgroundObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappFormLayoutBackground> GetAll()
        {
            return _unitOfWork.OneClappFormLayoutBackgroundRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public OneClappFormLayoutBackground GetById(long Id)
        {
            return _unitOfWork.OneClappFormLayoutBackgroundRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public OneClappFormLayoutBackground DeleteOneClappFormLayoutBackground(long Id)
        {
            var oneClappFormLayoutBackgroundObj = _unitOfWork.OneClappFormLayoutBackgroundRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (oneClappFormLayoutBackgroundObj != null)
            {
                oneClappFormLayoutBackgroundObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormLayoutBackgroundRepository.UpdateAsync(oneClappFormLayoutBackgroundObj, oneClappFormLayoutBackgroundObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IOneClappFormLayoutBackgroundService : IService<OneClappFormLayoutBackground>
    {
        Task<OneClappFormLayoutBackground> CheckInsertOrUpdate(OneClappFormLayoutBackgroundDto model);
        List<OneClappFormLayoutBackground> GetAll();
        OneClappFormLayoutBackground DeleteOneClappFormLayoutBackground(long Id);
        OneClappFormLayoutBackground GetById(long Id);
    }
}