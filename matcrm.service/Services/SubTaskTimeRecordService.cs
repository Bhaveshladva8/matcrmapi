using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class SubTaskTimeRecordService : Service<SubTaskTimeRecord>, ISubTaskTimeRecordService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubTaskTimeRecordService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public SubTaskTimeRecord CheckInsertOrUpdate(SubTaskTimeRecordDto timeRecordDto)
        {
            var subTaskTimeRecordObj = _mapper.Map<SubTaskTimeRecord>(timeRecordDto);
            // var existingItem = _unitOfWork.SubTaskTimeRecordRepository.GetMany (t => t.TenantName == tenant.TenantName && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault ();

            return InsertSubTaskTimeRecord(subTaskTimeRecordObj);
            // if (existingItem != null) {
            //     return UpdateTenant (existingItem);
            // } else {
            //     return InsertTenant (tenant);
            // }
        }

        public SubTaskTimeRecord InsertSubTaskTimeRecord(SubTaskTimeRecord subTaskTimeRecordObj)
        {
            subTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.SubTaskTimeRecordRepository.Add(subTaskTimeRecordObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }

        public long GetTotalSubTaskTimeRecord(long subTaskId)
        {
            var timeRecords = _unitOfWork.SubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == subTaskId && t.IsDeleted == false).Result.ToList();
            var total = timeRecords.Sum(t => t.Duration).Value;
            return total;
        }

        public List<SubTaskTimeRecord> DeleteTimeRecordBySubTaskId(long SubTaskId)
        {
            var subTaskTimeRecordList = _unitOfWork.SubTaskTimeRecordRepository.GetMany(t => t.SubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (subTaskTimeRecordList != null && subTaskTimeRecordList.Count() > 0)
            {
                foreach (var item in subTaskTimeRecordList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.SubTaskTimeRecordRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return subTaskTimeRecordList;
        }
    }

    public partial interface ISubTaskTimeRecordService : IService<SubTaskTimeRecord>
    {
        SubTaskTimeRecord CheckInsertOrUpdate(SubTaskTimeRecordDto timeRecordDto);
        long GetTotalSubTaskTimeRecord(long subTaskId);
        List<SubTaskTimeRecord> DeleteTimeRecordBySubTaskId(long SubTaskId);
    }
}