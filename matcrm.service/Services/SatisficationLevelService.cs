using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class SatisficationLevelService : Service<SatisficationLevel>, ISatisficationLevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public SatisficationLevelService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<SatisficationLevel> CheckInsertOrUpdate(SatisficationLevel satisficationLevelObj)
        {
            var existingItem = _unitOfWork.SatisficationLevelRepository.GetMany(t => t.Id == satisficationLevelObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSatisficationLevel(satisficationLevelObj);
            }
            else
            {
                satisficationLevelObj.CreatedBy = existingItem.CreatedBy;
                satisficationLevelObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateSatisficationLevel(satisficationLevelObj, existingItem.Id);
            }
        }
        public async Task<SatisficationLevel> InsertSatisficationLevel(SatisficationLevel satisficationLevelObj)
        {
            satisficationLevelObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SatisficationLevelRepository.AddAsync(satisficationLevelObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<SatisficationLevel> UpdateSatisficationLevel(SatisficationLevel existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.SatisficationLevelRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<SatisficationLevel> DeleteById(int Id)
        {
            var satisficationLevelObj = _unitOfWork.SatisficationLevelRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (satisficationLevelObj != null)
            {
                satisficationLevelObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.SatisficationLevelRepository.UpdateAsync(satisficationLevelObj, satisficationLevelObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return satisficationLevelObj;
        }
        public List<SatisficationLevel> GetAll()
        {
            return _unitOfWork.SatisficationLevelRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public SatisficationLevel GetById(long Id)
        {
            return _unitOfWork.SatisficationLevelRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<SatisficationLevel> GetByTenant(long TenantId)
        {
            return _unitOfWork.SatisficationLevelRepository.GetMany(t => t.DeletedOn == null && (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId)).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface ISatisficationLevelService : IService<SatisficationLevel>
    {
        Task<SatisficationLevel> CheckInsertOrUpdate(SatisficationLevel model);
        Task<SatisficationLevel> DeleteById(int Id);
        List<SatisficationLevel> GetAll();
        SatisficationLevel GetById(long Id);
        List<SatisficationLevel> GetByTenant(long TenantId);
    }
}