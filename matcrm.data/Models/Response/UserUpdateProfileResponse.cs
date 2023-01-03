using System;

namespace matcrm.data.Models.Response
{
    public class UserUpdateProfileResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }        
        public int? DialCode { get; set; }        
        public string AccessToken { get; set; }
        public bool IsSubscribed { get; set; }
        public string RoleName { get; set; }
        public string Tenant { get; set; }       
        public string ProfileImage { get; set; }
    }
}