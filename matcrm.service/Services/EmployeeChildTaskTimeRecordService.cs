using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class EmployeeChildTaskTimeRecordService : Service<EmployeeChildTaskTimeRecord>, IEmployeeChildTaskTimeRecordService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeChildTaskTimeRecordService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeChildTaskTimeRecord> CheckInsertOrUpdate(EmployeeChildTaskTimeRecordDto timeRecordDto)
        {
            var employeeChildTaskTimeRecordObj = _mapper.Map<EmployeeChildTaskTimeRecord>(timeRecordDto);
            // var existingItem = _unitOfWork.EmployeeChildTaskTimeRecordRepository.GetMany(t => t.EmployeeChildTaskId == timeRecordDto.EmployeeChildTaskId && t.IsDeleted == false).Result.FirstOrDefault();

            return await InsertEmployeeChildTaskTimeRecord(employeeChildTaskTimeRecordObj);
            // if (existingItem != null)
            // {
            //     existingItem.Duration = timeRecord.Duration;
            //      return await UpdateEmployeeChildTaskTimeRecord(existingItem);
            // }
            // else
            // {
            //     return await InsertEmployeeChildTaskTimeRecord(timeRecord);
            // }
        }

        public async Task<EmployeeChildTaskTimeRecord> InsertEmployeeChildTaskTimeRecord(EmployeeChildTaskTimeRecord employeeChildTaskTimeRecordObj)
        {
            employeeChildTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskTimeRecordRepository.AddAsync(employeeChildTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public async Task<EmployeeChildTaskTimeRecord> UpdateEmployeeChildTaskTimeRecord(EmployeeChildTaskTimeRecord employeeChildTaskTimeRecordObj)
        {
            employeeChildTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskTimeRecordRepository.UpdateAsync(employeeChildTaskTimeRecordObj, employeeChildTaskTimeRecordObj.Id);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public long GetTotalEmployeeChildTaskTimeRecord(long EmployeeChildTaskId)
        {
            var timeRecords = _unitOfWork.EmployeeChildTaskTimeRecordRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
            var total = timeRecords.Sum(t => t.Duration).Value;
            return total;
        }

        public async Task<List<EmployeeChildTaskTimeRecord>> DeleteTimeRecordByEmployeeChildTaskId(long EmployeeChildTaskId)
        {
            var employeeChildTaskTimeRecordList = _unitOfWork.EmployeeChildTaskTimeRecordRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeChildTaskTimeRecordList != null && employeeChildTaskTimeRecordList.Count() > 0)
            {
                foreach (var item in employeeChildTaskTimeRecordList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeChildTaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskTimeRecordList;
        }
    }

    public partial interface IEmployeeChildTaskTimeRecordService : IService<EmployeeChildTaskTimeRecord>
    {
        Task<EmployeeChildTaskTimeRecord> CheckInsertOrUpdate(EmployeeChildTaskTimeRecordDto timeRecordDto);
        long GetTotalEmployeeChildTaskTimeRecord(long EmployeeChildTaskId);
        Task<List<EmployeeChildTaskTimeRecord>> DeleteTimeRecordByEmployeeChildTaskId(long EmployeeChildTaskId);
    }
}