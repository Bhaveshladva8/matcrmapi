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
    public partial class CheckListUserService : Service<CheckListUser>, ICheckListUserService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CheckListUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CheckListUser> CheckInsertOrUpdate(CheckListUserDto model)
        {
            var checkListUserObj = _mapper.Map<CheckListUser>(model);
            var existingItem = _unitOfWork.CheckListUserRepository.GetMany(t => t.CheckListId == checkListUserObj.CheckListId && t.UserId == checkListUserObj.UserId).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCheckListUser(checkListUserObj);
            }
            else
            {
                existingItem.IsChecked = checkListUserObj.IsChecked;
                return await UpdateCheckListUser(existingItem, existingItem.Id);
            }
        }

        public async Task<CheckListUser> InsertCheckListUser(CheckListUser checkListUserObj)
        {
            checkListUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CheckListUserRepository.AddAsync(checkListUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CheckListUser> UpdateCheckListUser(CheckListUser existingItem, long existingId)
        {
           await _unitOfWork.CheckListUserRepository.UpdateAsync(existingItem, existingId);
           await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CheckListUser> GetByTenant(int tenantId)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.TenantId == tenantId).Result.ToList();
        }


        public List<CheckListUser> GetByUserAndTenant(int userId, int tenantId)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.UserId == userId && t.TenantId == tenantId).Result.ToList();
        }

        public List<CheckListUser> GetByUser(int userId)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.UserId == userId).Result.ToList();
        }

         public CheckListUser GetByUserAndCheckListId(int userId, long checkListId)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.UserId == userId && t.CheckListId == checkListId).Result.FirstOrDefault();
        }

        public CheckListUser GetById(long Id)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public List<CheckListUser> GetCheckListId(long CheckListId)
        {
            return _unitOfWork.CheckListUserRepository.GetMany(t => t.CheckListId == CheckListId).Result.ToList();
        }

        public CheckListUser DeleteCheckListUser(long Id)
        {
            var checkListUserObj = _unitOfWork.CheckListUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if(checkListUserObj != null)
            {
                _unitOfWork.CheckListUserRepository.DeleteAsync(checkListUserObj);
                _unitOfWork.CommitAsync();
            }

            return checkListUserObj;
        }

        public List<CheckListUser> DeleteByCheckListId(long CheckListId)
        {
            var checkListUsersList = _unitOfWork.CheckListUserRepository.GetMany(t => t.CheckListId == CheckListId && t.IsDeleted == false).Result.ToList();
            if (checkListUsersList != null && checkListUsersList.Count() > 0)
            {
                foreach (var existingItem in checkListUsersList)
                {
                    _unitOfWork.CheckListUserRepository.DeleteAsync(existingItem);
                }

                _unitOfWork.CommitAsync();
            }

            return checkListUsersList;
        }
    }

    public partial interface ICheckListUserService : IService<CheckListUser>
    {
        Task<CheckListUser> CheckInsertOrUpdate(CheckListUserDto model);
        List<CheckListUser> GetByTenant(int tenantId);
        List<CheckListUser> GetByUserAndTenant(int userId, int tenantId);
        CheckListUser GetById(long Id);
        List<CheckListUser> GetByUser(int userId);
        List<CheckListUser> GetCheckListId(long CheckListId);
        CheckListUser DeleteCheckListUser(long Id);
        List<CheckListUser> DeleteByCheckListId(long CheckListId);
        CheckListUser GetByUserAndCheckListId(int userId, long checkListId);
    }
}