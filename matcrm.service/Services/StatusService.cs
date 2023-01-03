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
    public partial class StatusService : Service<Status>, IStatusService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public StatusService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<Status> GetByTenant(int tenantId)
        {
            return _unitOfWork.StatusRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.Include(t => t.CustomTable).ToList();
        }

        public Status GetById(long Id)
        {
            return _unitOfWork.StatusRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public async Task<Status> CheckInsertOrUpdate(StatusDto model)
        {
            var statusObj = _mapper.Map<Status>(model);

            Status? existingItem = null;

            if (statusObj.Id != null && statusObj.Id > 0)
            {
                existingItem = _unitOfWork.StatusRepository.GetMany(t => t.Id == statusObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.StatusRepository.GetMany(t => t.Name.ToLower() == model.Name.ToLower() && t.TenantId == model.TenantId && t.CustomTableId == model.CustomTableId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            //var existingItem = _unitOfWork.StatusRepository.GetMany(t => t.Id == statusObj.Id && t.TenantId == statusObj.TenantId.Value && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertStatus(statusObj);
            }
            else
            {
                statusObj.Id = existingItem.Id;
                statusObj.CreatedBy = existingItem.CreatedBy;
                statusObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateStatus(statusObj, existingItem.Id);
            }
        }

        public async Task<Status> InsertStatus(Status statusObj)
        {
            statusObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.StatusRepository.AddAsync(statusObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public async Task<Status> UpdateStatus(Status existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.StatusRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public async Task<Status> DeleteStatus(long Id)
        {
            var statusObj = _unitOfWork.StatusRepository.GetMany(u => u.Id == Id && u.IsDeleted == false).Result.FirstOrDefault();
            if (statusObj != null)
            {
                statusObj.IsDeleted = true;
                statusObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.StatusRepository.UpdateAsync(statusObj, statusObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return statusObj;
        }

        // public Status CheckExistOrNot(StatusDto model){
        //     return _unitOfWork.StatusRepository.GetMany(t => t.Name.ToLower() == model.Name.ToLower() && t.TenantId == model.TenantId && t.CustomTableId == model.CustomTableId && t.IsDeleted == false).Result.FirstOrDefault();            
        // }

        public bool CheckExistOrNot(StatusDto model)
        {
            var existingItem = _unitOfWork.StatusRepository.GetMany(t => t.Name.ToLower() == model.Name.ToLower() && t.TenantId == model.TenantId && t.CustomTableId == model.CustomTableId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public List<Status> GetByCustomTableName(long customtableId, long TenantId)
        {
            //return _unitOfWork.StatusRepository.GetMany(t => t.IsDeleted == false && t.CustomTableId == customtableId && t.TenantId == TenantId).Result.ToList();
            return _unitOfWork.StatusRepository.GetMany(t => t.DeletedOn == null && (t.CustomTableId == customtableId || t.CustomTableId == null) && (t.CreatedBy == null || t.TenantId == TenantId)).Result.ToList();
        }
        public List<Status> GetAll()
        {
            //return _unitOfWork.StatusRepository.GetMany(t => t.IsDeleted == false && t.CustomTableId == customtableId && t.TenantId == TenantId).Result.ToList();
            return _unitOfWork.StatusRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

    }

    public partial interface IStatusService : IService<Status>
    {
        List<Status> GetByTenant(int tenantId);
        Status GetById(long Id);
        List<Status> GetAll();
        Task<Status> CheckInsertOrUpdate(StatusDto model);
        // Task<Status> DeleteStatus(StatusDto model);
        Task<Status> DeleteStatus(long Id);
        //Status CheckExistOrNot(StatusDto model);
        bool CheckExistOrNot(StatusDto model);
        List<Status> GetByCustomTableName(long customtableId, long TenantId);
    }
}