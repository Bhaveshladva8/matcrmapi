using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class EmployeeTaskTimeRecordService : Service<EmployeeTaskTimeRecord>, IEmployeeTaskTimeRecordService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTaskTimeRecordService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeTaskTimeRecord> CheckInsertOrUpdate(EmployeeTaskTimeRecordDto timeRecordDto)
        {
            var employeeTaskTimeRecordObj = _mapper.Map<EmployeeTaskTimeRecord>(timeRecordDto);
            // var existingItem = _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany (t => t.TenantName == tenant.TenantName && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault ();
            // var existingItem = _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTaskId == timeRecordDto.EmployeeTaskId && t.IsDeleted == false).Result.FirstOrDefault();

            return await InsertEmployeeTaskTimeRecord(employeeTaskTimeRecordObj);
            // if (existingItem != null)
            // {
            //     existingItem.Duration = timeRecordDto.Duration;
            //     return await UpdateEmployeeTaskTimeRecord(existingItem);
            // }
            // else
            // {
            //     return await InsertEmployeeTaskTimeRecord(timeRecord);
            // }
        }

        public async Task<EmployeeTaskTimeRecord> InsertEmployeeTaskTimeRecord(EmployeeTaskTimeRecord employeeTaskTimeRecordObj)
        {
            employeeTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskTimeRecordRepository.AddAsync(employeeTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public async Task<EmployeeTaskTimeRecord> UpdateEmployeeTaskTimeRecord(EmployeeTaskTimeRecord employeeTaskTimeRecord)
        {
            // EmployeeTaskTimeRecord.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskTimeRecordRepository.UpdateAsync(employeeTaskTimeRecord, employeeTaskTimeRecord.Id);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public EmployeeTaskTimeRecord GetById(long Id)
        {
            return _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.Id == Id).Result.Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).Include(t => t.EmployeeTask).FirstOrDefault();
        }

        public long GetTotalEmployeeTaskTimeRecord(long EmployeeTaskId)
        {
            var timeRecords = _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
            var total = timeRecords.Sum(t => t.Duration).Value;
            return total;
        }

        public List<EmployeeTaskTimeRecord> GetListByTaskIdList(List<long> TaskIds)
        {
            return _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.IsDeleted == false && t.EmployeeTaskId != null && TaskIds.Any(b => t.EmployeeTaskId.Value == b)).Result.Include(t => t.EmployeeTask).Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).ToList();
        }

        public List<EmployeeTaskTimeRecord> getListByTaskId(long taskId)
        {
            return _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTaskId == taskId && t.IsDeleted == false).Result.Include(t => t.EmployeeTask).Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).ToList();
        }

        public List<EmployeeTaskTimeRecord> InVoice(DateTime StartDate, DateTime EndDate, int TenantId)
        {
            return _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTask.Tenant.TenantId == TenantId && t.CreatedOn >= StartDate && t.CreatedOn <= EndDate).Result.Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).Include(t => t.EmployeeTask).Include(t => t.EmployeeTask.Tenant).ToList();
            // return _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTask.Tenant.TenantId == TenantId).Result.Include(t => t.ServiceArticle).Include(t => t.ServiceArticle.Currency).Include(t => t.EmployeeTask).Include(t => t.EmployeeTask.Tenant).ToList();
        }

        public async Task<List<EmployeeTaskTimeRecord>> DeleteTimeRecordByTaskId(long EmployeeTaskId)
        {
            var employeeTaskTimeRecordsList = _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeTaskTimeRecordsList != null && employeeTaskTimeRecordsList.Count() > 0)
            {
                foreach (var item in employeeTaskTimeRecordsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeTaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskTimeRecordsList;
        }

        public async Task<List<EmployeeTaskTimeRecord>> DeleteTimeRecordByServiceArticleId(long ServiceArticleId)
        {
            var employeeTaskTimeRecordsList = _unitOfWork.EmployeeTaskTimeRecordRepository.GetMany(t => t.ServiceArticleId == ServiceArticleId && t.IsDeleted == false).Result.ToList();
            if (employeeTaskTimeRecordsList != null && employeeTaskTimeRecordsList.Count() > 0)
            {
                foreach (var item in employeeTaskTimeRecordsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeTaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskTimeRecordsList;
        }
    }

    public partial interface IEmployeeTaskTimeRecordService : IService<EmployeeTaskTimeRecord>
    {
        Task<EmployeeTaskTimeRecord> CheckInsertOrUpdate(EmployeeTaskTimeRecordDto timeRecordDto);
        long GetTotalEmployeeTaskTimeRecord(long taskId);
        List<EmployeeTaskTimeRecord> getListByTaskId(long taskId);
        List<EmployeeTaskTimeRecord> InVoice(DateTime StartDate, DateTime EndDate, int TenantId);
        Task<List<EmployeeTaskTimeRecord>> DeleteTimeRecordByTaskId(long TaskId);
        EmployeeTaskTimeRecord GetById(long Id);
        List<EmployeeTaskTimeRecord> GetListByTaskIdList(List<long> TaskIds);
        Task<List<EmployeeTaskTimeRecord>> DeleteTimeRecordByServiceArticleId(long ServiceArticleId);
    }
}