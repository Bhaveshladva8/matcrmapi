using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;

namespace matcrm.data.Models.Response
{
    public class EmployeeTaskListResponse
    {
        // public long Id { get; set; }        
        // public string Description { get; set; }
        // public string Name { get; set; }
        // public long? StatusId { get; set; }
        // public long? ProjectId { get; set; }
        // public string Status { get; set; }
        // public DateTime? CreatedOn { get; set; }
        // public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }
        // public string TotalDuration { get; set; }
        // public DateTime? Enddate { get; set; }
        // public long TimeRecordCount { get; set; }
        
        public long Id { get; set; }
        public string TaskNo { get; set; }
        public string Name { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLogoURL { get; set; }
        public long? StatusId { get; set; }
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime? Date { get; set; }
        public List<EmployeeTaskUserRequestResponse> AssignedUsers { get; set; }

    }
}