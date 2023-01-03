using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaxRateService : Service<TaxRate>, ITaxRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public TaxRateService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<TaxRate> CheckInsertOrUpdate(TaxRate taxRateObj)
        {
            TaxRate? existingItem = null;

            if(taxRateObj.Id != null && taxRateObj.Id > 0)
            {
                existingItem = _unitOfWork.TaxRateRepository.GetMany(t => t.Id == taxRateObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.TaxRateRepository.GetMany(t => t.TaxType.ToLower() == taxRateObj.TaxType.ToLower() && t.TenantId == taxRateObj.TenantId && t.TaxId == taxRateObj.TaxId && t.Percentage == taxRateObj.Percentage && t.DeletedOn == null).Result.FirstOrDefault();
            }
            if (existingItem == null)
            {
                return await InsertTaxRate(taxRateObj);
            }
            else
            {                
                taxRateObj.CreatedBy = existingItem.CreatedBy;
                taxRateObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateTaxRate(taxRateObj, existingItem.Id);
            }

            // var existingItem = _unitOfWork.TaxRateRepository.GetMany(t => t.Id == taxRateObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            // if (existingItem == null)
            // {
            //     return await InsertTaxRate(taxRateObj);
            // }
            // else
            // {
            //     taxRateObj.CreatedBy = existingItem.CreatedBy;
            //     taxRateObj.CreatedOn = existingItem.CreatedOn;
            //     return await UpdateTaxRate(taxRateObj, existingItem.Id);
            // }
        }

        public async Task<TaxRate> InsertTaxRate(TaxRate taxRateObj)
        {
            taxRateObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.TaxRateRepository.AddAsync(taxRateObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<TaxRate> UpdateTaxRate(TaxRate existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.TaxRateRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<TaxRate> GetByTaxId(long Id)
        {
            return _unitOfWork.TaxRateRepository.GetMany(t => t.DeletedOn == null && t.TaxId == Id).Result.ToList();
        }
        public async Task<List<TaxRate>> DeleteByTaxId(long TaxId, int deletedby)
        {
            var taxRateList = _unitOfWork.TaxRateRepository.GetMany(t => t.TaxId == TaxId && t.DeletedOn == null).Result.ToList();
            if (taxRateList != null && taxRateList.Count() > 0)
            {
                foreach (var existingItem in taxRateList)
                {
                    existingItem.DeletedBy = deletedby;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.TaxRateRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return taxRateList;
        }

        public async Task<TaxRate> DeleteTaxRate(long Id,int deletedby)
        {
            var taxRateObj = _unitOfWork.TaxRateRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (taxRateObj != null)
            {   
                taxRateObj.DeletedBy = deletedby;             
                taxRateObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.TaxRateRepository.UpdateAsync(taxRateObj, taxRateObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return taxRateObj;
        }
    }
    public partial interface ITaxRateService : IService<TaxRate>
    {
        Task<TaxRate> CheckInsertOrUpdate(TaxRate model);
        List<TaxRate> GetByTaxId(long Id);
        Task<TaxRate> DeleteTaxRate(long Id,int deletedby);
        Task<List<TaxRate>> DeleteByTaxId(long TaxId, int deletedby);
    }
}