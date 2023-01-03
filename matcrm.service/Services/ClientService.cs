using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ClientService : Service<matcrm.data.Models.Tables.Client>, IClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<matcrm.data.Models.Tables.Client> CheckInsertOrUpdate(matcrm.data.Models.Tables.Client clientObj)
        {
            var existingItem = _unitOfWork.ClientRepository.GetMany(t => t.Id == clientObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClient(clientObj);
            }
            else
            {
                clientObj.CreatedBy = existingItem.CreatedBy;
                clientObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClient(clientObj, existingItem.Id);
            }
        }

        public async Task<matcrm.data.Models.Tables.Client> InsertClient(matcrm.data.Models.Tables.Client clientObj)
        {
            clientObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientRepository.AddAsync(clientObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<matcrm.data.Models.Tables.Client> UpdateClient(matcrm.data.Models.Tables.Client existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public matcrm.data.Models.Tables.Client GetById(long Id)
        {
            return _unitOfWork.ClientRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.Country).Include(t => t.State).Include(t => t.City).Include(t => t.StandardTimeZone).Include(t => t.InvoiceInterval).FirstOrDefault();
        }
        public List<matcrm.data.Models.Tables.Client> GetByTenant(int tenantId)
        {
            return _unitOfWork.ClientRepository.GetMany(t => t.TenantId == tenantId && t.DeletedOn == null).Result.ToList();
        }

        public List<matcrm.data.Models.Tables.Client> GetAll()
        {
            return _unitOfWork.ClientRepository.GetMany(t => t.DeletedOn == null).Result.Include(t => t.InvoiceInterval).ToList();
        }
        public async Task<matcrm.data.Models.Tables.Client> DeleteClient(long Id)
        {
            var clientObj = _unitOfWork.ClientRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientObj != null)
            {
                clientObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientRepository.UpdateAsync(clientObj, clientObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return clientObj;
        }
    }

    public partial interface IClientService : IService<matcrm.data.Models.Tables.Client>
    {
        Task<matcrm.data.Models.Tables.Client> CheckInsertOrUpdate(matcrm.data.Models.Tables.Client clientObj);
        matcrm.data.Models.Tables.Client GetById(long Id);
        List<matcrm.data.Models.Tables.Client> GetByTenant(int tenantId);
        Task<matcrm.data.Models.Tables.Client> DeleteClient(long Id);
        List<matcrm.data.Models.Tables.Client> GetAll();
    }
}