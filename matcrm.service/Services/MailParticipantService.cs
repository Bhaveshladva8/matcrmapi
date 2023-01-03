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
    public partial class MailParticipantService : Service<MailParticipant>, IMailParticipantService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailParticipantService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailParticipant> CheckInsertOrUpdate(MailParticipantDto model)
        {
            var mailParticipantObj = _mapper.Map<MailParticipant>(model);
            var existingItem = _unitOfWork.MailParticipantRepository.GetMany(t => t.ThreadId == mailParticipantObj.ThreadId && t.TeamMemberId == mailParticipantObj.TeamMemberId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailParticipant(mailParticipantObj);
            }
            else
            {
                existingItem.ThreadId = mailParticipantObj.ThreadId;
                existingItem.IntProviderAppSecretId = mailParticipantObj.IntProviderAppSecretId;
                existingItem.UpdatedBy = mailParticipantObj.CreatedBy;
                return await UpdateMailParticipant(existingItem, existingItem.Id);
            }
        }

        public async Task<MailParticipant> InsertMailParticipant(MailParticipant mailParticipantObj)
        {
            mailParticipantObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailParticipantRepository.Add(mailParticipantObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailParticipant> UpdateMailParticipant(MailParticipant existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MailParticipantRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<MailParticipant> GetAllByThread(string ThreadId)
        {
            return _unitOfWork.MailParticipantRepository.GetMany(t => t.ThreadId == ThreadId && t.IsDeleted == false).Result.ToList();
        }

        public MailParticipant GetById(long Id)
        {
            return _unitOfWork.MailParticipantRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public MailParticipant GetByUserThread(int UserId, string ThreadId)
        {
            return _unitOfWork.MailParticipantRepository.GetMany(t => t.ThreadId == ThreadId && t.TeamMemberId == UserId && t.IsDeleted == false).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).FirstOrDefault();
        }

        public List<MailParticipant> GetAllByTeamMember(int TeamMemberId)
        {
            return _unitOfWork.MailParticipantRepository.GetMany(t => t.TeamMemberId == TeamMemberId && t.IsDeleted == false).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).ToList();
        }

        public async Task<MailParticipant> Delete(long Id)
        {
            var mailParticipantObj = _unitOfWork.MailParticipantRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (mailParticipantObj != null)
            {
                mailParticipantObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailParticipantRepository.UpdateAsync(mailParticipantObj, mailParticipantObj.Id).Result;
                await _unitOfWork.CommitAsync();
            }
            return mailParticipantObj;
        }

        public List<MailParticipant> DeleteByThread(string ThreadId)
        {
            var mailParticipantList = _unitOfWork.MailParticipantRepository.GetMany(t => t.ThreadId == ThreadId && t.IsDeleted == false).Result.ToList();
            if (mailParticipantList != null && mailParticipantList.Count() > 0)
            {
                foreach (var existingItem in mailParticipantList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.MailParticipantRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return mailParticipantList;
        }

        public async Task<List<MailParticipant>> DeleteBySecretId(long IntProviderAppSecretId)
        {
            var mailParticipantList = _unitOfWork.MailParticipantRepository.GetMany(t => t.IntProviderAppSecretId == IntProviderAppSecretId && t.DeletedOn == null).Result.ToList();
            if (mailParticipantList != null && mailParticipantList.Count() > 0)
            {
                foreach (var existingItem in mailParticipantList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = await _unitOfWork.MailParticipantRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mailParticipantList;
        }
    }

    public partial interface IMailParticipantService : IService<MailParticipant>
    {
        Task<MailParticipant> CheckInsertOrUpdate(MailParticipantDto model);
        List<MailParticipant> GetAllByThread(string ThreadId);
        MailParticipant GetById(long Id);
        MailParticipant GetByUserThread(int UserId, string ThreadId);
        List<MailParticipant> GetAllByTeamMember(int TeamMemberId);
        Task<MailParticipant> Delete(long Id);
        List<MailParticipant> DeleteByThread(string ThreadId);
        Task<List<MailParticipant>> DeleteBySecretId(long IntProviderAppSecretId);
    }
}