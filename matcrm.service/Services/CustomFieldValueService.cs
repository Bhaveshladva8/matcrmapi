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
    public partial class CustomFieldValueService : Service<CustomFieldValue>, ICustomFieldValueService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomFieldValueService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomFieldValue> CheckInsertOrUpdate(CustomFieldValueDto model)
        {
            var customFieldValueObj = _mapper.Map<CustomFieldValue>(model);
            CustomFieldValue? existingItem = null;
            if (model.ControlType == "Checkbox")
            {
                existingItem = _unitOfWork.CustomFieldValueRepository.GetMany(t => t.FieldId == customFieldValueObj.FieldId && t.ModuleId == customFieldValueObj.ModuleId && t.RecordId == customFieldValueObj.RecordId && t.TenantId == customFieldValueObj.TenantId && t.IsDeleted == false && t.OptionId == customFieldValueObj.OptionId).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.CustomFieldValueRepository.GetMany(t => t.FieldId == customFieldValueObj.FieldId && t.ModuleId == customFieldValueObj.ModuleId && t.RecordId == customFieldValueObj.RecordId && t.TenantId == customFieldValueObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertCustomFieldValue(customFieldValueObj);
            }
            else
            {
                if (customFieldValueObj != null)
                {
                    existingItem.Value = customFieldValueObj.Value;
                }
                existingItem.OptionId = customFieldValueObj.OptionId;
                existingItem.ModuleId = customFieldValueObj.ModuleId;
                existingItem.FieldId = customFieldValueObj.FieldId;
                return await UpdateCustomFieldValue(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomFieldValue> InsertCustomFieldValue(CustomFieldValue customFieldValueObj)
        {
            customFieldValueObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomFieldValueRepository.AddAsync(customFieldValueObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomFieldValue> UpdateCustomFieldValue(CustomFieldValue existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomFieldValueRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CustomFieldValue> GetAllValues(long FieldId, long TenantId, long ModuleId, long RecordId)
        {
            return _unitOfWork.CustomFieldValueRepository.GetMany(t => t.TenantId == TenantId && t.FieldId == FieldId && t.ModuleId == ModuleId && t.RecordId == RecordId && t.IsDeleted == false).Result.ToList();
        }

        public CustomFieldValue GetById(long Id)
        {
            return _unitOfWork.CustomFieldValueRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<List<CustomFieldValue>> DeleteList(CustomFieldValueDto model)
        {
            var customFieldList = _unitOfWork.CustomFieldValueRepository.GetMany(t => t.RecordId == model.RecordId.Value && model.FieldId != null && t.FieldId == model.FieldId.Value && t.ModuleId == model.ModuleId.Value).Result.ToList();
            if (customFieldList != null && customFieldList.Count() > 0)
            {
                foreach (var existingItem in customFieldList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CustomFieldValueRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return customFieldList;
        }

        public async Task<List<CustomFieldValue>> DeleteFieldValueList(long FieldId, long ModuleId, long TenantId)
        {
            var customFieldValueList = _unitOfWork.CustomFieldValueRepository.GetMany(t => t.FieldId == FieldId && t.ModuleId == ModuleId && t.TenantId == TenantId && t.IsDeleted == false).Result.ToList();
            foreach (var existingItem in customFieldValueList)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.CustomFieldValueRepository.UpdateAsync(existingItem, existingItem.Id);
            }
            await _unitOfWork.CommitAsync();

            return customFieldValueList;
        }
    }

    public partial interface ICustomFieldValueService : IService<CustomFieldValue>
    {
        Task<CustomFieldValue> CheckInsertOrUpdate(CustomFieldValueDto model);
        List<CustomFieldValue> GetAllValues(long FieldId, long TenantId, long ModuleId, long RecordId);
        CustomFieldValue GetById(long FieldId);
        Task<List<CustomFieldValue>> DeleteList(CustomFieldValueDto model);
        Task<List<CustomFieldValue>> DeleteFieldValueList(long FieldId, long ModuleId, long TenantId);
    }
}