using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class UserERPSystemService : Service<UserERPSystem>, IUserERPSystemService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public UserERPSystemService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserERPSystem> CheckInsertOrUpdate (UserERPSystemDto model) {
            var userERPSystemObj = _mapper.Map<UserERPSystem> (model);
            var existingItem = _unitOfWork.UserERPSystemRepository.GetMany (t => t.Id == userERPSystemObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertUserERPSystem (userERPSystemObj);
            } else {
                existingItem.Tenant = userERPSystemObj.Tenant;
                existingItem.AuthKey = userERPSystemObj.AuthKey;
                existingItem.Email = userERPSystemObj.Email;
                existingItem.ERPId = userERPSystemObj.ERPId;
                existingItem.IsActive = userERPSystemObj.IsActive;

                return await UpdateUserERPSystem (existingItem, existingItem.Id);
            }
        }

        public async Task<UserERPSystem> UpdateUserERPSystem (UserERPSystem updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.UserERPSystemRepository.UpdateAsync (updatedItem, existingId);
           await _unitOfWork.CommitAsync ();
            return update;
        }

        public async Task<UserERPSystem> InsertUserERPSystem (UserERPSystem userERPSystemObj) {
            userERPSystemObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.UserERPSystemRepository.AddAsync (userERPSystemObj);
           await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<UserERPSystem> GetAllByAdmin () {
            return _unitOfWork.UserERPSystemRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public List<UserERPSystem> GetAllByTenantAdmin (long userId) {
            return _unitOfWork.UserERPSystemRepository.GetMany (t => t.UserId == userId && t.IsDeleted == false ).Result.ToList ();
        }

        public UserERPSystem GetUserERPSystemById (long Id) {
            return _unitOfWork.UserERPSystemRepository.GetMany (t =>t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public UserERPSystem ValidateUserERPSystem (UserERPSystemDto model)
        {
                 var obj = _mapper.Map<UserERPSystem> (model);
                var existingItem = _unitOfWork.UserERPSystemRepository.GetMany (t => t.AuthKey == obj.AuthKey && t.Email == obj.Email && t.Id != obj.Id && t.IsDeleted == false).Result.FirstOrDefault ();

                return existingItem;
        }
        public UserERPSystem DeleteUserERPSystem (long Id) {
            var userERPSystemObj = _unitOfWork.UserERPSystemRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (userERPSystemObj != null) {
                userERPSystemObj.IsDeleted = true;
                userERPSystemObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.UserERPSystemRepository.UpdateAsync (userERPSystemObj, userERPSystemObj.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }
    }

    public partial interface IUserERPSystemService : IService<UserERPSystem> {
        Task<UserERPSystem> CheckInsertOrUpdate (UserERPSystemDto model);
        UserERPSystem ValidateUserERPSystem (UserERPSystemDto model);
        List<UserERPSystem> GetAllByAdmin ();
        List<UserERPSystem> GetAllByTenantAdmin (long userId);
        UserERPSystem GetUserERPSystemById (long userId);
        UserERPSystem DeleteUserERPSystem (long Id);
    }
}