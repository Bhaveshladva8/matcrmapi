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
    public partial class CustomTenantFieldService : Service<CustomTenantField>, ICustomTenantFieldService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomTenantFieldService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomTenantField> CheckInsertOrUpdate(CustomTenantFieldDto model)
        {
            var customTenantFieldObj = _mapper.Map<CustomTenantField>(model);
            var existingItem = _unitOfWork.CustomTenantFieldRepository.GetMany(t => t.FieldId == customTenantFieldObj.FieldId && t.TenantId == customTenantFieldObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCustomTenantField(customTenantFieldObj);
            }
            else
            {
                existingItem.FieldId = customTenantFieldObj.FieldId;
                existingItem.TenantId = customTenantFieldObj.TenantId;
                return await UpdateCustomTenantField(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomTenantField> InsertCustomTenantField(CustomTenantField customTenantFieldObj)
        {
            customTenantFieldObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomTenantFieldRepository.AddAsync(customTenantFieldObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomTenantField> UpdateCustomTenantField(CustomTenantField existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
           await _unitOfWork.CustomTenantFieldRepository.UpdateAsync(existingItem, existingId);
           await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public CustomTenantField GetCustomTenantField(long FieldId, long TenantId)
        {
            return _unitOfWork.CustomTenantFieldRepository.GetMany(t => t.FieldId == FieldId && t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<CustomTenantField> DeleteTenantField(long FieldId, long TenantId)
        {
            var customTenantFieldObj = _unitOfWork.CustomTenantFieldRepository.GetMany(t => t.FieldId == FieldId && t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (customTenantFieldObj != null)
            {
                customTenantFieldObj.IsDeleted = true;
                customTenantFieldObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.CustomTenantFieldRepository.UpdateAsync(customTenantFieldObj, customTenantFieldObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return deletedItem;
            }
            else
                return null;
        }
    }

    public partial interface ICustomTenantFieldService : IService<CustomTenantField>
    {
        Task<CustomTenantField> CheckInsertOrUpdate(CustomTenantFieldDto model);
        CustomTenantField GetCustomTenantField(long FieldId, long TenantId);
        Task<CustomTenantField> DeleteTenantField(long FieldId, long TenantId);
    }
}