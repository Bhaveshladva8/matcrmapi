using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappTaskUserSerivce : Service<OneClappTaskUser>, IOneClappTaskUserSerivce
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappTaskUserSerivce(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappTaskUser CheckInsertOrUpdate(OneClappTaskUserDto model)
        {
            var oneClappTaskUserObj = _mapper.Map<OneClappTaskUser>(model);
            var existingItem = _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.OneClappTaskId == oneClappTaskUserObj.OneClappTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertTaskUser(oneClappTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public bool IsExistOrNot(OneClappTaskUserDto model)
        {
            var existingItem = _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.OneClappTaskId == model.OneClappTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public OneClappTaskUser InsertTaskUser(OneClappTaskUser oneClappTaskUserObj)
        {
            oneClappTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappTaskUserRepository.Add(oneClappTaskUserObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappTaskUser> GetAssignUsersByTask(long TaskId)
        {
            return _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.OneClappTaskId == TaskId && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappTaskUser> GetAll()
        {
            return _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappTaskUser> DeleteByTaskId(long TaskId)
        {
            var oneClappTaskUserList = _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.OneClappTaskId == TaskId && t.IsDeleted == false).Result.ToList();
            if (oneClappTaskUserList != null && oneClappTaskUserList.Count() > 0)
            {
                foreach (var item in oneClappTaskUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.OneClappTaskUserRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return oneClappTaskUserList;
        }

        public OneClappTaskUser DeleteAssignedTaskUser(long Id)
        {
            var oneClappTaskUserObj = _unitOfWork.OneClappTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappTaskUserObj != null)
            {
                oneClappTaskUserObj.IsDeleted = true;
                oneClappTaskUserObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.OneClappTaskUserRepository.UpdateAsync(oneClappTaskUserObj, oneClappTaskUserObj.Id);
                _unitOfWork.CommitAsync();
            }
            return oneClappTaskUserObj;
        }
    }

    public partial interface IOneClappTaskUserSerivce : IService<OneClappTaskUser>
    {
        OneClappTaskUser CheckInsertOrUpdate(OneClappTaskUserDto model);
        List<OneClappTaskUser> GetAll();
        List<OneClappTaskUser> GetAssignUsersByTask(long TaskId);
        bool IsExistOrNot(OneClappTaskUserDto model);
        List<OneClappTaskUser> DeleteByTaskId(long TaskId);
        OneClappTaskUser DeleteAssignedTaskUser(long Id);
    }
}