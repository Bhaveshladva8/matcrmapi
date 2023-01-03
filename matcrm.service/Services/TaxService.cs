using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaxService : Service<Tax>, ITaxService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public TaxService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<Tax> GetByTenant(int tenantId)
        {
            return _unitOfWork.TaxRepository.GetMany(t => t.TenantId == tenantId && t.DeletedOn == null).Result.ToList();
        }

        public async Task<Tax> CheckInsertOrUpdate(Tax taxObj)
        {
            var existingItem = _unitOfWork.TaxRepository.GetMany(t => t.Id == taxObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTax(taxObj);
            }
            else
            {
                taxObj.CreatedBy = existingItem.CreatedBy;
                taxObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateTax(taxObj, existingItem.Id);
            }
        }

        public async Task<Tax> InsertTax(Tax taxObj)
        {
            taxObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TaxRepository.AddAsync(taxObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Tax> UpdateTax(Tax existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.TaxRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public Tax GetById(long Id)
        {
            return _unitOfWork.TaxRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }
        public async Task<Tax> DeleteTax(long Id, int deletedby)
        {
            var taxObj = _unitOfWork.TaxRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (taxObj != null)
            {
                taxObj.DeletedBy = deletedby;
                taxObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.TaxRepository.UpdateAsync(taxObj, taxObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return taxObj;
        }
    }

    public partial interface ITaxService : IService<Tax>
    {
        List<Tax> GetByTenant(int tenantId);
        Task<Tax> CheckInsertOrUpdate(Tax model);
        Tax GetById(long Id);
        Task<Tax> DeleteTax(long Id, int deletedby);
    }
}