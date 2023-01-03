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
    public partial class ExternalUserService : Service<ExternalUser>, IExternalUserService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ExternalUser> CheckInsertOrUpdate(ExternalUserDto model)
        {
            var externalUserObj = _mapper.Map<ExternalUser>(model);
            // var existingItem = _unitOfWork.ExternalUserRepository.GetMany(t => t.Id == obj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            var existingItem = _unitOfWork.ExternalUserRepository.GetMany(t => t.UserId == externalUserObj.UserId && t.Email == externalUserObj.Email && t.IntProviderId == externalUserObj.IntProviderId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertExternalUser(externalUserObj);
            }
            else
            {
                existingItem.ExpiredOn = externalUserObj.ExpiredOn;
                existingItem.Token_Type = externalUserObj.Token_Type;
                existingItem.Id_Token = externalUserObj.Id_Token;
                existingItem.FirstName = externalUserObj.FirstName;
                existingItem.LastName = externalUserObj.LastName;
                return await UpdateExternalUser(existingItem, existingItem.Id);
            }
        }

        public async Task<ExternalUser> InsertExternalUser(ExternalUser externalUserObj)
        {
            externalUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ExternalUserRepository.AddAsync(externalUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ExternalUser> UpdateExternalUser(ExternalUser existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ExternalUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ExternalUser> GetAll()
        {
            return _unitOfWork.ExternalUserRepository.GetMany(t => t.DeletedOn != null).Result.Include(m => m.IntProvider).ToList();
        }


        public ExternalUser GetExternalUserById(long Id)
        {
            return _unitOfWork.ExternalUserRepository.GetMany(t => t.Id == Id && t.DeletedOn != null).Result.Include(m => m.IntProvider).FirstOrDefault();
        }

        public ExternalUser GetActiveSecretByUser(int UserId)
        {
            return _unitOfWork.ExternalUserRepository.GetMany(t => t.UserId == UserId && t.DeletedOn != null).Result.Include(m => m.IntProvider).FirstOrDefault();
        }

        public ExternalUser GetByUserAndEmail(int UserId, string Email, long intProviderId)
        {
            return _unitOfWork.ExternalUserRepository.GetMany(t => t.UserId == UserId && t.Email == Email && t.IntProviderId == intProviderId && t.DeletedOn != null).Result.Include(m => m.IntProvider).FirstOrDefault();
        }

        public List<ExternalUser> GetAllByUser(int UserId)
        {
            return _unitOfWork.ExternalUserRepository.GetMany(t => t.UserId == UserId && t.DeletedOn != null).Result.Include(m => m.IntProvider).ToList();
        }

        public ExternalUser DeleteExternalUser(long Id)
        {
            var externalUserObj = _unitOfWork.ExternalUserRepository.GetMany(t => t.Id == Id && t.DeletedOn != null).Result.FirstOrDefault();
            if (externalUserObj != null)
            {
                externalUserObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.ExternalUserRepository.UpdateAsync(externalUserObj, externalUserObj.Id);
                _unitOfWork.CommitAsync();
            }
            return externalUserObj;
        }
    }

    public partial interface IExternalUserService : IService<ExternalUser>
    {
        Task<ExternalUser> CheckInsertOrUpdate(ExternalUserDto model);
        List<ExternalUser> GetAll();
        ExternalUser GetExternalUserById(long Id);
        ExternalUser DeleteExternalUser(long Id);
        ExternalUser GetActiveSecretByUser(int UserId);
        ExternalUser GetByUserAndEmail(int UserId, string Email, long intProviderId);
        List<ExternalUser> GetAllByUser(int UserId);
        Task<ExternalUser> UpdateExternalUser(ExternalUser existingItem, long existingId);
    }
}