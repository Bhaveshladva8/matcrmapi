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
    public partial class ClientInvoiceService : Service<ClientInvoice>, IClientInvoiceService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ClientInvoiceService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientInvoice> CheckInsertOrUpdate(ClientInvoice model)
        {
            var ClientInvoiceObj = _mapper.Map<ClientInvoice>(model);
            //var existingItem = _unitOfWork.ClientInvoiceRepository.GetMany(t => t.InvoiceDate == model.InvoiceDate && t.CreatedBy == model.CreatedBy && t.DeletedOn == null).Result.Include(t => t.CreatedUser).FirstOrDefault();
            var existingItem = _unitOfWork.ClientInvoiceRepository.GetMany(t => t.InvoiceNo == model.InvoiceNo && t.InvoiceDate == model.InvoiceDate && t.CreatedBy == model.CreatedBy && t.DeletedOn == null).Result.Include(t => t.CreatedUser).FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientInvoice(ClientInvoiceObj);
            }
            else
            {
                ClientInvoiceObj.CreatedOn = existingItem.CreatedOn;
                ClientInvoiceObj.CreatedBy = existingItem.CreatedBy;
                ClientInvoiceObj.Id = existingItem.Id;
                return await UpdateClientInvoice(ClientInvoiceObj, existingItem.Id);
            }
        }

        public async Task<ClientInvoice> UpdateClientInvoice(ClientInvoice updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.ClientInvoiceRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<ClientInvoice> InsertClientInvoice(ClientInvoice ClientInvoiceObj)
        {
            ClientInvoiceObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientInvoiceRepository.AddAsync(ClientInvoiceObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<ClientInvoice> GetAll()
        {
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public List<ClientInvoice> GetAllByClient(long ClientId)
        {
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
        }


        public ClientInvoice GetById(long Id)
        {
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public List<ClientInvoice> GetByTenant(long TenantId)
        {
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.DeletedOn == null && t.CreatedUser.TenantId == TenantId).Result.Include(t => t.CreatedUser).ToList();
        }

        public ClientInvoice GetByInvoiceNo(string InvoiceNo){
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.InvoiceNo == InvoiceNo && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public async Task<ClientInvoice> DeleteClientInvoice(long Id)
        {
            var ClientInvoiceObj = _unitOfWork.ClientInvoiceRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (ClientInvoiceObj != null)
            {
                ClientInvoiceObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientInvoiceRepository.UpdateAsync(ClientInvoiceObj, ClientInvoiceObj.Id);
                await _unitOfWork.CommitAsync();            
            }
            return ClientInvoiceObj;
        }        
        public List<ClientInvoice> GetListByIdList(List<long> Ids)
        {
            return _unitOfWork.ClientInvoiceRepository.GetMany(t => t.DeletedOn == null && Ids.Any(b => t.Id == b)).Result.ToList();
        }
    }

    public partial interface IClientInvoiceService : IService<ClientInvoice>
    {
        Task<ClientInvoice> CheckInsertOrUpdate(ClientInvoice model);
        List<ClientInvoice> GetAll();
        Task<ClientInvoice> DeleteClientInvoice(long Id);
        ClientInvoice GetById(long Id);
        ClientInvoice GetByInvoiceNo(string InvoiceNo);
        List<ClientInvoice> GetByTenant(long TenantId);
        List<ClientInvoice> GetAllByClient(long ClientId);        
        List<ClientInvoice> GetListByIdList(List<long> Ids);
    }
}