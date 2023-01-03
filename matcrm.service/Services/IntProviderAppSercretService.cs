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
    public partial class IntProviderAppSecretService : Service<IntProviderAppSecret>, IIntProviderAppSecretService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public IntProviderAppSecretService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IntProviderAppSecret> CheckInsertOrUpdate(IntProviderAppSecretDto model, bool IsOtherMail = false)
        {
            var intProviderAppSecretObj = _mapper.Map<IntProviderAppSecret>(model);
            IntProviderAppSecret? existingItem = null;
            if (IsOtherMail == true)
            {
                existingItem = _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == intProviderAppSecretObj.CreatedBy && t.Email == model.Email && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                // var existingItem = _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.Id == obj.Id && t.IsDeleted == false).Result.FirstOrDefault();
                existingItem = _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == intProviderAppSecretObj.CreatedBy && t.Email == model.Email && t.IntProviderAppId == model.IntProviderAppId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            if (existingItem == null)
            {
                return await InsertIntProviderAppSecret(intProviderAppSecretObj);
            }
            else
            {
                existingItem.Access_Token = intProviderAppSecretObj.Access_Token;
                existingItem.Expires_In = intProviderAppSecretObj.Expires_In;
                existingItem.Token_Type = intProviderAppSecretObj.Token_Type;
                existingItem.Id_Token = intProviderAppSecretObj.Id_Token;
                return await UpdateIntProviderAppSecret(existingItem, existingItem.Id);
            }
        }

        public async Task<IntProviderAppSecret> InsertIntProviderAppSecret(IntProviderAppSecret intProviderAppSecretObj)
        {
            intProviderAppSecretObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.IntProviderAppSecretRepository.AddAsync(intProviderAppSecretObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<IntProviderAppSecret> UpdateIntProviderAppSecret(IntProviderAppSecret existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.IntProviderAppSecretRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<IntProviderAppSecret> GetAll()
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<IntProviderAppSecret> GetAllSelected(int UserId)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.IsDeleted == false && t.IsSelect == true && t.CreatedBy == UserId).Result.Include(s => s.IntProviderApp).Include(t => t.IntProviderApp.IntProvider).ToList();
        }

        public IntProviderAppSecret GetIntProviderAppSecretById(long Id)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(s => s.IntProviderApp).Include(t => t.IntProviderApp.IntProvider).FirstOrDefault();
        }

        public IntProviderAppSecret GetActiveSecretByUser(int UserId)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.IsDefault == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public IntProviderAppSecret GetActiveSecretByUserAndEmail(int UserId, string email, long appId)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.Email == email && t.IntProviderAppId == appId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public IntProviderAppSecret GetByUserAndEmail(int UserId, string Email)
        {
            // return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.Email == Email && t.IsDefault == true && t.IsDeleted == false).Result.FirstOrDefault();
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.Email == Email && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<IntProviderAppSecret> GetAllByUser(int UserId)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.IsDeleted == false).Result.Include(t => t.IntProviderApp).Include(t => t.IntProviderApp.IntProvider).ToList();
        }

        public async Task<IntProviderAppSecret> DeleteIntProviderAppSecret(long Id)
        {
            var intProviderAppSecretObj = _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (intProviderAppSecretObj != null)
            {
                intProviderAppSecretObj.IsDeleted = true;
                intProviderAppSecretObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.IntProviderAppSecretRepository.UpdateAsync(intProviderAppSecretObj, intProviderAppSecretObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return intProviderAppSecretObj;
        }

        public long GetCalendarCount(int UserId)
        {
            return _unitOfWork.IntProviderAppSecretRepository.GetMany(t => t.CreatedBy == UserId && t.IsDeleted == false && t.IntProviderApp.Name.ToLower() == "calendar").Result.Include(s => s.IntProviderApp).Count();
        }

    }

    public partial interface IIntProviderAppSecretService : IService<IntProviderAppSecret>
    {
        Task<IntProviderAppSecret> CheckInsertOrUpdate(IntProviderAppSecretDto model, bool IsOtherMail = false);
        List<IntProviderAppSecret> GetAll();
        IntProviderAppSecret GetIntProviderAppSecretById(long Id);
        Task<IntProviderAppSecret> DeleteIntProviderAppSecret(long Id);
        IntProviderAppSecret GetActiveSecretByUser(int UserId);
        IntProviderAppSecret GetActiveSecretByUserAndEmail(int UserId, string email, long appId);
        IntProviderAppSecret GetByUserAndEmail(int UserId, string Email);
        List<IntProviderAppSecret> GetAllByUser(int UserId);
        long GetCalendarCount(int UserId);
        Task<IntProviderAppSecret> UpdateIntProviderAppSecret(IntProviderAppSecret existingItem, long existingId);
        List<IntProviderAppSecret> GetAllSelected(int UserId);
    }
}