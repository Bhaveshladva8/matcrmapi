using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace matcrm.data.Models.Request
{
    public class EmployeeProjectUserRequestResponse
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public long? EmployeeProjectId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImage { get; set; }
        public string? ProfileImageURL { get; set; }   
    }
}