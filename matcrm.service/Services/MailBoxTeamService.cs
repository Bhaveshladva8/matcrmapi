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
    public partial class MailBoxTeamService : Service<MailBoxTeam>, IMailBoxTeamService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailBoxTeamService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailBoxTeam> CheckInsertOrUpdate(MailBoxTeamDto model)
        {
            var mailBoxTeamObj = _mapper.Map<MailBoxTeam>(model);
            var existingItem = _unitOfWork.MailBoxTeamRepository.GetMany(t => t.CreatedBy == mailBoxTeamObj.CreatedBy && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailBoxTeam(mailBoxTeamObj);
            }
            else
            {
                return await UpdateMailBoxTeam(existingItem, existingItem.Id);
            }
        }

        public async Task<MailBoxTeam> InsertMailBoxTeam(MailBoxTeam mailBoxTeamObj)
        {
            mailBoxTeamObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailBoxTeamRepository.Add(mailBoxTeamObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailBoxTeam> UpdateMailBoxTeam(MailBoxTeam existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MailBoxTeamRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public MailBoxTeam GetByUser(int UserId)
        {
            return _unitOfWork.MailBoxTeamRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.FirstOrDefault();
        }



        public async Task<MailBoxTeam> Delete(long Id)
        {
            var mailBoxTeamObj = _unitOfWork.MailBoxTeamRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailBoxTeamObj != null)
            {
                mailBoxTeamObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailBoxTeamRepository.UpdateAsync(mailBoxTeamObj, mailBoxTeamObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<MailBoxTeam> DeleteByUser(int UserId)
        {
            var mailBoxTeamObj = _unitOfWork.MailBoxTeamRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailBoxTeamObj != null)
            {
                mailBoxTeamObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.MailBoxTeamRepository.UpdateAsync(mailBoxTeamObj, mailBoxTeamObj.Id).Result;
                await _unitOfWork.CommitAsync();
            }
            return mailBoxTeamObj;
        }
    }

    public partial interface IMailBoxTeamService : IService<MailBoxTeam>
    {
        Task<MailBoxTeam> CheckInsertOrUpdate(MailBoxTeamDto model);
        MailBoxTeam GetByUser(int UserId);
        Task<MailBoxTeam> Delete(long Id);
        Task<MailBoxTeam> DeleteByUser(int UserId);
    }
}