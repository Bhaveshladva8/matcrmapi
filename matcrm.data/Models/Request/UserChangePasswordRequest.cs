namespace matcrm.data.Models.Request
{
    public class UserChangePasswordRequest
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string TempGuid { get; set; }
        //public int? TenantId { get; set; }
        public string ConfirmPassword { get; set; }
    }
}