using System;

namespace matcrm.data.Models.Response
{
    public class UserGetAllUsersByTenantResponse
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }        
        
    }
}