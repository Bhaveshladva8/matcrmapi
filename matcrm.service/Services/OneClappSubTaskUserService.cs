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
    public partial class OneClappSubTaskUserService : Service<OneClappSubTaskUser>, IOneClappSubTaskUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappSubTaskUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappSubTaskUser CheckInsertOrUpdate(OneClappSubTaskUserDto model)
        {
            var oneClappSubTaskUserObj = _mapper.Map<OneClappSubTaskUser>(model);
            var existingItem = _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.OneClappSubTaskId == oneClappSubTaskUserObj.OneClappSubTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertSubTaskUser(oneClappSubTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public OneClappSubTaskUser InsertSubTaskUser(OneClappSubTaskUser oneClappSubTaskUserObj)
        {
            oneClappSubTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappSubTaskUserRepository.Add(oneClappSubTaskUserObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappSubTaskUser> GetAssignUsersBySubTask(long SubTaskId)
        {
            return _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.OneClappSubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappSubTaskUser> GetAll()
        {
            return _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public bool IsExistOrNot(OneClappSubTaskUserDto model)
        {
            var existingItem = _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.OneClappSubTaskId == model.OneClappSubTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<OneClappSubTaskUser> DeleteBySubTaskId(long SubTaskId)
        {
            var oneClappSubTaskUserList = _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.OneClappSubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (oneClappSubTaskUserList != null && oneClappSubTaskUserList.Count() > 0)
            {
                foreach (var item in oneClappSubTaskUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.OneClappSubTaskUserRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return oneClappSubTaskUserList;
        }
        public OneClappSubTaskUser DeleteAssignedSubTaskUser(long Id)
        {
            var oneClappSubTaskUserObj = _unitOfWork.OneClappSubTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappSubTaskUserObj != null)
            {
                oneClappSubTaskUserObj.IsDeleted = true;
                oneClappSubTaskUserObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.OneClappSubTaskUserRepository.UpdateAsync(oneClappSubTaskUserObj, oneClappSubTaskUserObj.Id);
                _unitOfWork.CommitAsync();
            }
            return oneClappSubTaskUserObj;
        }
    }

    public partial interface IOneClappSubTaskUserService : IService<OneClappSubTaskUser>
    {
        OneClappSubTaskUser CheckInsertOrUpdate(OneClappSubTaskUserDto model);
        List<OneClappSubTaskUser> GetAll();
        List<OneClappSubTaskUser> GetAssignUsersBySubTask(long SubTaskId);
        bool IsExistOrNot(OneClappSubTaskUserDto model);
        List<OneClappSubTaskUser> DeleteBySubTaskId(long SubTaskId);
        OneClappSubTaskUser DeleteAssignedSubTaskUser(long Id);
    }
}