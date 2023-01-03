using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class UserUpdateProfileRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public IFormFile File { get; set; }
        public int? DialCode { get; set; }
        public bool IsSignUp { get; set; }
        public string Password { get; set; }
    }
}