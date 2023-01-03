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
    public partial class CheckListAssignUserService : Service<CheckListAssignUser>, ICheckListAssignUserService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CheckListAssignUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CheckListAssignUser> CheckInsertOrUpdate(CheckListAssignUserDto model)
        {
            var checkListAssignUserObj = _mapper.Map<CheckListAssignUser>(model);
            var existingItem = _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.CheckListId == checkListAssignUserObj.CheckListId && t.AssignUserId == checkListAssignUserObj.AssignUserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCheckListAssignUser(checkListAssignUserObj);
            }
            else
            {
                return await UpdateCheckListAssignUser(existingItem, existingItem.Id);
            }
        }

        public async Task<CheckListAssignUser> InsertCheckListAssignUser(CheckListAssignUser checkListAssignUserObj)
        {
            checkListAssignUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CheckListAssignUserRepository.AddAsync(checkListAssignUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CheckListAssignUser> UpdateCheckListAssignUser(CheckListAssignUser existingItem, long existingId)
        {
            await _unitOfWork.CheckListAssignUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CheckListAssignUser> GetByTenant(int tenantId)
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<CheckListAssignUser> GetAll()
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }


        public List<CheckListAssignUser> GetByUserAndTenant(int userId, int tenantId)
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.AssignUserId == userId && t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<CheckListAssignUser> GetByUser(int userId)
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.AssignUserId == userId && t.IsDeleted == false).Result.ToList();
        }

        public CheckListAssignUser GetById(long Id)
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<CheckListAssignUser> GetCheckListId(long CheckListId)
        {
            return _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.CheckListId == CheckListId && t.IsDeleted == false).Result.ToList();
        }

        public CheckListAssignUser DeleteCheckListAssignUser(long Id)
        {
            var checkListAssignUserObj = _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (checkListAssignUserObj != null)
            {
                _unitOfWork.CheckListAssignUserRepository.DeleteAsync(checkListAssignUserObj);
                _unitOfWork.CommitAsync();
            }

            return checkListAssignUserObj;
        }

        public List<CheckListAssignUser> DeleteByCheckListId(long CheckListId)
        {
            var checkListAssignUsersList = _unitOfWork.CheckListAssignUserRepository.GetMany(t => t.CheckListId == CheckListId && t.IsDeleted == false).Result.ToList();
            if (checkListAssignUsersList != null && checkListAssignUsersList.Count() > 0)
            {
                foreach (var existingItem in checkListAssignUsersList)
                {
                    _unitOfWork.CheckListAssignUserRepository.DeleteAsync(existingItem);
                }
                _unitOfWork.CommitAsync();
            }

            return checkListAssignUsersList;
        }
    }

    public partial interface ICheckListAssignUserService : IService<CheckListAssignUser>
    {
        Task<CheckListAssignUser> CheckInsertOrUpdate(CheckListAssignUserDto model);
        List<CheckListAssignUser> GetByTenant(int tenantId);
        List<CheckListAssignUser> GetAll();
        List<CheckListAssignUser> GetByUserAndTenant(int userId, int tenantId);
        CheckListAssignUser GetById(long Id);
        List<CheckListAssignUser> GetByUser(int userId);
        List<CheckListAssignUser> GetCheckListId(long CheckListId);
        CheckListAssignUser DeleteCheckListAssignUser(long Id);
        List<CheckListAssignUser> DeleteByCheckListId(long CheckListId);
    }
}