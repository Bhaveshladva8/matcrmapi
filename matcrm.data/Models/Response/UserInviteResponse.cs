namespace matcrm.data.Models.Response
{
    public class UserInviteResponse
    {
        public int Id { get; set; }
        public bool IsEmailVerified { get; set; } 
        public bool IsSuccessSignUp { get; set; }
        public bool IsEmailValid { get; set; } = true;
        public bool IsUserAlreadyExist { get; set; }
        public bool IsSignUp { get; set; }
        
    }
}