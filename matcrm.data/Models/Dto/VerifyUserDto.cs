namespace matcrm.data.Models.Dto
{
    public class VerifyUserDto
    {
         public bool IsVerified { get; set; } = true;

        public bool IsAlreadyVerified { get; set; } = false;

        public bool IsVerificationCodeValid { get; set; } = true;

        public bool IsVerificationCodeUsed { get; set; } = false;

        public bool IsCodeHashVerified { get; set; }

        public string VerificationCodeHash { get; set; }

        public string VerificationCode { get; set; }

        public string Hash { get; set; }

        public string Message { get; set; }

        // public NotificationVM NotificationVM { get; set; }

        public long UserId { get; set; }

        public long? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        
        public string ResetPasswordLink { get; set; }
    }
}