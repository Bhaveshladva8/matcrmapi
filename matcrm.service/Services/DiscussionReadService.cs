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
    public partial class DiscussionReadService : Service<DiscussionRead>, IDiscussionReadService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DiscussionReadService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<DiscussionRead> CheckInsertOrUpdate(DiscussionReadDto model)
        {
            var discussionReadObj = _mapper.Map<DiscussionRead>(model);
            var existingItem = _unitOfWork.DiscussionReadRepository.GetMany(t => t.DiscussionId == discussionReadObj.DiscussionId && t.ReadBy == discussionReadObj.ReadBy && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDiscussionRead(discussionReadObj);
            }
            else
            {
                // existingItem.Comment = obj.Comment;
                return await UpdateDiscussionRead(existingItem, existingItem.Id);
            }
        }

        public async Task<DiscussionRead> InsertDiscussionRead(DiscussionRead discussionReadObj)
        {
            discussionReadObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.DiscussionReadRepository.Add(discussionReadObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<DiscussionRead> UpdateDiscussionRead(DiscussionRead existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.DiscussionReadRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<DiscussionRead> GetAllDiscussionRead(long DiscussionId)
        {
            return _unitOfWork.DiscussionReadRepository.GetMany(t => t.DiscussionId == DiscussionId && t.DeletedOn == null).Result.ToList();
        }

        public List<DiscussionRead> GetAll()
        {
            return _unitOfWork.DiscussionReadRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public DiscussionRead GetDiscussionByUserAndDiscussionId(long DiscussionId, int UserId)
        {
            return _unitOfWork.DiscussionReadRepository.GetMany(t => t.DiscussionId == DiscussionId && t.ReadBy == UserId && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public async Task<DiscussionRead> Delete(long Id)
        {
            var discussionReadObj = _unitOfWork.DiscussionReadRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (discussionReadObj != null)
            {
                discussionReadObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.DiscussionReadRepository.UpdateAsync(discussionReadObj, discussionReadObj.Id).Result;
                await _unitOfWork.CommitAsync();

                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<DiscussionRead> DeleteByDiscussion(long DiscussionId)
        {
            var discussionReadList = _unitOfWork.DiscussionReadRepository.GetMany(t => t.DiscussionId == DiscussionId).Result.ToList();
            if (discussionReadList != null && discussionReadList.Count > 0)
            {
                foreach (var existingItem in discussionReadList)
                {
                    // existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.DiscussionReadRepository.DeleteAsync(existingItem).Result;
                }
                _unitOfWork.CommitAsync();
            }

            return discussionReadList;
        }
    }

    public partial interface IDiscussionReadService : IService<DiscussionRead>
    {
        Task<DiscussionRead> CheckInsertOrUpdate(DiscussionReadDto model);
        List<DiscussionRead> GetAllDiscussionRead(long DiscussionId);
        List<DiscussionRead> GetAll();
        DiscussionRead GetDiscussionByUserAndDiscussionId(long DiscussionId, int UserId);
        Task<DiscussionRead> Delete(long Id);
        List<DiscussionRead> DeleteByDiscussion(long DiscussionId);
    }
}