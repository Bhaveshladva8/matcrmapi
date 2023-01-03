using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class SubTaskCommentService : Service<SubTaskComment>, ISubTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public SubTaskComment CheckInsertOrUpdate(SubTaskCommentDto model)
        {
            var subTaskCommentObj = _mapper.Map<SubTaskComment>(model);
            var existingItem = _unitOfWork.SubTaskCommentRepository.GetMany(t => t.Id == subTaskCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertSubTaskComment(subTaskCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return UpdateSubTaskComment(existingItem, existingItem.Id);
            }
        }

        public SubTaskComment InsertSubTaskComment(SubTaskComment subTaskCommentObj)
        {
            subTaskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.SubTaskCommentRepository.Add(subTaskCommentObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public SubTaskComment UpdateSubTaskComment(SubTaskComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.SubTaskCommentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<SubTaskComment> GetAllBySubTaskId(long subTaskId)
        {
            return _unitOfWork.SubTaskCommentRepository.GetMany(t => t.SubTaskId == subTaskId && t.IsDeleted == false).Result.ToList();
        }

        public SubTaskComment GetSubTaskCommentById(long Id)
        {
            return _unitOfWork.SubTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public SubTaskComment DeleteSubTaskComment(long Id)
        {
            var subTaskCommentObj = _unitOfWork.SubTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (subTaskCommentObj != null)
            {
                subTaskCommentObj.IsDeleted = true;
                subTaskCommentObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.SubTaskCommentRepository.UpdateAsync(subTaskCommentObj, subTaskCommentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return subTaskCommentObj;
        }

        public List<SubTaskComment> DeleteCommentBySubTaskId(long SubTaskId)
        {
            var subTaskCommentList = _unitOfWork.SubTaskCommentRepository.GetMany(t => t.SubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (subTaskCommentList != null && subTaskCommentList.Count() > 0)
            {
                foreach (var item in subTaskCommentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.SubTaskCommentRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return subTaskCommentList;
        }
    }

    public partial interface ISubTaskCommentService : IService<SubTaskComment>
    {
        SubTaskComment CheckInsertOrUpdate(SubTaskCommentDto model);
        List<SubTaskComment> GetAllBySubTaskId(long subTaskId);
        SubTaskComment GetSubTaskCommentById(long Id);
        SubTaskComment DeleteSubTaskComment(long Id);
        List<SubTaskComment> DeleteCommentBySubTaskId(long SubTaskId);
    }
}