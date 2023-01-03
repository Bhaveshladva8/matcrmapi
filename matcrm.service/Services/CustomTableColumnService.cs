using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace matcrm.service.Services
{
    public partial class CustomTableColumnService : Service<CustomTableColumn>, ICustomTableColumnService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomTableColumnService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomTableColumn> CheckInsertOrUpdate(CustomTableColumnDto model)
        {
            var customTableColumnObj = _mapper.Map<CustomTableColumn>(model);
            var existingItem = _unitOfWork.CustomTableColumnRepository.GetMany(t => t.CustomFieldId == model.CustomFieldId && t.MasterTableId == customTableColumnObj.MasterTableId && t.TenantId == model.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCustomTableColumn(customTableColumnObj);
            }
            else
            {
                existingItem.Name = customTableColumnObj.Name;
                existingItem.Key = customTableColumnObj.Key;
                return await UpdateCustomTableColumn(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomTableColumn> InsertCustomTableColumn(CustomTableColumn customTableColumnObj)
        {
            customTableColumnObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.CustomTableColumnRepository.Add(customTableColumnObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomTableColumn> UpdateCustomTableColumn(CustomTableColumn existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomTableColumnRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CustomTableColumn> GetAll()
        {
            return _unitOfWork.CustomTableColumnRepository.GetMany(t => t.IsDeleted == false).Result.Include(t => t.CustomField).Include(t => t.CustomTable).Include(t => t.CustomControl).ToList();
        }

        public CustomTableColumn GetById(long Id)
        {
            return _unitOfWork.CustomTableColumnRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.CustomControl).Include(t => t.CustomTable).FirstOrDefault();
        }

        public CustomTableColumn GetByName(string Name)
        {
            return _unitOfWork.CustomTableColumnRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<CustomTableColumn> GetAllByTable(long MasterTableId)
        {
            return _unitOfWork.CustomTableColumnRepository.GetMany(t => t.MasterTableId == MasterTableId && t.IsDeleted == false).Result.ToList();
        }

        public List<CustomTableColumn> GetAllDefaultByTable(long MasterTableId)
        {
            return _unitOfWork.CustomTableColumnRepository.GetMany(t => t.MasterTableId == MasterTableId && t.IsDefault == true && t.IsDeleted == false).Result.Include(t => t.CustomControl).ToList();
        }

        public async Task<List<CustomTableColumn>> DeleteCustomFields(CustomTableColumnDto Model)
        {
            // var customTableColumns = _unitOfWork.CustomTableColumnRepository.GetMany (t => t.MasterTableId == Model.MasterTableId && t.Name == Model.Name && t.ControlId == Model.ControlId && t.IsDeleted == false).Result.ToList ();
            var customTableColumnList = _unitOfWork.CustomTableColumnRepository.GetMany(t => t.MasterTableId == Model.MasterTableId && t.CustomFieldId == Model.CustomFieldId && t.TenantId == Model.TenantId && t.IsDeleted == false).Result.ToList();
            if(customTableColumnList != null && customTableColumnList.Count()>0)
            {
                foreach (var item in customTableColumnList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CustomTableColumnRepository.UpdateAsync(item, item.Id);

                }
                await _unitOfWork.CommitAsync();
            }
            return customTableColumnList;
        }

    }

    public partial interface ICustomTableColumnService : IService<CustomTableColumn>
    {
        Task<CustomTableColumn> CheckInsertOrUpdate(CustomTableColumnDto model);
        List<CustomTableColumn> GetAll();
        CustomTableColumn GetById(long Id);
        CustomTableColumn GetByName(string Name);
        List<CustomTableColumn> GetAllByTable(long MasterTableId);
        List<CustomTableColumn> GetAllDefaultByTable(long MasterTableId);
        Task<List<CustomTableColumn>> DeleteCustomFields(CustomTableColumnDto Model);
    }
}