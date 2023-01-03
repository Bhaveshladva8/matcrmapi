namespace matcrm.data.Models.Response
{
    public class UserRegisterResponse
    {
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEmailValid { get; set; } = true;
        public bool IsEmailVerified { get; set; }
        public bool IsSignUp { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsSuccessSignUp { get; set; }
        public bool IsUserAlreadyExist { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public int? TenantId { get; set; }
        public string UserName { get; set; }
    }
}