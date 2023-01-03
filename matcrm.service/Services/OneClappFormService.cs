using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappFormService : Service<OneClappForm>, IOneClappFormService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappFormService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappForm> CheckInsertOrUpdate(OneClappFormDto model)
        {
            var oneClappFormObj = _mapper.Map<OneClappForm>(model);
            // var existingItem = _unitOfWork.OneClappFormRepository.GetMany (t => t.OneClappFormNumber == obj.OneClappFormNumber && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.OneClappFormRepository.GetMany(t => t.Id == oneClappFormObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappForm(oneClappFormObj);
            }
            else
            {
                oneClappFormObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormObj.FormKey = existingItem.FormKey;
                oneClappFormObj.FormGuid = existingItem.FormGuid;

                return await UpdateOneClappForm(oneClappFormObj, existingItem.Id);
            }
        }

        public async Task<OneClappForm> InsertOneClappForm(OneClappForm oneClappFormObj)
        {
            oneClappFormObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappFormRepository.AddAsync(oneClappFormObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OneClappForm> UpdateOneClappForm(OneClappForm existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.OneClappFormRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OneClappForm> GetAllOneClappForm()
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OneClappForm GetById(long Id)
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.Include(t => t.OneClappFormAction).FirstOrDefault();
        }

        public OneClappForm GetByFormKey(string FormKey)
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.IsDeleted == false && t.FormKey == FormKey).Result.Include(t => t.OneClappFormHeader).Include(t => t.OneClappFormLayout).Include(t => t.OneClappFormLayout.OneClappFormLayoutBackground).Include(t => t.OneClappFormAction).FirstOrDefault();
        }

        public List<OneClappForm> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappForm> GetAll()
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappForm> GetByUserAndTenant(int tenantId, int userId)
        {
            return _unitOfWork.OneClappFormRepository.GetMany(t => t.TenantId == tenantId && t.CreatedBy == userId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedBy).ToList();
        }

        public OneClappForm DeleteOneClappForm(long FormId)
        {
            var oneClappFormObj = _unitOfWork.OneClappFormRepository.GetMany(t => t.Id == FormId).Result.FirstOrDefault();
            if (oneClappFormObj != null)
            {
                oneClappFormObj.IsDeleted = true;
                oneClappFormObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormRepository.UpdateAsync(oneClappFormObj, oneClappFormObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IOneClappFormService : IService<OneClappForm>
    {
        Task<OneClappForm> CheckInsertOrUpdate(OneClappFormDto model);
        List<OneClappForm> GetAllOneClappForm();
        OneClappForm DeleteOneClappForm(long FormId);
        OneClappForm GetById(long Id);
        OneClappForm GetByFormKey(string FormKey);
        List<OneClappForm> GetAllByTenant(int tenantId);
        List<OneClappForm> GetAll();
        List<OneClappForm> GetByUserAndTenant(int tenantId, int userId);
    }
}