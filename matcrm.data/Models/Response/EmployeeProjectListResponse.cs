using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class EmployeeProjectListResponse
    {        
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? LogoURL { get; set; }      
        public long? StatusId { get; set; }        
        public string StatusName { get; set; }
        public long? MateCategoryId { get; set; }        
        public string MateCategoryName { get; set; }
        public string TotalDuration { get; set; }
        public DateTime? Enddate { get; set; }
        public long TimeRecordCount { get; set; }
    }
}