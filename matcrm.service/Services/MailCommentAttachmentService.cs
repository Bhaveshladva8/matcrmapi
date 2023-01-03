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
    public partial class MailCommentAttachmentService : Service<MailCommentAttachment>, IMailCommentAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MailCommentAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MailCommentAttachment> CheckInsertOrUpdate(MailCommentAttachmentDto model)
        {
            var mailCommentAttachmentObj = _mapper.Map<MailCommentAttachment>(model);
            var existingItem = _unitOfWork.MailCommentAttachmentRepository.GetMany(t => t.FileName == mailCommentAttachmentObj.FileName && t.MailCommentId == mailCommentAttachmentObj.MailCommentId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailCommentAttachment(mailCommentAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateMailCommentAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<MailCommentAttachment> InsertMailCommentAttachment(MailCommentAttachment mailCommentAttachmentObj)
        {
            mailCommentAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MailCommentAttachmentRepository.AddAsync(mailCommentAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailCommentAttachment> UpdateMailCommentAttachment(MailCommentAttachment existingItem, long existingId)
        {
            await _unitOfWork.MailCommentAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<MailCommentAttachment> GetAllByMailCommentId(long MailCommentId)
        {
            return _unitOfWork.MailCommentAttachmentRepository.GetMany(t => t.MailCommentId == MailCommentId && t.IsDeleted == false).Result.ToList();
        }

        public MailCommentAttachment GetById(long Id)
        {
            return _unitOfWork.MailCommentAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<MailCommentAttachment> DeleteMailCommentAttachmentById(long Id)
        {
            var mailCommentAttachmentObj = _unitOfWork.MailCommentAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.MailComment).FirstOrDefault();
            if (mailCommentAttachmentObj != null)
            {
                mailCommentAttachmentObj.IsDeleted = true;
                mailCommentAttachmentObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.MailCommentAttachmentRepository.UpdateAsync(mailCommentAttachmentObj, mailCommentAttachmentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mailCommentAttachmentObj;
        }


        public List<MailCommentAttachment> DeleteAttachmentByMailComment(long MailCommentId)
        {
            var mailCommentAttachmentList = _unitOfWork.MailCommentAttachmentRepository.GetMany(t => t.MailCommentId == MailCommentId).Result.ToList();
            if (mailCommentAttachmentList != null && mailCommentAttachmentList.Count() > 0)
            {
                foreach (var item in mailCommentAttachmentList)
                {
                    item.IsDeleted = true;
                    _unitOfWork.MailCommentAttachmentRepository.DeleteAsync(item);
                }

                _unitOfWork.CommitAsync();
            }
            return mailCommentAttachmentList;
        }
    }
    public partial interface IMailCommentAttachmentService : IService<MailCommentAttachment>
    {
        Task<MailCommentAttachment> CheckInsertOrUpdate(MailCommentAttachmentDto model);
        List<MailCommentAttachment> GetAllByMailCommentId(long MailCommentId);
        MailCommentAttachment GetById(long Id);
        Task<MailCommentAttachment> DeleteMailCommentAttachmentById(long Id);
        List<MailCommentAttachment> DeleteAttachmentByMailComment(long MailCommentId);
        Task<MailCommentAttachment> UpdateMailCommentAttachment(MailCommentAttachment existingItem, long existingId);
    }
}