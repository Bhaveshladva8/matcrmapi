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
    public partial class ModuleFieldService : Service<ModuleField>, IModuleFieldService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ModuleFieldService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ModuleField> CheckInsertOrUpdate(ModuleFieldDto model)
        {
            var moduleFieldObj = _mapper.Map<ModuleField>(model);
            var existingItem = _unitOfWork.ModuleFieldRepository.GetMany(t => t.ModuleId == moduleFieldObj.ModuleId && t.FieldId == moduleFieldObj.FieldId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertModuleField(moduleFieldObj);
            }
            else
            {
                existingItem.ModuleId = moduleFieldObj.ModuleId;
                existingItem.IsHide = moduleFieldObj.IsHide;
                return await UpdateModuleField(existingItem, existingItem.Id);
            }
        }

        public async Task<ModuleField> InsertModuleField(ModuleField moduleFieldObj)
        {
            moduleFieldObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ModuleFieldRepository.Add(moduleFieldObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ModuleField> UpdateModuleField(ModuleField existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ModuleFieldRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ModuleField> GetAllModuleField(long ModuleId)
        {
            return _unitOfWork.ModuleFieldRepository.GetMany(t => t.ModuleId == ModuleId && t.IsDeleted == false).Result.Include(t => t.CustomField).ToList();
        }

        public List<ModuleField> GetAllByField(long FieldId)
        {
            return _unitOfWork.ModuleFieldRepository.GetMany(t => t.FieldId == FieldId && t.IsDeleted == false).Result.ToList();
        }

        public async Task<ModuleField> Delete(long Id)
        {
            var moduleFieldObj = _unitOfWork.ModuleFieldRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (moduleFieldObj != null)
            {
                moduleFieldObj.IsDeleted = true;
                moduleFieldObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.ModuleFieldRepository.UpdateAsync(moduleFieldObj, moduleFieldObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public ModuleField DeleteByField(long FieldId, long ModuleId)
        {
            var moduleFieldObj = _unitOfWork.ModuleFieldRepository.GetMany(t => t.FieldId == FieldId && t.ModuleId == ModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            if (moduleFieldObj != null)
            {
                moduleFieldObj.IsDeleted = true;
                moduleFieldObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.ModuleFieldRepository.UpdateAsync(moduleFieldObj, moduleFieldObj.Id).Result;
                _unitOfWork.CommitAsync();
                return deletedItem;
            }
            else
                return null;
        }
    }

    public partial interface IModuleFieldService : IService<ModuleField>
    {
        Task<ModuleField> CheckInsertOrUpdate(ModuleFieldDto model);
        List<ModuleField> GetAllModuleField(long ModuleId);
        List<ModuleField> GetAllByField(long FieldId);
        Task<ModuleField> Delete(long Id);
        ModuleField DeleteByField(long FieldId, long ModuleId);
    }
}