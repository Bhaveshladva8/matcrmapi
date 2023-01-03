namespace matcrm.data.Models.Request
{
    public class UserRegisterRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int? TenantId { get; set; }
    }
}