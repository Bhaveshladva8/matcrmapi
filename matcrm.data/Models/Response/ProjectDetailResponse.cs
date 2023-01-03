using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.ViewModels;

namespace matcrm.data.Models.Response
{
    public class ProjectDetailResponse
    {
        public ProjectDetailResponse()
        {
            AssignedUsers = new List<EmployeeProjectUserRequestResponse>();
            ProjectTimeRecords = new List<MateProjectTimeRecordProjectDetailResponse>();            
            //Sections = new List<EmployeeSectionVM>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }       
        public string? LogoURL { get; set; }        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public long? Priority { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public long? ProjectCategoryId { get; set; }        
        public string ProjectCategoryName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public string UserName { get; set; }
        public string UserProfileURL { get; set; }
        public long? TaskCount { get; set; }
        public long? TicketCount { get; set; }
        public List<EmployeeProjectUserRequestResponse> AssignedUsers { get; set; }       
        public List<MateProjectTimeRecordProjectDetailResponse> ProjectTimeRecords { get; set; }        
        public List<ProjectTaskDetailResponse> Tasks { get; set; }
    }


    public class MateProjectTimeRecordProjectDetailResponse
    {        
        public long MateTimeRecordId { get; set; }        
        public string Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? Duration { get; set; }
        public bool? IsBillable { get; set; }
    }
    public class ProjectTaskDetailResponse
    {
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        //public string StatusColor { get; set; }
        public long TotalCount { get; set; }
    }
}