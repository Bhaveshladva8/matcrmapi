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
    public partial class MateTicketTimeRecordService : Service<MateTicketTimeRecord>, IMateTicketTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateTicketTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public List<MateTicketTimeRecord> GetByTicketId(long TicketId)
        {
            return _unitOfWork.MateTicketTimeRecordRepository.GetMany(t => t.MateTicketId == TicketId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public async Task<MateTicketTimeRecord> CheckInsertOrUpdate(MateTicketTimeRecord mateTicketTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateTicketTimeRecordRepository.GetMany(t => t.Id == mateTicketTimeRecordObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTicketTimeRecord(mateTicketTimeRecordObj);
            }
            else
            {
                return await UpdateMateTicketTimeRecord(mateTicketTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateTicketTimeRecord> InsertMateTicketTimeRecord(MateTicketTimeRecord mateTicketTimeRecordObj)
        {
            var newItem = await _unitOfWork.MateTicketTimeRecordRepository.AddAsync(mateTicketTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTicketTimeRecord> UpdateMateTicketTimeRecord(MateTicketTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateTicketTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateTicketTimeRecord GetBymateTimeRecordId(long mateTimeRecordId)
        {
            return _unitOfWork.MateTicketTimeRecordRepository.GetMany(t => t.MateTimeRecordId == mateTimeRecordId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).FirstOrDefault();
        }
        public List<MateTicketTimeRecord> GetByUserId(long UserId)
        {
            return _unitOfWork.MateTicketTimeRecordRepository.GetMany(t => t.MateTimeRecord.UserId == UserId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTicket).ToList();
        }
        public List<MateTicketTimeRecord> GetByTicketIdAndUserId(long TicketId, long UserId)
        {
            return _unitOfWork.MateTicketTimeRecordRepository.GetMany(t => t.MateTicketId == TicketId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.UserId == UserId).Result.Include(t => t.MateTimeRecord).ToList();
        }
    }
    public partial interface IMateTicketTimeRecordService : IService<MateTicketTimeRecord>
    {
        List<MateTicketTimeRecord> GetByTicketId(long TicketId);
        Task<MateTicketTimeRecord> CheckInsertOrUpdate(MateTicketTimeRecord model);
        MateTicketTimeRecord GetBymateTimeRecordId(long mateTimeRecordId);
        List<MateTicketTimeRecord> GetByUserId(long UserId);
        List<MateTicketTimeRecord> GetByTicketIdAndUserId(long TicketId, long UserId);
    }
}