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
    public partial class OneClappFormHeaderService : Service<OneClappFormHeader>, IOneClappFormHeaderService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappFormHeaderService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormHeader> CheckInsertOrUpdate(OneClappFormHeaderDto model)
        {
            var oneClappFormHeaderObj = _mapper.Map<OneClappFormHeader>(model);
            var existingItem = _unitOfWork.OneClappFormHeaderRepository.GetMany(t => t.Id == oneClappFormHeaderObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappFormHeader(oneClappFormHeaderObj);
            }
            else
            {
                oneClappFormHeaderObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormHeaderObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormHeaderObj.Id = existingItem.Id;
                return await UpdateOneClappFormHeader(oneClappFormHeaderObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormHeader> UpdateOneClappFormHeader(OneClappFormHeader updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappFormHeaderRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OneClappFormHeader> InsertOneClappFormHeader(OneClappFormHeader oneClappFormHeaderObj)
        {
            oneClappFormHeaderObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappFormHeaderRepository.AddAsync(oneClappFormHeaderObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappFormHeader> GetAll()
        {
            return _unitOfWork.OneClappFormHeaderRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public OneClappFormHeader GetById(long Id)
        {
            return _unitOfWork.OneClappFormHeaderRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public OneClappFormHeader DeleteOneClappFormHeader(long Id)
        {
            var oneClappFormHeaderObj = _unitOfWork.OneClappFormHeaderRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (oneClappFormHeaderObj != null)
            {
                oneClappFormHeaderObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormHeaderRepository.UpdateAsync(oneClappFormHeaderObj, oneClappFormHeaderObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IOneClappFormHeaderService : IService<OneClappFormHeader>
    {
        Task<OneClappFormHeader> CheckInsertOrUpdate(OneClappFormHeaderDto model);
        List<OneClappFormHeader> GetAll();
        OneClappFormHeader DeleteOneClappFormHeader(long Id);
        OneClappFormHeader GetById(long Id);
    }
}