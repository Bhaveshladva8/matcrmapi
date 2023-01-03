using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class TaskTimeRecordService : Service<TaskTimeRecord>, ITaskTimeRecordService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TaskTimeRecordService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public TaskTimeRecord CheckInsertOrUpdate(TaskTimeRecordDto timeRecordDto)
        {
            var taskTimeRecordObj = _mapper.Map<TaskTimeRecord>(timeRecordDto);
            // var existingItem = _unitOfWork.TaskTimeRecordRepository.GetMany (t => t.TenantName == tenant.TenantName && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault ();

            return InsertTaskTimeRecord(taskTimeRecordObj);
            // if (existingItem != null) {
            //     return UpdateTenant (existingItem);
            // } else {
            //     return InsertTenant (tenant);
            // }
        }

        public TaskTimeRecord InsertTaskTimeRecord(TaskTimeRecord taskTimeRecordObj)
        {
            taskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TaskTimeRecordRepository.Add(taskTimeRecordObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }

        public long GetTotalTaskTimeRecord(long taskId)
        {
            var timeRecords = _unitOfWork.TaskTimeRecordRepository.GetMany(t => t.TaskId == taskId && t.IsDeleted == false).Result.ToList();
            var total = timeRecords.Sum(t => t.Duration).Value;
            return total;
        }

        public List<TaskTimeRecord> DeleteTimeRecordByTaskId(long TaskId)
        {
            var taskTimeRecordList = _unitOfWork.TaskTimeRecordRepository.GetMany(t => t.TaskId == TaskId && t.IsDeleted == false).Result.ToList();
            if (taskTimeRecordList != null && taskTimeRecordList.Count() > 0)
            {
                foreach (var item in taskTimeRecordList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.TaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return taskTimeRecordList;
        }
    }

    public partial interface ITaskTimeRecordService : IService<TaskTimeRecord>
    {
        TaskTimeRecord CheckInsertOrUpdate(TaskTimeRecordDto timeRecordDto);
        long GetTotalTaskTimeRecord(long taskId);
        List<TaskTimeRecord> DeleteTimeRecordByTaskId(long TaskId);
    }
}