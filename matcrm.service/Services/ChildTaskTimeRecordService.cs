using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class ChildTaskTimeRecordService : Service<ChildTaskTimeRecord>, IChildTaskTimeRecordService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ChildTaskTimeRecordService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public ChildTaskTimeRecord CheckInsertOrUpdate (ChildTaskTimeRecordDto timeRecordDto) {
            var childTaskTimeRecordObj = _mapper.Map<ChildTaskTimeRecord> (timeRecordDto);
            // var existingItem = _unitOfWork.ChildTaskTimeRecordRepository.GetMany (t => t.TenantName == tenant.TenantName && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault ();

            return InsertChildTaskTimeRecord (childTaskTimeRecordObj);
            // if (existingItem != null) {
            //     return UpdateTenant (existingItem);
            // } else {
            //     return InsertTenant (tenant);
            // }
        }

        public ChildTaskTimeRecord InsertChildTaskTimeRecord (ChildTaskTimeRecord childTaskTimeRecordObj) {
            childTaskTimeRecordObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ChildTaskTimeRecordRepository.Add (childTaskTimeRecordObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }

        public long GetTotalChildTaskTimeRecord (long childTaskId) {
            var childTaskTimeRecords = _unitOfWork.ChildTaskTimeRecordRepository.GetMany (t => t.ChildTaskId == childTaskId && t.IsDeleted == false).Result.ToList ();
            var total = childTaskTimeRecords.Sum (t => t.Duration).Value;
            return total;
        }

        public List<ChildTaskTimeRecord> DeleteTimeRecordByChildTaskId (long ChildTaskId) {
            var childTaskTimeRecordsList = _unitOfWork.ChildTaskTimeRecordRepository.GetMany (t => t.ChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList ();
            if(childTaskTimeRecordsList != null && childTaskTimeRecordsList.Count() > 0)
            {
                foreach (var item in childTaskTimeRecordsList) {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.ChildTaskTimeRecordRepository.UpdateAsync (item, item.Id);
                }
                _unitOfWork.CommitAsync ();
            }

            return childTaskTimeRecordsList;
        }
    }

    public partial interface IChildTaskTimeRecordService : IService<ChildTaskTimeRecord> {
        ChildTaskTimeRecord CheckInsertOrUpdate (ChildTaskTimeRecordDto timeRecordDto);
        long GetTotalChildTaskTimeRecord (long childTaskId);
        List<ChildTaskTimeRecord> DeleteTimeRecordByChildTaskId (long ChildTaskId);
    }
}