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
    public partial class OneClappFormLayoutService : Service<OneClappFormLayout>, IOneClappFormLayoutService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappFormLayoutService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormLayout> CheckInsertOrUpdate(OneClappFormLayoutDto model)
        {
            var oneClappFormLayoutObj = _mapper.Map<OneClappFormLayout>(model);
            var existingItem = _unitOfWork.OneClappFormLayoutRepository.GetMany(t => t.Id == oneClappFormLayoutObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappFormLayout(oneClappFormLayoutObj);
            }
            else
            {
                oneClappFormLayoutObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormLayoutObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormLayoutObj.Id = existingItem.Id;
                return await UpdateOneClappFormLayout(oneClappFormLayoutObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormLayout> UpdateOneClappFormLayout(OneClappFormLayout updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappFormLayoutRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OneClappFormLayout> InsertOneClappFormLayout(OneClappFormLayout oneClappFormLayoutObj)
        {
            oneClappFormLayoutObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappFormLayoutRepository.AddAsync(oneClappFormLayoutObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappFormLayout> GetAll()
        {
            return _unitOfWork.OneClappFormLayoutRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public OneClappFormLayout GetById(long Id)
        {
            return _unitOfWork.OneClappFormLayoutRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public OneClappFormLayout DeleteOneClappFormLayout(long Id)
        {
            var oneClappFormLayoutObj = _unitOfWork.OneClappFormLayoutRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (oneClappFormLayoutObj != null)
            {
                oneClappFormLayoutObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormLayoutRepository.UpdateAsync(oneClappFormLayoutObj, oneClappFormLayoutObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IOneClappFormLayoutService : IService<OneClappFormLayout>
    {
        Task<OneClappFormLayout> CheckInsertOrUpdate(OneClappFormLayoutDto model);
        List<OneClappFormLayout> GetAll();
        OneClappFormLayout DeleteOneClappFormLayout(long Id);
        OneClappFormLayout GetById(long Id);
    }
}