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
    public partial class EmployeeSubTaskTimeRecordService : Service<EmployeeSubTaskTimeRecord>, IEmployeeSubTaskTimeRecordService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeSubTaskTimeRecordService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTaskTimeRecord> CheckInsertOrUpdate(EmployeeSubTaskTimeRecordDto timeRecordDto)
        {
            var employeeSubTaskTimeRecordObj = _mapper.Map<EmployeeSubTaskTimeRecord>(timeRecordDto);
            // var existingItem = _unitOfWork.EmployeeSubTaskTimeRecordRepository.GetMany (t => t.SubTaskId == timeRecordDto.SubTaskId && t.IsDeleted == false).Result.FirstOrDefault ();

            return await InsertEmployeeSubTaskTimeRecord(employeeSubTaskTimeRecordObj);
            // if (existingItem != null) {
            //     existingItem.Duration = timeRecord.Duration;
            //     return await UpdateEmployeeSubTaskTimeRecord(existingItem);
            // } else {
            //     return await InsertEmployeeSubTaskTimeRecord(timeRecord);
            // }
        }

        public async Task<EmployeeSubTaskTimeRecord> InsertEmployeeSubTaskTimeRecord(EmployeeSubTaskTimeRecord employeeSubTaskTimeRecordObj)
        {
            employeeSubTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskTimeRecordRepository.AddAsync(employeeSubTaskTimeRecordObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public async Task<EmployeeSubTaskTimeRecord> UpdateEmployeeSubTaskTimeRecord(EmployeeSubTaskTimeRecord employeeSubTaskTimeRecordObj)
        {
            employeeSubTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskTimeRecordRepository.UpdateAsync(employeeSubTaskTimeRecordObj, employeeSubTaskTimeRecordObj.Id);
            await _unitOfWork.CommitAsync();

            return newItem;
        }

        public long GetTotalEmployeeSubTaskTimeRecord(long subTaskId)
        {
            var timeRecords = _unitOfWork.EmployeeSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == subTaskId && t.IsDeleted == false).Result.ToList();
            var total = timeRecords.Sum(t => t.Duration).Value;
            return total;
        }

        public async Task<List<EmployeeSubTaskTimeRecord>> DeleteTimeRecordBySubTaskId(long SubTaskId)
        {
            var employeeSubTaskTimeRecordList = _unitOfWork.EmployeeSubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeSubTaskTimeRecordList != null && employeeSubTaskTimeRecordList.Count() > 0)
            {
                foreach (var item in employeeSubTaskTimeRecordList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeSubTaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskTimeRecordList;
        }
    }

    public partial interface IEmployeeSubTaskTimeRecordService : IService<EmployeeSubTaskTimeRecord>
    {
        Task<EmployeeSubTaskTimeRecord> CheckInsertOrUpdate(EmployeeSubTaskTimeRecordDto timeRecordDto);
        long GetTotalEmployeeSubTaskTimeRecord(long subTaskId);
        Task<List<EmployeeSubTaskTimeRecord>> DeleteTimeRecordBySubTaskId(long SubTaskId);
    }
}