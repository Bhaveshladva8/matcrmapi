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
    public partial class EmailLogService : Service<EmailLog>, IEmailLogService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmailLogService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmailLog> CheckInsertOrUpdate(EmailLogDto model)
        {
            var emailLogObj = _mapper.Map<EmailLog>(model);
            var existingItem = _unitOfWork.EmailLogRepository.GetMany(t => t.Id == emailLogObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmailLog(emailLogObj);
            }
            else
            {
                existingItem.Body = emailLogObj.Body;
                existingItem.Status = emailLogObj.Status;
                existingItem.FromEmail = emailLogObj.FromEmail;
                existingItem.ToEmail = emailLogObj.ToEmail;
                existingItem.Subject = emailLogObj.Subject;
                existingItem.TemplateCode = emailLogObj.TemplateCode;
                existingItem.FromLabel = emailLogObj.FromLabel;
                existingItem.Tried = emailLogObj.Tried;
                return await UpdateEmailLog(existingItem, existingItem.Id);
            }
        }

        public async Task<EmailLog> InsertEmailLog(EmailLog emailLogObj)
        {
            emailLogObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmailLogRepository.AddAsync(emailLogObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmailLog> UpdateEmailLog(EmailLog existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmailLogRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmailLog> GetAllEmailByAdmin(long? TenantId)
        {

            return _unitOfWork.EmailLogRepository.GetMany(t => t.TenantId == TenantId).Result.ToList();


        }

        public List<EmailLog> GetEmailLogByTenant(long TenantId)
        {
            return _unitOfWork.EmailLogRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.ToList();
        }

        public EmailLog GetEmailLogById(long Id)
        {
            return _unitOfWork.EmailLogRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<EmailLog> GetAllByAdmin()
        {
            return _unitOfWork.EmailLogRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<EmailLog> GetAllFailEmail()
        {
            return _unitOfWork.EmailLogRepository.GetMany(t => t.Status == false && t.Tried < 3 && t.IsDeleted == false).Result.ToList();
        }

        public EmailLog DeleteEmailLog(long Id)
        {
            var emailLogObj = _unitOfWork.EmailLogRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (emailLogObj != null)
            {
                emailLogObj.IsDeleted = true;
                emailLogObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.EmailLogRepository.UpdateAsync(emailLogObj, emailLogObj.Id);
                _unitOfWork.CommitAsync();
            }
            return emailLogObj;
        }
    }

    public partial interface IEmailLogService : IService<EmailLog>
    {
        Task<EmailLog> CheckInsertOrUpdate(EmailLogDto model);

        List<EmailLog> GetAllByAdmin();
        List<EmailLog> GetAllEmailByAdmin(long? TenantId);
        List<EmailLog> GetEmailLogByTenant(long TenantId);
        EmailLog GetEmailLogById(long Id);
        EmailLog DeleteEmailLog(long Id);
        List<EmailLog> GetAllFailEmail();
    }
}