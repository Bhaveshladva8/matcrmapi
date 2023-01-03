using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class EmailProviderService : Service<EmailProvider>, IEmailProviderService {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmailProviderService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmailProvider> CheckInsertOrUpdate (EmailProviderDto model) {
            var emailProviderObj = _mapper.Map<EmailProvider> (model);
            var existingItem = _unitOfWork.EmailProviderRepository.GetMany (t => t.Id == emailProviderObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertEmailProvider (emailProviderObj);
            } else {
                existingItem.ProviderName = emailProviderObj.ProviderName;
                existingItem.Host = emailProviderObj.Host;
                existingItem.Port = emailProviderObj.Port;
                return await UpdateEmailProvider (existingItem, existingItem.Id);
            }
        }

        public async Task<EmailProvider> InsertEmailProvider (EmailProvider emailProviderObj) {
            emailProviderObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmailProviderRepository.AddAsync (emailProviderObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<EmailProvider> UpdateEmailProvider (EmailProvider existingItem, int existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmailProviderRepository.UpdateAsync (existingItem, existingId);
            await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<EmailProvider> GetAll () {
            return _unitOfWork.EmailProviderRepository.GetMany (t => t.IsDeleted == false).Result.ToList();
        }


        public EmailProvider GetEmailProvider (string ProviderName) {
            return _unitOfWork.EmailProviderRepository.GetMany (t => t.ProviderName == ProviderName && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public EmailProvider GetEmailProviderById (int Id) {
            return _unitOfWork.EmailProviderRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public EmailProvider DeleteEmailProvider (int Id) {
            var emailProviderObj = _unitOfWork.EmailProviderRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if(emailProviderObj != null)
            {
                emailProviderObj.IsDeleted = true;
                emailProviderObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.EmailProviderRepository.UpdateAsync (emailProviderObj, emailProviderObj.Id);
                _unitOfWork.CommitAsync ();
            }
            return emailProviderObj;
        }
    }

    public partial interface IEmailProviderService : IService<EmailProvider> {
        Task<EmailProvider> CheckInsertOrUpdate (EmailProviderDto model);
        List<EmailProvider> GetAll ();
        EmailProvider GetEmailProvider (string ProviderName);
        EmailProvider GetEmailProviderById (int Id);
        EmailProvider DeleteEmailProvider (int Id);
    }
}