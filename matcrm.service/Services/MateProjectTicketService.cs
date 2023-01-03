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
    public partial class MateProjectTicketService : Service<MateProjectTicket>, IMateProjectTicketService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MateProjectTicketService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateProjectTicket> CheckInsertOrUpdate(MateProjectTicket MateProjectTicketObj)
        {
            //MateProjectTicket? existingItem = null;
            var existingItem = _unitOfWork.MateProjectTicketRepository.GetMany(t => t.MateTicketId == MateProjectTicketObj.MateTicketId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateProjectTicket(MateProjectTicketObj);
            }
            else
            {
                MateProjectTicketObj.Id = existingItem.Id;
                return await UpdateMateProjectTicket(MateProjectTicketObj, existingItem.Id);
            }
        }
        public async Task<MateProjectTicket> InsertMateProjectTicket(MateProjectTicket MateProjectTicketObj)
        {
            var newItem = await _unitOfWork.MateProjectTicketRepository.AddAsync(MateProjectTicketObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateProjectTicket> UpdateMateProjectTicket(MateProjectTicket existingItem, long existingId)
        {
            await _unitOfWork.MateProjectTicketRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<MateProjectTicket> GetAllByProjectId(long EmployeeProjectId, long TenantId)
        {
            return _unitOfWork.MateProjectTicketRepository.GetMany(t => t.EmployeeProjectId == EmployeeProjectId && t.MateTicket.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.MateTicket).Include(t => t.EmployeeProject).Include(t => t.MateTicket.CreatedUser).ToList();
        }
        public MateProjectTicket GetByTicketId(long TicketId)
        {
            return _unitOfWork.MateProjectTicketRepository.GetMany(t => t.MateTicketId == TicketId && t.DeletedOn == null).Result.Include(t => t.MateTicket).Include(t => t.EmployeeProject).FirstOrDefault();
        }
        public List<MateProjectTicket> GetAllByTenantId(long TenantId)
        {
            return _unitOfWork.MateProjectTicketRepository.GetMany(t => t.MateTicket.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.MateTicket).Include(t => t.MateTicket.CreatedUser).ToList();
        }
        public async Task<MateProjectTicket> DeleteByTicketId(long TicketId)
        {
            var mateProjectTicketObj = _unitOfWork.MateProjectTicketRepository.GetMany(u => u.MateTicketId == TicketId && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateProjectTicketObj != null)
            {
                mateProjectTicketObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateProjectTicketRepository.UpdateAsync(mateProjectTicketObj, mateProjectTicketObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateProjectTicketObj;
        }
        public async Task<MateProjectTicket> DeleteById(long Id)
        {
            var mateProjectTicketObj = _unitOfWork.MateProjectTicketRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateProjectTicketObj != null)
            {
                mateProjectTicketObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateProjectTicketRepository.UpdateAsync(mateProjectTicketObj, mateProjectTicketObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateProjectTicketObj;
        }
    }
    public partial interface IMateProjectTicketService : IService<MateProjectTicket>
    {
        Task<MateProjectTicket> CheckInsertOrUpdate(MateProjectTicket model);
        List<MateProjectTicket> GetAllByProjectId(long EmployeeProjectId, long TenantId);
        MateProjectTicket GetByTicketId(long TicketId);
        List<MateProjectTicket> GetAllByTenantId(long TenantId);
        Task<MateProjectTicket> DeleteByTicketId(long TicketId);
        Task<MateProjectTicket> DeleteById(long Id);        
    }
}