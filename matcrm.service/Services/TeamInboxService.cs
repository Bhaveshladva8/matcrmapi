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
    public partial class TeamInboxService : Service<TeamInbox>, ITeamInboxService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TeamInboxService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamInbox> CheckInsertOrUpdate(TeamInboxDto model)
        {
            var teamInboxObj= _mapper.Map<TeamInbox>(model);
            TeamInbox? existingItem;
            if (model.Id != null)
            {
                existingItem = _unitOfWork.TeamInboxRepository.GetMany(t => t.Id == teamInboxObj.Id && t.CreatedBy == teamInboxObj.CreatedBy && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.TeamInboxRepository.GetMany(t => t.IntProviderAppSecretId == teamInboxObj.IntProviderAppSecretId && t.Name == teamInboxObj.Name && t.MailBoxTeamId == teamInboxObj.MailBoxTeamId && t.CreatedBy == teamInboxObj.CreatedBy && t.DeletedOn == null).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertTeamInbox(teamInboxObj);
            }
            else
            {
                // obj.CreatedOn = existingItem.CreatedOn;
                // obj.CreatedBy = existingItem.CreatedBy;
                // obj.UpdatedBy = existingItem.CreatedBy;
                // obj.Id = existingItem.Id;
                existingItem.Name = teamInboxObj.Name;
                existingItem.Color = teamInboxObj.Color;
                existingItem.IsPublic = teamInboxObj.IsPublic;
                return await UpdateTeamInbox(existingItem, existingItem.Id);
            }
        }

        public async Task<TeamInbox> UpdateTeamInbox(TeamInbox updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.TeamInboxRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<TeamInbox> InsertTeamInbox(TeamInbox teamInboxObj)
        {
            teamInboxObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TeamInboxRepository.AddAsync(teamInboxObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<TeamInbox> GetAll()
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public TeamInbox GetByName(string Name)
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null && t.Name == Name).Result.FirstOrDefault();
        }

        public TeamInbox GetById(long Id)
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).FirstOrDefault();
        }

        public TeamInbox GetByAppSecretId(long AppSecretId, int userId)
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null && t.IntProviderAppSecretId == AppSecretId && t.CreatedBy == userId).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).FirstOrDefault();
        }

        public List<TeamInbox> GetByUser(int UserId)
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null && t.CreatedBy == UserId).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).ToList();
        }

        public List<TeamInbox> GetByTeam(long TeamId)
        {
            return _unitOfWork.TeamInboxRepository.GetMany(t => t.DeletedOn == null && t.MailBoxTeamId == TeamId).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.IntProviderAppSecret.IntProviderApp.IntProvider).ToList();
        }

        public async Task<TeamInbox> DeleteTeamInbox(long Id)
        {
            var teamInboxObj = _unitOfWork.TeamInboxRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (teamInboxObj != null)
            {
                teamInboxObj.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.TeamInboxRepository.UpdateAsync(teamInboxObj, teamInboxObj.Id);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ITeamInboxService : IService<TeamInbox>
    {
        Task<TeamInbox> CheckInsertOrUpdate(TeamInboxDto model);
        List<TeamInbox> GetAll();
        Task<TeamInbox> DeleteTeamInbox(long Id);
        TeamInbox GetById(long Id);
        TeamInbox GetByAppSecretId(long AppSecretId,  int userId);
        List<TeamInbox> GetByUser(int UserId);
        List<TeamInbox> GetByTeam(long TeamId);
        // TeamInbox GetByName(string Name);
    }
}