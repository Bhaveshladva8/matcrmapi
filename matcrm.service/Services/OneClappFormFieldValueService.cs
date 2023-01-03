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
    public partial class OneClappFormFieldValueService : Service<OneClappFormFieldValue>, IOneClappFormFieldValueService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappFormFieldValueService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormFieldValue> CheckInsertOrUpdate(OneClappFormFieldValueDto model)
        {
            var oneClappFormFieldValueObj = _mapper.Map<OneClappFormFieldValue>(model);
            OneClappFormFieldValue? existingItem = null;
            // if (model.ControlType == "Checkbox")
            // {
            //     existingItem = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.FieldId == obj.FieldId && t.ModuleId == obj.ModuleId && t.RecordId == obj.RecordId && t.TenantId == obj.TenantId && t.IsDeleted == false && t.OptionId == obj.OptionId).Result.FirstOrDefault();
            // }
            // else
            // {
            //     existingItem = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.FieldId == obj.FieldId && t.ModuleId == obj.ModuleId && t.RecordId == obj.RecordId && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            // }
            if (model.ControlType == "Checkbox")
            {

                existingItem = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormFieldId == oneClappFormFieldValueObj.OneClappFormFieldId && t.OneClappRequestFormId == oneClappFormFieldValueObj.OneClappRequestFormId && t.OneClappFormId == oneClappFormFieldValueObj.OneClappFormId && t.OptionId == oneClappFormFieldValueObj.OptionId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormFieldId == oneClappFormFieldValueObj.OneClappFormFieldId && t.OneClappRequestFormId == oneClappFormFieldValueObj.OneClappRequestFormId && t.OneClappFormId == oneClappFormFieldValueObj.OneClappFormId && t.IsDeleted == false).Result.FirstOrDefault();
            }


            if (existingItem == null)
            {
                return await InsertOneClappFormFieldValue(oneClappFormFieldValueObj);
                // var AddUpdate =  InsertOneClappFormFieldValue(obj);
                //  return AddUpdate.Result;
            }
            else
            {
                existingItem.Value = oneClappFormFieldValueObj.Value;
                existingItem.OptionId = oneClappFormFieldValueObj.OptionId;
                return await UpdateOneClappFormFieldValue(existingItem, existingItem.Id);
                // var AddUpdate = UpdateOneClappFormFieldValue(existingItem, existingItem.Id);
                // return AddUpdate.Result;
            }
        }

        public async Task<OneClappFormFieldValue> InsertOneClappFormFieldValue(OneClappFormFieldValue oneClappFormFieldValueObj)
        {
            oneClappFormFieldValueObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappFormFieldValueRepository.Add(oneClappFormFieldValueObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OneClappFormFieldValue> UpdateOneClappFormFieldValue(OneClappFormFieldValue existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.OneClappFormFieldValueRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OneClappFormFieldValue> GetAllValues(long FormFieldId, long FormId)
        {
            return _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormFieldId == FormFieldId && t.OneClappFormId == FormId && t.IsDeleted == false).Result.ToList();
        }

        public OneClappFormFieldValue GetById(long Id)
        {
            return _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<OneClappFormFieldValue> GetByRequestId(long RequestId)
        {
            return _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappRequestFormId == RequestId && t.IsDeleted == false).Result.Include(t => t.CustomControlOption).ToList();
        }

        public List<OneClappFormFieldValue> GetByRequestAndCustomField(long RequestId, long customFieldId)
        {
            return _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappRequestFormId == RequestId && t.CustomFieldId == customFieldId && t.IsDeleted == false).Result.Include(t => t.CustomControlOption).ToList();
        }

        public List<OneClappFormFieldValue> DeleteList(OneClappFormFieldValueDto model)
        {
            var oneClappFormFieldValueList = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormId == model.OneClappFormId.Value && t.OneClappFormFieldId == model.OneClappFormFieldId.Value && t.IsDeleted == false).Result.ToList();
            if (oneClappFormFieldValueList != null && oneClappFormFieldValueList.Count() > 0)
            {
                foreach (var existingItem in oneClappFormFieldValueList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappFormFieldValueRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                    _unitOfWork.CommitAsync();

                }
            }
            return oneClappFormFieldValueList;
        }

        public List<OneClappFormFieldValue> DeleteByForm(long FormId)
        {
            var oneClappFormFieldValueList = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormId == FormId && t.IsDeleted == false).Result.ToList();
            if (oneClappFormFieldValueList != null && oneClappFormFieldValueList.Count() > 0)
            {
                foreach (var existingItem in oneClappFormFieldValueList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappFormFieldValueRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                    _unitOfWork.CommitAsync();

                }
            }
            return oneClappFormFieldValueList;
        }

        public List<OneClappFormFieldValue> GetAllByRequestIds(List<long> requestIds)
        {

            var propImageList = GetAllAsync().Result.Join(requestIds, sc => sc.OneClappRequestFormId, pli => pli, (sc, pli) => sc).ToList();

            return propImageList;
        }

        public List<OneClappFormFieldValue> DeleteFieldValueList(long FieldId, long TenantId)
        {
            var oneClappFormFieldValueList = _unitOfWork.OneClappFormFieldValueRepository.GetMany(t => t.OneClappFormFieldId == FieldId && t.TenantId == TenantId && t.IsDeleted == false).Result.ToList();
            if (oneClappFormFieldValueList != null && oneClappFormFieldValueList.Count() > 0)
            {
                foreach (var existingItem in oneClappFormFieldValueList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.OneClappFormFieldValueRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                    _unitOfWork.CommitAsync();

                }
            }
            return oneClappFormFieldValueList;
        }
    }

    public partial interface IOneClappFormFieldValueService : IService<OneClappFormFieldValue>
    {
        Task<OneClappFormFieldValue> CheckInsertOrUpdate(OneClappFormFieldValueDto model);
        List<OneClappFormFieldValue> GetAllValues(long FormFieldId, long FormId);
        OneClappFormFieldValue GetById(long Id);
        List<OneClappFormFieldValue> DeleteList(OneClappFormFieldValueDto model);
        List<OneClappFormFieldValue> DeleteFieldValueList(long FieldId, long TenantId);
        List<OneClappFormFieldValue> GetByRequestId(long RequestId);
        List<OneClappFormFieldValue> GetByRequestAndCustomField(long RequestId, long customFieldId);
        List<OneClappFormFieldValue> GetAllByRequestIds(List<long> requestIds);
        List<OneClappFormFieldValue> DeleteByForm(long FormId);
    }
}