namespace matcrm.data.Models.Request
{
    public class UserExternalLoginRequest
    {
        public string Provider { get; set; }
        public string IdToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}