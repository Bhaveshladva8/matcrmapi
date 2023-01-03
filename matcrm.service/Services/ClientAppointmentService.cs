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
    public partial class ClientAppointmentService : Service<ClientAppointment>, IClientAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ClientAppointmentService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ClientAppointment> CheckInsertOrUpdate(ClientAppointment clientAppointmentObj)
        {
            var existingItem = _unitOfWork.ClientAppointmentRepository.GetMany(t => t.Id == clientAppointmentObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertClientAppointment(clientAppointmentObj);
            }
            else
            {
                clientAppointmentObj.CreatedBy = existingItem.CreatedBy;
                clientAppointmentObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateClientAppointment(clientAppointmentObj, existingItem.Id);
            }
        }
        public async Task<ClientAppointment> InsertClientAppointment(ClientAppointment clientAppointmentObj)
        {
            clientAppointmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ClientAppointmentRepository.AddAsync(clientAppointmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ClientAppointment> UpdateClientAppointment(ClientAppointment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ClientAppointmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<ClientAppointment> DeleteById(int Id)
        {
            var clientAppointmentObj = _unitOfWork.ClientAppointmentRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (clientAppointmentObj != null)
            {
                clientAppointmentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ClientAppointmentRepository.UpdateAsync(clientAppointmentObj, clientAppointmentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return clientAppointmentObj;
        }
        public List<ClientAppointment> GetAllByClientId(long ClientId)
        {
            return _unitOfWork.ClientAppointmentRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.Include(t => t.ClientUser).Include(t => t.Status).ToList();
        }
        public ClientAppointment GetById(long Id)
        {
            return _unitOfWork.ClientAppointmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.ClientUser).Include(t => t.Status).FirstOrDefault();
        }
        public async Task<List<ClientAppointment>> DeleteByClientId(long ClientId)
        {
            var clientAppointmentList = _unitOfWork.ClientAppointmentRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (clientAppointmentList != null && clientAppointmentList.Count() > 0)
            {
                foreach (var existingItem in clientAppointmentList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.ClientAppointmentRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return clientAppointmentList;
        }
        public List<ClientAppointment> GetAllByClientUserId(long ClientUserId)
        {
            return _unitOfWork.ClientAppointmentRepository.GetMany(t => t.DeletedOn == null && t.ClientUserId == ClientUserId).Result.Include(t => t.ClientUser).Include(t => t.Status).ToList();
        }
    }
    public partial interface IClientAppointmentService : IService<ClientAppointment>
    {
        Task<ClientAppointment> CheckInsertOrUpdate(ClientAppointment model);
        Task<ClientAppointment> DeleteById(int Id);
        List<ClientAppointment> GetAllByClientId(long ClientId);
        ClientAppointment GetById(long Id);
        Task<List<ClientAppointment>> DeleteByClientId(long ClientId);
        List<ClientAppointment> GetAllByClientUserId(long ClientUserId);

    }
}