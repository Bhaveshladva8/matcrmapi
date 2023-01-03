using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class MateSubTaskTimeRecordService : Service<MateSubTaskTimeRecord>, IMateSubTaskTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateSubTaskTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateSubTaskTimeRecord> CheckInsertOrUpdate(MateSubTaskTimeRecord subTaskTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.Id == subTaskTimeRecordObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubTaskTimeRecord(subTaskTimeRecordObj);
            }
            else
            {
                return await UpdateSubTaskTimeRecord(subTaskTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateSubTaskTimeRecord> InsertSubTaskTimeRecord(MateSubTaskTimeRecord subTaskTimeRecordObj)
        {
            var newItem = await _unitOfWork.MateSubTaskTimeRecordRepository.AddAsync(subTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateSubTaskTimeRecord> UpdateSubTaskTimeRecord(MateSubTaskTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateSubTaskTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<MateSubTaskTimeRecord> GetBySubTaskId(long SubTaskId, MateTimeRecordInvoiceRequest model)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= model.StartDate && t.MateTimeRecord.CreatedOn <= model.EndDate).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }
        public List<MateSubTaskTimeRecord> GetBySubTask(long SubTaskId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate && t.MateTimeRecord.IsBillable == true).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).Include(t => t.MateTimeRecord.ServiceArticle.Tax).ToList();
        }
        public List<MateSubTaskTimeRecord> GetListBySubTaskIdList(List<long> subTaskIds)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.DeletedOn == null && t.SubTaskId != null && subTaskIds.Any(b => t.SubTaskId.Value == b)).Result.Include(t => t.MateTimeRecord).ToList();
        }

        public List<MateSubTaskTimeRecord> GetMateSubTaskTimeRecordBySubTaskId(long SubTaskId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.MateTimeRecord != null && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }
        public List<MateSubTaskTimeRecord> GetBySubTaskIdAndUserId(long SubTaskId, long UserId)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.UserId == UserId).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public MateSubTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.MateTimeRecordId == mateTimeRecordId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).FirstOrDefault();
        }
        public List<MateSubTaskTimeRecord> GetRecordBySubTaskId(long subTaskId)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == subTaskId).Result.ToList();
        }
        public List<MateSubTaskTimeRecord> GetByUserId(long UserId)
        {
            return _unitOfWork.MateSubTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.UserId == UserId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.EmployeeSubTask).ToList();
        }
    }

    public partial interface IMateSubTaskTimeRecordService : IService<MateSubTaskTimeRecord>
    {
        Task<MateSubTaskTimeRecord> CheckInsertOrUpdate(MateSubTaskTimeRecord model);
        List<MateSubTaskTimeRecord> GetBySubTaskId(long SubTaskId, MateTimeRecordInvoiceRequest model);
        List<MateSubTaskTimeRecord> GetBySubTask(long SubTaskId, DateTime StartDate, DateTime EndDate);
        List<MateSubTaskTimeRecord> GetListBySubTaskIdList(List<long> subTaskIds);
        List<MateSubTaskTimeRecord> GetMateSubTaskTimeRecordBySubTaskId(long SubTaskId, DateTime StartDate, DateTime EndDate);
        List<MateSubTaskTimeRecord> GetBySubTaskIdAndUserId(long SubTaskId, long UserId);
        MateSubTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId);
        List<MateSubTaskTimeRecord> GetRecordBySubTaskId(long subTaskId);
        List<MateSubTaskTimeRecord> GetByUserId(long UserId);

    }
}