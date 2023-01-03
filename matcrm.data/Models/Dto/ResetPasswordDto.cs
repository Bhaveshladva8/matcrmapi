namespace matcrm.data.Models.Dto {
    public class ResetPasswordDto {
        public string Email { get; set; }

        public bool IsEmailValid { get; set; }

        public string EmailErrorMessage { get; set; }

        public bool IsCapchaVerified { get; set; }

        public string TempGuid { get; set; }
        public int? TenantId { get; set; }
    }
}