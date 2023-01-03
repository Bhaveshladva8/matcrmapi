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
    public partial class ModuleRecordCustomFieldService : Service<ModuleRecordCustomField>, IModuleRecordCustomFieldService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ModuleRecordCustomFieldService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public ModuleRecordCustomField CheckInsertOrUpdate(ModuleRecordCustomFieldDto model)
        {
            var moduleRecordCustomFieldObj = _mapper.Map<ModuleRecordCustomField>(model);
            // var existingItem = _unitOfWork.ModuleRecordCustomFieldRepository.GetMany (t => t.ControlId == obj.ControlId && t.Name == obj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.ModuleFieldId == moduleRecordCustomFieldObj.ModuleFieldId && t.RecordId == moduleRecordCustomFieldObj.RecordId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertModuleRecordCustomField(moduleRecordCustomFieldObj);
            }
            else
            {
                return UpdateModuleRecordCustomField(existingItem, existingItem.Id);
            }
        }

        public ModuleRecordCustomField InsertModuleRecordCustomField(ModuleRecordCustomField moduleRecordCustomFieldObj)
        {
            moduleRecordCustomFieldObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ModuleRecordCustomFieldRepository.Add(moduleRecordCustomFieldObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public ModuleRecordCustomField UpdateModuleRecordCustomField(ModuleRecordCustomField existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.ModuleRecordCustomFieldRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public ModuleRecordCustomField GetByModule(long ModuleFieldId)
        {
            return _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.ModuleFieldId == ModuleFieldId && t.IsDeleted == false).Result.Include(t => t.ModuleField).Include(t => t.ModuleField.CustomField).FirstOrDefault();
        }

        public ModuleRecordCustomField GetById(long Id)
        {
            return _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.ModuleField).Include(t => t.ModuleField.CustomField).FirstOrDefault();
        }

        public List<ModuleRecordCustomField> GetAll(){
            return _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.IsDeleted == false).Result.Include(t => t.ModuleField).Include(t => t.ModuleField.CustomField).ToList();
        }

        public List<ModuleRecordCustomField> GetByModuleFieldIdList(List<long> ModuleFieldIdList)
        {
            return _unitOfWork.ModuleRecordCustomFieldRepository
                                .GetMany(t => ModuleFieldIdList.Contains(t.ModuleFieldId.Value)).Result.Include(t => t.ModuleField).Include(t => t.ModuleField.CustomField).ToList();
        }


        public async Task<ModuleRecordCustomField> DeleteById(long Id)
        {
            var moduleRecordCustomFieldObj = _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (moduleRecordCustomFieldObj != null)
            {
                moduleRecordCustomFieldObj.IsDeleted = true;
                moduleRecordCustomFieldObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.ModuleRecordCustomFieldRepository.UpdateAsync(moduleRecordCustomFieldObj, moduleRecordCustomFieldObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return deletedItem;
            }
            else
            {
                return null;
            }
        }

        public ModuleRecordCustomField DeleteByModuleFieldAndRecord(long ModuleFieldId, long RecordId)
        {
            var moduleRecordCustomFieldObj = _unitOfWork.ModuleRecordCustomFieldRepository.GetMany(t => t.ModuleFieldId == ModuleFieldId && t.RecordId == RecordId && t.IsDeleted == false).Result.FirstOrDefault();
            if (moduleRecordCustomFieldObj != null)
            {
                moduleRecordCustomFieldObj.IsDeleted = true;
                moduleRecordCustomFieldObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.ModuleRecordCustomFieldRepository.UpdateAsync(moduleRecordCustomFieldObj, moduleRecordCustomFieldObj.Id).Result;
                _unitOfWork.CommitAsync();
                return deletedItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IModuleRecordCustomFieldService : IService<ModuleRecordCustomField>
    {
        ModuleRecordCustomField CheckInsertOrUpdate(ModuleRecordCustomFieldDto model);
        ModuleRecordCustomField GetByModule(long ModuleFieldId);
        ModuleRecordCustomField GetById(long FieldId);
        Task<ModuleRecordCustomField> DeleteById(long FieldId);
        ModuleRecordCustomField DeleteByModuleFieldAndRecord(long ModuleFieldId, long RecordId);
        List<ModuleRecordCustomField> GetByModuleFieldIdList(List<long> ModuleFieldIdList);
        List<ModuleRecordCustomField> GetAll();
    }
}