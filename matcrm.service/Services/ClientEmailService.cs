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
    public partial class ClientEmailService : Service<ClientEmail>, IClientEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientEmailService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientEmail> CheckInsertOrUpdate(ClientEmail clientEmailObj)
        {
            var existingItem = _unitOfWork.ClientEmailRepository.GetMany(t => t.Id == clientEmailObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientEmail(clientEmailObj);
            }
            else
            {
                clientEmailObj.CreatedBy = existingItem.CreatedBy;
                clientEmailObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClientEmail(clientEmailObj, existingItem.Id);
            }
        }

        public async Task<ClientEmail> InsertClientEmail(ClientEmail clientEmailObj)
        {
            clientEmailObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientEmailRepository.AddAsync(clientEmailObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientEmail> UpdateClientEmail(ClientEmail existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientEmailRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public async Task<List<ClientEmail>> DeleteByClientId(long clientId)
        {
            var clientEmailList = _unitOfWork.ClientEmailRepository.GetMany(u => u.ClientId == clientId && u.DeletedOn == null).Result.ToList();
            if (clientEmailList != null && clientEmailList.Count() > 0)
            {
                foreach (var existingItem in clientEmailList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientEmailRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return clientEmailList;
        }
        public List<ClientEmail> GetByClientId(long clientId)
        {
            return _unitOfWork.ClientEmailRepository.GetMany(t => t.ClientId == clientId && t.DeletedOn == null).Result.ToList();
        }

        public ClientEmail GetByClientIdWithPrimary(long clientId)
        {
            return _unitOfWork.ClientEmailRepository.GetMany(t => t.ClientId == clientId && t.IsPrimary == true && t.DeletedOn == null).Result.Include(t => t.Client).FirstOrDefault();
        }

        public async Task<ClientEmail> DeleteById(long Id)
        {
            var clientEmailObj = _unitOfWork.ClientEmailRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientEmailObj != null)
            {
                clientEmailObj.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.ClientEmailRepository.UpdateAsync(clientEmailObj, clientEmailObj.Id);

                await _unitOfWork.CommitAsync();
            }
            return clientEmailObj;
        }

        public bool IsExistOrNot(ClientEmail model)
        {
            var existingItem = _unitOfWork.ClientEmailRepository.GetMany(t => t.ClientId == model.ClientId && t.Email == model.Email && t.EmailTypeId == model.EmailTypeId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public ClientEmail GetById(long Id)
        {
            return _unitOfWork.ClientEmailRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.Client).FirstOrDefault();
        }
    }
    public partial interface IClientEmailService : IService<ClientEmail>
    {
        Task<ClientEmail> CheckInsertOrUpdate(ClientEmail model);
        Task<List<ClientEmail>> DeleteByClientId(long clientId);
        Task<ClientEmail> DeleteById(long Id);
        List<ClientEmail> GetByClientId(long clientId);
        ClientEmail GetByClientIdWithPrimary(long clientId);
        bool IsExistOrNot(ClientEmail model);
        ClientEmail GetById(long Id);
    }
}