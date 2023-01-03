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
    public partial class MateTaskTimeRecordService : Service<MateTaskTimeRecord>, IMateTaskTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateTaskTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateTaskTimeRecord> CheckInsertOrUpdate(MateTaskTimeRecord mateTaskTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.Id == mateTaskTimeRecordObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTaskTimeRecord(mateTaskTimeRecordObj);
            }
            else
            {
                return await UpdateMateTaskTimeRecord(mateTaskTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateTaskTimeRecord> InsertMateTaskTimeRecord(MateTaskTimeRecord mateTaskTimeRecordObj)
        {
            var newItem = await _unitOfWork.MateTaskTimeRecordRepository.AddAsync(mateTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTaskTimeRecord> UpdateMateTaskTimeRecord(MateTaskTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateTaskTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        // public async Task<MateTaskTimeRecord> DeleteMateTaskTimeRecord(long mateTimeRecordId)
        // {
        //     var mateTaskTimeRecordObj = _unitOfWork.MateTaskTimeRecordRepository.GetMany(u => u.MateTimeRecordId == mateTimeRecordId).Result.FirstOrDefault();
        //     if (mateTaskTimeRecordObj != null)
        //     {
        //         await _unitOfWork.MateTaskTimeRecordRepository.UpdateAsync(mateTaskTimeRecordObj, mateTaskTimeRecordObj.Id);
        //         await _unitOfWork.CommitAsync();
        //     }
        //     return mateTaskTimeRecordObj;
        // }

        public List<MateTaskTimeRecord> GetMateTaskTimeRecordByTaskId(long taskId, MateTimeRecordInvoiceRequest model)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.TaskId == taskId && t.MateTimeRecord != null  && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= model.StartDate && t.MateTimeRecord.CreatedOn <= model.EndDate ).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }

        public List<MateTaskTimeRecord> GetMateTaskTimeRecordByTask(long taskId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.TaskId == taskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate && t.MateTimeRecord.IsBillable == true).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).Include(t => t.MateTimeRecord.ServiceArticle.Tax).ToList();
        }

        public MateTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.MateTimeRecordId == mateTimeRecordId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).FirstOrDefault();
        }

        public List<MateTaskTimeRecord> GetTimeRecordByTaskId(long taskId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.TaskId == taskId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateTaskTimeRecord> GetByTaskId(long taskId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.TaskId == taskId).Result.ToList();
        }
        public List<MateTaskTimeRecord> GetTimeRecordByServiceArticleId(long ServiceArticleId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.ServiceArticleId == ServiceArticleId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateTaskTimeRecord> GetListByTaskIdList(List<long> taskIds)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.DeletedOn == null && t.TaskId != null && taskIds.Any(b => t.TaskId.Value == b)).Result.Include(t => t.MateTimeRecord).ToList();
        }

        public List<MateTaskTimeRecord> GetByTaskIdAndUserId(long taskId, long UserId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.TaskId == taskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.UserId == UserId).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateTaskTimeRecord> GetByUserId(long UserId)
        {
            return _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.UserId == UserId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.EmployeeTask).ToList();
        }

        public DateTime? GetTaskTimeRecordStartDate(List<long> TaskIds)
        {
            var timeRecordData = _unitOfWork.MateTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord != null && t.MateTimeRecord.CreatedOn != null && t.MateTimeRecord.DeletedOn == null && TaskIds.Contains(t.TaskId.Value)).Result.Include(t => t.MateTimeRecord).OrderBy(t => t.MateTimeRecord.CreatedOn).FirstOrDefault();
            if (timeRecordData != null)
            {
                return timeRecordData.MateTimeRecord.CreatedOn.Value;
            }
            else
            {
                return null;
            }
        }

    }
    public partial interface IMateTaskTimeRecordService : IService<MateTaskTimeRecord>
    {
        Task<MateTaskTimeRecord> CheckInsertOrUpdate(MateTaskTimeRecord model);
        MateTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId);
        List<MateTaskTimeRecord> GetMateTaskTimeRecordByTaskId(long taskId, MateTimeRecordInvoiceRequest model);
        List<MateTaskTimeRecord> GetMateTaskTimeRecordByTask(long taskId, DateTime StartDate, DateTime EndDate);
        //long GetTotalMateTaskTimeRecord(long taskId);
        List<MateTaskTimeRecord> GetTimeRecordByTaskId(long taskId);
        List<MateTaskTimeRecord> GetByTaskId(long taskId);
        List<MateTaskTimeRecord> GetTimeRecordByServiceArticleId(long ServiceArticleId);
        List<MateTaskTimeRecord> GetListByTaskIdList(List<long> taskIds);
        List<MateTaskTimeRecord> GetByTaskIdAndUserId(long taskId, long UserId);
        List<MateTaskTimeRecord> GetByUserId(long UserId);
        DateTime? GetTaskTimeRecordStartDate(List<long> TaskIds);

    }
}