using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;

namespace matcrm.data.Models.Response
{
    public class EmployeeTaskDetailResponse
    {
        public EmployeeTaskDetailResponse()
        {            
            AssignedUsers = new List<EmployeeTaskUserDetailResponse>();
            SubTasks = new List<EmployeeTaskDetailSubTask>();
            Comments = new List<EmployeeTaskDetailMateComment>();            
            Activities = new List<EmployeeTaskDetailActivityResponse>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }                
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public long? MatePriorityId { get; set; }
        public string MatePriorityName { get; set; }
        public long? ProjectId { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public List<EmployeeTaskUserDetailResponse> AssignedUsers { get; set; }       
        public List<EmployeeTaskDetailSubTask> SubTasks { get; set; }
        public List<EmployeeTaskDetailMateComment> Comments { get; set; }
        public List<EmployeeTaskDetailActivityResponse> Activities { get; set; }
    }
    public class EmployeeTaskUserDetailResponse
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string ProfileURL { get; set; }
    }

    public class EmployeeTaskDetailMateComment
    {
        public EmployeeTaskDetailMateComment()
        {
            Attachments = new List<EmployeeTaskDetailMateCommentAttachment>();            
        }
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public string Comment { get; set; }
        public long? MateReplyCommentId { get; set; }        
        public List<EmployeeTaskDetailMateCommentAttachment> Attachments { get; set; }
    }

    public class EmployeeTaskDetailMateCommentAttachment
    {
        public string? Name { get; set; }
        public string? URL { get; set; }
    }
    public class EmployeeTaskDetailSubTask
    {
        public EmployeeTaskDetailSubTask()
        {
            AssignedUsers = new List<EmployeeTaskDetailSubTaskUser>();            
        }        
        public long? Id { get; set; }
        public string Description { get; set; }
        public List<EmployeeTaskDetailSubTaskUser> AssignedUsers { get; set; }
    }

    public class EmployeeTaskDetailSubTaskUser
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string ProfileURL { get; set; }
    }

    public class EmployeeTaskDetailActivityResponse
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string ProfileURL { get; set; }
        public string Activity { get; set; }
        public DateTime? CreatedOn { get; set; }
    }


}