using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.ViewModels {

    public class TaskListVM {
        public List<SectionVM> Sections { get; set; }
        public List<OneClappTaskVM> Tasks { get; set; }
    }

    public class SectionVM {
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<OneClappTaskVM> Tasks { get; set; }
    }

    public class WeClappTaskVM {
        public List<OneClappTaskVM> Tasks { get; set; }
    }

    public class OneClappTaskVM {
        public long Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? StatusId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public long? SectionId { get; set; }
        public List<OneClappSubTaskVM> SubTasks { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public long? Priority { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Minutes { get; set; }
        public decimal? Seconds { get; set; }
       public List<OneClappTaskUserDto> AssignedUsers { get; set; }
        public long? AssignUserCount { get; set; }
        public List<TaskCommentDto> Comments { get; set; }
        public List<TaskAttachment> Documents { get; set; }
        public List<TaskActivityDto> Activities { get; set; }
    }

    public class OneClappSubTaskVM {
        public long Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? OneClappTaskId { get; set; }
        public long? StatusId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<OneClappChildTaskVM> ChildTasks { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public double? Hours { get; set; }
        public double? Minutes { get; set; }
        public double? Seconds { get; set; }
        public List<OneClappSubTaskUserDto> AssignedUsers { get; set; }
        public List<SubTaskAttachment> Documents { get; set; }
        public List<SubTaskActivityDto> Activities { get; set; }
        public List<SubTaskCommentDto> Comments { get; set; }
    }

    public class OneClappChildTaskVM {
        public long Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? OneClappSubTaskId { get; set; }
        public long? StatusId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public decimal? Hours { get; set; }
        public decimal? Minutes { get; set; }
        public decimal? Seconds { get; set; }  
        public List<OneClappChildTaskUserDto> AssignedUsers { get; set; }
        public List<ChildTaskAttachment> Documents { get; set; }
        public List<ChildTaskActivityDto> Activities { get; set; }
        public List<ChildTaskCommentDto> Comments { get; set; }
    }
}