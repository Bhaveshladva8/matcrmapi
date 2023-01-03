using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;

namespace matcrm.service.Services
{
    public partial class OneClappFormFieldService : Service<OneClappFormField>, IOneClappFormFieldService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappFormFieldService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormField> CheckInsertOrUpdate(OneClappFormFieldDto model)
        {
            var oneClappFormFieldObj = _mapper.Map<OneClappFormField>(model);
            // var existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany (t => t.OneClappFormFieldNumber == obj.OneClappFormFieldNumber && t.IsDeleted == false).Result.FirstOrDefault ();
            OneClappFormField? existingItem = null;
            if (oneClappFormFieldObj.CustomFieldId == null)
            {
                existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == oneClappFormFieldObj.OneClappFormId && t.CustomTableColumnId == oneClappFormFieldObj.CustomTableColumnId && t.CustomModuleId == oneClappFormFieldObj.CustomModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == oneClappFormFieldObj.OneClappFormId && t.CustomFieldId == oneClappFormFieldObj.CustomFieldId && t.CustomModuleId == oneClappFormFieldObj.CustomModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertOneClappFormField(oneClappFormFieldObj);
            }
            else
            {
                oneClappFormFieldObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormFieldObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormFieldObj.Id = existingItem.Id;
                oneClappFormFieldObj.CustomModuleId = existingItem.CustomModuleId;
                oneClappFormFieldObj.CustomTableColumnId = existingItem.CustomTableColumnId;
                return await UpdateOneClappFormField(oneClappFormFieldObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormField> CheckInsertOrUpdate1(OneClappFormFieldVM model)
        {
            var oneClappFormFieldObj = _mapper.Map<OneClappFormField>(model);
            OneClappFormField? existingItem = null;
            if (oneClappFormFieldObj.Id != null)
            {
                existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.Id == oneClappFormFieldObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else if (oneClappFormFieldObj.CustomFieldId == null)
            {
                existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == oneClappFormFieldObj.OneClappFormId && t.CustomTableColumnId == oneClappFormFieldObj.CustomTableColumnId && t.CustomModuleId == oneClappFormFieldObj.CustomModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == oneClappFormFieldObj.OneClappFormId && t.CustomFieldId == oneClappFormFieldObj.CustomFieldId && t.CustomModuleId == oneClappFormFieldObj.CustomModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            if (existingItem == null)
            {
                return await InsertOneClappFormField(oneClappFormFieldObj);
            }
            else
            {
                oneClappFormFieldObj.CreatedBy = existingItem.CreatedBy;
                oneClappFormFieldObj.CreatedOn = existingItem.CreatedOn;
                oneClappFormFieldObj.CustomFieldId = existingItem.CustomFieldId;
                oneClappFormFieldObj.Id = existingItem.Id;
                oneClappFormFieldObj.CustomModuleId = existingItem.CustomModuleId;
                oneClappFormFieldObj.CustomTableColumnId = existingItem.CustomTableColumnId;
                return await UpdateOneClappFormField(oneClappFormFieldObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormField> InsertOneClappFormField(OneClappFormField oneClappFormFieldObj)
        {
            oneClappFormFieldObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappFormFieldRepository.Add(oneClappFormFieldObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OneClappFormField> UpdateOneClappFormField(OneClappFormField existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.OneClappFormFieldRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OneClappFormField> GetAllOneClappFormField()
        {
            return _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OneClappFormField GetById(long Id)
        {
            return _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.Include(t => t.CustomTableColumn).FirstOrDefault();
        }

        public List<OneClappFormField> GetAllByForm(long FormId)
        {
            return _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == FormId && t.IsDeleted == false).Result.OrderBy(t => t.Priority).Include(t => t.CustomField).Include(t => t.CustomField.CustomControl).Include(t => t.CustomTableColumn).Include(t => t.CustomTableColumn.CustomTable).ToList();
        }

        public List<OneClappFormField> GetByUser(int userId)
        {
            return _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public OneClappFormField DeleteOneClappFormField(long Id)
        {
            var oneClappFormFieldObj = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappFormFieldObj != null)
            {
                oneClappFormFieldObj.IsDeleted = true;
                oneClappFormFieldObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormFieldRepository.UpdateAsync(oneClappFormFieldObj, oneClappFormFieldObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<OneClappFormField> DeleteList(List<OneClappFormField> oneClappFormFieldList)
        {
            if (oneClappFormFieldList != null && oneClappFormFieldList.Count() > 0)
            {
                foreach (var existingItem in oneClappFormFieldList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappFormFieldRepository.UpdateAsync(existingItem, existingItem.Id).Result;

                    // return newItem;
                }
                _unitOfWork.CommitAsync();
            }
            return oneClappFormFieldList;
        }

        public List<OneClappFormField> DeleteByFormId(long FormId)
        {
            var oneClappFormFieldList = _unitOfWork.OneClappFormFieldRepository.GetMany(t => t.OneClappFormId == FormId && t.IsDeleted == false).Result.ToList();
            if (oneClappFormFieldList != null && oneClappFormFieldList.Count() > 0)
            {
                foreach (var existingItem in oneClappFormFieldList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappFormFieldRepository.UpdateAsync(existingItem, existingItem.Id).Result;

                    // return newItem;
                }
                _unitOfWork.CommitAsync();
            }
            return oneClappFormFieldList;
        }
    }

    public partial interface IOneClappFormFieldService : IService<OneClappFormField>
    {
        Task<OneClappFormField> CheckInsertOrUpdate(OneClappFormFieldDto model);
        List<OneClappFormField> GetAllOneClappFormField();
        OneClappFormField DeleteOneClappFormField(long Id);
        List<OneClappFormField> DeleteByFormId(long FormId);
        OneClappFormField GetById(long Id);
        List<OneClappFormField> GetAllByForm(long FormId);
        List<OneClappFormField> GetByUser(int userId);
        List<OneClappFormField> DeleteList(List<OneClappFormField> fieldList);
        Task<OneClappFormField> CheckInsertOrUpdate1(OneClappFormFieldVM model);
    }
}