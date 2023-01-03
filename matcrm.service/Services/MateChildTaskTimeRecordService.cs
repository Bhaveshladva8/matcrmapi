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
    public partial class MateChildTaskTimeRecordService : Service<MateChildTaskTimeRecord>, IMateChildTaskTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateChildTaskTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateChildTaskTimeRecord> CheckInsertOrUpdate(MateChildTaskTimeRecord childTaskTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.Id == childTaskTimeRecordObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertChildTaskTimeRecord(childTaskTimeRecordObj);
            }
            else
            {
                return await UpdateChildTaskTimeRecord(childTaskTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateChildTaskTimeRecord> InsertChildTaskTimeRecord(MateChildTaskTimeRecord childTaskTimeRecordObj)
        {
            var newItem = await _unitOfWork.MateChildTaskTimeRecordRepository.AddAsync(childTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateChildTaskTimeRecord> UpdateChildTaskTimeRecord(MateChildTaskTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateChildTaskTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<MateChildTaskTimeRecord> GetByChildTaskId(long ChildTaskId, MateTimeRecordInvoiceRequest model)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= model.StartDate && t.MateTimeRecord.CreatedOn <= model.EndDate).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }
        public List<MateChildTaskTimeRecord> GetByChildTask(long ChildTaskId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate && t.MateTimeRecord.IsBillable == true).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).Include(t => t.MateTimeRecord.ServiceArticle.Tax).ToList();
        }
        public List<MateChildTaskTimeRecord> GetListByChildTaskIdList(List<long> ChildTaskIds)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord != null && t.MateTimeRecord.DeletedOn == null && t.ChildTaskId != null && ChildTaskIds.Any(b => t.ChildTaskId.Value == b)).Result.Include(t => t.MateTimeRecord).ToList();
        }

        public List<MateChildTaskTimeRecord> GetMateChildTaskTimeRecordByChildTaskId(long ChildTaskId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.MateTimeRecord != null && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }
        public List<MateChildTaskTimeRecord> GetByChildTaskIdAndUserId(long ChildTaskId, long UserId)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.UserId == UserId).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public MateChildTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.MateTimeRecordId == mateTimeRecordId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).FirstOrDefault();
        }
        public List<MateChildTaskTimeRecord> GetRecordByChildTaskId(long childTaskId)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.ChildTaskId == childTaskId).Result.ToList();
        }
        public List<MateChildTaskTimeRecord> GetByUserId(long UserId)
        {
            return _unitOfWork.MateChildTaskTimeRecordRepository.GetMany(t => t.MateTimeRecord.UserId == UserId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.EmployeeChildTask).ToList();
        }
    }

    public partial interface IMateChildTaskTimeRecordService : IService<MateChildTaskTimeRecord>
    {
        Task<MateChildTaskTimeRecord> CheckInsertOrUpdate(MateChildTaskTimeRecord model);
        List<MateChildTaskTimeRecord> GetByChildTaskId(long ChildTaskId, MateTimeRecordInvoiceRequest model);
        List<MateChildTaskTimeRecord> GetByChildTask(long ChildTaskId, DateTime StartDate, DateTime EndDate);
        List<MateChildTaskTimeRecord> GetListByChildTaskIdList(List<long> ChildTaskIds);
        List<MateChildTaskTimeRecord> GetMateChildTaskTimeRecordByChildTaskId(long ChildTaskId, DateTime StartDate, DateTime EndDate);
        List<MateChildTaskTimeRecord> GetByChildTaskIdAndUserId(long ChildTaskId, long UserId);
        MateChildTaskTimeRecord GetBymateTimeRecordId(long mateTimeRecordId);
        List<MateChildTaskTimeRecord> GetRecordByChildTaskId(long childTaskId);
        List<MateChildTaskTimeRecord> GetByUserId(long UserId);

    }
}