using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.ViewModels
{
    public class EmployeeSubTaskVM
    {
        public EmployeeSubTaskVM()
        {
            ChildTasks = new List<EmployeeChildTaskVM>();
            Comments = new List<EmployeeSubTaskMateComment>();
            Activities = new List<EmployeeSubTaskActivityDto>();
            AssignedUsers = new List<EmployeeSubTaskUserDto>();
            Documents = new List<EmployeeSubTaskAttachment>();
        }
        public long Id { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? StatusId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<EmployeeChildTaskVM> ChildTasks { get; set; }
        // public long? CreatedBy { get; set; }
        // public bool IsExpanded { get; set; }
        // public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        // public double? Hours { get; set; }
        // public double? Minutes { get; set; }
        // public double? Seconds { get; set; }
        public List<EmployeeSubTaskUserDto> AssignedUsers { get; set; }
        public List<EmployeeSubTaskAttachment> Documents { get; set; }
        public List<EmployeeSubTaskActivityDto> Activities { get; set; }
        public List<EmployeeSubTaskMateComment> Comments { get; set; }
    }

    public class EmployeeSubTaskListVM
    {
        public EmployeeSubTaskListVM()
        {
            SubTasks = new List<EmployeeSubTaskVM>();
        }
        public List<EmployeeSubTaskVM> SubTasks { get; set; }
    }


    public class EmployeeSubTaskMateComment
    {
        public EmployeeSubTaskMateComment()
        {
            Attachments = new List<EmployeeSubTaskMateCommentAttachment>();            
        }
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public string Comment { get; set; }
        public long? MateReplyCommentId { get; set; }        
        public List<EmployeeSubTaskMateCommentAttachment> Attachments { get; set; }
    }

    public class EmployeeSubTaskMateCommentAttachment
    {
        public string? Name { get; set; }
        public string? URL { get; set; }
    }
}