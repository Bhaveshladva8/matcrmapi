using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class DiscussionCommentService : Service<DiscussionComment>, IDiscussionCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DiscussionCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<DiscussionComment> CheckInsertOrUpdate(DiscussionCommentDto model, string methodName)
        {
            var discussionCommentObj = _mapper.Map<DiscussionComment>(model);
            var existingItem = _unitOfWork.DiscussionCommentRepository.GetMany(t => t.Id == discussionCommentObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDiscussionComment(discussionCommentObj);
            }
            else
            {
                existingItem.Comment = discussionCommentObj.Comment;
                if (methodName == "PinUnpin")
                {
                    existingItem.IsPinned = discussionCommentObj.IsPinned;
                    existingItem.PinnedBy = discussionCommentObj.PinnedBy;
                }

                return await UpdateDiscussionComment(existingItem, existingItem.Id);
            }
        }

        public async Task<DiscussionComment> InsertDiscussionComment(DiscussionComment discussionCommentObj)
        {
            discussionCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.DiscussionCommentRepository.Add(discussionCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<DiscussionComment> UpdateDiscussionComment(DiscussionComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.DiscussionCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<DiscussionComment> GetAllCommentByDiscussionId(long DiscussionId)
        {
            return _unitOfWork.DiscussionCommentRepository.GetMany(t => t.DiscussionId == DiscussionId && t.DeletedOn == null).Result.Include(t => t.ReplyDiscussionComment).OrderBy(t => t.CreatedOn).ToList();
        }

        public DiscussionComment GetById(long Id)
        {
            return _unitOfWork.DiscussionCommentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.ReplyDiscussionComment).FirstOrDefault();
        }



        public async Task<DiscussionComment> Delete(long Id)
        {
            var discussionCommentObj = _unitOfWork.DiscussionCommentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (discussionCommentObj != null)
            {
                discussionCommentObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.DiscussionCommentRepository.UpdateAsync(discussionCommentObj, discussionCommentObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<DiscussionComment> DeleteByDiscussion(long DiscussionId)
        {
            var discussionCommentList = _unitOfWork.DiscussionCommentRepository.GetMany(t => t.DiscussionId == DiscussionId).Result.ToList();
            if (discussionCommentList != null && discussionCommentList.Count() > 0)
            {
                foreach (var existingItem in discussionCommentList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.DiscussionCommentRepository.DeleteAsync(existingItem).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return discussionCommentList;
        }
    }

    public partial interface IDiscussionCommentService : IService<DiscussionComment>
    {
        Task<DiscussionComment> CheckInsertOrUpdate(DiscussionCommentDto model, string methodName);
        List<DiscussionComment> GetAllCommentByDiscussionId(long DiscussionId);
        DiscussionComment GetById(long Id);
        Task<DiscussionComment> Delete(long Id);
        List<DiscussionComment> DeleteByDiscussion(long DiscussionId);
    }
}