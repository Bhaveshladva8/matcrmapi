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
    public partial class CustomControlOptionService : Service<CustomControlOption>, ICustomControlOptionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomControlOptionService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomControlOption> CheckInsertOrUpdate(CustomControlOptionDto model)
        {
            var customControlOptionObj = _mapper.Map<CustomControlOption>(model);
            CustomControlOption? existingItem = null;
            if (model.Id != null)
            {
                existingItem = _unitOfWork.CustomControlOptionRepository.GetMany(t => t.Id == customControlOptionObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.CustomControlOptionRepository.GetMany(t => t.Option == customControlOptionObj.Option && t.CustomFieldId == customControlOptionObj.CustomFieldId && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertCustomControlOption(customControlOptionObj);
            }
            else
            {
                existingItem.Option = customControlOptionObj.Option;
                return await UpdateCustomControlOption(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomControlOption> InsertCustomControlOption(CustomControlOption customControlOptionObj)
        {
            customControlOptionObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomControlOptionRepository.AddAsync(customControlOptionObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomControlOption> UpdateCustomControlOption(CustomControlOption existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomControlOptionRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CustomControlOption> GetAllControlOption(long CustomFieldId)
        {
            return _unitOfWork.CustomControlOptionRepository.GetMany(t => t.CustomFieldId == CustomFieldId && t.IsDeleted == false).Result.ToList();
        }

        public CustomControlOption GetById(long Id)
        {
            return _unitOfWork.CustomControlOptionRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<List<CustomControlOption>> DeleteOptions(long FieldId)
        {
            var customControlOptionsList = _unitOfWork.CustomControlOptionRepository.GetMany(t => t.CustomFieldId == FieldId && t.IsDeleted == false).Result.ToList();
            if(customControlOptionsList != null && customControlOptionsList.Count() > 0)
            {
                foreach (var existingItem in customControlOptionsList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = await _unitOfWork.CustomControlOptionRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }            
            return customControlOptionsList;
        }
    }

    public partial interface ICustomControlOptionService : IService<CustomControlOption>
    {
        Task<CustomControlOption> CheckInsertOrUpdate(CustomControlOptionDto model);
        List<CustomControlOption> GetAllControlOption(long CustomFieldId);
        CustomControlOption GetById(long Id);
        Task<List<CustomControlOption>> DeleteOptions(long FieldId);
    }
}