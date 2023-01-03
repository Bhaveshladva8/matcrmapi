using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class SubTaskActivityService : Service<SubTaskActivity>, ISubTaskActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubTaskActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public SubTaskActivity CheckInsertOrUpdate(SubTaskActivity subTaskActivityObj)
        {
            // var existingItem = _unitOfWork.SubTaskActivityRepository.GetMany (t => t.UserId == obj.UserId && t.SubTaskId == obj.SubTaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertSubTaskActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateSubTaskActivity (existingItem, existingItem.Id);
            // }
            return InsertSubTaskActivity(subTaskActivityObj);
        }

        public SubTaskActivity InsertSubTaskActivity(SubTaskActivity subTaskActivityObj)
        {
            subTaskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.SubTaskActivityRepository.Add(subTaskActivityObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public SubTaskActivity UpdateSubTaskActivity(SubTaskActivity existingItem, int existingId)
        {
            _unitOfWork.SubTaskActivityRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<SubTaskActivity> GetAllBySubTaskId(long subTaskId)
        {
            return _unitOfWork.SubTaskActivityRepository.GetMany(t => t.SubTaskId == subTaskId).Result.ToList();
        }

        public SubTaskActivity GetSubTaskActivitytById(long Id)
        {
            return _unitOfWork.SubTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public SubTaskActivity DeleteSubTaskActivityById(long Id)
        {
            var subTaskActivityObj = _unitOfWork.SubTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (subTaskActivityObj != null)
            {
                _unitOfWork.SubTaskActivityRepository.DeleteAsync(subTaskActivityObj);
                _unitOfWork.CommitAsync();
            }
            return subTaskActivityObj;
        }

        public List<SubTaskActivity> DeleteSubTaskActivityBySubTaskId(long SubTaskId)
        {
            var subTaskActivityList = _unitOfWork.SubTaskActivityRepository.GetMany(t => t.SubTaskId == SubTaskId).Result.ToList();
            if (subTaskActivityList != null && subTaskActivityList.Count() > 0)
            {
                foreach (var item in subTaskActivityList)
                {
                    _unitOfWork.SubTaskActivityRepository.DeleteAsync(item);
                }

                _unitOfWork.CommitAsync();
            }
            return subTaskActivityList;
        }

    }

    public partial interface ISubTaskActivityService : IService<SubTaskActivity>
    {
        SubTaskActivity CheckInsertOrUpdate(SubTaskActivity model);
        List<SubTaskActivity> GetAllBySubTaskId(long taskId);
        SubTaskActivity GetSubTaskActivitytById(long Id);
        SubTaskActivity DeleteSubTaskActivityById(long Id);
        List<SubTaskActivity> DeleteSubTaskActivityBySubTaskId(long taskId);
    }
}