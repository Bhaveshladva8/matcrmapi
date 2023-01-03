using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class AddUpdateEmployeeProjectRequest
    {
        public AddUpdateEmployeeProjectRequest()
        {           
            AssignedUsers = new List<EmployeeProjectUserRequestResponse>();            
        }
        public long? Id { get; set; }         
        public string Name { get; set; }
        //public string? Duration { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public IFormFile File { get; set; }
        // public List<CustomFieldDto> CustomFields { get; set; }
        public List<EmployeeProjectUserRequestResponse> AssignedUsers { get; set; }
        public long? ClientId { get; set; }
        public long? ContractId { get; set; }
        public long? MateCategoryId { get; set; }
        
    }
}