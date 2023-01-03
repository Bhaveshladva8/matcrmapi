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
    public partial class TeamInboxAccessService : Service<TeamInboxAccess>, ITeamInboxAccessService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TeamInboxAccessService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<TeamInboxAccess> CheckInsertOrUpdate(TeamInboxAccessDto model)
        {
            var teamInboxAccessObj = _mapper.Map<TeamInboxAccess>(model);
            var existingItem = _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.TeamInboxId == teamInboxAccessObj.TeamInboxId && t.TeamMateId == teamInboxAccessObj.TeamMateId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTeamInboxAccess(teamInboxAccessObj);
            }
            else
            {
                teamInboxAccessObj.CreatedOn = existingItem.CreatedOn;
                teamInboxAccessObj.CreatedBy = existingItem.CreatedBy;
                teamInboxAccessObj.Id = existingItem.Id;
                teamInboxAccessObj.TeamInboxId = existingItem.TeamInboxId;
                return await UpdateTeamInboxAccess(teamInboxAccessObj, existingItem.Id);
            }
        }

        public async Task<TeamInboxAccess> UpdateTeamInboxAccess(TeamInboxAccess updatedItem, long existingId)
        {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.TeamInboxAccessRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<TeamInboxAccess> InsertTeamInboxAccess(TeamInboxAccess teamInboxAccessObj)
        {
            teamInboxAccessObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TeamInboxAccessRepository.AddAsync(teamInboxAccessObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<TeamInboxAccess> GetAll()
        {
            return _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<TeamInboxAccess> GetByTeamInbox(long teamInboxId)
        {
            return _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.TeamInboxId == teamInboxId && t.IsDeleted == false).Result.ToList();
        }

        public List<TeamInboxAccess> GetByTeamMate(int TeamMateId)
        {
            return _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.TeamMateId == TeamMateId && t.IsDeleted == false).Result.Include(t => t.TeamInbox).Include(t => t.TeamInbox.IntProviderAppSecret).Include(t => t.TeamInbox.IntProviderAppSecret.IntProviderApp).Include(t => t.TeamInbox.IntProviderAppSecret.IntProviderApp.IntProvider).ToList();
        }

        public TeamInboxAccess GetById(long Id)
        {
            return _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }


        public async Task<TeamInboxAccess> DeleteTeamInboxAccess(long Id)
        {
            var teamInboxAccessObj = _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (teamInboxAccessObj != null)
            {
                teamInboxAccessObj.DeletedOn = DateTime.UtcNow;
                teamInboxAccessObj.IsDeleted = true;
                var newItem = await _unitOfWork.TeamInboxAccessRepository.UpdateAsync(teamInboxAccessObj, teamInboxAccessObj.Id);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<TeamInboxAccess>> DeleteByTeam(long TeamInboxId)
        {
            var teamInboxAccessList = _unitOfWork.TeamInboxAccessRepository.GetMany(t => t.TeamInboxId == TeamInboxId && t.IsDeleted == false).Result.ToList();
            if (teamInboxAccessList != null && teamInboxAccessList.Count() > 0)
            {
                foreach (var existingItem in teamInboxAccessList)
                {
                    if (existingItem != null)
                    {
                        existingItem.DeletedOn = DateTime.UtcNow;
                        existingItem.IsDeleted = true;
                        var newItem = await _unitOfWork.TeamInboxAccessRepository.UpdateAsync(existingItem, existingItem.Id);
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            return teamInboxAccessList;
        }
    }

    public partial interface ITeamInboxAccessService : IService<TeamInboxAccess>
    {
        Task<TeamInboxAccess> CheckInsertOrUpdate(TeamInboxAccessDto model);
        List<TeamInboxAccess> GetByTeamInbox(long teamInboxId);
        List<TeamInboxAccess> GetAll();
        Task<TeamInboxAccess> DeleteTeamInboxAccess(long Id);
        Task<List<TeamInboxAccess>> DeleteByTeam(long TeamInboxId);
        TeamInboxAccess GetById(long Id);
        List<TeamInboxAccess> GetByTeamMate(int TeamMateId);
    }
}