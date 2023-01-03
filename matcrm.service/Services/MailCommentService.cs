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
    public partial class MailCommentService : Service<MailComment>, IMailCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailComment> CheckInsertOrUpdate(MailCommentDto model, string methodName)
        {
            var mailCommentObj = _mapper.Map<MailComment>(model);
            var existingItem = _unitOfWork.MailCommentRepository.GetMany(t => t.Id == mailCommentObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailComment(mailCommentObj);
            }
            else
            {
                existingItem.Comment = mailCommentObj.Comment;
                if (methodName == "PinUnPin")
                {
                    existingItem.IsPinned = mailCommentObj.IsPinned;
                    existingItem.PinnedBy = mailCommentObj.PinnedBy;
                }
                return await UpdateMailComment(existingItem, existingItem.Id);
            }
        }

        public async Task<MailComment> InsertMailComment(MailComment mailCommentObj)
        {
            mailCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailCommentRepository.Add(mailCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailComment> UpdateMailComment(MailComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            existingItem.UpdatedBy = existingItem.CreatedBy;
            await _unitOfWork.MailCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<MailComment> GetAllByThread(string ThreadId)
        {
            return _unitOfWork.MailCommentRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.OrderBy(t => t.CreatedOn).ToList();
        }

        public MailComment GetById(long Id)
        {
            return _unitOfWork.MailCommentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }



        public async Task<MailComment> Delete(long Id)
        {
            var mailCommentObj = _unitOfWork.MailCommentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailCommentObj != null)
            {
                mailCommentObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailCommentRepository.UpdateAsync(mailCommentObj, mailCommentObj.Id).Result;
                await _unitOfWork.CommitAsync();
            }
            return mailCommentObj;
        }

        public List<MailComment> DeleteByThread(string ThreadId)
        {
            var mailCommentList = _unitOfWork.MailCommentRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.ToList();
            if (mailCommentList != null && mailCommentList.Count() > 0)
            {
                foreach (var existingItem in mailCommentList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.MailCommentRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return mailCommentList;
        }
    }

    public partial interface IMailCommentService : IService<MailComment>
    {
        Task<MailComment> CheckInsertOrUpdate(MailCommentDto model, string methodName);
        List<MailComment> GetAllByThread(string ThreadId);
        MailComment GetById(long Id);
        Task<MailComment> Delete(long Id);
        List<MailComment> DeleteByThread(string ThreadId);
    }
}