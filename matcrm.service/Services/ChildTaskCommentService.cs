using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class ChildTaskCommentService : Service<ChildTaskComment>, IChildTaskCommentService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ChildTaskCommentService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public ChildTaskComment CheckInsertOrUpdate (ChildTaskCommentDto model) {
            var childTaskCommentObj = _mapper.Map<ChildTaskComment> (model);
            var existingItem = _unitOfWork.ChildTaskCommentRepository.GetMany (t => t.Id == model.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertChildTaskComment (childTaskCommentObj);
            } else {
                existingItem.Comment = model.Comment;
                return UpdateChildTaskComment (existingItem, existingItem.Id);
            }
        }

        public ChildTaskComment InsertChildTaskComment (ChildTaskComment childTaskCommentObj) {
            childTaskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ChildTaskCommentRepository.Add (childTaskCommentObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }
        public ChildTaskComment UpdateChildTaskComment (ChildTaskComment existingItem, long existingId) {
            existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.ChildTaskCommentRepository.UpdateAsync (existingItem, existingId);
            _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<ChildTaskComment> GetAllByChildTaskId (long chiTaskId) {
            return _unitOfWork.ChildTaskCommentRepository.GetMany (t => t.ChildTaskId == chiTaskId && t.IsDeleted == false).Result.ToList ();
        }

        public ChildTaskComment GetChildTaskCommentById (long Id) {
            return _unitOfWork.ChildTaskCommentRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public ChildTaskComment DeleteChildTaskComment (long Id) {
            var childTaskCommentObj = _unitOfWork.ChildTaskCommentRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if(childTaskCommentObj != null)
            {
                childTaskCommentObj.IsDeleted = true;
                childTaskCommentObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.ChildTaskCommentRepository.UpdateAsync (childTaskCommentObj, childTaskCommentObj.Id);
                _unitOfWork.CommitAsync ();
            }
            return childTaskCommentObj;
        }

        public List<ChildTaskComment> DeleteCommentByChildTaskId (long ChildTaskId) {
            var childTaskCommentsList = _unitOfWork.ChildTaskCommentRepository.GetMany (t => t.ChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList ();
            if(childTaskCommentsList != null && childTaskCommentsList.Count() > 0)
            {
                foreach (var item in childTaskCommentsList) {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.ChildTaskCommentRepository.UpdateAsync (item, item.Id);
                }
                _unitOfWork.CommitAsync ();
            }

            return childTaskCommentsList;
        }
    }

    public partial interface IChildTaskCommentService : IService<ChildTaskComment> {
        ChildTaskComment CheckInsertOrUpdate (ChildTaskCommentDto model);
        List<ChildTaskComment> GetAllByChildTaskId (long childTaskId);
        ChildTaskComment GetChildTaskCommentById (long Id);
        ChildTaskComment DeleteChildTaskComment (long Id);
        List<ChildTaskComment> DeleteCommentByChildTaskId (long ChildTaskId);
    }
}