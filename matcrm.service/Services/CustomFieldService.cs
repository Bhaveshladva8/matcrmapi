using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomFieldService : Service<CustomField>, ICustomFieldService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomFieldService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomField> CheckInsertOrUpdate (CustomFieldDto model) {
            var customFieldObj = _mapper.Map<CustomField> (model);
            // var existingItem = _unitOfWork.CustomFieldRepository.GetMany (t => t.ControlId == obj.ControlId && t.Name == obj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.CustomFieldRepository.GetMany (t => t.Id == customFieldObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomField (customFieldObj);
            } else {
                existingItem.Name = customFieldObj.Name;
                existingItem.Description = customFieldObj.Description;
                existingItem.ControlId = customFieldObj.ControlId;
                existingItem.IsRequired = customFieldObj.IsRequired;
                return await UpdateCustomField (existingItem, existingItem.Id);
            }
        }

        public async Task<CustomField> InsertCustomField (CustomField customFieldObj) {
            customFieldObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomFieldRepository.AddAsync (customFieldObj);
           await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<CustomField> UpdateCustomField (CustomField existingItem, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomFieldRepository.UpdateAsync (existingItem, existingId);
            await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<CustomField> GetAllControlOption (long ControlId) {
            return _unitOfWork.CustomFieldRepository.GetMany (t => t.ControlId == ControlId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomField GetById (long FieldId) {
            return _unitOfWork.CustomFieldRepository.GetMany (t => t.Id == FieldId && t.IsDeleted == false).Result.Include(t => t.CustomControl).FirstOrDefault ();
        }

        public CustomField GetByName (string Name) {
            return _unitOfWork.CustomFieldRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public CustomField DeleteById (long FieldId){
            var customFieldObj = _unitOfWork.CustomFieldRepository.GetMany (t => t.Id == FieldId && t.IsDeleted == false).Result.FirstOrDefault ();
            if(customFieldObj != null){
                customFieldObj.IsDeleted = true;
                customFieldObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.CustomFieldRepository.UpdateAsync(customFieldObj, customFieldObj.Id).Result;
                _unitOfWork.CommitAsync();
                return deletedItem;
            } else {
                return null;
            }
        }
    }

    public partial interface ICustomFieldService : IService<CustomField> {
        Task<CustomField> CheckInsertOrUpdate (CustomFieldDto model);
        List<CustomField> GetAllControlOption (long ControlId);
        CustomField GetById (long FieldId);
        CustomField DeleteById (long FieldId);
        CustomField GetByName (string Name);
    }
}