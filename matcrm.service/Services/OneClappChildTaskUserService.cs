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
    public partial class OneClappChildTaskUserService : Service<OneClappChildTaskUser>, IOneClappChildTaskUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappChildTaskUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public OneClappChildTaskUser CheckInsertOrUpdate(OneClappChildTaskUserDto model)
        {
            var oneClappChildTaskUserObj = _mapper.Map<OneClappChildTaskUser>(model);
            var existingItem = _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.OneClappChildTaskId == oneClappChildTaskUserObj.OneClappChildTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertChildTaskUser(oneClappChildTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public OneClappChildTaskUser InsertChildTaskUser(OneClappChildTaskUser oneClappChildTaskUserObj)
        {
            oneClappChildTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.OneClappChildTaskUserRepository.Add(oneClappChildTaskUserObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OneClappChildTaskUser> GetAssignUsersByChildTask(long ChildTaskId)
        {
            return _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.OneClappChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList();
        }

        public List<OneClappChildTaskUser> GetAll()
        {
            return _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public bool IsExistOrNot(OneClappChildTaskUserDto model)
        {
            var existingItem = _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.OneClappChildTaskId == model.OneClappChildTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<OneClappChildTaskUser>> DeleteByChildTaskId(long ChildTaskId)
        {
            var oneClappChildTaskUserList = _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.OneClappChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList();
            if (oneClappChildTaskUserList != null && oneClappChildTaskUserList.Count() > 0)
            {
                foreach (var item in oneClappChildTaskUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.OneClappChildTaskUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return oneClappChildTaskUserList;
        }
        public async Task<OneClappChildTaskUser> DeleteAssignedChildTaskUser(long Id)
        {
            var oneClappChildTaskUserObj = _unitOfWork.OneClappChildTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappChildTaskUserObj != null)
            {
                oneClappChildTaskUserObj.IsDeleted = true;
                oneClappChildTaskUserObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.OneClappChildTaskUserRepository.UpdateAsync(oneClappChildTaskUserObj, oneClappChildTaskUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return oneClappChildTaskUserObj;
        }
    }

    public partial interface IOneClappChildTaskUserService : IService<OneClappChildTaskUser>
    {
        OneClappChildTaskUser CheckInsertOrUpdate(OneClappChildTaskUserDto model);
        List<OneClappChildTaskUser> GetAll();
        List<OneClappChildTaskUser> GetAssignUsersByChildTask(long TaskId);
        bool IsExistOrNot(OneClappChildTaskUserDto model);
        Task<List<OneClappChildTaskUser>> DeleteByChildTaskId(long ChildTaskId);
        Task<OneClappChildTaskUser> DeleteAssignedChildTaskUser(long Id);
    }
}