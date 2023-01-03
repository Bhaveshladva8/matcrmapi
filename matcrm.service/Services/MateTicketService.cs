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
    public partial class MateTicketService : Service<MateTicket>, IMateTicketService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateTicketService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateTicket> CheckInsertOrUpdate(MateTicket mateTicketObj)
        {
            var existingItem = _unitOfWork.MateTicketRepository.GetMany(t => t.Id == mateTicketObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTicket(mateTicketObj);
            }
            else
            {
                mateTicketObj.CreatedBy = existingItem.CreatedBy;
                mateTicketObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateMateTicket(mateTicketObj, existingItem.Id);
            }
        }

        public async Task<MateTicket> InsertMateTicket(MateTicket mateTicketObj)
        {
            mateTicketObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MateTicketRepository.AddAsync(mateTicketObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTicket> UpdateMateTicket(MateTicket existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MateTicketRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<MateTicket> GetAllByTenantId(long TenantId)
        {
            return _unitOfWork.MateTicketRepository.GetMany(t => t.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).Include(t => t.Status).ToList();
        }
        public List<MateTicket> GetAllByStatusId(long TenantId, long StatusId)
        {
            return _unitOfWork.MateTicketRepository.GetMany(t => t.CreatedUser.TenantId == TenantId && t.StatusId == StatusId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).Include(t => t.Status).ToList();
        }
        public MateTicket GetById(long Id)
        {
            return _unitOfWork.MateTicketRepository.GetMany(t => t.Id== Id && t.DeletedOn == null).Result.Include(t => t.Status).Include(t => t.MateCategory).Include(t => t.MatePriority).FirstOrDefault();
        }
        public async Task<MateTicket> DeleteById(long Id)
        {
            var mateTicketObj = _unitOfWork.MateTicketRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mateTicketObj != null)
            {                
                mateTicketObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateTicketRepository.UpdateAsync(mateTicketObj, mateTicketObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateTicketObj;

        }
    }
    public partial interface IMateTicketService : IService<MateTicket>
    {
        Task<MateTicket> CheckInsertOrUpdate(MateTicket model);
        List<MateTicket> GetAllByTenantId(long TenantId);
        List<MateTicket> GetAllByStatusId(long TenantId, long StatusId);
        MateTicket GetById(long Id);
        Task<MateTicket> DeleteById(long Id);
    }
}