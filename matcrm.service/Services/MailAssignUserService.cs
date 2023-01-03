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
    public partial class MailAssignUserService : Service<MailAssignUser>, IMailAssignUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailAssignUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailAssignUser> CheckInsertOrUpdate(MailAssignUserDto model)
        {
            var mailAssignUserObj = _mapper.Map<MailAssignUser>(model);
            var existingItem = _unitOfWork.MailAssignUserRepository.GetMany(t => t.ThreadId == mailAssignUserObj.ThreadId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailAssignUser(mailAssignUserObj);
            }
            else
            {
                existingItem.TeamMemberId = mailAssignUserObj.TeamMemberId;
                return await UpdateMailAssignUser(existingItem, existingItem.Id);
            }
        }

        public async Task<MailAssignUser> InsertMailAssignUser(MailAssignUser mailAssignUserObj)
        {
            mailAssignUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailAssignUserRepository.Add(mailAssignUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailAssignUser> UpdateMailAssignUser(MailAssignUser existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MailAssignUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public MailAssignUser GetAllMailAssignUserByThread(string ThreadId)
        {
            return _unitOfWork.MailAssignUserRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public List<MailAssignUser> GetAllByTeamMember(int TeamMemberId)
        {
            return _unitOfWork.MailAssignUserRepository.GetMany(t => t.TeamMemberId == TeamMemberId && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.User).ToList();
        }

        public List<MailAssignUser> GetAllBySecretId(long IntProviderAppSecretId)
        {
            return _unitOfWork.MailAssignUserRepository.GetMany(t => t.IntProviderAppSecretId == IntProviderAppSecretId && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.User).ToList();
        }


        public MailAssignUser GetById(long Id)
        {
            return _unitOfWork.MailAssignUserRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.User).FirstOrDefault();
        }

        public MailAssignUser GetByUserThread(int TeamMemberId, string ThreadId)
        {
            return _unitOfWork.MailAssignUserRepository.GetMany(t => t.TeamMemberId == TeamMemberId && t.ThreadId == ThreadId && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).Include(t => t.User).FirstOrDefault();
        }

        public async Task<MailAssignUser> Delete(long Id)
        {
            var mailAssignUserObj = _unitOfWork.MailAssignUserRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailAssignUserObj != null)
            {
                mailAssignUserObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailAssignUserRepository.UpdateAsync(mailAssignUserObj, mailAssignUserObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }

        }

        public MailAssignUser DeleteByThread(string ThreadId)
        {
            var mailAssignUserObj = _unitOfWork.MailAssignUserRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailAssignUserObj != null)
            {
                mailAssignUserObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.MailAssignUserRepository.UpdateAsync(mailAssignUserObj, mailAssignUserObj.Id).Result;

                _unitOfWork.CommitAsync();
            }
            return mailAssignUserObj;
        }

        public async Task<List<MailAssignUser>> DeleteBySecretId(long IntProviderAppSecretId)
        {
            var mailAssignUserList = _unitOfWork.MailAssignUserRepository.GetMany(t => t.IntProviderAppSecretId == IntProviderAppSecretId && t.DeletedOn == null).Result.ToList();
            if (mailAssignUserList != null && mailAssignUserList.Count() > 0)
            {
                foreach (var existingItem in mailAssignUserList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = await _unitOfWork.MailAssignUserRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mailAssignUserList;
        }
    }

    public partial interface IMailAssignUserService : IService<MailAssignUser>
    {
        Task<MailAssignUser> CheckInsertOrUpdate(MailAssignUserDto model);
        MailAssignUser GetAllMailAssignUserByThread(string ThreadId);
        MailAssignUser GetByUserThread(int TeamMemberId, string ThreadId);
        Task<MailAssignUser> Delete(long Id);
        MailAssignUser DeleteByThread(string ThreadId);
        Task<List<MailAssignUser>> DeleteBySecretId(long IntProviderAppSecretId);
        List<MailAssignUser> GetAllByTeamMember(int TeamMemberId);
        MailAssignUser GetById(long Id);
    }
}