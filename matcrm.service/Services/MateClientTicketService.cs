using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using matcrm.data.Models.Tables;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
namespace matcrm.service.Services
{
    public partial class MateClientTicketService : Service<MateClientTicket>, IMateClientTicketService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MateClientTicketService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateClientTicket> CheckInsertOrUpdate(MateClientTicket mateClientTicketObj)
        {
            //MateClientTicket? existingItem = null;
            var existingItem = _unitOfWork.MateClientTicketRepository.GetMany(t => t.MateTicketId == mateClientTicketObj.MateTicketId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateClientTicket(mateClientTicketObj);
            }
            else
            {
                mateClientTicketObj.Id = existingItem.Id;
                return await UpdateMateClientTicket(mateClientTicketObj, existingItem.Id);
            }
        }
        public async Task<MateClientTicket> InsertMateClientTicket(MateClientTicket mateClientTicketObj)
        {
            var newItem = await _unitOfWork.MateClientTicketRepository.AddAsync(mateClientTicketObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateClientTicket> UpdateMateClientTicket(MateClientTicket existingItem, long existingId)
        {
            await _unitOfWork.MateClientTicketRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateClientTicket GetByTicketId(long TicketId)
        {
            return _unitOfWork.MateClientTicketRepository.GetMany(t => t.MateTicketId == TicketId).Result.Include(t => t.Client).FirstOrDefault();
        }
        public async Task<MateClientTicket> DeleteByTicketId(long TicketId)
        {
            var mateClientTicketObj = _unitOfWork.MateClientTicketRepository.GetMany(u => u.MateTicketId == TicketId && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateClientTicketObj != null)
            {
                mateClientTicketObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateClientTicketRepository.UpdateAsync(mateClientTicketObj, mateClientTicketObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateClientTicketObj;
        }
        public List<MateClientTicket> GetByClientId(long ClientId)
        {
            return _unitOfWork.MateClientTicketRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
        }
        public async Task<List<MateClientTicket>> DeleteByClientId(long ClientId)
        {
            var mateClientTicketList = _unitOfWork.MateClientTicketRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (mateClientTicketList != null && mateClientTicketList.Count() > 0)
            {
                foreach (var item in mateClientTicketList)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.MateClientTicketRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mateClientTicketList;
        }
    }
    public partial interface IMateClientTicketService : IService<MateClientTicket>
    {
        Task<MateClientTicket> CheckInsertOrUpdate(MateClientTicket model);
        MateClientTicket GetByTicketId(long TicketId);
        Task<MateClientTicket> DeleteByTicketId(long TicketId);
        List<MateClientTicket> GetByClientId(long ClientId);
        Task<List<MateClientTicket>> DeleteByClientId(long ClientId);
    }
}