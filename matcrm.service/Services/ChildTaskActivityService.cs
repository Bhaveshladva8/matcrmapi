using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class ChildTaskActivityService : Service<ChildTaskActivity>, IChildTaskActivityService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ChildTaskActivityService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public ChildTaskActivity CheckInsertOrUpdate (ChildTaskActivity childTaskActivityObj) {
            var existingItem = _unitOfWork.ChildTaskActivityRepository.GetMany (t => t.Id == childTaskActivityObj.Id).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertChildTaskActivity (childTaskActivityObj);
            } else {
                return existingItem;
                // return UpdateChildTaskActivity (existingItem, existingItem.Id);
            }
        }

        public ChildTaskActivity InsertChildTaskActivity (ChildTaskActivity childTaskActivityObj) {
            childTaskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ChildTaskActivityRepository.Add (childTaskActivityObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }
        public ChildTaskActivity UpdateChildTaskActivity (ChildTaskActivity existingItem, int existingId) {
            _unitOfWork.ChildTaskActivityRepository.UpdateAsync (existingItem, existingId);
            _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<ChildTaskActivity> GetAllByChildTaskId (long childTaskId) {
            return _unitOfWork.ChildTaskActivityRepository.GetMany (t => t.ChildTaskId == childTaskId).Result.ToList ();
        }

        public ChildTaskActivity GetChildTaskActivitytById (long Id) {
            return _unitOfWork.ChildTaskActivityRepository.GetMany (t => t.Id == Id).Result.FirstOrDefault ();
        }

        public ChildTaskActivity DeleteChildTaskActivityById (long Id) {
            var childTaskActivityObj = _unitOfWork.ChildTaskActivityRepository.GetMany (t => t.Id == Id).Result.FirstOrDefault ();
            if(childTaskActivityObj != null)
            {
                _unitOfWork.ChildTaskActivityRepository.DeleteAsync (childTaskActivityObj);
                _unitOfWork.CommitAsync ();
            }
            return childTaskActivityObj;
        }

        public List<ChildTaskActivity> DeleteChildTaskActivityByChildTaskId (long childTaskId) {
            var childTaskActivitiesList = _unitOfWork.ChildTaskActivityRepository.GetMany (t => t.ChildTaskId == childTaskId).Result.ToList ();
            if(childTaskActivitiesList != null && childTaskActivitiesList.Count > 0)
            {
                foreach (var item in childTaskActivitiesList) {
                    _unitOfWork.ChildTaskActivityRepository.DeleteAsync (item);
                }
                _unitOfWork.CommitAsync ();
            }

            return childTaskActivitiesList;
        }

    }

    public partial interface IChildTaskActivityService : IService<ChildTaskActivity> {
        ChildTaskActivity CheckInsertOrUpdate (ChildTaskActivity model);
        List<ChildTaskActivity> GetAllByChildTaskId (long childTaskId);
        ChildTaskActivity GetChildTaskActivitytById (long Id);
        ChildTaskActivity DeleteChildTaskActivityById (long Id);
        List<ChildTaskActivity> DeleteChildTaskActivityByChildTaskId (long childTaskId);
    }
}