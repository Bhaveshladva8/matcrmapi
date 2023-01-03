using System;

namespace matcrm.data.Models.Response
{
    public class UserExternalLoginResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }        
        public long? OneClappLatestThemeConfigId { get; set; }   
        public string WeClappToken { get; set; }
        public string ProfileImage { get; set; }
        public int? WeClappUserId { get; set; }
        public bool IsSubscribed { get; set; }     
        
    }
}