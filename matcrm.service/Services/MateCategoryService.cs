using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using matcrm.data.Models.Tables;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data.Models.Dto;

namespace matcrm.service.Services
{
    public partial class MateCategoryService : Service<MateCategory>, IMateCategoryService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MateCategoryService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateCategory> CheckInsertOrUpdate(MateCategoryDto mateCategoryDtoObj, long TenantId)
        {
            var mateCategoryObj = _mapper.Map<MateCategory>(mateCategoryDtoObj);
            MateCategory? existingItem = null;
            if (mateCategoryObj.Id != null && mateCategoryObj.Id > 0)
            {
                existingItem = _unitOfWork.MateCategoryRepository.GetMany(t => t.Id == mateCategoryObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.MateCategoryRepository.GetMany(t => t.Name.ToLower() == mateCategoryObj.Name.ToLower() && t.CreatedUser.TenantId == TenantId && t.CustomTableId == mateCategoryObj.CustomTableId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).FirstOrDefault();
            }
            //var existingItem = _unitOfWork.MateCategoryRepository.GetMany(t => t.Id == mateCategoryObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateCategory(mateCategoryObj);
            }
            else
            {
                mateCategoryObj.Id = existingItem.Id;
                mateCategoryObj.CreatedBy = existingItem.CreatedBy;
                mateCategoryObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateMateCategory(mateCategoryObj, existingItem.Id);
            }
        }
        public async Task<MateCategory> InsertMateCategory(MateCategory mateCategoryObj)
        {
            mateCategoryObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MateCategoryRepository.AddAsync(mateCategoryObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateCategory> UpdateMateCategory(MateCategory existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MateCategoryRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<MateCategory> DeleteById(long Id)
        {
            var mateCategoryObj = _unitOfWork.MateCategoryRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateCategoryObj != null)
            {
                mateCategoryObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateCategoryRepository.UpdateAsync(mateCategoryObj, mateCategoryObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateCategoryObj;
        }
        public List<MateCategory> GetAll()
        {
            return _unitOfWork.MateCategoryRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public MateCategory GetById(long Id)
        {
            return _unitOfWork.MateCategoryRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<MateCategory> GetByTenant(long TenantId)
        {
            return _unitOfWork.MateCategoryRepository.GetMany(t => t.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.CustomTable).ToList();
        }
        public List<MateCategory> GetByCustomTableName(long customtableId, long TenantId)
        {
            return _unitOfWork.MateCategoryRepository.GetMany(t => t.DeletedOn == null && (t.CustomTableId == customtableId || t.CustomTableId == null) && (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId)).Result.Include(t => t.CreatedUser).ToList();
            //return _unitOfWork.MateCategoryRepository.GetMany(t => t.DeletedOn == null && t.CustomTableId == customtableId && t.CreatedUser.TenantId == TenantId).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IMateCategoryService : IService<MateCategory>
    {
        Task<MateCategory> CheckInsertOrUpdate(MateCategoryDto mateCategoryDtoObj, long TenantId);
        Task<MateCategory> DeleteById(long Id);
        List<MateCategory> GetAll();
        MateCategory GetById(long Id);
        List<MateCategory> GetByTenant(long TenantId);
        List<MateCategory> GetByCustomTableName(long customtableId, long TenantId);
    }
}