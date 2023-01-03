using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class InvoiceIntervalService : Service<InvoiceInterval>, IInvoiceIntervalService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceIntervalService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceInterval> CheckInsertOrUpdate(InvoiceInterval model, int TenantId)
        {
            var InvoiceIntervalObj = _mapper.Map<InvoiceInterval>(model);
            var existingItem = _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.Name == InvoiceIntervalObj.Name && t.Interval == InvoiceIntervalObj.Interval && t.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertInvoiceInterval(InvoiceIntervalObj);
            }
            else
            {
                InvoiceIntervalObj.CreatedOn = existingItem.CreatedOn;
                InvoiceIntervalObj.CreatedBy = existingItem.CreatedBy;
                InvoiceIntervalObj.Id = existingItem.Id;
                return await UpdateInvoiceInterval(InvoiceIntervalObj, existingItem.Id);
            }
        }

        public async Task<InvoiceInterval> UpdateInvoiceInterval(InvoiceInterval updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.InvoiceIntervalRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<InvoiceInterval> InsertInvoiceInterval(InvoiceInterval InvoiceIntervalObj)
        {
            InvoiceIntervalObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.InvoiceIntervalRepository.AddAsync(InvoiceIntervalObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<InvoiceInterval> GetAll()
        {
            return _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public InvoiceInterval GetByName(string Name)
        {
            return _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.DeletedOn == null && t.Name == Name).Result.FirstOrDefault();
        }

        public InvoiceInterval GetById(long Id)
        {
            return _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public List<InvoiceInterval> GetByTenant(long TenantId)
        {
            return _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.DeletedOn == null && (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId)).Result.Include(t => t.CreatedUser).ToList();
        }

        public async Task<InvoiceInterval> DeleteInvoiceInterval(long Id)
        {
            var InvoiceIntervalObj = _unitOfWork.InvoiceIntervalRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (InvoiceIntervalObj != null)
            {
                InvoiceIntervalObj.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.InvoiceIntervalRepository.UpdateAsync(InvoiceIntervalObj, InvoiceIntervalObj.Id);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IInvoiceIntervalService : IService<InvoiceInterval>
    {
        Task<InvoiceInterval> CheckInsertOrUpdate(InvoiceInterval model, int TenantId);
        List<InvoiceInterval> GetAll();
        Task<InvoiceInterval> DeleteInvoiceInterval(long Id);
        InvoiceInterval GetById(long Id);
        InvoiceInterval GetByName(string Name);
        List<InvoiceInterval> GetByTenant(long TenantId);
    }
}