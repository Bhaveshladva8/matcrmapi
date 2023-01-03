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
    public partial class CheckListService : Service<CheckList>, ICheckListService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CheckListService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CheckList> CheckInsertOrUpdate(CheckListDto model)
        {
            var checkListObj = _mapper.Map<CheckList>(model);
            var existingItem = _unitOfWork.CheckListRepository.GetMany(t => t.Id == checkListObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCheckList(checkListObj);
            }
            else
            {
                existingItem.Name = checkListObj.Name;
                existingItem.Description = checkListObj.Description;
                return await UpdateCheckList(existingItem, existingItem.Id);
            }
        }

        public async Task<CheckList> InsertCheckList(CheckList checkListObj)
        {
            checkListObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CheckListRepository.AddAsync(checkListObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CheckList> UpdateCheckList(CheckList existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CheckListRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CheckList> GetAll()
        {
            return _unitOfWork.CheckListRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<CheckList> GetByTenant(int tenantId)
        {
            return _unitOfWork.CheckListRepository.GetMany(t => (t.TenantId == tenantId || t.TenantId == null) && t.IsDeleted == false).Result.ToList();
        }

        public List<CheckList> GetAllByModule(long ModuleId, int? tenantId)
        {
            if (tenantId == null)
            {
                return _unitOfWork.CheckListRepository.GetMany(t => t.ModuleId == ModuleId && t.IsDeleted == false).Result.ToList();
            }
            else
            {
                return _unitOfWork.CheckListRepository.GetMany(t => t.ModuleId == ModuleId && t.IsDeleted == false && t.TenantId == tenantId).Result.ToList();
            }
        }


        public CheckList GetCheckList(string Name)
        {
            return _unitOfWork.CheckListRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public CheckList GetCheckListById(long Id)
        {
            return _unitOfWork.CheckListRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<CheckList> DeleteCheckList(long Id)
        {
            var CheckListObj = _unitOfWork.CheckListRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();

            if(CheckListObj != null)
            {
                CheckListObj.IsDeleted = true;
                CheckListObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.CheckListRepository.UpdateAsync(CheckListObj, CheckListObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return CheckListObj;
        }
    }

    public partial interface ICheckListService : IService<CheckList>
    {
        Task<CheckList> CheckInsertOrUpdate(CheckListDto model);
        List<CheckList> GetAll();
        List<CheckList> GetByTenant(int tenantId);
        List<CheckList> GetAllByModule(long ModuleId, int? tenantId);
        CheckList GetCheckList(string Name);
        CheckList GetCheckListById(long Id);
        Task<CheckList> DeleteCheckList(long Id);
    }
}