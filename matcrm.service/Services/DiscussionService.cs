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
    public partial class DiscussionService : Service<Discussion>, IDiscussionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DiscussionService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Discussion> CheckInsertOrUpdate(DiscussionDto model, string methodName)
        {
            var discussionObj = _mapper.Map<Discussion>(model);
            var existingItem = _unitOfWork.DiscussionRepository.GetMany(t => t.Id == discussionObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDiscussion(discussionObj);
            }
            else
            {
                existingItem.IsArchived = discussionObj.IsArchived;
                existingItem.Note = discussionObj.Note;
                existingItem.Topic = discussionObj.Topic;
                if (!string.IsNullOrEmpty(methodName) && methodName == "Assign")
                {
                    existingItem.AssignUserId = discussionObj.AssignUserId;
                }
                else if (!string.IsNullOrEmpty(methodName) && methodName == "UnAssign")
                {
                    existingItem.AssignUserId = null;
                }
                else if (!string.IsNullOrEmpty(methodName) && methodName == "AssignCustomer")
                {
                    existingItem.CustomerId = discussionObj.CustomerId;
                }
                else if (!string.IsNullOrEmpty(methodName) && methodName == "UnAssignCustomer")
                {
                    existingItem.CustomerId = null;
                }
                existingItem.UpdatedBy = discussionObj.UpdatedBy;
                return await UpdateDiscussion(existingItem, existingItem.Id);
            }
        }

        public async Task<Discussion> InsertDiscussion(Discussion discussionObj)
        {
            discussionObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.DiscussionRepository.Add(discussionObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Discussion> UpdateDiscussion(Discussion existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.DiscussionRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<Discussion> GetByUser(int UserId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.ToList();
        }

        public List<Discussion> GetAllByUser(int UserId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.CreatedBy == UserId).Result.ToList();
        }
        public List<Discussion> GetTrashedByUser(int UserId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn != null).Result.ToList();
        }

        public List<Discussion> GetByAssignUser(int AssignUserId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.AssignUserId == AssignUserId && t.DeletedOn == null).Result.ToList();
        }

        public List<Discussion> GetByAssignCustomer(long CustomerId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.CustomerId == CustomerId && t.DeletedOn == null).Result.ToList();
        }

        public Discussion GetById(long DiscussionId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.Id == DiscussionId && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public Discussion GetDiscussion(long DiscussionId)
        {
            return _unitOfWork.DiscussionRepository.GetMany(t => t.Id == DiscussionId).Result.FirstOrDefault();
        }

        public async Task<Discussion> Trash(long Id)
        {
            var discussionObj = _unitOfWork.DiscussionRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (discussionObj != null)
            {
                discussionObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.DiscussionRepository.UpdateAsync(discussionObj, discussionObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }

        }

        public async Task<Discussion> Delete(long Id)
        {
            var discussionObj = _unitOfWork.DiscussionRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            // existingItem.DeletedOn = DateTime.UtcNow;
            if (discussionObj != null)
            {
                await _unitOfWork.DiscussionRepository.DeleteAsync(discussionObj);
                await _unitOfWork.CommitAsync();
            }
            return discussionObj;
        }

        public async Task<Discussion> DeleteByUser(int UserId)
        {
            var discussionObj = _unitOfWork.DiscussionRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (discussionObj != null)
            {
                discussionObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.DiscussionRepository.UpdateAsync(discussionObj, discussionObj.Id).Result;
                await _unitOfWork.CommitAsync();
            }
            return discussionObj;
        }
    }

    public partial interface IDiscussionService : IService<Discussion>
    {
        Task<Discussion> CheckInsertOrUpdate(DiscussionDto model, string methodName);
        List<Discussion> GetByUser(int UserId);
        List<Discussion> GetAllByUser(int UserId);
        List<Discussion> GetTrashedByUser(int UserId);
        List<Discussion> GetByAssignUser(int AssignUserId);
        List<Discussion> GetByAssignCustomer(long CustomerId);
        Discussion GetById(long DiscussionId);
        Task<Discussion> Trash(long Id);
        Task<Discussion> DeleteByUser(int UserId);
        Task<Discussion> Delete(long Id);
        Discussion GetDiscussion(long DiscussionId);
    }
}