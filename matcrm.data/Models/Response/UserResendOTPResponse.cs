namespace matcrm.data.Models.Response
{
    public class UserResendOTPResponse
    {
        public string Email { get; set; }
        public bool IsEmailValid { get; set; }
        public bool IsCapchaVerified { get; set; }
        public string TempGuid { get; set; }

        
    }
}