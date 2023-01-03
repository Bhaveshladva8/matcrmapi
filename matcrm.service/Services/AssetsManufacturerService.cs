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
    public partial class AssetsManufacturerService : Service<AssetsManufacturer>, IAssetsManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public AssetsManufacturerService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AssetsManufacturer> CheckInsertOrUpdate(AssetsManufacturer AssetsManufacturerObj)
        {
            var existingItem = _unitOfWork.AssetsManufacturerRepository.GetMany(t => t.Id == AssetsManufacturerObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertAssetsManufacturer(AssetsManufacturerObj);
            }
            else
            {
                AssetsManufacturerObj.CreatedOn = existingItem.CreatedOn;
                AssetsManufacturerObj.CreatedBy = existingItem.CreatedBy;
                return await UpdateAssetsManufacturer(AssetsManufacturerObj, existingItem.Id);
            }
        }
        public async Task<AssetsManufacturer> InsertAssetsManufacturer(AssetsManufacturer AssetsManufacturerObj)
        {
            AssetsManufacturerObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.AssetsManufacturerRepository.AddAsync(AssetsManufacturerObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public async Task<AssetsManufacturer> UpdateAssetsManufacturer(AssetsManufacturer updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.AssetsManufacturerRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }
        public List<AssetsManufacturer> GetAll()
        {
            return _unitOfWork.AssetsManufacturerRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public AssetsManufacturer GetById(long Id)
        {
            return _unitOfWork.AssetsManufacturerRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public async Task<AssetsManufacturer> DeleteById(long Id)
        {
            var existingItem = _unitOfWork.AssetsManufacturerRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.AssetsManufacturerRepository.UpdateAsync(existingItem, existingItem.Id);
                await _unitOfWork.CommitAsync();
            }
            return existingItem;
        }
        public List<AssetsManufacturer> GetByTenant(long TenantId)
        {
            return _unitOfWork.AssetsManufacturerRepository.GetMany(t => (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId) && t.DeletedOn == null).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IAssetsManufacturerService : IService<AssetsManufacturer>
    {
        Task<AssetsManufacturer> CheckInsertOrUpdate(AssetsManufacturer model);
        List<AssetsManufacturer> GetAll();
        AssetsManufacturer GetById(long Id);
        Task<AssetsManufacturer> DeleteById(long Id);
        List<AssetsManufacturer> GetByTenant(long TenantId);
    }
}