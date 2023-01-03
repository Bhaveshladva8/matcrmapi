using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using matcrm.data.Models.Tables;
using matcrm.data.Models.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class MatePriorityService : Service<MatePriority>, IMatePriorityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MatePriorityService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public List<MatePriority> GetByTenant(int TenantId)
        {
            return _unitOfWork.MatePriorityRepository.GetMany(t => t.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.CustomTable).Include(t => t.CreatedUser).ToList();
        }
        public MatePriority GetById(long Id)
        {
            return _unitOfWork.MatePriorityRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }
        public async Task<MatePriority> CheckInsertOrUpdate(MatePriorityDto model)
        {
            var matePriorityObj = _mapper.Map<MatePriority>(model);

            MatePriority? existingItem = null;

            if (matePriorityObj.Id != null && matePriorityObj.Id > 0)
            {
                existingItem = _unitOfWork.MatePriorityRepository.GetMany(t => t.Id == matePriorityObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.MatePriorityRepository.GetMany(t => t.Name.ToLower() == model.Name.ToLower() && t.CreatedUser.TenantId == model.TenantId && t.CustomTableId == model.CustomTableId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).FirstOrDefault();
            }
            if (existingItem == null)
            {
                return await InsertMatePriority(matePriorityObj);
            }
            else
            {
                matePriorityObj.Id = existingItem.Id;
                matePriorityObj.CreatedBy = existingItem.CreatedBy;
                matePriorityObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateMatePriority(matePriorityObj, existingItem.Id);
            }
        }
        public async Task<MatePriority> InsertMatePriority(MatePriority matePriorityObj)
        {
            matePriorityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MatePriorityRepository.AddAsync(matePriorityObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public async Task<MatePriority> UpdateMatePriority(MatePriority existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MatePriorityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public async Task<MatePriority> DeleteById(long Id)
        {
            var matePriorityObj = _unitOfWork.MatePriorityRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (matePriorityObj != null)
            {
                matePriorityObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MatePriorityRepository.UpdateAsync(matePriorityObj, matePriorityObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return matePriorityObj;
        }

        public List<MatePriority> GetByCustomTableId(long customtableId, long TenantId)
        {
            return _unitOfWork.MatePriorityRepository.GetMany(t => t.DeletedOn == null && (t.CustomTableId == customtableId || t.CustomTableId == null) && (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId)).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IMatePriorityService : IService<MatePriority>
    {
        List<MatePriority> GetByTenant(int tenantId);
        MatePriority GetById(long Id);
        Task<MatePriority> CheckInsertOrUpdate(MatePriorityDto model);
        Task<MatePriority> DeleteById(long Id);
        List<MatePriority> GetByCustomTableId(long customtableId, long TenantId);
    }
}