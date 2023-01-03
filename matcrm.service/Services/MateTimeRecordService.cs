using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class MateTimeRecordService : Service<MateTimeRecord>, IMateTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateTimeRecord> CheckInsertOrUpdate(MateTimeRecord mateTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateTimeRecordRepository.GetMany(t => t.Id == mateTimeRecordObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTimeRecord(mateTimeRecordObj);
            }
            else
            {
                mateTimeRecordObj.CreatedOn = existingItem.CreatedOn;
                return await UpdatetMateTimeRecord(mateTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateTimeRecord> InsertMateTimeRecord(MateTimeRecord mateTimeRecordObj)
        {
            mateTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MateTimeRecordRepository.AddAsync(mateTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTimeRecord> UpdatetMateTimeRecord(MateTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<MateTimeRecord> GetAll()
        {
            return _unitOfWork.MateTimeRecordRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public MateTimeRecord GetById(long Id)
        {
            return _unitOfWork.MateTimeRecordRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }
        public async Task<MateTimeRecord> DeleteMateTimeRecord(long Id)
        {
            var mateTimeRecordObj = _unitOfWork.MateTimeRecordRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateTimeRecordObj != null)
            {
                mateTimeRecordObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateTimeRecordRepository.UpdateAsync(mateTimeRecordObj, mateTimeRecordObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateTimeRecordObj;
        }
       
    }

    public partial interface IMateTimeRecordService : IService<MateTimeRecord>
    {
        Task<MateTimeRecord> CheckInsertOrUpdate(MateTimeRecord model);
        List<MateTimeRecord> GetAll();
        MateTimeRecord GetById(long Id);
        Task<MateTimeRecord> DeleteMateTimeRecord(long Id);
       
    }
}