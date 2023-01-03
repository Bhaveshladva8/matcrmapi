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
    public partial class EmailTemplateService : Service<EmailTemplate>, IEmailTemplateService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public EmailTemplateService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public EmailTemplate GetEmailTemplateByCode(string code)
        {
            return _unitOfWork.EmailTemplateRepository.GetMany(t => t.TemplateCode.ToLower() == code.ToLower()).Result.FirstOrDefault();
        }

        public EmailTemplate GetEmailTemplateById(long emailTemplateId)
        {
            return _unitOfWork.EmailTemplateRepository.GetMany(x => x.EmailTemplateId == emailTemplateId).Result.FirstOrDefault();
        }

        public async Task<EmailTemplate> CheckInsertOrUpdate(EmailTemplateDto model)
        {
            var emailTemplateObj = _mapper.Map<EmailTemplate>(model);
            var existingItem = _unitOfWork.EmailTemplateRepository.GetMany(t => t.EmailTemplateId == emailTemplateObj.EmailTemplateId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                emailTemplateObj.EmailTemplateId = Convert.ToInt64(model.EmailTemplateId);
                return await InsertEmailTemplate(emailTemplateObj);
            }
            else
            {
                return await UpdateEmailTemplate(emailTemplateObj, existingItem.EmailTemplateId);
            }
        }

        public async Task<EmailTemplate> InsertEmailTemplate(EmailTemplate emailTemplateObj)
        {
            emailTemplateObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmailTemplateRepository.AddAsync(emailTemplateObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public async Task<EmailTemplate> UpdateEmailTemplate(EmailTemplate updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.EmailTemplateRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public List<EmailTemplate> GetAll()
        {
            return _unitOfWork.EmailTemplateRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public EmailTemplate DeleteEmailTemplate(long EmailTemplateId)
        {
            var emailTemplateObj = _unitOfWork.EmailTemplateRepository.GetMany(t => t.EmailTemplateId == EmailTemplateId).Result.FirstOrDefault();
            if(emailTemplateObj != null)
            {
                emailTemplateObj.IsDeleted = true;
                emailTemplateObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.EmailTemplateRepository.UpdateAsync(emailTemplateObj, emailTemplateObj.EmailTemplateId);
                _unitOfWork.CommitAsync();
            }
            return emailTemplateObj;
        }
    }

    public partial interface IEmailTemplateService : IService<EmailTemplate>
    {
        EmailTemplate GetEmailTemplateByCode(string code);
        EmailTemplate GetEmailTemplateById(long emailTemplateId);
        Task<EmailTemplate> CheckInsertOrUpdate(EmailTemplateDto model);
        List<EmailTemplate> GetAll();
        EmailTemplate DeleteEmailTemplate(long EmailTemplateId);
    }
}