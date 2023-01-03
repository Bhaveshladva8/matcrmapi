using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Utility;

namespace matcrm.service.Services
{
    public partial class EmailConfigService : Service<EmailConfig>, IEmailConfigService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmailConfigService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmailConfig> CheckInsertOrUpdate(EmailConfigDto model)
        {
            var emailConfigObj = _mapper.Map<EmailConfig>(model);
            var existingItem = _unitOfWork.EmailConfigRepository.GetMany(t => t.TenantId == emailConfigObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmailConfig(emailConfigObj);
            }
            else
            {
                existingItem.Email = emailConfigObj.Email;
                if (existingItem.Password != ShaHashData.EncodePasswordToBase64(emailConfigObj.Password))
                {
                    existingItem.Password = ShaHashData.EncodePasswordToBase64(emailConfigObj.Password);
                }
                else
                {
                    existingItem.Password = ShaHashData.EncodePasswordToBase64(emailConfigObj.Password);
                }

                existingItem.EmailProviderId = emailConfigObj.EmailProviderId;
                existingItem.TenantId = emailConfigObj.TenantId;
                // existingItem.IsActive = true;
                return await UpdateEmailConfig(existingItem, existingItem.Id);
            }
        }

        public async Task<EmailConfig> InsertEmailConfig(EmailConfig emailConfigObj)
        {
            emailConfigObj.CreatedOn = DateTime.UtcNow;
            emailConfigObj.IsActive = true;
            emailConfigObj.Password = ShaHashData.EncodePasswordToBase64(emailConfigObj.Password);
            var newItem = await _unitOfWork.EmailConfigRepository.AddAsync(emailConfigObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmailConfig> UpdateEmailConfig(EmailConfig existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmailConfigRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmailConfig> GetAll()
        {
            return _unitOfWork.EmailConfigRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }


        public List<EmailConfig> GetAllActiveEmailConfig()
        {
            return _unitOfWork.EmailConfigRepository.GetMany(t => t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public EmailConfig GetEmailConfigByTenant(long TenantId)
        {
            return _unitOfWork.EmailConfigRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public EmailConfig GetEmailConfigById(long Id)
        {
            return _unitOfWork.EmailConfigRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public EmailConfig GetDefaultEmailConfig()
        {
            return _unitOfWork.EmailConfigRepository.GetMany(t => t.TenantId == null && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public EmailConfig DeleteEmailConfig(long Id)
        {
            var emailConfigObj = _unitOfWork.EmailConfigRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (emailConfigObj != null)
            {
                emailConfigObj.IsDeleted = true;
                emailConfigObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.EmailConfigRepository.UpdateAsync(emailConfigObj, emailConfigObj.Id);
                _unitOfWork.CommitAsync();
            }
            return emailConfigObj;
        }
    }

    public partial interface IEmailConfigService : IService<EmailConfig>
    {
        Task<EmailConfig> CheckInsertOrUpdate(EmailConfigDto model);
        List<EmailConfig> GetAll();
        List<EmailConfig> GetAllActiveEmailConfig();
        EmailConfig GetEmailConfigByTenant(long TenantId);

        EmailConfig GetDefaultEmailConfig();
        EmailConfig GetEmailConfigById(long Id);
        EmailConfig DeleteEmailConfig(long Id);
    }
}