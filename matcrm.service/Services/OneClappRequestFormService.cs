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
    public partial class OneClappRequestFormService : Service<OneClappRequestForm>, IOneClappRequestFormService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappRequestFormService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappRequestForm> CheckInsertOrUpdate(OneClappRequestFormDto model)
        {
            var oneClappRequestFormObj = _mapper.Map<OneClappRequestForm>(model);
            var existingItem = _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.Id == oneClappRequestFormObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappRequestForm(oneClappRequestFormObj);
            }
            else
            {
                return await UpdateOneClappRequestForm(oneClappRequestFormObj, existingItem.Id);
            }
        }

        public async Task<OneClappRequestForm> UpdateOneClappRequestForm(OneClappRequestForm updatedItem, long existingId)
        {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappRequestFormRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OneClappRequestForm> InsertOneClappRequestForm(OneClappRequestForm oneClappRequestFormObj)
        {
            oneClappRequestFormObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappRequestFormRepository.AddAsync(oneClappRequestFormObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappRequestForm> GetAll()
        {
            return _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.IsVerify == false && t.IsDeleted == false).Result.ToList();
        }

        public OneClappRequestForm GetById(long Id)
        {
            return _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<OneClappRequestForm> GetByFormId(long OneClappFormId)
        {
            return _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.OneClappFormId == OneClappFormId && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappRequestForm> GetByUser(int userId)
        {
            return _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public OneClappRequestForm DeleteById(long Id)
        {

            var oneClappRequestFormObj = _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappRequestFormObj != null)
            {
                oneClappRequestFormObj.IsDeleted = true;
                oneClappRequestFormObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappRequestFormRepository.UpdateAsync(oneClappRequestFormObj, oneClappRequestFormObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<OneClappRequestForm> DeleteByFormId(long OneClappFormId)
        {

            var oneClappRequestFormList = _unitOfWork.OneClappRequestFormRepository.GetMany(t => t.OneClappFormId == OneClappFormId && t.IsDeleted == false).Result.ToList();
            if (oneClappRequestFormList != null && oneClappRequestFormList.Count() > 0)
            {
                foreach (var existingItem in oneClappRequestFormList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappRequestFormRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return oneClappRequestFormList;
        }
    }

    public partial interface IOneClappRequestFormService : IService<OneClappRequestForm>
    {
        Task<OneClappRequestForm> CheckInsertOrUpdate(OneClappRequestFormDto model);
        List<OneClappRequestForm> GetAll();
        OneClappRequestForm DeleteById(long Id);
        List<OneClappRequestForm> GetByFormId(long OneClappFormId);
        OneClappRequestForm GetById(long Id);
        List<OneClappRequestForm> GetByUser(int userId);
        List<OneClappRequestForm> DeleteByFormId(long OneClappFormId);
    }
}