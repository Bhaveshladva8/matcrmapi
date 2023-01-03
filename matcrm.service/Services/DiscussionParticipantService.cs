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
    public partial class DiscussionParticipantService : Service<DiscussionParticipant>, IDiscussionParticipantService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DiscussionParticipantService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<DiscussionParticipant> CheckInsertOrUpdate(DiscussionParticipantDto model)
        {
            var discussionParticipantObj = _mapper.Map<DiscussionParticipant>(model);
            var existingItem = _unitOfWork.DiscussionParticipantRepository.GetMany(t => t.DiscussionId == discussionParticipantObj.DiscussionId && t.TeamMemberId == discussionParticipantObj.TeamMemberId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDiscussionParticipant(discussionParticipantObj);
            }
            else
            {
                existingItem.TeamMemberId = discussionParticipantObj.TeamMemberId;
                existingItem.UpdatedBy = discussionParticipantObj.CreatedBy;
                return await UpdateDiscussionParticipant(existingItem, existingItem.Id);
            }
        }

        public async Task<DiscussionParticipant> InsertDiscussionParticipant(DiscussionParticipant discussionParticipantObj)
        {
            discussionParticipantObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.DiscussionParticipantRepository.Add(discussionParticipantObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<DiscussionParticipant> UpdateDiscussionParticipant(DiscussionParticipant existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.DiscussionParticipantRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<DiscussionParticipant> GetAllDiscussionParticipant(long DiscussionId)
        {
            return _unitOfWork.DiscussionParticipantRepository.GetMany(t => t.DiscussionId == DiscussionId && t.DeletedOn == null).Result.ToList();
        }


        public List<DiscussionParticipant> GetAllByTeamMate(int TeamMateId)
        {
            return _unitOfWork.DiscussionParticipantRepository.GetMany(t => t.TeamMemberId == TeamMateId && t.DeletedOn == null).Result.Include(t => t.Discussion).ToList();
        }

        public async Task<DiscussionParticipant> Delete(long Id)
        {
            var discussionParticipantObj = _unitOfWork.DiscussionParticipantRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (discussionParticipantObj != null)
            {
                discussionParticipantObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.DiscussionParticipantRepository.UpdateAsync(discussionParticipantObj, discussionParticipantObj.Id).Result;
                await _unitOfWork.CommitAsync();

                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<DiscussionParticipant> DeleteByDiscussion(long DiscussionId)
        {
            var discussionParticipantList = _unitOfWork.DiscussionParticipantRepository.GetMany(t => t.DiscussionId == DiscussionId).Result.ToList();
            if (discussionParticipantList != null && discussionParticipantList.Count() > 0)
            {
                foreach (var existingItem in discussionParticipantList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.DiscussionParticipantRepository.DeleteAsync(existingItem).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return discussionParticipantList;
        }
    }

    public partial interface IDiscussionParticipantService : IService<DiscussionParticipant>
    {
        Task<DiscussionParticipant> CheckInsertOrUpdate(DiscussionParticipantDto model);
        List<DiscussionParticipant> GetAllDiscussionParticipant(long DiscussionId);
        Task<DiscussionParticipant> Delete(long Id);
        List<DiscussionParticipant> DeleteByDiscussion(long DiscussionId);
        List<DiscussionParticipant> GetAllByTeamMate(int TeamMateId);
    }
}