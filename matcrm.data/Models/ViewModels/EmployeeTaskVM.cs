using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.ViewModels
{
    public class EmployeeTaskVM
    {
        public EmployeeTaskVM()
        {
            SubTasks = new List<EmployeeSubTaskVM>();
            AssignedUsers = new List<EmployeeTaskUserDto>();
            //Comments = new List<EmployeeTaskMateComment>();
            Documents = new List<EmployeeTaskAttachment>();
            Activities = new List<EmployeeTaskActivityDto>();
        }
        public long? Id { get; set; }
        public long? StatusId { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public bool IsActive { get; set; }
        public long? ProjectId { get; set; }
        public long? SectionId { get; set; }
        //public long? CreatedBy { get; set; }
        //public bool IsExpanded { get; set; }
        //public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public long? TimeRecordId { get; set; }
        public long? Priority { get; set; }
        //public decimal? Hours { get; set; }
        //public decimal? Minutes { get; set; }
        //public decimal? Seconds { get; set; }
        public List<EmployeeTaskUserDto> AssignedUsers { get; set; }
        //public long? AssignUserCount { get; set; }
        //public List<EmployeeTaskMateComment> Comments { get; set; }
        public List<EmployeeTaskAttachment> Documents { get; set; }
        public List<EmployeeTaskActivityDto> Activities { get; set; }
        public List<EmployeeSubTaskVM> SubTasks { get; set; }
    }

    // public class EmployeeTaskMateComment
    // {
    //     public EmployeeTaskMateComment()
    //     {
    //         Attachments = new List<EmployeeTaskMateCommentAttachment>();            
    //     }
    //     public long Id { get; set; }
    //     public int? UserId { get; set; }
    //     public string UserName { get; set; }
    //     public DateTime? CreatedOn { get; set; }        
    //     public string Comment { get; set; }
    //     public long? MateReplyCommentId { get; set; }        
    //     public List<EmployeeTaskMateCommentAttachment> Attachments { get; set; }
    // }

    // public class EmployeeTaskMateCommentAttachment
    // {
    //     public string? Name { get; set; }
    //     public string? URL { get; set; }
    // }

    public class EmployeeTaskListVM
    {
        public EmployeeTaskListVM()
        {
            Tasks = new List<EmployeeTaskVM>();
        }
        public List<EmployeeTaskVM> Tasks { get; set; }
    }

    public class ProjectTaskListVM
    {
        public ProjectTaskListVM()
        {
            Tasks = new List<EmployeeTaskVM>();
            Projects = new List<ProjectVM>();
        }
        public List<EmployeeTaskVM> Tasks { get; set; }
        public List<ProjectVM> Projects { get; set; }
    }

    public class EmployeeSectionTaskListVM
    {
        public EmployeeSectionTaskListVM()
        {
            Tasks = new List<EmployeeTaskVM>();
            Sections = new List<EmployeeSectionVM>();
        }
        public List<EmployeeTaskVM> Tasks { get; set; }
        public List<EmployeeSectionVM> Sections { get; set; }
    }

    public class ProjectVM
    {
        public ProjectVM()
        {
            Tasks = new List<EmployeeTaskVM>();
            AssignedUsers = new List<EmployeeProjectUserDto>();
            Sections = new List<EmployeeSectionVM>();
        }

        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? LogoName { get; set; }
        //[DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{hh:mm:ss}", ApplyFormatInEditMode = true)]
        //public TimeSpan? EstimatedTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? Priority { get; set; }
        public long? StatusId { get; set; }
        public long? TaskCount { get; set; }
        public List<EmployeeTaskVM> Tasks { get; set; }
        public List<EmployeeProjectUserDto> AssignedUsers { get; set; }
        public List<EmployeeSectionVM> Sections { get; set; }
    }

    public class EmployeeSectionVM
    {
        public EmployeeSectionVM()
        {
            Tasks = new List<EmployeeTaskVM>();
        }

        public long? Id { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long? ProjectId { get; set; }
        public long? SectionId { get; set; }
        public long? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<EmployeeTaskVM> Tasks { get; set; }
    }
}