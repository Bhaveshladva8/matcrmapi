using System;

namespace matcrm.data.Models.Dto
{
    public class ExternalUserDto
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id_Token { get; set; }
        public string Email { get; set; }
        public string Token_Type { get; set; }
        public long? IntProviderId { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}