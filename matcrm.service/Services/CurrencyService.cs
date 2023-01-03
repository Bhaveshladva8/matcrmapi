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
    public partial class CurrencyService : Service<Currency>, ICurrencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CurrencyService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Currency> CheckInsertOrUpdate(Currency currencyObj)
        {            
            var existingItem = _unitOfWork.CurrencyRepository.GetMany(t => t.Id == currencyObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCurrency(currencyObj);
            }
            else
            {
                currencyObj.CreatedBy = existingItem.CreatedBy;
                currencyObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateCurrency(currencyObj, existingItem.Id);
            }
        }

        public async Task<Currency> InsertCurrency(Currency currencyObj)
        {
            currencyObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CurrencyRepository.AddAsync(currencyObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Currency> UpdateCurrency(Currency existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CurrencyRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<Currency> GetByTenant(int tenantId)
        {
            return _unitOfWork.CurrencyRepository.GetMany(t => (t.CreatedBy == null || t.TenantId == tenantId) && t.DeletedOn == null).Result.ToList();
        }

        public List<Currency> GetAll()
        {
            return _unitOfWork.CurrencyRepository.GetMany(t =>t.DeletedOn == null).Result.ToList();
        }
        public Currency GetById(long Id)
        {
            return _unitOfWork.CurrencyRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public async Task<Currency> DeleteCurrency(long Id,int deletedby)
        {
            var currencyObj = _unitOfWork.CurrencyRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (currencyObj != null)
            {   
                currencyObj.DeletedBy = deletedby;             
                currencyObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CurrencyRepository.UpdateAsync(currencyObj, currencyObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return currencyObj;
        }
        
    }

    public partial interface ICurrencyService : IService<Currency>
    {
        Task<Currency> CheckInsertOrUpdate(Currency model);
        List<Currency> GetByTenant(int tenantId);
        Currency GetById(long Id);
        Task<Currency> DeleteCurrency(long Id,int deletedby);
        List<Currency> GetAll();
    }
}