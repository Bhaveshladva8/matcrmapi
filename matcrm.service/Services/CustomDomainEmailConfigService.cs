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
    public partial class CustomDomainEmailConfigService : Service<CustomDomainEmailConfig>, ICustomDomainEmailConfigService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomDomainEmailConfigService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomDomainEmailConfig> CheckInsertOrUpdate(CustomDomainEmailConfigDto model)
        {
            var customDomainEmailConfigObj = _mapper.Map<CustomDomainEmailConfig>(model);
            // var existingItem = _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.Id == obj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            var existingItem = _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.CreatedBy == customDomainEmailConfigObj.CreatedBy && t.Email == model.Email && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCustomDomainEmailConfig(customDomainEmailConfigObj);
            }
            else
            {
                existingItem.Name = customDomainEmailConfigObj.Name;
                existingItem.IMAPHost = customDomainEmailConfigObj.IMAPHost;
                existingItem.IMAPPort = customDomainEmailConfigObj.IMAPPort;
                existingItem.SMTPHost = customDomainEmailConfigObj.SMTPHost;
                existingItem.SMTPPort = customDomainEmailConfigObj.SMTPPort;
                return await UpdateCustomDomainEmailConfig(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomDomainEmailConfig> InsertCustomDomainEmailConfig(CustomDomainEmailConfig customDomainEmailConfigObj)
        {
            customDomainEmailConfigObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomDomainEmailConfigRepository.AddAsync(customDomainEmailConfigObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomDomainEmailConfig> UpdateCustomDomainEmailConfig(CustomDomainEmailConfig existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomDomainEmailConfigRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CustomDomainEmailConfig> GetAll()
        {
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public List<CustomDomainEmailConfig> GetByUser(int UserId)
        {
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.DeletedOn == null && t.CreatedBy == UserId).Result.Include(s => s.IntProviderAppSecret).ToList();
        }

        public CustomDomainEmailConfig GetCustomDomainEmailConfigById(long Id)
        {
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public CustomDomainEmailConfig GetByUserAndEmail(int UserId, string Email)
        {
            // return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.CreatedBy == UserId && t.Email == Email && t.IsDefault == true && t.DeletedOn == null).Result.FirstOrDefault();
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.CreatedBy == UserId && t.Email == Email && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public List<CustomDomainEmailConfig> GetAllByUser(int UserId)
        {
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.ToList();
        }

        public CustomDomainEmailConfig DeleteCustomDomainEmailConfig(long Id)
        {
            var customDomainEmailConfigObj = _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if(customDomainEmailConfigObj != null)
            {
                customDomainEmailConfigObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.CustomDomainEmailConfigRepository.UpdateAsync(customDomainEmailConfigObj, customDomainEmailConfigObj.Id);
                _unitOfWork.CommitAsync();
            }
            return customDomainEmailConfigObj;
        }

        public async Task<CustomDomainEmailConfig> DeleteByIntProviderAppSecretId(long AppSecretId)
        {
            var customDomainEmailConfigObj = _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.IntProviderAppSecretId == AppSecretId && t.DeletedOn == null).Result.FirstOrDefault();
            if (customDomainEmailConfigObj != null)
            {
                customDomainEmailConfigObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.CustomDomainEmailConfigRepository.UpdateAsync(customDomainEmailConfigObj, customDomainEmailConfigObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return customDomainEmailConfigObj;
        }

        public long GetCalendarCount(int UserId)
        {
            return _unitOfWork.CustomDomainEmailConfigRepository.GetMany(t => t.CreatedBy == UserId && t.DeletedOn == null).Result.Count();
        }

    }

    public partial interface ICustomDomainEmailConfigService : IService<CustomDomainEmailConfig>
    {
        Task<CustomDomainEmailConfig> CheckInsertOrUpdate(CustomDomainEmailConfigDto model);
        List<CustomDomainEmailConfig> GetAll();
        List<CustomDomainEmailConfig> GetByUser(int UserId);
        CustomDomainEmailConfig GetCustomDomainEmailConfigById(long Id);
        CustomDomainEmailConfig DeleteCustomDomainEmailConfig(long Id);
        Task<CustomDomainEmailConfig> DeleteByIntProviderAppSecretId(long AppSecretId);
        CustomDomainEmailConfig GetByUserAndEmail(int UserId, string Email);
        List<CustomDomainEmailConfig> GetAllByUser(int UserId);
        long GetCalendarCount(int UserId);
        Task<CustomDomainEmailConfig> UpdateCustomDomainEmailConfig(CustomDomainEmailConfig existingItem, long existingId);
    }
}