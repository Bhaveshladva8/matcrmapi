using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class DiscussionCommentAttachmentService : Service<DiscussionCommentAttachment>, IDiscussionCommentAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public DiscussionCommentAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<DiscussionCommentAttachment> CheckInsertOrUpdate(DiscussionCommentAttachmentDto model)
        {
            var discussionCommentAttachmentObj = _mapper.Map<DiscussionCommentAttachment>(model);
            var existingItem = _unitOfWork.DiscussionCommentAttachmentRepository.GetMany(t => t.FileName == discussionCommentAttachmentObj.FileName && t.DiscussionCommentId == discussionCommentAttachmentObj.DiscussionCommentId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDiscussionCommentAttachment(discussionCommentAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateDiscussionCommentAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<DiscussionCommentAttachment> InsertDiscussionCommentAttachment(DiscussionCommentAttachment discussionCommentAttachmentObj)
        {
            discussionCommentAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.DiscussionCommentAttachmentRepository.AddAsync(discussionCommentAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<DiscussionCommentAttachment> UpdateDiscussionCommentAttachment(DiscussionCommentAttachment existingItem, long existingId)
        {
            await _unitOfWork.DiscussionCommentAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<DiscussionCommentAttachment> GetAllByDiscussionCommentId(long DiscussionCommentId)
        {
            return _unitOfWork.DiscussionCommentAttachmentRepository.GetMany(t => t.DiscussionCommentId == DiscussionCommentId && t.IsDeleted == false).Result.ToList();
        }

        public DiscussionCommentAttachment GetById(long Id)
        {
            return _unitOfWork.DiscussionCommentAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<DiscussionCommentAttachment> DeleteDiscussionCommentAttachmentById(long Id)
        {
            var discussionCommentAttachmentObj = _unitOfWork.DiscussionCommentAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.DiscussionComment).Include(t => t.DiscussionComment.Discussion).FirstOrDefault();
            if (discussionCommentAttachmentObj != null)
            {
                discussionCommentAttachmentObj.IsDeleted = true;
                discussionCommentAttachmentObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.DiscussionCommentAttachmentRepository.UpdateAsync(discussionCommentAttachmentObj, discussionCommentAttachmentObj.Id);
                await _unitOfWork.CommitAsync();
            }

            return discussionCommentAttachmentObj;
        }


        public List<DiscussionCommentAttachment> DeleteAttachmentByDiscussionComment(long DiscussionCommentId)
        {
            var discussionCommentAttachmentList = _unitOfWork.DiscussionCommentAttachmentRepository.GetMany(t => t.DiscussionCommentId == DiscussionCommentId).Result.ToList();
            if (discussionCommentAttachmentList != null && discussionCommentAttachmentList.Count() > 0)
            {
                foreach (var item in discussionCommentAttachmentList)
                {
                    item.IsDeleted = true;
                    _unitOfWork.DiscussionCommentAttachmentRepository.DeleteAsync(item);
                }

                _unitOfWork.CommitAsync();
            }

            return discussionCommentAttachmentList;
        }
    }
    public partial interface IDiscussionCommentAttachmentService : IService<DiscussionCommentAttachment>
    {
        Task<DiscussionCommentAttachment> CheckInsertOrUpdate(DiscussionCommentAttachmentDto model);
        List<DiscussionCommentAttachment> GetAllByDiscussionCommentId(long DiscussionId);
        DiscussionCommentAttachment GetById(long Id);
        Task<DiscussionCommentAttachment> DeleteDiscussionCommentAttachmentById(long Id);
        List<DiscussionCommentAttachment> DeleteAttachmentByDiscussionComment(long DiscussionCommentId);
        Task<DiscussionCommentAttachment> UpdateDiscussionCommentAttachment(DiscussionCommentAttachment existingItem, long existingId);
    }
}