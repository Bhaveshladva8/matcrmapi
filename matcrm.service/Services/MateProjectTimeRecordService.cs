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
    public partial class MateProjectTimeRecordService : Service<MateProjectTimeRecord>, IMateProjectTimeRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public MateProjectTimeRecordService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateProjectTimeRecord> CheckInsertOrUpdate(MateProjectTimeRecord mateProjectTimeRecordObj)
        {
            var existingItem = _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.Id == mateProjectTimeRecordObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateProjectTimeRecord(mateProjectTimeRecordObj);
            }
            else
            {
                return await UpdateMateProjectTimeRecord(mateProjectTimeRecordObj, existingItem.Id);
            }
        }

        public async Task<MateProjectTimeRecord> InsertMateProjectTimeRecord(MateProjectTimeRecord mateProjectTimeRecordObj)
        {
            var newItem = await _unitOfWork.MateProjectTimeRecordRepository.AddAsync(mateProjectTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateProjectTimeRecord> UpdateMateProjectTimeRecord(MateProjectTimeRecord existingItem, long existingId)
        {
            await _unitOfWork.MateProjectTimeRecordRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        // public async Task<MateProjectTimeRecord> DeleteMateProjectTimeRecord(long mateTimeRecordId)
        // {
        //     var mateProjectTimeRecordObj = _unitOfWork.MateProjectTimeRecordRepository.GetMany(u => u.MateTimeRecordId == mateTimeRecordId).Result.FirstOrDefault();
        //     if (mateProjectTimeRecordObj != null)
        //     {
        //         await _unitOfWork.MateProjectTimeRecordRepository.UpdateAsync(mateProjectTimeRecordObj, mateProjectTimeRecordObj.Id);
        //         await _unitOfWork.CommitAsync();
        //     }
        //     return mateProjectTimeRecordObj;
        // }
        public List<MateProjectTimeRecord> GetMateProjectTimeRecordByProjectId(long projectId, MateTimeRecordInvoiceRequest model)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.ProjectId == projectId && t.MateTimeRecord != null && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.CreatedOn >= model.StartDate.Date && t.MateTimeRecord.CreatedOn <= model.EndDate.Date).Result.Include(t => t.MateTimeRecord).ToList();
        }

        public MateProjectTimeRecord GetBymateTimeRecordId(long mateTimeRecordId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.MateTimeRecordId == mateTimeRecordId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).FirstOrDefault();
        }
        public List<MateProjectTimeRecord> GetByProjectId(long projectId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.ProjectId == projectId).Result.ToList();
        }
        public List<MateProjectTimeRecord> GetTimeRecordByProjectId(long projectId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.ProjectId == projectId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateProjectTimeRecord> GetTimeRecordByServiceArticleId(long ServiceArticleId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.MateTimeRecord.ServiceArticleId == ServiceArticleId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).ToList();
        }

        public List<MateProjectTimeRecord> GetListByProjectIdList(List<long> projectIds)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.MateTimeRecord.DeletedOn == null && t.ProjectId != null && projectIds.Any(b => t.ProjectId.Value == b)).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateProjectTimeRecord> GetMateProjectTimeRecordByProject(long projectId, DateTime StartDate, DateTime EndDate)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.ProjectId == projectId && t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true && t.MateTimeRecord.CreatedOn >= StartDate && t.MateTimeRecord.CreatedOn <= EndDate && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.MateTimeRecord.ServiceArticle).ToList();
        }
        public List<MateProjectTimeRecord> GetByProjectIdAndUserId(long projectId, long UserId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.ProjectId == projectId && t.MateTimeRecord.DeletedOn == null && t.MateTimeRecord.UserId == UserId).Result.Include(t => t.MateTimeRecord).ToList();
        }
        public List<MateProjectTimeRecord> GetByUserId(long UserId)
        {
            return _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.MateTimeRecord.UserId == UserId && t.MateTimeRecord.DeletedOn == null).Result.Include(t => t.MateTimeRecord).Include(t => t.EmployeeProject).ToList();
        }

        public DateTime? GetProjectTimeRecordStartDate(List<long> ProjectIds)
        {
            var timeRecordData = _unitOfWork.MateProjectTimeRecordRepository.GetMany(t => t.MateTimeRecord != null && t.MateTimeRecord.CreatedOn != null && t.MateTimeRecord.DeletedOn == null && ProjectIds.Contains(t.ProjectId.Value)).Result.Include(t => t.MateTimeRecord).OrderBy(t => t.MateTimeRecord.CreatedOn).FirstOrDefault();
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

    public partial interface IMateProjectTimeRecordService : IService<MateProjectTimeRecord>
    {
        Task<MateProjectTimeRecord> CheckInsertOrUpdate(MateProjectTimeRecord model);
        //Task<MateProjectTimeRecord> DeleteMateProjectTimeRecord(long mateTimeRecordId);
        MateProjectTimeRecord GetBymateTimeRecordId(long mateTimeRecordId);
        List<MateProjectTimeRecord> GetMateProjectTimeRecordByProjectId(long projectId, MateTimeRecordInvoiceRequest model);
        List<MateProjectTimeRecord> GetByProjectId(long projectId);
        List<MateProjectTimeRecord> GetTimeRecordByProjectId(long projectId);
        List<MateProjectTimeRecord> GetTimeRecordByServiceArticleId(long ServiceArticleId);
        List<MateProjectTimeRecord> GetListByProjectIdList(List<long> projectIds);
        List<MateProjectTimeRecord> GetMateProjectTimeRecordByProject(long projectId, DateTime StartDate, DateTime EndDate);
        List<MateProjectTimeRecord> GetByProjectIdAndUserId(long projectId, long UserId);
        List<MateProjectTimeRecord> GetByUserId(long UserId);
        DateTime? GetProjectTimeRecordStartDate(List<long> ProjectIds);
    }
}