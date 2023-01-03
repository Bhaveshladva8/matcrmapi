using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class CustomTaskListResponse
    {
        public CustomTaskListResponse()
        {
            GroupColumnList = new List<CustomTaskGroupColumnResponse>();
            Columns = new List<CustomTableColumnResponse>();
            TaskGroupBY = new List<TaskGroupBY>();
        }
        public List<CustomTaskGroupColumnResponse> GroupColumnList { get; set; }
        public string SelectedGroupBy { get; set; }
        public List<CustomTableColumnResponse> Columns { get; set; }
        public List<TaskGroupBY> TaskGroupBY { get; set; }
    }

    public class CustomTableColumnResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        // public string DisplayName { get; set; }
        public string Key { get; set; }
        public bool IsShort { get; set; } = false;
        public string Direction { get; set; }
        public string Fx { get; set; }
        public bool IsRemove { get; set; } = false;
        public bool IsDragOff { get; set; } = false;
        public string ControlType { get; set; }
        public bool IsInsert { get; set; }
        public bool IsHide { get; set; }
        public long? Priority { get; set; }
    }

    public class CustomTaskResponse
    {
        public CustomTaskResponse()
        {
            AssignedUsers = new List<CustomTaskAssignUserResponse>();
            Tasks = new List<CustomSubTaskResponse>();
        }
        public long Id { get; set; }
        public string Description { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public long? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomTaskAssignUserResponse> AssignedUsers { get; set; }
        public List<CustomSubTaskResponse> Tasks { get; set; }
    }

    public class CustomSubTaskResponse
    {
        public CustomSubTaskResponse()
        {
            AssignedUsers = new List<CustomSubTaskAssignUserResponse>();
        }
        public long Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string Description { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public long? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomSubTaskAssignUserResponse> AssignedUsers { get; set; }
    }

    public class CustomChildTaskResponse
    {
        public CustomChildTaskResponse()
        {
            AssignedUsers = new List<CustomChildTaskAssignUserResponse>();
        }
        public long Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string Description { get; set; }
        public long? StatusId { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public long? Duration { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomChildTaskAssignUserResponse> AssignedUsers { get; set; }
        public List<CustomSubTaskAssignUserResponse> Tasks { get; set; }
    }

    public class CustomSubTaskAssignUserResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public string AssignUserFirstName { get; set; }
        public string AssignUserLastName { get; set; }
    }

    public class CustomChildTaskAssignUserResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeChildTaskId { get; set; }
        public string AssignUserFirstName { get; set; }
        public string AssignUserLastName { get; set; }
    }

    public class CustomTaskAssignUserResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeTaskId { get; set; }
        public string AssignUserFirstName { get; set; }
        public string AssignUserLastName { get; set; }
    }

    public class TaskGroupBY
    {
        public TaskGroupBY()
        {
            Id = new List<string>();
            Data = new List<Status>();
            TaskIds = new List<long>();
        }
        public List<string> Id { get; set; }
        public List<Status> Data { get; set; }
        public List<long> TaskIds { get; set; }
        // public List<CustomTaskResponse> Tasks { get; set; }
    }

    public class CustomTaskVM
    {
        public CustomTaskVM()
        {
            Tasks = new List<CustomSubTaskVM>();
            Assignee = new List<CustomTaskUserVM>();
        }
        public long? Id { get; set; }
        public long? MainTaskId { get; set; }
        public long? StatusId { get; set; }
        public string Key { get; set; }
        public string StatusName { get; set; }
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
        public List<CustomTaskUserVM> Assignee { get; set; }
        public string AssigneeName { get; set; }
        public List<CustomSubTaskVM> Tasks { get; set; }
    }

    public class CustomTaskUserVM
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
        // public long? EmployeeTaskId { get; set; }
    }

    public class CustomSubTaskVM
    {
        public CustomSubTaskVM()
        {
            Tasks = new List<CustomChildTaskVM>();
            Assignee = new List<EmployeeSubTaskUserDto>();
        }
        public long Id { get; set; }
        public long? EmployeeTaskId { get; set; }
        public long? MainTaskId { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string Key { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<CustomChildTaskVM> Tasks { get; set; }
        // public long? CreatedBy { get; set; }
        // public bool IsExpanded { get; set; }
        // public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public List<EmployeeSubTaskUserDto> Assignee { get; set; }
        public string? AssigneeName { get; set; }
    }

    public class CustomChildTaskVM
    {
        public CustomChildTaskVM()
        {
            Tasks = new List<CustomChildTaskVM>();
            Assignee = new List<EmployeeChildTaskUserDto>();
        }
        public long Id { get; set; }
        public long? MainTaskId { get; set; }
        public long? WeClappTimeRecordId { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // public bool IsActive { get; set; }
        // public long? CreatedBy { get; set; }
        // public bool IsExpanded { get; set; }
        // public bool IsEdit { get; set; }
        public long? Duration { get; set; }
        public List<EmployeeChildTaskUserDto> Assignee { get; set; }
        public List<CustomChildTaskVM> Tasks { get; set; }
        public string AssigneeName { get; set; }
    }

    public class AddUpdateCustomTaskRequestResponse
    {
        public long? Id { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public long? EmployeeTaskId { get; set; }

    }

}